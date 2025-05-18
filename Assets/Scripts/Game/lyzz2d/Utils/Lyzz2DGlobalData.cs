using System;
using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.lyzz2d.Utils
{
    public class Lyzz2DGlobalData : YxGameData
    {
        /// <summary>
        ///     本局游戏中的牌型
        /// </summary>
        private readonly List<int> _typeList = new List<int>();

        [HideInInspector]
        /// <summary>
        /// 庄的实际位置
        /// </summary>
        public int Bank = -1000;

        [HideInInspector]
        /// <summary>
        /// 随机庄的起始位置
        /// </summary>
        public int Bank0;

        /// <summary>
        ///     程序的启动参数
        /// </summary>
        public Dictionary<string, string> BootEnv;

        /// <summary>
        ///     房间参数对应的名称
        /// </summary>
        private Dictionary<string, string> cargs = new Dictionary<string, string>();

        [HideInInspector]
        /// <summary>
        /// 当前的游戏类型
        /// </summary>
        public CurrentGameType CurrentGame;

        [HideInInspector]
        /// <summary>
        /// 选断门状态
        /// </summary>
        public int DuanMenState = 0;

        [HideInInspector]
        /// <summary>
        /// 游戏状态
        /// </summary>
        public TotalState GameTotalStatus;

        [HideInInspector]
        /// <summary>
        /// 是否开启语音标识
        /// </summary>
        public bool IsChatVoiceOn = true;

        /// <summary>
        ///     是否点击过听按钮
        /// </summary>
        [HideInInspector] public bool IsClickTing = false;

        [HideInInspector]
        /// <summary>
        /// 杠参数是否选择
        /// </summary>
        public bool IsGangSelect;

        /// <summary>
        ///     是否为托管状态
        /// </summary>
        [HideInInspector] public bool IsInRobot = false;

        [HideInInspector]
        /// <summary>
        /// 圈是否存在
        /// </summary>
        public bool IsQuanExist;

        [HideInInspector]
        /// <summary>
        /// 牌堆中剩余牌的数量
        /// </summary>
        public int LeftNum;

        [HideInInspector]
        /// <summary>
        /// 当前活动状态用户的座位ID
        /// </summary>
        public int mCurrentPosition;

        [HideInInspector]
        /// <summary>
        /// 玩家人数
        /// </summary>
        public int PlayerNum = 4;

        [HideInInspector]
        /// <summary>
        /// 临时使用的SoundKey,以后会集成到大厅框架中
        /// </summary>
        public string SoundKey;

        /// <summary>
        ///     打出后有听的牌
        /// </summary>
        [HideInInspector] public List<int> TingOutCards = new List<int>();

        [HideInInspector]
        /// <summary>
        /// 该玩法全部牌数
        /// </summary>
        public int TotalNum;

        /// <summary>
        ///     所有玩家信息
        /// </summary>
        public List<ISFSObject> UserDatas;

        /// <summary>
        ///     进入游戏后开始游戏的第一次标记，显示开始动画。
        /// </summary>
        public bool IsFirstTime
        {
            get { return CurrentGame.NowRound == 0; }
        }

        /// <summary>
        ///     牌型
        /// </summary>
        public List<int> TypeList
        {
            get { return _typeList; }
        }


        public void Awake()
        {
            Application.targetFrameRate = 24;
            CurrentGame = new CurrentGameType();
        }

        /// <summary>
        ///     将GameInfo放到本地
        /// </summary>
        /// <param name="data"></param>
        public void InitData(ISFSObject data)
        {
            #region 解析数据

            string pCards;
            int pNum;
            int round;
            int state;
            ISFSArray otherUsers;
            int bank0;
            ISFSObject currentUser;
            int rate;
            int gtype;
            int ante;
            string roomName;
            int quan;
            int cardLenth;
            int id;
            string cargs;
            bool rejoin;
            int showRoomID;
            int totalRound;
            GameTools.TryGetValueWitheKey(data, out pCards, RequestKey.KeyPCards);
            GameTools.TryGetValueWitheKey(data, out pNum, RequestKey.KeyPlayerNum);
            GameTools.TryGetValueWitheKey(data, out round, RequestKey.KeyNowRound);
            GameTools.TryGetValueWitheKey(data, out state, RequestKey.KeyState);
            GameTools.TryGetValueWitheKey(data, out otherUsers, RequestKey.KeyUsers);
            GameTools.TryGetValueWitheKey(data, out bank0, RequestKey.KeyBanker0);
            GameTools.TryGetValueWitheKey(data, out currentUser, RequestKey.KeyUser);
            GameTools.TryGetValueWitheKey(data, out rate, RequestKey.KeyRate);
            GameTools.TryGetValueWitheKey(data, out gtype, RequestKey.KeyGameType);
            GameTools.TryGetValueWitheKey(data, out roomName, RequestKey.KeyRoomName);
            GameTools.TryGetValueWitheKey(data, out cardLenth, RequestKey.KeyCardLenth);
            GameTools.TryGetValueWitheKey(data, out id, RequestKey.KeyId);
            GameTools.TryGetValueWitheKey(data, out cargs, RequestKey.KeyCargs);
            GameTools.TryGetValueWitheKey(data, out rejoin, RequestKey.KeyRejoin);
            GameTools.TryGetValueWitheKey(data, out showRoomID, RequestKey.KeyShowRoomID);
            GameTools.TryGetValueWitheKey(data, out totalRound, RequestKey.KeyTotalRound);
            GameTools.TryGetValueWitheKey(data, out CurrentGame.RuleInfo, RequestKey.KeyRule);
            GameTools.TryGetValueWitheKey(data, out CurrentGame.OwnerId, RequestKey.KeyOwnerId);
            YxDebug.LogError("当前局的起始庄座位号是：" + bank0);

            #endregion

            #region 处理数据

            var index = 0;
            _typeList.Clear();
            //检查牌型
            while (index < pCards.Length)
            {
                _typeList.Add(Convert.ToInt32(pCards.Substring(index, 2), 16));
                index += 2;
            }
            YxDebug.LogError(string.Format("当前游戏中的牌型有{0}种", _typeList.Count));
            if (!(PlayerNum*_typeList.Count).Equals(cardLenth))
            {
                YxDebug.LogError(string.Format("所有玩家总牌数为{0}，玩家的牌数是{1}", _typeList.Count, cardLenth));
            }
            SetGameEnv(cargs);
            if (data.ContainsKey(RequestKey.KeyQuan))
            {
                IsQuanExist = true;
                GameTools.TryGetValueWitheKey(data, out quan, RequestKey.KeyQuan);
                CurrentGame.Quan = quan;
            }
            Bank0 = bank0;
            CurrentGame.RealRoomId = id;
            CurrentGame.GameRoomType = gtype;
            CurrentGame.ShowRoomId = showRoomID;
            CurrentGame.NowRound = round;
            CurrentGame.IsQuanExist = IsQuanExist;
            CurrentGame.Ante = 0;
            CurrentGame.Rate = rate;
            CurrentGame.TotalRound = totalRound;
            TotalNum = cardLenth;
            GameTotalStatus = (TotalState) state;
            LeftNum = TotalNum;
            UserDatas = new List<ISFSObject>();
            UserDatas.Add(currentUser);
            foreach (ISFSObject user in otherUsers)
            {
                UserDatas.Add(user);
            }

            #endregion
        }

        public void SetGameEnv(string clientArgs)
        {
            BootEnv = new Dictionary<string, string>();
            var kvs = clientArgs.Split(',');
            for (var i = 0; i < kvs.Length - 1; i += 2)
            {
                BootEnv[kvs[i]] = kvs[i + 1];
            }
            CheckGameRule();
        }


        public void ResetTotalNumber()
        {
            LeftNum = TotalNum;
        }

        public void CheckGameRule()
        {
            foreach (var env in BootEnv)
            {
                switch (env.Key)
                {
                    case "-jue":
                        IsGangSelect = false;
                        if (int.Parse(env.Value) == 2)
                        {
                            IsGangSelect = true;
                        }
                        break;
                }
            }
        }
    }
}