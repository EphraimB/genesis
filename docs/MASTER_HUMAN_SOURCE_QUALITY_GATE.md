# Genesis Master Human Source-Quality Replacement Gate

Status: **DECISION — proceed to a commissioned-source qualification POC; do not refine POC-1 further**

Research cutoff and web access date: **2026-09-19**

Scope: source-quality strategy only; no asset acquisition, implementation, vendor contact, or legal opinion

## 1. Executive decision

**Decision we can make now:** Genesis should commission a new, professionally authored, **Genesis-owned production master human** under a source-delivery and rights-assignment contract. The commission should replace the artistic source layer behind the existing Master Human backend boundary; it should not replace `GenesisPerson`, `CharacterSpecification`, the semantic mapper, packaging, validation, or runtime architecture.

The immediate next step is not the full commission and not another broad human implementation. It is a paid **source-quality qualification wedge**: a deliberately small, contractually clean art test covering the highest-risk areas—head/neck/shoulders, complete eyes and mouth, a production hand, skin/look development, hairline/brows/lashes, a collar/garment boundary, a bounded facial-expression set, and shoulder/wrist correctives. The wedge must be supplied as editable Blender source plus portable exports and must pass the fixed POC-1 comparison suite before Genesis commissions a complete master.

This route wins because it is the only currently evidenced strategy that can simultaneously provide:

- one-metre close-inspection quality;
- local, deterministic parameters owned by Genesis;
- raw and editable redistribution in `.genesis` packages;
- engine and vendor independence;
- stable rights to create unlimited derivative identities; and
- a clean place behind the existing versioned backend.

**Reallusion and Epic are not approved source bases under their published ordinary terms.** Reallusion's published software terms expressly exclude character-generation systems and character-generating businesses, while its public Enterprise page only offers a case-by-case inquiry and retains a competition restriction. Epic's current public pages establish strong technical portability, but the served legal documents conflict: the MetaHuman license page says MetaHumans may be used with any engine, the ECLA change log says MetaHuman-specific restrictions were deleted, yet the currently served ECLA still labels downloaded MetaHuman Content UE-Only and restricts source-format and content-creation redistribution. Neither set of public terms clearly authorizes Genesis's actual product model. A signed custom agreement satisfying section 13 could change that conclusion; absent that agreement, the answer is **NO**.

**Recommended fallback:** if a qualified owned-source commission cannot meet the gate, seek a written Reallusion custom/Enterprise proposal first, using the exact rights checklist in section 13. Reallusion is the first vendor fallback because its current product has broad body/head morph semantics and explicit cross-DCC export, and its public policy expressly exposes an Enterprise inquiry path for application/service integration. This is only a negotiation priority, not a license approval. Epic/MetaHuman is the second vendor fallback and the preferred technical benchmark.

**UNKNOWN:** no public evidence establishes that either vendor will grant the required generator, editable redistribution, sublicensing, survival, and local-AI rights, or at what price. Genesis must not ingest either source while those points remain unknown.

## 2. POC-1 evidence being carried forward

POC-1 is a completed, successful engineering experiment and a failed visual-source experiment. The following results are retained:

- `GenesisPerson` and `CharacterSpecification` remain semantic authority.
- The versioned `MasterHumanMapper` deterministically resolves semantic identity into backend parameters.
- The production backend, scripted Blender build, rig/export path, GLB import, Godot preview, `.genesis` packaging, backward compatibility, provenance gate, automated validation, and performance gates worked.
- The Game Close artifact was a 3,202,540-byte GLB with 62 runtime mesh instances.
- On the recorded reference run at 2560×1440, p95 frame time was 7.083 ms and reported video memory was 74.9 MiB, passing the precommitted limits of 16.7 ms and 4,096 MiB.
- The backend boundary already allows the source implementation to change without making a vendor database or asset format the authority for persistent identity.

The following visual evidence is also retained without reinterpretation: overall face 2/5, identity distinctiveness 2/5, eyes/lids 1/5, lips/mouth/nostrils 2/5, ears 3/5, hands/fingers/nails 2/5, skin 2/5, hairline/groom 1/5, body anatomy 3/5, facial expression 2/5, and posed deformation 2/5. The single-evaluator result did not establish the required multi-reviewer median, but several critical scores at 1–2 independently triggered STOP.

The POC-1 asset is therefore a regression baseline and architecture fixture, not a production-art base that is entitled to further polishing.

## 3. Exact source-quality gaps

The failed hypothesis was not “can Genesis put a humanoid in Godot?” It was “can the tested cleared procedural source path reach close-inspection production quality?” The gaps are coupled:

| Layer | Observed failure | Required replacement capability |
|---|---|---|
| Face and identity | Generic planes and weak identity signal | Anatomically designed primary/secondary forms, identity-preserving shape basis, production facial topology |
| Eyes | Weak lids, tearline, lashes, and eye integration | Separate cornea/sclera/iris/tearline/occlusion structures, lid thickness/contact, wetness and animation-safe topology |
| Mouth | Weak lips, nostrils, and oral cavity | Lip volume/contact, mouth bag, gums/teeth/tongue, nostril depth, phoneme/expression-safe loops |
| Skin | Flat response and insufficient microdetail | Calibrated albedo/roughness/specular/normal or displacement, scale-aware detail, portable approximation of subsurface response |
| Hair | Cap-like groom and poor hairline | Designed scalp transition, brows/lashes/facial hair, and at least one portable card/mesh representation in addition to any native groom |
| Expression | Low-quality shapes and identity drift risk | FACS-informed shapes, combination behavior, wrinkle strategy, and neutral recovery |
| Deformation | Shoulder, elbow, wrist, hip, and knee weakness | Production weights plus pose-space correctives tested across body variation |
| Hands | Weak fingers and nails | Anatomical sculpt, nail beds, knuckles/palmar forms, joint loops, weights, and correctives |
| Clothing | Weak construction and boundaries | Real seams/thickness/closures, body clearance, drape, garment-specific deformation |
| Variation | Narrow and generic identity space | Localized, reproducible face/body/age parameters with documented safe ranges |

These are not independent texture swaps. Facial topology affects expressions; body topology affects weights and correctives; eye sockets affect eyes and lids; scalp topology affects the hairline; and garment fit depends on body shape and deformation. That coupling is decisive in the targeted-commission analysis.

## 4. Reallusion analysis

### Technical fit

Character Creator 5 is a serious technical candidate. Current official materials document realistic and stylized characters, head/body morphing, digital-human shaders, hair and clothing, facial rigs, subdivision/HD workflows, Headshot 3, and export toward Blender, Maya, Unreal, Unity, OBJ, and FBX. Headshot 3 advertises spline-based facial reshaping, texture reprojection, body matching, and 1,400+ head morphs. This is substantially closer to Genesis's semantic needs than a beautiful fixed scan.

The fit is not complete. Genesis would still need to map vendor sliders to stable Genesis semantics, control version drift, define portable approximations for shader/groom features, and determine which morphs, correctives, and proprietary runtime data may be exported and redistributed. Marketing interoperability is evidence of an authoring pipeline, not evidence of platform redistribution rights.

### What published terms clearly allow

