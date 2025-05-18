using System;
using System.Collections.Generic;
using Assets.Scripts.Game.pdk.PokerRule;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Sfs2X.Entities.Data;
using YxFramwork.Common.Utils;
using YxFramwork.ConstDefine;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Common;

namespace Assets.Scripts.Game.pdk.InheritCommon
{
    public class GlobalData : YxGameData
    {

        //-------------静态相关-------------------------start--

        /// <summary>
        /// 获得所有参与游戏玩家的Id
        /// </summary>
        /// <returns></returns>
        public int[] GetUsersId()
        {
            var len = UserDic.Count;
            var userIds = new int[len];
            for (int i = 0; i < len; i++)
            {
                userIds[i] = UserDic[i].GetInt(RequestKey.KeyId);
            }

            return userIds;
        }

        /// <summary>
        /// 获得所有参与游戏玩家的name
        /// </summary>
        /// <returns></returns>
        public string[] GetAllUsersNames()
        {
            var len = UserDic.Count;
            var userIds = new string[len];
            for (int i = 0; i < len; i++)
            {
                userIds[i] = UserDic[i].GetUtfString(RequestKey.KeyName);
            }

            return userIds;
        }

        /// <summary>
        /// 服务器的ServInstance
        /// </summary>
        public static Ddz2RemoteServer ServInstance { private set; get; }

        /// <summary>
        /// 服务器发来的gameinfodata缓存
        /// </summary>
        private static ISFSObject _onGameInfoData;

        /// <summary>
        /// 存储加入游戏的玩家信息
        /// </summary>
        private static readonly Dictionary<int,ISFSObject> UserDic=new Dictionary<int, ISFSObject>();

        /// <summary>
        /// 场景销毁后，重置静态变量
        /// </summary>
        void OnDestroy()
        {
            ServInstance = null;
            _onGameInfoData = null;
            UserDic.Clear();
        }

        public static void SetServInstance(Ddz2RemoteServer obj)
        {
            ServInstance = obj;
        }
        /// <summary>
        /// 响应服务器OnGetGameInfo事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnGetGameInfo(object sender, DdzbaseEventArgs args)
        {
            _onGameInfoData = args.IsfObjData;
            var userSelf = _onGameInfoData.GetSFSObject(RequestKey.KeyUser);
            UserDic[userSelf.GetInt(RequestKey.KeySeat)] = userSelf;


            if(!_onGameInfoData.ContainsKey(RequestKey.KeyUserList)) return;
            ISFSArray users = _onGameInfoData.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject user in users)
            {
                UserDic[user.GetInt(RequestKey.KeySeat)] = user;
            }
            //Debug.LogError(UserDic.Count);

        }
        /// <summary>
        ///  响应服务器OnGetRejoinData事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnGetRejoinData(object sender, DdzbaseEventArgs args)
        {
            _onGameInfoData = args.IsfObjData;

            var selfUser =  _onGameInfoData.GetSFSObject(RequestKey.KeyUser);
            UserDic[selfUser.GetInt(RequestKey.KeySeat)] = selfUser;

            ISFSArray users = _onGameInfoData.GetSFSArray(RequestKey.KeyUserList);
            foreach (ISFSObject user in users)
            {
                UserDic[user.GetInt(RequestKey.KeySeat)] = user;
            }
            //Debug.LogError(UserDic.Count);
        }

        /// <summary>
        /// 玩家加入房间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void OnPlayerJoinRoom(object sender, DdzbaseEventArgs args)
        {
            //Debug.LogError("OnPlayerJoinRoom: " +  args.IsfObjData.GetDump());

            var user = args.IsfObjData.GetSFSObject(RequestKey.KeyUser);
            UserDic[user.GetInt(RequestKey.KeySeat)] = user;
        }


        protected override void OnGc()
        {
            base.OnGc();

            ClearParticalGob();
        }

        //---------------------------------------------------------------------------------------end
      
        void Awake()
        {
            PdkGameManager.AddOnGameInfoEvt(CheckGameStatus);
            PdkGameManager.AddOnGetRejoinDataEvt(CheckGameStatus);
            //Ddz2RemoteServer.AddOnServResponseEvtDic(GlobalConstKey.TypeGrabSpeaker, OnTypeGrabSpeaker);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeAllocate, OnAlloCateCds);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGameOver, OnTypeGameOver);
        }

