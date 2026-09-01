using System;
using System.Collections.Generic;
using NUnit.Framework;
using Oyun.Auction;
using Oyun.Core;
using Oyun.Economy;
using Oyun.Utils;

namespace Oyun.Testler
{
    public class MuzayedeTestleri
    {
        const ulong Tohum = 909090UL;

        static MuzayedeAyarlari Ayarlar { get { return MuzayedeAyarlari.Varsayilan; } }
        static Rastgele Uretici() { return Rastgele.Kok(Tohum).Dal("muzayede-katilim"); }

        static EsyaOrnegi Ornek(string esyaId, Kondisyon kondisyon = Kondisyon.IyiDurumda)
        {
            return Degerleme.Uret(
                EsyaKatalogu.Bul(esyaId), kondisyon,
                Rastgele.Kok(Tohum).Dal("deger-" + esyaId), DegerAyarlari.Varsayilan);
        }

        static Lot TekLot(string esyaId, int rezerv = 0)
        {
            return new Lot(Ornek(esyaId), rezerv);
        }

        // --- Havuz -------------------------------------------------------------

        [Test]
        public void HavuzdaOnIkiAliciVar()
        {
            // M1'de 12, cikista 30+ (docs/tasarim/05-icerik.md).
            Assert.AreEqual(12, AliciKatalogu.Hepsi.Count);
        }

        [Test]
        public void AliciIdleriTekrarsiz()
        {
            var gorulen = new HashSet<string>();
            foreach (var alici in AliciKatalogu.Hepsi)
                Assert.IsTrue(gorulen.Add(alici.Id), "Tekrarlanan id: " + alici.Id);
        }

        [Test]
        public void HerKategorininHevesliAlicisiVar()
        {
            // Her kategoride en az bir alici 1'in ustunde carpan vermeli.
            // Aksi halde o kategorideki esya her zaman ucuza gider ve
            // "hangi aliciyi bekliyorum" karari o kategoride yok olur.
            foreach (Kategori kategori in Enum.GetValues(typeof(Kategori)))
            {
                bool hevesliVar = false;
                foreach (var alici in AliciKatalogu.Hepsi)
                    if (alici.Carpan(kategori) > 1.0f) { hevesliVar = true; break; }

                Assert.IsTrue(hevesliVar, kategori + " kategorisine hevesli alici yok.");
            }
        }

        [Test]
        public void AliciCarpaniPrimVarsaPrimi()
        {
            var antikaci = AliciKatalogu.Bul("antikaci");

            Assert.AreEqual(1.8f, antikaci.Carpan(Kategori.Antika), 0.001f);
            Assert.AreEqual(antikaci.VarsayilanCarpan, antikaci.Carpan(Kategori.Mutfak), 0.001f);
        }

        [Test]
        public void GecersizAliciKurulamaz()
        {
            Assert.Throws<ArgumentException>(() => new Alici("", "A", "h", 100, 1f, 5));
            Assert.Throws<ArgumentException>(() => new Alici("a", "", "h", 100, 1f, 5));
            Assert.Throws<ArgumentException>(() => new Alici("a", "A", "", 100, 1f, 5));
            Assert.Throws<ArgumentException>(() => new Alici("a", "A", "h", 0, 1f, 5));
            Assert.Throws<ArgumentException>(() => new Alici("a", "A", "h", 100, 0f, 5));
            Assert.Throws<ArgumentException>(() => new Alici("a", "A", "h", 100, 1f, 0));
            Assert.Throws<ArgumentException>(() => new Alici("a", "A", "h", 100, 1f, 11));
        }

        // --- Katilim -----------------------------------------------------------

        [Test]
        public void GelenSayisiDortVeOranOlculmusOranda()
        {
            // Denemede 24 havuz / 8 gelen calisti -- havuzun ucte biri.
            // 12/8 orani ucte iki olur ve "bu hafta Koleksiyoncu var mi"
            // gerilimi olur (docs/tasarim/03-muzayede.md).
            Assert.AreEqual(4, Ayarlar.GelenSayisi);
            Assert.AreEqual(MuzayedeAyarlari.OlculmusKatilimOrani, Ayarlar.KatilimOrani, 0.01f);
        }

        [Test]
        public void GelenlerTekrarsizVeIstenenSayida()
        {
            var uretici = Uretici();
            for (int i = 0; i < 500; i++)
            {
                var gelenler = MuzayedeCozumleyici.GelenleriSec(uretici, Ayarlar);

                Assert.AreEqual(Ayarlar.GelenSayisi, gelenler.Count);

                var gorulen = new HashSet<string>();
                foreach (var alici in gelenler)
                    Assert.IsTrue(gorulen.Add(alici.Id), "Ayni alici iki kez geldi: " + alici.Id);
            }
        }

