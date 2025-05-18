using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.sanpian.CommonCode.ChessCommon
{
    public class GlobalUtils
    {
        public static void AddGameObjectChild(GameObject child, GameObject parent, Vector3 localPos, Vector3 localScale)
        {
            child.transform.parent = parent.transform;
            child.transform.localPosition = localPos;
            child.transform.localScale = localScale;
        }

        public static void IdentyTranform(Transform tf)
        {
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;
            tf.localRotation = Quaternion.identity;
        }

        public static void AddGameObjectChild(GameObject child, GameObject parent, Vector3 localPos)
        {
            AddGameObjectChild(child, parent, localPos, Vector3.one);
        }

        public static void AddGameObjectChild(GameObject child, GameObject parent, bool keepingTranform = false)
        {
            if (keepingTranform)
            {
                AddGameObjectChild(child, parent, child.transform.localPosition, child.transform.localScale);
            }
            else
            {
                AddGameObjectChild(child, parent, Vector3.zero);
            }
        }
        /// <summary>
        /// 获取常规的Stretch的对象，通常为了在这个go下面显示你的go
        /// </summary>
        /// <returns></returns>
        public static GameObject GetStretchGameObject()
        {
            GameObject camera = Camera.main.gameObject;
            UIStretch[] strech = camera.GetComponentsInChildren<UIStretch>();
            foreach (var uiStretch in strech)
            {
                if (uiStretch.style != UIStretch.Style.Both)
                {
                    return uiStretch.gameObject;
                }
            }
            return camera;
        }

        /// <summary>
        /// 开关的值，只有0和1
        /// 该方法主要是为了方便读配置文件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetSwitchValue(string str) {
            if (!string.IsNullOrEmpty(str) && int.Parse(str) > 0) {
                return 1;
            }
            return 0;
        }

//
//    /// <summary>
//    /// 该方法主要是为了在关闭某页面时，产生特效
//    /// </summary>
//    /// <param name="go"></param>
//    public static void DestroyGameObject(GameObject go)
//    {
//        if (go == null)
//        {
//            return;
//        }
//
//        TweenScale ts = go.AddComponent<TweenScale>();
//        ts.from = Vector3.one;
//        ts.to = Constants.ZERO_SCREEN_VECTOR3;
//        ts.duration = 0.2f;
//        //ts.delay = 0.1f;
//        ts.method = UITweener.Method.BounceOut;
//        ts.onFinished = tween =>
//            {
//                if (go.activeInHierarchy)
//                {
//                    NGUITools.Destroy(go);
//                }
//            };
//        ts.enabled = true;
//    }
//    /// <summary>
//    /// 该方法主要是为了在页面显示时，能够有特效
//    /// </summary>
//    /// <param name="go"></param>
//    public static void DisplayGameObject(GameObject go)
//    {
//        TweenScale ts = go.AddComponent<TweenScale>();
//        ts.from = Constants.ZERO_SCREEN_VECTOR3;
//        ts.to = Vector3.one;
//        ts.duration = 0.3f;
//        //ts.delay = 0.1f;
//        ts.method = UITweener.Method.BounceIn;
//        ts.enabled = true;
//    }

//    
//    /// <summary>
//    /// 
//    /// </summary>
//    /// <param name="tex">需要显示的texture</param>
//    /// <param name="url">img的url</param>
//    /// <returns></returns>
//    public static IEnumerator FillUrlImg(UITexture tex, string url)
//    {
//
//        if (string.IsNullOrEmpty(url))
//        {
//            yield return null;
//        }
//        else
//        {
//            WWW www = new WWW(url);
//            yield return www;
//            if (www.isDone && string.IsNullOrEmpty(www.error))
//            {
//                tex.mainTexture = www.texture;
//            }
//        }
//    }

        /// <summary>
        /// 获取声音，如果音乐关闭，则不返回AudioClip，也不会播放
        /// </summary>
        /// <param name="audioName"></param>
        /// <returns></returns>
        //public static AudioClip GetAudioClip(string audioName) {
        //    //if (!SanPianGameData.IsMusicOn)
        //    //{
        //    //    return null;
        //    //}
        //    return Resources.Load(audioName, typeof (AudioClip)) as AudioClip;
        //}

        public static string MD5String(string originalStr) {
            MD5 md5 = MD5.Create();
            byte[] bts = md5.ComputeHash(Encoding.UTF8.GetBytes(originalStr));
            originalStr = "";
            foreach (byte t in bts) {
                originalStr += t.ToString("x2");
            }
            return originalStr;
        }


        public static void TraceBaseEvent(BaseEvent eEvent) {
            Debug.Log("----------------------trace BaseEvent-----------------------------:" + eEvent.GetType());
            IDictionaryEnumerator a = eEvent.Params.GetEnumerator();
            while (a.MoveNext()) {
                Debug.Log("key:" + a.Key);
                SFSObject o = a.Value as SFSObject;
                Debug.Log(o != null ? o.GetDump() : a.Value);
            }
            Debug.Log("=====================trace BaseEvent end===========================");
        }

        /// <summary>
        /// 把所有孩子的layer都变成父亲的layer
        /// </summary>
        /// <param name="tf"></param>
        public static void ChangeLayer(Transform tf) {
            ChangeLayer(tf, tf.parent.gameObject.layer);
        }

        /// <summary>
        /// 把所有孩子的layer都变成指定的层
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="layer"></param>
        public static void ChangeLayer(Transform tf, int layer) {
            tf.gameObject.layer = layer;
            if (tf.childCount > 0) {
                for (int i = tf.childCount - 1; i >= 0; i--) {
                    ChangeLayer(tf.GetChild(i), layer);
                }
            }
        }

        public static int[] RemoveIntFormArray(int[] arr, int value) {
            int len = arr.Length;
            int i;
            int index = -1;
            for (i = 0; i < len; i++) {
                if (arr[i] == value) {
                    index = i;
                    break;
                }
            }
            if (index == -1) {
                return arr;
            }
            int[] newArr = new int[len - 1];
            for (i = 0; i < index; i++) {
                newArr[i] = arr[i];
            }
            for (i = index + 1; i < len; i++) {
                newArr[i - 1] = arr[i];
            }
            return newArr;
        }

        public static IEnumerator ShowTextAni(UILabel label, string txt, float time) {
            if (txt == null) {
                txt = "";
            }
            int len = txt.Length;
            for (int i = 1; i <= len; i++) {
                label.text = txt.Substring(0, i);
                yield return new WaitForSeconds(time);
            }
        }

        public static IEnumerator ShowNumAni(UILabel label, int num, float time) {
            int rate = num/10;
            if (rate == 0) {
                rate = num > 0 ? 1 : (num < 0) ? -1 : 0;
            }
            int total = 0;
            WaitForSeconds wait = new WaitForSeconds(time);
            for (int i = 0; i < 10; i++) {
                total += rate;
                label.text = total.ToString();
                yield return wait;
            }  
            label.text = num.ToString();
        }

        public static readonly DateTime DateTimeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();

        /// <summary>
        /// 将服务器时间戳转化为DateTime
        /// </summary>
        /// <param name="serverTimestamp"></param>
        /// <returns></returns>
        public static DateTime ConvertServerTimestamp2DateTime(long serverTimestamp) {
            return DateTimeStart + TimeSpan.FromSeconds(serverTimestamp);
        }

        public static string GetDateString(long serverTimestamp) {
            return ConvertServerTimestamp2DateTime(serverTimestamp).ToString("yyyy-M-d H:m:s");
        }

        internal static void DebugLog(string p)
        {
//#if UNITY_EDITOR||DEBUG
            Debug.Log(p);
//#endif
        }

        internal static void ErrorLog(string p)
        {
            Debug.LogError(p);
        }

        public static string[] GoldChengHao(long gold)
        {
            string[] ch = new string[2];
            int[] config = { 0, 5, 50, 200, 500, 1000, 2000, 4000, 10000, 20000, 30000, 40000, 50000,9999999 };
            string[] nameConcig = { "救济户", "包身工", "贫民", "房奴", "小资", "富翁", "小土豪", "大土豪", "千万富翁", "两千万富翁", "三千万富翁", "四千万富翁", "超级大富豪" };
            long max = 0;
            for (int i = 0; i < config.Length-1; i++)
            {
                long min = max;
                max = config[i+1]*1000;
                if (gold >= min && gold < max)
                {
                    ch[0] = nameConcig[i];
                    ch[1] = GoldStar(gold, min, max);
                    break;
                }
            }
           
            return ch;
        }

        private static string GoldStar(long gold,long min, long total)
        {
            long rate = ((total - min)/10);
            long len = (gold - min)/rate + 1;
            if (len < 1)
            {
                len = 1;
            }else if (len > 10)
            {
                len = 10;
            }
            string stars = "";
            for (int i = 0; i < len; i++)
            {
                stars += "★";
            }
            return stars;
        }
    }
}