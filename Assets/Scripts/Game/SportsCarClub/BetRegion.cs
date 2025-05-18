using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;
using YxFramwork.Tool;
using Random = UnityEngine.Random;
using YxFramwork.View;

namespace Assets.Scripts.Game.SportsCarClub
{
    /// <summary>
    /// 下注区代码
    /// </summary>
    public class BetRegion : MonoBehaviour
    {
        /// <summary>
        /// 当前的押注类型
        /// </summary>
        public BetType CurBetType;
        /// <summary>
        /// 下注位置
        /// </summary>
        public GameObject BetPos;
        /// <summary>
        /// 获奖提示红圈
        /// </summary>
        public GameObject Winning;
        /// <summary>
        /// 押注类型图
        /// </summary>
        public UISprite[] BetTypeImage;
        /// <summary>
        /// 下注宽
        /// </summary>
        public int BetW;
        /// <summary>
        /// 下注高
        /// </summary>
        public int BetH;
        /// <summary>
        /// 筹码管理
        /// </summary>
        private ArrayList _bets;
        /// <summary>
        /// 总值显示
        /// </summary>
        public UILabel TotalLabel;
        /// <summary>
        /// 自己下注显示
        /// </summary>
        public UILabel SelfLabel;
        /// <summary>
        /// 中奖动画
        /// </summary>
        public UISpriteAnimation Anim;
        /// <summary>
        /// 可下分数
        /// </summary>
        public UILabel CanBetUiLabel;
        /// <summary>
        /// 本区域的倍率
        /// </summary>
        public int Rate;
        /// <summary>
        /// 本区域的倍率显示
        /// </summary>
        //public UILabel RateLabel;

