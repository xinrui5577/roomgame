using System.Collections;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.FishKinds
{
    public abstract class GroupGenerateData : MonoBehaviour {
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

        protected abstract IEnumerator OnGenerate();
          
        public void Generate()
        {
            StartCoroutine(OnGenerate());
        }
    } 
}
