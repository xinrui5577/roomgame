using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;
using System.Collections;

namespace Assets.Scripts.Game.mx97
{
    public class MenuContainer : MonoBehaviour
    {
        public PrizeInfoWindow ThePrizeInfoWindow;
        public JackpotWindow TheJackpotWindow;

        /// <summary>
        /// 
        /// </summary>
        public GameObject StartTipObj;
        public GameObject ScoreTipObj;
        public UILabel ScoreLabel;

        private GameObject _mBtnAutoStart;
        private GameObject _mBtnUpScore;
        private GameObject _mBtnStart;
        private GameObject _mBtnBet;
        private GameObject _mBtnDownScore;

        private bool _mIsStarting;

        private bool _mIsAutoStart;

        public float autoIntervalTime = 1.5f;

        // Use this for initialization
        protected void Start()
        {
            var eventCenter = Facade.EventCenter;
            eventCenter.AddEventListener<Mx97EventType, object>(Mx97EventType.SwitchBtnWhenStart, OnSwitchBtnWhenStart);
            eventCenter.AddEventListener<Mx97EventType, object>(Mx97EventType.SwitchBtnWhenStop, OnSwitchBtnWhenStop);

            _mBtnAutoStart = transform.FindChild("BtnAuto").gameObject;
            _mBtnUpScore = transform.FindChild("BtnUpScore").gameObject;
            _mBtnStart = transform.FindChild("BtnStart").gameObject;
            _mBtnBet = transform.FindChild("BtnBet").gameObject;
            _mBtnDownScore = transform.FindChild("BtnDownScore").gameObject;
            if (ScoreTipObj != null)
            {
                ScoreTipObj.SetActive(true);
            }
            if (ScoreTipObj != null)
            {
                ScoreTipObj.SetActive(false);
            }

            var gdata = App.GetGameData<Mx97GlobalData>();
            if (_mBtnUpScore != null)
            {
                var longBtn = _mBtnUpScore.GetComponent<BtnLongPress>();
                if (longBtn != null)
                {
                    longBtn.LongAction += gdata.AddCurScore;
                }
            }
            if (_mBtnDownScore != null)
            {
                var longBtn = _mBtnDownScore.GetComponent<BtnLongPress>();
                if (longBtn != null)
                {
                    longBtn.LongAction += gdata.DedCurScore;
                }
            }

            UIEventListener.Get(_mBtnAutoStart).onClick = OnClicked;
            UIEventListener.Get(_mBtnStart).onClick = OnClicked;
            UIEventListener.Get(_mBtnBet).onClick = OnClicked;
//            UIEventListener.Get(_mBtnUpScore).onClick = OnClicked;
            UIEventListener.Get(_mBtnDownScore).onClick = OnClicked;
        }

