using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;
using YxFramwork.Tool;
#pragma warning disable 649

namespace Assets.Scripts.Game.Texas.skin01
{
    public class ResultItemInfoSkin1 : ResultItemInfo
    {
     
        [SerializeField]
        private UILabel _winTimeLabel;

        [SerializeField]
        private UILabel _lostTimeLabel;

        public override void SetResultItem(Sfs2X.Entities.Data.ISFSObject user, YxBaseUserInfo userInfo)
        {
            _winGold = user.GetInt("gold");
            _score.text = YxUtiles.ReduceNumber(_winGold);
          
            PlayerName = user.GetUtfString("nick");
            SetLordMark(userInfo.Id == 0);
            SetBigWinnerMark(false);
            gameObject.SetActive(true);
            PortraitDb.SetPortrait(userInfo.AvatarX, HeadImage, userInfo.SexI);      //设置头像

            if (_winTimeLabel != null)
                _winTimeLabel.text = user.GetInt("win").ToString();
            if (_lostTimeLabel != null)
                _lostTimeLabel.text = user.GetInt("lost").ToString();
        }

    }
}