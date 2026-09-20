"""Face-only look development supporting the saved native Multires sculpt.

The anatomical input is anatomy_working.blend, edited interactively in Blender.
This is a diagnostic art scene, not an exportable Genesis character or rig.
No external asset beyond the previously cleared CC0 base is used.
"""
import math
import random
from pathlib import Path
import bpy
import bmesh
from mathutils import Vector
from mathutils.bvhtree import BVHTree

ROOT = Path(__file__).resolve().parents[3]
BASE = ROOT / 'artifacts/master-human-poc2a/face'
STAGE = BASE / 'initial'
STAGE.mkdir(parents=True, exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(BASE / 'anatomy_working.blend'))
if bpy.context.object and bpy.context.object.mode != 'OBJECT':
    bpy.ops.object.mode_set(mode='OBJECT')
head = bpy.data.objects['Face_Artist_Master']
bpy.context.view_layer.objects.active = head
head.select_set(True)
for mod in list(head.modifiers):
    if mod.type == 'MULTIRES':
        mod.levels = 2
        mod.sculpt_levels = 2
        mod.render_levels = 2
    bpy.ops.object.modifier_apply(modifier=mod.name)
# Crop the unworked body below the portrait bust; the original sculpt is retained.
bm = bmesh.new(); bm.from_mesh(head.data)
bmesh.ops.delete(bm, geom=[v for v in bm.verts if v.co.z < 1.31], context='VERTS')
bm.to_mesh(head.data); bm.free()
head.name = 'Face_NativeSculpt'
for p in head.data.polygons: p.use_smooth = True
head['authorship'] = 'Derivative of cleared Blender CC0 realistic male; native sculpt retained and locally edited'
head['not_a_new_original_topology'] = True

def mat(name, col, rough=.5, sss=0):
    m = bpy.data.materials.new(name); m.use_nodes = True
    p = m.node_tree.nodes.get('Principled BSDF')
    p.inputs['Base Color'].default_value = (*col, 1)
    p.inputs['Roughness'].default_value = rough
    p.inputs['Subsurface Weight'].default_value = sss
    p.inputs['Subsurface Radius'].default_value = (1, .42, .22)
    p.inputs['Subsurface Scale'].default_value = .0013
    p.inputs['Specular IOR Level'].default_value = .3
    return m

def g(x,c,r): return math.exp(-((x-c)/r)**2)
def mix(a,b,t): return tuple(a[i]*(1-t)+b[i]*t for i in range(3))
def clamp(v): return max(0,min(1,v))

# Anatomically placed colour/roughness masks, retained as editable mesh attributes.
skin = mat('Skin_region_colour_and_microrelief', (.39,.205,.145), .5, .12)
head.data.materials.clear(); head.data.materials.append(skin)
color = head.data.color_attributes.new(name='SkinPaint', type='FLOAT_COLOR', domain='POINT')
lipattr = head.data.attributes.new(name='Vermilion', type='FLOAT', domain='POINT')
roughattr = head.data.attributes.new(name='RegionRoughness', type='FLOAT', domain='POINT')
for v in head.data.vertices:
    x,y,z = v.co
    front = clamp((-y-.045)/.055)
    c = (.40,.228,.169)
    cheek = (g(x,.048,.023)+g(x,-.048,.023))*g(z,1.548,.019)*front
    nose = g(x,0,.020)*g(z,1.539,.024)*front
    eye = (g(x,.033,.020)+g(x,-.033,.020))*g(z,1.570,.012)*front
    beard = g(x,0,.052)*g(z,1.478,.031)*front
    c = mix(c,(.44,.193,.157),clamp(.32*cheek+.25*nose))
    c = mix(c,(.30,.145,.116),clamp(.24*eye))
    c = mix(c,(.30,.196,.163),clamp(.12*beard))
    # Vermilion follows the native sculpt's lip bow, not a superimposed mouth tube.
    u = abs(x)/.030
    seam = 1.5050 - .0015*min(u,1)**2
    bow = .0048 + .0016*g(abs(x),.008,.005)
    height = bow if z > seam else .0068
    lip = clamp((1-u**3)*4)*math.exp(-((z-(seam+.0001))/(height+.0001))**6)
    lip *= clamp((-y-.12)/.02)
    c = mix(c,(.34,.115,.104),lip*.72)
    # Broad, very low-amplitude pigment variation; not a noisy fake skin texture.
    variation = .99+.012*math.sin(x*153+z*122)*math.sin(y*191-z*91)
    color.data[v.index].color = (*(q*variation for q in c),1)
    lipattr.data[v.index].value = lip
    roughattr.data[v.index].value = .49 - .08*nose - .055*lip + .035*cheek

