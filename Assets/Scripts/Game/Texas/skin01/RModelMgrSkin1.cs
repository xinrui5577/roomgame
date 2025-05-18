using UnityEngine;
using Assets.Scripts.Game.Texas.Mgr;
using Sfs2X.Entities.Data;
using YxFramwork.Common;


namespace Assets.Scripts.Game.Texas.skin01
{
    public class RModelMgrSkin1 : RModelMgr
    {

        [SerializeField]
        private UILabel _roundValue;

        /// <summary>
        /// 解散房间界面的标题
        /// </summary>
        [SerializeField]
        private UILabel _dismissTitelLabel;
  
        public override void ShowRoomInfo(ISFSObject gameInfo)
        {
            base.ShowRoomInfo(gameInfo);
            if(gameInfo.ContainsKey("round"))
            {
                int round = gameInfo.GetInt("round");
                App.GetGameData<TexasGameData>().CurRound = round;
                if (_roundValue != null) _roundValue.text = string.Format("{0:00} / {1:00}", round, MaxRound);
            }

            if(gameInfo.ContainsKey("isRoundOver"))
            {
                bool isRoundOver = gameInfo.GetBool("isRoundOver");
                if (_roundValue != null) _roundValue.transform.parent.gameObject.SetActive(isRoundOver);
                TimeValue.transform.parent.gameObject.SetActive(!isRoundOver);
            }
        }


        public override void HideRoomInfo()
        {
            base.HideRoomInfo();
            RoomInfo.transform.localScale = new Vector3(1, 0, 1);
        }

        public override void UpDataRoundValue(int round)
        {
            if (!App.GetGameData<TexasGameData>().IsRoomGame)
                return;
            if (!IsRoundGame) return;
            _roundValue.text = string.Format("{0:00} / {1:00}", round, MaxRound);
        }

        public override void SetTitelInfo(int id)
        {
            if(_dismissTitelLabel == null)
                return;

            var gdata = App.GameData;
            var playerList = gdata.PlayerList;
            var playerCount = playerList.Length;
            var userName = string.Empty;
            for (var i = 0; i < playerCount; i++)
            {
                var userInfo = gdata.GetPlayerInfo(i);
                if (userInfo == null || id != userInfo.Id) { continue;}
                userName = userInfo.NickM;
                break;
            }
            _dismissTitelLabel.text = "[FBCE98FF]玩家[FFFF96FF]" + userName + "[FBCE98FF]申请解散房间,请等待其他玩家选择";
        }

    }
}