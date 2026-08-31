# Ev Kiti — ölçü ve ihraç şeması

> Sahibi: C (Onat) · `docs/plan.md` Hafta 1, gün 2–3
> Kararlar: 46 (iki ayrı modüler kit), 48 (önce ihraç akışı), 58–62

Kit **script ile üretiliyor** (`Kaynak/Blender/ev_kiti.py`). Elle modellenmiyor:
modüler bir kitte hataların neredeyse tamamı ölçü hatasıdır ve script'te ölçü
tek yerde durur. Ölçü değişecekse `ev_kiti.py` içindeki ölçü bloğu değişir ve
yeniden koşulur — 9 parça birden güncellenir.

## Ölçüler

| Değer | m | Neden |
|-------|---|-------|
| Izgara modülü | 1.00 | Tüm yerleşim tam sayı koordinatta |
| Kat yüksekliği | 2.70 | Türkiye konut pratiği; yönetmelik alt sınırı 2.40 |
| Duvar kalınlığı | 0.10 | İç bölme duvarı |
| Zemin / tavan kalınlığı | 0.10 | |
| Kapı boşluğu | 0.80 × 2.00 | Yaygın iç kapı ölçüsü |
| Pencere boşluğu | 1.20 × 1.40 | |
| Denizlik kotu | 0.90 | Pencere alt kotu |

> **Kapı genişliği bir oynanış sayısıdır, süs değil.** `02-saha.md` dar kapıyı
> mekân kaynaklı engel sayıyor ve kanepeyi kapıdan geçirmek oyunun görsel şakası.
> 0.80 m bu yüzden seçildi. **Değişirse A'nın taşıma denemesi de değişir** —
> tek başına değiştirilmez, konuşulur.

## Pivot kuralı

Her parçanın pivotu, kapladığı ızgara hücresinin **iç-alt köşesinde** (0,0,0).

- **Duvar** — iç yüzü ızgara çizgisinde, kalınlık dışarı taşar
- **Zemin** — üst yüzeyi Y=0'da; yürünen düzlem ızgara kotu
- **Kapı kanadı** — pivot menteşe kenarında; dönme etkileşimi buna bağlı

Sonuç: Unity'de tam sayı koordinat ve 90°'nin katları dışında hiçbir şey
gerekmiyor. `test_odasi.py` bunu kanıtlıyor — 3×4 m oda kuruluyor ve tek parça
elle kaydırılmıyor.

## Parçalar

| Dosya | Ölçü (m) | Üçgen |
|-------|----------|-------|
| SM_Duvar_1m | 1.00 × 0.10 × 2.70 | 12 |
| SM_Duvar_2m | 2.00 × 0.10 × 2.70 | 12 |
| SM_DuvarKapi_1m | 1.00 × 0.10 × 2.70 | 28 |
| SM_DuvarPencere_2m | 2.00 × 0.10 × 2.70 | 32 |
| SM_DuvarKose_L | 1.10 × 1.10 × 2.70 | 24 |
| SM_Zemin_1m | 1.00 × 1.00 × 0.10 | 12 |
| SM_Tavan_1m | 1.00 × 1.00 × 0.10 | 12 |
| SM_KapiKanadi | 0.80 × 0.04 × 2.00 | 12 |
| SM_KapiSove | 0.90 × 0.12 × 2.05 | 28 |

**9 parça, toplam 172 üçgen.** Karar 46 "~9–10 parça" diyordu.

Boşluklar boolean farkla açıldı, kutu yapıştırılarak değil — parçanın içinde
görünmeyen yüzey kalmıyor, üçgen boşa gitmiyor.

## Nasıl çalıştırılır

```bash
Blender --background --python Kaynak/Blender/ev_kiti.py -- <cikti-klasoru>
Blender --background --python Kaynak/Blender/test_odasi.py -- <cikti> <cikti>
```

macOS'ta Blender yolu: `/Applications/Blender.app/Contents/MacOS/Blender`

