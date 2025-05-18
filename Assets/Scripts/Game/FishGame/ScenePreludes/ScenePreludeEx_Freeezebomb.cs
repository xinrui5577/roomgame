using Assets.Scripts.Game.FishGame.Common.Utils;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.FishGame.ScenePreludes
{
    public class ScenePreludeEx_Freeezebomb : MonoBehaviour {

        private FishGame.ScenePreludes.ScenePrelude mSp;
        // Use this for initialization
        void Awake()
        {
            mSp = GetComponent<FishGame.ScenePreludes.ScenePrelude>();
            if (mSp == null)
                return;
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtFreezeBombActive += Handle_FreezeBombActive;
            gdata.EvtFreezeBombDeactive += Handle_FreezeBombDeactive;
	
        }

        void Handle_FreezeBombActive()
        {
            mSp.Pause();
        }
        void Handle_FreezeBombDeactive()
        {
            mSp.Resume();
        }
	 
    }
}
