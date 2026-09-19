using System.Collections.ObjectModel;

namespace Genesis.Core;

public enum GenesisStage
{
    Blueprint,
    BodyFrame,
    AnatomicalForm,
    IdentityAppearance,
    HairDetails,
    Clothing,
    PersonalityVoice,
    Complete
}

public enum CharacterIdentityCategory
{
    PhysicalFacts,
    BodyIdentity,
    FaceIdentity,
    SkinIdentity,
    EyeIdentity,
    HairIdentity,
    ClothingIdentity,
    AccessoryIdentity
}

public enum SemanticValueKind { Number, Integer, Text, Boolean, Color }

public readonly record struct SemanticColor(double R, double G, double B, double A = 1)
{
    public SemanticColor Clamp() => new(
        Math.Clamp(R, 0, 1), Math.Clamp(G, 0, 1), Math.Clamp(B, 0, 1), Math.Clamp(A, 0, 1));
}

public readonly record struct SemanticValue(
    SemanticValueKind Kind,
    double NumberValue = 0,
    int IntegerValue = 0,
    string TextValue = "",
    bool BooleanValue = false,
    SemanticColor ColorValue = default)
{
    public static SemanticValue Number(double value) => new(SemanticValueKind.Number, NumberValue: value);
    public static SemanticValue Integer(int value) => new(SemanticValueKind.Integer, IntegerValue: value);
    public static SemanticValue Text(string value) => new(SemanticValueKind.Text, TextValue: value?.Trim() ?? string.Empty);
    public static SemanticValue Boolean(bool value) => new(SemanticValueKind.Boolean, BooleanValue: value);
    public static SemanticValue Color(SemanticColor value) => new(SemanticValueKind.Color, ColorValue: value.Clamp());
}

public static class SemanticPaths
{
    public const string Height = "facts.height_meters";
    public const string Weight = "facts.weight_kilograms";
    public const string Age = "facts.age_years";
    public const string DominantHand = "facts.dominant_hand";
    public const string BodyFrame = "body.frame";
    public const string BodyComposition = "body.composition";
    public const string ShoulderWidth = "body.shoulder_width";
    public const string TorsoWidth = "body.torso_width";
    public const string HipWidth = "body.hip_width";
    public const string ArmLength = "body.arm_length";
    public const string LegLength = "body.leg_length";
    public const string HeadShape = "face.head_shape";
    public const string JawWidth = "face.jaw_width";
    public const string EyeSpacing = "face.eye_spacing";
    public const string NoseWidth = "face.nose_width";
    public const string MouthWidth = "face.mouth_width";
    public const string ApparentAge = "face.apparent_age";
    public const string SkinTone = "skin.tone";
    public const string SkinMelanin = "skin.melanin";
    public const string SkinUndertone = "skin.undertone";
    public const string SkinRoughness = "skin.roughness";
    public const string SkinFreckles = "skin.freckles";
    public const string EyeColor = "eyes.color";
    public const string HairStyle = "hair.style";
    public const string HairLength = "hair.length";
    public const string HairVolume = "hair.volume";
    public const string HairCurl = "hair.curl_amount";
    public const string HairColor = "hair.color";
    public const string SecondaryHairStyle = "hair.secondary_style";
    public const string ClothingPresentation = "clothing.presentation";
    public const string Accessories = "accessories.description";
}

public sealed record SemanticPropertyDefinition(
    string Path,
    CharacterIdentityCategory Category,
    SemanticValueKind Kind,
    double? Minimum = null,
    double? Maximum = null,
    IReadOnlySet<string>? AllowedText = null);

public static class SemanticPropertyRegistry
{
    private static readonly IReadOnlyDictionary<string, SemanticPropertyDefinition> Definitions = Build();
    public static IReadOnlyCollection<SemanticPropertyDefinition> All => Definitions.Values.ToArray();

    public static SemanticPropertyDefinition Definition(string path) =>
        Definitions.TryGetValue(path ?? string.Empty, out var value)
            ? value
            : throw new KeyNotFoundException($"Unknown semantic person property '{path}'.");

