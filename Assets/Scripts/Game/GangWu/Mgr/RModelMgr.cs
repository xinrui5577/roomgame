using UnityEngine;
using System.Collections;
using com.yxixia.utile.YxDebug;
using YxFramwork.View;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.GangWu.ImgPress;
using Assets.Scripts.Game.GangWu.Main;
using YxFramwork.Controller;
using YxFramwork.Common;
using YxFramwork.Manager;
using Sfs2X.Entities.Data;
// ReSharper disable FieldCanBeMadeReadOnly.Local


namespace Assets.Scripts.Game.GangWu.Mgr
{
    
    public class RModelMgr : MonoBehaviour
    {

        //public RModelEvent Event;

        /// <summary>
        /// 房间的信息窗口
        /// </summary>
        [SerializeField]
        private GameObject _roomInfo = null;
        /// <summary>
        /// 本房间的最大局数
        /// </summary>
        [SerializeField]
        private UILabel _timeValue = null;
        /// <summary>
        /// 当前房间的ID
        /// </summary>
        [SerializeField]
        private UILabel _roomIdValue = null;

        /// <summary>
        /// 结算的预制体所在的Grid
        /// </summary>
        [SerializeField]
        private UIGrid _resutlItemsGrid = null;

        /// <summary>
        /// 开房模式结算窗口
        /// </summary>
        [SerializeField]
        private GameObject _roomResult = null;

        /// <summary>
        /// 当前时间，用于计时
        /// </summary>
        private int _currentTime = -1;

        /// <summary>
        /// 解散房间倒计时
        /// </summary>
        private int _dismissTime;

        /// <summary>
        /// 返回大厅按钮
        /// </summary>
        [SerializeField]
        private UIButton _bcakButton = null;

        /// <summary>
        /// 分享按钮
        /// </summary>
        [SerializeField]
        private UIButton _shareButton = null;

        /// <summary>
        /// 微信邀请按钮
        /// </summary>
        [SerializeField]
        private UIButton _invitButton = null;


        [SerializeField]
        private UILabel _countDownValue = null;

        /// <summary>
        /// 房间ID
        /// </summary>
        private int _roomId;


        /// <summary>
        /// 记录本桌的可游戏时间
        /// </summary>
        private int _maxRound;
      


        /// <summary>
        /// 解散房间窗口的Grid
        /// </summary>
        [SerializeField]
        private UIGrid _dismissGrid = null;

        /// <summary>
        /// 解散房间同意按钮
        /// </summary>
        [SerializeField]
        private UIButton _agreeBtn = null;

        /// <summary>
        /// 解散房间拒绝按钮
        /// </summary>
        [SerializeField]
        private UIButton _disagreeBtn = null;


        /// <summary>
        /// 关闭解散房间窗口按钮
        /// </summary>
        [SerializeField]
        private UIButton _closeBtn = null;


        /// <summary>
        /// 解散房间窗口
        /// </summary>
        public GameObject DismissRoom = null;

        /// <summary>
        /// 房间信息
        /// </summary>
        private string _ruleInfo;

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
        public void ShowRoomInfo(ISFSObject gameInfo)
        {
            var gdata = App.GetGameData<GangwuGameData>();
            gdata.IsRoomGame = true;
            DismissRoom.SetActive(false);
            int roomPlayedTime = 0;                                         //房间进行过的游戏时间
            //设置、显示剩余时间
            _maxRound = gameInfo.GetInt("maxRound");                        //给倒计时时间进行赋值,此处的maxRound为游戏时间

            //记录房间是否进行过游戏
            if (gameInfo.ContainsKey("roomPlayed"))
            {
                gdata.IsPlayed = gameInfo.GetBool("roomPlayed");
            }

            _currentTime = _maxRound * 60 - roomPlayedTime;                //计算当前游戏还有多长时间
            ShowRoomInfoTime(_currentTime);                                //显示时间

            //设置、显示房间号
            if (gameInfo.ContainsKey("rid"))
            {
                _roomId = gameInfo.GetInt("rid");
                _roomIdValue.text = _roomId.ToString();
            }

            if (gameInfo.ContainsKey("croomct"))
            {
                CalibrationTime((int)gameInfo.GetLong("croomct"));
            }
            
            _roomInfo.SetActive(true);

            //微信邀请在PC上无法运行
#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE
            _invitButton.gameObject.SetActive(!gdata.IsGameStart);
#endif

            if (gameInfo.ContainsKey("hup"))
            {
                ShowRoomDismiss();
            }
            if (gameInfo.ContainsKey("rule"))
            {
                _ruleInfo = gameInfo.GetUtfString("rule");
            }
            StartCoroutine(CountDownCurrentTimeUpdate());

        }


        /// <summary>
        /// 隐藏并重置房间信息内容
        /// </summary>
        public void HideRoomInfo()
        {
            _currentTime = -1;
            _roomIdValue.text = string.Empty;
            _timeValue.text = string.Empty;

            _roomInfo.SetActive(false);
            _roomResult.SetActive(false);

            //微信邀请在PC上无法运行
#if  UNITY_EDITOR||UNITY_ANDROID || UNITY_IPHONE
            _invitButton.gameObject.SetActive(false);
#endif
        }


 

