# 02 — Saha

Evi boşaltma aşaması. Oyunun yarısı burada.

## Dört kısıt

Her eşya bu dördünün kesişiminde bir bulmaca:

> **süre · kapasite · gürültü · yasa**

Bir eşyayı almak bu dördünden en az ikisine dokunmuyorsa o eşya karar değil,
toplama işidir.

## Taşıma: elde tek eşya + 5 paylaşımlı slot *(karar: 31.08.2026)*

İki farklı taşıma yolu var:

**Elde** — orta ve büyük eşyalar. Aynı anda **tek tane**. Her biri için kamyona
gidip gelmek gerekir.

**Slotta** — küçük eşyalar cebe/çantaya girer. **5 slot var ve aletlerle
paylaşılır.**

> **Doğan takas: alet mi, kapasite mi?**
>
> Levye ve el arabası aldıysan geriye 3 slot kalır — o sefer cebe atabileceğin
> küçük eşya sayısı üçtür. Alet almazsan beş küçük eşya taşırsın ama kilitli
> odayı açamaz, gardırobu kaldıramazsın.
>
> Bu, hazırlık aşamasındaki *"neyi feda ediyorsun?"* sorusunu her sefere yayıyor.
> Alet seçimi bir kez değil, sürekli hissediliyor.

Elde taşımanın üç sonucu var, üçü de istenen:

