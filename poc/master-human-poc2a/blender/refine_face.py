"""Two deliberately bounded face-only look-development passes.

Pass 1 saves a scene for further native sculpt edits before render_face.py.
Pass 2 is the last artistic pass. Initial and pass-1 evidence is never overwritten.
"""
import bpy
import bmesh
import math
import random
import sys
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree

ROOT=Path(__file__).resolve().parents[3]
BASE=ROOT/'artifacts/master-human-poc2a/face'
args=sys.argv[sys.argv.index('--')+1:] if '--' in sys.argv else ['pass1']
stage=args[0]
assert stage in ('pass1','pass2'), 'Only two refinement passes are authorized'
source='initial' if stage=='pass1' else 'pass1'
bpy.ops.wm.open_mainfile(filepath=str(BASE/source/'face.blend'))
head=bpy.data.objects['Face_NativeSculpt']
scene=bpy.context.scene

if stage=='pass1':
    # Correct eye patch orientation; initial mesh faces were inward-facing.
    for ob in bpy.data.objects:
        if ob.name.startswith(('Eye_Globe','Corneal_surface')):
            bm=bmesh.new(); bm.from_mesh(ob.data); bmesh.ops.reverse_faces(bm,faces=list(bm.faces)); bm.to_mesh(ob.data); bm.free()
        if ob.name.startswith('Corneal_surface'):
            mod=ob.modifiers.new('Corneal_shell_thickness','SOLIDIFY'); mod.thickness=.00022; mod.offset=-1
    bpy.data.objects['Key_neutral'].data.energy=14
    bpy.data.objects['Key_neutral'].data.size=.28
    bpy.data.objects['Fill_neutral'].data.energy=5
    bpy.data.objects['Top_neutral'].data.energy=2.5
    scene.world.node_tree.nodes['Background'].inputs[1].default_value=.22
    # The same revealing white studio, with clipping removed rather than grading.
    skin=bpy.data.materials['Skin_region_colour_and_microrelief']
    p=skin.node_tree.nodes.get('Principled BSDF'); p.inputs['Subsurface Weight'].default_value=.085
    skin.node_tree.nodes['Skin_18_micron_relief'].inputs['Strength'].default_value=.3
    skin.node_tree.nodes['Skin_18_micron_relief'].inputs['Distance'].default_value=.000030
    for v in head.data.vertices:
        c=head.data.color_attributes['SkinPaint'].data[v.index].color
        head.data.color_attributes['SkinPaint'].data[v.index].color=(c[0]*.80,c[1]*.70,c[2]*.66,1)
    # Sclera tint is restrained. The source is a light tissue, not a luminous white.
    sr=next(n for n in bpy.data.materials['Sclera_ivory_not_paper_white'].node_tree.nodes if n.type=='VALTORGB')
    sr.color_ramp.elements[0].color=(.28,.27,.23,1); sr.color_ramp.elements[1].color=(.46,.47,.40,1)
    hair=bpy.data.materials['Hair_warm_brown']; n=hair.node_tree.nodes; l=hair.node_tree.links
    hp=n.new('ShaderNodeBsdfHairPrincipled'); hp.parametrization='MELANIN'
    hp.inputs['Melanin'].default_value=.72; hp.inputs['Melanin Redness'].default_value=.35
    hp.inputs['Roughness'].default_value=.42; hp.inputs['Radial Roughness'].default_value=.48
    hp.inputs['Random Color'].default_value=.14
    l.new(hp.outputs[0],n.get('Material Output').inputs['Surface'])
    # Re-seat the lash root at the lid margin, not the superior lid crease.
    for ob in bpy.data.objects:
        if ob.name.startswith('Lashes_'):
            for sp in ob.data.splines:
                upper=sp.points[0].co.z>1.575
                if upper:
                    for pt in sp.points: pt.co.z-=.0011
    scene.cycles.samples=80

