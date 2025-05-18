namespace Assets.Scripts.Game.Fishing.enums
{
    public enum EFishingEventType
    {
        /// <summary>
        /// 创建子弹
        /// </summary>
        CreateBullet,  
        /// <summary>
        /// 鱼池大小变化
        /// </summary>
        ResizeFishponBound,
        /// <summary>
        /// 鱼的状态
        /// </summary>
        SwimmerState,
        /// <summary>
        /// 切换地图
        /// </summary>
        ChangeMap,
        /// <summary>
        /// 创建鱼
        /// </summary>
        CreateFish,
        /// <summary>
        /// 击中鱼
        /// </summary>
        HitFish,
        /// <summary>
        /// 新手引导
        /// </summary>
        Guide,
        /// <summary>
        /// 释放技能
        /// </summary>
        ReleaseArt,
        /// <summary>
        /// 飞金币完成
        /// </summary>
        FlyCoinFinished
    }
}
