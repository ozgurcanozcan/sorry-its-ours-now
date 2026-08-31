# 04 — Ekonomi ve İlerleme

## Borç — oyunun merkezi sayacı

- Devraldığın şirketin borcu var, **faiz işliyor**
- Her hafta asgari ödeme. Yapamazsan faiz yükselir ve **ekipmanın haczedilir**
- En kötü senaryo: tek kamyonetle başa dönmek
- **Oyun asla bitmez, iflas yok** — kilitli karar

Borcun asıl işi sahadaki riskli kararları anlamlı kılmak:
*"bu köpekli eve neden giriyorum?"* → *"çünkü cuma ödeme var."*

> **Tasarım kuralı:** Borç oyuncuyu **acele ettirsin, cezalandırmasın.**
> Kaybetme yavaş ve görünür olmalı; oyuncu her zaman toparlanabileceğini
> hissetmeli.

## Deneme sürümünden çıkan ölçümler

**Asgari ödeme faizi geçmeli.** Günlük %2 faiz makul görünüyordu ama haftada
~%14 ediyordu; asgari ödeme ise borcun %12'siydi. Sonuç: hiç ödeme kaçırmayan,
78.000 ₺ nakit biriktirmiş mükemmel oyuncunun bile borcu **artıyordu**.
Oyun matematiksel olarak kapalıydı.

> **Kısıt (sayı değil):** Haftalık asgari ödeme, haftalık faizden belirgin
> biçimde büyük olmalı. Aksi hâlde hiçbir oynayış borcu bitiremez.

**Dip olmalı, uçurum olmamalı.** Ekipmanı haczedilmiş ve hâlâ ödeyemeyen
oyuncunun borcu 42 günde 229.770 ₺'ye fırlıyordu — "kaybetme yavaş ve görünür
olmalı" kuralının tam tersi. Faizin dipte donması gerekiyor: alacak kalmadıysa
borç olduğu yerde bekler.

**Haciz ceza değil, acı bir takas olmalı.** Kamyon bir kademe küçülürken değeri
borçtan düşülünce oyuncu "kaybettim" değil "pahalıya ödedim" hissediyor.

## Şirket kademeleri

| # | Açılan | Yeni sorun |
|---|--------|-----------|
| 1 | Gecekondu, tek katlı ev | Kamyonet küçük |
| 2 | Apartman dairesi | Asansör, merdiven, komşu |
| 3 | Dükkân, küçük işletme | Ticari envanter, mesleki alet yasağı |
| 4 | Lüks villa | Güvenlik, kamera, gerçek antika |
| 5 | Fabrika, depo | Devasa eşya, ekip yönetimi |

Her kademe bir şey açıyor **ve** yeni bir sorun getiriyor. Sadece açan kademe
ilerleme değil, ödüldür.

## Kampanya — 3 perde, 15–20 görev

| Perde | Görev | Ne olur |
|-------|-------|---------|
| 1 | 1–6 | Devraldığın batık şirket, tek kamyonet, tek mahalle. Her görev bir mekaniği öğretir. |
| 2 | 7–14 | Apartman ve dükkânlar açılır. **Rakip haciz şirketi** sahneye girer, işleri kapar. |
| 3 | 15–20 | Villa ve fabrika ölçeği. Final kararı: borcu kapatıp temize çıkmak mı, rakibi yutmak mı? |

Kampanya bitince **Serbest Mod**: tüm kademeler açık, sınırsız prosedürel iş.

> **Teknik:** Kampanya görevleri elle yapılmış bölümler değil, **elle ayarlanmış
> tohumlar.** Aynı prosedürel sistem çalışır; görev sadece tohumu sabitler, birkaç
> odayı zorunlu kılar, belirli eşyaları garanti eder.
>
> Bu yüzden "rastgelelik tohumlu olsun" kuralı isteğe bağlı değil — kampanyanın
> var olabilmesi buna bağlı.

## Açık sorular

- [x] **Rakip şirket: hafif dokunuş** *(karar: 31.08.2026)*. Bazı iş emirleri
      "kapılmış" gelir, arada müzayedede karşına çıkıp fiyatı yükseltir. Ayrı bir
      rakip AI'ı yok. Açık kalan: kaç dosya kapılıyor, hangi sıklıkta salona geliyor?
- [x] Kamyon, depo, alet ve işletme giderleri (tamir vb.) *(karar: 31.08.2026)*
- [ ] Ekipman satın alınabiliyor mu, yoksa sadece kademe ilerledikçe mi geliyor?
- [x] 15-20 saat, ~25 USD bandı *(karar: 31.08.2026)*
- [x] Final kararı YOK. Borç kapanınca kampanya biter, serbest mod açılır.
- [x] Serbest modda borç YOK *(karar: 31.08.2026)* — ama baskı kaynağı açık,
      aşağı bak

## Serbest mod — borçsuz *(karar: 31.08.2026)*

Kampanya bitince açılan serbest modda **borç ve faiz yok.** Baskısız toplama
ve satma; rahatlama modu.

> **Çözülmesi gereken:** borç bu oyunun merkezi sayacı. *"Bu köpekli eve neden
> giriyorum?"* sorusunun cevabı *"çünkü cuma ödeme var"* idi. Borç gidince o
> cevap da gidiyor — riskli karar almanın sebebi kalmıyor.
>
> Serbest modun kendi baskısı olmalı. Adaylar:
> - **Koleksiyon** — nadir eşyaları tamamlama, müzede sergileme
> - **Hedefler** — "en pahalı satışını yap", "şu evi eksiksiz boşalt"
> - **Depo/kamyon yükseltme zinciri** — para hâlâ bir şeye yarıyor
> - **Zorluk kademeleri** — daha zor evler, daha sıkı süre
>
> Karar verilmedi. Ama "borç yok" tek başına yeterli bir tasarım değil,
> yerine bir şey konmalı.

## Kampanya uzunluğu: 15–20 saat *(karar: 31.08.2026)*

~25 USD fiyat bandı hedefleniyor.

> **Maliyet uyarısı:** 15–20 görev × ~1 saat demek. Bu, 60+ eşya, 5 ev tipi ve
> geniş bir alıcı havuzu gerektiriyor. Üç kişilik ekip için ciddi bir içerik
> yükü — kapsam kesme sırası (`00-ozet.md`) burada devreye girecek.
>
> 12 aylık takvimde bu hacim gerçekçi değilse kısaltmak yerine **çıkışı
> ertelemek** doğru olur; yarım içerikle 25 USD istemek en kötü seçenek.
