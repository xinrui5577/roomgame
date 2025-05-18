using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class SzmjGameInfo : MonoBehaviour, IGameStartCycle, IGameEndCycle
    {
        public Transform Title;

        private void Awake()
        {
            GameCenter.RegisterCycle(this);
        }

        public void OnGameEndCycle()
        {
            Title.gameObject.SetActive(false);
        }

        public void OnGameStartCycle()
        {
            Title.gameObject.SetActive(GameCenter.DataCenter.Game.Diling == 1);
        }
    }
}