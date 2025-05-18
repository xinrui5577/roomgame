using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.bjl3d
{
    public class Bjl3DGameData : YxGameData
    { 
        /// <summary>
        /// 交互信息
        /// </summary>
        public int[] AreaMaxZhu;//每个区域最多能下多少钱

        public int P;//下注时桌面的位置
        public int[] XianCards;//闲家的牌组
        public int XianValue;//闲的点数
        public int[] BetMoney=new int[ 8];//游戏结束各个位置下注的钱数
        public int[] BetJiesuan = new int[8];//游戏结算的时候每个位置的输赢 
        public int Win;//当前局数输赢的钱数
        public long Total;//当前的金币数量
        public int TodayWin;//今日总合计钱数
        public ISFSArray BankList;//等待上庄庄家的列表
        public int B;//庄家的座位号
        public int[] History=new int[12];//历史记录信息
        public int Hisidx;//历史纪录信息的索引 
        public int BankLimit;//申请下注的最少钱数
        public int Maxante;//游戏开始时位置钱数的限制
        public int[] Allow=new int[8];//游戏进行时个位置的可下注钱数变化，每隔0.5秒变化一次，所以需要判断是否含有这个字段

        public int[] ZhuangCards;//庄家的牌组
        public int ZhuangValue;//庄的点数
         
        public YxBaseUserInfo CurrentBanker = new YxBaseUserInfo();
        public  long ResultBnakerTotal = 0;
        public GameConfig GameConfig = new GameConfig();
        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            BankList = gameInfo.GetSFSArray("bankers");
            B = gameInfo.GetInt("banker");
            Hisidx = gameInfo.GetInt("hisidx");
            History = gameInfo.GetIntArray("history");
            BankLimit = gameInfo.GetInt("bankLimit");
            Maxante = gameInfo.GetInt("maxante");
        }
    }
}

