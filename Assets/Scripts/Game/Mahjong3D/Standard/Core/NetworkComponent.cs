using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class NetworkComponent : BaseComponent
    {
        /// <summary>
        /// 换房间
        /// </summary>
        public Action ChangeRoomEvent;
        /// <summary>
        /// 发送请求
        /// 发送请求的协议是字符串
        /// </summary>
        public Action<string, ISFSObject> SendRequest;
        /// <summary>
        /// 请求事件
        /// </summary>
        private event Action<ISFSObject> mRequestC2SEvent;

        /// <summary>
        /// 注册网络监听事件
        /// </summary>
        public override void OnInitalization()
        {
            var game = GetComponent<MahGameManager>();
            var server = GetComponent<MahServerManager>();

            game.UserOutEvent += OnUserOut;
            game.UserReadyEvent += OnUserReady;
            game.GameStateEvent += OnGameState;
            game.OnGetGameInfoEvent += OnGetGameInfo;
            game.OnOtherPlayerJoinRoomEvent += OnOtherPlayerJoinRoom;
            server.OnHandsUpEvent += OnHandUp;
            server.OnRollDiceEvent += OnRollDice;
            server.OnGameOverEvent += OnGameOver;
            ChangeRoomEvent += server.OnChangeRoom;
            mRequestC2SEvent += server.OnRequestC2S;
            SendRequest += server.OnSendFrameRequest;
        }

        /// <summary>
        /// 开启 重后台返回游戏时 执行重连
        /// </summary>
        public void OpenYuleRejoin()
        {
            var game = GetComponent<MahGameManager>();
            game.NeedRejoinByFocus = true;
        }

        /// <summary>
        /// 重连请求
        /// </summary>
        public void SendReJoinGame()
        {
            GetComponent<MahServerManager>().SendReJoinGame();
        }

        //打开网络接口，开始接收数据
        public void StartNetworkListener()
        {
            var server = GetComponent<MahServerManager>();
            if (server == null) return;

            server.enabled = true;
        }

        /// <summary>
        /// 向服务器发出请求接口
        /// </summary>
        /// <param name="func">请求数据委托</param>
        /// <code language= "csharp"><![CDATA[  
        /// public void Test()
        /// {
        ///     int param1 = 1;
        ///     int param2 = 2;
        ///     GameCenter.Get<MahjongNetworkManager>().OnRequestC2S((sfs) =>
        ///     {
        ///         sfs.PutInt("1", param1); 
        ///         sfs.PutInt("2", param2);
        ///         return sfs;
        ///     });
        /// }
        /// ]]></code>
        public bool OnRequestC2S(Func<SFSObject, ISFSObject> func)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            ISFSObject data = func(sfsObject);
            if (null != data)
            {
                mRequestC2SEvent(data);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 控制重连开关 重后台返回游戏时 执行重连
        /// </summary>
        public void CtrlYuleRejoin(bool flag)
        {
            GetComponent<MahGameManager>().NeedRejoinByFocus = flag;
        }

        /// <summary>
        /// 准备
        /// </summary>     
        private void OnUserReady(int localseat, ISFSObject data)
        {
            var dataCenter = GameCenter.DataCenter;
            var player = dataCenter.Players[localseat];
            if (player != null)
            {
                dataCenter.Players[localseat].IsReady = true;
                GameCenter.EventHandle.Dispatch((int)EventKeys.PlayerReady, new PlayerInfoArgs() { Chair = localseat });                            
            }
            if (localseat == 0)
            {
                GameCenter.EventHandle.Dispatch((int)EventKeys.HideHuFlag);
            }           
        }

        /// <summary>
        /// 玩家退出
        /// </summary>     
        private void OnUserOut(int localseat, ISFSObject data)
        {
            GameCenter.EventHandle.Dispatch((int)EventKeys.PlayerOut, new PlayerInfoArgs() { Chair = localseat });
        }

        /// <summary>
        /// 玩家进入
        /// </summary>     
        private void OnOtherPlayerJoinRoom(ISFSObject data)
        {
            var user = data.GetSFSObject(RequestKey.KeyUser);
            GameCenter.EventHandle.Dispatch((int)EventKeys.PlayerJoin, new PlayerInfoArgs() { Chair = MahjongUtility.GetChair(user) });
        }

        private void OnGetGameInfo(ISFSObject gameInfo)
        {
            GameCenter.DataCenter.GetAllDatas(gameInfo);

            //当遇到胡牌 小结算 大结算时重连，只更新数据，忽略重连逻辑
            if (GameCenter.Instance.IgonreReconnect) return;
            GameCenter.GameProcess.ChangeState<StateGameInfoInit>(new SfsFsmStateArgs() { SFSObject = gameInfo });
        }

        private void OnGameOver(ISFSObject data)
        {
            var db = GameCenter.DataCenter;
            db.IsGameOver = true;
            db.Game.SetTotalResult(data);
            GameCenter.Instance.SetIgonreReconnectState(true);
            GameCenter.GameLogic.GameResponse(CustomProl.GameOverLogic, data);
        }

        private void OnRollDice(ISFSObject data)
        {
            GameCenter.GameLogic.GameResponse(CustomProl.ResRollDice, data);
        }

        private void OnHandUp(ISFSObject data)
        {
            if (data.ContainsKey("cmd") && data.GetUtfString("cmd") == "dismiss")
            {
                int type = data.GetInt("type");
                string username = data.GetUtfString("username");
                MahjongPlayersData playersData = GameCenter.DataCenter.Players;
                for (int i = 0; i < GameCenter.DataCenter.MaxPlayerCount; i++)
                {
                    if (playersData[i].NickM == username)
                    {
                        GameCenter.EventHandle.Dispatch((int)EventKeys.OnEventHandUp, new HandupEventArgs()
                        {
                            UserName = username,
                            HandupType = (DismissFeedBack)type,
                            Chair = i
                        });
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 游戏状态
        /// </summary>     
        private void OnGameState(int state, ISFSObject info) { }
    }
}