using System.Text;
using System.Text.Json;
using Genesis.Core;
using Genesis.Core.MasterHuman;
using Genesis.Core.Package;

var root = FindRepository();
var personId = new PersonId(Guid.Parse("b1dc53c5-60dd-4f82-a7ec-673f52fe6501"));
var identity = new PersonIdentity(personId, "Genesis Master Human POC-1", 1,
    Guid.Parse("f351e1f0-52cb-45bc-932e-c33706100b14"), null,
    DateTimeOffset.Parse("2026-09-19T00:00:00Z"), DateTimeOffset.Parse("2026-09-19T00:00:00Z"));
var specification = CharacterSpecification.CreateDefault(personId, new PhysicalFacts(1.75, 74, 34));
specification.SetStage(GenesisStage.Complete);
var voice = new VoiceProfile(Guid.Parse("3e49d472-804d-4db7-9239-e34fc8052e3f"), "POC neutral",
    VoiceDesignKind.DesignedSynthetic, null, null, null, 1, 0, 1, "neutral", "No voice asset is included in POC-1.");
var resolved = MasterHumanMapper.Resolve(specification);
var person = new GenesisPerson(identity, specification, new PersonalityProfile(), voice,
    new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["masterHuman.mappingVersion"] = MasterHumanMapper.MappingVersion,
        ["masterHuman.topologyVersion"] = MasterHumanMapper.TopologyVersion,
        ["masterHuman.rigVersion"] = MasterHumanMapper.RigVersion,
        ["masterHuman.resolvedIdentityHash"] = resolved.SemanticFingerprint,
    });

var artifact = Path.Combine(root, "artifacts", "master-human-poc");
var glb = File.ReadAllBytes(Path.Combine(artifact, "game-close", "genesis_master_human_poc1.glb"));
var materials = JsonSerializer.Serialize(new
{
    schemaVersion = 1,
    backend = MasterHumanMapper.MappingVersion,
    parameters = new[] { "skin.melanin", "skin.undertone", "skin.roughness", "skin.freckles", "skin.tone", "eyes.color", "hair.color", "clothing.presentation" },
    textures = new[] { "master-human/textures/skin_albedo_poc1.png", "master-human/textures/fabric_weave_poc1.png" }
}, JsonOptions());
var mapping = JsonSerializer.Serialize(new
{
    schemaVersion = 1,
    mappingVersion = MasterHumanMapper.MappingVersion,
    topologyVersion = MasterHumanMapper.TopologyVersion,
    rigVersion = MasterHumanMapper.RigVersion,
    resolvedIdentityHash = resolved.SemanticFingerprint,
    semanticMorphMap = new Dictionary<string, string>
    {
        [SemanticPaths.BodyComposition] = "Body_Composition",
        [SemanticPaths.NoseWidth] = "Nose_Width",
        [SemanticPaths.ApparentAge] = "Apparent_Age",
        [SemanticPaths.Height] = "node-scale-y"
    },
    skeletonRoleMap = new Dictionary<string, string>
    {
        ["root"] = "root", ["hips"] = "pelvis", ["spine"] = "spine_01", ["chest"] = "spine_02",
        ["neck"] = "neck", ["head"] = "head", ["jaw"] = "jaw", ["leftEye"] = "eye.L", ["rightEye"] = "eye.R"
    }
}, JsonOptions());
var supplemental = new SortedDictionary<string, byte[]>(StringComparer.Ordinal)
{
    ["master-human/textures/skin_albedo_poc1.png"] = File.ReadAllBytes(Path.Combine(artifact, "textures", "skin_albedo_poc1.png")),
    ["master-human/textures/fabric_weave_poc1.png"] = File.ReadAllBytes(Path.Combine(artifact, "textures", "fabric_weave_poc1.png")),
    ["master-human/mapping.json"] = Encoding.UTF8.GetBytes(mapping),
    ["master-human/provenance/dependency-allowlist.json"] = File.ReadAllBytes(Path.Combine(root, "poc", "master-human", "provenance", "dependency-allowlist.json")),
    ["master-human/provenance/LICENSE_GATE.md"] = File.ReadAllBytes(Path.Combine(root, "poc", "master-human", "provenance", "LICENSE_GATE.md")),
    ["master-human/validation/blender_build_validation.json"] = File.ReadAllBytes(Path.Combine(artifact, "validation", "blender_build_validation.json")),
    ["master-human/validation/blender_reimport_validation.json"] = File.ReadAllBytes(Path.Combine(artifact, "validation", "blender_reimport_validation.json")),
    ["master-human/validation/godot_runtime_validation.json"] = File.ReadAllBytes(Path.Combine(artifact, "validation", "godot_runtime_validation.json")),
};
var output = Path.Combine(artifact, "package", "genesis_master_human_poc1.genesis");
new GenesisPackageSerializer().Save(output, person, new GenesisPackageAssets(glb, materials, supplemental));
var loaded = new GenesisPackageSerializer().LoadDetailed(output);
if (loaded.Person.Identity.PersonId != personId || loaded.Assets.MasterHumanGlb?.Length != glb.Length || loaded.Assets.SupplementalFiles?.Count != supplemental.Count)
    throw new InvalidDataException("Generated experimental package failed its immediate round-trip validation.");
Console.WriteLine($"GENESIS_MASTER_HUMAN_PACKAGE_OK path={output} bytes={new FileInfo(output).Length} files={supplemental.Count + 4}");

static JsonSerializerOptions JsonOptions() => new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };

static string FindRepository()
{
    var current = new DirectoryInfo(AppContext.BaseDirectory);
    while (current is not null && !File.Exists(Path.Combine(current.FullName, "Genesis.slnx"))) current = current.Parent;
    return current?.FullName ?? throw new DirectoryNotFoundException("Could not locate Genesis repository.");
}
