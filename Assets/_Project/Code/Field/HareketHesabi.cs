using UnityEngine;

namespace Oyun.Field
{

    // Girdi -> hareket donusumleri.
    //
    // Neden MonoBehaviour disinda: bu hesaplar sahne, kamera ya da CharacterController
    // gerektirmiyor. Ayri tutulunca EditMode testinden dogrudan cagrilabiliyorlar.
    // Kontrolcunun test edilemeyen kismi yalnizca girdi okuma ve hareket uygulama kaliyor.
    public static class HareketHesabi
    {
        // Yatay girdiyi oyuncunun baktigi yone gore dunya uzayina cevirir.
        public static Vector3 DunyaYonu(Vector2 girdi, float sapmaDerece)
        {
            Vector3 yon = new Vector3(girdi.x, 0f, girdi.y);

            // Capraz girdi (1,1) uzunlugu 1.41 eder. Sinirlanmazsa capraz yurumek
            // duz yurumekten hizli olur ve sure kisiti bundan sizar.
            if (yon.sqrMagnitude > 1f)
            {
                yon.Normalize();
            }

            return Quaternion.Euler(0f, sapmaDerece, 0f) * yon;
        }

        // Kamera egimini sinir icinde tutar.
        //
        // Fare yukari itildiginde bakis yukari gitmeli, bu yuzden delta cikariliyor.
        // Sinir asilirsa kamera tepeden asagi devrilir ve oyuncu yonunu kaybeder.
        public static float SinirliEgim(float mevcutEgim, float delta, float sinir)
        {
            return Mathf.Clamp(mevcutEgim - delta, -sinir, sinir);
        }

    // Tasinan agirligin yurume hizina etkisi.
    //
    // Neden bir kat: agirlik EsyaVerisi'nde kilogram olarak duruyor ve
    // docs/tasarim/02-saha.md "buyuk esya gercekten yorucudur, agirlik bir
    // sayi degil hissedilen bir sey" diyor. Hissedilmesinin tek yolu tasirken
    // yavaslamak.
    //
    // Sayilar olculmus degil. Saglamasi gereken kisitlar:
    //   - Bos elle hiz degismez (kat = 1).
    //   - En agir tek-kisilik esya tasinabilir kalmali, yani kat hicbir zaman
    //     EnDusukKat'in altina inmez; yoksa oyuncu yerinde sayar.
    //   - Fark hissedilir olmali: hali (8 kg) ile masa (25 kg) arasindaki
    //     fark oynarken ayirt edilebilmeli.
    public static float TasimaHizKati(float agirlik, float agirlikSiniri, float enDusukKat)
    {
        if (agirlik <= 0f || agirlikSiniri <= 0f)
        {
            return 1f;
        }

        float oran = Mathf.Clamp01(agirlik / agirlikSiniri);
        return Mathf.Lerp(1f, enDusukKat, oran);
    }

    // Kosulabilir mi? Karar 79: elde ya da slotta esya varken kosma kapali.
    //
    // Neden kural burada: kosmanin bedeli bu. Karar 79 uc aday arasindan
    // bunu secti -- takat bir kaynak ve bir cubuk gerektiriyordu, gurultu
    // ise ev sahibini bekliyordu (Hafta 3). Tasirken kosamamak olculen
    // sorunu dogrudan cozuyor: kosma, agirlik cezasinin yarisini telafi
    // ediyordu (gardiropta 0.50 -> 0.80).
    public static bool KosabilirMi(bool tasiyor)
    {
        return !tasiyor;
    }
}
}
