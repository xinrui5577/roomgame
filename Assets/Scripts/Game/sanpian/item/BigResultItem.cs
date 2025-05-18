using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.sanpian.server;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.sanpian.item
{
    public class BigResultItem : MonoBehaviour
    {
        public UILabel Name;
        public UILabel SmallSnow;
        public UILabel BigSnow;
        public UILabel Lost;
        public UILabel Win;
        public UILabel Score;
        public NguiTextureAdapter head;

        public void SetValue(ISFSObject user,int i)
        {
            Name.text = user.GetUtfString("nick");
            Score.text = YxUtiles.ReduceNumber(user.GetInt("gold"));
            SmallSnow.text = YxUtiles.ReduceNumber(user.GetInt("sanpianScore"));
            BigSnow.text = YxUtiles.ReduceNumber(user.GetInt("winScore"));
            Win.text = user.GetInt("win") + "";
            Lost.text = user.GetInt("lost") + "";
//            head.mainTexture = App.GetGameManager<SanPianGameManager>().PlayerArr[i].UIInfo.HeadTexture.mainTexture;
            var curUser = App.GetGameManager<SanPianGameManager>().PlayerArr[i];
            PortraitDb.SetPortrait(curUser.userInfo.HeadImage, curUser.UIInfo.HeadTexture, curUser.userInfo.Sex);
        }
    }
}
