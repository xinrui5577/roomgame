using Assets.Scripts.Game.FishGame.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.UI
{
    public class SystemUI : UIView
    {
        public TopUI TopView;
        public BottomUI BottomView;
        public CenterUI CenterView;

        private void Start()
        {
            UpdateCenter();
        }

        /// <summary>
        /// 隐藏顶部
        /// </summary>
        /// <param name="isShow"></param>
        public void DisplayTitle(bool isShow=true)
        {
            if (TopView == null) return;
            TopView.Display(isShow);
            UpdateCenter();
        }

        /// <summary>
        /// 隐藏底部
        /// </summary>
        /// <param name="isShow"></param>
        public void DisplayBottomView(bool isShow = true)
        {
            if (BottomView == null) return;
            BottomView.Display(isShow);
            UpdateCenter();
        }

        public void UpdateCenter()
        {
            var x = Bound.x; 
            var w = Bound.width;

            float y;
            if (BottomView != null && BottomView.IsActivite())
            {
                var tsLpos = BottomView.transform.localPosition;
                var bound = BottomView.Bound;
                var yforts = tsLpos.y + bound.y;
                y = Mathf.Max(yforts, yforts + bound.height);
            }
            else
            {
                y = Bound.y;
            }
            float h;
            if (TopView != null && TopView.IsActivite())
            {
                var tsLpos = TopView.transform.localPosition;
                var bound = TopView.Bound;
                var yforts = tsLpos.y + bound.y;
                h = Mathf.Min(yforts, yforts + bound.height) - y;
            }
            else
            {
                h = Bound.y + Bound.height - y;
            }
            CenterView.SetBound(x, y, w, h);
        }

        public void Init(ISFSObject gameInfo)
        {
            if (BottomView != null)
            {
                DisplayBottomView(App.GetGameData<FishGameData>().NeedUpperScore);
                BottomView.Init(gameInfo);
            }
        }

        protected override void OnDrawGizmos()
        { 
            UpdateCenter();
            base.OnDrawGizmos();
        }
    }
}
