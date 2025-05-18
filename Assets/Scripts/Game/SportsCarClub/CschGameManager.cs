using UnityEngine;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;
using YxFramwork.Manager;
using YxFramwork.View;
using Sfs2X.Entities.Data;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class CschGameManager : YxGameManager
    {
        /// <summary>
        /// 是否可以退出
        /// </summary>
        public bool CanQuit = true;
        /// <summary>
        /// 
        /// </summary>
        public UIToggle SoundToggle;
        public RightBottomManager RightBottomMgr;
        public BankerManager BankerMgr;
        // Use this for initialization
        protected override void OnAwake()
        {
            transform.GetComponent<UIRoot>().scalingStyle = (Application.platform == RuntimePlatform.Android) ? UIRoot.Scaling.ConstrainedOnMobiles : UIRoot.Scaling.Constrained;
        }

        protected override void OnStart()
        {
            if (SoundToggle != null) SoundToggle.startsActive = true;
            RegionsDic = new Dictionary<BetType, BetRegion>();
            foreach (var betRegion in Regions)
            {
                RegionsDic.Add(betRegion.CurBetType, betRegion);
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateCountNum();

            if (Input.GetKey(KeyCode.Escape))
            {
                OnQuitGame();
            }
        }

        #region 下注

        /// <summary>
        /// 下注区域
        /// </summary>
        public BetRegion[] Regions;

        /// <summary>
        /// 下注区域key为p
        /// </summary>
        [HideInInspector]
        public Dictionary<BetType, BetRegion> RegionsDic;
        /// <summary>
        /// 下注
        /// </summary>
        /// <param name="gold"></param>
        /// <param name="p"></param>
        /// <param name="seat"></param>
        public void AddBet(int gold, int p, int seat, bool isLastTime = false)
        {

            if (gold <= 0 || p >= Regions.Length)
            {
                return;
            }
            var bet = BetManager.GetInstance().GetBet(-1, gold);
            var self = App.GameData.GetPlayerInfo();
            if (seat == self.Seat)
            {
                var gdata = App.GetGameData<CschGameData>();
                CanQuit = false;
                gdata.GetPlayer().Coin -= gold;
                RegionsDic[(BetType)p].AddBet(bet, seat, gold,true);
                //UserManager.GetInstance().RefreshUserInfo();
                //App.GameData.GetPlayer().UpdateView();
            }
            else
                RegionsDic[(BetType)p].AddBet(bet, seat, gold);

            Facade.Instance<MusicManager>().Play("Bet");
            GetUpBetValue(seat, isLastTime, p, gold);

            //重复上轮相关  --将函数封装起来了...
            /*if (seat == self.Seat && !isLastTime)
            {
                //重复上轮的数据更新
                if (RightBottomMgr.UpValue.Count == 0)
                {
                    RightBottomMgr.LastTimeNum = RightBottomMgr.CurNum;
                    RightBottomMgr.UpValue = new List<int>();
                    for (var i = 0; i < Regions.Length; i++)
                    {
                        RightBottomMgr.UpValue.Add(0);
                    }
                }
                //刷新重复上轮数据
                if (RightBottomMgr.LastTimeNum != RightBottomMgr.CurNum)
                {
                    RightBottomMgr.LastTimeNum = RightBottomMgr.CurNum;
                    for (int i = 0; i < Regions.Length; i++)
                    {
                        RightBottomMgr.UpValue[i] = 0;
                    }
                }

                RightBottomMgr.UpValue[p] += gold;

            }*/

            var bestUp = 0;
            var bestIUp = 0;
            var bestDown = 0;
            var bestIDown = 0;
            for (var i = 0; i < RegionsDic.Count - 2; i++)
            {
                if (i < 4)
                {
                    if (bestUp <= RegionsDic[(BetType)i].TotalGold * RegionsDic[(BetType)i].Rate)
                    {
                        bestUp = RegionsDic[(BetType)i].TotalGold * RegionsDic[(BetType)i].Rate;
                        bestIUp = i;
                    }
                }
                else
                {
                    if (bestDown <= RegionsDic[(BetType)i].TotalGold * RegionsDic[(BetType)i].Rate)
                    {
                        bestDown = RegionsDic[(BetType)i].TotalGold * RegionsDic[(BetType)i].Rate;
                        bestIDown = i;
                    }
                }
            }


            for (var i = 0; i < RegionsDic.Count; i++)
            {
                var best = i < 4 ? bestUp : bestDown;
                var bestI = i < 4 ? bestIUp : bestIDown;

                var all = 0;

                all += (int)BankerMgr.Banker.Coin;

                var can = 0;
                if (i > 7)
                {
                    for (var j = 0; j < RegionsDic.Count; j++)
                    {
                        if (i == j || i == bestI)
                        {
                            continue;
                        }
                        all += RegionsDic[(BetType)j].TotalGold;
                    }

                    var len = i == 7 ? 4 : 8;
                    var betV = RegionsDic[(BetType)i].TotalGold * RegionsDic[(BetType)i].Rate +
                               best;

                    can = (int)((all - betV) / RegionsDic[(BetType)i].Rate);
                }
                else
                {

                    for (var j = 0; j < RegionsDic.Count; j++)
                    {
                        if (i == j || (i < 4 && j == 8) || (i > 4 && j == 9))
                        {
                            continue;
                        }

                        all += RegionsDic[(BetType)j].TotalGold;
                    }

                    var doubleIndex = i < 4 ? 8 : 9;
                    var betV = RegionsDic[(BetType)i].TotalGold * RegionsDic[(BetType)i].Rate +
                               RegionsDic[(BetType)doubleIndex].TotalGold * RegionsDic[(BetType)doubleIndex].Rate;

                    can = (int)((all - betV) / RegionsDic[(BetType)i].Rate);
                }
                RegionsDic[(BetType)i].CanBet = can <= 0 ? 0 : can;
                RegionsDic[(BetType)i].SetCanBetLabel(RegionsDic[(BetType)i].CanBet);
            }

        }

        /// <summary>
        /// 刷新重复上轮数据 => 不在之前的位置调用了
        /// </summary>
        public void RefreshRepeatData(int[] betData)
        {
            if (betData.Length != RightBottomMgr.UpValue.Count)
                return;

            //重复上轮的数据更新
            if (RightBottomMgr.UpValue.Count == 0)
            {
                RightBottomMgr.LastTimeNum = RightBottomMgr.CurNum;
                RightBottomMgr.UpValue = new List<int>();
                for (var i = 0; i < Regions.Length; i++)
                {
                    RightBottomMgr.UpValue.Add(0);
                }
            }
            //刷新重复上轮数据
            if (RightBottomMgr.LastTimeNum != RightBottomMgr.CurNum)
            {
                RightBottomMgr.LastTimeNum = RightBottomMgr.CurNum;
                for (int i = 0; i < Regions.Length; i++)
                {
                    RightBottomMgr.UpValue[i] = 0;
                }
            }

            for (int i = 0; i < betData.Length; i++)
            {
                RightBottomMgr.UpValue[i] += betData[i];
            }
        }

        /// <summary>
        /// 等待下局遮挡
        /// </summary>
        public GameObject Loading;

        /// <summary>
        /// 检测是否可以下注
        /// </summary>
        public void CheckBeginBet(ISFSObject gi)
        {
            long st = gi.GetLong("st");
            long ct = gi.GetLong("ct");
            BetManager.GetInstance().IsBeginBet = false;

            if (st != 0)
            {
                if (ct - st < 14)
                {
                    //BetManager.GetInstance().IsBeginBet = true;.
                }
                CountNum.gameObject.SetActive(false);
                InGameLogo.gameObject.SetActive(true);
                //todo 画筹码
            }
        }

        /// <summary>
        /// 清除所有筹码
        /// </summary>
        public void ClearAllBet()
        {
            for (int i = 0; i < Regions.Length; i++)
            {
                Regions[i].ClearBet();
                Regions[i].StopLuckAnim();
            }
        }
        [HideInInspector]
        public bool Execute = false;


        /// <summary>
        /// 获得上次的每个区域的下注值
        /// </summary>
        /// <param name="seat"></param>
        /// <param name="isLastTime"></param>
        /// <param name="betPos"></param>
        /// <param name="betGold"></param>
        public void GetUpBetValue(int seat, bool isLastTime, int betPos, int betGold)
        {
            if (seat == App.GameData.SelfSeat)
            {
                Execute = true;
                RightBottomMgr.UpBetValue[betPos] += betGold;
                YxDebug.LogError("下注的区域" + betPos);
            }
        }

        #endregion

        #region 开奖

        /// <summary>
        /// 无参代理
        /// </summary>
        public delegate void NoParamDelegate();

        /// <summary>
        /// 中奖下标
        /// </summary>
        public int LuckIndex = 0;

        public void OnDrawFinish()
        {
            int betint = LuckIndex % Wheel.GetInstance().ItemsValue.Length;
            BetType betT = (BetType)betint;
            BetType doubleT = BetType.BMFP;
            if ((int)betT < 4)
                doubleT = (BetType)8;
            else
                doubleT = (BetType)9;

            RegionsDic[betT].PlayLuckAnim();

            RegionsDic[doubleT].PlayLuckAnim();
        }

        #endregion

        #region 游戏状态

        /// <summary>
        /// 倒计时对象
        /// </summary>
        public UILabel CountNum;
        /// <summary>
        /// 正在游戏中
        /// </summary>
        public GameObject InGameLogo;
        /// <summary>
        /// 下注CD时间
        /// </summary>
        public int CdTime;

        /// <summary>
        /// 开始倒计时
        /// </summary>
        public void StartCountNum()
        {
            //            Facade.Instance<MusicManager>().PlayBacksound("BeginBet");
            _curCdTime = CdTime;
            _isCountNum = true;
            _startTime = (int)Time.time;
            CountNum.text = _curCdTime.ToString();
            CountNum.gameObject.SetActive(true);
            InGameLogo.SetActive(false);
        }

        private int _curCdTime;
        /// <summary>
        /// 是否倒计时
        /// </summary>
        private bool _isCountNum;
        /// <summary>
        /// 开始倒计时的时间
        /// </summary>
        private int _startTime;
        /// <summary>
        /// 倒计时
        /// </summary>
        public void UpdateCountNum()
        {
            if (_isCountNum && (Time.time - _startTime) - (CdTime - _curCdTime) > 1f)
            {
                if (_curCdTime <= 0)
                {
                    EndBet();
                    return;
                }
                _curCdTime--;
                if (_curCdTime <= 3)
                {
                    Facade.Instance<MusicManager>().Play("CountNum");
                }
                CountNum.text = _curCdTime.ToString();
            }
        }

        public void EndBet()
        {
            Facade.Instance<MusicManager>().Play("Zero");
            _isCountNum = false;
            CountNum.gameObject.SetActive(false);
            InGameLogo.SetActive(true);

            BetManager.GetInstance().IsBeginBet = false;
        }

        public void OnQuitGame()
        {
            bool isBanker = BankerMgr.IsBanker(App.GameData.SelfSeat);
            if (isBanker)
            {
                YxMessageBox.Show(new YxMessageBoxData
                {
                    Msg = "您现在是庄家,无法离开游戏!",
                    Delayed = 4
                });
            }
            else if (!CanQuit)
            {
                YxMessageBox.Show("请等待本局游戏结束后在退出!", 4);
            }
            else if (CanQuit)
            {
                YxMessageBox.Show("确定要退出游戏么？", "", (box, btnName) =>
                 {
                     if (btnName == YxMessageBox.BtnLeft)
                     {
                         App.QuitGame();
                     }
                 }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
            }

        }

        #endregion

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            YxDebug.Log(">>>>> 加入房间成功 <<<<< OnGetGameInfo");
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            CheckBeginBet(gameInfo);
            BetManager.GetInstance().CheckBets();
            CdTime = gameInfo.ContainsKey("cd") ? gameInfo.GetInt("cd") : 10;
            if (BankerMgr.IsBanker(-1))
            {
                BankerMgr.SetBankerInfo(null);
            }
            BankerMgr.RefreshBankerList(gameInfo.GetSFSArray("bankers"), gameInfo.GetInt("banker"));
            BankerMgr.RefreshPlayerList(gameInfo.GetSFSArray("users"));
            //            UserManager.GetInstance().RefreshUserInfo(gameInfo.GetSFSObject("user"));
            RightBottomMgr.BankerLimit = gameInfo.GetInt("bankLimit");
            RightBottomMgr.RefreshLastTimeBtn();
            //历史记录面板初始化
            HistoryRecord.GetInstance().InitHistory(gameInfo.GetIntArray("history"), gameInfo.GetInt("hisIdx"), gameInfo.GetIntArray("winTimes"));
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            YxDebug.Log("Request == " + (RequestType)type);
            if (response.ContainsKey("playerlist"))
            {
                BankerMgr.RefreshPlayerList(response.GetUtfStringArray("playerlist"));
            }

            switch ((RequestType)type)
            {
                case RequestType.Bet:
                    if (response.ContainsKey("golds"))  // -- 统一走else逻辑
                    {
                        YxDebug.Log("收到重复上轮");
                        var golds = response.GetIntArray("golds");
                        var seat = response.GetInt("seat");
                        if (seat == App.GameData.SelfSeat)
                        {
                            RightBottomMgr.LastTime.isEnabled = true;
                        }

                        for (int i = 0; i < golds.Length; i++)
                        {
                            AddBet(golds[i], i, seat, true);
                        }
                    }
                    else
                    {
                        int p = response.GetInt("p");
                        int gold = response.GetInt("gold");
                        int seat = response.GetInt("seat");
                        AddBet(gold, p, seat);
                    }
                    break;
                case RequestType.Reward:
                    break;
                case RequestType.ApplyBanker:
                    break;
                case RequestType.ApplyQuit:
                    break;
                case RequestType.BeginBet:
                    CanQuit = true;
                    Loading.SetActive(false);
                    RightBottomMgr.CurNum++;
                    ResultManager.GetInstance().CloseResult();
                    ClearAllBet();
                    BetManager.GetInstance().IsBeginBet = true;
                    StartCountNum();
                    Wheel.GetInstance().Selected.GetComponent<TweenAlpha>().enabled = false;
                    Wheel.GetInstance().Selected.GetComponent<TweenAlpha>().value =
                        Wheel.GetInstance().Selected.GetComponent<TweenAlpha>().from;

                    break;
                case RequestType.EndBet:
                    Loading.SetActive(false);
                    BetManager.GetInstance().IsBeginBet = false;
                    //开始摇奖
                    Wheel.GetInstance().StartTrun();

                    RefreshRepeatData(RightBottomMgr.UpBetValue.ToArray());

                    break;
                case RequestType.GiveCards:
                    int carIndex = response.GetInt("carInx");

                    //针对超过20的服务器反馈进行处理
                    if (carIndex >= 20 && Wheel.GetInstance().Items.Length <= 20)  //=>条件2考虑兼容问题
                        carIndex = carIndex % 8;

                    LuckIndex = carIndex;
                    Wheel.GetInstance().StopTrun(carIndex, OnDrawFinish);
                    break;
                case RequestType.Result:
                    CanQuit = true;
                    Loading.SetActive(false);
                    RightBottomMgr.RefreshLastTimeBtn();
                    if (!BankerMgr.IsBanker(-1))
                    {
                        BankerMgr.Banker.WinTotalCoin += response.GetInt("bankWin");
                        var bankerCoin = "￥" + YxUtiles.ReduceNumber(BankerMgr.Banker.WinTotalCoin);
                        BankerMgr.Banker.WinTotalCoinLabel.Text(bankerCoin);
                    }
                    var self = App.GameData.GetPlayer();
                    self.Coin = response.GetLong("total");
                    self.WinTotalCoin += response.GetInt("win");

                    YxDebug.Log("---------结算-----------");
                    ResultManager.GetInstance().OpenResult(response);
                    HistoryRecord.GetInstance().RefreshData(response.GetInt("carInx"), 1);
                    //YxDebug.Log("result");
                    break;
                case RequestType.BankerList:
                    //UserManager.GetInstance().BankerSeat = response.GetInt("banker");
                    if (response.GetInt("banker") == -1)
                    {
                        BankerMgr.SetBankerInfo(null);
                    }
                    BankerMgr.RefreshBankerList(response.GetSFSArray("bankers"), response.GetInt("banker"));
                    break;
                default:
                    YxDebug.Log("不存在的服务器交互!");
                    break;
            }
        }

        public override void UserOut(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserIdle(int localSeat, ISFSObject responseData)
        {
        }

        public override void UserOnLine(int localSeat, ISFSObject responseData)
        {
        }

        /// <summary>
        /// 声音控制
        /// </summary>
        /// <param name="isFocus"></param>
        /// <param name="state1"></param>
        /// <param name="state2"></param>
        public void OnSoundClick(bool isFocus, GameObject state1, GameObject state2)
        {
            var musicMgr = Facade.Instance<MusicManager>();
            if (isFocus)
            {
                musicMgr.EffectVolume = 1;
                musicMgr.MusicVolume = 1;
                state1.SetActive(true);
                state2.SetActive(false);
            }
            else
            {
                musicMgr.EffectVolume = 0;
                musicMgr.MusicVolume = 0;
                state1.SetActive(false);
                state2.SetActive(true);
            }
        }


        /// <summary>
        /// 打开战绩
        /// </summary>
        public void OnRecordClick()
        {
            RecordCtrl.GetInstance().CtrlShowPanel();
        }
    }

    /// <summary>
    /// 列表类型
    /// </summary>
    public enum ListType
    {
        None,
        Banker,
        Player,
    }
}