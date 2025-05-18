using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum GameType
    {
        /// <summary>
        /// 游戏
        /// </summary>
        Normal,
        /// <summary>
        /// 回放
        /// </summary>
        Replay,
    }

    public class GameCenter : MonoBehaviour
    {
        public static GameCenter Instance;

        /// <summary>
        /// 重连忽略
        /// </summary>
        public bool IgonreReconnect { get; set; }
        /// <summary>
        /// 金币房 游戏状态
        /// 防止游戏进行中 游戏切到后台，回来时已经游戏结束，但是动画还在进行，这时候继续下一局，会弹出上一局小结算界面。
        /// </summary>
        public bool YuLeBoutState { get; set; }

        public GameType GameType = GameType.Normal;

        public void SetIgonreReconnectState(bool state)
        {
            IgonreReconnect = state;
        }

        public bool IsGameStart()
        {
            //当前玩家数等于最大人数
            if (DataCenter.Players.CurrPlayerCount != DataCenter.ConfigData.MaxPlayerCount)
            {
                return false;
            }
            //所有玩家都准备
            if (!DataCenter.Players.CheckAllReady())
            {
                return false;
            }
            return true;
        }

        private void Awake() { Instance = this; }

        private void Start() { StartCoroutine(MahjongGameStart()); }

        private IEnumerator MahjongGameStart()
        {
            yield return new WaitForSeconds(0.1f);
            // 初始化任务管理器
            ContinueTaskManager.OnInit();
            // 初始化系统组件
            InitBuiltinComponents();
            // 游戏开始 
            GameProcess.StartupProcess();
        }

        private void OnDestroy()
        {
            mComponents.Clear();
            YuLeBoutState = false;
            IgonreReconnect = false;
            Instance = null;
        }

        private readonly static List<BaseComponent> mComponents = new List<BaseComponent>();

        private EventHandlerComponent mEventHandler;
        public static EventHandlerComponent EventHandle
        {
            get { return Instance.mEventHandler; }
        }

        private GameProcessComponent mGameProcess;
        public static GameProcessComponent GameProcess
        {
            get { return Instance.mGameProcess; }
        }

        private PoolsComponent mPools;
        public static PoolsComponent Pools
        {
            get { return Instance.mPools; }
        }

        private NetworkComponent mNetwork;
        public static NetworkComponent Network
        {
            get { return Instance.mNetwork; }
        }

        private MahjongSceneComponent mMahjongScene;
        public static MahjongSceneComponent Scene
        {
            get { return Instance.mMahjongScene; }
        }

        private HudComponent mHud;
        public static HudComponent Hud
        {
            get { return Instance.mHud; }
        }

        private DataCenterComponent mDataCenter;
        public static DataCenterComponent DataCenter
        {
            get { return Instance.mDataCenter; }
        }

        private ShortcutsComponent mShortcuts;

        public static ShortcutsComponent Shortcuts
        {
            get { return Instance.mShortcuts; }
        }

        private GameLifecycleComponent mLifecycle;
        public static GameLifecycleComponent Lifecycle
        {
            get { return Instance.mLifecycle; }
        }

        private AssetsComponent mAssets;
        public static AssetsComponent Assets
        {
            get { return Instance.mAssets; }
        }

        private ReplayComponent mReplay;

        public static ReplayComponent Replay
        {
            get { return Instance.mReplay; }
        }

        private ControllerComponent mController;
        public static ControllerComponent Controller
        {
            get { return Instance.mController; }
        }

        private GameLogicComponent mGameLogic;
        public static GameLogicComponent GameLogic
        {
            get { return Instance.mGameLogic; }
        }

        private void InitBuiltinComponents()
        {
            mEventHandler = GetSystemComponent<EventHandlerComponent>();
            mLifecycle = GetSystemComponent<GameLifecycleComponent>();
            mGameProcess = GetSystemComponent<GameProcessComponent>();
            mDataCenter = GetSystemComponent<DataCenterComponent>();
            mController = GetSystemComponent<ControllerComponent>();
            mGameLogic = GetSystemComponent<GameLogicComponent>();
            mShortcuts = GetSystemComponent<ShortcutsComponent>();
            mReplay = GetSystemComponent<ReplayComponent>();
            mNetwork = GetSystemComponent<NetworkComponent>();
            mAssets = GetSystemComponent<AssetsComponent>();
            mPools = GetSystemComponent<PoolsComponent>();

            mHud = GetSystemComponent<HudComponent>();
            mMahjongScene = GetSystemComponent<MahjongSceneComponent>();

            OnInitalization();
        }

        private void OnInitalization()
        {
            for (int i = 0; i < mComponents.Count; i++)
            {
                mComponents[i].OnLoad();
            }
            for (int i = 0; i < mComponents.Count; i++)
            {
                mComponents[i].OnInitalization();
            }
        }

        public T GetSystemComponent<T>() where T : BaseComponent
        {
            return (T)GetSystemComponent(typeof(T));
        }

        public BaseComponent GetSystemComponent(Type type)
        {
            for (int i = 0; i < mComponents.Count; i++)
            {
                if (mComponents[i].GetType() == type)
                {
                    return mComponents[i];
                }
            }
            return null;
        }

        /// <summary>
        /// 注册周期函数
        /// </summary>
        /// <param name="obj"> 注册对象 </param>
        public static void RegisterCycle(IGameLifecycle obj)
        {
            if (Instance != null) Instance.mLifecycle.Register(obj);
        }

        public static void RemoveRegisterCycle(IGameLifecycle obj)
        {
            if (Instance != null) Instance.mLifecycle.RemoveRegister(obj);
        }

        public static void RegisterComponent(BaseComponent component)
        {
            if (component != null) mComponents.Add(component);
        }
    }
}