using System;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.slyz.GameBase {
    public class GlobalMessage
    {
        private readonly Action<ISFSObject> NoneVs = sfsObject => { };
        private readonly Action NoneVn = () => { };
        /// <summary>
        /// 游戏背景音乐音量变化
        /// </summary>
        public Action<float> OnBgMusicVolumeChange;
        /// <summary>
        /// 游戏音效音量变化
        /// </summary>
        public Action<float> OnAudioVolumeChange;

        public Action<ISFSObject> OnGetGameInfo;
        public Action OnJoinRoom;
        public Action OnOutRoom;
        /// <summary>
        /// 用户信息需要刷新的时候调用，需要去服务器查询
        /// </summary>
        public Action OnUserDataRefresh;

        public Action<string> OnSystemLineMessage;
        public Action<string> OnRecieveSystemMessage;
        public Action<bool> OnRecieveSystemMessages;
        public Action<ISFSObject> OnMatchStart;
        public Action<ISFSObject> OnMatchOver;
        public Action<ISFSObject> OnReceiveNotice;
        public Action<ISFSObject> OnReceiveUserBroadcast;
        /// <summary>
        /// 检测是否需要重连游戏回调
        /// </summary>
        internal Action<bool> OnCheckReJoin = value => { };
        /// <summary>
        /// 排行榜变化
        /// </summary>
        public Action<ISFSObject> OnReceveTopRankChange;
        /// <summary>
        /// 连赢8局送10元话费
        /// </summary>
        public Action<ISFSObject> OnReceveContinueWin;
        public Action<ISFSObject> OnUserTalk;
        public Action OnChangePrefix;
		// --------------------------------------------------------- ADD FOR SLYZ - JUST DO UI WORK
        public Action OnConnectLost;                              // 掉线提示
		// RightPanel
		public Action OnShowAllGoldInfo;							// 显示奖池、下注金额 
        public Action OnShowTotalGlod;                            // 显示余额信息
		// MessageBG
		public Action OnShowMessage;								// 显示消息

		// LeftPanel
        public Action OnGameInit;                                 // 游戏UI初始化 点击开始按钮后 调用
        public Action OnGameStart;							    // 开始游戏回包 显示牌型等信息 
        public GlobalMessage()
        {
            OnGameStart = NoneVn;
            OnGameInit = NoneVn;
            OnShowMessage = NoneVn;
            OnShowTotalGlod = NoneVn;
            OnShowAllGoldInfo = NoneVn;
            OnConnectLost = NoneVn;
            OnChangePrefix = NoneVn;
            OnUserTalk = NoneVs;
            OnReceveContinueWin = NoneVs;
            OnReceveTopRankChange = NoneVs;
            OnReceiveUserBroadcast = NoneVs;
            OnReceiveNotice = NoneVs;
            OnMatchOver = NoneVs;
            OnMatchStart = NoneVs;
            OnUserDataRefresh = NoneVn;
            OnOutRoom = NoneVn;
            OnJoinRoom = NoneVn;
            OnGetGameInfo = NoneVs;
        } 
    }
}
