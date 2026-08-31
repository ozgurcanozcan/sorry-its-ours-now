using System;
using System.Collections.Generic;
using NUnit.Framework;
using Oyun.Utils;

namespace Oyun.Testler
{
    public class RastgeleTestleri
    {
        const ulong Tohum = 12345UL;

        // --- Belirlenimcilik ---------------------------------------------------

        [Test]
        public void AyniTohumAyniDiziyiVerir()
        {
            var a = Rastgele.Kok(Tohum);
            var b = Rastgele.Kok(Tohum);

            for (int i = 0; i < 1000; i++)
                Assert.AreEqual(a.Sonraki(), b.Sonraki(), "Adim " + i);
        }

        [Test]
        public void FarkliTohumFarkliDiziVerir()
        {
            var a = Rastgele.Kok(Tohum);
            var b = Rastgele.Kok(Tohum + 1UL);

            bool farkVar = false;
            for (int i = 0; i < 20 && !farkVar; i++)
                farkVar = a.Sonraki() != b.Sonraki();

            Assert.IsTrue(farkVar, "Komsu iki tohum 20 adim boyunca ayni cikti verdi.");
        }

        // Algoritmayi kilitler. Bu test kirilirsa kayitli tohumlar ve elle
        // ayarlanmis kampanya gorevleri baska dunya uretmeye baslar; degisiklik
        // bilincli ise docs/kararlar/ altina yazilmali.
        [Test]
        public void CiktiSurumlerArasindaSabit()
        {
            var uretici = Rastgele.Kok(Tohum);
            uint[] beklenen = { 2280515124u, 875822104u, 2165132003u, 3444695176u, 1217744654u };

            for (int i = 0; i < beklenen.Length; i++)
                Assert.AreEqual(beklenen[i], uretici.Sonraki(), "Adim " + i);
        }

        [Test]
        public void DalCiktisiSurumlerArasindaSabit()
        {
            var dal = Rastgele.Kok(Tohum).Dal("ev-uretimi");
            uint[] beklenen = { 980020527u, 4117939971u, 3901149799u };

            for (int i = 0; i < beklenen.Length; i++)
                Assert.AreEqual(beklenen[i], dal.Sonraki(), "Adim " + i);
        }

        // --- Dallar ------------------------------------------------------------

        [Test]
        public void DalAcilmaSirasindanEtkilenmez()
        {
            var kok1 = Rastgele.Kok(Tohum);
            var ilkSirada = kok1.Dal("muzayede");

            var kok2 = Rastgele.Kok(Tohum);
            kok2.Dal("ev-uretimi");
            kok2.Dal("hava-durumu");
            var sonSirada = kok2.Dal("muzayede");

            for (int i = 0; i < 100; i++)
                Assert.AreEqual(ilkSirada.Sonraki(), sonSirada.Sonraki(), "Adim " + i);
        }

        [Test]
        public void DalKokUreticiyiTuketmez()
        {
            var kok = Rastgele.Kok(Tohum);
            uint dalsiz = Rastgele.Kok(Tohum).Sonraki();

            kok.Dal("ev-uretimi");
            kok.Dal("muzayede");

            Assert.AreEqual(dalsiz, kok.Sonraki());
        }

        [Test]
        public void FarkliDalAdlariFarkliAkis()
        {
            var a = Rastgele.Kok(Tohum).Dal("ev-uretimi");
            var b = Rastgele.Kok(Tohum).Dal("muzayede");

            bool farkVar = false;
            for (int i = 0; i < 20 && !farkVar; i++)
                farkVar = a.Sonraki() != b.Sonraki();

            Assert.IsTrue(farkVar);
        }

        [Test]
        public void BosDalAdiHataVerir()
        {
            var kok = Rastgele.Kok(Tohum);
            Assert.Throws<ArgumentException>(() => kok.Dal(""));
            Assert.Throws<ArgumentException>(() => kok.Dal(null));
        }

        // --- Aralik ------------------------------------------------------------

        [Test]
        public void AralikSinirlarIcindeKalir()
        {
            var uretici = Rastgele.Kok(Tohum);
            for (int i = 0; i < 10000; i++)
            {
                int deger = uretici.Aralik(-5, 7);
                Assert.GreaterOrEqual(deger, -5);
                Assert.Less(deger, 7);
            }
        }

        [Test]
        public void AralikTumDegerleriUretir()
        {
            var uretici = Rastgele.Kok(Tohum);
            var gorulen = new HashSet<int>();
            for (int i = 0; i < 5000; i++)
                gorulen.Add(uretici.Aralik(0, 12));

            Assert.AreEqual(12, gorulen.Count, "12 kisilik havuzda ulasilamayan indeks var.");
        }

        [Test]
        public void AralikTekDegerliIseHepAyniSonuc()
        {
            var uretici = Rastgele.Kok(Tohum);
            for (int i = 0; i < 100; i++)
                Assert.AreEqual(3, uretici.Aralik(3, 4));
        }

