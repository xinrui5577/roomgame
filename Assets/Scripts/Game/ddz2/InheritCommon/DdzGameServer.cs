using System;
using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using com.yxixia.utile.YxDebug;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.InheritCommon
{
    public class DdzGameServer : YxGameServer
    {
 
  
        public override void Init(Dictionary<string, Action<ISFSObject>> responseDic)
        {
            //开房模式总结算
            responseDic[GameKey + "over"] = OnRoomGameOver;
            //投票解散
            responseDic["hup"] = OnHandsUp;
        }

       /// <summary>
        /// 接收投票消息 2发起 3同意 -1拒绝
        /// </summary>
        public void OnHandsUp(ISFSObject data)
        {
            var eventArgs = new DdzbaseEventArgs(data);
            Facade.EventCenter.DispatchEvent(GlobalConstKey.KeyOnHandUp, eventArgs);
        }


        /// <summary>
        /// 当开房模式游戏总结算
        /// </summary>
        public void OnRoomGameOver(ISFSObject data)
        {
            var eventArgs = new DdzbaseEventArgs(data);
            Facade.EventCenter.DispatchEvent(GlobalConstKey.KeyRoomGameOver, eventArgs);
        }


        //-------------------------------------------------------------------------------------------------


        /// <summary>
        /// 发送玩家准备网络请求
        /// </summary>
        public void SendPlayerReadyServCmd()
        {
            YxDebug.Log("发送准备");
            string key = GameKey + RequestCmd.Ready;
            SendFrameRequest(key, SFSObject.NewInstance());
        }

        /// <summary>
        /// 发送抢地主 
        /// </summary>
        /// <param name="sit">叫分的座位</param>
        /// <param name="value">叫分值</param>
        public void CallGameScore(int sit, int value)
        {
            var obj = new SFSObject();
            obj.PutInt(GlobalConstKey.C_Type, GlobalConstKey.TypeGrab);
            obj.PutInt(GlobalConstKey.C_Sit, sit);
            obj.PutInt(GlobalConstKey.C_Score, value);
            SendGameRequest(obj);
        }


        /// <summary>
        /// 发起解散投票 2发起 3同意 -1拒绝
        /// </summary>
        public void StartHandsUp(int yon)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString(GlobalConstKey.Cmd, NewRequestKey.KeyDismiss);
            iobj.PutInt(RequestKey.KeyType, yon);
            IRequest request = new ExtensionRequest(GlobalConstKey.Hup, iobj);
            SendRequest(request);
        }

        /// <summary>
        /// 发送自己不出请求
        /// </summary>
        public void TurnPass()
        {
            var obj = new SFSObject();
            obj.PutInt(GlobalConstKey.C_Type, GlobalConstKey.TypePass);
            obj.PutInt(GlobalConstKey.C_Sit, App.GetGameData<DdzGameData>().SelfSeat);
            SendGameRequest(obj);
        }

        /// <summary>
        /// 发送出牌信息
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="laiziRepCds"></param>
        /// <param name="ctype"></param>
        public void ThrowOutCard(int[] cards, int[] laiziRepCds, int ctype)
        {
            if (cards == null || cards.Length < 1)
            {
                YxDebug.LogError("out card is wrong!");
                return;
            }

            if (laiziRepCds == null || laiziRepCds.Length < 1) laiziRepCds = new[] { -1 };

            /*            int[] laizi = new int[1];
                        if (magic == -1)
                        {
                            laizi[0] = -1;
                        }
                        else laizi[0] = magic;*/
            SFSObject sfsObj = CreateSfsObj(GlobalConstKey.TypeOutCard, App.GetGameData<DdzGameData>().SelfSeat, cards, laiziRepCds, ctype);
            SendGameRequest(sfsObj);
        }

        public void SendAutoPlayState(bool state)
        {
            SFSObject sfsObj = CreateSfsObj(GlobalConstKey.TypeAuto);
            sfsObj.PutBool(NewRequestKey.KeyTf, state);
            SendGameRequest(sfsObj);
        }
   


        /// <summary>
        /// 解散房间
        /// </summary>
        public void DismissRoom()
        {
            YxDebug.Log("解散房间!");
            IRequest req = new ExtensionRequest("dissolve", new SFSObject());
            SendRequest(req);
        }

        /// <summary>
        /// 游戏没开始时 ，离开房间
        /// </summary>
        public void LeaveRoom()
        {
            IRequest req = new ExtensionRequest(NewRequestKey.KeyLeaveRoom, new SFSObject());
            SendRequest(req);
            App.QuitGame();
        }


        /// <summary>
        /// 发送语音
        /// </summary>
        /// <param name="url"></param>
        /// <param name="len"></param>
        /// <param name="seat"></param>
        public void SendVoiceChat(string url, int len, int seat)
        {
            var sfsObj = new SFSObject();
            sfsObj.PutUtfString("url", url);
            sfsObj.PutInt(RequestKey.KeySeat, seat);
            sfsObj.PutInt("len", len);
            SendFrameRequest("sound", sfsObj);
        }

        /// <summary>
        /// 发送聊天消息 表情
        /// </summary>
        /// <param name="index"></param>
        public void UserTalk(int index)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt("exp", index);
            iobj.PutInt("seat", App.GetGameData<DdzGameData>().SelfSeat);
            IRequest request = new ExtensionRequest(GameKey + RequestCmd.UserTalk, iobj);
            SendRequest(request);
        }

        /// <summary>
        /// 发送聊天消息 文字
        /// </summary>
        /// <param name="text"></param>
        public void UserTalk(string text)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutUtfString("text", text);
            var seat = App.GetGameData<DdzGameData>().SelfSeat;
            var sex = App.GetGameData<DdzGameData>().GetOnePlayerInfo(seat).SexI;
            if (sex != 1)
            {
                sex = 0;
            }
            iobj.PutInt("seat", seat);
            iobj.PutInt("sex", sex);
            IRequest request = new ExtensionRequest(GameKey + RequestCmd.UserTalk, iobj);
            SendRequest(request);
        }

        /// <summary>
        /// 发送加倍信息
        /// </summary>
        /// <param name="rate"></param>
        public void SendDouble(int rate)
        {
            ISFSObject iobj = new SFSObject();
            iobj.PutInt("type", GlobalConstKey.TypeDouble);
            iobj.PutInt("rate", rate);
            SendGameRequest(iobj);
        }



        private SFSObject CreateSfsObj(int type, int sit, int[] cards, int[] magicRepcds, int ctype)
        {
            SFSObject obj = CreateSfsObj(type, sit);
            obj.PutIntArray(GlobalConstKey.C_Cards, cards);
            if (magicRepcds[0] != -1)
            {
                YxDebug.LogError("癞子代表的牌=======》" + magicRepcds[0] + magicRepcds.Length);
                obj.PutIntArray(GlobalConstKey.C_Magic, magicRepcds);
            }
            obj.PutInt("ctype", ctype);
            return obj;
        }

        private SFSObject CreateSfsObj(int type, int sit)
        {
            var obj = CreateSfsObj(type);
            obj.PutInt(GlobalConstKey.C_Sit, sit);
            return obj;
        }

        private SFSObject CreateSfsObj(int type)
        {
            var obj = new SFSObject();
            obj.PutInt(GlobalConstKey.C_Type, type);
            return obj;
        }
    }
}
