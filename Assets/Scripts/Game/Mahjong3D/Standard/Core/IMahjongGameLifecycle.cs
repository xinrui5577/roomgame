namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public interface IGameLifecycle { }

    /// <summary>
    /// 【初始化游戏场景】时调用方法接口，执行所有继承此接口的脚本
    /// </summary>
    public interface ISceneInitCycle : IGameLifecycle
    {
        void OnSceneInitCycle();
    }

    /// <summary>
    /// 【接收到GameInfo】时调用方法接口，执行所有继承此接口的脚本
    /// </summary>
    public interface IGameInfoICycle : IGameLifecycle
    {
        void OnGameInfoICycle();
    }

    /// <summary>
    /// 【游戏准备】时调用方法接口，执行所有继承此接口的脚本
    /// </summary>
    public interface IGameReadyCycle : IGameLifecycle
    {
        void OnGameReadyCycle();
    }

    /// <summary>
    /// 【开始游戏】时调用方法接口，执行所有继承此接口的脚本
    /// </summary>
    public interface IGameStartCycle : IGameLifecycle
    {
        void OnGameStartCycle();
    }

    /// <summary>
    /// 【游戏一局結束】时调用方法接口，执行所有继承此接口的脚本
    /// </summary>
    public interface IGameEndCycle : IGameLifecycle
    {
        void OnGameEndCycle();
    }

    /// <summary>
    /// 【游戏重连】时调用方法接口，执行所有继承此接口的脚本
    /// </summary>
    public interface IReconnectedCycle : IGameLifecycle
    {
        void OnReconnectedCycle();
    }

    /// <summary>
    /// 【继续开局】时调用方法接口，执行所有继承此接口的脚本
    /// </summary>
    public interface IContinueGameCycle : IGameLifecycle
    {
        void OnContinueGameCycle();
    }

    /// <summary>
    /// 【游戏回放】时调用方法接口，执行所有继承此接口的脚本
    /// </summary>
    public interface IReplayCycle : IGameLifecycle
    {
        void OnReplayCycle();
    }
}