using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager;
using com.yx.chatsystem;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Chat
{
    public class SocialContentItem : YxView
    {
        [Tooltip("头像信息")]
        public BaseSocialHeadItem UserHead;
        [Tooltip("聊天内容")]
        public YxBaseLabelAdapter TextContent;
        [Tooltip("表情")]
        public YxBaseSpriteAdapter Phiz;
        [Tooltip("图片")]
        public YxBaseTextureAdapter Image;
        [Tooltip("语音时间")]
        public YxBaseLabelAdapter VoiceTime;
        [Tooltip("类型容器")]
        public GameObject[] TypeContainer;
        [Tooltip("播放语音事件")]
        public List<EventDelegate> PlayVoiceAction = new List<EventDelegate>();
        [Tooltip("语音初始化事件")]
        public List<EventDelegate> VoiceInitAction = new List<EventDelegate>();
        [Tooltip("图片加载事件")]
        public List<EventDelegate> ImageUpdateAction = new List<EventDelegate>();
        [Tooltip("聊天系统组件")]
        public YxVoiceChatSystem ChatSystem;
        [Tooltip("图片最大宽度限制")]
        public Vector2 ImageMaxSize=new Vector2(650,250);
        [Tooltip("表情是否需要动画")]
        public bool PhizNeedAnimation = false;

        [Tooltip("是否为当前玩家")]
        [HideInInspector]
        public bool IsSelf;

        private SocialMessageManager _manager;
        protected SocialMessageManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = Facade.Instance<SocialMessageManager>().InitManager();
                }
                return _manager;
            }
        }
        /// <summary>
        /// 语音长度
        /// </summary>
        [HideInInspector]
        public int VoiceLength;
        /// <summary>
        /// 是否在播放
        /// </summary>
        [HideInInspector]
        public bool VoicePlaying;
        /// <summary>
        /// 语音地址
        /// </summary>
        private string _voiceUrl;
        /// <summary>
        /// 当前聊天内容类型
        /// </summary>
        TalkContentType _curType;

        public Transform PhizContainer;

        public override void OnDestroy()
        {
            if (Facade.HasInstance<SocialMessageManager>())
            {
                if (_curType==TalkContentType.Voice)
                {
                    Manager.RemoveLocalEventListener<int>(SocialTools.KeyMessageContent, PlayVoiceContent);
                }
            }
            base.OnDestroy();
        }
        private string _talkContent;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data is IDictionary)
            {
                _talkContent = null;
                Dictionary<string, object> contentData= Data as Dictionary<string, object>;
                if (contentData != null)
                {
                    contentData.TryGetValueWitheKey(out IdCode, SocialTools.KeyId);
                    Dictionary<string, object> memberInfo;
                    contentData.TryGetValueWitheKey(out memberInfo, SocialTools.KeyMemberInfo);
                    UserHead.UpdateView(new SocialHeadData(memberInfo));
                    int contentType;
                    contentData.TryGetValueWitheKey(out contentType, SocialTools.KeyMessageContentType);
                    _curType = (TalkContentType) contentType;
                    
                    contentData.TryGetValueWitheKey(out _talkContent, SocialTools.KeyMessageContent);
                    for (int i = 0,length= TypeContainer.Length; i < length; i++)
                    {
                        TypeContainer[i].TrySetComponentValue(contentType==i);
                    }
                    switch (_curType)
                    {
                        case TalkContentType.Text:
                            TextContent.TrySetComponentValue(_talkContent);
                            break;
                        case TalkContentType.Phiz:
                            SetPhiz(_talkContent);
                            break;
                        case TalkContentType.Voice:
                            Manager.AddLocalEventListeners<int>(SocialTools.KeyMessageContent, PlayVoiceContent);
                            VoicePlaying = false;
                            SocialTools.DecodeVoiceData(_talkContent, out VoiceLength,out _voiceUrl);
                            VoiceTime.TrySetComponentValue(VoiceLength);
                            if (gameObject.activeInHierarchy)
                            {
                                StartCoroutine(VoiceInitAction.WaitExcuteCalls());
                            }
                            break;
                        case TalkContentType.Image:
                            if (Image)
                            { 
                                AsyncImage.Instance.GetAsyncImage(_talkContent, ((t2, code) =>
                                {
                                    var width = t2.width;
                                    var height = t2.height;
                                    var maxW = ImageMaxSize.x;
                                    var maxH = ImageMaxSize.y;
                                    var changW = width > maxW;
                                    var changH = height > maxH;

                                    if (changW || changH)
                                    {
                                        if (changW)
                                        {
                                            var rate = maxW / width;
                                            Image.Width = (int)maxW;
                                            Image.Height = (int)(height * rate);
                                        }
                                        if (changH)
                                        {
                                            var rate = maxH / height;
                                            Image.Width = (int)(width * rate);
                                            Image.Height = (int)maxH;
                                        }
                                    }
                                    else
                                    {
                                        Image.Snap = true;
                                    }
                                    
                                    Image.SetTexture(t2);
                                    contentData.TryGetValueWitheKey(out IsSelf, SocialTools.KeyIsSelf);
                                    if (gameObject.activeInHierarchy)
                                    {
                                        StartCoroutine(ImageUpdateAction.WaitExcuteCalls(true));
                                    }
                                }));
                            }
                            break;
                    }
                }
            }
        }

        private void SetPhiz(string talkContent)
        {
            var pre = ResourceManager.LoadAsset(App.GameKey, talkContent, talkContent);
            if (PhizNeedAnimation && pre != null)
            {
                YxWindowUtils.CreateGameObject(pre, Phiz.transform);
                Phiz.EnableSprite(false);
                return;
            }
            Phiz.TrySetComponentValue(talkContent);
        }

        /// <summary>
        /// 播放语音消息内容
        /// </summary>
        /// <param name="code"></param>
        private void PlayVoiceContent(int code)
        {
            if (_curType!= TalkContentType.Voice)
            {
                return;
            }
            if (VoicePlaying)
            {
                VoicePlaying = false;
                FreshVoiceState();
            }
            else
            {
                if (code == IdCode)
                {
                    VoicePlaying = true;
                    FreshVoiceState();
                }
            }
        }
        /// <summary>
        /// 播放语音过程
        /// </summary>
        /// <returns></returns>
        IEnumerator WaitPlayVoice()
        {
            yield return new WaitForSeconds(VoiceLength);
            VoicePlaying = false;
            FreshVoiceState();
        }
        /// <summary>
        /// 播放语音
        /// </summary>
        public void PlayVoice()
        {
            if (gameObject.activeInHierarchy)
            {
                if (ChatSystem)
                {
                    ChatSystem.DownloadRecord(_voiceUrl, delegate (AudioClip audioSource)
                    {
                        VoicePlaying = true;
                        StartCoroutine(Facade.Instance<MusicManager>().PlaySpeaker(audioSource, Vector3.zero, VoiceLength));
                        StartCoroutine(WaitPlayVoice());
                    });
                }
            }
        }
        /// <summary>
        /// 停止语音
        /// </summary>
        public void StopVoice()
        {
            Facade.Instance<MusicManager>().Stop();
        }
        /// <summary>
        /// 刷新语音状态
        /// </summary>
        private void FreshVoiceState()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(PlayVoiceAction.WaitExcuteCalls());
            }
        }
        /// <summary>
        /// 点击播放语音
        /// </summary>
        public void OnClickPlayVoice()
        {
            Manager.DispatchLocalEvent(SocialTools.KeyMessageContent,IdCode);
        }

        /// <summary>
        /// 语音背景基本长度
        /// </summary>
        [HideInInspector]
        public int VoiceBaseLength = 100;
        /// <summary>
        /// 适应图片长度
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sprite"></param>
        public void FitSpriteLengthWithData(int data,UISprite sprite)
        {
            sprite.width = VoiceBaseLength + data;
        }
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="play"></param>
        /// <param name="anim"></param>
        public void PlaySpriteAni(bool play,UISpriteAnimation anim)
        {
            anim.enabled = play;
            if (play)
            {
                anim.Play();
            }
            else
            {
                anim.Pause();
                anim.ResetToBeginning();
            }
        }

        public void OnOpenTexturePreviewWindow(YxWindow mainWindow)
        {
            if (string.IsNullOrEmpty(_talkContent)) { return; }
            var win = mainWindow.CreateChildWindow("TexturePreviewWindow");
            win.UpdateView(_talkContent);
        }

    }
}
