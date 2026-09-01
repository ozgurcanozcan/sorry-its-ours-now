using System;
using System.Collections.Generic;

namespace Oyun.Core
{
    // Esya tanimlarinin dogruluk kaynagi (karar: 31.08.2026, hibrit veri).
    // ScriptableObject varliklari BURADAN uretiliyor, tersi degil; ikisi
    // ayrisirsa EsyaKataloguTestleri kiriliyor.
    //
    // M1'de 7 esya var, 20 degil. Kalan 13'un ne adi ne piyasa referansi
    // belirlenmis durumda; docs/plan.md zaten Hafta 4'te "kalan esyalar,
    // 20'ye tamamla" diyor. Model tarafi da 7'de (docs/uretim/esyalar.md).
    //
    // TABAN DEGERLER NEREDEN GELIYOR
    // docs/uretim/esyalar.md Agustos 2026'da ikinci el piyasa araligi topladi,
    // ama aralik verdi, tek sayi degil. Buradaki her sayi su kurala uyuyor:
    //
    //   belgenin verdigi EN DAR bandin orta noktasi alinir; belge bandi
    //   daraltan bir nitelik yaziyorsa (ornegin "alt-orta banda yakin") once
    //   o uygulanir.
    //
    // Sayilar oyun ekonomisine gore OLCEKLENMEDI -- ham piyasa referansi.
    // Olcekleme docs/plan.md Hafta 5 denge isi, borc/haftalik odeme
    // buyuklugune gore oynayarak yapilacak.
    public static class EsyaKatalogu
    {
        static readonly EsyaVerisi[] hepsi =
        {
            // 3.500-15.000, belge "orta segment 5.000-8.000" diyor -> 6.500.
            // Haczedilemez: yasak listesinde (docs/tasarim/02-saha.md). Oyuncu
            // yine de alabiliyor, bedeli sure ve gurultu.
            new EsyaVerisi(
                id: "yatak", ad: "Yatak", kategori: Kategori.Mobilya,
                tabanDeger: 6500, agirlik: 35.0f, hacim: 1.60f,
                kirilgan: false, haczedilebilir: false, ikiKisiGerekir: true,
                cebeGirer: false, sabit: false, bolluk: 9),

            // 4.500-13.000, belge "ev tipi urunler alt-orta banda yakin" diyor.
            // Alt-orta band = 4.500-8.750, orta noktasi -> 6.500.
            new EsyaVerisi(
                id: "buzdolabi", ad: "Buzdolabı", kategori: Kategori.BeyazEsya,
                tabanDeger: 6500, agirlik: 65.0f, hacim: 0.70f,
                kirilgan: false, haczedilebilir: true, ikiKisiGerekir: true,
                cebeGirer: false, sabit: false, bolluk: 7),

            // 3.500-11.000, daraltan nitelik yok -> orta nokta 7.250.
            new EsyaVerisi(
                id: "camasir-makinesi", ad: "Çamaşır Makinesi", kategori: Kategori.BeyazEsya,
                tabanDeger: 7250, agirlik: 70.0f, hacim: 0.31f,
                kirilgan: false, haczedilebilir: true, ikiKisiGerekir: true,
                cebeGirer: false, sabit: false, bolluk: 7),

            // 3.000-14.900, daraltan nitelik yok -> orta nokta 8.950.
            new EsyaVerisi(
                id: "gardirop", ad: "Gardırop", kategori: Kategori.Mobilya,
                tabanDeger: 8950, agirlik: 80.0f, hacim: 1.10f,
                kirilgan: false, haczedilebilir: true, ikiKisiGerekir: true,
                cebeGirer: false, sabit: false, bolluk: 6),

            // OLCULMEDI. Belgenin bulabildigi tek referans 6 kisilik yemek
            // takimi (35.000 TL) ve tek masa icin guvenilir degil. Sayi yerine
            // kisit yazildi: gardiroptan ucuz, halidan pahali. Kisiti
            // EsyaKataloguTestleri koruyor.
            new EsyaVerisi(
                id: "masa", ad: "Masa", kategori: Kategori.Mobilya,
                tabanDeger: 4500, agirlik: 25.0f, hacim: 0.72f,
                kirilgan: false, haczedilebilir: true, ikiKisiGerekir: false,
                cebeGirer: false, sabit: false, bolluk: 8),

            // 500-3.999 (antika olmayan siradan hali) -> orta nokta 2.250.
            // Antika hali 50.000+ ayri bir esya, bu satir onu kapsamiyor.
            new EsyaVerisi(
                id: "hali", ad: "Halı", kategori: Kategori.Mobilya,
                tabanDeger: 2250, agirlik: 8.0f, hacim: 0.056f,
                kirilgan: false, haczedilebilir: true, ikiKisiGerekir: false,
                cebeGirer: false, sabit: false, bolluk: 7),

            // ZAYIF VERI. Tek ornek (~5.950 TL, 42"). Aralik yok, o yuzden
            // orta nokta kurali uygulanamadi; tek veri noktasi oldugu gibi alindi.
            new EsyaVerisi(
                id: "tv", ad: "Televizyon", kategori: Kategori.Elektronik,
                tabanDeger: 5950, agirlik: 12.0f, hacim: 0.048f,
                kirilgan: true, haczedilebilir: true, ikiKisiGerekir: false,
                cebeGirer: false, sabit: false, bolluk: 6),

            // --- Cebe giren esyalar --------------------------------------------
            //
            // Ilk uc kucuk esya. 5 slotlu tasima sistemi (docs/tasarim/02-saha.md,
            // PR #12) yazildi ama denenemedi: katalogda cebeGirer isaretli tek
            // esya yoktu, yedisi de buyuk mobilyaydi.
            //
            // TABAN DEGERLERI OLCULEMEDI. Web aramasi somut rakam vermedi;
            // bulunabilen tek veri "ikinci el telefon 0-17.500 TL" idi ve icinde
            // 0 gectigi icin orta nokta kurali uygulanamaz. Ucu de asagidaki
            // zayifTabanDeger listesinde; kisitlari EsyaKataloguTestleri koruyor.
            //
            // Olculer bilinen urun boyutlari -- esyalar.md'deki mobilya olculeri
            // gibi, tahmin degil.

            // 6 kisilik kutulu takim: kutu ~35 x 25 x 6 cm.
            // Kategori Antika, Mutfak degil: Antikaci alicisinin hedefi ve
            // koleksiyonluk. Mutfak olsaydi yasak listesine girerdi
            // (docs/tasarim/02-saha.md: "yatak, mutfak esyasi, mesleki alet").
            // Bu bir icerik karari -- degistirilmesi gerekiyorsa konusulmali.
            new EsyaVerisi(
                id: "gumus-takim", ad: "Gümüş Çatal Bıçak Takımı", kategori: Kategori.Antika,
                tabanDeger: 6000, agirlik: 2.0f, hacim: 0.00525f,
                kirilgan: false, haczedilebilir: true, ikiKisiGerekir: false,
                cebeGirer: true, sabit: false, bolluk: 2),

            // Kol saati: ~5 x 5 x 2 cm.
            new EsyaVerisi(
                id: "saat", ad: "Kol Saati", kategori: Kategori.Kisisel,
                tabanDeger: 1500, agirlik: 0.1f, hacim: 0.00005f,
                kirilgan: false, haczedilebilir: true, ikiKisiGerekir: false,
                cebeGirer: true, sabit: false, bolluk: 4),

            // Akilli telefon: ~16 x 8 x 1 cm. Kirilgan -- dusunce ekrani gider.
            new EsyaVerisi(
                id: "telefon", ad: "Cep Telefonu", kategori: Kategori.Elektronik,
                tabanDeger: 4000, agirlik: 0.2f, hacim: 0.000128f,
                kirilgan: true, haczedilebilir: true, ikiKisiGerekir: false,
                cebeGirer: true, sabit: false, bolluk: 5)
        };

