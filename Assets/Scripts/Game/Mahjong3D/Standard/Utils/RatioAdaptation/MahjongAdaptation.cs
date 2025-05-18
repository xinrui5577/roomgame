using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [System.Serializable]
    public class MahjongRatioConfig : RatioConfig
    {     
        public Vector3 Scale;    
    }

    public class MahjongAdaptation : AdaptationBase<MahjongRatioConfig>
    {
        protected override void Adptation()
        {
            transform.localScale = mCurrConfig.Scale;
        }
    }
}