using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Game.Mahjong3D.EditorTool
{
    public class EditorHelper
    {      
        public static T CreateAssets<T>()where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, "Assets/" + typeof(T).Name + ".asset");
            AssetDatabase.SaveAssets();
            return asset;
        }


    }
}