using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
using UnityEngine;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

#pragma warning disable 649


namespace Assets.Scripts.Game.paijiu
{
    public class ResultItemInfo : MonoBehaviour
    {
        /// <summary>
        /// 玩家名字的UILabel
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private UILabel _nameLabel = null;

        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private NguiTextureAdapter _headImage = null;


        /// <summary>
        /// 玩家得分的UILabel
        /// </summary>
        [SerializeField]
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
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
        private GameObject _bigWinnerMark;

        /// <summary>
        /// 房主图标
        /// </summary>
        [SerializeField]
        private GameObject _lordMark;

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



        /// <summary>
        /// 在玩家信息中显示的玩家赢取的金额
        /// </summary>
        public int WinGold
        {
            set { _winGold = value; _scoreLabel.text = _winGold.ToString(); }
            get { return _winGold; }
        }

        public GameObject BigWinnerMark
        {
            get
            {
                return _bigWinnerMark;
            }

            set
            {
                _bigWinnerMark = value;
            }
        }

        public GameObject LordMark
        {
            get
            {
                return _lordMark;
            }

            set
            {
                _lordMark = value;
            }
        }



        /// <summary>
        /// 设置是否显示大赢家图标
        /// </summary>
        /// <param name="isBigWinner">是否是大赢家</param>
        public void SetBigWinnerMark(bool isBigWinner)
        {
            BigWinnerMark.SetActive(isBigWinner);
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

            if (data.ContainsKey("seat"))
            {

                int seat = data.GetInt("seat");

                var gdata = App.GetGameData<PaiJiuGameData>();

                var userInfo = gdata.GetPlayer<PaiJiuPlayer>(seat, true).Info;
                if (userInfo != null)
                {
                    PlayerName = userInfo.NickM;
                    PortraitDb.SetPortrait(userInfo.AvatarX, _headImage, userInfo.SexI); //刷新头像
                }
                _lordMark.SetActive(seat == 0);
            }

            SetBigWinnerMark(false);
            if (data.ContainsKey("id"))
            {
                int id = data.GetInt("id");
                gameObject.SetActive(id >= 0);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        void SetScoreLabel(int score)
        {
            if (score == 0)
            {
                _scoreLabel.text = score.ToString();
                return;
            }

            if (score > 0)
            {
                _scoreLabel.text = "+" +YxUtiles.ReduceNumber(score);
            }
            else
            {
                _scoreLabel.text = YxUtiles.ReduceNumber(score);
            }
        }

    }
}