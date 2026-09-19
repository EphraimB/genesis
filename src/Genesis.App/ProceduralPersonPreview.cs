using Genesis.Core;
using Godot;

namespace Genesis.App;

/// <summary>A replaceable Milestone 1 presentation adapter; it owns no person data.</summary>
public partial class ProceduralPersonPreview : Node3D
{
    private readonly List<MeshInstance3D> _skinMeshes = [];
    private MeshInstance3D _torso = null!;
    private MeshInstance3D _hips = null!;
    private MeshInstance3D _head = null!;
    private MeshInstance3D _hair = null!;
    private ShaderMaterial _hologram = null!;
    private Label3D _stageLabel = null!;
    private MeshInstance3D _guide = null!;
    private double _height = 1.75;
    private float _phase;

    public override void _Ready()
    {
        _hologram = BuildHologramMaterial();
        BuildMannequin();
        BuildGuide();
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        _phase = (_phase + (float)delta * 0.18f) % 1f;
        _hologram.SetShaderParameter("scan_phase", _phase);
        RotateY((float)delta * 0.08f);
    }

    public void Apply(CharacterSpecification specification)
    {
        if (!IsNodeReady()) return;
        _height = specification.Facts.HeightMeters;
        var scale = (float)(_height / 1.75);
        Scale = new Vector3(scale, scale, scale);
        var shoulders = (float)specification.Value(SemanticPaths.ShoulderWidth).NumberValue;
        var torso = (float)specification.Value(SemanticPaths.TorsoWidth).NumberValue;
        var hips = (float)specification.Value(SemanticPaths.HipWidth).NumberValue;
        _torso.Scale = new Vector3(shoulders, 1, torso);
        _hips.Scale = new Vector3(hips, 1, hips);
        _head.Scale = new Vector3((float)specification.Value(SemanticPaths.JawWidth).NumberValue, 1, 1);
        var skin = ToGodot(specification.Value(SemanticPaths.SkinTone).ColorValue);
        foreach (var mesh in _skinMeshes) mesh.MaterialOverride = Surface(skin);
        _hair.MaterialOverride = Surface(ToGodot(specification.Value(SemanticPaths.HairColor).ColorValue), 0.7f);
        var style = specification.Value(SemanticPaths.HairStyle).TextValue;
        var hairLength = (float)specification.Value(SemanticPaths.HairLength).NumberValue;
        _hair.Visible = style != "None";
        _hair.Scale = new Vector3(1.03f, 0.5f + hairLength * 1.2f, 1.03f);
        _stageLabel.Text = $"{StageName(specification.Stage)}  •  {_height:0.00} m";
        _hologram.SetShaderParameter("reveal_progress", (float)Math.Max(0.12, specification.StageProgress));
    }

    private void BuildMannequin()
    {
        _hips = Part("Hips", new CapsuleMesh { Radius = 0.22f, Height = 0.42f }, new(0, 1.02f, 0), Surface(new Color("30566d")));
        _torso = Part("Torso", new CapsuleMesh { Radius = 0.28f, Height = 0.72f }, new(0, 1.37f, 0), Surface(new Color("29495e")));
        _head = Part("Head", new SphereMesh { Radius = 0.17f, Height = 0.34f }, new(0, 1.86f, 0));
        _hair = Part("Hair", new SphereMesh { Radius = 0.176f, Height = 0.22f }, new(0, 1.96f, 0), Surface(new Color("1a1010")));
        Limb("LeftArm", new(-0.37f, 1.36f, 0), new Vector3(0.10f, 0.7f, 0.10f), -8);
        Limb("RightArm", new(0.37f, 1.36f, 0), new Vector3(0.10f, 0.7f, 0.10f), 8);
        Limb("LeftLeg", new(-0.14f, 0.53f, 0), new Vector3(0.14f, 0.94f, 0.14f));
        Limb("RightLeg", new(0.14f, 0.53f, 0), new Vector3(0.14f, 0.94f, 0.14f));
        foreach (var mesh in GetChildren().OfType<MeshInstance3D>())
            if (mesh.Name != "Hair") _skinMeshes.Add(mesh);
        foreach (var mesh in GetChildren().OfType<MeshInstance3D>()) mesh.MaterialOverlay = _hologram;
    }

