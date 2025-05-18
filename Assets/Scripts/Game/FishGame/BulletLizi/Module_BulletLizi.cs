using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.BulletLizi
{
    public class Module_BulletLizi : MonoBehaviour {

        // Use this for initialization
        void Start () {
            HitProcessor.AddFunc_Odd(Func_GetFishOddAdditive, Func_GetFishOddAdditive);
        }

        /// <summary>
        /// 获得鱼额外死亡倍率函数(包含用于死亡几率计算和用于奖励的)
        /// </summary>
        /// <param name="killer"></param>
        /// <param name="b"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        HitProcessor.OperatorOddFix Func_GetFishOddAdditive(Player killer, Bullet b, Fish f, Fish fCauser)
        {
            //if (fCauser.HittableTypeS == "Normal" && b.FishOddsMulti == 2)
            if (fCauser.HittableType != HittableType.Normal || b.FishOddsMulti != 2) return null; 
            return new HitProcessor.OperatorOddFix(HitProcessor.Operator.LastModule, 2); 
        }
    }
}
