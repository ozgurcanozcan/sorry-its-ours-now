using System;

namespace Oyun.Core
{
    // Dunyadaki TEK BIR esya. EsyaVerisi turun tanimi, bu ise o turden bir ornek:
    // kendi kondisyonu, kendi gercek degeri, kendi tahmin araligi var.
    //
    // Degismez. Kondisyon degisince (kirilma, tamir) yeni bir ornek uretilir --
    // mimari "dogruluk tek yerde: OyunDurumu, komutlar donusturur" diyor
    // (.cursorrules), degistirilebilir nesne o modelde kacak nokta oluyor.
    //
    // Uretimi Oyun.Economy.Degerleme yapar; buradaki yapici ona acik.
    public sealed class EsyaOrnegi
    {
        public EsyaVerisi Tanim { get; }
        public Kondisyon Kondisyon { get; }

        /// <summary>Oyuncunun GORMEDIGI gercek deger. UI'ye verilmez.</summary>
        public int GercekDeger { get; }

        /// <summary>Oyuncunun gordugu tahmin. GercekDeger'i icerir ama ortasi o degildir.</summary>
        public DegerAraligi TahminAraligi { get; }

        // Gercek degerin aralik icindeki goreli yeri, -1 ile 1 arasi.
        //
        // Neden ornege sabit: her sorguda yeniden cekilseydi oyuncu esyaya defalarca
        // bakip araliklarin ortalamasini alarak gercek degeri bulurdu. Belirsizlik
        // ancak tek bir cekiliste kalirsa belirsizlik.
        //
        // Kondisyon degisince de AYNI kaliyor: yeni bir konum cekmek, kirilan bir
        // esyaya bakarak ikinci bir olcum yapmak anlamina gelirdi.
        internal float TahminKonumu { get; }

        internal EsyaOrnegi(
            EsyaVerisi tanim,
            Kondisyon kondisyon,
            int gercekDeger,
            DegerAraligi tahminAraligi,
            float tahminKonumu)
        {
            if (tanim == null) throw new ArgumentNullException(nameof(tanim));
            if (!Enum.IsDefined(typeof(Kondisyon), kondisyon))
                throw new ArgumentException("Tanimsiz kondisyon.", nameof(kondisyon));
            if (gercekDeger < 0)
                throw new ArgumentException("Gercek deger negatif olamaz.", nameof(gercekDeger));

            // Tasarimin can alici kurali: gosterilen aralik gercegi icermek zorunda.
            // Icermezse tahmin yalan soyler ve oyuncu ona guvenmemeyi ogrenir.
            if (!tahminAraligi.Icerir(gercekDeger))
                throw new ArgumentException(
                    "Tahmin araligi gercek degeri icermiyor: " + tahminAraligi + " icinde " + gercekDeger,
                    nameof(tahminAraligi));

            Tanim = tanim;
            Kondisyon = kondisyon;
            GercekDeger = gercekDeger;
            TahminAraligi = tahminAraligi;
            TahminKonumu = tahminKonumu;
        }

        public override string ToString()
        {
            return Tanim.Ad + " [" + Kondisyon + "] " + TahminAraligi;
        }
    }
}
