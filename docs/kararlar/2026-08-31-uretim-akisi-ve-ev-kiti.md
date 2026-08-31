# 31.08.2026 — Üretim akışı ve ev kiti (on altıncı tur)

**Onaylayan:** Onat (C)
**Bağlam:** C'nin Blender deneyimi yok. Kararlar bu gerçeğe göre alındı:
M1'in gri kutu sanatı elle modellenmek yerine script'le üretilecek.

---

## 58. Sanat üretimi script ile: Blender Python

Modüler kit ve gri kutu eşyalar `Kaynak/Blender/` altındaki Python
script'leriyle, Blender arayüzsüz çalıştırılarak üretilir.

**Gerekçe:** modüler bir kitte hataların neredeyse tamamı ölçü hatasıdır.
Script'te ölçü tek yerde durur, elle modellemede her parçada tekrar eder.
Ayrıca C'nin Blender bilmemesi M1'i bloke etmiyor.

**Sınırı açıkça yazalım:** script'in ürettiği şey **gri kutudur**. Doğru
boyutta, doğru pivotlu, ama karakteri yok. M1 için istenen tam olarak bu
(`docs/plan.md`: "Güzel görünmesi gerekmiyor. Eğlenceli olması gerekiyor").
**M2 ve sonrası için yeterli değil** — karakterli low-poly eşya, ev sahibi
modeli ve el yapımı siluet insan işidir. O noktada ya C öğrenmiş olacak, ya
hazır varlık alınacak, ya dördüncü kişi konuşulacak. Karar M2'ye bırakıldı
ama şimdiden bilinsin.

**Kaynak dosyalar `Assets/` dışında,** `Kaynak/Blender/` altında durur.
`Assets/` içine konursa Unity `.blend` dosyalarını kendisi içe aktarmaya
kalkar ve herkesin makinesinde Blender kurulu olmasını şart koşar.

Üretilen `.blend`, `.fbx` ve render'lar commit edilmez — kaynağı script.

## 59. İsimlendirme: `SM_` öneki

Statik mesh'ler `SM_` önekiyle adlandırılır: `SM_Duvar_1m`, `SM_KapiKanadi`.
Kit ve eşyalar için ortak.

## 60. Çarpışma: kit parçalarında Mesh Collider

**Karar 52 duruyor** — eşyalarda otomatik Box Collider.

Ama **kit parçaları için Box Collider çalışmaz**: kapı boşluklu duvarın kutusu
kapıyı kapatır, oyuncu geçemez. Kit parçalarına Mesh Collider verilir.

> Bu A'yı (Çağatay) doğrudan ilgilendiriyor: oyuncu kontrolü ve eşya taşıma
> kit parçalarının collider'ına çarpacak.

## 61. Tek kat yüksekliği: 2.70 m

Gecekondu ve daire aynı kat yüksekliğinde. Farklı yükseklik kiti ikiye katlar.

İki ev tipi arasındaki fark **yerleşim ve malzemeden** gelir: gecekondu dar
koridorlu ve az odalı, daire merdivenli ve kamyon uzakta (karar 21).

## 62. Ev kiti ölçüleri

| Değer | m |
|-------|---|
| Izgara modülü | 1.00 |
| Kat yüksekliği | 2.70 |
| Duvar kalınlığı | 0.10 |
| Kapı boşluğu | 0.80 × 2.00 |
| Pencere boşluğu | 1.20 × 1.40 |
| Denizlik kotu | 0.90 |

Ayrıntı ve pivot kuralı: `docs/uretim/ev-kiti.md`

> **Kapı genişliği bir denge sayısıdır.** `02-saha.md` dar kapıyı engel sayıyor;
> kanepeyi kapıdan geçirmek oyunun görsel şakası. 0.80 m tek başına
> değiştirilmez — A'nın taşıma denemesini doğrudan etkiliyor.
