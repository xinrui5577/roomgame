using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameHupUpItem : MonoBehaviour
    {
        public NguiTextureAdapter UserHead;
        public UISprite Icon;
        public UILabel UserName;

        public void SetItemView(string head,int sex,bool show,string userName)
        {
            PortraitDb.SetPortrait(head, UserHead, sex);
            Icon.spriteName = show ? "public_027" : "public_026";
            UserName.text = userName;
            name = userName;
        }

        public void ChangeIcon()
        {
            Icon.spriteName = "public_027";
        }
    }
}
