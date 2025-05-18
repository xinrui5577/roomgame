using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.Fishing.enums;
using Assets.Scripts.Game.Fishing.Factorys;
using Assets.Scripts.Game.Fishing.GenerateInfos;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// fish的制造者
    /// </summary>
    public class FishGenerator : MonoBehaviour
    {
        public bool IsGenerate;
        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public FishFactory TheFishFactory;
        /// <summary>
        /// 
        /// </summary>
        public GenerateInfo[] GenerateInfos;

        public int FormatsInfoCount; 
        private readonly Dictionary<EFishType, Coroutine> _coroutines = new Dictionary<EFishType, Coroutine>();

        void Start()
        {
//            for (var i = 0; i < 26; i++)
//            {
//                var asn = string.Format("Fish_{0}",i);
//                var bn = string.Format("fishs/{0}", asn);
//                ResourceManager.LoadAsset(asn,bn);
//            }
        }

        /// <summary>
        /// 开始生产鱼
        /// </summary>
        public void StartGenerate()
        {
            IsGenerate = true;
            foreach (var info in GenerateInfos)
            {
                var fishType = info.FishType;
                if (!_coroutines.ContainsKey(fishType) && info.FishInfos.Length > 0)
                {
                    info.Init();
                    _coroutines[fishType] = StartCoroutine(Generateing(info));
                }
            }
        }
         
        private IEnumerator Generateing(GenerateInfo info)
        {
            var eventCenter = Facade.EventCenter;
            yield return info.Wait();
            while (IsGenerate)
            {
                var fishData = info.Generateing(this);
                if (fishData != null)
                {
                    yield return info.Notice(fishData);
                    eventCenter.DispatchEvent(EFishingEventType.CreateFish, fishData);
                }
                yield return info.Wait();
            }
           RemoveCoroutines(info.FishType);
        }

        public void RemoveCoroutines(EFishType type)
        {
            _coroutines.Remove(type);
        }

        /// <summary>
        /// 获取捕鱼数据
        /// </summary>
        /// <param name="fishId"></param>
        /// <param name="fishType"></param>
        /// <returns></returns>
        public FishInfo GetFishInfo(int fishId,EFishType fishType)
        {
            var normal = GenerateInfos.GetElement((int)fishType);
            if (normal == null) return null;
            var infos = normal.FishInfos;
            return infos.GetElement(fishId);
        }

        /// <summary>
        /// 获取鱼阵
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetFormat(int index)
        {
            return string.Format("Formation_{0}", index);
        }

        private int _lastFormatIndex;
        /// <summary>
        /// 随机获取鱼阵
        /// </summary>
        /// <returns></returns>
        public string GetRandomFormat()
        {
            if (FormatsInfoCount < 1) return null;
            Random.InitState((int)DateTime.Now.Ticks);
//            var index = Random.Range(0, FormatsInfoCount - 1) % FormatsInfoCount;
            _lastFormatIndex++;
            if (_lastFormatIndex >= FormatsInfoCount)
            {
                _lastFormatIndex = 0;
            }
            return GetFormat(_lastFormatIndex);
        }


        /// <summary>
        /// 停止出于
        /// </summary>
        public void StopGenerate()
        {
            IsGenerate = false;
            StopAllCoroutines();
            _coroutines.Clear();
        }

        [Serializable]
        public class FishInfo
        {
            /// <summary>
            /// 捕鱼的id
            /// </summary>
            public int FishId;
            /// <summary>
            /// 名称
            /// </summary>
            public string Name;
            /// <summary>
            /// 最小倍率
            /// </summary>
            public int MinBet;
            /// <summary>
            /// 最大倍率
            /// </summary>
            public int MaxBet;
            /// <summary>
            /// 
            /// </summary>
            public bool Lock = true;
            /// <summary>
            /// 权重
            /// </summary>
            public int Weight = 100;
            /// <summary>
            /// 同一时间只能出现最大数量，小于等于0表示无限制
            /// </summary>
            public int MaxCount;
            /// <summary>
            /// 指定位置，0表示随机，>0 指定位置
            /// </summary>
            public float Position;
            /// <summary>
            /// 0表示随机 ，  >0 表示指定路径
            /// </summary>
            public long Path;
            /// <summary>
            /// 角度
            /// </summary>
            public float Angles;

        } 
    }
}
