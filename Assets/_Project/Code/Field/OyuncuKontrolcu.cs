using UnityEngine;
using UnityEngine.InputSystem;

namespace Oyun.Field
{

    // Birinci sahis yurume ve bakma.
    //
    // M1 kapsami bilerek dar: yalnizca yurume ve bakma. Kosma, egilme ve ziplama
    // icin verilmis bir karar yok (docs/kararlar/ ve docs/plan.md M1 listesi), o
    // yuzden eklenmedi. Girdi varliginda Sprint/Crouch/Jump eylemleri Unity
    // sablonundan geliyor; bagli degiller.
    //
    // Gorsel katman yok: karar 50'ye gore M1'de sadece eller, tam vucut M2'de.
    //
    // CharacterController olculeri sahnede/prefabda ayarlanir ama serbest degil:
    // ev kiti kapi boslugu 0.80 x 2.00 m (docs/uretim/ev-kiti.md, olculerek belirlendi).
    //   - Yukseklik 2.00'in altinda kalmali, yoksa kapidan gecilmez.
    //   - Yaricap 0.40'in altinda kalmali (cap < 0.80). 0.30 makul bir paydir.
    // Kapi genisligi bir oynanis sayisi; degisirse bu kisit da degisir, tek basina
    // degistirilmez.
    [RequireComponent(typeof(CharacterController))]
    public sealed class OyuncuKontrolcu : MonoBehaviour
    {
        [Header("Referanslar")]
        [Tooltip("Kameranin bagli oldugu pivot. Egim burada uygulanir, govde donmez.")]
        [SerializeField] private Transform kameraPivotu;

        [Tooltip("Girdi varligi disaridan verilir. Co-op'ta her oyuncu kendi ornegini alir.")]
        [SerializeField] private InputActionAsset girdiVarligi;

        [Header("Hareket")]
        // Sure M1'in baglayici kisitlarindan biri (docs/tasarim/02-saha.md).
        // Bu deger olculmus degil, baslangic noktasi. Saglamasi gereken kisit:
        // kamyona gidip gelmek hissedilir bir maliyet olmali ama kosusturmaca
        // sikici olmamali. Kesin deger oynanarak ayarlanacak.
        [SerializeField] private float yurumeHizi = 3f;

        // Kosma kati. Olculmus degil. Kisit: kacisi kolaylastirmamali ama
        // bos koridoru gecmeyi angarya olmaktan cikarmali. Sure M1'in
        // baglayici kisiti (02-saha.md), bu sayi onu dogrudan etkiliyor --
        // Hafta 5 denge isinde yeniden bakilacak.
        [SerializeField] private float kosmaKati = 1.6f;

        // Bu agirlikta hiz kati en dusuk degerine iner.
        [SerializeField] private float agirlikSiniri = 40f;

        // Hiz kati bunun altina inmez; yoksa oyuncu yerinde sayar.
        [SerializeField] private float enDusukHizKati = 0.5f;

        // Fizik sabiti, denge sayisi degil.
        [SerializeField] private float yercekimi = -9.81f;

        [Header("Bakma")]
        // Oyuncuya gore degisir; cikis oncesi ayarlar menusune baglanacak (README).
        [SerializeField] private float bakmaHassasiyeti = 0.1f;

        // 90 dereceye kadar acmak kamerayi tam dikeye getirip yon duygusunu bozar.
        [SerializeField] private float egimSiniri = 89f;

        private CharacterController kontrolcu;
        private InputAction hareketEylemi;
        private InputAction bakmaEylemi;
        private InputAction kosmaEylemi;

        /// <summary>Acikken fare bakisi kamerayi degil baska bir seyi cevirir.</summary>
        /// <remarks>
        /// Esya cevirme sirasinda kamera sabit kalmali; yoksa hem esya hem
        /// bakis ayni fare hareketiyle donuyor ve isin icinden cikilmiyor.
        /// </remarks>
        public bool BakisKilitli { get; set; }

        /// <summary>Elde tasinan agirlik, kg. OyuncuEtkilesim yaziyor.</summary>
        /// <remarks>
        /// Etkilesim bileseni bunu set ediyor; kontrolcu esyayi tanimiyor.
        /// Bagimlilik tek yonlu kaliyor: hareket kodu tasima kodunu bilmiyor,
        /// sadece bir sayi aliyor.
        /// </remarks>
        public float TasimaAgirligi { get; set; }

        private float egim;
        private float dikeyHiz;

        private void Awake()
        {
            kontrolcu = GetComponent<CharacterController>();

            // Sahne kurulumu insanda. Referans eksikse sessizce yanlis calismaktansa
            // burada dursun ki hata sahneyi kuran kiside yakalansin.
            if (kameraPivotu == null)
            {
                Debug.LogError($"{nameof(OyuncuKontrolcu)}: kameraPivotu atanmamis.", this);
                enabled = false;
                return;
            }

            if (girdiVarligi == null)
            {
                Debug.LogError($"{nameof(OyuncuKontrolcu)}: girdiVarligi atanmamis.", this);
                enabled = false;
                return;
            }

            // throwIfNotFound: eylem adi degisirse sessiz null yerine acik hata gelsin.
            InputActionMap haritasi = girdiVarligi.FindActionMap("Player", true);
            hareketEylemi = haritasi.FindAction("Move", true);
            bakmaEylemi = haritasi.FindAction("Look", true);
            kosmaEylemi = haritasi.FindAction("Sprint", true);
        }

        private void OnEnable()
        {
            hareketEylemi.Enable();
            bakmaEylemi.Enable();
            kosmaEylemi.Enable();
            ImleciKilitle(true);
        }

        private void OnDisable()
        {
            hareketEylemi.Disable();
            bakmaEylemi.Disable();
            kosmaEylemi.Disable();
            ImleciKilitle(false);
        }

        private void Update()
        {
            Bak();
            Yuru();
        }

        private void Bak()
        {
            if (BakisKilitli)
            {
                return;
            }

            Vector2 delta = bakmaEylemi.ReadValue<Vector2>() * bakmaHassasiyeti;

            // Sapma govdede: yurume yonu bakis yonunu takip etsin.
            transform.Rotate(Vector3.up, delta.x);

            // Egim pivotta: govdeyi egmek CharacterController kapsulunu bozar.
            egim = HareketHesabi.SinirliEgim(egim, delta.y, egimSiniri);
            kameraPivotu.localRotation = Quaternion.Euler(egim, 0f, 0f);
        }

        private void Yuru()
        {
            Vector3 yon = HareketHesabi.DunyaYonu(
                hareketEylemi.ReadValue<Vector2>(),
                transform.eulerAngles.y);

            // Yerdeyken kucuk bir negatif hiz tutuluyor: sifir olursa CharacterController
            // zeminden kopuk sayilip egimli yuzeylerde titriyor.
            if (kontrolcu.isGrounded && dikeyHiz < 0f)
            {
                dikeyHiz = -2f;
            }
            else
            {
                dikeyHiz += yercekimi * Time.deltaTime;
            }

            float kat = HareketHesabi.TasimaHizKati(TasimaAgirligi, agirlikSiniri, enDusukHizKati);

            if (kosmaEylemi.IsPressed())
            {
                kat *= kosmaKati;
            }

            Vector3 hareket = yon * (yurumeHizi * kat);
            hareket.y = dikeyHiz;

            kontrolcu.Move(hareket * Time.deltaTime);
        }

        private static void ImleciKilitle(bool kilitli)
        {
            Cursor.lockState = kilitli ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !kilitli;
        }
    }
}
