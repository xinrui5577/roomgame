using System;
using Assets.Scripts.Game.Fishing.enums;
using UnityEngine;

namespace Assets.Scripts.Game.Fishing.datas
{
    [CreateAssetMenu(menuName = "Fish/Create FishDescriptionInfo")]
    public class FishDescriptionInfo : ScriptableObject
    {
        public FishDescriptionData[] FishDatas;
    }

    [Serializable]
    public class FishDescriptionData
    {
        /// <summary>
        /// 
        /// </summary>
        public int FishId;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name;
        public int MinBet;
        public int MaxBet;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description;

        public Sprite Icon;
        /// <summary>
        /// 模型缩放大小
        /// </summary>
        public float ModelScale;

        public EFishType FishType;
    }
}
