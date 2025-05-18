using System;
using System.Collections.Generic;
using Assets.Scripts.Game.sss.ImgPress;
using Assets.Scripts.Game.sss.ImgPress.Main;
using Assets.Scripts.Game.sss.Tool;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.sss
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
        public UIGrid ScoreGrid;

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

        public SumUserInfo[] SumUsers;

        public UILabel[] SumLabels;

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
                App.GetGameManager<SssGameManager>().SettingMenu.HistoryView.SetActive(true);
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

            RoomInfo roomInfo = App.GetGameManager<SssGameManager>().RoomInfo;
            GameInfoLabel.text = string.Format("房间号:{0} 十三水-{1}局-好友同玩", roomInfo.RoomID, roomInfo.MaxRound);
        }


        void InitUsersInfo(ISFSObject data)
        {
            ISFSArray users = data.GetSFSArray("users");
            int index = 0;
            foreach (ISFSObject user in users)
            {
                if (user.GetInt("id") > 0)
                {
                    SumUsers[index].Init(user);
                    SetLabel(SumLabels[index], user.GetInt("gold"));
                }
                ++index;
            }

            ScoreGrid.hideInactive = true;
            ScoreGrid.Reposition();
            UserInfoGrid.hideInactive = true;
            UserInfoGrid.Reposition();
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
            if (sfsObj.ContainsKey("record"))
            {
                int round = sfsObj.GetIntArray("record").Length;

                for (int i = 0; i < round; i++)
                {
                    List<int> scoreList = new List<int>();
                    for (int j = 0; j < SumUsers.Length; j++)
                    {
                        if (SumUsers[j].Id > 0 && SumUsers[j].ScoreArray != null)
                            scoreList.Add(SumUsers[j].ScoreArray[i]);
                    }
                    GameObject go = Instantiate(ItemPrefab);
                    go.transform.parent = SumItemGrid.transform;
                    go.transform.localScale = Vector3.one;
                    go.GetComponent<SummaryItem>().InitItem(i + 1, scoreList.ToArray());
                }
            }

            ScoreGrid.hideInactive = true;
            ScoreGrid.Reposition();
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


        /// <summary>
        /// 点击分享战绩按钮,可复用
        /// </summary>
        public void OnClickShare()
        {
            YxWindowManager.ShowWaitFor();

            Facade.Instance<WeChatApi>().InitWechat();

            CompressImg img = GetComponent<CompressImg>() ?? gameObject.AddComponent<CompressImg>();

            UserController.Instance.GetShareInfo(info =>
            {
                YxWindowManager.HideWaitFor();
                img.DoScreenShot(new Rect(0, 0, Screen.width, Screen.height), imageUrl =>
                {
                    if (Application.platform == RuntimePlatform.Android)
                    {
                        imageUrl = "file://" + imageUrl;
                    }
                    info.ImageUrl = imageUrl;
                    info.ShareType = ShareType.Image;
                    Facade.Instance<WeChatApi>().ShareContent(info, str =>
                    {
                        Dictionary<string, object> parm = new Dictionary<string, object>()
                        {
                            {"option",2},
                            {"bundle_id",Application.bundleIdentifier},
                            {"share_plat",ChatShareType.ToString()},
                        };
                        Facade.Instance<TwManager>().SendAction("shareAwards", parm, null);
                    });
                });
            });
        }

        /// <summary>
        /// 点击返回大厅按钮
        /// </summary>
        public void OnClickBackBtn()
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
        }

    }
}