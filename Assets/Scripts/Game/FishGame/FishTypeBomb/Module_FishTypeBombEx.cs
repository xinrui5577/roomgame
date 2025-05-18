using UnityEngine;

namespace Assets.Scripts.Game.FishGame.FishTypeBomb
{
    public class Module_FishTypeBombEx : MonoBehaviour {


        //void Start()
        //{
        //    HitProcessor.AddFunc_Odd(Func_GetFishOddAdditive, Func_GetFishOddAdditive);
        //}


        //HitProcessor.OperatorOddFix Func_GetFishOddAdditive(Player killer, Bullet b, Fish f, Fish fCauser)
        //{
        //    //todo SameTypeBomb2不应该在这里处理
        //    if (fCauser.HittableTypeS != "SameTypeBombEx")
        //        return null;


        //    FishEx_OddsMulti cpOddMulti = fCauser.GetComponent<FishEx_OddsMulti>();

        //    if (cpOddMulti == null || cpOddMulti.OddsMulti == 1)
        //        return null;

        //    return new HitProcessor.OperatorOddFix(HitProcessor.Operator.LastModule, cpOddMulti.OddsMulti);
        //}
    }
}
