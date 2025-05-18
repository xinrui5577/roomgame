using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Constants;
using Assets.Scripts.Common.Managers;
using Assets.Scripts.Common.Windows.TabPages;
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
using UnityEngine.SceneManagement;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Hall.View
{
    public class MainWindow:YxBaseWindow
    { 
        /// <summary>
        /// 客服面板  todo 后期修改
        /// </summary>
        public GameObject PanelCustom;
        /// <summary>
        /// 客服qq     todo 后期修改
        /// </summary>
        public UILabel CustomLabel;
        /// <summary>
        /// 游戏列表初始位置
        /// </summary>
        public int GameListStartIndex = 0;
        /// <summary>
        /// 自动打开的窗口
        /// </summary>
        public string[] AutoPopWindowNames;
        [Tooltip("预览游戏列表,已过时")]
        public YxListView PreviewListView;
        [Tooltip("预览游戏列表")]
        public YxFramwork.View.YxListViews.YxListView PreviewGameListView;
        /// <summary>
        /// 游戏分组
        /// </summary>
        public GameGroupData GameGroupInfo;
        [Tooltip("游戏列表需要隐藏的内容")]
        // ReSharper disable once InconsistentNaming
        public GameObject[] GameListHideUI;
        [Tooltip("显示时需要进行的处理")]
        public List<EventDelegate> OnShowActions;
        [Tooltip("查找茶馆房间窗口名称")]
        public string FindTeaWindowName = "TeaFindRoom";
        [Tooltip("禁止进入茶馆状态")]
        public int[] ForbidJoinStatus = {4};

        protected override void OnAwake()
        { 
            AddListeners("HallWindow_hallMenuChange", OnFreshMenu); 
        }

        protected override void OnStart()
        {
            base.OnStart();
            var hallCtrl = HallMainController.Instance;
            hallCtrl.ExecuteCallBack();
        }

        /// <summary>
        /// todo 自行扩展
        /// 大厅启动时要执行的操作
        /// 需要使用  HallMainController 中的  AddLaunchInStartEvent、RemoveLaunchInStartEvent
        /// </summary>
        /// <param name="key"></param>
        private void LaunchEvent(string key)
        {
            switch (key)
            {
                case HallStartEventType.TeaWindow:
                    OpenFindTeaWindow();
                    break;
            }
        }

        /// <summary>
        /// 退出/切换账号
        /// </summary>
        public void OnQuitBtn()
        {
            YxMessageBox.Show(null, "QuitWindow", "您确定要退出吗？", "", (mesBox, btnName) =>
            {
                switch (btnName)
                {
                    case YxMessageBox.BtnLeft:
                        App.QuitGame();
                        break;
                    case YxMessageBox.BtnMiddle:
                        HallMainController.Instance.ChangeAccount();
                        break;
                }
            }, true, YxMessageBox.RightBtnStyle |YxMessageBox.MiddleBtnStyle| YxMessageBox.LeftBtnStyle);
        }

        protected virtual void OnFreshMenu(object msg)
        { 
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
            var autoState = App.AppStyle == YxEAppStyle.Concise ? 0 : HallModel.Instance.OptionSwitch.AutoPopWin;
            var len = AutoPopWindowNames.Length;
            for (var i = 0; i < len; i++)
            {
                var show = 1 << i;
                if ((autoState & show) != show) continue;
                
                var winName = AutoPopWindowNames[i];
                if (CheckWindowNeedOpen(winName))
                {
                    YxWindowManager.OpenWindow(winName,true,null,null, "SpreadWindow".Equals(winName));
                }
            }
        }
        /// <summary>
        /// 检查是否打开窗口, todo 临时处理
        /// </summary>
        /// <param name="winName"></param>
        /// <returns></returns>
        private bool CheckWindowNeedOpen(string winName)
        {
            if (string.IsNullOrEmpty(winName)) { return false;}
            var key = string.Format("AutoWindow_{0}_{1}", winName, App.UserId);
            switch (winName)
            {
                case "ActionNoticeQueueWindow":
                    var curDate = DateTime.Now.ToString("MM/dd/yyyy");
                    if (curDate == Util.GetString(key))
                    {
                        return false;
                    }
                    Util.SetString(key, curDate);
                    return true;
                case "SpreadWindow":
                    var promoter = UserInfoModel.Instance.UserInfo.Promoter;
                    if (promoter != false)
                    {
                        var times = Util.GetInt(key);
                        if (times > 3)
                        {
                            return false;
                        }
                        Util.SetInt(key, ++times);
                    }
                    return true;
                default:
                    return true;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            CurtainManager.CloseCurtain();
            UserController.Instance.GetUserDateWithBackPack(OnBindBackDate);
            ShowPreviewListView();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnShowActions.WaitExcuteCalls());
            }
            HallMainController.Instance.LaunchInStartEvent(LaunchEvent);
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
            YxView view = PreviewGameListView;
            var gm = GameListModel.Instance;
            var gGroup = gm.GetGroup(0);

            if (PreviewGameListView != null)
            {
                var arr = PreviewGameListView.SpecialDataFormat;
                var arrCount = arr.Length;
                var groups = gm.Groups;
                var groupCount = groups.Count;
                if (groupCount > 1)//多个组
                {
                    var gmSgroup = gm.GetSpecialGroup();
                    var gmGroupCount = gmSgroup.Length;
                    if (arrCount > 0)//是不是支持group
                    {
                        if (gmGroupCount < 1)
                        {
                            PreviewGameListView.SpecialDataFormat = new int[0];
                            PreviewGameListView.StartDataIndex = 0;
                        }
                        else
                        {
                            if (GameListStartIndex > 0)
                            {
                                PreviewGameListView.StartDataIndex = GameListStartIndex - arrCount + gmGroupCount;
                            }
                            PreviewGameListView.SpecialDataFormat = gmSgroup;
                        }
                    }
                    else//只显示一个组
                    {
                        PreviewGameListView.StartDataIndex = GameListStartIndex;
                        PreviewGameListView.SpecialDataFormat = new int[0];
                    }
                }
                else//一个组
                {
                    PreviewGameListView.StartDataIndex = GameListStartIndex;
                    if (arrCount > 0)
                    {
                        PreviewGameListView.SpecialDataFormat = new int[0];
                    }
                }
            }
            else if (PreviewListView != null)
            {
                view = PreviewListView;
            }
            else
            {
                return;
            }
            IList list = null;
            if (gGroup != null)
            {
                list = gGroup.GameListModels;
            }
            view.UpdateView(list);
        }

        private void OnBindBackDate(object msg)
        { 
        }

        protected override void OnBindDate(bool isChange = false)
        {
            if (!isChange) return;
            if (CustomLabel != null)
            {
                CustomLabel.text = LoginInfo.Instance.G_MobileHallServerText;
            }
            OnBindBackDate(null);
        }
          
        public override YxEWindowName WindowName
        { 
            get { return YxEWindowName.Main; }
        }

        protected override IBaseModel YxBaseModel
        {
            get { return UserInfoModel.Instance; }
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
        /// 打开任务
        /// </summary>
        public void OnTaskInfo()
        {
            var win = YxWindowManager.OpenWindow("TaskWindow") as YxTabPageWindow;
            if (win == null) return;
            win.UpdateView(HallModel.Instance.OptionSwitch.Task);
        }
         
        public void OnGameGroupClick(string group)
        {
            int index;
            if (!int.TryParse(group, out index)) return;
            GameListController.Instance.ShowGameList(index);
        }

        public void OnChangeGroup(UIToggle toggle)
        {
            if (!toggle.value) { return;}
            OnGameGroupClick(toggle.name);
        }

        /// <summary>
        /// 打开茶馆密码盘
        /// </summary>
        private string TeaId;

        public void OpenFindTeaWindow()
        {
            if (Util.HasKey(TeaUtil.KeySaveTeaKey))
            {
                TeaId = Util.GetString(TeaUtil.KeySaveTeaKey);
                // ReSharper disable once InvertIf
                if (TeaId.Length == 6)
                {
                    int roomType;
                    if (!int.TryParse(TeaId, out roomType)) return;
                    var dic = new Dictionary<string, object>();
                    dic["id"] = TeaId;  
                    Facade.Instance<TwManager>().SendAction("group.teaGetIn", dic, GetInTea);
                }
            }
            else
            {
                YxWindowManager.OpenWindow(FindTeaWindowName);
            }
        }
         
        /// <summary>
        /// todo 优化
        /// </summary>
        /// <param name="msg"></param>
        private void GetInTea(object msg)
        { 
            var dic = (Dictionary<string, object>)msg;
            var value = int.Parse(dic["mstatus"].ToString());
            if (JoinTeaCheck(value))
            {
                if (SceneManager.GetActiveScene().name.ToLower() == App.Skin.Hall)
                {
                    var win = YxWindowManager.OpenWindow("TeaPanel");
                    if (win != null)
                    {
                        var panel = win.GetComponent<TeaPanel>();
                        panel.UpdateView(dic);
                        panel.SetTeaCode(int.Parse(TeaId)); 
                        Close();    
                    }
                }
            }
            else
            {
                YxWindowManager.OpenWindow(FindTeaWindowName);
            } 
        }

        private bool JoinTeaCheck(int statusValue)
        {
            return !ForbidJoinStatus.Contains(statusValue);
        }

        /// <summary>
        /// 点击代理按钮，跳转到网页
        /// </summary>
        public void OnProxyClick()
        {  
            OnOpenUrlWithServer("index.php/mobile/download/proxy");
        }

        /// <summary>
        /// 创建茶馆
        /// </summary>
        public void OnOpenCreatTea()
        {
            Facade.Instance<TwManager>().SendAction("group.inquireTeaHouse", new Dictionary<string, object>(), data =>
            {
                if (data == null)
                {
                    YxWindowManager.OpenWindow("TeaCreateWindow");
                }
            });
        }

        public void OnQuicklyGame(string gameKey)
        {
            GameListController.Instance.QuickGame(gameKey);
        }

        public void JoinMatch()
        {
            RoomListController.Instance.OnJoinMatch();
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
