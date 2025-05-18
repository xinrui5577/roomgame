using System;
using Assets.Scripts.Game.FishGame.Utils;
using UnityEngine;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePreludeManager : MonoBehaviour
    { 
        public bool IsRandomPrelude = true;//是否随机鱼阵
        public int PreludeIdxStart = 0;//开始的idx  
        [NonSerialized]
        public FishGame.ScenePreludes.ScenePrelude CurPrelude;
    
        public PreludeConfig Config;

        private void Start()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (Config != null) return;
            var asset = ResourceManager.LoadAsset("PreludesConfig");
            InitConfig(asset);
        }

        private void InitConfig(Object asset)
        { 
            if (asset == null)
            {
                YxDebug.LogError("PreludeConfig no config");
                return;
            }
            YxDebug.Log("Load Finished PreludeConfig config");
            var go = asset as GameObject;
            if (go != null) Config = go.GetComponent<PreludeConfig>();
        }

        public FishGame.ScenePreludes.ScenePrelude DoPrelude()
        {
            if (Config == null) return null;
            var data = Config.ScenePreludes;
            var count = data.Length;
            if (count < 1) return null;
            var index = IsRandomPrelude ? Random.Range(0, count) : ++PreludeIdxStart % count;
            var spPrefab = data[index];  
            if (spPrefab == null) return null;
            var sp = Instantiate(spPrefab);
            sp.transform.parent = transform;
            var localPos = sp.transform.localPosition;
            localPos.z = 0F;
            sp.transform.localPosition = localPos;
            sp.Go();
            CurPrelude = sp;
            return sp; 
        } 
    }
}
