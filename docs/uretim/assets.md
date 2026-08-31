# Dış kaynak asset değerlendirmesi

> Sahibi: C (Onat) · 31.08.2026
> M1 için gerek yok — mevcut gri kutu (`ev-kiti.md`, `esyalar.md`) yeterli ve
> doğrulanmış durumda. Bu belge **M2 için** araştırma: gri kutuyu karakterli
> modele çevirirken elle üretmek yerine hazır kaynak kullanmak mümkün mü?

Dördü de indirilip gerçekten ölçüldü (Blender headless, `Kaynak/Blender/`
script'leriyle aynı yöntem) — sayfa metnine güvenilmedi, "ölçülür, uydurulmaz"
kuralına uygun.

## Kaynaklar

| Kaynak | Link | Lisans |
|--------|------|--------|
| Kenney Furniture Kit | https://kenney.nl/assets/furniture-kit | CC0 |
| Kenney Building Kit | https://kenney.nl/assets/building-kit | CC0 |
| VNBP Low Poly Home Set | https://vnbp.itch.io/low-poly-home-set | CC-BY 4.0 (atıf gerekli) |
| LowPolyAssets Household Items | https://lowpolyassets.itch.io/low-poly-household-item-pack | CC0 |
| Broken Vector Storage Pack | https://brokenvector.itch.io/low-poly-storage-pack | **belirsiz** — sorulmuş, cevapsız |

## Karşılaştırma — ölçülmüş sonuçlar

| Kaynak | Birim | Pivot | Mutlak ölçek | Lisans |
|--------|-------|-------|--------------|--------|
| LowPolyAssets | ❌ keyfi (~5–7x büyük, tutarsız) | ❌ karışık, tabanda değil | — | ✅ CC0 |
| Kenney Furniture Kit | ✅ gerçek metre | ✅ tam tabanda | ⚠️ sistematik küçük (~%50–60) | ✅ CC0 |
| **VNBP Home Set** | ✅ gerçek metre | ✅ tam tabanda | **✅ gerçekçi, bize en yakın** | ⚠️ CC-BY |
| Broken Vector Storage | ✅ gerçek metre (EUR-Pallet 1.20×0.14×0.80 = gerçek endüstri standardı ile doğrulandı) | ❌ merkezden, tabandan değil | ✅ gerçekçi | ❌ belirsiz |

### Eşya bazlı ölçüm (bizim gerçek ölçümüzle karşılaştırma — `esyalar.md`)

| Eşya | Bizim ölçü (m) | Kenney (m) | VNBP Home Set (m) |
|------|----------------|------------|---------------------|
| Yatak | 1.60×2.00×0.50 | 0.96×1.13×0.38 | 1.21×2.51×0.35 |
| Buzdolabı | 0.60×0.65×1.80 | 0.43×0.29×0.92 | 0.93×0.72×1.90 |
| Çamaşır makinesi | 0.60×0.60×0.85 | 0.39×0.39×0.47 | 0.86×0.77×0.95 |
| Gardırop | 1.00×0.55×2.00 | 0.40×0.25×0.85 (bookcase, gerçek gardırop yok) | **1.19×0.51×2.27 (neredeyse birebir)** |
| Masa | 1.20×0.80×0.75 | 0.84×0.45×0.33 (küçük sehpa) | 2.72×1.02×1.00 (6 kişilik, büyük) |
| Halı | 2.00×1.40×0.02 | 1.57×0.92×0.01 | 1.00×1.00×0.01 (küçük varyant) |
| TV | 1.00×0.08×0.60 | 0.68×0.13×0.45 | **0.91×0.15×0.62 (neredeyse birebir)** |

VNBP Home Set'in duvar/kapı/pencere ölçüleri de ölçüldü: **2.00×0.20×3.00 m**
(genişlik×kalınlık×yükseklik) — kendi içinde tutarlı ama **bizim ızgaramızdan
farklı** (biz 1 m modül + 0.10 m kalınlık + 2.70 m kat kullanıyoruz, karar 57).
İki kiti karıştırmak parçaların birbirine oturmamasına yol açar.

## Bulgular

1. **Hiçbiri "indir, doğrudan kullan" değil.** Her birinin en az bir düzeltme
   ihtiyacı var: LowPolyAssets ölçek+pivot, Kenney mutlak boyut, Storage Pack
   pivot, Home Set ızgara uyumsuzluğu.
2. **En sağlam iki aday: Kenney (CC0, mobilya) ve VNBP Home Set (CC-BY,
   mimari+mobilya, en gerçekçi ölçek).** İkisi ayrı ızgara sistemi kullanıyor,
   **tek kaynaktan gidilmeli, karıştırılmamalı.**
3. **Storage Pack içerik olarak depo eşyaları için mükemmel uyuyor** (palet,
   raf, kutu, varil, dolap, locker, hazine sandığı) ama lisans netleşmeden
   kullanılamaz.
4. Blender 5.2 artık **COLLADA (.dae) desteklemiyor** — Storage Pack'in
   orijinal formatı .dae, ölçüm Python ile ham XML okunarak yapıldı. Gerçekten
   kullanılacaksa dönüştürme adımı (ör. eski Blender sürümü ya da başka bir
   araç) gerekecek.
5. **Kenney'nin FBX'i Unity'de varsayılan ayarla ~10 kat büyük içe aktarılıyor.**
   Blender'da OBJ üzerinden ölçülen boyut doğruydu (tabloda yazan), ama gerçek
   Unity iş akışında (FBX + varsayılan import ayarı) ayrı bir hata çıktı —
   dosyanın içine gömülü `UnitScaleFactor: 100` (2018 tarihli, eski bir FBX
   ihracı) Unity'nin `Use File Scale` otomatik hesabını bozuyor. Gerçek Unity
   Editor'de sınandı (Onat gözüyle) ve doğrulandı: **`Use File Scale`
   kapatılıp `Scale Factor: 0.1` elle girilince** mesh boyutu Blender ölçümüyle
   birebir eşleşiyor. Kenney kullanılacaksa bu içe aktarma ayarı **her FBX'e
   tek tek uygulanmalı**, varsayılana güvenilmemeli.

## Alınacak Kararlar

Bunlar ekipçe konuşulmadan kapatılmadı — bilerek açık bırakıldı.

- [ ] **M2'de hazır asset mi kullanılacak, yoksa elle mi üretilecek?** (Karar
  53'ün bıraktığı kapı — karar verilmedi, sadece seçenekler araştırıldı.)
- [ ] **Hazır asset kullanılacaksa: Kenney mi, VNBP Home Set mi?** İkisi farklı
  ızgara sistemi kullanıyor, ikisi birden seçilemez.
- [ ] **VNBP Home Set seçilirse:** CC-BY 4.0 atıf zorunlu — oyunun credits
  ekranına "Low Poly Home Set by VNBP - Leo" gibi bir satır eklenmeli. Bu iş
  kalemi unutulmamalı.
- [ ] **Broken Vector Storage Pack kullanılacaksa:** önce lisans netleştirilmeli
  — yazara itch.io üzerinden (yorum) ya da support@brokenvector.net üzerinden
  yazılmalı. Netleşmeden kullanılmaz.
- [ ] Hiçbiri seçilmezse: script tabanlı üretim (`Kaynak/Blender/`) devam eder,
  gri kutunun üstüne C elle karakter/doku ekler.