        void OnClicked(GameObject go)
        {
            var musicMgr = Facade.Instance<MusicManager>();
            var eventCenter = Facade.EventCenter;
            if (go == _mBtnAutoStart)
            {
                _mIsAutoStart = true;
                SendStartRequest();
                musicMgr.Play("InsertIcon");
            } 
            else if (go == _mBtnStart)
            {
                SendStartRequest();
                musicMgr.Play("InsertIcon");
            }
            else if (go == _mBtnBet)
            {
                musicMgr.Play("InsertIcon");
                App.GetGameData<Mx97GlobalData>().ChangeAnteRate();
                eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshBetScore);
                eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.ChangeLineScore);
            }
//            else if (go == _mBtnUpScore)
//            {
//                var gdata = App.GetGameData<Mx97GlobalData>();
//                gdata.AddCurScore();
//                eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshCurScore);
//                musicMgr.Play("InsertIcon");
//            }
            else if (go == _mBtnDownScore)//减分
            {
                var gdata = App.GetGameData<Mx97GlobalData>();
                gdata.DedCurScore();
                eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshCurScore);
                musicMgr.Play("InsertIcon");
            }
        }

        public void AddScore()
        {
            var musicMgr = Facade.Instance<MusicManager>();
            var eventCenter = Facade.EventCenter;
            var gdata = App.GetGameData<Mx97GlobalData>();
            gdata.AddCurScore();
            eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshCurScore);
            musicMgr.Play("InsertIcon");
        } 

        private void OnSwitchBtnWhenStart(object obj)
        {
            _mBtnAutoStart.GetComponent<UIButton>().isEnabled = false;
            _mBtnUpScore.GetComponent<UIButton>().isEnabled = false;
            _mBtnBet.GetComponent<UIButton>().isEnabled = false;
            _mBtnDownScore.GetComponent<UIButton>().isEnabled = false;

            _mBtnStart.GetComponent<UIButton>().normalSprite = "5";
            _mBtnStart.GetComponent<UIButton>().hoverSprite = "5";
            _mBtnStart.GetComponent<UIButton>().pressedSprite = "6";

            if (StartTipObj != null)
            {
                StartTipObj.SetActive(false);
            }

            Invoke("OnEnableStartBtn", 1);
        }
        protected void OnEnableStartBtn()
        {
            _mBtnStart.GetComponent<UIButton>().isEnabled = true;
        }

        private void OnSwitchBtnWhenStop(object obj)
        { 
            //刷按钮
            StartCoroutine(TimerToSwitchBtn(0, false));

            var gdata = App.GetGameData<Mx97GlobalData>();
            var mGold = gdata.StartData.MGotJackpotGlod;
            //var realGold = mGold - App.GetGameData<Mx97GlobalData>().AnteRateAll * App.ShowGoldRate;

            if (ScoreTipObj != null)
            {
                ScoreTipObj.SetActive(true);
            }
            if (ScoreLabel != null)
            {
                ScoreLabel.text = YxUtiles.GetShowNumberForm(mGold);
            }

            //控制是否显示结算
            if (0 >= mGold)
            {
                if (_mIsAutoStart)
                    StartCoroutine(TimerToSwitchBtn(.5f, true));

                return;
            }

            //if (_mIsAutoStart) { _mIsAutoStart = false; }   --不会终止自动

            if (TheJackpotWindow != null)
            {
                TheJackpotWindow.Show();
                TheJackpotWindow.UpdateView(mGold);

                if (_mIsAutoStart)
                {
                    _mIsAutoStart = false;

                    if (!BigWin.getInstance().IsBigWin())
                    {
                        StartCoroutine(TheJackpotWindow.customHide(autoIntervalTime));
                        StartCoroutine(TimerToSwitchBtn(autoIntervalTime, true));
                    }
                }
            }
        }
        protected IEnumerator TimerToSwitchBtn(float waitTime, bool auto)
        {
            _mBtnAutoStart.GetComponent<UIButton>().isEnabled = true;
            _mBtnUpScore.GetComponent<UIButton>().isEnabled = true;
            _mBtnStart.GetComponent<UIButton>().isEnabled = true;
            _mBtnBet.GetComponent<UIButton>().isEnabled = true;
            _mBtnDownScore.GetComponent<UIButton>().isEnabled = true;

            _mBtnStart.GetComponent<UIButton>().normalSprite = "2";
            _mBtnStart.GetComponent<UIButton>().hoverSprite = "2";
            _mBtnStart.GetComponent<UIButton>().pressedSprite = "3";

            _mIsStarting = false;

            yield return new WaitForSeconds(waitTime);

            _mIsAutoStart = auto;

            if (_mIsAutoStart) { SendStartRequest(); }
        }

        // 发送开始游戏请求
        private void SendStartRequest()
        {
            var eventCenter = Facade.EventCenter;
            if (ScoreTipObj != null)
            {
                ScoreTipObj.SetActive(false);
            }
            if (_mIsStarting)
            {
                eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.StopScrollAni);
                _mBtnStart.GetComponent<UIButton>().isEnabled = false;

                if (_mIsAutoStart)
                    _mIsAutoStart = false;
            }
            else
            {
                var gdata = App.GetGameData<Mx97GlobalData>();
                var curScore = gdata.CurScore;
                var betScore = gdata.CurBet;
                if (curScore < betScore)
                {
                    if (!GameObject.FindWithTag("Tips"))
                        YxMessageTip.Show("当前分数不能低于押注分数，请上分！");

                    if (_mIsAutoStart)
                        _mIsAutoStart = false;
                    return;
                }
                if (curScore > gdata.GetPlayer().Coin)
                {
                    if (!GameObject.FindWithTag("Tips"))
                        YxMessageTip.Show("当前分数已超出上限！");

                    if (_mIsAutoStart)
                        _mIsAutoStart = false;
                    return;
                }

                gdata.AddCurScore(-betScore);
                eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.RefreshCurScore);

                // 发送请求数据
                SFSObject data = SFSObject.NewInstance();
                data.PutInt(RequestKey.KeyType, 1);

                data.PutInt("ante", gdata.CurAnte);
                App.GetRServer<Mx97RemoteController>().SendGameRequest(data);

                eventCenter.DispatchEvent<Mx97EventType, object>(Mx97EventType.InitGameResult);
                _mBtnStart.GetComponent<UIButton>().isEnabled = false;
            }
            _mIsStarting = !_mIsStarting;
        }
    }
}
