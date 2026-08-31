using UnityEngine;
using UnityEngine.InputSystem;

namespace Oyun.Field
{
    // Bakma ve elde tasima. docs/plan.md M1 Hafta 1 Gun 2 (A).
    //
    // Kapsam bilerek dar: bak, "E" ile al, elde tut, tekrar "E" ile birak.
    // Slot sistemi Gun 3'un, kamyona yukleme Gun 4'un isi. Bugun sadece elde
    // TEK esya var -- bu bir kisitlama degil, karar (docs/tasarim/02-saha.md
    // "Elde tek esya"): kamyona gidip gelmeyi zorunlu kilan sey bu.
    [RequireComponent(typeof(CharacterController))]
    public sealed class OyuncuEtkilesim : MonoBehaviour
    {
        [Header("Referanslar")]
        [Tooltip("Isinin atildigi kamera. Genelde KameraPivotu altindaki Main Camera.")]
        [SerializeField] private Transform kamera;

        [Tooltip("Esyanin tutuldugu capa. Kameranin cocugu olmali ki bakisla birlikte donsun.")]
        [SerializeField] private Transform elCapasi;

        [Tooltip("Girdi varligi disaridan verilir. Co-op'ta her oyuncu kendi ornegini alir.")]
        [SerializeField] private InputActionAsset girdiVarligi;

        [Header("Erisim")]
        // 2 m: oynanarak secildi (31.08). 5 m denendi, odanin oteki
        // ucundan esya alinabiliyordu ve mesafe anlamini kaybediyordu --
        // oysa 02-saha.md mesafeyi bir maliyet sayiyor.
        //
        // Onceki gerekce: oyuncunun bakip da
        // uzanamadigi icin sinirlenmesi gerekmiyor -- bu oyunda zorluk
        // mesafeden degil sureden ve kapasiteden geliyor (02-saha.md).
        // Yani kol boyundan biraz uzun, ama odanin oteki ucundan esya
        // alinacak kadar degil. Kesin deger oynanarak ayarlanacak.
        [SerializeField] private float erisimMesafesi = 2f;

        [Tooltip("Isinin carpacagi katmanlar. Oyuncunun kendisi disarida kalmali.")]
        [SerializeField] private LayerMask erisimKatmanlari = ~0;

        // Ince isin alcak ve ince esyalarda ise yaramiyor: TV 8 cm kalinliginda
        // bir levha, masa 0.75 m yuksekliginde. Goz hizasi 1.65 m'den asagi
        // bakarken isinin o yuzeye denk gelmesi sansa kaliyor -- 31.08'de
        // olculdu, "TV alinmiyor, masa zor aliniyor".
        //
        // Cozum kalinlastirmak. Dunyayi degistirmek yanlis olurdu: mobilya
        // olculeri gercek ve dogru (docs/uretim/esyalar.md).
        [SerializeField] private float nisanYaricapi = 0.30f;

        // Tam nisan almak zorunda kalmamak icin: isin bosa cikarsa yakindaki
        // esyalar arasindan bakis yonune en yakini seciliyor. Bu aci o
        // secimin sinirini koyuyor.
        //
        // Neden: alcak ve ince esyalarda (TV 0.08 m kalinlik, masa 0.75 m)
        // goz hizasindan isin tutturmak beceriye donusuyordu -- 31.08'de
        // olculdu. Zorluk nisandan degil sureden ve kapasiteden gelmeli
        // (docs/tasarim/02-saha.md).
        [SerializeField] private float secimAcisi = 55f;

        // Birakirken hafif bir itme: tam ayagin dibine dusmesin, ama firlatma
        // da olmasin. Firlatma ayri bir mekanik ve M1'de yok.
        [SerializeField] private float birakmaHizi = 1.5f;

        private InputAction etkilesimEylemi;
        private InputAction birakmaEylemi;

        /// <summary>Su anda bakilan esya. Yoksa null. HUD bunu okur.</summary>
        public Esya BakilanEsya { get; private set; }

        /// <summary>Elde tutulan esya. Yoksa null.</summary>
        public Esya ElindekiEsya { get; private set; }

        /// <summary>Bakilan esya alinamiyorsa sebebi. HUD bunu gosterir.</summary>
        public AlmaSonucu SonDegerlendirme { get; private set; }


        private void Awake()
        {
            if (kamera == null)
            {
                Debug.LogError($"{nameof(OyuncuEtkilesim)}: kamera atanmamis.", this);
                enabled = false;
                return;
            }

            if (elCapasi == null)
            {
                Debug.LogError($"{nameof(OyuncuEtkilesim)}: elCapasi atanmamis.", this);
                enabled = false;
                return;
            }

            if (girdiVarligi == null)
            {
                Debug.LogError($"{nameof(OyuncuEtkilesim)}: girdiVarligi atanmamis.", this);
                enabled = false;
                return;
            }

            InputActionMap haritasi = girdiVarligi.FindActionMap("Player", true);
            etkilesimEylemi = haritasi.FindAction("Interact", true);
            birakmaEylemi = haritasi.FindAction("Birak", true);
        }

