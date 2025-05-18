using System.Collections;
using Assets.Scripts.Game.FishGame.Common.Brains.FishAI;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.FishGenereate;
using Assets.Scripts.Game.FishGame.FishKinds;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class FishEmitter_Flock : GroupGenerateData
    {

        public Fish Prefab_Fish;
     
        public float Radius = 384F;//生成半径
        public int NumMax = 9;//最多生成个数
        public int NumMin = 4;//最少生成个数 

        private void Awake()
        {
            gameObject.name = "FlockHeader_" + Prefab_Fish.name;
        }

        protected override IEnumerator OnGenerate()
        { 
            var num = Random.Range(NumMin, NumMax + 1);
            var headSwimmer = gameObject.AddComponent<Swimmer>();
            var gdata = App.GetGameData<FishGameData>();
            if (gdata.EvtLeaderInstance != null)
                gdata.EvtLeaderInstance(headSwimmer);
            gameObject.AddComponent<DestroyWhenSwimmerOut>();
            Prefab_Fish.swimmer.CopyDataTo(headSwimmer);
            headSwimmer.gameObject.AddComponent<FishDimenSetWhenEnterWorld>();
            headSwimmer.SetLiveDimension(Radius / headSwimmer.BoundCircleRadius * 2F);
            var hole = KindGenerator.CreateBirthHole(Radius, headSwimmer.SwimDepth);
            SetSwimmer(headSwimmer, hole);
            headSwimmer.Go();

            for (var i = 0; i < num; i++) 
            {
                var f = Instantiate(Prefab_Fish);
                var s = f.swimmer; 
                f.name = Prefab_Fish.name;
                //删除所有其他ai
                f.ClearAi();
                var aiFollow = f.gameObject.AddComponent<FishAI_Flock>();
                aiFollow.SetLeader(headSwimmer);
                SetSwimmer(s, hole);
                s.Go(); 
            }
            yield break;
        }

        private void SetSwimmer(Swimmer swimmer,BirthHole hole)
        {
            swimmer.gameObject.AddComponent<FishDimenSetWhenEnterWorld>();
            swimmer.SetLiveDimension(Radius / swimmer.BoundCircleRadius * 2F);
            var tsSwimmer = swimmer.transform;
            tsSwimmer.parent = hole.Parent;
            Vector3 localPos = Random.insideUnitCircle * (Radius - swimmer.BoundCircleRadius);
            localPos = hole.Position + localPos;  
            tsSwimmer.localPosition = localPos;
            tsSwimmer.rotation = hole.Rotation;
        } 
    }
}
