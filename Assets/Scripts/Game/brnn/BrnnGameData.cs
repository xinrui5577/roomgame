using System.Collections.Generic;
using YxFramwork.Common.Utils;
using Sfs2X.Entities.Data;
using YxFramwork.Common.Model;
using YxFramwork.Enums;

namespace Assets.Scripts.Game.brnn
{
    public class BrnnGameData : YxGameData
    {
        /// <summary>
        /// 用户的登陆账号
        /// </summary>
        public string LoginName = "";
        public string Password = "";
        public string UserId = "";
        public string UserToken = "";

        /// <summary>
        /// 自己
        /// </summary>
        public List<YxBaseUserInfo> UserList = new List<YxBaseUserInfo>();
        public BrnnPlayer CurrentBanker;
        
        public bool BeginBet = false;
        public int CurrentCanInGold = -1;
        public int MiniApplyBanker = 50000;
        public long ThisCanInGold = 0;
        public long ResultBnakerTotal = 0;
        public int ResultUserTotal = 0;

        private int _maxNiuRate = 1;
        public int MaxNiuRate
        {
            private set { _maxNiuRate = value > 0 ? value : 1; }
            get { return _maxNiuRate; }
        }

        /// <summary>
        /// 自己是不是庄家
        /// </summary>
        public bool IsBanker
        {
            get
            {
                var info = CurrentBanker.Info;
                return info != null && info.Seat == GetPlayerInfo().Seat;
            }
        }

        /// <summary>
        /// 房间号
        /// </summary>
        public int RoomType;

        public bool IsMusicOn = true;

        public List<string> RadioList;

        //押注钱数
        public int[] ZNumber;

        public void SetGameStatus(YxEGameStatus status)
        {
            if (IsBanker)
            {
                GStatus = YxEGameStatus.PlayAndConfine;
                return;
            }

            GStatus = status;
        }


        protected override void InitGameData(ISFSObject gameInfo)
        {
            base.InitGameData(gameInfo);
            MiniApplyBanker = gameInfo.GetInt("bkmingold");
            if (gameInfo.ContainsKey("maxNiuRate"))
            {
                MaxNiuRate = gameInfo.GetInt("maxNiuRate");
            }
        }
    }
}
