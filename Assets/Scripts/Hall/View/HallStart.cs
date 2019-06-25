using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View
{
    public class HallStart : MonoBehaviour
    {
        public string BackSoundName = "bgm3";
        /// <summary>
        /// 默认音效音量的key
        /// </summary>
        public string DefEffectKey = "DefEffect";
        /// <summary>
        /// 默认音乐音量大小
        /// </summary>
        public string DefMusicKey = "DefMusic";
        /// <summary>
        /// 默认音效音量大小
        /// </summary>
        public float DefEffectValue = 1.0f;
        /// <summary>
        /// 默认音乐音量大小
        /// </summary>
        public float DefMusicValue = 1.0f;
        // Use this for initialization
        public void Start()
        {
            HallMainController.Instance.LaunchHall(App.HasLogin);
            var musicMgr = Facade.Instance<MusicManager>();
            musicMgr.InitBackSound();
            //            musicMgr.LoadAudioBundle(App.HallName);
//            if (musicMgr.LoadAudioBundle(App.HallName, "DefAudioSource"))
//            {
//                YxDebug.LogError("加载背景音乐");
//                if (!musicMgr.PlayBacksound())
//                {
//                    var typeName = PlayerPrefs.GetString("MusiceType");
//                    if (string.IsNullOrEmpty(typeName)) typeName = BackSoundName;
//                    musicMgr.PlayBacksound(typeName);
//                }
//            }
            //            musicMgr.LoadAudioBundleSync(App.HallName, "DefAudioSource", hasLoad =>
            //                {
            //                    
            //                });

            DefEffectKey += Application.bundleIdentifier;
            DefMusicKey += Application.bundleIdentifier;
            if (!PlayerPrefs.HasKey(DefEffectKey))
            {
                PlayerPrefs.SetFloat(DefEffectKey, DefEffectValue);
                musicMgr.EffectVolume = DefEffectValue;
            }
            if (!PlayerPrefs.HasKey(DefMusicKey))
            {
                PlayerPrefs.SetFloat(DefMusicKey, DefMusicValue);
                musicMgr.MusicVolume = DefMusicValue;
            }
        }
    }
}
