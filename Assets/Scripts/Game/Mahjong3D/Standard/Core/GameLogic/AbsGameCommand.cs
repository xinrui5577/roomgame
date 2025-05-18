namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public interface IGameCommand
    {
        void OnReset();
    }

    public abstract class AbsGameCommand<T> : IGameCommand where T : AbsCommandAction
    {
        protected T mLogicAction;

        public T LogicAction
        {
            get
            {
                if (mLogicAction == null)
                {
                    var binder = GameCenter.Assets.TypeBinder;
                    if (binder != null)
                    {
                        mLogicAction = binder.GetInstance<T>(this.GetType());
                        mLogicAction.OnInit();
                    }
                }
                return mLogicAction;
            }
        }

        public virtual void OnReset()
        {
            if (mLogicAction != null) LogicAction.OnReset();
        }

        public TAction GetLogicAction<TAction>() where TAction : T
        {
            if (mLogicAction != null) return mLogicAction as TAction;
            return null;
        }
    }
}