        [Test]
        public void HerAliciBirGunGelebiliyor()
        {
            var uretici = Uretici();
            var gorulen = new HashSet<string>();

            for (int i = 0; i < 2000; i++)
                foreach (var alici in MuzayedeCozumleyici.GelenleriSec(uretici, Ayarlar))
                    gorulen.Add(alici.Id);

            Assert.AreEqual(AliciKatalogu.Hepsi.Count, gorulen.Count,
                "Havuzda hic gelmeyen alici var.");
        }

        [Test]
        public void NadirAliciGercektenNadir()
        {
            // Koleksiyoncu agirlik 2, Hurdaci 9. "Bu hafta Koleksiyoncu var mi"
            // gerilimi bu farka bagli; fark olcumde gorunmuyorsa gerilim yok.
            var uretici = Uretici();
            int koleksiyoncu = 0;
            int hurdaci = 0;
            const int Oturum = 3000;

            for (int i = 0; i < Oturum; i++)
                foreach (var alici in MuzayedeCozumleyici.GelenleriSec(uretici, Ayarlar))
                {
                    if (alici.Id == "koleksiyoncu") koleksiyoncu++;
                    else if (alici.Id == "hurdaci") hurdaci++;
                }

            Assert.Less(koleksiyoncu, hurdaci,
                "Koleksiyoncu (agirlik 2) Hurdaci'dan (agirlik 9) az gelmeli. " +
                "Olculen: " + koleksiyoncu + " / " + hurdaci);

            // Nadir ama yok degil: her uc oturumdan en az birinde gorunmemeli,
            // her on oturumdan birinde gorunmeli gibi bir bantta kalmali.
            float oran = koleksiyoncu / (float)Oturum;
            Assert.Greater(oran, 0.02f, "Koleksiyoncu neredeyse hic gelmiyor: " + oran);
            Assert.Less(oran, 0.40f, "Koleksiyoncu nadir sayilmayacak kadar sik geliyor: " + oran);
        }

        [Test]
        public void AyniTohumAyniSalon()
        {
            var a = MuzayedeCozumleyici.GelenleriSec(Uretici(), Ayarlar);
            var b = MuzayedeCozumleyici.GelenleriSec(Uretici(), Ayarlar);

            for (int i = 0; i < a.Count; i++)
                Assert.AreEqual(a[i].Id, b[i].Id);
        }

        // --- Degerlendirme -----------------------------------------------------

        [Test]
        public void DegerlendirmeKategoriCarpaniniKullanir()
        {
            var lot = TekLot("tv");
            var elektronikci = AliciKatalogu.Bul("elektronikci");
            var antikaci = AliciKatalogu.Bul("antikaci");

            int elektronikciDeger = MuzayedeCozumleyici.Degerlendirme(elektronikci, lot);
            int antikaciDeger = MuzayedeCozumleyici.Degerlendirme(antikaci, lot);

            // Elektronikci 1.7, Antikaci elektronige 0.3.
            Assert.Greater(elektronikciDeger, antikaciDeger);
            Assert.AreEqual(lot.GercekDeger * 1.7f, elektronikciDeger, 1.0f);
        }

        [Test]
        public void KarisikLotEsyaBasinaHesaplanir()
        {
            // Antikaci, icinde tek bir mobilya olan lota o mobilya kadar deger
            // biciyor -- lotun tamamina kendi primini uygulamiyor.
            var lot = new Lot(new[] { Ornek("hali"), Ornek("tv") }, 0);
            var antikaci = AliciKatalogu.Bul("antikaci");

            int beklenen = (int)Math.Round(
                Ornek("hali").GercekDeger * 1.1 + Ornek("tv").GercekDeger * 0.3,
                MidpointRounding.AwayFromZero);

            Assert.AreEqual(beklenen, MuzayedeCozumleyici.Degerlendirme(antikaci, lot), 1);
        }

        // --- Ikinci fiyat acik artirma -----------------------------------------

