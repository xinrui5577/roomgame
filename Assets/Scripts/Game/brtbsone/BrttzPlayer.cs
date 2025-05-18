using System;
using UnityEngine;
using System.Collections;
using YxFramwork.Framework;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brtbsone
{
    public class BrttzPlayer : YxBaseGamePlayer
    {
        public UILabel BankTime;


        public void SetBankerTime(int time)
        {
            if (BankTime == null) return;
            BankTime.text = time.ToString();
            BankTime.gameObject.SetActive(true);
            WinTotalCoinLabel.gameObject.SetActive(true);
        }

        public void HideBankerTime()
        {
            if (BankTime == null) return;
            BankTime.gameObject.SetActive(false);
            WinTotalCoinLabel.gameObject.SetActive(false);
        }

        public void AddBankerTime()
        {
            if (BankTime == null) return;
            var temp = Int32.Parse(BankTime.text);
            BankTime.text = (++temp).ToString();
        }
    }
}