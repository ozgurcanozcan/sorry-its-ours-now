using System;
using System.Collections.Generic;
using NUnit.Framework;
using Oyun.Core;
using Oyun.Economy;
using Oyun.Utils;

namespace Oyun.Testler
{
    public class DegerlemeTestleri
    {
        const ulong Tohum = 4242UL;

        static DegerAyarlari Ayarlar { get { return DegerAyarlari.Varsayilan; } }
        static Rastgele Uretici() { return Rastgele.Kok(Tohum).Dal("deger-bicme"); }

        static readonly Kondisyon[] Kondisyonlar =
        {
            Kondisyon.Hasarli, Kondisyon.Eski, Kondisyon.IyiDurumda
        };

        // --- Tasarimin can alici kurali: aralik gercegi icerir -----------------

        [Test]
        public void TahminAraligiGercekDegeriHerZamanIcerir()
        {
            // Icermeseydi tahmin yalan soylerdi ve oyuncu ona bakmamayi ogrenirdi.
            var uretici = Uretici();

            foreach (var tanim in EsyaKatalogu.Hepsi)
                foreach (var kondisyon in Kondisyonlar)
                    for (int deneme = 0; deneme < 200; deneme++)
                    {
                        var ornek = Degerleme.Uret(tanim, kondisyon, uretici, Ayarlar);

                        Assert.IsTrue(
                            ornek.TahminAraligi.Icerir(ornek.GercekDeger),
                            tanim.Id + " [" + kondisyon + "]: " + ornek.TahminAraligi +
                            " icinde " + ornek.GercekDeger + " yok.");
                    }
        }

        [Test]
        public void AraligiKonumUclarindaBileIcerir()
        {
            // -1 ve +1 sinir degerleri: gercek deger tam alt ya da tam ust sinirda.
            foreach (var tanim in EsyaKatalogu.Hepsi)
                foreach (var kondisyon in Kondisyonlar)
                {
                    int gercek = Degerleme.GercekDeger(tanim, kondisyon, Ayarlar);

                    foreach (float konum in new[] { -1.0f, -0.5f, 0.0f, 0.5f, 1.0f })
                    {
                        var aralik = Degerleme.TahminAraligi(gercek, konum, Ayarlar);
                        Assert.IsTrue(aralik.Icerir(gercek),
                            tanim.Id + " konum=" + konum + ": " + aralik + " icinde " + gercek + " yok.");
                    }
                }
        }

        // --- Orta nokta gercegi ele vermemeli ----------------------------------

        [Test]
        public void AraligiOrtasiGercekDegereEsitDegil()
        {
            // Ortalansaydi oyuncu her seferinde ortayi okuyup dogru cevabi alirdi
            // ve belirsizlik diye bir sey kalmazdi.
            var uretici = Uretici();
            var tanim = EsyaKatalogu.Bul("gardirop");

            int esitCikan = 0;
            const int Deneme = 500;
            for (int i = 0; i < Deneme; i++)
            {
                var ornek = Degerleme.Uret(tanim, Kondisyon.IyiDurumda, uretici, Ayarlar);
                if (ornek.TahminAraligi.Orta == ornek.GercekDeger) esitCikan++;
            }

            // Rastgele konum yuzunden arada bir denk gelmesi normal; kural
            // "her zaman esit degil", "hic esit olmaz" degil.
            Assert.Less(esitCikan, Deneme / 10,
                "Orta nokta cok sik gercek degere esit cikiyor: " + esitCikan + "/" + Deneme);
        }

