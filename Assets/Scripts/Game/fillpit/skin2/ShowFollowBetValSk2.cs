using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.fillpit.skin2
{
    public class ShowFollowBetValSk2 : MonoBehaviour
    {

        public string Format = "跟{0}";

        public UILabel FollowLabel;

        protected void OnEnable()
        {
            int lastBet = App.GetGameData<FillpitGameData>().LastBetValue;
            FollowLabel.text = string.Format(Format, YxUtiles.ReduceNumber(lastBet));
        }

    }
}
