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
        ReferansKupleri();
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
        zemin.transform.position = new Vector3(15f, 0f, 5f);
        zemin.transform.localScale = new Vector3(5f, 1f, 3f);
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
        oyuncu.transform.position = Vector3.zero;

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

        var kontrolcu = oyuncu.AddComponent<OyuncuKontrolcu>();

        // Alanlar private SerializeField; Inspector'da elle baglamak yerine
        // SerializedObject uzerinden baglaniyor ki unutulma ihtimali kalmasin.
        var seri = new SerializedObject(kontrolcu);
        seri.FindProperty("kameraPivotu").objectReferenceValue = pivot.transform;
        seri.FindProperty("girdiVarligi").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<InputActionAsset>(GirdiYolu);
        seri.ApplyModifiedPropertiesWithoutUndo();

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
        string[] kimlikler = AssetDatabase.FindAssets("t:Model", new[] { "Assets/_Project/Art/Models" });

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

        // Kapinin arkasinda, gecirmeye calisacagin esya.
        Parca("SM_Gardirop", new Vector3(1.5f, 0f, 6.5f), kok.transform);

        // Yedi esyanin tamami: olcek hissi ve carpisma denemesi icin.
        // Kapinin arkasina, birbirine girmeyecek araliklarla diziliyorlar.
        Parca("SM_Buzdolabi", new Vector3(-2.5f, 0f, 6.5f), kok.transform);
        Parca("SM_CamasirMakinesi", new Vector3(-1.5f, 0f, 6.5f), kok.transform);
        Parca("SM_Masa", new Vector3(-1f, 0f, 8f), kok.transform);
        Parca("SM_Yatak", new Vector3(-4f, 0f, 7f), kok.transform);
        Parca("SM_TV", new Vector3(2.5f, 0f, 8f), kok.transform);
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
        string[] kimlikler = AssetDatabase.FindAssets($"{ad} t:Model", new[] { ModelKlasoru });
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

        var kok = GameObject.Find(VitrinKoku);
        if (kok == null)
        {
            Debug.LogError("Vitrin yok - once 'Saha/Varlik Vitrini Kur' calistirilmali.");
            return;
        }

        string klasor = "D:/Temp/vitrin";
        System.IO.Directory.CreateDirectory(klasor);

        var kameraNesnesi = new GameObject("VitrinKamerasi");
        var kamera = kameraNesnesi.AddComponent<Camera>();
        kamera.clearFlags = CameraClearFlags.SolidColor;
        kamera.backgroundColor = new Color(0.16f, 0.18f, 0.21f);

        Vector3 merkez = kok.transform.position + new Vector3(4.5f, 1f, 4.5f);

        (string ad, Vector3 konum, Vector3 bakis)[] acilar =
        {
            ("01-ustten",  merkez + new Vector3(0f, 16f, -0.5f), merkez),
            ("02-onden",   merkez + new Vector3(0f, 3.5f, -13f), merkez + new Vector3(0f, -0.5f, 0f)),
            ("03-yandan",  merkez + new Vector3(-13f, 4f, 0f),   merkez),
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
        KapiTestiKur();
        VarlikVitriniKur();
        Debug.Log("Tam kurulum bitti: zemin + oyuncu + kapi testi + vitrin.");
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
}
