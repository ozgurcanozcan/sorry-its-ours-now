using UnityEngine;

namespace Oyun.Core
{
    // EsyaVerisi'nin Unity varligi olarak hali. Prefab'lar buna referans veriyor
    // (Onat'in Inspector akisi); degerler EsyaKatalogu'ndan uretiliyor.
    //
    // Bu varliklari ELLE DUZENLEME. Dogruluk kaynagi EsyaKatalogu; elle yapilan
    // degisiklik bir sonraki uretimde silinir ve o ana kadar da testi kirar.
    // Bir sayiyi degistirmek gerekiyorsa katalogda degistir, sonra
    // "Oyun / Esya varliklarini uret" menusunu calistir.
    [CreateAssetMenu(fileName = "Esya", menuName = "Oyun/Esya Tanimi")]
    public sealed class EsyaTanimi : ScriptableObject
    {
        [SerializeField] string id;
        [SerializeField] string ad;
        [SerializeField] Kategori kategori;
        [SerializeField] int tabanDeger;
        [SerializeField] float agirlik;
        [SerializeField] float hacim;
        [SerializeField] bool kirilgan;
        [SerializeField] bool haczedilebilir;
        [SerializeField] bool ikiKisiGerekir;
        [SerializeField] bool cebeGirer;
        [SerializeField] bool sabit;
        [SerializeField] int bolluk;

        public string Id { get { return id; } }

        /// <summary>Varliktaki degerlerden bir EsyaVerisi kurar.</summary>
        /// <remarks>
        /// EsyaVerisi'nin yapicisi dogrulama yapiyor: varlik bozuksa burada patlar.
        /// Sessizce yanlis davranan bir esya uretmektense burada durmak iyi.
        /// </remarks>
        public EsyaVerisi Veri()
        {
            return new EsyaVerisi(id, ad, kategori, tabanDeger, agirlik, hacim,
                kirilgan, haczedilebilir, ikiKisiGerekir, cebeGirer, sabit, bolluk);
        }

#if UNITY_EDITOR
        /// <summary>Uretici script'i icin. Calisma zamaninda cagrilamaz.</summary>
        public void KatalogdanYaz(EsyaVerisi veri)
        {
            id = veri.Id;
            ad = veri.Ad;
            kategori = veri.Kategori;
            tabanDeger = veri.TabanDeger;
            agirlik = veri.Agirlik;
            hacim = veri.Hacim;
            kirilgan = veri.Kirilgan;
            haczedilebilir = veri.Haczedilebilir;
            ikiKisiGerekir = veri.IkiKisiGerekir;
            cebeGirer = veri.CebeGirer;
            sabit = veri.Sabit;
            bolluk = veri.Bolluk;
        }
#endif
    }
}
