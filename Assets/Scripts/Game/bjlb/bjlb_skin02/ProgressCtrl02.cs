using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using System;


namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    public class ProgressCtrl02 : ProgressCtrl
    {

        /// <summary>
        /// 整时间
        /// </summary>
        float _totalCD;

        /// <summary>
        /// 倒计时填图
        /// </summary>
        [SerializeField]
        private UISprite _fillSpr;

        /// <summary>
        /// 时间贴图父层级
        /// </summary>
        [SerializeField]
        private UIGrid _timeGrid;

        /// <summary>
        /// 时间贴图
        /// </summary>
        [SerializeField]
        private UISprite[] _timeSprites;

        public UITweener BeginBetTip;

        public UITweener StopBetTip;


        public void PlayClock(int time,int type,Action onTimeUp = null)
        {
            if (CdLabel != null)
                CdLabel.gameObject.SetActive(true);
            
            switch (type)
            { 
                case RequestType.BeginBet:
                    SetTipSprite("StartBet");
                    ShowTip(BeginBetTip);
                    break;
                case RequestType.GiveCards:
                    SetTipSprite("GiveCards");
                    break;
   
            }

            SetClock(time, onTimeUp);
        }


        public override void SetClock(int s, Action callback = null)
        {
            HideAllTimeSprites();
            CancelInvoke("CountDown");

            base.SetClock(s, callback);
            _totalCD = s;
        }


        protected override void CountDown()
        {
            SetSprite(CdSum);
            if (CdSum < 0)
            {
                HideAllTimeSprites();
                if (CallBack != null)
                {
                    CallBack();
                }
                CancelInvoke("CountDown");
                gameObject.SetActive(false);
                return;
            }

            _timeGrid.gameObject.SetActive(true);
            _fillSpr.fillAmount = CdSum / _totalCD;
            CdSum--;
            Facade.Instance<MusicManager>().Play("Clock");
        }


        private void SetSprite(int time)
        {
            SetNumSprite(time, 0);

            _timeGrid.repositionNow = true;
            _timeGrid.Reposition();
        }

        /// <summary>
        /// 显示时间
        /// </summary>
        /// <param name="time"></param>
        /// <param name="index"></param>
        void SetNumSprite(int time, int index)
        {
            if (index >= _timeSprites.Length) return;
            bool haveTime = time > 0;
            var spr = _timeSprites[index];
            spr.spriteName = (time % 10).ToString();
            spr.gameObject.SetActive(haveTime || index == 0);

            if (haveTime)
                SetNumSprite(time / 10, index + 1);
        }

        /// <summary>
        /// 隐藏所有时间贴图
        /// </summary>
        void HideAllTimeSprites()
        {
            foreach (UISprite spr in _timeSprites)
            {
                spr.gameObject.SetActive(false);
            }
        }

        public void ShowTip(UITweener tip)
        {
            if (tip == null) return;

            tip.gameObject.SetActive(true);
            tip.ResetToBeginning();
            tip.PlayForward();
        }

        public override void StopClock(int type)
        {
            base.StopClock(type);
            if(type == RequestType.EndBet)
            {
                ShowTip(StopBetTip);
            }
            CancelInvoke();
            gameObject.SetActive(false);
        }
    }
}