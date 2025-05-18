/** 
 *文件名称:     ChatSystem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-17 
 *描述:    
 *历史记录:     这个是聊天系统，负责游戏中聊天相关的消息发送与接收
*/

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Game.GameCtrl;
using Assets.Scripts.Game.lyzz2d.Utils;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using com.yx.chatsystem;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.lyzz2d.Game.Component
{
    public class ChatControl : MonoSingleton<ChatControl>
    {
        /// <summary>
        ///     常用语标识
        /// </summary>
        private readonly string _conmonTag = "#common:";

        /// <summary>
        ///     聊天系统
        /// </summary>
        public YxVoiceChatSystem ChatSystem;

        /// <summary>
        ///     语音缓存
        /// </summary>
        private List<TalkData> clips;

        public override void Awake()
        {
            base.Awake();
            clips = new List<TalkData>();
            RequestSoundkey();
        }

        public void OnUserTalk(ISFSObject param)
        {
            int seat;
            int index;
            string text;
            int type;
            bool isOnline;
            GameTools.TryGetValueWitheKey(param, out type, RequestKey.KeyType);
            GameTools.TryGetValueWitheKey(param, out seat, RequestKey.KeySeat);
            var players = App.GetGameManager<Lyzz2DGameManager>().Players;
            if (players == null || players[seat] == null)
            {
                return;
            }
            switch (type)
            {
                case 0:
                    string str;
                    GameTools.TryGetValueWitheKey(param, out text, RequestKey.KeyText);
                    if (text.Contains(_conmonTag))
                    {
                        var commonIndex = int.Parse(text.Replace(_conmonTag, null));
                        str = ConstantData.Common[commonIndex];
                        Facade.Instance<MusicManager>()
                            .Play(GameTools.GetNormalTalkVoice(players[seat].UserInfo.Sex, commonIndex));
                    }
                    else
                    {
                        str = text;
                    }
                    players[seat].CurrentInfoPanel.ShowTalkContent(str);
                    break;
                case 1:
                    GameTools.TryGetValueWitheKey(param, out index, RequestKey.KeyExp);
                    players[seat].CurrentInfoPanel.ShowPhizContent(index);
                    break;
                case 2:
                    GameTools.TryGetValueWitheKey(param, out isOnline, RequestKey.KeyIsOnLine);
                    players[seat].UserInfo.IsOnLine = isOnline;
                    players[seat].SetUserHead(isOnline, true);
                    break;
            }
        }

        public void SendUserPhizTalk(int index)
        {
            App.GetRServer<Lyzz2DGameServer>().SendUserTalk(index);
        }

        #region 语音

        private void RequestSoundkey()
        {
            Facade.Instance<TwManager>().SendAction
                (
                    RequestCmd.SoundApi,
                    new Dictionary<string, object>(),
                    msg =>
                    {
                        var key = msg.ToString();
                        if (key != null)
                        {
                            App.GetGameData<Lyzz2DGlobalData>().SoundKey = key;
                            var info = ChatSystem.UploadInfoData;
                            info.Url = App.GetGameData<Lyzz2DGlobalData>().SoundKey;
                            info.OnFail = OnUploadVoiceFail;
                            info.OnFinish = OnUpLoadVoiceSuccess;
                        }
                    },
                    false,
                    ErrMsg =>
                    {
                        var errDic = (IDictionary) ErrMsg;
                        var show = errDic["errorMessage"].ToString();
                    }
                );
        }

        private void OnUpLoadVoiceSuccess(string url)
        {
            var lenth = Instance.ChatSystem.UploadInfoData.LengthSce;
            App.GetRServer<Lyzz2DGameServer>()
                .SendVoiceChat(url, lenth, App.GetGameManager<Lyzz2DGameManager>().SelfPlayer.UserInfo.Seat);
        }

        private void OnUploadVoiceFail(string error)
        {
            if (string.IsNullOrEmpty(error))
            {
                return;
            }
            YxDebug.LogError(error);
        }

        /// <summary>
        ///     玩家聊天
        /// </summary>
        /// <param name="param"></param>
        public void OnUserSpeak(ISFSObject param)
        {
            var url = param.GetUtfString("url");
            var seat = param.ContainsKey(RequestKey.KeySeat) ? param.GetInt("seat") : 0;
            var len = param.ContainsKey("len") ? param.GetInt("len") : 1;
            ChatSystem.DownloadRecord(
                url,
                clip =>
                {
                    if (clips == null)
                    {
                        clips = new List<TalkData>();
                    }
                    clips.Add(new TalkData
                    {
                        Clip = clip,
                        Seat = seat,
                        Lenth = len
                    });
                    PlayVoice();
                },
                err => { YxDebug.LogError("下载音频失败"); }
                );
        }

        private void PlayVoice()
        {
            var data = clips[0];
            App.GetGameManager<Lyzz2DGameManager>().Players[data.Seat].OnPlayeTalk(data.Clip, data.Lenth);
            Invoke("OnFinish", data.Lenth);
        }

        private void OnFinish()
        {
            clips.Remove(clips[0]);
            CancelInvoke("OnFinish");
            if (clips.Count == 0)
            {
            }
            else
            {
                PlayVoice();
            }
        }

        #endregion
    }
}