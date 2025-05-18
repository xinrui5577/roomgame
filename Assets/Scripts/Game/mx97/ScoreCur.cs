using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.mx97
{
    public class ScoreCur : MonoBehaviour
    {

        public UILabel MContentLabel;

        // Use this for initialization
        protected void Start ()
        {
            Facade.EventCenter.AddEventListener<Mx97EventType,object>(Mx97EventType.RefreshCurScore, OnRefreshCurScore);
        }
	
        // Update is called once per frame
        private void OnRefreshCurScore (object obj)
        {
            if (MContentLabel == null) { return;}
            var score = App.GetGameData<Mx97GlobalData>().CurScore;
            MContentLabel.text = "￥" + YxUtiles.GetShowNumberForm(score);
        }
    }
}
