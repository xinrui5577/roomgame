using UnityEngine;

namespace Assets.Scripts.Game.BaiTuan
{
    public enum BtwRequestType
    {
        Bet = 1, //下注
        ApplyBanker = 2,
        ApplyQuit = 3,
        BankerList = 4,
        BeginBet = 5,
        EndBet = 6,
        GiveCards = 7,
        Result = 8,
        GroupBet = 10
    }

    public enum BtwSkin02RequestType
    {
        ShangZhuang = 101,
        XiaZhuang = 102,
        ZhuangChange = 103,
        Start = 105,
        Stop = 106,
        XiaZhu = 107,
        RollResult = 108,
        GameResult = 109,
        GroupBet = 110
    }
}