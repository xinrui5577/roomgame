namespace Assets.Scripts.Game.lyzz2d.Game.GameCtrl
{
    public class CurrentGameType
    {
        private int _gameType;

        private string _roundAboutInfo;

        /// <summary>
        ///     底注
        /// </summary>
        public int Ante;

        /// <summary>
        ///     倍率
        /// </summary>
        public int Rate = 1;

        /// <summary>
        ///     普通房间id
        /// </summary>
        public int RealRoomId;

        /// <summary>
        ///     房间ID
        /// </summary>
        public string RoomId;

        /// <summary>
        ///     规则信息
        /// </summary>
        public string RuleInfo;

        /// <summary>
        ///     房间选择的id
        /// </summary>
        public int ShowRoomId;

        /// <summary>
        ///     总局数
        /// </summary>
        public int TotalRound;

        /// <summary>
        /// 房主ID
        /// </summary>
        public int OwnerId;

        /// <summary>
        ///     房间类型：-1为创建房间，0~n为后台创建的房间
        /// </summary>
        public int GameRoomType
        {
            set
            {
                _gameType = value;
                switch (_gameType)
                {
                    case -1:
                        RoomId = ShowRoomId.ToString();
                        break;
                    default:
                        ShowRoomId = 0;
                        RoomId = "";
                        break;
                }
            }
            get { return _gameType; }
        }

        /// <summary>
        ///     当前的局数
        /// </summary>
        public int NowRound { set; get; }

        /// <summary>
        ///     当前的圈数：从0开始，每次都需要自己在服务器返回的数据上加1
        /// </summary>
        public int Quan { set; get; }

        public bool IsQuanExist { get; set; }

        public string ShowRoundInfo
        {
            get
            {
                if (IsQuanExist)
                {
                    _roundAboutInfo = string.Format("圈数：{0}/{1}", Quan + 1, TotalRound);
                }
                else
                {
                    _roundAboutInfo = string.Format("局数：{0}/{1}", NowRound, TotalRound);
                }
                return _roundAboutInfo;
            }
        }
    }
}