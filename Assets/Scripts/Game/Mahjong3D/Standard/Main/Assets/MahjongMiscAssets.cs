using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [System.Serializable]
    public struct ObjectItem
    {
        public string Name;
        public Object Object;
    }

    [System.Serializable]
    public class ObjectData : DataTable<ObjectItem> { }

    [CreateAssetMenu(fileName = "MahjongMiscAssets", menuName = "Mahjong Scriptable/MahjongMiscAssets", order = 101)]
    public class MahjongMiscAssets : ScriptableObjBast
    {
        [Table(typeof(ObjectItem))]
        public ObjectData ObjectData;

        public T GetAssetComponent<T>(string name) where T : Component
        {
            var obj = GetAssetToObj(name);
            if (null != obj)
            {
                T cp = obj.GetComponent<T>();
                if (null != cp)
                {
                    return cp;
                }
                else
                {
                    com.yxixia.utile.YxDebug.YxDebug.LogError("MahjongMiscAssets --> " + name + " 组件不存在！");
                }
            }
            return null;
        }

        public GameObject GetAssetToObj(string name)
        {
            return GetAsset<GameObject>(name);
        }

        public T GetAsset<T>(string name) where T : Object
        {
            ObjectItem obj = default(ObjectItem);
            for (int i = 0; i < ObjectData.Count; i++)
            {
                var temp = ObjectData[i];
                if (temp.Name.Equals(name))
                {
                    obj = temp;
                    break;
                }
            }
            if (null != obj.Object)
            {
                return obj.Object as T;
            }
            else
            {
                com.yxixia.utile.YxDebug.YxDebug.LogError("MahjongMiscAssets --> " + name + " 资源不存在！");
            }
            return null;
        }
    }
}