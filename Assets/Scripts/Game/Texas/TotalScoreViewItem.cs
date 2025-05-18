using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Texas
{
    public class TotalScoreViewItem : MonoBehaviour
    {

        public UILabel NameLabel;

        public UILabel ScoreLabel;

        public string ScoreFormat;

        public void SetInfo(TotalScoreUserInfo userInfo)
        {
            NameLabel.text = userInfo.UserName;
            ScoreLabel.text = string.Format(ScoreFormat, YxUtiles.ReduceNumber(userInfo.TotalScore));
            gameObject.SetActive(true);
            NameLabel.color = userInfo.UserId == App.GameData.GetPlayerInfo().Id ? Color.yellow : Color.white;
        }
    }
}
