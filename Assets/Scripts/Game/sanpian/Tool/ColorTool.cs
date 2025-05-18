using UnityEngine;

namespace Assets.Scripts.Game.sanpian.Tool
{
    public class ColorTool {

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
    }
}
