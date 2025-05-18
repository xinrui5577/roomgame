using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{
    public class MahjongToolExtension
    {
        public static T InstantiateGameObject<T>(string name, Transform parent) where T : MonoBehaviour
        {
            GameObject go = InstantiateAsset(name);
            if (null == go) return null;
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            return go.GetComponent<T>();
        }

        protected static GameObject InstantiateAsset(string name)
        {
            GameObject asset = ResourceManager.LoadAsset(name, name);
            GameObject go = null;
            if (null != asset)
            {
                go = Object.Instantiate(asset);
                go.name = name;
            }
            return go;
        }

    }
}
