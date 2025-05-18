using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    public class NguiToggleAdapter : YxBaseToggleAdapter
    {
        public UISprite DownSprite;
        public UISprite UpSprite;

        public UILabel DownLabel;
        public UILabel UpLabel;
        [Tooltip("按下的效果前缀")]
        public string DownSuffix = "{0}_hover";
        [Tooltip("松开的效果前缀")]
        public string UpSuffix = "";
        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        private UIToggle _toggle;
        public UIToggle Toggle
        {
            get { return _toggle == null ? _toggle = GetComponent<UIToggle>() : _toggle; }
        }

        public override bool SetSkinName(string skinName)
        {
            if (DownSprite != null)
            {
                DownSprite.spriteName = string.IsNullOrEmpty(DownSuffix) ? skinName:string.Format(DownSuffix,skinName);
            }
            if (UpSprite != null)
            {
                UpSprite.spriteName = string.IsNullOrEmpty(UpSuffix) ? skinName : string.Format(UpSuffix, skinName);
            }
            return true;
        }

        public override void SetLabel(string content)
        {
            if (DownLabel != null)
            {
                DownLabel.text = content;
            }
            if (UpLabel != null)
            {
                UpLabel.text = content; ;
            }
        }

        public override bool StartsActive
        {
            get
            {
                var toggle = Toggle;
                return toggle !=null && toggle.startsActive;
            }
            set
            {
                var toggle = Toggle;
                if (toggle != null)
                {
                    toggle.startsActive = value;
                }
            }
        }
        public override bool Value
        {
            get
            {
                var toggle = Toggle;
                return toggle != null && toggle.value;
            }
            set
            {
                var toggle = Toggle;
                if (toggle != null)
                {
                    toggle.value = value;
                }
            }
        }
    }
}
