"""Complete the interrupted final face pass; not a third artistic pass.

Input is the saved, interactively inflated lower-lip sculpt in Pass 2.
The earlier initialized scene is retained under pass2/progress_01.
Pass 1 is read only. No body, rig, export, or Genesis integration work.
"""
import math
import random
from pathlib import Path

import bpy
import numpy as np
from mathutils import Vector, noise

ROOT = Path(__file__).resolve().parents[3]
BASE = ROOT / 'artifacts/master-human-poc2a/face'
OUT = BASE / 'pass2'
bpy.ops.wm.open_mainfile(filepath=str(OUT / 'native_lip_sculpt.blend'))
if bpy.context.object and bpy.context.object.mode != 'OBJECT':
    bpy.ops.object.mode_set(mode='OBJECT')
head = bpy.data.objects['Face_NativeSculpt']

# Restore the entire saved Pass 1 eye/brow system, not a new eye experiment.
prefixes = ('Eye_Globe_', 'Corneal_surface_', 'Upper_wet_margin_',
            'Lower_wet_margin_', 'Lashes_', 'Brow_fibres_')
for ob in list(bpy.data.objects):
    if ob.name.startswith(prefixes):
        bpy.data.objects.remove(ob, do_unlink=True)
with bpy.data.libraries.load(str(BASE / 'pass1/face.blend'), link=False) as (src, dst):
    dst.objects = [name for name in src.objects if name.startswith(prefixes) or name == 'Face_NativeSculpt']
for ob in dst.objects:
    if ob.name.startswith('Face_NativeSculpt'):
        # The observed native Inflate stroke moved 96 lip vertices by at most
        # 0.023 mm. Amplify that artist-drawn footprint to a restrained 0.42 mm,
        # rather than replacing it with a new generic parametric lip shape.
        for v in head.data.vertices:
            delta = v.co - ob.data.vertices[v.index].co
            if delta.length > 1e-8:
                assert abs(v.co.x) < .016 and 1.492 < v.co.z < 1.501
                v.co += delta * 17
        bpy.data.objects.remove(ob, do_unlink=True)
        continue
    bpy.context.collection.objects.link(ob)
    ob['pass2_policy'] = 'Unmodified object appended from preserved Pass 1'

def g(x, c, r):
    return math.exp(-((x - c) / r) ** 2)

def clamp(x):
    return max(0.0, min(1.0, x))

def smooth(x):
    x = clamp(x)
    return x * x * (3 - 2 * x)

def mix(a, b, t):
    return tuple(a[i] * (1 - t) + b[i] * t for i in range(3))

def attr(name):
    return head.data.attributes.get(name) or head.data.attributes.new(name, 'FLOAT', 'POINT')

pores = attr('PoreRegion')
lipattr = attr('Vermilion')
rough = attr('RegionRoughness')
paint = head.data.color_attributes['SkinPaint']
# Sparse deliberately placed pigment marks; no imported skin or scan texture.
marks = [(-.051, 1.552, .00085, .22), (.060, 1.535, .0006, .18),
         (-.027, 1.623, .00055, .12), (.017, 1.638, .0005, .08),
         (.041, 1.549, .00037, .08), (-.057, 1.542, .00045, .09)]
