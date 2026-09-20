"""Render the eight face-only diagnostic views of the current saved art stage."""
import bpy
from pathlib import Path
from mathutils import Vector
scene=bpy.context.scene
out=Path(bpy.data.filepath).parent
camera=scene.camera
try:
    prefs=bpy.context.preferences.addons['cycles'].preferences
    prefs.compute_device_type='OPTIX'; prefs.get_devices()
    for device in prefs.devices: device.use=device.type=='OPTIX'
    if any(device.use for device in prefs.devices): scene.cycles.device='GPU'
except Exception:
    scene.cycles.device='CPU'
views=[
 ('01_face_front',(0,-.72,1.57),(0,0,1.57),85,(900,900)),
 ('02_face_three_quarter',(.48,-.64,1.58),(0,0,1.56),90,(900,900)),
 ('03_face_profile',(.64,-.11,1.57),(0,-.055,1.565),78,(900,900)),
 ('04_face_one_meter',(.10,-1.12,1.57),(0,-.12,1.565),85,(900,900)),
 ('05_eye_closeup',(.032,-.35,1.582),(.032,-.123,1.574),85,(1050,700)),
 ('06_mouth_closeup',(.01,-.38,1.513),(0,-.13,1.506),100,(1050,700)),
 ('07_hairline_closeup',(.025,-.46,1.687),(0,-.04,1.642),96,(1050,700)),
 ('08_skin_grazing_light',(.18,-.70,1.58),(0,0,1.56),90,(900,900))]
key=bpy.data.objects['Key_neutral']; fill=bpy.data.objects['Fill_neutral']; old=key.location.copy(); old_fill=fill.data.energy
for name,pos,target,lens,res in views:
    camera.location=pos; camera.rotation_euler=(Vector(target)-camera.location).to_track_quat('-Z','Y').to_euler(); camera.data.lens=lens
    scene.render.resolution_x,scene.render.resolution_y=res
    if name.startswith('08'):
        key.location=(-.44,-.08,1.68); key.rotation_euler=(Vector((0,-.05,1.56))-key.location).to_track_quat('-Z','Y').to_euler(); fill.data.energy=old_fill*.6
    scene.render.filepath=str(out/(name+'.png'))
    bpy.ops.render.render(write_still=True)
    print('DIAGNOSTIC_SAVED',scene.render.filepath,flush=True)
key.location=old; key.rotation_euler=(Vector((0,-.02,1.57))-key.location).to_track_quat('-Z','Y').to_euler(); fill.data.energy=old_fill
name,pos,target,lens,res=views[0]
camera.location=pos; camera.rotation_euler=(Vector(target)-camera.location).to_track_quat('-Z','Y').to_euler(); camera.data.lens=lens
scene.render.resolution_x,scene.render.resolution_y=res
bpy.ops.wm.save_as_mainfile(filepath=bpy.data.filepath)
