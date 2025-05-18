using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhChip : MonoBehaviour
    {

        public UILabel TextValue;

        public System.Collections.Generic.List<string> SprName;

        public System.Collections.Generic.List<Color> ValueColor;

        public int Value;
        public void SetChip(int index, int value)
        {
            if (index >= 0 && index <= SprName.Count - 1)
            {
                Value = value;
                UISprite spr = GetComponent<UISprite>();
                if (spr != null)
                {
                    spr.spriteName = SprName[index];
                    TextValue.text = YxUtiles.ReduceNumber(value);
                    TextValue.color = ValueColor[index];
                }
                UIButton[] btns = GetComponents<UIButton>();
                if (btns.Length > 1)
                {
                    btns[1].hover = ValueColor[index];
                    btns[1].pressed = ValueColor[index];
                    btns[1].hover = ValueColor[index];
                }
            }
        }
    }
}
