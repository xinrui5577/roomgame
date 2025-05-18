using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.Shuihuzhuan
{
    public class WmarginGameData : YxGameData
    {
        /// <summary>
        /// 线数
        /// </summary>
        public int BetLineNum = 0;
        /// <summary>
        ///  总压注钱数
        /// </summary>
        public int BetNum = 0;
        /// <summary>
        /// 压注钱
        /// </summary>
        public int BetBaseNum = 0; 
        /// <summary>
        /// 小
        /// </summary>
        public int Yazhu1 = 0;
        /// <summary>
        /// 大
        /// </summary>
        public int Yazhu2 = 1;
        /// <summary>
        /// 和
        /// </summary>
        public int Yazhu3 = 2;
        //-----------------------------------------------------
        /// <summary>
        /// 是否自动
        /// </summary>
        public bool IsAuto = false;
        /// <summary>
        /// 切换游戏状态是否完成
        /// </summary>
        public bool changeState = false;
        /// <summary>
        /// 小玛丽的状态
        /// </summary>
        public bool isMary = false;
        /// <summary>
        /// 状态   1 刚下注  2  输了   3  赢了
        /// </summary>
        public int ZhuanState = 1;
        /// <summary>
        /// 进入玛丽的状态
        /// </summary>
        public bool Malizhuantai = false;

        public bool BeginBet = false;
        //-----------------------------------------------------服务器数据
        /// <summary>
        /// 水浒传服务器数据 
        /// </summary>
        public int[] iTypeImgid = new int[15];

        /// <summary>
        /// 服务器数据
        /// </summary>
        public int[] iLineImgid = new int[18];
        /// <summary>
        /// 小和大服务器数据 
        /// </summary>
        public int[] iHistory = new int[10];
        /// <summary>
        /// 当局所得
        /// </summary>
        public int iWinMoney = -1;
        /// <summary>
        /// 骰子数
        /// </summary>
        public int iDice1 = 0;
        public int iDice2 = 0;
        /// <summary>
        //  玛丽次数
        /// </summary>
        public int iMaliGames = 0;
        /// <summary>
        /// 接受4个数组
        /// </summary>
        public int[] iMaliImage = new int[4];
        /// <summary>
        /// 玛丽转的图片
        /// </summary>
        public int iMaliZhuanImage = 0;
        /// <summary>
        /// 玛丽赢得钱数
        /// </summary>
        public int MaliWinMony = 0;
        /// <summary>
        ///下注上限
        /// </summary>
        public int iXiazhushangxian = 0;
        /// <summary>
        /// 线数
        /// </summary>
        public int iBetLineNum = 0;
        /// <summary>
        ///  总压注钱数
        /// </summary>
        public int iBetNum = 0;
        /// <summary>
        /// 压注钱
        /// </summary>
        public int iBetBaseNum = 0; 
        public bool IsAotozhuangtai = true;
        //-----------------------------------------------------
        /// <summary>
        ///9条线
        /// </summary>
        public int[,] m_TypeArray = { { 5, 6, 7, 8, 9 }, { 0, 1, 2, 3, 4 }, { 10, 11, 12, 13, 14 }, { 0, 6, 12, 8, 4 }, { 10, 6, 2, 8, 14 }, { 0, 1, 7, 3, 4 }, { 10, 11, 7, 13, 14 }, { 5, 11, 12, 13, 9 }, { 5, 1, 2, 3, 9 } };
        /// <summary>
        /// 结果的数据
        /// </summary>
        public int[,] m_ResultArray = { { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 } };
        /// <summary>
        /// 9条线初始动画
        /// </summary>
        public int[] m_LineType = new int[9];
        /// <summary>
        /// 显示秒动画的位置
        /// </summary>
        public int[] m_ShowSecAnimate = new int[15];
        /// <summary>
        /// 是否显示提示
        /// </summary>
        public bool NeedShowAnim = false;

        public long CurrentTotalMoney = 0;

        public string[] LineTypeSoundName = new []{ "shuihuchuan", "zhongyitang", "titianxingdao", "songjiang", "linchong", "luzhisheng", "daoguang", "daoguang", "daoguang"};
        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            NeedShowAnim = gameInfo.GetBool("show");
            BetLineNum = gameInfo.GetInt("line");
            iXiazhushangxian = gameInfo.GetInt("ante");
            BetBaseNum = gameInfo.GetInt("ante");
            BetNum = App.GetGameData<WmarginGameData>().BetLineNum * App.GetGameData<WmarginGameData>().BetBaseNum;
            Facade.EventCenter.DispatchEvent(EWmarginEventType.RefreshTotalMoney, GetPlayerInfo().CoinA);
        }
    }
}

