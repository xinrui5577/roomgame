using Assets.Scripts.Game.FishGame.Common.Brains.FishAI;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.FishKinds;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    /// <summary>
    /// 鱼队列生成
    /// </summary>
    /// <remarks>自动删除</remarks>
    [System.Serializable]
    public class FishEmitter_Queue : GroupGenerateData
    {
        public float Distance = 0.2F; //生成距离
        public int NumMax = 3; //最多生成个数
        public int NumMin = 2; //最少生成个数

        public float Fish_Speed = 0.5F;
        public float Fish_RotateSpd = 90F; //角度/秒 

        //Ai
        public float Fish_RotateAngleRndRange = 30F;
        public float Fish_RotateInterval = 5F; //转向间隔
        public float Fish_RotateIntervalRndRange = 1F; //转向随机范围

        private void Awake()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtStopGenerateFish += Handle_StopGenerateFish;
            name = "QueueHeader_" + FishPerfab.name;
        }

        private void OnDestroy()
        {
            var gdata = App.GetGameData<FishGameData>();
            if (gdata == null) return;
            if (gdata.EvtStopGenerateFish != null) gdata.EvtStopGenerateFish -= Handle_StopGenerateFish;
        }

        private void Handle_StopGenerateFish()
        {
            StopCoroutine("_Coro_Generate");
            Destroy(gameObject);
        }

        protected override IEnumerator OnGenerate()
        {
            var num = Random.Range(NumMin, NumMax + 1); 
            var headSwimmer = gameObject.AddComponent<Swimmer>();
            gameObject.AddComponent<DestroyWhenSwimmerOut>();
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtLeaderInstance != null)
                gdata.EvtLeaderInstance(headSwimmer);

            FishPerfab.swimmer.CopyDataTo(headSwimmer);
            headSwimmer.SetLiveDimension(Defines.ClearFishRadius);
            headSwimmer.RotateSpd = Fish_RotateSpd;
            headSwimmer.Speed = Fish_Speed;

            var fishAiLeader = gameObject.AddComponent<FishAI_FreeSwimSingle>();
            FishPerfab.GetComponent<FishAI_FreeSwimSingle>().CopyDataTo(fishAiLeader);
            fishAiLeader.enabled = false;
            fishAiLeader.RotateAngleRndRange = Fish_RotateAngleRndRange;
            fishAiLeader.RotateInterval = Fish_RotateInterval;
            fishAiLeader.RotateIntervalRndRange = Fish_RotateIntervalRndRange;
            var hole = KindGenerator.CreateBirthHole(headSwimmer.BoundCircleRadius, headSwimmer.SwimDepth);
            transform.localPosition = hole.Position;
            transform.rotation = hole.Rotation;
            headSwimmer.Go();

            Fish lastFish = null;
            var distanceToLeader = 0F;
            for (var i = 0; i < num; i++)
            {
                yield return new WaitForSeconds((Distance + headSwimmer.BoundCircleRadius*2F)/headSwimmer.Speed);
                var f = Instantiate(FishPerfab);
                var s = f.swimmer;
                f.name = FishPerfab.name;

                s.RotateSpd = Fish_RotateSpd;
                s.Speed = Fish_Speed;
                //动画设置
                if (lastFish != null)
                {
                    f.AniSprite.PlayFrom(f.AniSprite.DefaultClip, lastFish.AniSprite.ClipTimeSeconds);
                }
                //删除所有其他ai
                f.ClearAi();
                var aiFollow = f.gameObject.AddComponent<FishAI_Follow>();
                aiFollow.SetTarget(headSwimmer);
                distanceToLeader += (Distance + headSwimmer.BoundCircleRadius*2F);
                aiFollow.DistanceToLeader = distanceToLeader;

                //方位设置
                var fishTs = f.transform;
                fishTs.parent = GameMain.Singleton.FishGenerator.transform;
                fishTs.transform.parent = hole.Parent;
                fishTs.transform.localPosition = hole.Position;
                fishTs.transform.localRotation = hole.Rotation;
                s.Go();
                lastFish = f;
            }
            fishAiLeader.enabled = true;
        }
    }
}
