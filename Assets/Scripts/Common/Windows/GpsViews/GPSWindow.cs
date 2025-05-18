using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Common.Windows.GpsViews
{
    /// <summary>
    /// Gps窗口
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class GPSWindow : YxNguiWindow
    {
        /// <summary>
        /// 连线视图集合
        /// </summary>
        public YxView[] LinePViews;
        /// <summary>
        /// 更多版视图
        /// </summary>
        public YxView MorePView;
        /// <summary>
        /// 背景，当LinePViews数量等于0的时候，将会以玩家座位连接
        /// </summary>
        public GameObject BackGround;
        /// <summary>
        /// 包括自己最小人数，用来索引LinePViews的时候，玩家实际人数，减去该值，得到索引获取LinePViews里的位置信息
        /// </summary>
        public int MinPlayerCount = 3; 
        private YxView _curView;

        protected override void OnAwake()
        {
            var gdata = App.GameData;
            if (gdata == null)//空  隐藏所有
            {
                ShowLineView();
                if (MorePView != null) MorePView.gameObject.SetActive(false);
                if (BackGround != null) BackGround.SetActive(false);
                return;
            }
            var count = gdata.SeatTotalCount;
            var lineCount = LinePViews.Length; 
            if (count >= (lineCount + MinPlayerCount))//更多模式
            {
                if (MorePView != null) 
                {
                    ShowLineView();
                    MorePView.gameObject.SetActive(true);
                    _curView = MorePView;
                    return;
                }
            }
            if (lineCount < 1)//todo 全屏连线模式
            {
                if (BackGround!=null)BackGround.SetActive(false);
                return;
            }
            //连线模式
            var index = count - MinPlayerCount;
//            var gpsMgr = Facade.GetInterimManager<YxGPSManager>();
//            if (gpsMgr != null && gpsMgr.HasSelf)
//            {
//                index++;
//            }
            _curView = ShowLineView(index);
        }

        private YxView ShowLineView(int index = -1)
        {
            YxView view = null;
            var len = LinePViews.Length;
            for (var i = 0; i < len; i++)
            {
                var v = LinePViews[i];
                if(v == null)continue;
                var flag = i == index;
                v.gameObject.SetActive(flag);
                if (flag) view = v;
            }
            return view;
        }

        protected override void OnFreshView()
        {
            if (_curView == null) return;
            _curView.UpdateView();
        }
    }
}
