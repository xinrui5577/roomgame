using System.Collections.Generic;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    public class NguiButtonAdapter : YxBaseButtonAdapter
    {
        public UILabel Label;
        private UIButton _button;

        public UIButton Button
        {
            get
            {
                if (_button == null)
                {
                    _button = GetComponent<UIButton>();
                }
                return _button;
            }
        }

        public override YxUIType UIType
        {
            get { return YxUIType.Nguid; }
        }

        public override bool SetSkinName(string skinName)
        {
            var btn = Button;
            if (btn == null) return false;
            var sp = btn.GetComponent<UISprite>();
            if (sp == null) return false;
            var atlas = sp.atlas;
            if (atlas == null) return false;
            var list = atlas.spriteList;
            var count = list.Count;
            var hover = string.Format("{0}_hover", skinName);
            var press = string.Format("{0}_press", skinName);
            var disable = string.Format("{0}_disable", skinName);
            var flag = 0;
            var dict = new Dictionary<string, bool>();
            for (var i = 0; i < count; i++)
            {
                var spData = list[i];
                var spName = spData.name;
                if (spName != skinName && spName != hover && spName != press && spName != disable) continue;
                dict[spName] = true;
                flag++;
                if (flag >= 4) break;
            }
            if (!dict.ContainsKey(skinName)) return false;
            btn.normalSprite = skinName;
            btn.hoverSprite = dict.ContainsKey(hover) ? hover : null;
            btn.pressedSprite = dict.ContainsKey(press) ? press : null;
            btn.disabledSprite = dict.ContainsKey(disable) ? disable : null;
            return true;
        }

        public override void SetLabel(string content)
        {
            if (Label == null) return;
            if (string.IsNullOrEmpty(content))
            {
                Label.gameObject.SetActive(false);
                return;
            }
            Label.text = content;
            Label.gameObject.SetActive(true);
        }
    }
}
