using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class EffectObject : ObjectBase
    {
        public const string AssetsNamePrefix = "EffectMahjong-";

        public PoolObjectType Type;
        public ParticleSystem ZuihouyijuEffect;

        public override void Execute()
        {
            base.Execute();
            ZuihouyijuEffect.Stop();
            ZuihouyijuEffect.Play();
        }
    }
}