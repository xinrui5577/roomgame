using Assets.Scripts.Game.Fishing.entitys;
using UnityEngine;

namespace Assets.Scripts.Game.Fishing.datas
{
    /// <summary>
    /// 子弹数据
    /// </summary>
    public struct BulletData
    {
        /// <summary>
        /// 子弹服务器分配的Id
        /// </summary>
        public int Id;
        /// <summary>
        /// 
        /// </summary>
        public int Bet;
        /// <summary>
        /// 谁的子弹
        /// </summary>
        public FishingPlayer Player;
        /// <summary>
        /// 子弹模型
        /// </summary>
        public Bullet PreFabBullet;
        /// <summary>
        /// 是否为穿透弹
        /// </summary>
        public bool IsPenetrate;
        /// <summary>
        /// 威力等级
        /// </summary>
        public int PowerLeve;
        /// <summary>
        /// 生成位置
        /// </summary>
        public Vector3 StartPos;
        /// <summary>
        /// 生成位置
        /// </summary>
        public Quaternion StartDirection;
        /// <summary>
        /// 
        /// </summary>
        public Fish TargetFish;
    }
}
