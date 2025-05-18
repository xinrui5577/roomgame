using UnityEngine;
using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    public class RuleItemView : YxView {
        public Vector4 RuleAnchor;

        protected override void OnStart()
        {
            var widget = GetComponent<UIWidget>();
            if (widget == null) return;
            widget.SetAnchor(transform.parent.gameObject, (int)RuleAnchor.x, (int)RuleAnchor.z, (int)RuleAnchor.y, (int)RuleAnchor.w);
            widget.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
        }

    }
}
