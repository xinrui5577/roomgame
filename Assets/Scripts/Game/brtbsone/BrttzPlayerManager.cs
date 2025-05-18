using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine.VR;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brtbsone
{
    public class BrttzPlayerManager : MonoBehaviour
    {
        public List<BrttzTablePlayer> TablePlayersList;

        protected string PlayerListKey = "goldRank";

        public bool TablePlayerBet(int seat, int gold, int p, bool isSelf = false)
        {
            var player = TablePlayersList.Find(item => item.Info != null && item.Info.Seat == seat);
            if (player == null) return false;
            if (isSelf)
                player.TableSelfBet(gold);
            else
                player.TablePlayerBet(p, gold);
            return true;
        }

        public void InitTablePlayerInfo(ISFSObject playerInfo)
        {
            if (!playerInfo.ContainsKey(PlayerListKey)) return;
            var playerList = playerInfo.GetIntArray(PlayerListKey);
            var playerCount = playerList.Length;
            for (int i = 0; i < TablePlayersList.Count; i++)
            {
                if (playerCount <= i) continue;
                int seat = playerList[i];
                UpdataTablePlayerInfo(TablePlayersList[i], seat, true);
            }
        }

        protected void UpdataTablePlayerInfo(BrttzTablePlayer player, int seat, bool isServerSeat)
        {
            if (seat == -1)
            {
                player.Info = null;
                player.UpdateView();
                return;
            }
            var data = App.GetGameData<BrttzGameData>();
            player.Info = data.GetPlayerInfo(seat, isServerSeat);
            player.UpdateView();
            player.gameObject.SetActive(true);
        }
    }
}