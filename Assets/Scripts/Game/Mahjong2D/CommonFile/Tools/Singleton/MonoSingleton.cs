using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton
{
    /// <summary>
    /// 有一点小问题，就是这个走mono的生命周期，你懂得.....
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoSingleton<T>:MonoBehaviour where T: Component
    {
        private static T _instance;

        private static readonly  object _safeLock=new object();

        public static T Instance
        {

            get
            {
                lock (_safeLock)
                {
                    if (_instance==null)
                    {
                        GameObject obj =new GameObject(typeof (T).Name);
                        _instance=obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
            set
            {
                lock (_safeLock)
                {
                    _instance = value;
                }
            }
        }

        public void DelMe()
        {
            if(_instance!=null)
            {
                lock(_safeLock)
                {
                    //LTools.LogSystem("Yellow", "删除Mono单例" + _instance.GetType());
                    _instance = null;             
                }
            }
        }


        public virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                //LTools.LogSystem("Yellow", "Mono单例" + _instance.GetType().Name + "加载完毕,加载为本身,挂在对象名称为"+_instance.name);
            }
            else
            {
                //LTools.LogSystem("Yellow", "Mono单例" + _instance.GetType().Name + "存在" + _instance.name);
                Destroy(gameObject);
            }
           
        }

        public virtual void OnDestroy()
        {
            DelMe();
        }
    }
}
