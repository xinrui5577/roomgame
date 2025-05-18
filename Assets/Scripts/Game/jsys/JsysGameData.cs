using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.jsys
{
    public class JsysGameData : YxGameData
    {
        /// <summary>
        /// 用户的登陆账号
        /// </summary>
        public  string LoginName;
        public string Password;
        public  string UserId;
        public  string UserToken;
        public long UserMoney;//初始化玩家的钱数
        public  int BetTime; //开奖时间
        public  int[] BetPos=new int[ 12];
        public  int[] BetData;
        public int[] Multiplying/* = new int[12]*/;//倍率
        public  int Winning;//彩金
        public  int[] History/* = new int[12]*/; //路子   
        public  long StartTime;//游戏运行的时间
        public  bool Rejoin;
        public  long Svt;//系统时间
        public  long BetBehindMoney;//下注后的钱
        public  int Gold;//游戏后输赢的金币
        public int WinGold;//获得的总金币
        public bool StartBet;

        public bool IsAnimal;//是否是动物
        public bool IsShark; 
        //}//是否为鲨鱼
        public  int FishIdx=1;//鲨鱼的索引

        public int StarPos;        ///开始位置
        public int EndPos;         ///结束位置
        public int Winnings;//彩金
        public int EndAnimal;//最后一个动物
        public int SharkPos;//鲨鱼的位置
        public bool Judge =true;//判断
        public bool Judge1 = false;//判断
        public bool isOut;//是否可以退出房间
 
        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            var user = gameInfo.GetSFSObject("user");
            UserMoney = user.GetLong("ttgold");
            BetTime = gameInfo.GetInt("cd");
            History = gameInfo.GetIntArray("history");//历史纪录 
            Multiplying = gameInfo.ContainsKey("rates") ? gameInfo.GetIntArray("rates") : null;
            //判断是否为重连
            if (gameInfo.ContainsKey("rejoin"))
            {
                Rejoin = gameInfo.GetBool("rejoin");
                Svt = gameInfo.GetLong("svt");
                StartTime = gameInfo.GetLong("startTime");
            }
            StartBet = false;
        }

        /// <summary>
        /// 程序的启动参数
        /// </summary>
        public  Dictionary<string, string> BootEnv;
        /// <summary>
        /// 房间号
        /// </summary>
        public  int RoomType = 0;

        public  bool IsMusicOn = false;

        public  List<string> RadioList;

      
    }
}

