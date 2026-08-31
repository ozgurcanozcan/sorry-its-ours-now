# 08 — Dünya

**Oyun gezilebilir bir kasabada geçiyor.** *(karar: 31.08.2026)*

Depo bir binadır, evler binalardır, aralarında kamyonla gidilir. Menüden adres
seçip sahne yüklenmiyor — **dünyanın içinde yaşıyorsun.**

Referans: Schedule I.

## Kararlar

| Konu | Karar |
|------|-------|
| Yapı | **Tek kasaba, kesintisiz.** Mahalleler arası yükleme ekranı yok |
| Ulaşım | **Kamyonu sen sürüyorsun.** Araç fiziği, direksiyon, park etme |
| Canlılık | **Trafik + yayalar + girilebilir dükkânlar** |
| Zamanlama | **M2.** M1'de harita ekranı kalıyor |

## Zamanlama: M1'de yok, M2'de var

M1 planlandığı gibi kalıyor — bilgisayardan dosya seç, sahne yüklensin, evi boşalt.
Dünya M2'de geliyor.

> **Neden böyle doğru:** M1'in tek sorusu *"bu döngü eğlenceli mi?"* Bu sorunun
> cevabı haritadan bağımsız. Harita eğlenceyi artırır ama yaratmaz — çekirdek
> sıkıcıysa gezilebilir bir kasaba onu kurtarmaz.
>
> Ayrıca sıra doğru: kasabayı kurmadan önce evlerin ve deponun nasıl göründüğünü
> bilmek gerekiyor. Onat M1'de iç mekân dilini oturtacak, dış mekân onun üstüne
> gelecek.

## M1'de alınacak önlemler

Harita M2'de ama M1'de yapılan işler onu **engellememeli**:

- **Depo bir bina gibi tasarlansın.** İçi M1'de kuruluyor; dışı M2'de sokağa
  bakacak. Girişi, garaj kapısı, cephesi olacağı düşünülerek yerleştirilsin.
- **Ev sahneleri bir sokağa oturacak şekilde kurulsun.** Kapı yönü, giriş
  noktası, kamyonun duracağı yer belirsiz kalmasın.
- **Sahne geçişi tek noktadan yapılsın.** M1'de "dosya seç → yükle", M2'de
  "kamyonla git → kapıdan gir" olacak. Aradaki fark tek bir sistemin içinde
  kalmalı ki değiştirmek kolay olsun.
- **Saat sistemi yol süresini kaldırabilmeli.** M1'de yol süresi sıfır; M2'de
  gerçek olacak. Saat mantığı buna hazır yazılsın.

## Maliyet — açıkça yazılıyor

Bu, projenin en pahalı kararı. Kayıt için:

**C (Onat):** iç mekân kitine ek olarak **dış mekân üretimi** — bina cepheleri,
sokak, kaldırım, çevre nesneleri, dükkân cepheleri. Ayrı bir üretim hattı; oda
kitiyle çözülmüyor.

**A (Çağatay):** **araç kontrolü** yepyeni bir sistem. Direksiyon, fizik, park
etme, çarpışma. Kendi başına haftalar. Ayrıca trafik ve yaya AI'ı.

**B (Özgür):** yol süresinin saate bağlanması, harita üzerinden iş seçimi,
dükkânların ekonomiye bağlanması.

**Ortak:** kesintisiz tek kasaba **streaming** ve performans işi getiriyor —
uzaktaki binaların yüklenmesi/boşaltılması, LOD, ışıklandırma bütçesi.

> **Bu bir itiraz değil, plana yazılması gereken bir gerçek.** M2'nin süresi
> bu yüzden yeniden hesaplanmalı; dünkü 8 haftalık M2 tahmini bu kararı
> içermiyordu.

## Doğan fırsatlar

Maliyeti büyük ama karşılığında birkaç şey bedavaya geliyor:

**Yol süresi gerçek bir karar oluyor.** *"Şehrin öbür ucundaki zengin ev mi,
yakındaki üç gecekondu mu?"* İş seçimi coğrafyaya bağlanıyor.

**Kamyon park yeri kararı anlam kazanıyor** *(karar 21)*. Apartmanda sokakta
park etmek artık soyut bir mesafe değil, gerçekten yürüyeceğin yol.

**Dükkânlar ekonomiye bağlanabiliyor.** Alet satın alma, tamirci, hurdacı —
hepsi menüde değil, gidilecek bir yerde.

**Ev sahibiyle sokakta karşılaşma** mümkün oluyor. Haczettiğin adamı ertesi
gün bakkalda görmek, bu oyunun tonuna çok uygun.

## Açık sorular

- [ ] Kasaba ne kadar büyük? (yürüyerek uçtan uca kaç dakika)
- [ ] Kaç mahalle karakteri olacak? (gecekondu / apartman / sanayi / merkez)
- [ ] Ev içine giriş kesintisiz mi, kapıda kısa bir yükleme mi?
- [ ] Depo haritanın neresinde? Kademe atlayınca taşınıyor musun?
- [ ] Trafikte kaza olur mu? Olursa sonucu ne?
- [ ] Yayalarla etkileşim var mı, yoksa sadece dekor mu?
- [ ] Girilebilir dükkânlar hangileri? (hırdavatçı, bakkal, hurdacı, tamirci?)
- [ ] Gece haritada ne değişiyor? (dükkânlar kapanıyor mu?)
- [ ] Evler haritada sabit mi, yoksa aynı bina farklı işlerde farklı iç mekân mı?
