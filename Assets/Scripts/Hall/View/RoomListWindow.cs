using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 房间列表视图
    /// </summary>
    public class RoomListWindow : YxBaseWindow
    {
        [Tooltip("房间列表")]
        public YxListView ListView;
        [Tooltip("背景")]
        public HallBackground HallBg;
        [Tooltip("创建房间视图")]
        public GameObject CreateRoomView;
        [Tooltip("休闲视图")]
        public GameObject FreeRoomView;
        [Tooltip("指定gamekey")]
        public string AppointGameKey;
        [Tooltip("MainWindow需要隐藏的内容")]
        public GameObject[] HideUIs;
        [Tooltip("游戏名称label")]
        public UILabel GameNameLabel;
        private int _state;

        protected override void OnAwake()
        {
            base.OnAwake();
            ChangeState(); 
        }

        public override WindowName WindowName
        {
            get { return WindowName.RoomList;}
        }

        protected override IBaseModel YxBaseModel
        {
            get { return RoomListModel.Instance; }
        }
           
        protected override void OnShow(object o)
        { 
            base.OnShow(o);
            HallBg.Change(HallState.Roomlist);
            if (!string.IsNullOrEmpty(AppointGameKey))
            {
                App.GameKey = AppointGameKey;
                RoomListController.Instance.GetRoomList(AppointGameKey);
            }
            if (GameNameLabel == null) return;
            var gameModels = GameListModel.Instance.GameUnitModels;
            if (!gameModels.ContainsKey(App.GameKey)) return;
            var gameModel = gameModels[App.GameKey];
            if (gameModel != null)
            {
                GameNameLabel.text = gameModel.GameName;
            }
        }
         
        protected override void OnBindDate(bool isChange = false)
        {
            base.OnBindDate(isChange);
            var model = RoomListModel.Instance; 
            if (model.RoomKind > 0)
            {
                ShowCreateRoom();
            }
            else
            { 
                ShowFreeRoom();
            }
            CurtainManager.CloseCurtain();
        }

        /// <summary>
        /// 显示房间列表
        /// </summary>
        private void ShowFreeRoom()
        {
            ChangeState(1);
            var model = RoomListModel.Instance; 
            var list = model.RoomUnitModel;
            if (list == null) return;
            ListView.UpdateView(list);
        }

        /// <summary>
        /// 显示创建房间界面
        /// </summary>
        public void ShowCreateRoom()
        {
            ChangeState(2);
        }

        public void ChangeState(int state=0)
        {
            _state = state;
            if (CreateRoomView != null) CreateRoomView.SetActive(_state == 2);
            else _state = 1;
            if (FreeRoomView != null) FreeRoomView.gameObject.SetActive(_state == 1);
        }

        /// <summary>
        /// 点击free图标
        /// </summary>
        public void OnFreeRoomClick()
        { 
            ShowFreeRoom();
        }

        public void OnOpenCreateWindow(string index)
        {
            var win = YxWindowManager.OpenWindow("DefCreateRoomWindow",true);
            var createWin = (CreateRoomWindow)win;
            if (createWin == null) return;
            int dindex;
            int.TryParse(index,out dindex);
            createWin.TabDefaultIndex = dindex;
        }

        public void OnOpenCreateWindowWithKey()
        { 
            var win = YxWindowManager.OpenWindow("DefCreateRoomWindow");
            if (win == null) return;
            var cwin = win.GetComponent<CreateRoomWindow>();
            if (cwin == null) return;
            cwin.GameKey = App.GameKey;
        }

        public void OnFindRoomWindow(string index)
        {
            var win = YxWindowManager.OpenWindow("DefFindRoomWindow", true);
            var findWin = (FindRoomWindow)win;
            if (findWin == null) return;
            int dindex;
            int.TryParse(index, out dindex);
            findWin.TabDefaultIndex = dindex;
        }

        private void OnEnable()
        {
            YxWindowUtils.DisplayUI(HideUIs, false);
        }

        private void OnDisable()
        {
            YxWindowUtils.DisplayUI(HideUIs);
        }

        protected override void OnClose()
        {
            if (RoomListModel.Instance.RoomKind > 0 && _state == 1) ChangeState(2);
            else GameListController.Instance.ShowGameList(GameListModel.Instance.CurGroup);
        }

        /// <summary>
        /// todo 临时方法，请不要使用
        /// </summary>
        public void ReturnMain()
        {
            HallMainController.Instance.ShowHallMain();
        }
    }
}