    public static void Validate(string path, SemanticValue value)
    {
        var definition = Definition(path);
        if (value.Kind != definition.Kind)
            throw new ArgumentException($"Property '{path}' expects {definition.Kind}, not {value.Kind}.");
        if (value.Kind == SemanticValueKind.Number &&
            (!double.IsFinite(value.NumberValue) ||
             (definition.Minimum.HasValue && value.NumberValue < definition.Minimum.Value) ||
             (definition.Maximum.HasValue && value.NumberValue > definition.Maximum.Value)))
            throw new ArgumentOutOfRangeException(nameof(value), $"Property '{path}' is outside its supported range.");
        if (value.Kind == SemanticValueKind.Integer &&
            ((definition.Minimum.HasValue && value.IntegerValue < definition.Minimum.Value) ||
             (definition.Maximum.HasValue && value.IntegerValue > definition.Maximum.Value)))
            throw new ArgumentOutOfRangeException(nameof(value), $"Property '{path}' is outside its supported range.");
        if (value.Kind == SemanticValueKind.Text)
        {
            if (string.IsNullOrWhiteSpace(value.TextValue))
                throw new ArgumentException($"Property '{path}' cannot be empty.");
            if (definition.AllowedText is not null && !definition.AllowedText.Contains(value.TextValue))
                throw new ArgumentException($"Property '{path}' does not accept '{value.TextValue}'.");
        }
    }

    private static IReadOnlyDictionary<string, SemanticPropertyDefinition> Build()
    {
        var values = new Dictionary<string, SemanticPropertyDefinition>(StringComparer.Ordinal);
        Add(SemanticPaths.Height, CharacterIdentityCategory.PhysicalFacts, SemanticValueKind.Number, PhysicalFacts.MinimumHeightMeters, PhysicalFacts.MaximumHeightMeters);
        Add(SemanticPaths.Weight, CharacterIdentityCategory.PhysicalFacts, SemanticValueKind.Number, PhysicalFacts.MinimumWeightKilograms, PhysicalFacts.MaximumWeightKilograms);
        Add(SemanticPaths.Age, CharacterIdentityCategory.PhysicalFacts, SemanticValueKind.Integer, PhysicalFacts.MinimumAgeYears, PhysicalFacts.MaximumAgeYears);
        AddText(SemanticPaths.DominantHand, CharacterIdentityCategory.PhysicalFacts, Enum.GetNames<DominantHand>());
        AddText(SemanticPaths.BodyFrame, CharacterIdentityCategory.BodyIdentity, ["Lean", "Average", "Athletic", "Broad", "Stocky"]);
        Add(SemanticPaths.BodyComposition, CharacterIdentityCategory.BodyIdentity, SemanticValueKind.Number, 0, 1);
        foreach (var path in new[] { SemanticPaths.ShoulderWidth, SemanticPaths.TorsoWidth, SemanticPaths.HipWidth })
            Add(path, CharacterIdentityCategory.BodyIdentity, SemanticValueKind.Number, 0.7, 1.3);
        foreach (var path in new[] { SemanticPaths.ArmLength, SemanticPaths.LegLength, SemanticPaths.JawWidth, SemanticPaths.EyeSpacing, SemanticPaths.NoseWidth, SemanticPaths.MouthWidth })
            Add(path, path.StartsWith("body.", StringComparison.Ordinal) ? CharacterIdentityCategory.BodyIdentity : CharacterIdentityCategory.FaceIdentity, SemanticValueKind.Number, 0.75, 1.25);
        AddText(SemanticPaths.HeadShape, CharacterIdentityCategory.FaceIdentity, ["Oval", "Round", "Square", "Long", "Heart"]);
        Add(SemanticPaths.ApparentAge, CharacterIdentityCategory.FaceIdentity, SemanticValueKind.Number, 0, 1);
        Add(SemanticPaths.SkinTone, CharacterIdentityCategory.SkinIdentity, SemanticValueKind.Color);
        foreach (var path in new[] { SemanticPaths.SkinMelanin, SemanticPaths.SkinUndertone, SemanticPaths.SkinRoughness, SemanticPaths.SkinFreckles })
            Add(path, CharacterIdentityCategory.SkinIdentity, SemanticValueKind.Number, 0, 1);
        Add(SemanticPaths.EyeColor, CharacterIdentityCategory.EyeIdentity, SemanticValueKind.Color);
        AddText(SemanticPaths.HairStyle, CharacterIdentityCategory.HairIdentity, ["None", "Buzz", "Short", "Medium", "Long", "Curly", "Ponytail", "Bun"]);
        Add(SemanticPaths.HairLength, CharacterIdentityCategory.HairIdentity, SemanticValueKind.Number, 0, 1);
        Add(SemanticPaths.HairVolume, CharacterIdentityCategory.HairIdentity, SemanticValueKind.Number, 0.5, 1.5);
        Add(SemanticPaths.HairCurl, CharacterIdentityCategory.HairIdentity, SemanticValueKind.Number, 0, 1);
        Add(SemanticPaths.HairColor, CharacterIdentityCategory.HairIdentity, SemanticValueKind.Color);
        AddText(SemanticPaths.SecondaryHairStyle, CharacterIdentityCategory.HairIdentity, ["None", "Short", "Curly", "Ponytail", "Bun"]);
        AddText(SemanticPaths.ClothingPresentation, CharacterIdentityCategory.ClothingIdentity, ["Neutral", "Casual", "Formal", "Athletic", "Workwear"]);
        Add(SemanticPaths.Accessories, CharacterIdentityCategory.AccessoryIdentity, SemanticValueKind.Text);
        return new ReadOnlyDictionary<string, SemanticPropertyDefinition>(values);

        void Add(string path, CharacterIdentityCategory category, SemanticValueKind kind, double? min = null, double? max = null) =>
            values.Add(path, new(path, category, kind, min, max));
        void AddText(string path, CharacterIdentityCategory category, IEnumerable<string> allowed) =>
            values.Add(path, new(path, category, SemanticValueKind.Text, AllowedText: allowed.ToHashSet(StringComparer.Ordinal)));
    }
}