        [Test]
        public void GercekDegerAralikIcindeDagiliyor()
        {
            // Gercek deger hep aralik ortasinda ya da hep bir uctaysa oyuncu
            // kaliba guvenmeyi ogrenir. Alt yarida da ust yarida da cikmali.
            var uretici = Uretici();
            var tanim = EsyaKatalogu.Bul("yatak");

            int altYari = 0;
            int ustYari = 0;
            for (int i = 0; i < 400; i++)
            {
                var ornek = Degerleme.Uret(tanim, Kondisyon.Eski, uretici, Ayarlar);
                if (ornek.GercekDeger < ornek.TahminAraligi.Orta) altYari++;
                else if (ornek.GercekDeger > ornek.TahminAraligi.Orta) ustYari++;
            }

            Assert.Greater(altYari, 50, "Gercek deger neredeyse hic alt yarida cikmiyor.");
            Assert.Greater(ustYari, 50, "Gercek deger neredeyse hic ust yarida cikmiyor.");
        }

        // --- Belirlenimcilik ---------------------------------------------------

        [Test]
        public void AyniTohumAyniDeger()
        {
            var tanim = EsyaKatalogu.Bul("tv");

            var a = Degerleme.Uret(tanim, Kondisyon.Eski, Uretici(), Ayarlar);
            var b = Degerleme.Uret(tanim, Kondisyon.Eski, Uretici(), Ayarlar);

            Assert.AreEqual(a.GercekDeger, b.GercekDeger);
            Assert.AreEqual(a.TahminAraligi, b.TahminAraligi);
        }

        // --- Gercek deger: taban x kondisyon -----------------------------------

        [Test]
        public void IyiDurumdaTabanDegerinKendisi()
        {
            // Taban deger calisir durumdaki ikinci el fiyati; carpani 1 olmali.
            foreach (var tanim in EsyaKatalogu.Hepsi)
                Assert.AreEqual(
                    tanim.TabanDeger,
                    Degerleme.GercekDeger(tanim, Kondisyon.IyiDurumda, Ayarlar),
                    tanim.Id);
        }

        [Test]
        public void KondisyonDustukceDegerDuser()
        {
            foreach (var tanim in EsyaKatalogu.Hepsi)
            {
                int hasarli = Degerleme.GercekDeger(tanim, Kondisyon.Hasarli, Ayarlar);
                int eski = Degerleme.GercekDeger(tanim, Kondisyon.Eski, Ayarlar);
                int iyi = Degerleme.GercekDeger(tanim, Kondisyon.IyiDurumda, Ayarlar);

                Assert.Less(hasarli, eski, tanim.Id);
                Assert.Less(eski, iyi, tanim.Id);
            }
        }

        [Test]
        public void KondisyonFarkiTahminBelirsizligindenBuyuk()
        {
            // Kondisyon GORUNUR, deger GIZLI (docs/tasarim/02-saha.md). Kademe
            // farki gurultunun altinda kalirsa gorunur olan bilgi olmaktan cikar.
            var ayarlar = Ayarlar;
            float kademeFarki = 1.0f - ayarlar.KademeCarpani;

            Assert.Greater(kademeFarki, ayarlar.YariGenislik,
                "Kademe farki (" + kademeFarki + ") tahmin yari genisliginden (" +
                ayarlar.YariGenislik + ") buyuk olmali.");
        }

        [Test]
        public void KisitiCigneyenAyarKurulamaz()
        {
            // Hafta 5'te sayi degistirilirken kisit sessizce cignenmesin.
            Assert.Throws<ArgumentException>(() => new DegerAyarlari(0.28f, 0.80f, 25));
            Assert.Throws<ArgumentException>(() => new DegerAyarlari(0.28f, 0.72f, 25));
            Assert.DoesNotThrow(() => new DegerAyarlari(0.28f, 0.70f, 25));
        }

        [Test]
        public void GecersizAyarReddedilir()
        {
            Assert.Throws<ArgumentException>(() => new DegerAyarlari(0.0f, 0.70f, 25));
            Assert.Throws<ArgumentException>(() => new DegerAyarlari(1.0f, 0.70f, 25));
            Assert.Throws<ArgumentException>(() => new DegerAyarlari(0.28f, 0.0f, 25));
            Assert.Throws<ArgumentException>(() => new DegerAyarlari(0.28f, 1.0f, 25));
            Assert.Throws<ArgumentException>(() => new DegerAyarlari(0.28f, 0.70f, 0));
        }

