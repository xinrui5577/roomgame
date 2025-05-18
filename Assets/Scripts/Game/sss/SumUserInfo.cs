using Assets.Scripts.Common.Adapters;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;

#pragma warning disable 649

namespace Assets.Scripts.Game.sss
{
    public class SumUserInfo : MonoBehaviour
    {

        [SerializeField]
        private UILabel _nameLabel;

        [SerializeField]
        private UILabel _idLabel;

        [SerializeField]
        private NguiTextureAdapter _userIcon;


        [HideInInspector]
        public int SumScore;

        /// <summary>
        /// 玩家得分数据存储
        /// </summary>
        [HideInInspector]
        public int[] ScoreArray;

        [HideInInspector]
        public int Id;

        public void Init(ISFSObject data)
        {
            SumScore = data.GetInt("gold");
            _nameLabel.text = data.GetUtfString("nick");
            Id = data.GetInt("id");
            _idLabel.text = "ID:" + Id;
            ScoreArray = data.GetIntArray("record");
            gameObject.SetActive(Id > 0);
            var seat = data.GetInt("seat");
            var userInfo = App.GetGameData<SssGameData>().GetPlayerInfo(seat, true);
            if (userInfo!=null)
            {
                PortraitDb.SetPortrait(userInfo.AvatarX, _userIcon, userInfo.SexI);
            }
        }

    }
}