using System;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Tbs
{
    public class BankerBtnMgr : MonoBehaviour
    {
        /// <summary>
        /// 父节点
        /// </summary>
        public GameObject BG;
        /// <summary>
        /// 按键
        /// </summary>
        public UIButton[] Btns;

        protected void Start()
        {
            for (int i = 0; i < Btns.Length; i++)
            {
                Tools.NguiAddOnClick(Btns[i].gameObject, OnClickListener, i);
            }
        }

        /// <summary>
        /// 是否自动
        /// </summary>
        public bool IsAuto {
            set
            {
                _isAuto = value;
                Btns[2].normalSprite = _isAuto ? "btn_auto_p" : "btn_auto_n";
            }
            get { return _isAuto; }
        }

        private bool _isAuto;

        public void OpenPanel(BankerBtn btn)
        {
            BG.SetActive(true);

            SetBtn(btn);
        }
        /// <summary>
        /// 当前按键状态
        /// </summary>
        public BankerBtn CurBanker;

        public void SetBtn(BankerBtn btn)
        {
            CurBanker = btn;

            for (int i = 0; i < Btns.Length; i++)
            {
                if (i == (int)BankerBtn.Auto)
                {
                    continue;
                }

                if (btn == BankerBtn.None)
                {
                    Btns[i].isEnabled = false;
                }
                else if (i == (int)btn)
                {
                    Btns[i].isEnabled = true;
                }
                else
                {
                    Btns[i].isEnabled = false;
                }
            }
            //自动
            if (!_isAuto)
            {
                return;
            }
            var gmanager = App.GetGameManager<TbsGameManager>();
            switch (btn)
            {
                case BankerBtn.CutPoker:
                    gmanager.PokerMgr.SendBeginCut();
                    break;
                case BankerBtn.RollDice:
                    gmanager.DiceManager.SendRollDice();
                    break;
            }
        }

        public void ClosePanel()
        {
            BG.SetActive(false);
        }

        /// <summary>
        /// 按键点击事件
        /// </summary>
        /// <param name="gob"></param>
        public void OnClickListener(GameObject gob)
        {
            var type = (BankerBtn)UIEventListener.Get(gob).parameter;

            var gmanager = App.GetGameManager<TbsGameManager>();
            switch (type)
            {
                case BankerBtn.CutPoker:
                    gmanager.PokerMgr.SendBeginCut();
                    break;
                case BankerBtn.RollDice:
                    gmanager.DiceManager.SendRollDice();
                    break;
                case BankerBtn.Auto:
                    _isAuto = !_isAuto;
                    Btns[2].normalSprite = _isAuto ? "btn_auto_p" : "btn_auto_n";

                    if (_isAuto)
                    {
                        switch (CurBanker)
                        {
                            case BankerBtn.CutPoker:
                                gmanager.PokerMgr.SendBeginCut();
                                break;
                            case BankerBtn.RollDice:
                                gmanager.DiceManager.SendRollDice();
                                break;
                        }
                        App.GetGameData<TbsGameData>().GetPlayer<TbsPlayer>().UserBetPoker.ClickOpen();
                    }
                
                    break;
                case BankerBtn.ChangeBanker:
                    Debug.LogAssertion("发送切锅请求");
                    App.GetRServer<TbsRemoteController>().SendRequest(GameRequestType.Banker, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

    
    }

    public enum BankerBtn
    {
        CutPoker, //切牌
        RollDice, //掷骰子
        Auto, //自动
        ChangeBanker, //切锅
        None, //无
    }
}