        // --- Kondisyon degisimi ------------------------------------------------

        [Test]
        public void KondisyonDegisinceDegerYenidenHesaplanir()
        {
            var tanim = EsyaKatalogu.Bul("buzdolabi");
            var iyi = Degerleme.Uret(tanim, Kondisyon.IyiDurumda, Uretici(), Ayarlar);

            var kirilmis = Degerleme.KondisyonuDegistir(iyi, Kondisyon.Eski, Ayarlar);

            Assert.AreEqual(Kondisyon.Eski, kirilmis.Kondisyon);
            Assert.Less(kirilmis.GercekDeger, iyi.GercekDeger);
            Assert.IsTrue(kirilmis.TahminAraligi.Icerir(kirilmis.GercekDeger));
        }

        [Test]
        public void KondisyonDegisimiYeniOlcumVermez()
        {
            // Tahmin konumu korunuyor: aksi halde esyayi kirip tamir ettirmek
            // gercek deger uzerinde ikinci bir olcum yapmak olurdu ve oyuncu
            // belirsizligi bu yolla eritebilirdi.
            //
            // Konum korunuyorsa yeni aralik eskisinin kondisyon oraniyla
            // olceklenmis hali olmali (yuvarlama payi disinda).
            var tanim = EsyaKatalogu.Bul("gardirop");
            var iyi = Degerleme.Uret(tanim, Kondisyon.IyiDurumda, Uretici(), Ayarlar);
            var eski = Degerleme.KondisyonuDegistir(iyi, Kondisyon.Eski, Ayarlar);

            float oran = Ayarlar.KademeCarpani;
            int pay = 2 * Ayarlar.YuvarlamaAdimi;

            Assert.AreEqual(iyi.TahminAraligi.Alt * oran, eski.TahminAraligi.Alt, pay);
            Assert.AreEqual(iyi.TahminAraligi.Ust * oran, eski.TahminAraligi.Ust, pay);
        }

        [Test]
        public void TamirGeriDondurur()
        {
            // Tamir bir ya da iki kademe yukari tasiyor (docs/tasarim/07-depo.md).
            var tanim = EsyaKatalogu.Bul("masa");
            var hasarli = Degerleme.Uret(tanim, Kondisyon.Hasarli, Uretici(), Ayarlar);

            var birKademe = Degerleme.KondisyonuDegistir(hasarli, Kondisyon.Eski, Ayarlar);
            var ikiKademe = Degerleme.KondisyonuDegistir(hasarli, Kondisyon.IyiDurumda, Ayarlar);

            Assert.Greater(birKademe.GercekDeger, hasarli.GercekDeger);
            Assert.Greater(ikiKademe.GercekDeger, birKademe.GercekDeger);
            Assert.AreEqual(tanim.TabanDeger, ikiKademe.GercekDeger);
        }

        // --- Yuvarlama ---------------------------------------------------------

        [Test]
        public void YuvarlamaEnUcuzEsyayiBozmuyor()
        {
            // Yuvarlama araligi genisletiyor. Ucuz esyada bu etki olculmus
            // +-%28'i fiilen buyutur. Kisit: en fazla %10 sisme.
            var ayarlar = Ayarlar;
            double enKotuSisme = 0.0;
            string enKotu = "";

            foreach (var tanim in EsyaKatalogu.Hepsi)
                foreach (var kondisyon in Kondisyonlar)
                {
                    int gercek = Degerleme.GercekDeger(tanim, kondisyon, ayarlar);

                    foreach (float konum in new[] { -1.0f, -0.5f, 0.0f, 0.5f, 1.0f })
                    {
                        var aralik = Degerleme.TahminAraligi(gercek, konum, ayarlar);

                        double merkez = gercek / (1.0 + ayarlar.YariGenislik * konum);
                        double idealGenislik = 2.0 * ayarlar.YariGenislik * merkez;
                        double sisme = (aralik.Genislik - idealGenislik) / idealGenislik;

                        if (sisme > enKotuSisme)
                        {
                            enKotuSisme = sisme;
                            enKotu = tanim.Id + " [" + kondisyon + "] konum=" + konum;
                        }
                    }
                }

            Assert.Less(enKotuSisme, 0.10,
                "Yuvarlama araligi %10'dan fazla sisiriyor. En kotu: " + enKotu +
                " -> %" + (enKotuSisme * 100.0).ToString("0.0") +
                ". Yuvarlama adimi kucultulmeli.");
        }

