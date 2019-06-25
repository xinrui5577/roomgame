using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Common.UI
{
    public class UguiNoticeMessage : YxNoticeMessage
    {
        public override void OnAwake()
        {
            base.OnAwake();
        }

        protected override Vector4 GetShowBounds()
        {
            throw new System.NotImplementedException();
        }
    }
}
