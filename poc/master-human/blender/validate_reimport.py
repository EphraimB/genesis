"""Clean-room GLB re-import validation for Master Human POC-1."""

import json
from pathlib import Path

import bpy


ROOT = Path(__file__).resolve().parents[3]
GLB = ROOT / "artifacts" / "master-human-poc" / "game-close" / "genesis_master_human_poc1.glb"
REPORT = ROOT / "artifacts" / "master-human-poc" / "validation" / "blender_reimport_validation.json"

bpy.ops.object.select_all(action="SELECT")
bpy.ops.object.delete(use_global=False)
bpy.ops.import_scene.gltf(filepath=str(GLB))

meshes = [obj for obj in bpy.context.scene.objects if obj.type == "MESH"]
armatures = [obj for obj in bpy.context.scene.objects if obj.type == "ARMATURE"]
body = max(meshes, key=lambda obj: len(obj.data.vertices))
shape_keys = [] if body.data.shape_keys is None else [key.name for key in body.data.shape_keys.key_blocks if key.name != "Basis"]
bones = [] if not armatures else [bone.name for bone in armatures[0].data.bones]
required_shapes = {"Blink_L", "Blink_R", "Jaw_Open", "Lip_Seal", "Smile", "Frown", "Brow_Raise",
                   "Viseme_A", "Viseme_E", "Viseme_I", "Viseme_O", "Viseme_U", "Viseme_FV"}
required_bones = {"root", "pelvis", "spine_01", "spine_02", "neck", "head", "jaw", "eye.L", "eye.R",
                  "hand.L", "hand.R", "foot.L", "foot.R"}
checks = {
    "glbImported": len(meshes) > 0,
    "bodyTopologyPresent": len(body.data.vertices) >= 10000,
    "armaturePresent": len(armatures) == 1,
    "requiredBonesPresent": required_bones.issubset(bones),
    "requiredFaceShapesPresent": required_shapes.issubset(shape_keys),
    "materialsPresent": len(bpy.data.materials) >= 8,
}
report = {
    "schemaVersion": 1,
    "source": str(GLB.relative_to(ROOT)).replace("\\", "/"),
    "meshCount": len(meshes),
    "bodyVertices": len(body.data.vertices),
    "armatureCount": len(armatures),
    "boneCount": len(bones),
    "shapeKeys": shape_keys,
    "materialCount": len(bpy.data.materials),
    "checks": checks,
    "passed": all(checks.values()),
}
REPORT.write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
print("GENESIS_MASTER_HUMAN_REIMPORT_OK" if report["passed"] else "GENESIS_MASTER_HUMAN_REIMPORT_FAILED")
print(json.dumps(checks, sort_keys=True))
if not report["passed"]:
    raise SystemExit(1)