Under the current published Software EULA and content policy, a normal license can be used as an authoring tool for images, video, games, XR, applications, and interactive projects within the stated license conditions. The Standard content license supports export to third-party DCCs. Finished characters may be embedded in ordinary applications and games under the relevant content licenses, and the policy describes commercial use and unlimited projects.

That is **Tier 2 — AUTHORING TOOL** use and, for an ordinary inseparable game/app character, a limited project-output use. It does not answer Genesis's use case.

### What published terms prohibit for Genesis

The current Software EULA states that Character Creator base topology and character rigs are Reallusion property. Its grant for original character creations is non-sublicensable, nontransferable, and revocable, and it expressly excludes use in a character-generation system, AI training/ML/synthetic data, a character-generating business, and sale of CC-base 3D models as third-party content assets.

The content policy likewise says special uses may not compete with Reallusion or distribute through similar 3D-model services. Its client/contractor FAQ permits game/app use of exports but excludes republishing for online stores, in-app purchases, and character creation. The Content EULA restricts raw redistribution, sublicensing, use in online or in-software character generation and APIs, and requires protected/proprietary delivery in the uses it permits. Reallusion's AI Actor terms add system-locking and non-exportability for that separate service.

Genesis is itself a character-generation platform whose primary output is an editable digital person. The ordinary terms are therefore an unambiguous **NO** for use as the Genesis production source.

### What would require written custom permission

The public Enterprise table mentions application/service integration, content resell or redistribution, and AI/ML by special arrangement. It does not publish the operative grant, price, sublicensing rights, survival rights, generator exception, or a waiver of the competition restriction. Enterprise availability is a lead, not permission.

A custom agreement would have to override, by name, the character-generation-system, character-generating-business, competing-service, raw/editable redistribution, proprietary-container/extractability, third-party marketplace, sublicensing, topology/rig ownership, and relevant AI restrictions. It would also need to grant all section 13 rights for the exact base, morph, material, hair, clothing, and plugin content used.

**UNKNOWN:** whether Reallusion offers such terms, whether third-party Marketplace contributors can be included, which CC5/Headshot/ActorMIXER data may ship, required runtime or protection measures, audit obligations, fees/royalties/minimums, and post-termination rights.

## 5. MetaHuman/Epic analysis

### Technical fit

MetaHuman is the strongest vendor technical benchmark for close-up faces. Current official documentation describes arbitrary-topology head/body conforming, standard MetaHuman topology and rig, editable unbaked materials, geometry/material/DNA export, hundreds of semantic RigLogic channels, LODs, and DCC round-tripping. MetaHuman 5.8 adds full-body mesh conversion, more explicit export controls, standalone DNA assets, Python-accessible face-model coefficients, and an open Devkit. OpenRigLogic and DNA libraries are published under MIT, which materially improves runtime inspection and integration outside Unreal.

The distinction between runtime code and character content is critical. MIT-licensed OpenRigLogic/DNA libraries do not place MetaHuman character meshes, textures, grooms, DNA instances, or Epic's model database under MIT. A portable runtime is not a redistribution license for its input data.

Technical limitations remain relevant even with permission: MetaHuman outputs adopt its topology/rig; its highest-fidelity hair and materials need portable substitutes; DNA/RigLogic integration is materially more complex than glTF morphs; and a subset of editor workflows still depends on Unreal and, in some paths, Epic services. The parameter basis is rich but must be proven sufficiently local and stable for commands such as “narrow the nose,” “lock the face,” and undo across versions.

### What published terms clearly allow

The MetaHuman license page currently says MetaHuman is under the standard Unreal Engine license and that MetaHumans can be used with any engine or creative software. The Unreal Engine EULA classifies user-developed character models and animations that contain no Engine Code as possible Non-Engine Products, including use with other engines. DCC export documentation confirms geometry, material, and DNA workflows.

Those statements support cross-engine production use. They do **not**, by themselves, grant Genesis the right to redistribute editable source content through a general-purpose generator.

### Current legal conflict and Genesis conclusion

The currently served Epic Content License Agreement says Licensed Content may generally be distributed to end users only in object code as an inseparable part of a project, allows source-format sharing only to project employees/contractors, prohibits stand-alone transfer, and restricts enabling third parties to use Licensed Content for public content creation. Its served MetaHuman Addendum still says downloaded MetaHuman Content is UE-Only.

At the same time, Epic's own ECLA change log says an update deleted MetaHuman-specific restrictions, and the current MetaHuman license page says any engine. These official pages are not internally coherent as served on the research date. Even if “UE-Only” is stale, the general source-format, stand-alone, sublicensing, and third-party content-creation restrictions still do not clearly authorize a `.genesis` package containing editable MetaHuman-derived geometry, textures, rig, morph/DNA data, and user creation rights.

The Unreal EULA also bars using MetaHuman characters, animation curves, or function-replicating renders to build/enhance databases or to train/test AI/ML systems. A local language model merely translating a user's words into deterministic, pre-existing parameters is not necessarily training, but Genesis must obtain written confirmation that this inference-only semantic-control use is permitted and must prohibit training on MetaHuman data unless separately granted.

Epic's legacy MetaHuman Creator EULA page says it applies to the cloud-based Creator associated with Unreal Engine 5.5 or earlier; MetaHuman Creator in Unreal Engine 5.6 and later is governed by the Unreal Engine EULA. Genesis must therefore record the tool/version and accepted terms for every evaluated artifact rather than mixing legacy and current grants.

Accordingly:

- cross-engine rendered or application use: **clearly contemplated**;
- DCC export for one's production: **technically documented**;
- editable source redistribution and sublicensing through a character generator: **not clearly granted; treat as prohibited pending writing**;
- AI training/database construction: **expressly restricted**;
- local, non-training semantic command interpretation: **UNKNOWN for Genesis's proposed product; require confirmation**.

Custom terms are possible in principle because Epic's licensing page invites custom-license discussions. No public terms establish that Epic will grant Genesis's required rights or at what price. Section 13 is the minimum request.

## 6. Commissioned Genesis-owned source analysis

### Required specialties

A production master is not one sculpting task. The work packages are:

1. anatomical design/sculpt and identity design;
2. production face/body/hand retopology and UV strategy;
3. eye, tearline, oral/dental, nail, and ear anatomy;
4. calibrated skin texture and look development;
5. scalp, hairline, brows, lashes, and portable groom representation;
6. FACS-informed facial shape sculpting and combination/corrective behavior;
7. body rigging, weights, and shoulder/wrist/hip/knee correctives;
8. garment pattern/construction, fit, thickness, drape, and deformation;
9. technical-art integration, LODs, export, and cross-engine validation; and
10. provenance and rights documentation for every input.

A senior character generalist may be able to create the qualification wedge or coordinate a full asset, but relying on one person to be expert in anatomical sculpt, facial rigging/FACS, skin lookdev, grooming, clothing, body deformation, and engine technical art is high risk. Blender Studio's own realistic-human workflow separates sculpting, retopology, shape/wrinkle work, clothing, baking, and iteration, while the Digital Emily production credits separate scanning, rigging, modeling, animation, rendering, tracking, production, and performance. The full commission should therefore be led by a senior character artist/technical director with specialist review or subcontracted expertise, even if one counterparty owns delivery.

