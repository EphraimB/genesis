# Architecture

## Boundary

```text
                        GENESIS

  Genesis.App (Godot)             future authoring front ends
          │                                  │
          └──────────────┬───────────────────┘
                         ▼
                   Genesis.Core
       identity • facts • semantics • profiles
           edits • locks • history • packages
                         │
                    .genesis file
                         │
         ┌───────────────┼────────────────┐
         ▼               ▼                ▼
   game adapters     LLM adapters     DCC/XR adapters
```

`Genesis.Core` targets ordinary .NET and has no Godot reference. It does not contain nodes, scenes, vectors, resources, engine serialization, rendering state, or application roles. `Genesis.App` depends on Core and translates semantic data into controls and a temporary 3D presentation.

The dependency direction is one-way. Rendering never becomes identity, and a future conversational system may propose semantic edits but cannot mutate scene nodes.

## Core responsibilities

- Stable `PersonId` and independently editable display name.
- Traceable person revisions with revision and parent-revision IDs.
- Authoritative physical facts.
- Versioned `CharacterSpecification` and semantic registry.
- Structured, appearance-independent `PersonalityProfile`.
- Provider-independent `VoiceProfile`.
- Atomic edit plans, persistent locks, and transient exact undo/redo.
- Versioned `.genesis` package serialization with clear failures.
- Provider boundary for a future local Character Director.

## App responsibilities

- File workflow and user feedback.
- Stage/progress presentation.
- Small temporary editing controls.
- Central 3D viewport and procedural mannequin.
- Mapping `CharacterSpecification` to visible proportions, color, and hair.

The mannequin is a presentation adapter and owns no identity. Replacing it with a production rig must not require a package or domain rewrite.

## Deliberate exclusions

Team, jersey number, football position, uniform assignment, routes, coverage, possession, plays, flags, score, and game behavior remain application-owned. Genesis metadata can describe a general occupation or activity, but no consumer-specific role is authoritative person identity.
