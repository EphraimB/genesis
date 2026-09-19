using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using Genesis.Core;
using Genesis.Core.MasterHuman;
using Genesis.Core.Package;

var repository = FindRepository();
var temp = Path.Combine(Path.GetTempPath(), "genesis-master-human-validation", Guid.NewGuid().ToString("N"));
var tests = new (string Name, Action Test)[]
{
    ("License gate is closed and fully hashed", LicenseGate),
    ("Blender build validation passed", BlenderReport),
    ("Godot runtime validation passed", GodotReport),
    ("Game-close artifact is a matching GLB", GameCloseArtifact),
    ("Fixed render suite is complete", RenderSuite),
    ("Semantic mapping is deterministic", DeterministicMapping),
    ("Targeted mapper edit preserves unrelated semantics", TargetedMapping),
    ("Legacy package remains readable", LegacyPackage),
    ("Optional Master Human package assets round trip", AssetPackage),
    ("Actual experimental package is complete", ActualPackage),
    ("Unsafe supplemental paths are rejected", UnsafeSupplementalPath),
};
var failures = 0;
Directory.CreateDirectory(temp);
try
{
    foreach (var (name, test) in tests)
    {
        try { test(); Console.WriteLine($"PASS  {name}"); }
        catch (Exception error) { failures++; Console.WriteLine($"FAIL  {name}\n      {error.Message}"); }
    }
}
finally { Directory.Delete(temp, true); }
Console.WriteLine($"\nMaster Human validation: {tests.Length - failures}/{tests.Length} passed.");
return failures == 0 ? 0 : 1;

void LicenseGate()
{
    using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(repository, "poc", "master-human", "provenance", "dependency-allowlist.json")));
    var root = document.RootElement;
    Require(root.GetProperty("policy").GetProperty("default").GetString() == "deny", "Gate is not default-deny.");
    Require(root.GetProperty("quarantinePendingHash").GetArrayLength() == 0, "An input remains in quarantine.");
    foreach (var input in root.GetProperty("approved").EnumerateArray())
        Require(input.GetProperty("sha256").GetString()?.Length == 64, $"{input.GetProperty("id").GetString()} is not SHA-256 pinned.");
    Require(root.GetProperty("explicitlyDenied").ToString().Contains("smplx", StringComparison.OrdinalIgnoreCase), "SMPL-X is not explicitly denied.");
}

void BlenderReport()
{
    using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(repository, "artifacts", "master-human-poc", "validation", "blender_build_validation.json")));
    Require(document.RootElement.GetProperty("passed").GetBoolean(), "Blender validation report did not pass.");
    var keys = document.RootElement.GetProperty("shapeKeys").EnumerateArray().Select(x => x.GetString()).ToHashSet(StringComparer.Ordinal);
    foreach (var required in new[] { "Blink_L", "Blink_R", "Jaw_Open", "Lip_Seal", "Smile", "Frown", "Brow_Raise", "Viseme_A", "Viseme_E", "Viseme_I", "Viseme_O", "Viseme_U", "Viseme_FV" })
        Require(keys.Contains(required), $"Missing shape key {required}.");
}

void GodotReport()
{
    using var document = JsonDocument.Parse(File.ReadAllText(Path.Combine(repository, "artifacts", "master-human-poc", "validation", "godot_runtime_validation.json")));
    var root = document.RootElement;
    Require(root.GetProperty("passed").GetBoolean(), "Godot runtime validation report did not pass.");
    Require(root.GetProperty("p95FrameMilliseconds").GetDouble() <= 16.7, "Godot p95 frame time exceeds the gate.");
    Require(root.GetProperty("reportedVideoMemoryMiB").GetDouble() <= 4096, "Godot video memory exceeds the gate.");
    Require(root.GetProperty("resolution").GetString() == "2560x1440", "Godot benchmark resolution is not the required resolution.");
}

