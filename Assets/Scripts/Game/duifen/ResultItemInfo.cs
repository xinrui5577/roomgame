using Assets.Scripts.Common.Adapters;
using UnityEngine;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;

// ReSharper disable FieldCanBeMadeReadOnly.Local
#pragma warning disable 649


namespace Assets.Scripts.Game.duifen
{
    public class ResultItemInfo : MonoBehaviour
    {
        /// <summary>
        /// 玩家名字的UILabel
        /// </summary>
        [SerializeField]
        private UILabel _nameLabel = null;

        /// <summary>
        /// id
        /// </summary>
        [SerializeField]
        private UILabel _idLabel = null;

        /// <summary>
        /// 玩家得分的UILabel
        /// </summary>
        [SerializeField]
        private UILabel _scoreLabel = null;

        /// <summary>
        /// 显示赢的次数
        /// </summary>
        [SerializeField]
        private UILabel _winTimeLabel;

        /// <summary>
        /// 显示输的次数
        /// </summary>
        [SerializeField]
        private UILabel _lostTimeLabel;

        /// <summary>
        /// 头像
        /// </summary>
        [SerializeField]
        private NguiTextureAdapter _headImage;

        private int _winGold;

        /// <summary>
        /// 赢的次数
        /// </summary>
        private int _winTimes;

        /// <summary>
        /// 输的次数
        /// </summary>
        private int _lostTimes;


        /// <summary>
        /// 大赢家图标
        /// </summary>
        [SerializeField]
        private GameObject _bigWinnerMark = null;

        /// <summary>
        /// 房主图标
        /// </summary>
        [SerializeField]
        private GameObject _lordMark = null;

        /// <summary>
        /// 赢的次数
        /// </summary>
        public int WinTime
        {
            set { _winTimes = value; _winTimeLabel.text = _winTimes.ToString(); }
            get { return _winTimes; }
        }

        /// <summary>
        /// 输的次数
        /// </summary>
        public int LostTime
        {
            set { _lostTimes = value; _lostTimeLabel.text = _winTimes.ToString(); }
            get { return _lostTimes; }
        }

        /// <summary>
        /// 要在玩家信息中显示的玩家的名字
        /// </summary>
        public string PlayerName { set { _nameLabel.text = value; } }

        // ReSharper disable once InconsistentNaming
        public string PlayerID { set { _idLabel.text = value; } }


        /// <summary>
        /// 在玩家信息中显示的玩家赢取的金额
        /// </summary>
        public int WinGold
        {
            set { _winGold = value; _scoreLabel.text = _winGold.ToString(); }
            get { return _winGold; }
        }

       

        /// <summary>
        /// 设置是否显示大赢家图标
        /// </summary>
        /// <param name="isBigWinner">是否是大赢家</param>
        public void SetBigWinnerMark(bool isBigWinner)
        {
            _bigWinnerMark.SetActive(isBigWinner);
        }



        /// <summary>
        /// 设置是否显示房主图标
        /// </summary>
        /// <param name="isLord">是否是房主</param>
        public void SetLordMark(bool isLord)
        {
            _lordMark.SetActive(isLord);
        }


        public void InitItem(ISFSObject data)
        {
            
            _winTimeLabel.text = data.ContainsKey("ttwin") ? data.GetInt("ttwin").ToString() : "0";
            
            _lostTimeLabel.text = data.ContainsKey("ttlost") ? data.GetInt("ttlost").ToString() : "0";
            
            int score = data.ContainsKey("gold") ? data.GetInt("gold") : 0;
            SetScoreLabel(score);

            InitPlayerName(data);
            InitPlayerHeadImage(data);
         
            
            SetBigWinnerMark(false);
            if(data.ContainsKey("id"))
            {
                int id = data.GetInt("id");
                PlayerID = "ID:" + id;
                gameObject.SetActive(id >= 0);
                _lordMark.SetActive(id == App.GetGameData<DuifenGlobalData>().OwnerId);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private void InitPlayerHeadImage(ISFSObject data)
        {
            if (data.ContainsKey("avatar"))
            {
                PortraitDb.SetPortrait(data.GetUtfString("avatar"), _headImage, data.GetShort("sex"));
            }
            else
            {
                var userInfo = GetUserInfo(data.GetInt("seat"));
                if (userInfo == null)
                    return;
                PortraitDb.SetPortrait(userInfo.AvatarX, _headImage, userInfo.SexI);
            }
        }

        void InitPlayerName(ISFSObject data)
        {
            if (data.ContainsKey("nick"))
            {
                PlayerName = data.GetUtfString("nick");
            }
            else
            {
                var userInfo = GetUserInfo(data.GetInt("seat"));
                PlayerName = userInfo == null ? "" : userInfo.NickM;
            }
        }

        YxBaseGameUserInfo GetUserInfo(int seat)
        {
            return App.GameData.GetPlayerInfo(seat, true);
        }


        void SetScoreLabel(int score)
        {
            if(score == 0)
            {
                _scoreLabel.text = score.ToString();
                return;
            }

            if(score > 0)
            {
                _scoreLabel.text = "+" + App.GetGameData<DuifenGlobalData>().GetShowGoldValue(score);
            }
            else
            {
                _scoreLabel.text = App.GetGameData<DuifenGlobalData>().GetShowGoldValue(score);
            }
        }

    }
}