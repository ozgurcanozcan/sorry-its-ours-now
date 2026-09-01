using Oyun.Core;
using Oyun.Field;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;

// Gecici greybox test sahnesi ureten editor araci.
//
// Neden arac: sahne kurulumu elle yapilinca her denemede tekrar ediliyor ve
// referans baglamayi unutmak en sik hata. Arac ayni sahneyi her seferinde ayni
// sekilde kuruyor, CharacterController olculeri de ev kiti kisitiyla uyumlu
// kaliyor (kapi boslugu 0.80 x 2.00 m).
//
// Bu sahne gecicidir. Onat'in ev kiti geldiginde yerini gercek Field sahnesi alir.
public static class SahaGreyboxAraci
{
    private const string SahneYolu = "Assets/_Project/Scenes/Field.unity";
    private const string GirdiYolu = "Assets/InputSystem_Actions.inputactions";

    // Ev kiti olculeri (docs/uretim/ev-kiti.md). Kapsul kapidan gecebilmeli.
    private const float OyuncuYuksekligi = 1.8f;
    private const float OyuncuYaricapi = 0.3f;
    private const float GozHizasi = 1.65f;

    [MenuItem("Saha/Test Sahnesi Kur")]
    public static void TestSahnesiKur()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }

        var sahne = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Zemin();
        Oyuncu();

        EditorSceneManager.MarkSceneDirty(sahne);
        EditorSceneManager.SaveScene(sahne, SahneYolu);
        Debug.Log($"Test sahnesi kuruldu: {SahneYolu}");
    }

    private static void Zemin()
    {
        var zemin = GameObject.CreatePrimitive(PrimitiveType.Plane);
        zemin.name = "Zemin";

        // Plane 10x10 birim. Olcek (5,1,3) = 50 x 30 m, x=-10..40 araligi.
        //
        // Neden bu kadar genis: vitrin x=30'da, oyun alani x=0'da. 20x20 zeminde
        // ikisinin arasi bosluk kaliyor ve yuruyunce dusuluyor. Terrain de ise
        // yarardi ama 557 KB'lik varlik dosyasi uretiyor; gri kutu asamasinda
        // tek bir Plane ayni isi bedavaya goruyor.
        //
        // Ust yuzey Y=0 - ev kiti zemin kotu kurali.
        // Iki ev yan yana: 1+1 (x=-9..9) ve 2+1 (x=16..40). Disarida pay
        // birakiyoruz ki kapilara disaridan yaklasilabilsin.
        // Plane 10 birim; olcek (8, 1, 4) = 80 x 40 m.
        zemin.transform.position = new Vector3(14f, 0f, 6f);
        zemin.transform.localScale = new Vector3(8f, 1f, 4f);
    }

    private static void ReferansKupleri()
    {
        // Bos duzlukte yurudugunu anlamak imkansiz: parallaks icin sabit nesne sart.
        // Kupler 1 m; oyuncu 1.8 m oldugundan olcek hissi de veriyorlar.
        Vector3[] konumlar =
        {
            new Vector3(0f, 0.5f, 4f),
            new Vector3(6f, 0.5f, 6f),
            new Vector3(-6f, 0.5f, 6f),
            new Vector3(6f, 0.5f, -6f),
            new Vector3(-6f, 0.5f, -6f),
            new Vector3(0f, 0.5f, -8f),
        };

        var kok = new GameObject("ReferansKupleri");

        for (int i = 0; i < konumlar.Length; i++)
        {
            var kup = GameObject.CreatePrimitive(PrimitiveType.Cube);
            kup.name = $"Kup_{i}";
            kup.transform.SetParent(kok.transform, false);
            kup.transform.position = konumlar[i];
        }

        // Uzaktan gorunen bir isaret: donusun ise yaradigini anlamak icin.
        var kule = GameObject.CreatePrimitive(PrimitiveType.Cube);
        kule.name = "Kule";
        kule.transform.SetParent(kok.transform, false);
        kule.transform.position = new Vector3(0f, 2.5f, 9f);
        kule.transform.localScale = new Vector3(1f, 5f, 1f);
    }

    private static void Oyuncu()
    {
        var oyuncu = new GameObject("Oyuncu");

        // Evin disinda, kapinin karsisinda. Oyun "eve girmekle" basliyor
        // (docs/tasarim/02-saha.md), sahne de oyle baslamali.
        oyuncu.transform.position = new Vector3(-3f, 0f, -4f);

        var kapsul = oyuncu.AddComponent<CharacterController>();
        kapsul.height = OyuncuYuksekligi;
        kapsul.radius = OyuncuYaricapi;
        kapsul.center = new Vector3(0f, OyuncuYuksekligi * 0.5f, 0f);

        var pivot = new GameObject("KameraPivotu");
        pivot.transform.SetParent(oyuncu.transform, false);
        pivot.transform.localPosition = new Vector3(0f, GozHizasi, 0f);

        var kamera = Camera.main;
        if (kamera == null)
        {
            var kameraNesnesi = new GameObject("Main Camera") { tag = "MainCamera" };
            kamera = kameraNesnesi.AddComponent<Camera>();
            kameraNesnesi.AddComponent<AudioListener>();
        }

        kamera.transform.SetParent(pivot.transform, false);
        kamera.transform.localPosition = Vector3.zero;
        kamera.transform.localRotation = Quaternion.identity;

        // El capasi kameranin cocugu: bakisla birlikte donsun, esya elde
        // sabit dursun. Gozun biraz onunde ve altinda - tam gozde olursa
        // esya ekrani kapatiyor.
        var elCapasi = new GameObject("ElCapasi");
        elCapasi.transform.SetParent(kamera.transform, false);
        elCapasi.transform.localPosition = new Vector3(0.25f, -0.25f, 0.6f);

        var kontrolcu = oyuncu.AddComponent<OyuncuKontrolcu>();

        // Alanlar private SerializeField; Inspector'da elle baglamak yerine
        // SerializedObject uzerinden baglaniyor ki unutulma ihtimali kalmasin.
        var seri = new SerializedObject(kontrolcu);
        seri.FindProperty("kameraPivotu").objectReferenceValue = pivot.transform;
        seri.FindProperty("girdiVarligi").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<InputActionAsset>(GirdiYolu);
        seri.ApplyModifiedPropertiesWithoutUndo();

        var etkilesim = oyuncu.AddComponent<OyuncuEtkilesim>();
        var seri2 = new SerializedObject(etkilesim);
        seri2.FindProperty("kamera").objectReferenceValue = kamera.transform;
        seri2.FindProperty("elCapasi").objectReferenceValue = elCapasi.transform;
        seri2.FindProperty("girdiVarligi").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<UnityEngine.InputSystem.InputActionAsset>(GirdiYolu);
        seri2.ApplyModifiedPropertiesWithoutUndo();

        Selection.activeGameObject = oyuncu;
    }

    // Ice aktarilan modellerin Unity'deki gercek olcusunu raporlar.
    //
    // Neden gerekli: Onat Kenney FBX'lerinin varsayilan ayarla 10x buyuk geldigini
    // belgeledi (docs/uretim/assets.md). Kendi script'imizin ciktisi 1:1 gelmeli
    // ama bu "gelmeli" ile yetinilecek bir sey degil - olculur.
    //
    // Blender X,Y,Z -> Unity X,Z,Y (Y-up donusumu ihracta pisiyor). Yani
    // Blenderdaki 1.00 x 0.10 x 2.70 duvar, Unity'de 1.00 x 2.70 x 0.10 olmali.
    [MenuItem("Saha/Model Olculerini Dogrula")]
    public static void ModelOlculeriniDogrula()
    {
        string[] kimlikler = AssetDatabase.FindAssets("t:Model", new[] { "Assets/_Project/Art/Models", "Assets/ThirdParty" });

        if (kimlikler.Length == 0)
        {
            Debug.LogWarning("Assets/_Project/Art/Models altinda model yok.");
            return;
        }

        var rapor = new System.Text.StringBuilder();
        rapor.AppendLine($"Model olcu raporu - {kimlikler.Length} model (Unity birimi = metre)");

        foreach (string kimlik in kimlikler)
        {
            string yol = AssetDatabase.GUIDToAssetPath(kimlik);
            var onKalip = AssetDatabase.LoadAssetAtPath<GameObject>(yol);
            if (onKalip == null)
            {
                continue;
            }

            var ornek = (GameObject)PrefabUtility.InstantiatePrefab(onKalip);
            Bounds? sinir = null;

            foreach (var eleyici in ornek.GetComponentsInChildren<MeshRenderer>())
            {
                if (sinir.HasValue)
                {
                    var g = sinir.Value;
                    g.Encapsulate(eleyici.bounds);
                    sinir = g;
                }
                else
                {
                    sinir = eleyici.bounds;
                }
            }

            if (sinir.HasValue)
            {
                Vector3 b = sinir.Value.size;
                rapor.AppendLine($"  {onKalip.name,-22} {b.x,6:F2} x {b.y,6:F2} x {b.z,6:F2} m");
            }
            else
            {
                rapor.AppendLine($"  {onKalip.name,-22} MeshRenderer yok");
            }

            Object.DestroyImmediate(ornek);
        }

        Debug.Log(rapor.ToString());
    }

    // Kapidan gecme testi: iki duvar + arasinda kapi bosluklu parca.
    //
    // Neden: kapi boslugu 0.80 m, oyuncu kapsulunun capi 0.60 m. Kagitta geciyor,
    // ama CharacterController'in skin width'i, egim toleransi ve carpisma cozumu
    // isin icine girince kagit yeterli olmuyor - yurunerek olculmesi gerekiyor.
    // Gun 6 entegrasyonunun ("karakter takilmadan dolasiyor") provasi.
    //
    // Gardirop da kapinin yanina konuyor: 1.00 m genis, kapi 0.80 m. Gecmemesi
    // gerekiyor (karar 63, kasitli). Gozle gorulsun diye orada duruyor.
    [MenuItem("Saha/Kapi Testi Kur")]
    public static void KapiTestiKur()
    {
        var sahne = EditorSceneManager.OpenScene(SahneYolu, OpenSceneMode.Single);

        // Tekrar calistirilirsa kopyalanmasin.
        var eski = GameObject.Find(TestKoku);
        if (eski != null)
        {
            Object.DestroyImmediate(eski);
        }

        var kok = new GameObject(TestKoku);

        // Duvar hatti z=5'te, x=-2..3 arasi. Kapi boslugu tam ortada, x=0..1.
        // Oyuncu (0,0,0)'dan +Z'ye yuruyunce kapiya denk geliyor.
        Parca("SM_Duvar_2m", new Vector3(-2f, 0f, 5f), kok.transform);
        Parca("SM_DuvarKapi_1m", new Vector3(0f, 0f, 5f), kok.transform);
        Parca("SM_Duvar_2m", new Vector3(1f, 0f, 5f), kok.transform);

        // Baslangicin hemen onunde, kesin alinabilir iki esya. Mekanigin
        // calistigini gormek icin duvara kadar yurumek gerekmemeli - 31.08'de
        // ilk denemede en yakin esyalarin hepsi "iki kisi gerekir" ciktigi icin
        // mekanik bozuk sanildi.
        Parca("SM_Masa", new Vector3(-1.5f, 0f, 2.5f), kok.transform);
        Parca("SM_TV", new Vector3(1.5f, 0f, 2.5f), kok.transform);

        // Kapinin arkasinda, gecirmeye calisacagin esya.
        Parca("SM_Gardirop", new Vector3(1.5f, 0f, 6.5f), kok.transform);

        // Yedi esyanin tamami: olcek hissi ve carpisma denemesi icin.
        // Kapinin arkasina, birbirine girmeyecek araliklarla diziliyorlar.
        Parca("SM_Buzdolabi", new Vector3(-2.5f, 0f, 6.5f), kok.transform);
        Parca("SM_CamasirMakinesi", new Vector3(-1.5f, 0f, 6.5f), kok.transform);
        Parca("SM_Yatak", new Vector3(-4f, 0f, 7f), kok.transform);
        Parca("SM_Hali", new Vector3(0f, 0f, 9.5f), kok.transform);

        EditorSceneManager.MarkSceneDirty(sahne);
        EditorSceneManager.SaveScene(sahne, SahneYolu);
        Debug.Log($"Kapi testi kuruldu: {SahneYolu}");
    }

    private const string TestKoku = "KapiTesti";
    private const string ModelKlasoru = "Assets/_Project/Art/Models";

    private static void Parca(string ad, Vector3 konum, Transform ebeveyn)
    {
        // Modeller EvKiti/ ve Esyalar/ alt klasorlerine bolunmus durumda; sabit yol
        // yerine ada gore araniyor ki klasor duzeni degisince kirilmasin.
        string[] kimlikler = AssetDatabase.FindAssets($"\"{ad}\" t:Model", new[] { ModelKlasoru, "Assets/ThirdParty" });
        string yol = kimlikler.Length > 0 ? AssetDatabase.GUIDToAssetPath(kimlikler[0]) : null;
        var onKalip = yol != null ? AssetDatabase.LoadAssetAtPath<GameObject>(yol) : null;

        if (onKalip == null)
        {
            Debug.LogError($"Model bulunamadi: {ad} ({ModelKlasoru} altinda)");
            return;
        }

        var ornek = (GameObject)PrefabUtility.InstantiatePrefab(onKalip, ebeveyn);
        ornek.name = ad;
        ornek.transform.position = konum;

        CarpisanEkle(ornek);
        EsyaYap(ornek, ad);
    }

    private const string VitrinKoku = "Vitrin";

    // Butun modelleri yan yana dizer: ne uretildigini tek bakista gormek icin.
    // Oyun alanindan uzakta (x=30) duruyor ki test sahnesine karismasin.
    [MenuItem("Saha/Varlik Vitrini Kur")]
    public static void VarlikVitriniKur()
    {
        var sahne = EditorSceneManager.OpenScene(SahneYolu, OpenSceneMode.Single);

        var eski = GameObject.Find(VitrinKoku);
        if (eski != null)
        {
            Object.DestroyImmediate(eski);
        }

        var kok = new GameObject(VitrinKoku);
        kok.transform.position = new Vector3(30f, 0f, 0f);

        // Vitrin zemini: modeller bosluga asili durmasin.
        var zemin = GameObject.CreatePrimitive(PrimitiveType.Plane);
        zemin.name = "VitrinZemini";
        zemin.transform.SetParent(kok.transform, false);
        zemin.transform.localPosition = new Vector3(4f, 0f, 4f);
        zemin.transform.localScale = new Vector3(2f, 1f, 2f);

        string[] kimlikler = AssetDatabase.FindAssets("t:Model", new[] { ModelKlasoru });
        System.Array.Sort(kimlikler, (a, b) =>
            string.CompareOrdinal(AssetDatabase.GUIDToAssetPath(a), AssetDatabase.GUIDToAssetPath(b)));

        // 4 sutunlu izgara, 3 m aralik - en genis parca 2 m, carpismasinlar.
        const int Sutun = 4;
        const float Aralik = 3f;

        for (int i = 0; i < kimlikler.Length; i++)
        {
            string yol = AssetDatabase.GUIDToAssetPath(kimlikler[i]);
            var onKalip = AssetDatabase.LoadAssetAtPath<GameObject>(yol);
            if (onKalip == null)
            {
                continue;
            }

            var ornek = (GameObject)PrefabUtility.InstantiatePrefab(onKalip, kok.transform);
            ornek.name = onKalip.name;
            ornek.transform.localPosition = new Vector3((i % Sutun) * Aralik, 0f, (i / Sutun) * Aralik);

            // Vitrinde de carpisan olmali: modellerin sadece boyutuna degil,
            // etrafinda donerken nasil hissettirdigine de bakiliyor.
            CarpisanEkle(ornek);

            // Esya olan modeller vitrinde de alinabilir olmali. Aksi halde
            // ayni model bir yerde etkilesiyor, otekinde olu duruyor ve
            // "bu neden calismiyor" sorusu doguyor (31.08'de yasandi).
            EsyaYap(ornek, ornek.name);
        }

        Debug.Log($"Vitrin kuruldu: {kimlikler.Length} model, kok x=30");
        EditorSceneManager.MarkSceneDirty(sahne);
        EditorSceneManager.SaveScene(sahne, SahneYolu);
    }

    // Vitrini birkac acidan PNG olarak kaydeder.
    //
    // Neden: Claude sahneye bakamiyor (araclar.md "nasil gorundugune bakamaz").
    // Fotograf, insanin Unity'yi acmadan da gorebilmesi ve benim ciktinin
    // gercekten olustugunu dogrulayabilmem icin.
    [MenuItem("Saha/Vitrin Fotografi Cek")]
    public static void VitrinFotografiCek()
    {
        EditorSceneManager.OpenScene(SahneYolu, OpenSceneMode.Single);

        // Hangi vitrin varsa onu cek; VNBP oncelikli cunku dokulu olan o.
        var kok = GameObject.Find(EvlerKoku);
        if (kok == null)
        {
            Debug.LogError("Evler yok - once 'Saha/Tam Kurulum' calistirilmali.");
            return;
        }

        string klasor = "D:/Temp/vitrin";
        System.IO.Directory.CreateDirectory(klasor);

        var kameraNesnesi = new GameObject("VitrinKamerasi");
        var kamera = kameraNesnesi.AddComponent<Camera>();
        kamera.clearFlags = CameraClearFlags.SolidColor;
        kamera.backgroundColor = new Color(0.16f, 0.18f, 0.21f);

        Vector3 merkez = new Vector3(8f, 1f, 3f);

        (string ad, Vector3 konum, Vector3 bakis)[] acilar =
        {
            ("01-ustten",  merkez + new Vector3(0f, 26f, -0.5f), merkez),
            ("02-onden",   merkez + new Vector3(0f, 10f, -20f), merkez),
            ("03-yandan",  merkez + new Vector3(-24f, 12f, 0f),  merkez),
        };

        foreach (var (ad, konum, bakis) in acilar)
        {
            kameraNesnesi.transform.position = konum;
            kameraNesnesi.transform.LookAt(bakis);

            var hedef = new RenderTexture(1600, 900, 24);
            kamera.targetTexture = hedef;
            kamera.Render();

            RenderTexture.active = hedef;
            var resim = new Texture2D(1600, 900, TextureFormat.RGB24, false);
            resim.ReadPixels(new Rect(0, 0, 1600, 900), 0, 0);
            resim.Apply();

            RenderTexture.active = null;
            kamera.targetTexture = null;

            string dosya = $"{klasor}/{ad}.png";
            System.IO.File.WriteAllBytes(dosya, resim.EncodeToPNG());
            Debug.Log($"Fotograf: {dosya}");

            Object.DestroyImmediate(resim);
            hedef.Release();
            Object.DestroyImmediate(hedef);
        }

        Object.DestroyImmediate(kameraNesnesi);
    }

    // Sahneyi sifirdan, tam hali ile kurar: zemin + oyuncu + kapi testi + vitrin.
    //
    // Neden tek komut: uc adim ayri ayri calistirildiginda araya insan hatasi
    // giriyor (sira karisirsa vitrin siliniyor). Sahne her seferinde ayni ciksin.
    [MenuItem("Saha/Tam Kurulum")]
    public static void TamKurulum()
    {
        TestSahnesiKur();
        EvlerKur();
        Debug.Log("Tam kurulum bitti: zemin + oyuncu + iki ev (1+1 ve 2+1).");
    }

    // FBX ice aktarimi carpisan eklemiyor - Unity'nin varsayilani bu.
    // Carpisan olmadan duvar duvar degil, esya esya degil: oyuncu icinden gecer.
    // Hem kapi testinde hem vitrinde ayni sey gerekiyor, o yuzden tek yerde.
    private static void CarpisanEkle(GameObject nesne)
    {
        foreach (var suzgec in nesne.GetComponentsInChildren<MeshFilter>())
        {
            if (suzgec.sharedMesh == null)
            {
                continue;
            }

            var carpisan = suzgec.gameObject.AddComponent<MeshCollider>();
            carpisan.sharedMesh = suzgec.sharedMesh;
        }
    }

    private const string VnbpKoku = "VitrinVNBP";
    private const string VnbpKlasoru = "Assets/ThirdParty/VNBP-HomeSet/Models";

    // VNBP setini AYRI bir vitrinde gosterir.
    //
    // Neden ayri: karar 64 "iki izgara ayni odada karistirilmaz" diyor. VNBP
    // duvari 2.00 x 0.20 x 3.00 m, bizim kit 1 m modul / 0.10 kalinlik / 2.70 kat.
    // Yan yana koymak hangi kitin secilecegi kararini gormeyi kolaylastirir ama
    // ayni odaya sokmak yanlis olur - o yuzden ayri alan.
    [MenuItem("Saha/VNBP Vitrini Kur")]
    public static void VnbpVitriniKur()
    {
        var sahne = EditorSceneManager.OpenScene(SahneYolu, OpenSceneMode.Single);

        var eski = GameObject.Find(VnbpKoku);
        if (eski != null)
        {
            Object.DestroyImmediate(eski);
        }

        var kok = new GameObject(VnbpKoku);
        kok.transform.position = new Vector3(26f, 0f, -18f);

        string[] kimlikler = AssetDatabase.FindAssets("t:Model", new[] { VnbpKlasoru });
        System.Array.Sort(kimlikler, (a, b) =>
            string.CompareOrdinal(AssetDatabase.GUIDToAssetPath(a), AssetDatabase.GUIDToAssetPath(b)));

        const int Sutun = 4;
        const float Aralik = 3.5f;

        for (int i = 0; i < kimlikler.Length; i++)
        {
            var onKalip = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(kimlikler[i]));
            if (onKalip == null)
            {
                continue;
            }

            var ornek = (GameObject)PrefabUtility.InstantiatePrefab(onKalip, kok.transform);
            ornek.name = onKalip.name;
            ornek.transform.localPosition = new Vector3((i % Sutun) * Aralik, 0f, (i / Sutun) * Aralik);
            CarpisanEkle(ornek);
            EsyaYap(ornek, ornek.name);
        }

        Debug.Log($"VNBP vitrini kuruldu: {kimlikler.Length} model, kok (26, 0, -18)");
        EditorSceneManager.MarkSceneDirty(sahne);
        EditorSceneManager.SaveScene(sahne, SahneYolu);
    }

    // Kapi bosluklarinin gercek genisligini ve yuksekligini olcer.
    //
    // Neden gerekli: sinir kutusu duvar panelinin disini veriyor, delik degil.
    // Hangi kitin secilecegi karari (karar 64) kapi genisligine bagli - bizim
    // kitte 0.80 m "dar kapi" engelinin kaynagi. VNBP'ninki bilinmeden karar
    // verilemez. Olculur, tahmin edilmez.
    //
    // Yontem: duvarin onune bir sira isin atilir; carpmayan isinlar deliktir.
    [MenuItem("Saha/Kapi Boslugu Olc")]
    public static void KapiBoslugoOlc()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        string[] adaylar = { "SM_DuvarKapi_1m", "Wall Standard Door" };
        var rapor = new System.Text.StringBuilder();
        rapor.AppendLine("Kapi boslugu olcumu (isin sondasi)");

        foreach (string ad in adaylar)
        {
            string[] kimlikler = AssetDatabase.FindAssets($"\"{ad}\" t:Model",
                new[] { "Assets/_Project/Art/Models", "Assets/ThirdParty" });

            if (kimlikler.Length == 0)
            {
                rapor.AppendLine($"  {ad}: model bulunamadi");
                continue;
            }

            var onKalip = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(kimlikler[0]));
            var ornek = (GameObject)PrefabUtility.InstantiatePrefab(onKalip);
            ornek.transform.position = Vector3.zero;
            CarpisanEkle(ornek);
            Physics.SyncTransforms();

            // Tarama duvarin KENDI sinirlariyla kisitli olmali. Yoksa isinlar
            // duvarin yanindan gecer ve "bosluk" diye panelin disini olceriz.
            Bounds? kutu = null;
            foreach (var eleyici in ornek.GetComponentsInChildren<MeshRenderer>())
            {
                if (kutu.HasValue)
                {
                    var g = kutu.Value;
                    g.Encapsulate(eleyici.bounds);
                    kutu = g;
                }
                else
                {
                    kutu = eleyici.bounds;
                }
            }

            if (!kutu.HasValue)
            {
                rapor.AppendLine($"  {ad}: MeshRenderer yok");
                Object.DestroyImmediate(ornek);
                continue;
            }

            Bounds b = kutu.Value;
            // Kenarlardan 1 mm iceri girilir; tam sinirda isin duvari siyirabiliyor.
            float xBas = b.min.x + 0.001f, xSon = b.max.x - 0.001f;
            float yBas = b.min.y + 0.001f, ySon = b.max.y - 0.001f;
            float onZ = b.min.z - 1f;

            // Yatay tarama: kapi kanadi yuksekligi civarinda (1.0 m) delik aranir.
            float genislik = Tara(xBas, xSon, 0.005f, x => new Vector3(x, 1.0f, onZ), Vector3.forward);

            // Dikey tarama: deligin tam ortasindan.
            float ortaX = OrtaNokta(xBas, xSon, 0.005f, x => new Vector3(x, 1.0f, onZ), Vector3.forward);
            float yukseklik = Tara(yBas, ySon, 0.005f, y => new Vector3(ortaX, y, onZ), Vector3.forward);

            rapor.AppendLine($"  {ad,-22} bosluk: {genislik:F2} m genis x {yukseklik:F2} m yuksek");
            Object.DestroyImmediate(ornek);
        }

        Debug.Log(rapor.ToString());
    }

    // Verilen eksende, isinin carpmadigi kesintisiz en uzun araligin uzunlugu.
    private static float Tara(float bas, float son, float adim,
                              System.Func<float, Vector3> baslangic, Vector3 yon)
    {
        float enUzun = 0f, suanki = 0f;

        for (float t = bas; t <= son; t += adim)
        {
            if (Physics.Raycast(baslangic(t), yon, 4f))
            {
                suanki = 0f;
            }
            else
            {
                suanki += adim;
                if (suanki > enUzun)
                {
                    enUzun = suanki;
                }
            }
        }

        return enUzun;
    }

    // Kesintisiz bosluk araliginin orta noktasi.
    private static float OrtaNokta(float bas, float son, float adim,
                                   System.Func<float, Vector3> baslangic, Vector3 yon)
    {
        float enUzun = 0f, suanki = 0f, enIyiSon = bas;

        for (float t = bas; t <= son; t += adim)
        {
            if (Physics.Raycast(baslangic(t), yon, 4f))
            {
                suanki = 0f;
            }
            else
            {
                suanki += adim;
                if (suanki > enUzun)
                {
                    enUzun = suanki;
                    enIyiSon = t;
                }
            }
        }

        return enIyiSon - enUzun * 0.5f;
    }

    private const string VnbpTestKoku = "VnbpKapiTesti";

    // VNBP setiyle ikinci bir kapi testi. Gri kutu testinin batisinda, x=-22.
    //
    // Neden ayri alan: karar 64 iki izgarayi ayni odada karistirmayi yasakliyor.
    // Karistirmak tasarimi da bozuyor - VNBP kapisi 1.06 m, bizim gardirop
    // 1.00 m; karisik kurulumda gardirop kapidan geciyor ve "gardirop kapidan
    // gecmez" sakasi oluyor. VNBP'nin kendi gardirobu 1.19 m, kendi kapisindan
    // gecmiyor. Setler kendi iclerinde tutarli, karisim tutarsiz.
    //
    // Amac karsilastirma: ikisini de yuruyup hangi kitin secilecegine karar
    // vermek (karar 64 bu karari A'ya birakiyor).
    [MenuItem("Saha/VNBP Kapi Testi Kur")]
    public static void VnbpKapiTestiKur()
    {
        var sahne = EditorSceneManager.OpenScene(SahneYolu, OpenSceneMode.Single);

        var eski = GameObject.Find(VnbpTestKoku);
        if (eski != null)
        {
            Object.DestroyImmediate(eski);
        }

        var kok = new GameObject(VnbpTestKoku);

        // VNBP duvari 2.00 m genis, pivotu tabanda-merkezde (docs/uretim/assets.md).
        // 2 m arayla dizilince kendiliginden bitisiyor.
        Parca("Wall Standard", new Vector3(-26f, 0f, 5f), kok.transform);
        Parca("Wall Standard Door", new Vector3(-24f, 0f, 5f), kok.transform);
        Parca("Wall Standard", new Vector3(-22f, 0f, 5f), kok.transform);
        Parca("Wall Standard Window 1", new Vector3(-20f, 0f, 5f), kok.transform);

        // Kapinin arkasi: gardirop (1.19 m - kapidan gecmemeli) ve digerleri.
        Parca("Wardrobe", new Vector3(-22f, 0f, 7f), kok.transform);
        Parca("Fridge", new Vector3(-25f, 0f, 7f), kok.transform);
        Parca("Washing Machine", new Vector3(-26.5f, 0f, 7f), kok.transform);
        Parca("Dining Table", new Vector3(-24f, 0f, 9.5f), kok.transform);
        Parca("Bed Base 1 1", new Vector3(-27.5f, 0f, 9.5f), kok.transform);
        Parca("TV 1", new Vector3(-20.5f, 0f, 8f), kok.transform);
        Parca("Rug 1", new Vector3(-24f, 0f, 12f), kok.transform);

        Debug.Log("VNBP kapi testi kuruldu: x=-26..-20, kapi x=-24");
        EditorSceneManager.MarkSceneDirty(sahne);
        EditorSceneManager.SaveScene(sahne, SahneYolu);
    }

    // Model adi -> EsyaTanimi varlik adi. Iki kit de ayni yedi esyayi
    // temsil ediyor, isimleri farkli. Eslesmeyen model (duvar, zemin) esya
    // degil; listede olmamasi onu "alinamaz" yapmiyor, sadece esya saymiyor.
    private static readonly System.Collections.Generic.Dictionary<string, string> EsyaEslesmesi =
        new System.Collections.Generic.Dictionary<string, string>
        {
            { "SM_Buzdolabi", "buzdolabi" },
            { "SM_CamasirMakinesi", "camasir-makinesi" },
            { "SM_Gardirop", "gardirop" },
            { "SM_Hali", "hali" },
            { "SM_Masa", "masa" },
            { "SM_TV", "tv" },
            { "SM_Yatak", "yatak" },
            { "Fridge", "buzdolabi" },
            { "Washing Machine", "camasir-makinesi" },
            { "Wardrobe", "gardirop" },
            { "Rug 1", "hali" },
            { "Dining Table", "masa" },
            { "TV 1", "tv" },
            { "Bed Base 1 1", "yatak" },
        };

    // Modelse ve esya listesinde varsa: Rigidbody + Esya ekleyip tanimi baglar.
    private static void EsyaYap(GameObject nesne, string modelAdi)
    {
        string varlikAdi;
        if (!EsyaEslesmesi.TryGetValue(modelAdi, out varlikAdi))
        {
            return;
        }

        string[] kimlikler = AssetDatabase.FindAssets($"{varlikAdi} t:EsyaTanimi",
            new[] { "Assets/_Project/Data/Items" });

        if (kimlikler.Length == 0)
        {
            Debug.LogWarning($"EsyaTanimi bulunamadi: {varlikAdi}");
            return;
        }

        var tanim = AssetDatabase.LoadAssetAtPath<EsyaTanimi>(
            AssetDatabase.GUIDToAssetPath(kimlikler[0]));

        // Esya RequireComponent(Rigidbody) istiyor; AddComponent onu kendisi
        // ekliyor ama kutlesini veriden almiyor - agirlik EsyaVerisi'nde.
        var esya = nesne.AddComponent<Esya>();
        var seri = new SerializedObject(esya);
        seri.FindProperty("tanim").objectReferenceValue = tanim;
        seri.ApplyModifiedPropertiesWithoutUndo();

        // MeshCollider varsayilan olarak convex degil; convex olmayan collider
        // hareketli Rigidbody ile calismiyor. Esyalar hareket edecegi icin sart.
        foreach (var carpisan in nesne.GetComponentsInChildren<MeshCollider>())
        {
            carpisan.convex = true;
        }
    }

    private static void Duvar(string ad, Vector3 konum, float donus, Transform ebeveyn)
    {
        string[] kimlikler = AssetDatabase.FindAssets($"\"{ad}\" t:Model", new[] { VnbpKiti });
        if (kimlikler.Length == 0)
        {
            Debug.LogError($"Duvar bulunamadi: {ad}");
            return;
        }

        var onKalip = AssetDatabase.LoadAssetAtPath<GameObject>(
            AssetDatabase.GUIDToAssetPath(kimlikler[0]));

        var ornek = (GameObject)PrefabUtility.InstantiatePrefab(onKalip, ebeveyn);
        ornek.name = ad;
        ornek.transform.position = konum;
        ornek.transform.rotation = Quaternion.Euler(0f, donus, 0f);

        CarpisanEkle(ornek);
    }

    // Hazir prefab yerlestirir (ham modelden farki: bilesenleri zaten uzerinde).
    private static void Onkalip(string ad, Vector3 konum, float donus, Transform ebeveyn)
    {
        string[] kimlikler = AssetDatabase.FindAssets($"\"{ad}\" t:Prefab",
            new[] { "Assets/_Project/Prefabs" });

        if (kimlikler.Length == 0)
        {
            Debug.LogError($"Prefab bulunamadi: {ad}");
            return;
        }

        var kaynak = AssetDatabase.LoadAssetAtPath<GameObject>(
            AssetDatabase.GUIDToAssetPath(kimlikler[0]));

        var ornek = (GameObject)PrefabUtility.InstantiatePrefab(kaynak, ebeveyn);
        ornek.transform.position = konum;
        ornek.transform.rotation = Quaternion.Euler(0f, donus, 0f);
    }

    private const string EvlerKoku = "Evler";
    private const string VnbpKiti = "Assets/ThirdParty/VNBP-HomeSet/Models/EvKiti";

    // VNBP duvari: 2.00 genis, 0.20 kalin, 3.00 yuksek. Pivot taban-merkezde
    // (olculdu 31.08). Kapi boslugu 1.06 x 2.07.
    private const float Modul = 2f;

    // Iki ev: 1+1 (18x14 m) ve 2+1 (24x18 m).
    //
    // Olculer gercek daire buyuklugu DEGIL, bilerek buyuk: oyuncu icerde
    // dolasirken mesafe hissetmeli ve esya tasimak sure yemeli
    // (docs/tasarim/02-saha.md, baglayici kisit sure ve kapasite).
    // Onceki 10x8 "masa kadar" hissettirdi (31.08, oynayarak olculdu).
    //
    // Ikisi de 2 m modulun kati; VNBP kiti oyle (karar 64, iki izgara ayni
    // evde karistirilmaz -- bu sahnede gri kutu hic yok).
    //
    // Bu evler GECICI. Gercek gecekondu ve daire Onat'in isi.
    [MenuItem("Saha/Evleri Kur")]
    public static void EvlerKur()
    {
        var sahne = EditorSceneManager.OpenScene(SahneYolu, OpenSceneMode.Single);

        var eski = GameObject.Find(EvlerKoku);
        if (eski != null)
        {
            Object.DestroyImmediate(eski);
        }

        var kok = new GameObject(EvlerKoku).transform;

        BirArtiBir(kok);
        IkiArtiBir(kok);

        EditorSceneManager.MarkSceneDirty(sahne);
        EditorSceneManager.SaveScene(sahne, SahneYolu);

        Debug.Log("Iki ev kuruldu: 1+1 (18x14 m) ve 2+1 (24x18 m).");
        DuvarCakismasiDenetle();
    }

    // Duvar dizisi: baslangictan itibaren adet kadar parca, 2 m arayla.
    private static void Dizi(Transform ev, Vector3 bas, Vector3 yon, int adet, float donus,
                             int kapi = -1, int pencere1 = -1, int pencere2 = -1, int pencere3 = -1)
    {
        for (int i = 0; i < adet; i++)
        {
            string ad = "Wall Standard";
            if (i == kapi)
            {
                ad = "Wall Standard Door";
            }
            else if (i == pencere1 || i == pencere2 || i == pencere3)
            {
                ad = "Wall Standard Window 1";
            }

            Duvar(ad, bas + yon * (Modul * i), donus, ev);
        }
    }

    // 1+1: 18 x 14 m. Bati salon (12x14), dogu yatak odasi (6x14).
    private static void BirArtiBir(Transform kok)
    {
        var ev = new GameObject("Ev_1plus1").transform;
        ev.SetParent(kok, false);

        // Dis cepheler: x=-9..9, z=0..14.
        Dizi(ev, new Vector3(-8f, 0f, 0f), Vector3.right, 9, 0f, kapi: 2, pencere1: 6);
        Dizi(ev, new Vector3(-8f, 0f, 14f), Vector3.right, 9, 180f, pencere1: 1, pencere2: 4, pencere3: 7);
        Dizi(ev, new Vector3(-9f, 0f, 1f), Vector3.forward, 7, 90f, pencere1: 2, pencere2: 5);
        Dizi(ev, new Vector3(9f, 0f, 1f), Vector3.forward, 7, -90f, pencere1: 3);

        // Ic bolme x=3, kapisi z=5'te.
        Dizi(ev, new Vector3(3f, 0f, 1f), Vector3.forward, 7, 90f, kapi: 2);

        // Salon (x=-9..3). Esyalar duvarlardan uzak, ortada genis dolasim.
        Onkalip("Esya_TV", new Vector3(-7.5f, 0f, 7f), 90f, ev);
        Parca("Dining Table", new Vector3(-4f, 0f, 4f), ev);
        Parca("Rug 1", new Vector3(-3f, 0f, 9f), ev);
        Parca("Washing Machine", new Vector3(-6.5f, 0f, 10.5f), ev);
        Parca("Fridge", new Vector3(0f, 0f, 11f), ev);

        // Yatak odasi (x=3..9).
        Parca("Bed Base 1 1", new Vector3(6f, 0f, 7.5f), ev);
        Parca("Wardrobe", new Vector3(6f, 0f, 11.5f), ev);
        Parca("Dining Table", new Vector3(6f, 0f, 3f), ev);
    }

    // 2+1: 24 x 18 m. Guneyde salon (24x10), kuzeyde iki oda.
    private static void IkiArtiBir(Transform kok)
    {
        var ev = new GameObject("Ev_2plus1").transform;
        ev.SetParent(kok, false);

        const float X0 = 16f;
        const float X1 = 40f;

        // Dis cepheler: x=16..40, z=0..18.
        Dizi(ev, new Vector3(X0 + 1f, 0f, 0f), Vector3.right, 12, 0f, kapi: 3, pencere1: 7, pencere2: 10);
        Dizi(ev, new Vector3(X0 + 1f, 0f, 18f), Vector3.right, 12, 180f, pencere1: 2, pencere2: 6, pencere3: 9);
        Dizi(ev, new Vector3(X0, 0f, 1f), Vector3.forward, 9, 90f, pencere1: 3, pencere2: 6);
        Dizi(ev, new Vector3(X1, 0f, 1f), Vector3.forward, 9, -90f, pencere1: 3, pencere2: 6);

        // Salonu odalardan ayiran bolme z=10, kapisi x=X0+5.
        Dizi(ev, new Vector3(X0 + 1f, 0f, 10f), Vector3.right, 12, 0f, kapi: 2);

        // Iki odayi ayiran bolme x=X0+12, z=10..18, kapisi z=13.
        Dizi(ev, new Vector3(X0 + 12f, 0f, 11f), Vector3.forward, 4, 90f, kapi: 1);

        // Salon (z=0..10).
        Onkalip("Esya_TV", new Vector3(X0 + 12f, 0f, 8.5f), 180f, ev);
        Parca("Dining Table", new Vector3(X0 + 5f, 0f, 4f), ev);
        Parca("Rug 1", new Vector3(X0 + 17f, 0f, 5f), ev);
        Parca("Washing Machine", new Vector3(X0 + 20f, 0f, 3.5f), ev);
        Parca("Fridge", new Vector3(X0 + 3f, 0f, 7.5f), ev);

        // Kuzeybati odasi (x=16..28, z=10..18).
        Parca("Bed Base 1 1", new Vector3(X0 + 3f, 0f, 14f), ev);
        Parca("Wardrobe", new Vector3(X0 + 8f, 0f, 15f), ev);

        // Kuzeydogu odasi (x=28..40, z=10..18).
        Parca("Bed Base 1 1", new Vector3(X0 + 15f, 0f, 14f), ev);
        Parca("Dining Table", new Vector3(X0 + 20f, 0f, 15f), ev);
    }

    // Her esyanin duvarla cakisip cakismadigini OLCER.
    //
    // Neden gerekli: iki kez "esya duvarin icinde" sikayeti geldi ve iki
    // seferinde de koordinatlara bakip "olmamasi lazim" dedim. Goz ve hesap
    // yetmiyor; kurulumdan sonra fizik motoruna sorulmali.
    [MenuItem("Saha/Duvar Cakismasini Denetle")]
    public static void DuvarCakismasiDenetle()
    {
        // Sahne acik degilse hicbir esya bulunmaz ve denetim yanlislikla
        // "temiz" der. Batch modda tam bu oldu (31.08). Once sahne acilir.
        if (!UnityEngine.SceneManagement.SceneManager.GetActiveScene().path.EndsWith("Field.unity"))
        {
            EditorSceneManager.OpenScene(SahneYolu, OpenSceneMode.Single);
        }

        Physics.SyncTransforms();

        var esyalar = Object.FindObjectsByType<Esya>(FindObjectsSortMode.None);
        if (esyalar.Length == 0)
        {
            Debug.LogWarning("DENETIM: sahnede hic esya yok, denetlenecek bir sey bulunamadi.");
            return;
        }

        var rapor = new System.Text.StringBuilder();
        int cakisan = 0;

        foreach (var esya in esyalar)
        {
            Bounds? kutu = null;
            foreach (var eleyici in esya.GetComponentsInChildren<MeshRenderer>())
            {
                if (kutu.HasValue)
                {
                    var g = kutu.Value;
                    g.Encapsulate(eleyici.bounds);
                    kutu = g;
                }
                else
                {
                    kutu = eleyici.bounds;
                }
            }

            if (!kutu.HasValue)
            {
                continue;
            }

            // Kucuk pay geri cekiliyor: yuzeyi degen esya cakisan sayilmasin.
            Vector3 yariBoyut = Vector3.Max(kutu.Value.extents - Vector3.one * 0.03f,
                                            Vector3.one * 0.01f);

            foreach (var carpisan in Physics.OverlapBox(kutu.Value.center, yariBoyut,
                         Quaternion.identity, ~0, QueryTriggerInteraction.Ignore))
            {
                if (carpisan.GetComponentInParent<Esya>() != null)
                {
                    continue;
                }

                if (!carpisan.name.StartsWith("Wall"))
                {
                    continue;
                }

                cakisan++;
                Bounds db = carpisan.bounds;
                Bounds eb = kutu.Value;

                rapor.AppendLine($"  CAKISMA {esya.name}");
                rapor.AppendLine($"     esya  x[{eb.min.x:F2}..{eb.max.x:F2}] "
                    + $"z[{eb.min.z:F2}..{eb.max.z:F2}] y[{eb.min.y:F2}..{eb.max.y:F2}]");
                rapor.AppendLine($"     duvar x[{db.min.x:F2}..{db.max.x:F2}] "
                    + $"z[{db.min.z:F2}..{db.max.z:F2}] ({carpisan.name})");
                break;
            }
        }

        if (cakisan == 0)
        {
            Debug.Log($"DENETIM: {esyalar.Length} esya denetlendi, duvarla cakisan yok.");
        }
        else
        {
            Debug.LogWarning($"DENETIM: {cakisan} esya duvarla cakisiyor.\n{rapor}");
        }
    }
}
