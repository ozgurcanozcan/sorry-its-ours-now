using System;
using System.Collections.Generic;
using Oyun.Core;

namespace Oyun.Auction
{
    // Muzayedeye cikan tek bir kalem: bir ya da birkac esya, bir rezerv fiyati.
    //
    // REZERV NE ISE YARAR (docs/tasarim/03-muzayede.md, olculmus bulgu)
    // Ikinci fiyat acik artirmada rezerv sana yuksek fiyat KAZANDIRMAZ,
    // sadece ucuza gitmeni engeller. Yuksek rezerv = satmama riski,
    // dusuk rezerv = yakma riski.
    //
    // Satilmayan lot kayboLmuyor, depoda kaliyor ve sonraki muzayedeye tekrar
    // cikarilabiliyor (karar 31.08.2026) -- rezervin anlamli olmasi buna bagli.
    public sealed class Lot
    {
        readonly EsyaOrnegi[] esyalar;

        /// <summary>Altina satilmayacagi fiyat, TL. 0 = rezerv yok.</summary>
        public int Rezerv { get; }

        public Lot(IReadOnlyList<EsyaOrnegi> esyalar, int rezerv)
        {
            if (esyalar == null) throw new ArgumentNullException(nameof(esyalar));
            if (esyalar.Count == 0)
                throw new ArgumentException("Bos lot cikarilamaz.", nameof(esyalar));
            if (rezerv < 0)
                throw new ArgumentException("Rezerv negatif olamaz.", nameof(rezerv));

            this.esyalar = new EsyaOrnegi[esyalar.Count];
            for (int i = 0; i < esyalar.Count; i++)
            {
                if (esyalar[i] == null)
                    throw new ArgumentException("Lotta bos esya var.", nameof(esyalar));

                this.esyalar[i] = esyalar[i];
            }

            Rezerv = rezerv;
        }

        public Lot(EsyaOrnegi esya, int rezerv) : this(new[] { esya }, rezerv) { }

        public IReadOnlyList<EsyaOrnegi> Esyalar { get { return esyalar; } }

        /// <summary>Icindeki esyalarin gizli gercek degerlerinin toplami.</summary>
        /// <remarks>Oyuncu bunu gormez; alici tekliflerinin dayandigi sayi bu.</remarks>
        public int GercekDeger
        {
            get
            {
                int toplam = 0;
                foreach (var esya in esyalar) toplam += esya.GercekDeger;
                return toplam;
            }
        }

        public override string ToString()
        {
            return esyalar.Length == 1
                ? esyalar[0].Tanim.Ad + " (rezerv " + Rezerv + ")"
                : esyalar.Length + " parca lot (rezerv " + Rezerv + ")";
        }
    }
}
