# Master Human POC-1 license gate

This directory is the authoritative dependency gate for Master Human POC-1. The machine-readable allowlist is `dependency-allowlist.json`.

## Gate rules

- Default deny: no external code, geometry, target, texture, groom, garment, model, or installer may contribute unless enumerated.
- Code and asset licenses are recorded separately.
- Network package installation is prohibited. Approved archives are downloaded directly and verified before extraction.
- The Anny wheel is never installed with `pip`; direct ZIP inspection avoids dependency resolution and prevents optional SMPL-X/non-commercial downloads.
- Denied path patterns are scanned before an archive is used.
- Publisher license/notice files are copied into this directory after verified extraction.
- Generated provenance records include the downloaded file hash and hashes of every upstream file that contributes to the final artifact.
- Genesis-created procedural textures, hair, and garments have no external asset input; their generating script and parameter hashes are recorded.

## Human Base Meshes checksum exception

Blender publishes Human Base Meshes v1.4.1 with a precise URL, version, byte length, modification time, ETag, and CC0 declaration, but it does not publish a cryptographic checksum next to the bundle. The bundle may therefore be downloaded only into an isolated quarantine location. Before extraction or Blender import, its SHA-256 must be calculated and written into the allowlist, the byte length and remote fingerprint must match, and archive paths must pass the denied-content scan. This is a supply-chain control, not permission to use an unverified file.

## Permitted derivation language

Any resulting mesh that contains or was transferred from Anny/MakeHuman or Blender Human Base Mesh geometry is a **Genesis-maintained derivative from enumerated CC0 inputs**. It must not be described as wholly original. Genesis-authored rigging, morph composition, materials, procedural grooms, clothing, validation, and semantic mapping are separately identifiable contributions.

## Primary license sources

- Anny repository and component notices: <https://github.com/naver/anny>
- Anny 0.6.0 publisher metadata and digest: <https://pypi.org/project/anny/0.6.0/>
- MakeHuman/MPFB license split: <https://static.makehumancommunity.org/about/license.html>
- Blender Human Base Meshes CC0 declaration: <https://www.blender.org/download/demo-files/>
- Blender asset repository CC0 license: <https://projects.blender.org/blender/blender-assets>
- Blender application license: <https://www.blender.org/about/license/>
- Godot license: <https://godotengine.org/license/>

