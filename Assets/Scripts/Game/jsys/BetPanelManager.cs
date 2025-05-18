using com.yxixia.utile.YxDebug;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.jsys
{
    public class BetPanelManager : MonoBehaviour
    {
        //我的金币

        public Text MyMoneyText;
        //彩金

        public Text WiningText;
        //得分

        public Text GetMoneyText;
        //切换筹码

        public Text ChipText;
        //下注各动物按钮

        public Button[] BetButtons;
        //清空按钮

        public Button ClearButton;
        //续投按钮

        public Button GoOnButton;
        //各区域押注数量

        public Text[] BetTexts;
        //各区域倍数

        public Text[] MultipleTexts;
        //临时假数据，切换筹码

        public Transform Uitf;

        public Transform Hidetf;

        public Vector3 RealHideTf;

        protected void Start()
        {
            _bb = new int[12];
            ChipText.transform.parent.GetComponent<Button>().onClick.AddListener(OnClickBtn);
        }
        //按钮事件
        private void OnClickBtn()
        {
            ClickBtn();
        }

        public void FreshBtn()
        {
            ClickBtn();
        }

        private bool ClickBtn()
        {
            var panDuan = false;
            var gdata = App.GetGameData<JsysGameData>();
            if (gdata.StartBet)
            {
                var anteRate = gdata.AnteRate;
                if (anteRate[BetIndex] > gdata.UserMoney)
                {
                    ShowBetButton(false);
                }
                else
                {
                    ShowBetButton(true);
                    panDuan = true;
                }
            }
            return panDuan;
        }

        //新一把游戏开始
        public void GameBeginXizhu()
        {
            //Debug.Log("所有游戏状态回到最初");
            var gdata = App.GetGameData<JsysGameData>();
            var gameMgr = App.GetGameManager<JsysGameManager>();
            var musicMgr = Facade.Instance<MusicManager>();
            musicMgr.Stop();
            //隐藏结算UI
            //TurnGroupsManager.Instance.GameConfig.TurnTableState = (int)GameConfig.GoldSharkState.Bet;  
            gameMgr.ResultUIMgr.HideJieSuanUI();
            gameMgr.AnimationMgr.HideBetPanel();
            //倍率显示
            gameMgr.TurnGroupsMgr.GameConfig.Imultiplying = gdata.Multiplying;
            //筹码显示归零
            //ShowChipTextUI(Chip);自己注释的
            ButtonUIInit();
            SetMoney(gdata.UserMoney);
            //总筹码显示           
            gdata.Gold = 0;
            gameMgr.ModelMgr.ChangeToHaidi();
        }
        //新一把游戏下注界面清零
        public void ButtonUIInit()
        {
            for (var i = 0; i < 12; i++)
            {
                _bb[i] = 0;
                SetBetText(0, BetTexts[i]);
            }
        }

        private void SetBetText(int bet,Text label)
        {
            label.text = YxUtiles.GetShowNumberToString(bet);
        }

        //显示彩金Text
        public void ShowiWiningText(int iwining)
        {
            WiningText.text = YxUtiles.GetShowNumberToString(iwining);
        }
        //显示得分切换Text
        public void ShowIgetMoney(long igetmoney)
        {
            GetMoneyText.text = YxUtiles.GetShowNumberToString(igetmoney);
        }

        //游戏等待状态
        public void Gamewaitshow()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            musicMgr.Stop();
            musicMgr.Play("Xiazhu");
            var turnGroupMgr = App.GetGameManager<JsysGameManager>().TurnGroupsMgr;
            turnGroupMgr.GameConfig.TurnTableState = (int)GameConfig.GoldSharkState.Bet;
            ShowBetButton(false);
            //显示押注面板
            if (!turnGroupMgr.GameConfig.IsBetPanelOnShow)
            {
                ShowUI();
            }
            InitChips();
        }
        //倍率显示
        public void ShowImultiply(int[] imultiplying)
        {
            for (int i = 0; i < 12; i++)
            {
                MultipleTexts[i].text = "x" + imultiplying[i];
            }
        }

        /// <summary>
        ///显示押注可操作状态
        /// </summary>
        /// <param name="isCanBet"></param>
        public void ShowBetButton(bool isCanBet)
        {
            for (var i = 0; i < BetButtons.Length; i++)
            {
                BetButtons[i].interactable = isCanBet;
            }
            GoOnButton.interactable = isCanBet;
            ClearButton.interactable = isCanBet;
        }
        //显示金币
        public void SetMoney(long money)
        {
            if (money < 0) money = 0;
            MyMoneyText.text = YxUtiles.GetShowNumberToString(money);
        }

        //显示下注面板
        public void ShowUI()
        {
            transform.DOMove(Uitf.position, 1f).SetEase(Ease.OutBounce);
            App.GetGameManager<JsysGameManager>().TurnGroupsMgr.GameConfig.IsBetPanelOnShow = true;
        }
        //隐藏下注面板
        public void HideUI()
        {
            transform.DOMove(Hidetf.position, 0.5f);
            App.GetGameManager<JsysGameManager>().TurnGroupsMgr.GameConfig.IsBetPanelOnShow = false;
        }

        /// <summary>
        /// 发送下注
        /// </summary>
        public void SendBet()
        {
            bool isNull = false;
            foreach (int i in _bb)
            {
                if (i != 0)
                {
                    isNull = true;
                }
            }
            if (isNull)
            {
                for (int i = 0; i < _bb.Length; i++)
                {
                    _xuYa[i] = _bb[i];
                }
                App.GetRServer<GameServer>().ClickedToSend(_bb);
                App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
            }
            for (int index = 0; index < _bb.Length; index++)
            {
                _bb[index] = 0;
            }
            _isXuya = true;
        }
        //押注数据
        private int[] _bb;
        //续压数据
        private int[] _xuYa = new int[12];
        private bool _isClear;
        //押注数据显示
        public void ShowBetData(int num)
        {
            var anteRate = App.GameData.AnteRate;
            if (_isClear)
            {
                for (int i = 0; i < _bb.Length; i++)
                {
                    _bb[i] = _xuYa[i];
                }
                _isClear = false;
            }
            _bb[num] += anteRate[BetIndex];

            SetBetText(_bb[num], BetTexts[num]);
            SetMoney(App.GetGameData<JsysGameData>().UserMoney - anteRate[BetIndex]);
        }
        //下注控件
        private bool _isXuya = true;
        public void Beting(int num)
        {
            var anteRate = App.GameData.AnteRate;
            var gdata = App.GetGameData<JsysGameData>();
            var musicMgr = Facade.Instance<MusicManager>();
            if (num <= 11)
            {
                musicMgr.Play("Xiazhu");
                YxDebug.Log("num" + num);
                if (ClickBtn())
                {
                    var bet = anteRate[BetIndex];
                    if (gdata.UserMoney >= bet)
                    {
                        ShowBetData(num);
                        gdata.UserMoney -= bet;
                    }
                }
                _isXuya = true;
            }
            else if (num == 12)
            {
                musicMgr.Play("Xuya");

                for (var i = 0; i < _bb.Length; i++)
                {
                    var xuya = _xuYa[i];
                    if(gdata.UserMoney < xuya) { break;}
                    SetBetText(xuya, BetTexts[i]);
                    gdata.UserMoney -= xuya;
                    _bb[i] = xuya;
                    _isClear = true;
                }
                if (_isXuya)
                {
                    SetMoney(gdata.UserMoney);
                    _isXuya = false;
                }
            }
            else if (num == 13)
            {
                musicMgr.Play("Xiazhu");
                for (int index = 0; index < _bb.Length; index++)
                {
                    gdata.UserMoney += _bb[index];
                }
                ButtonUIInit();
                _isClear = false;
                SetMoney(gdata.UserMoney);
                _isXuya = true;
            }
        }
        //筹码数值的索引
        public int BetIndex;
        //切换筹码
        public void ChangeChips()
        {
            BetIndex++;
            var anteRate = App.GameData.AnteRate;
            if (BetIndex >= anteRate.Count)
            {
                BetIndex = 0;
            }
            ChipText.text = YxUtiles.GetShowNumberToString(anteRate[BetIndex]);
            Facade.Instance<MusicManager>().Play("Qiehuan");
        }

        public void InitChips()
        {
            BetIndex = -1;
            ChangeChips();
        }
    }
}
