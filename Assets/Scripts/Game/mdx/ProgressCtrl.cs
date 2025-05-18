using System;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.mdx
{
    public class ProgressCtrl : YxView
    {

        public UILabel CdLabel;
        public UILabel StrLabel;
        public TimesUpTip TimesUpLabel;
        protected new Action CallBack;
        protected int CdSum;


        public virtual void SetClock(int s, Action callback = null)
        {
            gameObject.SetActive(true);
            CdSum = s;
            CallBack = callback;

            if (CdLabel != null)
            {
                CdLabel.gameObject.SetActive(s > 0);
            }
            if (s == 0)
            {
                return; 
            }
            InvokeRepeating("CountDown", 0, 1);
        }

        public void PlayClock(string tip, int s, Action callback = null)
        {
            SetStrLabel(tip);
            if (CdLabel != null)
            {
                CdLabel.gameObject.SetActive(true);
            }
            SetClock(s, callback);
        }

        public virtual void StopClock(int type)
        {
            //SetTip("停止下注", "StopBet");
            if (CdLabel != null)
            {
                CdLabel.gameObject.SetActive(false);
            }
            SetClock(0);
        }

        protected void SetTip(string tip, string spriteName)
        {
            SetStrLabel(tip);
            SetTipSprite(spriteName);
        }

        protected void SetTipSprite(string spriteName)
        {
            //if (TipSprite == null) return;
            //TipSprite.spriteName = spriteName;
            if (StrLabel != null) StrLabel.gameObject.SetActive(false);
        }

        protected void SetStrLabel(string tip)
        {
            if (StrLabel == null) { return; }
            StrLabel.text = tip;
            StrLabel.gameObject.SetActive(true);
        }

        protected virtual void CountDown()
        {
            CdSum--;
            if (CdSum <= 0)
            {
                if (CdLabel != null)
                    CdLabel.text = "";
                if (CallBack != null)
                {
                    CallBack();
                }
                CancelInvoke("CountDown");
                gameObject.SetActive(false);
                return;
            }
            else if (CdSum <= 3)
            {
                PlayTimesUp(CdSum);
            }
            Facade.Instance<MusicManager>().Play("Clock");

            if (CdLabel != null)
                CdLabel.text = CdSum + "秒";
        }

        private void PlayTimesUp(int cdSum)
        {
            TimesUpLabel.ShowTimesUpTip(cdSum);
        }


        public void StopClock(bool iscallback)
        {
            CancelInvoke("CountDown");
            if (iscallback)
            {
                CallBack();
            }
        }
    }
}
