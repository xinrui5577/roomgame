using System.Collections.Generic;
using Assets.Scripts.Common.components;
using com.yxixia.utile.Utiles;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.Panels
{
    /// <summary>
    /// 大厅组合面板
    /// </summary>
    public class HallPanel : YxBaseGroupPanel
    {
        private bool _needFreshUserInfo;
        protected override void OnAwake()
        {
            base.OnAwake();
            InitBackPart();
            Facade.EventCenter.AddEventListeners<YxESysEventType, object>(YxESysEventType.SysFreshUserInfo, obj => { _needFreshUserInfo = true; });
        }

        private void InitBackPart()
        {
            gameObject.AddComponent<QuitPart>();
        }

        protected override void OnStart()
        {
            CurtainManager.CloseCurtain();
            base.OnStart();
            UserController.Instance.SendSimpleUserData();
            var actions = new[] { "gameLogo", "optionSwitch" };
            CurTwManager.SendActions(actions, new Dictionary<string, object>(), FreshHallModel, false);//,null,true,"gameLogo&optionsw");
        }

        /// <summary>
        /// 刷新开关
        /// </summary>
        /// <param name="data"></param>
        public void FreshHallModel(object data)
        {
            HallModel.Instance.Convert(data);
            HallModel.Instance.Save();
            Facade.EventCenter.DispatchEvent<string, object>("HallWindow_hallMenuChange");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isPause"></param>
        protected void OnApplicationFocus(bool isPause)
        {
            if (!isPause) return;
            CheckFreshUserInfo();
            CheckPasteBoard();
        }

        private void CheckFreshUserInfo()
        {
            if (!_needFreshUserInfo) return;
            _needFreshUserInfo = false;
            UserController.Instance.SendSimpleUserData();
        }

        private static string _lastPasteOrder;
        /// <summary>
        /// 检查剪切板
        /// </summary>
        private void CheckPasteBoard()
        {
            var pasteBoard = Facade.Instance<YxGameTools>().PasteBoard;
            if (string.IsNullOrEmpty(pasteBoard)) { return; }
            const string sign = "※";
            var order = StringHelper.FindBetweenSign(pasteBoard, sign, sign);
            if (order.Equals(_lastPasteOrder)) { return; }
            _lastPasteOrder = order;
            var infos = order.Split(':');
            if (infos.Length < 1) { return; }
            switch (infos[0])
            {
                case "find":
                    int roomType;
                    if (int.TryParse(infos[1], out roomType))
                    {
                        RoomListController.Instance.FindRoomAndOpenWindow(roomType);
                    }
                    break;
            }
            _lastPasteOrder = string.Empty;
        }

        /// <summary>
        /// 游戏分组点击
        /// </summary>
        /// <param name="group"></param>
        public void OnGameGroupClick(string group)
        {
            int index;
            if (!int.TryParse(group, out index)) return;
            var gamelistCtr = GameListController.Instance;
            gamelistCtr.DataFormat = null;
            GameListModel.Instance.CurGroup = index; 
            ShowChildPanel("GameListPanel");
        }
    }
}
