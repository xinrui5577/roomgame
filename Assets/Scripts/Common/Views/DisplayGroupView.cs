using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Views
{
    /// <summary>
    /// 要显示的内容显示内容
    /// </summary>
    public class DisplayGroupView : YxView
    {
        public bool NeedShow;
        /// <summary>
        /// 需要显示或者隐藏的对象
        /// </summary>
        public GameObject[] Displays;

        protected override void OnEnable()
        {
            YxWindowUtils.DisplayUI(Displays, NeedShow);
        }

        protected override void OnDisable()
        {
            YxWindowUtils.DisplayUI(Displays, !NeedShow);
        }
    }
}
