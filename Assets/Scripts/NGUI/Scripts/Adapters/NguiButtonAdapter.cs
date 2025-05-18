using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    public class NguiButtonAdapter : YxBaseButtonAdapter
    {
        [Tooltip("按钮的文本")]
        public UILabel Label; 
        [Tooltip("按钮的文本")]
        private UIButton _button;

        private UIButton Button
        {
            get { return _button == null ? _button = GetComponent<UIButton>() : _button; }
        }

        protected override void InitSoundListen()
        {
            var trigger = GetComponent<UIEventTrigger>();
            if (trigger == null)
            {
                trigger = gameObject.AddComponent<UIEventTrigger>();
            }
            var listener = GetComponent<UIEventListener>();
            if (listener == null)
            {
                gameObject.AddComponent<UIEventListener>();
            }
            if (!string.IsNullOrEmpty(SoundPlayer.ClickSName))
            {
                trigger.onClick.Add(new EventDelegate(SoundPlayer.OnYxClick));
            }
            if (!string.IsNullOrEmpty(SoundPlayer.DoubleClickSName))
            {
                trigger.onDoubleClick.Add(new EventDelegate(SoundPlayer.OnYxDoubleClick));
            }
            if (!string.IsNullOrEmpty(SoundPlayer.PressSName))
            {
                trigger.onPress.Add(new EventDelegate(SoundPlayer.OnYxPress));
            }
            if (!string.IsNullOrEmpty(SoundPlayer.ReleaseSName))
            {
                trigger.onRelease.Add(new EventDelegate(SoundPlayer.OnYxRelease));
            }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
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
            var hover = string.Format(HoverSuffix, skinName);
            var press = string.Format(PressSuffix, skinName);
            var disable = string.Format(DisableSuffix, skinName);
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

        public override bool IsEnabled
        {
            get { return Button.isEnabled; }
            set { Button.isEnabled = value; }
        }
    }
}
