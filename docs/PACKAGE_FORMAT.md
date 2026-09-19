# Package format

## Decision

Milestone 1 uses a single ZIP container with the `.genesis` extension. A single file is easy to move, choose in a file dialog, checksum, and reference from another project. ZIP is broadly readable outside Godot and can later hold binary assets without embedding them in JSON. A directory package would be convenient during authoring but easier to split accidentally; a custom opaque binary format would reduce portability.

The file is a container, not executable content. Readers load only known declarative entries and never deserialize arbitrary types or code.

## Version 1 layout

```text
person.genesis
├── manifest.json
└── person.json
```

`manifest.json` contains `packageVersion`, `personSchemaVersion`, stable person ID, revision ID, and a typed content list. `person.json` contains identity, physical facts, character specification, personality, voice, and optional metadata.

The manifest is ready to list future package-relative entries such as:

```text
models/       textures/       hair/
voice/        thumbnails/     provenance/
```

Milestone 1 does not create empty or fake production assets.

## Compatibility and safety

- Package and person schema versions are explicit and independent.
- A reader rejects newer major integer versions with a useful error.
- Unknown optional JSON fields are ignored, so additive evolution does not break old readers.
- Required entries, IDs, and revision IDs are cross-checked.
- Duplicate names, missing entries, oversized JSON, invalid values, unsafe asset references, and corrupt ZIP data fail clearly.
- Writes use a sibling temporary file and replace the destination only after a complete archive is produced.
- Entry timestamps and property ordering are stabilized where practical. Deterministic person semantics, not byte identity, are the public guarantee.

Future schema migrations should be explicit transforms. Silent coercion of incompatible identity data is prohibited.
