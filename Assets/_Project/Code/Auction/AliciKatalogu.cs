using System;
using System.Collections.Generic;
using Oyun.Core;

namespace Oyun.Auction
{
    // M1 alici havuzu: 12 kisi (docs/tasarim/05-icerik.md). Cikista 30+.
    //
    // HICBIR SAYI OLCULMEDI. Butceler, carpanlar ve katilim agirliklari
    // Hafta 5 denge isi (docs/plan.md). Saglamalari gereken kisitlar
    // MuzayedeTestleri icinde duruyor; en onemlisi:
    //
    //   gelen alicilarin toplam butcesi, tipik bir muzayedenin arz degerinden
    //   KUCUK olmali -- yoksa butce tukenmez ve alici cesitliligi anlamsizlasir
    //   (docs/tasarim/03-muzayede.md, denemede olculmus bulgu).
    //
    // Kisilikler denemeden tasindi (Antikaci, Hurdaci, Koleksiyoncu,
    // Spekulator, Emlakci, Lokantaci, Emekli amca, Yabanci alici, Rakip firma)
    // ve sekiz kategoriyi kapsayacak sekilde ucu eklendi.
    public static class AliciKatalogu
    {
        static readonly Alici[] hepsi =
        {
            new Alici(
                id: "antikaci", ad: "Antikacı Nuri",
                huy: "Eskiyi tanır, yeniye burun kıvırır.",
                butce: 9000, varsayilanCarpan: 0.6f, katilimAgirligi: 7,
                kategoriPrimleri: new Dictionary<Kategori, float>
                {
                    { Kategori.Antika, 1.8f },
                    { Kategori.Mobilya, 1.1f },
                    { Kategori.Elektronik, 0.3f }
                }),

            new Alici(
                id: "hurdaci", ad: "Hurdacı Sabri",
                huy: "Değerine değil kilosuna bakar.",
                butce: 3500, varsayilanCarpan: 0.4f, katilimAgirligi: 9,
                kategoriPrimleri: new Dictionary<Kategori, float>
                {
                    { Kategori.Hurda, 1.6f },
                    { Kategori.BeyazEsya, 0.9f },
                    { Kategori.Antika, 0.3f }
                }),

            // Nadir gelir ama geldiginde salonu degistirir. "Bu hafta
            // Koleksiyoncu var mi?" gerilimi (03-muzayede.md) buradan doguyor.
            new Alici(
                id: "koleksiyoncu", ad: "Koleksiyoncu",
                huy: "Tek bir şeyin peşinde; bulunca fiyat sormaz.",
                butce: 11000, varsayilanCarpan: 0.5f, katilimAgirligi: 2,
                kategoriPrimleri: new Dictionary<Kategori, float>
                {
                    { Kategori.Antika, 2.4f },
                    { Kategori.Kisisel, 1.5f }
                }),

            // Prim yok: her seye ayni dusuk carpani veriyor. Genis butcesiyle
            // ucuz lotlari supurup fiyat tabanini kuruyor.
            new Alici(
                id: "spekulator", ad: "Spekülatör Cem",
                huy: "Ucuza kapıp pahalıya satmayı düşünür.",
                butce: 9000, varsayilanCarpan: 0.7f, katilimAgirligi: 6),

            new Alici(
                id: "emlakci", ad: "Emlakçı Hülya",
                huy: "Boş daireleri döşemek için alır.",
                butce: 8000, varsayilanCarpan: 0.8f, katilimAgirligi: 6,
                kategoriPrimleri: new Dictionary<Kategori, float>
                {
                    { Kategori.Mobilya, 1.4f },
                    { Kategori.BeyazEsya, 1.3f }
                }),

            new Alici(
                id: "lokantaci", ad: "Lokantacı Mesut",
                huy: "Mutfağa giren her şeyi alır, gerisine bakmaz.",
                butce: 6000, varsayilanCarpan: 0.6f, katilimAgirligi: 5,
                kategoriPrimleri: new Dictionary<Kategori, float>
                {
                    { Kategori.Mutfak, 1.7f },
                    { Kategori.BeyazEsya, 1.2f }
                }),

            // Butcesi en dar olan. Sik gelir, ucuz lotlarda rakip cikar.
            new Alici(
                id: "emekli-amca", ad: "Emekli Amca",
                huy: "Pazarlığı sever, cebi dar.",
                butce: 2500, varsayilanCarpan: 0.9f, katilimAgirligi: 8,
                kategoriPrimleri: new Dictionary<Kategori, float>
                {
                    { Kategori.Mobilya, 1.1f }
                }),

            // Tek varsayilan carpani 1'in ustunde olan alici. Nadir gelir;
            // geldigi hafta her sey biraz daha pahaliya gider.
            new Alici(
                id: "yabanci-alici", ad: "Yabancı Alıcı",
                huy: "Fiyatları bilmiyor, bazen fazlasını veriyor.",
                butce: 7000, varsayilanCarpan: 1.3f, katilimAgirligi: 3),

            // docs/tasarim/04-ekonomi.md: rakip firma "arada muzayedede
            // karsina cikip fiyati yukseltir". Ayri bir AI'i yok, alici olarak
            // duruyor.
            new Alici(
                id: "rakip-firma", ad: "Rakip Firma",
                huy: "Senin işini bilir; fiyatı yükseltmeye gelir.",
                butce: 8000, varsayilanCarpan: 1.0f, katilimAgirligi: 4),

            new Alici(
                id: "elektronikci", ad: "Elektronikçi Barış",
                huy: "Ekranı olan her şeye bakar.",
                butce: 7000, varsayilanCarpan: 0.5f, katilimAgirligi: 6,
                kategoriPrimleri: new Dictionary<Kategori, float>
                {
                    { Kategori.Elektronik, 1.7f }
                }),

            new Alici(
                id: "usta", ad: "Usta Kemal",
                huy: "Alet gördü mü durmaz.",
                butce: 4000, varsayilanCarpan: 0.5f, katilimAgirligi: 5,
                kategoriPrimleri: new Dictionary<Kategori, float>
                {
                    { Kategori.Aletler, 1.8f }
                }),

            new Alici(
                id: "kuyumcu", ad: "Kuyumcu Ferit",
                huy: "Altın, gümüş, saat — gerisi tenekedir.",
                butce: 9000, varsayilanCarpan: 0.4f, katilimAgirligi: 4,
                kategoriPrimleri: new Dictionary<Kategori, float>
                {
                    { Kategori.Kisisel, 1.9f },
                    { Kategori.Antika, 1.4f }
                })
        };

        static readonly Dictionary<string, Alici> kimlige = Kur();

        static Dictionary<string, Alici> Kur()
        {
            var sozluk = new Dictionary<string, Alici>(hepsi.Length, StringComparer.Ordinal);
            foreach (var alici in hepsi)
            {
                if (sozluk.ContainsKey(alici.Id))
                    throw new InvalidOperationException("Havuzda tekrarlanan alici id'si: " + alici.Id);

                sozluk.Add(alici.Id, alici);
            }

            return sozluk;
        }

        public static IReadOnlyList<Alici> Hepsi { get { return hepsi; } }

        public static Alici Bul(string id)
        {
            Alici alici;
            if (!kimlige.TryGetValue(id ?? string.Empty, out alici))
                throw new KeyNotFoundException("Havuzda boyle bir alici yok: " + id);

            return alici;
        }

        public static bool VarMi(string id)
        {
            return id != null && kimlige.ContainsKey(id);
        }
    }
}
