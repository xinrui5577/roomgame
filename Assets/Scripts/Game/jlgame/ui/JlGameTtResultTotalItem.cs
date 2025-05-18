using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.jlgame.network;
using UnityEngine;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameTtResultTotalItem : MonoBehaviour
    {
        public NguiTextureAdapter UserHead;
        public UILabel UserName;
        public UILabel UserId;
        public UILabel TotalWin;
        public GameObject RoomOwner;
        public GameObject BigWinner;

        public void SetView(TtResultUserData ttResultUserData)
        {
            PortraitDb.SetPortrait(ttResultUserData.UserHead, UserHead, ttResultUserData.Sex);
            if (ttResultUserData.IsRoomOwner)
            {
                RoomOwner.SetActive(true);
            }

            if (ttResultUserData.IsWinner)
            {
                BigWinner.SetActive(true);
            }
            UserName.text = ttResultUserData.UserName;
            UserId.text =string.Format("ID:{0}", ttResultUserData.UserId);
            var value = "";
            TotalWin.applyGradient = true;
            if (ttResultUserData.UserGold >= 0)
            {
                value += string.Format("+{0}", ttResultUserData.UserGold);
                TotalWin.gradientTop= new Color(255 / 255f, 255 / 255f, 0/ 255f);
                TotalWin.gradientBottom= new Color(255 / 255f,120 / 255f, 0 / 255f);
                TotalWin.effectColor = new Color(120 / 255f, 0 / 255f, 0 / 255f);
            }
            else
            {
                value += string.Format("{0}", ttResultUserData.UserGold);
                TotalWin.gradientTop = new Color(0 / 255f, 255 / 255f, 255 / 255f);
                TotalWin.gradientBottom = new Color(0 / 255f, 100 / 255f, 255 / 255f);
                TotalWin.effectColor = new Color(0 / 255f, 0 / 255f, 120 / 255f);
            }

            TotalWin.text = value;
        }

    }
}
