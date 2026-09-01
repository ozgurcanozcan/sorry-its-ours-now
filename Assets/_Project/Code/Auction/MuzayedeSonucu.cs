using System;
using System.Collections.Generic;

namespace Oyun.Auction
{
    /// <summary>Bir lotun akibeti.</summary>
    public sealed class LotSonucu
    {
        public Lot Lot { get; }

        /// <summary>Satildiysa kazanan alici, satilmadiysa null.</summary>
        public Alici Kazanan { get; }

        /// <summary>Odenen fiyat, TL. Satilmadiysa 0.</summary>
        public int Fiyat { get; }

        /// <summary>Satilmama sebebi. Satildiysa <see cref="SatisSonucu.Satildi"/>.</summary>
        public SatisSonucu Sonuc { get; }

        /// <summary>Bu lota teklif verebilen alici sayisi (butcesi yeten).</summary>
        /// <remarks>
        /// Denge olcumu icin: ikinci fiyat acik artirma en az iki teklif ister.
        /// Bu sayi 2'nin altina sik duserse salon fazla ince demektir.
        /// </remarks>
        public int TeklifSayisi { get; }

        /// <summary>Kazananin lota bictigi deger. Odedigi fiyattan buyuk ya da esit.</summary>
        public int KazananDegerlendirmesi { get; }

        public LotSonucu(Lot lot, Alici kazanan, int fiyat, SatisSonucu sonuc,
            int teklifSayisi, int kazananDegerlendirmesi)
        {
            if (lot == null) throw new ArgumentNullException(nameof(lot));
            if (sonuc == SatisSonucu.Satildi && kazanan == null)
                throw new ArgumentException("Satilan lotun kazanani olmali.", nameof(kazanan));
            if (sonuc != SatisSonucu.Satildi && kazanan != null)
                throw new ArgumentException("Satilmayan lotun kazanani olamaz.", nameof(kazanan));

            Lot = lot;
            Kazanan = kazanan;
            Fiyat = fiyat;
            Sonuc = sonuc;
            TeklifSayisi = teklifSayisi;
            KazananDegerlendirmesi = kazananDegerlendirmesi;
        }

        public bool Satildi { get { return Sonuc == SatisSonucu.Satildi; } }

        public override string ToString()
        {
            return Satildi
                ? Lot + " -> " + Kazanan.Ad + ", " + Fiyat + " TL"
                : Lot + " -> satilmadi (" + Sonuc + ")";
        }
    }

    public enum SatisSonucu
    {
        Satildi = 0,

        /// <summary>En yuksek teklif rezervin altinda kaldi.</summary>
        RezervinAltinda = 1,

        /// <summary>Salonda teklif verebilecek butcesi olan kimse yoktu.</summary>
        AliciYok = 2
    }

    /// <summary>Bir muzayede oturumunun tamami.</summary>
    public sealed class MuzayedeSonucu
    {
        readonly LotSonucu[] lotlar;
        readonly Alici[] gelenler;

        public MuzayedeSonucu(IReadOnlyList<Alici> gelenler, IReadOnlyList<LotSonucu> lotlar)
        {
            if (gelenler == null) throw new ArgumentNullException(nameof(gelenler));
            if (lotlar == null) throw new ArgumentNullException(nameof(lotlar));

            this.gelenler = new Alici[gelenler.Count];
            for (int i = 0; i < gelenler.Count; i++) this.gelenler[i] = gelenler[i];

            this.lotlar = new LotSonucu[lotlar.Count];
            for (int i = 0; i < lotlar.Count; i++) this.lotlar[i] = lotlar[i];
        }

        /// <summary>Salona gelen alicilar. Oyuncu bunu ancak sonuclardan sezer.</summary>
        public IReadOnlyList<Alici> Gelenler { get { return gelenler; } }

        public IReadOnlyList<LotSonucu> Lotlar { get { return lotlar; } }

        /// <summary>Oturumdan kalan toplam para, TL.</summary>
        public int ToplamHasilat
        {
            get
            {
                int toplam = 0;
                foreach (var sonuc in lotlar) toplam += sonuc.Fiyat;
                return toplam;
            }
        }

        /// <summary>Satilmayan lotlar. Depoda kaliyor, sonraki muzayedeye tekrar cikabilir.</summary>
        public IReadOnlyList<LotSonucu> SatilmayanLotlar
        {
            get
            {
                var kalanlar = new List<LotSonucu>();
                foreach (var sonuc in lotlar)
                    if (!sonuc.Satildi) kalanlar.Add(sonuc);

                return kalanlar;
            }
        }
    }
}