if stage=='pass2':
    def gauss(a,c,r): return math.exp(-((a-c)/r)**2)
    def clamp(a): return max(0,min(1,a))
    def blend(a,b,t): return tuple(a[i]*(1-t)+b[i]*t for i in range(3))
    skin=bpy.data.materials['Skin_region_colour_and_microrelief']
    n=skin.node_tree.nodes; l=skin.node_tree.links; p=n.get('Principled BSDF')
    # Repaint the lower vermilion and region-specific tissue colour on native mesh.
    for v in head.data.vertices:
        x,y,z=v.co; front=clamp((-y-.055)/.05)
        u=abs(x)/.0295; seam=1.5048-.0014*min(u,1)**2
        height=(.0044+.0018*gauss(abs(x),.008,.005)) if z>seam else .0105
        lip=clamp((1-u**3)*4)*math.exp(-((z-seam)/height)**6)*clamp((-y-.125)/.016)
        old=head.data.color_attributes['SkinPaint'].data[v.index].color
        c=blend(old,(.285,.093,.077),lip*.48)
        # Tiny, individually placed pigmentation marks; these are not scans.
        marks=[(-.051,1.552,.00085,.28),(.060,1.535,.00060,.23),(-.027,1.623,.00055,.14),(.017,1.638,.0005,.1)]
        for mx,mz,r,w in marks:
            c=blend(c,(.095,.043,.026),gauss(x,mx,r)*gauss(z,mz,r)*front*w)
        cheek=(gauss(x,.047,.022)+gauss(x,-.045,.026))*gauss(z,1.548,.023)*front
        c=blend(c,(.34,.135,.107),.12*cheek)
        head.data.color_attributes['SkinPaint'].data[v.index].color=(*c,1)
        head.data.attributes['Vermilion'].data[v.index].value=lip
        nose=gauss(x,0,.022)*gauss(z,1.543,.035)*front
        head.data.attributes['RegionRoughness'].data[v.index].value=.53-.12*nose-.16*lip
    tc=next(q for q in n if q.type=='TEX_COORD')
    pores=n.new('ShaderNodeTexVoronoi'); pores.name='Sparse_epidermal_pores_0_4mm_spacing'; pores.inputs['Scale'].default_value=2400
    l.new(tc.outputs['Object'],pores.inputs['Vector'])
    pr=n.new('ShaderNodeValToRGB'); pr.name='Individual_pore_dimples_not_cloud_noise'
    pr.color_ramp.interpolation='EASE'
    pr.color_ramp.elements[0].position=.035; pr.color_ramp.elements[0].color=(0,0,0,1)
    pr.color_ramp.elements[1].position=.22; pr.color_ramp.elements[1].color=(1,1,1,1)
    l.new(pores.outputs['Distance'],pr.inputs[0])
    pb=n.new('ShaderNodeBump'); pb.name='Fine_pore_depth_65_microns'; pb.inputs['Distance'].default_value=.00013; pb.inputs['Strength'].default_value=.50
    l.new(pr.outputs['Color'],pb.inputs['Height'])
    oldb=n.get('Skin_18_micron_relief'); oldb.inputs['Distance'].default_value=.000018; oldb.inputs['Strength'].default_value=.15
    l.new(oldb.outputs['Normal'],pb.inputs['Normal'])
    # Fine broken vertical lip relief, gated by the repainted native vermilion mask.
    wave=n.new('ShaderNodeTexWave'); wave.bands_direction='X'; wave.inputs['Scale'].default_value=550; wave.inputs['Distortion'].default_value=1.8; wave.inputs['Detail Scale'].default_value=1.4
    l.new(tc.outputs['Object'],wave.inputs['Vector'])
    va=n.new('ShaderNodeAttribute'); va.attribute_name='Vermilion'
    mul=n.new('ShaderNodeMath'); mul.operation='MULTIPLY'; l.new(wave.outputs['Fac'],mul.inputs[0]); l.new(va.outputs['Fac'],mul.inputs[1])
    lb=n.new('ShaderNodeBump'); lb.name='Fine_vermilion_folds'; lb.inputs['Distance'].default_value=.000070; lb.inputs['Strength'].default_value=.35
    l.new(mul.outputs[0],lb.inputs['Height']); l.new(pb.outputs['Normal'],lb.inputs['Normal']); l.new(lb.outputs['Normal'],p.inputs['Normal'])
    p.inputs['Specular IOR Level'].default_value=.28
    # Blend iris-to-globe depth continuously, removing the hard pale ledge.
    for ob in bpy.data.objects:
        if not ob.name.startswith('Eye_Globe_'): continue
        sign=1 if ob.name.endswith('_1') else -1; cx=sign*.0329; cy=-.1211; cz=1.5737; radius=.0133
        for v in ob.data.vertices:
            xx=(v.co.x-cx)/radius; zz=(v.co.z-cz)/radius; r=math.sqrt(xx*xx+zz*zz)
            if v.co.y<cy and r<.50:
                sph=-math.sqrt(max(0,1-r*r)); plane=-.91+.042*(1-(r/.44)**2)
                t=clamp((r-.43)/.07); t=t*t*(3-2*t)
                v.co.y=cy+radius*(plane*(1-t)+sph*t)
                a=math.atan2(zz,xx)
                fibre=clamp(.48+.21*math.sin(a*73+math.sin(a*17)*2+r*24)+.12*math.sin(a*127-r*38))
                co=blend((.025,.030,.011),(.105,.079,.025),fibre)
                co=blend(co,(.14,.064,.016),gauss(r,.21,.04)*.34)
                co=blend(co,(.009,.014,.009),clamp((r-.385)/.055)*.94)
                ob.data.color_attributes['IrisPigment'].data[v.index].color=(*co,1)
        for face in ob.data.polygons:
            t=(face.index//192+.5)*math.pi/128
            face.material_index=2 if t<.154 else 1 if t<.46 else 0
    # Clear meniscus, not a pink graphic line over the sculpt.
    for ob in bpy.data.objects:
        if ob.name.startswith('Upper_wet_margin'): ob.hide_render=True
        if ob.name.startswith('Lower_wet_margin'): ob.data.bevel_depth=.000055
    # Less uniformly parallel brow growth and a less rigid short crop.
    rng=random.Random(8301)
    for ob in bpy.data.objects:
        if ob.name.startswith('Brow_fibres'):
            for sp in ob.data.splines:
                root=sp.points[0].co.copy(); factor=rng.uniform(.60,1.08); bend=rng.uniform(-.001,.001)
                for i,pt in enumerate(sp.points):
                    u=i/(len(sp.points)-1); delta=pt.co-root; pt.co=root+delta*factor; pt.co.z+=bend*u*u
            ob.data.bevel_depth=.000030
        if ob.name=='Short_crop_surface_groom':
            for sp in ob.data.splines:
                root=sp.points[0].co.copy(); factor=rng.uniform(.75,1.13)
                for i,pt in enumerate(sp.points):
                    u=i/(len(sp.points)-1); pt.co=root+(pt.co-root)*factor; pt.co.x+=.00065*math.sin(root.x*290+root.z*84)*u*u
                sp.type='NURBS'; sp.order_u=3; sp.use_endpoint_u=True
            ob.data.resolution_u=3
    hp=next(q for q in bpy.data.materials['Hair_warm_brown'].node_tree.nodes if q.type=='BSDF_HAIR_PRINCIPLED')
    hp.inputs['Melanin'].default_value=.90; hp.inputs['Roughness'].default_value=.5
    # Short side growth gives the crop a graded temple transition, not a cap edge.
    bpy.context.view_layer.update(); bvh=BVHTree.FromObject(head,bpy.context.evaluated_depsgraph_get())
    cu=bpy.data.curves.new('Temple_fade','CURVE'); cu.dimensions='3D'; cu.bevel_depth=.000029; cu.bevel_resolution=1
    for sign in (-1,1):
        for i in range(4200):
            y=rng.uniform(-.080,.042); z=rng.uniform(1.588,1.652)
            if rng.random()>clamp((z-1.588)/.043): continue
            hit,no,ix,dist=bvh.ray_cast(Vector((sign*.18,y,z)),Vector((-sign,0,0)),.20)
            if not hit or abs(hit.x)<.05: continue
            length=rng.uniform(.001,.004)*clamp((z-1.583)/.035)
            tangent=Vector((0,.25,-1)); tangent=(tangent-no*tangent.dot(no)).normalized()
            sp=cu.splines.new('POLY'); sp.points.add(3)
            for j,pt in enumerate(sp.points):
                t=j/3; pos=hit+no*(.00008+length*.4*t)+tangent*length*t
                pt.co=(*pos,1); pt.radius=max(.03,1-t)
    ob=bpy.data.objects.new('Temple_fade',cu); bpy.context.collection.objects.link(ob); cu.materials.append(bpy.data.materials['Hair_warm_brown'])
    scene.cycles.samples=112

scene['face_experiment_stage']=stage
out=BASE/stage; out.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.save_as_mainfile(filepath=str(out/'face.blend'))
print('READY_FOR_NATIVE_ART_OR_RENDER',out/'face.blend',flush=True)
