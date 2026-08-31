# 03 — Müzayede

Oyunun diğer yarısı. "Çöpü sanat eseri diye pazarlama" kısmı.

## Mekanikler

| Mekanik | Özet | Durum |
|---------|------|-------|
| Lot kurma | Tek tek veya set. Uyumlu set çarpan alır. | Tanımlı |
| Rezerv | Alt fiyat. Yüksek = satmama riski, düşük = yakma riski. | Tanımlı |
| Alıcılar | Farklı kategoriye farklı para veren kişilikler | Tanımlı |
| Piyasa | Haftalık trend, haber bülteninden sezilebilir | **Açık** |
| Restorasyon | Zaman + para → değer çarpanı. Her eşyada kâr etmez. | **Açık** |
| Katalog | Abartılı ilan → daha çok alıcı, ama iş emirleri kötüleşir | Dar itibara bağlı |

## Kilitli karar: canlı salon yok

İlk sürümde müzayede **sonucu anında hesaplar ve gösterir.** Canlı salon
animasyonuna ancak eğlenceli olduğu kanıtlandıktan sonra yatırım yapılır.

Kapsam kesme sırasında en üstteki madde bu.

## Deneme sürümünden çıkan ölçümler

**İkinci fiyat açık artırma iyi çalışıyor.** Kazanan en yüksek teklifi veren ama
ödediği, ikinci teklifin biraz üstü. Sonucu şu ve tasarım açısından önemli:
**rezerv sana yüksek fiyat kazandırmaz, sadece ucuza gitmeni engeller.**

**Alıcı bütçesi oturum boyunca tükenmeli.** Tükenmezse her lot bağımsız hesaplanır
ve alıcı çeşitliliği anlamsızlaşır — 3.000 ₺ bütçeli komşu 6.000 ₺'lik alışveriş
yapabiliyordu. Tükendiğinde "neyi önce satıyorum" kararı doğuyor.

**Tahmin aralığı dar olmalı.** Antikada ±%55 aralık kararı kumara çeviriyordu;
±%28'e inince karar geri geldi. Belirsizliğin asıl kaynağı fiyat tahmini değil,
**salonda kimin olduğu.**

**Alıcı havuzu geniş, katılım rastgele.** 24 kişilik havuzdan her müzayedeye 8'i
gelirse "bugün antikacı var mı" gerilimi doğuyor. Oyuncu havuzu görmez, oynayarak
sezer.

**Rezerv, satılmayan eşya kaybolduğu sürece anlamsız.** Denemede yüksek rezerv
koymanın hiçbir getirisi yoktu çünkü satılmayan lot buharlaşıyordu. Rezervin
anlamlı olması için **eşyanın elde kalıp sonra tekrar çıkarılabilmesi** gerekiyor.

> Bu sonuncusu doğrudan `01-dongu.md`'deki **depo** sorusuna bağlanıyor.
> Depo yoksa rezerv de yok.

## Açık sorular

- [x] Satılmayan eşya **depoda kalır**, sonraki müzayedeye tekrar çıkarılabilir
      *(karar: 31.08.2026)*. Bekletmenin bedeli hâlâ açık — `07-depo.md`
- [ ] Piyasa trendi oyunda kalıyor mu? Kalıyorsa oyuncu nereden öğreniyor?
- [ ] Restorasyon var mı? Varsa mini oyun mu, tek tuş mu, zaman mı harcıyor?
- [ ] Katalog abartma kalıyor (bedeli: iş emri kalitesinin düşmesi). Açık olan,
      abartmanın nasıl yapıldığı — kaydırıcı mı, hazır ifade seçimi mi?
- [ ] Lot kurma arayüzü nasıl? Sürükle-bırak mı, liste mi?
- [ ] Oyuncu müzayedeye kaç lot çıkarabiliyor? Sınır var mı?
- [ ] Alıcı sayısı: havuzda kaç kişi, her seferinde kaç kişi gelecek?
