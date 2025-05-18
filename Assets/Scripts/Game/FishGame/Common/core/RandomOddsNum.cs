using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class RandomOddsNum : MonoBehaviour {
        public int MinOdds = 2;
        public int MaxOdds = 101;
        // Use this for initialization
        void Start () {
            Fish f = GetComponent<Fish>();
            f.Odds = Random.Range(MinOdds, MaxOdds);
        }

    }
}
