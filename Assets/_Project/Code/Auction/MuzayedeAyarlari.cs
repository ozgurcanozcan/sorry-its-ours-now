using System;

namespace Oyun.Auction
{
    // Muzayedenin ayar sayilari.
    public sealed class MuzayedeAyarlari
    {
        /// <summary>
        /// Her muzayedeye havuzdan kac alici gelir. M1'de 4 (karar: 01.09.2026).
        /// </summary>
        /// <remarks>
        /// Onemli olan sayi degil ORAN. Denemede 24 havuz / 8 gelen calisti --
        /// havuzun ucte biri salondaydi ve "bu hafta Antikaci var mi" gerilimi
        /// oradan dogdu (docs/tasarim/03-muzayede.md).
        ///
        /// M1'e once 12 havuz / 8 gelen yazilmisti; o oran ucte iki, yani havuz
        /// neredeyse her hafta salonda. 12/4 olculmus orani geri getiriyor.
        /// </remarks>
        public const int VarsayilanGelenSayisi = 4;

        /// <summary>
        /// Denemede olculen gelen/havuz orani. Kisit olarak kullaniliyor.
        /// </summary>
        public const float OlculmusKatilimOrani = 1.0f / 3.0f;

        /// <summary>
        /// Kazananin ikinci teklifin uzerine odedigi fark, TL. OLCULMEDI.
        /// </summary>
        /// <remarks>
        /// Ikinci fiyat acik artirmada kazanan, ikinci teklifin "biraz" ustunu
        /// oder (docs/tasarim/03-muzayede.md). Bu "biraz"in buyuklugu oyunu
        /// degistirmiyor -- sadece sayinin okunabilir olmasi icin var.
        /// </remarks>
        public const int VarsayilanTeklifArtisi = 50;

        public int GelenSayisi { get; }
        public int TeklifArtisi { get; }

        public static MuzayedeAyarlari Varsayilan { get; } =
            new MuzayedeAyarlari(VarsayilanGelenSayisi, VarsayilanTeklifArtisi);

        public MuzayedeAyarlari(int gelenSayisi, int teklifArtisi)
        {
            if (gelenSayisi < 2)
                throw new ArgumentException(
                    "En az 2 alici gelmeli: ikinci fiyat acik artirmanin anlamli olmasi " +
                    "icin ikinci bir teklif gerekiyor.", nameof(gelenSayisi));
            if (gelenSayisi > AliciKatalogu.Hepsi.Count)
                throw new ArgumentException(
                    "Havuzdan fazla alici gelemez.", nameof(gelenSayisi));
            if (teklifArtisi < 0)
                throw new ArgumentException("Teklif artisi negatif olamaz.", nameof(teklifArtisi));

            GelenSayisi = gelenSayisi;
            TeklifArtisi = teklifArtisi;
        }

        /// <summary>Bu ayarla gercek katilim orani. Kisit testinin baktigi sayi.</summary>
        public float KatilimOrani
        {
            get { return GelenSayisi / (float)AliciKatalogu.Hepsi.Count; }
        }
    }
}
