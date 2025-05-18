using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum AdaptationType
    {
        Normal, //1334*720
        Ipad,   //4:3
    }

    public class RatioConfig
    {
        public AdaptationType Type;
    }

    public abstract class AdaptationBase<T> : MonoBehaviour where T : RatioConfig
    {
        public T[] Configs;
        protected T mCurrConfig;

        /// <summary>
        /// 分辨率
        /// </summary>
        protected float mRatio
        {
            get
            {
                float width = Screen.width;
                float height = Screen.height;
                return height / width;
            }
        }

        private void Awake()
        {
            AdaptationType currAdpType = AdaptationType.Normal;
            if (mRatio <= 0.65f)
            {
                currAdpType = AdaptationType.Normal;
            }
            else
            {
                currAdpType = AdaptationType.Ipad;
            }
            for (int i = 0; i < Configs.Length; i++)
            {
                if (currAdpType == Configs[i].Type)
                {
                    mCurrConfig = Configs[i];
                }
            }
        }

        private void Start()
        {
            if (!mCurrConfig.ExIsNullOjbect())
            {
                Adptation();
            }
        }

        protected abstract void Adptation();
    }
}