### Contractual minimum

Before source files enter the Genesis repository or build system, the agreement and acceptance schedule must provide:

- work-made-for-hire treatment where legally effective **and** a present, worldwide assignment of all copyright and related transferable rights as the fallback;
- an exclusive, perpetual, irrevocable, transferable, sublicensable license for any right that cannot be assigned;
- delivery of native source, high/low meshes, topology, UVs, textures and source texture projects, displacement/normal data, grooms/cards, rigs, weights, morphs, correctives, garment sources, scripts, and export settings;
- the right to modify, retopologize, derive, combine, automate, create unlimited identities, and build new morphs/correctives;
- the right to distribute editable and runtime forms in `.genesis`, GLB/glTF, FBX, USD, Blender, and future formats, with sublicenses to end users and downstream applications;
- explicit permission for use in a character generator and for local AI-assisted semantic editing, without granting training rights unless Genesis intentionally buys them;
- a complete third-party-materials schedule; no undisclosed vendor bases, marketplace assets, stock textures, fonts, scans, or generated-AI inputs;
- source URL, creator, acquisition date, version, hash, exact license, and modification history for every approved external input;
- warranties of authorship, authority, non-infringement, and compliance with the approved-input schedule, plus a practical remedy/indemnity appropriate to the supplier;
- model/performer/reference/scan releases covering likeness, biometric/privacy, commercial modification, derived identities, and redistribution when any real person is used;
- moral-rights waiver/non-assert to the extent lawful, and comparable consents where waiver is unavailable;
- contractor/subcontractor invention and copyright assignments flowing to the contracting supplier and then Genesis;
- acceptance milestones tied to section 20, revision/remediation terms, confidentiality, portfolio/publicity approval, delivery escrow/backups, and no dependency on a supplier-hosted service.

This is an engineering procurement specification for later legal review, not legal advice.

## 7. Targeted commissioning analysis

Retaining the POC-1 CC0 topology has attractive rights and integration simplicity, but it is the wrong production constraint.

Blender Studio's realistic-human guidance says topology should be locked before downstream detailing and that changing it later wastes substantial work. Its MetaHuman template guidance independently emphasizes that semantically important loops—lids, folds, ears, and nasolabial areas—must be in the correct places, not merely nearby. POC-1's weaknesses in lids, mouth, expression, shoulders, knees, hands, and garment boundaries are precisely the areas controlled by topology, weights, and corrective design. Adding premium textures and sculpts to an insufficient deformation basis would preserve the bottleneck.

Therefore:

- retain the CC0 POC-1 mesh as a blockout, regression fixture, scale/reference aid, and provenance-safe prototype;
- do not require a commissioned artist to preserve its vertex layout or loops;
- permit the artist to replace topology, UVs, rig, and shape basis when justified by the acceptance tests;
- retain Genesis's semantic names and backend contract, not the old vertex indices;
- reuse any truly separable POC-1 code, tests, cameras, lighting, packaging, metrics, and parameter semantics.

A targeted commission remains useful only as the **qualification wedge** described in section 19. It is targeted by deliverable/risk, not by forcing the old topology into production. Commissioning only better eyes, textures, and hair on the current topology is rejected because it cannot credibly solve facial expression and body deformation together.

## 8. Newly discovered permissive-source analysis

The 2026 scan found useful permissive components, but no permissive complete source that closes the full POC-1 gap.

| Candidate | Current status and license | Tier | Rights relevant to Genesis | Quality/integration conclusion |
|---|---|---:|---|---|
| Blender Human Base Meshes v1.4.1 | Official Blender bundle, updated 2026-01-20, CC0 | 4 | Commercial derivative and raw redistribution permitted under CC0 | Excellent cleared sculpt/blockout base; not a finished rigged, textured, expressive close-up master. Already representative of the path POC-1 could not finish procedurally. |
| MakeHuman/MPFB core assets and official system packs | Active community project; core mesh/targets/assets and exports stated CC0; code is AGPL/GPL separately | 4 for verified CC0 assets; 2 for tools | Official FAQ expressly permits using base/targets in another character generator and selling/modifying exports | Strongest permissive semantic reservoir: body/face targets, eyes/teeth, hand targets, ARKit-style face units, clothes. Current asset quality and materials do not independently meet the one-metre bar. File-level license verification is still required. |
| USC ICT FaceKit “Light” | Public repository; repository LICENSE is MIT | 3–4 candidate, pending provenance review | MIT permits use, modification, distribution, sublicensing, and sale with notice | Materially better partial source: 26,719-vertex head/face/oral/eye topology, 100 PCA identity modes, 53 expression shapes. Light version lacks identity albedo modes and an FBX face rig; it is face-only. Participant/scan consent and file-level ownership should be confirmed before production ingestion despite the repository license. |
| Poly Haven | Active curated CC0 asset library; optional provenance attestations are offered | 4 for selected verified assets | Raw commercial redistribution and derivatives permitted under CC0 | Useful for generic studio HDRIs and non-human materials; no evidenced production eye/skin/oral/groom system. It can improve test environments, not solve the master human. |
| SMPL-X Body / SMPL-X model | A distributable body subset is CC BY 4.0, while the generator model/software download is restricted to non-commercial scientific research and underlying model technology is patent-noted | 1–3 depending artifact | Some posed body outputs may be shared with attribution; the identity/shape model needed for a commercial generator is not granted by the research license | Useful technical reference and possible custom-license subject, not a permissive Genesis generator source. It does not provide the required production skin/hair/clothing look. |
| FaceScape | Current dataset; non-commercial research, non-transferable, no redistribution | 1 | No commercial use or shipping | High-quality research reference only. |
| Digital Emily 2 / Wikihuman | Download page limits data/images to research and illustration, no transfer/redistribution, commercial use, or resale | 1 | No commercial or redistributable Genesis use | Valuable lookdev reference, legally unusable as source. A separate third-party GitHub scene has an MIT license, but that cannot enlarge rights in the underlying restricted Emily data. |
| MB-Lab | Original repository archived in 2024; GPL code and historical asset-license ambiguity | 1–2 | Copyleft/tool and asset provenance require file-level analysis | Not a cleaner or higher-quality source than MakeHuman/Blender; exclude from the production-source shortlist. |

Rights detail for each scanned candidate:

