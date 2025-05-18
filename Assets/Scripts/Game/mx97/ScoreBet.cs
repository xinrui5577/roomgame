using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.mx97
{
    public class ScoreBet : MonoBehaviour
    {

        public UILabel ContentLabel;

        // Use this for initialization
       protected void Start ()
        {
            Facade.EventCenter.AddEventListener<Mx97EventType, object>(Mx97EventType.RefreshBetScore, OnRefreshBetScore);
        }
	
        private void OnRefreshBetScore(object obj)
        {
            if (ContentLabel != null)
            {
                var gdata = App.GetGameData<Mx97GlobalData>();
                var iBetScore = gdata.CurBet;
                ContentLabel.text = "￥" + YxUtiles.GetShowNumberForm(iBetScore);
            }
            
        }
    }
}
