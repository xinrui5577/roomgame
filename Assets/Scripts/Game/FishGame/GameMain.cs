#define ENABLE_MCU_VERIFY
//临时禁止mcu验证
using System;
using System.Collections;
using Assets.Scripts.Game.FishGame.Backgrounds;
using Assets.Scripts.Game.FishGame.ChessCommon;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using Assets.Scripts.Game.FishGame.Common.Servers;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Effect;
using Assets.Scripts.Game.FishGame.FishGenereate;
using Assets.Scripts.Game.FishGame.Fishs;
using Assets.Scripts.Game.FishGame.GunLocker;
using Assets.Scripts.Game.FishGame.script;
using Assets.Scripts.Game.FishGame.ScenePreludes;
using Assets.Scripts.Game.FishGame.UI;
using Assets.Scripts.Game.FishGame.Users;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Utils;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Game.FishGame
{
    public class GameMain : YxGameManager
    {
        #region 事件
        ///事件
        /// <summary>
        /// </summary>
        /// <param name="oldType"></param>
        /// <param name="newType"></param>
        public delegate void EventBackGroundChangeArenaType(ArenaType oldType, ArenaType newType);
        /// <summary>
        /// </summary>
        public delegate void EventBackGroundClearAllDataBefore();
        /// <summary>
        /// </summary>
        /// <param name="b"></param>
        public delegate void EventBulletDestroy(Bullet b);
        /// <summary>
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="scoreGetted"></param>
        public delegate void EventFishBombKilled(Player killer, int scoreGetted);

        /// <summary>
        /// 清鱼
        /// </summary>
        /// <param name="f"></param>
        public delegate void EventFishClear(Fish f);

        /// <summary>
        /// </summary>
        /// <param name="killer">射杀者</param>
        /// <param name="bulletScore">子弹分数</param>
        /// <param name="fishOddBonus">小倍数</param>
        /// <param name="bulletOddsMulti">离子炮倍数</param>
        /// <param name="fish">鱼</param>
        /// <param name="reward">获得金币</param> 
        public delegate void EventFishKilled(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, Fish fish, int reward);

        public delegate void EventInputInsertCoin(int playerControll, int n);

        /// <summary>
        /// </summary>
        /// <param name="killer"></param>
        public delegate void EventKillLockingFish(Player killer);

        /// <summary>
        /// </summary>
        /// <param name="s"></param>
        public delegate void EventLeaderInstance(Swimmer s);

        /// <summary>
        ///     玩家从鱼类获得分数
        /// </summary>
        /// <param name="p"></param>
        /// <param name="score"></param>
        /// <param name="firstFish"></param>
        /// <param name="bulletScore"></param>
        public delegate void EventPlayerGainScoreFromFish(Player p, int score, Fish firstFish, int bulletScore);

        /// <summary>
        /// </summary>
        /// <param name="p"></param>
        /// <param name="newGun"></param>
        public delegate void EventPlayerGunChanged(Player p, Gun newGun);

        /// <summary>
        /// </summary>
        /// <param name="p"></param>
        /// <param name="gun"></param>
        /// <param name="useScore"></param>
        /// <param name="isLock"></param>
        /// <param name="bulletId"></param>
        public delegate void EventPlayerGunFired(Player p, Gun gun, int useScore, bool isLock = false, int bulletId = -1);

        /// <summary>
        /// </summary>
        /// <param name="p"></param>
        /// <param name="scoreNew"></param>
        /// <param name="scoreChange"></param>
        public delegate void EventPlayerScoreChanged(Player p, int scoreNew, int scoreChange);

        /// <summary>
        /// </summary>
        /// <param name="p"></param>
        /// <param name="scoreNew"></param>
        public delegate void EventPlayerWonScoreChanged(Player p, int scoreNew);

        /// <summary>
        /// </summary>
        /// <param name="curentVol"></param>
        public delegate void EventSoundVolumeChanged(float curentVol);
        #endregion
        /// <summary>
        /// 状态
        /// </summary>
        public enum State
        {
            /// <summary>
            /// 待机状态,游戏未开始
            /// </summary>
            Idle,
            /// <summary>
            /// 普通出鱼
            /// </summary>
            Normal,
            /// <summary>
            /// 过场,(一定没子弹)
            /// </summary>
            Sweeping,
            /// <summary>
            /// 开场阵列
            /// </summary>
            Preluding,
            /// <summary>
            /// 后台
            /// </summary>
            BackStage,
            /// <summary>
            /// 扫场前等待子弹消失
            /// </summary>
            BeforeSweepingWaitBulletClear
        }
        #region 变量
        //变量
        public static int MainVersion = 211;
        public static int SubVersion = 200;
        public static float WorldWidth = 3.555555555555556F;
        public static float WorldHeight = 2F;

        public static bool IsEditorShutdown = false; //程序是否已经关闭(用于在Editor模式中,如果是程序已经关闭,则不创建对象)
        public static State State_; //GameMain状态

        //私有变量
        private static GameMain mSingleton;
        public static bool IsMainProcessPause = false; //主进程暂停

        public Transform GameUITs;
        public SystemUI GameSystemUI;
        public Ef_FishDieEffectAdditive Ef_FishDieEffectAdd; //鱼死亡特效
        public FishGenerator FishGenerator; //鱼群生成
        public float GameSpeed = 1F; //游戏速度   

        public Module_GunLocker Gunlocker;

        [NonSerialized]
        public int NumFishAlive = 0; //活鱼数目
        public FishOperationManager Operation;
        public bool OneKillDie = false; // 
        public PopupDigitFishDie PopupDigitFishdie; //鱼死亡数字 

        /// <summary>
        /// 玩家炮台组
        /// </summary>
        public PlayerBatterys PlayersBatterys;
        /// <summary>
        /// 背景管理
        /// </summary>
        public SceneBGManager SceneBGMgr;
        /// <summary>
        /// 鱼阵管理 
        /// </summary>
        public ScenePreludeManager ScenePreludeMgr;
        /// <summary>
        /// 使用到得屏幕数【暂时未实现多屏功能】
        /// </summary>
        public int ScreenNumUsing = 1;
        /// <summary>
        ///     普通场景持续时间
        /// </summary>
        public float TimeNormalScene = 240F;

        public Transform TopLeftTs;
        /// <summary>
        /// 池塘
        /// </summary>
        public Transform PondTs;
        /// <summary>
        ///     2D世界范围
        /// </summary>
        [NonSerialized]
        public Rect WorldDimension;
        /// <summary>
        /// 游戏服务
        /// </summary>
        [SerializeField, Tooltip("游戏交互服务类")]
        public FishGameServer GameServer;
        private BackStageSetting mBSSetting;

        public BackStageSetting BSSetting
        {
            get
            {
                if (mBSSetting == null)
                {
                    mBSSetting = GetComponent<BackStageSetting>();//没获取到
                    mBSSetting.TryNewPersistentDatas();
                }
                return mBSSetting;
            }
        }

        public Player CurPlayer { get; private set; }
        #endregion

        //属性
        public static GameMain Singleton
        {
            get { return mSingleton; }
        }

        protected void Awake()
        {
            YxDebug.Log("========= GameMain Awake 唤醒 ===============");
            Clear();
            mSingleton = this; //初始化数据   

            State_ = State.Idle; //.Idle;  
            //初始化 2D世界范围
            WorldDimension.x = Defines.WorldDimensionUnit.x;
            //多屏 -Defines.WorldDimensionUnit.width * 0.5F * (ScreenNumUsing - 1);
            WorldDimension.y = Defines.WorldDimensionUnit.y;
            WorldDimension.width = Defines.WorldDimensionUnit.width; // *ScreenNumUsing;
            WorldDimension.height = Defines.WorldDimensionUnit.height;
            //设置是否锁帧
            Time.timeScale = GameSpeed;
        }

        protected void Start()
        {
            InitBackGround();
            InitOperation();
            InitSystemUI();
            InitPlayer();
        }

        private void InitSystemUI()
        {
            var asset = ResourceManager.LoadAsset("GameSystemUI");
            var go = Instantiate(asset);
            if (go == null) return;
            GameSystemUI = go.GetComponent<SystemUI>();
            var centerView = GameSystemUI.CenterView;
            centerView.UpdateBoundEvent += UpdateBound;
            PondTs.parent = centerView.transform;
            PondTs.localPosition = new Vector3(0, 0, 40);
        }

        /// <summary>
        /// 初始化背景
        /// </summary>
        private void InitBackGround()
        {
            var asset = ResourceManager.LoadAsset("SBgConfig");
            if (asset == null) return;
            var cfg = asset.GetComponent<SceneBgConfig>();
            if (cfg == null)
            {
                YxDebug.LogError("sbgConfig资源不存在！！！");
                return;
            }
            SceneBGMgr.SbgConfig = cfg;
            SceneBGMgr.InitBackground();
        }

        /// <summary>
        /// 控制相关
        /// </summary> 
        private void InitOperation()
        {
            var asset = ResourceManager.LoadAsset("OperationUI");
            var go = Instantiate(asset);
            Operation = go.GetComponent<FishOperationManager>();
            GameObjectUtile.ResetTransformInfo(go.transform, GameUITs);
        }

        /// <summary>
        /// 初始化玩家
        /// </summary> 
        public void InitPlayer()
        {
            var asset = ResourceManager.LoadAsset("PlayerBatterys6");
            var go = Instantiate(asset);
            go.transform.parent = PondTs;
            go.transform.localPosition = Vector3.zero;
            PlayersBatterys = go.GetComponent<PlayerBatterys>();
            Defines.NumPlayer = PlayersBatterys.Count;
        }

        public void RotatePond(float angle)
        {
            if (PondTs == null) return;
            PondTs.eulerAngles = new Vector3(0, 0, angle);
        }

        public void UpdateBound(Rect worldRect)
        {
            WorldDimension.Set(worldRect.x, worldRect.y, worldRect.width, worldRect.height);
            if (PlayersBatterys == null) return;
            PlayersBatterys.UpdatePosition(worldRect);
        }

        /// <summary>
        ///     用于其他脚本可以在Start订阅StartGame事件,并响应
        /// </summary>
        private IEnumerator _Coro_DelayInitGame()
        {
            yield return 1;
            if (Moudle_main.EvtGameStart != null)
                Moudle_main.EvtGameStart();
            Operation.ChangePlayerScore(0);
        }

        /// <summary>
        /// 上分
        /// </summary>
        public void BuyCoin()
        {
            if (GameServer == null) return;
            var gdata = App.GetGameData<FishGameData>();
            if (!gdata.CanBuyCoin) return;
            gdata.CanBuyCoin = false;
            var selfSeat = App.GameData.SelfSeat % 6;
            var baseScore = BSSetting.Dat_PlayersGunScore[selfSeat].Val;
            GameServer.SendBuyCoin(baseScore * gdata.BaseUpperScore);
            Facade.Instance<MusicManager>().Play("onscore");
        }

        /// <summary>
        /// 下分
        /// </summary>
        public void Retrieve()
        {
            if (GameServer == null) return;
            var gdata = App.GetGameData<FishGameData>();
            if (!gdata.CanRetrieveCoin) return;
            gdata.CanRetrieveCoin = false;
            var selfSeat = App.GameData.SelfSeat % 6;
            var curCoin = BSSetting.Dat_PlayersScore[selfSeat].Val;
            GameServer.SendSellCoind(curCoin);
        }

        public void RobotOut(int nid)
        {
            if (GameServer == null) return;
            GameServer.SendRobotQuit(nid);
        }

        /// <summary>
        /// 游戏流程
        /// </summary>
        /// <returns></returns>
        private IEnumerator _Coro_MainProcess()
        {
            var gdata = App.GetGameData<FishGameData>();
            var evtMainProcessFirstEnterScene = gdata.EvtMainProcessFirstEnterScene;
            if (evtMainProcessFirstEnterScene != null)
                evtMainProcessFirstEnterScene();
            var evtMainProcessPrepareChangeScene = gdata.EvtMainProcessPrepareChangeScene;
            var evtMainProcessFinishChangeScene = gdata.EvtMainProcessFinishChangeScene;
            var evtMainProcessFinishPrelude = gdata.EvtMainProcessFinishPrelude;
            var musicMgr = Facade.Instance<MusicManager>();
            while (true)
            {
                //普通鱼模式 --------------------------------------------------------------
                State_ = State.Normal;
                gdata.ResetDepth();
                musicMgr.ChangeNextBackSound();
                FishGenerator.StartFishGenerate();
                var tmpTime = Time.time;
                while (IsMainProcessPause || Time.time - tmpTime < TimeNormalScene)
                {
                    // YxDebug.Log("==========倒计时：" + (Time.time - tmpTime) +"("+ TimeNormalScene+")");
                    yield return 1;
                }

                //准备过场 --------------------------------------------------------------  
                State_ = State.BeforeSweepingWaitBulletClear;
                gdata.ResetDepth();
                if (evtMainProcessPrepareChangeScene != null)
                    evtMainProcessPrepareChangeScene();

                //停止玩家攻击 --------------------------------------------------------------
                foreach (var p in PlayersBatterys)
                {
                    if (p.IsHide()) continue;
                    p.GunInst.Fireable = false;
                    p.CanChangeScore = false;
                    Operation.Reset();
                }

                //等待场景所有子弹消失 --------------------------------------------------------------

                while (true)
                {
                    float numPlayerHaventBullet = 0;
                    foreach (var p in PlayersBatterys)
                    {
                        numPlayerHaventBullet += (p != null && p.GunInst) ? p.GunInst.NumBulletInWorld : 0;
                    }
                    if (numPlayerHaventBullet < 1) break;
                    yield return 1;
                }
                State_ = State.Sweeping;

                //停止出鱼 -------------------------------------------------------------- 
                FishGenerator.StopFishGenerate();
                //                SoundMgr.StopBgm();
                //切换音乐
                musicMgr.ChangeRandomBackSound();

                //开始播放过场,清鱼 --------------------------------------------------------------
                SceneBGMgr.Sweep();
                while (SceneBGMgr.IsSweep) yield return 1;

                State_ = State.Preluding;

                musicMgr.ChangeNextBackSound();

                if (evtMainProcessFinishChangeScene != null)
                    evtMainProcessFinishChangeScene();

                //恢复玩家攻击 --------------------------------------------------------------
                foreach (var p in PlayersBatterys)
                {
                    var gun = p.GunInst;
                    if (gun == null) continue;
                    gun.Fireable = true;
                    p.CanChangeScore = true;
                }

                //开场鱼阵 -------------------------------------------------------------- 
                var pl = ScenePreludeMgr.DoPrelude();
                if (pl != null)
                {
                    //string preludeName = pl.gameObject.name;
                    //YxDebug.Log("sceneprelude start = " + preludeName+ "   fish num = "+NumFishAlive);
                    bool waitPrelude = true;
                    pl.Evt_PreludeEnd += () => { waitPrelude = false; };
                    while (waitPrelude)
                    {
                        yield return new WaitForSeconds(0.1F);
                    }
                    //YxDebug.Log("sceneprelude end = " + preludeName + "   fish num = " + NumFishAlive);
                }

                //下一次正常出鱼 --------------------------------------------------------------
                if (evtMainProcessFinishPrelude != null)
                    evtMainProcessFinishPrelude();
            }
        }

#if PRELUDE
    private bool _isPlayer;
    private int index;
    public void OnGUI()
    { 
        index = int.Parse(GUILayout.TextField(index.ToString()));
        if (GUILayout.Button("鱼阵"))
        {
            _isPlayer = true;
        }

        if (_isPlayer)
        {
            _isPlayer = false;
            ScenePreludeMgr.IsRandomPrelude = false;
            var pl = ScenePreludeMgr.DoPrelude();
            pl.Evt_PreludeEnd += () =>
                {
                    index++;
                    _isPlayer = true;
                };
        }
    }
#endif

        private void Update()
        {
            if (State_ < State.Normal) return;
            CheckNeedMoreCoin();
        }


        /// <summary>
        /// 检查是否有足够的金币
        /// </summary>
        private void CheckNeedMoreCoin()
        {
            if (PlayersBatterys == null) return;
            if (PlayersBatterys.MinGunStyle < 1) return;
            if (CurPlayer == null || CurPlayer.GunInst.NumBulletInWorld > 0) return;
            var selfSeat = App.GameData.SelfSeat % 6;
            var curPlayerScore = BSSetting.Dat_PlayersScore[selfSeat].Val;
            if (curPlayerScore >= PlayersBatterys.MinGunStyle)
            {
                var useScore = BSSetting.Dat_PlayersGunScore[selfSeat].Val;
                if (curPlayerScore < useScore)
                {
                    Operation.ChangePriorGunStyle();
                }
                return;
            }
            // 您的金币太少了，充值后可以体验更刺激的游戏。  
            YxMessageBox.Show("您的金币太少了，充值后可以体验更刺激的游戏。", "系统提示！", (msgBox, btnName) =>
             {
                 OnQuitGame();
             });
        }

        /// <summary>
        /// 离开座位
        /// </summary>
        /// <param name="playerIndex"></param>
        public void LeaveRoom(int playerIndex)
        {
            var player = PlayersBatterys[playerIndex];
            player.Display(false);
        }

        public void Clear()
        {
            Pool_GameObj.Clear();
        }

        /// <summary>
        /// 设置玩家信息
        /// </summary>
        /// <param name="response"></param>
        /// <param name="isBuy"></param>
        public void SetUserInfo(ISFSObject response, bool isBuy = true)
        {
            Player player;
            if (response.ContainsKey(RequestKey.KeySeat))
            {
                var seat = response.GetInt(RequestKey.KeySeat);
                player = PlayersBatterys[seat];
            }
            else
            {
                player = PlayersBatterys.UserSelf;
            }

            var coin = response.GetInt(RequestKey.KeyCoin);
            if (isBuy)
            {
                player.BuyCoin(coin);
            }
            else
            {
                player.RetrieveCoin();
            }
            var totalCoin = response.ContainsKey(RequestKey.KeyGold) ? response.GetLong(RequestKey.KeyGold) : 0;
            var bottomView = GameSystemUI.BottomView;
            App.GetGameData<FishGameData>().TotalCoin = totalCoin;
            if (bottomView != null) bottomView.SetUserCoin(totalCoin);
        }

        /// <summary>
        /// 其他玩家进入
        /// </summary>
        /// <param name="playerData"></param>
        public void OtherJoinRoom(ISFSObject playerData)
        {
            var playerInfo = playerData.GetSFSObject(RequestKey.KeyUser);
            PlayersBatterys.AddPlayer(playerInfo);
        }

        /// <summary>
        /// 玩家退出
        /// </summary>
        /// <param name="seat"></param>
        public void UserOutRoom(int seat)
        {
            PlayersBatterys.RemovePlayer(seat);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var ca = GetComponent<Camera>();
            var size = ca.orthographicSize;
            var size2 = size * ca.aspect;
            var far = ca.farClipPlane;

            DrawRect(transform, size2, size, 0);
            DrawRect(transform, size2, size, far);

            DrawLine(transform, -size2, -size, 0, far);
            DrawLine(transform, size2, -size, 0, far);
            DrawLine(transform, size2, size, 0, far);
            DrawLine(transform, -size2, size, 0, far);

            //背景区
            DrawRect(transform, size2, size, 950);
            //鱼
            DrawRect(transform, size2, size, 800);
            //波纹
            DrawRect(transform, size2, size, 300);
            //子弹
            DrawRect(transform, size2, size, 200);
            //UI
            DrawRect(transform, size2, size, 100);

        }


        public static void DrawRect(Transform ts, float w, float h, float f)
        {
            var a = ts.TransformPoint(new Vector3(-w, -h, f)); //a
            var b = ts.TransformPoint(new Vector3(w, -h, f)); //b
            var c = ts.TransformPoint(new Vector3(w, h, f)); //c
            var d = ts.TransformPoint(new Vector3(-w, h, f)); //d

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }

        public static void DrawLine(Transform ts, float w, float h, float f, float f1)
        {
            var a = ts.TransformPoint(new Vector3(w, h, f)); //a
            var aa = ts.TransformPoint(new Vector3(w, h, f1)); //b
            Gizmos.DrawLine(a, aa);
        }
#endif

        public static void OnQuitGame()
        {
            YxMessageBox.Show(null, "QuitMesBox", "确定要退出游戏吗？", "系统提示", (box, btnName) =>
             {
                 if (btnName == YxMessageBox.BtnLeft)
                 {
                     App.QuitGame();
                 }
             }, false, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        }

        public void JoinRobot(ISFSObject response)
        {
            if (PlayersBatterys == null) return;
            if (!response.ContainsKey("name")) return;
            var npcName = response.GetUtfString(RequestKey.KeyName);
            var nid = response.GetInt("nid");
            PlayersBatterys.AddNpc(npcName, nid);
        }

        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            YxDebug.Log("-----------游戏开始--------------");
            var gdata = App.GetGameData<FishGameData>();
            gdata.GameState = YxGameState.Run;
            State_ = State.Normal;
            GameOdds.GainRatio = GameOdds.GainRatios[(int)(BSSetting.GameDifficult_.Val)];
            GameOdds.GainRatioConditFactor = GameOdds.GainRatioConditFactors[(int)(BSSetting.ArenaType_.Val)];
            StartCoroutine(_Coro_MainProcess());
            Pool_GameObj.Init();
            var evtMainProcessStartGame = gdata.EvtMainProcessStartGame;
            if (evtMainProcessStartGame != null) evtMainProcessStartGame();
            App.GetGameData<FishGameData>().Msgrate = gameInfo.GetInt(FishRequestKey.Msgrate);
            GameSystemUI.Init(gameInfo);
            PlayersBatterys.Init(gameInfo);
            var self = PlayersBatterys.UserSelf;
            Operation.UserSelf = self;
            if (self.Idx >= PlayersBatterys.Count / 2) RotatePond(180);

            FishGenerator.Init(gameInfo);
            StartCoroutine(_Coro_DelayInitGame());
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {
        }

        public override void GameStatus(int status, ISFSObject info)
        {
        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            switch ((FishRequestType)type)
            {
                case FishRequestType.HitFish: //碰鱼
                    {
                        //todo 通过座位碰鱼
                        var player = PlayersBatterys.UserSelf;
                        if (player != null) player.OnGameDataRecv(response); 
                    }
                    break;
                case FishRequestType.BuyCoin: //玩家上分 
                    {
                        YxDebug.Log("玩家上分！");
                        var gdata = App.GetGameData<FishGameData>();
                        gdata.CanBuyCoin = true;
                        gdata.CanRetrieveCoin = true;
                        SetUserInfo(response);
                    }
                    break;
                case FishRequestType.Sell:
                    {
                        YxDebug.Log("玩家下分！");
                        var gdata = App.GetGameData<FishGameData>();
                        gdata.CanRetrieveCoin = true;
                        SetUserInfo(response, false);
                    }
                    break;
                case FishRequestType.Quit: //退出玩家
                    YxDebug.Log("可以断开游戏");
                    App.QuitGame();
                    break;
                case FishRequestType.RobotOut:
                    JoinRobot(response);
                    break;
                case FishRequestType.Message:
                    //                    var userName = response.GetUtfString(FishRequestKey.Msg);
                    var userName = response.GetUtfString("UserName");
                    var fishName = response.GetUtfString("FishName");
                    var coin = response.GetInt("Coin");
                    var fishRate = response.GetInt("FishRate");
                    var msg = string.Format("恭喜玩家 <b><color=#ffff00>{0}</color></b> 捕捉到 <b><color=#ffff00>{1}</color></b> ,以 <b><color=#ff0000>{2}</color></b>倍数获得大奖 <b><color=#ff0000>{3}</color></b>金币", userName, fishName, fishRate, coin);
                    var noticeMsg = new YxNoticeMessageData()
                    {
                        Message = msg,
                        ShowType = 1
                    };
                    Debug.LogError(msg);
                    YxNoticeMessage.ShowNoticeMsg(noticeMsg);
                    break;
                case FishRequestType.FirePower: //发射请求 
                    {
                        //todo 通过座位发射
                        var blt = response.GetInt(FishRequestKey.Blt);
                        var id = response.GetInt(RequestKey.KeyId);
                        var isLock = response.GetBool(FishRequestKey.LockB);
                        var player = PlayersBatterys.UserSelf;
                        var gun = player.GunInst;
                        if (gun == null) break;
                        gun.OnFire(blt, isLock, id);
                    }
                    break;
                    //case 出鱼:id , 三点
            }
        }

        public override void OnOtherPlayerJoinRoom(ISFSObject sfsObject)
        {
            base.OnOtherPlayerJoinRoom(sfsObject);
            OtherJoinRoom(sfsObject);
        }
    }
}