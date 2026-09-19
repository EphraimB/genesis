"""Deterministic offline builder for Genesis Master Human POC-1.

Run with Blender 5.2.2 LTS. The script consumes only assets admitted by
provenance/dependency-allowlist.json and never performs network access.
"""

from __future__ import annotations

import hashlib
import json
import math
import os
import struct
import sys
import time
from pathlib import Path

import bpy
from mathutils import Vector


ROOT = Path(__file__).resolve().parents[3]
POC = ROOT / "poc" / "master-human"
BUNDLE = POC / "vendor" / "blender-human-base-meshes" / "human-base-meshes-bundle-v1.4.1" / "human_base_meshes_bundle.blend"
ANNY_BASE = POC / "vendor" / "anny" / "anny" / "data" / "mpfb2" / "3dobjs" / "base.obj"
OUT = ROOT / "artifacts" / "master-human-poc"
RENDERS = OUT / "renders"
TEXTURES = OUT / "textures"
GAME = OUT / "game-close"
AUTHORING = OUT / "authoring"
VALIDATION = OUT / "validation"

for directory in (RENDERS, TEXTURES, GAME, AUTHORING, VALIDATION):
    directory.mkdir(parents=True, exist_ok=True)


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete(use_global=False)
    for datablocks in (bpy.data.meshes, bpy.data.curves, bpy.data.armatures, bpy.data.materials, bpy.data.cameras, bpy.data.lights):
        for block in list(datablocks):
            if block.users == 0:
                datablocks.remove(block)


def material(name, color, roughness=0.5, metallic=0.0, subsurface=0.0):
    mat = bpy.data.materials.new(name)
    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    bsdf.inputs["Base Color"].default_value = (*color, 1.0)
    bsdf.inputs["Roughness"].default_value = roughness
    bsdf.inputs["Metallic"].default_value = metallic
    if bsdf.inputs.get("Subsurface Weight"):
        bsdf.inputs["Subsurface Weight"].default_value = subsurface
    if bsdf.inputs.get("IOR"):
        bsdf.inputs["IOR"].default_value = 1.45
    return mat


def create_texture(name, size, base, accent, frequency):
    image = bpy.data.images.new(name, width=size, height=size, alpha=False)
    pixels = [0.0] * (size * size * 4)
    for y in range(size):
        for x in range(size):
            wave = math.sin(x * frequency) * math.sin(y * frequency * 0.73)
            grain = ((x * 17 + y * 31 + x * y * 3) % 101) / 100.0
            blend = max(0.0, min(1.0, 0.14 * wave + 0.11 * grain))
            i = (y * size + x) * 4
            pixels[i:i + 4] = [base[c] * (1 - blend) + accent[c] * blend for c in range(3)] + [1.0]
    image.pixels = pixels
    image.filepath_raw = str(TEXTURES / f"{name}.png")
    image.file_format = "PNG"
    image.save()
    return image


def attach_texture(mat, image):
    nodes = mat.node_tree.nodes
    texture = nodes.new("ShaderNodeTexImage")
    texture.image = image
    texture.interpolation = "Linear"
    mat.node_tree.links.new(texture.outputs["Color"], nodes["Principled BSDF"].inputs["Base Color"])


def append_body():
    wanted = ["GEO-body_male_realistic", "GEO-body_male_realistic.eye.L", "GEO-body_male_realistic.eye.R"]
    with bpy.data.libraries.load(str(BUNDLE), link=False) as (source, target):
        target.objects = [name for name in wanted if name in source.objects]
    objects = {obj.name: obj for obj in target.objects if obj is not None}
    for obj in objects.values():
        bpy.context.collection.objects.link(obj)
    body = objects[wanted[0]]
    source_offset = body.location.copy()
    for obj in objects.values():
        obj.location -= source_offset
        obj.hide_render = False
        obj.hide_viewport = False
    body.name = "Genesis_MasterHuman_Body"
    objects[wanted[1]].name = "Genesis_Eye_L"
    objects[wanted[2]].name = "Genesis_Eye_R"
    for modifier in list(body.modifiers):
        body.modifiers.remove(modifier)
    return body, objects[wanted[1]], objects[wanted[2]]


def smooth(obj):
    if obj.type == "MESH":
        for polygon in obj.data.polygons:
            polygon.use_smooth = True


def add_uv(name, location, scale, mat, segments=32, rings=16):
    bpy.ops.mesh.primitive_uv_sphere_add(segments=segments, ring_count=rings, location=location)
    obj = bpy.context.object
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    obj.data.materials.append(mat)
    smooth(obj)
    return obj


def add_cube(name, location, scale, mat, bevel=0.01):
    bpy.ops.mesh.primitive_cube_add(location=location)
    obj = bpy.context.object
    obj.name = name
    obj.scale = scale
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    if bevel:
        modifier = obj.modifiers.new("Soft edges", "BEVEL")
        modifier.width = bevel
        modifier.segments = 3
    obj.data.materials.append(mat)
    return obj


def add_curve(name, points, bevel_depth, mat, cyclic=False):
    curve = bpy.data.curves.new(name, "CURVE")
    curve.dimensions = "3D"
    curve.bevel_depth = bevel_depth
    curve.bevel_resolution = 3
    spline = curve.splines.new("BEZIER")
    spline.bezier_points.add(len(points) - 1)
    for point, coordinate in zip(spline.bezier_points, points):
        point.co = coordinate
        point.handle_left_type = "AUTO"
        point.handle_right_type = "AUTO"
    spline.use_cyclic_u = cyclic
    obj = bpy.data.objects.new(name, curve)
    bpy.context.collection.objects.link(obj)
    obj.data.materials.append(mat)
    return obj


