using UnityEngine;

namespace Assets.Scripts.Game.FishGame.FishKinds
{
    public class FishGeneratConfig : MonoBehaviour
    {
        /// <summary>
        /// 单一屏幕出鱼数
        /// </summary>
        public int MaxFishAtUnitWorld = 100;
        /// <summary>
        /// 普通鱼数据
        /// </summary>
        public FishKindGenerator NormalGenerator;
        public KindGenerator[] Generators;
    }
}
                                    