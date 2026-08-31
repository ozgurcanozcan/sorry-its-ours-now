# Araçlar — kim, ne zaman, neden

> Üç insan ve üç yazılım aracı aynı repoda çalışıyor. Bu belge kimin ne zaman
> hangi aracı kullandığını söyler. Amaç araç saymak değil, **çakışmayı önlemek**.

## Sürümler

Üçümüz de **birebir aynı** sürümü kullanıyoruz. Farklı sürüm = bozuk proje
dosyası = kayıp gün.

| Araç | Sürüm | Not |
|------|-------|-----|
| **Unity** | `6000.5.10f1` | Unity Hub → Installs → Add. Değişti: 31.08.2026, `docs/kararlar/` |
| Render Pipeline | URP | Proje oluştururken "Universal 3D" şablonu |
| **Rider** | Güncel | Ticari olmayan kullanımda ücretsiz |
| **Cursor** | Güncel | |
| **Blender** | Güncel LTS | Üçümüz aynı sürüm |

Unity Hub'da kurarken işaretlenecek modüller:
- `Windows Build Support (IL2CPP)` — Steam çıkışı için
- `Documentation` — çevrimdışı dokümantasyon

## Rol dağılımı — kişiye göre değil, işe göre

| İş | Araç |
|----|------|
| Repo geneli, çok dosyalı değişiklik, git/PR, headless Unity, test koşturma, inceleme | **Claude Code** |
| Hata ayıklama, kod içinde gezinme, Unity hatalarını okuma, refactor | **Rider** |
| Tek dosya yazarken AI yardımı, "bu ne yapıyor" diye sorma, öğrenme | **Cursor** |

Bir aracın diğerinin işini yapmaya çalışması sorun çıkarıyor. Bölüşüm bu.

---

## Rider

**Unity'nin harici editörü Rider olacak.**

Kurulum: `Unity > Edit > Preferences > External Tools > External Script Editor`
→ Rider. Sonra `Regenerate project files`.

Bundan sonra Unity konsolundaki bir hataya tıklayınca Rider doğru satırı açar.

### Neden Rider

Unity için en iyi entegrasyona sahip IDE. En önemli özelliği **hata ayıklayıcı**:

`Run > Attach to Unity Process` → kırılma noktası koy → Unity'de Play'e bas →
kod o satırda durur, değişkenlerin içine bakarsın.

Saha kodunda (hareket, çarpışma, taşıma) bu neredeyse zorunlu. `Debug.Log` ile
fizik hatası kovalamak saat yakar.

Ayrıca Unity'ye özel denetimleri var — `Update()` içinde `GetComponent`
çağırdığında uyarır, `string` karşılaştırmasında etiket kullanmanı önerir.

### Lisans

Ticari olmayan kullanımda ücretsiz. Oyun gelir getirmeye başlayınca lisans
gerekir (~150 USD/yıl). Çıkışa kadar dert değil.

---

## Cursor

**Repoyu klasör olarak açın, Unity üzerinden değil.** Unity'nin harici editörü
yapmayın — Rider ile aynı rolü üstlenirse proje dosyaları çakışır.

### Ne için

- Bir betiğe bakarken "bu satır ne yapıyor" diye sormak
- Tek dosyada hızlı düzeltme, insan başındayken
- Satır içi tamamlama — C# öğrenirken sözdizimi hatırlamakla vakit kaybetmemek

### Ne için DEĞİL

**Ajan / composer kipini çok dosyalı otonom iş için kullanmayın.**

Sebep: Claude Code repo genelinde çalışıyor. İkisi aynı anda çok dosyaya
yazarsa "bunu kim değiştirdi, neden" sorusunun cevabı kalmaz. Git geçmişi
karışır, çakışma çözmek imkânsızlaşır.

Sohbet ve tek dosya düzenleme kipinde kalırsa sorun yok.

### Kurallar dosyası

Cursor proje kurallarını `.cursorrules` (ya da sürüme göre `.cursor/rules/`)
dosyasından okur. Repoda var, **commit edilir** — üçümüz aynı kurallarla
çalışsın diye.

---

## Claude Code

Repo geneli iş: mimari, çok dosyalı özellikler, git ve PR akışı, doküman,
inceleme, simülasyon.

### Yapabildiği: Unity'yi arayüzsüz çalıştırmak

```bash
Unity.exe -batchmode -quit -nographics -projectPath <proje> \
          -executeMethod <Sinif.Metot> -logFile <yol>
```

Bununla gerçek derleme kontrolü yapabilir, EditMode testlerini koşturabilir,
menü komutlarını çalıştırabilir, veri varlığı üretebilir.

**Ama Unity KAPALI olmalı.** Unity projeyi kilitler; açıkken batch iş düşer.

> **Kural:** Claude batch iş çalıştıracaksa Discord'a yazar, Unity kapatılır,
> iş bitince haber verir.
>
> Rider ve Cursor açık kalabilir, onlar kilitlemiyor.

