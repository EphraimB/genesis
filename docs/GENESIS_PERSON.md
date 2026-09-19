# Genesis Person

A `GenesisPerson` has four independent branches and optional inert metadata:

```text
GenesisPerson
├── PersonIdentity
│   ├── stable PersonId
│   ├── editable display name
│   └── revision lineage
├── CharacterSpecification
│   ├── PhysicalFacts
│   ├── semantic appearance values
│   ├── Genesis stage
│   └── category/property locks
├── PersonalityProfile
└── VoiceProfile
```

The `PersonId` is the cross-application identity key. Display names are presentation data and may change without changing that key. Each meaningful mutation can produce a new revision ID linked to the prior revision while retaining the same person ID. Milestone 1 stores one current revision per package; revision history storage and synchronization are future concerns.

Physical facts are values a creator supplies directly: height, weight, age, and dominant hand. They are not inferred from a mesh. The character specification holds derived visual semantics such as frame, proportions, face dimensions, skin, eyes, hair, clothing presentation, and accessories.

Personality is normalized structured data plus optional conversational tendencies. It is never inferred from appearance and does not encode simulation outcomes. Voice describes the design intent, optional provider/model references, speaker reference, and delivery defaults without making a specific TTS engine part of the Genesis schema.

Metadata is for optional generic labels such as occupation, activity, or pronouns. Consumers should not treat arbitrary metadata as a substitute for the typed identity branches.
