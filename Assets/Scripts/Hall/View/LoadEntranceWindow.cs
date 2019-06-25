using UnityEngine;
using YxFramwork.Framework; 

namespace Assets.Scripts.Hall.View
{
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


        public override WindowName WindowName
        {
            get { return WindowName.LoadEntrance; }
        }
    }
}
