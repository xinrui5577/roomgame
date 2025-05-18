using UnityEngine;

namespace Assets.Scripts.Game.fillpit.skin1
{
    public class CoverCard : MonoBehaviour {
     
        public Transform Target;

        public PokerCard Poker;

        public void ShowPoker(int cardVal,int index)
        {
            Poker.SetCardId(cardVal);
            Poker.SetCardFront();
            Poker.SetCardDepth(index*2);
            Poker.transform.localPosition = Vector3.zero;
        }
    }
}
