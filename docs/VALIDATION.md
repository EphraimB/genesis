# Validation

Run from the repository root:

```powershell
dotnet build Genesis.slnx
dotnet run --project tests/Genesis.Validation/Genesis.Validation.csproj
godot --headless --path . -- --validate-genesis-app
godot --path . --quit-after 3
git diff --check
```

The executable Core suite validates:

1. New stable identity.
2. Complete package round trip.
3. Stable ID preservation.
4. Display-name independence and revision lineage.
5. Character specification round trip.
6. Personality round trip.
7. Voice round trip.
8. Persistent locks.
9. Targeted edits preserving unrelated properties.
10. Atomic rejection of an invalid multi-operation plan.
11. Exact undo.
12. Exact redo.
13. Safe malformed-package failure.
14. Clear future-package-version failure.
15. Clear future-person-schema failure.
16. Tolerance of unknown optional JSON fields.
17. Absence of a Godot assembly dependency in `Genesis.Core`.

The headless app switch builds the actual scene, UI, viewport, preview, and initial person before printing `GENESIS_APP_VALIDATION_OK` and exiting. The timed normal launch checks the ordinary application path rather than the validation-only console project.
