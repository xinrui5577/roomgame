using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.fillpit.skin1
{
    public class DismissMsgItemSk1 : DismissMsgItem
    {
        public NguiTextureAdapter HeadImage;

        public UISprite ResultMark;

        public GameObject ChoisingMark;

        public override void SetItemData(YxBaseGameUserInfo userInfo)
        {
            base.SetItemData(userInfo);
            int seat = userInfo.Seat;
            HeadImage.SetTexture(App.GameData.GetPlayer<PlayerPanel>(seat, true).HeadPortrait.GetTexture());
        }

        public override int PlayerType
        {
            set
            {
                switch (value)
                {
                    case 2:
                        ChoisingMark.SetActive(true);
                        ResultMark.gameObject.SetActive(false);
                        break;
                    case 3:
                        ChoisingMark.SetActive(false);
                        SetResultMark("yes");
                        break;
                    case -1:
                        ChoisingMark.SetActive(false);
                        SetResultMark("no");
                        break;
                }
            }
        }

        void SetResultMark(string spriteName)
        {
            ResultMark.gameObject.SetActive(true);
            ResultMark.spriteName = spriteName;
            ResultMark.MakePixelPerfect();
        }
    }
}
