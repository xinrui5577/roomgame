namespace Assets.Scripts.Game.Fishing.enums
{
    public enum EFishAbilityType
    {
        /// <summary>
        /// 正常的
        /// </summary>
        Normal,
        /// <summary>
        /// 炸弹，伤害周围鱼
        /// </summary>
        Bomb,
        /// <summary>
        /// 同类炸弹，伤害同类鱼
        /// </summary>
        SimilarBomb,
        /// <summary>
        /// 定身术
        /// </summary>
        FixedBody,
        /// <summary>
        /// 冰冻,冰冻屏幕鱼一段时间
        /// </summary>
        Frozen,
        /// <summary>
        /// 火烧
        /// </summary>
        Burn,
        /// <summary>
        /// 地震
        /// </summary>
        Earthdin,
        /// <summary>
        /// 导弹，掉落导弹
        /// </summary>
        Missile,
        /// <summary>
        /// 不死之身，无法捕捉，但是概率性的给随机金币
        /// </summary>
        Immortal
    }
}
