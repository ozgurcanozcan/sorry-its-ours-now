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
        private Transform elCapasi;
        private Vector3 tutmaOfseti;

        // Elin donusune eklenen ofset. Oyuncu esyayi cevirdiginde bu birikiyor,
        // yani cevirdigin aci sen donunce de korunuyor -- kanepeyi yan cevirip
        // kapiya dogru yurumek boyle mumkun oluyor.
        private Quaternion tutmaDonusu = Quaternion.identity;

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

        /// <summary>Ele alinirken cagrilir. Esya fizik nesnesi olarak KALIR.</summary>
        /// <remarks>
        /// Onceden fizik kapatilip esya elin cocugu yapiliyordu; o zaman esya
        /// duvarlarin icinden geciyordu. Bu sadece bir kusur degil, tasarimi
        /// olduruyordu: docs/tasarim/02-saha.md "kanepeyi kapidan gecirmeye
        /// calismak bu oyunun gorsel sakasi" diyor. Esya duvardan geciyorsa
        /// dar kapi bir engel olmaktan cikar.
        ///
        /// Simdi esya dinamik kaliyor ve her fizik adiminda ele dogru
        /// cekiliyor. Duvara carparsa orada duruyor, el ilerlese bile.
        /// Yercekimi kapatiliyor -- yoksa elde surekli asagi sarkar.
        /// </remarks>
        public void EleAlindi(Transform capa)
        {
            elCapasi = capa;

            // Esyanin o anki dunya donusunu koru: yerde yan yatiyorsa elde de
            // yan baslasin, birden dikilmesin.
            tutmaDonusu = Quaternion.Inverse(capa.rotation) * transform.rotation;
            tutmaOfseti = TutmaYeri();

            transform.SetParent(null, true);

            katiCisim.isKinematic = false;
            katiCisim.detectCollisions = true;
            katiCisim.useGravity = false;

            // Sonumleme yuksek: esya ele varinca salinmasin, titremesin.
            katiCisim.linearDamping = 12f;
            katiCisim.angularDamping = 12f;
            katiCisim.interpolation = RigidbodyInterpolation.Interpolate;
            katiCisim.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        // Esyayi ele dogru ceker. FixedUpdate cunku fizik nesnesi.
        private void FixedUpdate()
        {
            if (elCapasi == null)
            {
                return;
            }

            Vector3 hedef = elCapasi.TransformPoint(tutmaOfseti);
            Vector3 fark = hedef - katiCisim.position;

            Vector3 istenenHiz = Vector3.ClampMagnitude(
                fark / Time.fixedDeltaTime * CekmeGucu, EnYuksekTasimaHizi);

            // Hizi DOGRUDAN set etmiyoruz, kuvvet uyguluyoruz.
            //
            // Neden: hiz set edilince esya durdurulamaz bir pistona donusuyor
            // ve carptigi seyin kutlesi hic hesaba katilmiyordu -- elde 12 kg'lik
            // televizyonla 80 kg'lik gardirobu itebiliyordun (31.08'de olculdu).
            //
            // Kuvvetle: her esya en fazla EnYuksekIvme kadar ivmelenebiliyor,
            // yani uygulayabildigi kuvvet kendi kutlesiyle orantili. Baska bir
            // cisme carpinca ivme (m * a) / (m + M) oluyor:
            //   hafif esya + agir cisim -> neredeyse hic itmiyor
            //   agir esya + hafif cisim -> rahat itiyor
            // Istenen davranis fizikten kendiliginde cikiyor, ayri kural yok.
            Vector3 hizFarki = istenenHiz - katiCisim.linearVelocity;
            Vector3 kuvvet = hizFarki * katiCisim.mass / Time.fixedDeltaTime;

            katiCisim.AddForce(
                Vector3.ClampMagnitude(kuvvet, katiCisim.mass * EnYuksekIvme));

            // Donusu ele uyduruyoruz; carpisma donusu bozarsa kendini toparlar.
            katiCisim.MoveRotation(Quaternion.Slerp(
                katiCisim.rotation, elCapasi.rotation * tutmaDonusu, DonusYumusatma));
        }

        // Ceken kuvvetin sertligi. 1 = tek adimda yetis, 0 = hic yetisme.
        // Dusuk deger esyayi "agir" hissettiriyor ama duvara carpinca daha
        // yumusak duruyor.
        private const float CekmeGucu = 0.35f;

        // Ustunde esya ele ulasmak icin duvarlardan gecmeye calisir.
        private const float EnYuksekTasimaHizi = 8f;

        // Bir esyanin ulasabilecegi en yuksek ivme (m/s^2). Yercekiminin
        // yaklasik iki kati: esya elde tasinabilecek kadar cevik, ama kendinden
        // agir bir cismi itebilecek kadar degil.
        //
        // Olculmus degil. Saglamasi gereken kisit: en agir esya (gardirop,
        // 80 kg) elde tasinabilmeli; en hafif esya (hali, 8 kg) en agiri
        // yerinden oynatamamali.
        private const float EnYuksekIvme = 20f;

        private const float DonusYumusatma = 0.25f;

        /// <summary>Cebe/slota konurken cagrilir: gorunmez olur, tasiyiciya baglanir.</summary>
        /// <remarks>
        /// Elde tasimadan farki gorunurluk. Slottaki esya cepte duruyor, ekranda
        /// gorunmuyor -- ama sahneden silinmiyor: uzerindeki durum (ileride
        /// kondisyon, hasar) ornekle birlikte yasamali. Yok edip yeniden
        /// uretmek o bilgiyi kaybettirirdi.
        /// </remarks>
        public void SlotaKondu(Transform tasiyici)
        {
            elCapasi = null;
            katiCisim.isKinematic = true;
            katiCisim.detectCollisions = false;

            transform.SetParent(tasiyici, false);
            transform.localPosition = Vector3.zero;

            foreach (var eleyici in eleyiciler)
            {
                if (eleyici != null)
                {
                    eleyici.enabled = false;
                }
            }
        }

        /// <summary>Birakilirken cagrilir: fizik geri verilir.</summary>
        public void Birakildi(Vector3 atmaHizi)
        {
            elCapasi = null;
            // Slottan cikiyorsa gorunurlugu geri ver.
            foreach (var eleyici in eleyiciler)
            {
                if (eleyici != null)
                {
                    eleyici.enabled = true;
                }
            }

            transform.SetParent(null, true);

            katiCisim.isKinematic = false;
            katiCisim.detectCollisions = true;
            katiCisim.useGravity = true;
            katiCisim.linearDamping = 0f;
            katiCisim.angularDamping = 0.05f;
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

        /// <summary>Elde tutulan esyayi cevirir. Aci birikiyor, kalici.</summary>
        /// <remarks>
        /// Dunya eksenlerinde donduruluyor, esyanin kendi eksenlerinde degil:
        /// oyuncu ekranda gordugu yone gore cevirmek istiyor, esyanin ic
        /// koordinat sistemini dusunmek zorunda kalmamali.
        /// </remarks>
        public void Cevir(Quaternion dunyaDonusu)
        {
            if (elCapasi == null)
            {
                return;
            }

            tutmaDonusu = Quaternion.Inverse(elCapasi.rotation) * dunyaDonusu
                          * elCapasi.rotation * tutmaDonusu;
        }
    }
}
