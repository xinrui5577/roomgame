using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.GangWu.Main;
using Assets.Scripts.Game.GangWu.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.GangWu.Mgr
{
    /// <summary>
    /// 玩家说话管理类 说话:德州扑克轮到本玩家决定跟牌与否的简称
    /// </summary>
    public class SpeakMgr : MonoBehaviour
    {

        /// <summary>
        /// 按键数组
        /// </summary>
        public GameObject[] Buttons;
        /// <summary>
        /// 其他标签页 加注,说话,结束选项,自动选项
        /// </summary>
        public GameObject[] Pages;
        /// <summary>
        /// 全部玩家最大下注值
        /// </summary>
        [HideInInspector]
        public int MaxBetNum;
   
        /// <summary>
        /// 是否自动弃牌或者看牌
        /// </summary>
        [HideInInspector]
        public bool IsAutoSeeOrFold;
        /// <summary>
        /// 是否自动跟注
        /// </summary>
        [HideInInspector]
        public bool IsAutoCall;
     

        /// <summary>
        /// 自己下注的总计
        /// </summary>
        private int _addBetSum;

        /// <summary>
        /// 显示玩家下注的所有筹码
        /// </summary>
        [SerializeField]
        private UILabel _addBetSumLabel = null;

        /// <summary>
        /// 自己预加注的总和,同时显示在游戏桌面上
        /// </summary>
        public int AddBetSum
        {
            set
            {
                _addBetSum = value;
                _addBetSumLabel.text = "+" + YxUtiles.ReduceNumber(_addBetSum);//App.GetGameData<GlobalData>().GetShowGold(_addBetSum);
                _addBetSumLabel.gameObject.SetActive(_addBetSum > 0);
                TweenScale ts = _addBetSumLabel.gameObject.GetComponent<TweenScale>();
                ts.from = Vector3.one * 1.5f;
                ts.to = Vector3.one;
                ts.duration = 0.3f;
                ts.ResetToBeginning();
                ts.PlayForward();
            }

            get { return _addBetSum; }
        }


        private readonly Stack<int> _betRemenber = new Stack<int>();


        /// <summary>
        /// 记录每轮下注过程
        /// </summary>
        public Stack<int> BetRemenber { get { return _betRemenber; } }


        // Use this for initialization
        protected void Start()
        {
            ShowNothing();
            InitOnClick();
        }

     

        /// <summary>
        /// 显示说话界面
        /// </summary>
        public void ShowSpeak()
        {
            if (IsAutoSpeak())
            {
                return;
            }

            var gdata = App.GetGameData<GangwuGameData>();
            bool isFollow = MaxBetNum > 0;

            var self = gdata.GetPlayer<PlayerPanel>();

            GameObject gob = Tools.GobShowOnlyOne(Pages, "Speak");
            bool haveRoomGold = self.Coin > 0;          //自己是否还有筹码

            //显示跟注或看牌
            GameObject callBtn = gob.transform.FindChild(SpeakButton.Call_Btn.ToString()).gameObject;
            GameObject seePokerBtn = gob.transform.FindChild(SpeakButton.SeePoker_Btn.ToString()).gameObject;
            callBtn.SetActive(isFollow);
            seePokerBtn.SetActive(!isFollow);


            Tools.SetBtnCouldClick(callBtn, haveRoomGold);

            GameObject addBetBtn = gob.transform.FindChild(SpeakButton.AddBet_Btn.ToString()).gameObject;

            bool couldAddBet = (MaxBetNum - self.BetMoney < self.Coin - gdata.MinRoomGold|| gdata.CardCount > 3)&& gdata.NoOneAllin && !gdata.AllocateFour;

            //自己有筹码,且所有玩家都有筹码,不在 all in 状态下,才能加注
            Tools.SetBtnCouldClick(addBetBtn, couldAddBet && haveRoomGold);
        }


        /// <summary>
        /// 是否选择自动选项
        /// </summary>
        /// <returns></returns>
        bool IsAutoSpeak()
        {

            GameObject seeToggle = Tools.GobSelectName(Buttons, SpeakButton.SeePokerOrFold_Toggle.ToString());
            GameObject callToggle = Tools.GobSelectName(Buttons, SpeakButton.Call_Toggle.ToString());

            //自动:看牌或弃牌
            if (IsAutoSeeOrFold)
            {
                seeToggle.GetComponent<UIToggle>().value = false;
                callToggle.GetComponent<UIToggle>().value = false;
                IsAutoSeeOrFold = false;
                IsAutoCall = false;

                if (MaxBetNum == 0)
                {
                    Dictionary<string, object> data = new Dictionary<string, object>() { { "gold", 0 } };
                    App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.Bet, data);
                    ShowNothing();
                }
                else
                {
                    App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.Fold, null);
                    ShowNothing();
                }

                return true;
            }
            //自动:跟注
            if (IsAutoCall)
            {
                seeToggle.GetComponent<UIToggle>().value = false;
                callToggle.GetComponent<UIToggle>().value = false;
                IsAutoSeeOrFold = false;
                IsAutoCall = false;
                var self = App.GameData.GetPlayer<PlayerPanel>();
                Dictionary<string, object> data = new Dictionary<string, object>() { { "gold", (int)(self.Coin - (MaxBetNum - self.BetMoney) > 0 ?
                                                                                         MaxBetNum - self.BetMoney: self.Coin) } };
       
                App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.Bet, data);
                ShowNothing();

                return true;
            }

            return false;
        }



        /// <summary>
        /// 显示自动界面
        /// </summary>
        public void ShowAuto()
        {
            Tools.GobShowOnlyOne(Pages, "AutoOption");
        }
        /// <summary>
        /// 显示结束时是否翻牌
        /// </summary>
        public void ShowOver()
        {
            Tools.GobShowOnlyOne(Pages, "GameOverOption");
        }
        /// <summary>
        /// 弃牌后不显示按键
        /// </summary>
        public void ShowNothing()
        {
            Tools.GobShowOnlyOne(Pages, "");
        }
        /// <summary>
        /// 根据自己的状态显示界面
        /// </summary>
        public void ShowSelfType()
        {

            switch (App.GameData.GetPlayer<PlayerPanel>().CurGameType)
            {
                case PlayerGameType.None:
                    //ShowAuto();
                    break;
                case PlayerGameType.SeePoker:
                    ShowAuto();
                    break;
                case PlayerGameType.Call:
                    ShowAuto();
                    break;
                case PlayerGameType.Fold:
                    ShowOver();
                    break;
                case PlayerGameType.AllIn:
                    ShowOver();
                    break;
            }
        }

        /// <summary>
        /// 初始化按键监听
        /// </summary>
        public void InitOnClick()
        {
            foreach (SpeakButton btnid in Enum.GetValues(typeof(SpeakButton)))
            {
                foreach (GameObject btn in Buttons)
                {
                    if (btn.name.Equals(btnid.ToString()))
                    {
                        Tools.NguiAddOnClick(btn, OnClickListener, (int)btnid);
                        break;
                    }
                }
            }
        }

        protected void OnClickListener(GameObject gob)
        {
            SpeakButton btnid = (SpeakButton)UIEventListener.Get(gob).parameter;
            Dictionary<string, object> data;
            var mgr = App.GetGameManager<GangWuGameManager>();
            var gdata = App.GetGameData<GangwuGameData>();
            var self = gdata.GetPlayer<PlayerPanel>();
            switch (btnid)
            {
                case SpeakButton.AddBet_Btn:

                    GameObject page = Tools.GobShowOnlyOne(Pages, "AddBet");
                    int addBet = MaxBetNum - self.BetMoney;
                    
                    AddBetSum = addBet;
                    AdvanceBet(addBet);         //通知服务器,开启预加注

                    mgr.BetMgr.RefreshAddBetBtns();
                    Transform btn = page.transform.FindChild("Allin_Btn");

                    var selfCoin = self.Coin;
                    bool couldAllin = gdata.CardCount > 3 && selfCoin > 0;

                    Tools.SetBtnCouldClick(btn.gameObject, couldAllin && selfCoin > 0);
                    break;

                case SpeakButton.Call_Btn:
                    data = new Dictionary<string, object>() { { "gold", MaxBetNum - self.BetMoney } };
                    App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.Bet, data);
                    
                    break;
                case SpeakButton.Fold_Btn:
                    App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.Fold, null);
                    
                    break;
                case SpeakButton.SeePoker_Btn:
                    data = new Dictionary<string, object>() { { "gold", 0 } };
                    App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.Bet, data);
                    
                    break;
                case SpeakButton.Allin_Btn:

                    int leastRoomGold = gdata.LeastRoomGold;
                    int betGold = leastRoomGold + gdata.LeastGoldPanel.BetMoney  - self.BetMoney;
                    
                    if (betGold < 0)
                    {
                        YxDebug.Log("==== Allin Gold less than zero , please check !! ====");
                        return;
                    }

                    data = new Dictionary<string, object>() { { "gold", betGold } };
                    App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.Bet, data);
                    break;

                case SpeakButton.SeePokerOrFold_Toggle:
                    IsAutoSeeOrFold = !IsAutoSeeOrFold;
                    break;

                case SpeakButton.Call_Toggle:
                    IsAutoCall = !IsAutoCall;
                    break;

                case SpeakButton.CloseAdd_Btn:
                    Tools.GobShowOnlyOne(Pages, "Speak");
                    break;

                case SpeakButton.BackBet_Btn:

                    int back = _betRemenber.Pop();
                    AddBetSum -= back;
                    mgr.BetMgr.RefreshAddBetBtns();
                   

                    break;

                case SpeakButton.GoBet_Btn:

                    data = new Dictionary<string, object>() { { "gold", AddBetSum } };
                    App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.Bet, data);
                    break;

                case SpeakButton.Cancel_Btn:

                    AdvanceBet(-1);
                    AddBetSum = 0;
                    Tools.GobShowOnlyOne(Pages, "Speak");

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        /// <summary>
        /// 发送预加注信息给服务器
        /// </summary>
        /// <param name="addVal">预加注的值,-1为关闭功能</param>
        void AdvanceBet(int addVal)
        {
            //通知服务器,取消预加注
            var data = new Dictionary<string, object>() { { "gold", addVal } };
            App.GetRServer<GangWuGameServer>().SendRequest(GameRequestType.AdvanceBet, data);
        }

        /// <summary>
        /// 重置信息
        /// </summary>
        public void Reset()
        {
            MaxBetNum = 0;
            ShowNothing();

            GameObject seeToggle = Tools.GobSelectName(Buttons, SpeakButton.SeePokerOrFold_Toggle.ToString());
            GameObject callToggle = Tools.GobSelectName(Buttons, SpeakButton.Call_Toggle.ToString());
            seeToggle.GetComponent<UIToggle>().value = false;
            callToggle.GetComponent<UIToggle>().value = false;
            IsAutoSeeOrFold = false;
            IsAutoCall = false;
        }


        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected enum SpeakButton
        {
            AddBet_Btn,
            Call_Btn,
            Fold_Btn,
            SeePoker_Btn,
            EnterAddBet_Btn,
            HalfPool_Btn,
            TwoThirdsPool_Btn,
            AllPool_Btn,
            Allin_Btn,
            SeePokerOrFold_Toggle,
            Call_Toggle,
            CloseAdd_Btn,
            BackBet_Btn,        //弹出上一次下注的筹码
            GoBet_Btn,          //确认下注
            Cancel_Btn,         //取消下注,返回speak菜单
            Length,             //长度一定为最后一个
        }
    }
}
