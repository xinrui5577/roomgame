using System.Linq;
using YxFramwork.Common.Utils;
using UnityEngine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.GangWu
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class GangwuGameData : YxGameData
    { 
        /// <summary>
        /// 是否接收到GameInfo了
        /// </summary>
        [HideInInspector]
        public bool IsGameInfo;
        /// <summary>
        /// 说话等待时间
        /// </summary>
        [HideInInspector]
        public int SpeakCd;
       
        /// <summary>
        /// 筹码之间的动画间隔
        /// </summary>
        [HideInInspector]
        public float BetSpeace = 0.05f;
        /// <summary>
        /// 能下注的最大池底倍数 0为无池底倍数限制
        /// </summary>
        [HideInInspector]
        public int MaxPoolNum = 1;
        [HideInInspector]
        public bool IsPlayed = false;

        /// <summary>
        /// 当前桌局数计数
        /// </summary>
        [HideInInspector]
        public int CurRound = 0;

        /// <summary>
        /// 是否是开房模式
        /// </summary>
        [HideInInspector]
        public bool IsRoomGame = false;

        /// <summary>
        /// 是否是第四轮
        /// </summary>
        [HideInInspector]
        public bool AllocateFour = false;

        [HideInInspector]
        public int CardCount = 0;

        /// <summary>
        /// 临时使用的SoundKey,以后会集成到大厅框架中
        /// </summary>
        [HideInInspector]
        public string SoundKey;

        private int _minBet;
        /// <summary>
        /// 到最后一轮前,玩家手中剩余的最低的筹码值
        /// </summary>
        public int MinRoomGold { set { _minBet = value; } get { return _minBet < 0 ? 0 : _minBet; } }


        /// <summary>
        /// 牌桌上是否有人已经all in , 如果有,则不能继续加注
        /// </summary>
        public bool NoOneAllin
        {
            get
            {
                return
                    PlayerList.Cast<PlayerPanel>()
                        .All(
                            panel =>
                                panel.Info == null || !panel.ReadyState ||
                                (panel.CurGameType != PlayerGameType.AllIn && panel.Coin > 0));
            }
        }

        /// <summary>
        /// 当前房间中,玩家筹码最少的金额
        /// </summary>
        public int LeastRoomGold
        {
            get
            {
                var leastRoomGold = GetPlayer().Info.CoinA;
                foreach (var panel in PlayerList)
                {
                    var playerPanel = (PlayerPanel)panel;
                    if (playerPanel.Info != null && playerPanel.ReadyState &&
                        playerPanel.CurGameType != PlayerGameType.Fold && playerPanel.Coin < leastRoomGold)
                        leastRoomGold = panel.Coin;
                }
                return (int)leastRoomGold;
            }
        }

        /// <summary>
        /// 当前房间中,玩家筹码最少的座位号
        /// </summary>
        public int LeastRoomGoldSeat
        {
            get
            {
                long leastRoomGold = GetPlayer().Info.CoinA;
                int leastSeat = SelfSeat;
                foreach (var panel in PlayerList)
                {
                    var playerPanel = (PlayerPanel)panel;

                    if (playerPanel.Info == null || !playerPanel.ReadyState ||
                        playerPanel.CurGameType == PlayerGameType.Fold || panel.Coin >= leastRoomGold) continue;

                    leastRoomGold = panel.Coin;
                    leastSeat = panel.Info.Seat;
                }
                return leastSeat;
            }
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            Ante = gameInfo.GetInt("ante");
            MaxPoolNum = gameInfo.GetInt("betLimit");
            MinRoomGold = gameInfo.GetInt("tcMin");
            IsGameStart = gameInfo.GetBool("playing");
        }

        /// <summary>
        /// 获取正在游戏的筹码数最少的玩家Panel
        /// </summary>
        public PlayerPanel LeastGoldPanel { get { return GetPlayer<PlayerPanel>(LeastRoomGoldSeat, true); } }
    }
}
