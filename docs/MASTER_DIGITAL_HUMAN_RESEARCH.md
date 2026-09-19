# Genesis Master Digital Human Research and Decision

- **Decision status:** Recommended technical direction for a bounded proof of concept
- **Research date:** 2026-09-19
- **Scope:** Architecture and licensing research only; no production-human implementation or asset acquisition
- **Legal note:** This is engineering research, not legal advice. A release that incorporates third-party material still requires a final asset-by-asset legal review.

## 1. Executive decision

Genesis should pursue **C: a license-constrained hybrid master-human strategy**.

The production direction is:

1. Keep `GenesisPerson` and `CharacterSpecification` as the authoritative, persistent identity model.
2. Build a Genesis-controlled semantic mapper, canonical production asset, rig, identity/performance separation, validation pipeline, quality tiers, and portable exports.
3. Seed the first body/identity parameter space from **Anny v0.6** and only the assets in its dependency closure that are explicitly cleared for redistribution (Apache-2.0 and CC0 material). Do not install, use, or redistribute its optional SMPL-X topology.
4. Use the **Blender Human Base Meshes v1.4.1 (CC0)** and the Blender Studio realistic-human research as anatomical, topology, sculpting, and validation references. If any geometry is actually derived from the bundle, record that fact even though CC0 imposes no attribution requirement.
5. Treat **MakeHuman/MPFB 2.0.x** as a valuable source of CC0 target data, facial units, visemes, and workflow knowledge, but not as the final close-range rendering system. Its GPL application/add-on code should remain an offline tool unless Genesis deliberately accepts the corresponding code-license obligations.
6. Author and maintain a Genesis production topology, deformation rig, correctives, materials, groom assets, clothing system, and semantic mapping layer. “Genesis-maintained” is the accurate term: upstream derivation and notices remain recorded; modification does not erase provenance.
7. Use Blender as an offline, scripted authoring and validation environment. Use glTF/GLB as the baseline delivery representation, not as the complete authoring representation.

This is not a decision to ship Anny, MPFB, or a Blender base mesh unchanged. It is a decision to avoid recreating all human anatomy and useful parameter data from zero while also avoiding a proprietary generator as Genesis's identity database.

The next milestone must build **one** serious human and attempt to disprove this strategy quickly. It is a GO only if that person passes close-view anatomy, identity persistence, deterministic editing, deformation, portability, performance, and licensing gates. After two bounded art/technical refinement passes, failure of a critical visual or legal gate is a STOP, not an invitation to spend months polishing a weak base.

## 2. Genesis requirements

The master human is a platform asset, not a football character. It must support independent consumption by games, Godot, Unity, Unreal, Blender, film/XR tools, simulation, and future local-agent applications.

The non-negotiable requirements are:

- **Persistent identity:** the same semantic specification resolves to the same person. Expression, pose, clothing, and speech do not redefine identity.
- **Deterministic semantic editing:** natural-language intent becomes an explicit edit plan and then bounded parameter changes. The visual system must not hallucinate a replacement mesh after each request.
- **Locality and locks:** “keep the face” and “change only the hair” must be mechanically enforceable, not prompt suggestions.
- **Many genuinely distinct people:** variation must include global proportions, regional facial structure, body composition, apparent age, asymmetry, pigmentation, surface traits, and modular features. A preset library with different noses is insufficient.
- **Close-view credibility:** the face at approximately one metre, eyes, eyelids, lips, nostrils, ears, teeth, oral cavity, hands, fingers, nails, feet, skin, hairline, and deformation are first-class targets.
- **Performance separation:** expressions, blinks, gaze, visemes, pose correctives, and transient wrinkle response are not stored as identity.
- **Portability:** a useful person must survive outside the Genesis/Godot renderer with a documented skeleton, morph semantics, PBR textures, and metadata.
- **Provenance:** every code dependency and every mesh, target, texture, groom, garment, and learned-data contribution must have a source, version, hash, license, and transformation record.
- **Scalable quality:** Genesis may show one hero at very high quality, while consumers can select close, medium, distant, or crowd tiers.

## 3. Current Genesis architecture constraints

The existing Milestone 1 architecture is compatible with this decision and does not need redesign.

### What already fits

- `Genesis.Core` is engine-independent and does not depend on Godot.
- `GenesisPerson` composes stable identity, `CharacterSpecification`, `PersonalityProfile`, and `VoiceProfile` rather than treating the preview mesh as the person.
- `PhysicalFacts` already keeps measured/biographical facts—height, weight, age, and dominant hand—inside the character specification with validation. Visual apparent age should later be a distinct appearance control; it must not silently rewrite the person's factual age.
- Semantic edit plans, transactions, persistent category locks, and undo/redo already establish the correct control boundary.
- The `.genesis` ZIP package already separates a manifest and semantic person document from extensible asset directories.
- `ProceduralPersonPreview` is explicitly a replaceable visual adapter. It is not an authoritative human representation.
- The documented Genesis stages permit a later production backend without contaminating Core with engine concepts. The current procedural/holographic preview remains a useful early-stage visualization, not a quality baseline or production-human dependency.

### The pressure point

`CharacterSpecification` is intentionally small. Production will require more semantic fields and a versioned mapping profile, but this should be an additive evolution rather than a replacement of the model. The renderer must consume a resolved parameter document produced from `CharacterSpecification`; the resolved mesh or a vendor parameter vector must never become the source of truth.

No blocker discovered in this research requires changing Milestone 1 code now.

## 4. Original agent-built strategy

### What an agentic workflow can do well

Blender supports background operation, Python scripting, and command-line rendering. A Genesis build system can therefore make engineering properties repeatable:

- exact anthropometric measurements and unit conventions;
- symmetry, controlled asymmetry, and protected regions;
- topology, manifold, normal, UV, material-slot, and naming checks;
- bone placement, hierarchy, rest pose, weight normalization, and influence limits;
- morph range, locality, collision, and combination tests;
- deterministic LOD, texture baking, export, and package assembly;
- multi-camera renders under fixed lighting;
- image-difference and landmark regression tests;
- skeleton/morph/material round-trip checks in Blender and Godot;
- provenance manifests and content hashes.

The loop proposed in the request is feasible: modify an asset, render fixed front/side/three-quarter/full-body/detail views, run geometry and landmark checks, inspect visually, then make a bounded correction. This can converge efficiently once the starting anatomy and evaluation references are good.

### What the workflow cannot manufacture reliably from engineering rules alone

Photoreal human quality is not reducible to measurements. The Blender Studio production research repeatedly treats eye and dental construction, eyelid wrapping, lip volume, facial fat, ears, hand anatomy, asymmetry, skin sliding, tertiary detail, and expression sculpting as expert, reference-driven work. It also recommends locking topology before downstream rigging and shading because later changes are expensive.

An agent can detect many defects and execute precise changes, but current coding/graphics-agent workflows do not provide evidence that they can independently invent a production-quality human identity space, realistic facial anatomy, expressive correctives, skin microstructure, and grooming from primitive geometry. Automated visual scoring can also reward a plausible image while hiding bad topology or view-specific tricks.

### Direct answer

**Current agentic workflows can create and operate the Genesis production pipeline, and can help a high-quality human converge. They should not be expected to create the entire photoreal anatomical/artistic foundation from zero.** Expert-quality, license-cleared source geometry/targets and periodic human visual judgment are still required. For that reason, an entirely original-from-zero master is not the primary strategy.

