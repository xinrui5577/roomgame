using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongCounter : MahjongTablePart
    {
        public DigitelDisplay CounterDisplay;

        public int Counter
        {
            get { return CounterDisplay.Number; }
            set { CounterDisplay.SetMahjongCounter(value); }
        }

        private void Start()
        {
            Counter = 0;
        }

        public override void OnReset()
        {
            Counter = 0;
            CounterDisplay.gameObject.SetActive(false);
        }

        public void SetMahjongCounter(int number)
        {
            Counter = number;
            CounterDisplay.gameObject.SetActive(true);
        }
    }
}