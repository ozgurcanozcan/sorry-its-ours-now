using System;
using Oyun.Core;

namespace Oyun.Economy
{
    // Deger bicmenin ayar sayilari. Ikisinin kaynagi farkli, karistirilmamali.
    public sealed class DegerAyarlari
    {
        /// <summary>
        /// Tahmin araliginin yari genisligi. OLCULDU: docs/tasarim/03-muzayede.md.
        /// </summary>
        /// <remarks>
        /// Antikada +-%55 aralik karari kumara ceviriyordu; +-%28'e inince karar
        /// geri geldi. Deneme surumunden cikan olcum, tahmin degil.
        /// </remarks>
        public const float OlculmusYariGenislik = 0.28f;

        /// <summary>
        /// Her kondisyon kademesi bir ustunun bu kadari. OLCULMEDI -- Hafta 5 denge isi.
        /// </summary>
        /// <remarks>
        /// Uc ayri carpan yerine tek oran yazildi: olculecek sayi ucten bire dustu.
        ///
        /// Saglamasi gereken kisit (docs/plan.md Hafta 5'te olculecek):
        ///   1 - KademeCarpani > OlculmusYariGenislik
        ///
        /// Yani bir kademe dusmenin degere etkisi, tahmin belirsizliginden buyuk
        /// olmali. Aksi halde GORUNUR olan kondisyon, GIZLI olan gurultunun altinda
        /// kalir; oysa docs/tasarim/02-saha.md "kondisyon gorunur, deger gorunmez"
        /// ayrimini oyuncunun elindeki tek bilgi olarak kuruyor.
        /// </remarks>
        public const float VarsayilanKademeCarpani = 0.70f;

        /// <summary>Gosterilen sinirlarin yuvarlandigi adim. Gorsel, denge sayisi degil.</summary>
        /// <remarks>
        /// "~400-1.200 TL" okunur, "~412-1.187 TL" okunmaz. Alt asagi, ust yukari
        /// yuvarlaniyor -- yakina yuvarlamak gercek degeri araligin disina itebilirdi.
        ///
        /// Ama yuvarlama araligi GENISLETIYOR, yani belirsizligi artiriyor. Ucuz
        /// esyada bu etki buyuk: 50'lik adim en ucuz esyanin araligini %20'ye
        /// varan oranda sisiriyordu, yani olculmus +-%28 fiilen +-%34 gibi
        /// davraniyordu. 25'te bu %10'un altina iniyor.
        ///
        /// Saglamasi gereken kisit (DegerlemeTestleri koruyor): yuvarlama,
        /// katalogdaki en ucuz esyanin en dar araligini %10'dan fazla
        /// genisletmemeli. Kataloga cok ucuz bir esya (Hurda) girerse test kirilir
        /// ve adim kucultulur.
        /// </remarks>
        public const int VarsayilanYuvarlamaAdimi = 25;

        public float YariGenislik { get; }
        public float KademeCarpani { get; }
        public int YuvarlamaAdimi { get; }

        public static DegerAyarlari Varsayilan { get; } = new DegerAyarlari(
            OlculmusYariGenislik, VarsayilanKademeCarpani, VarsayilanYuvarlamaAdimi);

        public DegerAyarlari(float yariGenislik, float kademeCarpani, int yuvarlamaAdimi)
        {
            if (yariGenislik <= 0.0f || yariGenislik >= 1.0f)
                throw new ArgumentException("Yari genislik 0 ile 1 arasinda olmali.", nameof(yariGenislik));
            if (kademeCarpani <= 0.0f || kademeCarpani >= 1.0f)
                throw new ArgumentException("Kademe carpani 0 ile 1 arasinda olmali.", nameof(kademeCarpani));
            if (yuvarlamaAdimi < 1)
                throw new ArgumentException("Yuvarlama adimi en az 1 olmali.", nameof(yuvarlamaAdimi));

            // Kisit kodda duruyor ki Hafta 5'te sayi degistirilirken sessizce
            // cignenmesin: kondisyon gorunur bilgi, tahmin gurultusu gizli.
            // Gurultu buyurse gorunur olan anlamsizlasir.
            if (1.0f - kademeCarpani <= yariGenislik)
                throw new ArgumentException(
                    "Kademe farki tahmin belirsizliginden buyuk olmali: 1 - " + kademeCarpani +
                    " = " + (1.0f - kademeCarpani) + " <= " + yariGenislik,
                    nameof(kademeCarpani));

            YariGenislik = yariGenislik;
            KademeCarpani = kademeCarpani;
            YuvarlamaAdimi = yuvarlamaAdimi;
        }

        /// <summary>Kondisyonun taban degere uygulanan carpani. Iyi durumda = 1.</summary>
        /// <remarks>
        /// Taban deger calisir durumdaki ikinci el fiyati (docs/uretim/esyalar.md),
        /// yani "iyi durumda" olcumun kendisi; carpani 1 olmak zorunda.
        /// </remarks>
        public float KondisyonCarpani(Kondisyon kondisyon)
        {
            switch (kondisyon)
            {
                case Kondisyon.IyiDurumda: return 1.0f;
                case Kondisyon.Eski: return KademeCarpani;
                case Kondisyon.Hasarli: return KademeCarpani * KademeCarpani;
                default:
                    throw new ArgumentException("Tanimsiz kondisyon: " + kondisyon, nameof(kondisyon));
            }
        }
    }
}
