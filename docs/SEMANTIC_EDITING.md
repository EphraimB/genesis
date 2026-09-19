# Semantic editing

The authoring contract is:

```text
user intent / future local AI
              ↓
      Character Director
              ↓
   inspectable CharacterEditPlan
              ↓
 registry validation + locks
              ↓
 cloned candidate specification
              ↓
 atomic commit + history
              ↓
 persistent deterministic person
```

Semantic paths such as `body.shoulder_width`, `face.jaw_width`, and `hair.length` have a declared category, value kind, and valid range. An edit plan contains a transaction ID and one or more set, adjust, lock, or unlock operations.

The edit service applies every operation to a clone. Any invalid or locked operation rejects the whole plan and returns an unchanged clone, preventing partial identity updates. Unmentioned properties remain exactly unchanged.

Category and property locks are part of the character specification and persist in the package. Undo and redo hold exact before/after snapshots for the current authoring session and are deliberately transient. Loading a package starts a new edit history.

The `ILocalCharacterDirector` interface accepts data snapshots and safe input references. It returns a proposal; it never receives Godot nodes, materials, arbitrary scene paths, or permission to bypass validation. Milestone 1 registers no AI implementation and the UI says so plainly.
