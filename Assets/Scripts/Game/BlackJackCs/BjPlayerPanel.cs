using System;
using System.Collections.Generic;
using Assets.Scripts.Game.BlackJackCs.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Tool;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.BlackJackCs
{
    public class BjPlayerPanel : YxBaseGamePlayer
    {

        #region 私有变量

        /// <summary>
        /// 点数显示
        /// </summary>
        [SerializeField] protected UILabel PointLabel;


        /// <summary>
        /// 手牌父层Grid
        /// </summary>
        //[SerializeField] private UIGrid _pokerParent = null;

        /// <summary>
        /// 玩家下筹码区域
        /// </summary>
        [SerializeField] private Transform _betArea = null;

        /// <summary>
        /// 显示状态,爆牌/黑杰克
        /// </summary>
        [SerializeField] protected StateMark StateMark;

        /// <summary>
        /// 显示状态,是否买保险
        /// </summary>
        [SerializeField] private InsuranceMark _insureranceMark = null;

        /// <summary>
        /// 显示状态,是否是双倍
        /// </summary>
        [SerializeField]
        private DoubleMark _doubleMark = null;

        /// <summary>
        /// 扑克的位置
        /// </summary>
        [SerializeField] protected Transform[] PokerPos;

        /// <summary>
        /// 倒计时
        /// </summary>
        [SerializeField] protected UISprite CountDown;

        /// <summary>
        /// 准备按钮
        /// </summary>
        public UIButton ReadyBtn;

        /// <summary>
        /// 准备图标
        /// </summary>
        [SerializeField]
        public GameObject ReadyMark;

        /// <summary>
        /// 总下注显示lable
        /// </summary>
        [SerializeField]
        private UILabel _allBetLabel = null;

        /// <summary>
        /// 总下注筹码的父层级
        /// </summary>
        [SerializeField]
        private GameObject _allBetObj = null;

        /// <summary>
        /// 牌的个数
        /// </summary>
        protected int OnesPokerCount;



        /// <summary>
        /// 记录手牌点数
        /// </summary>
        protected int HandCardPoint;

        /// <summary>
        /// 是否有A
        /// </summary>
        protected bool HaveAce;

        #endregion


        #region 公共变量

        [HideInInspector]
        public Transform[] PokersTrans = null;

        /// <summary>
        /// 记录玩家座位信息
        /// </summary>
        [HideInInspector]
        public int Seat;

        //public UILabel NikeNameLabel;

        /// <summary>
        /// 预先存储牌的值
        /// </summary>
        [HideInInspector]
        public int[] CardsId = new int[5];      //防止发牌未发到手上,翻牌报错

        /// <summary>
        /// 下注金额
        /// </summary>
        private int _betMoney;

        /// <summary>
        /// 总下注金额
        /// </summary>
        public int BetMoney
        {
            set
            {
                _betMoney = value;
                _allBetLabel.text = YxUtiles.ReduceNumber(_betMoney);
                _allBetObj.gameObject.SetActive(_betMoney > 0);
            }
            get { return _betMoney; }
        }
        

        public Transform GetPokerTran
        {
            get
            {
                OnesPokerCount = OnesPokerCount > PokerPos.Length - 1 ? PokerPos.Length - 1 : OnesPokerCount;
                PokerPos[OnesPokerCount].gameObject.SetActive(true);
                PokerPos[OnesPokerCount].localPosition = new Vector3(32 * OnesPokerCount, 0, 0);
                return PokerPos[OnesPokerCount++]; 
            }
        }


        /// <summary>
        /// 手牌个数(不是索引)
        /// </summary>
        public int PokerCount
        {
            set { OnesPokerCount = value; }
            get { return OnesPokerCount > PokerPos.Length -1 ? PokerPos.Length -1 : OnesPokerCount; }
        }

        #endregion

    

        // Update is called once per frame
        protected void Update()
        {
            CountDownUpdate();
        }
   
        /// <summary>
        /// 设置手牌点数
        /// </summary>
        protected void SetHandCardPoint()
        {
            HandCardPoint = 0;
            for(int i = 0;i<CardsId.Length;i++)
            {
                if (CardsId[i] == 0)
                    continue;

                int p = CardsId[i] % 16;
                if (p == 1)
                {
                    HaveAce = true;
                }
                p = p > 10 ? 10 : p;
                HandCardPoint += p;
            }
        }


        /// <summary>
        /// 检测牌的点数,并显示,如果是black,直接显示
        /// </summary>
        public virtual void CheckCardPoint()
        {

            SetHandCardPoint();

            if (HandCardPoint == 0)
                return;
            if (HandCardPoint > 21)
            {
                StateMark.ShowLostMark();
                ShowPoint(HandCardPoint);
                Invoke("ThrowPokersWithAnim", 2f);

                return;
            }


            //当玩家有A且没有超过21点时,可能出现两个数字
            if (HaveAce)
            {
                if (HandCardPoint + 10 == 21)
                {
                    if (OnesPokerCount == 2)
                    {
                        StateMark.ShowBalckJackMark();
                    }

                    ShowPoint(21);
                }
                else if (HandCardPoint + 10 < 21)
                {
                    ShowPoint(HandCardPoint, HandCardPoint + 10);
                }
                else
                {
                    ShowPoint(HandCardPoint);
                }
            }
            else
            {
                ShowPoint(HandCardPoint);
            }
        }


        public void ThrowPokersWithAnim()
        {
            CancelInvoke("ThrowPokersWithAnim");
            StateMark.HideLostMark();      //隐藏输牌标记
            foreach (Transform pos in PokerPos)
            {
                if (pos.childCount <= 0)
                    continue;

                App.GetGameManager<BlackJackGameManager>().DealPokerMgr.HidePokerWithAnim(pos.GetChild(0).gameObject);
                pos.DestroyChildren();
                pos.gameObject.SetActive(false);
            }
            OnesPokerCount = 0;
            PointLabel.gameObject.SetActive(false);
            BetMoney = 0;
            
            foreach(var bet in _bets)
            {
                Destroy(bet.gameObject);
            }
            _bets.Clear();

        }
        

        /// <summary>
        /// 显示点数
        /// </summary>
        /// <param name="point1">当前点数</param>
        /// <param name="point2">有A时,且不超过21时的点数</param>
        public void ShowPoint(int point1,int point2 = 0)
        {

            PointLabel.text = point1.ToString();

            if (point1 == 21)
            {
                PointLabel.gradientTop = Tools.ChangeToColor(0xFFFF00);
                PointLabel.gradientBottom = Tools.ChangeToColor(0xFF9600);
                PointLabel.transform.localScale = Vector3.one * 1.5f;

            }
            else
            {
                PointLabel.gradientTop = Tools.ChangeToColor(0xFFFFCE);
                PointLabel.gradientBottom = Tools.ChangeToColor(0xFDB54D);
                PointLabel.transform.localScale = Vector3.one;
            }

            if (point2 > 0)
            {
                PointLabel.text += "/" + point2;
            }
            PointLabel.gameObject.SetActive(true);

        }


        #region 玩家说话倒计时功能


        /// <summary>
        /// 倒计时读秒
        /// </summary>
        private float _readSecond;
        /// <summary>
        /// 倒计时每秒走的总长
        /// </summary>
        private float _cdTime;
        /// <summary>
        /// 倒计时每秒颜色走的总长
        /// </summary>
        private float _cdColorTime;
        /// <summary>
        /// 是否开始倒计时
        /// </summary>
        private bool _isCountDown;

        /// <summary>
        /// 游戏计时是否循环
        /// </summary>
        bool _isCdLoop;

        /// <summary>
        /// 计时时间
        /// </summary>
        float _commonCDTime = 15f;

        /// <summary>
        /// 开始倒计时
        /// </summary>
        public void BeginCountDown(float time)
        {
            if (CountDown == null)
                return;
            if (time <= 0)
            {
                YxDebug.LogError("倒计时时间不能小于等于0!!");
                return;
            }
         
            if (time > 150f && App.GetGameData<BlackJackGameData>().IsRoomGame)
            {
                _isCdLoop = true;
                time = _commonCDTime;
            }
           

            CountDown.fillAmount = 1f;
            CountDown.color = Color.green;
            CountDown.gameObject.SetActive(true);
            _cdTime = 1f / time;
            _cdColorTime = 2f / time;
            _readSecond = 0f;
            _isCountDown = true;

            CountDown.gameObject.SetActive(true);
        }


        /// <summary>
        /// 倒数update
        /// </summary>
        private void CountDownUpdate()
        {
            if (!_isCountDown)
            {
                return;
            }

            CountDown.fillAmount -= _cdTime * Time.deltaTime;
            CountDown.fillAmount = CountDown.fillAmount < 0 ? 0 : CountDown.fillAmount;

            if (Math.Abs(CountDown.color.r - 1f) > 0.001f)
            {
                float r = (CountDown.color.r + _cdColorTime * Time.deltaTime) >= 1f
                    ? 1f
                    : CountDown.color.r + _cdColorTime * Time.deltaTime;
                CountDown.color = new Color(r, 1f, 0f);
                if (Math.Abs(CountDown.color.r - 1f) < 0.001f && Info.Seat == App.GameData.SelfSeat)
                {
#if UNITY_ANDROID || UNITY_IOS
                    //调用震动
                    Handheld.Vibrate();
#endif
                }
            }
            else
            {
                _readSecond += Time.deltaTime;
                if (_readSecond > 1f)
                {
                    _readSecond = 0f;
                }

                CountDown.color = new Color(1f, (CountDown.color.g - _cdColorTime * Time.deltaTime) <= 0f
                    ? 0f
                    : CountDown.color.g - _cdColorTime * Time.deltaTime, 0f);
            }



            if (CountDown.fillAmount <= 0)
            {
                if (_isCdLoop)
                {
                    CountDown.fillAmount = 1f;
                    CountDown.color = Color.green;
                }
                else
                {
                    StopCountDown();
                }
            }
        }


        /// <summary>
        /// 停止倒计时
        /// </summary>
        public void StopCountDown()
        {
            if(CountDown != null)
                CountDown.gameObject.SetActive(false);
            _isCountDown = false;
            _isCdLoop = false;
            YxDebug.Log("倒计时结束!!");
        }
        #endregion

        /// <summary>
        /// 玩家买了保险
        /// </summary>
        public void BuyInsurance(int gold)
        {
            _insureranceMark.ShowInsuranceMark();
            Coin -= gold;
            //CurUserInfo.Gold -= gold;
            //RefreshPanelInfo();
        }

        public void BuyDouble(int gold)
        {
            _doubleMark.ShowDoubleMark();
            Coin -= gold;
            //CurUserInfo.Gold -= gold;
            //RefreshPanelInfo();
            BetMoney += gold;
        }

        public virtual void RefreshHeadImage()
        {

        }


        readonly List<Bet> _bets = new List<Bet>();


        /// <summary>
        /// 玩家下注
        /// </summary>
        public void PlayerBet(int money, int depth = 99)
        {
            if (money <= 0)
            {
                return;
            }
            var mgr = App.GetGameManager<BlackJackGameManager>();

            BetMoney += money;


            GameObject[] bets = mgr.BetMgr.CreatBetArray(money, 9, transform);        //在翻倍场中,会出现下5个注的情况，再此多预留一个，烂底翻倍场出现9的情况


            //飞出筹码到指定位置
            for (int i = 0; i < bets.Length; i++)
            {
                Bet bet = bets[i].GetComponent<Bet>();
                bet.transform.parent = _betArea;
                //main.BetMgr.BetParent.GetComponent<UIPanel>().depth = 4;        //为下注筹码在牌上方飞出,设置层级关系
                 

                bet.BeginMove(
                    bet.transform.localPosition, GetRandomPos(Vector3.zero),
                    i * App.GetGameData<BlackJackGameData>().BetSpeace, BetFinishedType.None,
                    () =>
                    {
                        bet.transform.parent = _betArea;
                        //main.BetMgr.BetParent.GetComponent<UIPanel>().depth = 2;    //将层级重置回正常,为发牌时牌在桌面筹码上方飞出
                    });
                _bets.Add(bet);
            }
            Coin -= money;
        }

        /// <summary>
        /// 准备按钮的点击事件,挂在到外部物体
        /// </summary>
        public void OnClickReadyBtn()
        {
            if (ReadyBtn == null)
                return;

            App.GetRServer<BlackJackGameServer>().ReadyGame();
        }


        /// <summary>
        /// 设置准备显示状态
        /// </summary>
        /// <param name="active">准备状态</param>
        public void SetReadyMarkActive(bool active)
        {
            if (ReadyMark == null)
                return;
            ReadyMark.SetActive(active);
        }

        /// <summary>
        /// 获取下注的目标位置
        /// </summary>
        /// <param name="basePos">基础位置</param>
        /// <returns></returns>
        Vector3 GetRandomPos( Vector3 basePos )
        {
            float x = UnityEngine.Random.Range(-32, 33);
            float y = UnityEngine.Random.Range(-32, 33);
            return new Vector3(basePos.x + x, basePos.y + y, basePos.z);
        }

        /// <summary>
        /// 设置准备按钮的显示状态
        /// </summary>
        /// <param name="acitve"></param>
        public void SetReadyBtnActive(bool acitve)
        {
            if (ReadyBtn == null)
                return;
            ReadyBtn.gameObject.SetActive(acitve);
        }


        public void Reset()
        {
            PointLabel.text = "";
            PointLabel.gameObject.SetActive(false);
            OnesPokerCount = 0;

            HandCardPoint = 0;
            if(_insureranceMark != null)
                _insureranceMark.HideInsuranceMark();
            if(_doubleMark != null)
                _doubleMark.HideDoubleMark();

            StateMark.gameObject.SetActive(false);

            HaveAce = false;
            BetMoney = 0;
            StopCountDown();

            for (int i = 0; i < CardsId.Length; i++)
            {
                CardsId[i] = 0;
            }

            foreach (Bet bet in _bets)
            {
                Destroy(bet.gameObject);
            }
            _bets.Clear();

            foreach (var t in PokerPos)
            {
                t.DestroyChildren();
                t.gameObject.SetActive(false);
            }

            SetReadyBtnActive(false);
            SetReadyMarkActive(false);
        }


        public void OnUserReady()
        {
            ReadyState = true;
            SetObjActive(ReadyStateFlag, true);
            SetObjActive(ReadyBtn.gameObject, false);
        }

        public void OnGameBegin()
        {
            SetObjActive(ReadyBtn.gameObject, false);
            SetObjActive(ReadyStateFlag, false);
        }

        void SetObjActive(GameObject go, bool active)
        {
            if (go == null) return;
            go.SetActive(active);
        }

    }


    

}