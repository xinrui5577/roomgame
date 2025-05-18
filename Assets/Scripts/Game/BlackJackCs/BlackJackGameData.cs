using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.BlackJackCs
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class BlackJackGameData : YxGameData
    {

        /// <summary>
        /// 当庄的最小限额
        /// </summary>
        public int BankerLimit;


        /// <summary>
        /// 是否接收到GameInfo了
        /// </summary>
        public bool IsGameInfo;

        /// <summary>
        /// 说话等待时间
        /// </summary>
        public int SpeakCd;

        /// <summary>
        /// 游戏是否进行中
        /// </summary>
        public bool IsGameing;

        /// <summary>
        /// 筹码之间的动画间隔
        /// </summary>
        public float BetSpeace = 0.05f;

        /// <summary>
        /// 能下注的最大池底倍数 0为无池底倍数限制
        /// </summary>
        public int MaxPoolNum = 1;

        /// <summary>
        /// 是否是房间模式
        /// </summary>
        public bool IsRoomGame = false;

        /// <summary>
        /// 临时使用的SoundKey,以后会集成到大厅框架中
        /// </summary>
        [HideInInspector]
        public string SoundKey;

        /// <summary>
        /// 自己是否正在游戏
        /// </summary>
        public bool IsPlaying = false;


        private int _showGoldRate = 1;


        public BankerPanel BjBankerBanker;

        /// <summary>
        /// 显示筹码的面值
        /// </summary>
        public int ShowGoldRate
        {
            set { _showGoldRate = value > 0 ? value : 1; }
            get { return _showGoldRate; }
        }

        public BjPlayerPanel GetSelfPanel()
        {
           return GetPlayer<BjPlayerPanel>(SelfLocalSeat);
        }
        

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var blackJackUserInfo = new BlackJackUserInfo();
            blackJackUserInfo.Parse(userData);
            return blackJackUserInfo;
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            if (gameInfo.ContainsKey(RequestKey.KeyUser))
            {
                var userData = gameInfo.GetSFSObject(RequestKey.KeyUser);
                SelfLocalSeat = GetLocalSeat(userData.GetInt(RequestKey.KeySeat));
            }
        }

        public bool IsMyTurn(int serverSeat)
        {
            int localSeat = GetLocalSeat(serverSeat);
            return SelfLocalSeat == localSeat;
        }
    }
}
