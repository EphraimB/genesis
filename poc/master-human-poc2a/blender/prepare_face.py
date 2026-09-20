"""Open the already cleared CC0 head for the face-only artistic experiment.

No POC-1 source is modified. The retained native mesh is the artistic substrate;
this scene deliberately has no Genesis runtime/export/packaging integration.
"""
from pathlib import Path
import bpy

ROOT = Path(__file__).resolve().parents[3]
OUT = ROOT / 'artifacts/master-human-poc2a/face'
OUT.mkdir(parents=True, exist_ok=True)
BUNDLE = ROOT / 'poc/master-human/vendor/blender-human-base-meshes/human-base-meshes-bundle-v1.4.1/human_base_meshes_bundle.blend'
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete(use_global=False)
with bpy.data.libraries.load(str(BUNDLE), link=False) as (src, dst):
    dst.objects = ['GEO-body_male_realistic']
head = dst.objects[0]
bpy.context.collection.objects.link(head)
head.location = (0, 0, 0)
head.name = 'Face_Artist_Master'
head['source'] = 'Blender human base meshes 1.4.1 / CC0 / already allowlisted for POC-1'
head['scope'] = 'Face only; lower-body source retained outside framing, no body work'
head.select_set(True)
bpy.context.view_layer.objects.active = head
for mod in head.modifiers:
    print('NATIVE_MODIFIER', mod.name, mod.type, getattr(mod, 'total_levels', None))
    if mod.type == 'MULTIRES':
        mod.levels = min(2, mod.total_levels)
        mod.sculpt_levels = min(2, mod.total_levels)
        mod.render_levels = min(2, mod.total_levels)
head.data.materials.clear()
mat = bpy.data.materials.new('Anatomy_Clay')
mat.diffuse_color = (.43, .30, .24, 1)
head.data.materials.append(mat)
for p in head.data.polygons:
    p.use_smooth = True
for area in bpy.context.screen.areas:
    if area.type == 'VIEW_3D':
        area.spaces.active.region_3d.view_distance = .42
        area.spaces.active.region_3d.view_location = (0, -.02, 1.565)
        from mathutils import Quaternion
        area.spaces.active.region_3d.view_rotation = Quaternion((1, 0, 0), 1.57079632679)
        area.spaces.active.region_3d.view_perspective = 'ORTHO'
bpy.ops.wm.save_as_mainfile(filepath=str(OUT / 'anatomy_working.blend'))
print('FACE_READY', OUT / 'anatomy_working.blend')
