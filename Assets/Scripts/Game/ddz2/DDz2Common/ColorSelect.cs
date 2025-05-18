using System;
using UnityEngine;

namespace Assets.Scripts.Game.ddz2.DDz2Common
{
    /// <summary>
    /// 颜色slider，从NGUI中拿过来的用用
    /// </summary>
    public class ColorSelect : MonoBehaviour
    {
        [SerializeField]
        private UITexture _changeTex;
        [SerializeField]
        private Color[] _colors = new Color[] { Color.red, Color.yellow, Color.green };
        public static Action<Color,float> OnColorChange;

        [SerializeField] protected UISlider UiColorSlider;

        void Start()
        {
           UiColorSlider.value = PlayerPrefs.GetFloat(TableColorCtrl.OntableColorChangeKey, 0.5f);
           OnValueChange();
        }

        public void OnValueChange()
        {
            if (UIProgressBar.current == null)
            {
                return;
            }

            float val = UIProgressBar.current.value;

            OnValueChangeDo(val);


        }

        public Color OnValueChangeDo(float val)
        {
            if (_changeTex == null || _colors.Length == 0) return new Color();
            float v = val;
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
            if (OnColorChange != null)
            {
                OnColorChange(c, v);
            }

            return c;
        }
    }
}

