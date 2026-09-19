using Genesis.Core;
using Genesis.Core.Package;
using Godot;

namespace Genesis.App;

public partial class Main : Control
{
    private readonly GenesisPackageSerializer _packages = new();
    private GenesisPerson _person = null!;
    private CharacterEditHistory _history = null!;
    private string? _path;
    private bool _syncing;
    private Label _status = null!;
    private Label _identity = null!;
    private Label _stage = null!;
    private ProgressBar _progress = null!;
    private LineEdit _name = null!;
    private SpinBox _height = null!;
    private SpinBox _weight = null!;
    private SpinBox _age = null!;
    private HSlider _shoulders = null!;
    private HSlider _hairLength = null!;
    private OptionButton _hairStyle = null!;
    private CheckButton _factsLock = null!;
    private CheckButton _bodyLock = null!;
    private CheckButton _hairLock = null!;
    private Button _undo = null!;
    private Button _redo = null!;
    private ProceduralPersonPreview _preview = null!;
    private FileDialog _openDialog = null!;
    private FileDialog _saveDialog = null!;
    private ConfirmationDialog _newDialog = null!;
    private LineEdit _newName = null!;
    private SpinBox _newHeight = null!;
    private SpinBox _newWeight = null!;
    private SpinBox _newAge = null!;

    public override void _Ready()
    {
        BuildTheme();
        BuildUi();
        NewPerson("Untitled Person", new PhysicalFacts(1.75, 72, 30));
        if (OS.GetCmdlineUserArgs().Contains("--validate-genesis-app"))
        {
            GD.Print("GENESIS_APP_VALIDATION_OK");
            GetTree().Quit();
        }
    }

    private void BuildUi()
    {
        var root = new VBoxContainer { Name = "ApplicationLayout" };
        root.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
        root.AddThemeConstantOverride("separation", 0);
        AddChild(root);
        root.AddChild(BuildTopBar());

        var content = new HSplitContainer { SizeFlagsVertical = SizeFlags.ExpandFill, SplitOffsets = [330] };
        root.AddChild(content);
        content.AddChild(BuildInspector());

        var centerAndDetails = new HSplitContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SplitOffsets = [780] };
        content.AddChild(centerAndDetails);
        centerAndDetails.AddChild(BuildViewport());
        centerAndDetails.AddChild(BuildSemanticPanel());

