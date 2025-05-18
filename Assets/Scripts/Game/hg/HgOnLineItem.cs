using System.Collections.Generic;
using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.hg
{
    public class HgOnLineItem : Item
    {
        public List<GameObject> RankSpecial;
        public UISprite UserRankNomalNo;
        public UISprite UserRankNomalS;
        public UISprite UserRankNomalG;
        public NguiTextureAdapter UserHead;
        public UILabel UserName;
        public UILabel UserGold;
        public UILabel AboutAround;
        public UILabel BetGold;
        public UILabel WinAround;

        public override void SetRankData(int rankNum, HgUserInfo userInfo, int aboutAround)
        {
            if (rankNum <= 5)
            {
                RankSpecial[rankNum].SetActive(true);
            }
            else if (rankNum > 5 && rankNum <= 9)
            {
                UserRankNomalNo.gameObject.SetActive(true);
                UserRankNomalS.gameObject.SetActive(true);
                UserRankNomalS.spriteName = string.Format("num{0}", rankNum);
            }
            else
            {
                UserRankNomalNo.gameObject.SetActive(true);
                UserRankNomalS.gameObject.SetActive(true);
                UserRankNomalG.gameObject.SetActive(true);

                var sNum = rankNum / 10 % 10;
                UserRankNomalS.spriteName = string.Format("num{0}", sNum);

                var gNum = rankNum % 10;
                UserRankNomalG.spriteName = string.Format("num{0}", gNum);
            }
            PortraitDb.SetPortrait(userInfo.AvatarX, UserHead, userInfo.SexI);
            UserName.text = userInfo.NickM;
            UserGold.text = YxUtiles.GetShowNumberForm(userInfo.CoinA, 0, "0.#");

            AboutAround.text = string.Format("近{0}局", aboutAround);

            BetGold.text = YxUtiles.GetShowNumberForm(userInfo.TwentyBet, 0, "0.#");
            WinAround.text = string.Format("{0}局", userInfo.TwentyWin);
        }
    }
}
