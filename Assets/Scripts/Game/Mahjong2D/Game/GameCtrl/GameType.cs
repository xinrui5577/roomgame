namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl {

    public class CurrentGameType
    {
        private int _gameType;

        /// <summary>
        /// 房间类型：-1为创建房间，0~n为后台创建的房间
        /// </summary>
        public int GameRoomType
        {
            set
            {
                _gameType = value;
            }
        }

        public bool IsCreateRoom
        {
            get { return _gameType == -1; }
        }

        /// <summary>
        /// 普通房间id
        /// </summary>
        public int RealRoomId;

        /// <summary>
        /// 房间选择的id
        /// </summary>
        public int ShowRoomId = 0;

        /// <summary>
        /// 房间名称
        /// </summary>
        public string RoomName;

        /// <summary>
        /// 当前的局数
        /// </summary>
        public int NowRound
        {
            set
            {
                _nowRound = value;
            }
            get { return _nowRound; }
        }

        private int _nowRound;

        /// <summary>
        /// 当前的圈数：从0开始，每次都需要自己在服务器返回的数据上加1
        /// </summary>
        public int Quan
        {
            set
            {
                _quan = value;
            }
            get { return _quan; }
        }

        public bool IsQuanExist
        {
            get { return _quanExist; }
            set
            {
                _quanExist = value;
            }
        }

        public string ShowRoundInfo
        {
            get
            {
                 
                if (_quanExist)
                {
                    var quanNum = _quan + 1;
                    if (quanNum>=TotalRound)
                    {
                        quanNum = TotalRound;
                    }
                    _roundAboutInfo = string.Format("圈数：{0}/{1}", quanNum, TotalRound);
                }
                else
                {
                    _roundAboutInfo = string.Format("局数：{0}/{1}", _nowRound, TotalRound);
                }
                return _roundAboutInfo;
            }
        }
        /// <summary>
        /// 创建房间参数（显示）
        /// </summary>
        public string Rules;
        /// <summary>
        /// 简单房间规则（牌桌玩法显示）
        /// </summary>
        public string SimpleRule;

        /// <summary>
        /// 牌桌上显示的圈数相关的信息
        /// </summary>
        private string _roundAboutInfo;
        /// <summary>
        /// 是否按圈计算
        /// </summary>
        private bool _quanExist;
        /// <summary>
        /// 当前圈数
        /// </summary>
        private int _quan;

        /// <summary>
        /// 底注
        /// </summary>
        public int Ante;

        /// <summary>
        /// 倍率
        /// </summary>
        public int Rate=1;

        /// <summary>
        /// 总局数
        /// </summary>
        public int TotalRound;
    }
}
