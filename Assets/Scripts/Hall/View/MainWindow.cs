using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Windows.TabPages;
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Tea;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager; 
using YxFramwork.View;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Hall.View
{
    public class MainWindow:YxBaseWindow
    {
        /// <summary>
        /// 玩家名称Label
        /// </summary>
        public UILabel UserName;
        /// <summary>
        /// 用户金币
        /// </summary>
        public UILabel UserGold;
        [Tooltip("玩家金币")]
        public NguiLabelAdapter UserGoldAdapter;
        /// <summary>
        /// id
        /// </summary>
        public UILabel IdLabel;
        /// <summary>
        /// 用户元宝
        /// </summary>
        public UILabel UserCash;
        /// <summary>
        /// 房卡
        /// </summary>
        public UILabel RoomCard;
        /// <summary>
        /// 头像
        /// </summary>
        public UITexture Portrait;
        /// <summary>
        /// 客服面板  todo 后期修改
        /// </summary>
        public GameObject PanelCustom;
        /// <summary>
        /// 客服qq     todo 后期修改
        /// </summary>
        public UILabel CustomLabel;
        /// <summary>
        /// 上排菜单
        /// </summary>
        public UIGrid TopMenuGrid;
        /// <summary>
        /// 下排菜单
        /// </summary>
        public UIGrid BottomMenuGrid;
        /// <summary>
        /// 自动打开的窗口
        /// </summary>
        public string[] AutoPopWindowNames;
        /// <summary>
        /// 菜单按钮（受开关管理）
        /// </summary>
        public GameObject[] MenuBtns;
        [Tooltip("背景")]
        public HallBackground HallBg;
        [Tooltip("预览游戏列表")]
        public YxListView PreviewListView;
        /// <summary>
        /// 游戏分组
        /// </summary>
        public GameGroupData GameGroupInfo;
        [Tooltip("游戏列表需要隐藏的内容")]
        public GameObject[] GameListHideUI;
        [Tooltip("版本信息")]
        public UILabel VerLabel;
        [Tooltip("存储对应玩家茶馆Id的Key")]
        public string KeyTeaId="TeaId";
        [Tooltip("显示时需要进行的处理")]
        public List<EventDelegate> OnShowActions;

        protected override void OnAwake()
        {
            var count = MenuBtns.Length;
            for (var i = 0; i < count;i++ )
            {
                var btn = MenuBtns[i];
                if (btn == null) continue;
                btn.SetActive(false);
            } 
            YxMsgCenterHandler.GetIntance().AddListener("HallWindow_hallMenuChange", OnFreshMenu); 
        }

        /// <summary>
        /// 退出/切换账号
        /// </summary>
        public void OnQuitBtn()
        {
            YxMessageBox.Show(null, "DefQuitWindow", "您确定要退出吗？", "", (mesBox, btnName) =>
            {
                switch (btnName)
                {
                    case YxMessageBox.BtnLeft:
                        Application.Quit();
                        break;
                    case YxMessageBox.BtnMiddle:
                        HallMainController.Instance.ChangeAccount();
                        break;
                }
            }, true, YxMessageBox.RightBtnStyle |YxMessageBox.MiddleBtnStyle| YxMessageBox.LeftBtnStyle);
        }

        protected virtual void OnFreshMenu(object msg)
        { 
            FreshMenu();
            ShowAutoPopWindows();
        }

        private static bool _hasPop;
        /// <summary>
        /// 自动打开窗口
        /// </summary>
        private void ShowAutoPopWindows()
        {
            if (_hasPop) return;
            _hasPop = true;
            var autoState = App.AppStyle == EAppStyle.Concise ? 0 : HallModel.Instance.OptionSwitch.AutoPopWin;
            var len = AutoPopWindowNames.Length;
            for (var i = 0; i < len; i++)
            {
                var show = 1 << i;
                if ((autoState & show) != show) continue;
                
                var winName = AutoPopWindowNames[i];
                YxWindowManager.OpenWindow(winName);
            }
        }

        private void FreshMenu()
        {
            var menustate = App.AppStyle == EAppStyle.Concise ? 1 : HallModel.Instance.OptionSwitch.HallMenue;
            var count = MenuBtns.Length;
            for (var i = 0; i < count; i++)
            {
                var btn = MenuBtns[i];
                if (btn == null) continue;
                var show = 1 << i;
                var isShow = (menustate & show) == show;
                btn.SetActive(isShow);
            }
            if (TopMenuGrid != null)
            {
                TopMenuGrid.repositionNow = true;
                TopMenuGrid.Reposition();
            }
            if (BottomMenuGrid != null)
            {
                BottomMenuGrid.repositionNow = true;
                BottomMenuGrid.Reposition();
            }
        }

        protected override void OnShow(object o)
        {
            base.OnShow(o);
            CurtainManager.CloseCurtain();
            UserController.Instance.GetUserDateWithBackPack(OnBindBackDate);
            if (HallBg != null) HallBg.Change(HallMainController.Instance.State);
            ShowPreviewListView();
            StartCoroutine(YxTools.WaitExcuteCalls(OnShowActions));
        }

        private void InitGameGroup()
        {
            var btns = GameGroupInfo.GameGroupBtns;
            var groups = GameListModel.Instance.Groups;
            var count = Mathf.Min(groups.Count, btns.Length);
            for (var i = 0; i < count; i++)
            {
                var btn = btns[i];
                if(btn==null)continue;
                var group = groups[i];
                GameGroupInfo.SetBtnState(btn,group.Key);
            }
        }

        /// <summary>
        /// 刷新玩家数据
        /// </summary>
        public void OnFreshUserData()
        {
            UserController.Instance.GetUserDate();
        }

        private void ShowPreviewListView()
        {
            if (PreviewListView == null) return;
            var gm = GameListModel.Instance;
            var gGroup = gm.GetGroup(gm.CurGroup);
            if (gGroup == null) return;
            PreviewListView.UpdateView(gGroup.GetGameList());
        }

        private void OnBindBackDate(object msg)
        {
            if (RoomCard != null) RoomCard.text = UserInfoModel.Instance.BackPack.GetItem("item2_q").ToString();
        }

        private YxWindow _spreadWin;
        protected override void OnBindDate(bool isChange = false)
        {
            if (!isChange) return;
            var userInfo = UserInfoModel.Instance.UserInfo;
            if (UserName != null)
            {
                UserName.text = userInfo.NickM;
            }
            if (IdLabel != null)
            {
                IdLabel.text = App.UserId;
            }
            if (UserGold != null)
            {
                UserGold.text = userInfo.CoinA.ToString();
            }
            if (UserGoldAdapter)
            {
                UserGoldAdapter.Text(userInfo.CoinA);
            }
            if (UserCash != null)
            {
                UserCash.text = userInfo.CashA.ToString();
            }
            if (CustomLabel != null)
            {
                CustomLabel.text = LoginInfo.Instance.G_MobileHallServerText;
            }
            if (Portrait != null)
            {
                PortraitRes.SetPortrait(userInfo.AvatarX, Portrait, userInfo.SexI);
            }
            OnBindBackDate(null);
            RefreshTopMenu();
            ShowVersion(VerLabel);
        }
         
        public override WindowName WindowName
        {
            get { return WindowName.Main; }
        }

        protected override IBaseModel YxBaseModel
        {
            get { return UserInfoModel.Instance; }
        }
         
        /// <summary>
        /// 打开设置窗口 todo 删除
        /// </summary>
        public void OnSettingInfo()
        {
            YxWindowManager.OpenWindow("SettingWindow");
        }

        /// <summary>
        /// 打开客服
        /// </summary>
        public void OnCustomServiceInfo()
        {
            var service = LoginInfo.Instance.G_MobileHallServerText;
            if (!string.IsNullOrEmpty(service))
            {
                if (GlobalUtile.IsUrl(service))
                {
                    Application.OpenURL(service);
                    return;
                }
                PanelCustom.SetActive(!PanelCustom.activeSelf);
                return;
            }
            YxWindowManager.OpenWindow("CustomerServiceWindow");
        }

        /// <summary>
        /// 打开公告 todo 删除
        /// </summary>
        public void OnNotice()
        {
            YxWindowManager.OpenWindow("DefNoticeWindow");
        }

        /// <summary>
        /// 打开任务
        /// </summary>
        public void OnTaskInfo()
        {
            var win = YxWindowManager.OpenWindow("TaskWindow") as YxTabPageWindow;
            if (win == null) return;
            win.UpdateView(HallModel.Instance.OptionSwitch.Task);
        }

        /// <summary>
        /// 打开银行 todo 删除
        /// </summary>
        public void OnBankInfo()
        {
            YxWindowManager.OpenWindow("BankWindow");
        }

        /// <summary>
        /// 打开商店 todo 删除
        /// </summary>
        public void OnShopInfo()
        {
            YxWindowManager.OpenWindow("ShopWindow");
        }

        /// <summary>
        /// 打开排行榜 todo 删除
        /// </summary>
        public void OnRankInfo()
        {
            YxWindowManager.OpenWindow("RankWindow");
        }

        /// <summary>
        /// 打开玩家信息
        /// </summary>
        public void OnUserInfo()
        {
            if (App.AppStyle == EAppStyle.Concise) return;
            YxWindowManager.OpenWindow("UserInfoWindow");
        }
         
        /// <summary>
        /// 喇叭消息
        /// </summary>
        public void OnTrumpetWindow()
        {
            if (MenuBtns.Length < 7) return;
            var btn = MenuBtns[6];
            if (btn != null && !btn.gameObject.activeSelf) return;
            YxWindowManager.OpenWindow("TrumpetWindow");
        }

        public void OnGameGroupClick(string group)
        {
            int index;
            if (!int.TryParse(group, out index)) return;
            GameListModel.Instance.CurGroup = index;
            GameListController.Instance.ShowGameList(index);
        }

        /// <summary>
        /// 打开茶馆密码盘
        /// </summary>
        private string TeaId;
        /// <summary>
        /// 用于存储茶馆ID的key
        /// </summary>
        private string SaveTeaId
        {
            get { return string.Format("{0}_{1}_{2}", Application.bundleIdentifier, App.UserId, KeyTeaId); }
        }

        public void OpenFindTeaWindow()
        {
            if(PlayerPrefs.HasKey(SaveTeaId))
            {
                TeaId = PlayerPrefs.GetString(SaveTeaId);
                if (TeaId.Length == 6)
                {
                    int roomType;
                    if (!int.TryParse(TeaId, out roomType)) return;
                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic["id"] = TeaId;
                    Facade.Instance<TwManger>().SendAction("group.teaGetIn", dic, GetInTea);
                }
            }
            else
            {
                YxWindowManager.OpenWindow("TeaFindRoom", true);
            }

        }

        private void GetInTea(object msg)
        {
            Dictionary<string, object> dic = (Dictionary<string, object>)msg;
            long value = (long)dic["mstatus"];

            if (value != 4)
            {
                string tea_name = (string)dic["name"];
                YxWindow obj = YxWindowManager.OpenWindow("TeaPanel", true);
                TeaPanel panel = obj.GetComponent<TeaPanel>();
                panel.CasePower((int)value);
                if (dic.ContainsKey("roomNum"))
                {
                    panel.roomNum = Convert.ToInt32(dic["roomNum"]);
                }
                panel.SetTeaName(tea_name);
                panel.SetTeaCode(int.Parse(TeaId));
                panel.TeaState = (int)value;
                Close();
            }
            else
            {
                YxWindowManager.OpenWindow("TeaFindRoom", true);
            }

        }
        /// <summary>
        /// 临时处理各个界面的各种信息
        /// </summary>
        private void RefreshTopMenu()
        {
            TopMenu menu = FindObjectOfType<TopMenu>();
            if (menu)
            {
                menu.SendMessage("OnUserDataChange");
            }
        }

        /// <summary>
        /// 设置显示版本号信息
        /// </summary>
        /// <param name="label"></param>
        public void ShowVersion(UILabel label)
        {
            YxTools.TrySetComponentValue(label,Application.version);
        }
		        /// <summary>
        /// 点击代理按钮，跳转到网页
        /// </summary>
        public void OnProxyClick()
        {
            string ctoken = LoginInfo.Instance.ctoken;
            string userid = LoginInfo.Instance.user_id;
            string url = App.Config.GetUrlWithServer("index.php/mobile/download/proxy", string.Format("&token={0}&userid={1}", ctoken, userid));
            Application.OpenURL(url);
        }
    }



    [Serializable]  
    public class GameGroupData
    {
        public string BtnNormal;
        public string BtnHover;
        public string BtnPressed;
        public string BtnDisabled;
        public UIButton[] GameGroupBtns;

        public void SetBtnState(UIButton btn, string prefix)
        {
            if (string.IsNullOrEmpty(BtnNormal)) btn.normalSprite = string.Format("{0}{1}", prefix, BtnNormal);
            if (string.IsNullOrEmpty(BtnHover)) btn.hoverSprite = string.Format("{0}{1}", prefix, BtnHover);
            if (string.IsNullOrEmpty(BtnPressed)) btn.pressedSprite = string.Format("{0}{1}", prefix, BtnPressed);
            if (string.IsNullOrEmpty(BtnDisabled)) btn.disabledSprite = string.Format("{0}{1}", prefix, BtnDisabled);
        }
    }
}
