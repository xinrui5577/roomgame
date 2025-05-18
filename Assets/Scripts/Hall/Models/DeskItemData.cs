using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;

namespace Assets.Scripts.Hall.Models
{
    public class DeskItemData
    {
        /// <summary>
        /// 房间类型，初级房高级房等类型
        /// </summary>
        public int Type;
        /// <summary>
        /// 桌面id（也就是真实RoomId）
        /// </summary>
        public int Id;
        /// <summary>
        /// 最小限制
        /// </summary>
        public int MinCoin;
        /// <summary>
        /// 最大限制
        /// </summary>
        public int MaxCoin;
        /// <summary>
        /// 房间名
        /// </summary>
        public string RoomName;
        /// <summary>
        /// 游戏Id
        /// </summary>
        public int GameId;

        public void Parse(Dictionary<string,object> dict)
        {
            dict.Parse("game_type", ref Type);
            dict.Parse("roomId", ref Id);
            dict.Parse("gold_min_q", ref MinCoin);
            dict.Parse("gold_max_q", ref MaxCoin);
            dict.Parse("game_name", ref RoomName);
            dict.Parse("game_id", ref GameId);
        }
    }
}