n = skin.node_tree.nodes; l = skin.node_tree.links; p=n.get('Principled BSDF')
at=n.new('ShaderNodeVertexColor'); at.layer_name='SkinPaint'; l.new(at.outputs['Color'],p.inputs['Base Color'])
ra=n.new('ShaderNodeAttribute'); ra.attribute_name='RegionRoughness'; l.new(ra.outputs['Fac'],p.inputs['Roughness'])
tex=n.new('ShaderNodeTexCoord')
noise=n.new('ShaderNodeTexNoise'); noise.name='Pores_submillimetre'; noise.inputs['Scale'].default_value=2900; noise.inputs['Detail'].default_value=2; noise.inputs['Roughness'].default_value=.58
l.new(tex.outputs['Object'],noise.inputs['Vector'])
bump=n.new('ShaderNodeBump'); bump.name='Skin_18_micron_relief'; bump.inputs['Strength'].default_value=.17; bump.inputs['Distance'].default_value=.000018
l.new(noise.outputs['Fac'],bump.inputs['Height']); l.new(bump.outputs['Normal'],p.inputs['Normal'])

sclera=mat('Sclera_ivory_not_paper_white',(.56,.57,.51),.24,.06)
sn=sclera.node_tree.nodes; sl=sclera.node_tree.links; sp=sn.get('Principled BSDF')
st=sn.new('ShaderNodeTexNoise'); st.inputs['Scale'].default_value=16; st.inputs['Detail'].default_value=2
sr=sn.new('ShaderNodeValToRGB'); sr.color_ramp.elements[0].color=(.43,.40,.35,1); sr.color_ramp.elements[1].color=(.67,.67,.59,1)
sl.new(st.outputs['Fac'],sr.inputs[0]); sl.new(sr.outputs['Color'],sp.inputs['Base Color'])
iris=mat('Iris_hazel_radial_stroma',(.12,.065,.018),.45)
ni=iris.node_tree.nodes; li=iris.node_tree.links; vc=ni.new('ShaderNodeVertexColor'); vc.layer_name='IrisPigment'; li.new(vc.outputs['Color'],ni.get('Principled BSDF').inputs['Base Color'])
pupil=mat('Pupil_cavity',(.001,.0008,.0005),.25)
cornea=mat('Cornea_IOR_1_376',(.98,.99,1),.035)
cp=cornea.node_tree.nodes.get('Principled BSDF'); cp.inputs['Transmission Weight'].default_value=1; cp.inputs['IOR'].default_value=1.376; cp.inputs['Specular IOR Level'].default_value=.5
hairmat=mat('Hair_warm_brown',(.027,.013,.007),.56)
wetmat=mat('Wet_lid_margin',(.36,.16,.125),.16,.06)

def mesh_obj(name, verts, faces, mats):
    me=bpy.data.meshes.new(name); me.from_pydata(verts,[],faces); me.update()
    ob=bpy.data.objects.new(name,me); bpy.context.collection.objects.link(ob)
    for m in mats: me.materials.append(m)
    for p in me.polygons: p.use_smooth=True
    return ob

