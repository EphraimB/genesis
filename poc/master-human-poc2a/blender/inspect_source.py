"""Read-only anatomy/topology probe for the previously cleared CC0 bundle."""
import bpy
import bmesh
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
BUNDLE = ROOT / 'poc/master-human/vendor/blender-human-base-meshes/human-base-meshes-bundle-v1.4.1/human_base_meshes_bundle.blend'
OUT = ROOT / 'artifacts/master-human-poc2a/inspection'
OUT.mkdir(parents=True, exist_ok=True)
names = ['GEO-body_male_realistic', 'GEO-body_male_realistic.eye.L', 'GEO-body_male_realistic.eye.R']
with bpy.data.libraries.load(str(BUNDLE), link=False) as (source,target):
    target.objects = [n for n in names if n in source.objects]
report = {}
for obj in target.objects:
    if obj is None: continue
    bpy.context.collection.objects.link(obj)
    mesh=obj.data
    bm=bmesh.new(); bm.from_mesh(mesh); bm.verts.ensure_lookup_table()
    boundaries=[v for v in bm.verts if any(e.is_boundary for e in v.link_edges)]
    report[obj.name]={'location':list(obj.location),'dimensions':list(obj.dimensions),'vertices':len(mesh.vertices),
        'modifiers':[(m.name,m.type) for m in obj.modifiers],
        'bounds':[[min(v.co[a] for v in mesh.vertices),max(v.co[a] for v in mesh.vertices)] for a in range(3)],
        'faceBoundary':[[v.index,*v.co] for v in boundaries if v.co.z>1.42],
        'faceVertices':[[v.index,*v.co] for v in mesh.vertices if v.co.z>1.42 and v.co.y<-.07]}
    bm.free()
(OUT/'source.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps({k:{a:v[a] for a in ('location','dimensions','vertices','bounds','modifiers','faceBoundary')} for k,v in report.items()}))
