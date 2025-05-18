using UnityEngine;

namespace Assets.Scripts.Game.pdk.PokerRule
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
        C1=8,

        /// <summary>
        /// 对子
        /// </summary>
        C2 = 7,

        /// <summary>
        /// 顺子
        /// </summary>
        C123 = 6,

        /// <summary>
        /// 连对
        /// </summary>
        C1122 = 5,

        /// <summary>
        /// 3带2
        /// </summary>
        C32 = 4,

        /// <summary>
        /// 飞机带2张牌
        /// </summary>
        C1112223434 = 3,

        /// <summary>
        /// 炸弹
        /// </summary>
        C4 = 1,


        //------------以下都是跑得快用不着的牌型了

        /// <summary>
        /// 3条
        /// </summary>
        C3 = 103,

        /// <summary>
        /// 3带1
        /// </summary>
        C31=1031,

        /// <summary>
        /// 飞机（三顺）
        /// </summary>
        C111222 = 109,

        /// <summary>
        /// 飞机带单排
        /// </summary>
        C11122234 = 1010,


        /// <summary>
        /// 4带2个单，或者一对
        /// </summary>
        C411 = 13,

/*        /// <summary>
        /// 4带2对
        /// </summary>
        C422  =15,*/

        /// <summary>
        /// 超级炸弹
        /// </summary>
        C5 = 108,

        /// <summary>
        /// 火箭 (天炸？)
        /// </summary>
        C42 = 107,
    }
}
