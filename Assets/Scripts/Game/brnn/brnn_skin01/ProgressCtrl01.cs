using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn.brnn_skin01
{
    public class ProgressCtrl01 : ProgressCtrl
    {
        public GameObject Clock;
        //去除不必要的变量
        public override void BeginCountdown()
        {
            Clock.SetActive(true);
            CancelInvoke("CyclePerform_Num");
            InvokeRepeating("CyclePerform_Num", 0, 1);
        }

        //去除不必要变量
        public override void EndCountdown()
        {
            Clock.SetActive(false);
            CancelInvoke("CyclePerform_Num");
            App.GetGameData<BrnnGameData>().BeginBet = false;
        }
    }
}