        _status = new Label { Text = "Ready", CustomMinimumSize = new Vector2(0, 32), VerticalAlignment = VerticalAlignment.Center };
        _status.AddThemeColorOverride("font_color", new Color("7adff2"));
        _status.AddThemeConstantOverride("outline_size", 4);
        root.AddChild(_status);
        BuildDialogs();
    }

    private Control BuildTopBar()
    {
        var bar = new HBoxContainer { CustomMinimumSize = new Vector2(0, 70) };
        bar.AddThemeConstantOverride("separation", 8);
        var brand = new Label { Text = "  G E N E S I S", CustomMinimumSize = new Vector2(280, 0), VerticalAlignment = VerticalAlignment.Center };
        brand.AddThemeFontSizeOverride("font_size", 24);
        brand.AddThemeColorOverride("font_color", new Color("5ce5ff"));
        bar.AddChild(brand);
        AddButton(bar, "New Person", ShowNewDialog);
        AddButton(bar, "Open", () => _openDialog.PopupCenteredRatio(0.72f));
        AddButton(bar, "Save", Save);
        AddButton(bar, "Save As", () => _saveDialog.PopupCenteredRatio(0.72f));
        _undo = AddButton(bar, "Undo", Undo);
        _redo = AddButton(bar, "Redo", Redo);
        var spacer = new Control { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        bar.AddChild(spacer);
        var marker = new Label { Text = "MILESTONE 1  •  LOCAL  •  PORTABLE  ", VerticalAlignment = VerticalAlignment.Center };
        marker.AddThemeColorOverride("font_color", new Color("7894a6"));
        bar.AddChild(marker);
        return bar;
    }

    private Control BuildInspector()
    {
        var scroll = new ScrollContainer { CustomMinimumSize = new Vector2(330, 0), SizeFlagsVertical = SizeFlags.ExpandFill };
        var panel = new VBoxContainer { CustomMinimumSize = new Vector2(310, 0) };
        panel.AddThemeConstantOverride("separation", 10);
        scroll.AddChild(panel);
        Section(panel, "PERSON IDENTITY");
        _name = new LineEdit { PlaceholderText = "Display name" };
        _name.TextSubmitted += value => { if (!_syncing) Rename(value); };
        _name.FocusExited += () => { if (!_syncing && _name.Text != _person.Identity.DisplayName) Rename(_name.Text); };
        panel.AddChild(_name);
        _identity = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart };
        _identity.AddThemeColorOverride("font_color", new Color("86a3b5"));
        panel.AddChild(_identity);
        Section(panel, "GENESIS PROGRESS");
        _stage = new Label();
        _stage.AddThemeFontSizeOverride("font_size", 18);
        panel.AddChild(_stage);
        _progress = new ProgressBar { MaxValue = 1, ShowPercentage = false, CustomMinimumSize = new Vector2(0, 8) };
        panel.AddChild(_progress);
        var stageButtons = new HBoxContainer();
        AddButton(stageButtons, "Back", () => MoveStage(-1));
        AddButton(stageButtons, "Continue", () => MoveStage(1));
        panel.AddChild(stageButtons);
        Section(panel, "PHYSICAL FACTS");
        _height = Spin(panel, "Height (m)", 1, 2.5, 0.01, SemanticPaths.Height);
        _weight = Spin(panel, "Weight (kg)", 25, 350, 0.5, SemanticPaths.Weight);
        _age = Spin(panel, "Age (years)", 1, 125, 1, SemanticPaths.Age);
        _factsLock = Lock(panel, "Lock physical facts", CharacterIdentityCategory.PhysicalFacts);
        var note = new Label { Text = "Facts are authoritative inputs. Appearance, personality, and voice remain independent.", AutowrapMode = TextServer.AutowrapMode.WordSmart };
        note.AddThemeColorOverride("font_color", new Color("7894a6"));
        panel.AddChild(note);
        return scroll;
    }

    private Control BuildViewport()
    {
        var frame = new MarginContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        frame.AddThemeConstantOverride("margin_left", 12);
        frame.AddThemeConstantOverride("margin_right", 12);
        frame.AddThemeConstantOverride("margin_top", 12);
        frame.AddThemeConstantOverride("margin_bottom", 12);
        var container = new SubViewportContainer { Stretch = true, SizeFlagsHorizontal = SizeFlags.ExpandFill, SizeFlagsVertical = SizeFlags.ExpandFill };
        frame.AddChild(container);
        var viewport = new SubViewport { OwnWorld3D = true, TransparentBg = false, Msaa3D = Viewport.Msaa.Msaa4X, RenderTargetUpdateMode = SubViewport.UpdateMode.Always };
        container.AddChild(viewport);
        var world = new Node3D();
        viewport.AddChild(world);
        var camera = new Camera3D { Position = new Vector3(0, 1.15f, 4.25f), Current = true, Fov = 34 };
        camera.LookAtFromPosition(camera.Position, new Vector3(0, 1.05f, 0));
        world.AddChild(camera);
        world.AddChild(new DirectionalLight3D { RotationDegrees = new Vector3(-42, -28, 0), LightEnergy = 1.7f, ShadowEnabled = true });
        world.AddChild(new OmniLight3D { Position = new Vector3(-2, 2.2f, 2), LightColor = new Color("28c4ff"), OmniRange = 8, LightEnergy = 5 });
        world.AddChild(new WorldEnvironment { Environment = new Godot.Environment { BackgroundMode = Godot.Environment.BGMode.Color, BackgroundColor = new Color("08111b"), AmbientLightSource = Godot.Environment.AmbientSource.Color, AmbientLightColor = new Color("244152"), AmbientLightEnergy = 0.8f, TonemapMode = Godot.Environment.ToneMapper.Filmic } });
        var floor = new MeshInstance3D { Mesh = new CylinderMesh { TopRadius = 1.2f, BottomRadius = 1.4f, Height = 0.05f }, Position = new Vector3(0, -0.03f, 0), MaterialOverride = new StandardMaterial3D { AlbedoColor = new Color("0a2532"), Metallic = 0.6f, Roughness = 0.25f, EmissionEnabled = true, Emission = new Color("06384a") } };
        world.AddChild(floor);
        _preview = new ProceduralPersonPreview();
        world.AddChild(_preview);
        var overlay = new VBoxContainer { MouseFilter = MouseFilterEnum.Ignore, OffsetLeft = 20, OffsetTop = 20 };
        overlay.AddChild(new Label { Text = "PROCEDURAL IDENTITY PREVIEW" });
        var warning = new Label { Text = "Production digital-human graphics are not connected yet." };
        warning.AddThemeColorOverride("font_color", new Color("8ca5b2"));
        overlay.AddChild(warning);
        container.AddChild(overlay);
        return frame;
    }

    private Control BuildSemanticPanel()
    {
        var scroll = new ScrollContainer { CustomMinimumSize = new Vector2(330, 0), SizeFlagsVertical = SizeFlags.ExpandFill };
        var panel = new VBoxContainer { CustomMinimumSize = new Vector2(310, 0) };
        panel.AddThemeConstantOverride("separation", 10);
        scroll.AddChild(panel);
        Section(panel, "SEMANTIC APPEARANCE");
        panel.AddChild(new Label { Text = "A compact deterministic surface—not a wall of production sliders.", AutowrapMode = TextServer.AutowrapMode.WordSmart });
        _shoulders = Slider(panel, "Shoulder width", 0.7, 1.3, SemanticPaths.ShoulderWidth);
        _bodyLock = Lock(panel, "Lock body identity", CharacterIdentityCategory.BodyIdentity);
        panel.AddChild(new HSeparator());
        panel.AddChild(new Label { Text = "Hair style" });
        _hairStyle = new OptionButton();
        foreach (var value in new[] { "None", "Buzz", "Short", "Medium", "Long", "Curly", "Ponytail", "Bun" }) _hairStyle.AddItem(value);
        _hairStyle.ItemSelected += index => { if (!_syncing) Edit(CharacterEditOperation.Set(SemanticPaths.HairStyle, SemanticValue.Text(_hairStyle.GetItemText((int)index))), "Set hair style"); };
        panel.AddChild(_hairStyle);
        _hairLength = Slider(panel, "Hair length", 0, 1, SemanticPaths.HairLength);
        _hairLock = Lock(panel, "Lock hair identity", CharacterIdentityCategory.HairIdentity);
        Section(panel, "CHARACTER DIRECTOR");
        var ai = new Label { Text = "Local Character AI not connected\n\nFuture conversation will produce an inspectable semantic edit plan, then pass through these same locks and transactions.", AutowrapMode = TextServer.AutowrapMode.WordSmart };
        ai.AddThemeColorOverride("font_color", new Color("7697aa"));
        panel.AddChild(ai);
        return scroll;
    }

    private void BuildDialogs()
    {
        _openDialog = new FileDialog { FileMode = FileDialog.FileModeEnum.OpenFile, Access = FileDialog.AccessEnum.Filesystem, UseNativeDialog = true, Filters = ["*.genesis ; Genesis Person"] };
        _openDialog.FileSelected += Open;
        AddChild(_openDialog);
        _saveDialog = new FileDialog { FileMode = FileDialog.FileModeEnum.SaveFile, Access = FileDialog.AccessEnum.Filesystem, UseNativeDialog = true, Filters = ["*.genesis ; Genesis Person"], CurrentFile = "person.genesis" };
        _saveDialog.FileSelected += SaveAs;
        AddChild(_saveDialog);
        _newDialog = new ConfirmationDialog { Title = "New Genesis Person", OkButtonText = "Create Person", Size = new Vector2I(460, 420) };
        var fields = new VBoxContainer();
        fields.AddThemeConstantOverride("separation", 10);
        fields.AddChild(new Label { Text = "Enter stable physical facts before the initial CharacterSpecification is created." });
        _newName = new LineEdit { Text = "New Person" };
        fields.AddChild(Labeled("Display name", _newName));
        _newHeight = BareSpin(1, 2.5, 0.01, 1.75);
        fields.AddChild(Labeled("Height (m)", _newHeight));
        _newWeight = BareSpin(25, 350, 0.5, 72);
        fields.AddChild(Labeled("Weight (kg)", _newWeight));
        _newAge = BareSpin(1, 125, 1, 30);
        fields.AddChild(Labeled("Age (years)", _newAge));
        _newDialog.AddChild(fields);
        _newDialog.Confirmed += () => NewPerson(string.IsNullOrWhiteSpace(_newName.Text) ? "New Person" : _newName.Text, new PhysicalFacts(_newHeight.Value, _newWeight.Value, (int)_newAge.Value));
        AddChild(_newDialog);
    }

    private void ShowNewDialog() => _newDialog.PopupCentered();

    private void NewPerson(string name, PhysicalFacts facts)
    {
        _person = GenesisPerson.Create(name, facts);
        _history = new CharacterEditHistory(_person.Character);
        _path = null;
        Sync("New portable person created. Save to assign a package location.");
    }

    private void Rename(string value)
    {
        try { _person = _person.WithDisplayName(value); Sync("Display name changed; stable person ID preserved."); }
        catch (ArgumentException ex) { Sync($"Name not changed: {ex.Message}"); }
    }

    private void Edit(CharacterEditOperation operation, string label)
    {
        var result = _history.Apply(new CharacterEditPlan([operation], label));
        if (!result.Validation.IsValid) { Sync(result.Validation.Errors[0]); return; }
        _person = new GenesisPerson(_person.Identity.NextRevision(), _history.Current, _person.Personality, _person.Voice, _person.Metadata);
        Sync(label);
    }

    private void MoveStage(int delta)
    {
        var target = (GenesisStage)Math.Clamp((int)_history.Current.Stage + delta, 0, (int)GenesisStage.Complete);
        var spec = _history.SetStage(target);
        _person = new GenesisPerson(_person.Identity.NextRevision(), spec, _person.Personality, _person.Voice, _person.Metadata);
        Sync("Genesis stage updated.");
    }

    private void Undo()
    {
        if (!_history.CanUndo) return;
        var spec = _history.Undo();
        _person = new GenesisPerson(_person.Identity.NextRevision(), spec, _person.Personality, _person.Voice, _person.Metadata);
        Sync("Edit undone.");
    }

    private void Redo()
    {
        if (!_history.CanRedo) return;
        var spec = _history.Redo();
        _person = new GenesisPerson(_person.Identity.NextRevision(), spec, _person.Personality, _person.Voice, _person.Metadata);
        Sync("Edit redone.");
    }

    private void Save()
    {
        if (_path is null) { _saveDialog.PopupCenteredRatio(0.72f); return; }
        SaveAs(_path);
    }

    private void SaveAs(string path)
    {
        try
        {
            if (!path.EndsWith(GenesisPackageSerializer.Extension, StringComparison.OrdinalIgnoreCase)) path += GenesisPackageSerializer.Extension;
            _packages.Save(path, _person);
            _path = path;
            Sync($"Saved {Path.GetFileName(path)}");
        }
        catch (Exception ex) { Sync($"Save failed: {ex.Message}"); }
    }

    private void Open(string path)
    {
        try
        {
            _person = _packages.Load(path);
            _history = new CharacterEditHistory(_person.Character);
            _path = path;
            Sync($"Opened {Path.GetFileName(path)} — same stable identity restored.");
        }
        catch (Exception ex) { Sync($"Open failed safely: {ex.Message}"); }
    }

    private void Sync(string message)
    {
        if (_name is null) return;
        _syncing = true;
        var spec = _history.Current;
        _name.Text = _person.Identity.DisplayName;
        _identity.Text = $"Stable ID\n{_person.Identity.PersonId}\nRevision { _person.Identity.RevisionNumber }";
        _stage.Text = StageText(spec.Stage);
        _progress.Value = spec.StageProgress;
        _height.Value = spec.Facts.HeightMeters;
        _weight.Value = spec.Facts.WeightKilograms;
        _age.Value = spec.Facts.AgeYears;
        _shoulders.Value = spec.Value(SemanticPaths.ShoulderWidth).NumberValue;
        _hairLength.Value = spec.Value(SemanticPaths.HairLength).NumberValue;
        var hair = spec.Value(SemanticPaths.HairStyle).TextValue;
        for (var i = 0; i < _hairStyle.ItemCount; i++) if (_hairStyle.GetItemText(i) == hair) _hairStyle.Select(i);
        _factsLock.ButtonPressed = spec.LockedCategories.Contains(CharacterIdentityCategory.PhysicalFacts);
        _bodyLock.ButtonPressed = spec.LockedCategories.Contains(CharacterIdentityCategory.BodyIdentity);
        _hairLock.ButtonPressed = spec.LockedCategories.Contains(CharacterIdentityCategory.HairIdentity);
        _undo.Disabled = !_history.CanUndo;
        _redo.Disabled = !_history.CanRedo;
        _status.Text = $"  {message}";
        _preview.Apply(spec);
        _syncing = false;
    }

    private SpinBox Spin(Container parent, string label, double min, double max, double step, string path)
    {
        var value = BareSpin(min, max, step, min);
        value.ValueChanged += next => { if (!_syncing) Edit(CharacterEditOperation.Set(path, path == SemanticPaths.Age ? SemanticValue.Integer((int)next) : SemanticValue.Number(next)), $"Set {label.ToLowerInvariant()}"); };
        parent.AddChild(Labeled(label, value));
        return value;
    }

    private HSlider Slider(Container parent, string label, double min, double max, string path)
    {
        parent.AddChild(new Label { Text = label });
        var slider = new HSlider { MinValue = min, MaxValue = max, Step = 0.01, CustomMinimumSize = new Vector2(0, 24) };
        slider.DragEnded += changed => { if (changed && !_syncing) Edit(CharacterEditOperation.Set(path, SemanticValue.Number(slider.Value)), $"Set {label.ToLowerInvariant()}"); };
        parent.AddChild(slider);
        return slider;
    }

    private CheckButton Lock(Container parent, string label, CharacterIdentityCategory category)
    {
        var button = new CheckButton { Text = label };
        button.Toggled += enabled => { if (!_syncing) Edit(enabled ? CharacterEditOperation.Lock(category) : CharacterEditOperation.Unlock(category), enabled ? $"Lock {category}" : $"Unlock {category}"); };
        parent.AddChild(button);
        return button;
    }

    private static SpinBox BareSpin(double min, double max, double step, double value) => new() { MinValue = min, MaxValue = max, Step = step, Value = value, AllowGreater = false, AllowLesser = false };
    private static Control Labeled(string label, Control control) { var box = new VBoxContainer(); box.AddChild(new Label { Text = label }); box.AddChild(control); return box; }
    private static void Section(Container parent, string text) { parent.AddChild(new HSeparator()); var label = new Label { Text = text }; label.AddThemeColorOverride("font_color", new Color("5ce5ff")); label.AddThemeFontSizeOverride("font_size", 15); parent.AddChild(label); }
    private static Button AddButton(Container parent, string text, Action action) { var button = new Button { Text = text }; button.Pressed += action; parent.AddChild(button); return button; }
    private static string StageText(GenesisStage value) => value switch { GenesisStage.IdentityAppearance => "Identity / Appearance", GenesisStage.HairDetails => "Hair / Details", GenesisStage.PersonalityVoice => "Personality / Voice", _ => value.ToString() };

    private void BuildTheme()
    {
        var theme = new Theme();
        theme.SetDefaultFontSize(14);
        theme.SetColor("font_color", "Label", new Color("d8e8ef"));
        Theme = theme;
    }
}
