using Oyun.Core;
using UnityEngine;

namespace Oyun.Field
{
    // Sahnedeki alinabilir bir esya.
    //
    // Bu bilesen esyanin TURUNU degil ORNEGINI temsil ediyor: ayni buzdolabi
    // tanimindan iki tane olabilir ve ilerideki kondisyon/hasar bilgisi
    // ornege ait olacak. Tanim (tur) EsyaTanimi varliginda, degismez.
    //
    // Veri Code/Core'dan geliyor, buraya kopyalanmiyor. Bir alan gerekirse
    // EsyaVerisi'ne eklenir; burada ikinci bir dogruluk kaynagi acilmaz.
    [RequireComponent(typeof(Rigidbody))]
    public sealed class Esya : MonoBehaviour
    {
        [Tooltip("Esya tanimi varligi (Assets/_Project/Data/Items). Prefab'a Inspector'dan baglanir.")]
        [SerializeField] private EsyaTanimi tanim;

        private EsyaVerisi veri;
        private Rigidbody katiCisim;
        private MeshRenderer[] eleyiciler;
        private MaterialPropertyBlock blok;
        private bool vurgulu;

        /// <summary>Esyanin tur verisi. Tanim atanmamissa null.</summary>
        public EsyaVerisi Veri
        {
            get
            {
                // Tembel kurulum: EsyaTanimi.Veri() her cagrida yeni nesne
                // uretiyor ve dogrulama yapiyor. Her karede isin gormesi
                // gereken bir is degil, bir kez kurulup saklaniyor.
                if (veri == null && tanim != null)
                {
                    veri = tanim.Veri();
                }

                return veri;
            }
        }

        /// <summary>Oyuncuya gosterilecek ad. Tanim yoksa nesne adi.</summary>
        public string Ad
        {
            get { return Veri != null ? Veri.Ad : name; }
        }

        private void Awake()
        {
            katiCisim = GetComponent<Rigidbody>();
            eleyiciler = GetComponentsInChildren<MeshRenderer>();
            blok = new MaterialPropertyBlock();

            if (tanim == null)
            {
                // Sessizce durmasindansa soyle: prefab'i kuran kisi gorsun.
                Debug.LogWarning($"{name}: EsyaTanimi atanmamis, esya verisiz calisiyor.", this);
                return;
            }

            // Kutle veriden geliyor, prefab'dan degil.
            //
            // Neden: agirlik EsyaVerisi'nde tanimli ve tasima zorlugu ile
            // ikiKisiGerekir esigi ona bakiyor. Prefab'da elle girilirse ayni
            // sayi iki yerde durur ve kacinilmaz olarak ayrisir -- PR #10'da
            // olculdu: televizyonun verisi 12 kg derken prefab'in kutlesi 1'di.
            //
            // Prefab kuran kisinin kutleye dokunmasi gerekmiyor.
            if (Veri != null)
            {
                katiCisim.mass = Veri.Agirlik;
            }
        }

        /// <summary>Ele alinirken cagrilir: fizik biraktirilir, elin cocugu olur.</summary>
        /// <remarks>
        /// Fizigi kapatmak sart. Acik kalirsa esya elin icinde carpisip
        /// oyuncuyu itiyor ve kamera titriyor.
        /// </remarks>
        public void EleAlindi(Transform elCapasi)
        {
            katiCisim.isKinematic = true;
            katiCisim.detectCollisions = false;

            transform.SetParent(elCapasi, false);
            transform.localRotation = Quaternion.identity;

            // Tutma yeri esyanin boyutuna gore kayiyor: buyuk esya daha
            // uzakta ve daha asagida tutuluyor.
            //
            // Neden: sabit bir noktada tutulunca 1 m genisligindeki televizyon
            // ekranin yarisini kapatiyordu (31.08'de olculdu). Kucultmek yanlis
            // olurdu -- olculer gercek ve dogru (docs/uretim/esyalar.md).
            // Uzaga itmek hem sorunu cozuyor hem tasarima uyuyor: buyuk esya
            // gorus alanini daraltmali, ama tamamen kor etmemeli.
            transform.localPosition = TutmaYeri();
        }

        /// <summary>Birakilirken cagrilir: fizik geri verilir.</summary>
        public void Birakildi(Vector3 atmaHizi)
        {
            transform.SetParent(null, true);

            katiCisim.isKinematic = false;
            katiCisim.detectCollisions = true;
            katiCisim.linearVelocity = atmaHizi;
        }

        /// <summary>Bakildiginda parlar. Malzeme varligi uretmez.</summary>
        /// <remarks>
        /// MaterialPropertyBlock kullaniliyor cunku alternatifi her esya icin
        /// ayri malzeme varligi uretmek olurdu -- Art klasoru C'nin alani ve
        /// gri kutu asamasinda kalici varlik uretmek yanlis. Blok yalnizca
        /// cizim aninda etki ediyor, diske hicbir sey yazilmiyor.
        ///
        /// Gecici: kalici vurgulama (kenar cizgisi shader'i) C'nin isi.
        /// </remarks>
        public void Vurgula(bool acik)
        {
            if (vurgulu == acik || eleyiciler == null)
            {
                return;
            }

            vurgulu = acik;

            foreach (var eleyici in eleyiciler)
            {
                if (eleyici == null)
                {
                    continue;
                }

                eleyici.GetPropertyBlock(blok);

                if (acik)
                {
                    blok.SetColor(BaseColor, new Color(1f, 0.85f, 0.35f));
                }
                else
                {
                    blok.Clear();
                }

                eleyici.SetPropertyBlock(blok);
            }
        }

        static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        // Esyanin en buyuk boyutuna gore tutma noktasi.
        private Vector3 TutmaYeri()
        {
            float enBuyuk = 0.3f;

            if (eleyiciler != null)
            {
                foreach (var eleyici in eleyiciler)
                {
                    if (eleyici == null)
                    {
                        continue;
                    }

                    Vector3 b = eleyici.bounds.size;
                    enBuyuk = Mathf.Max(enBuyuk, Mathf.Max(b.x, Mathf.Max(b.y, b.z)));
                }
            }

            // Kucuk esya (0.3 m) elin dibinde; buyuk esya (2 m) kol mesafesinde
            // ve daha asagida. Ust sinir var: kanepe boyutunda bir sey
            // sonsuza itilmesin.
            float oran = Mathf.InverseLerp(0.3f, 2.0f, enBuyuk);

            return new Vector3(
                Mathf.Lerp(0.20f, 0.05f, oran),   // buyudukce ortaya kayiyor
                Mathf.Lerp(-0.20f, -0.55f, oran), // ve asagi
                Mathf.Lerp(0.45f, 1.15f, oran));  // ve uzaga
        }
    }
}
