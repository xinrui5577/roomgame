using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.duifen.ImgPress.Main;
using Assets.Scripts.Game.duifen.Tool;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;
#pragma warning disable 649

namespace Assets.Scripts.Game.duifen.Mgr
{
    /// <summary>
    /// 玩家说话管理类 说话:德州扑克轮到本玩家决定跟牌与否的简称
    /// </summary>
    public class SpeakMgr : MonoBehaviour
    {

        /// <summary>
        /// 记录开牌的下注额
        /// </summary>
        private int _maxBet;

        /// <summary>
        /// 显示开牌需要的下注额
        /// </summary>
        [SerializeField]
        private UILabel _kaipaiLabel;

        /// <summary>
        /// 显示自动跟住需要的下注额
        /// </summary>
        [SerializeField]
        private UILabel _autoFollowLabel;

        /// <summary>
        /// 按键数组
        /// </summary>
        public GameObject[] Buttons;

        /// <summary>
        /// 其他标签页 加注,说话,结束选项,自动选项
        /// </summary>
        public GameObject[] Pages;

        /// <summary>
        /// 本回合上一个下注额
        /// </summary>
        [HideInInspector]
        public int TurnBet;

        /// <summary>
        /// 是否能弃牌
        /// </summary>
        private bool _couldFold;

        public bool CouldFold { set { _couldFold = value; } }

        /// <summary>
        /// 上一次玩家下注额
        /// </summary>
        [HideInInspector]
        public int MinBetValue;

        public AddBetBtnItem[] AddBetBtns;

        /// <summary>
        /// 自动跟注
        /// </summary>
        public bool IsAutoFollow;


        // Use this for initialization
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void Start()
        {
            ShowNothing();
            InitOnClick();
        }

        public void RefreshBetSpeakBtns()
        {
            Tools.GobShowOnlyOne(Pages, "BetSpeakBtns");
            //SetItemActive(Pages, "BetSpeakBtns", true);               //显示所有按钮
            SetBtnState("AddBtn", false);
            SetBtnState("FollowBtn", false);
            SetBtnState("FoldBtn", _couldFold || App.GetGameManager<DuifenGameManager>().CurTurn > 3);
            SetBtnState("KaiPaiBtn", true);
            UpdateAutoFollowInfo();
        }

        /// <summary>
        /// 显示说话界面
        /// </summary>
        public virtual void ShowSpeak(GameRequestType rt)
        {
            YxDebug.Log("ShowSpeak rt === " + rt);
            if (rt == GameRequestType.None)
                return;

            RefreshBetSpeakBtns();

            if (IsAutoFollow)
            {
                AutoFollow();
                return;
            }

            switch (rt)
            {
                case GameRequestType.Cards:
                    var curTurn = App.GetGameManager<DuifenGameManager>().CurTurn;
                    bool finishCard = curTurn >= 3;   //是否发完牌
                    SetItemActive(Pages, "LastTurnBtns", finishCard);
                    SetItemActive(Pages, "AddBetBtns", true);
                    SetBtnState("AddBtn", true);
                    SetBtnState("KaiPaiBtn", false);
                    RefreshBetBtnsEnable();

                    if (finishCard)
                    {
                        SetItemActive(Buttons, "QiLiBtn", curTurn == 3);
                    }
                    break;

                case GameRequestType.Fold:
                case GameRequestType.Bet:

                    ShowBetBtns();

                    break;

                case GameRequestType.QiLi:

                    SetItemActive(Pages, "LastTurnBtns", true);
                    ShowBetBtns();
                    SetItemActive(Buttons, "QiLiBtn", true);
                    SetItemActive(Pages, "QiLiBtns", true);
                    SetBtnState("KaiPaiBtn", false);
                    break;
            }
        }



        void ShowBetBtns()
        {
            int curTurn = App.GetGameManager<DuifenGameManager>().CurTurn;

            SetItemActive(Pages, "AddBetBtns", false);       //只有在没玩家下注或者所有牌都发完的情况下,显示下注按钮
            SetItemActive(Pages, "LastTurnBtns", curTurn >= 3);
            SetBtnState("AddBtn", curTurn >= 3 || TurnBet == 0);
            SetBtnState("FollowBtn", TurnBet > 0 || curTurn > 3);
            SetItemActive(Buttons, "QiLiBtn", TurnBet == 0 && curTurn == 3);
            SetBtnState("KaiPaiBtn", curTurn > 3 || TurnBet > 0);
        }

