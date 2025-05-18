using System.Collections.Generic;

namespace Assets.Scripts.Game.bjl3d
{
    public class GameConfig
    {

        /// <summary>
        /// 下注时间 15
        /// </summary>
        public int XiaZhuTime = 15;
        /// <summary>
        /// 开拍时间 10
        /// </summary>
        public int KaiPaiTime = 10;
        /// <summary>
        ///  空闲时间10
        /// </summary>
        public int FreeTime = 10;
        /// <summary>
        /// 显示中奖时间5
        /// </summary>
        public int ShowWinTime = 5;


        /// <summary>
        /// 筹码类型
        /// </summary>
        public int CoinType = 0;
        /// <summary>
        /// 下庄时间
        /// </summary>
        public bool IsXiaZhuTime = true;

        public bool IsSuperUser = false;

        //发牌标记
        /// <summary>
        /// 发牌标记
        /// </summary>
        public int XFapaiSpeedflag = 0;

        //每个区域能下最大注码
        /// <summary>
        ///每个区域能下最大注码
        /// </summary>
        public long[] LAreaMaxZhu = new long[8];


        //自己是否是庄家
        /// <summary>
        /// 自己是否为庄家
        /// </summary>
        public bool IsSelfZ = false;

        //上庄需要最少金币
        /// <summary>
        /// 上庄需要最少金币
        /// </summary>
        public long ShangZhuangLimitMoney = 0;
        /// <summary>
        /// 历史纪录页面的显示数据
        /// </summary>
        public List<int> LuziInfo=new List<int>();
        /// <summary>
        /// 游戏的状态
        /// </summary>
        public int GameState;
        public int GameNum = 0;
        public bool isXianshi;

    }
}

