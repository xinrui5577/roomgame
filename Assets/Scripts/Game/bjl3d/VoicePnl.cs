using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.bjl3d
{
    public class VoicePnl : MonoBehaviour
    {
        public void SliderTest(float even)
        {
            Facade.Instance<MusicManager>().EffectVolume = even;
        }
    }
}