| Candidate | Commercial use | Derivatives | Raw/editable redistribution | Use in a character generator | Attribution/notice | Genesis integration difficulty |
|---|---|---|---|---|---|---|
| Blender Human Base Meshes v1.4.1 | Yes | Yes | Yes | Yes | None required by CC0 | Medium: production sculpt, retopology, rig, materials and variation still needed |
| MakeHuman/MPFB verified core assets | Yes | Yes | Yes | Expressly yes for CC0 assets | None required by CC0; keep provenance anyway | Medium: semantics are useful, but quality and final integration remain substantial |
| ICT FaceKit Light repository data | Yes under published MIT grant | Yes | Yes with MIT notice | Yes under the copyright grant, subject to unresolved scan/person/patent diligence | Preserve MIT copyright/license notice | High: face-only; reconcile PCA controls, topology, body seam, rig, materials and releases |
| Poly Haven selected assets | Yes | Yes | Yes | Yes for downloaded CC0 assets | None for assets; API service has separate credit/usage terms | Low for test HDRIs/materials, but negligible direct human-quality coverage |
| SMPL-X Body subset | Yes under CC BY 4.0 for the defined Body artifact | Yes under CC BY | Yes for that defined Body artifact | **No under the free research model/software license** for commercial shape generation | Attribution/change notice required | High: split grants, model dependency, patents noted, no production look |
| FaceScape | No | Research-only as licensed | No | No | Publication requirements do not cure commercial/redistribution bar | Prohibited for this product |
| Digital Emily 2/Wikihuman | No | Research/illustration only | No transfer or redistribution | No | Credit does not cure the restriction | Prohibited for this product |
| MB-Lab | **UNKNOWN per asset/file**; GPL applies to code | **UNKNOWN per asset/file** | **UNKNOWN per asset/file** | **UNKNOWN** | GPL/copyright notices and any asset terms apply | High and unjustified because the project is archived and cleaner alternatives exist |

**Permissive-source conclusion:** ICT FaceKit Light is the most important newly identified component because it may improve face topology, identity basis, and expressions without vendor lock-in. It does not solve skin, hair, body deformation, clothing, or a full-body rights chain, and its underlying capture/provenance record needs review. It should be offered to the commissioned supplier as an optional, pre-approved research/derivation input—not made the required canonical topology.

## 9. Component-by-component source-quality opportunities

| Component | Best cleared opportunity found | What it can solve | What remains missing / decision |
|---|---|---|---|
| Face/anatomy | ICT FaceKit Light (MIT candidate); Blender/MakeHuman CC0 references | Production-oriented face/oral/eye topology and identity/expression basis; anatomical blockouts | Commissioned artistic design, body integration, file-level provenance, skin and production rigging. Do not mandate a particular input. |
| Eyes | ICT FaceKit includes eyeballs, eye sockets, lacrimal, occlusion, and eyelash geometry; MakeHuman system assets include CC0 eyes | Structural reference/components | Production iris/sclera/cornea textures, lid contact, tear response, animation and lookdev still require commission. |
| Dental/mouth | ICT FaceKit includes mouth socket, gums, tongue, and teeth; MakeHuman has CC0 teeth | Starting topology/components | Bite design, textures, contact, phonemes, LODs, and integration require commission. |
| Skin/microdetail | MakeHuman CC0 skin packs; generic CC0 material libraries | Legally safe low/mid-quality references and non-skin support materials | No evidenced permissive source supplies production facial albedo/specular/displacement diversity with adequate provenance. Commission/capture/author this layer. |
| Hair/grooming | MakeHuman CC0 hair packs, described largely as low-poly/stylized | Prototyping and format tests | Commission hairline, brows, lashes, facial-hair capability, and dual native/portable representations. |
| Facial expressions/FACS | ICT FaceKit's 53 expressions; MakeHuman CC0 ARKit-style face-unit pack | A candidate semantic basis and regression inputs | Professional shape cleanup, combinations, wrinkles, jaw/lip/tongue coordination, and identity-preservation tests. |
| Deformation/correctives | MakeHuman CC0 arm/cheek target packs; Blender topology references | Reference and early semantic tests | Production shoulder/wrist/hip/knee weights and pose-space correctives must be commissioned on final topology. |
| Hands | Blender base meshes and MakeHuman CC0 hand targets | Anatomical blockout/variation reference | Final hand/nail sculpt, loops, weights, and correctives must be commissioned. |
| Clothing | MakeHuman CC0 garments; generic CC0 fabric materials | Pipeline and collision tests | Production patterning, seams, boundaries, thickness, body-variation fit and drape must be commissioned. |

No candidate may be copied into the master merely because the enclosing repository has an open license. Each actual file must enter the dependency allowlist with a stable URL/version, hash, author/owner, license text, scope (code versus asset/data), and any likeness/scan evidence.

## 10. Semantic-editability analysis

The semantic architecture should remain:

`GenesisPerson → CharacterSpecification → versioned semantic mapper → resolved identity → production human backend`

The source-quality layer must expose a documented, deterministic control basis. It need not use the same morph topology as POC-1, but it must support local controls with bounded cross-effects and versioned migration.

| Strategy | Semantic strengths | Semantic risks | Gate result |
|---|---|---|---|
| Genesis-owned commission | Genesis defines names, ranges, locality, combination rules, and corrective ownership; full reproducibility | Up-front design and art cost; diversity must be deliberately authored | Best fit |
| Reallusion custom | Extensive existing face/body/head morph ecosystem and randomization | Vendor slider semantics/versioning, proprietary bases, exportable-morph scope, and headless/programmatic access unknown | Technically strong, legally blocked without custom terms |
| MetaHuman custom | Rich face coefficients/RigLogic and standard topology; current Python coefficient access | Coefficients may not map cleanly to user concepts; database/AI restrictions and dependency/version risks | Strong benchmark, blocked without custom terms |
| Permissive components | MakeHuman targets and ICT PCA/expressions are inspectable and reproducible | Quality is uneven; PCA controls are not automatically intuitive/local; integration work is substantial | Useful inputs, not complete answer |
| Fixed hero scan | High neutral likeness | Poor identity breadth and semantic locality; scan rights and expression basis separate | Reject as primary source |

For every new source, the mapper must own semantic intent, including `nose_width`, `eye_spacing`, `apparent_age`, `body_muscularity`, hairstyle selection, face lock, undo, and stable random seeds. Vendor or artist channel names may be implementation detail. A source fails if two runs with the same specification do not produce equivalent resolved parameters and geometry/material fingerprints within documented deterministic tolerances.

“Local AI editing” should initially mean an LLM or parser converts natural language into validated `CharacterSpecification` operations. The model does not train on character data, directly deform meshes, or silently invent parameters. This separation makes edits reversible/auditable and avoids conflating semantic inference with restricted AI training.

## 11. Portability analysis

| Strategy | Blender | Godot | Unity | Unreal | GLB/morph/skeleton | Material/hair | Vendor absent |
|---|---|---|---|---|---|---|---|
| Genesis-owned commission | Native source required | Direct production target | Standard import target | Standard import target | Fully designable; require portable subset | Author native look plus portable PBR/cards/mesh | Yes |
| Reallusion custom | Official export/integration | Via FBX/GLB conversion and Genesis work | Official pipeline | Official pipeline | Feasible, but exact export/redistribution scope unknown | High-end vendor shaders/hair need conversion | Runtime may be absent technically; legal/runtime obligations UNKNOWN |
| MetaHuman custom | DCC export and MIT DNA/OpenRigLogic improve access | Requires conversion/runtime strategy | Requires integration | Native strongest path | Geometry/DNA export; GLB is not the native fidelity path | Hair/material portability remains lossy | Some runtime can be open, but editor/source generation remains vendor-shaped |
| Permissive components | Strong | Strong after authoring | Strong | Strong | Standard formats feasible | No complete high-quality system | Yes |

