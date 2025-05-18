using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGamePlaying : FsmState
    {
        public override void OnEnter(FsmStateArgs args)
        {
            GameCenter.Instance.SetIgonreReconnectState(false);
            AddMahjongFriends();
            //执行继承IGameStartCycle接口脚本
            GameCenter.Lifecycle.GameStartCycle();
        }

        /// <summary>
        /// 添加本局麻友
        /// </summary>
        private void AddMahjongFriends()
        {
            var db = GameCenter.DataCenter;
            if (db.Room.RoomType == MahRoomType.FanKa && db.ConfigData.InviteMahFriends && db.Room.CurrRound == 1)
            {
                bool flag = false;
                switch (db.Room.LoopType)
                {
                    case MahGameLoopType.Round:
                        if (db.Room.CurrRound == 1)
                        {
                            flag = true;
                        }
                        break;
                    case MahGameLoopType.Circle:
                        if (db.Room.CurrRound == 1 && db.Game.FristBankerSeat == db.BankerSeat)
                        {
                            flag = true;
                        }
                        break;
                }
                if (flag)
                {
                    List<int> list = new List<int>();
                    for (int i = 0; i < db.MaxPlayerCount; i++)
                    {
                        int id = int.Parse(db.Players[i].UserId);
                        list.Add(id);
                    }
                    var apiInfo = new Dictionary<string, object>()
                    {
                        { "bundleID", Application.bundleIdentifier },
                        { "ID", list}
                    };
                    MahjongUtility.SendAction("mahjongwm.addWmFriends", apiInfo, _ => { });
                }
            }
        }

        public override void OnLeave(bool isShutdown) { }
    }
}