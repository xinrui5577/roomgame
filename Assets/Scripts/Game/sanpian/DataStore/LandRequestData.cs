namespace Assets.Scripts.Game.sanpian.DataStore
{
    public class LandRequestData //地主请求数据
    {
        private const string C_Type = "type";
        private const string C_Cards = "cards";
        private const string C_Sit = "seat";
        private const string C_OPCard = "opCard";
        private const string C_Score = "score";
        private const string C_Magic = "laizi";
        private const string C_type = "ctype";
        private const string C_ante = "cante";
        private const string C_needRejoin = "needRejoin";



        public enum GameRequestType
        {
            /// <summary>
            /// 发牌
            /// </summary>
            TypeAllocate = 1,

            /// <summary>
            /// 选队友
            /// </summary>
            TypeAlly = 2,

            /// <summary>
            /// 谁出牌
            /// </summary>
            TypeSpeaker = 3,

            /// <summary>
            /// 出牌
            /// </summary>
            TypeChuPai = 4,

            /// <summary>
            /// 不出
            /// </summary>
            TypeBuChu = 5,

            /// <summary>
            /// 雪结果
            /// </summary>
            Snow = 6,

            /// <summary>
            /// 结算
            /// </summary>
            TypeResult = 7,

            /// <summary>
            /// 队友手牌
            /// </summary>
            MateCards = 8,

            /// <summary>
            /// 更新积分
            /// </summary>
            UpdateScore = 9,

            /// <summary>
            /// 自动出牌
            /// </summary>
            AutoCard = 10,


            /// <summary>
            /// 明幺
            /// </summary>
            MingYao = 11,

            /// <summary>
            /// 选择雪牌
            /// </summary>
            TypeChooseSnow = 12,

            /// <summary>
            /// 清出牌区域
            /// </summary>
            ClearPoker= 13,

            /// <summary>
            /// 强制同步手牌
            /// </summary>
            UpdateCards = 14,

            /// <summary>
            /// 换位置
            /// </summary>
            ChangeSeat = 15,

            /// <summary>
            /// 片分数
            /// </summary>
            PianScore=16,
        }
    }
}
