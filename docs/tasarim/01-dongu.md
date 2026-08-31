# 01 — Günlük Döngü

Oyunun omurgası. Her aşama **bir karar** sorar; karar sormayan aşama aşama
değildir, menüdür.

**Merkez depodur.** Gün orada başlar, orada biter, arada iki kez oraya dönülür.
Ayrı bir "ofis" ya da "hazırlık" ekranı yok — ikisi de depoda (`07-depo.md`).

| # | Nerede | Ne yapılıyor | Sorduğu karar |
|---|--------|--------------|---------------|
| 1 | **Depo — bilgisayar** | Haciz dosyası seç (adres, borç, tahmini değer, risk, süre) | *hangi riski alıyorsun?* |
| 2 | **Depo — raf ve garaj** | Alet ve kamyon seç | *neyi feda ediyorsun?* |
| 3 | **Saha** | Evi boşalt | *neyi alıp neyi bırakıyorsun?* |
| 4 | **Depo — tezgâh** | Temizle, tamir et, lot kur | *emeği nereye harcıyorsun?* |
| 5 | **Müzayede** | Rezerv koy, sat | *bugün mü satıyorsun, bekliyor musun?* |
| 6 | **Depo — bilgisayar** | Hesap, borç ödemesi, yükseltme | *bugünü mü yarını mı besliyorsun?* |

Depo üç kez geçiyor — çünkü mekân değil, merkez.

## Aşamaların durumu

| Aşama | Durum |
|-------|-------|
| Depo | Rolü net (merkez, ilerleme ekseni), içi **açık** — `07-depo.md` |
| Saha | En tanımlı aşama. Dört kısıt belli — `02-saha.md` |
| Müzayede | Mekanikler listeli, derinlik kısmen **açık** — `03-muzayede.md` |
| Kapanış | Borç sistemi tanımlı — `04-ekonomi.md` |

## Bir günün süresi

**Açık.** Bilinen tek kısıt: saha ziyareti için 8 dakika konuşuldu.

Depo üç kez devreye girdiği için bir gün 15–20 dakikaya çıkabilir. Bu, kampanyanın
15–20 görevinin kaç saat ettiğini ve oyunun fiyatını doğrudan belirliyor.

> **Dikkat:** depo merkez olunca "menüde geçen süre" artıyor. Oyunun ritmi
> saha–müzayede gerilimi üzerine kurulu; depoda geçen zaman *hazırlık* hissi
> vermeli, *idari iş* hissi değil. Bu bir denge sorunu ve ancak oynayarak
> çözülür.

## Açık sorular

- [ ] Bir günde kaç iş alınabiliyor? Tek mi, birden fazla mı?
- [ ] Oyuncu bir işi reddedebiliyor mu? Reddetmenin bedeli ne?
- [ ] Aşamalar sıralı mı, yoksa oyuncu istediğinde müzayedeye gidebiliyor mu?
      (Bekletme mekaniği varsa ikincisi gerekli olabilir.)
- [ ] Bir oyun günü gerçek zamanda kaç dakika?
- [ ] Müzayede her gün mü, haftanın belirli günlerinde mi? İkincisi "bugün mü
      satıyorum" kararını güçlendirir ama günleri birbirinden farklı kılar.
