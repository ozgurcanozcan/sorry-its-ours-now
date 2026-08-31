# Ev kiti - modüler oda parcalari, 1 m izgara.
# Plan: docs/plan.md Hafta 1 gun 2-3 (C). Karar 46: ev kiti ~9-10 parca.
#
# EKSEN: Blender'da X=genislik, Y=derinlik, Z=yukseklik.
# Unity'ye Y-up / -Z-forward ile ihrac ediliyor, orada Y=yukseklik oluyor.
#
# PIVOT KURALI: her parcanin pivotu, kapladigi izgara hucresinin
# ic-alt-kosesinde (0,0,0). Duvarin ic yuzu izgara cizgisinde, kalinlik
# disari tasar. Boylece Unity'de tam sayi koordinata birakinca parcalar
# kendiliginden ust uste binmeden oturuyor.

import bpy, bmesh, os, sys, math

CIKTI = sys.argv[sys.argv.index("--") + 1]
FBX = os.path.join(CIKTI, "fbx"); os.makedirs(FBX, exist_ok=True)

# --- olculer -----------------------------------------------------------
# Kaynak: TS 9111 / imar yonetmeligi ve Turkiye'de yaygin konut olculeri.
# Hepsi burada tek yerde; degistirmek isteyen bu blogu degistirir.
IZGARA        = 1.00   # modul genisligi
KAT_YUKSEK    = 2.70   # net tavan yuksekligi (yonetmelik min 2.40)
DUVAR_KALIN   = 0.10   # ic bolme duvari
ZEMIN_KALIN   = 0.10
KAPI_GENIS    = 0.80   # ic kapi bosluğu - "dar kapi" engeli buradan doguyor
KAPI_YUKSEK   = 2.00
PENCERE_GENIS = 1.20
PENCERE_YUKSEK= 1.40
DENIZLIK      = 0.90   # pencere alt kotu

# --- yardimcilar -------------------------------------------------------

def temiz_sahne():
    bpy.ops.wm.read_factory_settings(use_empty=True)
    s = bpy.context.scene
    s.unit_settings.system = 'METRIC'
    s.unit_settings.scale_length = 1.0   # 1 birim = 1 metre; kaymasi ihracati bozar

def kutu(ad, x0, y0, z0, sx, sy, sz):
    """Min kosesi (x0,y0,z0) olan, (sx,sy,sz) boyutunda kutu."""
    bpy.ops.mesh.primitive_cube_add(size=1.0,
        location=(x0 + sx/2, y0 + sy/2, z0 + sz/2))
    o = bpy.context.active_object
    o.name = ad
    o.scale = (sx, sy, sz)
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    return o

def bosluk_ac(govde, delik):
    """Boolean fark. Kutu birlestirmek yerine bunu kullaniyoruz ki
    parcanin icinde gorunmeyen yuzey kalmasin - ucgen sayisi bosa gitmesin."""
    m = govde.modifiers.new("bosluk", 'BOOLEAN')
    m.operation = 'DIFFERENCE'
    m.object = delik
    m.solver = 'EXACT'
    bpy.context.view_layer.objects.active = govde
    bpy.ops.object.modifier_apply(modifier=m.name)
    bpy.data.objects.remove(delik, do_unlink=True)
    return govde

def pivot_sifirla(o):
    bpy.context.scene.cursor.location = (0, 0, 0)
    bpy.ops.object.select_all(action='DESELECT')
    o.select_set(True); bpy.context.view_layer.objects.active = o
    bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
    bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)
    bpy.ops.object.shade_flat()
    return o

def ucgen_say(o):
    me = o.data
    return sum(len(p.vertices) - 2 for p in me.polygons)

def ihrac(o):
    bpy.ops.object.select_all(action='DESELECT')
    o.select_set(True); bpy.context.view_layer.objects.active = o
    yol = os.path.join(FBX, o.name + ".fbx")
    # axis_forward='Z' (Unity icin genelde '-Z' tavsiye edilir ama bizim
    # pivot kuralimizda (ic-alt kose 0,0,0, parca +X yonune buyur) X ekseni
    # ters cikiyordu. Unity'de gercek olcup bulundu - bkz.
    # docs/uretim/ev-kiti.md "Deney: axis_forward".
    bpy.ops.export_scene.fbx(filepath=yol, use_selection=True,
        apply_scale_options='FBX_SCALE_NONE', global_scale=1.0,
        axis_forward='Z', axis_up='Y', bake_space_transform=True,
        object_types={'MESH'}, mesh_smooth_type='FACE', add_leaf_bones=False)
    return yol

