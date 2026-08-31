# Sistem kodunun ilk kararları — 31.08.2026

> Hafta 1, Gün 1 (B · Özgür). İlk `.cs` dosyaları yazılırken çıkan kararlar.
> Onaylayan: Özgür.

---

## 1. Eşya verisi: kod kaynak, ScriptableObject türetilir

**Karar:** Eşya tanımlarının doğruluk kaynağı **kod içindeki katalog**.
Bir editör script'i ondan `.asset` dosyalarını üretir; Onat prefab'ları
o varlıklara bağlar.

**Neden:**

| | Kod tablosu | Sadece ScriptableObject |
|---|---|---|
| `git diff` | Okunur | YAML, okunmaz |
| EditMode testi | Doğrudan | Asset yüklemek gerekir |
| Denge ayarı (Hafta 5) | Derleme gerekir | Inspector'dan anında |
| Prefab bağlama | id ile çözülür | Doğrudan referans |
| Klasör sahipliği | `Code/Core` (B) | `Data/Items` (C) |

Prefab bağlama ve Inspector, C'nin (Onat) çalışma biçimi; `git diff` ve test
ise denge sayılarının ölçülebilir kalması için gerekli. Hibrit ikisini de
tutuyor.

**Bilinen bedeli:** İki temsil senkron tutulmak zorunda. Üretim adımı
atlanırsa sessizce ayrışırlar.

> **Bunu yakalayan bir test yazılacak:** üretilen `.asset`'ler katalogla
> uyuşmuyorsa EditMode testi kırılır. Sürüklenmenin sessiz kalmaması bu
> kararın ön koşulu.

**Alternatifler neden seçilmedi:** Salt kod → Onat Inspector'dan
düzenleyemez. Salt SO → her veri değişimi için birinin Unity açması gerekir,
ve dosyalar C'nin klasörüne düşer, oysa 20 eşyayı B dolduruyor
(`docs/plan.md` Hafta 1, gün 2).

---

## 2. `klasman` alanı şimdilik yok

**Karar:** Tamir zorluğu alanı `EsyaVerisi` şablonuna **konmadı**.

**Neden:** `05-icerik.md` bunu açık soru olarak işaretliyor — kaç kademe,
kategoriden mi türüyor, taban değerle mi ilişkili. Tamirci `docs/plan.md`'de
Hafta 4'te geliyor; şu an karar vermek için ne baskı var ne de veri.
`Code/Core`'a **ekleme serbest** olduğu için sonradan eklemek ucuz.

**Açık kalan:** Karar verilirken `05-icerik.md`'nin değer verdiği *"ucuz ama
zor"* birleşimi (kimsenin tamir ettirmeye değmeyeceği şey) korunmalı —
"pahalı olan zordur" kuralı onu imkânsız kılar.

---

## 3. Değer belirsizliği eşya alanı değil, sistem parametresi

**Karar:** `degerBelirsizligi` `EsyaVerisi` şablonunda yok. Değer biçme
sisteminin parametresi olarak duruyor.

**Neden:** `03-muzayede.md` **ölçülmüş** tek bir genel kural veriyor: ±%28.
(±%55 aralık kararı kumara çeviriyordu; ±%28'e inince karar geri geldi.)
Eşya başına ayrı yüzde yazmak, o tek ölçümü eşya sayısı kadar tahmine
bölerdi — "sayı uydurulmaz" kuralının tam karşısı.

`docs/uretim/esyalar.md` de bu genel kuralın kullanılmasını öneriyordu.

**Açık kalan:** Bir kategori (antika) gerçekten farklı belirsizlik
istiyorsa, bu eşya alanı değil **kategori tablosu** olarak eklenir.

---

## 4. Rastgelelik: PCG32, adla türetilen dallar

**Karar:** `Code/Utils/Rastgele` — PCG32 tabanlı, kendi yazdığımız üretici.
Alt akışlar `Dal(ad)` ile açılıyor ve sonuç yalnızca `(tohum, ad)` çiftine
bağlı, dalların açılma sırasına değil.

**Neden sıra bağımsızlığı:** `04-ekonomi.md` kampanya görevlerini "elle
ayarlanmış tohumlar" olarak tanımlıyor. Akışlar tek bir sıradan çekseydi,
sisteme yeni bir rastgelelik tüketicisi eklemek **eski bütün tohumların**
ürettiği dünyayı kaydırırdı. Adla türetince yeni dal eskileri etkilemiyor.

**Neden `UnityEngine.Random` değil:** Tek global durum taşıyor (ev üretimi
ile müzayede katılımı birbirini kaydırırdı) ve iç algoritması Unity
sürümleri arasında garantili değil.

**Neden yanlılık düzeltmesi var:** Alıcı havuzu 12 kişilik, ikinin kuvveti
değil. Klasik `% 12` küçük indeksleri sistematik olarak fazla üretirdi.
Lemire'in çarpma yöntemi kullanıldı; ki-kare testi eşiği 31.26 (11 serbestlik
derecesi, p=0.001), ölçülen 5.05.

