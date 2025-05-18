using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.Utils;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePrelude_Road : FishGame.ScenePreludes.ScenePrelude
    { 
        public int CompositeRoadFishIndex;
        public Transform[] TsLineFish;
    
        public float Speed = 1F;

        public float RoadToLeftRightSpace = 0.1F;//辣椒到左右间距
        public float RoadFishStartOffsetY = 0.2F;//开始位置
        public float RoadFishSwimDistance = 1F;//辣椒鱼开始游的距离
        public float RoadFishSwimDistanceMax = 1.02F;//辣椒鱼开始游的距离
        public float TimeLimit = 60F;//时间限制

        private bool mIsEnded = false;

        public override void Go()
        {
            var pos = transform.position;
            pos.z = Defines.GlobleDepth_FishBase;
            transform.position = pos;

            StartCoroutine(_Coro_Process());
            StartCoroutine(_Coro_TimeCountdown());
        }

        void EndPrelude()
        {
            if (mIsEnded) return;
            mIsEnded = true;
            if (Evt_PreludeEnd != null)
                Evt_PreludeEnd();
            GameMain.Singleton.FishGenerator.KillAllImmediate();
            Destroy(gameObject);
        }

        IEnumerator _Coro_TimeCountdown()
        {
            yield return new WaitForSeconds(TimeLimit);
            EndPrelude();
        }

        public IEnumerator _Coro_GenerateRoadSideFish(Fish[,] fishUpDown, int oneSideNum)
        {
            var main = GameMain.Singleton;
            if (main == null) yield break;
            var fishGenerator = main.FishGenerator;
            if (fishGenerator == null) yield break;
            var prefabFish = fishGenerator.GetFishPrefab(CompositeRoadFishIndex);
            if (prefabFish == null) yield break;
            ////生成位置
            yield return 0;
            var generateNum = 0;
            var leftRightLimit = new Vector2(GameMain.Singleton.WorldDimension.x + RoadToLeftRightSpace, GameMain.Singleton.WorldDimension.xMax - RoadToLeftRightSpace);
            var fishStopTime = 0F;
            var gdata = App.GetGameData<FishGameData>();
            while (generateNum < oneSideNum)
            {
                var fishs = new Fish[2] { Instantiate(prefabFish), Instantiate(prefabFish)};
                var yMax = GameMain.Singleton.WorldDimension.yMax + RoadFishStartOffsetY;
                var yof =  GameMain.Singleton.WorldDimension.y - RoadFishStartOffsetY;
                for (var i = 0; i != 2; ++i)
                {
                    var fish = fishs[i];
                    var swimmer = fish.swimmer;
                    fishUpDown[i, generateNum] = fish;
                    fish.swimmer.SetLiveDimension(10F);
                    fish.ClearAi();

                    fish.transform.parent = transform;
                    var depth = gdata.ApplyFishDepth(swimmer.SwimDepth); 
                    fish.transform.localPosition = new Vector3(Random.Range(leftRightLimit.x, leftRightLimit.y), i == 0 ? yMax : yof, -depth);
                    fish.transform.right = Vector3.up * (i == 0 ? -1F : 1F);//上面

                    fish.swimmer.Go();

                    fishStopTime = Random.Range(RoadFishSwimDistance, RoadFishSwimDistanceMax) / swimmer.Speed;


                    StartCoroutine(_Coro_FishStopAfter(swimmer, fishStopTime));

                } 
                ++generateNum;
                yield return new WaitForSeconds(Random.Range(0F, 0.05F));
            }
        }
        //public 
        public IEnumerator _Coro_Process()
        {

            //辣椒鱼往中间走
            var oneSideNum = 160;
            var fishUpDown = new Fish[2,oneSideNum];
            StartCoroutine(_Coro_GenerateRoadSideFish(fishUpDown, oneSideNum)); 

            Vector3[] posLineFish = new Vector3[2] { TsLineFish[0].position, TsLineFish[1].position }; 
            posLineFish[0].x -= GameMain.Singleton.WorldDimension.width * 0.5F;
            posLineFish[1].x += GameMain.Singleton.WorldDimension.width * 0.5F;
            for (int i = 0; i != 2; ++i)
                TsLineFish[i].position = posLineFish[i];

            Transform ts;
            while (true)
            {
           
                int nullNum = 0;
                for (int i = 0; i != TsLineFish.Length; ++i)
                {
                    if (TsLineFish[i] == null)
                    {
                        ++nullNum;
                        continue;
                    }

                    ts = TsLineFish[i];
                    ts.position += ts.right * Speed * Time.deltaTime;
                    if ((ts.right.x > 0F && ts.position.x > GameMain.Singleton.WorldDimension.xMax)//向左并达到左边屏幕边
                        || (ts.right.x <= 0F && ts.position.x < GameMain.Singleton.WorldDimension.x))//向右移动并达到右边屏幕边
                    {
                        List<Fish> fishToClear = new List<Fish>();

                        foreach (Transform tChild in TsLineFish[i])
                        {
                            Fish f = tChild.GetComponent<Fish>();
                            if (f != null && f.Attackable)
                            {
                                fishToClear.Add(f);
                            }
                        }
                        foreach (Fish f in fishToClear)
                        {
                            f.Clear();
                        }

                        Destroy(TsLineFish[i].gameObject);
                        TsLineFish[i] = null;
                    }
                }

                if (nullNum == TsLineFish.Length)
                    break;

                yield return 0;
            }

            //两边路散走
            ////设置碰撞框
 
            //float fishSpeedHalf = Prefab_FishCompositeRoad.swimmer.Speed * 0.5F;
            Swimmer swimmerTmp = null;
            for (int i = 0; i != 2; ++i)
            {
                for (int j = 0; j != oneSideNum; ++j)
                {

                    if (fishUpDown[i, j] == null)
                        continue;
                    swimmerTmp = fishUpDown[i, j].swimmer;
                    swimmerTmp.SetLiveDimension(Defines.ClearFishRadius);
                    swimmerTmp.Speed = Random.Range(swimmerTmp.Speed * 0.9F, swimmerTmp.Speed * 1.5F);
                    swimmerTmp.Go();
                }
            }



            //等待清鱼,改用时间限制
            while (GameMain.Singleton.NumFishAlive != 0)
            {
                yield return 0;
            }

            EndPrelude();
        }

   
        IEnumerator _Coro_FishStopAfter(Swimmer f,float time)
        {
            yield return new WaitForSeconds(time);
            if(f != null)
                f.StopImm();
        }
        //void OnDrawGizmos()
        //{
        //    //Gizmos.DrawIcon(transform.position, "Light.tiff",true);
        //}
    }
}
