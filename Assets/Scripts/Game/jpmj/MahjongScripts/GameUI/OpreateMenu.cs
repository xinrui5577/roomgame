using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.Helper;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class OpreateMenu : MonoBehaviour
    {
        public List<Button> Btns = new List<Button>();
        public GameObject MenuBg;

        public const int SwitchChi = 1;
        public const int SwitchPeng = 2;
        public const int SwitchGang = 4;
        public const int SwitchHu = 8;
        public const int SwitchTing = 16;

        protected List<Button> mOpStateCache = new List<Button>();

        public virtual void ShowMenu(SwitchCombination switchCbt)
        {
            MenuBg.SetActive(true);
            for (int i = 0; i < Btns.Count; i++)
            {
                bool isActive = switchCbt.IsAllowAt(i);
                Btns[i].gameObject.SetActive(isActive);            
            }
        }
        public virtual void ClearButtonStateCache(int eventId, EventData evn)
        {
            mOpStateCache.Clear();
        }

        public virtual void RedisplayButtons(int eventId, EventData evn)
        {
            for (int i = 0; i < mOpStateCache.Count; i++)
            {
                mOpStateCache[i].gameObject.SetActive(true);
            }
        }

        public void ShowMenuBg()
        {
            MenuBg.SetActive(true);
        }

        public virtual void OnChiClick()
        {
            EventDispatch.Dispatch((int)NetEventId.OnChiClick, new EventData());
            MenuBg.SetActive(false);
        }

        public virtual void OnPengClick()
        {
            EventDispatch.Dispatch((int)NetEventId.OnPengClick, new EventData());
            MenuBg.SetActive(false);
        }

        public virtual void OnGangClick()
        {
            EventDispatch.Dispatch((int)NetEventId.OnGangClick, new EventData());
            MenuBg.SetActive(false);
            CancelTrusteeship();
        }

        public virtual void OnHuClick()
        {
            EventDispatch.Dispatch((int)NetEventId.OnHuClick, new EventData());
            MenuBg.SetActive(false);
            CancelTrusteeship();
        }

        public virtual void OnTingClick()
        {
            EventDispatch.Dispatch((int)NetEventId.OnTingClick, new EventData());
            EventDispatch.Dispatch((int)UIEventId.LiangPaiTip, new EventData(1));
            MenuBg.SetActive(false);
            CancelTrusteeship();
        }

        public virtual void OnGuoClick()
        {
            EventDispatch.Dispatch((int)NetEventId.OnGuoClick, new EventData());
            Reset();
        }

        protected virtual void CancelTrusteeship()
        {
            if (TrusteeshipHelper.Instance != null)
            {
                if (TrusteeshipHelper.Instance.IsTrusteeship)
                {
                    EventDispatch.Dispatch((int)UIEventId.DisableTrusteeship);
                }
                TrusteeshipHelper.Instance.IsAllowTrusteeship = false;
            }
        }

        public virtual void OnXJFDClick() { }

        public virtual void OnNiuClick() { }

        public void Reset()
        {
            if (MenuBg.activeSelf)
            {
                MenuBg.SetActive(false);
                for (int i = 0; i < Btns.Count; i++)
                {
                    Btns[i].gameObject.SetActive(false);
                }
            }
        }

        public void HideMenu()
        {
            if (MenuBg.activeSelf)
            {
                MenuBg.SetActive(false);
            }
        }

    }
}
