using System;
using NUnit.Framework;
using Oyun.Core;

namespace Oyun.Testler
{
    public class EsyaVerisiTestleri
    {
        // Gecerli bir esya; testler tek alanini bozarak calisiyor.
        // Sayilar docs/uretim/esyalar.md'deki buzdolabi satiri.
        static EsyaVerisi Kur(
            string id = "buzdolabi",
            string ad = "Buzdolabi",
            Kategori kategori = Kategori.BeyazEsya,
            int tabanDeger = 6500,
            float agirlik = 65.0f,
            float hacim = 0.70f,
            bool kirilgan = false,
            bool haczedilebilir = true,
            bool ikiKisiGerekir = true,
            bool cebeGirer = false,
            bool sabit = false,
            int bolluk = 7)
        {
            return new EsyaVerisi(id, ad, kategori, tabanDeger, agirlik, hacim,
                kirilgan, haczedilebilir, ikiKisiGerekir, cebeGirer, sabit, bolluk);
        }

        [Test]
        public void AlanlarVerildigiGibiOkunur()
        {
            var esya = Kur();

            Assert.AreEqual("buzdolabi", esya.Id);
            Assert.AreEqual("Buzdolabi", esya.Ad);
            Assert.AreEqual(Kategori.BeyazEsya, esya.Kategori);
            Assert.AreEqual(6500, esya.TabanDeger);
            Assert.AreEqual(65.0f, esya.Agirlik, 0.001f);
            Assert.AreEqual(0.70f, esya.Hacim, 0.001f);
            Assert.IsFalse(esya.Kirilgan);
            Assert.IsTrue(esya.Haczedilebilir);
            Assert.IsTrue(esya.IkiKisiGerekir);
            Assert.IsFalse(esya.CebeGirer);
            Assert.IsFalse(esya.Sabit);
            Assert.AreEqual(7, esya.Bolluk);
        }

        // --- Zorunlu alanlar ---------------------------------------------------

        [Test]
        public void BosIdKabulEdilmez()
        {
            Assert.Throws<ArgumentException>(() => Kur(id: null));
            Assert.Throws<ArgumentException>(() => Kur(id: ""));
            Assert.Throws<ArgumentException>(() => Kur(id: "   "));
        }

        [Test]
        public void BosAdKabulEdilmez()
        {
            Assert.Throws<ArgumentException>(() => Kur(ad: null));
            Assert.Throws<ArgumentException>(() => Kur(ad: "  "));
        }

        [Test]
        public void TanimsizKategoriKabulEdilmez()
        {
            Assert.Throws<ArgumentException>(() => Kur(kategori: (Kategori)99));
        }

        // --- Sayisal sinirlar --------------------------------------------------

        [Test]
        public void NegatifTabanDegerKabulEdilmez()
        {
            Assert.Throws<ArgumentException>(() => Kur(tabanDeger: -1));
        }

        [Test]
        public void BedavaEsyaGecerli()
        {
            // Hurda kategorisinde taban degeri sifir olan esya olabilir; sinir
            // negatifte, sifirda degil.
            Assert.AreEqual(0, Kur(tabanDeger: 0).TabanDeger);
        }

        [Test]
        public void AgirlikVeHacimPozitifOlmali()
        {
            Assert.Throws<ArgumentException>(() => Kur(agirlik: 0.0f));
            Assert.Throws<ArgumentException>(() => Kur(agirlik: -5.0f));
            Assert.Throws<ArgumentException>(() => Kur(hacim: 0.0f));
            Assert.Throws<ArgumentException>(() => Kur(hacim: -0.1f));
        }

        [Test]
        public void BollukBirIleOnArasinda()
        {
            Assert.Throws<ArgumentException>(() => Kur(bolluk: 0));
            Assert.Throws<ArgumentException>(() => Kur(bolluk: 11));
            Assert.Throws<ArgumentException>(() => Kur(bolluk: -3));

            // Sinirlarin kendisi gecerli: yatak 9, el dokuma kilim 1.
            Assert.AreEqual(1, Kur(bolluk: 1).Bolluk);
            Assert.AreEqual(10, Kur(bolluk: 10).Bolluk);
        }

        // --- Celiskili birlesim ------------------------------------------------

        [Test]
        public void CebeGirenEsyaIkiKisiGerektiremez()
        {
            Assert.Throws<ArgumentException>(() => Kur(cebeGirer: true, ikiKisiGerekir: true));
        }

        [Test]
        public void SabitEsyaDigerTasimaKurallariylaCelismez()
        {
            // "sabit" edinmeyi anlatiyor, tasimayi degil: sokulmus bakir tesisat
            // slota girer, radyator iki kisi ister. Ikisi de gecerli veri.
            Assert.DoesNotThrow(() => Kur(sabit: true, cebeGirer: true, ikiKisiGerekir: false));
            Assert.DoesNotThrow(() => Kur(sabit: true, cebeGirer: false, ikiKisiGerekir: true));
        }

        // --- Yasak listesi -----------------------------------------------------

        [Test]
        public void HaczedilemeyenEsyaTanimlanabilir()
        {
            // Yatak yasak listesinde (docs/tasarim/02-saha.md). Oyuncu yine de
            // alabiliyor; bedeli sure ve gurultu. Yani veri bunu tasiyabilmeli.
            var yatak = Kur(id: "yatak", ad: "Yatak", kategori: Kategori.Mobilya,
                haczedilebilir: false, bolluk: 9);

            Assert.IsFalse(yatak.Haczedilebilir);
        }

        // --- Degismezlik -------------------------------------------------------

        [Test]
        public void EsyaVerisiSaltOkunur()
        {
            // Tanim paylasilan veri: bir evde degistirilirse butun evleri etkiler.
            // Yazilabilir alan eklendiginde bu test kirilsin.
            foreach (var alan in typeof(EsyaVerisi).GetProperties())
                Assert.IsFalse(alan.CanWrite, "Yazilabilir alan: " + alan.Name);
        }

        [Test]
        public void MetinGosterimiKimligiIcerir()
        {
            StringAssert.Contains("buzdolabi", Kur().ToString());
        }
    }
}
