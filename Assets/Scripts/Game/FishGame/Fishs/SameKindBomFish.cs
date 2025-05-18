using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Fishs
{
    public class SameKindBomFish : Fish
    {
        protected List<Fish> FishAffect;

        public Fish[] Prefab_SameTypeBombAffect;//如果是同类鱼炸弹,是炸那个类型的
        public override int GetFishOddBonus(Bullet bullet)
        { 
            var prefabAffects = Prefab_SameTypeBombAffect;
            var numTypeToBomb = prefabAffects.Length;
            var fishDicts = new Dictionary<int, Fish>[numTypeToBomb];
            var fishTypeIndexMap = GameMain.Singleton.FishGenerator.FishTypeIndexMap;
            for (var i = 0; i < numTypeToBomb; ++i)
            {
                var type = prefabAffects[i].TypeIndex;
                if (!fishTypeIndexMap.ContainsKey(type)) continue;
                fishDicts[i] = fishTypeIndexMap[type];
            }
            //计算是否爆炸
            FishAffect = new List<Fish> { this };

            OddBonus = HitProcessor.GetFishOddBonus(bullet, this, this);
            var oddTotalForCaclDieratio = OddBonus;//fishFirst.Odds * oddMulti + GetFishOddForDieRatio(bulletOwner, b, fishFirst);//用于赔率计算

            foreach (var fishDict in fishDicts)
            {
                if (fishDict == null) continue;
                foreach (var fKv in fishDict)
                {
                    var tmpFish = fKv.Value;
                    if (!tmpFish.Attackable || tmpFish.HittableType != HittableType.Normal) continue;
                    FishAffect.Add(tmpFish);

                    var otherOddBonus = HitProcessor.GetFishOddBonus(bullet, tmpFish, this);
                    tmpFish.OddBonus = otherOddBonus;
                    oddTotalForCaclDieratio += otherOddBonus;
                }
            }
            return oddTotalForCaclDieratio;
        }

        protected override List<FishOddsData> OnBeHit(Bullet bullet, FishOddsData oddsData)
        {
            var hitProcessocr = bullet.Owner.HitProcessor;
            return hitProcessocr == null ? null : hitProcessocr.OneBulletKillFish(bullet.Score, oddsData, new List<FishOddsData>());
        }

        protected override void OnDieSkillEffect(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, int reward)
        {  
            //杀死其他鱼
            foreach (var fDie in FishAffect)
            {
                if (!fDie.Attackable) continue;
                PlayFlashEffect(transform.position, fDie);
                fDie.Kill(killer, bulletScore, fDie.OddBonus, bulletOddsMulti, 1.5f,0);//fishOddBonus
            } 
        }
          
        protected override void OnHitFishEvent(Player killer, Fish fish, int bulletScore, int rate,bool isLockFishBullet, int reward)
        {
            base.OnHitFishEvent(killer, fish, bulletScore, rate, isLockFishBullet,reward);
            //触发事件
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtSameTypeBombKiled != null)
            {
                var temp = bulletScore * rate;
                var getReward = reward > 0 ? reward : temp;
                if (reward>0 && reward != temp)
                {
                    YxDebug.LogWrite("SameKindBomFish   显示{0}【计算{1}】", null, reward, temp);
                }
                gdata.EvtSameTypeBombKiled(killer, getReward); 
            }
        }

        /// <summary>
        /// 播放闪电链等相似的特效
        /// </summary>
        /// <param name="position"></param> 
        /// <param name="targetFish"></param>
        public static void PlayFlashEffect(Vector3 position, Fish targetFish)
        {
            var gm = GameMain.Singleton;
            var perfabEff = gm.Ef_FishDieEffectAdd.PerfabSameTypeEffect;
            if (perfabEff == null) return;
            var eff = Instantiate(perfabEff);
            eff.transform.parent = GameMain.Singleton.PopupDigitFishdie.transform;
            eff.ThunderLightSize = targetFish.GetComponent<Swimmer>().BoundCircleRadius;
            position.z = 610;
            var targetPos = targetFish.transform.position;
            targetPos.z = 610;
            eff.Play(position, targetPos);
        }
    }
}
