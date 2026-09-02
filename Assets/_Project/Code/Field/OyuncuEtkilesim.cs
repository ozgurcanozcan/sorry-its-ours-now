using System.Collections.Generic;
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

        // Bu mesafenin altinda koni genisliyor (bkz. YakindakiEnIyi).
        private const float YakinMesafe = 1.5f;

        // Yakindaki en genis koni. 100 derece: ayaginin dibindeki esyayi
        // duz bakarken kapsiyor ama arkani donmeden yandakini almiyor.
        private const float YakinKoni = 100f;

        // Kucuk esyanin secim avantaji, derece.
        //
        // 30 denendi ve fazla geldi: masaya nisan alsan bile yanindaki
        // telefon kazaniyordu (01.09). 12 yeterli -- masanin ustundeki
        // telefonu kazandiriyor ama masaya dogrudan bakinca masa geliyor.
        private const float KucukEsyaAvantaji = 12f;

        // Birakirken hafif bir itme: tam ayagin dibine dusmesin, ama firlatma
        // da olmasin. Firlatma ayri bir mekanik ve M1'de yok.
        [SerializeField] private float birakmaHizi = 1.5f;

        // Esya cevirme hassasiyeti. Bakma hassasiyetinden bagimsiz: esyayi
        // ceviren hareket daha kucuk ve kontrollu olmali, bakis gibi hizli
        // degil -- dar kapiya hizalamaya calisirken ince ayar gerekiyor.
        [SerializeField] private float cevirmeHassasiyeti = 0.4f;

        private CharacterController kapsul;
        private InputAction etkilesimEylemi;
        private InputAction birakmaEylemi;
        private InputAction dondurmeEylemi;
        private InputAction bakmaEylemi;
        private OyuncuKontrolcu kontrolcu;

        /// <summary>Su anda bakilan esya. Yoksa null. HUD bunu okur.</summary>
        public Esya BakilanEsya { get; private set; }

        /// <summary>Elde tutulan esya. Yoksa null.</summary>
        public Esya ElindekiEsya { get; private set; }

        // Slottaki esyalar. Liste, dizi degil: slotlar Hafta 2'de aletlerle
        // paylasilacak ve alet cikarilinca aradaki bosluk kapanmali.
        private readonly List<Esya> slotlar = new List<Esya>();

        /// <summary>Dolu slot sayisi. Hafta 2'de aletler de buraya girecek.</summary>
        public int DoluSlot { get { return slotlar.Count; } }

        /// <summary>Bakilan esya alinamiyorsa sebebi. HUD bunu gosterir.</summary>
        public AlmaSonucu SonDegerlendirme { get; private set; }


        private void Awake()
        {
            kapsul = GetComponent<CharacterController>();

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
            dondurmeEylemi = haritasi.FindAction("Dondur", true);
            bakmaEylemi = haritasi.FindAction("Look", true);
            kontrolcu = GetComponent<OyuncuKontrolcu>();
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
            dondurmeEylemi.Enable();
            bakmaEylemi.Enable();
            birakmaEylemi.started += BirakmaBasildi;
        }

        private void OnDisable()
        {
            etkilesimEylemi.started -= Etkiles;
            etkilesimEylemi.Disable();

            birakmaEylemi.started -= BirakmaBasildi;
            birakmaEylemi.Disable();
            dondurmeEylemi.Disable();
            bakmaEylemi.Disable();

            if (kontrolcu != null) kontrolcu.BakisKilitli = false;
        }

        private void Update()
        {
            Cevirme();
            Bak();
            KopanEsyayiBirak();
        }

        // "R" basili tutulurken fare esyayi cevirir, kamerayi degil.
        //
        // Neden basili tutma: ayri bir kip degil. Birakinca aninda bakmaya
        // donuyorsun, esyayi hizalayip yurumeye devam ediyorsun. Kip olsaydi
        // her seferinde girip cikmak gerekirdi.
        private void Cevirme()
        {
            bool cevirilebilir = ElindekiEsya != null && dondurmeEylemi.IsPressed();

            if (kontrolcu != null)
            {
                kontrolcu.BakisKilitli = cevirilebilir;
            }

            if (!cevirilebilir)
            {
                return;
            }

            Vector2 delta = bakmaEylemi.ReadValue<Vector2>() * cevirmeHassasiyeti;

            if (delta.sqrMagnitude < 0.0001f)
            {
                return;
            }

            // Kameranin eksenlerinde donduruluyor: fareyi saga itince esya
            // ekranda saga doner, esyanin kendi ekseni ne olursa olsun.
            Quaternion donus = Quaternion.AngleAxis(delta.x, kamera.up)
                             * Quaternion.AngleAxis(-delta.y, kamera.right);

            ElindekiEsya.Cevir(donus);
        }

        // Duvarin ardinda sikisan esya elden kopar.
        //
        // Neden: esya artik duvarlara carpiyor, yani el ilerlerken esya geride
        // kalabiliyor. Bagi koparmazsak oyuncu esyayi gormeden surukler ve
        // kose donunce lastik gibi firlar.
        private void KopanEsyayiBirak()
        {
            if (ElindekiEsya == null)
            {
                return;
            }

            float uzaklik = Vector3.Distance(ElindekiEsya.transform.position, elCapasi.position);

            if (uzaklik > KopmaMesafesi)
            {
                Birak();
            }
        }

        // Elden kopma mesafesi. Tutma noktasi esya boyutuna gore 1.15 m'ye
        // kadar uzayabiliyor; esik onun uzerinde olmali yoksa buyuk esya
        // taminca kendiliginden dusurulur.
        // Agir esya ele yavas yetisiyor; esik dar olursa yururken kendiliginden
        // dusuyordu (31.08'de olculdu). Oyuncu tasirken zaten yavasladigi icin
        // 2.5 m yeterli pay birakiyor.
        private const float KopmaMesafesi = 2.5f;

        // Her karede bakilan esyayi tazeler. Sonuc HUD icin saklaniyor;
        // bu bilesen ekrana bir sey cizmiyor -- cizim Code/UI'nin isi.
        private void Bak()
        {
            // Elde esya varken de bakmaya devam ediliyor: kucuk esya cebe
            // girer, el dolu olsa bile alinabilir (docs/tasarim/02-saha.md).
            Esya yeni = EnUygunEsya();

            if (yeni != BakilanEsya)
            {
                if (BakilanEsya != null) BakilanEsya.Vurgula(false);
                if (yeni != null) yeni.Vurgula(true);
            }

            BakilanEsya = yeni;

            if (BakilanEsya != null)
            {
                SonDegerlendirme = EtkilesimKurallari.Degerlendir(BakilanEsya.Veri, ElindekiEsya != null, DoluSlot);
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
            // Puanlama kademesi: yakindaki adaylar arasindan bakisa en yakin.
            Esya puanli = YakindakiEnIyi();
            if (puanli != null)
            {
                return puanli;
            }

            // SON CARE: kalin kure.
            //
            // Sirasi onemli. Once denendiginde masa gibi genis yuzeyleri
            // hep once yakaliyor ve ustundeki kucuk esyayi golgeliyordu.
            // En sona alininca ters etkisi kalkiyor ama faydasi duruyor:
            // ince ve buyuk esyalari (TV 8 cm levha) hicbir kademe
            // yakalayamazsa bu yakaliyor. 01.09'da olculdu -- kure
            // kaldirilinca televizyon ve masa cok zorlasti.
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

            return null;
        }

        // Yakindaki esyalar arasindan bakis yonune en yakin olani secer.
        private Esya YakindakiEnIyi()
        {
            Collider[] yakinlar = Physics.OverlapSphere(kamera.position, erisimMesafesi * 2f,
                erisimKatmanlari, QueryTriggerInteraction.Ignore);

            Esya enIyi = null;
            float enIyiAci = float.MaxValue;

            foreach (var carpisan in yakinlar)
            {
                var aday = carpisan.GetComponentInParent<Esya>();
                if (aday == null || aday == ElindekiEsya || slotlar.Contains(aday))
                {
                    continue;
                }

                if (!ErisimIcinde(carpisan))
                {
                    continue;
                }

                Vector3 yon = carpisan.bounds.center - kamera.position;
                float aci = Vector3.Angle(kamera.forward, yon);

                // Yakindaki esyada koni genisliyor.
                //
                // Neden: goz hizasi 1.65 m. Yerdeki bir esyaya 0.5 m mesafeden
                // duz bakarsan aradaki aci 73 derece -- sabit 55 derecelik koni
                // onu disarida birakiyor ve ayaginin dibindeki saati alamiyorsun
                // (01.09'da olculdu). Uzaklastikca aci dogal olarak kuculuyor,
                // yani genis koni sadece yakinda gerekiyor.
                //
                // Uzakta dar kalmasi da onemli: genis koni her yerde acik olsa
                // arkani donmeden yandaki esyayi alirdin.
                float mesafe = yon.magnitude;
                float buKoni = mesafe < YakinMesafe
                    ? Mathf.Lerp(YakinKoni, secimAcisi, mesafe / YakinMesafe)
                    : secimAcisi;

                if (aci >= buKoni)
                {
                    continue;
                }

                // Kucuk esyalar oncelikli.
                //
                // Neden: masanin ustundeki telefona bakarken masa da aday ve
                // gorus merkezine daha yakin -- aci yarisini masa kazaniyor,
                // oyuncu telefon yerine masayi aliyor. Oysa niyet belli:
                // ortada kucuk esya varsa oyuncu onu istiyor.
                //
                // Avantaj aciyi tamamen ezmiyor: telefon gorus alanindan
                // cikinca masa yine kazaniyor.
                float puan = aci;
                if (EtkilesimKurallari.SlotaGirer(aday.Veri))
                {
                    puan -= KucukEsyaAvantaji;
                }

                if (puan >= enIyiAci)
                {
                    continue;
                }

                // Duvarin ardindaki esya alinmamali.
                //
                // Pay esyanin kendi boyutuna gore: kucuk esya yerde durdugunda
                // merkezi zemine cok yakin oluyor ve isin esyaya varmadan
                // zemine carpip "engellendi" saniliyordu. 5 cm'lik kol saati
                // bu yuzden hic alinamiyordu (01.09'da olculdu).
                //
                // Pay olarak esyanin en buyuk yari boyutu kullaniliyor: isin
                // esyanin sinirina varinca kesiliyor, iceri girmiyor.
                float uzaklik = yon.magnitude;
                Vector3 yariBoyut = carpisan.bounds.extents;
                float pay = Mathf.Max(0.08f, Mathf.Max(yariBoyut.x, Mathf.Max(yariBoyut.y, yariBoyut.z)));

                // BASKA BIR ESYA ENGEL SAYILMAZ.
                //
                // Onceden "isin hedefe varmadan baska bir seye carptiysa
                // engellenmistir" deniyordu. Ama masa da bir Esya: telefon
                // masanin ustunde dururken isin masaya degiyor ve telefon
                // elenmis oluyordu. Yerdeyken de ayni sey zeminle oluyordu.
                // 01.09'da olculdu: "telefonu hic alamiyorum".
                //
                // Engel sadece duvar/zemin gibi esya olmayan seyler olmali.
                // Yan etkisi kabul edilebilir: gardirobun ardindaki kucuk
                // esya gorunmese de secilebilir -- ama zaten erisim 2 m ve
                // gardirop o mesafede one cikmis olur.
                if (uzaklik > pay
                    && Physics.Raycast(kamera.position, yon.normalized, out RaycastHit engel,
                        uzaklik - pay, erisimKatmanlari, QueryTriggerInteraction.Ignore)
                    && engel.collider.GetComponentInParent<Esya>() == null)
                {
                    continue;
                }

                enIyi = aday;
                enIyiAci = puan;
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
            if (BakilanEsya == null)
            {
                return;
            }

            AlmaSonucu sonuc = EtkilesimKurallari.Degerlendir(BakilanEsya.Veri, ElindekiEsya != null, DoluSlot);
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
            Birak();
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

            if (EtkilesimKurallari.SlotaGirer(esya.Veri))
            {
                esya.SlotaKondu(transform);
                slotlar.Add(esya);
                AgirligiBildir();
            }
            else
            {
                esya.EleAlindi(elCapasi);
                ElindekiEsya = esya;

                // Esya artik gercek fizik nesnesi ve elin onunde duruyor.
                // Oyuncuyla carpismasi engellenmezse kendini itip oyuncuyu
                // sahnede kaydiriyor ve kamera titriyor.
                CarpismayiAyarla(esya, true);
                AgirligiBildir();
            }

            BakilanEsya = null;
        }

        // Once eldeki birakiliyor, el bosalinca slottakiler.
        //
        // Neden bu sira: elde tutulan esya gorunuyor ve oyuncunun aklinda olan
        // o. Once cepten bir sey dusurmek surpriz olurdu.
        private void Birak()
        {
            Esya esya;

            if (ElindekiEsya != null)
            {
                esya = ElindekiEsya;
                ElindekiEsya = null;
            }
            else if (slotlar.Count > 0)
            {
                esya = slotlar[slotlar.Count - 1];
                slotlar.RemoveAt(slotlar.Count - 1);

                // Slottaki esya oyuncunun icinde duruyordu; onune tasi ki
                // ayaginin dibinde belirip carpismasin.
                esya.transform.position = kamera.position + kamera.forward * 0.6f;
            }
            else
            {
                return;
            }

            CarpismayiAyarla(esya, false);
            esya.Birakildi(kamera.forward * birakmaHizi);
            AgirligiBildir();
        }

        // Tasinan agirligi hareket koduna bildirir.
        //
        // Sadece eldeki sayiliyor: cepteki kucuk esyalar (en fazla birkac kg)
        // yurumeyi yavaslatacak kadar agir degil ve slot sistemi zaten ayri
        // bir kapasite. Aletler geldiginde (Hafta 2) yeniden bakilmali.
        private void AgirligiBildir()
        {
            if (kontrolcu == null)
            {
                return;
            }

            kontrolcu.TasimaAgirligi =
                ElindekiEsya != null && ElindekiEsya.Veri != null
                    ? ElindekiEsya.Veri.Agirlik
                    : 0f;

            // Karar 79: elde YA DA slotta esya varken kosma kapali.
            kontrolcu.Tasiyor = ElindekiEsya != null || DoluSlot > 0;
        }

        private void CarpismayiAyarla(Esya esya, bool yoksay)
        {
            if (kapsul == null)
            {
                return;
            }

            foreach (var carpisan in esya.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(carpisan, kapsul, yoksay);
            }
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

            // Tasima durumu: el + slotlar.
            // Cep gostergesi sayi degil, dolu/bos kareler: bir bakista kac
            // yer kaldigi goruluyor, okumaya gerek kalmiyor.
            var cep = new System.Text.StringBuilder(EtkilesimKurallari.SlotSayisi);
            for (int i = 0; i < EtkilesimKurallari.SlotSayisi; i++)
            {
                cep.Append(i < DoluSlot ? "[+]" : "[ ]");
            }

            string el = ElindekiEsya != null ? ElindekiEsya.Ad : "-";
            GUI.Label(new Rect(Y, Y, 800f, 30f), $"El  {el}{(ElindekiEsya != null ? "    [R] tut ve cevir" : "")}", stil);
            GUI.Label(new Rect(Y, Y + 26f, 800f, 30f), $"Cep {cep}", stil);

            if (BakilanEsya == null)
            {
                return;
            }

            AlmaSonucu sonuc = EtkilesimKurallari.Degerlendir(BakilanEsya.Veri, ElindekiEsya != null, DoluSlot);
            string metin = sonuc == AlmaSonucu.Alinabilir
                ? $"{BakilanEsya.Ad}   [E] al"
                : $"{BakilanEsya.Ad}   -- {EtkilesimKurallari.Aciklama(sonuc)}";

            stil.normal.textColor = sonuc == AlmaSonucu.Alinabilir ? Color.white : new Color(1f, 0.7f, 0.4f);
            GUI.Label(new Rect(Y, Y + 58f, 800f, 30f), metin, stil);
        }
    }
}
