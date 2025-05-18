using Assets.Scripts.Game.jpmj.MahjongScripts.Public.Adpater;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.Editor
{
    public class ScriptableObjectEditor 
    {
        [MenuItem("Assets/AdapterConfig", false, 0)]
        public static void CreateAdapterConfig()
        {
            ScriptableObject asset = ScriptableObject.CreateInstance<AdpaterConfigAssets>();
            AssetDatabase.CreateAsset(asset, "Assets/" + typeof(AdpaterConfigAssets).Name + ".asset");
            AssetDatabase.SaveAssets();
        }

    }
}
