using UnityEngine;

namespace Assets.Scripts.Game.brnn3d
{
    public class MidUIController : MonoBehaviour
    {
        public BetMoneyUI TheBetMoneyUI;
        public GameBackUI TheGameBackUI;
        public NiuNumberUI TheNiuNumberUI;
        public SettleMentUI TheSettleMentUI;

        public void GameBackUINoClicked()
        {
            TheGameBackUI.HideSelf();
        }
    }

}