The commissioned source must ship two representations:

1. an editable, highest-fidelity Blender/source representation; and
2. a documented portable runtime profile using ordinary meshes, skeletons, morph targets, texture maps, and hair cards/mesh as needed.

Engine-specific shaders, strand grooms, wrinkle systems, and corrective drivers may be optional enhancements, never the only usable form. A `.genesis` person must remain loadable and meaningfully editable without the original DCC, vendor generator, cloud service, or proprietary database.

## 12. Licensing/redistribution analysis

The source tiers are applied per component, not per brand:

1. **REFERENCE ONLY:** may inform quality but contributes no shipped data. FaceScape and Digital Emily are here.
2. **AUTHORING TOOL:** can assist creation, but its content/output may not be redistributed as Genesis requires. Reallusion and MetaHuman are at most here under ordinary terms for this product model.
3. **DERIVATION SOURCE:** Genesis may create production derivatives, subject to license conditions. Some permissive assets and a future custom vendor grant may qualify.
4. **REDISTRIBUTABLE COMPONENT:** raw/editable component may appear in `.genesis`. Verified CC0 assets and MIT asset data can qualify.
5. **GENESIS-OWNED / COMMISSIONED:** Genesis owns or holds assignment-equivalent rights broad enough for unlimited platform use and sublicensing.

The minimum production-master rule is: all indispensable geometry, textures, rigs, morphs/correctives, grooms, and garment data must be Tier 4 or 5, and the combination must permit Tier-4-style downstream packaging. An authoring tool may be used only if its license expressly permits the resulting delivered data and does not contaminate the master with non-redistributable bases.

“Can sell a game” is not sufficient. Genesis specifically requires commercial modification, derivative identity generation, raw/editable packaging, sublicensing to end users, import into third-party software, and continued downstream use. Any ambiguity on one of those rights is a STOP until reviewed and resolved in writing.

CC0/MIT also does not prove privacy, publicity, biometric-consent, trademark, patent, or chain-of-title clearance. Human scan/model data needs a separate provenance/release check even when repository copyright terms look permissive.

## 13. Custom-license rights checklist

The following is the minimum written grant for either Reallusion or Epic. A reference to “commercial use,” “any engine,” “applications,” or “Enterprise” is insufficient unless the agreement expressly covers these items.

### Product and creation rights

- Genesis may operate a general-purpose commercial character generator that may compete with the vendor's creator tools.
- Genesis and its users may create unlimited distinct and derivative identities, including automated, headless, batch, and programmatic generation.
- Genesis may expose semantic face/body/age/hair/clothing/expression editing, locking, undo, randomization, and user-supplied likeness workflows.
- Genesis may modify topology, UVs, materials, textures, rigs, skeletons, morphs, correctives, LODs, grooms, clothing, and vendor data required to operate them.
- Local AI/LLM systems may translate natural-language requests into deterministic editing operations. Training, synthetic-data generation, and model/database construction should be negotiated separately and may be excluded initially.

### Distribution and sublicensing rights

- Genesis may distribute generated people as editable `.genesis` packages and in GLB/glTF, FBX, USD, Blender, and successor formats.
- Packages may contain geometry, textures, materials, skeletons, skin weights, animations, morph deltas, corrective data, groom/card/mesh data, clothing, metadata, and necessary DNA/runtime data.
- Assets need not be encrypted, inseparable object code, or locked to a proprietary container; ordinary package extraction and user editing are permitted.
- Genesis may grant end users perpetual commercial rights to use, modify, combine, export, redistribute within their own applications, and import the person into any engine/DCC.
- Users may save, share, collaborate on, and—if Genesis chooses—sell their created people. Marketplace/in-app purchase rights must be explicit rather than assumed.
- Genesis may use employees, affiliates, cloud build systems, and contractors and may provide them source data under ordinary confidentiality controls.

### Platform and runtime rights

- No vendor application, account, seat, runtime, cloud service, or downstream license is required for recipients to load/use a generated person.
- Any required runtime/source code may be redistributed on stated terms compatible with Genesis's planned licenses and supported platforms.
- Offline creation/build and operation are permitted where technically feasible.
- Blender, Godot, Unity, Unreal, filmmaking, XR, simulation, local-agent/TTS, and future application uses are covered without per-title approval.

### Commercial and continuity rights

- Fees, royalties, revenue definitions, seats, territories, fields of use, support, audit rights, attribution, and trademark limits are complete and unambiguous.
- The grant is worldwide, perpetual, irrevocable for paid-up versions/content, transferable in a change of control, and sublicensable as above.
- Previously generated people and released Genesis versions survive expiration, termination, vendor acquisition, product discontinuation, and later license changes.
- Genesis may maintain/build/export existing content after termination and retain archival source needed to support users.
- Vendor warranties cover authority and disclosed third-party content; an agreed remedy addresses IP claims.
- The agreement identifies every covered product/version/content pack and a process for adding upgrades; no marketplace contributor content is included without matching rights.

### Required written answers before a vendor POC

1. Which exact files/data may be stored and shipped inside `.genesis`?
2. May end users extract and edit those files without their own vendor account/license?
3. May Genesis expose the vendor-derived parameter basis in its own UI/API and generate unlimited identities?
4. May generated people be sold/shared as assets, not only embedded in games?
5. Are topology, rig, morph, DNA, texture, groom, and clothing rights all included?
6. Is non-training LLM semantic control permitted, and what telemetry/data handling applies?
7. What rights survive termination and future EULA changes?
8. What technical/runtime dependencies, notices, protection measures, royalties, minimums, and audits apply?

Any “no,” missing answer, or reliance on a public FAQ rather than signed controlling terms is a STOP for vendor-source ingestion.

## 14. Cost/effort evidence

Credible public evidence does not support a fixed budget for a redistribution-ready, anatomically credible, rigged digital-human master with full IP assignment. The budget is therefore **UNKNOWN until rights-cleared, specification-based quotes are obtained**.

Available anchors, none of which is a quote for this work:

- The U.S. Bureau of Labor Statistics reports a May 2025 median of **$102,030/year or $49.06/hour** for special effects artists and animators. This is employee wage data, not a loaded contractor/studio rate and not a specialist digital-human buyout price.
- Upwork's current public pages show broad marketplace ranges of **$25–$40/hour for 3D artists**, **$17–$30/hour for 3D modelers**, and a sample **$800–$3,000** “game-ready character.” Upwork itself says the hourly figures reflect historical contracts. These numbers are low-specificity marketplace anchors and do not evidence the cost of Genesis's FACS, correctives, lookdev, source delivery, provenance, or IP assignment.
- Reallusion publicly lists CC5 at **$299**, CC5 Deluxe at **$479**, and a CC5 + Headshot 3 package at **$329** on the accessed pages. Those prices demonstrate inexpensive authoring-tool access, not the unpublished Enterprise rights Genesis needs.
- Epic lists ordinary Unreal access as free below relevant $1M thresholds, a **$1,850/seat/year** seat option for qualifying non-game commercial use above the threshold, and the usual 5% royalty framework for Royalty Products above exclusions. Custom terms are quote-only. Ordinary engine price does not buy editable MetaHuman generator redistribution rights.
- The historical Digital Emily production page reports that a specialist team built the rigged face over “a few months” after scans were delivered. It is evidence that high-fidelity facial work is a multidisciplinary production, not a contemporary quote.

