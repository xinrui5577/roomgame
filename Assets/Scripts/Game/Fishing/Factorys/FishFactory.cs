using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Fishing.commons;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Fishing.Factorys
{
    /// <summary>
    /// 渔场
    /// </summary>
    public class FishFactory : MonoBehaviour
    {
        private readonly Dictionary<string, int> _fishCountDict = new Dictionary<string, int>();
        /// <summary>
        /// 
        /// </summary>
        public FishGenerator TheGenerator;
        public bool IsGenerate = true; 
        private Transform _transform;

        private readonly Dictionary<int,Fish> _fishDict = new Dictionary<int, Fish>();

        void Awake()
        {
            _transform = transform;
            var eventCenter = Facade.EventCenter;
            eventCenter.AddEventListeners<EFishingEventType, Rect>(EFishingEventType.ResizeFishponBound, ResizeBound);
            eventCenter.AddEventListeners<EFishingEventType, FishData>(EFishingEventType.CreateFish, GeneratorFish);
        }

        void Start()
        {
            InitGenerator();
        }

        /// <summary>
        /// 初始化制造者
        /// </summary>
        private void InitGenerator()
        {
            var pre = ResourceManager.LoadAsset("FishGenerator");
            if (pre == null) return;
            var go = GameObjectUtile.Instantiate(pre, _transform);
            TheGenerator = go.GetComponent<FishGenerator>();
            TheGenerator.TheFishFactory = this;
        }

        private Rect _rect;
        protected void ResizeBound(Rect rect)
        {
            _rect = rect;
        }

        /// <summary>
        /// 生产鱼阵
        /// </summary>
        public float GeneratorFormation()
        {
            if (TheGenerator == null) return 0;
            var formatName = TheGenerator.GetRandomFormat();
            if (string.IsNullOrEmpty(formatName)) return 0;
            var bdName = string.Format("fishs/{0}", formatName);
            var pre = ResourceManager.LoadAsset(formatName, bdName);
            if (pre != null)
            {
                var formationPre = pre.GetComponent<FishFormation>();
                if (formationPre != null)
                {
                    var formation = GameObjectUtile.Instantiate(formationPre, _transform);
                    return formation.SprinkleFish();
                }
            }
            return 0;
        }

        /// <summary>
        /// 生产鱼
        /// </summary>
        public void GeneratorNormal()
        {
            if (TheGenerator == null) return;
            TheGenerator.StartGenerate(); 
        }

        /// <summary>
        /// 生产一条鱼
        /// </summary>
        public void GeneratorFish(FishData data)
        { 
            var homeInfo = data.Homeplace;
            var homeType = (int)homeInfo;
            var homeRate = homeInfo - homeType;
            var homeplace = Vector3.zero;
            var angles = 0f;
            var liveArea = _rect;

            var position = data.Info.Position;
            if (position > 0)
            {
                homeType = (int)position;
                homeRate = position - homeType;
                data.Direction = data.Info.Angles;
            }
            var swimmer = CreateSwimmer(data);
            if (swimmer == null)
            {
                return;
            }
            var radius = swimmer.Radius;
            switch (homeType)
            {
                case 0://中心
                    break;
                case 1://上
                    homeplace.x = liveArea.xMin + homeRate * liveArea.width;
                    homeplace.y = liveArea.yMax + radius;
                    angles = -90+data.Direction;
                    break;
                case 2://右
                    homeplace.x = liveArea.xMax + radius;
                    homeplace.y = liveArea.yMin + homeRate * liveArea.height;
                    angles = 180+data.Direction;
                    break;
                case 3://下
                    homeplace.x = liveArea.xMin + homeRate * liveArea.width;
                    homeplace.y = liveArea.yMin - radius;
                    angles = 90+data.Direction;
                    break;
                case 4://左
                    homeplace.x = liveArea.xMin - radius;
                    homeplace.y = liveArea.yMin + homeRate * liveArea.height;
                    angles = data.Direction;
                    break;
            }
            var path = data.Info.Path;
            if (path <= 0)
            {
                Random.InitState((int) DateTime.Now.Ticks);
                path = Random.Range(0, int.MaxValue);
            }
            ResetSwimmer(swimmer, path, homeplace, angles);
            AddFish(data.FishId,data.Type);
            swimmer.RemoveEvent = RemoveFish;
        }

        public Swimmer CreateSwimmer(FishData data)
        {
            var pre = LoadFishModel(data.Type, data.FishId);
            if (pre == null) return null;
            var go = Instantiate(pre);
            if (go == null) return null;
            var fish = go.GetComponent<Fish>();
            if (fish == null) return null;
            var fishId = _curFishId++;
            fish.Data = data;
            data.Id = fishId;
            var swimmer = go.GetComponent<Swimmer>(); 
            fish.TheSwimmer = swimmer;
            _fishDict[fishId] = fish;
            return swimmer;
        }

        public void ResetSwimmer(Swimmer swimmer, long path,Vector3 pos, float angles)
        {
            var ts = swimmer.transform;
            var radius = swimmer.Radius;
            var liveArea = _rect;
            liveArea.xMin -= radius;
            liveArea.xMax += radius;
            liveArea.yMin -= radius;
            liveArea.yMax += radius;
            swimmer.Angles = angles;
            swimmer.LiveArea = liveArea;
            swimmer.Path = path;
            ts.parent = _transform;
            pos.z = 0;
            ts.localPosition = pos;
            ts.localScale = Vector3.one;
            ts.rotation = Quaternion.identity;
        }

        public static GameObject LoadFishModel(EFishType type ,int fishId)
        {
            var asName = string.Format("{0}_{1}", type, fishId);
            var bdName = string.Format("fishs/{0}", asName);
            var pre = ResourceManager.LoadAsset(asName, bdName);
            return pre;
        }

        private int _curFishId = 0;

        /// <summary>
        /// 停止出鱼
        /// </summary>
        public void StopGenerator()
        {
            if (TheGenerator == null) return;
            TheGenerator.StopGenerate();
        }

        /// <summary>
        /// 获得鱼池里的鱼 ，没有就返回空
        /// </summary>
        /// <param name="fishId"></param>
        /// <returns></returns>
        public Fish GetFish(int fishId)
        {
            return _fishDict.ContainsKey(fishId) ? _fishDict[fishId] : null;
        }

        public string GetFishKey(int fishId, EFishType type)
        {
            return string.Format("{0}_{1}", type, fishId);
        }

        public void AddFish(int fishId,EFishType type)
        {
            var key = GetFishKey(fishId,type);
            if (_fishCountDict.ContainsKey(key))
            {
                _fishCountDict[key]++;
            }
            else
            {
                _fishCountDict[key] = 1;
            }
        }


        public void RemoveFish(int fishId, EFishType type)
        {
            var key = GetFishKey(fishId, type);
            if (_fishCountDict.ContainsKey(key))
            {
                _fishCountDict[key]--;
                if (_fishCountDict[key] < 1)
                {
                    _fishCountDict.Remove(key);
                }
            }
        }

        public int GetFishCountById(int fishId, EFishType type)
        {
            var key = GetFishKey(fishId, type);
            return _fishCountDict.ContainsKey(key) ? _fishCountDict[key] : 0;
        }

    }
}
