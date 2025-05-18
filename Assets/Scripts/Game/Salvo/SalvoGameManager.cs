using System;
using System.Collections;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.Salvo.Entity;
using Assets.Scripts.Game.Salvo.Utiles;
using Assets.Scripts.Game.Salvo.View;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Salvo
{
    public class SalvoGameManager : YxGameManager
    {
        /// <summary>
        /// 开始标题
        /// </summary>
        public GameObject StartSign;
        /// <summary>
        /// 加入金币提示
        /// </summary>
        public GameObject InsertCoinSign;  
        /// <summary>
        /// 赌注
        /// </summary>
        public YxBaseNumberAdapter BetNum; 
        /// <summary>
        /// 扑克类型视图
        /// </summary>
        public PokerTypeView PokerTypeV;
        /// <summary>
        /// 减注按钮
        /// </summary>
        public UIButton ReductionBtn;
        /// <summary>
        /// 加注按钮
        /// </summary>
        public UIButton RefuelBtn;
        /// <summary>
        /// 开始按钮
        /// </summary>
        public UIButton StartBtn;
        /// <summary>
        /// 自动开始按钮
        /// </summary>
        public UIToggle AutoToggle;
        /// <summary>
        /// 扑克区域
        /// </summary>
        public PokerAreaView PokerArea;
        /// <summary>
        /// 胜利
        /// </summary>
        public GameObject SignWin;
        /// <summary>
        /// 最大倍率（10为基）
        /// </summary>
        public int MaxRate = 1; 

        private GameState _gameState = GameState.Start;
        public GameState GameState {
            get { return _gameState; }
        }

        protected void Start()
        {  
            StartBtn.isEnabled = false;
            RefuelBtn.isEnabled = false;
            ReductionBtn.isEnabled = false;
            if(AutoToggle!=null) { AutoToggle.GetComponent<UIButton>().isEnabled = false;}
            var reductionBtnL = UIEventListener.Get(ReductionBtn.gameObject);
            reductionBtnL.onPress = OnReductionBtnPress;
            var refuelBtnL = UIEventListener.Get(RefuelBtn.gameObject);
            refuelBtnL.onPress = OnRefuelBtnPress;
        }
          

        public void SetBet(int bet = 0)
        {
            App.GetGameData<SalvoGameData>().Bet = bet;
            BetNum.SetNumber(bet);
            PokerTypeV.SetBet(bet);
            var isMore = bet > 0;
            ReductionBtn.isEnabled = isMore;
            StartBtn.isEnabled = isMore; 
            InsertCoinSign.SetActive(!isMore);
        }
         
        /// <summary>
        /// 开始按钮事件
        /// </summary>
        public void OnStartBtn()
        {
            Facade.Instance<MusicManager>().Play("startorover");
            var gdata = App.GetGameData<SalvoGameData>();
            var player = gdata.GetPlayer<SalvoPlayer>();
            switch (_gameState)
            {
                case GameState.Start:
                    {
                        ShowWinSigne(false);
                        StartSign.SetActive(false);
                        StartBtn.isEnabled = false;
                        PokerArea.Show();
                        PokerTypeV.StopTwinkle();
                        var bet = App.GetGameData<SalvoGameData>().Bet; 
                        PokerTypeV.SetBet(bet);
                        PokerArea.TurnPoker(() =>
                            {
                                CheckButtonEnabel();
                                //_gameState = GameState.Turn;//todo 以后需要分开，解开此行，隐藏下面四行代码
                                ChangeBtnsEnable(false);
                                var temp = player.Coin - BetNum.Number;
                                player.SetCoinValue(temp,1.5f,23); 
                                App.GetRServer<SalvoGameServer>().SendGetPokersDate(gdata.Bet);
                            }); 
                    }
                    break;
                case GameState.Turn:
                    {
                        ChangeBtnsEnable(false);
                        var temp  = player.Coin - BetNum.Number;
                        player.SetCoinValue(temp, 1.5f, 23);
                        App.GetRServer<SalvoGameServer>().SendGetPokersDate(gdata.Bet);
                    }
                    break;
                case GameState.Replace:
                    {
                        ChangeBtnsEnable(false);
                        App.GetRServer<SalvoGameServer>().SendreplacePokersDate(gdata.Bet, PokerArea.GetChangePokers());
                    }
                    break; 
            } 
        } 

        private void ChangeBtnsEnable(bool enable)
        {
            StartBtn.isEnabled = enable;
            ReductionBtn.isEnabled = enable;
            RefuelBtn.isEnabled = enable;
        }
        private bool _isReductionPress;
        private bool _isRefuelPress;
        /// <summary>
        /// 减注按钮事件
        /// </summary>
        public void OnReductionBtn()
        {
            Facade.Instance<MusicManager>().Play("ADD");
            var salvoGd = App.GetGameData<SalvoGameData>();
            var curBet = salvoGd.Bet;
            var baseBet = salvoGd.BaseBet;
            curBet -= baseBet;
            if (curBet >= 0) SetBet(curBet);
            CheckButtonEnabel();
        }

        private void OnReductionBtnPress(GameObject btn = null, bool isPress = true)
        {
            if (!isPress)
            { 
                _isReductionPress = false;
                return;
            }
            if (_isReductionPress) return; 
            _isReductionPress = true;
            StartCoroutine(ReductionPressEvent());
        }

        private IEnumerator ReductionPressEvent()
        {
            yield return new WaitForSeconds(0.5f);
            var wait = new WaitForSeconds(0.1f);
            while (ReductionBtn.isEnabled && _isReductionPress)
            {
                OnReductionBtn();
                yield return wait;
            }
        }
         
        /// <summary>
        /// 加注按钮事件
        /// </summary>
        public void OnRefuelBtn()
        {
            Facade.Instance<MusicManager>().Play("ADD");
            var gdata = App.GetGameData<SalvoGameData>();
            var curCredit = gdata.GetPlayer().Coin;
            var curBet = gdata.Bet;
            var baseBet = gdata.BaseBet;
            curBet += baseBet;
            if (curBet <= curCredit)SetBet(curBet);
            CheckButtonEnabel(); 
        }

        private void OnRefuelBtnPress(GameObject btn = null, bool isPress = true)
        { 
            if (!isPress)
            {
                _isRefuelPress = false;
                return;
            }
            if (_isRefuelPress) return;
            _isRefuelPress = true;
            StartCoroutine(RefuelPressEvent());
        }

        private IEnumerator RefuelPressEvent()
        {
            yield return new WaitForSeconds(0.5f);
            var wait = new WaitForSeconds(0.1f);
            while (RefuelBtn.isEnabled && _isRefuelPress)
            {
                OnRefuelBtn();
                yield return wait;
            }
        }
         
        /// <summary>
        /// 第一次开始:转牌
        /// </summary>
        /// <param name="pokers"></param>
        public void OnTurnPokers(int[] pokers)
        {
            _gameState = GameState.Start;
            PokerArea.InitPokers(pokers);
            PokerArea.TurnPoker(() =>
                {
                    StartBtn.isEnabled = true;
                    HeldDate heldData;
                    PokerTypeRule.GetGoodPokerIndex(PokerArea.GetValues(), out heldData);
                    if (heldData != null) PokerArea.SetHeldPokers(heldData.Value);
                    _gameState = GameState.Replace;
                }); 
        }

        /// <summary>
        /// 第二次开始:换牌
        /// </summary>
        /// <param name="pokers"></param>
        public void OnReplacePokers(int[] pokers)
        { 
            PokerArea.SetReplacePokers(pokers);
            PokerArea.ReplacePokers(() =>
                { 
                    HeldDate heldData;
                    var type = PokerTypeRule.GetGoodPokerIndex(PokerArea.GetValues(), out heldData);
                    PokerTypeV.Twinkle((int)type);
                    var helds = heldData!=null?heldData.Value : 0;
                    PokerArea.HighlightPoker(helds);
                    if (type == PokerType.None)
                    {
                        Facade.Instance<MusicManager>().Play("LOSE");
                        OnReadyStart();
                        return;
                    }
                    ShowWinSigne();
                    StartCoroutine(UpdateCreditNum((int)type));
                }); 
        }

        private IEnumerator UpdateCreditNum(int index)
        {
            var gdata = App.GameData;
            var player = gdata.GetPlayer<SalvoPlayer>();
            var temp = player.Info.CoinA;//Coin + BetNum.Number * PokerTypeV.Odds[index];  
            if (index > 9)
            {
                Facade.Instance<MusicManager>().Play("win_3");
            }else if (index > 6)
            {
                Facade.Instance<MusicManager>().Play("win_2");
            }else if (index > 3)
            {
                Facade.Instance<MusicManager>().Play("win_1");
            }
            else
            {
                Facade.Instance<MusicManager>().Play("win_0");
            }
            yield return new WaitForSeconds(1);
            PokerTypeV.SetOdd(index, 0); 
            player.SetCoinValue(temp,1.5f,53, num=>OnReadyStart(), num => Facade.Instance<MusicManager>().Play("giveback"));
        }

        private void Update()
        {
            if (AutoToggle == null) return;
            if (!AutoToggle.value) return;
            if (!StartBtn.isEnabled) return;
            StartBtn.isEnabled = false;
            OnStartBtn();
        }

        /// <summary>
        /// 检查按钮可用性
        /// </summary>
        private void CheckButtonEnabel()
        {
            var gdata = App.GetGameData<SalvoGameData>();
            var curCredit = gdata.GetPlayer().Coin;
            var curBet = BetNum.Number;
            var baseBet = gdata.BaseBet;
            if (curCredit < baseBet)//没钱
            {
                ChangeBtnsEnable(false);
                return;
            } 
            ReductionBtn.isEnabled = curBet>0;
            RefuelBtn.isEnabled = (curCredit - curBet >= baseBet) && curBet < baseBet * MaxRate * 2 * 5;
            StartBtn.isEnabled = curBet>0 && curCredit >= curBet;
        }

        /// <summary>
        /// 重新开始
        /// </summary>
        private void OnReadyStart()
        {
            StartBtn.isEnabled = true;
            _gameState = GameState.Start;
            CheckButtonEnabel();
        }

        public void ShowWinSigne(bool isShow = true)
        {
            SignWin.SetActive(isShow);
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            //状态 
            SetBet();
            RefuelBtn.isEnabled = true;
            if(AutoToggle!=null) AutoToggle.GetComponent<UIButton>().isEnabled = true;
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int type, ISFSObject gameInfo)
        { 
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            switch (type)
            {
                case 1://开始
                {
                    var cards = response.GetIntArray(RequestKey.KeyCards);
                    YxDebug.LogArray(cards);
                    OnTurnPokers(cards);
                }
                    break;
                case 2://换牌
                {
                    var data = response.GetSFSObject("data");
                    var cards = data.GetIntArray(RequestKey.KeyCards);
                    YxDebug.LogArray(cards);
                    App.GetGameData<SalvoGameData>().GetPlayer().Info.CoinA = response.GetLong(RequestKey.KeyTotalGold);
                    OnReplacePokers(cards);
                }
                    break;
            }
        }
    }

    public enum GameState
    {
        None=0,
        Start=1,
        Turn=2,
        Replace=3
    }
}
