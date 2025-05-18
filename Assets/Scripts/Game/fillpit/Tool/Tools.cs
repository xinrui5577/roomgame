using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.fillpit.Tool
{
    public class Tools : MonoBehaviour {

        public static readonly string AssetBundlePath =
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            "file://" + Application.dataPath + "/StreamingAssets/";
#elif UNITY_ANDROID
            "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
            "file://"+Application.dataPath + "/Raw/";
#endif

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
        /// 通过Hex的值获取32位颜色
        /// </summary>
        /// <param name="hex">十六进制数0xFFFFFF</param>
        /// <returns></returns>
        public static Color ChangeToColor(int hex)
        {
            float r = (hex >> 16) & 0x0000FF;
            float g = (hex >> 8) & 0x0000FF;
            float b = hex & 0x0000FF;
            //Debug.Log("G == " + (G / 225));
            return new Color(r / 255, g / 255, b / 255, 1);
        }
    }
}
