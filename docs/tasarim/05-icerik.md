# 05 — İçerik

Eşyalar, evler, alıcılar, palet. Bu belge **içerik envanteri** — mekanikler
başka belgelerde, burada "ne kadar ve hangileri" var.

## Ölçek

| Ne | M1 (prototip) | Çıkış |
|----|---------------|-------|
| Eşya çeşidi | **20** | **60+** |
| Alıcı havuzu | **12** | **30+** |
| Müzayedeye gelen | 8 *(gözden geçirilmeli)* | 8 |
| Ev tipi | 1–2 (gecekondu, daire) | 5 |

M1'de 20 eşya yeterli — denemede bu sayıyla oynandı ve mekanik çalıştı.
Çıkışa kadar kademeli büyür, C'nin yükü zamana yayılır.

> **12 havuz / 8 gelen oranı sorunlu.** Havuzun üçte ikisi her hafta salonda
> olursa *"bu hafta Koleksiyoncu var mı?"* gerilimi kaybolur — oysa mekaniğin
> bütün anlamı orada.
>
> M1'de gelen sayısı **4–5** olmalı, ya da havuz 20'ye çıkmalı. Alıcılar veri
> (model değil), yani havuzu büyütmek C'ye değil B'ye yük.

## Eşya veri alanları

```
ad · id · kategori · taban değer · ağırlık · hacim
kırılgan · haczedilebilir · ikiKişiGerekir · cebeGirer · sabit
bolluk · değer belirsizliği · klasman
```

**Kategoriler:** Mobilya · Elektronik · Antika · Mutfak · Kişisel · **Hurda** ·
Beyaz Eşya · Aletler

*"Çöp" adı Hurda oldu (31.08.2026): Hurdacı alıcısıyla eşleşiyor, ve bakır
tesisatın değerli olması "çöp" adıyla çelişiyordu.*

### Alanların anlamı

**bolluk** (1–10) — bir evde bulunma sıklığı. Yatak 9, el dokuma kilim 1.
Antikanın nadir olması şart; her ev antika doluysa değer biçme gerilimi kalmaz.

**cebeGirer** — küçük eşya mı? 5 paylaşımlı slota giriyor (`02-saha.md`).
Hacimden türetilmiyor: yağlıboya tablo hacimce küçük ama ince ve geniş, cebe
girmemeli. Elle karar verilebilmeli.

**sabit** — duvara/zemine bağlı. Radyatör, mutfak dolabı, bakır tesisat, klima,
avize. **Matkap** olmadan hiç alınamaz; sökme işlemi ayrıca süre yer.

**klasman** — tamir zorluğu. Tamircinin becerisiyle birlikte başarı ihtimalini
belirler (`07-depo.md`).

> **Açık:** klasman kaç kademe? Kategoriden mi türüyor, ayrı alan mı, taban
> değerle mi ilişkili? "Pahalı olan zordur" basit bir kural olurdu ama
> *"ucuz ama zor"* eşyaları imkânsız kılar — oysa o birleşim ilginç: kimsenin
> tamir ettirmeye değmeyeceği bir şey.

## Kondisyon — üretim maliyeti

Her eşya üç kondisyondan birinde: **hasarlı · eski · iyi durumda**, ve bu
**görsel olarak desteklenecek**.

> **C için kritik karar:** 20 eşya × 3 kondisyon = 60 görünüm. Üç ayrı model
> olarak yapılırsa üretim üçe katlanır.
>
> Doğru yol: **tek model + kondisyon katmanı** — aynı mesh, üstüne malzeme
> değişimi veya decal (toz, çizik, çatlak, ezik, sararma). Yeni eşya eklemek
> hâlâ tek model demek olur.
>
> Low-poly stiline de uygun: kondisyon biçimden değil yüzeyden okunur.

**Açık:** katman nasıl kurulacak — malzeme varyantı, decal, yoksa vertex renk?
C'nin ilk mimari kararlarından biri, 20 eşyanın hepsini etkiliyor, erken verilmeli.

## Ev tipleri

| Tip | Kademe | Karakteri |
|-----|--------|-----------|
| Gecekondu / tek katlı | 1 | Küçük, dar, az eşya. Kamyon kapının önünde. |
| Apartman dairesi | 2 | Merdiven, asansör yok. Kamyon sokakta. |
| Dükkân | 3 | Ticari envanter, mesleki alet yasağı |
| Lüks villa | 4 | Güvenlik, kamera, gerçek antika |
| Fabrika / depo | 5 | Devasa eşya |

**Ertelendi:** her ev tipinde kaç oda, kaç eşya, hangi bolluk dağılımı.
Denemedeki ölçekle (3–4 oda / 12–15 eşya) başlanır, playtest'te ayarlanır.

## Alıcılar

Alan seti:

```
ad · huy (tek cümle) · bütçe · kategori primleri · varsayılan çarpan · katılım sıklığı
```

Oyuncu havuzu **görmez**, oynayarak sezer. Katılım sıklığı gizli.

Denemede 24 kişilik havuz kuruldu ve çalıştı; kişilikler (Antikacı, Hurdacı,
Koleksiyoncu, Spekülatör, Emlakçı, Lokantacı, Emekli amca, Yabancı alıcı,
Rakip firma sahibi…) taşınabilir.

## Ev sahipleri

Karikatür figürler. Ağlayan değil, absürt derecede inatçı (`06-ton.md`).

Sadece engel değil, **bilgi kaynağı**: bir eşyayı fazla koruyorsa orada gizli
değer vardır (`02-saha.md`).

**Açık:** kaç farklı tip? Her birinin ayrı davranışı mı var, yoksa tek davranış
farklı görünüşler mi?

## Renk paleti

8–12 renk. Low-poly'de tutarlılık modelden değil paletten gelir.

Ayrıca **kategori renkleri** lazım (gri kutu aşamasında): oyuncu eşyanın ne
olduğunu uzaktan ayırt edebilmeli. Sekiz kategori var.

**Sahibi:** C (Onat).

## Açık sorular

- [ ] Klasman kaç kademe, neye göre belirleniyor?
- [ ] Kondisyon katmanı nasıl kurulacak (malzeme / decal / vertex renk)?
- [ ] Her ev tipi için oda ve eşya sayısı — bilerek ertelendi
- [ ] Kaç farklı ev sahibi tipi?
- [ ] Renk paleti — C belirleyecek
- [ ] Alıcı havuzu / gelen sayısı oranı gözden geçirilmeli
