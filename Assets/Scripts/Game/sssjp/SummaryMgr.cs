using System;
using System.Collections.Generic;
using Assets.Scripts.Game.sssjp.ImgPress.Main;
using Assets.Scripts.Game.sssjp.Tool;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.sssjp
{
    public class SummaryMgr : MonoBehaviour
    {

        /// <summary>
        /// 结算预制体的脚本
        /// </summary>
        public GameObject ItemPrefab;

        /// <summary>
        /// 时间Label
        /// </summary>
        public UILabel TimeLabel;

        /// <summary>
        /// 房间信息Label
        /// </summary>
        public UILabel GameInfoLabel;

        /// <summary>
        /// 玩家信息Grid
        /// </summary>
        public UIGrid UserInfoGrid;

        /// <summary>
        /// 总结算Grid
        /// </summary>
        //public UIGrid ScoreGrid;

        /// <summary>
        /// 结算单元的父层级Grid
        /// </summary>
        public UIGrid SumItemGrid;

        /// <summary>
        /// 分享战绩按钮
        /// </summary>
        public UIButton ShareBtn;

        /// <summary>
        /// 关闭窗体按钮
        /// </summary>
        public UIButton CloseBtn;

        /// <summary>
        /// 查看战绩按钮
        /// </summary>
        public UIButton HistoryBtn;

        /// <summary>
        /// 返回大厅按钮
        /// </summary>
        public UIButton BackBtn;

        public SumUserInfo SumUserPrefab;
        public List<SumUserInfo> SumUserList = new List<SumUserInfo>();

        public UILabel[] SumLabels;

        public int Area = 1120;

        /// <summary>
        /// 排序方式,0为按座位号排序,1和-1按总分排序
        /// </summary>
        public int Sequence;

        private List<SummaryUserInfo> _summaryUserInfoList = new List<SummaryUserInfo>();

        /// <summary>
        /// WxSenceSession,分享给好友 ; WxSenceTimeLine,分享朋友圈 ; WxSenceFavorite,收藏到微信(未曾使用过)
        /// </summary>
        public SharePlat ChatShareType = SharePlat.WxSenceSession;

        /// <summary>
        /// 时间戳的起始时间
        /// </summary>
        DateTime _dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));


        // Use this for initialization
        protected void Start()
        {
            ShareBtn.onClick.Add(new EventDelegate(OnClickShare));
            BackBtn.onClick.Add(new EventDelegate(OnClickBackBtn));
            CloseBtn.onClick.Add(new EventDelegate(() => { gameObject.SetActive(false); }));
            HistoryBtn.onClick.Add(new EventDelegate(() =>
            {
                App.GetGameManager<SssjpGameManager>().SettingMenu.HistoryView.SetActive(true);
            }));
        }

        /// <summary>
        /// 初始化窗口信息
        /// </summary>
        /// <param name="data"></param>
        public void Init(ISFSObject data)
        {
            InitUsersInfo(data);
            InitRoomInfo(data);
            InitSummaryInfo(data);

            gameObject.SetActive(true);
        }

        /// <summary>
        /// 初始化房间信息
        /// </summary>
        /// <param name="data">数据</param>
        void InitRoomInfo(ISFSObject data)
        {
            TimeLabel.text = "对战时间: " + ToRealTime(data.GetLong("svt"));

            RoomInfo roomInfo = App.GetGameManager<SssjpGameManager>().RoomInfo;
            GameInfoLabel.text = string.Format("房间号:{0} {1}", roomInfo.RoomID, roomInfo.RuleInfo);
        }


        void InitUsersInfo(ISFSObject data)
        {
            ISFSArray users = data.GetSFSArray("users");
            foreach (ISFSObject user in users)
            {
                int id = user.GetInt("id");
                if (id < 0) continue;
                var userInfo = new SummaryUserInfo
                {
                    Id = id,
                    Nick = user.GetUtfString("nick"),
                    Seat = user.GetInt("seat"),
                    Gold = user.GetInt("gold"),
                    SexI = user.GetShort("sex"),
                    Record = user.GetIntArray("record"),
                    AvatarX = user.GetUtfString("avatar"),
                };
                _summaryUserInfoList.Add(userInfo);
            }
            _summaryUserInfoList.Sort((s1, s2) => Sequence*(s1.Gold - s2.Gold));

            int count = _summaryUserInfoList.Count;
        
            var parent = UserInfoGrid.transform;
            int bwIndex = -1;
            int bwScore = -1;
            for (int i = 0; i < count; i++)
            {
                var userInfo = _summaryUserInfoList[i];
        
                int ttScore = userInfo.Gold;
                if (ttScore > bwScore)
                {
                    bwIndex = i;
                    bwScore = ttScore;
                }
                var sumUser = Instantiate(SumUserPrefab);
                sumUser.transform.parent = parent;
                sumUser.transform.localScale = Vector3.one;
                sumUser.Init(userInfo);
                SumUserList.Add(sumUser);
            }
            SumUserList[bwIndex].ShowBigWinnerMark();
            int space = Area / count;
            foreach (var item in SumUserList)
            {
                item.SetWidget(space);
            }

            UserInfoGrid.cellWidth = space;
            UserInfoGrid.hideInactive = true;
            UserInfoGrid.Reposition();
            SumUserList[count - 1].HideLine();
        }

        /// <summary>
        /// 将时间戳转换为正常时间
        /// </summary>
        /// <param name="timpStamp">服务器传输的时间数据</param>
        /// <returns></returns>
        string ToRealTime(long timpStamp)
        {
            long unixTime = timpStamp * 10000000L;
            TimeSpan toNow = new TimeSpan(unixTime);
            DateTime dt = _dtStart.Add(toNow);
            return dt.ToString("yyyy/MM/dd  HH:mm:ss");
        }


        /// <summary>
        /// 初始化结算界面数据
        /// </summary>
        /// <param name="data"></param>
        void InitSummaryInfo(ISFSObject data)
        {
            ISFSObject sfsObj = data.GetSFSArray("users").GetSFSObject(0);
            int round = data.GetInt("round");
            if (sfsObj.ContainsKey("record"))
            {
                int len = SumUserList.Count;
                for (int i = 0; i < round; i++)
                {
                    List<int> scoreList = new List<int>();
                    for (int j = 0; j < len; j++)
                    {
                        if (SumUserList[j].Id <= 0) continue;
                        if (i >= SumUserList[j].ScoreArray.Length)
                        {
                            Debug.LogError("发现有人少了一局");
                            continue;
                        }

                        scoreList.Add(SumUserList[j].ScoreArray[i]);
                    }
                    GameObject go = Instantiate(ItemPrefab);
                    go.transform.parent = SumItemGrid.transform;
                    go.transform.localScale = Vector3.one;
                    go.GetComponent<SummaryItem>().InitItem(i + 1, scoreList.ToArray());
                }
            }
        }

        void SetLabel(UILabel label, int score)
        {
            label.text = YxUtiles.ReduceNumber(score);
            if (score < 0)
            {
                label.gradientTop = Tools.ChangeToColor(0x6FFBF1);
                label.gradientBottom = Tools.ChangeToColor(0x0090FF);
                label.effectColor = Tools.ChangeToColor(0x002EA3);
            }
            else
            {
                label.gradientTop = Tools.ChangeToColor(0xFFFF00);
                label.gradientBottom = Tools.ChangeToColor(0xFF9600);
                label.effectColor = Tools.ChangeToColor(0x831717);
            }

            label.gameObject.SetActive(true);
        }

        private string _screenshot;

        /// <summary>
        /// 是否有分享奖励
        /// </summary>
        public bool HaveRequest = false;

        /// <summary>
        /// 点击分享战绩按钮,可复用
        /// </summary>
        public void OnClickShare()
        {
            _screenshot = App.UI.CaptureScreenshot();
            Invoke("CaptureScreenshot", 1f);
        }

        void CaptureScreenshot()
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);
            var dic = new Dictionary<string, object>();
            dic["type"] = 1;
            dic["image"] = _screenshot;
            dic["shareType"] = 1;
            dic["sharePlat"] = 0;
            UserController.Instance.GetShareInfo(dic, info => Facade.Instance<WeChatApi>().ShareContent(info, str =>
            {
                //发送数据,获取奖励
                if (HaveRequest)
                {
                    var dict = new Dictionary<string, object>();
                    dict["option"] = 2;
                    dict["sharePlat"] = SharePlat.WxSenceSession.ToString();
                    Facade.Instance<TwManager>().SendAction("shareGameResultRequest", dict, null);
                }
            }));
        }

        public bool ShowMessageBox = true;

        /// <summary>
        /// 点击返回大厅按钮
        /// </summary>
        public void OnClickBackBtn()
        {
            if (ShowMessageBox)
            {
                YxMessageBox.Show(new YxMessageBoxData()
                {
                    Msg = "确定要退出房间么!?",
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
                return;
            }
            App.QuitGame();
        }

    }

    public class SummaryUserInfo
    {
        public int Id;

        public int Seat;

        public string Nick;

        public int Gold;

        public short SexI;

        /// <summary>
        /// 小分
        /// </summary>
        public int[] Record;

        public string AvatarX;
    }
}