using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using YxFramwork.Common;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.FishKinds
{
    [System.Serializable]
    public class GenerateData
    {
        /// <summary>
        /// 标识
        /// </summary>
       [System.NonSerialized]
        public int Id; 
        /// <summary>
        /// 权重
        /// </summary>
        public int Weight = 100;
        /// <summary>
        /// 命中权重 
        /// </summary>
        [System.NonSerialized]
        public int HittedWeight;
        /// <summary>
        /// 鱼的资源
        /// </summary>
        public Fish FishPerfab;
        [System.NonSerialized]
        public KindGenerator KindGenerator;
        /// <summary>
        /// 同一时间只能出现最大数量
        /// </summary>
        public int MaxCount;
        
        /// <summary>
        /// 鱼的背景
        /// </summary>
        public GameObject FishBack;
        public virtual void OnGenerate(GameObject background=null)
        {
            var globData = App.GetGameData<FishGameData>();
            if (globData == null) return;
            var type = FishPerfab.TypeIndex;
            var curcount = globData.FishCount(type);
            if (MaxCount>0 && curcount >= MaxCount) return;
            globData.AddFish(type);
            var f = Object.Instantiate(FishPerfab);
            f.Back = background;
            var swimmer = f.swimmer;
            var fishTs = f.transform;
            f.name = FishPerfab.name;
            var hole = KindGenerator.CreateBirthHole(swimmer.BoundCircleRadius, swimmer.SwimDepth);
            fishTs.parent = hole.Parent;
            fishTs.localPosition = hole.Position;
            fishTs.rotation = hole.Rotation;
            swimmer.Go(); 
        } 
    }
}
