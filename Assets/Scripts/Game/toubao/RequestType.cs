using UnityEngine;

namespace Assets.Scripts.Game.toubao
{
    public class RequestType : MonoBehaviour
    {
        /// <summary>
        /// 上庄请求
        /// </summary>
        public const int Bet = 101;
        /// <summary>
        /// 下庄请求
        /// </summary>
        public const int XiaZhuang = 102;
        /// <summary>
        /// 发送庄家列表
        /// </summary>
        public const int ZhuangChange = 103;
        /// <summary>
        /// 上庄带钱
        /// </summary>
        public const int ZhuangGold = 104;
        /// <summary>
        /// 开始下注
        /// </summary>
        public const int Start = 105;
        /// <summary>
        /// 停止下注
        /// </summary>
        public const int Stop = 106;
        /// <summary>
        /// 下注交互
        /// </summary>
        public const int XiaZhu = 107;
        /// <summary>
        /// 发送牌或者骰子或者麻将牌,反正就是揭晓游戏结果
        /// </summary>
        public const int RollResult = 108;
        /// <summary>
        /// 结算
        /// </summary>
        public const int GameResult = 109;  

    }
}
