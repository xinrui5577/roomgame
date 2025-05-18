using com.yxixia.utile.YxDebug;
using UnityEngine; 

namespace Assets.Scripts.Game.duifen.Base
{
    /// <summary>
    /// 单例父类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    //public class Instance<T> : MonoBehaviour where T : Instance<T>
    //{

    //    private static T _instance;

    //    public static T GetInstance()
    //    {
    //        if (_instance == null)
    //        {
    //            var go = new GameObject();
    //            _instance = go.AddComponent<T>();
    //            YxDebug.LogError("_instance为空!!");
    //        }
    //        return _instance;
    //    }

    //    protected void Awake()
    //    {
    //        _instance = GetComponent<T>();
    //    }

    //    // ReSharper disable once ArrangeTypeMemberModifiers
    //    // ReSharper disable once UnusedMember.Local
    //    void OnDestroy()
    //    {
    //        Destroy(_instance);
    //        _instance = null;
    //    }
    //}
}
