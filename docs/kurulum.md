# Kurulum — sıfırdan çalışır hâle

> Üçünüz de bu listeyi kendi makinesinde baştan sona uygulayacak.
> Her maddenin sonunda **"nasıl anlarım"** var — kurmakla çalıştığını görmek
> ayrı şeyler.

---

# 1. Herkes — ortak kurulum

## 1.1 Git ve Git LFS

İndir: [git-scm.com](https://git-scm.com/download/win)

Kurulumda **"Git Credential Manager"** seçili kalsın (varsayılan).

Sonra:

```bash
git lfs install
```

**Nasıl anlarım:** `git --version` ve `git lfs version` sürüm yazdırıyorsa tamam.

## 1.2 Git kimliği ve ayarları

```bash
git config --global user.name "Ad Soyad"
git config --global user.email "github-epostan@ornek.com"
git config --global credential.helper manager
git config --global core.autocrlf false
```

**Neden `credential.helper manager`:** eski `wincred` kullanıcı adı + parola
saklıyor, GitHub ise 2021'de parolayla girişi kapattı. `manager` tarayıcıdan
giriş yaptırıp jetonu güvenli saklıyor. Bu ayar olmadan **push çalışmaz.**

**Neden `core.autocrlf false`:** repodaki `.gitattributes` satır sonlarını zaten
yönetiyor. `autocrlf` açık kalırsa her commit'te "LF will be replaced by CRLF"
uyarıları gelir ve dosyalar gereksiz yere değişmiş görünür.

**Nasıl anlarım:** `git config --global --list` üç satırı da gösteriyorsa tamam.

## 1.3 GitHub erişimi

- [ ] GitHub hesabın var
- [ ] Repo davetini kabul ettin ([sorry-its-ours-now](https://github.com/ozgurcanozcan/sorry-its-ours-now))

**Nasıl anlarım:** repo sayfası açılıyorsa tamam. 404 alıyorsan davet bekliyor.

## 1.4 GitHub CLI *(zorunlu değil ama işe yarar)*

```bash
winget install --id GitHub.cli -e
```

Kurduktan sonra **yeni bir terminal aç** (PATH eski pencerelere gelmez), sonra:

```bash
gh auth login
```

Sırasıyla: **GitHub.com** → **HTTPS** → *"Authenticate Git with your GitHub
credentials?"* → **Yes** → **Login with a web browser**

Son adımdaki "Yes" önemli: git'in kimlik derdini de aynı anda çözüyor.

**Nasıl anlarım:** `gh auth status` → "Logged in to github.com account ..."

## 1.5 Repoyu klonla

```bash
git clone https://github.com/ozgurcanozcan/sorry-its-ours-now.git
```

> **Yol seçimine dikkat:** klasör yolunda **Türkçe karakter ve boşluk olmasın.**
> `C:\Dev\sorry-its-ours-now` gibi bir yer iyi.
>
> Sebep: bazı araçlar Windows'un 8.3 kısa yol biçimine düşüyor (`ZGR~1` gibi) ve
> orada tökezliyor. Deneme sürümünde Unity'nin batch kaydı tam bu yüzden
> yazılamadı, "hiç çalışmadı" sanıldı.

**Nasıl anlarım:** klasörde `docs/`, `Assets/`, `CLAUDE.md` görünüyorsa tamam.

## 1.6 Unity Hub + Unity 6000.3.23f1

1. [unity.com/download](https://unity.com/download) → Unity Hub
2. Hub'da Unity hesabıyla giriş yap, **Personal lisansı etkinleştir**
3. **Installs → Install Editor → Archive** → sürüm: **`6000.3.23f1`**

**Kurulumda işaretlenecek modüller:**

- [ ] `Windows Build Support (IL2CPP)` — Steam çıkışı için zorunlu
- [ ] `Documentation` — çevrimdışı dokümantasyon, öğrenirken çok işe yarar

> **Üçünüz de birebir aynı sürüm.** Farklı sürüm = bozuk proje dosyası =
> kayıp gün. Sürüm değişikliği üçü konuşmadan yapılmaz.

**Nasıl anlarım:** Hub → Installs'ta `6000.3.23f1` görünüyor ve yanında modüller
listeleniyor.

## 1.7 Unity'de projeyi aç

Hub → **Add** → klonladığın klasörü seç → aç.

İlk açılış `Library/` klasörünü sıfırdan üretir, **5–15 dakika sürebilir.**
Sonraki açılışlar hızlı.

**Nasıl anlarım:** Console'da kırmızı hata yok, Project panelinde
`Assets/_Project` klasörleri görünüyor.

## 1.8 Unity ayarı: harici editör

`Edit → Preferences → External Tools → External Script Editor` → **Rider**
Ardından **Regenerate project files**.

**Nasıl anlarım:** Console'daki bir hataya çift tıklayınca Rider doğru satırı açıyor.

## 1.9 UnityYAMLMerge *(sahne çakışması için)*

Sahne ve prefab dosyaları çakıştığında Unity'nin kendi birleştiricisi devreye
girsin diye:

```bash
git config merge.unityyamlmerge.name "Unity SmartMerge"
git config merge.unityyamlmerge.driver 'UnityYAMLMerge merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"'
```

> Bu bir sigorta. Asıl kural hâlâ geçerli: **sahne başına tek kişi, tek gün.**

## 1.10 Claude Code

Repo klasöründen başlat — `CLAUDE.md` ve proje kuralları böyle okunur.

**Nasıl anlarım:** Claude'a "hangi daldayız" diye sor; `dev` demesi lazım.

## 1.11 Discord

Günlük koordinasyon burada. Şu üç şey mutlaka yazılır:

- "Field sahnesi bugün bende"
- "Claude batch iş çalıştıracak, Unity'yi kapatın"
- "Şu imzayı değiştirmem gerekiyor, sorun var mı?"

---

# 2. A — Çağatay (saha)

## 2.1 Rider

[jetbrains.com/rider](https://www.jetbrains.com/rider/) — ticari olmayan kullanım
için ücretsiz. Kayıt gerektiriyor, oyun gelir getirene kadar bedava.

Unity eklentisi gömülü geliyor, ayrıca kurmana gerek yok.

## 2.2 Rider hata ayıklayıcı

`Run → Attach to Unity Process` → çalışan Unity'yi seç.

Sonra bir betiğe kırılma noktası koy, Unity'de **Play**'e bas — kod o satırda durur.

**Nasıl anlarım:** kırılma noktasında durup değişkenlerin içine bakabiliyorsan tamam.

> Saha kodunda buna ihtiyacın olacak. Hareket, çarpışma ve taşıma hatalarını
> `Debug.Log` ile kovalamak saat yakar.

---

# 3. B — Özgür (sistem/UI)

## 3.1 Cursor

[cursor.com](https://cursor.com) — repoyu **klasör olarak** aç, Unity üzerinden değil.

**Unity'nin harici editörü yapma.** Rider ile aynı rolü üstlenirse proje
dosyaları çakışır.

`.cursorrules` repoda hazır, açar açmaz proje kurallarını okuyacak.

**Nasıl anlarım:** Cursor'a "bu projede hangi dilde kod yazılıyor" diye sor;
"Türkçe" demesi lazım.

## 3.2 Cursor ayarı: ajan kipi

Composer / agent kipini **çok dosyalı otonom iş için kullanma.** Sohbet ve tek
dosya düzenleme serbest.

Sebep: repo genelinde Claude Code çalışıyor. İkisi aynı anda çok dosyaya yazarsa
"bunu kim değiştirdi" sorusunun cevabı kalmaz.

## 3.3 Rider *(hata ayıklama için)*

Sen de kur. Sistem kodunda mantık hatası aradığında Cursor değil Rider lazım
olacak — kırılma noktası koyup değerlerin nasıl değiştiğini görmek için.

---

# 4. C — Onat (sanat)

## 4.1 Blender

[blender.org](https://blender.org) — güncel **LTS** sürümü.

Üçünüz aynı sürüm demiştik ama bu esas olarak seni bağlıyor; `.blend` dosyaları
sürüm farkından etkileniyor.

## 4.2 Blender ayarı: birim sistemi

`Scene Properties → Units`:

- Unit System: **Metric**
- Unit Scale: **1.0**
- Length: **Meters**

**Nasıl anlarım:** varsayılan küpü ölçtüğünde 2 m çıkıyorsa tamam.

> **1 Unity birimi = 1 metre.** Yanlış ölçekte gelen model sonradan
> düzeltilirse bütün çarpışma kutuları ve fizik ayarları bozulur.

## 4.3 PureRef

[pureref.com](https://pureref.com) — referans panosu için, ücretsiz.

Palet buradan türetilecek (karar 47): Türk apartmanı, gecekondu içi, eski eşya,
sanayi deposu.

## 4.4 İhraç ayarları — Gün 1'in işi

FBX ihraç ayarlarını **sen kanıtlayacaksın** (`docs/plan.md`, Hafta 1 Gün 1):
bir küp, doğru ölçek, doğru pivot, Unity'de doğru boyut.

Çalışan ayarları bulunca `docs/` altına yaz — sonraki 60 model o ayarla çıkacak.

Başlangıç noktası: Blender FBX ihracında `Scale 1.0`, `Forward -Z`, `Up Y`.
Ama doğrulamadan kabul etme.

---

# 5. Kritik kurallar — kurulum sonrası

**Aynı anda tek ajan.** Claude Code çalışırken Cursor'ın ajan kipi çalışmaz.

**Claude batch iş çalıştıracaksa Unity kapatılır.** Unity projeyi kilitler,
açıkken batch iş düşer. Discord'a yazılır.

**Sahne başına tek kişi, tek gün.** İşini sahneye değil prefab'a yap.

**Her gün push, her gün çalışan bir hâl bırak.**

**Feature dalları `dev`'den açılır, `dev`'e döner.** `main`'den dallanma.

Tamamı: [`docs/kurallar.md`](kurallar.md) · [`docs/araclar.md`](araclar.md)

---

# 6. İlk on dakika — hepsi kurulduktan sonra

```bash
cd sorry-its-ours-now
git checkout dev
git pull
```

Unity Hub'dan projeyi aç, Console'un temiz olduğunu gör.

Sonra `docs/plan.md` → **M1, Hafta 1, Gün 1**:

- **A** — Field sahnesi ve birinci şahıs kontrol
- **B** — tohumlu rastgelelik ve eşya veri şablonu
- **C** — Blender→Unity akışını kanıtla

Takıldığın yerde Claude'a sor — repo klasöründen çalıştırdığın sürece proje
kurallarını zaten biliyor.