## 5. Existing-system strategy

### Anny v0.6

Anny is the strongest current open candidate for the **parametric body foundation**. NAVER describes it as a scan-free, differentiable, all-ages model with interpretable phenotype controls. The official repository exposes age, height, weight, body-shape, local-change, and facial-action parameters, includes an `anny` rig, and supports alternate topology/rig output.

Strengths:

- current release activity (v0.6, 2026-08-06);
- deterministic numerical parameterization and a scriptable PyTorch interface;
- interpretable body controls and uncommon coverage from infants to elders;
- permissive Apache-2.0 code and identified CC0/Apache asset components;
- common topology across generated people;
- separate facial actions rather than baking expressions into identity.

Limitations:

- it is a human shape model, not a finished photoreal character product;
- it does not solve hero skin, eyes, teeth, oral cavity, facial identity richness, hair, clothing, production facial deformation, or engine-ready shading;
- its installer can obtain optional non-commercial assets; dependency resolution must therefore be pinned and audited rather than accepted by default;
- optional SMPL-X topology is not acceptable under the default non-commercial SMPL-X terms.

Conclusion: select Anny as a candidate **source parameter layer**, subject to a clean dependency manifest, not as the finished master.

### MakeHuman / MPFB 2.0.x

MPFB remains active and useful. MPFB 2.0.15 added viseme and ARKit-oriented face-shape packs, 2.0.16 expanded expression tooling and scripting examples, and the current 2.0.17 release (2026-07-22) added seeded, controlled character randomization and batch generation. Its fixed-topology target system is well suited to deterministic semantic morphing, and its export-copy tooling can prepare visemes and facial units.

Strengths:

- mature parametric targets and broad body variation;
- strong Blender integration and scripting potential;
- core MakeHuman assets are CC0;
- recent expression, viseme, and ARKit face-unit work;
- practical rig, garment, and proxy workflows.

Limitations:

- MPFB code is GPL and MakeHuman application code is AGPL; code and assets must not be conflated;
- only core/system assets are uniformly CC0; community assets may be CC0 or CC-BY and require individual tracking;
- the hm08 base and default materials do not meet the one-metre photoreal bar without substantial replacement and refinement;
- Blender procedural materials and helpers do not automatically become faithful portable PBR assets;
- its topology and default rig are useful starting data, not a final production answer.

Conclusion: use cleared asset data and workflow knowledge where helpful; do not adopt the complete MPFB runtime or output stack as Genesis's master.

### Blender Human Base Meshes v1.4.1

Blender publishes a current 49 MB CC0 human base-mesh bundle (v1.4.1, updated 2026-01-20). The bundle and release notes emphasize quad topology, UVs/UDIMs, multiresolution detail, face sets, and improved human topology. Blender Studio's realistic-human research provides production evidence about topology lock, reprojection, sculpt layers, FACS, and age.

Strengths are clean licensing, useful topology/anatomy reference, sculpt-friendly organization, and production-oriented teaching material. The bundle is not a generator, identity model, complete rig, material system, or clothing/hair solution. It is therefore a reference/derivative seed, not a standalone foundation.

### CharMorph and MB-Lab

MB-Lab 1.8.1 is archived and its development moved toward CharMorph. CharMorph is active (v0.4.2 was published 2026-01-10) and provides morphing, fitting, Rigify support, and several character families.

Its licensing is unsuitable as a uniform Genesis foundation. CharMorph code is GPL, but its character databases are licensed per character. Its documentation states that MB-Lab character meshes/data are AGPL and resulting meshes inherit that license, while other characters are CC-BY or CC0. A user can make a compliant result, but Genesis cannot safely assume that every generated person has the same redistribution terms.

Conclusion: not selected. Individual CC0/CC-BY components could be reconsidered only through the same provenance gate as any other asset.

### Commercial quality references

**MetaHuman** is the strongest quality and facial-rig benchmark in this set. Current MetaHuman documentation describes high-fidelity faces/bodies/hair, a DNA/RigLogic representation, many semantic controls, multiple LODs, and DCC export. Epic now states that MetaHumans can be used with any engine under the standard Unreal Engine license. However, the system remains tied to proprietary DNA/RigLogic and Unreal authoring infrastructure, and the current Unreal EULA contains specific AI/database restrictions concerning MetaHuman characters and data. Whether Genesis could redistribute editable raw MetaHuman-derived people inside a general character-creation platform is not established by the reviewed pages and is **UNKNOWN without written permission or legal review**.

**Reallusion Character Creator 5 / Headshot 3** is a strong visual and workflow benchmark with extensive morphs, shaders, hair, clothing, and DCC/engine integrations. Reallusion's own EULA and licensing guidance reserve its topology/rig, restrict standalone/raw redistribution and character-generator use, add AI/synthetic-data restrictions, and point application integrations toward enterprise licensing. It is not suitable as the default portable Genesis foundation under ordinary terms.

**Daz Genesis 9** offers a rich content and morph ecosystem, facial action controls, and good rendering potential. The Daz EULA distinguishes broad 2D render rights from interactive/3D use, protects redistributable content from extraction, restricts raw derivative distribution, and contains AI-related limits. A `.genesis` package containing editable raw Daz-derived geometry is not compatible with the default objective.

**Human Generator (HumGen3D)** is a useful Blender quality reference, but its official FAQ separates GPL add-on code from proprietary digital assets and prohibits sharing or selling readily extractable models/textures. It is not a portable source foundation.

**SMPL-X** is technically valuable research: a unified body, hands, and face model with learned correctives. Its official model license is for non-commercial scientific research and prohibits redistribution; commercial licensing requires separate contact. It is excluded from the default Genesis dependency closure.

## 6. Hybrid strategy

The selected hybrid has four layers:

```text
GenesisPerson / CharacterSpecification
        ↓
versioned semantic resolver + locks + constraints
        ↓
resolved Genesis identity parameters
        ↓
Genesis-maintained production asset and build pipeline
        ↓
quality-tier artifacts + metadata in a .genesis package
```

The production asset may be derived from explicit CC0/Apache sources, but Genesis controls its topology version, naming, UV policy, rig, correctives, morph composition, material definitions, groom interfaces, garment contracts, validation, and exports.

This creates an important substitution boundary. Anny may supply the first body basis, but a later backend can replace it if it cannot meet facial or deformation requirements. Existing persons remain defined semantically; a migration process can resolve them against a new backend version and report visual deltas.

The strategy does **not** mean combining every available mesh. The proof of concept must select one canonical topology path, then transfer or fit only cleared information to it. Uncontrolled topology conversion would destroy morph consistency and make expression, clothing, and LOD work unmanageable.

## 7. Candidate comparison matrix

Scores are relative to the Genesis goal: 1 = poor/high risk, 3 = usable with major work, 5 = strong. “License fit” includes raw editable redistribution in a general-purpose platform, not merely rendered-image or closed-game rights.