for v in head.data.vertices:
    x, y, z = v.co
    front = smooth((-y - .045) / .055)
    cheek = (g(x, .046, .026) + g(x, -.049, .023)) * g(z, 1.548, .024) * front
    nose = g(x, 0, .021) * g(z, 1.545, .028) * front
    eye = (g(x, .033, .020) + g(x, -.033, .020)) * g(z, 1.573, .011) * front
    forehead = g(x, 0, .065) * g(z, 1.628, .022) * front
    beard = g(x, 0, .053) * g(z, 1.474, .032) * front
    u = min(1.0, abs(x) / .0302)
    seam = 1.502 - .0015 * u * u + .00022 * math.sin(x * 77)
    upper = seam + .010 * (1-u*u)**.8 + .0018*g(abs(x), .008, .004) - .0012*g(x, 0, .004)
    lower = seam - .0112 * (1-u*u)**.65
    lip = smooth((z-lower)/.0012) * smooth((upper-z)/.0011)
    lip *= smooth((.0302-abs(x))/.003) * smooth((-y-.125)/.014)
    # Same identity/pigment family as Pass 1, with tissue-specific variation.
    c = (.320, .1596, .1115)
    c = mix(c, (.350, .130, .100), clamp(.42*cheek+.28*nose))
    c = mix(c, (.255, .119, .089), .20*eye)
    c = mix(c, (.252, .147, .120), .22*beard)
    c = mix(c, (.327, .163, .113), .15*forehead)
    mottling = noise.noise_vector(Vector((x*210, y*210, z*210)))
    c = tuple(c[i] * (1 + .048*mottling[i]) for i in range(3))
    broad = noise.noise_vector(Vector((x*85+3.1, y*85, z*85)))
    c = tuple(c[i] * (1 + .035*broad[i]) for i in range(3))
    lipcolor = (.282, .096, .078) if z > seam else (.309, .122, .101)
    c = mix(c, lipcolor, .72*lip)
    for mx, mz, radius, weight in marks:
        c = mix(c, (.095, .043, .026), g(x,mx,radius)*g(z,mz,radius)*front*weight)
    # Slight warmth inside commissures, without drawing a dark outline.
    corners = g(abs(x), .0272, .0023)*g(z,1.501,.0023)*front
    c = mix(c, (.237,.105,.081), corners*.12)
    paint.data[v.index].color = (*c, 1)
    lipattr.data[v.index].value = lip
    rough.data[v.index].value = .525 - .105*nose - .038*forehead + .025*cheek - .025*eye - .115*lip
    pores.data[v.index].value = clamp((.18 + .55*cheek + .68*nose + .12*forehead)*(1-.88*eye)*(1-lip))

skin = head.data.materials[0]
n = skin.node_tree.nodes
l = skin.node_tree.links
n.clear()
def node(kind, name):
    v = n.new(kind)
    v.name = name
    v.label = name
    return v

p = node('ShaderNodeBsdfPrincipled', 'Skin_tissue_response')
p.inputs['Subsurface Weight'].default_value = .10
p.inputs['Subsurface Radius'].default_value = (1, .48, .25)
p.inputs['Subsurface Scale'].default_value = .0010
p.inputs['Specular IOR Level'].default_value = .30
output = node('ShaderNodeOutputMaterial', 'Skin_output')
l.new(p.outputs[0], output.inputs['Surface'])
color = node('ShaderNodeVertexColor', 'Region_pigment_paint'); color.layer_name = 'SkinPaint'
l.new(color.outputs['Color'], p.inputs['Base Color'])
tc = node('ShaderNodeTexCoord', 'Metric_coordinates')
ra = node('ShaderNodeAttribute', 'Tissue_roughness'); ra.attribute_name = 'RegionRoughness'
rn = node('ShaderNodeTexNoise', 'Subtle_roughness_breakup_2mm'); rn.inputs['Scale'].default_value=480
l.new(tc.outputs['Object'],rn.inputs['Vector'])
rr = node('ShaderNodeMapRange', 'Roughness_variation_plus_minus_0_018')
rr.inputs['To Min'].default_value=-.035; rr.inputs['To Max'].default_value=.035
l.new(rn.outputs['Fac'],rr.inputs['Value'])
add = node('ShaderNodeMath','Region_plus_micro_roughness'); add.operation='ADD'
l.new(ra.outputs['Fac'],add.inputs[0]); l.new(rr.outputs[0],add.inputs[1]); l.new(add.outputs[0],p.inputs['Roughness'])

pn = node('ShaderNodeTexVoronoi', 'Pore_centres_0_55mm_apart'); pn.inputs['Scale'].default_value=1800
l.new(tc.outputs['Object'], pn.inputs['Vector'])
pr = node('ShaderNodeValToRGB', 'Pore_diameter_about_0_2mm')
pr.color_ramp.interpolation='EASE'
pr.color_ramp.elements[0].position=.035; pr.color_ramp.elements[0].color=(0,0,0,1)
pr.color_ramp.elements[1].position=.22; pr.color_ramp.elements[1].color=(1,1,1,1)
l.new(pn.outputs['Distance'],pr.inputs[0])
pa = node('ShaderNodeAttribute','Pore_strength_by_region'); pa.attribute_name='PoreRegion'
pb = node('ShaderNodeBump','Regional_pores_maximum_38_microns')
pb.inputs['Distance'].default_value=.000038
l.new(pa.outputs['Fac'],pb.inputs['Strength']); l.new(pr.outputs['Color'],pb.inputs['Height'])
# Finer epidermal relief breaks the pore-free flat plateaus without making
# identical follicle dimples over eyelids and lips. Distances are in metres.
micro = node('ShaderNodeTexNoise', 'Epidermal_relief_0_3mm')
micro.inputs['Scale'].default_value=3400; micro.inputs['Detail'].default_value=2.4
micro.inputs['Roughness'].default_value=.68
l.new(tc.outputs['Object'],micro.inputs['Vector'])
mb = node('ShaderNodeBump','Epidermal_relief_12_microns')
mb.inputs['Strength'].default_value=.65; mb.inputs['Distance'].default_value=.000018
l.new(micro.outputs['Fac'],mb.inputs['Height']); l.new(mb.outputs['Normal'],pb.inputs['Normal'])

