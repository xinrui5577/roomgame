using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 事件分发器参数
    /// </summary>
    public class EvtHandlerArgs : EvtArgs
    {
        public T Conver<T>() where T : EvtHandlerArgs
        {
            return this as T;
        }
    }

    /// <summary>
    /// 事件分发组件
    /// </summary>
    public class EventHandlerComponent : BaseComponent
    {
        private EventHandlerSystem<EvtHandlerArgs> EventComponent = new EventHandlerSystem<EvtHandlerArgs>();

        public void Subscriber(int key, EvtHandler<EvtHandlerArgs> action)
        {
            EventComponent.Subscriber(key, action);
        }

        public void Unsubscriber(int key, EvtHandler<EvtHandlerArgs> action)
        {
            EventComponent.Unsubscriber(key, action);
        }

        public void Dispatch(int key, EvtHandlerArgs args = null)
        {
            EventComponent.Dispatch(key, args);
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        /// <typeparam name="T">执行事件需要的对象类型</typeparam>
        /// <param name="key">事件编号</param>
        /// <param name="funcOp">事件对象赋值操作</param>
        public void Dispatch<TArgs>(int key, Action<TArgs> actionOp) where TArgs : EvtHandlerArgs
        {
            if (actionOp == null) return;

            TArgs args = Activator.CreateInstance<TArgs>();
            actionOp(args);
            EventComponent.Dispatch(key, args);
        }
    }
}