        [Test]
        public void AralikGecersizSinirHataVerir()
        {
            var uretici = Rastgele.Kok(Tohum);
            Assert.Throws<ArgumentException>(() => uretici.Aralik(5, 5));
            Assert.Throws<ArgumentException>(() => uretici.Aralik(5, 4));
        }

        // 12 kisilik alici havuzu ikinin kuvveti degil; "% 12" kullanilsa kucuk
        // indeksler sistematik olarak fazla cikardi. Yanliligi olcen sinir bu.
        [Test]
        public void AralikYanliligiOlculebilirDegil()
        {
            const int Kova = 12;
            const int Cekilis = 240000;
            const int Beklenen = Cekilis / Kova;

            var uretici = Rastgele.Kok(Tohum);
            var sayac = new int[Kova];
            for (int i = 0; i < Cekilis; i++)
                sayac[uretici.Aralik(Kova)]++;

            // Ki-kare, 11 serbestlik derecesi, p=0.001 -> 31.26. Yansiz bir
            // uretici bu esigi binde bir asar; testin sallanma payi bu.
            double kiKare = 0.0;
            for (int i = 0; i < Kova; i++)
            {
                double fark = sayac[i] - Beklenen;
                kiKare += fark * fark / Beklenen;
            }

            Assert.Less(kiKare, 31.26, "Dagilim duz degil, ki-kare = " + kiKare);
        }

        // --- Kesir ve olasilik -------------------------------------------------

        [Test]
        public void KesirSifirIleBirArasinda()
        {
            var uretici = Rastgele.Kok(Tohum);
            for (int i = 0; i < 10000; i++)
            {
                float deger = uretici.Kesir();
                Assert.GreaterOrEqual(deger, 0.0f);
                Assert.Less(deger, 1.0f);
            }
        }

        [Test]
        public void KesirAraligiSinirlarIcinde()
        {
            var uretici = Rastgele.Kok(Tohum);
            for (int i = 0; i < 10000; i++)
            {
                float deger = uretici.Kesir(-1.5f, 2.5f);
                Assert.GreaterOrEqual(deger, -1.5f);
                Assert.Less(deger, 2.5f);
            }
        }

        [Test]
        public void OlasilikUcNoktalariKesin()
        {
            var uretici = Rastgele.Kok(Tohum);
            for (int i = 0; i < 100; i++)
            {
                Assert.IsFalse(uretici.Olasilik(0.0f));
                Assert.IsTrue(uretici.Olasilik(1.0f));
            }
        }

        [Test]
        public void OlasilikVerilenOraniTutturur()
        {
            var uretici = Rastgele.Kok(Tohum);
            int dogru = 0;
            for (int i = 0; i < 100000; i++)
                if (uretici.Olasilik(0.25f)) dogru++;

            // 100k cekiliste standart sapma ~137; +-1500 yaklasik 11 sigma.
            Assert.AreEqual(25000, dogru, 1500);
        }

        // --- Secim ve karistirma -----------------------------------------------

        [Test]
        public void SecListedenBirOgeVerir()
        {
            var uretici = Rastgele.Kok(Tohum);
            var liste = new List<string> { "a", "b", "c" };

            for (int i = 0; i < 100; i++)
                Assert.Contains(uretici.Sec(liste), liste);
        }

        [Test]
        public void BosListedenSecimHataVerir()
        {
            var uretici = Rastgele.Kok(Tohum);
            Assert.Throws<ArgumentException>(() => uretici.Sec(new List<int>()));
            Assert.Throws<ArgumentNullException>(() => uretici.Sec<int>(null));
        }

        [Test]
        public void KaristirPermutasyonUretir()
        {
            var uretici = Rastgele.Kok(Tohum);
            var liste = new List<int>();
            for (int i = 0; i < 50; i++) liste.Add(i);

            uretici.Karistir(liste);

            liste.Sort();
            for (int i = 0; i < 50; i++)
                Assert.AreEqual(i, liste[i], "Karistirma oge kaybetti veya cogaltti.");
        }

        [Test]
        public void KaristirAyniTohumdaAyniSonuc()
        {
            var a = new List<int>();
            var b = new List<int>();
            for (int i = 0; i < 50; i++) { a.Add(i); b.Add(i); }

            Rastgele.Kok(Tohum).Karistir(a);
            Rastgele.Kok(Tohum).Karistir(b);

            CollectionAssert.AreEqual(a, b);
        }

        [Test]
        public void KaristirSirayiGercektenDegistirir()
        {
            var uretici = Rastgele.Kok(Tohum);
            var liste = new List<int>();
            for (int i = 0; i < 50; i++) liste.Add(i);

            uretici.Karistir(liste);

            bool farkVar = false;
            for (int i = 0; i < liste.Count && !farkVar; i++)
                farkVar = liste[i] != i;

            Assert.IsTrue(farkVar);
        }

        // --- SecFarkli: muzayedeye gelen alicilar ------------------------------