        // Use this for initialization
        void Start()
        {
            SetBetType();
            _bets = new ArrayList();
            //InitRate();
        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 点击下注区
        /// </summary>
        public void OnClick()
        {
            var gold = App.GetGameData<CschGameData>().AnteRate[BetManager.GetInstance().CurBetIndex];

            if (!IsBet(gold))
            {
                return;
            }

            YxDebug.Log("下注!!");
            //AddBet(BetManager.GetInstance().Bets[BetManager.GetInstance().CurBetIndex]);//直接加注
            IDictionary bet = new Dictionary<string, object>();
            bet.Add("p", (int)CurBetType);
            bet.Add("gold", gold);
            CschGameServer.GetInstance().SendRequest(RequestType.Bet, bet);
        }

        /// <summary>
        /// 是否可以下注
        /// </summary>
        /// <param name="gold"></param>
        /// <returns></returns>
        public bool IsBet(int gold)
        {
            if (!BetManager.GetInstance().IsBeginBet)
                return false;
            var gameMgr = App.GetGameManager<CschGameManager>();
            var self = App.GameData.GetPlayerInfo();
            if (gameMgr.BankerMgr.IsBanker(self.Seat))
            {
                YxMessageTip.Show("您是店主,不能跟自己玩哦!");
                return false;
            }

            if (self.CoinA < gold)
            {
                YxMessageTip.Show("金币不足,请充值或换更小的筹码!");
                return false;
            }

            if (gold > CanBet)
            {
                YxMessageTip.Show("不能下注,店家金币不足!");
                return false;
            }

            return true;
        }

        //当前筹码个数
        private int _curBetCount;
        //最小深度
        public int MinDepth;
        /// <summary>
        /// 该区域下注总金币
        /// </summary>
        [HideInInspector]
        public int TotalGold;
        /// <summary>
        /// 该区域自己下注的金币
        /// </summary>
        [HideInInspector]
        public int SelfGold;
        /// <summary>
        /// 添加筹码 
        /// </summary>
        /// <param name="bet">筹码</param>
        /// <param name="seat">下注人的座位号  </param>
        public void AddBet(GameObject bet, int seat, int gold, bool isSelf = false)
        {
            _curBetCount++;
            var gob = Instantiate(bet);
            var betManager = BetManager.GetInstance();
            gob.transform.parent = betManager.transform;
            if (isSelf)
            {
                gob.transform.localPosition = bet.transform.localPosition;
            }
            else
            {
                gob.transform.localPosition = betManager.AllPlayerPos.localPosition;
            }
            gob.transform.parent = BetPos.transform;
            //gob.transform.localPosition = Vector3.zero;
            gob.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            //gob.GetComponent<UISprite>().depth = _curBetCount + MinDepth;
            gob.transform.FindChild("Title").gameObject.SetActive(true);
            gob.transform.FindChild("Selected").gameObject.SetActive(false);
            gob.GetComponent<BoxCollider>().enabled = false;
            var w = Random.Range(-BetW, BetW);
            var h = Random.Range(-BetH, BetH);
            var v3 = new Vector3(w, h, 0);
            var target = Vector3.zero + v3;
            //gob.transform.transform.localPosition += v3;
            _bets.Add(gob);
            TotalGold += gold;
            if (seat == App.GameData.SelfSeat)
            {
                SelfGold += gold;
            }
            TotalLabel.text = TotalGold > 0 ? YxUtiles.GetShowNumberForm(TotalGold) : "";
            SelfLabel.text = SelfGold > 0 ? YxUtiles.GetShowNumberForm(SelfGold) : "";
            SetChipTween(gob.transform, target, betManager.TweenInfo);
        }
        
        private void SetChipTween(Transform chipTs, Vector3 to, TweenInfo tweenInfo,
            List<EventDelegate> actionList = null)
        {
            var tp = chipTs.GetComponent<TweenPosition>();
            if (tp == null) return;

            Vector3 from = chipTs.localPosition;

            tp.from = from;
            tp.to = to;
            tp.animationCurve = tweenInfo.TweenCurve;
            tp.duration = tweenInfo.Duration;

            tp.onFinished = actionList == null ? tweenInfo.OnFinish : actionList;
            tp.ResetToBeginning();
            tp.PlayForward();
        }
        /// <summary>
        /// 可下分数
        /// </summary>
        public int CanBet;

        /// <summary>
        /// 清除区域内的筹码
        /// </summary>
        public void ClearBet()
        {
            for (var i = 0; i < _bets.Count; i++)
            {
                Destroy((GameObject)_bets[i]);
            }

            _curBetCount = 0;
            _bets.Clear();
            TotalGold = 0;
            SelfGold = 0;
            TotalLabel.text = "";
            SelfLabel.text = "";
            CanBet = (int)(App.GetGameManager<CschGameManager>().BankerMgr.Banker.Coin / Rate);
            SetCanBetLabel(CanBet);
        }

        public void SetCanBetLabel(long maxBet)
        {
            CanBetUiLabel.text = string.Format("可下:[-]{0}", YxUtiles.GetShowNumberForm(maxBet));
        }

        /// <summary>
        /// 播放中奖特效
        /// </summary>
        public void PlayLuckAnim()
        {
            //Winning.GetComponent<TweenAlpha>().value = Winning.GetComponent<TweenAlpha>().from;
            Winning.SetActive(true);

            switch (CurBetType)
            {
                case BetType.Bugatti:
                case BetType.Maybach:
                case BetType.Lamborghini:
                case BetType.Bentley:
                    Anim.gameObject.SetActive(true);
                    Anim.ResetToBeginning();
                    Anim.Play();
                    Invoke("CloseEffect", 1f);
                    //Anim.
                    //YxDebug.Log("播放动画");
                    break;
            }
        }
        /// <summary>
        /// 关闭中奖特效
        /// </summary>
        public void StopLuckAnim()
        {
            Winning.SetActive(false);
            switch (CurBetType)
            {
                case BetType.Bugatti:
                case BetType.Maybach:
                case BetType.Lamborghini:
                case BetType.Bentley:
                    Anim.Pause();
                    Anim.gameObject.SetActive(false);
                    //YxDebug.Log("关闭动画");
                    break;
            }
        }

        public void CloseEffect()
        {
            Anim.Pause();
            Anim.gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置下注区域类型
        /// </summary>
        private void SetBetType()
        {
            switch (CurBetType)
            {
                case BetType.BMFP:
                    if (BetTypeImage.Length != 4)
                    {
                        YxDebug.Log("少图标!");
                        return;
                    }
                    //BetTypeImage[0].spriteName = "car_0";
                    //BetTypeImage[1].spriteName = "car_1";
                    //BetTypeImage[2].spriteName = "car_2";
                    //BetTypeImage[3].spriteName = "car_3";
                    //for (int i = 0; i < BetTypeImage.Length; i++)
                    //{
                    //    BetTypeImage[i].MakePixelPerfect();
                    //}
                    break;
                case BetType.LBBJ:
                    if (BetTypeImage.Length != 4)
                    {
                        YxDebug.Log("少图标!");
                        return;
                    }
                    //BetTypeImage[0].spriteName = "car_4";
                    //BetTypeImage[1].spriteName = "car_5";
                    //BetTypeImage[2].spriteName = "car_6";
                    //BetTypeImage[3].spriteName = "car_7";
                    //for (int i = 0; i < BetTypeImage.Length; i++)
                    //{
                    //    BetTypeImage[i].MakePixelPerfect();
                    //}
                    break;
                default:
                    BetTypeImage[0].spriteName = "car_" + (int)CurBetType;
                    BetTypeImage[0].MakePixelPerfect();
                    BetTypeImage[0].transform.localScale = new Vector3(0.4f, 0.4f, 1f);
                    UISprite animSprite = Anim.transform.FindChild("Sprite").GetComponent<UISprite>();
                    animSprite.spriteName = "car_" + (int)CurBetType;
                    animSprite.MakePixelPerfect();
                    animSprite.transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
            }
        }
        /// <summary>
        /// 初始化倍率
        /// </summary>
        //public void InitRate()
        //{
        //    RateLabel.text = Rate + "倍";
        //}
    }

    /// <summary>
    /// 下注类型
    /// </summary>
    public enum BetType
    {
        Bugatti = 0, //布加迪
        Ferrari = 1, //法拉利
        Maybach = 2, //迈巴赫
        Porsche = 3, //保时捷
        Lamborghini = 4, //兰博基尼
        BMW = 5, //宝马
        Bentley = 6, //宾利
        Jaguar = 7, //捷豹
        BMFP, //布加迪,迈巴赫,法拉利,保时捷
        LBBJ, //兰博,宾利,宝马,捷豹
    }
}