| Candidate | Current status | Close-quality potential | Deterministic parameterization | Automation | Portable raw redistribution / license fit | Genesis fit | Decision |
|---|---|---:|---:|---:|---:|---:|---|
| Original from zero | Genesis-controlled | 2 initially | 5 | 5 | 5 if all inputs are cleared | 3 | Not primary; anatomy/art cost is unbounded |
| Anny v0.6 | Active, 2026 | 3 as a base | 5 | 5 | 4 with clean dependency closure | 5 | Selected parameter/body seed |
| MakeHuman / MPFB 2.0.x | Active, 2026 releases | 2–3 | 5 | 5 | 4 for verified CC0 assets; code differs | 4 | Selected data/workflow source, not final renderer |
| Blender Human Base Meshes 1.4.1 | Current bundle, 2026 | 4 with expert work | 1 | 4 | 5 (CC0) | 4 | Selected topology/anatomy reference |
| CharMorph / MB-Lab | CharMorph active; MB-Lab archived | 2–3 | 4 | 4 | 2 due per-character/AGPL variation | 2 | Reject as uniform foundation |
| MetaHuman | Current commercial ecosystem | 5 | 5 | 3 outside Unreal | 2 / UNKNOWN for Genesis raw packages | 2 | Benchmark; custom-license fallback |
| Reallusion CC5 | Current commercial ecosystem | 4–5 | 5 | 4 | 1 under ordinary terms | 2 | Benchmark; enterprise fallback |
| Daz Genesis 9 | Current commercial ecosystem | 4 | 5 | 3 | 1 for editable raw packages | 2 | Reject as foundation |
| Human Generator | Current Blender product | 4 | 4 | 4 | 1 for extractable assets | 2 | Benchmark only |
| SMPL-X | Maintained research model | 3 as geometry | 5 | 5 | 1 under default non-commercial terms | 2 | Exclude without commercial license |
| **Selected hybrid** | Genesis-controlled process over cleared sources | **5 target** | **5** | **5** | **5 target, audited per artifact** | **5** | **Primary direction** |

The hybrid's quality score is a target to prove, not an assertion that the open source meshes already achieve it.

### Technical coverage detail

| Candidate | Topology, body, hands | Face, rig, expressions, visemes | Materials, hair, clothing | Automation and export | One-metre judgment |
|---|---|---|---|---|---|
| Original from zero | Completely controllable, but all anatomy/topology must be created and validated | Custom skeleton and morphs are feasible; quality data and correctives must be created | Completely custom | Excellent through Blender Python/headless builds; glTF designed directly | Unproven and highest convergence risk |
| Anny v0.6 | Common parametric topology; interpretable all-ages body; local changes; default repository rig is 104 bones; not a finished hands/feet art asset | Facial actions are distinct from identity, but a hero facial rig, visemes, eyes, teeth, and oral cavity still need production work | No complete hero skin/groom/wardrobe solution | Strong PyTorch API; Blender/export pipeline must be built and validated | Promising geometric base, insufficient alone |
| MakeHuman / MPFB | Stable hm08 quad topology and extensive body targets; helper/proxy cleanup is needed for delivery | Multiple rigs; current face-unit, expression, and viseme packs; final facial deformation quality needs replacement/refinement | Usable skin/procedural examples and large asset ecosystem, but quality and per-item license vary | Strong Blender scripting; export-copy workflow; glTF through Blender, with explicit morph/material preparation | Useful source data, visibly below target unchanged |
| Blender Human Base Meshes | Sculpt-ready quads, UV/UDIM, multiresolution and face sets; good anatomy/topology reference | No complete parametric identity system, production skeleton, expression set, or visemes | No finished portable human material/groom/wardrobe system | Excellent Blender automation; exports only what Genesis authors | Strong seed with expert work, not a system |
| CharMorph / MB-Lab | Depends on selected character; morphing and alternative topology support | Rigify/facial support varies by character and database | Assets vary by character and license | Blender add-on automation; Blender export path | Major work and inconsistent foundation |
| MetaHuman | High-detail standardized topology, body presets, hands, multiple LODs | Proprietary DNA/RigLogic, hundreds of semantic rig channels, strong expression system | High-end skin, eyes, hair, and wardrobe ecosystem | Unreal-centered authoring with documented DCC exports; not an engine-neutral source system | Reference-quality benchmark |
| Reallusion CC5 / Headshot 3 | Strong body/head morphing and production topology; vendor advertises extensive head morph coverage | Full facial profile, expressions, visemes, rig and engine integrations | Strong digital-human shader, hair and clothing ecosystem | Mature DCC/engine pipeline, but dependent on licensed proprietary content | Strong benchmark; rights mismatch |
| Daz Genesis 9 | Mature unified figure and morph ecosystem, detailed hands/body | Facial action controls and add-on morph ecosystem | Large proprietary material, hair, and clothing store | DCC/export workflows exist, but editable content remains license-constrained | Capable renders; rights mismatch |
| Human Generator | Production-oriented Blender humans and morphs | Product provides a rigged generated result; exact portable facial coverage must be evaluated per licensed version | Integrated proprietary skin, hair, and clothing content | Blender-integrated and scriptable in principle; raw results remain constrained | Strong visual benchmark |
| SMPL-X | 10,475-vertex unified body/hands/face research topology with learned pose correctives | 54-joint research model with face/hands; not a finished production face/performance rig | No complete photoreal material, eye, dental, hair, or clothing system | Strong Python/research automation; custom Blender/glTF work required | Geometry/fit reference, not a finished human |

“Supports glTF” is deliberately not a binary quality claim. A Blender or DCC export may carry mesh, skin, and morph data while losing procedural materials, solver behavior, strand hair, naming conventions, or legal permission to redistribute the result.

## 8. Licensing and provenance matrix

| System/material | Tool or code license | Asset/model/output terms relevant to Genesis | Required Genesis treatment |
|---|---|---|---|
| Blender | GPL application | Blender does not make ordinary user-created output GPL merely by processing it | Offline authoring tool; record exact version |
| Blender Human Base Meshes 1.4.1 | Not applicable to bundle | Official demo page labels the bundle CC0 | Derivatives permitted; retain voluntary provenance and hashes |
| Anny v0.6 | Apache-2.0 | Repository identifies CC0 MPFB/face-unit assets and Apache-2.0 components; optional sources can have other terms | Preserve Apache license/NOTICE as applicable; pin and audit every fetched file; exclude optional SMPL-X |
| MPFB 2.x | GPL | Core assets are CC0; third-party repository assets may be CC0 or CC-BY | Keep code external unless GPL integration is intentional; whitelist assets and preserve CC-BY attribution where used |
| MakeHuman application | AGPL | Core assets are CC0 according to official license page | Do not equate application license with every asset; audit source asset record |
| CharMorph | GPLv3 | Per-character licenses vary | No blanket use; inspect each character database |
| MB-Lab character data through CharMorph | AGPLv3 per CharMorph documentation | Documentation says generated meshes inherit AGPL | Do not use in the default portable master |
| MetaHuman | Unreal Engine EULA | Any-engine use is advertised, but proprietary DNA/RigLogic, commercial terms, AI/database restrictions, and raw platform redistribution remain material | Benchmark only; require counsel and written/custom rights before editable `.genesis` distribution |
| Reallusion CC/Headshot | Proprietary | EULA restricts raw model redistribution, character-generator integration, and AI/synthetic-data uses; enterprise path exists | Benchmark only unless an enterprise agreement expressly covers Genesis |
| Daz Genesis 9/content | Proprietary content EULA | 2D renders differ from interactive/raw 3D rights; extraction and derivative redistribution restrictions apply | Do not place raw editable Daz-derived content in `.genesis` |
| Human Generator | GPL add-on code | Separate proprietary digital-asset terms prohibit sharing/selling readily extractable assets | Benchmark only |
| SMPL-X model | Non-commercial scientific research license | Default model terms prohibit commercial use and redistribution; separate commercial licensing available | Exclude from default implementation and from Anny optional topology |
| Genesis-authored geometry/materials | Genesis-selected | Only as clean as their references, scans, textures, generated inputs, and contributor agreements | Record creation method, references, model/tool versions, and rights; do not label “original” if derived |

