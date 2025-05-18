using System;
using System.Security.Cryptography;
using System.Text;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.Ttzkf
{
    public class GlobalUtils
    {
        public static void AddGameObjectChild(GameObject child, GameObject parent, Vector3 localPos, Vector3 localScale)
        {
            child.transform.parent = parent.transform;
            child.transform.localPosition = localPos;
            child.transform.localScale = localScale;
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

        public static string Md5String(string originalStr) {
            MD5 md5 = MD5.Create();
            byte[] bts = md5.ComputeHash(Encoding.UTF8.GetBytes(originalStr));
            originalStr = "";
            foreach (byte t in bts) {
                originalStr += t.ToString("x2");
            }
            return originalStr;
        }


        public static void TraceBaseEvent(BaseEvent eEvent) {
            #if DEBUG||UNITY_EDITOR
            
                var a = eEvent.Params.GetEnumerator();
                while (a.MoveNext()) 
                {                  
                    var o = a.Value as SFSObject;
                }          
            #endif
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
            var newArr = new int[len - 1];
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
            var wait = new WaitForSeconds(time);
            for (int i = 0; i < 10; i++) {
                total += rate;
                label.text = total+"";
                yield return wait;
            }  
            label.text = num+"";
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

  
    }
}