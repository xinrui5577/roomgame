using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class GamesNumUI : MonoBehaviour
    {
        public Text JuText;
        public Text BaText;

        public void SetGamesNumUI()
        {
            if (JuText != null)
                JuText.text = App.GetGameData<Brnn3dGameData>().Frame + "";
            if (BaText != null)
                BaText.text = App.GetGameData<Brnn3dGameData>().Bundle + "";
        }

    }
}