# Uneven, individually laid out vermilion creases, packed inside the .blend.
# This is a height field for the 3D material, not retouching a diagnostic render.
w, h = 1536, 1024
xx = np.linspace(-.04,.04,w,dtype=np.float32)[None,:]
zz = np.linspace(1.480,1.525,h,dtype=np.float32)[:,None]
height = np.ones((h,w),dtype=np.float32)
# x position, centre z, half-length, width, lean, depth; deliberately non-periodic.
strokes = [(-.025,1.500,.0020,.00010,-.15,.30),(-.0218,1.4985,.0034,.00011,.08,.48),
           (-.0184,1.4972,.0026,.00009,-.08,.34),(-.0141,1.4968,.0040,.00013,.05,.56),
           (-.0107,1.4978,.0022,.00009,.11,.40),(-.0068,1.4957,.0044,.00012,-.04,.48),
           (-.0029,1.4975,.0031,.00010,.12,.43),(.0015,1.4963,.0041,.00012,-.08,.55),
           (.0057,1.4971,.0027,.00009,.04,.35),(.0104,1.4968,.0037,.00013,-.10,.50),
           (.0140,1.4984,.0029,.00010,.14,.39),(.0188,1.4977,.0035,.00012,.10,.44),
           (.0233,1.4998,.0019,.00009,-.13,.32),
           (-.019,1.5070,.0018,.00009,.14,.26),(-.013,1.5088,.0020,.00010,-.12,.32),
           (-.005,1.5098,.0022,.00009,.08,.28),(.0052,1.5100,.0018,.00010,-.08,.30),
           (.0125,1.5092,.0017,.00009,.12,.26),(.0201,1.5068,.0014,.00008,-.15,.22)]
for i,(cx,cz,length,width,lean,depth) in enumerate(strokes):
    dz = zz-cz
    centre = cx + lean*dz + .00013*np.sin(dz*850+i*1.7)
    groove = np.exp(-((xx-centre)/width)**2) * np.exp(-(dz/length)**6)
    height -= depth*groove
    if i in (1,3,7,9,11):
        branch = cx + .00035 + (lean+.24)*dz
        height -= depth*.36*np.exp(-((xx-branch)/(width*.72))**2)*np.exp(-((dz-.0008)/(length*.6))**4)
pixels = np.ones((h,w,4),dtype=np.float32)
pixels[:,:,:3] = height[:,:,None]
im = bpy.data.images.new('Artist_laid_vermilion_microfolds',width=w,height=h,float_buffer=True)
im.colorspace_settings.name='Non-Color'; im.pixels.foreach_set(pixels.ravel()); im.pack()
sep = node('ShaderNodeSeparateXYZ','Lip_metric_projection'); l.new(tc.outputs['Object'],sep.inputs[0])
mx = node('ShaderNodeMapRange','Lip_x'); mx.inputs['From Min'].default_value=-.04; mx.inputs['From Max'].default_value=.04
mz = node('ShaderNodeMapRange','Lip_z'); mz.inputs['From Min'].default_value=1.480; mz.inputs['From Max'].default_value=1.525
l.new(sep.outputs['X'],mx.inputs['Value']); l.new(sep.outputs['Z'],mz.inputs['Value'])
uv = node('ShaderNodeCombineXYZ','Lip_projection'); l.new(mx.outputs[0],uv.inputs['X']); l.new(mz.outputs[0],uv.inputs['Y'])
it = node('ShaderNodeTexImage','Packed_nonperiodic_lip_relief'); it.image=im; it.extension='EXTEND'
l.new(uv.outputs[0],it.inputs['Vector'])
la = node('ShaderNodeAttribute','Vermilion_only'); la.attribute_name='Vermilion'
lb = node('ShaderNodeBump','Subtle_lip_creases_40_microns'); lb.inputs['Distance'].default_value=.000040
l.new(la.outputs['Fac'],lb.inputs['Strength']); l.new(it.outputs['Color'],lb.inputs['Height'])
l.new(pb.outputs['Normal'],lb.inputs['Normal']); l.new(lb.outputs['Normal'],p.inputs['Normal'])

