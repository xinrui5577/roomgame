using System.Collections;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;

namespace Assets.Scripts.Game.BlackJackCs
{
    public class BlackJackGameServer : YxGameServer
    {
        /// <summary>
        /// 显示指定玩家说话
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="cd"></param>
        /// <param name="isdouble"></param>
        public void Speaker(int speaker,float cd = -1,bool isdouble = false)
        {
            var gdata = App.GetGameData<BlackJackGameData>();
            var speakerPanel = gdata.GetPlayer<BjPlayerPanel>(speaker,true);
            if (speakerPanel.Info == null)
            {
                YxDebug.Log("座位上没有玩家!");
                return;
            }
            speakerPanel.BeginCountDown(cd <= 0 ? gdata.SpeakCd : cd);

            if (gdata.IsMyTurn(speaker))
            {
                App.GetGameManager<BlackJackGameManager>().SpeakMgr.SpeakViewActive(isdouble);
            }
        }

        #region 接收交互
        
        /// <summary>
        /// 发送语音
        /// </summary>
        /// <param name="url"></param>
        /// <param name="len"></param>
        /// <param name="seat"></param>
        public void SendVoiceChat(string url, int len, int seat)
        {
            SFSObject sfsObj = new SFSObject();
            sfsObj.PutUtfString("url", url);
            sfsObj.PutInt(RequestKey.KeySeat, seat);
            sfsObj.PutInt("len", len);
            SendFrameRequest("sound", sfsObj);
        }

        #endregion

        #region 发送交互

       

        /// <summary>
        /// 发送准备信息
        /// </summary>
        public void ReadyGame()
        {
            YxDebug.Log("发送准备!!");
            var key = GameKey + RequestCmd.Ready;
            SendFrameRequest(key, SFSObject.NewInstance());
        }
       


        /// <summary>
        /// 发送交互信息
        /// </summary>
        /// <param name="rt">游戏状态</param>
        /// <param name="data">下注金额键值对,key为gold</param>
        public void SendRequest(GameRequestType rt, IDictionary data =null)
        {
            YxDebug.Log("!!SendRequest == " + rt);
            var gdata = App.GameData;
            if (!App.GetGameData<BlackJackGameData>().IsGameInfo)
            {
                YxDebug.LogError("GameInfo还没有初始化!!");
                return;
            }
            SFSObject sfsObject = new SFSObject();
            //sfsObject 
            switch (rt)
            {
                case GameRequestType.ApplyBanker:
                case GameRequestType.ApplyQuit:

                    sfsObject.PutInt("type", (int)rt);
                    sfsObject.PutInt("seat", gdata.SelfSeat);
                    break;

                case GameRequestType.Ante:

                    sfsObject.PutInt("gold", data == null ? 0 : (int)data["gold"]);
                    sfsObject.PutInt("type", (int)rt);

                    break;

                case GameRequestType.Insurance:
                case GameRequestType.Stand:
                case GameRequestType.AddRate:
                case GameRequestType.Hit:
                    sfsObject.PutInt("type", (int)rt);
                    break;
                #endregion
            }

            YxDebug.Log("发送数据:");
            YxDebug.TraceSfsObject(sfsObject);

            SendGameRequest(sfsObject);
        }
        
        /// <summary>
        /// 发送表情聊天
        /// </summary>
        /// <param name="index"></param>
        public void SendUserTalk(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt(RequestKey.KeyExp, index);
            SendUserTalk(iobj, 1);
        }

        /// <summary>
        /// 发送文本聊天
        /// </summary>
        /// <param name="text">发送的消息内容</param>
        public void SendCommonText(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString(RequestKey.KeyText, text);
            SendUserTalk(iobj, 0);
        }

        public void SendPlayerInputString(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString(RequestKey.KeyText, text);
            SendUserTalk(iobj, 2);
        }

       

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="type">类型,0：文本消息(包括常用语) 1：常用语 2.后台状态（临时处理）</param>
        private void SendUserTalk(ISFSObject data, int type)
        {
            if (!App.GetGameData<BlackJackGameData>().IsGameInfo)
            {
                return;
            }
            data.PutInt(RequestKey.KeyType, type);
            data.PutInt(RequestKey.KeySeat, App.GetGameData<BlackJackGameData>().SelfSeat);
            SendFrameRequest(RequestCmd.UserTalk, data);
        }
    }
}