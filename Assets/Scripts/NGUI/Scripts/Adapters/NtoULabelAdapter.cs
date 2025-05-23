using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Adapters
{ 
    public class NtoULabelAdapter : YxBaseAdapter
    {
        public UIFont FontRes;

        [ContextMenu("FreshFont")]
        private void Start()
        {
            if (FontRes == null)
            {
                Destroy(this);
                return;
            }
            var ladapter = GetComponent<YxBaseLabelAdapter>();
            if (ladapter == null) return;
            ladapter.Font(FontRes.dynamicFont);
        }

        [ContextMenu("ClearFont")]
        public void ClearFont()
        {
            var ladapter = GetComponent<YxBaseLabelAdapter>();
            if (ladapter == null) return;
            ladapter.Font(null);
        }

        public override YxEUIType UIType
        {
            get { return YxEUIType.Default; }
        }
    }
}