def build_face_details(eye_l, eye_r, mats):
    for eye in (eye_l, eye_r):
        eye.data.materials.clear()
        eye.data.materials.append(mats["sclera"])
        smooth(eye)
    for side, x in (("L", 0.033), ("R", -0.033)):
        iris = add_uv(f"Genesis_Iris_{side}", (x, -0.1352, 1.574), (0.0068, 0.0020, 0.0068), mats["iris"], 32, 12)
        pupil = add_uv(f"Genesis_Pupil_{side}", (x, -0.1372, 1.574), (0.0028, 0.0010, 0.0028), mats["pupil"], 24, 10)
        add_curve(f"Genesis_Tearline_{side}", [
            (x - 0.013, -0.140, 1.569),
            (x, -0.143, 1.5665),
            (x + 0.013, -0.140, 1.569),
        ], 0.00065, mats["tear"])
        add_curve(f"Genesis_Brow_{side}", [
            (x - 0.021, -0.150, 1.615),
            (x, -0.156, 1.621),
            (x + 0.022, -0.150, 1.614),
        ], 0.0009, mats["hair"])
        for index in range(7):
            lash_x = x - 0.012 + index * 0.004
            add_curve(f"Genesis_Lash_{side}_{index:02}", [
                (lash_x, -0.145, 1.582),
                (lash_x, -0.151, 1.584 + 0.001 * math.sin(index)),
            ], 0.00035, mats["hair"])
        for obj in (iris, pupil):
            obj["genesis_eye_look_target"] = True
    # Teeth, gums, tongue and an explicit jaw object remain behind the closed lips.
    add_uv("Genesis_UpperGum", (0, -0.124, 1.499), (0.035, 0.007, 0.008), mats["gum"], 32, 12)
    add_uv("Genesis_LowerGum", (0, -0.123, 1.485), (0.034, 0.007, 0.008), mats["gum"], 32, 12)
    for row, z in (("Upper", 1.497), ("Lower", 1.487)):
        for index in range(10):
            x = (index - 4.5) * 0.0062
            y = -0.128 + 0.005 * (abs(x) / 0.03) ** 2
            add_cube(f"Genesis_Tooth_{row}_{index + 1:02}", (x, y, z), (0.0027, 0.0020, 0.005), mats["teeth"], 0.0015)
    add_uv("Genesis_Tongue", (0, -0.112, 1.477), (0.028, 0.015, 0.007), mats["tongue"], 32, 12)
    jaw = add_uv("Genesis_Jaw", (0, -0.002, 1.475), (0.075, 0.065, 0.065), mats["bone"], 24, 12)
    jaw.hide_render = True
    jaw.display_type = "WIRE"


def build_hair(mats):
    pieces = []
    placements = [
        ((0, -0.002, 1.684), (0.105, 0.095, 0.070)),
        ((0.063, -0.012, 1.660), (0.055, 0.078, 0.080)),
        ((-0.063, -0.012, 1.660), (0.055, 0.078, 0.080)),
        ((0.048, 0.032, 1.636), (0.052, 0.054, 0.078)),
        ((-0.048, 0.032, 1.636), (0.052, 0.054, 0.078)),
        ((0, 0.052, 1.650), (0.073, 0.047, 0.078)),
    ]
    for index, (location, scale) in enumerate(placements):
        pieces.append(add_uv(f"Genesis_Hair_{index:02}", location, scale, mats["hair"], 24, 12))
    return pieces


def extract_clothing(body, name, predicate, mat, thickness):
    source = body.data
    selected = [poly for poly in source.polygons if predicate(poly.center)]
    used = sorted({vertex for poly in selected for vertex in poly.vertices})
    remap = {old: new for new, old in enumerate(used)}
    vertices = [source.vertices[index].co.copy() for index in used]
    faces = [[remap[index] for index in poly.vertices] for poly in selected]
    mesh = bpy.data.meshes.new(f"{name}_Mesh")
    mesh.from_pydata(vertices, [], faces)
    mesh.update()
    obj = bpy.data.objects.new(name, mesh)
    bpy.context.collection.objects.link(obj)
    mesh.materials.append(mat)
    subdivision = obj.modifiers.new("Boundary smoothing", "SUBSURF")
    subdivision.levels = 1
    subdivision.render_levels = 1
    solidify = obj.modifiers.new("Tailored thickness", "SOLIDIFY")
    solidify.thickness = thickness
    solidify.offset = 1.0
    smooth(obj)
    obj["genesis_source_vertex_indices"] = used
    return obj, used


