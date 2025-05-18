using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.Single
{
    /// <summary>
    ///     有一点小问题，就是这个走mono的生命周期，你懂得.....
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        private static readonly object SafeLock = new object();

        public static T Instance
        {
            get
            {
                lock (SafeLock)
                {
                    if (_instance == null)
                    {
                        var obj = new GameObject(typeof (T).Name);
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
            set
            {
                lock (SafeLock)
                {
                    _instance = value;
                }
            }
        }

        public void DelMe()
        {
            if (_instance != null)
            {
                lock (SafeLock)
                {
                    _instance = null;
                }
            }
        }


        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public virtual void OnDestroy()
        {
            DelMe();
        }
    }
}