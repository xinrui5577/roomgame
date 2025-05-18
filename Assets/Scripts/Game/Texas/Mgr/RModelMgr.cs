using System.Collections;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Texas.ImgPress;
using Assets.Scripts.Game.Texas.Main;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;
#pragma warning disable 649

namespace Assets.Scripts.Game.Texas.Mgr
{
    //public delegate void RModelEvent(Sfs2X.Entities.Data.ISFSObject gameInfo);
    public class RModelMgr : MonoBehaviour
    {

        //public RModelEvent Event;

        /// <summary>
        /// 房间的信息窗口
        /// </summary>
        [SerializeField]
        protected GameObject RoomInfo = null;

        /// <summary>
        /// 本房间的最大局数
        /// </summary>
        [SerializeField]
        protected UILabel TimeValue = null;

        /// <summary>
        /// 当前房间的ID
        /// </summary>
        [SerializeField]
        protected UILabel RoomIdValue = null;



        /// <summary>
        /// 结算的预制体所在的Grid
        /// </summary>
        [SerializeField]
        private UIGrid _resutlItemsGrid = null;

        /// <summary>
        /// 开房模式结算窗口
        /// </summary>
        [SerializeField]
        protected GameObject RoomResult = null;

        /// <summary>
        /// 当前时间，用于计时
        /// </summary>
        protected int CurrentTime = -1;

        /// <summary>
        /// 解散房间倒计时
        /// </summary>
        protected int DismissTime;

        /// <summary>
        /// 返回大厅按钮
        /// </summary>
        [SerializeField]
        private UIButton _bcakButton;

        /// <summary>
        /// 分享按钮
        /// </summary>
        [SerializeField]
        private UIButton _shareButton;


        /// <summary>
        /// 微信邀请按钮
        /// </summary>
        [SerializeField]
        private UIButton _invitButton;

        /// <summary>
        /// 截图对象
        /// </summary>
        [SerializeField]
        private CompressImg _img;

        [SerializeField]
        private UILabel _countDownValue;

        [SerializeField]
        private UILabel _timeBgLabel;

        /// <summary>
        /// 房间ID
        /// </summary>
        private int _roomId;

        /// <summary>
        /// 记录本桌的可游戏时间
        /// </summary>
        protected int MaxRound;

        /// <summary>
        /// 解散房间窗口的Grid
        /// </summary>
        [SerializeField]
        protected UIGrid DismissGrid;

        /// <summary>
        /// 解散房间同意按钮
        /// </summary>
        [SerializeField]
        private UIButton _agreeBtn;

        /// <summary>
        /// 解散房间拒绝按钮
        /// </summary>
        [SerializeField]
        private UIButton _disagreeBtn;

        /// <summary>
        /// 解散房间的关闭按键
        /// </summary>
        [SerializeField]
        private UIButton _closeBtn;


        /// <summary>
        /// 更换房间按钮
        /// </summary>
        [SerializeField]
        private GameObject _changRoomBtn;


        /// <summary>
        /// 解散房间窗口
        /// </summary>
        public GameObject DismissRoom;

        /// <summary>
        /// 房间信息
        /// </summary>
        private string _ruleInfo;

        protected bool IsRoundGame;

       

        /// <summary>
        /// 显示玩家结算数据的成员数组
        /// </summary>
        public ResultItemInfo[] ResultItems = new ResultItemInfo[6];

        public DismissMsgItem[] DismissItems = new DismissMsgItem[6];


        // Use this for initialization
        protected void Start()
        {
            AddOnClick();
        }

        /// <summary>
        /// 显示房间信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public virtual void ShowRoomInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<TexasGameData>();
            if (_changRoomBtn != null)
            {
                _changRoomBtn.SetActive(false);
            }
            DismissRoom.SetActive(false);

            if (gameInfo.ContainsKey("cargs2"))
            {
                ISFSObject cargs2 = gameInfo.GetSFSObject("cargs2");
                if (cargs2.ContainsKey("-overT"))
                {
                    int t = int.Parse(cargs2.GetUtfString("-overT"));
                    IsRoundGame = t > 0;
                }
            }


            //设置、显示房间号
            if (gameInfo.ContainsKey("rid"))
            {
                _roomId = gameInfo.GetInt("rid");
                RoomIdValue.text = _roomId.ToString();
            }
            MaxRound = gameInfo.GetInt("maxRound");