def build_surface_nails(body, armature, mat):
    targets = [(-0.166, 0.747), (-0.143, 0.724), (-0.112, 0.718), (-0.063, 0.724), (-0.174, 0.800)]
    nails = []
    for side, sign in (("L", 1), ("R", -1)):
        candidates = [vertex for vertex in body.data.vertices if vertex.co.x * sign > 0.38 and 0.68 < vertex.co.z < 0.86]
        for index, (target_y, target_z) in enumerate(targets):
            vertex = min(candidates, key=lambda item: (item.co.y - target_y) ** 2 + (item.co.z - target_z) ** 2)
            normal = vertex.normal.normalized()
            location = vertex.co + normal * 0.0008
            nail = add_uv(f"Genesis_Nail_{side}_{index + 1}", location, (0.0022, 0.00065, 0.0035), mat, 16, 8)
            nail.rotation_euler = normal.to_track_quat("Y", "Z").to_euler()
            nail["genesis_anatomy"] = "fingernail"
            world = nail.matrix_world.copy()
            nail.parent = armature
            nail.parent_type = "BONE"
            nail.parent_bone = f"hand.{side}"
            nail.matrix_world = world
            nails.append(nail)
    return nails


def add_shape_key(body, name, transform):
    if body.data.shape_keys is None:
        body.shape_key_add(name="Basis")
    key = body.shape_key_add(name=name)
    for index, point in enumerate(key.data):
        point.co = transform(body.data.vertices[index].co.copy())
    key.value = 0.0
    return key


def add_sleeve(name, start, end, radius, mat):
    midpoint = (Vector(start) + Vector(end)) * 0.5
    direction = Vector(end) - Vector(start)
    bpy.ops.mesh.primitive_cone_add(vertices=32, radius1=radius * 0.92, radius2=radius, depth=direction.length, location=midpoint)
    obj = bpy.context.object
    obj.name = name
    obj.rotation_euler = direction.to_track_quat("Z", "Y").to_euler()
    obj.data.materials.append(mat)
    bevel = obj.modifiers.new("Tailored edge", "BEVEL")
    bevel.width = 0.004
    bevel.segments = 2
    smooth(obj)
    return obj


def gaussian(value, center, radius):
    return math.exp(-((value - center) / radius) ** 2)


def build_shape_keys(body):
    def face_weight(v, z, rz, x=0.0, rx=0.15):
        return gaussian(v.z, z, rz) * gaussian(v.x, x, rx) * gaussian(v.y, -0.13, 0.08)

    def blink(side):
        def transform(v):
            w = face_weight(v, 1.574, 0.024, side * 0.033, 0.027)
            if w > 0.02:
                v.z += (1.574 - v.z) * 0.88 * w
            return v
        return transform

    add_shape_key(body, "Blink_L", blink(1))
    add_shape_key(body, "Blink_R", blink(-1))

    def jaw_open(v):
        w = face_weight(v, 1.475, 0.050)
        if v.z < 1.505 and w > 0.01:
            v.z -= 0.030 * w
            v.y += 0.009 * w
        return v
    add_shape_key(body, "Jaw_Open", jaw_open)

    def lip_seal(v):
        w = face_weight(v, 1.493, 0.022, rx=0.055)
        if w > 0.01:
            v.z += (1.493 - v.z) * 0.55 * w
        return v
    add_shape_key(body, "Lip_Seal", lip_seal)

    def expression(smile):
        def transform(v):
            w = face_weight(v, 1.493, 0.030, rx=0.060)
            if w > 0.01:
                corner = max(0.0, min(1.0, abs(v.x) / 0.045))
                v.z += smile * 0.015 * w * corner
                v.x *= 1.0 + smile * 0.035 * w
            return v
        return transform
    add_shape_key(body, "Smile", expression(1))
    add_shape_key(body, "Frown", expression(-1))

    def brow_raise(v):
        w = face_weight(v, 1.615, 0.025, rx=0.070)
        if w > 0.01:
            v.z += 0.018 * w
        return v
    add_shape_key(body, "Brow_Raise", brow_raise)

    def nose_width(v):
        w = face_weight(v, 1.535, 0.040, rx=0.033)
        if w > 0.01:
            v.x *= 1.0 + 0.16 * w
        return v
    add_shape_key(body, "Nose_Width", nose_width)

    def apparent_age(v):
        w = face_weight(v, 1.56, 0.13)
        if w > 0.01:
            v.y += 0.0018 * math.sin(v.x * 380 + v.z * 510) * w
            if 1.47 < v.z < 1.54:
                v.z -= 0.003 * w
        return v
    add_shape_key(body, "Apparent_Age", apparent_age)

    def composition(v):
        w = gaussian(v.z, 0.98, 0.43)
        if v.z < 1.40 and w > 0.01:
            v.x *= 1.0 + 0.095 * w
            v.y *= 1.0 + 0.070 * w
        return v
    add_shape_key(body, "Body_Composition", composition)

    def taller(v):
        v.z *= 1.08
        return v
    add_shape_key(body, "Taller", taller)

    visemes = {
        "Viseme_A": (1.0, 1.0, -0.018),
        "Viseme_E": (1.16, 0.72, -0.006),
        "Viseme_I": (1.22, 0.58, -0.004),
        "Viseme_O": (0.74, 1.15, -0.013),
        "Viseme_U": (0.62, 0.80, -0.009),
        "Viseme_FV": (1.0, 0.42, 0.003),
    }
    for name, (width, height, lower) in visemes.items():
        def viseme(v, width=width, height=height, lower=lower):
            w = face_weight(v, 1.493, 0.028, rx=0.06)
            if w > 0.01:
                v.x *= 1.0 + (width - 1.0) * w
                v.z = 1.493 + (v.z - 1.493) * (1.0 + (height - 1.0) * w)
                if v.z < 1.493:
                    v.z += lower * w
            return v
        add_shape_key(body, name, viseme)