Procurement should request separate prices and schedules for: (1) the qualification wedge, (2) a full neutral master, (3) the initial semantic identity/body basis, (4) FACS and body correctives, (5) portable hair/material profiles, (6) one production garment, (7) LOD/export integration, and (8) full source/IP assignment. Quotes must identify assumptions, revisions, subcontractors, third-party inputs, taxes, royalties, ongoing support, and rights premiums.

The qualification wedge is intentionally the cost-control mechanism: buy proof of source quality, collaboration, portability, and chain of title before committing to the full master. No budget range should be approved from generic marketplace numbers.

## 15. Decision matrix

Scores are 1 (poor/high risk) to 5 (strong/low risk). Vendor rights scores reflect current published terms, not hypothetical custom terms. “Cost” scores affordability/predictability, so high is better.

| Criterion | New Genesis-owned commission | Target old topology only | Reallusion ordinary/custom-unknown | MetaHuman ordinary/custom-unknown | Permissive-component assembly |
|---|---:|---:|---:|---:|---:|
| One-metre visual ceiling | 5 | 3 | 4 | 5 | 3 |
| Semantic editability | 5 | 4 | 5 | 4 | 3 |
| Identity diversity | 4 | 3 | 5 | 4 | 3 |
| Portability | 5 | 5 | 3 | 3 | 5 |
| Redistribution rights | 5 | 5 | 1 | 1 | 4 |
| Platform independence | 5 | 5 | 2 | 2 | 5 |
| Local-AI edit compatibility | 5 | 5 | 2 | 2 | 5 |
| Integration simplicity | 3 | 4 | 3 | 2 | 2 |
| Low vendor lock-in | 5 | 5 | 1 | 1 | 5 |
| Provenance clarity | 5 with contract | 5 | 2 | 2 | 3 |
| Long-term maintainability | 4 | 3 | 2 | 2 | 3 |
| Cost/predictability | 2 | 3 | 1 (unknown) | 1 (unknown) | 4 |
| Low future-license risk | 5 | 5 | 1 | 1 | 5 |
| Existing-architecture fit | 5 | 5 | 3 | 3 | 4 |
| **Unweighted total / 70** | **63** | **55** | **35** | **35** | **54** |

The numeric total is a decision aid, not fake precision. The decisive constraints are conjunctive: source quality **and** semantic control **and** redistribution **and** portability must all pass. Under published terms the vendor routes fail a hard rights constraint. The permissive assembly and old-topology route preserve rights but have already shown insufficient or unproven source quality. The owned commission is the only route with a plausible pass across every hard constraint.

## 16. Recommended primary strategy

Commission a **new Genesis-owned canonical master**, beginning with the qualification wedge.

Architecture:

`GenesisPerson → CharacterSpecification → existing/versioned mapper boundary → CommissionedMasterBackend vNext → portable assets/package`

Implementation principles for the later milestone:

- preserve semantic authority and public parameter names;
- allow the production artist to replace topology/UV/rig where quality requires;
- require a stable neutral, semantic morph/corrective manifest, safe ranges, and combination tests;
- make Blender source the inspectable authoring truth while keeping `.genesis` and portable runtime data independent of Blender;
- author high-fidelity and portable material/hair profiles together;
- admit CC0/MIT components only after file-level provenance review, and keep them separately identified in the allowlist;
- treat vendor tools only as optional workflow aids if their outputs are independently cleared, never as hidden canonical dependencies;
- version the new backend so POC-1 remains reproducible and comparable.

The strategy buys ownership of the bottleneck instead of renting a visually impressive but legally constrained identity system. It also lets Genesis expand the identity basis incrementally after one master meets the close-up gate.

## 17. Recommended fallback

If the qualification wedge fails twice with qualified suppliers, or credible bids show the owned route is infeasible, run a **license-information gate** before any vendor asset POC:

1. Request a Reallusion custom/Enterprise response against section 13, with covered CC5 base, Headshot, ActorMIXER, morph, texture, hair, clothing, and export data named explicitly.
2. Require signed terms or a redlined term sheet sufficient for legal review; marketing email assurances are not enough.
3. If Reallusion will not grant the hard rights, request the same from Epic for MetaHuman character content, DNA, rigs, textures, grooms, source redistribution, user sublicensing, local semantic editing, and post-termination survival.
4. Only after one agreement passes the legal gate should Genesis build a small vendor-source adapter POC behind the existing mapper.

Reallusion is first because the public product is closer to a broad face/body morph generator and Reallusion explicitly advertises an Enterprise path for application/service integration and character-system questions. MetaHuman remains the superior close-up technical benchmark and may become first if Epic supplies clearer, broader terms.

**Decision requiring vendor response:** whether either vendor can become an approved Tier 3–4 source, what covered data can ship, and total commercial terms. Until answered, both remain Tier 2/reference candidates for Genesis, not production dependencies.

## 18. Why the other strategies lost

- **Refine POC-1 again:** violates the completed experiment's precommitted STOP and does not change source quality.
- **Keep the POC-1 topology and commission surface polish:** preserves sunk work at the cost of facial and deformation quality; the failed areas are topology/rig/corrective coupled.
- **Assemble only permissive components:** useful for inputs and benchmarks, but no found combination supplies a coherent production skin, groom, full-body deformation system, clothing, identity space, and documented chain of title at the required quality.
- **Reallusion under ordinary terms:** technically attractive, expressly incompatible with a character generator/character-generating business and editable asset redistribution.
- **MetaHuman under ordinary terms:** strongest visual benchmark, but public legal pages conflict and do not clearly permit raw/editable sublicensing through an independent generator; AI/database restrictions also require care.
- **A single fixed scan/hero character:** can look excellent but lacks identity diversity and deterministic semantic editability, and scan/likeness rights create a separate dependency.
- **Make a vendor database the Genesis identity authority:** would discard the architecture POC-1 proved and create migration, lock-in, and package-survival risks without necessity.
- **Buy random marketplace assets:** provenance, raw redistribution, topology compatibility, contributor rights, and character-generator permissions are not reliably established.

## 19. Exact next POC

Name: **Master Human POC-2A — Commissioned Source-Quality Qualification Wedge**

Purpose: test the selected source strategy, supplier capability, rights chain, semantic locality, and portable look before funding a full master. It must reuse POC-1's cameras, lighting, validation concepts, mapper boundary, packaging path, Godot preview, and performance harness. It is not a full person and must not become one through scope creep.

### Precondition

A signed short-form commissioning agreement and input schedule must satisfy section 6 for the wedge. No work product enters Genesis before chain-of-title, source-delivery, redistribution, sublicensing, and non-training local-AI semantic-control rights are documented. All external inputs must be pre-approved; “standard industry assets” is not adequate disclosure.

### Exact art deliverable

