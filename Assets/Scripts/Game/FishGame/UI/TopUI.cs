using UnityEngine;

namespace Assets.Scripts.Game.FishGame.UI
{
    public class TopUI : UIView
    {

        public void OnCloseClick()
        {
            GameMain.OnQuitGame();
        }
    }
}
