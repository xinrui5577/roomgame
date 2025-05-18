using System.Collections;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Manager;
using YxFramwork.View.YxListViews;

namespace Assets.Scripts.Hall.View.HallWindows
{
    /// <inheritdoc />
    /// <summary>
    /// 游戏列表面板
    /// </summary>
    public class GameListWindow : YxBaseWindow
    { 
        /// <summary>
        /// 更多列表视图
        /// </summary>
        [Tooltip("所有游戏列表")]
        public YxListView ListView;
        /// <summary>
        /// MainWindow需要隐藏的内容
        /// </summary>
        [Tooltip("MainWindow需要隐藏的内容")]
        public GameObject[] HideUIs;
        /// <summary>
        /// 查找房间
        /// </summary>
        [Tooltip("查找房间")]
        public GameObject FindRoomBtn;

        protected override IBaseModel YxBaseModel
        {
            get { return GameListModel.Instance; }
        }

        public override YxEWindowName WindowName
        {
            get { return YxEWindowName.GameList; }
        }

        protected override void OnEnable()
        {
            YxWindowUtils.DisplayUI(HideUIs, false);
        }

        protected override void OnDisable()
        {
            YxWindowUtils.DisplayUI(HideUIs);
        }

        protected override void OnBindDate(bool isChange = false)
        {
            OnShowAllListItem();
            CurtainManager.CloseCurtain();
        }

        private int _curGroup = -1;
        private void OnShowAllListItem()
        {
            if (ListView == null) return; 
            var gm = GameListModel.Instance;
            var group = gm.CurGroup;
            if (group == _curGroup) return;
            _curGroup = group;
            var gGroup = gm.GetGroup(group);
            IList list = null;
            if (gGroup != null)
            {
                if (FindRoomBtn != null) FindRoomBtn.SetActive(gGroup.Type < 0);
                list = gGroup.GameListModels;
            }
            ListView.StartDataIndex = Util.GetInt("GameList_StartDataIndex");//GameListController.Instance.CurStartDataIndex;
            var dataFormat = GameListController.Instance.DataFormat;
            if (dataFormat != null && dataFormat.Length>0) ListView.DataFormat = dataFormat;
            ListView.UpdateView(list); 
        }

        protected override void OnClose()
        {
            HallMainController.Instance.ShowHallMain();
        }
         
        public override void OnDestroy()
        {
            base.OnDestroy();
//            GameListController.Instance.CurStartDataIndex = ListView.StartDataIndex;
            Util.SetInt("GameList_StartDataIndex",ListView.StartDataIndex);
        }
    } 
}
