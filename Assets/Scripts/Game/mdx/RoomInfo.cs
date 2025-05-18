using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.mdx
{
    public class RoomInfo : MonoBehaviour
    {

        public UILabel PlayerNumLabel;

        public UILabel BetTimeLabel;

        public string PlayerNumFormate = "在线:{0}";

        public string BetTimeFormate = "下注:{0}";

        private int _playerNum;

        private int _betTime;

        public int PlayerNum
        {
            private set { _playerNum = value; RefreshNumLabel(); }
            get { return _playerNum; }
        }

        public int BetTime
        {
            private set { _betTime = value; RefreshBetTimeLabel(); }
            get { return _betTime; }
        }

        public void OnGetGameInfo(ISFSObject gameInfo)
        {
            if (!gameInfo.ContainsKey("userNum")) return;
            int userNum = gameInfo.GetInt("userNum");

            PlayerNum = userNum;
            BetTime = 0;
        }

        public void OnPlayerBet()
        {
            BetTime ++;
        }

        public void OnGroupBet(ISFSObject gameData)
        {
            if (!gameData.ContainsKey("coin"))
                return;
            var coin = gameData.GetSFSArray("coin");
            BetTime += coin == null ? 0 : coin.Count;
        }


        public void OnPlayerJoinIn()
        {
            PlayerNum++;
        }

        public void OnUserOut()
        {
            PlayerNum--;
        }

        private void RefreshNumLabel()
        {
            PlayerNumLabel.text = string.Format(PlayerNumFormate, _playerNum);
        }

        private void RefreshBetTimeLabel()
        {
            BetTimeLabel.text = string.Format(BetTimeFormate, _betTime);
        }

        public void Reset()
        {
            BetTime = 0;
        }
    }
}
