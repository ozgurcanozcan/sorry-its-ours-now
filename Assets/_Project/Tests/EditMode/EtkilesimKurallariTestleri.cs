using NUnit.Framework;
using Oyun.Core;
using Oyun.Field;

namespace Oyun.Testler
{
    // Alma kurallari. Bu kurallar tasarimdan geliyor (docs/tasarim/02-saha.md),
    // uydurulmadi: sabit esya alet ister, agir esya tek kisiyle kaldirilmaz,
    // elde tek esya tasinir.
    public sealed class EtkilesimKurallariTestleri
    {
        // Gecerli bir esya uretir; test hangi bayragi deniyorsa onu degistirir.
        static EsyaVerisi Esya(bool sabit = false, bool ikiKisiGerekir = false,
                               bool cebeGirer = false)
        {
            return new EsyaVerisi(
                id: "test-esya",
                ad: "Test Esyasi",
                kategori: Kategori.Mobilya,
                tabanDeger: 100,
                agirlik: 5f,
                hacim: 0.2f,
                kirilgan: false,
                haczedilebilir: true,
                ikiKisiGerekir: ikiKisiGerekir,
                cebeGirer: cebeGirer,
                sabit: sabit,
                bolluk: 5);
        }

        [Test]
        public void SiradanEsya_BosElle_Alinabilir()
        {
            Assert.AreEqual(AlmaSonucu.Alinabilir,
                EtkilesimKurallari.Degerlendir(Esya(), elDolu: false, doluSlot: 0));
        }

        [Test]
        public void ElDoluyken_Alinamaz()
        {
            // "Elde tek esya" karari (02-saha.md): kamyona gidip gelmeyi
            // zorunlu kilan sey bu. Kural gevserse mekanigin tamami degisir.
            Assert.AreEqual(AlmaSonucu.ElDolu,
                EtkilesimKurallari.Degerlendir(Esya(), elDolu: true, doluSlot: 0));
        }

        [Test]
        public void SabitEsya_Alinamaz()
        {
            Assert.AreEqual(AlmaSonucu.Sabit,
                EtkilesimKurallari.Degerlendir(Esya(sabit: true), elDolu: false, doluSlot: 0));
        }

        [Test]
        public void IkiKisilikEsya_TekBasina_Alinamaz()
        {
            Assert.AreEqual(AlmaSonucu.IkiKisiGerekir,
                EtkilesimKurallari.Degerlendir(Esya(ikiKisiGerekir: true), elDolu: false, doluSlot: 0));
        }

        [Test]
        public void ElDolu_DigerSebeplerinOnunde()
        {
            // Sira onemli: oyuncunun duzeltebilecegi sebep once soylenmeli.
            // Sabit bir esyaya elin doluyken bakiyorsan once "elin dolu"
            // demek, sonra "zaten sabitti" demekten faydali.
            AlmaSonucu sonuc = EtkilesimKurallari.Degerlendir(
                Esya(sabit: true, ikiKisiGerekir: true), elDolu: true, doluSlot: 0);

            Assert.AreEqual(AlmaSonucu.ElDolu, sonuc);
        }

        [Test]
        public void VeriYoksa_Alinamaz()
        {
            // Tanimi baglanmamis bir prefab sessizce alinabilir olmamali.
            Assert.AreNotEqual(AlmaSonucu.Alinabilir,
                EtkilesimKurallari.Degerlendir(null, elDolu: false, doluSlot: 0));
        }

        [Test]
        public void Alinabilir_Disindaki_HerSonucun_Aciklamasi_Var()
        {
            // HUD sebebi gosterecek; bos metin "neden alamiyorum" sorusunu
            // cevapsiz birakir.
            foreach (AlmaSonucu sonuc in System.Enum.GetValues(typeof(AlmaSonucu)))
            {
                string aciklama = EtkilesimKurallari.Aciklama(sonuc);

                if (sonuc == AlmaSonucu.Alinabilir)
                {
                    Assert.IsEmpty(aciklama);
                }
                else
                {
                    Assert.IsNotEmpty(aciklama, $"{sonuc} icin aciklama yok");
                }
            }
        }

        [Test]
        public void KucukEsya_ElDoluyken_Alinabilir()
        {
            // 02-saha.md: kucuk esya cebe girer. Elde buyuk esya tasirken de
            // cebe atabilmek gerekiyor -- yoksa "elde tek esya" karari kucuk
            // esyalari da kilitler ve slot sisteminin anlami kalmaz.
            Assert.AreEqual(AlmaSonucu.Alinabilir,
                EtkilesimKurallari.Degerlendir(Esya(cebeGirer: true), elDolu: true, doluSlot: 0));
        }

        [Test]
        public void KucukEsya_SlotlarDoluyken_Alinamaz()
        {
            Assert.AreEqual(AlmaSonucu.SlotDolu,
                EtkilesimKurallari.Degerlendir(Esya(cebeGirer: true),
                    elDolu: false, doluSlot: EtkilesimKurallari.SlotSayisi));
        }

        [Test]
        public void BuyukEsya_SlotlarDoluyken_Alinabilir()
        {
            // Cep dolu olmasi elde tasimayi engellemez; ikisi ayri kapasite.
            Assert.AreEqual(AlmaSonucu.Alinabilir,
                EtkilesimKurallari.Degerlendir(Esya(),
                    elDolu: false, doluSlot: EtkilesimKurallari.SlotSayisi));
        }

        [Test]
        public void SlotaGirer_CebeGirerIsaretine_Bakar()
        {
            // Hacimden turetilmiyor: 02-saha.md "yagliboya tablo hacimce kucuk
            // ama cebe girmemeli, elle karar verilebilmeli".
            Assert.IsTrue(EtkilesimKurallari.SlotaGirer(Esya(cebeGirer: true)));
            Assert.IsFalse(EtkilesimKurallari.SlotaGirer(Esya()));
            Assert.IsFalse(EtkilesimKurallari.SlotaGirer(null));
        }

        [Test]
        public void SlotSayisi_Bes()
        {
            // Karar 31.08.2026, docs/tasarim/02-saha.md. Degisirse alet-kapasite
            // takasi da degisir; sabit bir sayi degil, bir tasarim karari.
            Assert.AreEqual(5, EtkilesimKurallari.SlotSayisi);
        }
    }
}
