using System;
using System.Collections;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;
using com.yxixia.utile.Utiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Fishing.GenerateInfos
{
    public class GenerateInfo : MonoBehaviour
    {
        /// <summary>
        /// 鱼的类型
        /// </summary>
        public EFishType FishType;
        /// <summary>
        /// 间隔
        /// </summary>
        public float IntervalTime = 1f;
        /// <summary>
        /// 当前类型的Id数组
        /// </summary>
        public FishGenerator.FishInfo[] FishInfos;
        private int _totalWeight;
        private int[] _weights;

        public int GetRandomFishId()
        {
            Random.InitState((int)DateTime.Now.Ticks);
            var targetWeight = Random.Range(0, _totalWeight);
            var len = _weights.Length;
           
            for (var i = 0; i < len; i++)
            {
                if (targetWeight <= _weights[i])
                {
                    return i;
                }
            }
            return 0;
        }
        private WaitForSeconds _waitForSeconds;

        public void Init()
        {
            var totalWeight = 0;
            var fishInfos = FishInfos;
            var len = fishInfos.Length;
            var weights = _weights = new int[len];
            for (var i = 0; i < len; i++)
            {
                var fishInfo = fishInfos[i];
                var weight = fishInfo.Weight;
                totalWeight += weight;
                weights[i] = totalWeight;
            }
            _totalWeight = totalWeight;
            InitWaitForSeconds();
        }

        protected virtual void InitWaitForSeconds()
        {
            _waitForSeconds = new WaitForSeconds(IntervalTime);
        }

        public virtual FishData Generateing(FishGenerator fishGenerator)
        {
            var index = GetRandomFishId(); 
            var fishInfo = FishInfos.GetElement(index);
            var fishId = fishInfo.FishId; 
            var maxCount = fishInfo.MaxCount;
            var curCount = fishGenerator.TheFishFactory.GetFishCountById(fishId,FishType);
            if (maxCount < 1 || curCount < maxCount)
            {
                var data = new FishData
                {
                    Type = FishType,
                    FishId = fishInfo.FishId,
                    Info = fishInfo
                };
                Random.InitState((int)DateTime.Now.Ticks);
                data.Direction = Random.Range(-45f, 45f);
                Random.InitState((int)DateTime.Now.Ticks);
                data.Homeplace = Random.Range(1f, 4.999f);
                return data;
            }
            return null;
        }

        public virtual IEnumerator Wait()
        {
            yield return _waitForSeconds; 
        }

        /// <summary>
        /// 通告
        /// </summary>
        /// <param name="fishData"></param>
        /// <returns></returns>
        public virtual IEnumerator Notice(FishData fishData)
        {
            yield break;
        }
    }
}
