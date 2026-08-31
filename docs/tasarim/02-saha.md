# 02 — Saha

Evi boşaltma aşaması. Oyunun yarısı burada.

## Dört kısıt

Her eşya bu dördünün kesişiminde bir bulmaca:

> **süre · kapasite · gürültü · yasa**

Bir eşyayı almak bu dördünden en az ikisine dokunmuyorsa o eşya karar değil,
toplama işidir.

## Taşıma: tek eşya, elde *(karar: 31.08.2026)*

Oyuncu **aynı anda tek eşya taşır** ve onu elinde tutar. Her eşya için kamyona
gidip gelmek gerekir.

Envanter yok, cep yok. Gümüş çay takımı da elde taşınır, gardırop da.

Bunun üç sonucu var ve üçü de istenen:

- **Mesafe önem kazanır.** Evin arka odasındaki bir eşya, kapının yanındakinden
  pahalıya mal olur. Oda yerleşimi (C'nin işi) doğrudan oynanışa dokunur.
- **Büyük eşya gerçekten yorucudur.** Ağırlık bir sayı değil, hissedilen bir şey.
- **Komiktir.** Kanepeyi kapıdan geçirmeye çalışmak bu oyunun görsel şakası.

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
Ağırlık, hacim, kırılganlık. Bazı eşyalar tek kişiyle taşınamaz.

Kırılgan eşya düşerse **bir kondisyon kademesi iner** (iyi → eski → hasarlı).
Denemede "değerin yarısı gider" şeklindeydi; kademe sistemi hem daha okunaklı
hem görsel olarak gösterilebilir.

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

- [x] Tek eşya, elde *(karar: 31.08.2026)*
- [x] Kamyona gidip gelinir — tek eşya taşındığı için zorunlu *(31.08.2026)*
- [x] Gerçek zamanlı sayaç *(karar: 31.08.2026)*
- [ ] Ev sahibi konuşuyor mu? (seslendirme / gıdıklama sesi / sadece yazı)
- [ ] Köpek ve komşu M1'de var mı, sonraya mı kalıyor?
- [ ] Dar itibar somut olarak nasıl işliyor? Kaç kötü davranıştan sonra dosyalar
      kötüleşir, oyuncu bunu nereden anlar, geri kazanılabilir mi?
- [ ] Aletler ne işe yarıyor? (levye = kilitli oda, el arabası = kapasite?)
- [ ] **İki kişi gereken eşyalar tek kişilikte nasıl alınıyor?** Sürüklenerek mi
      (yavaş), el arabasıyla mı, yoksa hiç alınamıyor mu? Tek eşya taşıma kararı
      bu soruyu acil hâle getirdi.
- [ ] Kamyon nerede park ediyor? Eve uzaklığı sabit mi, ev tipine göre değişiyor mu?
      (Apartmanda sokakta, gecekonduda kapının önünde gibi.)