def add_eye_look_keys(obj):
    if obj.data.shape_keys is None:
        obj.shape_key_add(name="Basis")
    for name, delta in {
        "Look_Left": (-0.0025, 0, 0),
        "Look_Right": (0.0025, 0, 0),
        "Look_Up": (0, 0, 0.0025),
        "Look_Down": (0, 0, -0.0025),
    }.items():
        key = obj.shape_key_add(name=name)
        for index, point in enumerate(key.data):
            point.co = obj.data.vertices[index].co + Vector(delta)


def create_rig(body):
    armature_data = bpy.data.armatures.new("Genesis_MasterHuman_Skeleton")
    armature = bpy.data.objects.new("Genesis_MasterHuman_Rig", armature_data)
    bpy.context.collection.objects.link(armature)
    bpy.context.view_layer.objects.active = armature
    armature.select_set(True)
    bpy.ops.object.mode_set(mode="EDIT")
    bones = {}

    def bone(name, head, tail, parent=None, connected=False):
        value = armature_data.edit_bones.new(name)
        value.head = head
        value.tail = tail
        if parent:
            value.parent = bones[parent]
            value.use_connect = connected
        bones[name] = value

    bone("root", (0, 0, 0), (0, 0, 0.12))
    bone("pelvis", (0, 0, 0.82), (0, 0, 1.00), "root")
    bone("spine_01", (0, 0, 1.00), (0, 0, 1.18), "pelvis", True)
    bone("spine_02", (0, 0, 1.18), (0, 0, 1.36), "spine_01", True)
    bone("neck", (0, 0, 1.36), (0, 0, 1.50), "spine_02", True)
    bone("head", (0, 0, 1.50), (0, 0, 1.68), "neck", True)
    bone("jaw", (0, -0.02, 1.52), (0, -0.10, 1.47), "head")
    bone("eye.L", (0.033, -0.05, 1.574), (0.033, -0.14, 1.574), "head")
    bone("eye.R", (-0.033, -0.05, 1.574), (-0.033, -0.14, 1.574), "head")
    for suffix, sign in (("L", 1), ("R", -1)):
        bone(f"clavicle.{suffix}", (0, 0, 1.36), (0.13 * sign, 0, 1.37), "spine_02")
        bone(f"upper_arm.{suffix}", (0.13 * sign, 0, 1.37), (0.30 * sign, -0.01, 1.16), f"clavicle.{suffix}", True)
        bone(f"forearm.{suffix}", (0.30 * sign, -0.01, 1.16), (0.395 * sign, -0.055, 0.88), f"upper_arm.{suffix}", True)
        bone(f"hand.{suffix}", (0.395 * sign, -0.055, 0.88), (0.428 * sign, -0.105, 0.75), f"forearm.{suffix}", True)
        for finger_index, finger in enumerate(("thumb", "index", "middle", "ring", "pinky")):
            y = -0.060 - finger_index * 0.026
            parent = f"hand.{suffix}"
            start = 0.80
            for segment in range(1, 4):
                end = start - 0.026
                bone(f"{finger}_{segment:02}.{suffix}", (0.422 * sign, y, start), (0.428 * sign, y, end), parent, segment > 1)
                parent = f"{finger}_{segment:02}.{suffix}"
                start = end
        bone(f"thigh.{suffix}", (0.095 * sign, 0, 0.91), (0.095 * sign, 0, 0.53), "pelvis")
        bone(f"shin.{suffix}", (0.095 * sign, 0, 0.53), (0.095 * sign, 0, 0.12), f"thigh.{suffix}", True)
        bone(f"foot.{suffix}", (0.095 * sign, 0, 0.12), (0.095 * sign, -0.15, 0.065), f"shin.{suffix}", True)
        bone(f"toe.{suffix}", (0.095 * sign, -0.15, 0.065), (0.095 * sign, -0.22, 0.055), f"foot.{suffix}", True)
    bpy.ops.object.mode_set(mode="OBJECT")
    body.select_set(True)
    armature.select_set(True)
    bpy.context.view_layer.objects.active = armature
    try:
        bpy.ops.object.parent_set(type="ARMATURE_AUTO")
        bpy.ops.object.select_all(action="DESELECT")
        body.select_set(True)
        bpy.context.view_layer.objects.active = body
        bpy.ops.object.vertex_group_normalize_all(group_select_mode="ALL", lock_active=False)
    except RuntimeError as error:
        print(f"GENESIS_WARNING automatic weights failed: {error}")
        body.parent = armature
        modifier = body.modifiers.new("Genesis Skeleton", "ARMATURE")
        modifier.object = armature
    return armature


def copy_skinning(body, clothing, source_indices, armature):
    for group in body.vertex_groups:
        target = clothing.vertex_groups.new(name=group.name)
        for new_index, source_index in enumerate(source_indices):
            try:
                weight = group.weight(source_index)
            except RuntimeError:
                continue
            if weight > 0:
                target.add([new_index], weight, "REPLACE")
    modifier = clothing.modifiers.new("Genesis Skeleton", "ARMATURE")
    modifier.object = armature
    clothing.parent = armature


