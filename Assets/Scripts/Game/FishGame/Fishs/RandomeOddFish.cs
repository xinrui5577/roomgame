using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.FishGame.Fishs
{
    public class RandomeOddFish : Fish
    {
        public Interval RandomeInterval;

        [SerializeField, UsedImplicitly]
        private tk2dTextMesh _oddText;

        protected override void OnStart()
        {
            InvokeRepeating("ChangeOdds", 0, 3);
        }

        private void ChangeOdds()
        {
            if (_oddText == null) return;
            Odds = Random.Range(RandomeInterval.Min, RandomeInterval.Max);
            _oddText.text = Odds.ToString();
        }
    }

    [Serializable]
    public struct Interval
    {
        public int Min;
        public int Max;
    }
}
