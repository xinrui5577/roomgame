using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [System.Serializable]
    public class MahjongAdpOffsetConfig : RatioConfig
    {
        public Vector3 Position;
    }

    public class MahjongAdpOffset : AdaptationBase<MahjongAdpOffsetConfig>
    {
        protected override void Adptation()
        {
            transform.localPosition = mCurrConfig.Position;
        }
    }
}