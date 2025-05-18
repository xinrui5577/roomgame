using Assets.Scripts.Game.sanpian.CommonCode.ChessCommon;
using Assets.Scripts.Game.sanpian.server;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.sanpian.DataStore
{
    public class SanPianGameData : YxGameData
    {
        public bool GameStart = false;

        public int MaxPeopleNum = 4;

        /// <summary>
        /// 规则
        /// </summary>
        public string Rule {private set; get; }

        //局数
        private int Rnum ;

        //设置局数，必须先获取最大局数
        public int RoundNum
        {
            get { return Rnum; }
            set
            {
                Rnum = value;
                App.GetGameManager<SanPianGameManager>().UIButtonCtrl.RoundNum.text = value + "/" + MaxRound;
            }
        }

        public int MaxRound;

        private int Rid;

        /// <summary>
        /// 是否为连片模式
        /// </summary>
        public bool IsLianPian { get; private set ; }


        //设置房间号
        public int RoomID
        {
            get { return Rid; }
            set
            {
                Rid = value;
                App.GetGameManager<SanPianGameManager>().UIButtonCtrl.RoomId.text = value + "";
                App.GetGameManager<SanPianGameManager>().UIButtonCtrl.RoomIdBig.text = value + "";
            }
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            IsLianPian = gameInfo.GetBool(Constants.KeyLianPian);
            if (gameInfo.ContainsKey("rule"))
            {
               Rule = gameInfo.GetUtfString("rule");
            }
        }

        public bool IsMusicOn = true;
        
        /// <summary>
        /// 当前游戏状态
        /// </summary>
        public GameStatus CurStatus = GameStatus.None;
        public int Few = 0;
        public readonly GlobalMessage GMessage = new GlobalMessage();
        /// <summary>
        /// 是否接收到GameInfo了
        /// </summary>
        public bool IsGameInfo = false;
        /// <summary>
        /// 游戏底注
        /// </summary>
        public int Ante = 0;
        /// <summary>
        /// 当前游戏类型
        /// </summary>
        public GameType CurGameType;
        /// <summary>
        /// 是否为开房游戏
        /// </summary>
        public bool IsRoomGame;

        /// <summary>
        /// 房主的ID
        /// </summary>
        public int OwnerId;
        /// <summary>
        /// 是否播放语音聊天
        /// </summary>
        public bool IsVoiceChat;
        /// <summary>
        /// 默认男生头像
        /// </summary>
        public Texture2D DefaultManIcon;
        /// <summary>
        /// 默认女生头像
        /// </summary>
        public Texture2D DefaultWomanIcon;

        /// <summary>
        /// 声音开关
        /// </summary>
        public bool isChatVoiceOn;

        public bool IsPlayed
        {
            get { return RoundNum > 0; }
        }

        public string[] Common =
        {
            "哎呀哎呀，厉害了我滴哥!" ,
            "把钱还我不玩了!",
            "你是乃嘎达滴银啊?",
            "咋整啊哥们，不会了!",
            "这牌让你打的，稀能!"
        };
    }

    /// <summary>
    /// 游戏类型
    /// </summary>
    public enum GameType
    {
        /// <summary>
        /// 叫分
        /// </summary>
        CallScore = 0,
        /// <summary>
        /// 踢地主
        /// </summary>
        Kick,
        /// <summary>
        /// 抢地主
        /// </summary>
        Grab,
        /// <summary>
        /// 叫分带流局
        /// </summary>
        CallScoreWithFlow,
    }
    /// <summary>
    /// 游戏当前状态
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// 空闲
        /// </summary>
        None,
        /// <summary>
        /// 选择庄家
        /// </summary>
        ChoseBanker,
        /// <summary>
        /// 加倍中
        /// </summary>
        Double,
        /// <summary>
        /// 游戏中
        /// </summary>
        Game,
        /// <summary>
        /// 结算中
        /// </summary>
        Result,
    }
}
