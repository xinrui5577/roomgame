using UnityEngine;

namespace Assets.Scripts.Game.mdx
{
    public class RequestType : MonoBehaviour
    {
        //新版百人游戏交互
        public const int ApplyBanker = 101;         //上庄
        public const int ApplyQuit = 102;           //下庄
        public const int ChangeBanker = 103;        //换庄
        public const int BankerGold = 104;          //上庄带钱
        public const int BeginBet = 105;            //开始下注
        public const int EndBet = 106;              //停止下注
        public const int Bet = 107;                 //下注
        public const int RockDices = 108;           //扔骰子,发牌等展示游戏结果的产生
        public const int Result = 109;              //游戏结算界面
        public const int GroupBet = 110;            //流式下注  
        public const int WinBankerTime = 200;           //抢庄

        //待校对
        public const int Reward = 2;                //
        public const int BankerList = 9;            //庄家列表

    }
}
