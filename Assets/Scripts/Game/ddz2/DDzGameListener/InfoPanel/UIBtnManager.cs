using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.InheritCommon;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class UIBtnManager : MonoBehaviour
    {
        public UIButton OutBtn;

        public UIButton ChangeRoomBtn;

        public UIButton SettingBtn;

        public UIButton AutoPlayBtn;

        public UIGrid TopRightGrid;
        /// <summary>
        /// 更换房间通用名字,无需加状态扩展名
        /// </summary>
        public string OutBtnSpriteName;

        /// <summary>
        /// 解散房间通用名字,无需加状态扩展名
        /// </summary>
        public string DismissSpriteName;

       protected const string AutoPlayKey = "-autoplay";

        protected void Awake()
        {
            OnAwake();
        }

        void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGetGameInfo);
            Facade.EventCenter.AddEventListeners<YxESysEventType, Dictionary<string, object>>(YxESysEventType.SysInitGameComponent, RepositionBtns);
        }

        private void OnGetGameInfo(DdzbaseEventArgs args)
        {

            InitBtnsActive(args);

            var gdata = App.GetGameData<DdzGameData>();
            //显示和设置退出按钮
            SetBtnActive(OutBtn.gameObject, true);
            bool isRoomGame = gdata.IsRoomGame;
            string spriteName = isRoomGame ? DismissSpriteName : OutBtnSpriteName;
            SetOutRoomBtnSprite(spriteName);

            //房卡模式下,隐藏更换房间按钮
            SetBtnActive(ChangeRoomBtn.gameObject, !isRoomGame);      
        }


        /// <summary>
        /// 初始化按钮的显示情况
        /// </summary>
        /// <param name="args"></param>
        void InitBtnsActive(DdzbaseEventArgs args)
        {
            var gameInfo = args.IsfObjData;
            if (!gameInfo.ContainsKey(NewRequestKey.KeyCargs2)) return;
            var cargs2 = gameInfo.GetSFSObject(NewRequestKey.KeyCargs2);
            if (cargs2.ContainsKey(AutoPlayKey))
            {
                bool showAutoPlay = int.Parse(cargs2.GetUtfString(AutoPlayKey)) > 0;
                AutoPlayBtn.gameObject.SetActive(showAutoPlay);
            }
            RepositionBtns(null);
        }

        /// <summary>
        /// 按钮排序
        /// </summary>
        /// <param name="dic"></param>
        private void RepositionBtns(Dictionary<string, object> dic)
        {
            if (TopRightGrid == null) return;

            TopRightGrid.repositionNow = true;
            TopRightGrid.Reposition();
        }

        public void SetBtnActive(GameObject btn, bool active)
        {
            if (btn == null) return;
            btn.SetActive(active);
        }

        /// <summary>
        /// 将退出房间按钮图标改为更换房间图标
        /// </summary>
        /// <param name="spriteName"></param>
        private void SetOutRoomBtnSprite(string spriteName)
        {
            var btnSprite = OutBtn.GetComponent<UISprite>();
            btnSprite.spriteName = string.Format("{0}_up", spriteName);
            OutBtn.normalSprite = string.Format("{0}_up", spriteName);
            OutBtn.pressedSprite = string.Format("{0}_over", spriteName);
            OutBtn.hoverSprite = string.Format("{0}_over", spriteName);
        }


        protected void Start()
        {
            AddOutBtnClick();
            AddChangeRoomBtnClick();
            AddSettingBtnClick();
            AddAutoPlayBtn();
        }


        /// <summary>
        /// 添加退出房间事件
        /// </summary>
        void AddOutBtnClick()
        {
            if (OutBtn == null) return;
            OutBtn.onClick = new List<EventDelegate> { new EventDelegate(OnClickOutBtn) };  
        }

        /// <summary>
        /// 添加更换房间事件
        /// </summary>
        void AddChangeRoomBtnClick()
        {
            if (ChangeRoomBtn == null) return;
            ChangeRoomBtn.onClick = new List<EventDelegate> { new EventDelegate(OnClickChangeRoomBtn) };
        }

        /// <summary>
        /// 添加托管按钮事件
        /// </summary>
        void AddAutoPlayBtn()
        {
            if (AutoPlayBtn == null) return;
            AutoPlayBtn.onClick = new List<EventDelegate> {new EventDelegate(OnClickAutoPlayBtn)};
        }

        /// <summary>
        /// 添加设置按钮事件
        /// </summary>
        void AddSettingBtnClick()
        {
            if (SettingBtn == null) return;
            var list = new List<EventDelegate>();
            var ed = new EventDelegate(this, "OnClickSettingBtn");
            list.Add(ed);
            ed.parameters[0] = new EventDelegate.Parameter { obj = SettingBtn.gameObject };
            EventDelegate.Add(SettingBtn.onClick, ed);
        }

        /// <summary>
        /// 点击托管按钮事件
        /// </summary>
        public void OnClickAutoPlayBtn()
        {
            //如果游戏数据没有初始化完成,不能托管
            if (!App.GetGameData<DdzGameData>().FinishRoomInfo)
            {
                YxDebug.LogError("房间信息没有初始化完成");
                return; 
            }
            App.GetRServer<DdzGameServer>().SendAutoPlayState(true);
        }


        /// <summary>
        /// 点击退出房间按钮
        /// </summary>
        public void OnClickOutBtn()
        {
            var gdata = App.GetGameData<DdzGameData>();
            if (!gdata.FinishRoomInfo) return;

            bool isPlayed = gdata.CurrentRound > 0;
            bool isRoomOwner = gdata.IsRoomOwner;

            if (!gdata.IsRoomGame)
            {
                if (CheckCouldOut())
                {
                    YxMessageBox.Show(new YxMessageBoxData
                    {
                        Msg = "您确定要退出游戏吗?",
                        BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                        Listener = (box, btnName) =>
                        {
                            if (btnName == YxMessageBox.BtnLeft)
                            {
                                if (CheckCouldOut())
                                {
                                    App.QuitGame();
                                }
                            }
                        }
                    });
                }
                return;
            }

            if (isPlayed)
            {
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "确定要发起投票,解散房间么?",
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            DismissRoom(2); //房间游戏已开始,发起投票
                        }
                    }
                });
            }
            else if (isRoomOwner)
            {
                //房卡游戏没有开始,房主解散房间
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "确定要解散房间吗?",
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {

                            if (CheckCouldOut())
                            {
                                IRequest req = new ExtensionRequest("dissolve", new SFSObject());
                                App.GetRServer<DdzGameServer>().SendRequest(req);
                            }
                        }
                    },
                    IsTopShow = true,
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                });
            }
            else
            {
               if(CheckCouldOut())
               {
                   App.QuitGame();
               }
            }
        }


        /// <summary>
        /// 点击更换房间按钮
        /// </summary>
        public void OnClickChangeRoomBtn()
        {
            var gdata = App.GetGameData<DdzGameData>();
            bool isRoomGame = gdata.IsRoomGame;

            if (isRoomGame) return;

            //非房卡模式下,直接更换房间
            if (CheckCouldOut())
            {
                App.GetRServer<DdzGameServer>().ChangeRoom();
            }
        }

        /// <summary>
        /// 发起和决定解散房间
        /// 2发起解散，3同意，-1拒绝
        /// </summary>
        public void DismissRoom(int yon)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("cmd", "dismiss");
            iobj.PutInt("id", App.GetGameData<DdzGameData>().GetPlayerInfo().Id);
            iobj.PutInt(YxFramwork.ConstDefine.RequestKey.KeyType, yon);

            IRequest request = new ExtensionRequest("hup", iobj);
            App.GetRServer<DdzGameServer>().SendRequest(request);
        }


        /// <summary>
        /// 非房卡模式下,检测是否可以退出游戏
        /// </summary>
        /// <returns></returns>
        bool CheckCouldOut()
        {
            var gdata = App.GetGameData<DdzGameData>();
            if (gdata == null) return false;

            bool isGameing = (gdata.IsGameStart || gdata.AllReady());
            if (isGameing)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "正在游戏中,无法退出房间!"
                });
            }
            return !isGameing;
        }


        /// <summary>
        /// 点击设置按钮
        /// </summary>
        public void OnClickSettingBtn(GameObject obj)
        {
            YxWindowManager.OpenWindow(obj.name);
        }
    }
}