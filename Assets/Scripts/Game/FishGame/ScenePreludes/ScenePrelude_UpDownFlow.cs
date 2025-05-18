using Assets.Scripts.Game.FishGame.FishGenereate;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.Utils;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePrelude_UpDownFlow : FishGame.ScenePreludes.ScenePrelude {
        [System.Serializable]
        public class GenerateData
        { 
            public int FishIndex;
            public float Delay = 0F;//开始生成延迟
            public float Elapse = 2F;// 持续时间
            public float IntervalGenerate = 0.2F;//生成间隔
            public bool IsUp = true;
        }

        public GenerateData[] GenData;
        public float TimeLimit = 60;
        public float FishMoveSpd = 0.4F;
        public float SpaceToLeftRight = 0.5F;
        //public GenerateData[] Down;

        private static List<float> LstRndLineX;
        private Stack<float> mRndLineX;
    
        //private FishGenerator mFishGenerator;
        private bool mIsEnded = false;


        private static int NumRndLineOneScreen = 10;//一个屏幕生成随机线数目
        private static Quaternion TowardUpRotation;
        private static Quaternion TowardDownRotation;
        private static int NumScreen;
        private static float RndOffsetX = 0.1F;//在随机线的基础上再偏移一个随机位置:+/-RndOffsetX
    
        IEnumerator _Coro_TimeCountdown()
        {
            yield return new WaitForSeconds(TimeLimit);
            EndPrelude();
        }
        IEnumerator _Coro_WaitFishAllDie()
        {
            while (GameMain.Singleton.NumFishAlive <= 2) yield return 0;

            while (GameMain.Singleton.NumFishAlive != 0) yield return 0; 

            EndPrelude();
        }

        private void EndPrelude()
        {
            if (mIsEnded) return;
            mIsEnded = true;
            if (Evt_PreludeEnd != null)
                Evt_PreludeEnd();
            GameMain.Singleton.FishGenerator.KillAllImmediate();
            Destroy(gameObject);
        }



        void Awake()
        { 
            TowardUpRotation = Quaternion.AngleAxis(90F, Vector3.forward);
            TowardDownRotation = Quaternion.AngleAxis(-90F, Vector3.forward);
            NumScreen =   GameMain.Singleton.ScreenNumUsing;

            LstRndLineX = new List<float>();
            mRndLineX = new Stack<float>();

            var space = (GameMain.Singleton.WorldDimension.width - SpaceToLeftRight * 2F) / (NumRndLineOneScreen * NumScreen);
            RndOffsetX = space * 0.5F;

            for (int i = 0; i != NumScreen * NumRndLineOneScreen; ++i)
            {
                LstRndLineX.Add(GameMain.Singleton.WorldDimension.x + SpaceToLeftRight + space * (0.5F + i));
            }

            LstRndLineX.Sort((float a, float b) => { return Random.Range(0, 3) - 1; });

            foreach (float val in LstRndLineX)
            {
                mRndLineX.Push(val);
            }
 
        }
        float GetRndLineX()
        {
            if (mRndLineX.Count == 0)
            {
                LstRndLineX.Sort((a, b) => Random.Range(0, 3) - 1);

                foreach (var val in LstRndLineX)
                {
                    mRndLineX.Push(val);
                }
            } 
            return mRndLineX.Pop() + Random.Range(-RndOffsetX, RndOffsetX);
        }

        private FishGenerator _fishGenerator;
        public override void Go()
        {
            var main = GameMain.Singleton;
            if (main == null) return;
            _fishGenerator = main.FishGenerator;
            if (_fishGenerator == null) return;  

            foreach(var gd in GenData)
            {
                StartCoroutine(_Coro_GenerateDelay(gd));
            }

            StartCoroutine(_Coro_TimeCountdown());
            StartCoroutine(_Coro_WaitFishAllDie());
        }
      
        IEnumerator _Coro_GenerateDelay(GenerateData gd)
        { 
            yield return new WaitForSeconds(gd.Delay); 
            var worldDim = GameMain.Singleton.WorldDimension;
            var elapse = 0F;
            var prefabFish = _fishGenerator.GetFishPrefab(gd.FishIndex);
            if (prefabFish == null) yield break;

            var gdata = App.GetGameData<FishGameData>();

            while (elapse < gd.Elapse)
            {

                var f = Instantiate(prefabFish);
                var s = f.swimmer;
                var fTs = f.transform;
                fTs.parent = transform;
                var depth = gdata.ApplyFishDepth(s.SwimDepth);
                f.transform.localPosition = new Vector3(GetRndLineX()
                                                        ,gd.IsUp? (worldDim.yMax + s.BoundCircleRadius) :(worldDim.yMin - s.BoundCircleRadius)
                                                        , -depth);
                fTs.localRotation = gd.IsUp ? TowardDownRotation : TowardUpRotation;
                f.ClearAi();
                s.Speed = FishMoveSpd;
                s.Go();
            
                elapse += gd.IntervalGenerate / NumScreen;
                yield return new WaitForSeconds(gd.IntervalGenerate / NumScreen);
            }
        }

    }
}
