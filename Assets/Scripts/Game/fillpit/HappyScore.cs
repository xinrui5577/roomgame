using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.fillpit
{
    public class HappyScore : MonoBehaviour
    {

        public UILabel ScoreLabel;


        public void SetScore()
        {
            var gdata = App.GetGameData<FillpitGameData>();
            if (gdata.IsLanDi)
            {
                int happyScore = gdata.Happys;
                if (happyScore <= 0) return;

                ShowScoreLabel(happyScore);
            }
            else
            {
                Hide();
            }
        }
        

        public void ShowScoreLabel(int score)
        {
            ScoreLabel.text = YxUtiles.GetShowNumberForm(score , 2, "#.##");
            int happyScore = App.GetGameData<FillpitGameData>().Happys;
            gameObject.SetActive(happyScore > 0);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }

    }
}