- **Mesafe önem kazanır.** Evin arka odasındaki bir eşya, kapının yanındakinden
  pahalıya mal olur. Oda yerleşimi (C'nin işi) doğrudan oynanışa dokunur.
- **Büyük eşya gerçekten yorucudur.** Ağırlık bir sayı değil, hissedilen bir şey.
- **Komiktir.** Kanepeyi kapıdan geçirmeye çalışmak bu oyunun görsel şakası.

**Hangi eşya küçük sayılır:** hacimden türetilmiyor, her eşyada **ayrı bir
işaret** var (`cebeGirer`). Yağlıboya tablo hacimce küçük ama ince ve geniş —
cebe girmemeli. Elle karar verilebilmesi gerekiyor.

## Süre: gerçek zamanlı sayaç *(karar: 31.08.2026)*

Sahada gerçek zamanlı bir geri sayım işler. Eylem başına harcama değil —
oyalanmak da, koşturmak da gerçek zaman yer.

> **Not:** deneme sürümündeki "hamle bütçesi" bu değildi. O, konsolda gerçek
> zaman olmadığı için kullanılan bir kuklaydı. Gerçek oyunda süre sahnedeki
> hareketten doğuyor: koridorun uzunluğu, merdiven, dar kapı, kamyona kaç kez
> gidip geldiğin.

### Doğan denge sorunu

Tek eşya taşıma + gerçek zamanlı sayaç birleşince **bağlayıcı kısıt kapasite
değil süre olma riski taşıyor.** Kamyon hiç dolmadan zaman biterse "neyi alıp
neyi bırakıyorum" sorusu "neye yetişebilirim"e indirgenir.

İkisinin de bağlayıcı olması gerekiyor: bazı günler kamyon dolmalı, bazı günler
zaman bitmeli. Bu ancak oynayarak ayarlanır.

## Mekanikler

### Değerleme
Gerçek değer gizli. Oyuncu bir aralık görür (`~400–1.200 ₺`), deneyimle daralır.
Ayrıntı: `03-muzayede.md`.

### Kondisyon — üç hâl *(karar: 31.08.2026)*

Her eşya üç kondisyondan birinde: **hasarlı · eski · iyi durumda**.

Sürekli bir sayı değil, üç ayrık hâl. Sebebi hem okunabilirlik hem üretim:
oyuncu bakınca anlayabilmeli, C de sonsuz ara hâl modellemek zorunda kalmamalı.

> **Kural: kondisyon görünür, değer görünmez.**
>
> Oyuncu eşyanın hasarlı mı iyi mi olduğunu **bakarak** anlar. Ama o eşyanın
> ne ettiğini bilmez. Belirsizlik kondisyondan değil, piyasadan ve salonda
> kimin olduğundan gelir.
>
> Bu ayrım önemli: kondisyon da gizli olsaydı oyuncunun elinde hiçbir bilgi
> kalmaz, karar kumara dönerdi.

Kondisyon **görsel olarak desteklenir** — oyuncu etiket okumadan ayırt edebilmeli.

### Fizik
Ağırlık, hacim, kırılganlık.

Kırılgan eşya düşerse **bir kondisyon kademesi iner** (iyi → eski → hasarlı).
Denemede "değerin yarısı gider" şeklindeydi; kademe sistemi hem daha okunaklı
hem görsel olarak gösterilebilir.

### Ağır eşya: el arabası *(karar: 31.08.2026)*

Buzdolabı, gardırop, koltuk takımı gibi eşyalar tek kişiyle elde taşınamaz.

- **Tek kişilik modda:** el arabasıyla taşınır. Alet slotunda el arabası yoksa
  o eşya alınamaz.
- **Co-op'ta:** iki kişi gerekir. El arabası yerine ortak taşıma.

Aynı eşya, moda göre iki farklı çözüm. Co-op geldiğinde tek kişilik oyun
kırılmıyor, sadece başka bir yolu açılıyor.

> **Dikkat edilecek denge:** el arabası tek kişilikte neredeyse zorunlu hâle
> gelirse alet slotu kararı ölür — "her seferinde el arabası al" tek doğru
> cevap olur.
>
> Çözüm hazırlık aşamasında: **haciz dosyası evde büyük eşya olup olmadığını
> sezdirmeli.** "Eşyalı 3+1 daire" ile "boş gecekondu" farklı hazırlık ister.
> O zaman el arabası bir seçim olur, refleks değil.

### Kamyon nerede park ediyor *(karar: 31.08.2026)*

**Ev tipine göre değişir.** Gecekonduda kapının önünde, apartmanda sokakta.

Mesafe, ev tipinin zorluğunun bir parçası: apartman zaten merdivenli, bir de
kamyon uzakta. Tek eşya taşındığı için bu doğrudan süreye yansıyor.

### Gizli değer — ipucu, sonra arama *(karar: 31.08.2026)*

Yatak altı, dolap arkası, çerçeve arkası, dondurucu.

Kör arama yok. Önce **ipucu** gelir, sonra oyuncu arar ve arama süre yer.

En doğal ipucu kaynağı **ev sahibinin kendisi:** bir eşyayı fazla koruyorsa,
önüne geçiyorsa, ona doğru bağırıyorsa orada bir şey vardır.

> Bu, ev sahibini **engel olmaktan çıkarıp bilgi kaynağına** çeviriyor.
> Aynı karakter hem seni yavaşlatıyor hem de nereye bakacağını söylüyor —
> ve bunu istemeden yapıyor. Tona da uygun: adam kendini ele veriyor.

**Açık:** başka ipucu kaynakları var mı? (Açık çekmece, taze toprak, duvarda
soluk kare, kilitli çekmece.) Ve arama etkileşimi ne kadar sürüyor?

### Engeller — M1'de sadece ev sahibi *(karar: 31.08.2026)*

Köpek ve komşu (polis sayacı) **M1'e girmiyor**, sonraki miladlara kalıyor.
Kapsam kesme sırasında zaten üst sıralardaydılar.

M1'in tek canlı engeli ev sahibi. Dar kapı, asansörsüz kat ve kilitli oda
mekân kaynaklı engeller olarak kalıyor — onlar C'nin oda kitinden doğuyor,
ayrı bir sistem gerektirmiyor.

### Yasak liste
Yatak, mutfak eşyası, mesleki alet. Hukuki doğruluk aranmıyor, basitleştirilmiş.

**Karar (denemede alındı):** yasak eşyanın bedeli **para cezası değil, sahada
zorluk.** Gizlice çıkarmak süre yer, gürültü çıkarır, ev sahibini üstüne çeker.
Para cezası zayıf kalıyordu — oyuncu hesabı yapıp yine alıyordu ve karar ölüyordu.

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

- [x] Tek eşya, elde *(karar: 31.08.2026)*
- [x] Kamyona gidip gelinir — tek eşya taşındığı için zorunlu *(31.08.2026)*
- [x] Gerçek zamanlı sayaç *(karar: 31.08.2026)*
- [x] Gıdıklama sesi + yazı *(karar: 31.08.2026)*
- [x] M1.de sadece ev sahibi; köpek ve komşu sonraya *(karar: 31.08.2026)*
- [ ] Dar itibar somut olarak nasıl işliyor? Kaç kötü davranıştan sonra dosyalar
      kötüleşir, oyuncu bunu nereden anlar, geri kazanılabilir mi?
- [ ] Aletler ne işe yarıyor? (levye = kilitli oda, el arabası = kapasite?)
- [x] Tek kişilikte el arabası, co-op.ta iki kişi *(karar: 31.08.2026)*
- [x] Ev tipine göre değişir *(karar: 31.08.2026)*