        public void HideSpeak(GameRequestType rt)
        {
            switch (rt)
            {
                case GameRequestType.Bet:
                    SetItemActive(Pages, "AddBetBtns", false);
                    SetBtnState("AddBtn", false);
                    SetBtnState("FollowBtn", false);
                    SetItemActive(Pages, "LastTurnBtns", false);
                    SetItemActive(Pages, "QiLiBtns", false);
                    break;
            }
        }


        /// <summary>
        /// 重连时,刷新所有按钮的显示情况,只在重连时候调用
        /// 1,显示加注相关按钮;2,显示相关跟注按钮;4,显示起立相关按钮;8,显示开牌相关按钮;16,显示是否同意起立界面
        /// </summary>
        /// <param name="state">一组二进制状态数,对应显示相关状态按钮</param>
        /// <param name="isSpeaker">是否是当前说话玩家</param>
        public void RejionRefreshBtns(int state, bool isSpeaker)
        {
            //设置是否允许弃牌
            _couldFold = (state & 32) > 0;
            SetBtnState("FoldBtn", _couldFold);

            if (!App.GetGameData<DuifenGlobalData>().IsGameing || !isSpeaker)
            {
                ShowNothing();
                return;
            }

            RefreshBetSpeakBtns();
            bool finishCard = App.GetGameManager<DuifenGameManager>().CurTurn >= 3;

            //显示加注按钮
            if ((state & 1) != 0)
            {
                SetBtnState("AddBtn", true);
            }

            //显示跟注相关按钮
            if ((state & 2) != 0)
            {
                SetBtnState("FollowBtn", true);
                SetItemActive(Buttons, "QiLiBtn", false);
            }

            SetItemActive(Pages, "LastTurnBtns", finishCard);

            //显示起立相关按钮
            if ((state & 4) != 0)
            {
                SetItemActive(Buttons, "QiLiBtn", true);
                SetBtnState("Fold", false);
            }

            //显示开牌相关按钮
            SetBtnState("KaiPaiBtn", (state & 8) != 0);

            SetItemActive(Pages, "QiLiBtns", (state & 16) != 0);

            RefreshBetBtnsEnable();
        }


        void SetBtnState(string btnName, bool enable)
        {
            GameObject obj = Tools.GobGet(Buttons, btnName);
            if (obj == null)
            {
                YxDebug.Log(" === The obj ,name == " + btnName + " is null !! === ");
                return;
            }
            obj.GetComponent<BoxCollider>().enabled = enable;
            obj.GetComponent<UIButton>().state = enable ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled;
        }

        void SetItemActive(GameObject[] group, string btnName, bool active)
        {
            GameObject obj = Tools.GobGet(group, btnName);
            if (obj == null)
            {
                YxDebug.Log(" === The obj ,name == " + btnName + " is null !! === ");
                return;
            }
            obj.SetActive(active);
        }