CC0 removes copyright conditions to the extent legally possible; it does not guarantee trademark, privacy, publicity, likeness, patent, or jurisdiction-specific rights. A future reference-image workflow must separately establish the user's authority to create and distribute the likeness.

No candidate receives blanket clearance for add-on content. For Anny and MPFB, only repository/distribution files whose own notices are on the allowlist are eligible; downloaded community textures, hair, and clothing retain their individual CC0, CC-BY, or other terms. The Blender CC0 bundle clears that bundle, not unrelated textures or grooms. CharMorph's geometry terms vary by character. MetaHuman, Reallusion, Daz, and Human Generator materials, hair, and clothing remain subject to their respective proprietary ecosystems even when the generator code or host application has an open-source license. Therefore the POC uses Genesis-created or explicitly allowlisted surface, groom, and garment inputs only.

## 9. Agentic Blender feasibility

The recommended pipeline should use Blender as a deterministic build machine as well as an artist-facing DCC:

1. Read a versioned resolved-identity document.
2. Reconstruct the canonical neutral person from a known source revision.
3. Apply global body, regional face, asymmetry, and surface parameters in a fixed dependency order.
4. Solve joint placement, attachment fit, and approved correctives.
5. Generate or select material instances, groom tier, garments, and body masks.
6. Validate topology, UVs, weights, morph locality, collisions, measurements, names, and provenance.
7. Render a fixed regression contact sheet and animation tests.
8. Export tiered artifacts; import them into clean Blender and Godot processes for round-trip validation.
9. Write a build report containing input hash, tool versions, warnings, render metrics, and output hashes.

This is feasible because Blender officially supports background rendering and scripting. The limitation is the feedback signal: numerical validation can reject impossible or broken humans but cannot certify that a face feels alive. Critical visual gates therefore require fixed photographic/anatomical references and at least three independent human reviewers who do not know which iteration they are seeing.

Machine-vision scores may assist triage, but they must not be the sole acceptance test. A score trained on portrait images can miss eyelid intersections, poor deformation, unstable identity, or topology that only works from one camera.

## 10. Required master-human topology, rig, morph, and material architecture

### Canonical topology

- One neutral, metric, canonical topology per major generation of the backend.
- Quad-dominant authoring mesh with clean loops around eyelids, mouth, nasolabial region, ears, shoulders, elbows, wrists, fingers, hips, knees, ankles, and toes.
- Separate meshes for cornea/eyeball, tear line, teeth, gums/tongue/oral cavity, nails when useful, hair/grooms, and garments. Interfaces and scale are versioned.
- Stable vertex order for identity targets, facial targets, corrective shapes, reprojection, and compatible garments.
- Authoring subdivision/multiresolution detail is baked to delivery normals/displacement; runtime meshes are triangulated deterministically.
- Topology version changes are migrations, never silent substitutions.

The final density must be driven by silhouette, deformation, and displacement tests. Initial delivery budgets are in Section 14; a high-resolution sculpt can be much denser because it is an authoring source, not a runtime payload.

### UV and material policy

- Author at high resolution with a stable UDIM layout if it materially improves head/body work.
- Bake consumer tiers into ordinary 0–1 texture sets unless a target explicitly supports UDIMs.
- Required portable PBR set: base colour, normal, roughness, and packed occlusion/other masks as appropriate.
- Hero source may additionally retain displacement, micro-normal, cavity, thickness/transmission, subsurface-control, pore/wrinkle, and region masks.
- Skin tone must be parameterized through physically plausible melanin/undertone/vascular/freckle/tan inputs, not a single RGB tint.
- Eyes need separate cornea, wetness/tear line, sclera/iris, limbal and pupil controls. Teeth and oral tissue require their own material response.
- Procedural Blender shader graphs are authoring assets. They must be baked or rebuilt per renderer; their presence in a `.blend` file does not make them portable.

### Body skeleton

Do not select a skeleton by an arbitrary bone count. Define semantic roles and produce target-specific retarget maps. The canonical deformation skeleton needs:

- root and pelvis;
- a tested multi-segment spine, chest, neck, and head chain;
- clavicle/scapular support;
- upper/lower limbs with forearm and upper-arm twist distribution;
- wrist support and complete three-segment finger chains plus thumb;
- thigh/calf twist support;
- ankle, ball, and toe controls;
- jaw, eyes, and optional tongue/teeth controls;
- additional deformation/helper joints only where tests prove their value.

A plausible production result will often be roughly 100–160 deforming joints when hands and facial helpers are included, but the role contract and deformation quality matter more than the count. A reduced game/crowd skeleton is derived by a documented mapping.

### Face and performance rig

- Use FACS-oriented action units as the conceptual basis, with an ARKit-compatible subset/mapping for broad capture interoperability.
- Plan for roughly 50–80 portable facial performance channels plus combination correctives, not a promise that 52 named shapes solve the face.
- Separate gaze, pupil, blink, jaw, tongue, lip seal, and viseme controls.
- Preserve skin volume and sliding through sculpted correctives and, in the authoring rig, joint/RBF/driver logic where necessary.
- Bake complex solver results into morph weights or animation for simple consumers.
- Include at least a practical phoneme/viseme set; support mapping from Microsoft/Meta-style sets without making either vendor's naming authoritative.

### Identity morph system

The user surface should expose approximately 30–60 understandable controls at first, but those should resolve into a richer basis—initially on the order of 150–300 regional/global identity dimensions, plus nonlinear correctives. Counts are hypotheses to validate, not public API promises.

Identity dimensions should cover:

- stature, limb/torso proportions, shoulder/pelvis geometry, hand/foot scale;
- lean/fat/muscle distribution by region rather than one “weight” scalar;
- cranial and mandibular proportions;
- brow, orbit, eyelid, eye placement, nose, cheek, mouth/lip, chin, jaw, ear, and neck regions;
- apparent age with coherent shape and material effects;
- bounded asymmetry and distinctive local traits;
- skin, eye, hair, brow, lash, and facial-hair appearance;
- modular hair and garment selections.

Linear morphs are useful locally, but age, body composition, and interacting face proportions require constrained nonlinear mapping and combination correctives. A semantic control should never directly expose an upstream vendor's opaque coefficient.

### Deformation correctives

Prioritize shoulders/clavicles, elbows, forearms/wrists, hips, knees, ankles, neck, jaw, eyelids, lip seal, cheeks, and extreme combinations. Correctives belong to performance unless they compensate for an identity-specific shape; identity-specific deltas must be reproducibly derived.

### Hair and clothing

- Keep scalp, hairline, brows, lashes, beard, and moustache as independently replaceable modules with explicit fit contracts.
- Retain curve/strand grooms for authoring/hero use and generate cards or other target-appropriate representations for real-time tiers.
- A hairstyle change must not alter face or body geometry hashes.
- Garments need their own topology, weights, correctives, body-occlusion masks, material variants, and supported identity envelope.
- Do not promise arbitrary clothing compatibility in the first master generation; validate a bounded garment contract.

