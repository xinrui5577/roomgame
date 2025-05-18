using UnityEngine;

namespace Assets.Scripts.Game.bjl3d
{
    public class RequestType : MonoBehaviour
    {
        public const int Bet = 1;               //下注
        public const int ApplyBanker = 3;       //上庄
        public const int ApplyQuit = 4;         //下庄
        public const int BeginBet = 5;          //开始下注
        public const int EndBet = 6;            //结束下注
        public const int GiveCards = 7;         //发牌信息
        public const int Result = 8;            //结果信息
        public const int BankerList = 9;        //庄家列表
    }
}