void GameCloseArtifact()
{
    var path = Path.Combine(repository, "artifacts", "master-human-poc", "game-close", "genesis_master_human_poc1.glb");
    var bytes = File.ReadAllBytes(path);
    Require(bytes.Length > 100_000, "GLB is unexpectedly small.");
    Require(bytes[0] == (byte)'g' && bytes[1] == (byte)'l' && bytes[2] == (byte)'T' && bytes[3] == (byte)'F', "GLB magic is invalid.");
    using var report = JsonDocument.Parse(File.ReadAllText(Path.Combine(repository, "artifacts", "master-human-poc", "validation", "blender_build_validation.json")));
    var expected = report.RootElement.GetProperty("glb").GetProperty("sha256").GetString();
    Require(Convert.ToHexString(SHA256.HashData(bytes)).Equals(expected, StringComparison.OrdinalIgnoreCase), "GLB hash differs from the build report.");
}

void RenderSuite()
{
    var directory = Path.Combine(repository, "artifacts", "master-human-poc", "renders");
    for (var index = 0; index <= 18; index++)
        Require(Directory.EnumerateFiles(directory, $"{index:00}_*.png").Any(), $"Render {index:00} is missing.");
}

void DeterministicMapping()
{
    var specification = GenesisPerson.Create("Deterministic", new PhysicalFacts(1.82, 80, 41)).Character;
    Require(MasterHumanMapper.Resolve(specification) == MasterHumanMapper.Resolve(specification.Clone()), "Equal specifications produced different mapping output.");
}

void TargetedMapping()
{
    var before = GenesisPerson.Create("Targeted").Character;
    var result = CharacterEditService.Apply(before, new CharacterEditPlan([CharacterEditOperation.Set(SemanticPaths.NoseWidth, SemanticValue.Number(1.2))]));
    Require(result.Validation.IsValid, string.Join("; ", result.Validation.Errors));
    foreach (var pair in before.Properties.Where(x => x.Key != SemanticPaths.NoseWidth))
        Require(result.Specification.Properties[pair.Key] == pair.Value, $"Unrelated property changed: {pair.Key}.");
    var oldMap = MasterHumanMapper.Resolve(before);
    var newMap = MasterHumanMapper.Resolve(result.Specification);
    Require(oldMap with { NoseWidth = newMap.NoseWidth, SemanticFingerprint = newMap.SemanticFingerprint } == newMap, "Nose edit changed an unrelated renderer parameter.");
}

void LegacyPackage()
{
    var source = Path.Combine(temp, "current.genesis");
    var path = Path.Combine(temp, "legacy.genesis");
    var person = GenesisPerson.Create("Legacy");
    var serializer = new GenesisPackageSerializer();
    serializer.Save(source, person);
    string manifest;
    JsonNode data;
    using (var archive = ZipFile.OpenRead(source))
    {
        manifest = ReadText(archive, "manifest.json");
        data = JsonNode.Parse(ReadText(archive, "person.json"))!;
    }
    var properties = data["characterSpecification"]!["properties"]!.AsObject();
    foreach (var property in new[] { SemanticPaths.BodyComposition, SemanticPaths.ApparentAge, SemanticPaths.SkinMelanin,
                 SemanticPaths.SkinUndertone, SemanticPaths.SkinRoughness, SemanticPaths.SkinFreckles, SemanticPaths.SecondaryHairStyle })
        properties.Remove(property);
    using (var archive = ZipFile.Open(path, ZipArchiveMode.Create))
    {
        WriteText(archive, "manifest.json", manifest);
        WriteText(archive, "person.json", data.ToJsonString());
    }
    var loaded = serializer.LoadDetailed(path);
    Require(loaded.Person.Identity.PersonId == person.Identity.PersonId, "Legacy identity did not round trip.");
    Require(loaded.Assets.MasterHumanGlb is null && loaded.Assets.MasterHumanMaterialManifest is null, "Legacy package unexpectedly acquired assets.");
    Require(MasterHumanMapper.Resolve(loaded.Person.Character).SkinRoughness == 0.48, "Legacy semantic defaults were not backfilled.");
}

