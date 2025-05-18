//using com.yx.chatsystem;
//using com.yxixia.utile.YxDebug;
//using Sfs2X.Entities.Data;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using YxFramwork.Common;
//using YxFramwork.ConstDefine;
//using YxFramwork.Framework.Core;
//using YxFramwork.Manager;
//using YxFramwork.View;
//
//namespace Assets.Scripts.Game.sanpian.server
//{
//    public class TalkManager : MonoBehaviour
//    {
//        private List<TalkData> clips;
//        public VoiceChatSystem ChatSystem;
//        public AudioSource PlayerVoiceChat;
//        public float BiaoQingTime;
//
//        void Start()
//        {
//            RequestSoundkey();
//            clips = new List<TalkData>();
//        }
//
//        private void RequestSoundkey()
//        {
//            if (Facade.Instance<TwManger>() == null)
//            {
//                return;
//            }
//            Facade.Instance<TwManger>().SendActionKey
//            (
//                 "soundApi",
//                 new Dictionary<string, object>(),
//                 msg =>
//                 {
//                     var key = msg.ToString();
//                     if (key != null)
//                     {
//                         UploadInfo info = ChatSystem.UploadInfoData;
//                         info.Url = key;
//                         info.OnFail = OnUploadVoiceFail;
//                         info.OnFinish = OnUpLoadVoiceSuccess;
//                     }
//                 },
//                 false,
//                ErrMsg =>
//                {
//                    var errDic = (IDictionary)ErrMsg;
//                    string show = errDic["errorMessage"].ToString();
//                    YxDebug.LogError("soundApi 请求失败error is:" + show);
//                }
//                );
//        }
//        private void OnUploadVoiceFail(string error)
//        {
//            if (string.IsNullOrEmpty(error))
//            {
//                YxDebug.LogError("语音的失败回调是null，什么情况");
//                return;
//            }
//            YxDebug.LogError(error);
//        }
//
//        private void OnUpLoadVoiceSuccess(string url)
//        {
//            int lenth = this.ChatSystem.UploadInfoData.LengthSce;
//            SanPianGameServer.Instance.SendVoiceChat(url, lenth, App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Seat);
//        }
//
//
//        //收语音调这个
//        public void OnUserSpeak(ISFSObject param)
//        {
//            string url = param.GetUtfString("url");
//            int seat = param.ContainsKey(RequestKey.KeySeat) ? param.GetInt("seat") : 0;
//            int len = param.ContainsKey("len") ? param.GetInt("len") : 1;
//            ChatSystem.DownloadRecord(
//                url,
//                clip =>
//                {
//                    clips.Add(new TalkData()
//                    {
//                        Clip = clip,
//                        Seat = seat,
//                        Lenth = len
//                    });
//                    PlayVoice();
//                },
//                err =>
//                {
//                    YxMessageBox.Show("下载音频失败");
//                }
//                );
//            YxDebug.Log("------> RemoteServer.OnReceiveVoiceChat url=" + url + " seat=" + seat + " len=" + len + "!");
//        }
//
//        private void PlayVoice()
//        {
//            YxDebug.LogError("当前队列中音频的数量是：" + clips.Count);
//            TalkData data = clips[0];
//            YxDebug.LogError("音频的长度是：" + data.Lenth);
//            PlayVoiceChat(data.Clip, data.Lenth, data.Seat);
//            Invoke("OnFinish", data.Lenth);
//        }
//        private float _voice;
//        private int temp_seat;
//        /// <summary>
//        /// 播放语音聊天
//        /// </summary>
//        public void PlayVoiceChat(AudioClip chatClip, float len, int seat)
//        {
//            _voice = Facade.Instance<MusicManager>().MusicVolume;
//            temp_seat = seat;
//            Facade.Instance<MusicManager>().MusicVolume = 0;
//            PlayerVoiceChat.clip = chatClip;
//            PlayerVoiceChat.Play();
//            App.GetGameManager<SanPianGameManager>().PlayerArr[seat].UIInfo.SpeakIcon.SetActive(true);
//            YxDebug.LogError("------> UserInfoPanel: PlayVoiceChat() len=" + len + "!");
//            Invoke("CloseSpeaker", len);
//        }
//
//        private void CloseSpeaker()
//        {
//            App.GetGameManager<SanPianGameManager>().PlayerArr[temp_seat].UIInfo.SpeakIcon.SetActive(false);
//            Facade.Instance<MusicManager>().MusicVolume = _voice;
//        }
//        void OnFinish()
//        {
//            clips.Remove(clips[0]);
//            CancelInvoke("OnFinish");
//            if (clips.Count == 0)
//            {
//                return;
//            }
//            else
//            {
//                PlayVoice();
//            }
//        }
//
//
//
//
//        //发送文字聊天
//        public void OnClickTalkBtn()
//        {
//            YxWindowManager.OpenWindow("Talk_Panel");
//        }
//
//        //接收文字表情聊天
//        private string _conmonTag = "#common:";
//        public void OnUserTalk(ISFSObject param)
//        {
//            int seat = param.GetInt(RequestKey.KeySeat);
//            int index = param.GetInt(RequestKey.KeyExp);
//            string text = param.GetUtfString(RequestKey.KeyText);
//            if (string.IsNullOrEmpty(text))
//            {
//                UISprite BiaoQingSprite = App.GetGameManager<SanPianGameManager>().PlayerArr[seat].UIInfo.BiaoQing;
//                BiaoQingSprite.spriteName = index + "";
//                BiaoQingSprite.gameObject.SetActive(true);
//                TweenAlpha tween = BiaoQingSprite.transform.GetComponent<TweenAlpha>();
//                tween.ResetToBeginning();
//                tween.PlayForward();
//
//            }
//            else
//            {
//                string str;
//                if (text.Contains(_conmonTag))
//                {
//                    int commonIndex = int.Parse(text.Replace(_conmonTag, null));
//                    str = App.GetGameData<SanPianGameData>().Common[commonIndex];
//                    App.GetGameManager<SanPianGameManager>().PlayerArr[seat].TalkVoicePlay(commonIndex);
//                }
//                else
//                {
//                    str = text;
//                }
//                App.GetGameManager<SanPianGameManager>().PlayerArr[seat].UIInfo.TalkBubble.ShowLabel(str);
//
//            }
//        }
//
//
//        public void HideObj(GameObject obj)
//        {
//            obj.SetActive(false);
//        }
//
//
//    }
//
//
//
//
//
//
//
//
//
//
//
//
//    public class TalkData
//    {
//        public AudioClip Clip;
//        public int Seat;
//        public int Lenth;
//    }
//}
