using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class SoundPnl : MonoBehaviour {

        public void SliderTest(float even)
        {
            Facade.Instance<MusicManager>().MusicVolume = even;
        }
    }
}
