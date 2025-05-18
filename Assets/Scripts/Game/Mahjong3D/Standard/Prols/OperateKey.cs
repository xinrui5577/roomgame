namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class OperateKey
    {
        public const int OpreateNone = 0x0;
        public const int OpreateChi = 0x1;
        public const int OpreatePeng = 0x2;
        public const int OpreateGang = 0x4;
        public const int OpreateHu = 0x8;
        public const int OpreateLiang = 0x10;
        /// <summary>
        /// kwx 亮牌
        /// </summary>
        public const int OpreateLiangDao = 0x14;
        public const int OpreateXFG = 0x20;
        public const int OpreateTing = 0x80;
        public const int OpreateXJFD = 1 << 9;
        public const int OperateLigang = 1 << 12;
        public const int OperateLaiZiGang = 1 << 13;
        public const int OperateDaiGu = 1 << 14;  
        public const int OperateJueGang = 1 << 6;
    }
}