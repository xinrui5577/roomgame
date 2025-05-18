using System;
using System.Collections.Generic;
using System.Reflection;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ReplayComponent : BaseComponent
    {
        /// <summary>
        /// 每一帧播放间隔
        /// </summary>
        public float FrameInterval;
        /// <summary>
        /// 当前播放速度
        /// </summary>
        protected float mCurrRate;

        /// <summary>
        /// 回放逻辑缓存
        /// 正播放   key > 0
        /// 回退播放 key < 0
        /// </summary>
        private Dictionary<int, Action<ReplayFrameData>> mPlacbackHandlers = new Dictionary<int, Action<ReplayFrameData>>();
        /// <summary>
        /// 持有服务器监听事件的对象缓存
        /// </summary>
        private Dictionary<Type, IGameCommand> mGameResponseCommands = new Dictionary<Type, IGameCommand>();

        public MahjongReplayGameManager ReplayManager { get; private set; }

        public MahjongReplayGameData ReplayData { get; private set; }

        /// <summary>
        /// 最小帧索引
        /// </summary>
        protected int mFrameMixLimit;
        /// <summary>
        /// 最大帧索引
        /// </summary>
        protected int mFrameMaxLimit;
        /// <summary>
        /// 当前帧
        /// </summary>
        protected int mCurrFrame;

        /// <summary>
        /// 播放中
        /// </summary>
        //protected bool mPlaying;
        /// <summary>
        /// 暂停播放
        /// </summary>
        protected bool mPauseFlag;

        /// <summary>
        /// 上一帧消息
        /// </summary>
        protected ReplayFrameData mLastFrameData;

        public override void OnInitalization()
        {
            ReplayManager = GetComponent<MahjongReplayGameManager>();
            ReplayData = GetComponent<MahjongReplayGameData>();
            CollectionCommands();
        }

        public void ReplayRestart()
        {
            OnReset();
            InitReplayScene();
        }

        public void StartupTask()
        {
            ContinueTaskManager.NewTask().AppendFuncTask(() => ExecuteReplayFrame()).Start();
        }

        public void InitReplayData()
        {
            //收集玩家手牌数据
            var playerCards = ReplayData.PlayerCards;
            mFrameMixLimit = playerCards.Count - 1;
            mFrameMaxLimit = ReplayData.FrameMessage.Count - 1;

            //设置帧率
            SetRate(1);

            //设置全是数据
            var dataCenter = GameCenter.DataCenter;
            dataCenter.ConfigData.MaxPlayerCount = playerCards.Count;
        }

        public void PauseTask()
        {
            mPauseFlag = true;
        }

        public void PlayTask()
        {
            mPauseFlag = false;
        }

        public void SetRate(float rate)
        {
            FrameInterval = rate * -0.08f + 1.08f;
        }

        /// <summary>
        /// 计算下一帧
        /// </summary>
        private void CalculateNextFrame()
        {
            mCurrFrame++;
            if (mCurrFrame >= mFrameMaxLimit)
            {
                mCurrFrame = mFrameMaxLimit;
            }
        }

        public void PlayNextFrame()
        {
            if (mPauseFlag)
            {
                CalculateNextFrame();
                if (mCurrFrame <= mFrameMaxLimit)
                {
                    Execute(ReplayData.FrameMessage[mCurrFrame]);
                }
            }
        }

        /// <summary>
        /// 执行回放
        /// </summary>
        protected IEnumerator<float> ExecuteReplayFrame()
        {
            while (true)
            {
                yield return FrameInterval;
                if (mPauseFlag)
                {
                    yield return ContinueTaskAgent.Pause;
                }
                else
                {
                    if (ReplayData.FrameMessage.Count > 0)
                    {
                        CalculateNextFrame();
                        ReplayFrameData data;
                        if (ReplayData.FrameMessage.TryGetValue(mCurrFrame, out data))
                        {
                            Execute(data);
                            if (mCurrFrame == mFrameMaxLimit)
                            {
                                OnReset();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        protected void Execute(ReplayFrameData message)
        {
            if (message == null) return;

            Action<ReplayFrameData> handler;
            if (mPlacbackHandlers.TryGetValue(message.Porocol, out handler))
            {
                // 显示dxnb
                GameCenter.Scene.TableManager.SwitchDirection(message.OpChair);

                message.SetLastFrameData(mLastFrameData);
                mLastFrameData = message;
                handler(message);
            }
        }

        public void OnReset()
        {
            mPauseFlag = true;
            mLastFrameData = null;
            mCurrFrame = mFrameMixLimit;

            foreach (var item in mGameResponseCommands)
            {
                item.Value.OnReset();
            }
        }

        public void InitReplayScene()
        {
            foreach (var item in ReplayData.PlayerCards)
            {
                Execute(item.Value);
            }
        }

        public void CollectionCommands()
        {
            var types = GameCenter.Assets.TypeBinder.ReplayLogicTypes();
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var response = Activator.CreateInstance(type) as IGameCommand; //获取响应对象
                if (response != null)
                {
                    mGameResponseCommands.Add(type, response);
                    var methods = type.GetMethods();
                    for (int k = 0; k < methods.Length; k++)
                    {
                        CollectionReplayHandler(response, methods[k]);
                    }
                }
            }
        }

        private void CollectionReplayHandler(IGameCommand response, MethodInfo method)
        {
            var handlerAtts = method.GetCustomAttributes(typeof(ReplayHandlerAttrubute), false);
            if (null != handlerAtts && handlerAtts.Length > 0)
            {
                var handlerAtt = handlerAtts[0] as ReplayHandlerAttrubute;
                var handler = CreateHandler<Action<ReplayFrameData>>(response, method.Name);
                if (null != handler)
                {
                    var symbol = handlerAtt.Type == ReplayType.Normal ? 1 : -1;
                    var key = handlerAtt.ProtocolKey * symbol;
                    mPlacbackHandlers.Add(key, handler);
                }
            }
        }

        private T CreateHandler<T>(IGameCommand response, string methodName) where T : class
        {
            Delegate action = Delegate.CreateDelegate(typeof(T), response, methodName);
            if (null != action) return action as T;
            return null;
        }
    }
}