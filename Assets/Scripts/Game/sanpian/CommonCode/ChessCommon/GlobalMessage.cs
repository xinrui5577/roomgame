using Assets.Scripts.Game.paoyao.CommonCode.SrcGameBase;
using Assets.Scripts.Game.sanpian.CommonCode.SrcGameBase;

namespace Assets.Scripts.Game.sanpian.CommonCode.ChessCommon {
    public class GlobalMessage
    {
        /// <summary>
        /// 游戏背景音乐音量变化
        /// </summary>
        public DVoidFloat OnBgMusicVolumeChange;
        /// <summary>
        /// 游戏音效音量变化
        /// </summary>
        public DVoidFloat OnAudioVolumeChange;

        /// <summary>
        /// 登陆完成
        /// </summary>
        public DVoidNoParam OnLogin;

        public DVoidSfsObject OnGetGameInfo;


        public DVoidNoParam OnJoinRoom;

        public DVoidNoParam OnOutRoom;
        /// <summary>
        /// 用户信息需要刷新的时候调用，需要去服务器查询
        /// </summary>
        public DVoidNoParam OnUserDataRefresh;

        public DVoidString OnRecieveSystemMessage;
        public DVoidBool OnRecieveSystemMessages;
        public DStrString OnRecieveSystemNotice;
        public DVoidSfsObject OnMatchStart;
        public DVoidSfsObject OnMatchOver;

        public DVoidSfsObject OnReceiveUserBroadcast;
        /// <summary>
        /// 检测是否需要重连游戏回调
        /// </summary>
        internal DVoidBool OnCheckReJoin;

        public DVoidSfsObject OnReceveTopRankChange;

        public DVoidSfsObject OnUserTalk;
        
    }
}
