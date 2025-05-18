using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
using com.yx.chatsystem;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.pdk.DDzGameListener.Chat
{
    /// <summary>
    /// 语音聊天系统
    /// </summary>
    public class VoiceChatListener : ServEvtListener {

        
        /// <summary>
        /// 语音接口
        /// </summary>
        [SerializeField]
        protected YxVoiceChatSystem ChatSystem;
        
        /// <summary>
        /// 倒计时label
        /// </summary>
        [SerializeField]
        protected UILabel CountDownLabel;

        /// <summary>
        /// 播放聊天的喇叭和audiosource组件
        /// </summary>
        [SerializeField] 
        protected GameObject ChatHornSelf;
        /// <summary>
        /// 播放聊天的喇叭和audiosource组件
        /// </summary>
        [SerializeField]
        protected GameObject ChatHornLeft;
        /// <summary>
        /// 播放聊天的喇叭和audiosource组件
        /// </summary>
        [SerializeField]
        protected GameObject ChatHornRight;

        /// <summary>
        /// 语音队列
        /// </summary>
        private readonly Queue<VoiceInfo> _voiceQueue = new Queue<VoiceInfo>();

        protected string SoundKey;

        protected override void OnAwake()
        {
            Ddz2RemoteServer.AddOnVoiceChatEvt(OnUserSpeak);
        }

        void Start()
        {
            RequestSoundkey();
        }

        void OnEnable()
        {
            ChatHornSelf.SetActive(false);
            ChatHornLeft.SetActive(false);
            ChatHornRight.SetActive(false);

        }

        /// <summary>
        /// 请求soundKey
        /// </summary>
        private void RequestSoundkey()
        {
            //设置倒计时代理
            ChatSystem.OnScecondFrame = OnCountDown;
            //YxDebug.LogError("发送请求soundkey");
            Facade.Instance<TwManager>().SendAction
            (
                 "soundApi",
                 new Dictionary<string, object>(),
                 msg =>
                 {
                     var key = msg.ToString();
                     {
                         SoundKey = key;
                         UploadInfo info = ChatSystem.UploadInfoData;
                         info.Url = SoundKey;
                         info.OnFail = OnUploadVoiceFail;
                         info.OnFinish = OnUpLoadVoiceSuccess;
                     }
                 },
                 false,
                errMsg =>
                {
                    var errDic = (IDictionary)errMsg;
                    string show = errDic["errorMessage"].ToString();
                }
                );
        }

        /// <summary>
        /// 上传失败
        /// </summary>
        /// <param name="error"></param>
        private void OnUploadVoiceFail(string error)
        {
            YxMessageBox.Show(string.Format("上传失败 {0}", error));
        }

        /// <summary>
        /// 上传成功
        /// </summary>
        /// <param name="url"></param>
        private void OnUpLoadVoiceSuccess(string url)
        {
            int lenth = ChatSystem.UploadInfoData.LengthSce;
            GlobalData.ServInstance.SendVoiceChat(url,lenth,App.GetGameData<GlobalData>().GetSelfSeat);
        }

        public override void RefreshUiInfo()
        {

        }

        /// <summary>
        /// 倒计时
        /// </summary>
        /// <param name="s"></param>
        protected void OnCountDown(int s)
        {
            int surplus = ChatSystem.RecordMaxSecLenght - s;
/*            if (surplus > 10)
            {
                CountDownLabel.text = "";
                return;
            }*/

            if (CountDownLabel != null) CountDownLabel.text = string.Format("还可以说{0}秒", surplus);
        }

        /// <summary>
        /// 接到语音播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void OnUserSpeak(object sender, DdzbaseEventArgs args)
        {
            if(!App.GetGameData<GlobalData>().IsPlayVoiceChat)return;

            var param = args.IsfObjData;

            string url = param.GetUtfString("url");
            int seat = param.ContainsKey(RequestKey.KeySeat) ? param.GetInt("seat") : 0;
            int len = param.ContainsKey("len") ? param.GetInt("len") : 1;
            ChatSystem.DownloadRecord(
                url,
                clip =>
                {
                    var info = new VoiceInfo
                        {
                        Clip = clip,
                        Seat = seat,
                        Length = len,
                    };
                    _voiceQueue.Enqueue(info);

                    PlayVoice();
                },
                err => YxMessageBox.Show("下载音频失败"));

            YxDebug.Log("------> RemoteServer.OnReceiveVoiceChat url=" + url + " seat=" + seat + " len=" + len + "!");
        }

        /// <summary>
        /// 播放语音聊天
        /// </summary>
        public void PlayVoice()
        {
            if (_voiceQueue.Count == 0)
            {
                return;
            }

            VoiceInfo info = _voiceQueue.Dequeue();
            StartCoroutine(PlayVoiceChat(info.Clip, info.Length, info.Seat));
        }


        /// <summary>
        /// 声音播放
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="lenth"></param>
        /// <param name="seat"></param>
        /// <returns></returns>
        private IEnumerator PlayVoiceChat(AudioClip clip,int lenth,int seat)
        {
            GameObject curGob = null;
            if (seat == App.GetGameData<GlobalData>().GetSelfSeat)
            {
                ChatHornSelf.SetActive(true);
                var audioSource = ChatHornSelf.GetComponentInChildren<AudioSource>();
                audioSource.PlayOneShot(clip);
                curGob = ChatHornSelf;
            }
            else if (seat == App.GetGameData<GlobalData>().GetLeftPlayerSeat)
            {
                ChatHornLeft.SetActive(true);
                var audioSource = ChatHornLeft.GetComponentInChildren<AudioSource>();
                audioSource.PlayOneShot(clip);
                curGob = ChatHornLeft;
            }
            else if (seat == App.GetGameData<GlobalData>().GetRightPlayerSeat)
            {
                ChatHornRight.SetActive(true);
                var audioSource = ChatHornRight.GetComponentInChildren<AudioSource>();
                audioSource.PlayOneShot(clip);
                curGob = ChatHornRight;
            }

            yield return new WaitForSeconds(lenth);
            if (curGob!=null) curGob.SetActive(false);
            PlayVoice();
          
        }



        internal struct VoiceInfo
        {
            /// <summary>
            /// 语音
            /// </summary>
            public AudioClip Clip;
            /// <summary>
            /// 座位号
            /// </summary>
            public int Seat;
            /// <summary>
            /// 长度
            /// </summary>
            public int Length;
        }

    }
}
