using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.Handup
{
    public class HandUpItem : MonoBehaviour
    {

        public UILabel NameLabel;

        public NguiTextureAdapter HeadImage;

        public UISprite ChoiseSprite;

        [HideInInspector] public int Id;

        [HideInInspector] public string NickName;

        public void SetItem(YxBaseUserInfo info)
        {
            if (info == null) return;
            Id = info.Id;
            NickName = info.NickM;
            NameLabel.text = NickName;
            PortraitDb.SetPortrait(info.AvatarX, HeadImage, info.SexI);
            SetSpriteName(0);
        }

        public void SetItemType(int type)
        {
            SetSpriteName(type);
        }

        protected void SetSpriteName(int type)
        {
            string spriteName = string.Empty;
            switch (type)
            {
                case -1:
                    spriteName = "no";
                    break;
                case 2:
                case 3:
                    spriteName = "yes";
                    break;
            }

            ChoiseSprite.spriteName = spriteName;
            ChoiseSprite.gameObject.SetActive(!string.IsNullOrEmpty(spriteName));
        }
    }
}
