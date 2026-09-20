# POC-2A face experiment: final Pass 2

Completed 2026-09-20. Provisional self-evaluation, not independent qualification.

## Verdict

STOP — FACE QUALITY INSUFFICIENT

All eight final diagnostic renders were opened and visually inspected individually.
The result is substantially better than POC-1, but still looks like a synthetic
mannequin at approximately one metre. Improvement over a weak baseline is not
evidence of a convincing digital human. There is no Pass 3.

## Was the interrupted Pass 2 merely initialized?

No. It contained meaningful but unfinished shader and groom changes. Inspection
preceded new work. The preserved Pass 1 source was last written at
2026-09-20 09:00:42 UTC (9,626,646 bytes); the interrupted Pass 2 at 09:03:11 UTC
(10,202,770 bytes), immediately after the 09:03:02 refinement-script edit.
Pass 1 had eight diagnostics; interrupted Pass 2 had none.

The head-coordinate hash was identical in both files:
`995023d63221c2c114d253ca6c0c6a2f8af91df257f06d881a75711f404a0805`.
But SkinPaint hashes differed, the skin material had 15 rather than 7 nodes,
hair curves had been converted from POLY to NURBS, and Temple_fade existed only
in Pass 2. Thus artistic work had started; it had not been rendered or qualified.

The interrupted scene was preserved in `pass2/progress_01/face.blend` before
continuation. Its diagnostics exposed repeating lip stripes, overly uniform
pores, and a new eye-surface artifact. These were not accepted as final evidence.

## Completed work

- Native Blender Inflate stroke restricted to the lower lip. The observed
  96-vertex footprint initially displaced at most 0.023 mm; the support script
  amplified that same footprint by 18, to approximately 0.42 mm. It did not
  reshape the nose, lids, cheeks, head proportions, or other anatomy.
- Repainted vermilion around the actual contact seam, region-specific skin
  pigment and roughness, restrained imperfections, regional pore strength,
  fine epidermal relief, and subsurface-response adjustment.
- Replaced periodic lip bands with individually placed, uneven fine creases.
  Their generated height image is packed in the final Blender source.
- Thinned 1,556 boundary strands from the existing groom, graduated boundary
  strand length/radius, added finer irregular hairline follicles, and retained
  the underlying sweep and temple transition. No solid hair cap was added.
- Restored the saved Pass 1 eye, cornea, lid-margin, lash, and brow objects.
  Read-only comparison confirmed matching geometry for all 12 restored objects.
- Used intermediate diagnostics within Pass 2; preserved an additional
  checkpoint under `pass2/progress_02`. Neither checkpoint is an extra pass.

## Visual critique

| View | Final observation |
| --- | --- |
| 01 front | Proportions retained; skin and expression still mannequin-like. |
| 02 three-quarter | Cheek/nose form holds; skin lacks convincing tissue variation. |
| 03 profile | Silhouette preserved; restrained lip projection, no major anatomy regression. |
| 04 approximately one metre | Clearly CG; added close-up detail does not solve overall credibility. |
| 05 eye close-up | Interrupted-Pass-2 artifact removed; pale iris rim, flat sclera and orderly lashes remain. |
| 06 mouth close-up | Lower vermilion has clearer volume/color transition; contact remains too clean and creases look sparse/etched. |
| 07 hairline close-up | More graduated and irregular roots; common strand sweep and pale sheen still look groom-generated. |
| 08 grazing-light skin | Fine relief is restrained, but broad skin response remains too smooth and waxy. |

Scores judge visible quality, not implementation effort. Scale: 1 clearly
unconvincing; 3 plausible structure with obvious CG limitations; 5 convincing
high-quality digital human.

| Category | Self-score / 5 |
| --- | ---: |
| Overall face | 2 |
| Identity distinctiveness | 2 |
| Eyes / lids / tearline | 2 |
| Mouth / lips / nostrils | 3 |
| Skin response / microdetail | 2 |
| Hairline / brows / lashes | 2 |

## Exact local deliverables

- Pass 1 source: `C:\Users\emb16\Documents\genesis\artifacts\master-human-poc2a\face\pass1\face.blend`
- Pass 2 source: `C:\Users\emb16\Documents\genesis\artifacts\master-human-poc2a\face\pass2\face.blend`
- Pass 1 renders: `C:\Users\emb16\Documents\genesis\artifacts\master-human-poc2a\face\pass1\`
- Pass 2 renders: `C:\Users\emb16\Documents\genesis\artifacts\master-human-poc2a\face\pass2\`
- Three-way comparisons: `C:\Users\emb16\Documents\genesis\artifacts\master-human-poc2a\face\pass2\comparisons\`
- Native lip-stroke input: `C:\Users\emb16\Documents\genesis\artifacts\master-human-poc2a\face\pass2\native_lip_sculpt.blend`

Both render directories contain `01_face_front.png`,
`02_face_three_quarter.png`, `03_face_profile.png`, `04_face_one_meter.png`,
`05_eye_closeup.png`, `06_mouth_closeup.png`, `07_hairline_closeup.png`,
`08_skin_grazing_light.png`. Final Pass 2 also has `contact_sheet.png`.
Each comparison uses the same view stem plus `_poc1_pass1_pass2.png`.

Comparisons are unretouched: POC-1 / Astra Pass 1 / Astra Pass 2. POC-1 lighting
differs, and profile/detail/one-metre framing differs. These are visual reference
comparisons, not controlled shader-only A/B measurements.

The final source is saved on disk. Reopen it before inspecting in an already-open
Blender window, which may still hold the earlier in-memory scene.

## Scope and preservation

Initial and Pass 1 were not rebuilt. Pass 1 source/renders were not overwritten.
No new external asset was acquired; the existing cleared CC0 base remains the
anatomical source. No Godot, Core, body, hand, garment, semantic, package, or
benchmark work was performed. No commit was made.

`git diff --check` passed (only the repository's LF-to-CRLF conversion warning).
Generated scenes, textures and renders remain in the already-ignored artifact
directory. The saved native sculpt inputs are required to reproduce this exact
artistic result; scripts alone are not a substitute for those local source files.
