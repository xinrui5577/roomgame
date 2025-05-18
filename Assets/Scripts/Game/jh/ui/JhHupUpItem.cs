using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.jh.ui {
    public class JhHupUpItem : MonoBehaviour
    {
        public YxBaseTextureAdapter Head;
        public UILabel Name;
        public UISprite Icon;

        public void SetInfo(string texture,string name,int sex,int icon = 0)
        {
            PortraitDb.SetPortrait(texture, Head, sex);
            Name.text = name;
            SetIcon(icon);
        }

        public void SetIcon(int icon)
        {
            switch (icon)
            {
                case 0:
                    Icon.gameObject.SetActive(false);
                    break;
                case 2:
                case 3:
                    Icon.spriteName = "public_035";
                    Icon.gameObject.SetActive(true);
                    break;
                case -1:
                    Icon.spriteName = "public_034";
                    Icon.gameObject.SetActive(true);
                    break;
            }
        }
        
    }

}
