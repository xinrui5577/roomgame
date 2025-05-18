using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.Tbs
{
    public class TbsGameData : YxGameData
    {
        /// <summary>
        /// 是否除了自己没有人在桌子内
        /// </summary>
        public bool IsNobody;
        /// <summary>
        /// 是否是创建房间
        /// </summary>
        public bool IsCreatRoom;
        /// <summary>
        /// f房间解散的时间限制
        /// </summary>
        public int HupTime;
        /// <summary>
        /// 游戏是否开始
        /// </summary>
        public bool IsGameStart;
        /// <summary>
        /// 游戏规则
        /// </summary>
        public string GameRule;

        public int BankerSeat;
        /// <summary>
        /// 锅内金币
        /// </summary>
        public int GuoGold;
        public int BetTotal;
        public int BetTime;
        public int BankerLimit;
        public int Boold;
        /// <summary>
        /// 从谁开始发牌
        /// </summary>
        public int DealFirstSeat;
        /// <summary>
        /// 首先发牌是哪门
        /// </summary>
        public int DealMen;
        /// <summary>
        /// 发牌的值
        /// </summary>
        public int[][] DealV;
        /// <summary>
        /// 是否已经发牌
        /// </summary>
        public bool IsDeal = false;

        public int Ante;
        public string AnteRate;
        public int RoomType;

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new TbsUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        }

        public override void InitCfg(ISFSObject cargs2)
        {
            base.InitCfg(cargs2);
            AnteRate = cargs2.ContainsKey("-anteRate") ? cargs2.GetUtfString("-anteRate") : "";
            var tout = cargs2.ContainsKey("-tptout") ? cargs2.GetUtfString("-tptout") : "300";
            HupTime = int.Parse(tout);
        }
        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            BankerSeat = gameInfo.GetInt("bank");
            Ante = gameInfo.GetInt("ante");
            var rule = gameInfo.ContainsKey("rule") ? gameInfo.GetUtfString("rule") : "";
            GameRule = rule;


            BankerLimit = Ante * 200;
            /*******设置界面数据*******/
            GuoGold = gameInfo.GetInt("guo");
            if (GuoGold > 0 && BankerSeat >= 0)
            {
                Boold = gameInfo.GetInt("boold");
                BetTotal = gameInfo.GetInt("betgold");

            }
        }
    }
}
