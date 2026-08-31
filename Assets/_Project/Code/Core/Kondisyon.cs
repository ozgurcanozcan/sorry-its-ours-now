namespace Oyun.Core
{
    /// <summary>Esyanin uc halinden biri (docs/tasarim/05-icerik.md).</summary>
    /// <remarks>
    /// Sayilar kotuden iyiye siralanmis olmak zorunda: tamir bir ya da iki
    /// kademe yukari tasiyor ve secim oyuncunun (docs/tasarim/07-depo.md),
    /// yani karsilastirma ve toplama islemi anlamli olmali.
    /// </remarks>
    public enum Kondisyon
    {
        Hasarli = 0,
        Eski = 1,
        IyiDurumda = 2
    }
}
