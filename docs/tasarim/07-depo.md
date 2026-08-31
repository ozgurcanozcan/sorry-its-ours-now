# 07 — Depo (merkez)

**Depo bir aşama değil, oyuncunun merkezi.** Gün burada başlar, burada biter,
arada iki kez daha buraya dönülür.

Bu karar "Ofis" ve "Hazırlık" aşamalarını yuttu — ikisi de artık depoda.
İki ayrı menü yerine tek bir mekân.

## Biçim: fiziksel mekân *(karar: 31.08.2026)*

Depo **içinde yürüdüğün bir mekân.** Sahayla aynı kip: birinci şahıs.
Bilgisayara yürüyüp başına geçiyorsun, raftan alet alıyorsun, tezgâha gidiyorsun.

Menü değil. Sebep: depo "merkez" hissi vermeli, ve bürokrasi mizahının bir mekânı
olmalı — form dolduran bir ekran, üstünde damga, yanında çay bardağı.

> **Maliyet uyarısı:** bu C için ciddi iş. Depo, saha evleri kadar önemli bir
> mekân ve oyuncunun en çok vakit geçirdiği yer olacak. Modüler oda kiti buraya
> da hizmet etmeli — yoksa depo ayrı bir üretim hattı olur.
>
> Ayrıca A için: depoda etkileşim (bilgisayar başına oturma, raftan alma) sahadaki
> eşya alma etkileşiminden farklı. İkisi tek sistemle çözülmeli.

## Depoda ne var

| Yer | Ne yapılıyor |
|-----|--------------|
| **Bilgisayar** | Haciz dosyaları, şirket durumu, borç, yükseltmeler, **tamirci** |
| **Alet rafı** | Sahaya çıkmadan alet seçilir — sınırlı slot |
| **Tezgâh** | Lot kurma |
| **Eşya alanı** | Sahadan gelen ve satılmayan eşyalar burada durur, **yer kaplar** |
| **Garaj / kapı** | Kamyon seçimi, sahaya çıkış |

## Eşya alanı — sabrın para birimi

Satılmayan eşya depoda kalır ve sonraki müzayedeye tekrar çıkarılabilir.
**Bu, rezerv kararını anlamlı kılan şey.**

**Bekletmenin tek bedeli: yer.** *(karar: 31.08.2026)*

Kira yok, bayatlama yok. Eşya yer kaplar; depo dolunca **yeni iş alamazsın**.

Bu tek kısıt üç şeyi birden çözüyor:

- Bekletmek bedava değil — ama bedeli para değil **fırsat**
- Depo yükseltmesi anlamlı bir ödül oluyor: daha çok yer = daha çok sabır
- "Şu kilimi Koleksiyoncu gelene kadar tutayım mı?" sorusunun cevabı depo
  doluluğuna bağlı, yani duruma göre değişiyor

> **Dikkat:** depo büyüdükçe bekleme kararı kolaylaşır. Bu bir kusur değil,
> ilerlemenin kendisi — ama denge kurulurken depo kapasitesi eşya hacimleriyle
> aynı ölçekte tutulmalı. Denemede kamyon kapasitesi bu yüzden kırılmıştı.

**Açık:** kapasite neyle ölçülüyor — eşya sayısı mı, hacim mi? Hacim daha
tutarlı olur (eşyaların zaten hacmi var, kamyon da hacimle ölçülüyor).

## Tamirci *(karar: 31.08.2026)*

Temizleme ve tamir **mini oyun değil, hizmet.** Dealer's Life 2'deki gibi:

1. Bilgisayardan bir tamirci seçilir
2. Ücret ödenir
3. **Eşya depoda kalır ama kilitlenir** — üstünde `TAMİRDE · Kalan süre: 3 gün`
   uyarısı görünür. Alınamaz, lota konamaz, satılamaz.
4. Süre dolunca kilit kalkar, eşya **bir kondisyon kademesi yükselmiş** olarak
   kullanıma döner ve **tahmin aralığı güncellenir** — oyuncu ne kazandığını görür

