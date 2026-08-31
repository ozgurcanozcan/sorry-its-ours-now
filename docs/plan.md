# Yapım Planı

> Bu plan `docs/tasarim/` altındaki 52 karara dayanıyor. Tasarım değişirse plan
> da değişir — ikisi birbirine bakar.

**Ekip:** A = Çağatay (saha) · B = Özgür (sistem/UI) · C = Onat (sanat)
**Tempo:** haftada 6 gün. **7. gün tatil.**

## Kilometre taşları

| Kod | Süre | Ne | Geçme kriteri |
|-----|------|-----|---------------|
| **M1** | 6 hafta | Oynanabilir çekirdek | **5 dış kişi oynadı, 3'ü "devam ederdim" dedi** |
| M2 | **yeniden hesaplanacak** | Dikey dilim + dünya haritası | Yabancı biri 20 dk anlatımsız oynuyor; kalan iş ölçüldü |
| M3 | 12 hafta | Sistem derinliği | 3 saat oynanıyor; Steam sayfası açık, wishlist geliyor |
| M4 | 10 hafta | İçerik üretimi | Tekrar hissi olmadan 8 saat |
| M5 | 8 hafta | Demo + pazarlama | Demo çökmüyor, fragman hazır |
| M6 | ? | Cila + çıkış | Kritik hata yok, üçü de "çıkabilir" diyor |

**M1 neden 6 hafta:** depo artık ayrı bir mekân ve oyunun merkezi. Dün 4 hafta
denenmişti; depo kararı ondan sonra geldi ve kapsamı büyüttü. 4 haftaya sıkıştırmak
depoyu menüye indirger, ki o zaman kararın kendisi anlamsızlaşır.

---

# M1 — Oynanabilir Çekirdek

## Tek hedef

> Yabancı biri oturuyor, kimse anlatmadan bir haftalık oyun döngüsünü oynuyor:
> depoda dosya seç → alet al → eve gir → eşya topla → depoya dön → lot kur →
> Cuma müzayedesinde sat → borcu öde → ertesi hafta.

Güzel görünmesi gerekmiyor. **Eğlenceli olması gerekiyor.**

## M1'de VAR

| Alan | Ne |
|------|-----|
| **Depo** | Tek kademe. Bilgisayar (dosya seç, borç göster, tamirci), alet rafı, tezgâh, eşya alanı |
| **Saha** | 2 ev tipi (gecekondu, daire). Gerçek zamanlı sayaç, saatli gün |
| **Taşıma** | Elde tek eşya + 5 paylaşımlı slot. 3 alet: el arabası, levye, matkap |
| **Engel** | Sadece ev sahibi. Gıdıklama sesi + yazı |
| **Kondisyon** | 3 hâl, decal katmanıyla |
| **Gizli değer** | Sadece ev sahibi ipucu üzerinden |
| **Müzayede** | Anında sonuç. Rezerv, lot, 12 alıcı havuzu |
| **Tamirci** | 1 tamirci. Kademe seçimi + başarısızlık ihtimali |
| **Borç** | Haftalık asgari ödeme, Cuma. Faiz. Haciz |
| **Görsel** | **Sadece eller.** Stilize gölgeleme |
| **İçerik** | 20 eşya, 12 alıcı |

## M1'de YOK — bilerek

Köpek ve komşu · piyasa trendi · katalog abartma · şikâyet/itibar · **tam vücut**
· 5 depo kademesi · kampanya yapısı · serbest mod · dükkân/villa/fabrika ·
canlı müzayede salonu · 2. ve 3. tamirci

Bunların hiçbiri M1'in cevaplaması gereken soruyu etkilemiyor.

---

## Hafta 1–2 · gün gün

### Hafta 1 — İskelet

| Gün | A · Çağatay | B · Özgür | C · Onat |
|-----|-------------|-----------|----------|
| 1 | Field sahnesi + birinci şahıs kontrol | Tohumlu rastgelelik + eşya veri şablonu | **Blender→Unity akışı**: bir küp, doğru ölçek, doğru pivot |
| 2 | Etkileşim: bak, "E" ile al, elde tut | 20 eşyayı doldur (kondisyon, cebeGirer, sabit, bolluk dahil) | Ev kiti: duvar, zemin, kapı boşluğu — 1 m ızgara |
| 3 | 5 slot sistemi: küçük eşya slota, büyük elde | Değer biçme: gizli değer + tahmin aralığı | Ev kiti: pencere, köşe, tavan, kapı kanadı |
| 4 | Kamyon: yükleme, hacim kapasitesi | 12 alıcı + müzayede çözümleyici (ekransız) | Gecekondu evi kur: 3–4 oda, dar koridor |
| 5 | Saatli gün + saha geri sayımı | Borç: haftalık ödeme, faiz, Cuma | 7 öncelikli eşya: yatak, buzdolabı, çamaşır mak., halı, gardırop, masa, TV |
| 6 | **Entegrasyon** — A'nın eşyaları B'nin verisini kullanıyor | **Entegrasyon** | **Entegrasyon** — ölçekler doğru, karakter takılmadan dolaşıyor |

