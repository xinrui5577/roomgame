using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Utils
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class GlobalUtils
    {
        /// <summary>
        /// 给游戏体添加一个子游戏体
        /// </summary>
        /// <param name="child">子游戏体</param>
        /// <param name="parent">父游戏体</param>
        /// <param name="localPos">相对父游戏体的位置</param>
        /// <param name="localScale">相对父游戏体的缩放</param> 
        public static void AddGameObjectChild(GameObject child, GameObject parent, Vector3 localPos, Vector3 localScale)
        {
            var ts = child.transform;
            ts.parent = parent.transform;
            ts.localPosition = localPos;
            ts.localScale = localScale; 
        }
         
        /// <summary>
        /// 给游戏体添加一个子游戏体
        /// </summary>
        /// <param name="child">子游戏体</param>
        /// <param name="parent">父游戏体</param>
        /// <param name="localPos">相对父游戏体的位置</param>
        public static void AddGameObjectChild(GameObject child, GameObject parent, Vector3 localPos)
        {
            AddGameObjectChild(child, parent, localPos, Vector3.one);
        }

        /// <summary>
        /// 给游戏体添加一个子游戏体
        /// </summary>
        /// <param name="child">子游戏体</param>
        /// <param name="parent">父游戏体</param>
        /// <param name="keepingTranform">是否保持自身属性</param>
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
        /// 设置游戏体初始状态
        /// </summary>
        /// <param name="tf"></param>
        public static void IdentyTranform(Transform tf)
        {
            tf.localPosition = Vector3.zero;
            tf.localScale = Vector3.one;
            tf.localRotation = Quaternion.identity;
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
        
        /// <summary>
        /// 由一个字符串生成MD5值
        /// </summary>
        /// <param name="originalStr">字符串</param>
        /// <returns></returns>
        public static string Md5String(string originalStr) {
            var md5 = MD5.Create(); 
            var bts = md5.ComputeHash(Encoding.UTF8.GetBytes(originalStr));
            return bts.Aggregate("", (current, t) => current + t.ToString("x2"));//foreach (var t in bts) {originalStr += t.ToString("x2");}
        }
  
        /// <summary>
        /// 把所有子游戏体的layer都变成父游戏体的layer
        /// </summary>
        /// <param name="tf"></param>
        public static void ChangeLayer(Transform tf) {
            ChangeLayer(tf, tf.parent.gameObject.layer);
        }

        /// <summary>
        /// 把所有子游戏体的layer都变成指定的层
        /// </summary>
        /// <param name="tf">指定的游戏体</param>
        /// <param name="layer">层</param>
        public static void ChangeLayer(Transform tf, int layer) {
            tf.gameObject.layer = layer;
            if (tf.childCount <= 0) return;
            for (var i = tf.childCount - 1; i >= 0; i--) {
                ChangeLayer(tf.GetChild(i), layer);
            }
        }

        /// <summary>
        /// 删除数组中指定的值
        /// </summary>
        /// <param name="arr">数组</param>
        /// <param name="value">值</param>
        /// <returns>返回一个新的数组</returns>
        public static int[] RemoveIntFormArray(int[] arr, int value) {
            var len = arr.Length;
            int i;
            var index = -1;
            for (i = 0; i < len; i++) {
                if (arr[i] != value) continue;
                index = i;
                break;
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
  
        private static readonly float[] BtnClickTime ={0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
        public static bool CheckButClick(int btnXiaZhu)
        {
            var time = Time.time;
            if (BtnClickTime[btnXiaZhu] + 0.5f > time)
            {
                return false;
            }
            BtnClickTime[btnXiaZhu] = time;
            return true;
        }

        /// <summary>
        /// 加密/解密
        /// </summary>
        /// <param name="content"></param>
        /// <param name="length">0到指定的长度，未指定（-1）则整个数组长度</param>
        public static void Encryption(byte[] content,int length=-1)
        {
            //return;
            var len = length > 0 ? length : content.Length;
            var pwd = new byte[] { 5, 1, 8 ,8 };
            var pwdLen = pwd.Length;
            for (var i = 0; i < len; i++)
            {
                content[i] ^= pwd[i % pwdLen];
            }
        }
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static byte[] StringEncryption(string content, Encoding code)
        {
            var bytes = code.GetBytes(content);
            Encryption(bytes);
            return bytes;
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="content"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string StringDeciphering(string content, Encoding code)
        {
            var bytes = StringEncryption(content, code);
            return code.GetString(bytes);
        }

        public static T CreateGameObjecet<T>(string goName) where T : MonoBehaviour
        {
            var go = new GameObject("GameServer");
            return go.AddComponent<T>();
        } 
    }
}