using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Windows
{
    [RequireComponent(typeof(NguiPanelAdapter))]
    public class YxNguiWindow : YxWindow
    { 
/*//        public override void SetOrder(int order)
//        {
//            var panels = transform.GetComponents<UIPanel>();
//            foreach (var panel in panels)
//            {
//                panel.sortingOrder = order;
//                panel.depth = order;
//            }
//        }*/
    }
}
