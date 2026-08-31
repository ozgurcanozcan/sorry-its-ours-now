# 02 — Saha

Evi boşaltma aşaması. Oyunun yarısı burada.

## Dört kısıt

Her eşya bu dördünün kesişiminde bir bulmaca:

> **süre · kapasite · gürültü · yasa**

Bir eşyayı almak bu dördünden en az ikisine dokunmuyorsa o eşya karar değil,
toplama işidir.

## Mekanikler

### Değerleme
Gerçek değer gizli. Oyuncu bir aralık görür (`~400–1.200 ₺`), deneyimle daralır.
Ayrıntı: `03-muzayede.md`.

### Fizik
Ağırlık, hacim, kırılganlık. Düşen televizyon değerinin yarısını kaybeder.
Bazı eşyalar tek kişiyle taşınamaz.

### Gizli değer
Yatak altı, dolap arkası, çerçeve arkası, dondurucu. Aramak süre yer.

**Açık:** aramak nasıl yapılıyor? Ayrı bir etkileşim mi, yoksa eşyayı taşırken
kendiliğinden mi çıkıyor? Oyuncu nereye bakacağını nereden biliyor?

### Yasak liste
Yatak, mutfak eşyası, mesleki alet. Hukuki doğruluk aranmıyor, basitleştirilmiş.

**Karar (denemede alındı):** yasak eşyanın bedeli **para cezası değil, sahada
zorluk.** Gizlice çıkarmak süre yer, gürültü çıkarır, ev sahibini üstüne çeker.
Para cezası zayıf kalıyordu — oyuncu hesabı yapıp yine alıyordu ve karar ölüyordu.

### Engeller
Ev sahibi · köpek · komşu (polis sayacı) · dar kapı · asansörsüz kat · kilitli oda.

**Açık:** her birinin nasıl çalıştığı. Şu an sadece liste.

### İtibar — dar hâliyle *(karar: 31.08.2026)*
Ayrı bir ilerleme ağacı **yok**. Sadece ceza tarafı var: yasak eşya almak, yalan
ilan vermek gibi davranışlar **iş emirlerinin kalitesini düşürür**.

Yani itibar bir puan değil, bir sonuç: kötü oynarsan bilgisayara düşen dosyalar
kötüleşir. "Memur mu çakal mı" diye iki ayrı oynanış tarzı kurulmuyor — borç
zaten merkezi sayaç, ikinci bir eksen dikkati böler.

## Deneme sürümünden çıkan ölçümler

Bunlar kod değil, **tasarım bulgusu.** Kod atıldı, bulgular kalıyor.

**Kapasite eşya hacimleriyle aynı ölçekte olmalı.** Kamyon kapasitesi eşyaların
toplamından belirgin biçimde küçük değilse "neyi alıp neyi bırakıyorum" sorusu
yok olur. Denemede 15 eşyalık evden ancak 5–6 eşya alınabildiğinde karar doğdu.

**Yasak eşya süre maliyeti olarak güçlü.** Buzdolabını çıkarmak 8 dakikalık
ziyaretin 3 dakikasını yiyordu — yani üç sıradan eşyadan vazgeçmek demekti.
Bu gerçek bir karardı; para cezası değildi.

**Kırılgan eşyanın riski süreye bağlanınca anlam kazanıyor.** Son dakikalarda
alınan cam eşyanın kırılma ihtimali artınca "acele etme" davranışı doğdu.

**Ev içeriği bolluğa göre üretilmeli.** Düz rastgele seçimde gecekonduda üç el
dokuma kilim çıkıyordu; hem kurgu bozuluyor hem antika nadirliği eriyordu.

## Açık sorular

- [ ] Oyuncu aynı anda kaç eşya taşıyabiliyor? Tek mi, birkaç mı?
- [ ] Kamyona gidip gelme var mı, yoksa eşya alınca doğrudan kamyona mı sayılıyor?
- [ ] Süre gerçek zamanlı sayaç mı, yoksa eylem başına mı harcanıyor?
- [ ] Ev sahibi konuşuyor mu? (seslendirme / gıdıklama sesi / sadece yazı)
- [ ] Köpek ve komşu M1'de var mı, sonraya mı kalıyor?
- [ ] Dar itibar somut olarak nasıl işliyor? Kaç kötü davranıştan sonra dosyalar
      kötüleşir, oyuncu bunu nereden anlar, geri kazanılabilir mi?
- [ ] Aletler ne işe yarıyor? (levye = kilitli oda, el arabası = kapasite?)
