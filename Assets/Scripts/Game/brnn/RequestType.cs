using UnityEngine;

namespace Assets.Scripts.Game.brnn
{
    public class RequestType : MonoBehaviour
    {
        public const int Bet = 1;  //下注
        public const int ApplyBanker = 2;
        public const int ApplyQuit = 3;
        public const int BankerList = 4;
        public const int BeginBet = 5;
        public const int EndBet = 6;
        public const int GiveCards = 7;
        public const int Result = 8;
        public const int GroupBet = 10;
    }
}
