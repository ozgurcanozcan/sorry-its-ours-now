using System;

namespace Oyun.Core
{
    // Bir esya TURUNUN degismez tanimi -- sahnedeki tek bir esya ornegi degil.
    // Kondisyon, gizli deger ve bulundugu oda ornege ait; onlar burada yok.
    //
    // Alan seti docs/tasarim/05-icerik.md'den. Iki alan bilerek disarida:
    //
    //   klasman           -- tamir zorlugu. Kac kademe oldugu ve neye gore
    //                        belirlendigi henuz karara baglanmadi; tamirci
    //                        Hafta 4'te geliyor, o zaman eklenecek.
    //   degerBelirsizligi -- docs/tasarim/03-muzayede.md olculmus tek bir genel
    //                        kural veriyor (+-%28). Esya basina yuzde yazmak o
    //                        olcumu esya sayisi kadar tahmine boluyordu; deger
    //                        bicme sisteminin parametresi olarak duruyor.
    //
    // Ikisi de sonradan eklenebilir: "Code/Core'a ekleme serbest, degistirme degil".
    public sealed class EsyaVerisi
    {
        /// <summary>Kalici kimlik. Prefab'lar ve kayit dosyasi bu metne baglanir.</summary>
        /// <remarks>
        /// Bir kez yayimlandiktan sonra degistirilemez: eski kayitlar bu metinle
        /// esyayi buluyor. Gorunen ad degisebilir, id degisemez.
        /// </remarks>
        public string Id { get; }

        /// <summary>Oyuncuya gorunen ad.</summary>
        public string Ad { get; }

        public Kategori Kategori { get; }

        /// <summary>Ikinci el piyasa referansi, TL. Kondisyon ve alici carpani bunun uzerine biner.</summary>
        /// <remarks>
        /// docs/uretim/esyalar.md: bu sayilar ham piyasa referansi, oyun
        /// ekonomisine gore olceklenmedi. Olcekleme Hafta 5 denge isi.
        /// Kurus yok -- para tamsayi, kayan noktada birikimli hata borcu kaydirir.
        /// </remarks>
        public int TabanDeger { get; }

        /// <summary>Kilogram. Tasima zorlugu ve ikiKisiGerekir esigi buna bakar.</summary>
        public float Agirlik { get; }

        /// <summary>Metrekup. Depo ve kamyon kapasitesi ayni birimde (karar 26).</summary>
        public float Hacim { get; }

        /// <summary>Dusurulunce/carpilinca kondisyon kaybeder.</summary>
        public bool Kirilgan { get; }

        /// <summary>
        /// Yasal olarak alinabilir mi. Yatak, mutfak esyasi ve mesleki alet yasak
        /// listesinde (docs/tasarim/02-saha.md). Yasak olani almak engellenmez --
        /// bedeli sure ve gurultu, para cezasi degil.
        /// </summary>
        public bool Haczedilebilir { get; }

        /// <summary>Tek basina tasinamaz.</summary>
        public bool IkiKisiGerekir { get; }

        /// <summary>
        /// 5 paylasimli slottan birine giren kucuk esya (docs/tasarim/02-saha.md).
        /// Hacimden turetilmiyor: yagliboya tablo hacimce kucuk ama genis, cebe
        /// girmemeli. Elle karar veriliyor.
        /// </summary>
        public bool CebeGirer { get; }

        /// <summary>
        /// Duvara/zemine bagli: radyator, mutfak dolabi, bakir tesisat, klima, avize.
        /// Matkap olmadan hic alinamaz; sokme islemi ayrica sure yer.
        /// </summary>
        public bool Sabit { get; }

        /// <summary>
        /// 1-10: bir evde bulunma sikligi. Yatak 9, el dokuma kilim 1.
        /// </summary>
        /// <remarks>
        /// Denge sayisi degil, icerik dagilim parametresi. Ev icerigi bu agirliga
        /// gore uretiliyor; duz rastgele secimde gecekonduda uc el dokuma kilim
        /// cikiyor ve antikanin nadirligi kayboluyor (docs/tasarim/02-saha.md).
        /// </remarks>
        public int Bolluk { get; }

        public const int EnDusukBolluk = 1;
        public const int EnYuksekBolluk = 10;

        public EsyaVerisi(
            string id,
            string ad,
            Kategori kategori,
            int tabanDeger,
            float agirlik,
            float hacim,
            bool kirilgan,
            bool haczedilebilir,
            bool ikiKisiGerekir,
            bool cebeGirer,
            bool sabit,
            int bolluk)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Esya id'si bos olamaz.", nameof(id));
            if (string.IsNullOrWhiteSpace(ad))
                throw new ArgumentException("Esya adi bos olamaz. (id: " + id + ")", nameof(ad));
            if (!Enum.IsDefined(typeof(Kategori), kategori))
                throw new ArgumentException("Tanimsiz kategori. (id: " + id + ")", nameof(kategori));
            if (tabanDeger < 0)
                throw new ArgumentException("Taban deger negatif olamaz. (id: " + id + ")", nameof(tabanDeger));
            if (!(agirlik > 0.0f))
                throw new ArgumentException("Agirlik pozitif olmali. (id: " + id + ")", nameof(agirlik));
            if (!(hacim > 0.0f))
                throw new ArgumentException("Hacim pozitif olmali. (id: " + id + ")", nameof(hacim));
            if (bolluk < EnDusukBolluk || bolluk > EnYuksekBolluk)
                throw new ArgumentException(
                    "Bolluk " + EnDusukBolluk + "-" + EnYuksekBolluk + " arasinda olmali. (id: " + id + ")",
                    nameof(bolluk));

            // Tek celiskili birlesim bu. "sabit" edinmeyi anlatiyor (matkapla
            // sokuluyor), tasimayi degil -- sokulmus bakir tesisat slota girebilir,
            // radyator iki kisi gerektirebilir. O yuzden sabit ile digerleri celismiyor.
            if (cebeGirer && ikiKisiGerekir)
                throw new ArgumentException(
                    "Cebe giren esya iki kisi gerektiremez. (id: " + id + ")", nameof(ikiKisiGerekir));

            Id = id;
            Ad = ad;
            Kategori = kategori;
            TabanDeger = tabanDeger;
            Agirlik = agirlik;
            Hacim = hacim;
            Kirilgan = kirilgan;
            Haczedilebilir = haczedilebilir;
            IkiKisiGerekir = ikiKisiGerekir;
            CebeGirer = cebeGirer;
            Sabit = sabit;
            Bolluk = bolluk;
        }

        public override string ToString()
        {
            return Ad + " (" + Id + ", " + Kategori + ", " + TabanDeger + " TL)";
        }
    }
}
