using System.Collections;
using Assets.Scripts.Game.lswc.Core;
using System.Collections.Generic;
using UnityEngine;
using com.yxixia.utile.YxDebug; 

namespace Assets.Scripts.Game.lswc.Manager
{
    public class LSResourseManager : InstanceControl
    {
        private Dictionary<string, Sprite> _spriteCache;

        private Dictionary<string, Material> _materialCache;

        private LoadState _state = LoadState.BeginLoad;

        public Material[] Materials;

        public Sprite[] Sprites;

        public GameObject[] Prefab;

        public delegate void ResourseLoadState();

        [HideInInspector]
        public ResourseLoadState OnLoadBegin;

        [HideInInspector]
        public ResourseLoadState OnLoad;

        [HideInInspector]
        public ResourseLoadState OnLoadFinished;

        private float beginTime;

        private void Awake()
        {
            InitReousrse();
        }

        public void InitReousrse()
        {
            LoadBegin(); 
            LoadResourse();
        }

        private void LoadBegin()
        {
            beginTime = Time.realtimeSinceStartup;
            if (OnLoadBegin != null)
            {
                OnLoadBegin();
            }
        }

        private void LoadResourse()
        {
            _state = LoadState.OnLoad;
            if (OnLoad != null)
            {
                OnLoad();
            }
            _spriteCache = new Dictionary<string, Sprite>();
            _materialCache = new Dictionary<string, Material>();
            StartCoroutine(LoadSpriteResourse());
            StartCoroutine(LoadMaterialResourse());
            _state = LoadState.FinishLoad;
        }
        #region 本来统一用Object存所有的引用对象，但是考虑到装箱影响效率，又写回来了

        private IEnumerator LoadSpriteResourse()
        {
            foreach (var sprite in Sprites)
            {
                if (_spriteCache.ContainsKey(sprite.name))
                {
                    continue;
                }
                _spriteCache.Add(sprite.name, sprite);
            }
            yield return _spriteCache;
        }

        private IEnumerator LoadMaterialResourse()
        {
            foreach (var material in Materials)
            {
                if (_materialCache.ContainsKey(material.name))
                {
                    continue;
                }
                _materialCache.Add(material.name, material);
            }
            yield return _materialCache;
        }
        #endregion

        private void Update()
        {
            if (_state == LoadState.FinishLoad)
            {
                YxDebug.LogWarning("加载总时间是: " + (Time.realtimeSinceStartup - beginTime));
                _state = LoadState.Null;
                YxDebug.LogWarning("图片文件数量是: " + _spriteCache.Count);
                YxDebug.LogWarning("材质文件数量是: " + _materialCache.Count);
                if (OnLoadFinished != null)
                {
                    OnLoadFinished();
                }
            }
        }

        public Material GetMaterial(string materialName)
        {
            if(_materialCache.ContainsKey(materialName))
            {
                return _materialCache[materialName];
            }
            else
            {
                YxDebug.LogError("Material " + materialName + " is not exist");
                return null;
            }
        }
          
        public Sprite GetSprite(string spriteName)
        {
            if (_spriteCache.ContainsKey(spriteName))
            {
                return _spriteCache[spriteName];
            }
            else
            {
                YxDebug.LogError("Sprite " + spriteName + " is not exist");
                return null;
            }
        }

        public override void OnExit()
        {
        }
    }

    public enum ResourseType
    {
        Audio = 0,
        Sprite,
        Material,
    }

    public enum LoadState
    {
        BeginLoad,
        OnLoad,
        FinishLoad,
        Null,
    }

}