        /// <summary>
        /// 时间校准
        /// </summary>
        public void CalibrationTime(int time)
        {
            _currentTime = _maxRound * 60 - time;
            ShowRoomInfoTime(_currentTime);
        }

        /// <summary>
        /// 将时间转换成固定格式的时间格式,以分钟为单位
        /// </summary>
        /// <returns></returns>
        void ShowRoomInfoTime(int time)
        {
            if (time < 0)
            {
                time = 0;
            }
            string timeString = GetHour(time) + ":" + GetMinute(time) + ":" + GetSecond(time);
            _timeValue.text = timeString;
        }

        /// <summary>
        /// 显示解散房间的窗口
        /// </summary>
        public void ShowRoomDismiss()
        {
            if (DismissRoom.activeSelf)
            {
                return;
            }
           
                int index = 0;

                var users = App.GameData.PlayerList;
                foreach (var yxBaseGamePlayer in users)
                {
                    var user = (PlayerPanel) yxBaseGamePlayer;
                    if (user.Info.Id <= 0)
                    {
                        continue;
                    }

                    DismissMsgItem item = DismissItems[index++];
                    item.PlayerName = user.Nick;
                    item.PlayerId = user.Info.Id;
                    item.PlayerType = 2;

                }

                for (int i = 0; i < DismissItems.Length; i++)
                {
                    DismissItems[i].gameObject.SetActive(i < index);
                }
                SetMannerBtn(true);
                _dismissGrid.hideInactive = true;
                _dismissGrid.Reposition();
            
            DismissRoom.SetActive(true);
            DismissRoom.transform.GetChild(0).gameObject.SetActive(true);
           
            _dismissTime = 20;
            StartCoroutine(CountDownDismissUpdate());
        }

        
        /// <summary>
        /// 更新玩家投票结果
        /// </summary>
        /// <param name="id">玩家id</param>
        /// <param name="playerType">玩家投票态度,3为同意,-1为拒绝</param>
        public void UpdateDismissInfo(int id,int playerType)
        {
            if (playerType == 2)
                ShowRoomDismiss();

            for (int i = 0; i < DismissItems.Length; i++)
            {
                if (DismissItems[i].PlayerId == id)
                {
                    DismissItems[i].PlayerType = playerType;
                }
            }

            if (id == App.GameData.GetPlayer().Info.Id)
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
            return string.Format("{0:D2}",time % 60);
        }

        

        /// <summary>
        /// 进行倒计时
        /// </summary>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator CountDownCurrentTimeUpdate()
        {
            while (_currentTime > 0)
            {
                ShowRoomInfoTime(--_currentTime);
                yield return new WaitForSeconds(1.0f);
            }
            StopCoroutine(CountDownCurrentTimeUpdate());
        }

        /// <summary>
        /// 投票解散房间的倒计时
        /// </summary>
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private IEnumerator CountDownDismissUpdate()
        {
            while (_dismissTime > 0)
            {
                _countDownValue.text = string.Format("{0:D2}秒", --_dismissTime);
                yield return new WaitForSeconds(1.0f);
            }
            DismissRoom.transform.GetChild(0).gameObject.SetActive(false);
            DismissRoom.SetActive(false);
            StopCoroutine(CountDownDismissUpdate());
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
            _roomResult.SetActive(true);
        }
      

        void AddOnClick()
        {
            //添加返回大厅按钮的OnClick事件
            _bcakButton.onClick.Add(new EventDelegate(() =>
            {
                YxMessageBox.Show(new YxMessageBoxData()
                    {
                        Msg = "确定要退出大厅么?",
                        BtnStyle = YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle,
                        IsTopShow = true,
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
                YxTools.ShareFriend(_roomId.ToString(),_ruleInfo);
            }));

            //分享战绩
            _shareButton.onClick.Add(new EventDelegate(() =>
            {
                YxWindowManager.ShowWaitFor();

                Facade.Instance<WeChatApi>().InitWechat();

                UserController.Instance.GetShareInfo(info =>
               {
                   YxWindowManager.HideWaitFor();

                   var img = GetComponent<CompressImg>() ?? gameObject.AddComponent<CompressImg>();

                   img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
                   {
                       YxDebug.Log("Url == " + imageUrl);
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

            //港五暂无开房模式
            _agreeBtn.onClick.Add(new EventDelegate(() => { App.GetRServer<GangWuGameServer>().DismissRoom(3); SetMannerBtn(false); }));

            _disagreeBtn.onClick.Add(new EventDelegate(() => { App.GetRServer<GangWuGameServer>().DismissRoom(-1); SetMannerBtn(false); }));

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



        /// <summary>
        /// 隐藏微信邀请按钮
        /// </summary>
        public void HideInvitButton()
        {
            //微信邀请在PC上无法运行
#if  UNITY_EDITOR||UNITY_ANDROID || UNITY_IPHONE
            _invitButton.gameObject.SetActive(false);
#endif
        }





    }
}
