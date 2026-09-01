using NUnit.Framework;
using Oyun.Field;
using UnityEngine;

namespace Oyun.Testler
{
    // HareketHesabi'nin iki fonksiyonu. Ikisi de saf: sahne, kamera ve fizik
    // gerektirmiyor, dogrudan cagrilabiliyor.
    public sealed class HareketHesabiTestleri
    {
        const float Tolerans = 0.0001f;

        [Test]
        public void CaprazGirdi_DuzGirdiden_HizliDegil()
        {
            // Bu testin varlik sebebi: (1,1) uzunlugu 1.41. Sinirlanmazsa
            // capraz yurumek duz yurumekten %41 hizli olur ve sure kisiti
            // oradan sizar (docs/tasarim/02-saha.md).
            Vector3 duz = HareketHesabi.DunyaYonu(new Vector2(0f, 1f), 0f);
            Vector3 capraz = HareketHesabi.DunyaYonu(new Vector2(1f, 1f), 0f);

            Assert.AreEqual(1f, duz.magnitude, Tolerans);
            Assert.AreEqual(1f, capraz.magnitude, Tolerans);
        }

        [Test]
        public void KismiGirdi_Buyutulmez()
        {
            // Analog cubuk yarim itildiginde yavas yurunmeli. Normalize
            // kosulsuz uygulanirsa bu kaybolur ve her girdi tam hiz olur.
            Vector3 yon = HareketHesabi.DunyaYonu(new Vector2(0f, 0.5f), 0f);

            Assert.AreEqual(0.5f, yon.magnitude, Tolerans);
        }

        [Test]
        public void Sapma_GirdiyiDunyaEksenineCevirir()
        {
            // 90 derece saga donmusken "ileri" dunyada +X olmali.
            Vector3 yon = HareketHesabi.DunyaYonu(new Vector2(0f, 1f), 90f);

            Assert.AreEqual(1f, yon.x, Tolerans);
            Assert.AreEqual(0f, yon.y, Tolerans);
            Assert.AreEqual(0f, yon.z, Tolerans);
        }

        [Test]
        public void Girdi_YokkenYon_Sifir()
        {
            Vector3 yon = HareketHesabi.DunyaYonu(Vector2.zero, 45f);

            Assert.AreEqual(0f, yon.magnitude, Tolerans);
        }

        [Test]
        public void Egim_SinirinDisinaCikmaz()
        {
            // Sinir asilirsa kamera tepeden devrilir ve oyuncu yonunu kaybeder.
            float yukari = HareketHesabi.SinirliEgim(0f, 1000f, 89f);
            float asagi = HareketHesabi.SinirliEgim(0f, -1000f, 89f);

            Assert.AreEqual(-89f, yukari, Tolerans);
            Assert.AreEqual(89f, asagi, Tolerans);
        }

        [Test]
        public void Egim_FareYukariItilince_YukariBakar()
        {
            // Isaret hatasi en kolay gozden kacan hata: ters olsa da "calisiyor"
            // gorunuyor. Yukari itmek negatif egim (yukari bakis) uretmeli.
            float egim = HareketHesabi.SinirliEgim(0f, 10f, 89f);

            Assert.Less(egim, 0f);
        }

        [Test]
        public void BosElle_HizDegismez()
        {
            Assert.AreEqual(1f, HareketHesabi.TasimaHizKati(0f, 40f, 0.5f), Tolerans);
        }

        [Test]
        public void AgirEsya_Yavaslatir()
        {
            float hafif = HareketHesabi.TasimaHizKati(8f, 40f, 0.5f);
            float agir = HareketHesabi.TasimaHizKati(25f, 40f, 0.5f);

            // 02-saha.md: "agirlik bir sayi degil hissedilen bir sey". Hali ile
            // masa arasindaki fark oynarken ayirt edilebilmeli.
            Assert.Less(agir, hafif);
            Assert.Less(hafif, 1f);
        }

        [Test]
        public void Kat_EnDusugun_Altina_Inmez()
        {
            // Yoksa oyuncu yerinde sayar ve esya tasimak imkansizlasir.
            float cokAgir = HareketHesabi.TasimaHizKati(500f, 40f, 0.5f);

            Assert.AreEqual(0.5f, cokAgir, Tolerans);
        }

        [Test]
        public void Gecersiz_Sinir_Hizi_Bozmaz()
        {
            Assert.AreEqual(1f, HareketHesabi.TasimaHizKati(10f, 0f, 0.5f), Tolerans);
            Assert.AreEqual(1f, HareketHesabi.TasimaHizKati(-5f, 40f, 0.5f), Tolerans);
        }

        [Test]
        public void EliBosken_Kosabilir()
        {
            Assert.IsTrue(HareketHesabi.KosabilirMi(tasiyor: false));
        }

        [Test]
        public void TasirkenKosamaz()
        {
            // Karar 79. Kosmanin bedeli bu; olculen sorun kosmanin agirlik
            // cezasinin yarisini telafi etmesiydi (gardiropta 0.50 -> 0.80).
            Assert.IsFalse(HareketHesabi.KosabilirMi(tasiyor: true));
        }
    }
}
