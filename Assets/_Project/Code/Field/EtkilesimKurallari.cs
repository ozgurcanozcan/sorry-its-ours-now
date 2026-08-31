using Oyun.Core;

namespace Oyun.Field
{
    /// <summary>Bir esyanin elde alinip alinamayacaginin sonucu.</summary>
    /// <remarks>
    /// Bool yerine sebep donuyor: oyuncuya "alamazsin" demek yetmiyor, NEDEN
    /// alamadigini soylemek gerekiyor. Sabit esya matkap ister, agir esya el
    /// arabasi -- ikisi de sonraki gunlerin isi ama sebep bugunden ayrilmali,
    /// yoksa arayuz sonradan tahmin etmek zorunda kalir.
    /// </remarks>
    public enum AlmaSonucu
    {
        Alinabilir = 0,
        ElDolu = 1,
        Sabit = 2,
        IkiKisiGerekir = 3
    }

    // Alma kurallari. MonoBehaviour disinda: sahne, kamera ve fizik
    // gerektirmiyor, EditMode testinden dogrudan cagrilabiliyor.
    public static class EtkilesimKurallari
    {
        /// <summary>Bu esya su anda elde alinabilir mi, alinamazsa neden?</summary>
        /// <remarks>
        /// Sira onemli. Once elin dolu mu diye bakiliyor cunku bu oyuncunun
        /// degistirebilecegi tek durum; "sabit" ya da "iki kisi gerekir" ise
        /// esyanin kendi ozelligi ve oyuncu o an hicbir sey yapamaz. Once
        /// duzeltilebilir olani soylemek daha faydali bir geri bildirim.
        /// </remarks>
        public static AlmaSonucu Degerlendir(EsyaVerisi veri, bool elDolu)
        {
            if (veri == null)
            {
                return AlmaSonucu.Sabit;
            }

            if (elDolu)
            {
                return AlmaSonucu.ElDolu;
            }

            // docs/tasarim/02-saha.md: sabit esya levye/matkap ister.
            if (veri.Sabit)
            {
                return AlmaSonucu.Sabit;
            }

            // Tek kisilik modda el arabasi gerekiyor (karar: 02-saha.md
            // "Agir esya: el arabasi"). El arabasi Hafta 2'de geliyor.
            if (veri.IkiKisiGerekir)
            {
                return AlmaSonucu.IkiKisiGerekir;
            }

            return AlmaSonucu.Alinabilir;
        }

        /// <summary>Oyuncuya gosterilecek kisa sebep metni.</summary>
        public static string Aciklama(AlmaSonucu sonuc)
        {
            switch (sonuc)
            {
                case AlmaSonucu.Alinabilir: return "";
                case AlmaSonucu.ElDolu: return "Elin dolu";
                case AlmaSonucu.Sabit: return "Sabit -- alet gerekiyor";
                case AlmaSonucu.IkiKisiGerekir: return "Tek basina kaldiramazsin";
                default: return "";
            }
        }
    }
}
