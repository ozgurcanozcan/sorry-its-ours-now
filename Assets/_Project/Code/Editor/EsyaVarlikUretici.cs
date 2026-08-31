using System.Collections.Generic;
using System.Text;
using Oyun.Core;
using UnityEditor;
using UnityEngine;

namespace Oyun.Editor
{
    // EsyaKatalogu'ndan ScriptableObject varliklari uretir (karar: 31.08.2026).
    //
    // Katalog dogruluk kaynagi; bu script sadece onu Unity'nin gorebilecegi
    // bicime cevirir. Varliklar prefab'lara GUID ile bagli oldugu icin mevcut
    // dosya SILINIP yeniden yaratilmiyor, uzerine yaziliyor -- silinseydi her
    // uretimde Onat'in butun prefab baglari kopardi.
    public static class EsyaVarlikUretici
    {
        public const string Klasor = "Assets/_Project/Data/Items";

        [MenuItem("Oyun/Esya varliklarini uret")]
        public static void Uret()
        {
            KlasoruHazirla();

            int yeni = 0;
            int guncellenen = 0;

            foreach (var veri in EsyaKatalogu.Hepsi)
            {
                string yol = Yol(veri.Id);
                var varlik = AssetDatabase.LoadAssetAtPath<EsyaTanimi>(yol);

                if (varlik == null)
                {
                    varlik = ScriptableObject.CreateInstance<EsyaTanimi>();
                    varlik.KatalogdanYaz(veri);
                    AssetDatabase.CreateAsset(varlik, yol);
                    yeni++;
                }
                else
                {
                    varlik.KatalogdanYaz(veri);
                    EditorUtility.SetDirty(varlik);
                    guncellenen++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Esya varliklari uretildi: " + yeni + " yeni, " + guncellenen + " guncellendi.");

            // Silmiyoruz: prefab'lar GUID ile bagli, silinen varlik sessizce
            // kopuk referans birakir. Insana soyleniyor, karari o veriyor.
            var oksuz = OksuzVarliklar();
            if (oksuz.Count > 0)
            {
                var metin = new StringBuilder();
                metin.Append("Katalogda karsiligi olmayan esya varligi var. Prefab baglari ");
                metin.Append("kopmasin diye silinmedi; kullanilmadigindan eminsen elle sil:");
                foreach (string yol in oksuz) metin.Append("\n  ").Append(yol);
                Debug.LogWarning(metin.ToString());
            }
        }

        /// <summary>Katalogda karsiligi kalmamis varliklarin yollari.</summary>
        public static List<string> OksuzVarliklar()
        {
            var oksuz = new List<string>();
            if (!AssetDatabase.IsValidFolder(Klasor)) return oksuz;

            foreach (string guid in AssetDatabase.FindAssets("t:" + nameof(EsyaTanimi), new[] { Klasor }))
            {
                string yol = AssetDatabase.GUIDToAssetPath(guid);
                var varlik = AssetDatabase.LoadAssetAtPath<EsyaTanimi>(yol);
                if (varlik == null || !EsyaKatalogu.VarMi(varlik.Id)) oksuz.Add(yol);
            }

            return oksuz;
        }

        public static string Yol(string id)
        {
            return Klasor + "/" + id + ".asset";
        }

        static void KlasoruHazirla()
        {
            if (AssetDatabase.IsValidFolder(Klasor)) return;

            string birikim = "Assets";
            foreach (string parca in Klasor.Substring("Assets/".Length).Split('/'))
            {
                string sonraki = birikim + "/" + parca;
                if (!AssetDatabase.IsValidFolder(sonraki)) AssetDatabase.CreateFolder(birikim, parca);
                birikim = sonraki;
            }
        }
    }
}