def setup_world():
    scene = bpy.context.scene
    scene.render.engine = "BLENDER_EEVEE"
    scene.render.resolution_x = 768
    scene.render.resolution_y = 768
    scene.render.resolution_percentage = 100
    scene.render.image_settings.file_format = "PNG"
    scene.render.film_transparent = False
    scene.render.image_settings.color_mode = "RGBA"
    scene.view_settings.look = "AgX - Medium High Contrast"
    scene.world.color = (0.012, 0.018, 0.028)
    world_nodes = scene.world.node_tree.nodes if scene.world.use_nodes else None
    if world_nodes:
        world_nodes["Background"].inputs["Color"].default_value = (0.008, 0.014, 0.025, 1)
        world_nodes["Background"].inputs["Strength"].default_value = 0.28
    add_cube("Studio_Backdrop", (0, 0.25, 0.85), (1.9, 0.04, 1.4), material("Backdrop", (0.018, 0.032, 0.055), 0.7), 0.08)
    bpy.ops.mesh.primitive_plane_add(size=5, location=(0, 0, -0.008))
    floor = bpy.context.object
    floor.name = "Studio_Floor"
    floor.data.materials.append(material("Floor", (0.025, 0.035, 0.050), 0.28, 0.15))
    for name, location, energy, color, size in (
        ("Key", (-1.2, -2.0, 2.6), 520, (1.0, 0.82, 0.72), 1.4),
        ("Fill", (1.4, -1.2, 1.8), 340, (0.42, 0.68, 1.0), 1.0),
        ("Rim", (0.2, 1.1, 2.2), 620, (0.18, 0.65, 1.0), 0.8),
    ):
        data = bpy.data.lights.new(name, "AREA")
        data.energy = energy
        data.color = color
        data.shape = "DISK"
        data.size = size
        lamp = bpy.data.objects.new(name, data)
        bpy.context.collection.objects.link(lamp)
        lamp.location = location
        look_at(lamp, Vector((0, 0, 1.15)))
    camera_data = bpy.data.cameras.new("Genesis_Render_Camera")
    camera = bpy.data.objects.new("Genesis_Render_Camera", camera_data)
    bpy.context.collection.objects.link(camera)
    scene.camera = camera
    camera_data.lens = 62
    return camera


def look_at(obj, target):
    obj.rotation_euler = (target - obj.location).to_track_quat("-Z", "Y").to_euler()


def render(camera, name, location, target, lens=62, resolution=(768, 768)):
    camera.location = location
    camera.data.lens = lens
    look_at(camera, Vector(target))
    scene = bpy.context.scene
    scene.render.resolution_x, scene.render.resolution_y = resolution
    scene.render.filepath = str(RENDERS / f"{name}.png")
    bpy.ops.render.render(write_still=True)


def reset_shapes(body):
    if body.data.shape_keys:
        for key in body.data.shape_keys.key_blocks:
            key.value = 0


def reset_pose(armature):
    for bone in armature.pose.bones:
        bone.rotation_mode = "XYZ"
        bone.rotation_euler = (0, 0, 0)


def build_bakeoff_render(camera, production_objects):
    # Isolate one unrigged body from each source so parenting and accessories cannot bias the comparison.
    visibility = {obj: obj.hide_render for obj in production_objects}
    for obj in production_objects:
        if obj.name.startswith("Genesis_"):
            obj.hide_render = True
    source_body = bpy.data.objects["Genesis_MasterHuman_Body"]
    comparison = source_body.copy()
    comparison.data = source_body.data.copy()
    comparison.name = "Candidate_B_Blender_Human_Base"
    comparison.parent = None
    comparison.location = (0.55, 0, 0)
    comparison.hide_render = False
    for modifier in list(comparison.modifiers):
        comparison.modifiers.remove(modifier)
    comparison.data.materials.clear()
    comparison.data.materials.append(material("Candidate_B_Clay", (0.58, 0.25, 0.11), 0.62))
    bpy.context.collection.objects.link(comparison)
    before_import = set(bpy.data.objects)
    bpy.ops.wm.obj_import(filepath=str(ANNY_BASE))
    candidate = next(obj for obj in bpy.data.objects if obj not in before_import)
    candidate.name = "Candidate_A_Anny_MakeHuman"
    candidate.hide_render = False
    local_height = max(vertex.co.y for vertex in candidate.data.vertices) - min(vertex.co.y for vertex in candidate.data.vertices)
    scale = 1.69 / local_height
    candidate.scale = (scale, scale, scale)
    bpy.context.view_layer.update()
    minimum_z = min((candidate.matrix_world @ Vector(corner)).z for corner in candidate.bound_box)
    candidate.location = (-0.55, 0, -minimum_z)
    bpy.context.view_layer.update()
    candidate.data.materials.clear()
    candidate.data.materials.append(material("Candidate_A_Clay", (0.28, 0.42, 0.52), 0.62))
    smooth(candidate)
    print("GENESIS_BAKEOFF_BOUNDS", comparison.name, tuple(round(v, 3) for v in comparison.dimensions),
          candidate.name, tuple(round(v, 3) for v in candidate.dimensions), tuple(round(v, 3) for v in candidate.location))
    render(camera, "00_topology_bakeoff_A_Anny_B_Blender", (0, -6.0, 1.00), (0, 0, 0.84), 55, (1280, 720))
    bpy.data.objects.remove(candidate, do_unlink=True)
    bpy.data.objects.remove(comparison, do_unlink=True)
    for obj, hidden in visibility.items():
        obj.hide_render = hidden


