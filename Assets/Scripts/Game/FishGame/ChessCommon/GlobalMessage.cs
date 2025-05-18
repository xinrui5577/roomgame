namespace Assets.Scripts.Game.FishGame.ChessCommon {
    public class GlobalMessage
    {
        private static readonly DVoidSfsObject NoneVs = sfsObject => { };
        private static readonly DVoidNoParam NoneVn = () =>{};
        /// <summary>
        /// 游戏背景音乐音量变化
        /// </summary>
        public static DVoidFloat OnBgMusicVolumeChange;
        /// <summary>
        /// 游戏音效音量变化
        /// </summary>
        public static DVoidFloat OnAudioVolumeChange;

        /// <summary>
        /// 登陆完成
        /// </summary>
        public static DVoidNoParam OnLogin = NoneVn;

        public static DVoidSfsObject OnGetGameInfo = NoneVs;


        public static DVoidNoParam OnJoinRoom = NoneVn;

        public static DVoidNoParam OnOutRoom = NoneVn;
        /// <summary>
        /// 用户信息需要刷新的时候调用，需要去服务器查询
        /// </summary>
        public static DVoidNoParam OnUserDataRefresh = NoneVn;

        public static DVoidString OnSystemLineMessage;
        public static DVoidString OnRecieveSystemMessage;
        public static DVoidBool OnRecieveSystemMessages;
        public static DVoidSfsObject OnMatchStart = NoneVs;
        public static DVoidSfsObject OnMatchOver = NoneVs;

        public static DVoidSfsObject OnReceiveNotice = NoneVs;

        public static DVoidSfsObject OnReceiveUserBroadcast = NoneVs;
        /// <summary>
        /// 检测是否需要重连游戏回调
        /// </summary>
        internal static DVoidBool OnCheckReJoin = value => { };

        /// <summary>
        /// 排行榜变化
        /// </summary>
        public static DVoidSfsObject OnReceveTopRankChange = NoneVs;

        /// <summary>
        /// 连赢8局送10元话费
        /// </summary>
        public static DVoidSfsObject OnReceveContinueWin = NoneVs;

        public static DVoidSfsObject OnUserTalk = NoneVs;


        public static DVoidNoParam OnChangePrefix = NoneVn;
    }
}
