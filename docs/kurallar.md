# Çalışma Kuralları

> Bu belge üçümüzün ortak sözü. Bir kural işe yaramıyorsa değiştirilir —
> ama sessizce çiğnenmez, konuşulup değiştirilir.

## Ekip ve sahiplik

| Rol | Kişi | GitHub | Alan | Sahip olduğu klasörler |
|-----|------|--------|------|------------------------|
| A | Çağatay | `cagataydmn` | Saha | `Code/Field` |
| B | Özgür | `ozgurcanozcan` | Sistem/UI | `Code/Core`, `Code/Economy`, `Code/Auction`, `Code/UI`, `Code/Utils` |
| C | Onat | `onat2001` | Teknik sanat | `Art`, `Prefabs`, `Data`, `Audio` |

**Kendi klasörünün dışına yazma.** Yazman gerekiyorsa önce Discord'a düş.
Bu kural üretkenliği değil, *okunabilirliği* korur: bir dosya bozulduğunda
kimin baktığı belli olsun.

## Dallar

```
main   ← yalnızca denenmiş, çalışan sürüm. Buraya sadece dev üzerinden girilir.
dev    ← ortak entegrasyon dalı. Günlük iş burada birleşir.
feature/<konu>   ← dev'den açılır, dev'e döner
agent/<gorev-no> ← AI aracının kendi dalı (bkz. ajan-kurallari.md)
```

**Feature dalları `dev`'den açılır.** `main`'den dallanmayın. Bu kural bir kez
çiğnendi ve iki gün sonra `main` ile `dev` ayrıştı; toplamak yarım gün aldı.

`main`'e geçiş kilometre taşı tamamlandığında olur, tek seferde, `dev`'den.

## Pull request

- **Hedef her zaman `dev`.**
- **En az bir kişi onaylar.** Kendi PR'ını onaylayamazsın.
- **Aynı anda en fazla 2 açık PR.** Üçüncüyü açmadan önce biri kapanmalı.
- **PR küçük olsun.** "Haftalık çalışma" değil, "eşya taşıma eklendi".

Son madde deneyimden geliyor: bir günde dört PR açıldı, hiçbiri okunmadı.
Üretim hızı inceleme hızını geçtiğinde kod birikir, kimse okumaz ve kalite
sessizce düşer. Üretimi kısmak değil, **incelemeyi öne almak** gerekiyor.

## Sahneler

**Sahne başına tek kişi, tek gün.** Aynı gün aynı `.unity` dosyasını iki kişi
açmaz. Discord'da "Field sahnesi bugün bende" denir.

Unity sahne dosyaları metin ama insan gözüyle birleştirilemez. Çakışırsa
çözmek yerine biri baştan yapar — o yüzden çakıştırmayın.

**İşini sahneye değil prefab'a yap.** Sahnede duran her şey çakışma adayı.

## Paylaşılan imzalar

`Code/Core` altındaki sınıflar üçümüzün ortak sözleşmesi. Bir metodun imzasını
değiştirirsen başkasının kodu derlenmez.

**Kural: imza değişikliği önce konuşulur.** Ekleme serbest, değiştirme değil.

## Commit ve push

- **Her gün push.** Akşam commit edilmemiş iş yoktur.
- **Her gün çalışan bir hâl bırak.** Bozuk bir şeyin üstüne ertesi gün yeni
  özellik konmaz; önce çalıştır.
- Commit mesajı Türkçe, ne yaptığını söyler: `Esya alma etkilesimi eklendi`.

İkinci madde AI ile çalışırken kritik: kodu okuyarak hata avlamak zorsa, tek
çaren son çalışan hâle dönüp aradaki **tek** günü sorgulamak. İki hafta
biriktirirsen o imkânı kaybedersin.

## Unity dosyaları

- **`.meta` dosyaları commit edilir.** Edilmezse herkeste farklı GUID oluşur
  ve Inspector'daki referanslar sessizce kopar.
- `.sln`, `.csproj`, `Library/`, `Temp/` commit edilmez — `.gitignore`'da.
- Serileştirme **Force Text**, sürüm kontrolü **Visible Meta Files**.
  Proje ilk kurulduğunda ayarlanır, bir daha dokunulmaz.

## Kararlar

Alınan her kalıcı karar `docs/kararlar/` altına tarihli bir dosya olarak yazılır:
ne karar verildi, neden, kim onayladı.

Sebep: aynı şeyi iki kez tartışmamak. Karar yazılmazsa üç hafta sonra "biz ne
demiştik" diye baştan başlanır.

## Bir kuralı değiştirmek

Bu belgeyi değiştiren PR açılır, üçü de onaylar. Değişiklik `docs/kararlar/`
altına da not düşülür.
