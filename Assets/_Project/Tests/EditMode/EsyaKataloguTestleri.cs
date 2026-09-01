using System.Collections.Generic;
using NUnit.Framework;
using Oyun.Core;
using Oyun.Editor;
using UnityEditor;
using UnityEngine;

namespace Oyun.Testler
{
    public class EsyaKataloguTestleri
    {
        // --- Katalogun kendisi -------------------------------------------------

        [Test]
        public void KatalogdaOnEsyaVar()
        {
            // M1 hedefi 20. 7 buyuk esya (docs/uretim/esyalar.md) + 3 cebe giren.
            // Kalani docs/plan.md Hafta 4'te. Sayi degisince bu test hatirlatsin.
            Assert.AreEqual(10, EsyaKatalogu.Hepsi.Count);
        }

        [Test]
        public void IdlerTekrarsiz()
        {
            var gorulen = new HashSet<string>();
            foreach (var esya in EsyaKatalogu.Hepsi)
                Assert.IsTrue(gorulen.Add(esya.Id), "Tekrarlanan id: " + esya.Id);
        }

        [Test]
        public void BulVerilenIdyiGetirir()
        {
            Assert.AreEqual("Yatak", EsyaKatalogu.Bul("yatak").Ad);
            Assert.AreEqual(Kategori.Elektronik, EsyaKatalogu.Bul("tv").Kategori);
        }

        [Test]
        public void OlmayanIdSessizKalmaz()
        {
            // Bozuk kayit dosyasi null dondurup sonra baska yerde patlamasin.
            Assert.Throws<KeyNotFoundException>(() => EsyaKatalogu.Bul("olmayan-esya"));
            Assert.Throws<KeyNotFoundException>(() => EsyaKatalogu.Bul(null));

            Assert.IsFalse(EsyaKatalogu.VarMi("olmayan-esya"));
            Assert.IsFalse(EsyaKatalogu.VarMi(null));
            Assert.IsTrue(EsyaKatalogu.VarMi("yatak"));
        }

        // --- Belgeye baglilik --------------------------------------------------

        [Test]
        public void OlculmusFizikselDegerlerBelgeyleAyni()
        {
            // docs/uretim/esyalar.md "Agirlik ve hacim" tablosu. Bunlar olculmus
            // deger; degistirilirse kaynakla birlikte degismeli.
            Assert.AreEqual(35.0f, EsyaKatalogu.Bul("yatak").Agirlik, 0.01f);
            Assert.AreEqual(1.60f, EsyaKatalogu.Bul("yatak").Hacim, 0.001f);
            Assert.AreEqual(70.0f, EsyaKatalogu.Bul("camasir-makinesi").Agirlik, 0.01f);
            Assert.AreEqual(0.056f, EsyaKatalogu.Bul("hali").Hacim, 0.0001f);
            Assert.AreEqual(0.048f, EsyaKatalogu.Bul("tv").Hacim, 0.0001f);
        }

        [Test]
        public void YatakHaczedilemez()
        {
            // docs/tasarim/02-saha.md yasak listesi. Oyuncu yine de alabiliyor;
            // bedeli sure ve gurultu, para cezasi degil.
            Assert.IsFalse(EsyaKatalogu.Bul("yatak").Haczedilebilir);

            foreach (var esya in EsyaKatalogu.Hepsi)
                if (esya.Id != "yatak")
                    Assert.IsTrue(esya.Haczedilebilir, esya.Id + " neden haczedilemez?");
        }

        [Test]
        public void KirilganlarElektronikler()
        {
            // Ekranli iki esya. Kirilgan olan duserse bir kondisyon kademesi
            // iniyor (docs/tasarim/02-saha.md).
            var beklenen = new HashSet<string> { "tv", "telefon" };

            foreach (var esya in EsyaKatalogu.Hepsi)
                Assert.AreEqual(beklenen.Contains(esya.Id), esya.Kirilgan, esya.Id);
        }

        [Test]
        public void CebeGirenEsyalarVar()
        {
            // 5 slotlu tasima sistemi (docs/tasarim/02-saha.md) cebeGirer
            // isaretine bakiyor. Isaretli esya olmazsa sistem denenemiyor --
            // PR #12'de tam bu oldu.
            var cebeGirenler = new List<string>();
            foreach (var esya in EsyaKatalogu.Hepsi)
                if (esya.CebeGirer) cebeGirenler.Add(esya.Id);

            CollectionAssert.AreEquivalent(
                new[] { "gumus-takim", "saat", "telefon" }, cebeGirenler);
        }

        [Test]
        public void CebeGirenEsyalarTekKisilik()
        {
            // EsyaVerisi yapicisi zaten reddediyor; burada katalog seviyesinde
            // de duruyor ki yeni esya eklerken kural gorunur olsun.
            foreach (var esya in EsyaKatalogu.Hepsi)
                if (esya.CebeGirer)
                    Assert.IsFalse(esya.IkiKisiGerekir, esya.Id);
        }

