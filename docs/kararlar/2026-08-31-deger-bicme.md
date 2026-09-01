# Değer biçme — 31.08.2026

> Hafta 1, Gün 3 (B · Özgür). `docs/plan.md`: *"Değer biçme: gizli değer +
> tahmin aralığı."* Onaylayan: Özgür.

---

## 72. Tahmin aralığı gerçeğin etrafında ortalanmaz

**Karar:** Gerçek değer, gösterilen aralığın **içinde rastgele bir yere** düşer.
Aralık gerçeğe göre ortalanmaz.

**Neden:** Ortalansaydı orta nokta gerçek değerin ta kendisi olurdu. Oyuncu her
seferinde ortayı okuyup doğru cevabı alır, belirsizlik diye bir şey kalmazdı —
oysa `docs/tasarim/02-saha.md` "gerçek değer gizli, oyuncu bir aralık görür"
diyor.

Uygulama: gerçek değerin aralık içindeki göreli yeri `konum ∈ [-1, 1]` olarak
çekiliyor, merkez ondan geri çözülüyor.

---

## 73. Ama aralık gerçek değeri içermek **zorunda**

**Karar:** Gösterilen aralık her zaman gerçek değeri kapsar. Bu kural koda
gömüldü: `EsyaOrnegi` yapıcısı içermeyen bir aralığı reddediyor.

**Neden:** İçermeseydi tahmin yalan söylerdi. Oyuncu birkaç kez yanıldıktan
sonra aralığa hiç bakmamayı öğrenir ve mekanik ölür.

> İkisi birlikte tasarımın özü: **merkez bilgi vermemeli, sınırlar doğru
> olmalı.** Biri olmadan öteki anlamsız.

Yalnızca teste bırakılmadı çünkü bu bir değişmez (invariant), bir beklenti
değil. Sağlanamıyorsa program yanlış durumda demektir.

---

## 74. Tahmin konumu örneğe sabit

**Karar:** `konum` bir eşya örneği üretilirken bir kez çekilir ve **kondisyon
değişse bile korunur.**

**Neden — iki ayrı istismar:**

1. **Her sorguda yeniden çekilseydi**, oyuncu aynı eşyaya defalarca bakıp
   gördüğü aralıkların ortalamasını alarak gerçek değeri bulurdu. Belirsizlik
   ancak tek bir çekilişte kalırsa belirsizliktir.
2. **Kondisyon değişince yeniden çekilseydi**, eşyayı kırıp tamir ettirmek
   gerçek değer üzerinde ikinci bir ölçüm yapmak anlamına gelirdi.

---

## 75. Kondisyon çarpanı: üç sayı değil, tek oran

**Karar:** Her kondisyon kademesi bir üstünün **%70'i**
(`iyi 1.00 · eski 0.70 · hasarlı 0.49`). Taban değerin çarpanı 1 —
`docs/uretim/esyalar.md`'deki piyasa fiyatı zaten çalışır durumdaki eşyanın.

**ÖLÇÜLMEDİ.** Hafta 5 denge işi. Üç ayrı çarpan yerine tek oran yazıldı:
ölçülecek sayı üçten bire indi.

**Sağlaması gereken kısıt** (kodda, `DegerAyarlari` yapıcısı reddediyor):

```
1 − KademeCarpani  >  YariGenislik
0.30               >  0.28   ✓
```

**Neden bu kısıt:** `docs/tasarim/02-saha.md` "kondisyon görünür, değer
görünmez" ayrımını **oyuncunun elindeki tek bilgi** olarak kuruyor. Bir kademe
düşmenin değere etkisi tahmin belirsizliğinin altında kalırsa, görünür olan
sinyal gizli gürültünün altında kaybolur ve o ayrım anlamını yitirir.

Şu anki değerler kısıtı **0.02 payla** sağlıyor. Hafta 5'te oran gevşetilecekse
yarı genişlik de birlikte düşünülmeli.

---

## 76. Yuvarlama adımı 10

**Karar:** Gösterilen sınırlar 10 TL'ye yuvarlanır; alt aşağı, üst yukarı.

**Neden yuvarlama:** `~1.125–2.025 TL` okunur, `~1.131–2.017 TL` okunmaz.
**Neden alt aşağı / üst yukarı:** yakına yuvarlamak sınırı gerçek değerin
yanlış tarafına geçirip **karar 73'ü** çiğneyebilirdi.

**Kısıt** (`DegerlemeTestleri.YuvarlamaEnUcuzEsyayiBozmuyor` koruyor):
yuvarlama, katalogdaki en dar aralığı **%10'dan fazla genişletmemeli.**
Yuvarlama aralığı genişletiyor, yani ölçülmüş ±%28'i fiilen büyütüyor.

| Adım | En kötü şişme | |
|------|---------------|---|
| 50 | ~%20 | ±%28 fiilen ±%34 gibi |
| 25 | %10.8 | eşiği aşıyor |
| 20 | %7.9 | geçiyor ama payı ince |
| **10** | **%4.5** | seçilen |

**Neden 20 değil:** 20 de eşiği geçiyor ama bir sonraki ucuz eşyada yine
kırılırdı. 10, en kötü durumu eşiğin yarısında tutuyor.

### Bu sayı bir kez zaten değişti — test yakaladı

İlk hâli **25**'ti ve payı incedir diye yazılmıştı:

> *"Kataloğa daha ucuz bir eşya girdiği anda test kırılacak ve adım
> küçültülmesi gerekecek. Bu kasıtlı: sessizce bozulmasın."*

**Aynı gün oldu.** Kol saati (1.500 TL) kataloğa girince en kötü şişme
%8.9'dan %10.8'e çıktı ve `YuvarlamaEnUcuzEsyayiBozmuyor` kırıldı. Adım
10'a indirildi.

> Kaydedilmeye değer olan şu: kısıt sayı olarak değil **test olarak**
> yazıldığı için, sayı geçersizleştiğinde kimse fark etmek zorunda kalmadı.
> Hafta 5'te ölçülecek öteki sayılar için de aynı biçim kullanılmalı.

---

## 77. `EsyaOrnegi` değişmez

**Karar:** Dünyadaki tek bir eşya değişmez bir nesne. Kondisyon değişince
(kırılma, tamir) yenisi üretilir.

**Neden:** `.cursorrules` mimarisi *"doğruluk tek yerde: `OyunDurumu`,
MonoBehaviour'lar sadece gösterir, her eylem bir komut"* diyor. Değiştirilebilir
nesne o modelde kaçak nokta oluyor — durum `OyunDurumu` dışında da değişebilir
hâle geliyor.

---

## Bu kararların dokunduğu yerler

```
Assets/_Project/Code/Core/DegerAraligi.cs
Assets/_Project/Code/Core/EsyaOrnegi.cs
Assets/_Project/Code/Economy/DegerAyarlari.cs
Assets/_Project/Code/Economy/Degerleme.cs
Assets/_Project/Tests/EditMode/DegerlemeTestleri.cs
```

## Bilerek yapılmayan

**Örnek başına ek değer sapması yok.** Gerçek değer yalnızca
`taban değer × kondisyon`. `docs/tasarim/03-muzayede.md` açıkça diyor:
*"belirsizliğin asıl kaynağı fiyat tahmini değil, salonda kimin olduğu."*
Buraya bir sapma daha koymak, ölçülmemiş bir sayı daha uydurmak olurdu.