        [Test]
        public void GosterilenSinirlarAdimaOturuyor()
        {
            var uretici = Uretici();
            int adim = Ayarlar.YuvarlamaAdimi;

            foreach (var tanim in EsyaKatalogu.Hepsi)
            {
                var ornek = Degerleme.Uret(tanim, Kondisyon.Eski, uretici, Ayarlar);

                Assert.AreEqual(0, ornek.TahminAraligi.Alt % adim, tanim.Id + " alt");
                Assert.AreEqual(0, ornek.TahminAraligi.Ust % adim, tanim.Id + " ust");
            }
        }

        // --- Aralik tipi -------------------------------------------------------

        [Test]
        public void GecersizAralikKurulamaz()
        {
            Assert.Throws<ArgumentException>(() => new DegerAraligi(-1, 100));
            Assert.Throws<ArgumentException>(() => new DegerAraligi(200, 100));
            Assert.DoesNotThrow(() => new DegerAraligi(100, 100));
        }

        [Test]
        public void AralikIcerirSinirlariDahilSayar()
        {
            var aralik = new DegerAraligi(100, 200);

            Assert.IsTrue(aralik.Icerir(100));
            Assert.IsTrue(aralik.Icerir(200));
            Assert.IsTrue(aralik.Icerir(150));
            Assert.IsFalse(aralik.Icerir(99));
            Assert.IsFalse(aralik.Icerir(201));
        }

        [Test]
        public void GecersizGirdiReddedilir()
        {
            var uretici = Uretici();
            var tanim = EsyaKatalogu.Bul("hali");

            Assert.Throws<ArgumentNullException>(() => Degerleme.Uret(null, Kondisyon.Eski, uretici, Ayarlar));
            Assert.Throws<ArgumentNullException>(() => Degerleme.Uret(tanim, Kondisyon.Eski, null, Ayarlar));
            Assert.Throws<ArgumentNullException>(() => Degerleme.Uret(tanim, Kondisyon.Eski, uretici, null));
            Assert.Throws<ArgumentException>(() => Degerleme.TahminAraligi(100, 1.5f, Ayarlar));
            Assert.Throws<ArgumentException>(() => Degerleme.TahminAraligi(100, -1.5f, Ayarlar));
            Assert.Throws<ArgumentException>(() => Degerleme.GercekDeger(tanim, (Kondisyon)99, Ayarlar));
        }

        // --- Butun katalog uzerinde toplu saglama ------------------------------

        [Test]
        public void KatalogunTamamiUcKondisyondaSaglam()
        {
            var uretici = Uretici();
            var gorulen = new List<string>();

            foreach (var tanim in EsyaKatalogu.Hepsi)
                foreach (var kondisyon in Kondisyonlar)
                {
                    var ornek = Degerleme.Uret(tanim, kondisyon, uretici, Ayarlar);

                    Assert.Greater(ornek.GercekDeger, 0, tanim.Id);
                    Assert.IsTrue(ornek.TahminAraligi.Icerir(ornek.GercekDeger), tanim.Id);
                    Assert.Greater(ornek.TahminAraligi.Genislik, 0, tanim.Id);
                    gorulen.Add(tanim.Id + "/" + kondisyon);
                }

            Assert.AreEqual(EsyaKatalogu.Hepsi.Count * 3, gorulen.Count);
        }
    }
}