- One original adult neutral head, neck, and shoulder bust on production facial topology.
- Complete eyeballs/cornea/iris/sclera, lids with thickness, tearline/occlusion, ears, nostrils, lips, mouth bag, teeth, gums, and tongue.
- One detached but production-complete hand/wrist sample with nails and a rigged wrist/finger subset.
- Calibrated 4K-or-better working skin maps/source capable of downsampling: albedo without baked lighting, roughness/specular, normal and/or displacement, masks, and documented real-world scale.
- Hairline plus brows/lashes and a small facial-hair-capability sample; a portable cards/mesh representation is mandatory even if a strand groom is also supplied.
- One collar/sleeve garment-boundary sample with thickness, seam, body clearance, and deformation.
- A neutral facial rig plus exactly these semantic test controls: nose width, eye spacing, lip fullness, jaw width, apparent-age test layer, brow inner raise, brow down, blink L/R, squint L/R, smile, frown, jaw open, mouth funnel, mouth pucker, and cheek raise. Controls may be implemented with constituent shapes/correctives, but must be documented.
- Shoulder elevation/forward reach corrective and wrist flex/extension corrective; enough finger motion to inspect knuckle/nail deformation.
- Native Blender source, dependency manifest, source texture projects, export script/settings, GLB, and the data required to reproduce each test render.

### Integration work permitted in the later POC

Create one new backend version/adapter mapping the existing semantic fields needed by the wedge. Do not redesign `GenesisPerson` or broaden the production application. Package the wedge as an explicitly incomplete test asset. Re-run substantially the POC-1 fixed views: one-metre face, eye, mouth, hairline, grazing-light skin, hand, shoulder/wrist deformation, neutral versus expression identity, Blender export/roundtrip, Godot load/render, package/provenance, and performance.

### Excluded

No full body, identity library, multiple garments, full hairstyle catalog, animation library, Unity/Unreal product integration, crowds, photo likeness, voice, or generator UI. Those follow only after GO.

## 20. STOP/GO criteria

The criteria are fixed before supplier selection. All hard gates must pass.

### Rights/provenance — hard gate

- Signed rights satisfy the wedge-relevant portions of sections 6 and 13.
- Every input has creator/owner, URL or delivery record, date, version, hash, license/assignment, and modification history.
- No undisclosed third-party, marketplace, vendor-base, scan, likeness, or generative-AI asset is present.
- Genesis may ship and let users edit/extract the wedge in `.genesis` without a supplier/vendor license or runtime.

Any failure is immediate **STOP**, regardless of visual quality.

### Visual — hard gate

Use the same 1–5 rubric and matched POC-1 cameras/lighting. At least three reviewers who did not author the asset score anonymized A/B renders at final resolution. GO requires:

- median **≥4** for overall face, identity distinctiveness, eyes/lids/tearline, lips/mouth/nostrils, skin response/microdetail, hairline/brows/lashes, hand/nails, facial expression, shoulder deformation, and wrist/hand deformation;
- no individual review median below **3** in any listed category;
- improvement of at least **+1 point** over the POC-1 baseline in every comparable failed category;
- neutral likeness/identity remains recognizably stable through blink, smile, jaw-open, funnel/pucker, and brow tests, with no visible intersections or collapsed volumes in the fixed close-ups;
- the collar/sleeve sample has no body penetration in the required poses and reads as constructed fabric rather than painted skin.

The visual gate fails after one bounded supplier remediation pass: **STOP that supplier/source**, do not start an open-ended polish loop.

### Semantic/determinism — hard gate

- Repeated builds from the same specification produce identical resolved parameters and reproducible asset fingerprints except documented nondeterministic metadata.
- Each named edit moves in the requested direction and is locally bounded under a predeclared landmark/region test.
- Face lock prevents body/hair/garment edits from changing face geometry/material identity.
- Undo restores the prior resolved state and fingerprint.
- Valid two-control combinations do not create broken anatomy; invalid ranges are clamped/rejected deterministically.

### Portability/package — hard gate

- Editable source opens in the supported Blender version with no missing dependency.
- Exported GLB imports into a clean Godot project with required skeleton, skin, test morphs, textures, and portable hair representation.
- The `.genesis` package contains the declared portable representation, provenance, backend/version metadata, and no undeclared vendor runtime.
- The person remains usable after the supplier's build tools and network are unavailable.

### Performance — hard gate

On the same reference configuration and harness, p95 remains **≤16.7 ms** and reported video memory **≤4,096 MiB** at 2560×1440 after the established warm-up/measurement sequence. Record mesh/material/texture/morph counts so later optimizations are evidence-based. The POC-1 result remains the comparison baseline, not the new maximum expectation.

### Decision

- **GO:** every hard gate passes. Commission the full canonical master using the accepted topology/control standards and milestone contract.
- **STOP/RE-SOURCE:** rights, quality, semantic, portability, or performance fails after the one allowed remediation. Preserve evidence; evaluate a different qualified supplier or invoke section 17.
- **No conditional GO:** a visually superior asset that fails redistribution, determinism, package independence, or portability is a STOP.

## 21. Source-and-claim ledger

All web sources below were accessed **2026-09-19**. Local sources are repository state reviewed on the same date. URLs are recorded directly so later legal/engineering review can archive the controlling versions. Marketing pages support feature/cost claims only; legal claims rely on license text wherever available.

