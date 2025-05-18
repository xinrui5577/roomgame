using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;

namespace Assets.Scripts.Common.Adapters
{
    public class NguiToggleButtonAdapter : YxBaseButtonAdapter
    {
        private NguiToggleAdapter _toggle;
        protected NguiToggleAdapter ToggleAdapter
        {
            get
            {
                if (_toggle == null)
                {
                    _toggle = GetComponent<NguiToggleAdapter>();
                }
                return _toggle;
            }
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Nguid; }
        }

        public override bool SetSkinName(string skinName)
        {
            var toggle = ToggleAdapter;
            return toggle.SetSkinName(skinName);
        }

        public override void SetLabel(string content)
        {
            var toggle = ToggleAdapter;
            toggle.SetLabel(content);
        }

        public override bool IsEnabled
        {
            get
            {
                var c = GetComponent<Collider>();
                return c == null || c.enabled;
            }
            set
            {
                var c = GetComponent<Collider>();
                if(c != null) c.enabled = value;
            }
        }
    }
}
