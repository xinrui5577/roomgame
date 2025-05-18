using Assets.Scripts.Hall.View.RecordWindows;
using UnityEngine;

namespace Assets.Scripts.Hall_01.Item
{
    public class PlayerRecordInfo : MonoBehaviour
    {
        [SerializeField]
        private UILabel _playerName;
        [SerializeField]
        private UILabel _playerScore;
        [SerializeField]
        private UILabel _playerID;
        /// <summary>
        /// 设置战绩玩家信息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="roomType">房间模式</param>
        public void SetData(PlayerRecordData data,bool roomType)
        {
            _playerName.text = data.PlayerName;
            _playerScore.text = data.ScoreNum + (roomType ? "金币":"积分");
            _playerID.gameObject.SetActive(!data.ID.Equals(0));
            _playerID.text = string.Format("ID:{0}",data.ID);
        }
    }
    /// <summary>
    /// 记录中的玩家数据
    /// </summary>
    public class PlayerRecordData
    {
        /// <summary>
        /// 玩家名
        /// </summary>
        public string PlayerName;
        /// <summary>
        /// 得分
        /// </summary>
        public long ScoreNum;
        /// <summary>
        /// 玩家ID
        /// </summary>
        public long ID;
    }
}
