# 31.08.2026 — Unity sürümü güncellendi

**Onaylayan:** üçü de, yüz yüze (Discord'a düşülmedi)
**Bağlam:** Onat'ın makinesine Unity Hub kurulunca varsayılan/önerilen sürüm
`6000.3.23f1` değil `6000.5.10f1` çıktı.

## Karar: `6000.5.10f1` (3bd4f66ad299)

Henüz kimse projede çalışmaya başlamadığı için sürüm değiştirmenin bedeli şu
an sıfıra yakın — daha pahalı olacağı bir an varsa o an içerik dolduktan
sonrasıdır, bugün değil.

`docs/araclar.md`'deki sürüm satırı ve `ProjectSettings/ProjectVersion.txt`
buna göre güncellendi.

**Değişmeyen kural:** `docs/araclar.md`'nin dediği gibi üçü de **birebir aynı**
sürümü kuracak. Kurarken işaretlenecek modüller aynı kalıyor:
`Windows Build Support (IL2CPP)`, `Documentation`.

> **Not:** bu belge Claude Code tarafından, Onat'ın aktardığı yüz yüze karara
> dayanarak yazıldı — ekibin `docs/kurallar.md`'de tarif ettiği "kural
> değişikliği PR'la, üçü onaylar" akışının Discord yerine yüz yüze işlediği
> bir örnek.

---

## Ek: araştırma sonucu ve doğrulama *(Claude, 31.08.2026)*

Kararın gerekçesi sonradan araştırıldı, çünkü deneme reposunun README'sinde
şu not vardı: *"Tech Stream sürümleri (6000.4/6000.5) ise LTS değil,
kullanmayın."* Bu not yeni repoya taşınmamıştı.

### Sonuç: karar doğru, eski not eskimiş

Unity, Unity 6 ile **Tech Stream modelini kaldırdı.** Artık iki tür sürüm var:

| Tür | Örnek | Destek |
|-----|-------|--------|
| **LTS** | 6.3 (`6000.3`) | Aralık 2027'ye kadar |
| **Supported release** | 6.5 (`6000.5`, 15.06.2026) | Bir sonraki sürüme kadar, **LTS ile aynı kalitede** |

Unity'nin kendi ifadesi: Update/Supported sürümler *"LTS ile aynı titiz kalite
ve kararlılık testinden geçiyor ve aynı düzeyde destek alıyor."* Eski Tech
Stream'ler erken test içindi; o model artık yok.

**Unity'nin doğrudan tavsiyesi** (RichardFine, Unity Discussions):

> Sevkiyat yapan (ya da yapmak üzere olan) projeler için LTS'i öneriyoruz, ama
> geliştirmenin erken aşamasındaki projeler için en güncel Unity sürümünü
> öneriyoruz.

Bu proje sıfırıncı gününde. **6.5 doğru seçim.**

Kaynaklar: [Unity 6 Releases & Support](https://unity.com/releases/unity-6/support) ·
[Unity 6.5 duyurusu](https://discussions.unity.com/t/unity-6-5-is-now-available/1723176)

### Doğrulama

Proje `6000.5.10f1` ile batch modda açıldı ve yükseltme geçişi yapıldı:

- Çıkış kodu **0**
- Derleme hatası **0**
- Paket çözümleme sorunu **yok**
- URP **17.3.0**'da kaldı, değişmedi

Yükseltmenin dokunduğu dosyalar: `packages-lock.json`,
`UniversalRenderPipelineGlobalSettings.asset`, `EditorBuildSettings.asset`.
Ayrıca iki yeni ayar dosyası geldi: `PhysicsCoreProjectSettings2D.asset`,
`ProjectAuditorSettings.asset`.

### Ek doğrulama: gerçek GUI açılışı, batch modun kaçırdığını yakaladı

Aynı gün, Onat'ın makinesinde proje **gerçekten Unity Editor arayüzünde**
açıldı (batch modda değil, tam açılıp Package Manager'ın bitmesi beklenerek).
Sonuç, yukarıdaki batch mod doğrulamasıyla kısmen çelişiyordu:

- **`packages-lock.json`'da (gerçekte çözümlenen sürüm) URP zaten `17.5.0`**
  — "17.3.0'da kaldı" tespiti yanlış değildi ama eksikti: lock dosyası
  görünmeden önce güncellenmiş, sadece **`manifest.json` (istenen sürüm
  metni) hâlâ `17.3.0` yazıyordu.**
- Batch mod açılışı **5 dosyayı hiç dokunmadan bırakmış**: `manifest.json`,
  `Mobile_RPAsset.asset`, `PC_RPAsset.asset`, `PackageManagerSettings.asset`,
  `URPProjectSettings.asset` (asset sürüm numaraları, materyal versiyonu,
  yansıma probu atlası gibi alanlar). Gerçek GUI açılışında bunlar da
  güncellendi.

**Sonuç:** `-batchmode -quit -nographics` ile yapılan bir sürüm doğrulaması
**yeterli değil** — bazı asset-seviyesi migrasyonlar, Editor'ün render
pipeline'ı gerçekten yükleyip Scene View'u çizmesini gerektiriyor gibi
duruyor, headless modda tetiklenmiyor. Bu 5 dosya da eklendi.

> **Kural (yeni):** Unity sürüm yükseltmesi doğrulanırken **hem batch modda
> hem gerçek Editor arayüzünde** açılıp iki sonucun `git status`'u
> karşılaştırılmalı. Sadece batch mod "temiz" (0 hata) demek, migrasyonun
> eksiksiz olduğu anlamına gelmiyor.

### Sürüm yükseltme kuralları — bundan sonrası için

Unity içinde ileri yükseltme mümkün, **geri dönüş değil**: Unity projeyi eski
sürümde açmayı desteklemiyor. Bu yüzden dört kural:

1. **Sürüm yükseltmesi kendi PR'ında olur.** İçinde başka hiçbir şey olmaz —
   bir şey kırılırsa sebebi belli olsun.
2. **Kilometre taşı sınırında yapılır**, hafta ortasında değil.
3. **Yükseltmeden önce commit etiketlenir.** Geri dönüş imkânı kalsın.
4. **M4'ten sonra sürüm dondurulur.** O noktada güncel olan **LTS**'e geçilir
   ve çıkışa kadar dokunulmaz. (Unity'nin tavsiyesinin ikinci yarısı bu.)

### En riskli nokta: shader

Sürüm yükseltmelerinde editörün kendisi değil **paketler** kırar, ve bu projede
en kırılgan şey C'nin yazacağı **stilize gölgeleme shader'ı** olacak.

> **Öneri (C):** shader'ı **Shader Graph** ile kur, elle HLSL yazma.
> Graph, SRP sürüm değişikliklerini kendisi soğuruyor; elle yazılmış shader
> her yükseltmede tek tek düzeltiliyor.

İkinci risk: üçüncü parti **kod** eklentileri. Şu an sıfır, öyle kalması iyi
olur. Model/doku paketleri (Kenney vb.) risk taşımıyor.
