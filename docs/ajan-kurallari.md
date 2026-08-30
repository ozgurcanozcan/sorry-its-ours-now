# AI Araçları — Kurallar

> Bu belge hem insanlar hem araçlar için. Makine tarafı `.cursorrules` ve
> `CLAUDE.md` dosyalarında özetlenmiş hâliyle duruyor; üçü de aynı şeyi söyler.

## Temel kural

**Aynı repoda aynı anda tek ajan çalışır.**

Ajan = çok dosyalı, otonom değişiklik yapan araç. Claude Code ajandır.
Cursor'ın composer/agent kipi ajandır. Cursor'ın sohbet kipi değildir.

İkisi aynı anda çalışırsa git geçmişi anlamsızlaşır, çakışma çözülemez hâle
gelir ve "bu satırı kim, neden yazdı" sorusunun cevabı kaybolur.

## Ajanların uyacağı kurallar

### Yasaklar

- `UnityEngine.Random` kullanma. `Code/Utils/Rastgele` (tohumlu) kullanılır.
- Singleton yazma. `Instance` property'si yok.
- `Scenes/*.unity` dosyalarına dokunma.
- `.meta` dosyalarını elle düzenleme — Unity üretir.
- Başka rolün klasörüne yazma (bkz. `kurallar.md`).
- Paylaşılan imzayı (özellikle `Code/Core`) değiştirme. Ekleme serbest.

### Zorunluluklar

- Kod Türkçe yazılır: `OyuncuKontrolcu`, `yurumeHizi`, `EsyaVerisi`.
  Unity'nin kendi tipleri İngilizce kalır: `Transform`, `MonoBehaviour`.
- Yorumlar "ne" değil **"neden"** anlatır. Kodun ne yaptığı zaten okunuyor.
- Rastgelelik tohumlu olur, tohum `OyunDurumu` içinde saklanır.
- Her sistem kodunun EditMode testi olur.
- Sayı uydurulmaz. Denge sayısı ölçülerek belirlenir (bkz. aşağıda).

### Sayı kuralı

Bu proje bir kez şuna çarptı: günlük %2 faiz oranı yazıldı, makul görünüyordu,
ve **mükemmel oynayan bir oyuncunun bile borcunu düşürmesini imkânsız kılıyordu.**
42 günlük simülasyon koşturulunca çıktı.

> **Kural:** Denge sayısı (faiz, kapasite, fiyat, süre, olasılık) yazılırken
> ya ölçülür, ya da "şu koşulu sağlamalı" biçiminde bir kısıt olarak yazılır.
> "Makul görünüyor" gerekçe değildir.

## Şartname ile çalışan ajanlar

Claude dışında bir ajan (Antigravity vb.) kullanılacaksa şu döngüyle:

1. Claude `docs/gorevler/<no>.md` altına şartname yazar: hangi dosya, hangi
   imza, hangi kabul kriteri, **neye dokunmayacak**
2. Ajan kendi dalında çalışır: `agent/<no>`
3. Şartnamede yazmayan dosyaya dokunmaz
4. PR açar → **önce Claude inceler** (derleme, test, şartnameye uygunluk)
5. Sonra insana gider

**Uygun işler:** test yazımı, veri tanımı üretme, mekanik refactor, tekrarlı
dönüşümler.

**Uygun olmayan işler:** tasarım kararı, denge ayarı, sahne düzenleme,
"neden"in "ne"den önemli olduğu her şey.

## Otomasyon

Şu an yok, bilerek.

Kurulacak ilk otomasyon şu olmalı ve **GitHub Actions ile** yapılmalı:

```
PR açıldı
  → Unity batch: derle + EditMode testleri koştur
  → sonucu PR'a yorum olarak yaz
```

Bu, projenin bilinen darboğazını (inceleme kapasitesi) hedefler. n8n gibi bir
orkestratör ancak Actions yetmediğinde gelir — ikinci bir proje kurmaya değmez.

Otomatik model çağrıları para yakar: her push'ta değil, PR açılışında ve `dev`
birleşmesi öncesinde tetiklenir.

## İnceleme zinciri

```
ajan/insan kod yazar
  → Claude inceler (derleme, test, kurallara uygunluk)
    → insan onaylar
      → dev
```

Claude kendi yazdığını da inceler ama **kendi PR'ını onaylayamaz.** İnsan onayı
atlanmaz.
