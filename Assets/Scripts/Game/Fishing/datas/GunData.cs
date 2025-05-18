using Assets.Scripts.Game.Fishing.entitys;

namespace Assets.Scripts.Game.Fishing.datas
{
    public class GunData
    {
        /// <summary>
        /// 威力等级
        /// </summary>
        public int PowerLeve;
        /// <summary>
        /// 自动开火
        /// </summary>
        public bool IsAuto;
        /// <summary>
        /// 穿透状态
        /// </summary>
        public bool IsPenetrate;
        /// <summary>
        /// 锁定
        /// </summary>
        public bool IsLock;
        /// <summary>
        /// 是否双倍
        /// </summary>
        public bool IsDouble;
        /// <summary>
        /// 倍数
        /// </summary>
        public int Bet;
        /// <summary>
        /// 所属玩家
        /// </summary>
        public FishingPlayer Player;
        /// <summary>
        /// 总倍数
        /// </summary>
        public int TotalBet
        {
            get { return IsDouble ? Bet * 2 : Bet; }
        }
    }
}
