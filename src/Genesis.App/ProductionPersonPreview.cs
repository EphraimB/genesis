using Genesis.Core;
using Genesis.Core.MasterHuman;
using Godot;
using System.Diagnostics;

namespace Genesis.App;

public partial class ProductionPersonPreview : Node3D
{
    private Node3D? _model;
    private MasterHumanRenderParameters? _pending;

    public bool IsLoaded => _model is not null;
    public double LoadMilliseconds { get; private set; }
    public int MeshCount { get; private set; }

    public override void _Ready()
    {
        var timer = Stopwatch.StartNew();
        var scene = ResourceLoader.Load<PackedScene>(MasterHumanMapper.GameCloseAssetPath);
        if (scene is null)
        {
            GD.PushWarning($"Master Human asset was not imported: {MasterHumanMapper.GameCloseAssetPath}");
            return;
        }
        _model = scene.Instantiate<Node3D>();
        _model.Name = "MasterHumanGameClose";
        AddChild(_model);
        MeshCount = Meshes(_model).Count();
        timer.Stop();
        LoadMilliseconds = timer.Elapsed.TotalMilliseconds;
        if (_pending is not null) ApplyResolved(_pending);
    }

    public void Apply(CharacterSpecification specification)
    {
        _pending = MasterHumanMapper.Resolve(specification);
        if (_model is not null) ApplyResolved(_pending);
    }

    private void ApplyResolved(MasterHumanRenderParameters value)
    {
        if (_model is null) return;
        var compositionScale = Mathf.Lerp(0.92f, 1.10f, (float)value.BodyComposition);
        _model.Scale = new Vector3((float)value.HeightScale * compositionScale, (float)value.HeightScale, (float)value.HeightScale * compositionScale);
        foreach (var mesh in Meshes(_model))
        {
            SetBlendShape(mesh, "Body_Composition", (float)value.BodyComposition);
            SetBlendShape(mesh, "Nose_Width", (float)value.NoseWidth);
            SetBlendShape(mesh, "Apparent_Age", (float)value.ApparentAge);
            var name = mesh.Name.ToString();
            if (name.Contains("Body", StringComparison.OrdinalIgnoreCase))
                mesh.MaterialOverride = Surface(ResolveSkin(value), (float)Mathf.Lerp(0.28f, 0.72f, (float)value.SkinRoughness));
            else if (name.Contains("Iris", StringComparison.OrdinalIgnoreCase))
                mesh.MaterialOverride = Surface(ToGodot(value.EyeColor), 0.28f);
            else if (name.Contains("Hair", StringComparison.OrdinalIgnoreCase) || name.Contains("Brow", StringComparison.OrdinalIgnoreCase) || name.Contains("Lash", StringComparison.OrdinalIgnoreCase))
            {
                mesh.Visible = !string.Equals(value.HairStyle, "None", StringComparison.Ordinal);
                mesh.MaterialOverride = Surface(ToGodot(value.HairColor), 0.40f);
            }
            else if (name.Contains("Shirt", StringComparison.OrdinalIgnoreCase))
                mesh.MaterialOverride = Surface(ClothingColor(value.MaterialPreset), 0.76f);
        }
    }

    private static IEnumerable<MeshInstance3D> Meshes(Node node)
    {
        foreach (var child in node.GetChildren())
        {
            if (child is MeshInstance3D mesh) yield return mesh;
            foreach (var descendant in Meshes(child)) yield return descendant;
        }
    }

    private static void SetBlendShape(MeshInstance3D mesh, string name, float value)
    {
        if (mesh.Mesh is null) return;
        var index = mesh.FindBlendShapeByName(name);
        if (index >= 0) mesh.SetBlendShapeValue(index, value);
    }

    private static StandardMaterial3D Surface(Color color, float roughness) => new() { AlbedoColor = color, Roughness = roughness };
    private static Color ToGodot(SemanticColor color) => new((float)color.R, (float)color.G, (float)color.B, (float)color.A);
    private static Color ResolveSkin(MasterHumanRenderParameters value)
    {
        var baseColor = ToGodot(value.SkinTone);
        var melanin = (float)value.SkinMelanin;
        var undertone = (float)value.SkinUndertone - 0.5f;
        return new Color(
            Mathf.Clamp(baseColor.R * Mathf.Lerp(1.12f, 0.72f, melanin) + undertone * 0.045f, 0, 1),
            Mathf.Clamp(baseColor.G * Mathf.Lerp(1.08f, 0.76f, melanin), 0, 1),
            Mathf.Clamp(baseColor.B * Mathf.Lerp(1.04f, 0.80f, melanin) - undertone * 0.025f, 0, 1),
            baseColor.A);
    }
    private static Color ClothingColor(string preset) => preset switch
    {
        "Formal" => new Color("182235"),
        "Athletic" => new Color("126e83"),
        "Workwear" => new Color("574735"),
        "Casual" => new Color("285b73"),
        _ => new Color("214657")
    };
}
