using Assets.Scripts.Game.pdk.DDzGameListener;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    public class SettingCtrl : ServEvtListener
    {
        [SerializeField]
        protected UISlider MUiSlider;

        [SerializeField]
        protected UISlider SoundSlider;

        [SerializeField]
        protected UIToggle VoiceChatToggle;

        [SerializeField] protected MessageBoxScroll MesboxScorll; 

        public delegate void SoundValueChange(float value);
        private static SoundValueChange _onSoundValueChangeEvt;
        public static SoundValueChange OnSoundValueChangeEvt
        {
            set { _onSoundValueChangeEvt += value; }
        }

        /// <summary>
        /// 场景销毁后，重置静态变量
        /// </summary>
        public void OnDestroy()
        {
            _onSoundValueChangeEvt = null;
        }

        /// <summary>
        /// 音乐大小
        /// </summary>
        public const string MusicValueKey = "MusicKey20171844";
        /// <summary>
        /// 音效大小
        /// </summary>
        public const string SoundValueKey = "sdvalueKey20171844";
        /// <summary>
        /// 是否播放语音key
        /// </summary>
        public const string VoiceChatKey = "voickechat20170420";

        private GlobalData _globalData;

        public void Awake()
        {
            Ddz2RemoteServer.AddOnHandUpEvt(OnHandUpEvt);
        }

        /// <summary>
        /// 投票事件激发 2发起 3同意 -1拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnHandUpEvt(object sender, DdzbaseEventArgs args)
        {
            MesboxScorll.HideMessageBox();
        }

        public void Start()
        {
            _globalData = App.GetGameData<GlobalData>();
        }

        public void OnEnable()
        {
            //Debug.LogError(PlayerPrefs.GetFloat(MusicValueKey, 1));
            MUiSlider.value = Facade.Instance<MusicManager>().MusicVolume;//PlayerPrefs.GetFloat(MusicValueKey, 0);
            SoundSlider.value = Facade.Instance<MusicManager>().EffectVolume;//PlayerPrefs.GetFloat(SoundValueKey, 1);
            VoiceChatToggle.value = PlayerPrefs.GetInt(VoiceChatKey, 1) == 1;
        }


        /// <summary>
        /// 滑动MusicSlider
        /// </summary>
        public void OnMusicValueChange()
        {
            
           // if (MUiSlider != null) if (_globalData != null) _globalData.MusicAudioSource.volume = MUiSlider.value;
            if (MUiSlider != null) Facade.Instance<MusicManager>().MusicVolume = MUiSlider.value;
        }


        /// <summary>
        /// 滑动SoundSlider
        /// </summary>
        public void OnSoundValueChange()
        {
            if (_onSoundValueChangeEvt != null) _onSoundValueChangeEvt(SoundSlider.value);
            Facade.Instance<MusicManager>().EffectVolume = SoundSlider.value;
        }

        public void OnToggle()
        {
            App.GetGameData<GlobalData>().IsPlayVoiceChat = VoiceChatToggle.value;
        }

        /// <summary>
        /// 当点击关闭按钮
        /// </summary>
        public void OnCloseBtnClick()
        {
/*            PlayerPrefs.SetFloat(MusicValueKey, MUiSlider.value);
            PlayerPrefs.SetFloat(SoundValueKey, SoundSlider.value);*/
            Facade.Instance<MusicManager>().EffectVolume = SoundSlider.value;
            Facade.Instance<MusicManager>().MusicVolume = MUiSlider.value;
            PlayerPrefs.SetInt(VoiceChatKey, VoiceChatToggle.value ? 1 : 0);
            PlayerPrefs.Save();
        }

        ///// <summary>
        ///// 点击解散房间按钮
        ///// </summary>
        //public void OnClickDismisRoomBtn()
        //{
        //    var isSelfOwner = App.GetGameData<GlobalData>().IsSelfIsOwer;

        //    var msgstr = isSelfOwner ? "确定要解散房间么？" : "确定离开房间吗？";


        //    YxMessageBox.Show(msgstr, "", (box, btnName) =>
        //    {
        //        if (btnName == YxMessageBox.BtnLeft)
        //        {
        //            if (!App.GetGameData<GlobalData>().IsStartGame)
        //            {
        //                //App.GetGameData<GlobalData>().ClearParticalGob();

        //                if (isSelfOwner)
        //                    GlobalData.ServInstance.DismissRoom();
        //                else
        //                    GlobalData.ServInstance.LeaveRoom();
        //                return;
        //            }

        //            GlobalData.ServInstance.StartHandsUp(2);
        //        }
        //    }, true, YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle);
        //}

        public override void RefreshUiInfo()
        {
            
        }
    }
}