        private void OnAlloCateCds(object sender, DdzbaseEventArgs e)
        {
            IsStartGame = true;
        }
        private void OnTypeGameOver(object sender, DdzbaseEventArgs e)
        {
            IsStartGame = false;
            // //更新局数信息缓存
            //_onGameInfoData.PutInt(NewRequestKey.KeyCurRound, _onGameInfoData.GetInt(NewRequestKey.KeyCurRound) + 1);
        }
        /// <summary>
        /// 当有手牌数更新时,第一个int 是 userSeat 第二个 int 是 leftHandCdNum 
        /// </summary>
        private Action<int, int> _onHdCdsChange;
        /// <summary>
        /// 手牌数更新事件
        /// </summary>
        public Action<int, int> OnhandCdsNumChanged
        {
            set { _onHdCdsChange += value; }
        }
        /// <summary>
        /// 调用手牌数量改变全局事件
        /// </summary>
        /// <param name="userSeat"></param>
        /// <param name="leftHandCdNum"></param>
        public void OnSomePlayerHdcdsChange(int userSeat, int leftHandCdNum)
        {
            if (_onHdCdsChange == null) return;

            _onHdCdsChange(userSeat, leftHandCdNum);
        }

        /// <summary>
        /// 当玩家分数改变时 第一个int 是 userSeat 第二个 int 是 scoreGold 
        /// </summary>
        private Action<int, int> _onUserScoreChanged;
        public Action<int, int> OnUserScoreChanged
        {
            set { _onUserScoreChanged += value; }
        }
        /// <summary>
        /// 调用玩家数据改变全局事件
        /// </summary>
        /// <param name="userSeat"></param>
        /// <param name="scoreGold"></param>
        public void OnUserSocreChanged(int userSeat, int scoreGold)
        {
            if(_onUserScoreChanged==null) return;

            _onUserScoreChanged(userSeat, scoreGold);
        }

        //清理例子特效
        private Action _onClearParticalGob;
        public Action OnClearParticalGob
        {
            set { _onClearParticalGob += value; }
        }
        /// <summary>
        /// 清理例子特效，防止返回大厅崩溃
        /// </summary>
        public void ClearParticalGob()
        {
            if (_onClearParticalGob != null) _onClearParticalGob();
        }


        //加载声音clip
        [SerializeField]
        protected  List<AudioClip> AudioClips = new List<AudioClip>();
        private readonly Dictionary<string,AudioClip> _audioClipDic = new Dictionary<string, AudioClip>();
        void Start()
        {
            foreach (var clip in AudioClips)
            {
                _audioClipDic[clip.name] = clip;
            }

            IsPlayVoiceChat = PlayerPrefs.GetInt(SettingCtrl.VoiceChatKey, 1) == 1;
            MusicAudioSource.volume = Facade.Instance<MusicManager>().MusicVolume;//PlayerPrefs.GetFloat(SettingCtrl.MusicValueKey, 0);
            MusicAudioSource.mute = true;
        }

        public AudioClip GetSoundClip(string clipName)
        {
            return _audioClipDic.ContainsKey(clipName) ? _audioClipDic[clipName] : null;
        }

        /// <summary>
        /// 背景音乐
        /// </summary>
        public AudioSource MusicAudioSource;



        private bool _isPlayVoiceChat;
        /// <summary>
        /// 是否播放语音
        /// </summary>
        public bool IsPlayVoiceChat
        {
            get { return _isPlayVoiceChat; }
            set
            {
                _isPlayVoiceChat = value;
                if (_onPlayVoichatStateChange != null) _onPlayVoichatStateChange(value);
            }
        }

        private Action<bool> _onPlayVoichatStateChange;
        /// <summary>
        /// 当声音开关改变时
        /// </summary>
        public Action<bool> OnPlayerVoiceChatSteChange
        {
            set { _onPlayVoichatStateChange += value;}
        }

        

        /// <summary>
        /// 根据座位号获取玩家信息
        /// </summary>
        /// <param name="userSeat"></param>
        /// <returns></returns>
        public ISFSObject GetUserInfo(int userSeat)
        {
            if (!UserDic.ContainsKey(userSeat)) return null;
            return UserDic[userSeat];
        }

        /// <summary>
        /// 获得所有参与游戏玩家的名字
        /// </summary>
        /// <returns></returns>
        public string[] GetUsersName()
        {
            var len = UserDic.Count;
            var userNames = new string[len];
            for (int i = 0; i < len; i++)
            {
                userNames[i] = UserDic[i].GetUtfString(RequestKey.KeyName);
            }

            return userNames;
        }

        /// <summary>
        /// 是否是开房游戏,默认是开房 true开房 fasle娱乐
        /// </summary>
        public bool IsRoomGame
        {
            get
            {
                var infoData = GetGameInfoData();
                if (infoData != null && infoData.ContainsKey(NewRequestKey.KeyGtype)) return infoData.GetInt(NewRequestKey.KeyGtype) == -1;
                return true;
            }
        }