| ID | Source | Claims supported / limitations |
|---|---|---|
| L1 | `docs/MASTER_DIGITAL_HUMAN_RESEARCH.md` | Prior architecture, source categories, quality rubric, and earlier fallback hypothesis. Not treated as current vendor-license evidence. |
| L2 | `docs/MASTER_HUMAN_POC1_REPORT.md` | POC-1 engineering passes, exact scores, asset size, performance, STOP decision. |
| L3 | `poc/master-human/provenance/LICENSE_GATE.md` | Genesis provenance gate and allowed/rejected dependency policy. |
| L4 | `poc/master-human/provenance/dependency-allowlist.json` | Exact POC-1 allowlisted dependencies and licenses. |
| R1 | https://www.reallusion.com/Content/EULA/AP/EULA_AP.htm | Current Software EULA; CC base topology/rig ownership; non-sublicensable/revocable grant; character-generator, character-business, AI/ML/synthetic-data, and third-party asset-sale restrictions. |
| R2 | https://www.reallusion.com/license/content.html | Standard export, Enterprise inquiry categories, application/service integration, AI/ML, special-use and competition restrictions, and client/character-creation limits. Public overview, not an Enterprise contract. |
| R3 | https://www.reallusion.com/Content/EULA/EULA.htm | Content EULA source/raw distribution, sublicensing, character-generation/API, protection, AI, and special-authorization terms. |
| R4 | https://kb.reallusion.com/Purchase/53107/Can-I-sell-the-characters-I-create-with-Character-Creator | Reallusion FAQ requiring permission for third-party marketplace/app transformation/distribution and another character-generation system. FAQ is explanatory, subordinate to agreement. |
| R5 | https://www.reallusion.com/Content/EULA/AI-Service.htm | Separate AI Actor/service system-lock, export, extraction, and reuse restrictions; not assumed to govern ordinary CC assets. |
| R6 | https://www.reallusion.com/character-creator/ | CC5 features, formats/integrations, published $299/$479 packages. Product marketing, not proof of output rights or objective quality. |
| R7 | https://www.reallusion.com/character-creator/headshot/ | Headshot 3 technical features, morph count, and accessed package prices. Product marketing only. |
| R8 | https://www.reallusion.com/plan-and-pricing/individual/perpetual?product=cc | Current product/package description and perpetual-license context. Prices/promotions may change. |
| E1 | https://www.metahuman.com/license | Current statement that MetaHuman uses standard Unreal licensing and may be used with any engine; FAQ topics include asset sale, AI, and custom terms, but scraped page does not expose complete answers. |
| E2 | https://www.unrealengine.com/eula/unreal | Current UE EULA; Non-Engine Products, distribution, ownership, seats/royalties, MetaHuman AI/database/training restrictions, cloud-input likeness duties, and custom agreement precedence. |
| E3 | https://www.unrealengine.com/eula/content | Current served ECLA; object-code/inseparable project distribution, source sharing, stand-alone/content-creation restrictions, ownership, and served MetaHuman UE-Only addendum. |
| E4 | https://www.unrealengine.com/eula-change-log/content | Official ECLA change log stating MetaHuman-specific restrictions were deleted; conflicts with E3 as served and is not a substitute for controlling text. |
| E5 | https://www.unrealengine.com/license | Current ordinary UE pricing/royalty/seat summary and custom-license contact path. Does not price or grant Genesis-specific MetaHuman rights. |
| E6 | https://dev.epicgames.com/documentation/metahuman/metahuman-5-8-release-notes-in-unreal-engine | MetaHuman 5.8 arbitrary-topology/full-body conform, unbaked material controls, exports, Python coefficients, Devkit updates. Technical release notes only. |
| E7 | https://dev.epicgames.com/documentation/metahuman/metahuman-creator-from-custom-mesh-tool-in-unreal-engine | Custom mesh workflow, standard topology result, rigging, solver limitations, custom-topology and texture workflow notes. |
| E8 | https://dev.epicgames.com/documentation/metahuman/from-template-mesh | Semantic edge-loop/topology requirements and DCC workflow cautions. |
| E9 | https://dev.epicgames.com/documentation/metahuman/metahuman-creator-export-tool-in-unreal-engine | DCC geometry/material/DNA export and round-trip capability. Technical capability is not redistribution permission. |
| E10 | https://dev.epicgames.com/documentation/metahuman/metahuman-dna-rig-definition-and-rig-operation | DNA contents and RigLogic semantic-channel architecture. |
| E11 | https://dev.epicgames.com/documentation/metahuman/metahuman-devkit-in-unreal-engine | Open DNA/OpenRigLogic tools and outside-platform integration. Code licensing does not relicense character content. |
| E12 | https://github.com/EpicGames/MetaHuman-DNA-Calibration | Open source repository/license context for DNA tooling; exact tag/commit would need allowlisting before use. |
| E13 | https://www.unrealengine.com/eula/mhc | Legacy cloud MetaHuman Creator EULA scope; page states UE 5.6+ Creator is governed by the UE EULA. Relevant to version provenance, not the basis for the current decision. |
| B1 | https://www.blender.org/download/demo-files/ | Human Base Meshes v1.4.1, CC0, updated 2026-01-20, Blender requirement. |
| B2 | https://studio.blender.org/training/realistic-human-research/ | Blender Studio realistic-human work packages and production-derived workflow. Some training access is subscription-based; overview and listed sections are public. |
| B3 | https://studio.blender.org/training/realistic-human-research/retopology/ | Need to lock production topology before downstream detailing and rigging. |
| M1 | https://static.makehumancommunity.org/about/license.html | Core graphical assets are CC0; MakeHuman/MPFB code has separate AGPL/GPL licenses. |
| M2 | https://static.makehumancommunity.org/makehuman/faq/can_i_sell_models_created_with_makehuman.html | Official statement that exports may be modified, sold, and used commercially under CC0. |
| M3 | https://static.makehumancommunity.org/makehuman/faq/are_makehuman_files_free.html | Express statement that CC0 base/targets may be used in another character generator. |
| M4 | https://static.makehumancommunity.org/assets/assetpacks.html | Current CC0 system/target/material/mesh/pose packs, including eyes/teeth, hands, face units, skins, hair, and clothes. Individual files still require allowlisting. |
| I1 | https://github.com/USC-ICT/ICT-FaceKit | FaceKit Light topology, component counts, 100 identity modes, 53 expressions, and missing Light features. Repository description; production suitability and scan consents require diligence. |
| I2 | https://github.com/USC-ICT/ICT-FaceKit/blob/master/LICENSE | MIT grant for repository software/material as published. Does not independently document subject releases, patents, or file-level third-party inputs. |
| D1 | https://vgl.ict.usc.edu/Data/DigitalEmily2/ | Digital Emily 2/Wikihuman downloads and explicit research/illustration, no-transfer, no-commercial/resale notice. |
| D2 | https://vgl.ict.usc.edu/Research/DigitalEmily/ | Historical production description, multidisciplinary credits, and “few months” rig-development account. Not a current cost benchmark. |
| F1 | https://nju-3dv.github.io/projects/FaceScape/ | Current FaceScape access and non-commercial research restrictions. |
| F2 | https://nju-3dv.github.io/projects/FaceScape/static/license/LicenseAgreement_FaceScape.pdf | No commercial use, no distribution, non-transferable terms, and portrait restrictions. |
| S1 | https://smpl-x.is.tue.mpg.de/bodylicense.html | CC BY 4.0 license for the defined SMPL-X Body subset and model/patent distinctions. |
| S2 | https://github.com/vchoutas/smplx/blob/main/README.md | Non-commercial scientific-research license notice for SMPL-X model/data/software download. Repository state can change; pin before any evaluation. |
| P1 | https://polyhaven.com/license | CC0 raw redistribution/derivative/commercial permissions and website/asset distinction. |
| P2 | https://polyhaven.com/corporate | Optional provenance attestations and bulk snapshots; useful process evidence, not a human-source solution. |
| C1 | https://www.bls.gov/ooh/arts-and-design/multimedia-artists-and-animators.htm | May 2025 U.S. median wage and occupational/team/specialization context. Wage is not contractor/studio pricing. |
| C2 | https://www.upwork.com/hire/3d-artists/cost/ | Current broad 3D-artist marketplace hourly range; low-specificity cost anchor. |
| C3 | https://www.upwork.com/hire/3d-modelers/ | Current broad modeler range and sample game-character project range; explicitly not evidence for Genesis's full scope or rights premium. |

### Unresolved facts that must remain labeled UNKNOWN

- Reallusion and Epic custom-license availability, exact grant, price, minimums, royalties, audit/protection requirements, and survival.
- Which third-party vendor content can be included under a custom grant.
- Whether vendor terms would permit user asset resale/marketplace use in addition to ordinary application embedding.
- File-level provenance, participant releases, and patent/publicity review for ICT FaceKit production use.
- The cost, schedule, and suitable supplier for a Genesis-owned master until a rights-cleared statement of work receives bids.
- Whether one supplier can meet every specialty; the qualification wedge exists to test this rather than assume it.
