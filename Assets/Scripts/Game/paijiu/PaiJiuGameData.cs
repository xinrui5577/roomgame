using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.paijiu
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class PaiJiuGameData : YxGameData
    {
        private int _ante = 1;

        /// <summary>
        /// 底注,随牌局变化
        /// </summary>
        public int Ante
        {
            set { _ante = value > 0 ? value : 1; }
            get { return _ante; }
        }

        private int _guoBet = 1;

        /// <summary>
        /// 开局的盲注值
        /// </summary>
        public int GuoBet
        {
            set { _guoBet = value > 0 ? value : 1; }
            get { return _guoBet; }
        }

        /// <summary>
        /// 说话等待时间
        /// </summary>
        public int SpeakCd;

        /// <summary>
        /// 游戏的阶段
        /// 0.准备阶段; 1.下注阶段;　2.组牌阶段;　3.比牌结束;　4.结束;　
        /// </summary>
        public int Status;

        /// <summary>
        /// 游戏是否进行中
        /// </summary>
        public bool IsGameing = false;

        /// <summary>
        /// 有牌阶段
        /// </summary>
        public bool IsCardsStatus
        {
            get { return Status == 2 /*|| Status == 3*/; }
        }

        /// <summary>
        /// 筹码之间的动画间隔
        /// </summary>
        public float BetSpeace = 0.05f;

        /// <summary>
        /// 是否是房间模式
        /// </summary>
        public bool IsRoomGame = false;

        /// <summary>
        /// 庄家座位号
        /// </summary>
        public int BankerSeat = -1;

        /// <summary>
        /// 是否已经开始了游戏
        /// </summary>
        public bool IsPlayed
        {
            get { return CurRound > 0; }
        }

        [HideInInspector]
        public int CurRound = 0;

        /// <summary>
        /// 是否是重连
        /// </summary>
        public bool IsRejion = false;

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new PaiJiuUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        }
    }
}
