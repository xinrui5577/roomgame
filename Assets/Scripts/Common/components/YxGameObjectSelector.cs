using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.components
{
    public class YxGameObjectSelector : YxView
    {
        public int DefaultIndex;
        public GameObject[] GameObjects;

        protected override void OnAwake()
        {
            base.OnAwake();
            Change(DefaultIndex);
        } 

        public T Change<T>(int index) where T : MonoBehaviour
        {
            var go = Change(index);
            return go == null ? null : go.GetComponent<T>();
        }

        public GameObject Change(int index)
        {
            var count = GameObjects.Length;
            GameObject mb = null;
            for (var i = 0; i < count; i++)
            {
                var go = GameObjects[i];
                if (go == null) continue;
                if (i == index)
                {
                    go.SetActive(true);
                    mb = go;
                }
                else
                {
                    go.SetActive(false);
                }
            }
            return mb;
        }
    }
}