**Hafta sonu kontrolü:** Ev içinde dolaşıp eşya alabiliyor musun? Konsolda
müzayede sonucu çıkıyor mu?

### Hafta 2 — Depo doğuyor

| Gün | A · Çağatay | B · Özgür | C · Onat |
|-----|-------------|-----------|----------|
| 1 | Depo sahnesi + geçişler (depo ↔ saha) | OyunDurumu: para, borç, gün, saat | Depo kiti: beton duvar, kolon, kepenk kapı |
| 2 | Bilgisayar etkileşimi: başına geç, ekran aç | Bilgisayar ekranı: dosya listesi, borç durumu | Depo mekânı kur: raf alanı, tezgâh köşesi, garaj |
| 3 | Alet rafı: 5 slot, alet al/bırak | Alet sistemi verisi + slot mantığı | Alet modelleri: el arabası, levye, matkap |
| 4 | El arabası: ağır eşya taşıma | Depo eşya alanı: hacim kapasitesi, doluluk | Kondisyon decal seti: toz, çizik, çatlak, ezik |
| 5 | Levye ve matkap: kilitli oda, sabit eşya | Tezgâh: lot kurma (fiziksel) | 6 eşya daha (öncelik 2 listesinden) |
| 6 | **Entegrasyon — ilk tam gün** | **Entegrasyon** | **Entegrasyon** |

**Hafta sonu kontrolü:** Depoda başlayıp eve gidip dönebiliyor musun?

---

## Hafta 3–6 · haftalık hedefler

Bu haftalar gün gün yazılmadı — 1. ve 2. hafta bittiğinde ne kadar
sürdüğünü bilecek ve buradan devam edeceğiz. Şimdi gün atamak tahmin olur.

### Hafta 3 — Döngü kapanıyor
- **A:** ev sahibi engeli (yaklaşınca bağırır, süre hızlanır), ipucu davranışı
- **B:** müzayede ekranı, rezerv girişi, Cuma takvimi, gün sonu hesabı
- **C:** apartman dairesi (merdiven), ev sahibi modeli (1 tip)

**Kontrol:** Baştan sona bir hafta oynanabiliyor mu? Hayırsa Hafta 4'e geçme.

### Hafta 4 — Derinlik
- **A:** kırılganlık ve kondisyon düşmesi, gizli değer arama etkileşimi
- **B:** tamirci sistemi, kondisyon → değer bağlantısı, haciz
- **C:** kalan eşyalar (20'ye tamamla), 3 ev sahibi tipi

### Hafta 5 — Ayar
- Üçü de: denge. Kapasite mi süre mi bağlayıcı, alıcı havuzu oranı, faiz,
  tamir fiyatları. **Ölçerek, tahmin ederek değil.**
- **C:** referans panosu + palet, stilize gölgeleme shader'ı

### Hafta 6 — Dış test
- Yeni özellik yok
- 5 dış kişiye oynatılır, izlenir, karışılmaz
- En can yakıcı 3 sorun düzeltilir
- M1 kapanışı: kalan iş listesi, M2 kapsamı

---

## Dış test nasıl yapılır

- **Anlatmayın.** Tek cümle: "Şunu oyna." Kontrolleri bile söylemeyin.
- **Karışmayın.** Takılırsa izleyin. Yardım ettiğiniz an veri bozulur.
- **Ne dediklerini değil, ne yaptıklarını yazın.** "Güzelmiş" bilgi değil;
  "kanepeyi kapıdan geçiremedi, 40 saniye uğraştı, bıraktı" bilgi.
- **Tek soru, sonda:** "Devam eder miydin?"

## Kapsam kesme sırası

Takvim şişerse yukarıdan aşağı kesin:

1. Matkap ve sabit eşyalar → sonraki milat
2. Tamirci → sonraki milat (kondisyon kalır, tamir gitmez)
3. Gizli değer → sonraki milat
4. İkinci ev tipi (daire) → tek ev tipiyle test edilir
5. Levye ve kilitli oda → sonraki milat

**Asla kesilmez:** evi boşaltma döngüsü · değer biçme belirsizliği ·
alıcı kişilikleri · borç baskısı

## Bilinen riskler

**Depo, saha kadar büyük bir mekân.** C'nin yükü iki mekân birden. İki ayrı kit
kararı bunu artırıyor — hafifletici, depo kitinin daha az ve daha büyük parçadan
kurulması.

**Bağlayıcı kısıt kapasite yerine süre olabilir.** Tek eşya taşıma + gerçek
zamanlı sayaç birleşimi bunu yaratıyor. Hafta 5'te ölçülecek.

**Depo yer kısıtı + lot sınırı birlikte kilitleyebilir.** Erken oyunda en az
birinin gevşek olması gerekiyor.

**12 havuz / 8 gelen oranı nadirliği öldürüyor.** M1'de gelen sayısı 4–5 olmalı.

**Tam vücut M1'de yok, M2'de gelecek.** Karar duruyor; sadece sırası değişti.
Her eşya boyutu için ayrı taşıma animasyonu gerektirdiği için M1'i geciktirirdi.
