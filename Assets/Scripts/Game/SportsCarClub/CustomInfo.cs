using UnityEngine;
using System.Collections;
using YxFramwork.Framework.Core;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class CustomInfo : YxBaseGamePlayer
    {
        bool isGettedHeadIcon = false;
        

        // Use this for initialization
        protected override void SetCoin(long coin)
        {
            if (CoinLabel == null) return;
            base.SetCoin(coin);
            string tempStr = "￥" + YxUtiles.GetShowNumberForm(coin);
            CoinLabel.Text(tempStr);
        }

        protected override void SetPortrait(string avatar)
        {
            if (!isGettedHeadIcon && avatar != "")
            {
                base.SetPortrait(avatar);
                isGettedHeadIcon = true;
            }
        }
    }
}