using UnityEngine;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class FishEx_OddsMulti : MonoBehaviour {
        public int OddsMulti = 1;
        // Use this for initialization
        void Awake () {
            if (OddsMulti <= 0)
                YxDebug.LogError("oddsMulti值得必须大于0");
        }
    }
}