        private void OnEnable()
        {
            etkilesimEylemi.Enable();

            // performed DEGIL started.
            //
            // Interact eyleminde Unity sablonundan gelen bir "Hold" etkilesimi
            // var: performed ancak tus 0.4 sn basili tutulunca tetikleniyor,
            // normal basista hic tetiklenmiyor. 31.08'de olculdu -- etiket
            // cikiyor, E hicbir sey yapmiyordu.
            //
            // started basista aninda tetikleniyor ve Hold varliginda da
            // calisiyor. Paylasilan girdi varligina dokunmuyorum: "Hold"
            // ileride gizli deger aramasi icin isimize yarayabilir
            // (docs/tasarim/02-saha.md "arama sure yer"), silmek yerine
            // baypas etmek geri donusu kolay olan.
            etkilesimEylemi.started += Etkiles;

            birakmaEylemi.Enable();
            birakmaEylemi.started += BirakmaBasildi;
        }

        private void OnDisable()
        {
            etkilesimEylemi.started -= Etkiles;
            etkilesimEylemi.Disable();

            birakmaEylemi.started -= BirakmaBasildi;
            birakmaEylemi.Disable();
        }

        private void Update()
        {
            Bak();
        }

        // Her karede bakilan esyayi tazeler. Sonuc HUD icin saklaniyor;
        // bu bilesen ekrana bir sey cizmiyor -- cizim Code/UI'nin isi.
        private void Bak()
        {
            // Elde esya varken bakmaya devam etmenin anlami yok: tek esya
            // tasiniyor ve elin doluyken yeni esya alinamiyor.
            if (ElindekiEsya != null)
            {
                VurguyuBirak();
                SonDegerlendirme = AlmaSonucu.ElDolu;
                return;
            }

            Esya yeni = EnUygunEsya();

            if (yeni != BakilanEsya)
            {
                if (BakilanEsya != null) BakilanEsya.Vurgula(false);
                if (yeni != null) yeni.Vurgula(true);
            }

            BakilanEsya = yeni;

            if (BakilanEsya != null)
            {
                SonDegerlendirme = EtkilesimKurallari.Degerlendir(BakilanEsya.Veri, false);
            }
        }


        // Once ince isin, tutmazsa kalin kure.
        //
        // Sira onemli: ince isin tam nisan aldiginda dogru esyayi secer --
        // yan yana duran iki esyadan hangisine baktigin belirsiz kalmaz.
        // Kure yalnizca ince isin bosa cikinca devreye giriyor, yani
        // hedefleme affediciligi kesinligi bozmuyor.
        //
        // QueryTriggerInteraction.Ignore: tetikleyiciler (kapi bolgesi, oda
        // sinirlari) esya degil, isini kesmemeliler.

        // Her kademe ayni olcuyu kullanmali: kamera ile esyanin EN YAKIN
        // YUZEY NOKTASI arasindaki mesafe.
        //
        // Neden: onceden isin carpma noktasina, koni ise carpisan kutusunun
        // tasmasina bakiyordu. Sonuc, buyuk esyanin erken yakalanmasi, kucuk
        // esyaya sokulmak zorunda kalinmasiydi -- "her esyayi ayni uzakliktan
        // alabileyim" (31.08). Yuzey noktasi hem boyuttan bagimsiz hem de
        // oyuncunun hissettigi sey: esyaya ne kadar yakinim.
        private bool ErisimIcinde(Collider carpisan)
        {
            Vector3 enYakin = carpisan.ClosestPoint(kamera.position);
            return Vector3.Distance(kamera.position, enYakin) <= erisimMesafesi;
        }

        // Uc kademeli secim: tam nisan > kalin nisan > yakindaki en uygun.
        //
        // Sira onemli. Tam nisan aldiginda ne sectigin belirsiz kalmasin diye
        // ince isin once deneniyor. Kademe indikce affedicilik artiyor.
        private Esya EnUygunEsya()
        {
            var isin = new Ray(kamera.position, kamera.forward);

            if (Physics.Raycast(isin, out RaycastHit vurus, erisimMesafesi,
                    erisimKatmanlari, QueryTriggerInteraction.Ignore))
            {

                var dogrudan = vurus.collider.GetComponentInParent<Esya>();
                if (dogrudan != null && ErisimIcinde(vurus.collider))
                {
                    return dogrudan;
                }
            }
            // Kure kameranin biraz gerisinden baslatiliyor: tam kamerada
            // baslarsa oyuncunun kapsulunun icinde dogar.
            Vector3 geri = kamera.position - kamera.forward * nisanYaricapi;

            if (Physics.SphereCast(geri, nisanYaricapi, kamera.forward, out RaycastHit kureVurus,
                    erisimMesafesi, erisimKatmanlari, QueryTriggerInteraction.Ignore))
            {
                var kalin = kureVurus.collider.GetComponentInParent<Esya>();
                if (kalin != null && ErisimIcinde(kureVurus.collider))
                {
                    return kalin;
                }
            }

            return YakindakiEnIyi();
        }

