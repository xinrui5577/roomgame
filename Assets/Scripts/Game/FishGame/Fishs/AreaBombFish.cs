using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Fishs
{
    public class AreaBombFish : SameKindBomFish
    {
        public float AreaBombRadius = 1.6F;//如果是范围炸弹的话.爆炸的范围半径
        public int AreaBombOddLimit = 300;//炸弹倍数限制,超过倍数之后的鱼不参与赔率计算

        public override int GetFishOddBonus(Bullet bullet)
        {
            var bulletTs = bullet.transform; 
            //生成碰撞范围
            var results = Physics.SphereCastAll(bulletTs.position - Bullet.ColliderOffsetZ, AreaBombRadius, Vector3.forward);
            //计算是否爆炸 
            FishAffect = new List<Fish> { this }; 
            var oddTotalForCaclDieratio = 0;//计算死亡几率的倍率 
            foreach (var hitObj in results)//需要碰撞的鱼
            {
                var tmpFish = hitObj.transform.GetComponent<Fish>();
                if (tmpFish != null && tmpFish.Attackable && tmpFish.HittableType == HittableType.Normal)
                {
                    FishAffect.Add(tmpFish);
                    var otherOddBonus = HitProcessor.GetFishOddBonus(bullet, tmpFish, this);//用于赔率计算;
                    oddTotalForCaclDieratio += otherOddBonus;
                    tmpFish.OddBonus = oddTotalForCaclDieratio; 
                }
                if (oddTotalForCaclDieratio > AreaBombOddLimit) break;
            }
            return oddTotalForCaclDieratio;
        }

        protected override List<FishOddsData> OnBeHit(Bullet bullet, FishOddsData oddsData)
        {
            var hitProcessocr = bullet.Owner.HitProcessor;
            return hitProcessocr == null ? null : hitProcessocr.OneBulletKillFish(bullet.Score, oddsData, new List<FishOddsData>());
        }

        protected override void OnDieSkillEffect(Player killer, int bulletScore, int rate, int bulletOddsMulti, int reward)
        {
            //todo
        }
    }
}