            if (IsRoundGame)
            {
                if (gameInfo.ContainsKey("round"))
                {
                    int round = gameInfo.GetInt("round");
                    gdata.CurRound = round;
                    UpDataRoundValue(round);
                    gdata.IsPlayed = round > 0;
                }
                if(_timeBgLabel != null)
                    _timeBgLabel.text = "局数:";
            }
            else
            {
                int roomPlayedTime = 0;                                         //房间进行过的游戏时间
                CurrentTime = MaxRound * 60 - roomPlayedTime;                //计算当前游戏还有多长时间
                ShowRoomInfoTime(CurrentTime);                                //显示时间

                //记录房间是否进行过游戏
                if (gameInfo.ContainsKey("roomPlayed"))
                {
                    var isPlayed = gameInfo.GetBool("roomPlayed");
                    gdata.IsPlayed = isPlayed;
                    SetButtonsActive(!isPlayed);
                    if (isPlayed)
                    {
                        if (gameInfo.ContainsKey("croomct"))
                        {
                            CalibrationTime((int)gameInfo.GetLong("croomct"));
                        }
                    }
                }
            }

            RoomInfo.transform.localScale = Vector3.one;
            RoomInfo.SetActive(true);
            


            //微信邀请在PC上无法运行
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE
            _invitButton.gameObject.SetActive(gdata.GStatus != YxEGameStatus.PlayAndConfine && !gdata.IsPlayed);
#endif

            if (gameInfo.ContainsKey("hup"))
            {
                ShowRoomDismiss((int)((gameInfo.GetLong("svt")) - gameInfo.GetLong("hupstart")));
                string hupStr = gameInfo.GetUtfString("hup");
                string[] hupStrArr = hupStr.Split(',');
                foreach (string hupItem in hupStrArr)
                {
                    int id = int.Parse(hupItem);
                    UpdateDismissInfo(id, 3);
                }

                SetTitelInfo(int.Parse(hupStrArr[0]));
            }

            if (gameInfo.ContainsKey("rule"))
            {
                _ruleInfo = gameInfo.GetUtfString("rule");
            }

            if (gdata.IsPlayed)
            {
                BeginCountDown();
            }
        }

        
        public virtual void UpDataRoundValue(int round)
        {
            if (!IsRoundGame) return;
            TimeValue.text = string.Format("{0} / {1}", round, MaxRound);
        }

        /// <summary>
        /// 隐藏并重置房间信息内容
        /// </summary>
        public virtual void HideRoomInfo()
        {
            CurrentTime = -1;
            //_playerNumValue.text = string.Empty;
            RoomIdValue.text = string.Empty;
            TimeValue.text = string.Empty;

            RoomInfo.SetActive(false);
            RoomResult.SetActive(false);

            SetButtonsActive(false);
        }

        /// <summary>
        /// 时间校准
        /// </summary>
        public void CalibrationTime(int time)
        {
            if (IsRoundGame) return;
            CurrentTime = MaxRound * 60 - time;
            ShowRoomInfoTime(CurrentTime);
        }


        /// <summary>
        /// 将时间转换成固定格式的时间格式,以分钟为单位
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        void ShowRoomInfoTime(int time)
        {
            if (time < 0)
            {
                time = 0;
            }
            string timeString = GetHour(time) + ":" + GetMinute(time) + ":" + GetSecond(time);
            TimeValue.text = timeString;

        }

        /// <summary>
        /// 显示解散房间的窗口
        /// </summary>
        /// <param name="time"></param>
        public virtual void ShowRoomDismiss(int time = 0)
        {
            if (DismissRoom.activeSelf) { return; }
            var index = 0;

            //初始化玩家投票窗口信息
            var gdata = App.GameData;
            var playerList = gdata.PlayerList;
            var playerCount = playerList.Length;
            for (var i = 0; i < playerCount; i++)
            {
                var info = gdata.GetPlayerInfo(i);
                if (info == null || info.Id <= 0) { continue; }
                var item = DismissItems[index++];
                item.PlayerName = info.NickM;
                item.PlayerId = info.Id;
                item.PlayerType = 2;
            }

            //隐藏多余的内容
            for (var i = 0; i < DismissItems.Length; i++)
            {
                DismissItems[i].gameObject.SetActive(i < index);
            }
            SetMannerBtn(true);
            DismissGrid.hideInactive = true;
            DismissGrid.Reposition();

            DismissRoom.SetActive(true);
            DismissRoom.transform.GetChild(0).gameObject.SetActive(true);

            DismissTime = 300 - time;

            StartCoroutine(CountDownDismissUpdate());
        }



        /// <summary>
        /// 更新玩家投票结果
        /// </summary>
        /// <param name="id">玩家id</param>
        /// <param name="playerType">玩家投票态度,3为同意,-1为拒绝</param>
        public void UpdateDismissInfo(int id, int playerType)
        {

            for (var i = 0; i < DismissItems.Length; i++)
            {
                var dismissItem = DismissItems[i];
                if (dismissItem.PlayerId == id)
                {
                    dismissItem.PlayerType = playerType;
                }
            }

            if (id.ToString().Equals(App.GameData.GetPlayerInfo().UserId))
            {
                SetMannerBtn(false);
            }
        }

