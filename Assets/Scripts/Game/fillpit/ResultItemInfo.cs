using UnityEngine;
// ReSharper disable FieldCanBeMadeReadOnly.Local


namespace Assets.Scripts.Game.fillpit
{
    public class ResultItemInfo : MonoBehaviour
    {
        /// <summary>
        /// 玩家名字的UILabel
        /// </summary>
        [SerializeField]
        private UILabel _nameLabel = null;

        /// <summary>
        /// 玩家得分的UILabel
        /// </summary>
        [SerializeField]
        private UILabel _score = null;

        private int _winGold;

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
        /// 用于存储玩家的utf_string
        /// </summary>
        private string _nick = string.Empty;


        /// <summary>
        /// 玩家的utf_string值
        /// </summary>
        public string Nick 
        {
            set { _nick = value; }
            get { return _nick; }
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
            set { _winGold = value; _score.text = _winGold.ToString(); }
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

    

    }
}