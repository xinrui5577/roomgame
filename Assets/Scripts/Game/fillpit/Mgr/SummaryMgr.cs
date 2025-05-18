using UnityEngine;
using System.Collections;
using YxFramwork.View;
using YxFramwork.Common;
using Sfs2X.Entities.Data;
using YxFramwork.Tool;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Controller;
using System.Collections.Generic;
using Assets.Scripts.Game.fillpit.ImgPress.Main;
using com.yxixia.utile.YxDebug;

// ReSharper disable FieldCanBeMadeReadOnly.Local


namespace Assets.Scripts.Game.fillpit.Mgr
{
    public class SummaryMgr : MonoBehaviour
    {
        /// <summary>
        /// 小结窗口对象
        /// </summary>
        [SerializeField]
        private GameObject _briefSum = null;

        /// <summary>
        /// 小结窗口显示玩家信息的成员数组
        /// </summary>
        [SerializeField]
        private GameObject[] _sumItems = null;

        /// <summary>
        /// 小结窗口的Grid
        /// </summary>
        [SerializeField]
        private UIGrid _sumGrid = null;

        /// <summary>
        /// 继续游戏按钮
        /// </summary>
        [SerializeField]
        private UIButton _continueBtn = null;

        /// <summary>
        /// 返回大厅按钮
        /// </summary>
        [SerializeField]
        private UIButton _backBtn = null;

        /// <summary>
        /// 查看战绩按钮
        /// </summary>
        [SerializeField]
        private UIButton _viewRecordBtn = null;

        /// <summary>
        /// 按钮Grid
        /// </summary>
        [SerializeField]
        private UIGrid _btnsGrid = null;

        /// <summary>
        /// 喜分
        /// </summary>
        [SerializeField]
        private HappyScore _happyScore;

        /// <summary>
        /// 显示结算界面倒计时,如果动画未能正确播放的保险措施
        /// </summary>
        [HideInInspector]
        public float ResultTimer = 2f;

        /// <summary>
        /// 是否显示结算界面
        /// </summary>
        [HideInInspector]
        public bool ShowTurnResult = true;
        /// <summary>
        /// 是否正在显示小结算
        /// </summary>
        private bool isShowGameResult;

        /// <summary>
        /// 小结算
        /// </summary>
        /// <param name="dataArray"></param>
        public void OnGameResult(ISFSArray dataArray)
        {
            //如果允许显示小结算
            isShowGameResult = ShowTurnResult;
            InitBriefSum(dataArray);
            SetHappyScore();
            var gdata = App.GetGameData<FillpitGameData>();
            ResultTimer = gdata.IsLanDi && gdata.IsRoomGame ? 2.0f : 5f;
            StartCoroutine(ResultCd(dataArray));
        }

        private void SetHappyScore()
        {
            if (_happyScore == null) return;
            _happyScore.SetScore();
        }

        /// <summary>
        /// 在一段时间后,自动显示结算界面,防止动画未能正常播放引起的错误
        /// </summary>
        /// <returns></returns>
        IEnumerator ResultCd(ISFSArray dataArray)
        {
            var mgr = App.GetGameManager<FillpitGameManager>();
            var gdata = App.GetGameData<FillpitGameData>();
            while (mgr.DealerMgr.DealCards != null && mgr.DealerMgr.DealCards.Count > 0)
            {
                yield return new WaitForSeconds(0.1f);
            }
            foreach (ISFSObject data in dataArray)
            {
                int seat = data.GetInt("seat");
                int[] cards = data.GetIntArray("cards");
                var betPoker = gdata.GetPlayer<PlayerPanel>(seat,true).UserBetPoker;
                betPoker.TurnOverCard(cards);
            }
            _finishAnim = true;

            yield return new WaitForSeconds(ResultTimer);

            CouldShowBriefSum = true;
            ShowBriefSum();
        }

        internal bool CouldShowBriefSum;

        private bool _finishAnim;

        /// <summary>
        /// 初始化和显示小结窗口内容
        /// </summary>
        /// <param name="dataArray">小结数据中的玩家数组信息</param>
        private void InitBriefSum(ISFSArray dataArray)
        {
           
            //用于记录有多少个玩家，同时作为数组索引使用 
            int count = dataArray.Count;
           
            //显示玩家信息,隐藏不存在的玩家成员
            for (int i = 0; i < _sumItems.Length; i++)
            {
                _sumItems[i].SetActive(i < count);
            }

            for (int i = 0; i < count; i++)
            {
                SumItem si = _sumItems[i].GetComponent<SumItem>();

                si.InitSumItem(dataArray.GetSFSObject(i));
            }
            InitBtns();
        }

        /// <summary>
        /// 设置按钮的显示
        /// </summary>
        public void InitBtns()
        {
            //设置按钮
            //如果不是最后一局,隐藏查看战绩,显示继续游戏
            _continueBtn.gameObject.SetActive(true);
            _viewRecordBtn.gameObject.SetActive(false);
         
            //开房模式不能中途退出
            _backBtn.gameObject.SetActive(!App.GetGameData<FillpitGameData>().IsRoomGame);

            _btnsGrid.repositionNow = true;
            _btnsGrid.Reposition();
        }

