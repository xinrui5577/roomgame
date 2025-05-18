using System;
using UnityEngine;

/// <summary>
/// 颜色slider，从NGUI中拿过来的用用
/// </summary>
namespace Assets.Scripts.Game.Mahjong2D.Common.UI
{
    public class ColorSelect : MonoBehaviour
    {
        [SerializeField]
        private UITexture _changeTex;
        [SerializeField]
        private Color[] _colors = new Color[] { Color.red, Color.yellow, Color.green };
        public Action<Color> OnColorChange;

        public void OnValueChange(float val)
        {
            if (_changeTex == null || _colors.Length == 0) return;
            val *= (_colors.Length - 1);
            int startIndex = Mathf.FloorToInt(val);
            Color c = _colors[0];
            if (startIndex >= 0)
            {
                if (startIndex + 1 < _colors.Length)
                {
                    float factor = (val - startIndex);
                    c = Color.Lerp(_colors[startIndex], _colors[startIndex + 1], factor);
                }
                else if (startIndex < _colors.Length)
                {
                    c = _colors[startIndex];
                }
                else c = _colors[_colors.Length - 1];
            }
            _changeTex.color = c;
            if(OnColorChange!=null)
            {
                OnColorChange(c);
            }
        }
    }
}