## 11. Parametric identity strategy

Store identity at three levels:

1. **Semantic intent:** stable, user-facing values in `CharacterSpecification`, expressed in meaningful units or normalized documented ranges.
2. **Resolved identity:** a backend-versioned, deterministic vector plus derived measurements, selected modules, constraint outcomes, and a hash.
3. **Built artifacts:** meshes, morph targets, textures, grooms, rigs, and LODs generated from the resolved identity.

Only level 1 is authoritative. Level 2 makes builds reproducible and debuggable. Level 3 is replaceable cache/delivery data.

Identity mapping should be a directed, versioned graph rather than a bag of independent sliders. For example, `apparent_age` may affect proportions, fat distribution, facial soft tissue, roughness, pigmentation variation, and wrinkle masks, while a nose edit affects only the nose region and required transition/corrective zones. Constraints resolve invalid combinations predictably and report clamping.

To avoid the “same base person” problem:

- sample or author coherent identity presets across the full latent basis, not isolated features;
- include regional proportions and spatial relationships, not just feature size;
- include controlled asymmetry and surface identity;
- validate identity distance using landmarks, silhouettes, texture/material parameters, and blinded recognition tests;
- keep protected identity anchors stable during hair, clothing, pose, and performance changes.

Performance state lives in a separate document or runtime channel. A smile may activate performance morphs and dynamic wrinkle maps but may not write back into the identity vector.

## 12. `CharacterSpecification` mapping strategy

The existing type should evolve additively before production implementation. Recommended future concepts are:

- versioned namespaces such as `body.stature`, `body.composition`, `face.nose.width`, `appearance.skin.melanin`, and `hair.style`;
- explicit units, domains, defaults, and interpolation semantics;
- category/region membership so locks apply mechanically;
- persistent selections for modular features;
- separate apparent age from chronological biography where relevant;
- bounded asymmetry controls;
- a `mappingProfile`/backend version and resolved-vector hash in build metadata;
- migration functions between specification and backend versions.

The flow remains:

```text
user request
  → Character Director proposes a semantic edit plan
  → existing transaction/lock validation accepts or rejects it
  → CharacterSpecification changes
  → versioned mapper resolves production parameters
  → backend rebuilds only affected artifacts
```

`preserve_face` is implemented by locking semantic face categories and verifying protected face landmarks/parameters after the solve. `undo` restores the earlier specification; it does not attempt to reverse sculpt operations. This preserves the successful Milestone 1 design.

## 13. Portability and export strategy

### Baseline delivery

Use glTF 2.0/GLB for portable runtime delivery. The official specification supports triangle meshes, PBR metallic-roughness materials, linear-blend skins, joint hierarchies, morph targets, and animation of node transforms and morph weights. Godot recommends glTF 2.0 and imports skins, animations, PBR data, and blend shapes.

This does not mean one GLB contains the whole person. Core glTF does not represent Genesis's semantic solver, nonlinear identity graph, RBF logic, procedural skin shader, UDIM authoring layout, strand-groom authoring data, sculpt layers, license records, or category locks. Morph names are commonly carried through `mesh.extras.targetNames`, which is a convention rather than a core named-target facility. Consumers also differ in supported morph counts, joint influences, extensions, and shader features.

### Proposed `.genesis` contents

The portable package should eventually contain:

- the authoritative `person.json` / semantic `CharacterSpecification`;
- a package manifest with schema, backend, topology, rig, mapping, and exporter versions;
- an asset-provenance ledger: source URL, source version, license identifier/text reference, hash, transformation, and attribution;
- one or more named delivery profiles, such as `hero`, `game-close`, `medium`, `distant`, and `crowd`;
- GLB or glTF plus textures for each included delivery profile;
- skeleton-role map, rest-pose/axis/unit convention, morph semantic map, material semantic map, and supported animation/viseme mappings;
- thumbnails/contact-sheet renders and a machine-readable validation report;
- optional animation clips needed to validate or preview the person.

### Authoring-only material

Keep these in the controlled Genesis authoring workspace or a separately licensed archival bundle by default:

- Blender scenes and add-ons;
- high-resolution sculpt/multiresolution layers;
- UDIM working textures and baking scenes;
- source curve/strand grooms;
- solver graphs, RBF drivers, retopology cages, and intermediate caches;
- upstream tools or assets whose licenses do not permit redistribution.

OpenUSD may later be evaluated as a richer authoring interchange, but it is not required for the first proof. GLB remains the delivery baseline because it exercises the cross-engine constraints Genesis actually needs.

### Export rules

- metres, documented axes, deterministic A-pose rest, stable bone and morph names;
- no unsupported Blender procedural nodes in portable profiles;
- no more than four skin influences per vertex in the broad-compatibility profile; an optional eight-influence profile may be tested where supported;
- morph normals/tangents exported and verified where target importers support them;
- KTX2/Basis Universal considered for transport and GPU-friendly compression, with PNG/lossless masters retained in authoring;
- run Khronos validation plus clean Blender and Godot imports on every build;
- compare counts, bounds, transforms, material slots, rest pose, morph deltas, animation clips, and visual renders after import.

## 14. Performance strategy

The following are initial measurement targets, not permanent API limits. Triangle counts exclude garments and hair unless noted.

| Tier | Initial mesh target | Texture target | Rig/morph policy | Hair policy | Intended use |
|---|---:|---|---|---|---|
| Authoring / sculpt source | Millions after subdivision | UDIM, 8K-capable masters | Full controls and solver graph | Strand/curve groom | Offline authoring and baking only |
| Hero delivery / render LOD0 | 120k–200k triangles | Up to 8K head and 4K body sets when justified | Full deformation, face shapes, correctives | Curves/strands or dense cards | One-person Genesis/film-quality view |
| Game close | 60k–120k | 4K head, 2K–4K body | Full body; selected face set and correctives | High-quality cards or supported strands | Conversational camera |
| Game medium | 25k–60k | 2K sets | Reduced face and helper joints | Reduced cards | Several visible people |
| Game distant | 8k–20k | 1K atlas/set | Reduced skeleton; little/no facial morphing | Simple cards/caps | Background action |
| Crowd | 1k–8k or impostor | 0.5K–1K atlas | Minimal skeleton or baked motion | Simplified/atlas | Large populations |

These ranges must be adjusted from captured frame timings and visual deltas. The hero tier must not be forced into a crowd budget.

Texture memory is likely to dominate. One uncompressed RGBA8 4K texture is 64 MiB before mipmaps and about 85 MiB with a full mip chain; one 8K texture is 256 MiB before mipmaps and about 341 MiB with mips. Several head/body maps can therefore consume gigabytes without GPU block compression. KTX2/Basis can improve transport and enable device-appropriate GPU compression, but image file size is not the same as resident VRAM.

Morph cost scales with active/available targets and engine implementation; skeletal cost scales with vertices, influences, and draw calls. Hair transparency/overdraw and multilayer skin/eye shaders can cost more than the body mesh. The RTX 3080 Ti Laptop GPU with 16 GB VRAM is adequate for a one-human proof, but it should not be used to justify unbounded texture or groom growth.