Tamir, `02-saha.md`'deki üç kondisyon üzerinden çalışır:
`hasarlı → eski → iyi durumda`

Bunun güzelliği: el becerisi değil, **zamanlama ve ekonomi kararı**.

- *"Bu eşyaya 300 ₺ ve üç gün harcamaya değer mi?"*
- *"Cuma müzayedesine yetişir mi?"*

> **Önemli:** eşya tamirdeyken **yer kaplamaya devam eder.**
>
> Bu bilinçli bir karar. Yer açsaydı en iyi strateji "depo dolunca her şeyi
> tamire yolla" olurdu; tamir bir ekonomik karar olmaktan çıkıp **depolama
> hilesine** dönerdi. Yer kaplamaya devam edince tamir gerçek bir maliyet
> taşıyor: para, süre ve **yer**, aynı anda.
>
> Ayrıca oyuncu eşyasının nerede olduğunu görmeye devam ediyor — "eşyam
> kayboldu" kafa karışıklığı olmuyor.

**Açık:** kaç tamirci var? Dealer's Life'ta farklı tamircilerin fiyat, hız ve
kalite dengesi farklı — ucuz ve yavaş, pahalı ve hızlı, ya da riskli olan.
Aynı yapı buraya da uyar ama karar verilmedi.

**Açık:** tamir sonucu kesin mi? Kademe yükselmesi garanti mi, yoksa başarısız
olabiliyor mu? Riskli tamirciler ucuz, güvenilir olanlar pahalı olabilir —
ama başarısızlık sinir bozucu olabilir, dikkatli denenmeli.

**Açık:** hasarlı bir eşya iki kademe birden çıkabilir mi (hasarlı → iyi durumda),
yoksa tavanı "eski" mi? İkincisi sahada dikkatli olmayı ödüllendirir: kırdığın
eşyayı asla tam geri kazanamazsın.

## Aletler — sınırlı slot *(karar: 31.08.2026)*

Sahaya çıkarken raftan **belirli sayıda** alet alınır. Hacimle ilgisi yok,
kamyon yerinden yemiyor — ayrı ve okunaklı bir kısıt.

Bilinen adaylar: matkap, levye, el arabası, battaniye.
Bunlar `02-saha.md`'deki engellere cevap veriyor: kilitli oda, ağır eşya,
kırılganlık.

**Açık:** kaç slot? İki alet mi üç mü — bu sayı "neyi feda ediyorsun" kararının
sertliğini belirliyor.

**Açık:** aletler satın mı alınıyor, kademeyle mi geliyor? Yıpranıyor mu?

## Depo bir ilerleme ekseni

Oyun ilerledikçe daha gelişmiş depolara geçilir:

- daha fazla eşya alanı (bekletme kapasitesi)
- daha fazla alet slotu
- belki daha iyi bilgisayar (daha çok dosya görünür, daha iyi tamirciler açılır)

**Açık:** kaç kademe depo var ve her biri neyi açıyor.

> Not: depo kademesi ile **şirket kademesi** (`04-ekonomi.md`) aynı şey değil.
> Şirket kademesi hangi iş tiplerinin açıldığını söyler; depo kademesi fiziksel
> kapasiteyi. İkisi bağlantılı olabilir ama ayrı tutulmalı.

## Açık sorular

- [ ] Depo kapasitesi neyle ölçülüyor — eşya sayısı mı, hacim mi?
- [ ] Kaç kademe depo, her biri neyi açıyor?
- [ ] Kaç alet slotu?
- [ ] Aletler satın mı alınıyor, yıpranıyor mu?
- [ ] Kaç tamirci, fiyat/hız/kalite dengesi nasıl?
- [ ] Tamir sonucu kesin mi, riskli mi?
- [ ] Depoda geçen süre oyun gününden mi yiyor, serbest mi?
- [ ] Sahadan dönünce eşyalar otomatik mi boşalıyor, elle mi taşınıyor?
- [ ] Depo mekânı kaç oda? Tek büyük hacim mi, bölmeli mi?
