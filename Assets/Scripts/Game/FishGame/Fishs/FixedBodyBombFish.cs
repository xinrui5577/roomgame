using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Fishs
{
    public class FixedBodyBombFish : Fish {
         
        protected override List<FishOddsData> OnBeHit(Bullet bullet, FishOddsData oddsData)
        {
            var hitProcessocr = bullet.Owner.HitProcessor;
            if (hitProcessocr == null) return null;
            return hitProcessocr.OneBulletKillFish(bullet.Score, oddsData, new List<FishOddsData>()); 
        }

        protected override void OnHitFishEvent(Player killer, Fish fish, int bulletScore, int rate, bool isLockFishBullet, int reward)
        {
            base.OnHitFishEvent(killer, fish, bulletScore, rate, isLockFishBullet, reward);
            //触发事件
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtFreezeBombActive != null)
                gdata.EvtFreezeBombActive(); 
        }
    }
}