### Yapamadığı

Sahne düzenleyemez. Canvas kuramaz. Inspector'da referans bağlayamaz. Nesneyi
sürükleyemez. **Nasıl göründüğüne bakamaz.**

Yani: görsel olan her şey insanda. Arayüz yerleşimi, sahne kurulumu, ışık,
kamera açısı — bunlar tarif edilebilir ama Claude tarafından yapılamaz.

---

## Çakışma kuralları

1. **Aynı anda tek ajan.** Claude çalışırken Cursor'ın ajanı çalışmaz.
2. **Cursor'da yapılan değişiklik hemen commit edilir.** Diskte bekleyen yarım
   düzenleme, başkası dal değiştirdiğinde kaybolur.
3. **Batch iş öncesi Unity kapatılır**, Discord'a haber verilir.
4. **`.sln` ve `.csproj` commit edilmez.** Her araç kendi üretir; commit
   edilirlerse herkeste sürekli çakışır.

## Kim ne kullanıyor

**Çağatay (A)** — Rider birincil. Saha kodu hata ayıklayıcı ister.

**Özgür (B)** — Cursor birincil. Sıfırdan C# öğreniyor; satır içi yardım ve
soru sorabilmek öğrenme yüzeyi olarak daha uygun. Hata ayıklaması gerektiğinde
Rider'a geçer.

**Onat (C)** — ikisine de az ihtiyacı var, hayatı Blender'da geçecek. İçe
aktarma betiklerine bakması gerekirse Rider yeter.

### Batch modun sınırı — 31.08.2026'da öğrenildi

Unity'yi batch modda çalıştırmak **her geçişi tetiklemiyor.**

Sürüm 6.3 → 6.5 yükseltmesinde batch mod `packages-lock.json`'ı güncelledi ama
`manifest.json` ile dört asset dosyasına (Mobile/PC RPAsset, PackageManagerSettings,
URPProjectSettings) hiç dokunmadı. Proje gerçek arayüzde açılıp Package Manager'ın
bitmesi beklenince beşi birden güncellendi.

Sebep muhtemelen şu: bazı geçişler render pipeline'ın gerçekten yüklenmesini ve
Scene View'ın çizilmesini gerektiriyor; batch mod bunları yapmıyor.

> **Kural:** sürüm yükseltmesi hem **batch modda** hem **gerçek arayüzde** açılır,
> ikisinden sonra `git status` karşılaştırılır. "Batch temiz" tek başına yeterli
> değil — batch mod bir insanın Unity'yi açmasının yerini tutmuyor.

### İç içe Unity projesi tuzağı — aynı gün yaşandı

Unity Hub, repo klasörünün içinde yeni bir proje oluşturabiliyor. Kökteki
`.gitignore` kuralları (`/Library/`, `/Temp/`) **alt klasörleri kapsamıyor**,
bu yüzden `git add -A` iç projenin `Library`'sini de sahneliyor — binlerce dosya.

`.gitignore`'a `/*/Library/` gibi kurallar eklendi. Ama asıl korunma:
**`git add -A` yerine dosyaları adıyla ekleyin.**

### macOS'ta Homebrew yoksa: git-lfs ve gh CLI

Onat'ın makinesinde Homebrew kurulu değildi. İkisi de resmi kaynaktan,
`sudo` gerektirmeden kuruldu — aynı yol Çağatay/Özgür'de de işe yarar.

**git-lfs** — GitHub Desktop kurulu olan makinelerde zaten gömülü geliyor,
ayrıca indirmeye gerek yok:

```bash
mkdir -p ~/.local/bin
ln -sf "/Applications/GitHub Desktop.app/Contents/Resources/app/git/libexec/git-core/git-lfs" ~/.local/bin/git-lfs
export PATH="$HOME/.local/bin:$PATH"   # ~/.zshrc'ye kalıcı eklenir
git lfs install
```

**GitHub CLI (`gh`)** — resmi `cli/cli` reposundan (github.com/cli/cli/releases),
mimariye uygun `macOS_arm64.zip` ya da `macOS_amd64.zip`:

```bash
curl -sSL -o gh.zip "https://github.com/cli/cli/releases/latest/download/gh_<surum>_macOS_arm64.zip"
unzip gh.zip && cp gh_*/bin/gh ~/.local/bin/gh && chmod +x ~/.local/bin/gh
gh auth login --hostname github.com --git-protocol ssh --web
```

Son komut bir tek-seferlik kod ve `github.com/login/device` linki verir —
tarayıcıda onaylanınca `gh auth status` ile doğrulanır.

**Kalıcılık:** `~/.zshrc`'de `export PATH="$HOME/.local/bin:$PATH"` satırı
varsa (yoksa eklenir), yeni terminal oturumlarında da çalışır.