        [Test]
        public void KazananIkinciTeklifiOder()
        {
            // Denemede olculdu: kazanan en yuksek teklifi veren, ama odedigi
            // ikinci teklifin biraz ustu (docs/tasarim/03-muzayede.md).
            var lot = TekLot("tv");
            var elektronikci = AliciKatalogu.Bul("elektronikci"); // 1.7
            var spekulator = AliciKatalogu.Bul("spekulator");     // 0.7

            var sonuc = MuzayedeCozumleyici.Coz(
                new[] { lot }, new[] { elektronikci, spekulator }, Ayarlar);

            var lotSonucu = sonuc.Lotlar[0];
            int ikinciTeklif = MuzayedeCozumleyici.Degerlendirme(spekulator, lot);

            Assert.AreEqual("elektronikci", lotSonucu.Kazanan.Id);
            Assert.AreEqual(ikinciTeklif + Ayarlar.TeklifArtisi, lotSonucu.Fiyat);
            Assert.Less(lotSonucu.Fiyat, lotSonucu.KazananDegerlendirmesi,
                "Kazanan kendi teklifini odememeli.");
        }

        [Test]
        public void RezervUcuzaGitmeyiEngeller()
        {
            // Kritik tasarim sonucu: rezerv YUKSEK FIYAT KAZANDIRMAZ, sadece
            // taban koyar. Ikisi ayni testte olculuyor.
            var lot = TekLot("tv");
            var elektronikci = AliciKatalogu.Bul("elektronikci");
            var spekulator = AliciKatalogu.Bul("spekulator");

            int rezervsiz = MuzayedeCozumleyici
                .Coz(new[] { new Lot(lot.Esyalar, 0) }, new[] { elektronikci, spekulator }, Ayarlar)
                .Lotlar[0].Fiyat;

            // Rezerv ikinci teklifin altinda: fiyati DEGISTIRMEMELI.
            int dusukRezerv = MuzayedeCozumleyici
                .Coz(new[] { new Lot(lot.Esyalar, 1) }, new[] { elektronikci, spekulator }, Ayarlar)
                .Lotlar[0].Fiyat;

            Assert.AreEqual(rezervsiz, dusukRezerv, "Dusuk rezerv fiyati degistirmemeli.");

            // Rezerv ikinci teklifin ustunde ama en yuksegin altinda: taban olur.
            int yuksekRezerv = rezervsiz + 500;
            int rezervli = MuzayedeCozumleyici
                .Coz(new[] { new Lot(lot.Esyalar, yuksekRezerv) },
                     new[] { elektronikci, spekulator }, Ayarlar)
                .Lotlar[0].Fiyat;

            Assert.AreEqual(yuksekRezerv, rezervli, "Rezerv taban olarak calismali.");
        }

        [Test]
        public void RezervCokYuksekseSatilmaz()
        {
            var lot = TekLot("hali", rezerv: 999999);
            var sonuc = MuzayedeCozumleyici.Coz(
                new[] { lot }, AliciKatalogu.Hepsi, Ayarlar);

            Assert.IsFalse(sonuc.Lotlar[0].Satildi);
            Assert.AreEqual(SatisSonucu.RezervinAltinda, sonuc.Lotlar[0].Sonuc);
            Assert.AreEqual(0, sonuc.Lotlar[0].Fiyat);
            Assert.AreEqual(1, sonuc.SatilmayanLotlar.Count);
        }

        [Test]
        public void SatilmayanLotDepodaKalir()
        {
            // Karar 31.08.2026: satilmayan esya kaybolmuyor, sonraki
            // muzayedeye tekrar cikarilabiliyor. Rezervin anlamli olmasi
            // buna bagli (docs/tasarim/03-muzayede.md).
            var lot = TekLot("gardirop", rezerv: 999999);
            var sonuc = MuzayedeCozumleyici.Coz(new[] { lot }, AliciKatalogu.Hepsi, Ayarlar);

            Assert.AreSame(lot, sonuc.SatilmayanLotlar[0].Lot);
            Assert.AreEqual(0, sonuc.ToplamHasilat);
        }

        [Test]
        public void KazananinKendiDegerlendirmesiTavan()
        {
            // Tek teklif varsa fiyat rezerv olur ama kazananin bictigi degeri asamaz.
            var lot = TekLot("hali");
            var tekAlici = AliciKatalogu.Bul("hurdaci");
            int degerlendirme = MuzayedeCozumleyici.Degerlendirme(tekAlici, lot);

            var sonuc = MuzayedeCozumleyici.Coz(
                new[] { new Lot(lot.Esyalar, degerlendirme + 1000) }, new[] { tekAlici }, Ayarlar);

            Assert.IsFalse(sonuc.Lotlar[0].Satildi,
                "Rezerv alicinin bictigi degerin ustundeyse satilmamali.");
        }

