using System;
using System.Collections.Generic;

namespace Oyun.Utils
{
    // Tohumlu, belirlenimci rastgelelik. UnityEngine.Random yasak (docs/ajan-kurallari.md).
    //
    // Neden kendi ureticimiz:
    //   1. Kampanya gorevleri "elle ayarlanmis tohum" olarak tanimli
    //      (docs/tasarim/04-ekonomi.md). Ayni tohum her makinede ayni evi
    //      uretmek zorunda; UnityEngine.Random'in ic algoritmasi Unity surumleri
    //      arasinda garanti degil.
    //   2. UnityEngine.Random tek global durum tasir. Ev uretimi ile muzayede
    //      katilimi ayni akistan cekerse birinde yapilan degisiklik otekini
    //      kaydirir ve kayitli tohum anlamini yitirir.
    //
    // Algoritma PCG32 (O'Neill 2014): 64 bit durum, 32 bit cikti. Cekirdekte
    // kayan nokta yok -- platformlar arasi yuvarlama farki belirlenimciligi bozar.
    public sealed class Rastgele
    {
        const ulong Carpan = 6364136223846793005UL;

        // FNV-1a 64. string.GetHashCode() kullanilamaz: .NET'te surece gore
        // rastgelelestirilir, ayni tohum ertesi calistirmada baska sonuc verirdi.
        const ulong FnvBaslangic = 14695981039346656037UL;
        const ulong FnvCarpan = 1099511628211UL;

        readonly ulong tohum;
        ulong durum;
        ulong artis;

        Rastgele(ulong tohum, ulong akis)
        {
            this.tohum = tohum;

            // artis tek sayi olmak zorunda; cift olursa donem yariya duser.
            artis = (akis << 1) | 1UL;
            durum = 0UL;
            Sonraki();
            unchecked { durum += tohum; }
            Sonraki();
        }

        /// <summary>Bir oyunun kok ureticisi. Tohum OyunDurumu icinde saklanir.</summary>
        public static Rastgele Kok(ulong tohum)
        {
            return new Rastgele(tohum, 1UL);
        }

        /// <summary>Kok tohumdan turetilmis bagimsiz akis.</summary>
        /// <remarks>
        /// Sonuc yalnizca (tohum, ad) ciftine bagli -- dallarin acilma sirasina
        /// degil. Yeni bir dal eklemek mevcut dallarin ciktisini kaydirmaz;
        /// kampanya tohumunun kalici olmasi buna bagli.
        /// </remarks>
        public Rastgele Dal(string ad)
        {
            if (string.IsNullOrEmpty(ad))
                throw new ArgumentException("Dal adi bos olamaz.", nameof(ad));

            return new Rastgele(tohum, Karma(ad));
        }

        /// <summary>Bu ureticinin turedigi kok tohum.</summary>
        public ulong Tohum { get { return tohum; } }

        /// <summary>Ham 32 bit cikti.</summary>
        public uint Sonraki()
        {
            unchecked
            {
                ulong eski = durum;
                durum = eski * Carpan + artis;

                uint karisik = (uint)(((eski >> 18) ^ eski) >> 27);
                int donme = (int)(eski >> 59);
                return (karisik >> donme) | (karisik << ((-donme) & 31));
            }
        }

        /// <summary>[altDahil, ustHaric) araliginda tamsayi. Yanliligi yok.</summary>
        public int Aralik(int altDahil, int ustHaric)
        {
            if (ustHaric <= altDahil)
                throw new ArgumentException("ustHaric, altDahil'den buyuk olmali.", nameof(ustHaric));

            uint genislik = (uint)((long)ustHaric - altDahil);
            return altDahil + (int)AltindaSayi(genislik);
        }

        /// <summary>[0, ustHaric) araliginda tamsayi.</summary>
        public int Aralik(int ustHaric)
        {
            return Aralik(0, ustHaric);
        }

        /// <summary>[0, 1) araliginda kesir.</summary>
        public float Kesir()
        {
            // 24 bit: float mantisi kadar. Daha fazlasi yuvarlanip tam 1.0f uretebilirdi.
            return (Sonraki() >> 8) * (1.0f / 16777216.0f);
        }

        /// <summary>[alt, ust) araliginda kesir.</summary>
        public float Kesir(float alt, float ust)
        {
            if (ust < alt)
                throw new ArgumentException("ust, alt degerinden kucuk olamaz.", nameof(ust));

            return alt + (ust - alt) * Kesir();
        }

        /// <summary>Verilen ihtimalle dogru. 0 hicbir zaman, 1 her zaman.</summary>
        public bool Olasilik(float ihtimal)
        {
            if (ihtimal <= 0.0f) return false;
            if (ihtimal >= 1.0f) return true;
            return Kesir() < ihtimal;
        }

        /// <summary>Listeden esit ihtimalle bir oge.</summary>
        public T Sec<T>(IReadOnlyList<T> kaynak)
        {
            if (kaynak == null) throw new ArgumentNullException(nameof(kaynak));
            if (kaynak.Count == 0)
                throw new ArgumentException("Bos listeden secim yapilamaz.", nameof(kaynak));

            return kaynak[Aralik(kaynak.Count)];
        }