For the proof, benchmark Hero visually and benchmark Game Close in Godot at 2560×1440. Record median, 95th-percentile, and worst GPU/CPU frame times, peak incremental VRAM, draw calls, triangles, active morphs, skin influences, material count, and package/load size.

## 15. Photorealism risks

Highest-risk areas, in approximate order:

1. eyes, lid thickness/contact, tear line, gaze, and wetness;
2. lip volume/seal, nostrils, teeth, gums, tongue, and oral darkness;
3. facial identity diversity without implausible morph combinations;
4. expression skin sliding and preservation of volume;
5. hands, fingers, nails, feet, ears, and hairline under close inspection;
6. skin microdetail, subsurface response, roughness breakup, peach fuzz, and age coherence;
7. shoulder/hip/wrist deformation and clothing/body intersections;
8. groom silhouette, parting, roots, brows, lashes, and facial hair;
9. differences between the Blender reference render and portable engine shaders;
10. an evaluation loop that overfits fixed cameras or one attractive identity.

The POC deliberately includes side and three-quarter face views, hands, movement, and a one-metre camera to expose these weaknesses.

## 16. Licensing risks

- Automated installers can silently add assets with different terms; no network-fetched dependency enters the build without a manifest diff.
- “Open-source generator” says nothing about the output assets. Code, model weights, base meshes, textures, grooms, clothing, and generated output terms are separate records.
- CC-BY garments or hair create attribution obligations even if the base body is CC0.
- GPL/AGPL code integration is different from using a tool offline. Copying or linking code into Genesis requires a separate design decision.
- Anny's optional SMPL-X path can contaminate an otherwise permissive experiment if enabled casually.
- A commercial tool may grant use in a game but forbid raw editable redistribution or use in another character generator.
- Generative image/model tools introduce training-data and output-rights questions. Use them only when their terms and the specific source inputs are recorded and acceptable; otherwise mark provenance UNKNOWN and exclude the result.
- Real-person likeness creation adds consent, privacy, biometric, and publicity considerations independent of copyright.
- Upstream terms can change. Pin license text alongside the exact source version used; do not rely on a mutable web page at release time.

## 17. Technical blockers

There is no blocker in the current Genesis Core design. The unresolved production blockers are:

1. **Canonical topology selection:** prove that one cleared topology can support body range, close face, facial actions, hand/foot detail, clothing, and LOD transfer.
2. **Photoreal surface and facial work:** no reviewed open system supplies a finished hero-quality result.
3. **Identity basis richness:** Anny/MPFB parameters must be tested for facial distinctiveness and localized edits; custom targets will almost certainly be required.
4. **Nonlinear solve and correctives:** define deterministic interaction order without identity drift.
5. **Groom and garment contracts:** prove fit across the tested identity envelope.
6. **Portable shader parity:** bake or approximate hero skin, eye, and hair response across Blender and Godot.
7. **Legal dependency closure:** verify every file actually loaded by the POC and prevent optional/restricted assets from entering it.

These are exactly the questions the next small milestone should answer.

## 18. Recommended strategy

Adopt the following production rule:

> Genesis owns the semantic contract and original mapping/build code, and controls and versions the production asset revisions, validation, and packages. It uses only enumerated, provenance-cleared upstream inputs and never represents a derivative asset as wholly original. The first parameter/body seed is Anny v0.6 plus cleared CC0 MPFB data, with Blender's CC0 human bases and production research as topology/anatomy references. Genesis supplies the quality layer—canonical topology decision, custom facial identity shapes, rig, correctives, materials, grooms, garments, LODs, and exports.

The POC must begin with a dependency allowlist. “Use Anny” is not sufficient; the allowlist names the exact repository commit/release, files, URLs, hashes, and licenses and confirms that no SMPL-X or other non-commercial material is present.

The production topology should be chosen once the POC compares two bounded options: retain/finalize the cleared Anny/MakeHuman-derived topology, or transfer the parameter basis to a Genesis-maintained topology derived from a CC0 Blender base. Choose the option that best passes close-face deformation and transfer stability. This is one experiment inside the selected hybrid direction, not indecision between strategies.

## 19. Why the alternatives were not selected

**Original from zero:** maximizes control but puts the largest uncertainty—human anatomy and artistic quality—on the least proven capability. The agentic pipeline remains essential, but it needs high-quality cleared source data and expert review.

**Adopt Anny unchanged:** excellent deterministic body parameterization, insufficient finished face/surface/groom/clothing quality.

**Adopt MPFB unchanged:** excellent targets and automation, but default topology/material/render quality does not establish the one-metre bar, and mixed community-asset licenses require care.

**Adopt Blender base meshes unchanged:** clean, useful geometry, but no generator, canonical identity basis, finished rig, or production appearance system.

**Adopt CharMorph/MB-Lab:** mixed per-character licensing and AGPL inheritance create avoidable package complexity; visual quality still requires major work.

**Adopt MetaHuman:** high quality but proprietary DNA/RigLogic and Unreal dependence weaken platform independence. Reviewed terms do not establish the raw editable redistribution Genesis needs, and AI restrictions are material to the Character Director roadmap.

**Adopt Reallusion, Daz, or Human Generator:** good content ecosystems, but ordinary licenses are designed for rendered or embedded end products, not redistribution inside a competing general character-creation platform.

**Adopt SMPL-X:** technically attractive but default model terms are non-commercial and prohibit redistribution.

## 20. Concrete proof-of-concept milestone

### Name

**Master Human POC-1: one identity, one topology, one portable close-view build**

### Strict scope

- one adult human identity in neutral A-pose;
- one production topology path chosen from the two cleared hybrid options;
- eyes, lids, tear line, teeth, gums/tongue/basic oral cavity, ears, hands/fingers/nails, and feet;
- one hairstyle with brows and lashes;
- one simple fitted shirt and trousers with body masking;
- one hero material setup and one baked Game Close setup;
- one body skeleton, tested twist/deformation support, jaw and eye controls;
- a minimal facial set: left/right blink, jaw open, lip seal, smile, frown, brow raise, eye look, and at least six useful speech visemes;
- only the identity controls needed to test stature/body proportions, body build, nose/face proportions, apparent age, skin/material parameters, and hair selection;
- fixed Blender render suite and Godot import/performance scene;
- no general clothing library, child/elder extremes, mass crowd system, broad hairstyle catalog, capture system, or conversational LLM.

### Work packages

1. **License gate:** pin source versions; archive license texts and notices outside distributable artifacts as appropriate; hash and allowlist every dependency; prove the build contains no optional SMPL-X/non-commercial asset.
2. **Topology bake-off:** fit the same neutral identity and required face shapes to the retained cleared topology and the CC0-derived Genesis-maintained alternative. Compare eyelids/lips, shoulder/hand loops, shape transfer, UVs, and export. Select one before further art work.
3. **Anatomy pass:** complete close-view geometry and validate measurements, silhouettes, eye/lid contact, mouth interior, hands, and feet.
4. **Identity test:** produce the baseline plus at least five deterministic semantic edits: taller; less muscular; narrower nose/changed face proportions; modestly older; changed skin parameters; changed hairstyle. The final two can be combined only if all requested categories are still independently tested.
5. **Rig/deformation pass:** neutral, arm overhead, elbow 120°, wrist rotation, fist, hip flexion, knee bend, ankle/toe pose, head turn, jaw open, blink, smile, and viseme sequence.
6. **Look-development pass:** fixed neutral lighting plus hard grazing light; full body and front/side/three-quarter face; eye, mouth, hand, and hair crops; close face at a measured one-metre camera distance.
7. **Portability pass:** export Game Close GLB; validate it; import into a clean Blender file and Godot; compare rest pose, morphs, skeleton, textures, animations, bounds, and reference renders.
8. **Package pass:** create an experimental `.genesis` package containing semantic source, resolved metadata, Game Close artifact, provenance, and validation report. This is a POC artifact, not a format-breaking production commitment.

