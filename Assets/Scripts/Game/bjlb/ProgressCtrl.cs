using System;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.bjlb
{
    public class ProgressCtrl : YxView
    {

        public UILabel CdLabel;
        public UILabel StrLabel;
        public UISprite TipSprite;
        protected Action CallBack;
        protected int CdSum;

        protected override void OnStart()
        {
            base.OnStart();
            if (CdLabel != null)
            {
                CdLabel.gameObject.SetActive(false);
            }
        }

        public virtual void SetClock(int s, Action callback = null)
        {
            gameObject.SetActive(true);
            if (s == 0) { return; }
            CdSum = s;
            CallBack = callback;
            InvokeRepeating("CountDown", 0, 1);
        }

        public void PlayClock(int s, Action callback = null)
        {
            SetTip("开始下注", "StartBet");
            if (CdLabel != null)
                CdLabel.gameObject.SetActive(true);
            SetClock(s, callback);
        }

        public virtual void StopClock(int type)
        {
            SetTip("停止下注", "StopBet");
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
            if (TipSprite != null)
            {
                TipSprite.spriteName = spriteName;
                if (StrLabel != null) StrLabel.gameObject.SetActive(false);
                return;
            }
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
            if (CdSum < 0)
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
            Facade.Instance<MusicManager>().Play("Clock");

            if (CdLabel != null)
                CdLabel.text = CdSum + "";
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
