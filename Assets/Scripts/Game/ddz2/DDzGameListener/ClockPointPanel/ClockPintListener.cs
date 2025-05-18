using UnityEngine;
using System.Collections;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDz2Common;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel;
using YxFramwork.Framework.Core;


namespace Assets.Scripts.Game.ddz2.DDzGameListener.ClockPointPanel
{
    public class ClockPintListener : ServEvtListener
    {

        [SerializeField]
        protected GameObject ClockGob;


        [SerializeField]
        protected UILabel CuntDownLabel;

        /// <summary>
        /// 无限的标识
        /// </summary>
        [SerializeField]
        protected GameObject InfinitySprite;
  

        private int _countingTime;

        private bool _isCounting;

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeGrabSpeaker, OnTypeGrabSpeaker);
            //Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOneRoundOver, OnTypeOneRoundOver);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypePass, OnTypePass);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFirstOut, OnTypeFirstOut);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnRejoinGame);
        }

        private void OnTypeGrabSpeaker(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
            int cd = GetCountingTime(args, 10);
            ShowPointAndCuntDown(seat, cd);
        }
       
        private void HideAllGobs()
        {
            ClockGob.gameObject.SetActive(false);
        }


        private void StopCountDown()
        {
            _isCounting = false;
            StopAllCoroutines();
        }



        public override void RefreshUiInfo()
        {
            
        }


        private void OnRejoinGame(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            //没人行动，隐藏所有
            if (!data.ContainsKey(NewRequestKey.KeyCurrp))
            {
                HideAllGobs();
                return;
            }

            int cd = data.ContainsKey("cd") ? data.GetInt("cd") : 10;
            ShowPointAndCuntDown(data.GetInt(NewRequestKey.KeyCurrp), cd);
        }




        /// <summary>
        /// 当确定地主后，看自己是不是地主，来判断是否显示按钮
        /// </summary>
        /// <param name="args"></param>
        private void OnTypeFirstOut(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int defCd = data.ContainsKey(NewRequestKey.KeyJiaBei) && data.GetBool(NewRequestKey.KeyJiaBei) ? 5 : 10;

            int seat = data.GetInt(RequestKey.KeySeat);
            int cd = GetCountingTime(args, defCd);
            ShowPointAndCuntDown(seat, cd);
        }

       

        /// <summary>
        /// 当有人出牌时
        /// </summary>
        /// <param name="args"></param>
        private void OnTypeOutCard(DdzbaseEventArgs args)
        {
            AfterSomeBodyAction(args.IsfObjData.GetInt(RequestKey.KeySeat), GetCountingTime(args, 10));
        }

        int GetCountingTime(DdzbaseEventArgs args,int defult)
        {
            var data = args.IsfObjData;
            return data.ContainsKey("cd") ? data.GetInt("cd") : defult;
        }

        /// <summary>
        /// 有人pass的时候
        /// </summary>
        /// <param name="args"></param>         
        private void OnTypePass(DdzbaseEventArgs args)
        {
            AfterSomeBodyAction(args.IsfObjData.GetInt(RequestKey.KeySeat), GetCountingTime(args, 10));
        }


        /// <summary>
        /// 当游戏结算时
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnTypeOneRoundOver(DdzbaseEventArgs args)
        {
            HideAllGobs();
            StopCountDown();
        }


        /// <summary>
        /// 当某人行动时要进行的动作
        /// </summary>
        private void AfterSomeBodyAction(int playerSeat,int time)
        {
            var gdata = App.GetGameData<DdzGameData>();
            int next = gdata.GetPlayer<DdzPlayer>(playerSeat, true).GetLaterHandSeat();

            var player = gdata.GetPlayer<DdzPlayer>(next, true);
            
            SetClockPos(player.ClockParent);
            BeginCountDown(time);
        }

        private void SetClockPos(Transform parent)
        {
            if (parent == null)
            {
                return;
            }

            var tran = ClockGob.transform;
            tran.parent = parent;
            tran.localPosition = Vector3.zero;
            tran.localScale = Vector3.one;
        }


        /// <summary>
        /// 箭头指向方位和倒计时开始
        /// </summary>
        /// <param name="playerSeat"></param>
        /// <param name="time"></param>
        private void ShowPointAndCuntDown(int playerSeat, int time)
        {
            var gdata = App.GetGameData<DdzGameData>();
            var player = gdata.GetPlayer<DdzPlayer>(playerSeat, true);
            SetClockPos(player.ClockParent);
            BeginCountDown(time);
        }


        void BeginCountDown(int time)
        {
            ResetClockRock();
            _countingTime = time > 99 ? 15 : time;
            CuntDownLabel.text = _countingTime.ToString();
            ClockGob.SetActive(true);

            if (_isCounting) return;
            _isCounting = true;
            StartCoroutine(ReClock());
        }


        private IEnumerator ReClock()
        {
            CuntDownLabel.gameObject.SetActive(true);
            while (_isCounting && _countingTime > 0)
            {
                bool isInfinity = _countingTime > 99;
                string timeText = isInfinity ? _countingTime%2 > 0 ? "- " : " -" : _countingTime.ToString("00");
                //InfinitySprite.SetActive(isInfinity);
                //CuntDownLabel.gameObject.SetActive(!isInfinity);
                CuntDownLabel.text = timeText;
                if (_countingTime < 4)
                {
                    Facade.EventCenter.DispatchEvent<string, DdzbaseEventArgs>(GlobalConstKey.KeyRemind);
                    //Facade.Instance<MusicManager>().Play("k_remind");
                }
                yield return new WaitForSeconds(1);
                _countingTime--;
            }
            CuntDownLabel.text = "00";
            _isCounting = false;
            ClockRock();
        }


        void ResetClockRock()
        {
            var tweens = ClockGob.GetComponents<UITweener>();
            int len = tweens.Length;
            for (int i = 0; i < len; i++)
            {
                var tween = tweens[i];
                tween.ResetToBeginning();
                tween.enabled = false;
            }
        }

        void ClockRock()
        {
            var tweens = ClockGob.GetComponents<UITweener>();
            int len = tweens.Length;
            for (int i = 0; i < len; i++)
            {
                tweens[i].PlayForward();
            }
        }
    }
}