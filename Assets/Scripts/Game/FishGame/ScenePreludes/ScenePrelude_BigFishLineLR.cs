using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Game.FishGame.Common.Utils;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePrelude_BigFishLineLR : FishGame.ScenePreludes.ScenePrelude
    {
        public Fish[] Prefab_Fish;

        public float EmitLineUpLimit = -1F;
        public float EmitLineDownLimit = 1F;
        public float IntervalGenerateSharkMin = 0.5F;//生成鲨鱼间隔 最少
        public float IntervalGenerateSharkMax = 1.5F;//生成鲨鱼间隔 最大
        public float AngleGenerateShark = 20F;//生成鲨鱼角度
        public float SpeedShark = 0.2F;//鲨鱼移动速度
        public float TimeEmitShark = 10F;//发射鲨鱼时间
     
        public float TimeLimit = 54F;//时间限制

        //private float mMaxSharkRadius = 0F;
        private bool mIsEnded = false;

        public override void Go()
        {
        
            StartCoroutine(_Coro_Process());
            StartCoroutine(_Coro_TimeCountdown());
        }

        void EndPrelude()
        {
            if (!mIsEnded)
            {
                mIsEnded = true;
                if (Evt_PreludeEnd != null)
                    Evt_PreludeEnd();
                GameMain.Singleton.FishGenerator.KillAllImmediate();
                Destroy(gameObject);
            }
        }

        IEnumerator _Coro_TimeCountdown()
        {
            yield return new WaitForSeconds(TimeLimit);
            EndPrelude();
        }

        IEnumerator _Coro_Process()
        {
            yield return 0; 
//         //求出最大鲨鱼半径
            //         foreach (Fish f in Prefab_Fish)
//         {
//             if (f.swimmer.BoundCircleRadius > mMaxSharkRadius)
//                 mMaxSharkRadius = f.swimmer.BoundCircleRadius;
//         }

            //随机左右边
            bool isLeft = Random.Range(0, 2) == 0;
        

            float elapse = 0F;
            int emitSharkIdx = 0;
            while (elapse < TimeEmitShark)
            {

           
                Fish shark = Instantiate(Prefab_Fish[emitSharkIdx]) as Fish;
                Rect worldDim = GameMain.Singleton.WorldDimension;
                Vector3 generatorPot = new Vector3(isLeft ? worldDim.x - shark.swimmer.BoundCircleRadius : worldDim.xMax + shark.swimmer.BoundCircleRadius//x
                                                   , Random.Range(EmitLineUpLimit,EmitLineDownLimit)//y
                                                   , 0F);
 
                emitSharkIdx = (emitSharkIdx + 1) % Prefab_Fish.Length;
                shark.ClearAi();
                shark.swimmer.Speed = SpeedShark;

                shark.transform.parent = transform;
                //shark.transform.rotation =  Quaternion.AngleAxis((isLeft?0F:180F)+Random.Range(-AngleGenerateShark,AngleGenerateShark),Vector3.forward);
                shark.transform.rotation = isLeft ? Quaternion.identity : PubFunc.RightToRotation(-Vector3.right);
                var depth = App.GetGameData<FishGameData>().ApplyFishDepth(shark.swimmer.SwimDepth);
                generatorPot.z = depth;
                shark.transform.localPosition = generatorPot;
                shark.swimmer.Go();

                float delta = Random.Range(IntervalGenerateSharkMin, IntervalGenerateSharkMax);
                elapse += delta;
                yield return new WaitForSeconds(delta);
            }

            float swimNeedTime = GameMain.Singleton.WorldDimension.width / SpeedShark;//todo 不准确
            yield return new WaitForSeconds(swimNeedTime);
 

            //等待清鱼,改用时间限制
            while (GameMain.Singleton.NumFishAlive != 0)
            {
                yield return 0;
            }

            EndPrelude();
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            //Gizmos.DrawCube(transform.position, new Vector3(GameMain.Singleton.WorldDimension.width, 1, 1));
            Gizmos.DrawLine(new Vector3(-50f, EmitLineUpLimit, -5F), new Vector3(50F + GameMain.Singleton.WorldDimension.width, EmitLineUpLimit, -5F));
            Gizmos.DrawLine(new Vector3(-50f, EmitLineDownLimit, -5F), new Vector3(50F+ GameMain.Singleton.WorldDimension.width, EmitLineDownLimit, -5F));
        }
#endif

    }
}
