using System;
using System.Collections;
using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;
using Assets.Scripts.Game.Fishing.Res;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Fishing.Managers
{
    public class MapManager : MonoBehaviour
    {
        public const string MapDbAssetName = "MapDb";

        public Transform NewMapContainer;
        public Transform MapContainer;
        public Transform SeaWeveContainer;
        public Transform BubbleContainer;
        public Map CurrentMap;
        public MapDb TheMapConfig;
        public Camera ShadowCamera;
        public Shader ShadowCameraShader;
        /// <summary>
        /// 海浪
        /// </summary>
        public SeaWave SeaWave;

        public ParticleSystem Bubble;

        private Transform _transform;

        void Awake()
        {
            _transform = transform;
            InitMapDataInfo();
            InitSeaWave();
            InitBubble();
            InitShadowCamera();
            ChangeMap();
            Facade.EventCenter.AddEventListeners<EFishingEventType, Rect>(EFishingEventType.ResizeFishponBound, ResizeBound);
        }

        private void InitShadowCamera()
        {
            if(ShadowCameraShader==null)return;
            ShadowCamera.SetReplacementShader(ShadowCameraShader,"RenderType");
        }

        private void InitBubble()
        {
            var pre = ResourceManager.LoadAsset("Bubble");
            if (pre == null) return;
            var go = GameObjectUtile.Instantiate(pre, BubbleContainer);
            go.SetActive(false);
            Bubble = go.GetComponent<ParticleSystem>();
        }

        private float _interval;
        void Update()
        {
            if (Bubble == null) return;
            if (_interval > 0)
            {
                _interval -= Time.deltaTime;
            }
            else
            {
                Random.InitState((int)DateTime.Now.Ticks);
                _interval = Random.Range(8,12);
                var tsPar = Bubble.transform;
                tsPar.localPosition = new Vector3(Random.Range(_rect.xMin, _rect.xMax)
                    , Random.Range(_rect.yMin, _rect.yMax)
                    , 0);
                Bubble.Play();
                Bubble.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 创建海浪
        /// </summary>
        private void InitSeaWave()
        {
//            SeaWave = TheMapConfig.Create<SeaWave>("SeaWave");
            var pre = ResourceManager.LoadAsset("SeaWave");
            if (pre == null) return;
            var go = GameObjectUtile.Instantiate(pre, SeaWeveContainer);
            go.SetActive(false);
            SeaWave = go.GetComponent<SeaWave>();
        }

        private void InitMapDataInfo()
        {
            var pre = ResourceManager.LoadAsset(MapDbAssetName);
            if (pre == null) return;
            TheMapConfig = pre.GetComponent<MapDb>();
        }
 
        private Coroutine _changeMapCoroutine;
        /// <summary>
        /// 换图
        /// </summary>
        public void ChangeMap(int mapId = 0,Action changeAction = null)
        { 
            if (_changeMapCoroutine != null) return;
            _changeMapCoroutine = StartCoroutine(OnChangeMap(mapId, changeAction));
        }

        private IEnumerator OnChangeMap(int mapId, Action finishedChange)
        {
            //1、创建新地图(todo 协程加载)
            yield return CreateNewMap(mapId,NewMapContainer);
            yield return SeaWaveMove();
            ChangeNewMap();
            _changeMapCoroutine = null;
            if (finishedChange != null) finishedChange();
        }

        private void ChangeNewMap()
        {
            if (_newMap == null) return;
            if (CurrentMap != null)
            {
                Destroy(CurrentMap.gameObject);
                ResourceManager.UnloadBundle(App.GameKey, CurrentMap.name);
            }
            GameObjectUtile.ResetTransformInfo(_newMap, MapContainer);
            CurrentMap = _newMap.GetComponent<Map>();
        }

        /// <summary>
        /// todo 逻辑待看
        /// </summary>
        /// <returns></returns>
        private IEnumerator SeaWaveMove()
        {
            if (CurrentMap == null) yield break;
            var seaWaveTs = SeaWave.transform;
            var pos = seaWaveTs.localPosition;
            pos.x = _rect.xMax;
            seaWaveTs.localPosition = pos;
            SeaWave.BeginStart(_rect.width, 3);
            var target = _rect.xMin - SeaWave.Size;
            CurrentMap.MateriaPrivatization();
            while (seaWaveTs.localPosition.x > target)
            {
                var rate = (_rect.width/2 + seaWaveTs.localPosition.x)/ _rect.width;
                CurrentMap.SetFill(rate);
                yield return null;
            }
            SeaWave.SetActive(false);
        }

        private Rect _rect;
        protected void ResizeBound(Rect rect)
        {
            _rect = rect;
        }

        private Transform _newMap;
        /// <summary>
        /// 创建新地图
        /// </summary>
        private IEnumerator CreateNewMap(int mapId,Transform containter)
        {
            var asName = string.Format("map_{0}", mapId);
            var bundle = string.Format("maps/{0}", asName);
            var pre = ResourceManager.LoadAsset(asName, bundle);
            if(pre == null)yield break;
            var go = Instantiate(pre);
            go.name = bundle;
            _newMap = go.transform;
            GameObjectUtile.ResetTransformInfo(_newMap, containter);
            yield return null;
        }
         
        /// <summary>
        /// 创建地图
        /// </summary>
        private void CreateMap()
        {

        }
    }
}
