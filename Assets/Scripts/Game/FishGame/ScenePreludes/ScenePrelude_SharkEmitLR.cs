using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using Assets.Scripts.Game.FishGame.Common.Utils;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePrelude_SharkEmitLR : FishGame.ScenePreludes.ScenePrelude
    {  
        public int[] FishIndexs;
        public int SpecialFishIndex;
 
        public float IntervalGenerateSharkMin = 0.5F;//生成鲨鱼间隔 最少
        public float IntervalGenerateSharkMax = 1.5F;//生成鲨鱼间隔 最大
        public float AngleGenerateShark = 20F;//生成鲨鱼角度
        public float SpeedShark = 0.2F;//鲨鱼移动速度
        public float TimeEmitShark = 10F;//发射鲨鱼时间

        public float EmitLaJiaoDelay = 1F;//出辣椒鱼延迟
        public float IntervalEmitLaJiao = 1F;//出辣椒鱼延迟
        public float RoadToLeftRightSpace = 0.1F;//辣椒鱼与左右间距
        public float SpeedLaJiao = 0.3F;//辣椒移动速度
        public float TimeLimit = 54F;//时间限制

        //private float mMaxSharkRadius = 0F;
        private bool mIsEnded = false;
        private float mDepthOffset;
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
            var main = GameMain.Singleton;
            if (main == null)
            { 
                StartCoroutine(_Coro_WaitNullFish());
                yield break;
            }
            var fishGenerator = main.FishGenerator;
            if (fishGenerator == null)
            { 
                StartCoroutine(_Coro_WaitNullFish());
                yield break;
            }    
            yield return 0;
            StartCoroutine("_Coro_LaJiaoGenerating"); 
        
            //随机左右边
            var isLeft = Random.Range(0, 2) == 0;
        

            var elapse = 0F;
            var emitSharkIdx = 0;
            var len = FishIndexs.Length;
            while (elapse < TimeEmitShark)
            {
                var index = FishIndexs[emitSharkIdx];
                var prefabFish = fishGenerator.GetFishPrefab(index);
                CreatFish(prefabFish,isLeft);

                emitSharkIdx = (emitSharkIdx + 1) % len;
                var delta = Random.Range(IntervalGenerateSharkMin, IntervalGenerateSharkMax);
                elapse += delta;
                yield return new WaitForSeconds(delta);
            }

            var swimNeedTime = GameMain.Singleton.WorldDimension.width / SpeedShark;//todo 不准确
            yield return new WaitForSeconds(swimNeedTime);

            StopCoroutine("_Coro_LaJiaoGenerating");
            StartCoroutine(_Coro_WaitNullFish()); 
        }

        public void CreatFish(Fish prefabFish, bool isLeft)
        {
            if (prefabFish == null) return;
            var shark = Instantiate(prefabFish);
            var swimmer = shark.swimmer;
            var sharkTs = shark.transform;
            var generatorPot = new Vector3(isLeft ? GameMain.Singleton.WorldDimension.x - shark.swimmer.BoundCircleRadius
                                               : GameMain.Singleton.WorldDimension.xMax + shark.swimmer.BoundCircleRadius, 0F, 0F); 
            shark.ClearAi();
            swimmer.Speed = SpeedShark;

            sharkTs.parent = transform;
            sharkTs.rotation = Quaternion.AngleAxis((isLeft ? 0F : 180F) + Random.Range(-AngleGenerateShark, AngleGenerateShark), Vector3.forward);
            generatorPot.z = -swimmer.SwimDepth;
            shark.transform.localPosition = generatorPot;

            swimmer.Go();
        }

        IEnumerator _Coro_LaJiaoGenerating()
        {
            var main = GameMain.Singleton;
            if (main == null) yield break;
            var fishGenerator = main.FishGenerator;
            if (fishGenerator == null) yield break; 
            yield return new WaitForSeconds(EmitLaJiaoDelay);
            var leftRightLimit = new Vector2(GameMain.Singleton.WorldDimension.x + RoadToLeftRightSpace, GameMain.Singleton.WorldDimension.xMax - RoadToLeftRightSpace);

            var prefabFish = fishGenerator.GetFishPrefab(SpecialFishIndex);
            var gdata = App.GetGameData<FishGameData>();
            while (true)
            {
                var fishes = new Fish[2] { Instantiate(prefabFish), Instantiate(prefabFish)}; 
                for (var i = 0; i != 2; ++i)
                {
                    var fish = fishes[i];
                    var swimmer = fish.swimmer;
                    var fishTs = fish.transform;
                    var radius = fish.swimmer.BoundCircleRadius;
                    fish.ClearAi();
                    swimmer.Speed = SpeedLaJiao;
                    fishTs.parent = transform;
                    var depth = gdata.ApplyFishDepth(swimmer.SwimDepth);
                    fishTs.localPosition = new Vector3(Random.Range(leftRightLimit.x, leftRightLimit.y)
                                                       , i == 0 ? (GameMain.Singleton.WorldDimension.yMax + radius) : (GameMain.Singleton.WorldDimension.y - radius), -depth); 
                    fishTs.right = Vector3.up * (i == 0 ? -1F : 1F);//上面
                    swimmer.Go(); 
                } 
                yield return new WaitForSeconds(IntervalEmitLaJiao);
            }
        }

        public IEnumerator _Coro_WaitNullFish()
        {
            while (GameMain.Singleton.NumFishAlive != 0)
            {
                yield return 0;
            } 
            EndPrelude();
        }
    }
}
