using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using com.yx.chatsystem;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class VoiceInfo
    {
        public int chair;
        public int len;
        public AudioClip clip;
    }

    public class TalkPnl : MonoBehaviour
    {
        public YxVoiceChatSystem ChatSystem;
        public AudioSource Audio;

        private List<VoiceInfo> _voiceList = new List<VoiceInfo>();

        private bool _isStartVoice;

        public void OnGetSoundKey(string key)
        {
            var info = ChatSystem.UploadInfoData;
            info.Url = key;
            info.OnFinish = OnUpLoadVoiceSuccess;
            info.OnFail = OnUpLoadVoiceFaild;
        }

        private void OnUpLoadVoiceSuccess(string url)
        {
            int lenth = ChatSystem.UploadInfoData.LengthSce;

            EventDispatch.Dispatch((int)NetEventId.OnVoiceUpload, new EventData(lenth, url));
        }

        private void OnUpLoadVoiceFaild(string url)
        {
            YxDebug.LogError("上传声音失败 "+ url);   
        }

        public void DownLoadVoice(string url,int chair,int len)
        {
            ChatSystem.DownloadRecord(
                url,
                clip =>
                {
                    //下载完成后 加入播放序列
                    VoiceInfo info = new VoiceInfo();
                    info.chair = chair;
                    info.len = len;
                    info.clip = clip;

                    AddVoiceLsit(info);
                },
                err =>
                {
                    YxMessageBox.Show("下载音频失败");
                }
                );
            YxDebug.Log("------> RemoteServer.OnReceiveVoiceChat url=" + url + " chair=" + chair + " len=" + len + "!");

        }

        public void AddVoiceLsit(VoiceInfo info)
        {
            _voiceList.Add(info);
            if (!_isStartVoice)
            {
                StartCoroutine(PlayVoice());
            }
        }

        private IEnumerator PlayVoice()
        {
            Facade.Instance<MusicManager>().SetMusicPause(true);
            while (_voiceList.Count>0)
            {
                _isStartVoice = true;

                VoiceInfo info = _voiceList[0];
                _voiceList.RemoveAt(0);
                Audio.clip = info.clip;
                Audio.Play();
                //给界面发送当前播放的声音
                EventDispatch.Dispatch((int)UIEventId.ShowUserSpeak, new EventData(info.chair));
                yield return new WaitForSeconds(info.len);
                EventDispatch.Dispatch((int)UIEventId.StopUserSpeak, new EventData(info.chair));
            }

            _isStartVoice = false;
            Facade.Instance<MusicManager>().SetMusicPause(false);
        }
    }
}