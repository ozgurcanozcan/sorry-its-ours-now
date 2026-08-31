using Oyun.Field;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Oyun.Field.Editor
{
    // "E calismiyor" sikayetini olcerek arastiran teshis araci.
    //
    // Play kipine girmeden calisir: Physics sorgulari edit kipinde de
    // calisiyor (SyncTransforms sonrasi). Amac tahmini kesmek.
    public static class EtkilesimTeshis
    {
        [MenuItem("Saha/Etkilesim Teshisi")]
        public static void Calistir()
        {
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/Field.unity", OpenSceneMode.Single);
            Physics.SyncTransforms();

            var etkilesim = Object.FindAnyObjectByType<OyuncuEtkilesim>();
            if (etkilesim == null)
            {
                Debug.LogError("TESHIS: sahnede OyuncuEtkilesim yok.");
                return;
            }

            var oyuncu = etkilesim.transform;
            var kamera = Camera.main;
            if (kamera == null)
            {
                Debug.LogError("TESHIS: Main Camera yok.");
                return;
            }

            var esyalar = Object.FindObjectsByType<Esya>(FindObjectsSortMode.None);
            var rapor = new System.Text.StringBuilder();
            rapor.AppendLine($"TESHIS: {esyalar.Length} esya, oyuncu {oyuncu.position}, kamera {kamera.transform.position}");
            rapor.AppendLine();

            foreach (var esya in esyalar)
            {
                float mesafe = Vector3.Distance(kamera.transform.position, esya.transform.position);
                bool veriVar = esya.Veri != null;
                string sonuc = veriVar
                    ? EtkilesimKurallari.Degerlendir(esya.Veri, false).ToString()
                    : "VERI YOK";

                // Esyanin merkezine dogru isin at: carpan ilk sey esya mi?
                Vector3 hedef = esya.transform.position + Vector3.up * 0.5f;
                Vector3 baslangic = hedef - (hedef - kamera.transform.position).normalized * 2.0f;
                Vector3 yon = (hedef - baslangic).normalized;

                string vurusAdi = "(bosluk)";
                bool esyayaUlasti = false;

                if (Physics.Raycast(baslangic, yon, out RaycastHit vurus, 3f, ~0, QueryTriggerInteraction.Ignore))
                {
                    vurusAdi = vurus.collider.name;
                    esyayaUlasti = vurus.collider.GetComponentInParent<Esya>() == esya;
                }

                rapor.AppendLine(
                    $"  {esya.name,-24} mesafe {mesafe,6:F1} m | kural: {sonuc,-16} " +
                    $"| isin: {vurusAdi,-24} | esyaya ulasti: {esyayaUlasti}");
            }

            rapor.AppendLine();
            rapor.AppendLine($"Erisim mesafesi ayari icin: en yakin esya {EnYakin(kamera.transform.position, esyalar):F1} m uzakta.");
            Debug.Log(rapor.ToString());
        }

        static float EnYakin(Vector3 nokta, Esya[] esyalar)
        {
            float en = float.MaxValue;
            foreach (var e in esyalar)
            {
                float d = Vector3.Distance(nokta, e.transform.position);
                if (d < en) en = d;
            }
            return en == float.MaxValue ? -1f : en;
        }
    }
}
