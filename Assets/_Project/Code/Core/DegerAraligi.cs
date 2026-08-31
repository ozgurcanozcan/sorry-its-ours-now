using System;

namespace Oyun.Core
{
    // Oyuncuya gosterilen tahmin araligi: "~400-1.200 TL".
    //
    // Gercek deger gizli, oyuncu bu araligi gorur (docs/tasarim/02-saha.md).
    // Aralik gercek degeri ICERMEK zorunda -- icermeseydi tahmin yalan soylerdi
    // ve oyuncu ona hic guvenmemeyi ogrenirdi. Kuralin sinanmasi icin Icerir var.
    public readonly struct DegerAraligi : IEquatable<DegerAraligi>
    {
        public readonly int Alt;
        public readonly int Ust;

        public DegerAraligi(int alt, int ust)
        {
            if (alt < 0)
                throw new ArgumentException("Aralik alt siniri negatif olamaz.", nameof(alt));
            if (ust < alt)
                throw new ArgumentException("Aralik ust siniri alt sinirdan kucuk olamaz.", nameof(ust));

            Alt = alt;
            Ust = ust;
        }

        public int Genislik { get { return Ust - Alt; } }

        /// <summary>Aralik orta noktasi. Bilerek gercek degere esit DEGIL -- bkz. Degerleme.</summary>
        public int Orta { get { return Alt + (Ust - Alt) / 2; } }

        public bool Icerir(int deger)
        {
            return deger >= Alt && deger <= Ust;
        }

        public bool Equals(DegerAraligi digeri)
        {
            return Alt == digeri.Alt && Ust == digeri.Ust;
        }

        public override bool Equals(object nesne)
        {
            return nesne is DegerAraligi digeri && Equals(digeri);
        }

        public override int GetHashCode()
        {
            unchecked { return (Alt * 397) ^ Ust; }
        }

        public override string ToString()
        {
            return "~" + Alt + "-" + Ust + " TL";
        }
    }
}
