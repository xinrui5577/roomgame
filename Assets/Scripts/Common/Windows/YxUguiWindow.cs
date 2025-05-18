using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Windows
{
    [RequireComponent(typeof(UguiPanelAdapter))]
    public class YxUguiWindow : YxWindow
    {
        public override YxEUIType UIType
        {
            get { return YxEUIType.Ugui; }
        }
    }
}
