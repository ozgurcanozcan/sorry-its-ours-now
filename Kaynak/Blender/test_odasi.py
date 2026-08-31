# Kitin gercekten oturup oturmadigini kanitlayan test odasi.
# 3 x 4 m oda, sadece tam sayi koordinatlar + 90 derecenin katlari kullaniliyor.
# Eger pivot/olcu kurallari dogruysa hicbir parca elle kaydirilmadan oturmali.
import bpy, os, sys, math

KIT, CIKTI = sys.argv[sys.argv.index("--") + 1], sys.argv[sys.argv.index("--") + 2]
os.makedirs(CIKTI, exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=os.path.join(KIT, "ev_kiti.blend"))

kaynak = {o.name: o for o in bpy.data.objects}
for o in kaynak.values():
    o.hide_render = True

def koy(ad, x, y, derece=0, z=0.0):
    o = kaynak[ad].copy(); o.data = kaynak[ad].data
    o.location = (x, y, z); o.rotation_euler = (0, 0, math.radians(derece))
    o.hide_render = False
    bpy.context.collection.objects.link(o)
    return o

# --- 3 x 4 m oda ---
for i in range(3):
    for j in range(4):
        koy("SM_Zemin_1m", i, j)

koy("SM_DuvarKose_L", 0, 0,   0)     # sol-alt
koy("SM_DuvarKose_L", 3, 0,  90)     # sag-alt
koy("SM_DuvarKose_L", 3, 4, 180)     # sag-ust
koy("SM_DuvarKose_L", 0, 4, 270)     # sol-ust

koy("SM_DuvarKapi_1m",    1, 0,   0) # alt duvar, kapi
koy("SM_Duvar_1m",        2, 4, 180) # ust duvar
koy("SM_Duvar_2m",        0, 3, 270) # sol duvar
koy("SM_DuvarPencere_2m", 3, 1,  90) # sag duvar, pencere

# kapi kanadi mentesesinden aciliyor - pivot dogruysa cerceveden cikmaz
koy("SM_KapiSove",   1.1, 0, 0)
koy("SM_KapiKanadi", 1.1, 0, -110)

# --- isik / dunya ---
bpy.ops.object.light_add(type='SUN', location=(6, -8, 9), rotation=(math.radians(52), 0, math.radians(30)))
bpy.context.active_object.data.energy = 3.5
d = bpy.data.worlds.new("D"); d.use_nodes = True
arka = next(n for n in d.node_tree.nodes if n.type == 'BACKGROUND')
arka.inputs[0].default_value = (0.42, 0.45, 0.52, 1)
arka.inputs[1].default_value = 0.7
bpy.context.scene.world = d

bpy.ops.object.empty_add(location=(1.5, 2.0, 1.0))
hedef = bpy.context.active_object
s = bpy.context.scene
s.render.engine = 'BLENDER_EEVEE'
s.render.resolution_x, s.render.resolution_y = 900, 620

def cek(ad, konum, orto=None):
    bpy.ops.object.camera_add(location=konum)
    k = bpy.context.active_object
    c = k.constraints.new('TRACK_TO'); c.target = hedef
    if orto:
        k.data.type = 'ORTHO'; k.data.ortho_scale = orto
    s.camera = k
    s.render.filepath = os.path.join(CIKTI, ad)
    bpy.ops.render.render(write_still=True)
    print("RENDER", s.render.filepath)

cek("oda_perspektif.png", (7.5, -6.5, 5.5))
cek("oda_ustten.png",     (1.5, 1.999, 14), orto=6.5)
