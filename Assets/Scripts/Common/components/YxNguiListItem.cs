using UnityEngine;
using com.yxixia.utile.Utiles;
using YxFramwork.View.YxListViews;

namespace Assets.Scripts.Common.Components
{
    public class YxNguiListItem : YxListItem {

//        public override void SetOrder(int order)
//        {
//            base.SetOrder(order);
//            var widgets = transform.GetComponentsInChildren<UIWidget>();
//            foreach (var uiWidget in widgets)
//            {
//                uiWidget.depth = Order;
//            }
//        }

        [ContextMenu("UpAnchor")]
        public void UpAnchor()
        {
            var widget = GetComponent<UIWidget>();
            if (widget == null) return;
            var p = GlobalUtile.GetParentWidget<UIWidget>(widget.transform);
            if (p == null) return;
            widget.SetAnchor(p.gameObject, 0, 0, 0, 0);
            widget.updateAnchors = UIRect.AnchorUpdate.OnEnable;
        }
    }
}
