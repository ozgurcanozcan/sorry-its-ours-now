using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Oyun.Field.Editor
{
    // Girdi varligina "Birak" eylemini ekler.
    //
    // Neden arac: .inputactions bir JSON dosyasi ama icindeki id alanlari
    // GUID. Elle yazmak bozuk varlik uretme riski tasiyor; Unity'nin kendi
    // API'si gecerli id uretiyor. Bir kez calistirilir, sonuc commit edilir.
    public static class GirdiEylemiEkle
    {
        const string Yol = "Assets/InputSystem_Actions.inputactions";

        [MenuItem("Saha/Girdi: Birak eylemini ekle")]
        public static void Ekle()
        {
            var varlik = AssetDatabase.LoadAssetAtPath<InputActionAsset>(Yol);
            if (varlik == null)
            {
                Debug.LogError($"Girdi varligi bulunamadi: {Yol}");
                return;
            }

            InputActionMap harita = varlik.FindActionMap("Player", true);

            if (harita.FindAction("Birak") != null)
            {
                Debug.Log("Birak eylemi zaten var.");
                return;
            }

            // Varlik uzerinde degisiklik yapmak icin once devre disi birakilir.
            varlik.Disable();

            InputAction eylem = harita.AddAction("Birak", InputActionType.Button);
            eylem.AddBinding("<Keyboard>/q", groups: "Keyboard&Mouse");
            eylem.AddBinding("<Gamepad>/buttonEast", groups: "Gamepad");

            System.IO.File.WriteAllText(Yol, varlik.ToJson());
            AssetDatabase.ImportAsset(Yol, ImportAssetOptions.ForceUpdate);

            Debug.Log("Birak eylemi eklendi: Q (klavye), buttonEast (gamepad).");
        }
    }
}
