using UnityEngine;
using Sfs2X.Entities.Data;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace Assets.Scripts.Game.fillpit
{
    public class RoomInfo : MonoBehaviour {

        /// <summary>
        /// 显示房间ID
        /// </summary>
        [SerializeField]
        private UILabel _roomIdLabel = null;

        /// <summary>
        /// 显示局数
        /// </summary>
        [SerializeField]
        protected UILabel RoundLabel = null;
   
        /// <summary>
        /// 本房间当前局数
        /// </summary>
        protected int CurRound;

        public string RoomIdFormat = "房间号: {0}";

        public string RoomRoundFormat = "局   数: {0} / {1}";


        /// <summary>
        /// 房间最大局数
        /// </summary>
        public int MaxRound { private set; get; }

        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomId { private set; get; }

        /// <summary>
        /// 房间规则
        /// </summary>
        public string Rule{private set; get; }

        /// <summary>
        /// 展示房间信息
        /// </summary>
        /// <param name="gameInfo"></param>
        public void ShowRoomInfo(ISFSObject gameInfo)
        {

            if (gameInfo.ContainsKey("rid"))
            {
                RoomId = gameInfo.GetInt("rid");
                YxFramwork.Common.App.GetGameData<FillpitGameData>().RoomId = RoomId;
                _roomIdLabel.text =string.Format(RoomIdFormat, RoomId);
                _roomIdLabel.gameObject.SetActive(true);
                gameObject.SetActive(true);
            }

            if (gameInfo.ContainsKey("rule"))
            {
                Rule = gameInfo.GetUtfString("rule");
            }
            if (gameInfo.ContainsKey("maxRound"))
            {
                MaxRound = gameInfo.GetInt("maxRound");
                CurRound = gameInfo.GetInt("round");
                ShowDoubleString();
                RoundLabel.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 更新当前局数
        /// </summary>
        public void UpdateCurrentRound()
        {
            ++CurRound;
            ShowDoubleString();
        }

        protected virtual void ShowDoubleString()
        {
            var gdata = YxFramwork.Common.App.GetGameData<FillpitGameData>();
            string roundInfo = string.Empty;
            if (gdata.IsLanDi)
            {
                roundInfo += "(烂底局)   ";
            }

            roundInfo += string.Format(RoomRoundFormat, CurRound, MaxRound);

            if (gdata.IsDoubleGame)
            {
                roundInfo += "   (翻倍场)";
            }

            RoundLabel.text = roundInfo;
        }
    }
}