        /// <summary>
        /// 玩家最大人数
        /// </summary>
        public int PlayerMaxNum
        {

            get
            {
                var infoData = GetGameInfoData();
                if (infoData != null && infoData.ContainsKey(NewRequestKey.KeyPlayerNum))
                    return infoData.GetInt(NewRequestKey.KeyPlayerNum);

                return 3;//默认3个
            }
        }

        /// <summary>
        /// 当前客户端玩家信息
        /// </summary>
        public  ISFSObject UserSelfData
        {

            get
            {
                var infoData = GetGameInfoData();
                if (infoData != null && infoData.ContainsKey(RequestKey.KeyUser))
                    return infoData.GetSFSObject(RequestKey.KeyUser);

                return null;
            }
        }

        /// <summary>
        /// 玩家自己的座位
        /// </summary>
        public int GetSelfSeat
        {
            get
            {
                //return UserSelfData.GetInt(RequestKey.KeySeat);
                return SelfSeat;
            }
        }

        /// <summary>
        /// 玩家右边的座位
        /// </summary>
        public int GetRightPlayerSeat
        {
            get { return (GetSelfSeat + 1)%PlayerMaxNum; }
        }

        /// <summary>
        /// 玩家左边的位置
        /// </summary>
        public int GetLeftPlayerSeat
        {
            get
            {
                if (PlayerMaxNum != 3) return -1;

                return (GetSelfSeat + 2) % PlayerMaxNum;
            }
        }


        /// <summary>
        /// 加倍类型 0不加倍 1正常加倍 2农民加倍
        /// </summary>
        public int JiaBeiType
        {
            get 
            { 
                var data = GetGameInfoData();
                if (data == null) throw new ArgumentNullException(paramName: "没有游戏Gameinfo" + "的信息");
                return data.GetInt(NewRequestKey.KeyJiaBei);
            }
        }

        /// <summary>
        /// 标记是否黑三先出
        /// </summary>
        public bool IsHeiSanFirst
        {
            get
            {
                var data = GetGameInfoData();
                if (data == null) return false;

                if (data.ContainsKey(NewRequestKey.KeyHeisanFirst)&& data.GetBool(NewRequestKey.KeyHeisanFirst))
                {
                    if (data.ContainsKey(NewRequestKey.KeyCurRound) && data.GetInt(NewRequestKey.KeyCurRound) < 2)
                        return data.GetBool(NewRequestKey.KeyHeisanFirst);
                }
                return false;
            }
        }

        /// <summary>
        /// 标记整场比赛已经开始
        /// </summary>
        public bool IsStartGame { private set; get; }
        /// <summary>
        /// 开始叫分时,标记整场比赛已经开始
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected void OnTypeGrabSpeaker(object sender, DdzbaseEventArgs args)
        {
            IsStartGame = true;
        }

        /// <summary>
        /// 检查游戏状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CheckGameStatus(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            if (data.ContainsKey(NewRequestKey.KeyClientArgs2))
            {
                var cargsData = data.GetSFSObject(NewRequestKey.KeyClientArgs2);

                _roomPlayInfo = cargsData.GetUtfString(NewRequestKey.KeyModel).Equals("2") ? "跑得快15张，" : "跑得快16张，";
                _roomPlayInfo += " 人数:" + cargsData.GetUtfString(NewRequestKey.KeyPlen)+"人";
                if (cargsData.GetUtfString(NewRequestKey.KeyHeisan) == "1")
                {
                    _roomPlayInfo += "，黑三先出";
                }
            }

            if (data.ContainsKey(NewRequestKey.KeyOwerId))
            {
                _owerId = data.GetInt(NewRequestKey.KeyOwerId);
            }

            if (data.ContainsKey(NewRequestKey.KeyState)
                 && data.GetInt(NewRequestKey.KeyState) == GlobalConstKey.StatusIdle)
            {
                if (data.ContainsKey(NewRequestKey.KeyCurRound) && data.GetInt(NewRequestKey.KeyCurRound) <= 1)
                {
                    IsStartGame = false;
                    return;
                }
            }

