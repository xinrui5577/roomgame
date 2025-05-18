using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.fillpit
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class FillpitGameData : YxGameData
    {
        /// <summary>
        /// 房间底注值,不会变化
        /// </summary>
        public int BaseAnte = 0;

        /// <summary>
        /// 每局游戏ante值,翻倍和烂底局会变化
        /// </summary>
        public int CurAnte;
       
        /// <summary>
        /// 说话等待时间
        /// </summary>
        public int SpeakCd = 12;

        /// <summary>
        /// 游戏是否进行中
        /// </summary>
        public bool IsGameing
        {
            set { _gaming = value;
            }
            get { return _gaming; }
        }

        private bool _gaming;
        /// <summary>
        /// 筹码之间的动画间隔
        /// </summary>
        public float BetSpeace = 0.05f;
        /// <summary>
        /// 能下注的最大池底倍数 0为无池底倍数限制
        /// </summary>
        public int MaxPoolNum = 1;

        /// <summary>
        /// 是否是房间模式
        /// </summary>
        public bool IsRoomGame = false;

        [HideInInspector] public string SoundKey;

        /// <summary>
        /// 是否是双倍场
        /// </summary>
        public bool IsDoubleGame = false;

        /// <summary>
        /// 本局的结果是否是烂底
        /// </summary>
        public bool IsLanDi = false;

        private bool _isPlayed;

        /// <summary>
        /// 是否已经开始了游戏
        /// </summary>
        public bool IsPlayed
        {
            set { _isPlayed = value; }
            get { return IsRoomGame && _isPlayed; }
        }

        private bool _roomPlayed;

        public bool RoomPlayed
        {
            set { _roomPlayed = value; }
            get { return _roomPlayed; }
        }

        public int OwnerId = -1;

        /// <summary>
        /// 庄座位号
        /// </summary>
        public int Banker=-1;
        /// <summary>
        /// 倒计时准备
        /// </summary>
        public int ReadyCd = -1;
        /// <summary>
        /// 是否显示搓牌界面
        /// </summary>
        public bool ShowSeeCardView;

        /// <summary>
        /// 房间ID
        /// </summary>
        public int RoomId;

        /// <summary>
        /// 喜分,用于烂底扣分专用
        /// </summary>
        public int Happys;
   
        /// <summary>
        /// 是否有赖子牌
        /// </summary>
        public bool HaveLaiZi;

        /// <summary>
        /// 赖子牌的数值(带花色)
        /// </summary>
        public int LaiziValue;

        public int LastBetValue;

        public bool Dkak;

        public bool Sfak;

        public bool Nmno;
        /// <summary>
        /// 投票倒计时
        /// </summary>
        public int tpcdTime = 300;

        public bool ShowStartBtn
        {
            get { return IsRoomGame && !IsPlayed && SelfSeat == 0 && !Nmno; }
        }

        public bool IsRoomOwner
        {
            get
            {
                var selfInfo = GetPlayerInfo();
                return selfInfo != null && (OwnerId < 0 ? selfInfo.Seat == 0 : selfInfo.Id == OwnerId);
            }
        }

        protected override void InitGameData(ISFSObject gameInfo)
        {
            //cargs2 初始化
            base.InitGameData(gameInfo);

            //初始化房间参数
            if (gameInfo.ContainsKey("cargs2"))
            {
                var cargs2 = gameInfo.GetSFSObject("cargs2");
                if (cargs2 != null)
                {
                    if (cargs2.ContainsKey("-happys"))
                    {
                        Happys = int.Parse(cargs2.GetUtfString("-happys"));
                    }

                    if (cargs2.ContainsKey("-laizi"))
                    {
                        LaiziValue = int.Parse(cargs2.GetUtfString("-laizi"));
                        HaveLaiZi = LaiziValue > 0;
                    }

                    if (cargs2.ContainsKey("-crcd"))
                    {
                        SpeakCd = int.Parse(cargs2.GetUtfString("-crcd"));
                    }

                    if (cargs2.ContainsKey("-sfak"))
                    {
                        Sfak = int.Parse(cargs2.GetUtfString("-sfak")) > 0;
                    }

                    if (cargs2.ContainsKey("-dkak"))
                    {
                        Dkak = int.Parse(cargs2.GetUtfString("-dkak")) > 0;
                    }

                    if (cargs2.ContainsKey("-pointdif"))
                    {
                        bool showPointDif = !cargs2.GetUtfString("-pointdif").Equals("0");
                        foreach (var player in PlayerList)
                        {
                            var panel = (PlayerPanel)player;
                            panel.ShowPointDif = showPointDif;
                        }
                    }

                    if (cargs2.ContainsKey("-tptout"))
                    {
                        tpcdTime = int.Parse(cargs2.GetUtfString("-tptout"));
                    }

                    Nmno = cargs2.ContainsKey("-nmno");

                    //int hiden = cargs2.ContainsKey("-hideN") ? int.Parse(cargs2.GetUtfString("-hideN")) : 2;
                    //int fShowN = cargs2.ContainsKey("-fShowN") ? int.Parse(cargs2.GetUtfString("-fShowN")) : 1;
                }
            }


            //初始化房间信息
            if (gameInfo.ContainsKey("curante"))
            {
                CurAnte = gameInfo.GetInt("curante");
            }

            if (gameInfo.ContainsKey("playing"))
            {
                IsGameing = gameInfo.GetBool("playing");
            }

            if(gameInfo.ContainsKey("ownerId"))
            {
                OwnerId = gameInfo.GetInt("ownerId");
            }

            if (gameInfo.ContainsKey("banker"))
            {
                Banker = gameInfo.GetInt("banker");
            }

            if (gameInfo.ContainsKey("readyCd"))
            {
                ReadyCd = gameInfo.GetInt("readyCd");
            }
          
            var selfPanel = GetPlayer<PlayerPanel>();
            selfPanel.SetReadyBtnActive(!selfPanel.ReadyState);
        }

    }
}