# Preserve the sweep of the existing groom, tapering only its emergence zone.
rng = random.Random(20307)
groom = bpy.data.objects['Short_crop_surface_groom']
cu = groom.data
removed = 0
for sp in list(cu.splines):
    root = sp.points[0].co.copy()
    x,y,z = root[:3]
    frontness = clamp((-y-.040)/.070)
    line = 1.624 + .016*clamp((-y-.045)/.070) + .010*g(abs(x),.060,.018)*clamp((-y-.040)/.065)
    irregular = (.0015*math.sin(x*173+.5) + .0008*math.sin(x*431+1.2)
                 + .0023*g(x,.043,.012) - .0011*g(x,-.034,.016))*frontness
    edge = z-line-irregular
    if edge > .014:
        continue
    survival = smooth((edge+.001)/.011)
    if rng.random() > survival:
        cu.splines.remove(sp); removed += 1; continue
    factor = (.25+.75*smooth((edge+.001)/.012))*rng.uniform(.78,1.05)
    bend = rng.uniform(-.0010,.0010)*(1-survival)
    for j,pt in enumerate(sp.points):
        t=j/(len(sp.points)-1)
        pt.co=root+(pt.co-root)*factor
        pt.co.x += bend*t*t
        pt.radius *= .60+.40*survival

# Fine irregular boundary follicles sampled from the actual head surface.
mesh=head.data.copy(); mesh.calc_loop_triangles()
candidates=[]; weights=[]
for tri in mesh.loop_triangles:
    vs=[mesh.vertices[i].co.copy() for i in tri.vertices]
    center=sum(vs,Vector())/3; x,y,z=center
    line=1.624+.016*clamp((-y-.045)/.070)+.010*g(abs(x),.060,.018)*clamp((-y-.040)/.065)
    if y<-.035 and line-.005<z<line+.012:
        candidates.append((vs,tri.normal.copy(),line)); weights.append(tri.area)
baby=bpy.data.curves.new('Hairline_fine_transition','CURVE'); baby.dimensions='3D'; baby.bevel_depth=.000016; baby.bevel_resolution=1; baby.resolution_u=3
for vs,no,line in rng.choices(candidates,weights=weights,k=2600):
    a,b=rng.random(),rng.random()
    if a+b>1: a,b=1-a,1-b
    root=vs[0]+(vs[1]-vs[0])*a+(vs[2]-vs[0])*b
    x,y,z=root
    irregular=.0015*math.sin(x*173+.5)+.0008*math.sin(x*431+1.2)+.0023*g(x,.043,.012)-.0011*g(x,-.034,.016)
    emergence=smooth((z-line-irregular+.004)/.013)
    if rng.random() > (.12+.65*emergence): continue
    guide=Vector((.32+rng.uniform(-.12,.12),1,-.25))
    tangent=(guide-no*guide.dot(no)).normalized()
    length=rng.uniform(.0012,.0048)*(.5+.5*emergence)
    sp=baby.splines.new('NURBS'); sp.points.add(4); sp.order_u=3; sp.use_endpoint_u=True
    for j,pt in enumerate(sp.points):
        t=j/4; pos=root+no*(.000045+length*.22*math.sin(math.pi*t))+tangent*length*t
        pt.co=(*pos,1); pt.radius=max(.03,1-t)
bpy.data.meshes.remove(mesh)
ob=bpy.data.objects.new('Hairline_fine_transition',baby); bpy.context.collection.objects.link(ob)
baby.materials.append(groom.data.materials[0])
hp=next(q for q in groom.data.materials[0].node_tree.nodes if q.type=='BSDF_HAIR_PRINCIPLED')
hp.inputs['Melanin Redness'].default_value=.18
hp.inputs['Random Color'].default_value=.20

scene=bpy.context.scene
scene.cycles.samples=128
scene['face_experiment_stage']='pass2_final'
scene['pass2_native_edit']='Localized lower-lip Inflate stroke, radius 36px, strength 0.18; recorded in native_lip_sculpt.blend; footprint gain 18 for maximum 0.42mm displacement'
scene['pass2_final_scope']='Regional skin, lower lip, hairline only. Eye/brow objects restored unchanged from Pass 1.'
bpy.context.view_layer.objects.active=head
head.select_set(True)
bpy.ops.wm.save_as_mainfile(filepath=str(OUT/'face.blend'))
print('PASS2_FINAL_SCENE_SAVED', OUT/'face.blend', 'thinned_edge_strands',removed,flush=True)