        /// <summary>
        /// 显示小结窗口
        /// </summary>
        public void ShowBriefSum()
        {
            if (!CouldShowBriefSum || !_finishAnim)
                return;

            if (ShowTurnResult)
            {
                //显示小结窗口
                _briefSum.SetActive(true);

                _btnsGrid.hideInactive = true;
                _btnsGrid.Reposition();

                //重置布局
                _sumGrid.hideInactive = true;
                _sumGrid.Reposition();
            }
            
            var gdata = App.GetGameData<FillpitGameData>();
            bool landi = gdata.IsLanDi && gdata.IsRoomGame;
            App.GetGameManager<FillpitGameManager>().Reset(landi);
        }

        public SummaryMgr(HappyScore happyScore)
        {
            _happyScore = happyScore;
        }

        //-------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------

        /// <summary>
        /// 总战绩窗口对象
        /// </summary>
        [SerializeField]
        private GameObject _accountObj = null;

        [SerializeField]
        private UIGrid _accountGrid = null;

        /// <summary>
        /// 总结算的成员
        /// </summary>
        [SerializeField]
        private AccItem[] _accItems = null;

        /// <summary>
        /// 游戏结束
        /// </summary>
        [SerializeField]
        public bool GameOver;
       

        /// <summary>
        /// 处理和显示总结算信息
        /// </summary>
        /// <param name="dataArray"></param>
        public void InitAccount(ISFSArray dataArray)
        {
            //用于记录有多少个玩家，同时作为数组索引使用 
            int count = 0;
            int maxPoint = 0;           //本局最高分点数

            //对小结窗口玩家信息进行初始化
            foreach (ISFSObject data in dataArray)
            {
                if (data.ContainsKey("seat"))
                {
                    AccItem acc = _accItems[count++].GetComponent<AccItem>();
                    acc.InitAccItem(data);
                    int point = data.GetInt("gold");        //当前玩家的得分数
                    if (point > maxPoint)
                    {
                        maxPoint = point;
                    }
                }
            }

            //显示大赢家标记
            for (int i = 0; i < _accItems.Length; i++)
            {
                _accItems[i].SetBigWinnerMark(_accItems[i].PlayerScore >= maxPoint);
                _accItems[i].gameObject.SetActive(i < count);       //有数据的显示,没有数据的隐藏
            }
            _accountGrid.Reposition();
        }

        /// <summary>
        /// 通过当前局数,判定是投票还是满局
        /// </summary>
        /// <param name="round">由服务器获取的当前局数</param>
        public void SetAccountActive()
        {
            //bool isDismissRoom = round < App.GetGameManager<FillpitGameManager>().RoomInfo.MaxRound
            //                     || App.GetGameData<FillpitGameData>().IsGameing;
            //_briefSum.SetActive(!isDismissRoom && ShowTurnResult);
            //如果小于最大局数,或者在游戏中,说明是投票结束
            if (!isShowGameResult)
            {
                _accountObj.SetActive(true);
            }
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


        /// <summary>
        /// 点击继续按钮
        /// </summary>
        public void OnClickContinueBtn()
        {
            var gdata = App.GetGameData<FillpitGameData>();

            HideBriefSum();
            if (GameOver)
            {
                SetAccountActive();
            }

            //开房模式自动准备
            if (!gdata.IsRoomGame) return;
            if (!gdata.IsGameing && !GameOver)
            {
                App.GetRServer<FillpitGameServer>().ReadyGame();
            }
        }


        /// <summary>
        /// 点击查看战绩按钮
        /// </summary>
        public void OnCLickCheckSum()
        {
            isShowGameResult = false;
            _briefSum.SetActive(false);
            _accountObj.SetActive(true);
        }

        private string _screenshot;

        public void OnClickShare()
        {
            _screenshot = App.UI.CaptureScreenshot();
            Invoke("CaptureScreenshot", 1f);
        }   

        /// <summary>
        /// 是否有分享奖励
        /// </summary>
        public bool HaveRequest = false;

        void CaptureScreenshot()
        {
            Facade.Instance<WeChatApi>().InitWechat(App.Config.WxAppId);
            var dic = new Dictionary<string, object>();
            dic["type"] = 0;
            dic["game_key_c"] = App.GameKey;
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


        public void HideBriefSum()
        {
            isShowGameResult = false;
            _briefSum.SetActive(false);
        }

        public HappyScore TotalHappyScoreLabel;

        public void OnGameOver(ISFSObject data)
        {
            GameOver = true;
            InitAccount(data.GetSFSArray("users"));
            SetAccountActive();
            SetTotalHappyScore(data);
            InitBtns();
        }

        private void SetTotalHappyScore(ISFSObject data)
        {
            if (TotalHappyScoreLabel == null)
                return;

            if (!data.ContainsKey("curHappyS"))
                return;

            int score = data.GetInt("curHappyS");
            TotalHappyScoreLabel.ShowScoreLabel(score);
        }


        public void Reset()
        {
            StopAllCoroutines();
            _finishAnim = false;
            CouldShowBriefSum = false;
        }
    }
}