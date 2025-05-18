using System.Collections;
using System.Globalization;
using System.Linq;
using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Ttzkf
{
    public class TtzTools : MonoBehaviour {

        public static readonly string AssetBundlePath =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
            "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
            "file://"+Application.dataPath + "/Raw/";
#endif

        public static readonly string AssetBundleName =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            "Show_Windows";
#elif UNITY_ANDROID
            "Show_Android";
#elif UNITY_IPHONE
            "Show_IOS";
#endif

        /// <summary>
        /// 读取对应的texture(单一资源使用此方法，需要修改下载地址)
        /// </summary>
        public static IEnumerator LoadTexture(Transform trans, string name)
        {
            //Log.L("Bundle +++++++++++++");
            WWW bundle =
                WWW.LoadFromCacheOrDownload(
                    "",
                    6);//Tools.AssetBundlePath + name, 0);
            yield return bundle;

            YxDebug.Log("Bundle = " + bundle.error);

            var thisTexture = bundle.assetBundle.LoadAsset(name) as Texture;

            bundle.assetBundle.Unload(false);
            bundle.Dispose();

            //Log.L("Bundle +++++++++++++");

            if (trans != null)
            {
                trans.GetComponent<UITexture>().mainTexture = thisTexture;
            }
            yield return null;
        }

        public delegate void NullDelegate();

        public static AssetBundle Ab;
        /// <summary>
        /// 读取整块资源
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static IEnumerator LoadBundle(NullDelegate callback = null)
        {
            WWW bundle =
                WWW.LoadFromCacheOrDownload(
                    "",
                    1);
            yield return bundle;

            if (bundle.error != null )
            {
                YxDebug.Log("Error = " + bundle.error);
            }
            Ab = bundle.assetBundle;
            if (callback != null)
            {
                callback();
            }
        }
        /// <summary>
        /// 从整块资源中加载图片
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="name"></param>
        public static void LoadTexturefromAb(Transform tran,string name)
        {
            var thisTexture = Ab.LoadAsset(name) as Texture;
            if (tran != null)
            {
                tran.GetComponent<UITexture>().mainTexture = thisTexture;
            }
        }

        /// <summary>
        /// 为NGUI对象添加点击事件
        /// </summary>
        /// <param name="gob">点击对象</param>
        /// <param name="callback">监听事件</param>
        /// <param name="id">ID</param>
        public static void NguiAddOnClick(GameObject gob, UIEventListener.VoidDelegate callback,int id)
        {
            UIEventListener uiel = UIEventListener.Get(gob);
            uiel.onClick = callback;
            uiel.parameter = id;
        }

        /// <summary>
        /// 根据名字只显示唯一的gob
        /// </summary>
        public static GameObject GobShowOnlyOne(GameObject[] gobs,string name)
        {
            int index = -1;

            for (int i = 0; i < gobs.Length; i++)
            {
                GameObject gob = gobs[i];
                if (gob.name.Equals(name))
                {
                    gob.SetActive(true);
                    index = i;
                }
                else
                {
                    gob.SetActive(false);
                }
            }

            return index == -1 ? null : gobs[index];
        }

        /// <summary>
        /// 根据名字返回gob对象
        /// </summary>
        /// <returns></returns>
        public static GameObject GobSelectName(GameObject[] gobs,string name)
        {
            return gobs.FirstOrDefault(gob => gob.name.Equals(name));
        }

        /// <summary>
        /// 获取字典int
        /// </summary>
        public static int GetDicInt(IDictionary dic,object key)
        {

            if (!dic.Contains(key) || dic[key] == null) return 0;

            return int.Parse(dic[key].ToString());

        }
        /// <summary>
        /// 获取字典bool
        /// </summary>
        public static bool GetDicBool(IDictionary dic, object key)
        {
            return dic.Contains(key) && dic[key] != null && (bool) dic[key];
        }
        /// <summary>
        /// 获取字典string
        /// </summary>
        public static string GetDicString(IDictionary dic, object key)
        {
            if (!dic.Contains(key) || dic[key] == null) return "";

            return dic[key].ToString();
        }
        /// <summary>
        /// 获取钱
        /// </summary>
        /// <param name="gold"></param>
        /// <returns></returns>
        public static string GetShowGold(int gold)
        {
            int len = gold.ToString(CultureInfo.InvariantCulture).Length;

            if (len < 5)
            {
                return gold.ToString(CultureInfo.InvariantCulture);
            }
            if (len >= 5 && len < 6)
            {
                return (gold/10000f).ToString("f2") + "万";
            }
            return (gold / 10000).ToString(CultureInfo.InvariantCulture) + "万";
        }
    }
}
