# 31.08.2026 — İlk tasarım kararları

**Onaylayan:** Özgür (B)
**Bağlam:** Deneme sürümünden taşınan tasarım belgelerinde 36 açık soru vardı.
Diğerlerinin çoğunu kilitleyen dördü karara bağlandı.

---

## 1. Depo ayrı bir aşama değil, oyuncunun MERKEZİ

Gün depoda başlar, depoda biter, arada iki kez daha oraya dönülür.

İçinde: **bilgisayar** (haciz dosyaları, şirket durumu, borç, yükseltmeler),
**alet rafı** (matkap, levye vb.), **tezgâh** (temizleme, tamir, lot kurma),
**eşya alanı**, **garaj**.

Depo aynı zamanda bir **ilerleme ekseni** — oyun ilerledikçe daha gelişmiş
depolara geçilir.

**Sonuçları:**
- "Ofis" ve "Hazırlık" ayrı aşamalar olmaktan çıktı, ikisi de depoda
- Satılmayan eşya artık kaybolmuyor, depoda kalıyor → **rezerv kararı anlam kazandı**
  (denemede rezervin hiçbir getirisi yoktu, çünkü satılmayan buharlaşıyordu)
- Bürokrasi mizahının doğal yeri belirlendi: bilgisayar ekranı
- Günün süresi uzayabilir — depoda geçen zamanın "idari iş" değil "hazırlık"
  hissi vermesi bir denge sorunu, oynayarak çözülecek

Belge: `docs/tasarim/07-depo.md`

## 2. İtibar dar hâliyle kalıyor

Ayrı bir ilerleme ağacı yok. Sadece ceza tarafı: yasak eşya almak, yalan ilan
vermek → **iş emirlerinin kalitesi düşer**.

"Memur mu çakal mı" diye iki oynanış tarzı kurulmuyor. Gerekçe: borç zaten
merkezi sayaç, ikinci bir ilerleme ekseni dikkati böler.

Katalog abartma mekaniği bu dar hâle bağlı olarak **kalıyor**.

## 3. Rakip haciz şirketi: hafif dokunuş

Bazı iş emirleri "kapılmış" gelir; arada müzayedede karşına çıkıp fiyatı
yükseltir. Ayrı bir rakip AI'ı **yok**.

Gerekçe: gerçek bir rakip sistemi ayrı bir yapay zekâ demek — kapsam olarak
ciddi iş, karşılığı belirsiz.

## 4. Ev sahibi: gıdıklama sesi + yazı

Anlamsız mırıltı sesle çıkar, söylediği konuşma balonunda yazılı görünür.

Gerekçe: ses baskı ve karakter verir (sessiz sahne gerilim taşımaz), yazı anlamı
taşır ve çeviriye açıktır. Gerçek seslendirmenin maliyeti ve her dil için tekrar
kayıt derdi yok — yerellik kuralına da uygun.

---

# 31.08.2026 — Depo iç yapısı (ikinci tur)

**Onaylayan:** Özgür (B)

## 5. Depo fiziksel bir mekân — içinde yürünüyor

Menü değil. Sahayla aynı kip: birinci şahıs. Bilgisayara yürüyüp başına
geçiyorsun, raftan alet alıyorsun.

**Gerekçe:** depo "merkez" hissi vermeli ve bürokrasi mizahının bir mekânı olmalı.

**Maliyeti:** C için ciddi iş — depo, saha evleri kadar önemli bir mekân ve
oyuncunun en çok vakit geçirdiği yer. Modüler oda kiti buraya da hizmet etmeli.
A için: depodaki etkileşim (oturma, raftan alma) sahadaki eşya alma etkileşimiyle
tek sistem olmalı.

## 6. Bekletmenin tek bedeli: yer

Kira yok, bayatlama yok. Eşya depoda yer kaplar; depo dolunca yeni iş alınamaz.

**Gerekçe:** bedeli para değil **fırsat** yapmak. Aynı zamanda depo yükseltmesini
anlamlı bir ödüle çeviriyor: daha çok yer = daha çok sabır.

**Dikkat:** depo kapasitesi eşya hacimleriyle aynı ölçekte tutulmalı. Denemede
kamyon kapasitesi tam bu yüzden kırılmıştı (12/20/32 birim vs 0.06–2.20 m³).

