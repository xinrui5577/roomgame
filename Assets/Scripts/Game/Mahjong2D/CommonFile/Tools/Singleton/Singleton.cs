using System;

namespace Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton
{
    public class Singleton<T> where T : class 
    {

        private static T _instance;

        private static readonly object _safelock=new object();

        public static T Instance
        {
            get
            {
                lock (_safelock)
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
            if(_instance!=null)
            {
                lock (_safelock)
                {
                    _instance = null;
                }
            }
        }

    }
}
