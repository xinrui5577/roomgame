using System;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Manager;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.FishGame.Backgrounds
{
    public class SceneBgConfig : MonoBehaviour {
        /// <summary>
        /// 资源前缀名
        /// </summary>
        public string BackGroundBundleName = "BackGround";
        /// <summary>
        /// 波纹效果
        /// </summary>
        public GameObject PrefabWave;
        /// <summary>
        /// 过长打浪效果
        /// </summary>
        public Transform PrefabTsSweeper;
        /// <summary>
        /// 是否为随机背景
        /// </summary>
        public bool IsNewBgWhenStartGame = true;
        /// <summary>
        /// 过长时间
        /// </summary>
        public float UseTime = 3F;
        /// <summary>
        /// 声音
        /// </summary>
        public AudioClip SndZhuanChangLangHua;

        /// <summary>
        /// 背景名称数组
        /// </summary>
        public string[] BackGroundNames;

        public float SweeperZ = 590F;//场景切换时扫过的波浪 todo 待删 

        /// <summary>
        /// 随机获取一个背景索引
        /// </summary>
        /// <returns></returns>
        public int RandomBackGroundIndex()
        {
            var count = BackGroundNames.Length;
            return Random.Range(0, count - 1);
        }

        public GameObject LoadMap(int index)
        {
            var bgName = BackGroundNames[index];
            var bdName = string.Format("{0}/{1}", BackGroundBundleName,bgName);
            return ResourceManager.LoadAsset(bgName, bdName);
        }

        public void LoadMapAsync(int index,Action<AssetBundleInfo> onFinished)
        {
            var bgName = BackGroundNames[index];
            var bdName = string.Format("{0}/{1}", BackGroundBundleName, bgName);
            var info = new AssetBundleInfo
            {
                Attach = App.GameKey,
                Name = bdName,   
                AssetName = bgName
            };
            ResourceManager.LoadAssesAsync<GameObject>(info,false, onFinished);
        }
    }
}
