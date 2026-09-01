using System;
using System.Collections.Generic;
using Oyun.Core;
using Oyun.Utils;

namespace Oyun.Auction
{
    // Muzayedeyi ekransiz cozer. M1'de canli salon YOK: sonuc aninda
    // hesaplanip gosteriliyor (docs/tasarim/03-muzayede.md, kilitli karar).
    //
    // IKINCI FIYAT ACIK ARTIRMA (denemede olculdu, iyi calisti)
    // Kazanan en yuksek teklifi veren, ama odedigi IKINCI teklifin biraz ustu.
    // Tasarim acisindan onemli sonucu su: rezerv sana yuksek fiyat
    // KAZANDIRMAZ, sadece ucuza gitmeni engeller.
    //
    // BUTCE OTURUM BOYUNCA TUKENIR (denemede olculdu)
    // Tukenmezse her lot bagimsiz hesaplanir ve alici cesitliligi anlamsizlasir.
    // Tukendiginde "neyi once satiyorum" karari doguyor -- o yuzden lot SIRASI
    // cagirana ait, cozumleyici siralamiyor.
    public static class MuzayedeCozumleyici
    {
        /// <summary>Bir muzayede oturumunu bastan sona cozer.</summary>
        /// <param name="lotlar">Cikis sirasiyla. Sira onemli: butce tukeniyor.</param>
        /// <param name="uretici">
        /// Tohumlu uretici, kendi dalindan gelmeli -- ornegin
        /// <c>kok.Dal("muzayede-katilim")</c>.
        /// </param>
        public static MuzayedeSonucu Coz(
            IReadOnlyList<Lot> lotlar, Rastgele uretici, MuzayedeAyarlari ayarlar)
        {
            if (lotlar == null) throw new ArgumentNullException(nameof(lotlar));
            if (uretici == null) throw new ArgumentNullException(nameof(uretici));
            if (ayarlar == null) throw new ArgumentNullException(nameof(ayarlar));

            var gelenler = GelenleriSec(uretici, ayarlar);
            return Coz(lotlar, gelenler, ayarlar);
        }

        /// <summary>Gelenler belliyken cozer. Testler ve kampanya gorevleri icin.</summary>
        public static MuzayedeSonucu Coz(
            IReadOnlyList<Lot> lotlar, IReadOnlyList<Alici> gelenler, MuzayedeAyarlari ayarlar)
        {
            if (lotlar == null) throw new ArgumentNullException(nameof(lotlar));
            if (gelenler == null) throw new ArgumentNullException(nameof(gelenler));
            if (ayarlar == null) throw new ArgumentNullException(nameof(ayarlar));
            if (gelenler.Count == 0)
                throw new ArgumentException("Salonda kimse yok.", nameof(gelenler));

            // Butce oturum boyunca eriyor; katalogdaki Alici degismez, kalan
            // butce burada tutuluyor.
            var kalanButce = new Dictionary<string, int>(gelenler.Count, StringComparer.Ordinal);
            foreach (var alici in gelenler) kalanButce[alici.Id] = alici.Butce;

            var sonuclar = new List<LotSonucu>(lotlar.Count);
            foreach (var lot in lotlar)
                sonuclar.Add(LotuCoz(lot, gelenler, kalanButce, ayarlar));

            return new MuzayedeSonucu(gelenler, sonuclar);
        }

        /// <summary>Bir alicinin lota bictigi deger, TL.</summary>
        /// <remarks>
        /// Esya basina hesaplaniyor, lot basina degil: karisik bir lotta her
        /// esya kendi kategorisinin carpanini aliyor. Antikaci, icinde tek bir
        /// antika olan lota o antika kadar deger biciyor -- gerisine degil.
        /// </remarks>
        public static int Degerlendirme(Alici alici, Lot lot)
        {
            if (alici == null) throw new ArgumentNullException(nameof(alici));
            if (lot == null) throw new ArgumentNullException(nameof(lot));

            double toplam = 0.0;
            foreach (var esya in lot.Esyalar)
                toplam += esya.GercekDeger * (double)alici.Carpan(esya.Tanim.Kategori);

            return (int)Math.Round(toplam, MidpointRounding.AwayFromZero);
        }

        /// <summary>Havuzdan katilim agirligina gore, tekrarsiz alici secer.</summary>
        public static IReadOnlyList<Alici> GelenleriSec(Rastgele uretici, MuzayedeAyarlari ayarlar)
        {
            if (uretici == null) throw new ArgumentNullException(nameof(uretici));
            if (ayarlar == null) throw new ArgumentNullException(nameof(ayarlar));

            var havuz = new List<Alici>(AliciKatalogu.Hepsi);
            var agirliklar = new List<int>(havuz.Count);
            foreach (var alici in havuz) agirliklar.Add(alici.KatilimAgirligi);

            var gelenler = new List<Alici>(ayarlar.GelenSayisi);
            for (int i = 0; i < ayarlar.GelenSayisi; i++)
            {
                int secilen = uretici.AgirlikliSecIndeks(agirliklar);
                gelenler.Add(havuz[secilen]);

                // Secileni cikar: ayni alici iki kez gelemez.
                havuz.RemoveAt(secilen);
                agirliklar.RemoveAt(secilen);
            }

            return gelenler;
        }

        static LotSonucu LotuCoz(
            Lot lot,
            IReadOnlyList<Alici> gelenler,
            Dictionary<string, int> kalanButce,
            MuzayedeAyarlari ayarlar)
        {
            // Teklif = degerlendirme, ama kalan butceyi asamaz. Denemede butce
            // asilabildiginde 3.000 TL'lik komsu 6.000 TL'lik alisveris yapiyordu.
            Alici enIyi = null;
            int enYuksek = 0;
            int ikinci = 0;
            int teklifSayisi = 0;

            foreach (var alici in gelenler)
            {
                int butce = kalanButce[alici.Id];
                if (butce <= 0) continue;

                int teklif = Math.Min(Degerlendirme(alici, lot), butce);
                if (teklif <= 0) continue;

                teklifSayisi++;

                if (teklif > enYuksek)
                {
                    ikinci = enYuksek;
                    enYuksek = teklif;
                    enIyi = alici;
                }
                else if (teklif > ikinci)
                {
                    ikinci = teklif;
                }
            }

            if (enIyi == null)
                return new LotSonucu(lot, null, 0, SatisSonucu.AliciYok, 0, 0);

            if (enYuksek < lot.Rezerv)
                return new LotSonucu(lot, null, 0, SatisSonucu.RezervinAltinda, teklifSayisi, 0);

            // Ikinci fiyat: ikinci teklifin biraz ustu, ama rezervin altina
            // inmez ve kazananin kendi degerlendirmesini asamaz.
            int fiyat = Math.Max(lot.Rezerv, ikinci + ayarlar.TeklifArtisi);
            fiyat = Math.Min(fiyat, enYuksek);

            kalanButce[enIyi.Id] -= fiyat;

            return new LotSonucu(lot, enIyi, fiyat, SatisSonucu.Satildi, teklifSayisi, enYuksek);
        }
    }
}