## 7. Aletler: sınırlı slot

Hacimle ilgisi yok, kamyon yerinden yemiyor. Ayrı ve okunaklı bir kısıt.

## 8. Tamirci — mini oyun değil, hizmet

Dealer's Life 2 modeli: bilgisayardan tamirci seçilir, ücret ödenir, eşya
tamire girer, süre dolunca değeri artmış olarak kullanıma döner.

**Gerekçe:** karar el becerisi değil, zamanlama ve ekonomi. *"Cuma müzayedesine
yetişir mi?"*

**Eşya tamirdeyken depoda KALIR ve yer kaplamaya DEVAM EDER.** Üstünde
`TAMİRDE · Kalan süre: X gün` uyarısı görünür; alınamaz, lota konamaz, satılamaz.

> Bu madde bir açığı kapatmak için böyle. İlk taslakta eşya tamire gidince
> depodan çıkıyordu — o hâlde en iyi strateji "depo dolunca her şeyi tamire
> yolla" olurdu ve tamir, ekonomik karar olmaktan çıkıp depolama hilesine
> dönerdi. Yer kaplamaya devam edince tamirin üç maliyeti birden oluyor:
> para, süre ve yer.
>
> Yan fayda: oyuncu eşyasının nerede olduğunu görmeye devam ediyor.

---

# 31.08.2026 — Kondisyon sistemi (üçüncü tur)

**Onaylayan:** Özgür (B)

## 9. Eşyalar üç kondisyondan birinde: hasarlı · eski · iyi durumda

Sürekli bir "durum" sayısı değil, üç ayrık hâl. Denemede 0.55–1.25 arası
sürekli bir çarpandı; ayrık hâle geçiliyor.

**Gerekçe:** hem okunabilirlik hem üretilebilirlik. Oyuncu bakınca anlamalı,
C sonsuz ara hâl modellememeli.

**Kondisyon görsel olarak desteklenecek.** Oyuncu etiket okumadan ayırt edebilmeli.

### Doğan kural: kondisyon görünür, değer görünmez

Oyuncu eşyanın hasarlı mı iyi mi olduğunu bakarak anlar; ne ettiğini bilmez.
Belirsizlik kondisyondan değil, piyasadan ve salonda kimin olduğundan gelir.

Kondisyon da gizli olsaydı oyuncunun elinde hiçbir bilgi kalmaz, karar kumara
dönerdi.

### Bağlı değişiklikler

- **Kırılgan eşya düşerse bir kademe iner** (iyi → eski → hasarlı). Denemede
  "değerin yarısı gider" şeklindeydi; kademe hem okunaklı hem görsel.
