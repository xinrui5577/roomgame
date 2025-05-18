using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 登录总界面
    /// </summary>
    class LoadEntranceWindow:YxBaseMoreWindow
    {

        protected override void OnStart()
        { 
            UpAnchor();
        }

        [ContextMenu("UpAnchor")]
        public void UpAnchor()
        {
            var widget = GetComponent<UIWidget>();
            if (widget == null) return;
            var p = transform.parent;
            if (p == null) return;
            widget.SetAnchor(p.gameObject, 0, 0, 0, 0);
            widget.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
        }


        public override YxEWindowName WindowName
        {
            get { return YxEWindowName.LoadEntrance; }
        }
    }
}