        // Taban degeri saglam piyasa verisine dayanmayan esyalar. Hafta 5
        // dengesinde once bunlara bakilacak.
        //
        // Liste burada duruyor ki gozden kacmasin: bir testi var ve o test
        // listeyi katalogla birlikte tutuyor. Yeni esya eklerken olculmemis
        // deger koyarsan buraya da yazmadikca test kirilir.
        static readonly string[] zayifTabanDeger =
        {
            "masa",         // belgenin tek referansi 6 kisilik takim, tek masa icin gecersiz
            "tv",           // tek veri noktasi (~5.950 TL, 42")
            "gumus-takim",  // arama somut rakam vermedi
            "saat",         // arama somut rakam vermedi
            "telefon"       // "0-17.500 TL" -- icinde 0 gecen aralik, orta noktasi anlamsiz
        };

        static readonly Dictionary<string, EsyaVerisi> kimlige = Kur();

        static Dictionary<string, EsyaVerisi> Kur()
        {
            var sozluk = new Dictionary<string, EsyaVerisi>(hepsi.Length, StringComparer.Ordinal);
            foreach (var esya in hepsi)
            {
                if (sozluk.ContainsKey(esya.Id))
                    throw new InvalidOperationException("Katalogda tekrarlanan esya id'si: " + esya.Id);

                sozluk.Add(esya.Id, esya);
            }

            return sozluk;
        }

        public static IReadOnlyList<EsyaVerisi> Hepsi { get { return hepsi; } }

        /// <summary>Taban degeri saglam piyasa verisine dayanmayan esyalarin id'leri.</summary>
        public static IReadOnlyList<string> ZayifTabanDeger { get { return zayifTabanDeger; } }

        /// <summary>Id ile esya tanimi. Bulunamazsa hata verir -- kayit dosyasi bozuksa sessiz kalmamali.</summary>
        public static EsyaVerisi Bul(string id)
        {
            EsyaVerisi esya;
            if (!kimlige.TryGetValue(id ?? string.Empty, out esya))
                throw new KeyNotFoundException("Katalogda boyle bir esya yok: " + id);

            return esya;
        }

        public static bool VarMi(string id)
        {
            return id != null && kimlige.ContainsKey(id);
        }
    }
}
