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

Dealer's Life 2 modeli: bilgisayardan tamirci seçilir, ücret ödenir, **eşya
depodan kaybolur**, süre dolunca değeri artmış olarak **geri döner**.

**Gerekçe:** karar el becerisi değil, zamanlama ve ekonomi. *"Cuma müzayedesine
yetişir mi?"*

**Yan etki (istenen):** eşya tamirdeyken depoda yer kaplamıyor. Yani tamire
yollamak, yer açmanın da bir yolu. İki sistem birbirini besliyor — bu tutmalı.
