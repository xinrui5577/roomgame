using Assets.Scripts.Game.FishGame.Common.core;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.FishGame.Common.Utils;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePrelude_TwoCircleRotate : FishGame.ScenePreludes.ScenePrelude
    {
        [System.Serializable]
        public class GenerateData
        { 
            public int FishIndex;
            //public float IntervalGenerate = 0.3F;
            public float NumToGenerate = 12;//生成条数
            public float EllipseA = 0.6F;
            public float RunOutDelay = 1F;
            //public float EllipseB = 0.35F;
        }

        [System.Serializable]
        public class GenerateDatas
        {
            public GenerateData[] GDs;//一圈中列数据
        }
        [System.NonSerialized]
        public Vector3[] Locals_Center; 
        public int[] CenterFishIndexs;

        public GenerateDatas[] DataGenFishs;//圈数据

        public float OneCircleTime = 12F;//走一圈时间,,总时间是两圈半开始散开
        public float[] CenterFishRunoutTime;//中间鱼游出时间
        //public float MoGuiRunoutTime = 4F;
        public float FishRunoutSpeed = 0.6F;
        public float TimeLimit = 40F;//时间限制

        private int[] mNumLinesOneCircle;//一个圈有多少 鱼线,(数组序列是:圈序列)
        private List<Swimmer>[][] mAllSwimmer;
        private float mRunRound = 2.5F;
        private bool mIsEnded = false;

        public override void Go()
        {
            StartCoroutine(_Coro_Process());
            StartCoroutine(_Coro_TimeCountdown());
 
        }
        IEnumerator _Coro_TimeCountdown()
        {
            yield return new WaitForSeconds(TimeLimit);
            EndPrelude();
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

        public IEnumerator _Coro_Process()
        {
            yield return 0;
            //int numCircle = Prefabs_FishCenter.Length;
            var numCircle = GameMain.Singleton.ScreenNumUsing;

            mAllSwimmer = new List<Swimmer>[numCircle][];
            mNumLinesOneCircle = new int[numCircle];//圈数 
            var len = mNumLinesOneCircle.Length;
            for (var i = 0; i != len; ++i)
                mNumLinesOneCircle[i] = 4; 
            //确定位置
            Locals_Center = new Vector3[numCircle];
            for (var i = 0; i != numCircle; ++i)
            {
                var center = Locals_Center[i];
                center.x = GameMain.Singleton.WorldDimension.x + GameMain.Singleton.WorldDimension.width / numCircle * (0.5F + i);

                center.y = 0F;
                center.z = Defines.GMDepth_Fish;// Defines.GMDepth_Fish;
                //Debug.Log(Locals_Center[i]);
            }


            for (var circleIdx = 0; circleIdx < numCircle; circleIdx++)
            {

                StartCoroutine(_Coro_CenterFishGenerateAndMove(circleIdx));
                var numLinesOcc = mNumLinesOneCircle[circleIdx];
                var lineList = new List<Swimmer>[numLinesOcc];;
                mAllSwimmer[circleIdx] = lineList;
                for (var j = 0; j < numLinesOcc; j++)
                {
                    lineList[j] = new List<Swimmer>();
                }

                for (var i = 0; i != mNumLinesOneCircle[circleIdx]; ++i)
                {
                    StartCoroutine(_Coro_GenerateFish(circleIdx, i));
                    StartCoroutine(_Coro_FishMoveProcess(circleIdx, i));

                }

            }

            yield return 0;
    
            //等待清鱼//改用时间限制
            while (GameMain.Singleton.NumFishAlive != 0)
            {
                yield return 0;
            }

            EndPrelude();
        }

        IEnumerator _Coro_CenterFishGenerateAndMove(int circleIdx)
        {
            var main = GameMain.Singleton;
            if (main == null) yield break;
            var fishGenerator = main.FishGenerator;
            if (fishGenerator == null) yield break;
            var fishIndex = CenterFishIndexs[circleIdx];
            var prefabFish = fishGenerator.GetFishPrefab(fishIndex);
            if(prefabFish == null)yield break;

            var elapse = 0F;
            var rotateSpeed = 6.283185F / OneCircleTime;//弧度/s
            var useTime = OneCircleTime * mRunRound + CenterFishRunoutTime[circleIdx];

            var fishCenter = Instantiate(prefabFish);
            var centerSwimmer = fishCenter.swimmer;
            fishCenter.ClearAi();
            fishCenter.transform.parent = transform;
            fishCenter.transform.localPosition = new Vector3(Locals_Center[circleIdx].x,Locals_Center[circleIdx].y,Locals_Center[circleIdx].z-0.1F);
            fishCenter.transform.localRotation = Quaternion.identity;
         
            centerSwimmer.RotateSpd = 6.283185F; 
            while (elapse < useTime)
            {

                if (centerSwimmer == null || !fishCenter.Attackable) break;//暂时用attackable来确定鱼死亡 
                //中间鱼转动
                centerSwimmer.transform.localRotation = Quaternion.AxisAngle(Vector3.forward, centerSwimmer.RotateSpd);
                centerSwimmer.RotateSpd -= rotateSpeed * Time.deltaTime;

                elapse += Time.deltaTime;
                yield return 0;
            }
            if (centerSwimmer != null)
            {
                centerSwimmer.Speed = FishRunoutSpeed * 0.7F;
                centerSwimmer.Go();
            }
        }
        IEnumerator _Coro_FishMoveProcess(int circleIdx,int lineIdx)
        {
            //鱼群旋转\
            var gd = DataGenFishs[circleIdx].GDs[lineIdx];
            var elapse = 0F;
            var rotateSpeed = 6.283185F / OneCircleTime;//弧度/s
            var useTime = OneCircleTime * mRunRound + gd.RunOutDelay;
            var gdata = App.GetGameData<FishGameData>();
            while (elapse < useTime)
            {

                foreach (var s in mAllSwimmer[circleIdx][lineIdx])
                {
                    if (s == null)
                        continue;
                    var depth = gdata.ApplyFishDepth(s.SwimDepth);
                    //使用旋转速度记录当前已转动角度
                    s.transform.localPosition = 
                        new Vector3(Locals_Center[circleIdx].x + gd.EllipseA * Mathf.Cos(s.RotateSpd)
                                    , Locals_Center[circleIdx].y + gd.EllipseA * 0.75F * Mathf.Sin(s.RotateSpd)
                                    , -depth);

                    s.transform.localRotation = Quaternion.AngleAxis(-90F + s.RotateSpd * Mathf.Rad2Deg, Vector3.forward);

                    s.RotateSpd -= rotateSpeed * Time.deltaTime;

                }
                elapse += Time.deltaTime;
                yield return 0;
            }

        
            //云群散开
            foreach (var s in mAllSwimmer[circleIdx][lineIdx])
            {
                if (s == null) continue;
                s.Speed = FishRunoutSpeed;
                s.Go();
            }
        }

        IEnumerator _Coro_GenerateFish(int circleIdx, int lineIdx)
        {
            var main = GameMain.Singleton;
            if (main == null) yield break;
            var fishGenerator = main.FishGenerator;
            if (fishGenerator == null) yield break;  

            var gd = DataGenFishs[circleIdx].GDs[lineIdx];
            var prefabFish = fishGenerator.GetFishPrefab(gd.FishIndex);
            if(prefabFish == null) yield break;
        
            var depthFish = 0F;


            var elapse = 0F;
            var numToGenerate = gd.NumToGenerate;
            var generateInterval = OneCircleTime/numToGenerate;//为了不重叠生成多一条

            //生成条数
            for (var i = 0; i < numToGenerate; ++i) 
            {
                var f = Instantiate(prefabFish);
                var s = f.swimmer;
                f.ClearAi();
             
                f.AniSprite.PlayFrom(f.AniSprite.DefaultClip, elapse);
             
                //s.RotateSpd = curRotateRad;//使用旋转速度记录当前已转动角度,转向:360到0度 
                s.RotateSpd = 6.283185F;

                s.transform.parent = transform; 

                s.transform.localPosition = Locals_Center[circleIdx]+new Vector3(gd.EllipseA, 0F, depthFish);

                depthFish -= 0.005F; 
                s.transform.rotation = Quaternion.AngleAxis(-90F, Vector3.forward);
             
                mAllSwimmer[circleIdx][lineIdx].Add(s);

                elapse += generateInterval;
                yield return new WaitForSeconds(generateInterval);
            }
        }
    }
}
