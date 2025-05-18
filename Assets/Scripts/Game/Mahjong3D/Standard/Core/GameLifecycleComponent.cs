using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class GameLifecycleComponent : BaseComponent
    {
        /// <summary>
        /// 游戏流程对象缓存
        /// </summary>
        private List<IGameLifecycle> mGameLisfcycle = new List<IGameLifecycle>();

        public void SceneInitCycle()
        {
            for (int i = 0; i < mGameLisfcycle.Count; i++)
            {
                Convert<ISceneInitCycle>(mGameLisfcycle[i]).Do(c => c.OnSceneInitCycle());
            }
        }

        public void GameInfoICycle()
        {
            for (int i = 0; i < mGameLisfcycle.Count; i++)
            {
                Convert<IGameInfoICycle>(mGameLisfcycle[i]).Do(c => c.OnGameInfoICycle());
            }
        }

        public void GameReadyCycle()
        {
            for (int i = 0; i < mGameLisfcycle.Count; i++)
            {
                Convert<IGameReadyCycle>(mGameLisfcycle[i]).Do(c => c.OnGameReadyCycle());
            }
        }

        public void GameStartCycle()
        {
            for (int i = 0; i < mGameLisfcycle.Count; i++)
            {
                Convert<IGameStartCycle>(mGameLisfcycle[i]).Do(c => c.OnGameStartCycle());
            }
        }

        public void GameEndCycle()
        {
            for (int i = 0; i < mGameLisfcycle.Count; i++)
            {
                Convert<IGameEndCycle>(mGameLisfcycle[i]).Do(c => c.OnGameEndCycle());
            }
        }

        public void ReconnectedCycle()
        {
            for (int i = 0; i < mGameLisfcycle.Count; i++)
            {
                Convert<IReconnectedCycle>(mGameLisfcycle[i]).Do(c => c.OnReconnectedCycle());
            }
        }

        public void ContinueGameCycle()
        {
            for (int i = 0; i < mGameLisfcycle.Count; i++)
            {
                Convert<IContinueGameCycle>(mGameLisfcycle[i]).Do(c => c.OnContinueGameCycle());
            }
        }

        public void ContinueReplayCycle()
        {
            for (int i = 0; i < mGameLisfcycle.Count; i++)
            {
                Convert<IReplayCycle>(mGameLisfcycle[i]).Do(c => c.OnReplayCycle());
            }
        }

        private T Convert<T>(IGameLifecycle obj) where T : class, IGameLifecycle
        {
            if (!obj.ExIsNullOjbect())
            {
                return obj as T;
            }
            return null;
        }

        public void Register(IGameLifecycle obj)
        {
            if (!obj.ExIsNullOjbect())
            {
                mGameLisfcycle.Add(obj);
            }
        }

        public void RemoveRegister(IGameLifecycle obj)
        {
            if (!obj.ExIsNullOjbect())
            {
                mGameLisfcycle.Remove(obj);
            }
        }
    }
}