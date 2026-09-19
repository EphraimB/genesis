# Integration

Consumers should reference `Genesis.Core`, open a `.genesis` package with `GenesisPackageSerializer`, and map the semantic branches they understand. They must retain the stable `PersonId` rather than matching display names.

## Flag Football Studio

Flag Football Studio is the first intended consumer, not the architectural owner. A future importer should map a Genesis person into a roster entry, then assign team, jersey number, football position, uniform, and football behavior in the football project. Those fields must not be written back as authoritative Genesis identity.

Useful generic ideas adapted from the prototype are versioned character semantics, facts separated from appearance, persistent locks, atomic edit plans, exact undo/redo snapshots, structured personality, provider-neutral voice intent, stage presentation, and a visual-controller boundary.

Deliberately left behind are `Player`, `PlayerPawn`, team and roster ownership, position, uniform definitions, football animation/contact anchors, play simulation, cameras, venue/audio behavior, game project persistence, and the football serializer.

## Other future consumers

- An LLM adapter can load identity, personality, conversational tendencies, and voice metadata without loading a 3D engine.
- A 3D engine adapter can map character semantics and future packaged assets into its own rig and material system.
- Blender tooling can read the same appearance specification and future production assets without Godot knowledge.
- XR software can combine appearance, an animation-compatible rig, voice, and personality while owning its interaction behavior.

Consumers should ignore unknown optional fields, reject unsupported required versions, and avoid assuming that every future package contains model or voice assets.
