using System;
using System.Collections.Generic;
using Oyun.Core;

namespace Oyun.Auction
{
    // Muzayedeye gelen bir alici. Degismez tanim.
    //
    // Oyuncu havuzu GORMEZ (docs/tasarim/03-muzayede.md) -- kim olduklarini,
    // neye ne verdiklerini oynayarak sezer. O yuzden buradaki hicbir alan
    // dogrudan arayuze basilmaz; "huy" bile ancak dolayli ipucu olarak
    // kullanilabilir.
    public sealed class Alici
    {
        /// <summary>Kalici kimlik. Kayit dosyasi bu metne baglanir.</summary>
        public string Id { get; }

        public string Ad { get; }

        /// <summary>Tek cumlelik huy. Tasarim belgesi icin; arayuze basilmaz.</summary>
        public string Huy { get; }

        /// <summary>
        /// Oturum boyunca harcayabilecegi toplam para, TL.
        /// </summary>
        /// <remarks>
        /// Butce TUKENMELI (docs/tasarim/03-muzayede.md, olculmus bulgu):
        /// tukenmezse her lot bagimsiz hesaplanir ve alici cesitliligi
        /// anlamsizlasir -- denemede 3.000 TL butceli komsu 6.000 TL'lik
        /// alisveris yapabiliyordu. Tukendiginde "neyi once satiyorum"
        /// karari doguyor.
        /// </remarks>
        public int Butce { get; }

        /// <summary>Ilgilenmedigi kategoriye uyguladigi carpan.</summary>
        /// <remarks>
        /// Sifir degil: herkes her seye bir seyler veriyor, sadece
        /// ilgilenmedigine az. Bu sayede lot alicisiz kalmiyor, ucuza gidiyor.
        /// </remarks>
        public float VarsayilanCarpan { get; }

        /// <summary>
        /// 1-10: havuzdan cekilirken agirligi. Esya bollugu ile ayni olcek.
        /// </summary>
        /// <remarks>
        /// Mutlak bir olasilik degil, goreli agirlik: her muzayedeye sabit
        /// sayida alici geliyor ve secim bu agirliga gore yapiliyor.
        /// Oyuncu bunu gormez; "bu hafta Koleksiyoncu var mi" gerilimi
        /// buradan doguyor (docs/tasarim/03-muzayede.md).
        /// </remarks>
        public int KatilimAgirligi { get; }

        public const int EnDusukKatilim = 1;
        public const int EnYuksekKatilim = 10;

        // Kategori basina carpan. Yalnizca varsayilandan farkli olanlar dolu;
        // gerisi VarsayilanCarpan'a dusuyor.
        readonly Dictionary<Kategori, float> primler;

        public Alici(
            string id,
            string ad,
            string huy,
            int butce,
            float varsayilanCarpan,
            int katilimAgirligi,
            IReadOnlyDictionary<Kategori, float> kategoriPrimleri = null)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Alici id'si bos olamaz.", nameof(id));
            if (string.IsNullOrWhiteSpace(ad))
                throw new ArgumentException("Alici adi bos olamaz. (id: " + id + ")", nameof(ad));
            if (string.IsNullOrWhiteSpace(huy))
                throw new ArgumentException("Alici huyu bos olamaz. (id: " + id + ")", nameof(huy));
            if (butce <= 0)
                throw new ArgumentException("Butce pozitif olmali. (id: " + id + ")", nameof(butce));
            if (!(varsayilanCarpan > 0.0f))
                throw new ArgumentException(
                    "Varsayilan carpan pozitif olmali -- sifir carpan aliciyi lotta " +
                    "gorunmez yapar. (id: " + id + ")", nameof(varsayilanCarpan));
            if (katilimAgirligi < EnDusukKatilim || katilimAgirligi > EnYuksekKatilim)
                throw new ArgumentException(
                    "Katilim agirligi " + EnDusukKatilim + "-" + EnYuksekKatilim +
                    " arasinda olmali. (id: " + id + ")", nameof(katilimAgirligi));

            primler = new Dictionary<Kategori, float>();
            if (kategoriPrimleri != null)
            {
                foreach (var giris in kategoriPrimleri)
                {
                    if (!Enum.IsDefined(typeof(Kategori), giris.Key))
                        throw new ArgumentException(
                            "Tanimsiz kategori primi. (id: " + id + ")", nameof(kategoriPrimleri));
                    if (!(giris.Value > 0.0f))
                        throw new ArgumentException(
                            "Kategori primi pozitif olmali. (id: " + id + ", " + giris.Key + ")",
                            nameof(kategoriPrimleri));

                    primler.Add(giris.Key, giris.Value);
                }
            }

            Id = id;
            Ad = ad;
            Huy = huy;
            Butce = butce;
            VarsayilanCarpan = varsayilanCarpan;
            KatilimAgirligi = katilimAgirligi;
        }

        /// <summary>Bu kategoriye uyguladigi carpan.</summary>
        public float Carpan(Kategori kategori)
        {
            float prim;
            return primler.TryGetValue(kategori, out prim) ? prim : VarsayilanCarpan;
        }

        /// <summary>Varsayilandan farkli carpani olan kategoriler. Testler ve belge icin.</summary>
        public IReadOnlyDictionary<Kategori, float> Primler { get { return primler; } }

        public override string ToString()
        {
            return Ad + " (" + Id + ", " + Butce + " TL)";
        }
    }
}
