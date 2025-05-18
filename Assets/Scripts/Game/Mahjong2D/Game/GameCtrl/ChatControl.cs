/** 
 *文件名称:     ChatSystem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-17 
 *描述:    
 *历史记录:     这个是聊天系统，负责游戏中聊天相关的消息发送与接收
*/

using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Request;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Player;
using com.yx.chatsystem;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class ChatControl : MonoSingleton<ChatControl>
    {
        /// <summary>
        /// 聊天系统
        /// </summary>
        public YxVoiceChatSystem ChatSystem;
        /// <summary>
        /// 语音缓存
        /// </summary>
        private List<TalkData> _clips;
        /// <summary>
        /// 常用语标识
        /// </summary>
        private string _conmonTag = "#common:";

        /// <summary>
        /// 公用快捷语
        /// </summary>
        public string[] CommonSpeakStrings =
        {
            "大家好,很高心见到各位",
            "和你合作真是太愉快了",
            "快点啊,等到花儿都都谢了",
            "你的牌打的也太好了",
            "不要吵了,不要吵了,专心玩游戏吧",
            "怎么又断线,网络怎么这么差啊",
            "不好意思,我要离开一会",
            "不要走,决战到天亮",
            "你是MM还是GG",
            "交个朋友吧,能告诉我联系方式吗",
            "再见了,我会想念大家的"
        };


        public override void Awake()
        {
            base.Awake();
            _clips = new List<TalkData>();
        }

        #region 语音
        public void RequestSoundkey()
        {
            if(Facade.Instance<TwManager>()==null)
            {
                return;
            }
            if (string.IsNullOrEmpty(App.GameKey))
            {
                return;
            }
            Facade.Instance<TwManager>().SendAction
            (
                 RequestCmd.SoundApi,
                 new Dictionary<string, object>(),
                 msg =>
                 {
                     var key = msg.ToString();
                     {
                         App.GetGameData<Mahjong2DGameData>().SoundKey = key;
                         UploadInfo info =ChatSystem.UploadInfoData;
                         info.Url = App.GetGameData<Mahjong2DGameData>().SoundKey;
                         info.OnFail = OnUploadVoiceFail;
                         info.OnFinish = OnUpLoadVoiceSuccess;
                     }
                 },
                 false
                );
        }
        private void OnUpLoadVoiceSuccess(string url)
        {
            int lenth = Instance.ChatSystem.UploadInfoData.LengthSce;
            App.GetRServer<Mahjong2DGameServer>().SendVoiceChat(url, lenth, App.GetGameManager<Mahjong2DGameManager>().SelfPlayer.UserInfo.Seat);
        }
        private void OnUploadVoiceFail(string error)
        {
            if(string.IsNullOrEmpty(error))
            {
                Debug.LogError("语音的失败回调是null，什么情况");
                return;
            }
            YxDebug.LogError(error);
        }

        /// <summary>
        /// 玩家聊天
        /// </summary>
        /// <param name="param"></param>
        public void OnUserSpeak(ISFSObject param)
        {
            string url = param.GetUtfString("url");
            int seat = param.ContainsKey(RequestKey.KeySeat) ? param.GetInt("seat") : 0;
            int len = param.ContainsKey("len") ? param.GetInt("len") : 1;
            ChatSystem.DownloadRecord(
                url,
                clip =>
                {
                    if (_clips==null)
                    {
                        _clips=new List<TalkData>();
                    }
                    _clips.Add(new TalkData()
                    {
                        Clip = clip,
                        Seat = seat,
                        Lenth = len
                    });
                    PlayVoice();
                },
                err =>
                {
                    YxDebug.LogError("下载音频失败");
                }
                );
        }

        private void PlayVoice()
        {
            TalkData data = _clips[0];
            App.GetGameManager<Mahjong2DGameManager>().Players[data.Seat].OnPlayeTalk(data.Clip, data.Lenth);
            Invoke("OnFinish", data.Lenth);
        }
        void OnFinish()
        {
            _clips.Remove(_clips[0]);
            CancelInvoke("OnFinish");
            if (_clips.Count == 0)
            {
                return;
            }
            else
            {
                PlayVoice();
            }
        }
        #endregion
        public void OnUserTalk(ISFSObject param)
        {
            int seat;
            int index;
            string text;
            string strTalk;
            GameTools.TryGetValueWitheKey(param, out seat, RequestKey.KeySeat);
            MahjongPlayer[] players = App.GetGameManager<Mahjong2DGameManager>().Players;
            if (players == null || players.Length == 0 || players[seat] == null)
            {
                return;
            }
            int type;
            GameTools.TryGetValueWitheKey(param, out type, RequestKey.KeyType);
            switch (type)
            {
                case 0:
                    string str;
                    GameTools.TryGetValueWitheKey(param, out text, RequestKey.KeyText);
                    if (text.Contains(_conmonTag))
                    {
                        int commonIndex = int.Parse(text.Replace(_conmonTag, null));
                        str = CommonSpeakStrings[commonIndex];
                        Facade.Instance<MusicManager>().Play(GameTools.GetNormalTalkVoice(players[seat].UserInfo.Sex,
                                                                                commonIndex));
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
            }
        }
        public void SendUserPhizTalk(int index)
        {  
            if (App.GameKey.Equals("ykmj"))
            {
                App.GetRServer<Mahjong2DGameServer>().SendYkUserTalk(index);
            }
            else
            {
                App.GetRServer<Mahjong2DGameServer>().SendUserTalk(index);
            }
        }
    }
}
