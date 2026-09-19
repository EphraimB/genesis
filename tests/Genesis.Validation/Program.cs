using System.IO.Compression;
using System.Text;
using Genesis.Core;
using Genesis.Core.Package;

var suite = new ValidationSuite();
return suite.Run();

internal sealed class ValidationSuite
{
    private readonly List<(string Name, Action Test)> _tests;
    private readonly string _directory = Path.Combine(Path.GetTempPath(), "genesis-validation", Guid.NewGuid().ToString("N"));
    private readonly GenesisPackageSerializer _serializer = new();

    public ValidationSuite()
    {
        _tests =
        [
            ("New person receives stable identity", StableIdentity),
            ("Save/load preserves supported person data", CompleteRoundTrip),
            ("Stable ID survives save/load", IdRoundTrip),
            ("Display name changes independently of stable ID", RenamePreservesId),
            ("CharacterSpecification survives round trip", CharacterRoundTrip),
            ("PersonalityProfile survives round trip", PersonalityRoundTrip),
            ("VoiceProfile survives round trip", VoiceRoundTrip),
            ("Locks survive round trip", LocksRoundTrip),
            ("Semantic edit changes only intended property", TargetedSemanticEdit),
            ("Invalid multi-operation edit is atomic", AtomicEdit),
            ("Undo restores exact previous specification", UndoIsExact),
            ("Redo restores exact edited specification", RedoIsExact),
            ("Malformed package fails safely", MalformedPackage),
            ("Future package version fails clearly", FuturePackageVersion),
            ("Future person schema fails clearly", FuturePersonSchema),
            ("Unknown optional JSON fields are tolerated", UnknownFieldsTolerated),
            ("Genesis.Core has no Godot dependency", CoreHasNoGodotReference)
        ];
    }

    public int Run()
    {
        Directory.CreateDirectory(_directory);
        var failed = 0;
        try
        {
            foreach (var (name, test) in _tests)
            {
                try { test(); Console.WriteLine($"PASS  {name}"); }
                catch (Exception exception) { failed++; Console.WriteLine($"FAIL  {name}\n      {exception.Message}"); }
            }
        }
        finally { Directory.Delete(_directory, recursive: true); }
        Console.WriteLine($"\nGenesis validation: {_tests.Count - failed}/{_tests.Count} passed.");
        return failed == 0 ? 0 : 1;
    }

    private GenesisPerson RichPerson()
    {
        var original = GenesisPerson.Create("Morgan Vale", new PhysicalFacts(1.83, 79.5, 34, DominantHand.Ambidextrous));
        var edits = new CharacterEditPlan(
        [
            CharacterEditOperation.Set(SemanticPaths.BodyFrame, SemanticValue.Text("Athletic")),
            CharacterEditOperation.Set(SemanticPaths.ShoulderWidth, SemanticValue.Number(1.14)),
            CharacterEditOperation.Set(SemanticPaths.JawWidth, SemanticValue.Number(0.93)),
            CharacterEditOperation.Set(SemanticPaths.SkinTone, SemanticValue.Color(new(0.42, 0.24, 0.14))),
            CharacterEditOperation.Set(SemanticPaths.EyeColor, SemanticValue.Color(new(0.1, 0.36, 0.52))),
            CharacterEditOperation.Set(SemanticPaths.HairStyle, SemanticValue.Text("Curly")),
            CharacterEditOperation.Set(SemanticPaths.HairLength, SemanticValue.Number(0.72)),
            CharacterEditOperation.Lock(CharacterIdentityCategory.FaceIdentity),
            CharacterEditOperation.Lock(SemanticPaths.Height)
        ], "Define Morgan's deterministic appearance");
        var result = CharacterEditService.Apply(original.Character, edits);
        Require(result.Validation.IsValid, string.Join("; ", result.Validation.Errors));
        result.Specification.SetStage(GenesisStage.HairDetails);
        var personality = new PersonalityProfile(0.72, 0.38, 0.84, 0.91, 0.61, 0.57, 0.76, 0.68,
            ["asks thoughtful follow-up questions", "prefers concise explanations"]);
        var voice = new VoiceProfile(Guid.Parse("18923a2d-7f56-4a33-807f-129953fc5842"), "Morgan warm alto",
            VoiceDesignKind.DesignedSynthetic, "future-local-provider", "voices/morgan/model.bin", "speaker-04",
            0.94, -1.5, 0.86, "warm and measured", "Provider-neutral design target");
        return new GenesisPerson(original.Identity, result.Specification, personality, voice,
            new Dictionary<string, string> { ["occupation"] = "architect", ["pronouns"] = "they/them" });
    }

