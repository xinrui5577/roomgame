using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 游戏列表面板
    /// </summary>
    public class GameListPanel : YxBaseWindow
    { 
        /// <summary>
        /// 背景
        /// </summary>
        [Tooltip("背景")]
        public HallBackground HallBg; 
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

        public override WindowName WindowName
        {
            get { return WindowName.GameList; }
        }
          
        protected override void OnShow(object o)
        {
            base.OnShow(o);
            HallBg.Change(HallState.Gamelist);
        }

        private void OnEnable()
        {
            YxWindowUtils.DisplayUI(HideUIs, false);
        }

        private void OnDisable()
        {
            YxWindowUtils.DisplayUI(HideUIs);
        }

        protected override void OnBindDate(bool isChange = false)
        {
            OnShowAllListItem();
            CurtainManager.CloseCurtain();
        }
         
        private void OnShowAllListItem()
        {
            if (ListView == null) return;
            var gm = GameListModel.Instance;
            var gGroup = gm.GetGroup(gm.CurGroup);
            if (gGroup == null) return;
            if (FindRoomBtn != null) FindRoomBtn.SetActive(gGroup.Type < 0);
            //            ListView.SetItemDatas(new List<IList>());
            ListView.UpdateView(gGroup.GetGameList());
        }

        protected override void OnClose()
        {
            HallMainController.Instance.ShowHallMain();
        }
    } 
}