Only two bounded visual/technical refinement cycles are allowed after the first complete render suite. The review then applies the gates below.

## 21. STOP / GO criteria

### Scoring rubric

For visual review, three independent reviewers score each area from 0 to 4 using shuffled, unlabeled renders:

- **4:** convincing at target distance; no defect noticed without close inspection;
- **3:** credible; small defect does not break the person;
- **2:** visibly synthetic or distracting;
- **1:** major anatomical/material failure;
- **0:** absent or broken.

Critical areas are overall face, identity distinctiveness, eyes/lids, lips/mouth/nostrils, ears, hands/fingers/nails, skin, hairline/groom, body anatomy, facial expression, and posed deformation.

### GO — all must pass

1. **Legal closure:** every incorporated file has an approved source, version, hash, license, and transformation; required notices/attribution are generated; no restricted/UNKNOWN input is in the distributable POC.
2. **Visual bar:** median reviewer score is at least 3 for every critical area and no critical area receives more than one score below 3. Front, side, three-quarter, grazing-light, expression, and one-metre views all count.
3. **Identity persistence:** in a blinded matching task across neutral, smile, viseme, body edit, hair change, and clothing/pose views, reviewers match the person to the baseline at least 90% of the time and report no identity-changing artifact.
4. **Meaningful variation:** the five semantic edits are visibly distinct in their intended region. Reviewers can identify the requested direction at least 80% of the time without being told which output is which.
5. **Locality and locks:** a hair edit leaves protected face/body geometry byte-identical; a body-only edit leaves protected facial landmark positions within 0.5 mm RMS and face material parameters unchanged; a regional nose edit leaves the body unchanged and non-transition facial landmarks within 0.25 mm RMS. Every deliberate exception is declared by the mapping profile.
6. **Determinism:** repeated builds with identical pinned inputs on the same environment produce identical resolved-parameter and geometry hashes. Render pixels may use an explicit tolerance; cross-machine floating-point differences must not exceed 0.05 mm at any vertex or alter topology/order.
7. **Geometry health:** no unexpected non-manifold edges, inverted/zero-area faces, missing UVs, unnormalized weights, invalid transforms, duplicate required names, or morph vertex-count/order changes. Intentional seams and overlaps are allowlisted.
8. **Deformation:** all required poses and face actions have no mesh collapse, eye/teeth exposure error, lip-seal failure, visible body/garment penetration, or volume loss scored below 3. Automated collision and weight checks also pass.
9. **Portability:** Khronos validation succeeds; clean Blender and Godot imports preserve hierarchy, rest pose, required morph names/counts, animations, material/texture assignments, scale, and facing. Imported reference renders score at least 3 and show no identity change.
10. **Game Close performance:** on the stated RTX 3080 Ti Laptop/16 GB system at 2560×1440, the isolated Godot test sustains a 95th-percentile frame time at or below 16.7 ms after warm-up and adds no more than 4 GB peak VRAM. Measurements and quality settings are recorded rather than tuned invisibly.
11. **Rebuildability:** a clean scripted build can recreate the POC from the approved local inputs without manual mesh edits hidden outside source control/artifact storage, and emits a complete validation/provenance report.

### STOP — any one is sufficient after the two refinement cycles

- a critical visual area remains at median score 2 or lower, especially eyes/lids, mouth, hands, or deformation;
- the person only looks credible in one camera/lighting setup or loses identity during expression;
- semantic edits require destructive remeshing, untracked manual repair, or broad unintended changes;
- the chosen topology cannot support the required facial/body range and shape transfer without instability;
- a necessary source asset has incompatible, UNKNOWN, or non-redistributable terms;
- GLB/Godot cannot preserve the Game Close person without material identity loss or broken rig/morph behavior;
- deterministic rebuild/locality requirements cannot be met;
- performance can pass only by reducing the one-metre view below a reviewer score of 3.

### Fallback after STOP

First identify the failed layer. If the semantic pipeline, rig, exports, and license system pass but the open-source visual base fails, pause art expansion and seek a written enterprise/custom agreement from Reallusion or Epic that explicitly permits editable character-generation and `.genesis` redistribution. If the Anny parameter basis fails but the cleared topology and look development pass, keep the hybrid architecture and author/commission a richer Genesis morph library over the CC0-derived topology. Do not discard the Milestone 1 semantic architecture and do not continue polishing indefinitely without a new source-quality hypothesis.

## Sources and claim ledger

All links below were reviewed on 2026-09-19. Dates in descriptions are publication/release dates shown by the source where available.

### Anny