        // Yakindaki esyalar arasindan bakis yonune en yakin olani secer.
        private Esya YakindakiEnIyi()
        {
            Collider[] yakinlar = Physics.OverlapSphere(kamera.position, erisimMesafesi * 2f,
                erisimKatmanlari, QueryTriggerInteraction.Ignore);

            Esya enIyi = null;
            float enIyiAci = secimAcisi;

            foreach (var carpisan in yakinlar)
            {
                var aday = carpisan.GetComponentInParent<Esya>();
                if (aday == null || aday == ElindekiEsya)
                {
                    continue;
                }

                if (!ErisimIcinde(carpisan))
                {
                    continue;
                }

                Vector3 yon = carpisan.bounds.center - kamera.position;
                float aci = Vector3.Angle(kamera.forward, yon);

                if (aci >= enIyiAci)
                {
                    continue;
                }

                // Duvarin ardindaki esya alinmamali. Kucuk pay birakiliyor ki
                // isin esyanin kendi yuzeyine carpip "engellendi" sanilmasin.
                float uzaklik = yon.magnitude;
                if (uzaklik > 0.1f
                    && Physics.Raycast(kamera.position, yon.normalized, out RaycastHit engel,
                        uzaklik - 0.05f, erisimKatmanlari, QueryTriggerInteraction.Ignore)
                    && engel.collider.GetComponentInParent<Esya>() != aday)
                {
                    continue;
                }

                enIyi = aday;
                enIyiAci = aci;
            }

            if (enIyi != null)
            {
            }

            return enIyi;
        }

        // "E" yalnizca alir. Birakma "Q"ya tasindi: tek tusla al/birak
        // yapilinca birakilan esya onune dusuyor ve ayni tus onu hemen geri
        // aliyordu -- birakmamis gibi hissediliyor (31.08'de olculdu).
        private void Etkiles(InputAction.CallbackContext baglam)
        {
            if (ElindekiEsya != null)
            {
                return;
            }

            if (BakilanEsya == null)
            {
                return;
            }

            AlmaSonucu sonuc = EtkilesimKurallari.Degerlendir(BakilanEsya.Veri, false);
            SonDegerlendirme = sonuc;

            if (sonuc != AlmaSonucu.Alinabilir)
            {
                // Gecici: kalici geri bildirim Code/UI'nin isi (Gun 5 sonrasi).
                Debug.Log($"{BakilanEsya.Ad}: {EtkilesimKurallari.Aciklama(sonuc)}");
                return;
            }

            Al(BakilanEsya);
        }

        private void BirakmaBasildi(InputAction.CallbackContext baglam)
        {
            if (ElindekiEsya != null)
            {
                Birak();
            }
        }

        private void VurguyuBirak()
        {
            if (BakilanEsya != null)
            {
                BakilanEsya.Vurgula(false);
                BakilanEsya = null;
            }
        }

        private void Al(Esya esya)
        {
            esya.Vurgula(false);
            esya.EleAlindi(elCapasi);
            ElindekiEsya = esya;
            BakilanEsya = null;
        }

        private void Birak()
        {
            Esya esya = ElindekiEsya;
            ElindekiEsya = null;

            esya.Birakildi(kamera.forward * birakmaHizi);
        }

        // GECICI hata ayiklama ekrani. Kalici HUD Code/UI'nin isi (B).
        //
        // Neden simdilik burada: geri bildirim olmadan mekanik bozuk gorunuyor.
        // "Alamiyorum" ile "alamam cunku iki kisi gerekiyor" arasindaki fark
        // oyuncuya gosterilmezse kural degil hata gibi hissediliyor -- 31.08'de
        // yasandi. Code/UI gelince bu blok silinecek.
        private void OnGUI()
        {
            const int Y = 12;
            var stil = new GUIStyle(GUI.skin.label) { fontSize = 18 };

            // Nisangah: neye bakildigi belli olsun.
            float ortaX = Screen.width * 0.5f;
            float ortaY = Screen.height * 0.5f;
            GUI.Label(new Rect(ortaX - 6f, ortaY - 12f, 20f, 24f), "+", stil);

            if (ElindekiEsya != null)
            {
                GUI.Label(new Rect(Y, Y, 600f, 30f),
                    $"Elinde: {ElindekiEsya.Ad}   [Q] birak", stil);
                return;
            }

            if (BakilanEsya == null)
            {
                return;
            }

            AlmaSonucu sonuc = EtkilesimKurallari.Degerlendir(BakilanEsya.Veri, false);
            string metin = sonuc == AlmaSonucu.Alinabilir
                ? $"{BakilanEsya.Ad}   [E] al"
                : $"{BakilanEsya.Ad}   -- {EtkilesimKurallari.Aciklama(sonuc)}";

            stil.normal.textColor = sonuc == AlmaSonucu.Alinabilir ? Color.white : new Color(1f, 0.7f, 0.4f);
            GUI.Label(new Rect(Y, Y, 600f, 30f), metin, stil);
        }
    }
}
