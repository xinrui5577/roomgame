using System.Collections.Generic;
using Sfs2X.Entities.Data;
using System.Reflection;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GameLogicComponent : BaseComponent, IGameEndCycle
    {
        private List<AbsCommandAction> mC2SLogicAction = new List<AbsCommandAction>();
        private Dictionary<Type, IGameCommand> mReplayCommands = new Dictionary<Type, IGameCommand>();

        /// <summary>
        /// 响应服务器事件
        /// </summary>
        private Dictionary<int, Action<ISFSObject>> mResponseHandlers = new Dictionary<int, Action<ISFSObject>>();
        /// <summary>
        /// 消息队列
        /// </summary>
        private Queue<KeyValuePair<int, ISFSObject>> mS2CResponseQueue = new Queue<KeyValuePair<int, ISFSObject>>();
        /// <summary>
        /// 不执行Op逻辑的响应
        /// </summary>
        private List<int> mSimpleFilter = new List<int>();
        /// <summary>
        /// 消息延迟事件
        /// </summary>
        private float mResponseDelayTimer = 0.02f;

        public void RefreshResponseQueue()
        {
            mS2CResponseQueue.Clear();
        }

        /// <summary>
        /// 监听服务器事件
        /// </summary>
        /// <param name="protocolKey"></param>
        /// <param name="data"></param>
        public void GameResponse(int protocolKey, ISFSObject data)
        {
            mS2CResponseQueue.Enqueue(new KeyValuePair<int, ISFSObject>(protocolKey, data));
        }

        /// <summary>
        /// 获取S2C定义的事件
        /// </summary>     
        public Action<ISFSObject> DispatchResponseHandlers(int key)
        {
            if (mResponseHandlers.ContainsKey(key))
            {
                return mResponseHandlers[key];
            }
            return null;
        }

        public T GetGameResponseLogic<T>() where T : class, IGameCommand
        {
            Type type = typeof(T);
            if (mReplayCommands.ContainsKey(type))
            {
                return mReplayCommands[type] as T;
            }
            return null;
        }

        public override void OnInitalization()
        {
            GameLogicCollection();
            GameCenter.RegisterCycle(this);
            ContinueTaskManager.NewTask().AppendFuncTask(() => ContinueS2CResponse()).Start();
        }

        private void GameLogicCollection()
        {
            var types = GameCenter.Assets.TypeBinder.CollectionGameLogicType();
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var response = Activator.CreateInstance(type) as IGameCommand;
                if (response != null)
                {
                    mReplayCommands.Add(type, response);
                    var methods = type.GetMethods();
                    for (int k = 0; k < methods.Length; k++)
                    {
                        CollectionReplaykHandler(response, methods[k]);
                    }
                }
            }

            types = GameCenter.Assets.TypeBinder.CollectionC2SActionType();
            for (int i = 0; i < types.Count; i++)
            {
                var type = types[i];
                var action = Activator.CreateInstance(type) as AbsCommandAction;
                if (action != null)
                {
                    mC2SLogicAction.Add(action);
                    action.OnInit();
                }
            }
        }

        private void CollectionReplaykHandler(IGameCommand response, MethodInfo method)
        {
            S2CResponseHandlerAttribute handlerAtt = null;
            var handlerAtts = method.GetCustomAttributes(typeof(S2CResponseHandlerAttribute), false);
            if (null != handlerAtts && handlerAtts.Length > 0)
            {
                handlerAtt = handlerAtts[0] as S2CResponseHandlerAttribute;
                var handler = CreateHandler<Action<ISFSObject>>(response, method.Name);
                if (null != handler)
                {
                    mResponseHandlers.Add(handlerAtt.ProtocolKey, handler);
                }
            }
            //收集过滤事件
            handlerAtts = method.GetCustomAttributes(typeof(FilterOperateMenuAttribute), false);
            if (null != handlerAtts && handlerAtts.Length > 0 && handlerAtt != null)
            {
                mSimpleFilter.Add(handlerAtt.ProtocolKey);
            }
        }

        private T CreateHandler<T>(IGameCommand response, string methodName) where T : class
        {
            Delegate action = Delegate.CreateDelegate(typeof(T), response, methodName);
            if (null != action) return action as T;
            return null;
        }

        public void OnGameEndCycle()
        {
            foreach (var item in mReplayCommands)
            {
                item.Value.OnReset();
            }
        }

        /// <summary>
        /// 设置延迟时间
        /// </summary>       
        public void SetDelayTime(float delayTime)
        {
            mResponseDelayTimer = delayTime;
        }

        /// <summary>
        /// 协程任务
        /// </summary>
        /// <returns></returns>
        private IEnumerator<float> ContinueS2CResponse()
        {
            while (true)
            {
                yield return mResponseDelayTimer;
                if (mS2CResponseQueue.Count > 0)
                {
                    mResponseDelayTimer = 0.02f;
                    var param = mS2CResponseQueue.Dequeue();
                    ExecuteS2CResponse(param.Key, param.Value);
                }
            }
        }

        /// <summary>
        /// 执行响应事件
        /// </summary>
        /// <param name="protocolKey"></param>
        /// <param name="data"></param>
        public void ExecuteS2CResponse(int protocolKey, ISFSObject data)
        {
            Action<ISFSObject> handler = null;
            if (protocolKey != NetworkProls.OpreateType && mResponseHandlers.TryGetValue(protocolKey, out handler))
            {
                handler(data);
            }
            if (!mSimpleFilter.Contains(protocolKey))
            {
                GameCenter.Hud.UIPanelController.OnOperateUpdatePanel();
                if (protocolKey == NetworkProls.OpreateType || data.ContainsKey(AnalysisKeys.KeyOp))
                {
                    handler = mResponseHandlers[NetworkProls.OpreateType];
                    handler(data);
                }
            }
            //自定义逻辑
            if (mResponseHandlers.TryGetValue(CustomProl.CustomLogic, out handler))
            {
                handler(data);
            }
        }
    }
}