def rapor(o):
    d = o.dimensions
    print("PARCA %-22s %5.2f x %5.2f x %5.2f m | %3d ucgen | pivot %s"
          % (o.name, d.x, d.y, d.z, ucgen_say(o), tuple(round(v,3) for v in o.location)))

# --- parcalar ----------------------------------------------------------
# Duvar: ic yuzu y=0'da, kalinlik -Y yonune (disari) tasiyor.

def duvar(ad, genislik):
    return pivot_sifirla(kutu(ad, 0, -DUVAR_KALIN, 0,
                              genislik, DUVAR_KALIN, KAT_YUKSEK))

def duvar_kapi():
    g = kutu("SM_DuvarKapi_1m", 0, -DUVAR_KALIN, 0, IZGARA, DUVAR_KALIN, KAT_YUKSEK)
    x = (IZGARA - KAPI_GENIS) / 2                      # her iki yanda 10 cm sove payi
    d = kutu("_delik", x, -DUVAR_KALIN - 0.05, -0.05,
             KAPI_GENIS, DUVAR_KALIN + 0.10, KAPI_YUKSEK + 0.05)
    return pivot_sifirla(bosluk_ac(g, d))

def duvar_pencere():
    g = kutu("SM_DuvarPencere_2m", 0, -DUVAR_KALIN, 0, 2*IZGARA, DUVAR_KALIN, KAT_YUKSEK)
    x = (2*IZGARA - PENCERE_GENIS) / 2
    d = kutu("_delik", x, -DUVAR_KALIN - 0.05, DENIZLIK,
             PENCERE_GENIS, DUVAR_KALIN + 0.10, PENCERE_YUKSEK)
    return pivot_sifirla(bosluk_ac(g, d))

def kose():
    """L parca: iki kol, ic kosede bosluk birakmiyor."""
    a = kutu("SM_DuvarKose_L", 0, -DUVAR_KALIN, 0, IZGARA, DUVAR_KALIN, KAT_YUKSEK)
    b = kutu("_kol", -DUVAR_KALIN, -DUVAR_KALIN, 0, DUVAR_KALIN, IZGARA + DUVAR_KALIN, KAT_YUKSEK)
    bpy.ops.object.select_all(action='DESELECT')
    b.select_set(True); a.select_set(True)
    bpy.context.view_layer.objects.active = a
    bpy.ops.object.join()
    return pivot_sifirla(a)

def zemin():
    # ust yuzey z=0'da: yurunen duzlem izgara kotu
    return pivot_sifirla(kutu("SM_Zemin_1m", 0, 0, -ZEMIN_KALIN, IZGARA, IZGARA, ZEMIN_KALIN))

def tavan():
    return pivot_sifirla(kutu("SM_Tavan_1m", 0, 0, 0, IZGARA, IZGARA, ZEMIN_KALIN))

def kapi_kanadi():
    # pivot mentese kenarinda: A'nin donme etkilesimi bunu bekliyor
    return pivot_sifirla(kutu("SM_KapiKanadi", 0, 0, 0, KAPI_GENIS, 0.04, KAPI_YUKSEK))

def kapi_sove():
    g = kutu("SM_KapiSove", -0.05, -0.06, 0, KAPI_GENIS + 0.10, 0.12, KAPI_YUKSEK + 0.05)
    d = kutu("_delik", 0, -0.12, -0.05, KAPI_GENIS, 0.24, KAPI_YUKSEK + 0.05)
    return pivot_sifirla(bosluk_ac(g, d))

# --- uret --------------------------------------------------------------
temiz_sahne()
parcalar = [
    duvar("SM_Duvar_1m", IZGARA),
    duvar("SM_Duvar_2m", 2 * IZGARA),
    duvar_kapi(),
    duvar_pencere(),
    kose(),
    zemin(),
    tavan(),
    kapi_kanadi(),
    kapi_sove(),
]

mat = bpy.data.materials.new("M_GriKutu")
mat.use_nodes = True
mat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (0.62, 0.60, 0.57, 1)
mat.node_tree.nodes["Principled BSDF"].inputs["Roughness"].default_value = 0.9

toplam = 0
for p in parcalar:
    p.data.materials.append(mat)
    rapor(p); ihrac(p); toplam += ucgen_say(p)
print("PARCA TOPLAM %d parca, %d ucgen" % (len(parcalar), toplam))

bpy.ops.wm.save_as_mainfile(filepath=os.path.join(CIKTI, "ev_kiti.blend"))
