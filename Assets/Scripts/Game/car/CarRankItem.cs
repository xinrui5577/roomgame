using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.car
{
    public class CarRankItem : Item
    {
        public NguiTextureAdapter UserHead;
        public UILabel UserName;
        public UILabel UserGold;

        public override void SetRankData(int rankNum, CarUserInfo userInfo, int aboutAround)
        {
            PortraitDb.SetPortrait(userInfo.AvatarX, UserHead, userInfo.SexI);
            UserName.text = userInfo.NickM;
            UserGold.text = YxUtiles.GetShowNumberForm(userInfo.CoinA, 0, "0.#");
        }

    }

    public class Item : MonoBehaviour
    {
        public virtual void SetRankData(int rankNum, CarUserInfo userInfo, int aboutAround)
        {

        }
    }
}
