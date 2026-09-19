# GENESIS

Genesis creates portable digital people. Applications consume Genesis people and add application-specific roles and behavior.

Milestone 1 is a standalone Godot .NET application plus an engine-independent C# domain library. It proves that a person can be created from physical facts, represented by deterministic semantic data, previewed by a temporary holographic mannequin, saved to a `.genesis` package, and reopened as the same stable identity.

Flag Football Studio is the first intended consumer and integration test. It is not Genesis's architectural owner, and this repository has no dependency on it.

## Run

Requirements: .NET 8 SDK or newer and Godot 4.7.2 Mono.

```powershell
dotnet build Genesis.slnx
godot --path .
```

Run the portable validation suite and headless application smoke test:

```powershell
dotnet run --project tests/Genesis.Validation/Genesis.Validation.csproj
godot --headless --path . -- --validate-genesis-app
```

The UI supports New Person, Open, Save, Save As, stable identity display, Genesis stages, physical facts, a compact semantic editing surface, persistent category locks, undo/redo, and a central procedural holographic preview. It explicitly identifies the production-human renderer and local Character AI as unconnected.

## Repository structure

```text
Genesis.App.csproj              Godot application project
src/Genesis.App/                UI and replaceable presentation backend
src/Genesis.Core/               engine-independent person domain and package I/O
tests/Genesis.Validation/       dependency-light executable regression suite
scenes/                         Godot scene entry point
docs/                           architecture and integration contracts
```

See [Architecture](docs/ARCHITECTURE.md), [Genesis Person](docs/GENESIS_PERSON.md), [Package Format](docs/PACKAGE_FORMAT.md), [Semantic Editing](docs/SEMANTIC_EDITING.md), [Integration](docs/INTEGRATION.md), and [Validation](docs/VALIDATION.md).
