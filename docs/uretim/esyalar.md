# Eşyalar — 7 öncelikli, gri kutu

> Sahibi: C (Onat) · `docs/plan.md` Hafta 1, gün 5
> Alan şeması: `docs/tasarim/05-icerik.md`

`docs/uretim/ev-kiti.md`'deki gibi script ile üretildi (`Kaynak/Blender/esyalar.py`),
aynı doğrulanmış ihraç ayarıyla (`axis_forward='Z'`, bkz. ev-kiti.md "Deney").

**Bunlar gri kutu.** Doğru boyut, doğru pivot, karakteri yok — M1 için istenen
tam bu (`docs/plan.md`: "güzel görünmesi gerekmiyor, eğlenceli olması gerekiyor").
Karakterli hâle M2'de geçilecek, karar 53'te not düşüldü.

## Pivot kuralı — ev kitinden farklı

Ev kiti parçalarının pivotu ızgara hücresinin köşesindeydi (mimari eleman,
duvara hizalanıyor). **Eşyalarda pivot taban-merkezde** (X,Z ortada, Y=0'da) —
çünkü bunlar zeminde serbest duran objeler; yere koyunca kendiliğinden hizalanır,
döndürmek de kendi merkezi etrafında olur. Bu, oyun objeleri için genel pratik.

## Ölçüler — gerçek/bilinen mobilya ölçüleri

| Eşya | Genişlik×Derinlik×Yükseklik (m) | Kaynak |
|------|----------------------------------|--------|
| Yatak (çift kişilik) | 1.60 × 2.00 × 0.50 | Standart çift kişilik yatak+baza ölçüsü |
| Buzdolabı | 0.60 × 0.65 × 1.80 | Tipik no-frost dolaplı buzdolabı |
| Çamaşır makinesi | 0.60 × 0.60 × 0.85 | Standart önden yüklemeli |
| Gardırop | 1.00 × 0.55 × 2.00 | 2 kapılı orta boy |
| Masa | 1.20 × 0.80 × 0.75 | 4 kişilik dikdörtgen yemek masası |
| Halı | 2.00 × 1.40 × 0.02 | Orta boy salon halısı |
| TV | 1.00 × 0.08 × 0.60 | ~42–48" düz ekran |

Bunlar gerçek dünya ölçüsü, tahmin değil — sıradan bilinen mobilya boyutları.

## Ağırlık ve hacim

| Eşya | Ağırlık (kg) | Hacim (m³, karar 26) |
|------|-------------|------------------------|
| Yatak | ~35 | 1.60 |
| Buzdolabı | ~65 | 0.70 |
| Çamaşır makinesi | ~70 | 0.31 |
| Gardırop | ~80 | 1.10 |
| Masa | ~25 | 0.72 |
| Halı | ~8 (rulo) | 0.056 |
| TV | ~12 | 0.048 |

Hacim, kutu boyutlarından direkt hesaplandı (m³, karar 26 — depo/kamyon kapasitesiyle
aynı birim). Ağırlık, bu ürün sınıfları için yaygın bilinen tipik değerler —
üretici kataloglarının genel ortalaması, tek bir modelin spec'i değil. Denge
sayısı sayılmıyor (kapasite/fiyat/süre gibi oyunu kilitleyen bir şey değil),
ama yanlışsa (özellikle "iki kişi mi gerekir" sınırında kalan bir eşya) playtest'te
düzeltilir.

## Taban değer — gerçek 2. el piyasa referansı

**Ağustos 2026, web araması ile toplandı** (sahibinden/letgo örnekleri).
Bunlar oyun ekonomisine göre **ölçeklenmedi** — ham piyasa referansı. Ölçekleme
`docs/plan.md` Hafta 5 "denge" işi, oynayarak yapılacak (borç/haftalık ödeme
büyüklüğüne göre).

| Eşya | 2. el piyasa aralığı (TL) | Not |
|------|---------------------------|-----|
| Yatak | 3.500 – 15.000 | Orta segment 5.000–8.000 |
| Buzdolabı | 4.500 – 13.000 | Ev tipi ürünler alt-orta banda yakın |
| Çamaşır makinesi | 3.500 – 11.000 | Kapasiteye göre geniş aralık |
| Gardırop | 3.000 – 14.900 | Kapı sayısı/boyuta göre |
| Masa | *ayrı veri yok* | Bulunan referans 6 kişilik takım (35.000 TL) — tek masa için güvenilir değil, **tahmini** |
| Halı | 500 – 3.999 | Antika olmayan sıradan halı; antika 50.000+ ayrı kategori |
| TV | ~5.950 (tek örnek, 42") | Tek veri noktası, geniş aralık için daha fazla arama gerekir |

> **Masa ve TV için veri zayıf** — açıkça yazılıyor, gizlenmiyor. Masa satır
> "tahmini" işaretli, gerçek referans yok. Hafta 5'te ya daha iyi kaynak
> bulunur ya da playtest ile ayarlanır.

## Diğer alanlar (`05-icerik.md` şeması)

| Eşya | kategori | kırılgan | haczedilebilir | ikiKişiGerekir | cebeGirer | sabit | bolluk |
|------|----------|----------|-----------------|-----------------|-----------|-------|--------|
| Yatak | Mobilya | hayır | **hayır — yasak liste** | evet | hayır | hayır | 9 |
| Buzdolabı | Beyaz Eşya | hayır | evet | evet | hayır | hayır | 7 |
| Çamaşır makinesi | Beyaz Eşya | hayır | evet | evet | hayır | hayır | 7 |
| Gardırop | Mobilya | hayır | evet | evet | hayır | hayır | 6 |
| Masa | Mobilya | hayır | evet | hayır | hayır | hayır | 8 |
| Halı | Mobilya | hayır | evet | hayır | hayır | hayır | 7 |
| TV | Elektronik | **evet** | evet | hayır | hayır | hayır | 6 |

**Yatak `haczedilebilir: hayır`** — `02-saha.md` yasak listesinde açıkça yazıyor
("Yatak, mutfak eşyası, mesleki alet"). Oyuncu alabilir ama süre/gürültü
cezası var, para cezası değil (karar, denemede alınmıştı).

**Bolluk değeri "Yatak 9"** — `05-icerik.md`'de zaten örnek olarak verilmişti,
birebir kullanıldı. Diğerleri (`7,6,8` gibi) benim ilk tahminim, playtest'te
ayarlanabilir — bolluk bir denge sayısı değil, içerik dağılım parametresi.

## Tanımlanmamış alanlar — uydurulmadı, açık bırakıldı

`05-icerik.md`'nin kendisi şunları **açık soru** işaretliyor, ben de kapatmadım:

- **klasman** (tamir zorluğu) — "kaç kademe, kategoriden mi türüyor" henüz karara
  bağlanmamış bir sistem. Bu bir tasarım kararı, B'nin (Özgür) ya da ekibin işi.
- **değer belirsizliği** (±%) — `03-muzayede.md`'de genel kural ±%28 (antikada
  ±%55 kumar oluyordu). Eşya başına ayrı yüzde vermek yerine bu genel kuralın
  kullanılması öneriliyor, ama kesinleşmiş değil.

## Parçalar

| Dosya | Ölçü (m) | Üçgen |
|-------|----------|-------|
| SM_Yatak | 1.60 × 2.00 × 0.50 | 12 |
| SM_Buzdolabi | 0.60 × 0.65 × 1.80 | 12 |
| SM_CamasirMakinesi | 0.60 × 0.60 × 0.85 | 12 |
| SM_Gardirop | 1.00 × 0.55 × 2.00 | 12 |
| SM_Masa | 1.20 × 0.80 × 0.75 | 12 |
| SM_Hali | 2.00 × 1.40 × 0.02 | 12 |
| SM_TV | 1.00 × 0.08 × 0.60 | 12 |

**7 parça, 84 üçgen.** Kutu (blockout) olduğu için hepsi 12 üçgen — kondisyon
decal katmanı (karar 45) ve gerçek siluetler M2'de bu mesh'lerin üstüne gelir.

## Nasıl çalıştırılır

```bash
Blender --background --python Kaynak/Blender/esyalar.py -- <cikti-klasoru>
```

Çıktı commit edilmez (`.gitignore`), kaynağı script.

## Unity'de içe aktarma

**Koşuldu (31.08.2026), geçici bir test worktree'sinde** — ev kitiyle aynı
yöntem:

- [x] Sayısal ölçüm (mesh.bounds) — 7 eşyanın hepsi doğru boyut, doğru işaret,
      pivot tabanda (Y min = 0.00 hepsinde)
- [x] Görsel — Unity'nin gerçek grafik bağlamında render alındı, 7 obje de
      zeminde duruyor (havada asılı değil), yüzeyler dolu/solid (normal
      sorunu yok)

Kit için bulunan `axis_forward='Z'` ayarı sorunsuz taşındı, X ekseni sürprizi
tekrarlanmadı. Gerçek Unity Editor'de elle bakma (Onat gözüyle) henüz
yapılmadı — bu, FBX'ler gerçek `Assets/` içine girip commit edildiğinde olacak.
