using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Genesis.Core.Package;

public sealed class GenesisPackageException(string message, Exception? inner = null) : Exception(message, inner);

public sealed record GenesisPackageAssets(
    byte[]? MasterHumanGlb = null,
    string? MasterHumanMaterialManifest = null,
    IReadOnlyDictionary<string, byte[]>? SupplementalFiles = null);
public sealed record GenesisPackageReadResult(GenesisPerson Person, GenesisPackageAssets Assets);

public sealed class GenesisPackageSerializer
{
    public const int CurrentPackageVersion = 1;
    public const int CurrentPersonSchemaVersion = 1;
    public const string Extension = ".genesis";
    private const long MaximumJsonBytes = 4 * 1024 * 1024;
    private static readonly DateTimeOffset DeterministicTimestamp = new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
    private readonly JsonSerializerOptions _json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip
    };

    public void Save(string path, GenesisPerson person) => Save(path, person, null);

    public void Save(string path, GenesisPerson person, GenesisPackageAssets? assets)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        ArgumentNullException.ThrowIfNull(person);
        var fullPath = Path.GetFullPath(path);
        var parent = Path.GetDirectoryName(fullPath) ?? throw new GenesisPackageException("Package path has no parent directory.");
        Directory.CreateDirectory(parent);
        var temp = Path.Combine(parent, $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");
        var supplementalFiles = ValidatedSupplementalFiles(assets).ToArray();
        try
        {
            using (var stream = new FileStream(temp, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, leaveOpen: false, Encoding.UTF8))
            {
                var dto = PersonData.FromDomain(person);
                var content = new List<ContentEntryData> { new() { Path = "person.json", Kind = "person", Required = true } };
                if (assets?.MasterHumanGlb is not null)
                    content.Add(new() { Path = "master-human/model.glb", Kind = "master-human-game-close", Required = false });
                if (assets?.MasterHumanMaterialManifest is not null)
                    content.Add(new() { Path = "master-human/materials.json", Kind = "master-human-materials", Required = false });
                foreach (var supplemental in supplementalFiles)
                    content.Add(new() { Path = supplemental.Key, Kind = "master-human-support", Required = false });
                var manifest = new ManifestData
                {
                    PackageVersion = CurrentPackageVersion,
                    PersonSchemaVersion = CurrentPersonSchemaVersion,
                    PersonId = person.Identity.PersonId.ToString(),
                    RevisionId = person.Identity.RevisionId,
                    Content = content
                };
                WriteJson(archive, "manifest.json", manifest);
                WriteJson(archive, "person.json", dto);
                if (assets?.MasterHumanGlb is not null)
                    WriteBytes(archive, "master-human/model.glb", assets.MasterHumanGlb);
                if (assets?.MasterHumanMaterialManifest is not null)
                    WriteText(archive, "master-human/materials.json", assets.MasterHumanMaterialManifest);
                foreach (var supplemental in supplementalFiles)
                    WriteBytes(archive, supplemental.Key, supplemental.Value);
            }
            File.Move(temp, fullPath, overwrite: true);
        }
        catch
        {
            if (File.Exists(temp)) File.Delete(temp);
            throw;
        }
    }

    public GenesisPerson Load(string path) => LoadDetailed(path).Person;

    public GenesisPackageReadResult LoadDetailed(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false, Encoding.UTF8);
            if (archive.Entries.GroupBy(x => x.FullName, StringComparer.Ordinal).Any(x => x.Count() > 1))
                throw new GenesisPackageException("Package contains duplicate entry names.");
            var manifest = ReadJson<ManifestData>(archive, "manifest.json");
            if (manifest.PackageVersion > CurrentPackageVersion)
                throw new GenesisPackageException($"Genesis package version {manifest.PackageVersion} is newer than supported version {CurrentPackageVersion}.");
            if (manifest.PackageVersion < 1)
                throw new GenesisPackageException($"Genesis package version {manifest.PackageVersion} is invalid.");
            if (manifest.PersonSchemaVersion > CurrentPersonSchemaVersion)
                throw new GenesisPackageException($"Genesis person schema {manifest.PersonSchemaVersion} is newer than supported schema {CurrentPersonSchemaVersion}.");
            var data = ReadJson<PersonData>(archive, "person.json");
            var person = data.ToDomain();
            if (!string.Equals(manifest.PersonId, person.Identity.PersonId.ToString(), StringComparison.OrdinalIgnoreCase))
                throw new GenesisPackageException("Manifest identity does not match person data.");
            if (manifest.RevisionId != person.Identity.RevisionId)
                throw new GenesisPackageException("Manifest revision does not match person data.");
            var supplemental = manifest.Content
                .Where(x => x.Kind == "master-human-support")
                .ToDictionary(x => ValidateSupplementalPath(x.Path), x => ReadOptionalBytes(archive, x.Path, 128 * 1024 * 1024)
                    ?? throw new GenesisPackageException($"Declared optional package entry '{x.Path}' is missing."), StringComparer.Ordinal);
            var assets = new GenesisPackageAssets(
                ReadOptionalBytes(archive, "master-human/model.glb", 128 * 1024 * 1024),
                ReadOptionalText(archive, "master-human/materials.json", MaximumJsonBytes), supplemental);
            return new GenesisPackageReadResult(person, assets);
        }
        catch (GenesisPackageException) { throw; }
        catch (Exception exception) when (exception is IOException or InvalidDataException or JsonException or ArgumentException or NotSupportedException)
        {
            throw new GenesisPackageException($"Could not load Genesis package '{Path.GetFileName(path)}': {exception.Message}", exception);
        }
    }

    private void WriteJson<T>(ZipArchive archive, string path, T value)
    {
        var entry = archive.CreateEntry(path, CompressionLevel.Optimal);
        entry.LastWriteTime = DeterministicTimestamp;
        using var output = entry.Open();
        JsonSerializer.Serialize(output, value, _json);
    }

    private static void WriteBytes(ZipArchive archive, string path, ReadOnlySpan<byte> value)
    {
        var entry = archive.CreateEntry(path, CompressionLevel.Optimal);
        entry.LastWriteTime = DeterministicTimestamp;
        using var output = entry.Open();
        output.Write(value);
    }

    private static void WriteText(ZipArchive archive, string path, string value)
    {
        var entry = archive.CreateEntry(path, CompressionLevel.Optimal);
        entry.LastWriteTime = DeterministicTimestamp;
        using var writer = new StreamWriter(entry.Open(), new UTF8Encoding(false));
        writer.Write(value);
    }

    private static byte[]? ReadOptionalBytes(ZipArchive archive, string path, long maximumBytes)
    {
        var entry = archive.GetEntry(path);
        if (entry is null) return null;
        if (entry.Length > maximumBytes) throw new GenesisPackageException($"Optional package entry '{path}' exceeds the safe size limit.");
        using var input = entry.Open();
        using var memory = new MemoryStream((int)entry.Length);
        input.CopyTo(memory);
        return memory.ToArray();
    }

    private static string? ReadOptionalText(ZipArchive archive, string path, long maximumBytes)
    {
        var bytes = ReadOptionalBytes(archive, path, maximumBytes);
        return bytes is null ? null : Encoding.UTF8.GetString(bytes);
    }

    private static IEnumerable<KeyValuePair<string, byte[]>> ValidatedSupplementalFiles(GenesisPackageAssets? assets)
    {
        if (assets?.SupplementalFiles is null) yield break;
        foreach (var pair in assets.SupplementalFiles.OrderBy(x => x.Key, StringComparer.Ordinal))
        {
            ValidateSupplementalPath(pair.Key);
            if (pair.Value is null) throw new GenesisPackageException($"Supplemental Master Human entry '{pair.Key}' is null.");
            yield return pair;
        }
    }

    private static string ValidateSupplementalPath(string path)
    {
        if (!path.StartsWith("master-human/", StringComparison.Ordinal) ||
            path.Contains("..", StringComparison.Ordinal) || path.Contains('\\') || path.EndsWith('/'))
            throw new GenesisPackageException($"Unsafe supplemental Master Human path '{path}'.");
        if (path is "master-human/model.glb" or "master-human/materials.json")
            throw new GenesisPackageException($"Supplemental path '{path}' conflicts with a reserved package entry.");
        return path;
    }

    private T ReadJson<T>(ZipArchive archive, string path)
    {
        var entry = archive.GetEntry(path) ?? throw new GenesisPackageException($"Required package entry '{path}' is missing.");
        if (entry.Length > MaximumJsonBytes) throw new GenesisPackageException($"Package entry '{path}' exceeds the safe size limit.");
        using var input = entry.Open();
        return JsonSerializer.Deserialize<T>(input, _json) ?? throw new GenesisPackageException($"Package entry '{path}' is empty.");
    }

    private sealed class ManifestData
    {
        public int PackageVersion { get; set; }
        public int PersonSchemaVersion { get; set; }
        public string PersonId { get; set; } = string.Empty;
        public Guid RevisionId { get; set; }
        public List<ContentEntryData> Content { get; set; } = [];
    }

    private sealed class ContentEntryData
    {
        public string Path { get; set; } = string.Empty;
        public string Kind { get; set; } = string.Empty;
        public bool Required { get; set; }
    }

    private sealed class PersonData
    {
        public int SchemaVersion { get; set; }
        public IdentityData Identity { get; set; } = new();
        public FactsData PhysicalFacts { get; set; } = new();
        public CharacterData CharacterSpecification { get; set; } = new();
        public PersonalityData Personality { get; set; } = new();
        public VoiceData Voice { get; set; } = new();
        public SortedDictionary<string, string> Metadata { get; set; } = new(StringComparer.Ordinal);

        public static PersonData FromDomain(GenesisPerson person) => new()
        {
            SchemaVersion = CurrentPersonSchemaVersion,
            Identity = IdentityData.FromDomain(person.Identity),
            PhysicalFacts = FactsData.FromDomain(person.Character.Facts),
            CharacterSpecification = CharacterData.FromDomain(person.Character),
            Personality = PersonalityData.FromDomain(person.Personality),
            Voice = VoiceData.FromDomain(person.Voice),
            Metadata = new SortedDictionary<string, string>(person.Metadata.ToDictionary(x => x.Key, x => x.Value), StringComparer.Ordinal)
        };

        public GenesisPerson ToDomain()
        {
            if (SchemaVersion > CurrentPersonSchemaVersion)
                throw new GenesisPackageException($"Genesis person schema {SchemaVersion} is newer than supported schema {CurrentPersonSchemaVersion}.");
            if (SchemaVersion < 1) throw new GenesisPackageException("Genesis person schema is invalid.");
            var identity = Identity.ToDomain();
            return new GenesisPerson(identity, CharacterSpecification.ToDomain(identity.PersonId, PhysicalFacts.ToDomain()),
                Personality.ToDomain(), Voice.ToDomain(), Metadata);
        }
    }

    private sealed class IdentityData
    {
        public Guid PersonId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int RevisionNumber { get; set; }
        public Guid RevisionId { get; set; }
        public Guid? ParentRevisionId { get; set; }
        public DateTimeOffset CreatedUtc { get; set; }
        public DateTimeOffset ModifiedUtc { get; set; }
        public static IdentityData FromDomain(PersonIdentity value) => new() { PersonId = value.PersonId.Value, DisplayName = value.DisplayName, RevisionNumber = value.RevisionNumber, RevisionId = value.RevisionId, ParentRevisionId = value.ParentRevisionId, CreatedUtc = value.CreatedUtc, ModifiedUtc = value.ModifiedUtc };
        public PersonIdentity ToDomain() => new(new PersonId(PersonId), DisplayName, RevisionNumber, RevisionId, ParentRevisionId, CreatedUtc, ModifiedUtc);
    }

    private sealed class FactsData
    {
        public double HeightMeters { get; set; }
        public double WeightKilograms { get; set; }
        public int AgeYears { get; set; }
        public DominantHand DominantHand { get; set; }
        public static FactsData FromDomain(PhysicalFacts value) => new() { HeightMeters = value.HeightMeters, WeightKilograms = value.WeightKilograms, AgeYears = value.AgeYears, DominantHand = value.DominantHand };
        public PhysicalFacts ToDomain() => new(HeightMeters, WeightKilograms, AgeYears, DominantHand);
    }

    private sealed class CharacterData
    {
        public int SchemaVersion { get; set; }
        public GenesisStage Stage { get; set; }
        public SortedDictionary<string, SemanticValue> Properties { get; set; } = new(StringComparer.Ordinal);
        public List<CharacterIdentityCategory> LockedCategories { get; set; } = [];
        public List<string> LockedProperties { get; set; } = [];
        public static CharacterData FromDomain(CharacterSpecification value) => new() { SchemaVersion = value.SchemaVersion, Stage = value.Stage, Properties = new SortedDictionary<string, SemanticValue>(value.Properties.ToDictionary(x => x.Key, x => x.Value), StringComparer.Ordinal), LockedCategories = value.LockedCategories.Order().ToList(), LockedProperties = value.LockedProperties.Order(StringComparer.Ordinal).ToList() };
        public CharacterSpecification ToDomain(PersonId id, PhysicalFacts facts) => new(id, facts, Properties, LockedCategories, LockedProperties, Stage, SchemaVersion);
    }

    private sealed class PersonalityData
    {
        public double Confidence { get; set; }
        public double Talkativeness { get; set; }
        public double Curiosity { get; set; }
        public double Empathy { get; set; }
        public double Playfulness { get; set; }
        public double EmotionalIntensity { get; set; }
        public double Calmness { get; set; }
        public double Leadership { get; set; }
        public List<string> ConversationalTendencies { get; set; } = [];
        public static PersonalityData FromDomain(PersonalityProfile value) => new() { Confidence = value.Confidence, Talkativeness = value.Talkativeness, Curiosity = value.Curiosity, Empathy = value.Empathy, Playfulness = value.Playfulness, EmotionalIntensity = value.EmotionalIntensity, Calmness = value.Calmness, Leadership = value.Leadership, ConversationalTendencies = value.ConversationalTendencies.ToList() };
        public PersonalityProfile ToDomain() => new(Confidence, Talkativeness, Curiosity, Empathy, Playfulness, EmotionalIntensity, Calmness, Leadership, ConversationalTendencies);
    }

    private sealed class VoiceData
    {
        public Guid VoiceId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public VoiceDesignKind DesignKind { get; set; }
        public string? ProviderId { get; set; }
        public string? ModelReference { get; set; }
        public string? SpeakerReference { get; set; }
        public double SpeakingRate { get; set; }
        public double PitchSemitones { get; set; }
        public double Volume { get; set; }
        public string? Style { get; set; }
        public string? Description { get; set; }
        public static VoiceData FromDomain(VoiceProfile value) => new() { VoiceId = value.VoiceId, DisplayName = value.DisplayName, DesignKind = value.DesignKind, ProviderId = value.ProviderId, ModelReference = value.ModelReference, SpeakerReference = value.SpeakerReference, SpeakingRate = value.SpeakingRate, PitchSemitones = value.PitchSemitones, Volume = value.Volume, Style = value.Style, Description = value.Description };
        public VoiceProfile ToDomain() => new(VoiceId, DisplayName, DesignKind, ProviderId, ModelReference, SpeakerReference, SpeakingRate, PitchSemitones, Volume, Style, Description);
    }
}
