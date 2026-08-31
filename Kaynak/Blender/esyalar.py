# 7 oncelikli esya - gri kutu (blockout), M1 Hafta 1 gun 5.
# Pivot: taban-merkez (X,Y merkezde 0, Z=taban 0) - kit gibi izgara-kose degil,
# cunku bunlar zeminde serbest duran objeler; boylece yere koyunca hizalanmis
# olur, dondurmek de merkez etrafinda olur.
#
# Ihrac ayari ev_kiti.py ile ayni: axis_forward='Z' (Unity'de olculerek
# dogrulandi, bkz docs/uretim/ev-kiti.md "Deney: axis_forward").
#
# Boyutlar gercek/bilinen mobilya olculeri (TSE/piyasa standardi), taban
# degerler gercek 2. el piyasa aramasindan (agustos 2026, kaynak: WebSearch,
# sahibinden/letgo). Ikisi de docs/uretim/esyalar.md'de kaynagiyla yazili.

import bpy, os, sys

CIKTI = sys.argv[sys.argv.index("--") + 1]
FBX = os.path.join(CIKTI, "fbx"); os.makedirs(FBX, exist_ok=True)

def temiz_sahne():
    bpy.ops.wm.read_factory_settings(use_empty=True)
    s = bpy.context.scene
    s.unit_settings.system = 'METRIC'
    s.unit_settings.scale_length = 1.0

def kutu_tabanmerkez(ad, gx, gy, gz):
    """Blender: X=genislik, Y=derinlik, Z=yukseklik. Pivot taban-merkezde."""
    bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, gz/2))
    o = bpy.context.active_object
    o.name = ad
    o.data.name = ad + "_mesh"
    o.scale = (gx, gy, gz)
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    bpy.context.scene.cursor.location = (0, 0, 0)
    bpy.ops.object.select_all(action='DESELECT')
    o.select_set(True); bpy.context.view_layer.objects.active = o
    bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
    bpy.ops.object.shade_flat()
    return o

def ucgen_say(o):
    return sum(len(p.vertices) - 2 for p in o.data.polygons)

def ihrac(o):
    bpy.ops.object.select_all(action='DESELECT')
    o.select_set(True); bpy.context.view_layer.objects.active = o
    yol = os.path.join(FBX, o.name + ".fbx")
    bpy.ops.export_scene.fbx(filepath=yol, use_selection=True,
        apply_scale_options='FBX_SCALE_NONE', global_scale=1.0,
        axis_forward='Z', axis_up='Y', bake_space_transform=True,
        object_types={'MESH'}, mesh_smooth_type='FACE', add_leaf_bones=False)
    return yol

def rapor(o):
    d = o.dimensions
    print("ESYA %-20s %5.2f x %5.2f x %5.2f m | %3d ucgen"
          % (o.name, d.x, d.y, d.z, ucgen_say(o)))

# ad, genislik(X), derinlik(Y), yukseklik(Z) - metre
ESYALAR = [
    ("SM_Yatak",           1.60, 2.00, 0.50),
    ("SM_Buzdolabi",       0.60, 0.65, 1.80),
    ("SM_CamasirMakinesi", 0.60, 0.60, 0.85),
    ("SM_Gardirop",        1.00, 0.55, 2.00),
    ("SM_Masa",            1.20, 0.80, 0.75),
    ("SM_Hali",            2.00, 1.40, 0.02),
    ("SM_TV",              1.00, 0.08, 0.60),  # dikey obje: derinlik ince
]

temiz_sahne()
mat = bpy.data.materials.new("M_GriKutu")
mat.use_nodes = True
mat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (0.62, 0.60, 0.57, 1)

toplam_ucgen = 0
for ad, gx, gy, gz in ESYALAR:
    o = kutu_tabanmerkez(ad, gx, gy, gz)
    o.data.materials.append(mat)
    rapor(o); ihrac(o)
    toplam_ucgen += ucgen_say(o)

print("ESYA TOPLAM %d parca, %d ucgen" % (len(ESYALAR), toplam_ucgen))
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(CIKTI, "esyalar.blend"))
