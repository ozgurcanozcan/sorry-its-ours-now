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

## Zaman *(karar: 31.08.2026)*

**Günü oyuncu bitirir.** Sabit bir gerçek zamanlı süre yok; hazır olduğunda
"günü bitir" der.

**Ama saat her yerde işler** — sahada da, depoda da. Depoda oyalanmak bedava
değil.

**Müzayede haftada bir gün: Cuma.**

Bu üçü birleşince ritim şöyle kuruluyor: hafta boyunca ev boşaltıp depoyu
doldurursun, Cuma günü satarsın. Cuma'ya yetişemeyen eşya bir hafta bekler —
ve o hafta boyunca depoda **yer kaplar**.

> **"Cuma ödeme var" cümlesi artık iki anlama geliyor:** borcun asgari ödemesi
> de Cuma, müzayede de Cuma. Haftanın tamamı o güne doğru akıyor. Borç ile
> müzayede aynı takvim üzerinde buluşuyor — bu tesadüf değil, tutulmalı.

Tamir süreleri de bu takvimde ölçülüyor: *"üç gün sürer"* demek *"Cuma'ya
yetişir mi?"* demek.

### Çözülmesi gereken gerilim

"Günü oyuncu bitirir" ile "depoda saat işler" birbirine tam oturmuyor.
Saat işliyorsa gün kendiliğinden de bitiyor demektir. İki okuma var:

- **Saatli gün:** günün bir açılış–kapanış aralığı var (09:00–18:00 gibi).
  Saat dolunca gün biter; "günü bitir" erken kapatma seçeneğidir. Cuma
  müzayedesinin de bir saati vardır — geç kalırsan kaçırırsın.
- **İşlem sayaçlı gün:** saat yok, eylem var. Depodaki her işlem günden bir
  parça yer. Gün, işlem bütçesi bitince ya da oyuncu isteyince biter.

Birincisi daha zengin (müzayede saati, tamirci mesaisi, "yetişemedim" gerilimi),
ikincisi daha sade. Karar verilmedi.

## Açık: kampanyanın uzunluğu

Gün serbest olunca kampanyanın kaç saat ettiği de belirsizleşiyor. Bu, oyunun
fiyatını doğrudan etkiliyor — M2'de ölçülmesi gereken bir şey.

## Açık sorular

- [ ] Bir günde kaç iş alınabiliyor? Tek mi, birden fazla mı?
- [ ] Oyuncu bir işi reddedebiliyor mu? Reddetmenin bedeli ne?
- [ ] Aşamalar sıralı mı, yoksa oyuncu istediğinde müzayedeye gidebiliyor mu?
      (Bekletme mekaniği varsa ikincisi gerekli olabilir.)
- [x] Günü oyuncu bitirir, saat her yerde işler *(karar: 31.08.2026)* — ama
      ikisi arasındaki gerilim çözülmedi, yukarı bak
- [x] **Müzayede haftada bir gün: Cuma** *(karar: 31.08.2026)*. Borcun asgari
      ödemesi de Cuma — haftanın tamamı o güne akıyor.