public sealed class CharacterSpecification
{
    public const int CurrentSchemaVersion = 1;
    private readonly Dictionary<string, SemanticValue> _properties;
    private readonly HashSet<CharacterIdentityCategory> _lockedCategories;
    private readonly HashSet<string> _lockedProperties;

    public CharacterSpecification(PersonId personId, PhysicalFacts facts,
        IEnumerable<KeyValuePair<string, SemanticValue>> properties,
        IEnumerable<CharacterIdentityCategory>? lockedCategories = null,
        IEnumerable<string>? lockedProperties = null,
        GenesisStage stage = GenesisStage.Blueprint,
        int schemaVersion = CurrentSchemaVersion)
    {
        if (schemaVersion != CurrentSchemaVersion)
            throw new NotSupportedException($"Character specification schema {schemaVersion} is not supported.");
        if (!Enum.IsDefined(stage)) throw new ArgumentOutOfRangeException(nameof(stage));
        PersonId = personId;
        Facts = facts ?? throw new ArgumentNullException(nameof(facts));
        SchemaVersion = schemaVersion;
        Stage = stage;
        _properties = properties?.ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal)
            ?? throw new ArgumentNullException(nameof(properties));
        BackfillSchemaOneDefaults(_properties);
        _lockedCategories = lockedCategories?.ToHashSet() ?? [];
        _lockedProperties = lockedProperties?.ToHashSet(StringComparer.Ordinal) ?? [];
        foreach (var pair in _properties) SemanticPropertyRegistry.Validate(pair.Key, pair.Value);
        foreach (var path in _lockedProperties) SemanticPropertyRegistry.Definition(path);
    }

    public PersonId PersonId { get; }
    public int SchemaVersion { get; }
    public PhysicalFacts Facts { get; private set; }
    public GenesisStage Stage { get; private set; }
    public IReadOnlyDictionary<string, SemanticValue> Properties => new ReadOnlyDictionary<string, SemanticValue>(_properties);
    public IReadOnlySet<CharacterIdentityCategory> LockedCategories => _lockedCategories;
    public IReadOnlySet<string> LockedProperties => _lockedProperties;
    public double StageProgress => (double)Stage / (double)GenesisStage.Complete;

    public SemanticValue Value(string path) => path switch
    {
        SemanticPaths.Height => SemanticValue.Number(Facts.HeightMeters),
        SemanticPaths.Weight => SemanticValue.Number(Facts.WeightKilograms),
        SemanticPaths.Age => SemanticValue.Integer(Facts.AgeYears),
        SemanticPaths.DominantHand => SemanticValue.Text(Facts.DominantHand.ToString()),
        _ => _properties.TryGetValue(path, out var value) ? value : throw new KeyNotFoundException($"No value exists for '{path}'.")
    };

    public bool IsLocked(string path)
    {
        var definition = SemanticPropertyRegistry.Definition(path);
        return _lockedProperties.Contains(path) || _lockedCategories.Contains(definition.Category);
    }

    internal void ApplyValidated(string path, SemanticValue value)
    {
        SemanticPropertyRegistry.Validate(path, value);
        Facts = path switch
        {
            SemanticPaths.Height => new PhysicalFacts(value.NumberValue, Facts.WeightKilograms, Facts.AgeYears, Facts.DominantHand),
            SemanticPaths.Weight => new PhysicalFacts(Facts.HeightMeters, value.NumberValue, Facts.AgeYears, Facts.DominantHand),
            SemanticPaths.Age => new PhysicalFacts(Facts.HeightMeters, Facts.WeightKilograms, value.IntegerValue, Facts.DominantHand),
            SemanticPaths.DominantHand => new PhysicalFacts(Facts.HeightMeters, Facts.WeightKilograms, Facts.AgeYears, Enum.Parse<DominantHand>(value.TextValue)),
            _ => Facts
        };
        if (!path.StartsWith("facts.", StringComparison.Ordinal)) _properties[path] = value;
    }

    internal void SetCategoryLock(CharacterIdentityCategory category, bool locked)
    {
        if (locked) _lockedCategories.Add(category); else _lockedCategories.Remove(category);
    }

    internal void SetPropertyLock(string path, bool locked)
    {
        SemanticPropertyRegistry.Definition(path);
        if (locked) _lockedProperties.Add(path); else _lockedProperties.Remove(path);
    }

    public void SetStage(GenesisStage stage)
    {
        if (!Enum.IsDefined(stage)) throw new ArgumentOutOfRangeException(nameof(stage));
        Stage = stage;
    }

    public CharacterSpecification Clone() => new(PersonId, Facts, _properties, _lockedCategories, _lockedProperties, Stage, SchemaVersion);

    public static CharacterSpecification CreateDefault(PersonId personId, PhysicalFacts? facts = null)
    {
        facts ??= new PhysicalFacts(1.75, 72, 30);
        var properties = new Dictionary<string, SemanticValue>(StringComparer.Ordinal)
        {
            [SemanticPaths.BodyFrame] = SemanticValue.Text("Average"),
            [SemanticPaths.BodyComposition] = SemanticValue.Number(0.5),
            [SemanticPaths.ShoulderWidth] = SemanticValue.Number(1),
            [SemanticPaths.TorsoWidth] = SemanticValue.Number(1),
            [SemanticPaths.HipWidth] = SemanticValue.Number(1),
            [SemanticPaths.ArmLength] = SemanticValue.Number(1),
            [SemanticPaths.LegLength] = SemanticValue.Number(1),
            [SemanticPaths.HeadShape] = SemanticValue.Text("Oval"),
            [SemanticPaths.ApparentAge] = SemanticValue.Number(0.35),
            [SemanticPaths.JawWidth] = SemanticValue.Number(1),
            [SemanticPaths.EyeSpacing] = SemanticValue.Number(1),
            [SemanticPaths.NoseWidth] = SemanticValue.Number(1),
            [SemanticPaths.MouthWidth] = SemanticValue.Number(1),
            [SemanticPaths.SkinTone] = SemanticValue.Color(new(0.55, 0.34, 0.22)),
            [SemanticPaths.SkinMelanin] = SemanticValue.Number(0.48),
            [SemanticPaths.SkinUndertone] = SemanticValue.Number(0.56),
            [SemanticPaths.SkinRoughness] = SemanticValue.Number(0.48),
            [SemanticPaths.SkinFreckles] = SemanticValue.Number(0.12),
            [SemanticPaths.EyeColor] = SemanticValue.Color(new(0.20, 0.35, 0.28)),
            [SemanticPaths.HairStyle] = SemanticValue.Text("Short"),
            [SemanticPaths.HairLength] = SemanticValue.Number(0.25),
            [SemanticPaths.HairVolume] = SemanticValue.Number(1),
            [SemanticPaths.HairCurl] = SemanticValue.Number(0.15),
            [SemanticPaths.HairColor] = SemanticValue.Color(new(0.09, 0.05, 0.025)),
            [SemanticPaths.SecondaryHairStyle] = SemanticValue.Text("None"),
            [SemanticPaths.ClothingPresentation] = SemanticValue.Text("Neutral"),
            [SemanticPaths.Accessories] = SemanticValue.Text("None")
        };
        return new CharacterSpecification(personId, facts, properties);
    }

    // These properties were added during Master Human POC-1 without changing the public
    // schema version. Backfill them so Milestone 1 packages remain valid inputs.
    private static void BackfillSchemaOneDefaults(IDictionary<string, SemanticValue> properties)
    {
        properties.TryAdd(SemanticPaths.BodyComposition, SemanticValue.Number(0.5));
        properties.TryAdd(SemanticPaths.ApparentAge, SemanticValue.Number(0.35));
        properties.TryAdd(SemanticPaths.SkinMelanin, SemanticValue.Number(0.48));
        properties.TryAdd(SemanticPaths.SkinUndertone, SemanticValue.Number(0.56));
        properties.TryAdd(SemanticPaths.SkinRoughness, SemanticValue.Number(0.48));
        properties.TryAdd(SemanticPaths.SkinFreckles, SemanticValue.Number(0.12));
        properties.TryAdd(SemanticPaths.SecondaryHairStyle, SemanticValue.Text("None"));
    }
}
