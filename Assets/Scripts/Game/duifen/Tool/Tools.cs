using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Game.duifen.Tool
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

        public static GameObject GobGet(GameObject[] gobs , string name)
        {
            return gobs.FirstOrDefault(t => t.name.Equals(name));
        }


        /// <summary>
        /// 获取钱
        /// </summary>
        /// <param name="gold">数值</param>
        /// <param name="isPic">是否是图片</param>
        /// <returns></returns>
        public static string GetShowGold(float gold,bool isPic = false)
        {
            string unit = "";
            string tempStr = string.Empty;
            float tempGold = gold >= 0 ? gold : -gold;

            if (tempGold < 1000)
            {
                tempStr = tempGold.ToString("0.##");
            }
            else if (tempGold >= 1000 && tempGold < 10000)
            {
                tempStr = tempGold.ToString("0.#");
            }
            else if (tempGold >= 10000 && tempGold < 100000)
            {
                tempStr = (tempGold / 10000).ToString("0.##");
                unit = isPic ? "w" : "万";
            }
            else if (tempGold >= 100000 && tempGold < 10000000)
            {
                tempStr = (tempGold / 10000).ToString("0.#");
                unit =isPic? "w" : "万";
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
