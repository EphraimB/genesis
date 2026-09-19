# Genesis Master Human POC-1 Final Report

Date: 2026-09-19  
Decision: **STOP**  
Scope: one adult human, one canonical Game Close asset, one deterministic Genesis semantic path

POC-1 proves that the license gate, deterministic person model, scripted build, rig/export path, `.genesis` packaging, Blender portability, and Godot runtime can work together. It does **not** prove the required close-inspection visual quality. The fixed close-ups show critical failures in eyes/lids, mouth, hairline/groom, skin response, clothing edges, and posed deformation. Two allowed refinement passes were already consumed, so the research stop rule applies.

## 1. Upstream inputs and provenance

- Blender Human Base Meshes 1.4.1, official CC0 bundle, selected source for the production-direction topology. Genesis-computed SHA-256 after quarantine: `811f43accbb31a88266d932f8f5563b2d13586fca0ba2693aad1f5fe582b3515`.
- Anny 0.6.0 wheel, exact PyPI artifact, SHA-256 `050a0b24a3e7fdb89b46dc3581947ea84225305425ac9a1a97b65ea997fb2ad6`. Only its cleared MakeHuman/MPFB and face-unit material was eligible for the comparison/control path.
- Blender 5.2.2 LTS portable, SHA-256 `3849d17a682cba006075aaa3f3597ecb5c9c30ec31035b2e092c53e40679b535`.
- Godot 4.7.2-stable .NET portable (`ed1daf0`), SHA-256 `a2a48473a7414c5f19fab690518caebb738c09ef9601f6bd2388676a7f53b3c0`.
- Exact URLs, byte lengths, license decisions, hashes, and quarantine policy are in `poc/master-human/provenance/dependency-allowlist.json` and `LICENSE_GATE.md`.

## 2. Licenses and excluded dependencies

The gate is default-deny. Blender Human Base Meshes is CC0-1.0; selected Anny code is Apache-2.0 and the selected MPFB/face-unit data is CC0-1.0; Blender was used as a GPL authoring tool; Godot is MIT with bundled notices. SMPL, SMPL-X, Anny's optional non-commercial archive, online Blender assets, unallowlisted community MakeHuman assets, MetaHuman, Reallusion, Daz, Human Generator, MB-Lab, and CharMorph were excluded from the distributable POC. The denied-content scan and hash gate pass.

## 3. Canonical topology

Path B—Blender Human Base Meshes 1.4.1 realistic male—was selected over path A, the Anny/MakeHuman control. The canonical source body contains 10,582 vertices and 10,590 polygons with a stable recorded vertex-order SHA-256 and one UV layer. The decision is not a claim that the unchanged source is production-ready; it is the POC topology/anatomy basis.

## 4. Build pipeline

`poc/master-human/blender/build_master_human.py` performs an offline, scripted clean build from allowlisted local inputs. It builds the body details, morphs, 55-bone rig, fitted POC surfaces, materials, fixed renders, GLB, authoring `.blend`, and machine-readable validation report. The final measured Blender build took 29.093 seconds. No manual mesh edit is hidden outside the script and artifacts.

## 5. Rig

The deform rig contains 55 connected bones: root/pelvis/spine/neck/head, jaw and eyes, bilateral clavicle/arm/forearm/hand chains, five three-segment digits per hand, and bilateral thigh/shin/foot/toe chains. Final skin weights range from 0.999999859 to 1.000000158, with zero unweighted vertices and zero vertices outside the 0.999–1.001 normalization tolerance. GLB export warns that vertices with more than four influences are reduced to the strongest four and renormalized.

## 6. Identity parameters

The authored controls include `Nose_Width`, `Apparent_Age`, `Body_Composition`, and `Taller`, plus blink, jaw, lip seal, smile, frown, brow raise, six visemes, and four eye-look shapes. The report records changed-vertex count, source-Z bounds, and maximum displacement for every body shape. The body-composition shape is bounded below the protected face region; facial shapes meet explicit face/head-neck locality bounds.

## 7. Performance parameters

Game Close is a single 3,202,540-byte GLB with 62 runtime mesh instances. The normal benchmark uses the production backend, OpenGL Compatibility renderer, a 2560×1440 window, 60 warm-up frames, and 180 measured frames. Thresholds are p95 frame time ≤16.7 ms and reported video memory ≤4,096 MiB.

## 8. Materials

The package contains skin and fabric textures and a material manifest. Runtime semantics control skin base tone, melanin, undertone, roughness, eye color, hair color/visibility, and clothing presentation. Freckles and secondary hairstyle are retained as deterministic semantics but are not visually realized beyond this POC's intentionally bounded material/backend work. Materials are technically portable but visibly below the close-inspection target.

## 9. Genesis semantic mapping

`MasterHumanMapper` is engine-independent and versioned as `master-human-poc-1/v1`, with topology `genesis-cc0-human-base-1.4.1/poc-1` and rig `genesis-humanoid-55/poc-1`. It resolves stable height/composition/face/skin/eye/hair/clothing values and a SHA-256 identity fingerprint. Targeted-edit validation proves a nose-width edit changes only the resolved nose value and fingerprint. New POC-1 semantic fields are backfilled for schema-v1 Milestone 1 packages.

## 10. Production backend

`ProductionPersonPreview` loads the GLB through Godot's resource pipeline, instantiates it in the existing viewport, applies height/composition and supported blend shapes, and resolves semantic materials. The procedural preview remains available only as an explicit debug toggle. `Genesis.Core` has no Godot dependency.