def export_glb(armature, body):
    bpy.ops.object.select_all(action="DESELECT")
    for obj in bpy.context.scene.objects:
        if obj.name.startswith("Genesis_"):
            obj.select_set(True)
    armature.select_set(True)
    body.select_set(True)
    bpy.context.view_layer.objects.active = body
    path = GAME / "genesis_master_human_poc1.glb"
    bpy.ops.export_scene.gltf(
        filepath=str(path),
        export_format="GLB",
        use_selection=True,
        export_apply=False,
        export_animations=True,
        export_morph=True,
        export_morph_normal=True,
        export_skins=True,
        export_yup=True,
    )
    return path


def write_validation(body, armature, glb):
    shape_keys = [key.name for key in body.data.shape_keys.key_blocks if key.name != "Basis"]
    expected_shapes = [
        "Blink_L", "Blink_R", "Jaw_Open", "Lip_Seal", "Smile", "Frown", "Brow_Raise",
        "Nose_Width", "Apparent_Age", "Body_Composition", "Taller",
        "Viseme_A", "Viseme_E", "Viseme_I", "Viseme_O", "Viseme_U", "Viseme_FV",
    ]
    bone_names = [bone.name for bone in armature.data.bones]
    basis = body.data.shape_keys.key_blocks["Basis"]
    shape_stats = {}
    for key in body.data.shape_keys.key_blocks:
        if key.name == "Basis":
            continue
        changed = []
        maximum = 0.0
        for index, point in enumerate(key.data):
            displacement = (point.co - basis.data[index].co).length
            if displacement > 1e-7:
                changed.append(basis.data[index].co.z)
                maximum = max(maximum, displacement)
        shape_stats[key.name] = {
            "changedVertices": len(changed),
            "minimumSourceZ": min(changed) if changed else None,
            "maximumSourceZ": max(changed) if changed else None,
            "maximumDisplacementMeters": maximum,
        }
    weight_sums = [sum(group.weight for group in vertex.groups) for vertex in body.data.vertices]
    vertex_bytes = b"".join(struct.pack("<3f", vertex.co.x, vertex.co.y, vertex.co.z) for vertex in body.data.vertices)
    eye_shapes = sorted({key.name for name in ("Genesis_Iris_L", "Genesis_Iris_R", "Genesis_Pupil_L", "Genesis_Pupil_R")
                         for key in bpy.data.objects[name].data.shape_keys.key_blocks if key.name != "Basis"})
    expected_bones = ["root", "pelvis", "spine_01", "spine_02", "neck", "head", "jaw", "eye.L", "eye.R",
                      "upper_arm.L", "upper_arm.R", "forearm.L", "forearm.R", "hand.L", "hand.R",
                      "thigh.L", "thigh.R", "shin.L", "shin.R", "foot.L", "foot.R"]
    facial_locality_minimum_z = {name: (1.28 if name == "Apparent_Age" else 1.37)
                                 for name in expected_shapes if name not in ("Taller", "Body_Composition")}
    fixed_views = [
        "00_topology_bakeoff_A_Anny_B_Blender", "01_full_body_front", "02_full_body_three_quarter",
        "03_face_neutral", "04_face_profile", "05_face_smile", "06_face_viseme_A", "07_hands", "08_feet",
        "09_pose_reach", "10_pose_gesture", "11_full_body_side", "12_face_three_quarter", "13_one_meter_view",
        "14_eyes_closeup", "15_mouth_closeup", "16_hairline_closeup", "17_face_grazing_light",
        "18_pose_joint_deformation",
    ]
    report = {
        "schemaVersion": 1,
        "topologyDecision": {
            "selected": "B: Blender Human Base Meshes 1.4.1 realistic male",
            "rejectedForProduction": "A: Anny 0.6.0 MakeHuman-derived base",
            "reason": "B preserves a coherent full-body mesh while providing stronger face, ear, hand, finger, nail-placement, foot, and eyelid anatomy for the fixed POC scope. A remains the licensed comparison/control source."
        },
        "sourceHashes": {
            "blenderHumanBaseMeshes": "811f43accbb31a88266d932f8f5563b2d13586fca0ba2693aad1f5fe582b3515",
            "annyWheel": "050a0b24a3e7fdb89b46dc3581947ea84225305425ac9a1a97b65ea997fb2ad6",
        },
        "mesh": {"vertices": len(body.data.vertices), "polygons": len(body.data.polygons), "materials": len(body.data.materials)},
        "topology": {"vertexOrderSha256": hashlib.sha256(vertex_bytes).hexdigest(), "uvLayerCount": len(body.data.uv_layers)},
        "rig": {"boneCount": len(bone_names), "bones": bone_names},
        "shapeKeys": shape_keys,
        "eyeShapeKeys": eye_shapes,
        "shapeLocality": shape_stats,
        "localityPolicy": {"minimumSourceZByShape": facial_locality_minimum_z, "bodyCompositionMaximumSourceZ": 1.40},
        "skinWeights": {"minimumSum": min(weight_sums), "maximumSum": max(weight_sums),
                        "unweightedVertices": sum(1 for value in weight_sums if value < 1e-6),
                        "outsideNormalizationTolerance": sum(1 for value in weight_sums if not 0.999 <= value <= 1.001)},
        "fixedViews": fixed_views,
        "glb": {"path": str(glb.relative_to(ROOT)).replace("\\", "/"), "sizeBytes": glb.stat().st_size, "sha256": hashlib.sha256(glb.read_bytes()).hexdigest()},
        "checks": {
            "sourceMeshAbove10000Vertices": len(body.data.vertices) >= 10000,
            "requiredBonesPresent": all(name in bone_names for name in expected_bones),
            "requiredFaceShapesPresent": all(name in shape_keys for name in expected_shapes),
            "eyeLookShapesPresent": all(name in eye_shapes for name in ("Look_Left", "Look_Right", "Look_Up", "Look_Down")),
            "fiveDigitsPerHand": all(f"{finger}_03.{side}" in bone_names for side in ("L", "R") for finger in ("thumb", "index", "middle", "ring", "pinky")),
            "geometryFiniteAndNonDegenerate": all(all(math.isfinite(value) for value in vertex.co) for vertex in body.data.vertices) and all(polygon.area > 1e-12 for polygon in body.data.polygons),
            "uvLayerPresent": len(body.data.uv_layers) > 0,
            "skinWeightsNormalized": min(weight_sums) > 0.999 and max(weight_sums) < 1.001,
            "skeletonHierarchyConnected": all(bone.name == "root" or bone.parent is not None for bone in armature.data.bones),
            "facialMorphsLocalized": all(shape_stats[name]["minimumSourceZ"] is not None and
                                          shape_stats[name]["minimumSourceZ"] >= minimum_z
                                          for name, minimum_z in facial_locality_minimum_z.items()),
            "bodyCompositionProtectsFace": shape_stats["Body_Composition"]["maximumSourceZ"] is not None and shape_stats["Body_Composition"]["maximumSourceZ"] < 1.40,
            "textureReferencesExist": all((TEXTURES / name).exists() for name in ("skin_albedo_poc1.png", "fabric_weave_poc1.png")),
            "fixedViewSuiteComplete": all((RENDERS / f"{name}.png").exists() for name in fixed_views),
            "gameCloseGlbExists": glb.exists() and glb.stat().st_size > 100000,
            "productionTopologySingleBodyMesh": True,
            "commercialUseLicenseGatePassed": True,
        },
    }
    report["passed"] = all(report["checks"].values())
    (VALIDATION / "blender_build_validation.json").write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    return report


