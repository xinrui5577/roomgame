using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.Texas
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class TexasGameData : YxGameData
    {
        /// <summary>
        /// 说话等待时间
        /// </summary>
        [HideInInspector]
        public int SpeakCd;
        /// <summary>
        /// 筹码之间的动画间隔
        /// </summary>
        public float BetSpeace = 0.05f;
        /// <summary>
        /// 能下注的最大池底倍数 0为无池底倍数限制
        /// </summary>
        public int MaxPoolNum = 1;

        /// <summary>
        /// 游戏是否已经进行过
        /// </summary>
        [HideInInspector]
        public bool IsPlayed = false;

        /// <summary>
        /// 是否是开房模式
        /// </summary>
        public bool IsRoomGame = false;

        [HideInInspector]
        public int CurRound = 0;

        /// <summary>
        /// 房主Id
        /// </summary>
        private int _ownerId = -1;

        public bool IsRoomOwner
        {
            get
            {
                var selfInfo = GetPlayerInfo();
                return selfInfo != null && selfInfo.Id == _ownerId;
            }
        }

        

        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            Ante = gameInfo.GetInt("ante");
            MaxPoolNum = gameInfo.GetInt("betLimit");
            if (gameInfo.ContainsKey("cargs2"))
            {
                ISFSObject cargs2 = gameInfo.GetSFSObject("cargs2");
                if (cargs2.ContainsKey("-maxp"))
                {
                    int maxp = int.Parse(cargs2.GetUtfString("-maxp"));
                    int len = PlayerList.Length;
                    if (maxp < len)
                    {
                        var list = new YxFramwork.Framework.YxBaseGamePlayer[maxp];
                        for (int i = 0; i < maxp; i++)
                        {
                            list[i] = PlayerList[i];
                        }
                        PlayerList = list;
                    }
                }
            }

            if (gameInfo.ContainsKey("ownerId"))
            {
                _ownerId = gameInfo.GetInt("ownerId");
            }

            GStatus = (YxFramwork.Enums.YxEGameStatus) gameInfo.GetInt("state");
            
        }

       
    }
}