        // Slotlarin VAR OLMA SEBEBI bu. docs/tasarim/02-saha.md 5 slotu aletlerle
        // paylastiriyor ve "alet mi kapasite mi" takasini oradan kuruyor. Cebe
        // giren esya, elde tasinan buyuk esyadan cok daha deger-yogun olmazsa
        // slotu kullanmak anlamsizlasir: oyuncu hep buyuk esya tasir.
        [Test]
        public void CebeGirenlerDegerYogun()
        {
            float enDusukCep = float.MaxValue;
            float enYuksekBuyuk = 0.0f;

            foreach (var esya in EsyaKatalogu.Hepsi)
            {
                float yogunluk = esya.TabanDeger / esya.Hacim;

                if (esya.CebeGirer) enDusukCep = Mathf.Min(enDusukCep, yogunluk);
                else enYuksekBuyuk = Mathf.Max(enYuksekBuyuk, yogunluk);
            }

            Assert.Greater(enDusukCep, enYuksekBuyuk,
                "En az deger-yogun cep esyasi (" + enDusukCep.ToString("0") + " TL/m3), " +
                "en yogun buyuk esyadan (" + enYuksekBuyuk.ToString("0") + " TL/m3) dusuk. " +
                "Boyle olursa slot kullanmak anlamsizlasir.");
        }

        [Test]
        public void HicbiriSabitDegil()
        {
            // Sabit esyalar (bakir tesisat, radyator, klima) matkap gerektiriyor;
            // matkap ve sabit esyalar kalan 10 esyayla birlikte gelecek.
            foreach (var esya in EsyaKatalogu.Hepsi)
                Assert.IsFalse(esya.Sabit, esya.Id);
        }

        // --- Olculmemis degerler -----------------------------------------------

        [Test]
        public void ZayifDegerListesiKatalogaUyuyor()
        {
            // Liste unutulmasin diye: id'ler gecerli olmali.
            foreach (string id in EsyaKatalogu.ZayifTabanDeger)
                Assert.IsTrue(EsyaKatalogu.VarMi(id), "Zayif deger listesinde olmayan id: " + id);

            CollectionAssert.AreEquivalent(
                new[] { "masa", "tv", "gumus-takim", "saat", "telefon" },
                EsyaKatalogu.ZayifTabanDeger,
                "Zayif taban deger listesi degisti. Hafta 5 dengesi bu listeye bakiyor; " +
                "degisiklik bilincliyse docs/kararlar/ altina not dusulmeli.");
        }

        // Masanin taban degeri olculmedi: belgenin tek referansi 6 kisilik yemek
        // takimi ve tek masa icin gecerli degil. Sayi yerine kisit yazildi.
        [Test]
        public void MasaHaliIleGardiropArasinda()
        {
            int hali = EsyaKatalogu.Bul("hali").TabanDeger;
            int masa = EsyaKatalogu.Bul("masa").TabanDeger;
            int gardirop = EsyaKatalogu.Bul("gardirop").TabanDeger;

            Assert.Greater(masa, hali, "Masa siradan halidan ucuz olmamali.");
            Assert.Less(masa, gardirop, "Masa gardiroptan pahali olmamali.");
        }

        // --- Uretilen varliklarla senkron --------------------------------------
        //
        // Hibrit verinin bilinen bedeli: iki temsil ayrisabilir. Karar notu bu
        // testi on kosul olarak yaziyordu (docs/kararlar/2026-08-31-sistem-kodu).
        // Uretim adimi atlanirsa burada gorunur.

        [Test]
        public void HerEsyaninUretilmisVarligiVar()
        {
            foreach (var veri in EsyaKatalogu.Hepsi)
            {
                var varlik = AssetDatabase.LoadAssetAtPath<EsyaTanimi>(EsyaVarlikUretici.Yol(veri.Id));
                Assert.IsNotNull(varlik,
                    veri.Id + " icin varlik yok. \"Oyun / Esya varliklarini uret\" calistirilmali.");
            }
        }

        [Test]
        public void UretilmisVarliklarKataloglaAyni()
        {
            foreach (var veri in EsyaKatalogu.Hepsi)
            {
                var varlik = AssetDatabase.LoadAssetAtPath<EsyaTanimi>(EsyaVarlikUretici.Yol(veri.Id));
                if (varlik == null) continue; // eksiklik ustteki testin isi

                var varliktaki = varlik.Veri();
                string neden = veri.Id + " varligi katalogdan ayrismis. Varliklar elle duzenlenmez; " +
                               "katalogda degistirip yeniden uret.";

                Assert.AreEqual(veri.Id, varliktaki.Id, neden);
                Assert.AreEqual(veri.Ad, varliktaki.Ad, neden);
                Assert.AreEqual(veri.Kategori, varliktaki.Kategori, neden);
                Assert.AreEqual(veri.TabanDeger, varliktaki.TabanDeger, neden);
                Assert.AreEqual(veri.Agirlik, varliktaki.Agirlik, 0.0001f, neden);
                Assert.AreEqual(veri.Hacim, varliktaki.Hacim, 0.0000001f, neden);
                Assert.AreEqual(veri.Kirilgan, varliktaki.Kirilgan, neden);
                Assert.AreEqual(veri.Haczedilebilir, varliktaki.Haczedilebilir, neden);
                Assert.AreEqual(veri.IkiKisiGerekir, varliktaki.IkiKisiGerekir, neden);
                Assert.AreEqual(veri.CebeGirer, varliktaki.CebeGirer, neden);
                Assert.AreEqual(veri.Sabit, varliktaki.Sabit, neden);
                Assert.AreEqual(veri.Bolluk, varliktaki.Bolluk, neden);
            }
        }

        [Test]
        public void KatalogdaKarsiligiOlmayanVarlikYok()
        {
            CollectionAssert.IsEmpty(EsyaVarlikUretici.OksuzVarliklar(),
                "Katalogdan cikarilmis ama varligi duran esya var. Prefab baglari " +
                "kontrol edilip elle silinmeli.");
        }
    }
}
