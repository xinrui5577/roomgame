using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.sss.Tool
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
        /// 根据名字返回gob对象
        /// </summary>
        /// <returns></returns>
        public static GameObject GobSelectName(GameObject[] gobs,string name)
        {
            foreach (GameObject gob in gobs)
            {
                if (gob.name.Equals(name))
                {
                    return gob;
                }
            }

            return null;
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
            return new Color(r / 255, g / 255, b / 255, 1);
        }

        public static void SetLabelColor(UILabel label, int val)
        {
            if(val >= 0)
            {
                label.gradientTop = ChangeToColor(0xFFFF00);
                label.gradientBottom = ChangeToColor(0xFF9600);
            }
            else
            {
                label.gradientTop = ChangeToColor(0x77FFF9);
                label.gradientBottom = ChangeToColor(0x0060FF);
            }
        }

    }
}
