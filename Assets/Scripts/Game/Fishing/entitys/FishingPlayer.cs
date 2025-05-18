using System;
using Assets.Scripts.Game.Fishing.commons;
using Assets.Scripts.Game.Fishing.datas;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Fishing.entitys
{
    public class FishingPlayer : YxBaseGamePlayer
    {
        /// <summary>
        /// 枪槽
        /// </summary>
        public Transform GunManger;
        /// <summary>
        /// 金币容器
        /// </summary>
        public Transform CoinContaint;
        /// <summary>
        /// 金币堆容器
        /// </summary>
        public CoinPileContaint CoinPileContaint;
        /// <summary>
        /// 当前的枪
        /// </summary>
        [NonSerialized]
        public Gun TheGun;


        public virtual void KillFish(HitData hitData)
        {

        }

        protected override void FreshUserInfo()
        {
            base.FreshUserInfo();
            var info = GetData<FishingUserInfo>();
            if (info == null) { return;}
            ChangeGun(info.GunLeve);
            SetGunBet(info.BetLeve);
        }

        private void SetGunBet(int leve)
        {
            if (TheGun == null) return;
            var data = App.GetGameData<FishingGameData>();
            var styles = data.GunBets;
            var bet = styles.GetElement(leve);
            TheGun.SetBet(bet);
        }

        public int GunLeve
        {
            get
            {
                var info = this.GetInfo<FishingUserInfo>();
                return info == null ? 1 : info.GunLeve;
            }
            set
            {
                var info = this.GetInfo<FishingUserInfo>();
                if (info == null)return;
                info.GunLeve = value;
                this.ChangeGun(value);
            }
        }
        public int BetLeve
        {
            get
            {
                var info = this.GetInfo<FishingUserInfo>();
                return info == null ? 0 : info.BetLeve;
            }
            set
            {
                var info = this.GetInfo<FishingUserInfo>();
                if (info == null)return;
                info.BetLeve = value;
                SetGunBet(value);
            }
        }

        /// <summary>
        /// 换枪
        /// </summary>
        public void ChangeGun(int leve)
        {
            if (TheGun != null)
            {
                if (TheGun.Data.PowerLeve == leve)
                {
                    return;
                }
            }
            var assetName = string.Format("Gun{0}",leve);
            var bdName = string.Format("guns/{0}",assetName);
            var go = ResourceManager.LoadAsset(assetName, bdName);
            if (go == null)
            {
                YxDebug.LogError("换枪失败！！！","FishingPlayer");
                return;
            }
            var gunGo = GameObjectUtile.Instantiate(go, GunManger);

            GunData gunData;
            if (TheGun != null)
            {
                gunData = TheGun.Data;
                Destroy(TheGun.gameObject);
            }
            else
            {
                gunData = new GunData();
            }
            gunData.Player = this;
            gunData.PowerLeve = leve;
            TheGun = gunGo.GetComponent<Gun>();
            TheGun.Data = gunData;
            //todo  设置配置，GunSetting
        }



        /// <summary>
        /// 方向
        /// </summary>
        /// <param name="direction"></param>
        public void TurnGunManger(Vector3 direction)
        {
            GunManger.LookAt(direction, Vector3.back);
        }

        /// <summary>
        /// 执行射击
        /// </summary>
        /// <param name="blt"></param>
        /// <param name="id"></param>
        /// <param name="isLock"></param>
        public virtual void OnFire(int blt, int id, bool isLock)
        { 
            if (TheGun == null) return;
            var bulletLeve = TheGun.Data.IsDouble ? 1 : 0;
            var bulletData = new BulletData
            {
                Id = id,
                PreFabBullet = TheGun.BulletPrefabs.GetElement(bulletLeve),
                Bet = blt,
                PowerLeve = 0,//todo 粒子炮用
                StartPos = TheGun.BulletPoint.position,
                StartDirection = TheGun.BulletPoint.rotation,
                IsPenetrate = TheGun.Data.IsPenetrate,
                Player = this,
                TargetFish = TargetFish
            };
            Coin -= blt;
            TheGun.OnFire(bulletData);
        }

        /// <summary>
        /// 获得金币
        /// </summary>
        /// <param name="coin"></param>
        public void GainCoin(int coin)
        {
            SetCoin(coin);
        }


        protected Fish TargetFish;
        /// <summary>
        /// 瞄准
        /// </summary>
        public void OnLockFire(Fish fish)
        {
            //显示瞄准
            var aimingRuleSight = TheGun.TheAimingRuleSight;
            TargetFish = fish;
            if (aimingRuleSight != null)
            {
                aimingRuleSight.LockAt(TargetFish);
            }
        }

        private void Update()
        {
            //瞄准
            if (TargetFish != null)
            {
                var pos = TargetFish.transform.position;
                pos.z = 0;
                TurnGunManger(pos);
            }
            //
            OnUpdate();
        }

        protected virtual void OnUpdate()
        {
        }


        public void ChangeBarreTypeClick(bool isChange)
        {
            TheGun.ChangeDoublePattern(isChange);
        }
    }
}
