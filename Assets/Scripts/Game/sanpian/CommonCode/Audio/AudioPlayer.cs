using Assets.Scripts.Game.sanpian.CommonCode.ChessCommon;
using Assets.Scripts.Game.sanpian.DataStore;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sanpian.CommonCode.Audio
{
	class AudioPlayer:MonoBehaviour
	{

//	    public static void Pause(bool t) {
//            _instance._pause(t);
//        }

	    private static AudioPlayer _instance;

	    public static AudioPlayer GetInstance() {
	        return _instance;
	    }


        public static void PlayMusic(AudioClip audio)
        {
            _instance._playMusic(audio);
        }

        public static void PlayAudio(AudioClip audio) {
            _instance._playAudio(audio);
        }

	    public void Awake() {
	        if (_instance) {
	            Debug.LogError("you should create only one AudioPlayer in scene");
            }
	        _instance = this;
        }

	    private float _audioVolume;
	    private float _bgMusicVolume;
	    public void Start() {
            _audioVolume = PlayerPrefs.GetFloat(SoundSetting.AudioVolume, 0.5f);
            OnAudioValueChange(_audioVolume);
            _bgMusicVolume = PlayerPrefs.GetFloat(SoundSetting.BGMusicVolume, 0.5f);
            OnBgMusicValueChange(_bgMusicVolume);
            var gmsg = App.GetGameData<SanPianGameData>().GMessage;
            gmsg.OnAudioVolumeChange = OnAudioValueChange;
            gmsg.OnBgMusicVolumeChange = OnBgMusicValueChange;
        }

	    private void OnAudioValueChange(float value) {
            Debug.Log("audio change");
            App.GetGameData<SanPianGameData>().IsMusicOn = value > 0.05;
	        _audioVolume = value;
	    }

        private void OnBgMusicValueChange(float value) {
            Debug.Log("sound change");
            if (value < 0.05) {
                if (BgMusicSource.isPlaying) {
                    BgMusicSource.Pause();
                }
            }
            else {
                if (!BgMusicSource.isPlaying) {
                    BgMusicSource.Play();
                }
            }
            _bgMusicVolume = value;
            BgMusicSource.volume = _bgMusicVolume;
        }


	    public AudioSource AudioSource;
	    public AudioSource BgMusicSource;

        private void _playAudio(AudioClip audioName) {
            AudioSource.PlayOneShot(audioName, _audioVolume);
        }

        private void _playMusic(AudioClip ac)
        {
	        if (ac) {
	            BgMusicSource.clip = ac;
	            BgMusicSource.Play();
	        }
	        else {
	            BgMusicSource.Stop();
            }
	    }
	}
}
