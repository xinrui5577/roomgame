using UnityEngine;
using YxFramwork.Manager;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.LXGameScripts
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        public float SoundVolume
        {
            set
            {
                Facade.Instance<MusicManager>().MusicVolume = value;
                PlayerPrefs.SetFloat("SoundVolume", value);
            }
        }

        public float EffectVolume
        {
            set
            {
                //这个MusicManager为空,需要改
                Facade.Instance<MusicManager>().EffectVolume = value;
                PlayerPrefs.SetFloat("EffectVolume",value);
            }
        }

        void Awake()
        {
            Instance = this;
        }
        // Use this for initialization
        void Start()
        {
            SoundVolume = PlayerPrefs.GetFloat("SoundVolume", 1f);
            EffectVolume = PlayerPrefs.GetFloat("EffectVolume", 1f);
        }

        void OnDestroy()
        {
            Instance = null;
        }
    }
}

