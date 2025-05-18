using System.Collections;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.FishGenereate;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.FishKinds
{
    public abstract class KindGenerator : MonoBehaviour{
        [System.NonSerialized]
        public FishGenerator Generator;

        public bool IsRun;
        /// <summary>
        /// 间隔
        /// </summary>
        public RandomFloat Interval;
        /// <summary>
        /// 开始id
        /// </summary>
        public int BeginId = 0;
        /// <summary>
        /// 数据个数
        /// </summary>
        protected int Count = 0;
        /// <summary>
        /// 总权重
        /// </summary>
        protected int WeightTotal;

        public abstract void Init();

        public void StartGenerate(FishGenerator generator)
        { 
            Generator = generator;
            IsRun = true;
        }

        public void Stop()
        { 
            IsRun = false;
        }

        /// <summary>
        /// 生成鱼前限制
        /// </summary> 
        /// <returns></returns>
        protected virtual bool FrontLimit()
        {
            return false;
        }

        public abstract IEnumerator OnGenerate();

        /// <summary>
        /// 设置生产位置
        /// </summary>   
        /// <param name="bound"></param>
        /// <param name="depth"></param> 
        public virtual BirthHole CreateBirthHole(float bound, float depth)
        {
            var hole = new BirthHole();
            Vector2 interectPot;
            Vector2 rndPos = Random.onUnitSphere;
            Generator.InterectWorldRectWithOriginRay(rndPos, out interectPot);
            Vector3 posInstance = interectPot + rndPos * (bound);
            hole.Parent = Generator.transform; 
            var z = App.GetGameData<FishGameData>().ApplyFishDepth(depth);
            posInstance.z = z;
            hole.Position = posInstance;
            hole.Rotation = PubFunc.RightToRotation(-rndPos + new Vector2(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f))); 
            return hole;
        } 
    }

    [System.Serializable]
    public class RandomFloat
    {
        public float Min;
        public float Max = 2F;
        public float Value
        {
            get { return Random.Range(Min, Max); }
        }
    }

    public class BirthHole
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Transform Parent;
    }
}
