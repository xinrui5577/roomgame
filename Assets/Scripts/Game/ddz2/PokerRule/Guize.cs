using UnityEngine;

namespace Assets.Scripts.Game.ddz2.PokerRule
{
    public enum Guize
    {
        NOT_EXIT = -1,//异常
        BU_CHU = 0,//不出..
        YI_ZHANG = 1,//一张..
        DUI_ZI = 2,//对子..
        SAN_TIAO = 3,//3条..
        BOMB = 4,//普通炸弹..
        SHUN_ZI = 5,//顺子..
        SHUANG_SHUN = 6,//双顺..
        TIAN_ZHA = 7,//双王..
        SUPER_BOMB = 8,//超级炸弹
        PLANE_3 = 9,//3带
        PLANE_3_1 = 10,//3带1..
        PLANE_3_2 = 11,//3带2..
        PLANE_4 = 12,//4带
        PLANE_4_1 = 13,//4带1
        PLANE_4_2 = 14,//4带2
    }
}
