# Müzayede çözümleyici — 01.09.2026

> Hafta 1, Gün 4 (B · Özgür). `docs/plan.md`: *"12 alıcı + müzayede
> çözümleyici (ekransız)."* Karar: Özgür.

---

## 80. Alıcı havuzu 12, müzayedeye gelen 4

**Karar:** Havuz **12** kişi kalıyor, müzayedeye **4**'ü geliyor.

`03-muzayede.md` ve `05-icerik.md` bunu açık soru olarak işaretlemişti:
*"12 havuz / 8 gelen oranı gözden geçirilmeli."*

**Sorun mutlak sayılarda değil, oranda.** Denemede ölçülen yapılandırma
**24 havuz / 8 gelen** idi — havuzun **üçte biri** salondaydı ve
*"bu hafta Antikacı var mı?"* gerilimi oradan doğdu.

| | Oran | |
|---|---|---|
| Deneme (ölçüldü) | 8/24 = **1/3** | çalıştı |
| M1'e yazılan | 8/12 = **2/3** | havuz neredeyse her hafta salonda |
| **Karar** | 4/12 = **1/3** | ölçülen oranı geri getiriyor |

**Neden havuzu büyütmek değil:** Her alıcı bir demet **ölçülmemiş sayı**
(bütçe, kategori primleri, varsayılan çarpan, katılım ağırlığı). 12 alıcı
zaten ~50 ölçülmemiş sayı; 24 alıcı bunu ikiye katlardı. Katalogda hâlihazırda
eşyaların yarısının fiyatı ölçülmemişken bu yük kabul edilemezdi.

**Kabul edilen risk — ince salon.** 4 alıcıyla bazı lotlarda ikinci teklif
olmayabilir. İki şey bunu yumuşatıyor:

1. **Varsayılan çarpan sıfır değil.** Herkes her şeye bir şeyler veriyor,
   sadece ilgilenmediğine az. Lot alıcısız kalmıyor, ucuza gidiyor.
2. `MuzayedeTestleri.HerLottaEnAzIkiTeklifVar` bunu **ölçüyor**: lotların
   %25'inden fazlasında ikiden az teklif olursa test kırılıyor.

Düzeltmesi tek sayı: `GelenSayisi` 4 → 5.

---

## 81. Tek alıcı + rezervsiz lot tabandan gider — bu korunacak

**Bulgu, sonra karar.** Test yazarken çıktı ve **testim yanlış, kod doğruydu.**

İkinci fiyat açık artırmada fiyat ikinci teklife dayanır. Salonda tek teklif
varsa ikinci teklif **sıfırdır**, yani fiyat tabana düşer:

```
emekli amca (bütçe 2.500), gardırop (gerçek değer 8.950)
  değerlendirmesi : 9.845 TL
  ödediği         :    50 TL   (teklif artışı)
```

İlk hâlde bunu bir hata sandım ve "10 gardırop 2.500 TL'lik bütçeyi
tüketmeli" diye test yazmıştım. Tüketmiyor: 10 × 50 = 500 TL.

**Karar: bu davranış korunuyor.** Çünkü `03-muzayede.md`'nin
*"düşük rezerv = yakma riski"* dediği şeyin mekanik karşılığı tam olarak bu.
Rezerv koymayan oyuncu, salonda tek alıcı varsa eşyasını bedavaya veriyor.
Rezervin *neden* bir karar olduğu buradan geliyor.

`MuzayedeTestleri.TekAliciVeRezervsizLotTabandanGider` bunu koruyor —
biri "tek alıcı kendi teklifini ödesin" diye değiştirirse rezerv kararı
sessizce anlamsızlaşırdı.

> **Playtest'te izlenecek:** oyuncu bunu öğrenene kadar kaç eşya yakıyor?
> Öğrenmesi bir "aha" anı mı, yoksa cezalandırıcı bir sürpriz mi? Cevap
> ikincisiyse rezerv için varsayılan bir öneri gerekebilir.

---

## 82. Lot sırasını çözümleyici belirlemez

**Karar:** `MuzayedeCozumleyici` lotları verilen sırayla çözer, sıralamaz.

**Neden:** Bütçe oturum boyunca tükeniyor (`03-muzayede.md`, ölçülmüş bulgu).
Tükendiğine göre **hangi lotun önce çıktığı sonucu değiştirir** — ve
*"neyi önce satıyorum"* kararı tam olarak buradan doğuyor. Çözümleyici
sıralasaydı o kararı oyuncunun elinden alırdı.

`MuzayedeTestleri.LotSirasiSonucuDegistirir` bunu ölçüyor.

---

## Ölçülmemiş sayılar

Hepsi Hafta 5 denge işi. Kısıtları `MuzayedeTestleri` içinde:

| Ne | Kısıt |
|---|---|
| 12 alıcının bütçeleri | Gelen 4'ünün ortalama toplamı, tipik müzayede arzından **küçük** olmalı — yoksa bütçe tükenmez |
| Kategori primleri | Her kategoride en az bir alıcı 1'in üstünde çarpan vermeli |
| Katılım ağırlıkları | Nadir alıcı ölçülebilir biçimde nadir olmalı (Koleksiyoncu 2 < Hurdacı 9) |
| Teklif artışı (50 TL) | Oyunu değiştirmiyor, sayının okunabilirliği için |

---

## Bilerek yapılmayan

**Uyumlu set çarpanı yok.** `03-muzayede.md` *"tek tek veya set; uyumlu set
çarpan alır"* diyor ama lot kurma (tezgâhta, fiziksel) `docs/plan.md`'de
Hafta 2 Gün 5'te. Set çarpanı oraya ait; buraya konsaydı ölçülmemiş bir sayı
daha eklerdi.

**Canlı salon yok** — kilitli karar (`03-muzayede.md`). Sonuç anında hesaplanıyor.

---

## Dokunduğu yerler

```
Assets/_Project/Code/Auction/Alici.cs
Assets/_Project/Code/Auction/AliciKatalogu.cs
Assets/_Project/Code/Auction/Lot.cs
Assets/_Project/Code/Auction/MuzayedeAyarlari.cs
Assets/_Project/Code/Auction/MuzayedeCozumleyici.cs
Assets/_Project/Code/Auction/MuzayedeSonucu.cs
Assets/_Project/Tests/EditMode/MuzayedeTestleri.cs
```
