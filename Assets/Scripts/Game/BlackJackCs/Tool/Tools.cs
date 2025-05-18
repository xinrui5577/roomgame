using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.BlackJackCs.Tool
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
        /// 黑杰克图片名称
        /// </summary>
        public const string BLACKJACK = "blackjack";

        /// <summary>
        /// 失败的图片名称
        /// </summary>
        public const string LOST = "lost";


        /// <summary>
        /// 为NGUI对象添加点击事件
        /// </summary>
        /// <param name="gob">点击对象</param>
        /// <param name="callback">监听事件</param>
        /// <param name="ID">ID</param>
        public static void NguiAddOnClick(GameObject gob, UIEventListener.VoidDelegate callback,int ID)
        {
            UIEventListener uiel = UIEventListener.Get(gob);
            uiel.onClick = callback;
            uiel.parameter = ID;
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
        /// 获取钱
        /// </summary>
        /// <param name="gold"></param>
        /// <returns></returns>
        public static string GetShowGold(float gold, bool isPic = false)
        {
            string unit = "";
            string tempStr = string.Empty;
            float tempGold = gold >= 0 ? gold : -gold;

            if (tempGold < 100)
            {
                tempStr = tempGold.ToString("0.##");
            }
            else if (tempGold >= 100 && tempGold < 10000)
            {
                tempStr = tempGold.ToString("0");
            }
            else if (tempGold >= 10000 && tempGold < 100000)
            {
                tempStr = (tempGold / 10000).ToString("0.##");
                unit = isPic ? "w" : "万";
            }
            else if (tempGold >= 100000 && tempGold < 10000000)
            {
                tempStr = (tempGold / 10000).ToString("0.#");
                unit = isPic ? "w" : "万";
            }
            else if (tempGold >= 10000000 && tempGold < 100000000)
            {
                tempStr = (tempGold / 10000).ToString("0");
                unit = isPic ? "w" : "万";
            }
            else if (tempGold >= 100000000 && tempGold < 1000000000)
            {
                tempStr = (tempGold / 100000000).ToString("0.##");
                unit = isPic ? "y" : "亿";
            }
            else if (tempGold > 1000000000)
            {
                tempStr = (tempGold / 100000000).ToString("0");
                unit = isPic ? "y" : "亿";
            }

            return (gold >= 0 ? "" : "-") + tempStr + unit;
        }


        /// <summary>
        /// 通过Hex的值获取32位颜色
        /// </summary>
        /// <param name="Hex">十六进制数0xFFFFFF</param>
        /// <returns></returns>
        public static Color ChangeToColor(int Hex)
        {
            float R = (Hex >> 16) & 0x0000FF;
            float G = (Hex >> 8) & 0x0000FF;
            float B = Hex & 0x0000FF;
            //Debug.Log("G == " + (G / 225));
            return new Color(R / 255, G / 255, B / 255, 1);
        }
        


        public static void MoveView(GameObject obj, Vector3 from, Vector3 to,float time = 0.2f)
        {
            TweenPosition ts = obj.GetComponent<TweenPosition>() ?? obj.AddComponent<TweenPosition>();
            ts.from = from;
            ts.to = to;
            ts.duration = 0.2f;
            ts.ResetToBeginning();
            ts.PlayForward();
        }
    }
}
