using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.brnn.brnn_skin03
{
    public class TablePlayerManager : MonoBehaviour {

        /// <summary>
        /// 桌面玩家信息
        /// (用于0号为智多星,其他玩家按财富排列的情况)
        /// </summary>
        public List<TablePlayer> TablePlayerList;

        /// <summary>
        /// 神算子座位号
        /// </summary>
        private const string KeySsz = "ssz";

        private const string KeyGoldRank = "goldRank";


        /// <summary>
        /// 是否是桌面玩家下注
        /// (用于区分 其他玩家 和 桌面显示玩家 + 自己)
        /// (用于桌面玩家和其他玩家不同的下注方式)
        /// </summary>
        /// <returns></returns>
        public bool TableBet(int seat,int gold,int p)
        {
            var player = TablePlayerList.Find(item => item.Info != null && item.Info.Seat == seat);

            if (player == null)
            {
                return false;   //不是桌面玩家下注
            }
            //是桌面玩家下注,对处理
            player.TablePlayerBet(p,gold);      //获取玩家筹码池中的一个筹码

            return true;
        }

        public void InitTablePlayers(ISFSObject requestData)
        {
            List<int> seatList = new List<int>();

            if (!requestData.ContainsKey(KeySsz)) return;
            
           
            //添加神算子座位号
            int seat = requestData.ContainsKey(KeySsz) ? requestData.GetInt(KeySsz) : -1;
            UpdateTablePlayer(TablePlayerList[0], seat, true);
           

            if (requestData.ContainsKey(KeyGoldRank))
            {
                var goldRank = requestData.GetIntArray(KeyGoldRank);
                int rankCount = goldRank.Length;
                int playerCount = TablePlayerList.Count;
                for (int i = 0; i < playerCount; i++)
                {
                    if (rankCount <= i)
                    {
                        seatList.Add(-1);
                        continue;
                    }
                    int rankSeat = goldRank[i];
                    seatList.Add(rankSeat);
                    UpdateTablePlayer(TablePlayerList[i], rankSeat, true);
                }
            }
        }

        private void UpdateTablePlayer(YxBaseGamePlayer player,int seat,bool isServerseat)
        {
            if (seat == -1)
            {
                player.Info = null;
                player.UpdateView();
                return;
            }
            var gdata = App.GetGameData<BrnnGameData>();
            player.Info = gdata.GetPlayerInfo(seat, isServerseat);
            player.UpdateView();
            player.gameObject.SetActive(true);
        }
    }
}
