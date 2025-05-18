using Assets.Scripts.Common.Adapters;
using UnityEngine;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Hall.View.MahStadiumListWindow
{
    public class MahFriendItem : MonoBehaviour
    {
        public NguiTextureAdapter Head;
        public UILabel UserName;
        public UILabel UserId;
        public UIButton DeleteBtn;

        public void InitData(string url,string userName,int id)
        {
            if (Head!=null) PortraitDb.SetPortrait(url, Head, 1);
            if (UserName != null) UserName.text = userName;
            if (UserId != null) UserId.text = "ID:"+id;
        }
    }
}
