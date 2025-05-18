    using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.FishKinds
{
    public class UniqueGenerator : FishKindGenerator
    {
        private readonly Queue<GenerateData> _generateQueue = new Queue<GenerateData>();
        private GenerateData _lastFish = null;
         
        protected override GenerateData RandomData()
        {
            if (_generateQueue.Count < 1)
            {
                var tmpFishGdLst = Datas.ToList();
                tmpFishGdLst.Sort((a, b) => Random.Range(0, a.Weight) < Random.Range(0, b.Weight) ? 1 : -1);
                var gdNum = Datas.Length; //队列70%的鱼

                foreach (var fgd in tmpFishGdLst)
                {
                    _generateQueue.Enqueue(fgd);
                    if (--gdNum < 0) break;
                }
            }
            _lastFish = _generateQueue.Dequeue();
            return _lastFish;
        }
          
        protected override bool FrontLimit()
        {
            if (_lastFish == null) return false; 
            if(_lastFish.FishPerfab == null)return false; 
            var typeDict = Generator.FishTypeIndexMap;
            var type = _lastFish.FishPerfab.TypeIndex;
            if (typeDict.ContainsKey(type))
            {
                if (typeDict[type].Count > 0) return true;
            }
            return false;
        }

        protected override bool BehindLimit(GenerateData data)
        {
            var fishPrefab = data.FishPerfab;
            var fishTypeIndexMap = Generator.FishTypeIndexMap;
            var type = fishPrefab.TypeIndex;
            var map = fishTypeIndexMap.ContainsKey(type) ? fishTypeIndexMap[type] : null;
            return map != null && map.Count > 0;
        }
    }
}
