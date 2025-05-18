using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Common.components;
using Assets.Scripts.Hall.View.AboutRoomWindows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using com.yxixia.utile.Utiles;
using YxFramwork.Enums;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View
{
    class HallWindow : YxBaseMoreWindow
    {
        /// <summary>
        /// 标题logo
        /// </summary>
        [Tooltip("标题logo")]
        public UITexture TitleLogo;
        /// <summary>
        /// 大Logo
        /// </summary>
        [Tooltip("标题logo")]
        public UITexture MiddleLogo;
        private bool _needFreshUserInfo;
         
        protected override void OnStart()
        {
            UpAnchor();
            InitBackPart();
            Facade.EventCenter.AddEventListeners<YxESysEventType, object>(YxESysEventType.SysFreshUserInfo, obj => { _needFreshUserInfo = true; });
        }

        private void InitBackPart()
        {
            gameObject.AddComponent<QuitPart>();
        }

        [ContextMenu("UpAnchor")]
        public void UpAnchor()
        {
            var widget = GetComponent<UIWidget>();
            if (widget == null) return;
            var p = transform.parent;
            if (p == null) return;
            widget.SetAnchor(p.gameObject, 0, 0, 0, 0);
            widget.updateAnchors = UIRect.AnchorUpdate.OnUpdate;
        }

        public override YxEWindowName WindowName
        {
            get { return YxEWindowName.GameHall; }
        }

        protected override IBaseModel YxBaseModel
        {
            get { return HallModel.Instance; }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            var actions = new[] { "gameLogo", "optionSwitch" };
            CurTwManager.SendActions(actions, new Dictionary<string, object>(), OnUpdateLogo, false);//,null,true,"gameLogo&optionsw");
        }

        private static void OnUpdateLogo(object msg)
        {
            HallModel.Instance.Convert(msg);
            HallModel.Instance.Save();
            Facade.EventCenter.DispatchEvent<string, object>("HallWindow_hallMenuChange");
        }

        protected override void OnBindDate(bool isChange = false)
        {
            base.OnBindDate(isChange);
            var model = HallModel.Instance;
            AsyncImage.Instance.GetAsyncImage(model.HallLogoTitle, TitleLogoCallBack);
            AsyncImage.Instance.GetAsyncImage(model.HallLogoMiddle, MiddleLogoCallBack);
        }

        private void TitleLogoCallBack(Texture2D texture, int hashCode)
        {
            if (texture == null) return;
            if (TitleLogo == null) return;
            TitleLogo.mainTexture = texture;
        }

        /// <summary>
        /// 中间logo 
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="hashCode"></param>
        private void MiddleLogoCallBack(Texture2D texture, int hashCode)
        {
            if (texture == null) return;
            if (MiddleLogo == null) return;
            MiddleLogo.mainTexture = texture;
        }

        /// <summary>
        /// 充值
        /// </summary>
        public void OnOpenPay()
        {
            var cfg = App.Config as SysConfig;
            if (cfg == null) return;
            var info = LoginInfo.Instance;
            Application.OpenURL(cfg.GetRecharge(info.user_id, info.token));
        }

        /// <summary>
        /// 打开所有创建房间界面
        /// </summary>
        public void OnOpenCreateAllRoomWindow()
        {
            var win = YxWindowManager.OpenWindow("CreateRoomWindow");
            var createWin = (CreateRoomWindow)win;
            if (createWin == null) return;
            createWin.GameKey = "";
        }

        /// <summary>
        /// 快速游戏
        /// </summary>
        /// <param name="gameKey"></param>
        public void OnOpenGameClick(string gameKey)
        {
            if (string.IsNullOrEmpty(gameKey)) { return; }
            GameListController.Instance.QuickGame(gameKey);
        }

        /// <summary>
        /// todo //待优化   点击logo事件
        /// </summary>
        public void OnLogoClick()
        {
            Facade.Instance<TwManager>().SendAction("logoAdvise", new Dictionary<string, object>(), msg =>
                {
                    var dict = msg as Dictionary<string, object>;
                    if (dict == null) return;
                    if (dict.ContainsKey("url"))
                    {
                        var info = dict["url"];
                        if (info != null)
                        {
                            var strInfo = info.ToString();
                            Application.OpenURL(strInfo);
                            return;
                        }
                    }
                    var win = YxWindowManager.OpenWindow("SupportWindow");
                    if (win != null) win.UpdateView(dict);
                });
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
        public void OnManagenCtrl()
        {
            Facade.Instance<TwManager>().SendAction("mahjongwm.manageCtrl", new Dictionary<string, object>(), FreshCtrl);
        }

        private void FreshCtrl(object data)
        {
            OnOpenWindow("WebViewWindow");
        }


    }
}
