import bpy

print("GENESIS_BUNDLE_INSPECTION_BEGIN")
for collection in bpy.data.collections:
    print(f"COLLECTION|{collection.name}|objects={len(collection.objects)}")
for obj in bpy.data.objects:
    data = obj.data
    vertices = len(data.vertices) if hasattr(data, "vertices") else 0
    polygons = len(data.polygons) if hasattr(data, "polygons") else 0
    print(f"OBJECT|{obj.name}|{obj.type}|vertices={vertices}|polygons={polygons}")
print("GENESIS_BUNDLE_INSPECTION_END")
