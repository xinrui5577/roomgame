using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.Base
{
    public class Single<T> : MonoBehaviour where T : Single<T>
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public static void Gc()
        {
            if (_instance == null) return;
            Destroy(_instance.gameObject);
            _instance = null;
        }
    }
}