        [Test]
        public void SecFarkliTekrarsizVeIstenenSayida()
        {
            var uretici = Rastgele.Kok(Tohum);
            var havuz = new List<int>();
            for (int i = 0; i < 12; i++) havuz.Add(i);

            for (int deneme = 0; deneme < 500; deneme++)
            {
                int[] gelenler = uretici.SecFarkli(havuz, 5);

                Assert.AreEqual(5, gelenler.Length);
                CollectionAssert.AllItemsAreUnique(gelenler);
                foreach (int alici in gelenler) Assert.Contains(alici, havuz);
            }
        }

        [Test]
        public void SecFarkliHavuzunTamaminiVerebilir()
        {
            var uretici = Rastgele.Kok(Tohum);
            var havuz = new List<int> { 1, 2, 3 };

            int[] hepsi = uretici.SecFarkli(havuz, 3);

            Array.Sort(hepsi);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, hepsi);
        }

        [Test]
        public void SecFarkliHavuzdanFazlasiniIsteyinceHataVerir()
        {
            var uretici = Rastgele.Kok(Tohum);
            var havuz = new List<int> { 1, 2, 3 };

            Assert.Throws<ArgumentException>(() => uretici.SecFarkli(havuz, 4));
            Assert.Throws<ArgumentException>(() => uretici.SecFarkli(havuz, -1));
        }

        [Test]
        public void SecFarkliHerOgeyeUlasabilir()
        {
            var uretici = Rastgele.Kok(Tohum);
            var havuz = new List<int>();
            for (int i = 0; i < 12; i++) havuz.Add(i);

            var gorulen = new HashSet<int>();
            for (int deneme = 0; deneme < 500; deneme++)
                foreach (int alici in uretici.SecFarkli(havuz, 5)) gorulen.Add(alici);

            Assert.AreEqual(12, gorulen.Count, "Havuzda hic secilmeyen alici var.");
        }

        // --- Agirlikli secim: esya bollugu -------------------------------------

        [Test]
        public void AgirlikliSecOranaUyar()
        {
            // Yatak bolluk 9, el dokuma kilim bolluk 1 (docs/tasarim/05-icerik.md).
            var agirliklar = new List<int> { 9, 1 };
            var uretici = Rastgele.Kok(Tohum);

            int yatak = 0;
            const int Cekilis = 100000;
            for (int i = 0; i < Cekilis; i++)
                if (uretici.AgirlikliSecIndeks(agirliklar) == 0) yatak++;

            // Beklenen 90000, standart sapma ~95. +-1000 rahat bir pay.
            Assert.AreEqual(90000, yatak, 1000);
        }

        [Test]
        public void AgirligiSifirOlanSecilmez()
        {
            var agirliklar = new List<int> { 0, 5, 0, 5 };
            var uretici = Rastgele.Kok(Tohum);

            for (int i = 0; i < 10000; i++)
            {
                int secilen = uretici.AgirlikliSecIndeks(agirliklar);
                Assert.IsTrue(secilen == 1 || secilen == 3, "Agirligi sifir olan indeks secildi: " + secilen);
            }
        }

        [Test]
        public void AgirlikliSecSonOgeyeUlasabilir()
        {
            var agirliklar = new List<int> { 1, 1, 1, 1, 1 };
            var uretici = Rastgele.Kok(Tohum);

            var gorulen = new HashSet<int>();
            for (int i = 0; i < 2000; i++)
                gorulen.Add(uretici.AgirlikliSecIndeks(agirliklar));

            Assert.AreEqual(5, gorulen.Count);
        }

        [Test]
        public void AgirlikliSecGecersizGirdideHataVerir()
        {
            var uretici = Rastgele.Kok(Tohum);

            Assert.Throws<ArgumentNullException>(() => uretici.AgirlikliSecIndeks(null));
            Assert.Throws<ArgumentException>(() => uretici.AgirlikliSecIndeks(new List<int>()));
            Assert.Throws<ArgumentException>(() => uretici.AgirlikliSecIndeks(new List<int> { 0, 0 }));
            Assert.Throws<ArgumentException>(() => uretici.AgirlikliSecIndeks(new List<int> { 1, -1 }));
        }

        // --- Kayit / geri yukleme ---------------------------------------------

        [Test]
        public void KayittanYuklenenAyniYerdenDevamEder()
        {
            var uretici = Rastgele.Kok(Tohum);
            for (int i = 0; i < 37; i++) uretici.Sonraki();

            var anlik = uretici.DurumuAl();
            var yuklenen = Rastgele.DurumdanKur(anlik);

            for (int i = 0; i < 100; i++)
                Assert.AreEqual(uretici.Sonraki(), yuklenen.Sonraki(), "Adim " + i);
        }

        [Test]
        public void KayitliDurumTohumuKorur()
        {
            var dal = Rastgele.Kok(Tohum).Dal("muzayede");
            var yuklenen = Rastgele.DurumdanKur(dal.DurumuAl());

            Assert.AreEqual(Tohum, yuklenen.Tohum);
        }
    }
}