**Altın değer testi:** İki test, sabit tohum için beklenen ham çıktıyı
kilitliyor. Algoritma değişirse kayıtlı tohumlar ve elle ayarlanmış kampanya
görevleri başka dünya üretmeye başlar; test bunu yakalar. **Bu test bilerek
kırılıyorsa buraya not düşülmeli.**

---

## 5. Tek derleme birimi: `Oyun`

**Karar:** `Assets/_Project/Code/` altındaki bütün kod tek bir asmdef'te
(`Oyun`). Ayrıca `Oyun.Editor` (yalnız editör) ve `Oyun.Testler.EditMode`.

**Neden:** EditMode testi yazabilmek için en az bir asmdef zorunlu —
asmdef'li bir test derlemesi `Assembly-CSharp`'a referans veremiyor.

**Neden rol başına ayrı asmdef değil:** `Code/Field` (A) `Code/Core`'a (B)
bakacak, UI ekonomiye bakacak; rol başına derleme birimi M1 boyunca
bağımlılık sırası tartışması üretir, karşılığında kazandırdığı derleme
süresi bu boyutta ölçülemez.

> **Bu karar A'yı ve C'yi de bağlıyor** (`Code/Field` de aynı derleme
> biriminin içinde). Klasör sahipliği değişmiyor, sadece derleme sınırı
> ortak. İtiraz varsa konuşulup değiştirilir.

`Oyun` asmdef'i `Unity.InputSystem`'e referans veriyor — A'nın birinci şahıs
kontrolü için gerekecek.

### Yan etkisi: "Editor" klasörü sihri asmdef içinde çalışmıyor

**Ölçüldü, tahmin değil.** `Oyun.asmdef` (#5) ile A'nın editör aracı (#4)
`dev`'de buluştuğunda `Code/Field/Editor/SahaGreyboxAraci.cs` **çalışma zamanı
derlemesi `Oyun.dll`'e** girdi:

```
SahaGreyboxAraci  -> Oyun.dll          (yanlis)
EsyaVarlikUretici -> Oyun.Editor.dll   (dogru, kendi asmdef'i var)

Oyun.dll icinde "UnityEditor" gecen referans: VAR
```

Unity, klasör adına dayalı "Editor" kuralını asmdef kapsamında uygulamıyor;
editör kodu için ayrı bir asmdef şart.

**Bunu EditMode testleri yakalamadı.** Editor'de `UnityEditor` her derlemeye
açık olduğu için 60/60 geçiyordu. Kırılma yalnızca **player build**'inde
çıkardı — yani ilk demo alınırken, en kötü anda. Bulgu testten değil,
`Library/ScriptAssemblies/` içindeki assembly üyeliğine bakılarak çıktı.

**İki kişi bağımsız olarak aynı hatayı buldu.** Düzeltme #6 ile geldi
(`Code/Field/Editor/Oyun.Field.Editor.asmdef`, yalnız Editor platformu).
Doğrulandı: araç `Oyun.Field.Editor.dll`'e taşındı, `Oyun.dll` temizlendi,
60/60 korundu.

> **Kural olarak:** `Code/` altında `UnityEditor` kullanan her klasörün kendi
> Editor asmdef'i olmalı. Yeni bir rol editör aracı yazarsa aynısı gerekecek.
>
> **Ve daha genel olarak:** yeşil EditMode testi player build'inin derlendiğini
> göstermiyor. İkisi ayrı sorular.

---

## 6. Bulgu: Unity batch, test başarısızlığında da 0 döndürüyor

**Karar değil, ölçüm.** 31.08.2026'da sürüklenme testi bilerek kırıldı
(bir `.asset` elle bozuldu). Sonuç:

```
testler: 60 toplam, 59 geçti, 1 kaldı
Unity çıkış kodu: 0
```

`docs/ajan-kurallari.md` şu otomasyonu planlıyor:

> PR açıldı → Unity batch: derle + EditMode testleri koştur → sonucu PR'a yaz

**Bu otomasyon çıkış koduna bakarsa kırık testleri yeşil geçirir.**
`-testResults` XML'indeki `failed="..."` sayısı ayrıştırılmalı.

Aynı ölçüm sürüklenme testinin de çalıştığını gösterdi: `.asset` katalogdan
ayrıştığı anda `UretilmisVarliklarKataloglaAyni` kırılıyor. Hibrit veri
kararının ön koşulu karşılandı.

---

## Bu kararların dokunduğu yerler

```
Assets/_Project/Code/Utils/Rastgele.cs
Assets/_Project/Code/Core/EsyaVerisi.cs
Assets/_Project/Code/Core/Kategori.cs
Assets/_Project/Code/Core/Kondisyon.cs
Assets/_Project/Code/Oyun.asmdef
Assets/_Project/Code/Editor/Oyun.Editor.asmdef
Assets/_Project/Tests/EditMode/
```