    private GenesisPerson RoundTrip(GenesisPerson person, string name = "person.genesis")
    {
        var path = Path.Combine(_directory, name);
        _serializer.Save(path, person);
        return _serializer.Load(path);
    }

    private void StableIdentity() => Require(GenesisPerson.Create("A").Identity.PersonId.Value != Guid.Empty, "ID was empty.");

    private void CompleteRoundTrip()
    {
        var source = RichPerson();
        var loaded = RoundTrip(source);
        Require(PersonSignature(source) == PersonSignature(loaded), "Round-trip signature changed.");
    }

    private void IdRoundTrip()
    {
        var source = RichPerson();
        Require(source.Identity.PersonId == RoundTrip(source).Identity.PersonId, "Stable ID changed.");
    }

    private void RenamePreservesId()
    {
        var source = RichPerson();
        var renamed = source.WithDisplayName("Morgan North");
        Require(renamed.Identity.PersonId == source.Identity.PersonId, "Rename changed stable ID.");
        Require(renamed.Identity.DisplayName == "Morgan North", "Rename did not apply.");
        Require(renamed.Identity.RevisionNumber == source.Identity.RevisionNumber + 1, "Rename did not create a traceable revision.");
    }

    private void CharacterRoundTrip()
    {
        var source = RichPerson();
        Require(CharacterSignature(source.Character) == CharacterSignature(RoundTrip(source).Character), "Character specification changed.");
    }

    private void PersonalityRoundTrip()
    {
        var source = RichPerson();
        Require(PersonalitySignature(source.Personality) == PersonalitySignature(RoundTrip(source).Personality), "Personality changed.");
    }

    private void VoiceRoundTrip()
    {
        var source = RichPerson();
        Require(source.Voice == RoundTrip(source).Voice, "Voice changed.");
    }

    private void LocksRoundTrip()
    {
        var loaded = RoundTrip(RichPerson());
        Require(loaded.Character.LockedCategories.SetEquals([CharacterIdentityCategory.FaceIdentity]), "Category locks changed.");
        Require(loaded.Character.LockedProperties.SetEquals([SemanticPaths.Height]), "Property locks changed.");
    }

    private void TargetedSemanticEdit()
    {
        var source = GenesisPerson.Create("Target").Character;
        var before = source.Properties.ToDictionary(x => x.Key, x => x.Value);
        var result = CharacterEditService.Apply(source, new CharacterEditPlan(
            [CharacterEditOperation.Set(SemanticPaths.HairLength, SemanticValue.Number(0.8))]));
        Require(result.Validation.IsValid, "Edit was rejected.");
        foreach (var pair in before.Where(x => x.Key != SemanticPaths.HairLength))
            Require(result.Specification.Properties[pair.Key] == pair.Value, $"Unrelated property '{pair.Key}' changed.");
        Require(source.Value(SemanticPaths.HairLength).NumberValue == 0.25, "Source was mutated.");
    }

    private void AtomicEdit()
    {
        var source = GenesisPerson.Create("Atomic").Character;
        var signature = CharacterSignature(source);
        var result = CharacterEditService.Apply(source, new CharacterEditPlan(
        [
            CharacterEditOperation.Set(SemanticPaths.HairLength, SemanticValue.Number(0.8)),
            CharacterEditOperation.Set(SemanticPaths.ShoulderWidth, SemanticValue.Number(99))
        ]));
        Require(!result.Validation.IsValid, "Invalid edit was accepted.");
        Require(CharacterSignature(result.Specification) == signature, "A rejected plan partially changed the person.");
    }

    private void UndoIsExact()
    {
        var source = GenesisPerson.Create("Undo").Character;
        var signature = CharacterSignature(source);
        var history = new CharacterEditHistory(source);
        history.Apply(new CharacterEditPlan([CharacterEditOperation.Set(SemanticPaths.HairStyle, SemanticValue.Text("Long"))]));
        Require(CharacterSignature(history.Undo()) == signature, "Undo did not restore the exact specification.");
    }

    private void RedoIsExact()
    {
        var history = new CharacterEditHistory(GenesisPerson.Create("Redo").Character);
        var edited = history.Apply(new CharacterEditPlan([CharacterEditOperation.Set(SemanticPaths.HairStyle, SemanticValue.Text("Long"))])).Specification;
        history.Undo();
        Require(CharacterSignature(history.Redo()) == CharacterSignature(edited), "Redo did not restore the edited specification.");
    }

    private void MalformedPackage()
    {
        var path = Path.Combine(_directory, "malformed.genesis");
        File.WriteAllText(path, "not a zip package");
        var error = Throws<GenesisPackageException>(() => _serializer.Load(path));
        Require(error.Message.Contains("Could not load", StringComparison.Ordinal), "Malformed-package error was not useful.");
    }