        // --- Butce tukenmesi ---------------------------------------------------

        [Test]
        public void ButceOturumBoyuncaTukenir()
        {
            // Denemede olculdu: tukenmezse her lot bagimsiz hesaplanir ve alici
            // cesitliligi anlamsizlasir -- 3.000 TL butceli komsu 6.000 TL'lik
            // alisveris yapabiliyordu.
            var emekli = AliciKatalogu.Bul("emekli-amca"); // 2.500 TL

            var lotlar = new List<Lot>();
            for (int i = 0; i < 6; i++) lotlar.Add(TekLot("hali"));

            var sonuc = MuzayedeCozumleyici.Coz(lotlar, new[] { emekli }, Ayarlar);

            int harcanan = 0;
            foreach (var lotSonucu in sonuc.Lotlar) harcanan += lotSonucu.Fiyat;

            Assert.LessOrEqual(harcanan, emekli.Butce,
                "Alici butcesinden fazla harcadi: " + harcanan + " > " + emekli.Butce);
        }

        // Tek alici + rezerv yok = esya neredeyse bedava gider. Bu bir kusur
        // degil, ikinci fiyat acik artirmanin dogal sonucu: ikinci teklif
        // yoksa fiyat tabana duser. docs/tasarim/03-muzayede.md'nin
        // "dusuk rezerv = yakma riski" dedigi sey tam olarak bu.
        //
        // Test bu davranisi KORUYOR: sessizce "tek alici kendi teklifini
        // odesin" diye degistirilirse rezerv karari anlamsizlasir.
        [Test]
        public void TekAliciVeRezervsizLotTabandanGider()
        {
            var lot = TekLot("gardirop");
            var emekli = AliciKatalogu.Bul("emekli-amca");

            var sonuc = MuzayedeCozumleyici.Coz(new[] { lot }, new[] { emekli }, Ayarlar);
            var lotSonucu = sonuc.Lotlar[0];

            Assert.IsTrue(lotSonucu.Satildi);
            Assert.AreEqual(Ayarlar.TeklifArtisi, lotSonucu.Fiyat,
                "Ikinci teklif yoksa fiyat taban olmali.");
            Assert.Less(lotSonucu.Fiyat, lotSonucu.KazananDegerlendirmesi / 10,
                "Yakma riski olculebilir olmali: fiyat degerlendirmenin cok altinda.");
        }

        [Test]
        public void ButceBitinceSonrakiLotlarSatilmaz()
        {
            // Rezerv sart: rezervsiz lotlar tabandan gider ve butce tukenmez
            // (ust testteki davranis).
            var emekli = AliciKatalogu.Bul("emekli-amca"); // 2.500 TL

            var lotlar = new List<Lot>();
            for (int i = 0; i < 10; i++)
                lotlar.Add(new Lot(Ornek("gardirop"), 1200));

            var sonuc = MuzayedeCozumleyici.Coz(lotlar, new[] { emekli }, Ayarlar);

            int harcanan = 0;
            foreach (var lotSonucu in sonuc.Lotlar) harcanan += lotSonucu.Fiyat;

            Assert.LessOrEqual(harcanan, emekli.Butce);
            Assert.Greater(sonuc.SatilmayanLotlar.Count, 0,
                "2.500 TL butceyle 1.200 rezervli 10 lot alinamamali.");
            Assert.IsFalse(sonuc.Lotlar[sonuc.Lotlar.Count - 1].Satildi,
                "Butce bitince sonraki lotlar satilmamali.");
        }

        [Test]
        public void LotSirasiSonucuDegistirir()
        {
            // "Neyi once satiyorum" karari butce tukenmesinden doguyor.
            // Sira onemsiz olsaydi o karar da olmazdi.
            // Rezervli: rezervsiz lotlar tabandan gittigi icin butce tukenmez
            // ve sira onemsizlesir.
            var ucuz = new Lot(Ornek("hali"), 800);
            var pahali = new Lot(Ornek("gardirop"), 2000);
            var emekli = AliciKatalogu.Bul("emekli-amca");

            var ucuzOnce = MuzayedeCozumleyici.Coz(
                new[] { ucuz, pahali }, new[] { emekli }, Ayarlar);
            var pahaliOnce = MuzayedeCozumleyici.Coz(
                new[] { pahali, ucuz }, new[] { emekli }, Ayarlar);

            Assert.AreNotEqual(ucuzOnce.ToplamHasilat, pahaliOnce.ToplamHasilat,
                "Lot sirasi hasilati degistirmiyor -- butce tukenmiyor olabilir.");
        }

