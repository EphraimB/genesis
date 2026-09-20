using System.Collections.ObjectModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Genesis.Core;

namespace Genesis.MasterHuman.Poc2a;

// This is an isolated experimental addition to semantic person data. It is not
// a reinterpretation of face.mouth_width, and never becomes Blender-owned state.
public sealed record Poc2aIdentityExtension(double LipFullness = 1, bool LipFullnessLocked = false)
{
    public const string Path = "face.lip_fullness";
    public void Validate()
    {
        if (!double.IsFinite(LipFullness) || LipFullness < 0.75 || LipFullness > 1.25)
            throw new ArgumentOutOfRangeException(nameof(LipFullness), "Lip fullness must be finite and within [0.75, 1.25].");
    }
}

public sealed record Poc2aSemanticSource(CharacterSpecification Character, Poc2aIdentityExtension Extension)
{
    public Poc2aSemanticSource Snapshot() => new(Character.Clone(), Extension with { });
}

public sealed record Poc2aResolved(
    int SchemaVersion, string Backend, string AssetPath,
    IReadOnlyDictionary<string, double> Identity,
    IReadOnlyDictionary<string, double> Performance,
    object Appearance, string IdentityFingerprint, string Fingerprint);

public static class Poc2aMapper
{
    public const string Backend = "master-human-poc-2a-ai/v1";
    public const string AssetPath = "artifacts/master-human-poc2a/pass2/wedge.glb";
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public static readonly IReadOnlyDictionary<string, string> IdentitySources =
        new ReadOnlyDictionary<string, string>(new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["nose_width"] = SemanticPaths.NoseWidth,
            ["eye_spacing"] = SemanticPaths.EyeSpacing,
            ["lip_fullness"] = Poc2aIdentityExtension.Path,
            ["jaw_width"] = SemanticPaths.JawWidth,
            ["apparent_age"] = SemanticPaths.ApparentAge
        });

    public static readonly IReadOnlyDictionary<string, string> IdentityMorphs =
        new ReadOnlyDictionary<string, string>(new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["nose_width"] = "Nose_Width", ["eye_spacing"] = "Eye_Spacing",
            ["lip_fullness"] = "Lip_Fullness", ["jaw_width"] = "Jaw_Width",
            ["apparent_age"] = "Apparent_Age"
        });

    public static readonly IReadOnlyDictionary<string, string> PerformanceMorphs =
        new ReadOnlyDictionary<string, string>(new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["brow_inner_raise"] = "Brow_Inner_Raise", ["brow_down"] = "Brow_Down",
            ["blink_left"] = "Blink_L", ["blink_right"] = "Blink_R",
            ["squint_left"] = "Squint_L", ["squint_right"] = "Squint_R",
            ["smile"] = "Smile", ["frown"] = "Frown", ["jaw_open"] = "Jaw_Open",
            ["mouth_funnel"] = "Mouth_Funnel", ["mouth_pucker"] = "Mouth_Pucker",
            ["cheek_raise"] = "Cheek_Raise"
        });

    public static Poc2aResolved Resolve(Poc2aSemanticSource source,
        IReadOnlyDictionary<string, double>? performance = null)
    {
        ArgumentNullException.ThrowIfNull(source);
        source.Extension.Validate();
        var identity = new SortedDictionary<string, double>(StringComparer.Ordinal);
        foreach (var (channel, path) in IdentitySources)
        {
            var value = path == Poc2aIdentityExtension.Path ? source.Extension.LipFullness : source.Character.Value(path).NumberValue;
            identity[channel] = channel == "apparent_age" ? value : (value - 1) * 4;
        }
        var actions = new SortedDictionary<string, double>(StringComparer.Ordinal);
        foreach (var name in PerformanceMorphs.Keys) actions.Add(name, 0);
        foreach (var (name, value) in performance ?? new Dictionary<string, double>())
        {
            if (!actions.ContainsKey(name)) throw new ArgumentException($"Unknown performance channel: {name}.");
            if (!double.IsFinite(value) || value < 0 || value > 1)
                throw new ArgumentOutOfRangeException(nameof(performance), $"Performance channel {name} must be finite and within [0, 1].");
            actions[name] = value;
        }
        // Appearance uses existing semantic values but is not an additional POC
        // identity control. The experiment keeps this context fixed in visual tests.
        var appearance = new
        {
            skinTone = source.Character.Value(SemanticPaths.SkinTone).ColorValue,
            melanin = source.Character.Value(SemanticPaths.SkinMelanin).NumberValue,
            undertone = source.Character.Value(SemanticPaths.SkinUndertone).NumberValue,
            roughness = source.Character.Value(SemanticPaths.SkinRoughness).NumberValue,
            freckles = source.Character.Value(SemanticPaths.SkinFreckles).NumberValue,
            eyeColor = source.Character.Value(SemanticPaths.EyeColor).ColorValue,
            hairColor = source.Character.Value(SemanticPaths.HairColor).ColorValue
        };
        var identityText = Backend + "|" + source.Character.PersonId + "|" + string.Join(';',
            identity.Select(pair => pair.Key + "=" + pair.Value.ToString("R", CultureInfo.InvariantCulture)));
        var identityFingerprint = Hash(Encoding.UTF8.GetBytes(identityText));
        var fingerprint = Hash(Encoding.UTF8.GetBytes(identityText + "|" + JsonSerializer.Serialize(appearance, JsonOptions)));
        return new(1, Backend, AssetPath, new ReadOnlyDictionary<string, double>(identity),
            new ReadOnlyDictionary<string, double>(actions), appearance, identityFingerprint, fingerprint);
    }

    public static string Hash(ReadOnlySpan<byte> bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
}

// POC-only transaction wrapper gives the one extension the same atomicity,
// face-category locking and state restoration as existing Core semantic edits.
public sealed class Poc2aEditHistory
{
    private readonly Stack<Poc2aSemanticSource> _undo = new();
    private readonly Stack<Poc2aSemanticSource> _redo = new();
    private Poc2aSemanticSource _current;
    public Poc2aEditHistory(Poc2aSemanticSource source) => _current = source.Snapshot();
    public Poc2aSemanticSource Current => _current.Snapshot();

    public void Apply(CharacterEditPlan? plan = null, double? lipFullness = null, bool? lockLipFullness = null)
    {
        var candidate = _current.Snapshot();
        if (plan is not null)
        {
            var result = CharacterEditService.Apply(candidate.Character, plan);
            if (!result.Validation.IsValid)
                throw new InvalidOperationException(string.Join("; ", result.Validation.Errors));
            candidate = candidate with { Character = result.Specification };
        }
        if (lockLipFullness.HasValue)
            candidate = candidate with { Extension = candidate.Extension with { LipFullnessLocked = lockLipFullness.Value } };
        if (lipFullness.HasValue)
        {
            if (candidate.Extension.LipFullnessLocked || candidate.Character.LockedCategories.Contains(CharacterIdentityCategory.FaceIdentity))
                throw new CharacterLockException("Locked characteristic 'face.lip_fullness' was not changed.");
            candidate = candidate with { Extension = candidate.Extension with { LipFullness = lipFullness.Value } };
            candidate.Extension.Validate();
        }
        _undo.Push(_current.Snapshot());
        _redo.Clear();
        _current = candidate;
    }

    public Poc2aSemanticSource Undo()
    {
        if (_undo.TryPop(out var previous)) { _redo.Push(_current.Snapshot()); _current = previous; }
        return Current;
    }

    public Poc2aSemanticSource Redo()
    {
        if (_redo.TryPop(out var next)) { _undo.Push(_current.Snapshot()); _current = next; }
        return Current;
    }
}
