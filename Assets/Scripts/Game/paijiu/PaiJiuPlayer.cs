using Assets.Scripts.Game.paijiu.ImgPress.Main;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.paijiu
{
    public class PaiJiuPlayer : YxBaseGamePlayer
    {
        /// <summary>
        /// 玩家手牌管理
        /// </summary>
        public BetPoker UserBetPoker = null;
       

        /// <summary>
        /// 庄家标记
        /// </summary>
        public GameObject BankerMark = null;

        /// <summary>
        /// 下注显示
        /// </summary>
        public UILabel BetLabel = null;

        /// <summary>
        /// 蒙布,用于标注是否是正常状态
        /// </summary>
        public GameObject Mask = null;


        /// <summary>
        /// 准备标记,用于显示玩家是否准备
        /// </summary>
        public GameObject ReadyMark = null;


        /// <summary>
        /// 扑克的位置
        /// </summary>
        public Transform[] PokersTrans = null;

        /// <summary>
        /// 筹码的父层级
        /// </summary>
        //public Transform BetParent = null;

        /// <summary>
        /// 玩家胜利动画
        /// </summary>
        public GameObject WinAnimation = null;

        public Vector3[] FinishPos = new Vector3[4];

        public Vector3[] NormalPos = new Vector3[4];


        [HideInInspector]
        public string Ip;


        /// <summary>
        /// 玩家头像图片
        /// </summary>
        [HideInInspector]
        public Texture2D HeadImage = null;


        /// <summary>
        /// 显示玩家GPS信息 
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UILabel _gpsInfoLabel = null;

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

        public ShowScoreModel ShowScoreModel;


        /// <summary>
        /// 玩家有多少牌
        /// </summary>
        public int PokerCount
        {
            get { return UserBetPoker.PokerCount; }
        }

        public void ShowWinVal(long val)
        {
            if (Info == null)
            {
                return;
            }
            int lastTurnGold = (int)Info.CoinA + BetMoney;

            int score = (int)val - lastTurnGold;
            string winScore = string.Format("{0}{1}",score >= 0 ? "+":"", YxUtiles.ReduceNumber(score,2,true));
            YxDebug.Log(winScore, "winScore");
            ShowScoreModel.ShowScore(winScore);
            SetCoin(val);
        }


        // ReSharper disable once InconsistentNaming
        protected bool _isReady;

        /// <summary>
        /// 是否准备
        /// </summary>
        public bool IsReady
        {
            set
            {
                _isReady = value;
                Mask.SetActive(!_isReady);
            }
            get { return _isReady; }
        }


        public virtual void OnGameStart()
        {
            ReadyMark.SetActive(false);
        }


        public int BetMoney;

        /// <summary>
        /// 玩家下注
        /// </summary>
        public void PlayerBet(int money, bool changeRoomGold = true, int depth = 300)
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

            var main = App.GetGameManager<PaiJiuGameManager>();
            GameObject[] bets = main.BetMgr.CreatBetArray(money, 9, transform, depth);           //在翻倍场中,会出现下5个注的情况，再此多预留一个，烂底翻倍场出现9的情况


            for (int i = 0; i < bets.Length; i++)
            {
                Bet bet = bets[i].GetComponent<Bet>();
                bet.transform.parent = BetLabel.transform;

                bet.BeginMove(
                    bet.transform.localPosition, Vector3.zero,
                    i * App.GetGameData<PaiJiuGameData>().BetSpeace, BetFinishedType.Hide, 0.3f,
                    () =>
                    {
                        BetLabel.gameObject.SetActive(true);
                    });
            }

            ShowBetLabel();

            Facade.Instance<MusicManager>().Play("bet");                              //播放声音

            if (changeRoomGold)
            {
                Info.CoinA -= money;
                SetCoin(Info.CoinA);
            }

            RefreshPanel();
        }

        public void ShowBetLabel()
        {
            BetLabel.gameObject.SetActive(true);
            BetLabel.text =YxUtiles.ReduceNumber(BetMoney);
        }


        /// <summary>
        /// 设置庄家标记显示
        /// </summary>
        /// <param name="active"></param>
        public void SetBankerMarkActive(bool active)
        {
            BankerMark.SetActive(active);
        }


        /// <summary>
        /// 刷新界面信息
        /// </summary>
        public void RefreshPanel()
        {
            if (Info == null)
            {
                BetLabel.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// 设置层次关系
        /// </summary>
        /// <param name="depth"></param>
        public void SetPlayerDepth(int depth)
        {
            var wis = gameObject.GetComponentsInChildren<UIWidget>(true);
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < wis.Length; i++)
            {
                UIWidget wi = wis[i];
                wi.depth += depth;
            }
        }


        /// <summary>
        /// 刷新用户数据
        /// </summary>
        public void RefreshUserInfo()
        {
            if (Info == null)
            {
                YxDebug.LogError("没有用户信息!!");
                BetLabel.gameObject.SetActive(false);
                //ReadyMark.SetActive(false);
                return;
            }
            ShowUserInfo();
        }

        protected virtual void ShowUserInfo()
        {
            var userInfo = (PaiJiuUserInfo)Info;
            IsReady = userInfo.State;

            Mask.SetActive(!IsReady);
            //ReadyMark.SetActive(!gdata.IsGameing && IsReady);
        }


        public virtual void OnUserReady()
        {
            IsReady = true;
            var userInfo = (PaiJiuUserInfo)Info;
            if (userInfo == null) return;
            userInfo.State = true;
            //ReadyMark.SetActive(true);
        }




        public void CleanCards()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < PokersTrans.Length; i++)
            {
                Transform tran = PokersTrans[i];
                tran.DestroyChildren();
            }
        }

        /// <summary>
        /// 重置
        /// </summary>
        public virtual void Reset()
        {

            BetMoney = 0;
            IsReady = false;
            //ReadyMark.SetActive(false);
            SetBankerMarkActive(false);
            CleanCards();

            BetLabel.text = "";
            BetLabel.gameObject.SetActive(false);

            WinAnimation.SetActive(false);
            RefreshPanel();
            UserBetPoker.Reset();

            ResetPokersTrans();
            ShowScoreModel.StopPlay();
        }


        /// <summary>
        /// 玩家播放声音(区别性别)
        /// </summary>
        /// <param name="clipName">声音名字</param>
        public void PlaySound(string clipName)
        {
            Facade.Instance<MusicManager>().Play(string.Format("{0}{1}", clipName, Info.SexI));
        }

        public void ShowGpsInfo()
        {
            _gpsInfoLabel.gameObject.SetActive(true);
            _gpsInfoLabel.text = string.Format("{0}\nID:{1}\nIP:{3}\n所在地:{2}", Info.NickM,
                Info.Id, HasGpsInfo ?
                    Info.Country : "未提供位置信息\n请开启位置服务,并给予应用相应权限",
                Ip);
            _gpsInfoLabel.MakePixelPerfect();

            UISprite sp = _gpsInfoLabel.transform.GetChild(0).GetComponent<UISprite>();
            sp.width = _gpsInfoLabel.width + 50;
            sp.height = _gpsInfoLabel.height + 50;
        }

        public void HideGpsInfo()
        {
            _gpsInfoLabel.gameObject.SetActive(false);
        }


        internal void AddPoker(PaiJiuCard pokerCard)
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
        internal virtual void TurnCard(int cardIndex, int val, bool withAnim = true)
        {
            PaiJiuCard poker = UserBetPoker.PlayerPokers[cardIndex];
            if (poker == null)
                return;

            int preVal = poker.Id;
            if (preVal == val)
                return;

            poker.SetCardId(val);
            if (withAnim)
            {
                if (preVal == 0)
                {
                    poker.TurnCard();
                }
            }
            else
            {
                if (preVal == 0)
                {
                    poker.SetCardFront();
                }
            }
        }


        internal virtual void FinishSelect()
        {
            for (int i = 0; i < PokersTrans.Length; i++)
            {
                PokersTrans[i].localPosition = FinishPos[i];
                PokersTrans[i].localEulerAngles = new Vector3(0, 0, i >= PokersTrans.Length / 2 ? -90 : 0);
            }
        }


        protected virtual void ResetPokersTrans()
        {
            for (int i = 0; i < PokersTrans.Length; i++)
            {
                PokersTrans[i].localPosition = NormalPos[i];
                PokersTrans[i].localEulerAngles = Vector3.zero;
            }
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
