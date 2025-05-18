using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.Mahjong2D.Game.Data
{
    public class Mahjong2DGameData : YxGameData
    {
        /// <summary>
        /// 程序的启动参数
        /// </summary>
        public Dictionary<string, string> BootEnv;

        private bool _robotState;
        /// <summary>
        /// 是否为托管状态
        /// </summary>
        [HideInInspector]
        public bool IsInRobot
        {
            get
            {
                return _robotState;
            }
            set
            {
                _robotState = value;
                if (App.GetGameManager<Mahjong2DGameManager>())
                {
                    if (App.GetGameManager<Mahjong2DGameManager>().AutoStateByLocal)
                    {
                        Facade.EventCenter.DispatchEvent(ConstantData.KeyRobotToggle, _robotState);
                    }
                }
            }
        }
        [HideInInspector]
        /// <summary>
        /// 当前的游戏类型
        /// </summary>
        public CurrentGameType CurrentGame;
        [HideInInspector]
        /// <summary>
        /// 牌堆中剩余牌的数量
        /// </summary>
        public int LeftNum;
        [HideInInspector]
        /// <summary>
        /// 该玩法全部牌数
        /// </summary>
        public int TotalNum;
        [HideInInspector]
        /// <summary>
        /// 当前活动状态用户的座位ID
        /// </summary>
        public int mCurrentPosition;
        [HideInInspector]
        /// <summary>
        /// 临时使用的SoundKey,以后会集成到大厅框架中
        /// </summary>
        public string SoundKey;
        [HideInInspector]
        /// <summary>
        /// 游戏状态
        /// </summary>
        public TotalState GameTotalStatus;
        [HideInInspector]
        /// <summary>
        /// 玩家人数
        /// </summary>
        public int PlayerNum = 4;
        /// <summary>
        /// 当前的房主ID
        /// </summary>
        [HideInInspector]
        public int OwnerId;
        [HideInInspector]
        /// <summary>
        /// 随机庄的起始位置
        /// </summary>
        public int Bank0;
        [HideInInspector]
        /// <summary>
        /// 庄的实际位置
        /// </summary>
        public int Bank = -1000;
        /// <summary>
        /// 所有玩家信息
        /// </summary>
        public List<ISFSObject> UserDatas;
        [HideInInspector]
        /// <summary>
        /// 杠参数是否选择
        /// </summary>
        public bool IsGangSelect = false;
        [HideInInspector]
        /// <summary>
        /// 选断门状态
        /// </summary>
        public int DuanMenState = 0;
        /// <summary>
        /// 是否点击过听按钮
        /// </summary>
        [HideInInspector]
        public bool IsClickTing = false;
        /// <summary>
        /// 打出后有听的牌
        /// </summary>
        [HideInInspector]
        public List<int> TingOutCards = new List<int>();
        /// <summary>
        /// 是否选择了双宝
        /// </summary>
        [HideInInspector]
        public bool IsShuangBao = false;
        /// <summary>
        /// 明宝标识
        /// </summary>
        [HideInInspector]
        public bool IsMingBao = false;
        /// <summary>
        /// 麻将打出后，从放置位置落下的时间
        /// </summary>
        public float CardDownTime = 0.15f;
        /// <summary>
        /// 进入游戏后开始游戏的第一次标记，显示开始动画。
        /// </summary>
        public bool IsFirstTime
        {
            get
            {
                return CurrentGame.NowRound == 0;
            }
        }

        public bool IsCreateRoom
        {
            get { return CurrentGame.IsCreateRoom; }
        }

        /// <summary>
        /// 确定创建房间格式事件
        /// </summary>
        public List<EventDelegate> SureCreateRoomAction=new List<EventDelegate>();

        /// <summary>
        /// 房间参数对应的名称
        /// </summary>
        private Dictionary<string, string> cargs = new Dictionary<string, string>();
        [HideInInspector]
        /// <summary>
        /// 是否开启语音标识
        /// </summary>
        public bool IsChatVoiceOn = true;
        [HideInInspector]
        /// <summary>
        /// 圈是否存在
        /// </summary>
        public bool IsQuanExist = false;
        /// <summary>
        ///是否点击潇洒按钮
        /// </summary>
        public bool IsXiaoSa = false;
        /// <summary>
        /// 本局游戏中的牌型
        /// </summary>
        List<int> _typeList = new List<int>();
        [HideInInspector]
        public int HupTime = 300;
        /// <summary>
        /// 是否为乱风
        /// </summary>
        private bool _luanfeng;
        /// <summary>
        /// 牌型
        /// </summary>
        public List<int> TypeList
        {
            get { return _typeList; }
        }

        public void Awake()
        {
            CurrentGame = new CurrentGameType();
        }

        /// <summary>
        /// 将GameInfo放到本地
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
            string roomName;
            int quan;
            int cardLenth;
            int id;
            string cargs;
            bool rejoin;
            int showRoomID;
            int totalRound;
            string rule;
            int ownerId;
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
            GameTools.TryGetValueWitheKey(data, out rule, RequestKey.KeyRule);
            GameTools.TryGetValueWitheKey(data, out ownerId, RequestKey.KeyOwnerId);
            #endregion
            #region 处理数据
            PlayerNum = pNum;
            OwnerId = ownerId;
            int index = 0;
            _typeList.Clear();
            //检查牌型
            while (index < pCards.Length)
            {
                _typeList.Add(Convert.ToInt32(pCards.Substring(index, 2), 16));
                index += 2;
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
            CurrentGame.RoomName = roomName;
            CurrentGame.TotalRound = totalRound;
            GetGameInfoShow();
            FengGangHelper.Instance.InitFengDic(_luanfeng);
            if (string.IsNullOrEmpty(rule))
            {
                CurrentGame.Rules = SaveInfo;
            }
            else
            {
                CurrentGame.Rules = rule;
            }
            TotalNum = cardLenth;
            GameTotalStatus = (TotalState)state;
            LeftNum = TotalNum;
            UserDatas = new List<ISFSObject>();
            UserDatas.Add(currentUser);
            foreach (ISFSObject user in otherUsers)
            {
                UserDatas.Add(user);
            }
            if(gameObject.activeInHierarchy)
            {
                StartCoroutine(SureCreateRoomAction.WaitExcuteCalls());
            }
            #endregion
        }

        public void SetGameEnv(string clientArgs)
        {
            BootEnv = new Dictionary<string, string>();
            string[] kvs = clientArgs.Split(',');
            for (int i = 0; i < kvs.Length - 1; i += 2)
            {
                BootEnv[kvs[i]] = kvs[i + 1];
            }
        }



        public void ResetTotalNumber()
        {
            LeftNum = TotalNum;
        }
        [HideInInspector]
        public string SaveInfo;
        /// <summary>
        /// 是否潇洒的字段
        /// </summary>
        public bool XiaoSha = false;
        /// <summary>
        /// 下子的值
        /// </summary>
        public int XiaZhiValue
        {
            set
            {
                _xiaZhiValue = value;
            }
            get
            {
                return _xiaZhiValue;
            }
        }

        private int _xiaZhiValue;

        public string GetGameInfoShow()
        {
            SaveInfo = "";
            string showInfo = "";
            string singleInfo;
            int lineNum = 1;
            foreach (var env in BootEnv)
            {
                int checkValue = 0;
                if (int.TryParse(env.Value,out checkValue))
                {
                    if (checkValue<=0)
                    {
                        continue;
                    }
                }
               
                switch (env.Key)
                {
                    case "-jue":
                        IsGangSelect = false;
                        if (int.Parse(env.Value) == 1)
                        {
                            singleInfo = "绝";
                        }
                        else if (int.Parse(env.Value) == 2)
                        {
                            singleInfo = "杠";
                            IsGangSelect = true;
                        }
                        else
                        {
                            singleInfo = "";
                        }

                        break;
                    case "-lz":
                        singleInfo = "会牌";
                        break;
                    case "-piao":
                        singleInfo = "加刚";
                        break;
                    case "-duanmen":
                        singleInfo = "选断门";
                        break;
                    case "-jihu":
                        singleInfo = "鸡胡";
                        break;
                    case "-qionghu":
                        singleInfo = "穷胡";
                        break;
                    case "-bsj":

                        switch ((EnumGameKeys)Enum.Parse(typeof(EnumGameKeys), App.GameKey))
                        {
                            case EnumGameKeys.pjmj:
                                singleInfo = "点炮全包";
                                break;
                            default:
                                singleInfo = "包三家";
                                break;
                        }
                        break;
                    case "-gangci":
                        singleInfo = "杠开杠呲";
                        break;
                    case "-kaimen":
                        singleInfo = App.GameKey.Equals("ykmj") ? "推倒胡" : "开门胡";
                        break;
                    case "-qingyise":
                        singleInfo = "清一色";
                        break;
                    case "-xuanfeng":
                        singleInfo = "旋风杠";
                        break;
                    case "-qysqd":
                        singleInfo = "清一色,七对;";
                        break;
                    case "-hyszm":
                        singleInfo = "混一色,长毛;";
                        break;
                    case "-lf":
                        singleInfo = "乱风;";
                        int value;
                        int.TryParse(env.Value,out value);
                        _luanfeng = value >= 1;
                        break;
                    case "-ante":
                        float fValue;
                        float.TryParse(env.Value, out fValue);
                        singleInfo =string.Format("分数:{0}分;", fValue / 10);
                        break;
                    case "-qidui":
                        singleInfo = "七小对";
                        break;
                    case "-dianpao":
                        singleInfo = "点炮胡";
                        break;
                    case "-shuangbao":
                        IsShuangBao = true;
                        singleInfo = "双宝";
                        break;
                    case RequestKey.KeyMingBao:
                        IsMingBao = true;
                        singleInfo = "明宝";
                        break;
                    case RequestKey.KeyHupTime:
                        singleInfo = "";
                        HupTime = int.Parse(env.Value);
                        break;
                    case "-xiaosa":
                        singleInfo = "潇洒";
                        XiaoSha = true;
                        break;
                    case "-caigang":
                        singleInfo = "彩杠";
                        break;
                    case "-genzhuang":
                        singleInfo = "跟庄";
                        break;
                    case "-pph":
                        singleInfo = "飘胡";
                        break;
                    case "-gunjiang":
                        singleInfo = "滚将";
                        break;
                    case "-xiazhi":
                        if (int.Parse(env.Value) == 2)
                        {
                            singleInfo = "下两子儿";
                            _xiaZhiValue = 2;
                        }
                        else if (int.Parse(env.Value) == 5)
                        {
                            singleInfo = "下五子儿";
                            _xiaZhiValue = 5;
                        }
                        else
                        {
                            singleInfo = "不下子儿";
                            _xiaZhiValue = 0;
                        }
                        break;
                    default:
                        singleInfo = "";
                        break;
                }
                if (string.IsNullOrEmpty(singleInfo))
                {
                    continue;
                }
                SaveInfo += string.Format("{0} ", singleInfo);
                CurrentGame.SimpleRule = SaveInfo;
                //控制一下文本显示，不想多次操作uilabel，直接在string上处理，虽然会有偏差，但是效率高不少
                singleInfo = string.Format("{0} ", singleInfo);
                if ((showInfo + singleInfo).Length * 26 / lineNum > 460)
                {
                    showInfo += "\n";
                    lineNum++;
                }
                showInfo += singleInfo;
            }
            if (!CurrentGame.IsCreateRoom)
            {
                showInfo = "";
                SaveInfo = "";
            }
            return showInfo;
        }
    }
}
