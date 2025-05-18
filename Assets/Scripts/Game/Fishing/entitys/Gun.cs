using System.Collections.Generic;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.enums;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 炮
    /// </summary>
    public class Gun : MonoBehaviour
    {
        private Transform _transform;
        private bool _isFire;
         
        /// <summary>
        /// /数据
        /// </summary>
        public GunData Data;
        /// <summary>
        /// 开火间隔
        /// </summary>
        public float FireCd = 0.2f;
        /// <summary>
        /// 子弹上限
        /// </summary>
        public int BulletMax = 20;
        /// <summary>
        /// 可用子弹
        /// </summary>
        public Bullet[] BulletPrefabs;
        /// <summary>
        /// 火花
        /// </summary>
        public Spark TheSpark;
        /// <summary>
        /// 子弹生成位置
        /// </summary>
        public Transform BulletPoint;
        /// <summary>
        /// 倍数
        /// </summary>
        public YxBaseLabelAdapter BetLabel;
        /// <summary>
        /// 准星
        /// </summary>
        public AimingRuleSight TheAimingRuleSight;

        public Animation TheAnimation;

        public string FireSound = "sound_shot";
        /// <summary>
        /// 子弹缓存池
        /// </summary>
        private List<Bullet> _bulletBuffer = new List<Bullet>();

        void Start()
        {
            _transform = transform;
            _transform.localEulerAngles = new Vector3(90,90,90);
            if(TheAimingRuleSight!=null) TheAimingRuleSight.Init();
        }
          
        public void OnFire(BulletData bulletData)
        {
            Facade.Instance<MusicManager>().Play(FireSound);
            // 特效
            if (TheSpark != null)
            {
                TheSpark.Show();
            }
            if (TheAnimation != null)
            {
                TheAnimation.Play();
            }
            //声音
            // 生成子弹 
            Facade.EventCenter.DispatchEvent(EFishingEventType.CreateBullet, bulletData);
            // 
        }

        /// <summary>
        /// 减少倍数
        /// </summary>
        public void SetBet(int bet)
        {
            Data.Bet = bet;
            BetLabel.Text(bet);
        }

        /// <summary>
        /// 切换双倍模式
        /// </summary>
        /// <param name="isDouble"></param>
        public void ChangeDoublePattern(bool isDouble)
        {
            Data.IsDouble = isDouble;
        }

        /// <summary>
        /// 切换锁定模式
        /// </summary>
        /// <param name="isLock"></param>
        public void ChangeLockPattern(bool isLock)
        {
            Data.IsLock = isLock;
            if (!isLock)
            {
                TheAimingRuleSight.Hide();
            }
        }
    }
}
