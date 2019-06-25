using System.Collections.Generic;
using Assets.Scripts.Common;
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

namespace Assets.Scripts.Hall.View
{
    class HallWindow : YxBaseMoreWindow
    {
        public UITexture TitleLogo;
        public UITexture MiddleLogo;
        protected override void OnStart()
        {
            UpAnchor();
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

        public override WindowName WindowName
        {
            get { return WindowName.GameHall; }
        }

        protected override IBaseModel YxBaseModel
        {
            get { return HallModel.Instance; }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            var actions = new[] { "gameLogo", "optionSwitch" };
            TwManger.SendActions(actions, new Dictionary<string, object>(), OnUpdateLogo, false);//,null,true,"gameLogo&optionsw");
        }

        private static void OnUpdateLogo(object msg)
        {
            HallModel.Instance.Convert(msg);
            HallModel.Instance.Save();
            YxMsgCenterHandler.GetIntance().FireEvent("HallWindow_hallMenuChange");
        }

        protected override void OnBindDate(bool isChange = false)
        {
            base.OnBindDate(isChange);
            var model = HallModel.Instance;
            AsyncImage.Instance.GetAsyncImage(model.HallLogoTitle, TitleLogoCallBack);
            AsyncImage.Instance.GetAsyncImage(model.HallLogoMiddle, MiddleLogoCallBack);
        }

        private void TitleLogoCallBack(Texture2D texture)
        {
            if (texture == null) return;
            if (TitleLogo == null) return;
            TitleLogo.mainTexture = texture;
        }
        private void MiddleLogoCallBack(Texture2D texture)
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

        public void OnOpenPayWindow()
        {
            var win = YxWindowManager.OpenWindow("DefShopWindow");
        }

        public void OnOpenCreateAllRoomWindow()
        {
            var win = YxWindowManager.OpenWindow("DefCreateRoomWindow", true);
            var createWin = (CreateRoomWindow)win;
            if (createWin == null) return;
            createWin.GameKey = "";
        }

        

        /// <summary>
        /// 
        /// </summary>
        public void OnLogoClick()
        {
            Facade.Instance<TwManger>().SendAction("logoAdvise", new Dictionary<string, object>(), msg =>
                {
                    if (msg == null) return;
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
                    var win = YxWindowManager.OpenWindow("DefSupportWindow");
                    if (win != null) win.UpdateView(dict);
                });
        }

  

#if UNITY_EDITOR || YX_DEVE
        /// <summary>
        /// 点击游戏列表，打开游戏
        /// </summary>
        public void OnOpenGame(string gameKey)
        {
            CurGameKey = gameKey;
        }
        public static string CurGameKey;
        private string _editor_roomId;
        protected void OnGUI()
        {
            if (string.IsNullOrEmpty(CurGameKey)) return;
            GlobalUtile.ResizeGUIMatrix();
            var sh = Screen.height;
            var fontStyle = new GUIStyle
            {
                normal =
                {
                    //                            background = null,
                    textColor = Color.white
                }
                //                    fontSize = rowH
            };
            const int gx = 30;
            GUILayout.BeginHorizontal();
            GUI.Label(new Rect(gx, 10, 120, 20), "请输入房间id", fontStyle);
            _editor_roomId = GUI.TextField(new Rect(gx + 130, 10, 100, 20), _editor_roomId);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUI.Button(new Rect(gx, 95, 40, 20), "确 定"))
            {
                int rId;
                int.TryParse(_editor_roomId, out rId);
                if (rId > 0)
                {
                    var roomUm = RoomListModel.Instance.RoomUnitModel;
                    for (var i = 0; i <= rId; i++)
                    {
                        var rm = new RoomUnitModel(null)
                            {
                                TypeId = _editor_roomId,
                                GameKey = CurGameKey
                            };
                        roomUm.Add(rm);
                    }
                }
                RoomListController.Instance.QuickGame(CurGameKey);
                CurGameKey = "";
            }
            if (GUI.Button(new Rect(gx + 90, 95, 40, 20), "取 消"))
            {
                CurGameKey = "";
            }
            GUILayout.EndHorizontal();
        }
#endif
    }
}
