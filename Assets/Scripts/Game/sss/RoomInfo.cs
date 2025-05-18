using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
#pragma warning disable 649

namespace Assets.Scripts.Game.sss
{
    public class RoomInfo : MonoBehaviour
    {
        /// <summary>
        /// 房间ID
        /// </summary>
        [SerializeField]
        private UILabel _roomIdLabel;

        /// <summary>
        /// 房间底注
        /// </summary>
        [SerializeField]
        private UILabel _anteLabel;

        [SerializeField]
        private UILabel _roundLabel;

        private int _maxRound;

        private int _curRound;

        private int _roomId;

        /// <summary>
        /// 游戏的最大局数
        /// </summary>
        public int MaxRound
        {
            get { return _maxRound; }
        }

        /// <summary>
        /// 房间号
        /// </summary>
        public int RoomID
        {
            get { return _roomId; }
        }

        /// <summary>
        /// 规则
        /// </summary>
        public string RuleInfo
        {
            get;
            private set;
        }

        // Use this for initialization

        public void ShowRoomInfo(ISFSObject data)
        {
            var gdata = App.GetGameData<SssGameData>();
            if (!gdata.IsRoomGame)
                return;

            if (data.ContainsKey("rid"))
            {
                _roomId = data.GetInt("rid");
                _roomIdLabel.text = _roomId.ToString();
            }

            if (data.ContainsKey("maxRound"))
            {
                _maxRound = data.GetInt("maxRound");
            }

            if (data.ContainsKey("round"))
            {
                _curRound = data.GetInt("round");
                gdata.CurRound = _curRound;
            }
            if (data.ContainsKey("rule"))
            {
                RuleInfo = data.GetUtfString("rule");
            }
            _anteLabel.text = gdata.Ante.ToString();

            UpdataRoundInfo();

            gameObject.SetActive(true);

        }

        public void UpdataCurRound()
        {
            _curRound++;
            UpdataRoundInfo();
        }

        void UpdataRoundInfo()
        {
            _roundLabel.text = _curRound + "/" + _maxRound;
        }

    }
}