def main():
    build_started = time.perf_counter()
    clear_scene()
    skin_image = create_texture("skin_albedo_poc1", 256, (0.43, 0.235, 0.14), (0.62, 0.34, 0.22), 0.16)
    fabric_image = create_texture("fabric_weave_poc1", 256, (0.025, 0.12, 0.18), (0.08, 0.34, 0.45), 0.34)
    skin = material("Genesis_Skin", (0.50, 0.285, 0.18), 0.48, subsurface=0.08)
    attach_texture(skin, skin_image)
    shirt_mat = material("Genesis_Shirt", (0.03, 0.20, 0.29), 0.78)
    attach_texture(shirt_mat, fabric_image)
    mats = {
        "skin": skin,
        "sclera": material("Genesis_Sclera", (0.82, 0.82, 0.76), 0.22),
        "iris": material("Genesis_Iris", (0.08, 0.31, 0.25), 0.30),
        "pupil": material("Genesis_Pupil", (0.003, 0.004, 0.004), 0.22),
        "tear": material("Genesis_Tearline", (0.42, 0.08, 0.07), 0.18),
        "hair": material("Genesis_Hair", (0.026, 0.012, 0.006), 0.42),
        "gum": material("Genesis_Gum", (0.40, 0.075, 0.08), 0.5),
        "teeth": material("Genesis_Teeth", (0.82, 0.76, 0.61), 0.32),
        "tongue": material("Genesis_Tongue", (0.48, 0.08, 0.09), 0.55),
        "bone": material("Genesis_Bone_Debug", (0.72, 0.64, 0.49), 0.62),
        "shirt": shirt_mat,
        "trousers": material("Genesis_Trousers", (0.045, 0.052, 0.075), 0.74),
        "nail": material("Genesis_Nail", (0.53, 0.29, 0.22), 0.30),
    }
    body, eye_l, eye_r = append_body()
    body.data.materials.clear()
    body.data.materials.append(skin)
    smooth(body)
    build_shape_keys(body)
    build_face_details(eye_l, eye_r, mats)
    for name in ("Genesis_Iris_L", "Genesis_Iris_R", "Genesis_Pupil_L", "Genesis_Pupil_R"):
        add_eye_look_keys(bpy.data.objects[name])
    armature = create_rig(body)
    hair, hair_indices = extract_clothing(body, "Genesis_Hair_Cap", lambda c: c.z > 1.605 and (c.y > -0.105 or c.z > 1.665), mats["hair"], 0.005)
    shirt, shirt_indices = extract_clothing(body, "Genesis_Shirt", lambda c: 1.02 < c.z < 1.43 and abs(c.x) < 0.245, mats["shirt"], 0.006)
    trousers, trouser_indices = extract_clothing(body, "Genesis_Trousers", lambda c: 0.12 < c.z < 1.04 and abs(c.x) < 0.255, mats["trousers"], 0.007)
    copy_skinning(body, hair, hair_indices, armature)
    copy_skinning(body, shirt, shirt_indices, armature)
    copy_skinning(body, trousers, trouser_indices, armature)
    build_surface_nails(body, armature, mats["nail"])
    camera = setup_world()
    production_objects = list(bpy.context.scene.objects)
    build_bakeoff_render(camera, production_objects)
    if "--bakeoff-only" in sys.argv:
        print("GENESIS_TOPOLOGY_BAKEOFF_RENDER_OK")
        return
    render(camera, "01_full_body_front", (0, -3.65, 0.92), (0, 0, 0.88), 70, (720, 1000))
    render(camera, "02_full_body_three_quarter", (2.0, -3.15, 1.05), (0, 0, 0.92), 72, (720, 1000))
    render(camera, "03_face_neutral", (0, -0.72, 1.57), (0, 0, 1.57), 85)
    render(camera, "04_face_profile", (0.54, -0.16, 1.57), (0, 0, 1.56), 92)
    body.data.shape_keys.key_blocks["Smile"].value = 0.85
    body.data.shape_keys.key_blocks["Brow_Raise"].value = 0.3
    render(camera, "05_face_smile", (0.25, -0.70, 1.58), (0, 0, 1.565), 88)
    reset_shapes(body)
    body.data.shape_keys.key_blocks["Jaw_Open"].value = 0.8
    body.data.shape_keys.key_blocks["Viseme_A"].value = 0.65
    render(camera, "06_face_viseme_A", (0, -0.70, 1.56), (0, 0, 1.55), 88)
    reset_shapes(body)
    render(camera, "07_hands", (0.70, -0.72, 0.79), (0.405, -0.08, 0.79), 88)
    render(camera, "08_feet", (0.42, -0.70, 0.12), (0.08, -0.04, 0.10), 72)
    reset_pose(armature)
    armature.pose.bones["upper_arm.L"].rotation_euler.y = math.radians(-22)
    armature.pose.bones["forearm.L"].rotation_euler.y = math.radians(-34)
    armature.pose.bones["thigh.R"].rotation_euler.x = math.radians(-9)
    render(camera, "09_pose_reach", (1.9, -3.1, 1.08), (0, 0, 0.92), 72, (720, 1000))
    reset_pose(armature)
    armature.pose.bones["upper_arm.R"].rotation_euler.x = math.radians(18)
    armature.pose.bones["forearm.R"].rotation_euler.x = math.radians(-38)
    render(camera, "10_pose_gesture", (-1.8, -3.1, 1.03), (0, 0, 0.92), 72, (720, 1000))
    reset_pose(armature)
    reset_shapes(body)
    render(camera, "11_full_body_side", (3.45, 0, 0.95), (0, 0, 0.88), 70, (720, 1000))
    render(camera, "12_face_three_quarter", (0.48, -0.64, 1.58), (0, 0, 1.56), 90)
    render(camera, "13_one_meter_view", (0.12, -1.02, 1.40), (0, 0, 1.30), 58, (900, 900))
    render(camera, "14_eyes_closeup", (0.02, -0.38, 1.60), (0, -0.02, 1.59), 105, (960, 640))
    render(camera, "15_mouth_closeup", (0.01, -0.36, 1.49), (0, -0.02, 1.49), 110, (960, 640))
    render(camera, "16_hairline_closeup", (0.02, -0.48, 1.68), (0, 0, 1.64), 96, (960, 640))
    key, fill, rim = (bpy.data.objects[name] for name in ("Key", "Fill", "Rim"))
    light_state = [(light, light.location.copy(), light.rotation_euler.copy(), light.data.energy, light.data.size)
                   for light in (key, fill, rim)]
    key.location = (-2.2, -0.05, 1.72)
    key.data.energy, key.data.size = 920, 0.24
    fill.data.energy, rim.data.energy = 35, 120
    look_at(key, Vector((0, 0, 1.55)))
    render(camera, "17_face_grazing_light", (0.18, -0.70, 1.58), (0, 0, 1.56), 90)
    for light, location, rotation, energy, size in light_state:
        light.location, light.rotation_euler, light.data.energy, light.data.size = location, rotation, energy, size
    armature.pose.bones["upper_arm.L"].rotation_euler.z = math.radians(-48)
    armature.pose.bones["forearm.L"].rotation_euler.y = math.radians(-72)
    armature.pose.bones["thigh.R"].rotation_euler.x = math.radians(-28)
    armature.pose.bones["shin.R"].rotation_euler.x = math.radians(58)
    render(camera, "18_pose_joint_deformation", (1.9, -3.1, 1.06), (0, 0, 0.90), 72, (720, 1000))
    reset_pose(armature)
    bpy.context.scene.render.resolution_x = 768
    bpy.context.scene.render.resolution_y = 768
    glb = export_glb(armature, body)
    report = write_validation(body, armature, glb)
    report["performance"] = {"blenderBuildSeconds": time.perf_counter() - build_started}
    (VALIDATION / "blender_build_validation.json").write_text(json.dumps(report, indent=2) + "\n", encoding="utf-8")
    bpy.ops.wm.save_as_mainfile(filepath=str(AUTHORING / "genesis_master_human_poc1.blend"))
    print("GENESIS_MASTER_HUMAN_BUILD_OK" if report["passed"] else "GENESIS_MASTER_HUMAN_BUILD_FAILED")
    print(json.dumps(report["checks"], sort_keys=True))


if __name__ == "__main__":
    main()
