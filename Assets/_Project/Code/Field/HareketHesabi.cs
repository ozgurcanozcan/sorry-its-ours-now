using UnityEngine;

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
}