- [NAVER Anny repository](https://github.com/naver/anny) — v0.6 release/status; parameters, topology/rig options, included asset notices, optional dependency warning, and Apache-2.0 project license.
- [NAVER Labs: “Anny: a free-to-use 3D human parametric model for all ages”](https://europe.naverlabs.com/blog/anny-a-free-to-use-3d-human-parametric-model-for-all-ages/) — 2025-11-06 project overview; scan-free design, interpretable controls, all-ages intent, and Apache-2.0 release statement.
- [Anny research paper](https://arxiv.org/abs/2511.03589) — model method, continuous/interpretable human shape, scan-free construction, and evaluation context.
- [NAVER Labs code index](https://europe.naverlabs.com/research/code/) — official project/code listing.

### MakeHuman and MPFB

- [MakeHuman licensing](https://static.makehumancommunity.org/about/license.html) — separation of MPFB GPL code, MakeHuman AGPL code, and CC0 core assets.
- [MPFB repository](https://github.com/makehumancommunity/mpfb2) — active source repository and supported Blender baseline.
- [MPFB code license](https://github.com/makehumancommunity/mpfb2/blob/master/LICENSE.CODE.md) — GPL code terms.
- [MPFB asset license](https://github.com/makehumancommunity/mpfb2/blob/master/LICENSE.ASSETS.md) — CC0 asset terms for the identified distribution.
- [MPFB 2.0.16 release](https://static.makehumancommunity.org/mpfb/releases/release_2016.html) — 2026-06-13 release and expression/scripting changes.
- [MPFB 2.0.15 release](https://static.makehumancommunity.org/mpfb/releases/release_2015.html) — 2026-05-05 release and viseme/ARKit-related assets.
- [MPFB 2.0.17 release](https://static.makehumancommunity.org/mpfb/releases/release_2017.html) — 2026-07-22 current release; seeded controlled randomization, batch generation, and material-panel changes.
- [MakeHuman asset packs](https://static.makehumancommunity.org/assets/assetpacks.html) — current system packs and the distinction between system and third-party asset sources.
- [MPFB versus MakeHuman](https://static.makehumancommunity.org/mpfb/faq/differences_between_mpfb2_and_makehuman.html) — project relationship and shared base/assets.
- [MPFB materials overview](https://static.makehumancommunity.org/mpfb/docs/materials/overview.html) — Blender material capabilities.
- [MPFB exporting](https://static.makehumancommunity.org/mpfb/docs/exporting.html) — export, helper, rig, and material constraints.
- [MPFB export copy](https://static.makehumancommunity.org/mpfb/docs/exporting/export_copy.html) — facial units/viseme preparation and propagation to attachments.
- [MakeHuman base mesh](https://static.makehumancommunity.org/about/concepts/basemesh.html) — hm08 mesh/helper structure.
- [MakeHuman professional mesh topology](https://static.makehumancommunity.org/makehuman/docs/professional_mesh_topology.html) — topology intent and quad-face count.
- [MakeTarget target documentation](https://static.makehumancommunity.org/assets/creatingassets/maketarget/targets.html) — fixed-topology vertex-offset target system.
- [MakeHuman CC-BY asset FAQ](https://static.makehumancommunity.org/oldsite/faq/what_do_i_need_to_do_when_i_use_a_ccby_asset.html) — attribution implications for community assets.

### Blender

- [Blender demo files: Human Base Meshes](https://www.blender.org/download/demo-files/) — v1.4.1, 49 MB, CC0, updated 2026-01-20.
- [Blender 3.6 Human Base Mesh bundle notes](https://developer.blender.org/docs/release_notes/3.6/asset_bundles/) — quad topology, UV/UDIM, multiresolution, and face-set design.
- [Blender 4.0 bundle update archive](https://archive.blender.org/wiki/2024/wiki/Reference/Release_Notes/4.0/Asset_Bundles.html) — topology and base-mesh updates.
- [Blender Studio realistic-human research](https://studio.blender.org/training/realistic-human-research/) — production workflow overview.
- [Use of base meshes](https://studio.blender.org/training/realistic-human-research/use-of-base-meshes/) — base-mesh role and source-specific licensing warning.
- [Retopology](https://studio.blender.org/training/realistic-human-research/retopology/) — importance of topology lock and symmetry/asymmetry workflow.
- [Design sculpting](https://studio.blender.org/training/realistic-human-research/design-sculpting/) — anatomical/reference requirements and review process.
- [Reprojection and sculpt layers](https://studio.blender.org/training/realistic-human-research/reprojection-sculpt-layers/) — tertiary skin detail and sculpt-layer workflow.
- [What are FACS?](https://studio.blender.org/training/realistic-human-research/what-are-facs/) — action-unit basis and facial-shape complexity.
- [Creating facial shapes](https://studio.blender.org/training/realistic-human-research/creating-facial-shapes/) — facial volume, sliding, shape-key, and corrective workflow.
- [The advantage of age](https://studio.blender.org/training/realistic-human-research/the-advantage-of-age/) — age/scan/reference difficulty.
- [Blender command-line rendering](https://docs.blender.org/manual/en/latest/advanced/command_line/render.html) — supported background/headless render operation.

### CharMorph and MB-Lab

- [CharMorph repository](https://github.com/Upliner/CharMorph) — project status and implementation.
- [CharMorph license file](https://github.com/Upliner/CharMorph/blob/master/license.txt) — GPL code and per-character/model licensing explanation.
- [CharMorph documentation introduction](https://charmorph-docs.readthedocs.io/en/main/Introduction.html) — character-by-character CC0/CC-BY/AGPL matrix.
- [Blender Character Project downloads](https://blendercharacterproject.org/download.html) — CharMorph v0.4.2 release date.
- [MB-Lab releases](https://github.com/animate1978/MB-Lab/releases) — final release/history.
- [MB-Lab retirement discussion](https://github.com/animate1978/MB-Lab/discussions/410) — archived project and move toward CharMorph.

### Commercial systems and SMPL-X

- [MetaHuman licensing overview](https://www.metahuman.com/license?lang=en-US) — current any-engine statement and Unreal Engine license/revenue framing.
- [Unreal Engine EULA](https://www.unrealengine.com/eula/unreal) — current engine, seat/royalty, and MetaHuman/AI-related terms.
- [MetaHuman Creator](https://dev.epicgames.com/documentation/metahuman/metahuman-creator-in-unreal-engine) — current creator capabilities.
- [MetaHuman Creator export tool](https://dev.epicgames.com/documentation/metahuman/metahuman-creator-export-tool-in-unreal-engine) — DCC export contents and workflow.
- [MetaHuman DNA, rig definition, and RigLogic](https://dev.epicgames.com/documentation/metahuman/metahuman-dna-rig-definition-and-rig-operation) — proprietary DNA representation, LOD data, semantic channels, and rig operation.
- [MetaHuman Creator from DNA](https://dev.epicgames.com/documentation/metahuman/metahuman-creator-from-dna-tool-in-unreal-engine) — topology/UV/rig compatibility and editability constraints.
- [Reallusion Character Creator](https://www.reallusion.com/character-creator/) — current character, morph, shader, hair/clothing, and integration capabilities.
- [Reallusion Headshot](https://www.reallusion.com/character-creator/headshot/) — photo/mesh head-generation and morph claims.
- [Reallusion software EULA](https://www.reallusion.com/Content/EULA/AP/EULA_AP.htm) — topology/rig ownership, character-generator, AI/synthetic-data, and distribution restrictions.
- [Reallusion content licensing](https://www.reallusion.com/license/content.html) — standard/extended/enterprise usage distinctions.
- [Reallusion character resale FAQ](https://kb.reallusion.com/Purchase/53107/Can-I-sell-the-characters-I-create-with-Character-Creator) — 2025-08-27 guidance on marketplace/application/character-generator distribution.
- [Daz 3D EULA](https://www.daz3d.com/eula) — rendered-output, interactive-license, raw-content/derivative, extraction, and AI terms.
- [Daz Genesis 9 overview](https://delorean.daz3d.com/introducing-genesis-9) — Genesis 9 capabilities and facial-action claims.
- [Human Generator license FAQ](https://help.humgen3d.com/license) — GPL add-on versus proprietary asset terms and extractable-asset restrictions.
- [SMPL-X repository](https://github.com/vchoutas/smplx) — model structure and implementation.
- [SMPL-X model license](https://github.com/vchoutas/smplx/blob/main/LICENSE) — non-commercial scientific-research and redistribution limitations.

### Portability

- [glTF 2.0 specification](https://registry.khronos.org/glTF/specs/2.0/glTF-2.0.html) — meshes, skins, morph targets, PBR materials, animations, morph-name convention, and core limitations.
- [Khronos real-time asset creation guidelines](https://github.com/KhronosGroup/3DC-Asset-Creation/blob/main/asset-creation-guidelines/RealtimeAssetCreationGuidelines.md) — delivery-versus-authoring distinction, texture formats, PBR, and asset practices.
- [Khronos KTX artist guide](https://github.com/KhronosGroup/3D-Formats-Guidelines/blob/main/KTXArtistGuide.md) — KTX2/Basis transport and GPU-memory behavior.
- [Godot available 3D formats](https://docs.godotengine.org/en/stable/tutorials/assets_pipeline/importing_3d_scenes/available_formats.html) — glTF recommendation, GLB packaging, and blend-shape export guidance.
- [Godot 3D import configuration](https://docs.godotengine.org/en/stable/tutorials/assets_pipeline/importing_3d_scenes/import_configuration.html) — skeleton/skin, material, texture, custom-property, animation, and influence import behavior.
