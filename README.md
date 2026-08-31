# Sorry, It's Ours Now

> Birinci şahıs, stilize low-poly, kara komedi. Borçlunun evine dalıp
> taşıyabildiğin her şeyi kamyona tıkıştırdığın, akşam da o çöpü müzayedede
> sanat eseri diye pazarladığın, tamamen yasal bir yağma simülasyonu.

**KOZ GAMES** · Unity 6 · URP · Steam'de ücretli çıkış hedefi

---

## Ekip

| Rol | Kişi | GitHub | Alan |
|-----|------|--------|------|
| A | Çağatay | [@cagataydmn](https://github.com/cagataydmn) | Saha: hareket, etkileşim, taşıma, fizik, engeller |
| B | Özgür | [@ozgurcanozcan](https://github.com/ozgurcanozcan) | Sistem/UI: müzayede, ekonomi, envanter, kayıt |
| C | Onat | [@onat2001](https://github.com/onat2001) | Teknik sanat: 3D, oda kitleri, palet, ışık, ses |

## İlk kurulum

**1. Unity Hub'dan `6000.3.23f1` (Unity 6.3 LTS) kur.** Modüller:
`Windows Build Support (IL2CPP)` ve `Documentation`.

**2. Git LFS ve repo:**

```bash
git lfs install
git clone https://github.com/ozgurcanozcan/sorry-its-ours-now.git
```

**3. Rider'ı harici editör yap:** `Unity > Edit > Preferences > External Tools`
→ `External Script Editor` = Rider → `Regenerate project files`

**4. Cursor'ı ayrı aç** — repoyu klasör olarak. Unity'nin harici editörü yapma.

Ayrıntılar: [`docs/araclar.md`](docs/araclar.md)

## Belgeler

| Belge | İçerik |
|-------|--------|
| [`docs/kurallar.md`](docs/kurallar.md) | Dal düzeni, klasör sahipliği, PR akışı, sahne kuralı |
| [`docs/araclar.md`](docs/araclar.md) | Unity, Rider, Cursor, Claude — kim ne zaman |
| [`docs/ajan-kurallari.md`](docs/ajan-kurallari.md) | AI araçlarının uyacağı kurallar |
| [`docs/plan.md`](docs/plan.md) | Kilometre taşları, M1 kapsamı, haftalık görev dağılımı |
| [`docs/tasarim/`](docs/tasarim/) | Oyun tasarımı — 7 belge |
| `docs/kararlar/` | Tarihli karar kaydı — ne, neden, kim |
| `docs/gorevler/` | Ajanlara verilen şartnameler |

## Kuralların özeti

Uzun hâli `docs/kurallar.md`'de. Ezberlenecek beş madde:

1. **Feature dalları `dev`'den açılır, `dev`'e döner.** `main`'den dallanma.
2. **Aynı anda en fazla 2 açık PR.** Üretim hızı inceleme hızını geçmesin.
3. **Sahne başına tek kişi, tek gün.** İşini sahneye değil prefab'a yap.
4. **Kendi klasörünün dışına yazma.** Paylaşılan imzayı konuşmadan değiştirme.
5. **Her gün push, her gün çalışan bir hâl bırak.**

## Klasör yapısı

```
Assets/
  _Project/
    Art/        Models/ Materials/ Textures/
    Audio/      SFX/ Music/ Ambience/
    Code/       Core/ Field/ Auction/ Economy/ Generation/ UI/ Utils/ Editor/
    Data/       Items/ Buyers/ Residents/ Jobs/
    Prefabs/    Furniture/ RoomKits/ Systems/
    Scenes/     Boot MainMenu Office Field Auction
    UI/
  ThirdParty/   <- dışarıdan gelen her şey ayrı
docs/
```

## Durum — 31 Ağustos 2026

Tasarım ve plan yazıldı. **52 karar** alındı, hepsi gerekçesiyle
[`docs/kararlar/`](docs/kararlar/) altında kayıtlı.

Sırada **Unity projesinin kurulması** ve M1'in 1. haftası var
([`docs/plan.md`](docs/plan.md)).

İlk işler:
- **A** — Field sahnesi ve birinci şahıs kontrol
- **B** — tohumlu rastgelelik ve eşya veri şablonu
- **C** — Blender→Unity akışını kanıtlamak (ölçek, pivot, ihraç)