def build_eye(sign):
    cx=sign*.0329; cy=-.1211; cz=1.5737; radius=.0133
    # Lathed globe with a recessed iris and actual pupil well; no raised iris/pupil balls.
    rings=[i*math.pi/128 for i in range(129)]
    verts=[]; colors=[]
    for t in rings:
        r=math.sin(t); y=-math.cos(t)
        if t<.425:
            y=-.922+.045*(1-(r/.412)**2)
        for j in range(192):
            a=2*math.pi*j/192
            verts.append((cx+radius*r*math.cos(a),cy+radius*y,cz+radius*r*math.sin(a)))
            fibre=.5+.18*math.sin(a*73+math.sin(a*17)*2+r*24)+.16*math.sin(a*127-r*38)
            coll=g(r,.20,.055); rim=g(r,.407,.016)
            co=mix((.045,.034,.014),(.18,.115,.038),clamp(fibre))
            co=mix(co,(.22,.108,.025),coll*.38); co=mix(co,(.011,.017,.013),rim*.8)
            colors.append((*co,1))
    faces=[(i*192+j,i*192+(j+1)%192,(i+1)*192+(j+1)%192,(i+1)*192+j) for i in range(128) for j in range(192)]
    ob=mesh_obj('Eye_Globe_'+str(sign),verts,faces,[sclera,iris,pupil])
    ca=ob.data.color_attributes.new(name='IrisPigment',type='FLOAT_COLOR',domain='POINT')
    for i,c in enumerate(colors): ca.data[i].color=c
    for face in ob.data.polygons:
        t=(face.index//192+.5)*math.pi/128
        face.material_index=2 if t<.154 else 1 if t<.425 else 0
    verts=[]
    for i in range(33):
        r=.46*i/32
        y=-math.sqrt(1-r*r)-.048*(1-(r/.46)**2)**2
        for j in range(128):
            a=j*2*math.pi/128
            verts.append((cx+radius*r*math.cos(a),cy+radius*y,cz+radius*r*math.sin(a)))
    faces=[(i*128+j,i*128+(j+1)%128,(i+1)*128+(j+1)%128,(i+1)*128+j) for i in range(32) for j in range(128)]
    mesh_obj('Corneal_surface_'+str(sign),verts,faces,[cornea])

build_eye(-1); build_eye(1)
bpy.context.view_layer.update()
dg=bpy.context.evaluated_depsgraph_get()
bvh=BVHTree.FromObject(head,dg)
def front(x,z):
    hit,no,ix,dist=bvh.ray_cast(Vector((x,-.4,z)),Vector((0,1,0)),.6)
    return (hit,no) if hit else (Vector((x,-.10,z)),Vector((0,-1,0)))

def strand_object(name, strands, material, radius):
    cu=bpy.data.curves.new(name,'CURVE'); cu.dimensions='3D'; cu.resolution_u=1
    cu.bevel_depth=radius; cu.bevel_resolution=1; cu.resolution_u=2
    for pts in strands:
        sp=cu.splines.new('POLY'); sp.points.add(len(pts)-1)
        for i,v in enumerate(pts):
            sp.points[i].co=(*v,1); sp.points[i].radius=max(.025,1-i/(len(pts)-.4))
    ob=bpy.data.objects.new(name,cu); bpy.context.collection.objects.link(ob); cu.materials.append(material)
    return ob

rng=random.Random(421)
for sign in (-1,1):
    brows=[]
    for i in range(650):
        u=rng.random(); x=sign*(.016+.043*u)
        z=1.598+.0035*math.sin(math.pi*u)-.007*u+rng.gauss(0,.0011)*(1-.5*u)
        root,no=front(x,z); root+=no*.0001
        direction=Vector((sign*(.0035+.0015*u),-.0005,.0035*(1-u)-.0012*u))
        length=rng.uniform(.75,1.2)
        brows.append([root+direction*(t/4)*length+no*(.00055*math.sin(math.pi*t/4)) for t in range(5)])
    strand_object('Brow_fibres_'+str(sign),brows,hairmat,.000033)
    lashes=[]; wet=[]
    for upper in (True,False):
        for i in range(60 if upper else 32):
            u=(i+rng.random())/(60 if upper else 32); x=sign*(.019+.028*u)
            z=1.5734+(.0068 if upper else -.0036)*math.sin(math.pi*u)+.0007*u
            root,no=front(x,z); root+=no*.00008
            length=(.0045 if upper else .0025)*(.7+.3*math.sin(math.pi*u))*rng.uniform(.8,1.2)
            direction=Vector((sign*(u-.3)*.5,-.85,.42 if upper else -.3)).normalized()
            lashes.append([root+direction*length*(t/5)+Vector((0,0,(.0009 if upper else -.0004)*(t/5)**2)) for t in range(6)])
        for i in range(40):
            u=i/39; x=sign*(.019+.028*u); z=1.5734+(.0068 if upper else -.0036)*math.sin(math.pi*u)+.0007*u
            root,no=front(x,z); wet.append(root+no*.000065)
        strand_object(('Upper' if upper else 'Lower')+'_wet_margin_'+str(sign),[wet[-40:]],wetmat,.000085)
    strand_object('Lashes_'+str(sign),lashes,hairmat,.000023)

# Hair guides follow the actual sculpt surface; no extracted shell/cap geometry.
# Root density fades over the hairline. A short swept crop, not a hairstyle system.
tri=head.data.copy(); tri.calc_loop_triangles()
candidates=[]; weights=[]
for t in tri.loop_triangles:
    vs=[tri.vertices[i].co.copy() for i in t.vertices]; center=sum(vs,Vector())/3
    x,y,z=center
    line=1.624 + .016*max(0,min(1,(-y-.045)/.070)) + .010*g(abs(x),.060,.018)*max(0,min(1,(-y-.040)/.065))
    if z>line-.004:
        area=(vs[1]-vs[0]).cross(vs[2]-vs[0]).length/2
        candidates.append((vs,t.normal.copy(),line)); weights.append(area)
hair=[]
for vs,no,line in rng.choices(candidates,weights=weights,k=52000):
    a=rng.random(); b=rng.random()
    if a+b>1: a=1-a; b=1-b
    root=vs[0]+(vs[1]-vs[0])*a+(vs[2]-vs[0])*b
    fade=clamp((root.z-line)/.007)
    if rng.random()>fade: continue
    no.normalize()
    guide=Vector((.32,1,-.25)); tangent=(guide-no*guide.dot(no)).normalized()
    length=rng.uniform(.008,.018)*(0.65+.35*fade)
    pts=[]
    for i in range(7):
        t=i/6
        pts.append(root+no*(.0001+length*(.22*t+.18*math.sin(math.pi*t)))+tangent*(length*t)+Vector((.0002*math.sin(i+root.z*1000),0,0))*t)
    hair.append(pts)
strand_object('Short_crop_surface_groom',hair,hairmat,.000033)
bpy.data.meshes.remove(tri)

# Studio uses broad neutral white lights; no DOF, grading effects, or dark concealment.
scene=bpy.context.scene
world=bpy.data.worlds.new('Neutral_grey_world'); world.use_nodes=True; scene.world=world
world.node_tree.nodes['Background'].inputs[0].default_value=(.18,.18,.18,1)
world.node_tree.nodes['Background'].inputs[1].default_value=.35
def aim(ob,target): ob.rotation_euler=(Vector(target)-ob.location).to_track_quat('-Z','Y').to_euler()
def area(name,pos,power,size):
    d=bpy.data.lights.new(name,'AREA'); d.energy=power; d.shape='DISK'; d.size=size; d.color=(1,1,1)
    ob=bpy.data.objects.new(name,d); bpy.context.collection.objects.link(ob); ob.location=pos; aim(ob,(0,-.02,1.57)); return ob
area('Key_neutral',(-.42,-.55,1.94),45,.42)
area('Fill_neutral',(.50,-.25,1.64),17,.48)
area('Top_neutral',(.10,.20,1.95),26,.38)
camera=bpy.data.objects.new('Diagnostic_camera',bpy.data.cameras.new('Diagnostic_camera')); bpy.context.collection.objects.link(camera); scene.camera=camera
camera.data.clip_start=.005; camera.data.dof.use_dof=False
scene.render.engine='CYCLES'; scene.cycles.samples=48; scene.cycles.use_denoising=True
scene.cycles.max_bounces=8; scene.cycles.transmission_bounces=6
try:
    prefs=bpy.context.preferences.addons['cycles'].preferences; prefs.compute_device_type='OPTIX'; prefs.get_devices()
    gpu=False
    for device in prefs.devices:
        device.use=device.type!='CPU'; gpu |= device.use
    if gpu: scene.cycles.device='GPU'
    print('RENDER_DEVICES',[(d.name,d.type,d.use) for d in prefs.devices])
except Exception as e: print('CPU_RENDER_FALLBACK',repr(e))
scene.view_settings.view_transform='AgX'; scene.view_settings.look='AgX - Medium High Contrast'; scene.view_settings.exposure=0
scene.render.image_settings.file_format='PNG'; scene.render.image_settings.color_mode='RGB'; scene.render.resolution_percentage=100
scene['face_experiment_stage']='initial'
scene['native_sculpt_input']='../anatomy_working.blend'
bpy.ops.wm.save_as_mainfile(filepath=str(STAGE/'face.blend'))
exec(compile((Path(__file__).with_name('render_face.py')).read_text(), 'render_face.py', 'exec'))
