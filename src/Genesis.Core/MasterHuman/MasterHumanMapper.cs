using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Genesis.Core.MasterHuman;

public sealed record MasterHumanRenderParameters(
    string AssetPath,
    double HeightScale,
    double BodyComposition,
    double NoseWidth,
    double ApparentAge,
    SemanticColor SkinTone,
    double SkinMelanin,
    double SkinUndertone,
    double SkinRoughness,
    double SkinFreckles,
    SemanticColor EyeColor,
    SemanticColor HairColor,
    string HairStyle,
    string SecondaryHairStyle,
    string MaterialPreset,
    string SemanticFingerprint);

public static class MasterHumanMapper
{
    public const string MappingVersion = "master-human-poc-1/v1";
    public const string TopologyVersion = "genesis-cc0-human-base-1.4.1/poc-1";
    public const string RigVersion = "genesis-humanoid-55/poc-1";
    public const string GameCloseAssetPath = "res://artifacts/master-human-poc/game-close/genesis_master_human_poc1.glb";
    public const double CanonicalHeightMeters = 1.69;

    public static readonly IReadOnlySet<string> ConsumedPaths = new HashSet<string>(StringComparer.Ordinal)
    {
        SemanticPaths.Height,
        SemanticPaths.BodyComposition,
        SemanticPaths.NoseWidth,
        SemanticPaths.ApparentAge,
        SemanticPaths.SkinTone,
        SemanticPaths.SkinMelanin,
        SemanticPaths.SkinUndertone,
        SemanticPaths.SkinRoughness,
        SemanticPaths.SkinFreckles,
        SemanticPaths.EyeColor,
        SemanticPaths.HairColor,
        SemanticPaths.HairStyle,
        SemanticPaths.SecondaryHairStyle,
        SemanticPaths.ClothingPresentation
    };

    public static MasterHumanRenderParameters Resolve(CharacterSpecification specification)
    {
        ArgumentNullException.ThrowIfNull(specification);
        return new(
            GameCloseAssetPath,
            specification.Facts.HeightMeters / CanonicalHeightMeters,
            specification.Value(SemanticPaths.BodyComposition).NumberValue,
            NormalizeCentered(specification.Value(SemanticPaths.NoseWidth).NumberValue, 0.75, 1.25),
            specification.Value(SemanticPaths.ApparentAge).NumberValue,
            specification.Value(SemanticPaths.SkinTone).ColorValue,
            specification.Value(SemanticPaths.SkinMelanin).NumberValue,
            specification.Value(SemanticPaths.SkinUndertone).NumberValue,
            specification.Value(SemanticPaths.SkinRoughness).NumberValue,
            specification.Value(SemanticPaths.SkinFreckles).NumberValue,
            specification.Value(SemanticPaths.EyeColor).ColorValue,
            specification.Value(SemanticPaths.HairColor).ColorValue,
            specification.Value(SemanticPaths.HairStyle).TextValue,
            specification.Value(SemanticPaths.SecondaryHairStyle).TextValue,
            specification.Value(SemanticPaths.ClothingPresentation).TextValue,
            Fingerprint(specification));
    }

    public static string Fingerprint(CharacterSpecification specification)
    {
        var text = new StringBuilder();
        text.Append(MappingVersion).Append('|').Append(specification.PersonId).Append('|')
            .Append(specification.Facts.HeightMeters.ToString("R", CultureInfo.InvariantCulture)).Append('|');
        foreach (var path in ConsumedPaths.Order(StringComparer.Ordinal))
            text.Append(path).Append('=').Append(CanonicalValue(specification.Value(path))).Append(';');
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString()))).ToLowerInvariant();
    }

    private static double NormalizeCentered(double value, double minimum, double maximum) =>
        Math.Clamp((value - ((minimum + maximum) / 2)) / ((maximum - minimum) / 2), -1, 1);

    private static string CanonicalValue(SemanticValue value) => value.Kind switch
    {
        SemanticValueKind.Number => value.NumberValue.ToString("R", CultureInfo.InvariantCulture),
        SemanticValueKind.Integer => value.IntegerValue.ToString(CultureInfo.InvariantCulture),
        SemanticValueKind.Text => value.TextValue,
        SemanticValueKind.Boolean => value.BooleanValue ? "true" : "false",
        SemanticValueKind.Color => string.Join(',', value.ColorValue.R.ToString("R", CultureInfo.InvariantCulture),
            value.ColorValue.G.ToString("R", CultureInfo.InvariantCulture), value.ColorValue.B.ToString("R", CultureInfo.InvariantCulture),
            value.ColorValue.A.ToString("R", CultureInfo.InvariantCulture)),
        _ => throw new ArgumentOutOfRangeException(nameof(value))
    };
}
