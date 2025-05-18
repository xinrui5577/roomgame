using System;

namespace Assets.Scripts.Game.FishGame.ChessCommon
{
	public class RequestKey
	{
	    public const String KeyId = "id";
        /**类型[Key]*/
	    public const String KeyType = "type";
	    /**座位号[Key]*/
	    public const String KeySeat= "seat";
	    /**牌，数组[Key]*/
	    public const String KeyCards= "cards";
	    public const String KeyCard= "card";


	    public const string KeySeq = "seq";

        /**牌，数组[Key]*/
        public const String KeyCardsArr = "cardArr";
	    /**牌，单个[Key]*/
	    public const String KeyOpCard= "opCard";
	    /**金币，或者钱。[Key]*/
        public const String KeyGold = "gold";

        public const string KeyCoin = "coin";

        /// <summary>
        /// 金币，总数
        /// </summary>
        public const String KeyTotalGold = "ttgold";

        public const string KeyIsRegister = "isRegister";

	    public const string KeyName = "name";

	    public const string KeyUserList = "users";

	    public const string KeyUser = "user";
        public const string Country="country";

	    public const string KeyState = "state";

	    public const string KeyPlayerList = "players";
        /// <summary>
        /// 
        /// </summary>
        public const string KeyScore = "score";

        /// <summary>
        /// 胡牌的类型/打牌的类型
        /// </summary>
        public const string KeyCardType = "ctype";

	    public const string KeyTingList = "tings";

	    public const string KeyIsSame = "sm";
        public const string KeyPlaying = "playing"; //正在游戏标记

	    public const string KeyHasTing = "hasting";

	    public const string KeyHuType = "htype";

	    public const string KeyPiao = "piao";
		
		public const string KeyRoomPasswd = "-rp";
		
		public const string KeyGroups = "groups";
        public const string KeyExp = "exp";
        public const string KeyText = "text";

        /// <summary>
        /// 后台发回来直接显示的消息
        /// </summary>
	    public const string KeyMessage = "msg";
        public const string KeyMessages = "msgs";
        /// <summary>
        /// Code 
        /// </summary>
	    public const string KeyCode = "code";

	    public const string KeyNeedReJoin = "needRejoin";
	    public const string KeyRmlist = "rmlist";
	}
}
