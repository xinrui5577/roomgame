using System;

namespace Assets.Scripts.Game.lyzz2d.Utils.Single
{
    public class Singleton<T> where T : class
    {
        private static T _instance;

        private static readonly object Safelock = new object();

        public static T Instance
        {
            get
            {
                lock (Safelock)
                {
                    if (_instance == null)
                    {
                        _instance = (T) Activator.CreateInstance(typeof (T), true);
                    }
                }
                return _instance;
            }
        }

        public void DelMe()
        {
            if (_instance != null)
            {
                lock (Safelock)
                {
                    _instance = null;
                }
            }
        }
    }
}