using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.sssjp
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class SssGameData : YxGameData
    {
       
        [HideInInspector]
        public bool SwatModel = true;

        /// <summary>
        /// 庄家座位号
        /// </summary>
        [HideInInspector]
        public int BankerSeat = 0;

        [HideInInspector]
        public bool IsBankerModel = false;

        /// <summary>
        /// 是否是房间模式
        /// </summary>
        [HideInInspector]
        public bool IsRoomGame;

        /// <summary>
        /// 当前局数,从1开始
        /// </summary>
        [HideInInspector]
        public int CurRound = 0;

        /// <summary>
        /// 游戏是否正在进行
        /// </summary>
        [HideInInspector]
        public bool IsPlaying;

        [HideInInspector]
        public int ChoiseModel = -1;


        /// <summary>
        /// 倒计时自动准备时间
        /// </summary>
        [HideInInspector] public int ReadyTime = 59;

        [HideInInspector] public bool Special = true;

        /// <summary>
        /// 是否倒计时自动准备
        /// </summary>
        public bool AutoReady;

        /// <summary>
        /// 是否显示倒计时时间
        /// </summary>
        public bool ShowAutoReadyTime
        {
            get { return AutoReady && IsPlayed && ReadyTime > 0; }
        }

        /// <summary>
        /// 游戏是否已经开始
        /// </summary>
        [HideInInspector]
        public bool IsPlayed
        {
            get { return CurRound > 0 || IsPlaying; }
        }

        /// <summary>
        /// 玩家开房模式
        /// 0.不消耗房卡 ; 1.房主消耗房卡 ; 2.所有人消耗房卡 ; 3.代开房间
        /// 此处之处理3模式,此变量用于判断是否是3号模式
        /// </summary>
        public int UserType = 3;

        /// <summary>
        /// 是否是代开房间
        /// </summary>
        public bool DaiKai { get; set; }

        /// <summary>
        /// 打枪的加分,为0时,为翻倍场
        /// </summary>
        [HideInInspector]
        public int ShootScore;

        /// <summary>
        /// 选择牌的时间
        /// </summary>
        [HideInInspector]
        public int PutTime;

         /// <summary>
         /// 是否有打枪
         /// </summary>
        [HideInInspector]
        public bool HaveShoot = true;

        /// <summary>
        /// 用于将筹码修改为小数情况
        /// </summary>
        private int _showGoldRate = 1;

        /// <summary>
        /// 茶馆ID
        /// </summary>
        public string TeaID;

        /// <summary>
        /// 用于将筹码修改为小数情况
        /// </summary>
        public int ShowGoldRate
        {
            set { _showGoldRate = value; }
            get
            {
                _showGoldRate = _showGoldRate > 0 ? _showGoldRate : 1;
                return _showGoldRate;
            }
        }

        protected override Dictionary<int, int> CreateCustomSeatInfo()
        {
            var dic = new Dictionary<int, int>();
            dic[SelfSeat] = 0;
            int len = PlayerList.Length;
            int localSeat = 1;
            for (int i = 0; i < len; i++)
            {
                if (i == SelfSeat)
                {
                    continue;
                }
                dic[i] = localSeat++;
            }
            return dic;
        }      

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);

            //初始化游戏信息
            IsRoomGame = gameInfo.ContainsKey("rid");
            if (gameInfo.ContainsKey("teaId"))
            {
                TeaID = gameInfo.GetUtfString("teaId");
            }
            if (gameInfo.ContainsKey("status"))
            {
                int status = gameInfo.GetInt("status");
                bool playing = status > 0;
                IsPlaying = playing;
                IsGameStart = playing;
            }
            if (gameInfo.ContainsKey("puttime"))
            {
                PutTime = gameInfo.GetInt("puttime");
            }
        }

        public override void InitCfg(ISFSObject cargs2)
        {
            base.InitCfg(cargs2);

            if (cargs2.ContainsKey("-resulttype"))
            {
                SwatModel = false;
            }
            if (cargs2.ContainsKey("-daqiang"))
            {
                ShootScore = int.Parse(cargs2.GetUtfString("-daqiang"));
            }
            if (cargs2.ContainsKey("-ante"))
            {
                Ante = int.Parse(cargs2.GetUtfString("-ante"));
            }
            if (cargs2.ContainsKey("-usetype"))
            {
                //处理是否是代开模式
                DaiKai = cargs2.GetInt("-usetype") == UserType;
            }
            if (cargs2.ContainsKey("-coustomautord"))
            {
                AutoReady = !cargs2.GetUtfString("-coustomautord").Equals("0");
                if (cargs2.ContainsKey("-autoreadytime"))
                {
                    ReadyTime = int.Parse(cargs2.GetUtfString("-autoreadytime"));
                }
            }

            if (cargs2.ContainsKey("-quicktime"))
            {
                //是否有过打枪
                HaveShoot = cargs2.GetUtfString("-quicktime").Equals("0");
            }
            
            Special = cargs2.ContainsKey("-special") && !cargs2.GetUtfString("-special").Equals("0");
            HelpLz.Special = Special;
        }

        protected override YxBaseGameUserInfo OnInitUser(ISFSObject userData)
        {
            var userInfo = new SssUserInfo();
            userInfo.Parse(userData);
            return userInfo;
        }

    }


}
