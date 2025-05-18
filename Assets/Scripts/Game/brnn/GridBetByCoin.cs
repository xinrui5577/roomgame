using UnityEngine;

namespace Assets.Scripts.Game.brnn
{
    public class GridBetByCoin : MonoBehaviour
    {

        public GameObject[] Bet;
        public GameObject BetBg;
        private bool _isFirst = true;
        public void SetShowBet(long coin)
        {
            int p = 0;
            if (!_isFirst)
            {

                for (int i = 0; i < CommonObject.CurrentShowChip.Length; i++)
                {
                    if (CommonObject.CurrentSelectChip == CommonObject.CurrentShowChip[i])
                    {
                        p = i;
                        break;
                    }
                }
            }
            if (coin < 50000)
            {
                Bet[0].SetActive(true);
                Bet[1].SetActive(true);
                Bet[2].SetActive(true);
                Bet[3].SetActive(false);
                Bet[4].SetActive(false);
                Bet[5].SetActive(false);
                CommonObject.CurrentShowChip = new[] { Bet[0], Bet[1], Bet[2] };
            }
            else if (coin >= 50000 && coin < 100000)
            {
                Bet[0].SetActive(false);
                Bet[1].SetActive(true);
                Bet[2].SetActive(true);
                Bet[3].SetActive(true);
                Bet[4].SetActive(false);
                Bet[5].SetActive(false);
                CommonObject.CurrentShowChip = new[] { Bet[1], Bet[2], Bet[3] };
            }
            else if (coin >= 100000 && coin < 500000)
            {
                Bet[0].SetActive(false);
                Bet[1].SetActive(false);
                Bet[2].SetActive(true);
                Bet[3].SetActive(true);
                Bet[4].SetActive(true);
                Bet[5].SetActive(false);
                CommonObject.CurrentShowChip = new[] { Bet[2], Bet[3], Bet[4] };
            }
            else if (coin >= 500000)
            {
                Bet[0].SetActive(false);
                Bet[1].SetActive(false);
                Bet[2].SetActive(false);
                Bet[3].SetActive(true);
                Bet[4].SetActive(true);
                Bet[5].SetActive(true);
                CommonObject.CurrentShowChip = new[] { Bet[3], Bet[4], Bet[5] };

            }
            CommonObject.CurrentSelectChip = CommonObject.CurrentShowChip[p];
            gameObject.GetComponent<UIGrid>().Reposition();
            _isFirst = false;

        }
    }
}
