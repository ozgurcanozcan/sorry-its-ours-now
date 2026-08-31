using System;
using Oyun.Core;
using Oyun.Utils;

namespace Oyun.Economy
{
    // Deger bicme: gizli gercek deger + oyuncuya gosterilen tahmin araligi.
    //
    // ARALIK NEDEN GERCEGIN ETRAFINDA ORTALANMIYOR
    // Ortalansaydi orta nokta gercek degerin ta kendisi olurdu ve oyuncu her
    // seferinde ortalamayi okuyup dogru cevabi alirdi -- belirsizlik olurdu.
    // Bunun yerine gercek deger araligin icinde RASTGELE bir yere dusuyor.
    //
    // ARALIK NEDEN GERCEGI ICERMEK ZORUNDA
    // Icermeseydi tahmin yalan soylerdi. Oyuncu birkac kez yanildiktan sonra
    // araliga hic bakmamayi ogrenir ve mekanik olur. Yani: merkez bilgi
    // vermemeli ama sinirlar dogru olmali.
    //
    // BELIRSIZLIGIN ASIL KAYNAGI BU DEGIL
    // docs/tasarim/03-muzayede.md: "belirsizligin asil kaynagi fiyat tahmini
    // degil, salonda kimin oldugu." O yuzden burada ornek basina ek bir deger
    // sapmasi YOK -- gercek deger yalnizca taban deger x kondisyon. Uydurulacak
    // bir sayi daha eklemek olurdu.
    public static class Degerleme
    {
        /// <summary>Bir esya turunden dunyaya konacak tek bir ornek uretir.</summary>
        /// <remarks>
        /// Uretici tohumlu olmali ve kendi dalindan gelmeli, ornegin
        /// <c>kok.Dal("deger-bicme")</c> -- boylece ev uretimine yeni bir
        /// rastgelelik eklemek eski tohumlarin degerlerini kaydirmaz.
        /// </remarks>
        public static EsyaOrnegi Uret(
            EsyaVerisi tanim, Kondisyon kondisyon, Rastgele uretici, DegerAyarlari ayarlar)
        {
            if (tanim == null) throw new ArgumentNullException(nameof(tanim));
            if (uretici == null) throw new ArgumentNullException(nameof(uretici));
            if (ayarlar == null) throw new ArgumentNullException(nameof(ayarlar));

            float konum = uretici.Kesir(-1.0f, 1.0f);
            return Kur(tanim, kondisyon, konum, ayarlar);
        }

        /// <summary>Kondisyonu degismis yeni bir ornek. Kirilma ve tamir bunu kullanir.</summary>
        /// <remarks>
        /// Tahmin konumu KORUNUYOR. Yeniden cekilseydi, esyayi kirip tamir
        /// ettirmek gercek deger uzerinde ikinci bir olcum yapmak anlamina
        /// gelirdi; oyuncu bunu istismar ederek belirsizligi eritebilirdi.
        /// </remarks>
        public static EsyaOrnegi KondisyonuDegistir(
            EsyaOrnegi ornek, Kondisyon yeniKondisyon, DegerAyarlari ayarlar)
        {
            if (ornek == null) throw new ArgumentNullException(nameof(ornek));
            if (ayarlar == null) throw new ArgumentNullException(nameof(ayarlar));

            return Kur(ornek.Tanim, yeniKondisyon, ornek.TahminKonumu, ayarlar);
        }

        /// <summary>Gizli gercek deger: taban deger x kondisyon carpani.</summary>
        public static int GercekDeger(EsyaVerisi tanim, Kondisyon kondisyon, DegerAyarlari ayarlar)
        {
            if (tanim == null) throw new ArgumentNullException(nameof(tanim));
            if (ayarlar == null) throw new ArgumentNullException(nameof(ayarlar));

            double deger = tanim.TabanDeger * (double)ayarlar.KondisyonCarpani(kondisyon);

            // AwayFromZero: .NET varsayilani cifte yuvarliyor ve 7250 gibi
            // degerlerde beklenmedik yonde kayiyor.
            return (int)Math.Round(deger, MidpointRounding.AwayFromZero);
        }

        /// <summary>Gercek degeri iceren, ortasi gercegi ele vermeyen aralik.</summary>
        /// <param name="konum">Gercek degerin aralik icindeki goreli yeri, -1 ile 1 arasi.</param>
        public static DegerAraligi TahminAraligi(int gercekDeger, float konum, DegerAyarlari ayarlar)
        {
            if (gercekDeger < 0)
                throw new ArgumentException("Gercek deger negatif olamaz.", nameof(gercekDeger));
            if (ayarlar == null) throw new ArgumentNullException(nameof(ayarlar));
            if (konum < -1.0f || konum > 1.0f)
                throw new ArgumentException("Konum -1 ile 1 arasinda olmali.", nameof(konum));

            double yari = ayarlar.YariGenislik;

            // gercekDeger = merkez * (1 + yari * konum) olacak sekilde merkezi
            // geri cozuyoruz. konum -1..1 oldugu icin gercek deger [merkez*(1-yari),
            // merkez*(1+yari)] araliginin icinde kalmasi garanti.
            double merkez = gercekDeger / (1.0 + yari * konum);

            // Alt asagi, ust yukari: yakina yuvarlamak siniri gercek degerin
            // yanlis tarafina gecirip araligi yalanci yapabilirdi.
            int alt = AsagiYuvarla(merkez * (1.0 - yari), ayarlar.YuvarlamaAdimi);
            int ust = YukariYuvarla(merkez * (1.0 + yari), ayarlar.YuvarlamaAdimi);

            return new DegerAraligi(alt, ust);
        }

        static EsyaOrnegi Kur(
            EsyaVerisi tanim, Kondisyon kondisyon, float konum, DegerAyarlari ayarlar)
        {
            int gercek = GercekDeger(tanim, kondisyon, ayarlar);
            return new EsyaOrnegi(tanim, kondisyon, gercek, TahminAraligi(gercek, konum, ayarlar), konum);
        }

        static int AsagiYuvarla(double deger, int adim)
        {
            if (deger <= 0.0) return 0;
            return (int)(Math.Floor(deger / adim) * adim);
        }

        static int YukariYuvarla(double deger, int adim)
        {
            if (deger <= 0.0) return 0;
            return (int)(Math.Ceiling(deger / adim) * adim);
        }
    }
}