## 11. GLB portability

The final GLB SHA-256 is `4ed0d956ed2e8adb89bc2dfc1af6140d17dc9b8f7123ff40c5fd0690a1a88e0e`. Clean Blender re-import found 63 meshes, a 12,010-vertex imported body, one armature, all 55 bones, all required face shapes, and 15 materials. All six re-import checks pass. Godot subsequently imported the same rebuilt asset successfully.

## 12. Godot results

Godot 4.7.2 completed a clean editor/headless import, headless production-backend validation, and a normal GPU-backed run on the NVIDIA GeForce RTX 3080 Ti Laptop GPU. Headless load was 21.06 ms; normal load was 24.32 ms. The production backend reported 62 meshes and no load failure.

## 13. Performance

At 2560×1440, p95 frame time was 7.083 ms across 180 post-warm-up frames. Godot reported 74.9 MiB video memory. Both pass the POC gates of 16.7 ms and 4 GiB. These are isolated local measurements, not a cross-hardware benchmark.

## 14. Final render-suite path

The 19 PNG fixed views and HTML contact sheet are in `artifacts/master-human-poc/renders/`. They include the preserved topology comparison, front/side/three-quarter full body, neutral/profile/three-quarter face, smile, viseme, one-metre view, hands, feet, eyes, mouth, hairline, hard grazing light, and three pose/deformation views.

## 15. Known visual defects

The result is not photorealistic. The eyes are oversized and insufficiently seated in the lids; lid/tearline/lash geometry reads as separate tubes or cards. The mouth is a narrow seam with inadequate lip volume and oral-cavity evidence. The hair is a rigid cap with a visibly jagged, glossy hairline. Brows and lashes are sparse procedural geometry. Skin is smooth/plastic with weak pore, tone, and regional roughness response. Selected-face shirt/trouser construction produces jagged neck, sleeve, waist, and ankle boundaries; the wardrobe lacks production drape and thickness. Nails and mouth internals are rudimentary. Shoulder/elbow/knee and garment deformation is serviceable for pipeline proof but not close-inspection quality. Identity is generic and expression shapes do not preserve a convincingly natural face.

Single-evaluator fixed-view scores on the research 1–5 scale are: overall face 2, identity distinctiveness 2, eyes/lids 1, lips/mouth/nostrils 2, ears 3, hands/fingers/nails 2, skin 2, hairline/groom 1, body anatomy 3, facial expression 2, and posed deformation 2. The required multi-reviewer median was not established; even the available evaluation contains several critical scores at or below 2, which is independently sufficient for STOP.

## 16. Results of refinement passes 1 and 2

Both bounded passes were completed in the interrupted prior run and preserved; this recovery did not perform a third. Their aggregate result is a complete adult body with face, eyes, tearline, brows/lashes, mouth internals, ears, hands/feet/nails, hair cap, clothing, skin/fabric materials, rig, expressions, and production render set. They improved completeness enough to validate the end-to-end architecture, but did not lift the critical eyes, mouth, skin, hairline, garment-edge, or deformation areas above the visual stop threshold. Separate before/after archives were not retained, so pass-specific visual deltas cannot be independently reconstructed from the current worktree; the post-pass fixed suite is the authoritative evidence.

## 17. Experimental `.genesis` package results

`artifacts/master-human-poc/package/genesis_master_human_poc1.genesis` is a 1,925,411-byte deterministic ZIP package with SHA-256 `691f79f80bf0038cf6dd1bb8f667c976f4c52df738b5cc021c1a9ff797c0c9f3`. It contains `manifest.json`, `person.json`, the GLB, material manifest, two textures, mapping/rig metadata, license gate and allowlist, and Blender/re-import/Godot validation reports (12 entries total). The package builder produced it and immediately loaded it; validation reproduced the semantic fingerprint and rejected unsafe supplemental paths.

## 18. Complete validation results

- `dotnet build Genesis.slnx --no-restore`: pass, 0 warnings, 0 errors.
- Existing Genesis validation: 17/17 pass.
- Master Human POC validation: 11/11 pass.
- Experimental package build/load and actual-entry validation: pass.
- True old schema-v1 package with all new POC fields removed: loads and receives compatible defaults.
- Blender scripted build: all 16 checks pass.
- Blender clean GLB re-import: 6/6 checks pass.
- Godot clean import: pass.
- Godot headless production load: pass.
- Godot normal 2560×1440 GPU benchmark: pass.
- Python AST, JSON parsing, project restore, and C# compilation: pass.
- `git diff --check`: pass. Git emitted only the repository's LF-to-CRLF working-copy notices, not whitespace errors.

## 19. Explicit recommendation

**STOP.** The architecture, licensing, portability, deterministic editing, packaging, and performance hypotheses pass. The close-inspection visual hypothesis fails after the two allowed refinement passes, specifically in multiple critical areas scored 1–2. Continuing to polish this same procedural open-source look-development path would violate the precommitted stop rule.

## 20. Exact next milestone

Start a narrowly scoped **Master Human Source-Quality Replacement Gate**, not POC-1 pass 3. Seek a written enterprise/custom agreement from Reallusion or Epic that explicitly permits editable character generation and redistribution inside `.genesis`. In parallel only if that legal path is rejected, commission a Genesis-owned production morph/surface library over the already-cleared CC0 topology. Re-run the same one-person fixed views and the unchanged legal, deterministic, portability, deformation, and performance gates before adding hairstyles, wardrobe libraries, likeness fitting, AI, voice, or additional people.