    private void FuturePackageVersion()
    {
        var sourcePath = Path.Combine(_directory, "future-package.genesis");
        WriteRawPackage(sourcePath, """{"packageVersion":999,"personSchemaVersion":1,"personId":"00000000-0000-0000-0000-000000000001","revisionId":"00000000-0000-0000-0000-000000000002","content":[]}""", "{}");
        var error = Throws<GenesisPackageException>(() => _serializer.Load(sourcePath));
        Require(error.Message.Contains("newer than supported", StringComparison.Ordinal), "Future package error was not useful.");
    }

    private void FuturePersonSchema()
    {
        var path = Path.Combine(_directory, "future-person.genesis");
        WriteRawPackage(path, """{"packageVersion":1,"personSchemaVersion":999,"personId":"00000000-0000-0000-0000-000000000001","revisionId":"00000000-0000-0000-0000-000000000002","content":[]}""", "{}");
        var error = Throws<GenesisPackageException>(() => _serializer.Load(path));
        Require(error.Message.Contains("person schema", StringComparison.OrdinalIgnoreCase), "Future schema error was not useful.");
    }

    private void UnknownFieldsTolerated()
    {
        var person = RichPerson();
        var source = Path.Combine(_directory, "known.genesis");
        _serializer.Save(source, person);
        string manifest;
        string data;
        using (var archive = ZipFile.OpenRead(source))
        {
            manifest = InsertRootField(Read(archive, "manifest.json"), "\"futureManifestHint\":true,");
            data = InsertRootField(Read(archive, "person.json"), "\"futureOptionalBranch\":{\"value\":42},");
        }
        var target = Path.Combine(_directory, "unknown-fields.genesis");
        WriteRawPackage(target, manifest, data);
        Require(PersonSignature(person) == PersonSignature(_serializer.Load(target)), "Unknown optional fields changed known data.");
    }

    private static void CoreHasNoGodotReference()
    {
        var references = typeof(GenesisPerson).Assembly.GetReferencedAssemblies();
        Require(references.All(x => !x.Name!.StartsWith("Godot", StringComparison.OrdinalIgnoreCase)), "Genesis.Core references Godot.");
    }

    private static string PersonSignature(GenesisPerson person) => string.Join("|",
        person.Identity.PersonId, person.Identity.DisplayName, person.Identity.RevisionNumber, person.Identity.RevisionId,
        person.Identity.ParentRevisionId, person.Identity.CreatedUtc.ToString("O"), person.Identity.ModifiedUtc.ToString("O"),
        CharacterSignature(person.Character), PersonalitySignature(person.Personality), person.Voice,
        string.Join(';', person.Metadata.OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}")));

    private static string CharacterSignature(CharacterSpecification value) => string.Join("|",
        value.PersonId, value.SchemaVersion, value.Stage, value.Facts.HeightMeters.ToString("R"), value.Facts.WeightKilograms.ToString("R"),
        value.Facts.AgeYears, value.Facts.DominantHand,
        string.Join(';', value.Properties.OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}")),
        string.Join(',', value.LockedCategories.Order()), string.Join(',', value.LockedProperties.Order(StringComparer.Ordinal)));

    private static string PersonalitySignature(PersonalityProfile value) => string.Join("|", value.Confidence.ToString("R"),
        value.Talkativeness.ToString("R"), value.Curiosity.ToString("R"), value.Empathy.ToString("R"), value.Playfulness.ToString("R"),
        value.EmotionalIntensity.ToString("R"), value.Calmness.ToString("R"), value.Leadership.ToString("R"),
        string.Join(';', value.ConversationalTendencies));

    private static void WriteRawPackage(string path, string manifest, string person)
    {
        using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
        Write(archive, "manifest.json", manifest);
        Write(archive, "person.json", person);
    }

    private static void Write(ZipArchive archive, string path, string text)
    {
        var entry = archive.CreateEntry(path);
        using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
        writer.Write(text);
    }

    private static string Read(ZipArchive archive, string path)
    {
        using var reader = new StreamReader(archive.GetEntry(path)!.Open());
        return reader.ReadToEnd();
    }

    private static string InsertRootField(string json, string field)
    {
        var index = json.IndexOf('{');
        return index < 0 ? json : json.Insert(index + 1, field);
    }

    private static T Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T exception) { return exception; }
        throw new InvalidOperationException($"Expected {typeof(T).Name}.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}

internal static class SetExtensions
{
    public static bool SetEquals<T>(this IReadOnlySet<T> value, IEnumerable<T> expected) => value.Count == expected.Count() && expected.All(value.Contains);
}
