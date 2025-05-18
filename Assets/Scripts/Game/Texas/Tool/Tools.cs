using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.Texas.Tool
{
    public class Tools : MonoBehaviour {
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
        /// 通过Hex的值获取32位颜色
        /// </summary>
        /// <param name="hex">十六进制数0xFFFFFF</param>
        /// <returns></returns>
        public static Color ChangeToColor(int hex)
        {
            float one = (hex >> 16) & 0x0000FF;
            float two = (hex >> 8) & 0x0000FF;
            float three = hex & 0x0000FF;
            Debug.Log("three == " + (three / 225));
            return new Color(one / 255, two / 255, three / 255, 1);
        }
    }
}