        // --- Kisitlar ----------------------------------------------------------

        [Test]
        public void SalonButcesiArzdanKucuk()
        {
            // KISIT (docs/tasarim/03-muzayede.md, olculmus bulgu):
            // butce oturum boyunca TUKENMELI. Gelen alicilarin toplam butcesi
            // tipik bir muzayedenin arz degerinden buyukse hicbir sey tukenmez
            // ve "neyi once satiyorum" karari olmez.
            //
            // Tipik muzayede: 6 lot. docs/tasarim/02-saha.md'de olculdu --
            // "15 esyalik evden ancak 5-6 esya alinabildiginde karar dogdu".
            const int TipikLotSayisi = 6;

            var uretici = Uretici();
            long butceToplami = 0;
            const int Oturum = 2000;

            for (int i = 0; i < Oturum; i++)
                foreach (var alici in MuzayedeCozumleyici.GelenleriSec(uretici, Ayarlar))
                    butceToplami += alici.Butce;

            double ortalamaSalonButcesi = butceToplami / (double)Oturum;

            // Arz: katalog ortalamasi x tipik lot sayisi. Katalogdan
            // hesaplaniyor, sabit yazilmiyor -- esya eklenince kendini ayarlar.
            long tabanToplami = 0;
            foreach (var esya in EsyaKatalogu.Hepsi) tabanToplami += esya.TabanDeger;
            double arz = (tabanToplami / (double)EsyaKatalogu.Hepsi.Count) * TipikLotSayisi;

            Assert.Less(ortalamaSalonButcesi, arz,
                "Salon butcesi (" + ortalamaSalonButcesi.ToString("0") + " TL) tipik arzdan (" +
                arz.ToString("0") + " TL) buyuk. Butce tukenmez, alici cesitliligi anlamsizlasir.");
        }

        [Test]
        public void HerLottaEnAzIkiTeklifVar()
        {
            // KISIT: ikinci fiyat acik artirma en az iki teklif ister. Salon
            // 12/4'e inince bu risk dogdu; olculuyor.
            //
            // Varsayilan carpan sifir olmadigi icin herkes her seye teklif
            // veriyor -- lot alicisiz kalmiyor, ucuza gidiyor. Bu test o
            // tasarim secimini koruyor.
            var uretici = Uretici();
            int azTeklifliLot = 0;
            int toplamLot = 0;

            for (int oturum = 0; oturum < 300; oturum++)
            {
                var gelenler = MuzayedeCozumleyici.GelenleriSec(uretici, Ayarlar);

                var lotlar = new List<Lot>();
                foreach (var esya in EsyaKatalogu.Hepsi) lotlar.Add(TekLot(esya.Id));

                var sonuc = MuzayedeCozumleyici.Coz(lotlar, gelenler, Ayarlar);
                foreach (var lotSonucu in sonuc.Lotlar)
                {
                    toplamLot++;
                    if (lotSonucu.TeklifSayisi < 2) azTeklifliLot++;
                }
            }

            float oran = azTeklifliLot / (float)toplamLot;
            Assert.Less(oran, 0.25f,
                "Lotlarin %" + (oran * 100).ToString("0.0") + "'inde ikiden az teklif var. " +
                "Salon fazla ince: gelen sayisi artirilmali ya da butceler yukseltilmeli.");
        }

        // --- Gecersiz girdi ----------------------------------------------------

        [Test]
        public void GecersizGirdiReddedilir()
        {
            Assert.Throws<ArgumentNullException>(
                () => MuzayedeCozumleyici.Coz(null, Uretici(), Ayarlar));
            Assert.Throws<ArgumentNullException>(
                () => MuzayedeCozumleyici.Coz(new List<Lot>(), (Rastgele)null, Ayarlar));
            Assert.Throws<ArgumentException>(
                () => MuzayedeCozumleyici.Coz(new List<Lot>(), new List<Alici>(), Ayarlar));

            Assert.Throws<ArgumentException>(() => new Lot(new List<EsyaOrnegi>(), 0));
            Assert.Throws<ArgumentException>(() => new Lot(Ornek("hali"), -1));

            Assert.Throws<ArgumentException>(() => new MuzayedeAyarlari(1, 50));
            Assert.Throws<ArgumentException>(() => new MuzayedeAyarlari(99, 50));
            Assert.Throws<ArgumentException>(() => new MuzayedeAyarlari(4, -1));
        }
    }
}