            IsStartGame = true;
        }

        /// <summary>
        /// 房间参数
        /// </summary>
        private string _roomPlayInfo;
        /// <summary>
        /// 玩法信息
        /// </summary>
        public string RoomPlayInfo {
            get { return _roomPlayInfo; }
        }


        private int _owerId =-1;
        /// <summary>
        /// 获得房主id信息
        /// </summary>
        public int GetOwerId
        {
            get { return _owerId; }
        }

        /// <summary>
        /// 判断自己是不是房主
        /// </summary>
        public bool IsSelfIsOwer
        {
            get { return UserDic.ContainsKey(GetSelfSeat) && UserDic[GetSelfSeat].GetInt(RequestKey.KeyId) == _owerId; }
        }
        //-----------------------------------------------------
        /// <summary>
        /// 获得游戏信息体
        /// </summary>
        /// <returns></returns>
        private  ISFSObject GetGameInfoData()
        {
            if (_onGameInfoData != null) return _onGameInfoData;
            return null;
        }


        /// <summary>
        /// 根据id获得某个玩家userData
        /// </summary>
        /// <returns></returns>
        public ISFSObject GetPlayerUserData(int userId)
        {
            var len = UserDic.Count;

            for (int i = 0; i < len; i++)
            {
                if (UserDic[i].GetInt(RequestKey.KeyId) == userId)
                {
                    return UserDic[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 根据name获得某个玩家userData
        /// </summary>
        /// <returns></returns>
        public ISFSObject GetPlayerUserData(string userName)
        {
            var len = UserDic.Count;

            for (int i = 0; i < len; i++)
            {
                if (UserDic[i].GetUtfString(RequestKey.KeyName) == userName)
                {
                    return UserDic[i];
                }
            }

            return null;
        }
        public bool AllReady()
        {
            GlobalData _globalData = App.GetGameData<GlobalData>();
            var playerList = _globalData.PlayerList;
            //所有玩家都准备
            foreach (var player in playerList)
            {
                if (player.Info == null)
                {
                    return false;
                }
                if (player.ReadyState == false)
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 一些服务器返回的值的判断
    /// </summary>
    public static class GlobalConstKey
    {

        
        //游戏的status
        /// <summary>
        /// 游戏还没开始
        /// </summary>
        public const int StatusIdle = 0;

        /// <summary>
        /// 游戏还没开始
        /// </summary>
        public const int GameNotStart = 1;

        /// <summary>
        /// 游戏已经开始
        /// </summary>
        public const int StatusHasStart = 2;

        /// <summary>
        /// 选择庄家阶段
        /// </summary>
        public const int StatusChoseBanker = 3;

        /// <summary>
        /// 加倍阶段
        /// </summary>
        public const int StatusDouble = 4;




        /// <summary>
        /// 游戏类型
        /// </summary>
        public enum GameType
        {
            /// <summary>
            /// 叫分
            /// </summary>
            CallScore = 0,
            /// <summary>
            /// 踢地主
            /// </summary>
            Kick,
            /// <summary>
            /// 抢地主
            /// </summary>
            Grab,
            /// <summary>
            /// 叫分带流局
            /// </summary>
            CallScoreWithFlow,
        }

/*        /// <summary>
        /// 叫分带流局
        /// </summary>
        public const int CallScoreWithFlow = 3;*/



        //OnServerResponse的各种类型-------------------start
        /// <summary>
        /// 发牌
        /// </summary>
        public const int TypeAllocate = 0;

        /// <summary>
        /// 出牌
        /// </summary>
        public const int TypeOutCard = 1;

        /// <summary>
        /// 不出
        /// </summary>
        public const int TypePass = 2;

        /// <summary>
        /// 游戏结束 显示结算
        /// </summary>
        public const int TypeGameOver = 3;


        public const int TypeBombScore = 4;


        /// <summary>
        /// 抢地主
        /// </summary>
        public const int TypeGrab = 102;

        /// <summary>
        /// 抢到地主,先出牌
        /// </summary>
        public const int TypeFirstOut = 105;

        /// <summary>
        /// 指定抢地主或者叫分
        /// </summary>
        public const int TypeGrabSpeaker = 107;

        /// <summary>
        ///流局黄庄
        /// </summary>
        public const int TypeFlow = 108;

        /// <summary>
        /// 加倍
        /// </summary>
        public const int TypeDouble = 109;

        /// <summary>
        /// 加倍结束
        /// </summary>
        public const int TypeDoubleOver = 110;



        /// <summary>
        /// 向服务器发送的请求key名
        /// </summary>
        public const string C_Type = "type";
        public const string C_Cards = "cards";
        public const string C_Sit = "seat";
        public const string C_OPCard = "opCard";
        public const string C_Score = "score";
        public const string C_Magic = "laizi";
        public const string C_ante = "cante";
        public const string C_needRejoin = "needRejoin";
        public const string C_Bkp = "bkp";     //Bank Position 庄家位置

        public const string Cmd = "cmd";
        public const string Hup = "hup";
    }
}