        string GetHour(int time)
        {
            return string.Format("{0:D2}", time / 3600);
        }

        string GetMinute(int time)
        {
            return string.Format("{0:D2}", time % 3600 / 60);
        }

        string GetSecond(int time)
        {
            return string.Format("{0:D2}", time % 60);
        }



        /// <summary>
        /// 进行倒计时
        /// </summary>
        IEnumerator CountDownCurrentTimeUpdate()
        {
            while (CurrentTime > 0)
            {
                ShowRoomInfoTime(--CurrentTime);
                yield return new WaitForSeconds(1.0f);
            }
        }

        /// <summary>
        /// 投票解散房间的倒计时
        /// </summary>
        protected IEnumerator CountDownDismissUpdate()
        {
            while (DismissTime > 0)
            {
                _countDownValue.text = string.Format("{0:D2}秒", --DismissTime);
                yield return new WaitForSeconds(1.0f);
            }
            DismissRoom.transform.GetChild(0).gameObject.SetActive(false);
            DismissRoom.SetActive(false);
        }

        /// <summary>
        /// 显示结算窗口
        /// </summary>
        /// <param name="index">显示的玩家个数</param>
        public void ShowRoomResult(int index)
        {
            DismissRoom.transform.GetChild(0).gameObject.SetActive(false);

            for (int i = 0; i < ResultItems.Length; i++)
            {
                ResultItems[i].gameObject.SetActive(i <= index);
            }
            _resutlItemsGrid.hideInactive = true;
            _resutlItemsGrid.Reposition();
            RoomResult.SetActive(true);
        }
      
        void AddOnClick()
        {
            ////添加返回大厅按钮的OnClick事件
            _bcakButton.onClick.Add(new EventDelegate(() =>
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "确定要返回大厅吗?",
                    IsTopShow = true,
                    BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                    Listener = (box, btnName) =>
                    {
                        if (btnName == YxMessageBox.BtnLeft)
                        {
                            App.QuitGame();
                        }
                    },
                });
            }
            ));

            

            //微信邀请
            _invitButton.onClick.Add(new EventDelegate(() =>
            {
                YxTools.ShareFriend(_roomId.ToString(), _ruleInfo);
            }));
            //分享战绩
            _shareButton.onClick.Add(new EventDelegate(() =>
            {
                YxWindowManager.ShowWaitFor();

                Facade.Instance<WeChatApi>().InitWechat();

                UserController.Instance.GetShareInfo(info =>
               {
                   YxWindowManager.HideWaitFor();

                   _img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
                   {
                       if (Application.platform == RuntimePlatform.Android)
                       {
                           imageUrl = "file://" + imageUrl;
                       }
                       info.ImageUrl = imageUrl;
                       info.ShareType = ShareType.Image;
                       Facade.Instance<WeChatApi>().ShareContent(info);

                   });

               });


            }));

            _agreeBtn.onClick.Add(new EventDelegate(() => { App.GetRServer<TexasGameServer>().DismissRoom(3); SetMannerBtn(false); }));

            _disagreeBtn.onClick.Add(new EventDelegate(() => { App.GetRServer<TexasGameServer>().DismissRoom(-1); SetMannerBtn(false); }));

            _closeBtn.onClick.Add(new EventDelegate(() => { DismissRoom.transform.GetChild(0).gameObject.SetActive(false); }));


        }

        /// <summary>
        /// 设置投票按钮是否隐藏
        /// </summary>
        /// <param name="showBtn"></param>
        public void SetMannerBtn(bool showBtn)
        {
            _agreeBtn.gameObject.SetActive(showBtn);
            _disagreeBtn.gameObject.SetActive(showBtn);
        }
        public virtual void SetTitelInfo(int id)
        {

        }

        /// <summary>
        /// 设置准备和微信邀请按钮的显示
        /// </summary>
        /// <param name="active"></param>
        protected void SetButtonsActive(bool active)
        {
            //微信邀请在PC上无法运行
#if  UNITY_EDITOR||UNITY_ANDROID || UNITY_IPHONE
            _invitButton.gameObject.SetActive(active);
#endif
        }


        public void OnGameStart()
        {
            SetButtonsActive(false);
        }
        private bool _counting;

        /// <summary>
        /// 开始倒计时
        /// </summary>
        public void BeginCountDown()
        {
            if (IsRoundGame || _counting)
                return;
            _counting = true;
            StartCoroutine(CountDownCurrentTimeUpdate());
        }

        internal void SetDismissRoomInfo(ISFSObject data)
        {
            if (!data.ContainsKey("type"))
                return;

            int id = data.GetInt("id");
            int type = data.GetInt("type");
            ShowRoomDismiss();
            SetTitelInfo(id);
            UpdateDismissInfo(id, type);

            if (type < 0)
            {
                SetMannerBtn(false);
                DismissTime = 2;
            }
        }
    }
}
