using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.fillpit
{
    public class WinAnimFinishFuction : MonoBehaviour
    {
        public void ShowBriefSum()
        {
            gameObject.SetActive(false);
            App.GetGameManager<FillpitGameManager>().SummaryMgr.ResultTimer = 0;
        }
    }
}