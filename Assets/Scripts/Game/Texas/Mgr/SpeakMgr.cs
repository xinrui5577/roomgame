using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Assets.Scripts.Game.Texas.Main;
using Assets.Scripts.Game.Texas.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Texas.Mgr
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
        /// 池底选项
        /// </summary>
        public GameObject Pool;
        /// <summary>
        /// 其他标签页 加注,说话,结束选项,自动选项
        /// </summary>
        public GameObject[] Pages;
        /// <summary>
        /// 全部玩家最大下注值
        /// </summary>
        public long MaxBetNum;
        /// <summary>
        /// 允许下注的最大值
        /// </summary>
        [HideInInspector]
        public long AllowMax;
        /// <summary>
        /// 允许下注的最小值
        /// </summary>
        [HideInInspector]
        public long AllowMin;
        /// <summary>
        /// 池子总数
        /// </summary>
        [HideInInspector]
        public long PoolNum;
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
        /// 其他玩家下注的最大值
        /// </summary>
        public long OtherUserMaxBet;


        // Use this for initialization
        protected void Start()
        {
            ShowNothing();
            InitOnClick();
        }

        // Update is called once per frame
        protected void Update()
        {
            BarBtnUpdate();
        }

        /// <summary>
        /// 显示说话界面
        /// </summary>
        public void ShowSpeak()
        {
            var seeToggle = Tools.GobSelectName(Buttons, SpeakButton.SeePokerOrFold_Toggle.ToString());
            var callToggle = Tools.GobSelectName(Buttons, SpeakButton.Call_Toggle.ToString());

            //自动:看牌或弃牌
            if (IsAutoSeeOrFold)
            {
                seeToggle.GetComponent<UIToggle>().value = false;
                callToggle.GetComponent<UIToggle>().value = false;
                IsAutoSeeOrFold = false;
                IsAutoCall = false;

                if (MaxBetNum == 0)
                {
                    var data = new Dictionary<string, object> { { "gold", 0 } };
                    App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Bet, data);
                    ShowNothing();
                }
                else
                {
                    App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Fold, null);
                    ShowNothing();
                }
                return;
            }
            var gdata = App.GetGameData<TexasGameData>();
            var self = gdata.GetPlayer<PlayerPanel>();
            var selfInfo = self.GetInfo<YxBaseGameUserInfo>();
            //自动:跟注
            if (IsAutoCall)
            {
                seeToggle.GetComponent<UIToggle>().value = false;
                callToggle.GetComponent<UIToggle>().value = false;
                IsAutoSeeOrFold = false;
                IsAutoCall = false;
                var data = new Dictionary<string, object>() { { "gold", (int)(selfInfo.RoomCoin - (MaxBetNum - self.BetMoney) > 0 ? MaxBetNum - self.BetMoney : selfInfo.RoomCoin) } };
                App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Bet, data);
                ShowNothing();

                return;
            }

            var gob = Tools.GobShowOnlyOne(Pages, "Speak");

            //跟注是否可以点击
            var isCall = MaxBetNum - self.BetMoney <= selfInfo.RoomCoin;
            gob.transform.FindChild(SpeakButton.Call_Btn.ToString()).GetComponent<UIButton>().isEnabled = isCall;

            ShowPool(isCall);
            //显示跟注或看牌
            gob.transform.FindChild(SpeakButton.Call_Btn.ToString()).gameObject.SetActive(self.BetMoney < MaxBetNum);
            gob.transform.FindChild(SpeakButton.SeePoker_Btn.ToString()).gameObject.SetActive(self.BetMoney == MaxBetNum);
            //显示allin或者加注
            var isAllin = MaxBetNum * 2 - self.BetMoney >= selfInfo.RoomCoin;

            AllowMax = selfInfo.RoomCoin + self.BetMoney;
            //几倍池底限制
            var poolLimit = PoolNum * gdata.MaxPoolNum + PoolNum;
            if (gdata.MaxPoolNum != 0)
            {
                isAllin = isAllin && self.BetMoney + selfInfo.RoomCoin < poolLimit;
                AllowMax = AllowMax > poolLimit ? poolLimit : AllowMax;
            }
            AllowMax = AllowMax > OtherUserMaxBet ? OtherUserMaxBet : AllowMax;

            AllowMin = MaxBetNum * 2;
            AllowMin = AllowMin < AllowMax ? AllowMin : AllowMax;

            isAllin = isAllin && self.BetMoney + selfInfo.RoomCoin < OtherUserMaxBet;
           
            gob.transform.FindChild(SpeakButton.Allin_Btn.ToString()).gameObject.SetActive(isAllin);
            gob.transform.FindChild(SpeakButton.AddBet_Btn.ToString()).gameObject.SetActive(!isAllin && isCall && MaxBetNum != AllowMax);
        }
        /// <summary>
        /// 显示池底按键
        /// </summary>
        public void ShowPool(bool isCall)
        {
            OtherUserMaxBet = 0;
            var selfSeat = App.GameData.SelfSeat;
            var gdata = App.GetGameData<TexasGameData>();
            var playerList = gdata.PlayerList;
            var playerCount = playerList.Length;
            for (var i = 0; i < playerCount; i++)
            {
                var player = gdata.GetPlayer<PlayerPanel>(i);
                var playerInfo = player.GetInfo<YxBaseGameUserInfo>();
                if (playerInfo == null) continue;

                if (player.ReadyState && playerInfo.Seat != selfSeat && (playerInfo.RoomCoin + player.BetMoney) > OtherUserMaxBet)
                {
                    OtherUserMaxBet = playerInfo.RoomCoin + player.BetMoney;
                }
            }

            var grid = Pool.transform.FindChild("Grid");
            var self = gdata.GetPlayer<PlayerPanel>();
            var halfV = PoolNum >> 1;
            var half = halfV >= MaxBetNum * 2 && self.RoomCoin >= halfV && OtherUserMaxBet >= halfV;
            grid.FindChild(SpeakButton.HalfPool_Btn.ToString()).gameObject.SetActive(half);

            var twoThirdsV = (int)(PoolNum * (2f / 3f));
            var twoThirds = twoThirdsV >= MaxBetNum * 2 && self.RoomCoin >= twoThirdsV && OtherUserMaxBet >= twoThirdsV;
            grid.FindChild(SpeakButton.TwoThirdsPool_Btn.ToString()).gameObject.SetActive(twoThirds);
            var all = PoolNum >= MaxBetNum * 2 && self.RoomCoin >= PoolNum && OtherUserMaxBet >= PoolNum;
            grid.FindChild(SpeakButton.AllPool_Btn.ToString()).gameObject.SetActive(all);
            Pool.SetActive(isCall);
        }
        /// <summary>
        /// 显示自动界面
        /// </summary>
        public void ShowAuto()
        {
            var self = App.GameData.GetPlayerInfo();
            if (!self.State) { return;}
            Pool.SetActive(false);
            Tools.GobShowOnlyOne(Pages, "AutoOption");
        }
        /// <summary>
        /// 显示结束是否翻牌
        /// </summary>
        public void ShowOver()
        {
            Pool.SetActive(false);
            Tools.GobShowOnlyOne(Pages, "GameOverOption");
        }
        /// <summary>
        /// 弃牌后不显示按键
        /// </summary>
        public void ShowNothing()
        {
            Pool.SetActive(false);
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
                case PlayerGameType.SeePoker:
                case PlayerGameType.Call:
                    ShowAuto();
                    break;

                case PlayerGameType.Fold:
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
            /************进度条按键添加事件************/
            UIEventListener uiel = UIEventListener.Get(AddBtn.gameObject);
            uiel.onPress = OnBarBtnPress;
            uiel.parameter = 1;
            uiel = UIEventListener.Get(SubtractBtn.gameObject);
            uiel.onPress = OnBarBtnPress;
            uiel.parameter = 0;

        }

        protected void OnClickListener(GameObject gob)
        {
            SpeakButton btnid = (SpeakButton)UIEventListener.Get(gob).parameter;
            Dictionary<string, object> data;

            switch (btnid)
            {
                case SpeakButton.AddBet_Btn:
                    Tools.GobShowOnlyOne(Pages, "AddBet");
                    InitAdd(AllowMin, AllowMax);
                    break;
                case SpeakButton.Call_Btn:
                    {
                        data = new Dictionary<string, object>() { { "gold", (int)(MaxBetNum - App.GameData.GetPlayer<PlayerPanel>().BetMoney) } };
                        App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Bet, data);
                        ShowNothing();
                    }
                    break;
                case SpeakButton.Fold_Btn:
                    App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Fold, null);
                    ShowNothing();
                    break;
                case SpeakButton.SeePoker_Btn:
                    data = new Dictionary<string, object>() { { "gold", 0 } };
                    App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Bet, data);
                    ShowNothing();
                    break;
                case SpeakButton.EnterAddBet_Btn:
                    data = new Dictionary<string, object>() { { "gold", (int)(AddBetNum - App.GameData.GetPlayer<PlayerPanel>().BetMoney) } };
                    App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Bet, data);
                    ShowNothing();
                    break;
                case SpeakButton.HalfPool_Btn:
                    data = new Dictionary<string, object>() { { "gold", (int)(PoolNum >> 1) } };
                    App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Bet, data);
                    ShowNothing();
                    break;
                case SpeakButton.TwoThirdsPool_Btn:
                    data = new Dictionary<string, object>() { { "gold", (int)(PoolNum * (2f / 3f)) } };
                    App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Bet, data);
                    ShowNothing();
                    break;
                case SpeakButton.AllPool_Btn:
                    data = new Dictionary<string, object>() { { "gold", (int)PoolNum } };
                    App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Bet, data);
                    ShowNothing();
                    break;
                case SpeakButton.Allin_Btn:
                    data = new Dictionary<string, object>() { { "gold", (int)App.GameData.GetPlayer<PlayerPanel>().RoomCoin } };
                    App.GetRServer<TexasGameServer>().SendRequest(GameRequestType.Bet, data);
                    ShowOver();
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
            }
        }

        
        /// <summary>
        /// 根据名字返回gob对象
        /// </summary>
        /// <returns></returns>
        public static GameObject GetGobByName(GameObject[] gobs, string gobName)
        {
            int len = gobs.Length;
            for (int i = 0; i < len; i++)
            {
                var go = gobs[i];
                if (gobName == go.name)
                {
                    return go;
                }
            }
            return null;
        }

        /// <summary>
        /// 重置信息
        /// </summary>
        public void Reset()
        {
            MaxBetNum = 0;
            PoolNum = 0;
            GameObject seeToggle = Tools.GobSelectName(Buttons, SpeakButton.SeePokerOrFold_Toggle.ToString());
            GameObject callToggle = Tools.GobSelectName(Buttons, SpeakButton.Call_Toggle.ToString());
            seeToggle.GetComponent<UIToggle>().value = false;
            callToggle.GetComponent<UIToggle>().value = false;
            IsAutoSeeOrFold = false;
            IsAutoCall = false;
        }

        #region 加注逻辑

        /// <summary>
        /// 最小加注值
        /// </summary>
        [HideInInspector]
        public long MinAdd;

        /// <summary>
        /// 最大加注值
        /// </summary>
        [HideInInspector]
        public long MaxAdd;

        /// <summary>
        /// 加注显示
        /// </summary>
        public UILabel AddLabel;

        /// <summary>
        /// 加注值
        /// </summary>
        public long AddBetNum;

        /// <summary>
        /// 加注进度条
        /// </summary>
        public UIProgressBar AddBar;

        /// <summary>
        /// 进度条按键速度
        /// </summary>
        public float BarBtnSpeed;

        /// <summary>
        /// 增加进度
        /// </summary>
        public UIButton AddBtn;

        /// <summary>
        /// 减少进度
        /// </summary>
        public UIButton SubtractBtn;

        /// <summary>
        /// 初始化下注
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void InitAdd(long min, long max)
        {
            if (min >= max)
            {
                YxDebug.Log("min大于max");
            }
            var gdata = App.GetGameData<TexasGameData>();
            MinAdd = min;

            MaxAdd = max;
            AddBar.value = 0;

            AddBetNum = MinAdd;
            if (AddBetNum != MaxAdd)
            {
                AddBetNum -= AddBetNum % gdata.Ante;
            }
            AddLabel.text = YxUtiles.ReduceNumber(AddBetNum);
        }

        public void SetCurAddBet()
        {
            if (UIProgressBar.current == null)
            {
                return;
            }
            var gdata = App.GetGameData<TexasGameData>();
            var p = UIProgressBar.current.value;
            AddBetNum = (int)(p * (MaxAdd - MinAdd) + MinAdd);
            
            if (AddBetNum != MaxAdd && AddBetNum != MinAdd)
            {
                //对数值进行还原,用于YxUtiles.ReduceNumber能够显示正确的数值
                AddBetNum -= AddBetNum % gdata.Ante;
            }
            AddLabel.text = YxUtiles.ReduceNumber(AddBetNum);
        }

        /// <summary>
        /// 加减按键监听
        /// </summary>
        private void OnBarBtnPress(GameObject gob, bool state)
        {
            _pressState = state;
            _pressIndex = (int)UIEventListener.Get(gob).parameter;
        }

        private int _pressIndex;

        private bool _pressState;

        private void BarBtnUpdate()
        {
            if (!_pressState) { return; }
            switch (_pressIndex)
            {
                case 0:
                    AddBar.value = AddBar.value - BarBtnSpeed * Time.deltaTime <= _pressIndex ? _pressIndex : AddBar.value - BarBtnSpeed * Time.deltaTime;
                    break;
                case 1:
                    AddBar.value = AddBar.value + BarBtnSpeed * Time.deltaTime >= _pressIndex ? _pressIndex : AddBar.value + BarBtnSpeed * Time.deltaTime;
                    break;
            }
        }

        #endregion

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
            Length, //长度一定为最后一个
        }
    }
}