    private MeshInstance3D Part(string name, Mesh mesh, Vector3 position, Material? material = null)
    {
        var instance = new MeshInstance3D { Name = name, Mesh = mesh, Position = position, MaterialOverride = material };
        AddChild(instance);
        return instance;
    }

    private void Limb(string name, Vector3 position, Vector3 scale, float rotationDegrees = 0)
    {
        var limb = Part(name, new CapsuleMesh { Radius = 0.5f, Height = 1f }, position);
        limb.Scale = scale;
        limb.RotationDegrees = new Vector3(0, 0, rotationDegrees);
    }

    private void BuildGuide()
    {
        var immediate = new ImmediateMesh();
        var material = Surface(new Color(0.1f, 0.85f, 1f, 0.72f), 1);
        immediate.SurfaceBegin(Mesh.PrimitiveType.Lines, material);
        Line(new(-0.65f, 0, 0), new(-0.65f, 2.08f, 0));
        foreach (var y in new[] { 0f, 0.52f, 1.04f, 1.56f, 2.08f }) Line(new(-0.72f, y, 0), new(-0.58f, y, 0));
        immediate.SurfaceEnd();
        _guide = Part("MeasurementGuide", immediate, Vector3.Zero);
        _guide.MaterialOverlay = null;
        _stageLabel = new Label3D { Position = new(-0.78f, 2.18f, 0), FontSize = 30, OutlineSize = 8, Modulate = new Color("63e6ff"), Billboard = BaseMaterial3D.BillboardModeEnum.Enabled, NoDepthTest = true };
        AddChild(_stageLabel);
        return;
        void Line(Vector3 a, Vector3 b) { immediate.SurfaceAddVertex(a); immediate.SurfaceAddVertex(b); }
    }

    private static StandardMaterial3D Surface(Color color, float emission = 0.08f) => new()
    {
        AlbedoColor = color,
        Metallic = 0.05f,
        Roughness = 0.52f,
        EmissionEnabled = emission > 0,
        Emission = color * emission
    };

    private static ShaderMaterial BuildHologramMaterial()
    {
        var shader = new Shader { Code = """
            shader_type spatial;
            render_mode unshaded, blend_add, depth_prepass_alpha, cull_back;
            uniform float scan_phase = 0.0;
            uniform float reveal_progress = 0.15;
            varying float model_y;
            void vertex() { model_y = VERTEX.y; }
            void fragment() {
                vec3 cyan = vec3(0.05, 0.78, 1.0);
                float stripe = smoothstep(0.72, 1.0, sin((model_y - scan_phase * 3.0) * 58.0) * 0.5 + 0.5);
                float rim = pow(1.0 - abs(dot(normalize(NORMAL), normalize(VIEW))), 2.2);
                float hidden = step(reveal_progress * 2.2, model_y);
                ALBEDO = cyan;
                EMISSION = cyan * (0.18 + stripe * 0.28 + rim * 0.8);
                ALPHA = clamp(0.07 + rim * 0.22 + stripe * 0.07 + hidden * 0.08, 0.05, 0.40);
            }
            """ };
        return new ShaderMaterial { Shader = shader };
    }

    private static Color ToGodot(SemanticColor color) => new((float)color.R, (float)color.G, (float)color.B, (float)color.A);
    private static string StageName(GenesisStage stage) => stage switch
    {
        GenesisStage.IdentityAppearance => "IDENTITY / APPEARANCE",
        GenesisStage.HairDetails => "HAIR / DETAILS",
        GenesisStage.PersonalityVoice => "PERSONALITY / VOICE",
        _ => stage.ToString().ToUpperInvariant()
    };
}
