using System;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Manager;
using com.yx.chatsystem;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Chat
{
    /// <summary>
    /// 聊天输入处理
    /// </summary>
    public class SocialChatInputView : BaseSocialView
    {
        [Tooltip(" 语音系统")]
        public YxVoiceChatSystem ChatSystem;
        [Tooltip("表情图集")]
        public UIAtlas Atlas;
        [Tooltip("表情prefab")]
        public YxView PhizPrefab;
        [Tooltip("表情 grid")]
        public UIGrid PhizGrid;
        protected override void OnAwake()
        {
            base.OnAwake();
            InitPhizs();
        }

        /// <summary>
        /// 初始化图集
        /// </summary>
        void InitPhizs()
        {
            if (Atlas&& PhizGrid&& PhizPrefab)
            {
                var list = Atlas.spriteList;
                var count = list.Count;

                if (list.Count > 0)
                {
                    for (int i = 0; i < count; i++)
                    {
                        var itemView=PhizGrid.transform.GetChildView(i, PhizPrefab);
                        if (itemView!=null&& itemView.GetComponent<UISprite>())
                        {
                            itemView.name = list[i].name;
                            itemView.GetComponent<UISprite>().spriteName = itemView.name;
                        }
                    }
                }
            }
        }

        /// <summary>开始语音</summary>
        public void OnStartSpeakerVoice()
        {
            if (ChatSystem)
            {
                ChatSystem.OnStartRecord();
            }
        }

        /// <summary>结束语音</summary>
        public void OnEndSpeakerVoice()
        {
            if (ChatSystem)
            {
                var length=ChatSystem.StopRecord(ChatSystem.RecordMinSecLenght);
                ChatSystem.UploadRecord(SocialSetting.UpLoadUrl.CombinePath(SocialTools.KeyVoiceUpLoadAction), length,
                    delegate (string downUrl)
                    {
                        var uploadUrl = SocialTools.GetUpLoadUrl(downUrl);
                        if (!string.IsNullOrEmpty(uploadUrl))
                        {
                            TalkCenter.SendTalkInfo(SocialTools.EncodeVoiceData(length, uploadUrl), TalkContentType.Voice);
                        }
                    });
            }
        }
        /// <summary>
        /// 发送聊天消息文本
        /// </summary>
        /// <param name="content"></param>
        public void SendInputTextContent(string content)
        {
            if (String.IsNullOrEmpty(content))
            {
                return;
            }
            TalkCenter.SendTalkInfo(content,TalkContentType.Text);
        }
        /// <summary>
        /// 发送表情
        /// </summary>
        public void SendPhiz(string index)
        {
            if (String.IsNullOrEmpty(index))
            {
                return;
            }
            TalkCenter.SendTalkInfo(index,TalkContentType.Phiz);
        }
        /// <summary>
        /// 发送image
        /// </summary>
        /// <param name="url"></param>
        public void SendImage(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return;
            }
            TalkCenter.SendTalkInfo(url, TalkContentType.Image);
        }

        public void SelectImage()
        {

#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
                        if (NativeGallery.IsMediaPickerBusy())
            {
                Debug.LogError("IsMediaPickerBusy");
                return;
            }
            PickImage();
    return;
#endif
        }
//#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
        private void PickImage()
        {
            TalkCenter.SelectImageUpLoad(delegate (string uploadUrl)
            {
                TalkCenter.SendTalkInfo(uploadUrl, TalkContentType.Image);
            });
        }
//#endif
    }
}