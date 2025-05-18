using UnityEngine;

namespace Assets.Scripts.Game.bjlb
{
    public class RequestType : MonoBehaviour
    {
        public const int Bet = 1;               //下注
        public const int Reward = 2;            //
        public const int ApplyBanker = 3;       //上庄
        public const int ApplyQuit = 4;         //下庄
        public const int BeginBet = 5;          //
        public const int EndBet = 6;            //
        public const int GiveCards = 7;         //发牌信息
        public const int Result = 8;            //
        public const int BankerList = 9;        //庄家列表
        public const int GroupBet = 10;         //一起下注

    }
}
