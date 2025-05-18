using System;

namespace Assets.Scripts.Game.FishGame.ChessCommon
{
	public class RequestCMD
	{
        	/**
	     * 玩家准备好 
	     */
	    public const String Ready = "ready";
	
	    /**
	     * 创建游戏（房间）
	     */
	    public const String Create = "create";
	
	    /**
	     * 解除托管
	     */
	    public const String EndAuto = "AutoF";

	    /**
	     * 进入托管
	     */
	    public const String IntoAuto = "AutoT";
	
	
	    /**
	     * 正常交互
	     */
	    public const String Request = "rqst";

	    public const string RoomList = "rlst";
	
	    /**
	     * 加入房间
	     **/
	    public const String JoinRoom = "jRoom";

	    public const string ReJoinRoom = "rjin";

        public const string CheckNeedReJoin = "crj";

	    public const string UserOutRoom = "uout";
	
	    /**摇色子*/
	    public const String RollDICE ="rDice";
	
	
	    public const String QuickGame = "qkGame";

        /// <summary>
        /// 获得游戏列表
        /// </summary>
	    public const String GetGames = "grps";
        /// <summary>
        /// 房间列表
        /// </summary>
        public const String GetRooms = "rooms";

	   
	    public const string GameInfo = "gameInfo";

	    public const string BugReport = "bugrep";
        public const string UserTalk = "talk";
        public const string CheckRoomPassword = "chpswd";

	    public const string GameMessage ="errmsg";

	    public const string GetServerTime = "svt";
        public const string UserBroadcast = "ubc";
        public const string UOut = "uout";
	}
}
