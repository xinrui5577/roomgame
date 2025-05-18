using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [System.Serializable]
    public class ZhongniaoRatioConfig : RatioConfig
    {
        public Vector3 ZhongniaoRotation;
    }

    public class ZhongniaoAdaptation : AdaptationBase<ZhongniaoRatioConfig>
    {
        protected override void Adptation()
        {
            transform.localPosition = mCurrConfig.ZhongniaoRotation;
        }
    }
}