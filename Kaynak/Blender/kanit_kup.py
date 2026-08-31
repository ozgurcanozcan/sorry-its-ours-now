# Karar 48 kaniti: tek kup ile olcek + pivot + ihrac akisi.
# Amac guzel model degil; Unity'ye 1 birim = 1 metre olarak, pivot tabanda
# gelen bir mesh uretebildigimizi gostermek.
import bpy, os, sys, math

CIKTI = sys.argv[sys.argv.index("--") + 1]
os.makedirs(CIKTI, exist_ok=True)

# temiz sahne
bpy.ops.wm.read_factory_settings(use_empty=True)

# Blender'in birimi metre; sahne olcegi 1.0 olmali yoksa FBX'te olcek kayar
bpy.context.scene.unit_settings.system = 'METRIC'
bpy.context.scene.unit_settings.scale_length = 1.0

# 1 m kup
bpy.ops.mesh.primitive_cube_add(size=1.0, location=(0, 0, 0.5))
kup = bpy.context.active_object
kup.name = "SM_KanitKup"
kup.data.name = "SM_KanitKup_mesh"

# Pivot tabanin ortasinda olmali: Unity'de zemine oturtmak icin tek dogru yer.
# Mesh'i yukari kaydirip objeyi origine cekiyoruz.
bpy.context.scene.cursor.location = (0, 0, 0)
bpy.ops.object.origin_set(type='ORIGIN_CURSOR')

# Olcek/rotasyon uygula: FBX'e 1.0 olcek gitsin
bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

# --- dogrulama ---
print("KONTROL boyut  :", tuple(round(x, 4) for x in kup.dimensions))
print("KONTROL olcek  :", tuple(round(x, 4) for x in kup.scale))
print("KONTROL pivot  :", tuple(round(x, 4) for x in kup.location))
z_min = min((kup.matrix_world @ v.co).z for v in kup.data.vertices)
print("KONTROL taban_z:", round(z_min, 4))
print("KONTROL ucgen  :", len(kup.data.loop_triangles) or sum(len(p.vertices) - 2 for p in kup.data.polygons))

# --- FBX ihrac (Unity ayarlari) ---
fbx = os.path.join(CIKTI, "SM_KanitKup.fbx")
bpy.ops.export_scene.fbx(
    filepath=fbx,
    use_selection=False,
    apply_scale_options='FBX_SCALE_NONE',
    global_scale=1.0,
    axis_forward='-Z',
    axis_up='Y',
    bake_space_transform=True,
    object_types={'MESH'},
    mesh_smooth_type='FACE',
    add_leaf_bones=False,
)
print("KONTROL fbx    :", os.path.getsize(fbx), "bayt")

# --- onizleme render (benim gorebilmem icin) ---
mat = bpy.data.materials.new("M_Kanit")
mat.use_nodes = True
mat.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (0.75, 0.55, 0.3, 1)
kup.data.materials.append(mat)

bpy.ops.mesh.primitive_plane_add(size=8, location=(0, 0, 0))
zemin = bpy.context.active_object
zm = bpy.data.materials.new("M_Zemin")
zm.use_nodes = True
zm.node_tree.nodes["Principled BSDF"].inputs["Base Color"].default_value = (0.15, 0.15, 0.17, 1)
zemin.data.materials.append(zm)

# gunes on-sol ustten gelsin ki kupun iki yuzu de okunsun
bpy.ops.object.light_add(type='SUN', location=(4, -4, 6), rotation=(math.radians(50), 0, math.radians(35)))
bpy.context.active_object.data.energy = 4
sahne_dunya = bpy.data.worlds.new("Dunya")
sahne_dunya.use_nodes = True
sahne_dunya.node_tree.nodes["Background"].inputs[0].default_value = (0.35, 0.38, 0.45, 1)
sahne_dunya.node_tree.nodes["Background"].inputs[1].default_value = 0.6
bpy.context.scene.world = sahne_dunya
bpy.ops.object.camera_add(location=(2.6, -2.6, 1.9), rotation=(math.radians(68), 0, math.radians(45)))
sahne = bpy.context.scene
sahne.camera = bpy.context.active_object
sahne.render.engine = 'BLENDER_EEVEE'
sahne.render.resolution_x, sahne.render.resolution_y = 640, 480
sahne.render.filepath = os.path.join(CIKTI, "onizleme.png")
bpy.ops.render.render(write_still=True)
print("KONTROL render :", sahne.render.filepath)

bpy.ops.wm.save_as_mainfile(filepath=os.path.join(CIKTI, "kanit_kup.blend"))