- **Tamir bir kademe yükseltir** ve **tahmin aralığı güncellenir** — oyuncu
  ne kazandığını görür. (Karar 8'in tamamlayıcısı.)

### C için üretim uyarısı

20 eşya × 3 kondisyon = 60 görünüm. Üç ayrı model olarak yapılırsa üretim üçe
katlanır. Doğru yol **tek model + kondisyon katmanı**: aynı mesh, üstüne
malzeme değişimi veya decal (toz, çizik, çatlak, ezik, sararma).

Bu, low-poly stiline de uygun — kondisyon biçimden değil yüzeyden okunur.

---

# 31.08.2026 — Tamir derinleşti (dördüncü tur)

**Onaylayan:** Özgür (B)

## 10. Kaç kademe tamir edileceğini oyuncu seçer

Bir kademe ucuz ve hızlı, iki kademe pahalı ve uzun.
`hasarlı → eski` ya da `hasarlı → iyi durumda`.

Her eşyada ikinci bir soru daha doğuyor: *"yarım mı bırakayım, sonuna kadar mı
gideyim?"* Cuma müzayedesine yetişmesi gereken eşyada tek kademe akıllıca olabilir.

## 11. Tamir başarısız olabilir

Başarı ihtimali iki şeye bağlı:
- **Ürün klasmanı** — eşyanın tamir zorluğu (yeni kavram, tanımlanacak)
- **Tamircinin becerisi** — ucuz/acemi ya da pahalı/usta

İki kademe atlamak doğal olarak daha riskli.

### Doğan kural: ihtimal önceden gösterilir

Oyuncu "onayla" demeden önce başarı yüzdesini görmeli.

> Gördüğü hâlde riske girdiyse bu bir **karar**.
> Görmeden başına geldiyse bu bir **ceza**.

Kondisyonun görünür olmasıyla aynı mantık: kararı verebilmek için gereken bilgi
oyuncuda olmalı. Gizli kalan **sonuç** olmalı, **olasılık** değil.

### Açılan yeni kavram: ürün klasmanı

Eşyanın tamir zorluğu. Kategoriyle aynı şey değil — aynı kategoride kolay ve zor
eşyalar olabilir. `05-icerik.md`'ye eklendi, tanımlanmayı bekliyor.

---

# 31.08.2026 — Zaman (beşinci tur)

**Onaylayan:** Özgür (B)

## 12. Tamir başarısızlığı: para ve süre gider, eşya bozulmaz

Sonuç sürenin sonunda belli olur. Üç gün beklersin, eşya aynı hâlde döner.

Riski gerçek ama adil kılıyor: kaybettiğin para ve zaman, elindeki mal değil.
Cuma'ya yetiştirmek için riske girdiysen ve tutmadıysa o hafta satamıyorsun —
ceza zaten bu.

## 13. Günü oyuncu bitirir

Sabit gerçek zamanlı süre yok. Hazır olduğunda "günü bitir" der.

## 14. Depoda da saat işler

Depoda oyalanmak bedava değil.

> **Çözülmemiş gerilim:** 13 ile 14 tam oturmuyor. Saat işliyorsa gün
> kendiliğinden de bitiyor demektir. İki okuma var (saatli gün / işlem sayaçlı
> gün), `01-dongu.md`'de yazılı. Karar bekliyor.

## 15. Müzayede haftada bir gün: CUMA

En güçlü ritim seçeneği. Kaçırırsan bir hafta beklersin ve o hafta boyunca eşya
depoda **yer kaplar**.

> **Doğan örtüşme:** borcun asgari ödemesi de haftalık. İkisi aynı güne denk
> geliyor — *"Cuma ödeme var"* cümlesi hem satış hem borç anlamına geliyor.
> Haftanın tamamı o güne akıyor. Bu tesadüf değil, tutulmalı.

Tamir süreleri de bu takvimde ölçülüyor: "üç gün sürer" demek "Cuma'ya yetişir mi?"
demek.

---

# 31.08.2026 — Ritim (altıncı tur)

**Onaylayan:** Özgür (B)

## 16. Saatli gün: 09:00 – 00:00, Schedule I modeli

Gün sabah dokuzda başlar, gece yarısında **duvara çarpar**. 00:00'dan sonra
hiçbir işlem yapılamaz; oyuncu sadece boş boş dolaşır ve "günü bitir" demek
zorunda kalır.

Ceza ekranı değil, bir **his**: geç kaldın, elin kolun bağlı, depoda dikiliyorsun.
Ezik bir kötü adamın gecesi — tona denk düşüyor.

13 numaralı karar ("günü oyuncu bitirir") böylece 14 ile uzlaştı: bitirmeyi
oyuncu yapar, ama ne zaman bitireceğini saat söyler.

## 17. Bir günde birden fazla iş yapılabilir

Zaman yeterse. Küçük gecekonduyu öğleden önce bitirip ikinciye gidilebilir.

*"Bu eve girersem akşama yetişir miyim?"* sorusu iş seçiminin parçası oluyor.

## 18. Sahada gerçek zamanlı sayaç

Eylem başına harcama değil. Oyalanmak da koşturmak da gerçek zaman yer.

Denemedeki "hamle bütçesi" bu değildi — o, konsolda gerçek zaman olmadığı için
kullanılan bir kuklaydı.

## 19. Tek eşya, elde taşınır

Envanter yok, cep yok. Her eşya için kamyona gidip gelinir.

**Sonuçları (üçü de istenen):**
- Mesafe önem kazanır → oda yerleşimi (C) doğrudan oynanışa dokunur
- Büyük eşya gerçekten yorucu olur → ağırlık hissedilen bir şey olur
- Komik olur → kanepeyi kapıdan geçirmek bu oyunun görsel şakası

**Doğan denge sorunu:** tek eşya + gerçek zamanlı sayaç birleşince bağlayıcı
kısıt kapasite değil **süre** olma riski var. Kamyon hiç dolmadan zaman biterse
"neyi alıp neyi bırakıyorum" sorusu "neye yetişebilirim"e indirgenir. İkisinin
de bağlayıcı olması gerekiyor; ancak oynayarak ayarlanır.

**Acil hâle gelen soru:** iki kişi gerektiren eşyalar (buzdolabı, gardırop,
koltuk takımı) tek kişilikte nasıl alınıyor?

---

# 31.08.2026 — Saha ayrıntıları (yedinci tur)

**Onaylayan:** Özgür (B)

## 20. Ağır eşya: tek kişilikte el arabası, co-op'ta iki kişi

Aynı eşya, moda göre iki farklı çözüm. Co-op geldiğinde tek kişilik oyun
kırılmıyor, sadece başka bir yolu açılıyor.

**Dikkat:** el arabası tek kişilikte zorunlu hâle gelirse alet slotu kararı ölür.
Çözüm hazırlıkta: haciz dosyası evde büyük eşya olup olmadığını sezdirmeli.
"Eşyalı 3+1 daire" ile "boş gecekondu" farklı hazırlık ister.

## 21. Kamyon ev tipine göre park eder

Gecekonduda kapının önünde, apartmanda sokakta. Mesafe, ev tipinin zorluğunun
parçası — apartman zaten merdivenli, bir de kamyon uzakta.

Tek eşya taşındığı için (karar 19) bu doğrudan süreye yansıyor.

## 22. Gizli değer: önce ipucu, sonra arama

Kör arama yok. En doğal ipucu kaynağı **ev sahibinin kendisi**: bir eşyayı fazla
koruyorsa, önüne geçiyorsa orada bir şey vardır.

> Bu, ev sahibini engel olmaktan çıkarıp **bilgi kaynağına** çeviriyor. Aynı
> karakter hem yavaşlatıyor hem nereye bakacağını söylüyor — ve bunu istemeden
> yapıyor. Tona da uygun: adam kendini ele veriyor.

## 23. M1'de tek canlı engel: ev sahibi

Köpek ve komşu (polis sayacı) sonraki miladlara. Dar kapı, asansörsüz kat ve
kilitli oda mekân kaynaklı engeller olarak kalıyor — C'nin oda kitinden doğuyorlar,
ayrı sistem gerektirmiyorlar.

---

# 31.08.2026 — Slot, kapasite, piyasa (sekizinci tur)

**Onaylayan:** Özgür (B)

## 24. Alet rafı 5 slot — VE küçük eşyalarla paylaşılır

> **Bu karar, 19 numaralı kararı GÜNCELLER.** 19'da "tek eşya, elde, envanter yok"
> denmişti. Artık: orta/büyük eşya elde ve tek tek, **küçük eşya slota girer**,
> slotlar aletlerle ortak.

Levye + el arabası aldıysan geriye 3 slot kalır — o sefer taşıyabileceğin küçük
eşya sayısı üçtür.

**Doğan takas: alet mi, kapasite mi?** Hazırlıktaki "neyi feda ediyorsun?" sorusu
her sefere yayılıyor; alet seçimi bir kez değil sürekli hissediliyor.

## 25. Küçük eşya ayrı bir işaretle belirlenir (`cebeGirer`)

Hacimden türetilmiyor. Yağlıboya tablo hacimce küçük ama ince ve geniş — cebe
girmemeli. Otomatik kural bunu ayırt edemez.

## 26. Depo kapasitesi hacimle ölçülür (m³)

Eşyaların zaten hacmi var, kamyon da hacimle ölçülüyor. Tek birim, tek mantık.

## 27. Piyasa trendi kalıyor — bilgisayardan takip edilir

Haftalık kategori dalgalanması. Depodaki bilgisayarda okunur.

Bekleme kararını zenginleştiriyor: *"bu hafta antika prim yapıyor, kilimi şimdi
çıkarayım"* ya da *"beyaz eşya düşmüş, gelecek haftaya bırakayım."*

Bilgisayarın rolü genişliyor: dosya + hesap + tamirci + **piyasa**.

## 28. Para harcama kalemleri

- Kamyon yükseltmesi
- Depo yükseltmesi
- Alet satın alma
- **İşletme giderleri** — tamir ücreti ve benzeri iş kalemleri

Borç ödemesi bunlarla yarışıyor: *"bugünü mü yarını mı besliyorsun?"*
