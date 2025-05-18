namespace Assets.Scripts.Game.ddz2.PokerRule
{
    /** 牌类型 */
    public enum CardType
    {
        /// <summary>
        /// 异常
        /// </summary>
        Exception=-1,

        /// <summary>
        /// pass
        /// </summary>
        None=0,

        /// <summary>
        /// 单牌
        /// </summary>
        C1=1,

        /// <summary>
        /// 对子
        /// </summary>
        C2 = 2,

        /// <summary>
        /// 3条
        /// </summary>
        C3 = 3,

        /// <summary>
        /// 3带1
        /// </summary>
        C31=31,

        /// <summary>
        /// 3带2
        /// </summary>
        C32 = 32,

        /// <summary>
        /// 顺子
        /// </summary>
        C123 = 5,

        /// <summary>
        /// 连对
        /// </summary>
        C1122 = 6,

        /// <summary>
        /// 飞机（三顺）
        /// </summary>
        C111222 = 9,

        /// <summary>
        /// 飞机带单排
        /// </summary>
        C11122234 = 10,

        /// <summary>
        /// 飞机带对子
        /// </summary>
        C1112223344 = 11,

        /// <summary>
        /// 4带2个单，或者一对
        /// </summary>
        C411 = 13,

/*        /// <summary>
        /// 4带2对
        /// </summary>
        C422  =15,*/


        /// <summary>
        /// 炸弹
        /// </summary>
        C4 = 4,

        /// <summary>
        /// 超级炸弹
        /// </summary>
        C5 = 8,

        /// <summary>
        /// 火箭 (天炸？)
        /// </summary>
        C42 = 7,
    }
}