Çıktı (`.blend`, `.fbx`, render) **commit edilmez** — script'ten üretiliyor,
kaynağı script. `.gitignore`'da.

> Bir parça ileride Blender'da elle düzenlenirse o parçanın `.blend` dosyası
> kaynak hâline gelir ve commit edilir. O anda bu belge güncellenmeli, yoksa
> script yeniden koşulduğunda el işi silinir.

## FBX ihraç ayarları

```
apply_scale_options = FBX_SCALE_NONE      global_scale = 1.0
axis_forward = Z                          axis_up = Y
bake_space_transform = True               object_types = {MESH}
mesh_smooth_type = FACE                   add_leaf_bones = False
```

Blender Z-yukarı, Unity Y-yukarı çalışır. Bu ayar dönüşümü ihraç anında
pişiriyor; model Unity'de boş bir objenin altında −90° dönmüş görünmüyor.
Deneme sürümünde tam burada takılınmıştı (karar 48).

### Deney: `axis_forward` neden `-Z` değil `Z`

Çoğu Blender→Unity kılavuzu `axis_forward='-Z'` önerir ve script ilk yazıldığında
bu kullanılmıştı. Gerçek bir Unity içe aktarmasıyla test edilince (bkz. aşağıdaki
"Unity'de içe aktarma" bölümü) **X ekseninin ters çıktığı** görüldü: pivot kuralı
"parça (0,0,0)'dan +X yönüne büyür" diyor, ama `-Z` ile ihraç edilen parça Unity'de
**−X yönüne** büyüyordu. Y (yükseklik) ve Z (derinlik) doğruydu, sadece X ters.

Dört `axis_forward` değeri (X, −X, Z, −Z), asimetrik bir test kutusuyla Unity'de
gerçekten ölçülerek karşılaştırıldı — sonuç sayı: sadece `Z` üçünü birden
(X, Y, Z) doğru işaretle ve doğru pivotla veriyor. Test render'ı ile yüzeylerin
içe dönmediği (normal/winding sorunu olmadığı) de doğrulandı.

**Kural: bu proje için Blender→Unity ihracında `axis_forward='Z'` kullanılır,
kılavuzların önerdiği `-Z` değil.** Sebebi bizim pivot kuralımız (ızgara hücresinin
iç-alt köşesi, +X yönüne büyüme) — başka bir pivot kuralı seçilseydi sonuç
değişebilirdi. Yeni bir modelleme script'i yazılırsa bu değer kopyalanır,
"mantıklı görünen" `-Z`'ye geri dönülmez.

## Unity'de içe aktarma — kontrol listesi

**Koşuldu (31.08.2026), geçici bir test worktree'sinde** — henüz PR'a girmiş
gerçek `Assets/` üzerinde değil, ama aynı proje sürümü ve aynı FBX dosyalarıyla:

- [x] Scale Factor = 1 (`globalScale: 1`, `useFileScale: 1` — meta dosyasında görüldü)
- [x] Duvar Unity'de 2.70 birim yüksek (mesh.bounds ile ölçüldü)
- [x] Pivot tabanda ve ızgara köşesinde: X ve Y için min = 0.00
- [x] Normal/winding bozulmadı — test render'ında yüzeyler solid, içe dönük değil
- [x] İki duvarı gerçek sahnede yan yana koyup boşluk/bindirme yok mu — Onat
      (C) Unity Editor'ü açıp `SM_Duvar_1m` ve `SM_Duvar_2m`'i sahneye
      sürükleyip kendi gözüyle baktı (31.08.2026): **boşluksuz birleşiyor**
- [ ] Read/Write Enabled kapalı — henüz ayarlanmadı, gerçek içe aktarmada yapılacak

FBX'ler `.meta` dosyalarıyla **birlikte** commit edilir (`docs/kurallar.md`).
Bu PR henüz FBX/`.meta` taşımıyor — onlar Unity projesi `dev`'e sabitlenip
gerçek `Assets/_Project/Art/Models/` içinde üretildiğinde ayrı bir adımda girer.
