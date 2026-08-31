# 05 — İçerik

Eşyalar, evler, alıcılar, palet. Bu belge **içerik envanteri** — mekanikler
başka belgelerde, burada "ne kadar ve hangileri" var.

## Eşyalar

Deneme sürümünde 20 eşya tanımlandı ve dengesi denendi. Alanlar:

`ad · kategori · taban değer · ağırlık · hacim · kırılgan · haczedilebilir · bolluk · değer belirsizliği`

**Bolluk** (1–10): bir evde bulunma sıklığı. Yatak 9, el dokuma kilim 1.
Antikanın nadir olması şart — her ev antika doluysa değer biçme gerilimi kalmaz.

**Kategoriler:** Mobilya · Elektronik · Antika · Mutfak · Kişisel · Çöp ·
Beyaz Eşya · Aletler

## Kondisyon — üretim maliyeti *(karar: 31.08.2026)*

Her eşya üç kondisyondan birinde: **hasarlı · eski · iyi durumda**.
Ve bu **görsel olarak desteklenecek** — oyuncu bakınca ayırt edebilmeli.

> **C için kritik karar:** 20 eşya × 3 kondisyon = 60 farklı görünüm.
> Bunu üç ayrı model olarak yaparsak üretim üçe katlanır ve iş oraya gömülür.
>
> Doğru yol: **tek model + kondisyon katmanı.** Aynı mesh, üstüne malzeme
> değişimi veya decal — toz, çizik, çatlak, ezik, sararma. Böylece yeni bir
> eşya eklemek hâlâ tek model demek olur.
>
> Bu yaklaşım oyunun stiline de uygun: low-poly'de kondisyon farkı biçimden
> değil yüzeyden okunur.

**Açık:** kondisyon katmanı nasıl kurulacak — malzeme varyantı mı, decal mı,
vertex renk mi? Bu C'nin ilk mimari kararlarından biri ve 20 eşyanın hepsini
etkiliyor, erken verilmeli.

> Eşya tablosunun kendisi taşınacak (denemede dolduruldu ve dengesi ölçüldü),
> ama önce kategori listesi ve alan setinin son hâli onaylanmalı.

## Ev tipleri

| Tip | Kademe | Karakteri |
|-----|--------|-----------|
| Gecekondu / tek katlı | 1 | Küçük, dar, az eşya |
| Apartman dairesi | 2 | Merdiven, asansör yok, komşu |
| Dükkân | 3 | Ticari envanter, mesleki alet |
| Lüks villa | 4 | Güvenlik, kamera, gerçek antika |
| Fabrika / depo | 5 | Devasa eşya |

**Açık:** her ev tipinde kaç oda, kaç eşya, hangi bolluk dağılımı?

## Alıcılar

Denemede 24 kişilik havuz kuruldu ve çalıştı. Her alıcının alanları:

`ad · huy (tek cümle) · bütçe · kategori primleri · varsayılan çarpan · katılım sıklığı`

Havuzdan her müzayedeye 8 kişi geliyordu. Oyuncu havuzu görmüyor, oynayarak
seziyor.

**Açık:** 24 sayısı denemede seçildi, doğrulanmadı. Havuz kaç kişi olmalı,
her seferinde kaç kişi gelmeli?

## Ev sahipleri

Karikatür figürler. Ağlayan değil, absürt derecede inatçı (`06-ton.md`).

**Açık:** kaç farklı ev sahibi tipi? Her birinin ayrı davranışı mı var, yoksa
tek davranış farklı görünüşler mi?

## Renk paleti

8–12 renk seçilecek. Low-poly'de tutarlılık modelden değil paletten gelir.

| # | Hex | Kullanım |
|---|-----|----------|
| 1 | | |

Ayrıca gri kutu aşamasında **kategori renkleri** lazım: oyuncu eşyanın ne
olduğunu uzaktan ayırt edebilmeli. Sekiz kategori var.

**Sahibi:** C (Onat).

## Açık sorular

- [ ] Kategori listesi kesin mi? "Çöp" mü "Hurda" mı — ad seçilmeli
- [ ] İlk sürümde kaç eşya olacak? 20 yeter mi, 40 mı gerekli?
- [ ] Alıcı havuzu kaç kişi?
- [ ] Her ev tipi için oda ve eşya sayısı
- [ ] Palet — C belirleyecek