void AssetPackage()
{
    var path = Path.Combine(temp, "assets.genesis");
    var person = GenesisPerson.Create("Asset Person");
    var bytes = new byte[] { (byte)'g', (byte)'l', (byte)'T', (byte)'F', 2, 0, 0, 0 };
    const string materials = "{\"schemaVersion\":1,\"preset\":\"Neutral\"}";
    var serializer = new GenesisPackageSerializer();
    serializer.Save(path, person, new GenesisPackageAssets(bytes, materials));
    var loaded = serializer.LoadDetailed(path);
    Require(loaded.Assets.MasterHumanGlb!.SequenceEqual(bytes), "GLB package payload changed.");
    Require(loaded.Assets.MasterHumanMaterialManifest == materials, "Material manifest changed.");
    using var archive = ZipFile.OpenRead(path);
    Require(archive.GetEntry("master-human/model.glb") is not null, "Optional GLB entry is missing.");
}

void ActualPackage()
{
    var path = Path.Combine(repository, "artifacts", "master-human-poc", "package", "genesis_master_human_poc1.genesis");
    var loaded = new GenesisPackageSerializer().LoadDetailed(path);
    Require(loaded.Assets.MasterHumanGlb is { Length: > 100_000 }, "Actual package lacks the Game Close GLB.");
    Require(!string.IsNullOrWhiteSpace(loaded.Assets.MasterHumanMaterialManifest), "Actual package lacks its material manifest.");
    Require(loaded.Assets.SupplementalFiles?.Count == 8, "Actual package support-file set is incomplete.");
    Require(loaded.Person.Metadata["masterHuman.mappingVersion"] == MasterHumanMapper.MappingVersion, "Actual package mapping version is stale.");
    Require(MasterHumanMapper.Resolve(loaded.Person.Character).SemanticFingerprint == loaded.Person.Metadata["masterHuman.resolvedIdentityHash"],
        "Actual package semantic fingerprint does not reproduce.");
    using var archive = ZipFile.OpenRead(path);
    foreach (var entry in new[] { "manifest.json", "person.json", "master-human/model.glb", "master-human/materials.json",
                 "master-human/mapping.json", "master-human/provenance/dependency-allowlist.json",
                 "master-human/validation/blender_build_validation.json", "master-human/validation/blender_reimport_validation.json",
                 "master-human/validation/godot_runtime_validation.json" })
        Require(archive.GetEntry(entry) is not null, $"Actual package is missing {entry}.");
}

void UnsafeSupplementalPath()
{
    var path = Path.Combine(temp, "unsafe.genesis");
    var assets = new GenesisPackageAssets(SupplementalFiles: new Dictionary<string, byte[]> { ["master-human/../escape.txt"] = [1] });
    try
    {
        new GenesisPackageSerializer().Save(path, GenesisPerson.Create("Unsafe"), assets);
        throw new InvalidOperationException("Unsafe package path was accepted.");
    }
    catch (GenesisPackageException) { }
    Require(!File.Exists(path), "Rejected package left an output file behind.");
}

static string FindRepository()
{
    var current = new DirectoryInfo(AppContext.BaseDirectory);
    while (current is not null && !File.Exists(Path.Combine(current.FullName, "Genesis.slnx"))) current = current.Parent;
    return current?.FullName ?? throw new DirectoryNotFoundException("Could not locate the Genesis repository.");
}

static void Require(bool condition, string message)
{
    if (!condition) throw new InvalidOperationException(message);
}

static string ReadText(ZipArchive archive, string path)
{
    using var reader = new StreamReader(archive.GetEntry(path)!.Open());
    return reader.ReadToEnd();
}

static void WriteText(ZipArchive archive, string path, string text)
{
    var entry = archive.CreateEntry(path);
    using var writer = new StreamWriter(entry.Open());
    writer.Write(text);
}
