using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Fishing.entitys
{
    public class Web : MonoBehaviour
    {
        public string SoundName = "sound_net";

        void Start()
        {
            Facade.Instance<MusicManager>().Play(SoundName);
        }
    }
}
