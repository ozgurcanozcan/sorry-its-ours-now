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

**Kapasite hacimle ölçülür (m³)** *(karar: 31.08.2026)*. Eşyaların zaten hacmi
var ve kamyon da hacimle ölçülüyor — tek birim, tek mantık. Gardırop çok yer
kaplar, gümüş takım az.

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

### Kaç kademe — oyuncu seçer *(karar: 31.08.2026)*

Tamir bir kademe de yükseltebilir, iki kademe de. **Seçim oyuncunun.**

`hasarlı → eski` ucuz ve hızlı.
`hasarlı → iyi durumda` pahalı ve uzun.

Yani her eşyada ikinci bir soru daha var: *"yarım mı bırakayım, sonuna kadar
mı gideyim?"* Cuma müzayedesine yetişmesi gereken bir eşyada tek kademe akıllıca
olabilir.

### Başarısızlık ihtimali *(karar: 31.08.2026)*

Tamir garantili değil. Başarı ihtimali iki şeye bağlı:

- **Ürün klasmanı** — eşyanın tamir zorluğu *(tanımlanacak, bkz. `05-icerik.md`)*
- **Tamircinin becerisi** — ucuz ve acemi mi, pahalı ve usta mı

İki kademe atlamak doğal olarak daha riskli.

> **Kural: ihtimal önceden gösterilir.**
>
> Oyuncu "onayla" demeden önce başarı yüzdesini görmeli. Gördüğü hâlde riske
> girdiyse bu bir **karar**; görmeden başına geldiyse bu bir **ceza**.
>
> Aynı mantık kondisyonun görünür olmasıyla aynı: kararı verebilmesi için
> gereken bilgi oyuncuda olmalı. Gizli kalan şey **sonuç** olmalı, **olasılık**
> değil.

**Başarısızlıkta:** *(karar: 31.08.2026)* para ve süre gider, **eşya bozulmaz.**
Sonuç **sürenin sonunda** belli olur — üç gün beklersin, eşya aynı hâlde döner.

Bu, riski gerçek ama adil kılıyor: kaybettiğin para ve zaman, elindeki mal değil.
Cuma'ya yetiştirmek için riskli tamirciye verdiysen ve tutmadıysa, o hafta
satamıyorsun — ceza zaten bu.

### Üç tamirci *(karar: 31.08.2026)*

| | Fiyat | Hız | Beceri |
|---|---|---|---|
| **Acemi** | ucuz | yavaş | riskli |
| **Orta** | orta | orta | orta |
| **Usta** | pahalı | hızlı | güvenilir |

**Şirket kademesi ilerledikçe açılırlar.** Erken oyunda tek seçeneğin acemi
olması doğru: riski göze almak zorunda kalırsın, ilerledikçe rahatlarsın.

## Aletler — sınırlı slot *(karar: 31.08.2026)*

Sahaya çıkarken raftan **belirli sayıda** alet alınır. Hacimle ilgisi yok,
kamyon yerinden yemiyor — ayrı ve okunaklı bir kısıt.

### İlk alet seti *(karar: 31.08.2026)*

| Alet | Ne açıyor |
|------|-----------|
| **El arabası** | Ağır eşya — buzdolabı, gardırop, koltuk takımı. Tek kişilikte şart. |
| **Levye** | Kilitli oda ve dolap. Gürültü çıkarır, ev sahibini üzerine çeker. |
| **Matkap** | **Sabit eşya sökme** — radyatör, mutfak dolabı, bakır tesisat. |

Battaniye (kırılgan koruma) **listede yok**. Kırılganlık riski aletle değil,
oyuncunun dikkatiyle yönetiliyor: acele etmezsen düşürmezsin.

> **Matkap yeni bir eşya sınıfı açıyor: sabit eşyalar.** Duvara veya zemine
> bağlı oldukları için alet olmadan hiç alınamazlar. Bu, alet kararını en sert
> hâline getiriyor — matkap almadıysan evdeki en değerli şey gözünün önünde
> duruyor ve dokunamıyorsun.
>
> Sökme işlemi süre yer, yani matkap almak yetmiyor; zaman da ayırmak gerekiyor.

**5 slot** *(karar: 31.08.2026)* — ve bu slotlar **küçük eşyalarla paylaşılır**
(`02-saha.md`). Alet almak, o sefer taşıyacağın küçük eşya sayısını düşürür.

Aletler **para ile satın alınıyor** *(karar: 31.08.2026)* — erken oyunda gerçek
bir tercih. Yıpranıp yıpranmadığı açık.

## Depo bir ilerleme ekseni

Oyun ilerledikçe daha gelişmiş depolara geçilir:

- daha fazla eşya alanı (bekletme kapasitesi)
- daha fazla alet slotu
- belki daha iyi bilgisayar (daha çok dosya görünür, daha iyi tamirciler açılır)

**5 kademe** *(karar: 31.08.2026)*, şirket kademeleriyle eşleşiyor. Ayrıca her
deponun içinde ayrı geliştirme yapılabiliyor: raf, tezgâh, bilgisayar.

> **C için:** beş depo, beş ayrı bina değil. Aynı modüler kitten kurulsun,
> farkları boyut ve yerleşim olsun. Malzeme ve detay seviyesi kademeyle artsın
> (çıplak beton → boyalı duvar → düzgün aydınlatma).

> **Depo kademesi ile şirket kademesi eşleşiyor ama aynı şey değil.**
> Şirket kademesi hangi iş tiplerinin ve hangi tamircilerin açıldığını söyler;
> depo kademesi fiziksel kapasiteyi. Beşer kademe oldukları için birlikte
> ilerliyorlar, ama kodda ayrı tutulmalılar — ileride biri diğerinden
> ayrılabilir.

## Açık sorular

- [x] Hacim (m³) *(karar: 31.08.2026)*
- [x] 5 kademe + her kademede içsel geliştirme *(karar: 31.08.2026)*
- [x] 5 slot, küçük eşyalarla paylaşımlı *(karar: 31.08.2026)*
- [ ] Aletler satın mı alınıyor, yıpranıyor mu?
- [x] 3 tamirci, şirket kademesiyle açılır *(karar: 31.08.2026)*
- [x] Başarısızlıkta para ve süre gider, eşya bozulmaz *(karar: 31.08.2026)*
- [x] Başarısızlık sürenin sonunda belli olur *(karar: 31.08.2026)*
- [ ] Ürün klasmanı nasıl belirleniyor? (`05-icerik.md`)
- [x] Depoda da saat işler *(karar: 31.08.2026)*
- [ ] Sahadan dönünce eşyalar otomatik mi boşalıyor, elle mi taşınıyor?
- [ ] Depo mekânı kaç oda? Tek büyük hacim mi, bölmeli mi?
