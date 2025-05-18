using System;
using Assets.Scripts.Game.duifen.ImgPress.Main;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.duifen
{
    public class DuifenPlayerPanel : YxBaseGamePlayer
    {
      
        /// <summary>
        /// 玩家手牌管理
        /// </summary>
        public BetPoker UserBetPoker = null;
   
        /// <summary>
        /// 倒计时图片对象
        /// </summary>
        public UISprite CountDown = null;
        /// <summary>
        /// 下注显示
        /// </summary>
        public UILabel BetLabel = null;
        /// <summary>
        /// 大盲注显示对象
        /// </summary>
        //public GameObject Blinds = null;

        /// <summary>
        /// 蒙布,用于标注是否是正常状态
        /// </summary>
        public GameObject Mask = null;
    
        /// <summary>
        /// 显示玩家是否断开连接,显示为断开
        /// </summary>
        public GameObject ConnectMark = null;

        /// <summary>
        /// 扑克的位置
        /// </summary>
        public Transform[] PokersTrans = null;

        /// <summary>
        /// 玩家胜利动画
        /// </summary>
        public GameObject WinAnimation = null;


        [HideInInspector]
        public string Ip;

        /// <summary>
        /// 显示玩家手中的点数
        /// </summary>
        [SerializeField]
        // ReSharper disable once InconsistentNaming
        protected UILabel _pokerPointLabel = null;

        /// <summary>
        /// 显示玩家当前状态
        /// </summary>
        public UISprite GameTypeSprite = null;


        /// <summary>
        /// 记录玩家的状态
        /// 1.下注 3.弃牌,6.跟注,7.起立,8.开牌
        /// </summary>
        [HideInInspector]
        public int PlayerType = -1;

        /// <summary>
        /// GPS 经度数据
        /// </summary>
        [HideInInspector]
        public float GpsX = 0;


        /// <summary>
        /// GPS 维度数据
        /// </summary>
        [HideInInspector]
        public float GpsY = 0;

        /// <summary>
        /// 是否有GPS信息
        /// </summary>
        [HideInInspector]
        public bool HasGpsInfo = false;


        /// <summary>
        /// 玩家自动出牌标记,游戏结束不重置,等待服务器发信息
        /// </summary>
        public GameObject AutoMark;

        
        /// <summary>
        /// 玩家有多少牌
        /// </summary>
        public int PokerCount
        {
            // ReSharper disable once ConvertPropertyToExpressionBody
            get { return UserBetPoker.PokerCount; }
        }


        public virtual void ShowSelfPointLabel()
        {

        }

        public virtual void ShowPointLabel(int point = 0)
        {
            if (point == 0)
                return;

            _pokerPointLabel.text = point + "点";
            _pokerPointLabel.gameObject.SetActive(point > 0);
        }

      
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void FixedUpdate()
        {
            CountDownUpdate();
        }

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


        bool _isCdLoop;

        [SerializeField]
        float _commonCDTime = 15f;
        /// <summary>
        /// 开始倒计时
        /// </summary>
        public void BeginCountDown(float time)
        {
            if (time <= 0)
            {
                YxDebug.LogError("倒计时时间不能小于等于0!!");
                return;
            }

            if (time > 150f && App.GetGameData<DuifenGlobalData>().IsRoomGame)
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
        /// 停止倒计时
        /// </summary>
        public void StopCountDown()
        {
            CountDown.gameObject.SetActive(false);
            _isCountDown = false;
            _isCdLoop = false;
        }
        /// <summary>
        /// 倒计时读秒
        /// </summary>
        private float _readSecond;
        /// <summary>
        /// 倒数update
        /// </summary>
        private void CountDownUpdate()
        {
            if (!_isCountDown )
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

        public virtual void OnGameStart()
        {
            ReadyStateFlag.SetActive(false);
            PlayerBet(App.GetGameData<DuifenGlobalData>().GuoBet);
        }

        protected int BetMoney;
      
        /// <summary>
        /// 玩家下注w
        /// </summary>
        public void PlayerBet(int money)
        {
            if (Info == null)
            {
                YxDebug.LogError("没有用户信息!!");
                return;
            }

            if (money <= 0)
            {
                return;
            }

            BetMoney += money;
           
            ShowBetLabel();
           
            //Facade.Instance<MusicManager>().Play("bet");

            Coin -= money;
        }

        public void GiveMoney(DuifenPlayerPanel someone, int money,BetFinishedType type = BetFinishedType.None,float delay = 0,float flyTime = 0.3f ,int depth = 99)
        {
            var main = App.GetGameManager<DuifenGameManager>();
            GameObject[] bets = main.BetMgr.CreatBetArray(money, 9, transform, depth);
            for (int i = 0; i < bets.Length; i++)
            {
                Bet bet = bets[i].GetComponent<Bet>();
                bet.transform.parent = someone.transform;
                main.BetMgr.BetParent.GetComponent<UIPanel>().depth = 4;

                bet.BeginMove(bet.transform.localPosition, Vector3.zero, delay,
                    type, flyTime);
            }
        }


        public void ShowBetLabel()
        {
            BetLabel.gameObject.SetActive(true);
            BetLabel.text = App.GetGameData<DuifenGlobalData>().GetShowGoldValue(BetMoney, true);
        }

        
        /// <summary>
        /// 设置层次关系
        /// </summary>
        /// <param name="depth"></param>
        public void SetPlayerDepth(int depth)
        {
            var wis = gameObject.GetComponentsInChildren<UIWidget>(true);
            foreach (UIWidget wi in wis)
            {
                wi.depth += depth;
            }
        }
    

        public virtual void ShowUserInfo()
        {
            Mask.SetActive(!ReadyState);
            ConnectMark.SetActive(false);
            gameObject.SetActive(true);
        }



        public virtual void OnUserReady()
        {
            ReadyState = true;
            Mask.SetActive(false);
        }


        public virtual void PlayerFold()
        {
            UserBetPoker.FoldTurnCards();
            Mask.SetActive(false);
        }


        public void CleanCards()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < PokersTrans.Length; i++)
            {
                PokersTrans[i].DestroyChildren();
            }
        }


        public virtual void OnGetAutoInfo(bool isAuto)
        {
            if (AutoMark == null) return;
            AutoMark.SetActive(isAuto);
            if (!isAuto) return;
            var anim = AutoMark.GetComponent<UISpriteAnimation>();
            if (anim == null) return;
            anim.Play();
        }


        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {
            BetMoney = 0;
            StopCountDown();

            _pokerPointLabel.gameObject.SetActive(false);

            BetLabel.text = "";
            BetLabel.gameObject.SetActive(false);

            PlayerType = -1;
            HideGameType(true);

            WinAnimation.SetActive(false);
            UserBetPoker.Reset();
        }



        /// <summary>
        /// 通过名字显示玩家状态
        /// </summary>
        /// <param name="type">当前玩家状态,对应游戏状态</param>
        public void ShowGameType(PlayerGameType type)
        {
            switch (type)
            {
                case PlayerGameType.AddBet:
                    PlayerType = 1;
                    ShowTypeSprite("AddBet");
                    //PlaySound("bet");      //只有一个，不用随机
                    break;
                case  PlayerGameType.Call:
                    PlayerType = 2;
                    ShowTypeSprite("Call");
                    //PlaySound("call");        //跟注
                    break;
                case  PlayerGameType.Fold:
                    PlayerType = 3;
                    ShowTypeSprite("Fold");
                    break;
                case PlayerGameType.KaiPai:
                    PlayerType = 8;
                    ShowTypeSprite("KaiPai");
                    //PlaySound("kaiPai"); 
                    break;
                case PlayerGameType.QiLi:
                    PlayerType = 9;
                    ShowTypeSprite("QiLi");
                    //PlaySound("qili"); 
                    break;
                default:
                    HideGameType();
                    break;
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <param name="hideFold"></param>
        public void HideGameType(bool hideFold = false)
        {
            if (!hideFold && PlayerType == 3)
                return;

            PlayerType = -1;
            GameTypeSprite.gameObject.SetActive(false);
        }

        public void ShowTypeSprite(string sprName)
        {
            GameTypeSprite.spriteName = sprName;
            GameTypeSprite.gameObject.SetActive(true);
            GameTypeSprite.MakePixelPerfect();
        }

        /// <summary>
        /// 玩家播放声音(区别性别)
        /// </summary>
        /// <param name="clipName">声音名字</param>
        public void PlaySound(string clipName)
        {
            string source = Info.SexI == 0 ? "woman" : "man";
            Facade.Instance<MusicManager>().Play(clipName, source);
        }
   

        internal void AddPoker(PokerCard pokerCard)
        {
            UserBetPoker.AddPoker(pokerCard);
        }

        internal virtual void CouldStart()
        {

        }


        /// <summary>
        /// 是否有索引的这张牌
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal virtual bool HaveThisCard(int index)
        {
            //手牌个数大于索引值,说明有这张牌了
            return UserBetPoker.PokerCount > index;

        }

        /// <summary>
        /// 翻转一张手牌
        /// </summary>
        /// <param name="cardIndex">牌的索引</param>
        /// <param name="val">手牌的值</param>
        /// <param name="withAnim">是否有动画</param>
        internal virtual void TurnCard(int cardIndex,int val,bool withAnim = true)
        {
            //PokerCard poker = UserBetPoker.PlayerPokers[cardIndex];
            PokerCard poker = UserBetPoker.GetPokerCard(cardIndex);

            if (poker == null)
                return;

            if (PlayerType == 3)    //玩家弃牌不翻牌
                return;

            //已经翻牌了不翻牌
            int preVal = poker.Id;
            if (preVal == val)      
                return;

            poker.SetCardId(val);

            if (withAnim)
            {
                if (preVal == 0)
                {
                    poker.TurnCard();
                    UserBetPoker.AddPokerVal(poker.Value);
                }
            }
            else
            {
                if (preVal == 0)
                {
                    UserBetPoker.AddPokerVal(poker.Value);
                    poker.SetCardFront();
                }
            }
        }


        public void SetConnectMaskActive(bool active)
        {
            Mask.SetActive(active || !ReadyState);
            ConnectMark.SetActive(active);
        }
    }
    /// <summary>
    /// 玩家游戏状态
    /// </summary>
    public enum PlayerGameType
    {   
        /// <summary>
        /// 0.正常状态
        /// </summary>
        None = 0,           
        /// <summary>
        /// 1.下注
        /// </summary>
        AddBet = 1,      
        /// <summary>
        /// 2.跟注
        /// </summary>
        Call = 2,
        /// <summary>
        /// 3.弃牌
        /// </summary>
        Fold = 3,
        /// <summary>
        /// 7.起立
        /// </summary>
        QiLi = 7,
        /// <summary>
        /// 8.开牌
        /// </summary>
        KaiPai = 8,
    }
}
