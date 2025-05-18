using UnityEngine;
using System.Collections;
using Assets.Scripts.Common.components;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Hall_01.Item
{
    public class DbsmjPlayerRecordInfo : MonoBehaviour
    {
        [SerializeField]
        private UILabel _playerName;
        [SerializeField]
        private UILabel _playerScore;
        [SerializeField]
        private UILabel _playerID;
        [SerializeField]
        private YxBaseTextureAdapter _playerIcon;

        public void SetData(DbsmjPlayerRecordData data, bool roomType)
        {
            _playerName.text = data.PlayerName;
            _playerScore.text = data.ScoreNum.ToString();// + (roomType ? "金币" : "积分");

            if (_playerID != null)
            {
                _playerID.gameObject.SetActive(!data.ID.Equals(0));
                _playerID.text = string.Format("ID:{0}", data.ID);
            }

            if (!string.IsNullOrEmpty(data.Icon))
            {
                int sex = data.Sex >= 0 ? data.Sex : 0;
                PortraitDb.SetPortrait(data.Icon, _playerIcon, sex);
            }
        }
    }

    /// <summary>
    /// 记录中的玩家数据
    /// </summary>
    public class DbsmjPlayerRecordData
    {
        /// <summary>
        /// 玩家名
        /// </summary>
        public string PlayerName;
        /// <summary>
        /// 得分
        /// </summary>
        public float ScoreNum;
        /// <summary>
        /// 玩家ID
        /// </summary>
        public long ID;

        public string Icon;

        public int Sex;
    }
}