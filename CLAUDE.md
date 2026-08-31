# Sorry, It's Ours Now

Unity 6 (`6000.5.10f1`, URP) oyunu. Birinci şahıs, stilize low-poly, kara komedi:
gündüz borçlunun evini boşalt, akşam müzayedede sat.

Ekip: **A** Çağatay (saha) · **B** Özgür (sistem/UI) · **C** Onat (teknik sanat).

## Önce bunları oku

| Belge | Ne zaman |
|-------|----------|
| `docs/plan.md` | Ne üzerinde çalışıldığını anlamak için — M1 kapsamı ve haftalık görevler |
| `docs/kararlar/` | **Bir tasarım sorusu aklına geldiğinde ilk buraya bak.** 71 karar, hepsi gerekçeli |
| `docs/tasarim/` | O alanda iş yapmadan önce ilgili dosyayı oku (00–07) |
| `docs/kurallar.md` | Dal düzeni, klasör sahipliği, PR akışı |
| `docs/araclar.md` | Unity / Rider / Cursor / Claude — kim ne zaman |
| `docs/ajan-kurallari.md` | AI araçlarının uyacağı kurallar |

**Karar numaraları tüm dosyalar boyunca tektir.** Her dosya 1'den başlamaz.
Yeni karar yazmadan önce sıradaki numarayı **hesapla**:

```bash
grep -h '^## [0-9]' docs/kararlar/*.md | sed 's/\..*//' | tr -d '# ' | sort -n | tail -1
```

*(31.08.2026'da iki kez çakıştı — iki oturum aynı numarayı kullandı.)*

**Bir tasarım kararı zaten verilmiş olabilir.** `docs/kararlar/` içinde ara; yoksa
kullanıcıya sor, kendi başına karar verme. Verilen karar oraya yazılır.

Tasarım tartışması ayrı bir sohbet penceresinde yürüyor. Buradaki iş **uygulama**;
tasarım sorusu çıkarsa cevabı `docs/` altında aranır, orada yoksa sorulur.

## Dallar

`feature/<konu>` dalları **`dev`'den** açılır, **`dev`'e** döner.
`main`'den dallanma. `main`'e sadece kilometre taşında, `dev` üzerinden girilir.

Aynı anda en fazla 2 açık PR. Kendi PR'ını onaylama.

## İnceleme halkası — SANA ATANMAMIŞ PR'I İNCELEME

| Kimin işi | Kim inceler ve merge eder |
|-----------|---------------------------|
| Çağatay (A) | Özgür (B) |
| Özgür (B) | Onat (C) |
| Onat (C) | Çağatay (A) |

**`git pull` sonrası gelen değişiklikleri kendiliğinden incelemeye kalkma.**
Üç Claude aynı şeyi kontrol edince üç kat vakit gidiyor. Sadece kullanıcının
inceleyicisi olduğu PR'a bak, o da istendiğinde.

İncelerken `docs/kurallar.md` > "Onay ne demek" başlığındaki **altı maddelik
kapalı listeye** bak. Tasarımı baştan sorgulama; karar zaten `docs/kararlar/`
içinde.

## Klasör sahipliği

```
Code/Field                                  A
Code/Core, Economy, Auction, UI, Utils      B
Art, Prefabs, Data, Audio                   C
```

Başka rolün klasörüne yazma. Gerekiyorsa önce sor.
`Code/Core` altındaki mevcut imzaları değiştirme — ekleme serbest, değiştirme
başkasının kodunu kırar.

## Dil ve biçim

Kod Türkçe: `OyuncuKontrolcu`, `yurumeHizi`. Unity tipleri İngilizce: `Transform`.
Yorumlarda ASCII kullan (Türkçe karakter yok): `esya`, `deger`, `sure`.
Yorum "neden" anlatır, "ne" değil.

## Yasaklar

- `UnityEngine.Random` → `Code/Utils/Rastgele` (tohumlu)
- Singleton / `Instance` / `FindObjectOfType`
- `Scenes/*.unity` düzenleme
- `.meta` elle düzenleme

## Denge sayıları ölçülür

Faiz, kapasite, fiyat, süre, olasılık uydurulmaz. Ya ölç, ya da sağlaması
gereken kısıtı yaz.

Bilinen örnek: günlük %2 faiz makul görünüyordu ama asgari ödemeyi geçtiği için
mükemmel oynayan oyuncunun bile borcunu düşürmesini imkânsız kılıyordu. 42 günlük
simülasyonla çıktı.

## Unity'yi arayüzsüz çalıştırma

```bash
Unity.exe -batchmode -quit -nographics -projectPath <proje> \
          -executeMethod <Sinif.Metot> -logFile <yol>
```

- **Unity KAPALI olmalı**, yoksa kilit çakışır. Önce `Temp/UnityLockfile` kontrol et.
- Log yolunda 8.3 kısa yol (`ZGR~1`) kullanma — Unity yazamıyor, iş çalışmadı
  sanılıyor. Tam yol ya da `$env:TEMP` kullan.
- Batch iş öncesi kullanıcıya haber ver, Unity'yi kapatmasını iste.
- **Batch mod her geçişi tetiklemiyor.** Sürüm yükseltmesinde bazı asset-seviyesi
  geçişler (render pipeline varlıkları, `manifest.json`) yalnızca proje **gerçek
  arayüzde** açılınca oluyor. "Batch temiz" tek başına yeterli değil; bir insan
  Unity'yi açıp `git status`'a bakmalı. *(31.08.2026'da öğrenildi)*

## `git add -A` kullanma

Repo kökünde iç içe Unity projesi oluşabiliyor — Unity Hub yanlışlıkla repo
içinde proje kurarsa. Kökteki `/Library/` kuralı alt klasörleri kapsamıyor,
`git add -A` binlerce dosyayı sahneliyor.

**Değiştirdiğin dosyaları adıyla ekle.** *(31.08.2026'da yaşandı)*

## Yapamadıkların

Sahne düzenleme, Canvas kurma, Inspector'da referans bağlama, görünüşe bakma.
Görsel olan her şey insanda. Bunları tarif et, yapmaya çalışma.

**Batch mod bir insanın Unity'yi açmasının yerini tutmuyor** — yukarıya bak.

## Test

Sistem kodu yazıldığında EditMode testi de yazılır. Test yoksa kod bitmemiştir.
Testleri batch modda koştur, sonucu PR'a yaz.