        /// <summary>Agirliga gore indeks secer. Ev icerigi "bolluk" alanina gore uretiliyor.</summary>
        /// <remarks>
        /// docs/tasarim/02-saha.md: duz rastgele secimde gecekonduda uc el dokuma
        /// kilim cikiyor, antikanin nadirligi kayboluyor.
        /// </remarks>
        public int AgirlikliSecIndeks(IReadOnlyList<int> agirliklar)
        {
            if (agirliklar == null) throw new ArgumentNullException(nameof(agirliklar));
            if (agirliklar.Count == 0)
                throw new ArgumentException("Bos agirlik listesi.", nameof(agirliklar));

            long toplam = 0;
            for (int i = 0; i < agirliklar.Count; i++)
            {
                if (agirliklar[i] < 0)
                    throw new ArgumentException("Agirlik negatif olamaz.", nameof(agirliklar));

                toplam += agirliklar[i];
            }

            if (toplam == 0)
                throw new ArgumentException("Agirliklarin toplami sifir.", nameof(agirliklar));
            if (toplam > uint.MaxValue)
                throw new ArgumentException("Agirlik toplami 32 bite sigmiyor.", nameof(agirliklar));

            // Tamsayi uzerinden yuruyoruz; kayan noktada birikimli hata son ogeyi
            // erisilmez kilabiliyor.
            long hedef = AltindaSayi((uint)toplam);
            long birikim = 0;
            for (int i = 0; i < agirliklar.Count; i++)
            {
                birikim += agirliklar[i];
                if (hedef < birikim) return i;
            }

            return agirliklar.Count - 1;
        }

        /// <summary>Listeyi yerinde karistirir (Fisher-Yates).</summary>
        public void Karistir<T>(IList<T> liste)
        {
            if (liste == null) throw new ArgumentNullException(nameof(liste));

            for (int i = liste.Count - 1; i > 0; i--)
            {
                int j = Aralik(i + 1);
                T gecici = liste[i];
                liste[i] = liste[j];
                liste[j] = gecici;
            }
        }

        /// <summary>Kaynaktan tekrarsiz "adet" oge secer. Muzayedeye gelen alicilar boyle belirlenir.</summary>
        public T[] SecFarkli<T>(IReadOnlyList<T> kaynak, int adet)
        {
            if (kaynak == null) throw new ArgumentNullException(nameof(kaynak));
            if (adet < 0)
                throw new ArgumentException("Adet negatif olamaz.", nameof(adet));
            if (adet > kaynak.Count)
                throw new ArgumentException("Adet, kaynak boyutundan buyuk olamaz.", nameof(adet));

            var havuz = new List<T>(kaynak);
            var sonuc = new T[adet];
            for (int i = 0; i < adet; i++)
            {
                int j = Aralik(havuz.Count);
                sonuc[i] = havuz[j];

                // Secileni sondaki ile takas edip kisalt: List.RemoveAt(orta) O(n).
                havuz[j] = havuz[havuz.Count - 1];
                havuz.RemoveAt(havuz.Count - 1);
            }

            return sonuc;
        }

        /// <summary>Kayit icin anlik durum.</summary>
        public Anlik DurumuAl()
        {
            return new Anlik(tohum, durum, artis);
        }

        /// <summary>Kayittan geri yukler.</summary>
        public static Rastgele DurumdanKur(Anlik anlik)
        {
            var uretici = new Rastgele(anlik.Tohum, 1UL);
            uretici.durum = anlik.IcDurum;
            uretici.artis = anlik.Artis;
            return uretici;
        }

        // [0, genislik) araliginda yansiz tamsayi -- Lemire'in carpma yontemi.
        // Klasik "% genislik" ikinin kuvveti olmayan araliklarda kucuk degerleri
        // fazladan uretir; 12 kisilik alici havuzunda bu olculebilir bir egilim demek.
        uint AltindaSayi(uint genislik)
        {
            if (genislik == 0) return 0;

            unchecked
            {
                ulong carpim = (ulong)Sonraki() * genislik;
                uint kalan = (uint)carpim;
                if (kalan < genislik)
                {
                    uint esik = (uint)((0x100000000UL - genislik) % genislik);
                    while (kalan < esik)
                    {
                        carpim = (ulong)Sonraki() * genislik;
                        kalan = (uint)carpim;
                    }
                }

                return (uint)(carpim >> 32);
            }
        }

        static ulong Karma(string metin)
        {
            unchecked
            {
                ulong karma = FnvBaslangic;
                for (int i = 0; i < metin.Length; i++)
                {
                    char harf = metin[i];
                    karma = (karma ^ (byte)(harf & 0xFF)) * FnvCarpan;
                    karma = (karma ^ (byte)(harf >> 8)) * FnvCarpan;
                }

                return karma;
            }
        }

        /// <summary>Ureticinin kayda yazilabilir hali.</summary>
        public readonly struct Anlik
        {
            public readonly ulong Tohum;
            public readonly ulong IcDurum;
            public readonly ulong Artis;

            public Anlik(ulong tohum, ulong icDurum, ulong artis)
            {
                Tohum = tohum;
                IcDurum = icDurum;
                Artis = artis;
            }
        }
    }
}
