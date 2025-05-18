using Assets.Scripts.Game.FishGame.Common.Utils;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.Common.additive
{
    public class GC_WhenFinishChangedScene : MonoBehaviour {

        // Use this for initialization
        void Start () {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtMainProcessFinishChangeScene += Handle_MainProcess_FinishChangeScene;
        }
	
        void Handle_MainProcess_FinishChangeScene()
        {
            System.GC.Collect();
        }
    }
}