        /// <summary>
        /// 弃牌后不显示按键
        /// </summary>
        public void ShowNothing()
        {
            //Pool.SetActive(false);
            Tools.GobShowOnlyOne(Pages, "");
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
                    }
                }
            }
            /************进度条按键添加事件************/
            //UIEventListener uiel = UIEventListener.Get(AddBtn.gameObject);
            //uiel.onPress = OnBarBtnPress;
            //uiel.parameter = 1;
            //uiel = UIEventListener.Get(SubtractBtn.gameObject);
            //uiel.onPress = OnBarBtnPress;
            //uiel.parameter = 0;
        }

        protected void OnClickListener(GameObject gob)
        {
            SpeakButton btnid = (SpeakButton)UIEventListener.Get(gob).parameter;
            switch (btnid)
            {
                case SpeakButton.KaiPaiBtn:


                    Dictionary<string, object> data = new Dictionary<string, object>() { { "gold", _maxBet * 2 }, { "seat", App.GameData.SelfSeat } };
                    App.GetGameManager<DuifenGameManager>().SendRequest(GameRequestType.KaiPai, data);

                    break;

                case SpeakButton.FoldBtn:

                    FoldGame();

                    break;

                case SpeakButton.FollowBtn:

                    FollowBet();

                    break;

                case SpeakButton.AutoFollow:

                    IsAutoFollow = !IsAutoFollow;
                    RefreshAutoFollowBtn(gob);

                    break;

                case SpeakButton.QiLiBtn:

                    data = new Dictionary<string, object>() { { "gold", 0 }, { "seat", App.GameData.SelfSeat } };
                    App.GetGameManager<DuifenGameManager>().SendRequest(GameRequestType.QiLi, data);
                    _couldFold = true;

                    break;

                case SpeakButton.AddBtn:

                    RefreshBetBtnsEnable();
                    SetItemActive(Pages, "AddBetBtns", true);

                    break;
            }
        }

        void FoldGame()
        {
            App.GameData.GetPlayer<DuifenPlayerPanel>().PlayerType = 3;
            App.GetGameManager<DuifenGameManager>().SendRequest(GameRequestType.Fold, null);
        }

        void AutoFollow()
        {
            var manager = App.GetGameManager<DuifenGameManager>();

            int betVal = manager.CurTurn >= 3 ? AddBetBtns[AddBetBtns.Length - 1].BetValue : MinBetValue;

            App.GameData.GetPlayer<DuifenPlayerPanel>().PlayerType = 6;
            Dictionary<string, object> data = new Dictionary<string, object>() { { "gold", betVal }, { "seat", App.GameData.SelfSeat } };
            App.GetGameManager<DuifenGameManager>().SendRequest(GameRequestType.Bet, data);
        }

        void FollowBet()
        {
            var gdata = App.GameData;
            gdata.GetPlayer<DuifenPlayerPanel>().PlayerType = 6;
            Dictionary<string, object> data = new Dictionary<string, object>() { { "gold", MinBetValue }, { "seat", gdata.SelfSeat } };
            App.GetGameManager<DuifenGameManager>().SendRequest(GameRequestType.Bet, data);
        }


        /// <summary>
        /// 更新开牌下注额
        /// </summary>
        /// <param name="betVal"></param>
        public void UpdateKaiPaiInfo(int betVal)
        {
            _kaipaiLabel.text = App.GetGameData<DuifenGlobalData>().GetShowGoldValue(_maxBet * 2);
        }

        void UpdateAutoFollowInfo()
        {
            if (_autoFollowLabel == null)
                return;
            int bet = App.GetGameManager<DuifenGameManager>().CurTurn >= 3 ? AddBetBtns[AddBetBtns.Length - 1].BetValue : MinBetValue;
            _autoFollowLabel.text = App.GetGameData<DuifenGlobalData>().GetShowGoldValue(bet);
        }

        /// <summary>
        /// 更新开牌,上轮下注和自动跟注的筹码信息
        /// </summary>
        /// <param name="betVal"></param>
        public void UpdateBetInfo(int betVal)
        {

            MinBetValue = betVal;
            if (_maxBet >= betVal)
                return;

            _maxBet = betVal;
            UpdateKaiPaiInfo(betVal);
            UpdateAutoFollowInfo();
        }



        void RefreshAutoFollowBtn(GameObject gob = null)
        {
            if (gob == null)
            {
                gob = Tools.GobGet(Buttons, "AutoFollow");
            }

            SetBtnSprite(gob, IsAutoFollow ? "stopAuto" : "auto");
        }

        void SetBtnSprite(GameObject gob, string sprName)
        {
            gob.GetComponent<UISprite>().spriteName = sprName + "_up";
            var btn = gob.GetComponent<UIButton>();
            btn.normalSprite = sprName + "_up";
            btn.hoverSprite = sprName + "_over";
            btn.pressedSprite = sprName + "_over";
            btn.disabledSprite = string.Empty;
        }

        public void RefreshBetBtnsEnable()
        {
            var len = AddBetBtns.Length;
            for (int i = 0; i < len; i++)
            {
                AddBetBtns[i].SetBtnEnable(AddBetBtns[i].BetValue >= MinBetValue);
            }
        }



        /// <summary>
        /// 重置信息
        /// </summary>
        public void Reset()
        {
            MinBetValue = AddBetBtns[0].BetValue;
            _maxBet = 0;
            TurnBet = 0;
            UpdateAutoFollowInfo();
            ShowNothing();
            IsAutoFollow = false;
            RefreshAutoFollowBtn();
            _couldFold = false;
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        protected enum SpeakButton
        {
            FoldBtn,
            FollowBtn,
            KaiPaiBtn,
            AutoFollow,
            QiLiBtn,
            AddBtn,
        }
    }
}
