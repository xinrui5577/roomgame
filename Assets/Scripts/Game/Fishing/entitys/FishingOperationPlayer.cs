using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.enums;
using com.yxixia.utile.Utiles;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.View;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 可控制玩家
    /// </summary>
    public class FishingOperationPlayer : FishingPlayer
    {
        protected override void OnAwake()
        {
            base.OnAwake();
            Facade.EventCenter.DispatchEvent(EOperationEventTpe.InitPlayer,this);
           
        }

        protected override void OnStart()
        {
            base.OnStart();
            var pos = transform.position;
            var ri = new Vector4(pos.x, pos.y + 60, 0.124f);
            Facade.EventCenter.DispatchEvent(EFishingEventType.Guide, ri);
        }

        /// <summary>
        /// 减少倍数
        /// </summary>
        public void OnSubBetClick()
        {
            if (TheGun == null) return;
            //            TheGun.SetBet();
            var data = App.GetGameData<FishingGameData>();
            var info = GetData<FishingUserInfo>();
            var styles = data.GunBets;
            var len = styles.Length;
            info.BetLeve = (--info.BetLeve + len) % len;
            var bet = styles.GetElement(info.BetLeve);
            TheGun.SetBet(bet);
        }

        /// <summary>
        /// 添加倍数
        /// </summary>
        public void OnAddBetClick()
        {
            if (TheGun == null) return;
            var data = App.GetGameData<FishingGameData>();
            var info = GetData<FishingUserInfo>();
            var styles = data.GunBets;
            info.BetLeve = ++info.BetLeve % styles.Length;
            var bet = styles.GetElement(info.BetLeve);
            TheGun.SetBet(bet);
        }
         
        /// <summary>
        /// 自动开火
        /// </summary>
        public void OnAutoFireClick(Toggle toggle)
        {
            if (TheGun == null) return;
            if (toggle.isOn)
            {
                OnStartFire();
                TheGun.Data.IsAuto = true;
                YxMessageTip.Show("开启自动射击", 3);
            }
            else
            {
                OnStopFire();
                TheGun.Data.IsAuto = false;
            }
        }

        private float _lastFireTime;
        protected override void OnUpdate()
        {
            if (!CheckFireEnable())
            {
                return;
            }
            _lastFireTime = TheGun.FireCd;
            var data = TheGun.Data;
            //创建server子弹
            var score = data.IsDouble ? data.Bet * 2 : data.Bet;
            App.GetRServer<FishingGameServer>().PostFirePower(score, TargetFish!=null);
        }

        public void OnStartFire()
        {
            _isFire = true;
        }

        public void OnStopFire()
        {
            _isFire = false;
        }


        private bool _isFire = false;
        private bool CheckFireEnable()
        {
            if (TheGun == null) return false;
            if (_lastFireTime > 0)
            {
                _lastFireTime -= Time.deltaTime;
                return false;
            }
            var gunData = TheGun.Data;
            //todo if()  金币不足
            if (Coin < gunData.TotalBet)
            {
                YxMessageTip.Show("金币不足，请充值后继续！！");
                return false;
            }
            //todo if()  子弹不足 
            return _isFire || gunData.IsAuto || TargetFish!=null && TargetFish.Availability;
        }

        private Toggle _penetrateToggle;
        /// <summary>
        /// 穿透
        /// </summary>
        public void OnPenetrateClick(Toggle toggle)
        {
            _penetrateToggle = toggle;
            var isChange = toggle.isOn;
            ChangeGunPenetrate(isChange);
            if (isChange)
            {
                YxMessageTip.Show("穿透子弹可以打中一条直线上的所有鱼",4);
                CancelLockModel();
            }
        }

        public void OnCancelPenetrate()
        {
            if (_penetrateToggle == null) return;
            _penetrateToggle.isOn = false;
        }

        /// <summary>
        /// 穿透
        /// </summary>
        /// <param name="isPenetrate"></param>
        public void ChangeGunPenetrate(bool isPenetrate)
        {
            if (TheGun == null) return;
            TheGun.Data.IsPenetrate = isPenetrate;
        }

        /// <summary>
        /// 购买金币
        /// </summary>
        public void OnBuyCoinClick()
        {
        }

        /// <summary>
        /// 杀鱼
        /// </summary>
        /// <param name="hitData"></param>
        public override void KillFish(HitData hitData)
        {
            var server = App.GetRServer<FishingGameServer>();
            var bulletData = hitData.TheBulletData;
            var fishData = hitData.BeatenFishData;
            var fishId = fishData.Id;
            var bulletId = bulletData.Id;
            var fishInfo = fishData.Info;
            if (fishInfo.MaxBet > fishInfo.MinBet)
            {
                fishData.Rate = Random.Range(fishInfo.MinBet, fishInfo.MaxBet);
            }
            else
            {
                fishData.Rate = fishInfo.MinBet;
            }
            var rate = bulletData.PowerLeve > 0 ? fishData.Rate * 2 : fishData.Rate;
            server.PostHitFishValidity(fishId, bulletId, rate);
        }

        private Toggle _lockToggle;
        public void LockModel(Toggle toggle)
        {
            _lockToggle = toggle;
            var isLock = toggle.isOn;
            if (!isLock)
            {
                OnLockFire(null);
            }
            if (TheGun != null)
            {
                TheGun.ChangeLockPattern(isLock);
            }
        }

        public void CancelLockModel()
        {
            if (_lockToggle == null) return;
            _lockToggle.isOn = false;
        }
    }
}
