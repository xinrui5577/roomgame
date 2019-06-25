using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 广播
    /// </summary>
    [RequireComponent(typeof(UIPanel))]
    public class NguiNoticeMessage : YxNoticeMessage
    {
        private UIPanel _uiPanel;

        public override void OnAwake()
        {
            _uiPanel = GetComponent<UIPanel>();
        }

        /// <summary>
        /// 获取范围
        /// </summary>
        /// <returns>返回一个范围，左，上，右，下</returns>
        protected override Vector4 GetShowBounds()
        {
            if(_uiPanel==null)return new Vector4();
            var bounds = _uiPanel.baseClipRegion;
            var temp = bounds.z / 2;
            var tempC = bounds.x;
            bounds.x -= temp;
            bounds.z = tempC + temp;
            /****************************/
            temp = bounds.w/2;
            tempC = bounds.y;
            bounds.y -= temp;
            bounds.w = tempC + temp;
            return bounds; 
        }

//        private void OnGUI()
//        {
//            if (GUI.Button(new Rect(10, 10, 100, 50), "添加消息"))
//            {
//                YxNoticeMessage.ShowNoticeMsg("hahahahahaha");
//            }
//            if (GUI.Button(new Rect(10, 100, 100, 50), "添加消息s"))
//            {
//                YxNoticeMessage.ShowLoopNoticeMsgs(new[] { "111111" ,"222222222222222222","333333333333333333333333333333333333333"});
//            }
//        }
    }
}
