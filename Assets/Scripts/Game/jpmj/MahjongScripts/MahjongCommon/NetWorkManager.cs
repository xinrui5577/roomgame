using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Controller;
using YxFramwork.Enums;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{
    public class NetWorkManager : YxGameServer
    {
        protected TableData table;
        protected ShareContrl _shareContrl = new ShareContrl();
        protected string _soundKey;

        protected bool _isFristGetInCard;
        protected bool _isGameInfoBack;

        protected float _pauseTime;
        protected bool _isResponsePause;
        protected Coroutine DelayCoroutine;
        protected List<ISFSObject> DelayResponse = new List<ISFSObject>();

        public bool BuzhangState;

        protected override void OnAwake()
        {
            //初始化游戏里必要的数据
            table = gameObject.GetComponent<TableData>();

            _shareContrl.Init(table);

            RegistEvent();

            var obj = GameObject.Find("5_2");
            if (obj != null) Destroy(obj);

#pragma warning disable 612,618
            Application.RegisterLogCallback(HandleLog);
#pragma warning restore 612, 618
        }

        /// <summary>
        /// 异常时强制重连
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="stacktrace"></param>
        /// <param name="type"></param>
        private void HandleLog(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                DoSendRejoinGame();
            }
        }

        /// <summary>
        /// 强制重连刷新数据用
        /// </summary>
        public void DoSendRejoinGame()
        {
            ShouldRejoin();
        }

        [SerializeField]
        protected string PauseBackInfo = "刷新数据中......";
        [SerializeField]
        protected float PauseBackDelaytime = 2f;

        public bool NeedAutoRejoin = true;
        void OnApplicationPause(bool isPause)
        {

            if (!isPause)
            {
                StartCoroutine(RejoinGameFinish());
            }

        }
        /// <summary>
        /// 切换回来刷新数
        /// </summary>
        /// <param name="msgbox"></param>
        /// <returns></returns>
        private IEnumerator RejoinGameFinish()
        {
            yield return new WaitForSeconds(PauseBackDelaytime);
            if (NeedAutoRejoin)
            {
                ShouldRejoin();
            }
        }

        private void ShouldRejoin()
        {
            SendReJoinGame();
        }

        protected virtual void RegistEvent()
        {
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnOutMahjong, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnChiClick, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnPengClick, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnGangClick, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnHuClick, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnTingClick, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnGuoClick, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnUserReady, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnGameRestart, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnGameContinue, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnDismissRoom, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnUserTalk, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnUserDetail, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnTotalResult, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnVoiceUpload, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.GetNeedCard, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnTingPai, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnChangeRoom, OnChangeRoom);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.PauseResponse, OnPauseResponse);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnXJFDClick, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnSendXJFD, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnZiDongDaPai, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnQueryHuLish, OnQueryHuList);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnJiaPiaoClick, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.ReJionShowBao, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnContinue, OnContinue);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnSendLiangPai, OnRecvEvent);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnStartTing, OnClickStartTing);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnJiamaByType, OnJiamaByType);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnJueGang, OnJueGang);
            EventDispatch.Instance.RegisteEvent((int)NetEventId.OnPushNetRespons, OnPushNetRespones);
        }

        private void OnPushNetRespones(int eventId, EventData data)
        {
            var sfsOjb = (SFSObject)data.data1;
            OnServerResponse(sfsOjb);
        }

        protected virtual void OnContinue(int eventId, EventData data)
        {
            table.resetUserDetail();
        }

        public override void Init(Dictionary<string, Action<ISFSObject>> callBackDic)
        {
            callBackDic[GameKey + RequestCmd.JoinRoom] = OnUserJoinRoom;
            callBackDic[RequestCmd.UserTalk] = OnUserTalk;
            callBackDic[GameKey + RequestKeyOther.RollDICE] = OnRollDice;
            callBackDic[GameKey + RequestCmd.Ready] = OnGameReady;
            callBackDic["hup"] = OnHandsUp;
            callBackDic[GameKey + "over"] = OnGameOver;
            callBackDic["sound"] = OnUserSpeak;
            callBackDic["locat"] = OnReciveGPSInfo;
            HotRejoin = true;
        }

        //GPS       
        private void OnReciveGPSInfo(ISFSObject data)
        {
            table.OnReciveGPSInfo(data);
        }

        /// <summary>
        /// 网络接收 服务器给的数据
        /// </summary>
        /// -----------------------------------------------------------------------------
        public virtual void OnGetGameInfo(ISFSObject gameInfo)
        {
            table.SetTableData(gameInfo);
            table.SetGameStatus(EnGameStatus.None);
            table.SetGameStatus(EnGameStatus.GameFree);
            //正常进入游戏时 清除保存在本地的信息
            CpgLocalSave.Instance.Cleare();
            _isFristGetInCard = true;
            _isGameInfoBack = true;

            App.GameData.GStatus = YxEGameStatus.Normal;

            GetSoundKey();
        }

        public virtual void OnGetRejoinData(ISFSObject data)
        {
            StopAllCoroutines();
            table.Reset();
            table.ChangeStatusToFree();
            table.SetTableData(data);
            table.SetGameStatus(EnGameStatus.GamePlay);

            _isFristGetInCard = false;
            _isGameInfoBack = true;

            App.GameData.GStatus = YxEGameStatus.Play;
            _isResponsePause = false;
            GetSoundKey();
        }

        protected void OnHandsUp(ISFSObject obj)
        {
            table.OnHandUp(obj);
        }

        protected void OnGameOver(ISFSObject obj)
        {
            App.GameData.GStatus = YxEGameStatus.Over;
            table.OnGameOver(obj);
        }

        protected void OnUserTalk(ISFSObject obj)
        {
            table.OnUserTalk(obj);
        }

        public void OnUserSpeak(ISFSObject param)
        {
            table.OnUserSpeak(param);
        }

        public virtual void OnServerResponse(ISFSObject data)
        {
            if (!_isGameInfoBack || App.GameData.GStatus == YxEGameStatus.Over)
                return;

            if (_isResponsePause)
            {
                DelayResponse.Add(data);
                return;
            }

            ServerResponseDispatch(data);
        }

        protected virtual IEnumerator DelayResponseDispatch()
        {
            while (true)
            {
                yield return DelayResponseTime();

                YxDebug.Log("消息延迟时间结束~~~~~~~~~ ");
                if (DelayResponse.Count == 0)
                    break;

                var first = DelayResponse[0];

                ServerResponseDispatch(first);

                DelayResponse.RemoveAt(0);

                if (DelayResponse.Count == 0)
                    break;
            }

            _isResponsePause = false;
            YxDebug.Log("消息延迟结束回到正常收发消息~~~~~~~~~ ");
        }

        protected virtual IEnumerator DelayResponseTime()
        {
            yield return new WaitForSeconds(_pauseTime);
            _pauseTime = 0;
        }


        private void OnPauseResponse(int eventId, EventData data)
        {
            _pauseTime = (float)data.data1;
            if (!_isResponsePause)
            {
                YxDebug.Log("开启延迟消息~~~~~~~~~ 时间" + _pauseTime);
                _isResponsePause = true;
                StartCoroutine(DelayResponseDispatch());
            }
        }

        protected virtual void ServerResponseDispatch(ISFSObject data)
        {
            int type = data.GetInt(RequestKey.KeyType);
            switch (type)
            {
                case MjRequestData.MJRequestTypeAlloCate:           //发牌
                    table.OnSendCard(data);
                    break;
                case MjRequestData.MjRequestTypeCheckCards:
                    var cards = data.GetIntArray("cards");
                    UtilFunc.OutPutArray(cards, "错误后同步的手牌！");
                    break;
                case MjRequestData.MJRequestTypeGetInCard:
                    if (_isFristGetInCard)
                    {
                        _isFristGetInCard = false;
                        table.SetGameStatus(EnGameStatus.GamePlay);
                    }
                    table.OnGetInCard(data);
                    break;
                case MjRequestData.MJRequestTypeThrowoutCard:
                    table.OnThrowCard(data);
                    break;
                case MjRequestData.MJReqTypeLastCd:
                case MjRequestData.MJReqTypeZiMo:
                case MjRequestData.MJRequestTypeHu:
                    table.OnHu(data, type);
                    //娱乐模式下 胡牌后 改回游戏状态
                    if (table.RoomInfo.RoomType == EnRoomType.YuLe)
                    {
                        App.GameData.GStatus = YxEGameStatus.Normal;
                    }
                    break;
                case MjRequestData.MJRequestTypeCPG:
                    table.OnCpg(data);
                    break;
                case MjRequestData.MjRequestTypeQiangGangHu:
                    break;
                case MjRequestData.MJRequestTypeSelfGang:
                    table.OnCpg(data);
                    break;
                case MjRequestData.MJRequestTypeXFG:
                    table.OnXFGang(data);
                    break;
                case MjRequestData.MJRequestTypeLiangDao:
                    table.ShowLiangPai(data);
                    break;
                case MjRequestData.MJOpreateType:
                    table.OnOperate(data);
                    break;
                case MjRequestData.MJRequestTypeTing:
                    table.OnTing(data);
                    break;
                case MjRequestData.MJRequestTypeSelectPiao:
                    table.OnShowJiaPiaoBtn(data);
                    break;
                case MjRequestData.MJRequestTypeShowPiao:
                    table.OnJiaPiao(data);
                    break;
                case MjRequestData.MJRequestTypeDan:
                    table.OnXJFD(data);
                    break;
                case MjRequestData.MJRequestTypeBao:
                    table.OnBao(data);
                    break;
                case MjRequestData.MJRequestTypeGetHuCards:
                    table.OnQueryHulist(data);
                    break;
                case MjRequestData.MJReponeseStartTing:
                    table.OnStartTing();
                    break;
                case MjRequestData.MJRequestStartTing:
                    table.StartTing(data);
                    break;
                case MjRequestData.MJRequestTypeJiaMa:
                    table.ShowJiamaPnl(data);
                    break;
                case MjRequestData.MJRequestTypeJiaMaFinish:
                    table.JiamaResult(data);
                    break;
                case MjRequestData.MJRequestJueGang:
                    table.OnAnJueGang(data);
                    break;
                default:
                    table.OnNetResponseEventBack(type, data);
                    break;
            }
        }

        public virtual void OnUserOut(int seat)
        {
            if (App.GameData.GStatus != YxEGameStatus.Over)
            {
                table.OnUserOut(seat);
            }
        }

        public virtual void OnUserIdle(int serverSeat)
        {
            if (App.GameData.GStatus != YxEGameStatus.Over)
            {
                table.OnUserIdle(serverSeat);
            }
        }

        public virtual void OnUserOnLine(int serverSeat)
        {
            if (App.GameData.GStatus != YxEGameStatus.Over)
            {
                table.OnUserOnline(serverSeat);
            }
        }

        public void OnUserJoinRoom(ISFSObject data)
        {
            ISFSObject user = data.GetSFSObject(RequestKey.KeyUser);
            table.OnUserJoinGame(user);
        }

        protected void OnRollDice(ISFSObject data)
        {
            table.SetGameStatus(EnGameStatus.GameSendCard);
            table.OnSaiziPoint(data);
        }

        protected virtual void OnGameReady(ISFSObject data)
        {
            //table.SetGameStatus(TableData.EnGameStatus.GameReady);
            int seat = data.GetInt(RequestKey.KeySeat);
            table.OnUserReady(seat);
        }

        ///-----------------------------------------------------------------------------

        /// <summary>
        /// 网络请求 从UI层发过来的请求
        /// </summary>
        ///-----------------------------------------------------------------------------
        protected virtual void OnRecvEvent(int eventId, EventData evn)
        {
            NetEventId id = (NetEventId)eventId;
            switch (id)
            {
                case NetEventId.OnOutMahjong:
                    OnOutMahjong(evn);
                    break;
                case NetEventId.OnChiClick:
                    OnChiClick(evn);
                    break;
                case NetEventId.OnPengClick:
                    OnPengClick(evn);
                    break;
                case NetEventId.OnGangClick:
                    OnGangClick(evn);
                    break;
                case NetEventId.OnHuClick:
                    OnHuClick(evn);
                    break;
                case NetEventId.OnTingClick:
                    OnTingClick(evn);
                    break;
                case NetEventId.OnXJFDClick:
                    OnXJFDClick(evn);
                    break;
                case NetEventId.OnGuoClick:
                    OutPutForGuo();
                    if (table.CurrSeat != table.PlayerSeat)
                        OnGuoClick(evn);
                    break;
                case NetEventId.OnUserReady:
                    OnUserReady(evn);
                    break;
                case NetEventId.OnGameRestart:
                    OnGameRestart(evn);
                    break;
                case NetEventId.OnGameContinue:
                    OnGameContinue(evn);
                    break;
                case NetEventId.OnDismissRoom:
                    OnDismissRoom(evn);
                    break;
                case NetEventId.OnUserDetail:
                    OnUserDetail(evn);
                    break;
                case NetEventId.OnTotalResult:
                    OnTotalResult(evn);
                    break;
                case NetEventId.OnUserTalk:
                    OnUserTalk(evn);
                    break;
                case NetEventId.OnVoiceUpload:
                    SendVoiceChat(evn);
                    break;
                case NetEventId.GetNeedCard:
                    GetNeedCard(evn);       //要牌
                    break;
                case NetEventId.OnTingPai:
                    OnTingPai(evn);
                    break;
                case NetEventId.OnSendLiangPai:
                    OnSendLiangPai(evn);
                    break;
                case NetEventId.OnSendXJFD:
                    OnSendXJFD(evn);
                    break;
                case NetEventId.OnZiDongDaPai:
                    OnZiDongDaPai(evn);
                    break;
                case NetEventId.OnJiaPiaoClick:
                    OnJiaPiao(evn);
                    break;
                case NetEventId.ReJionShowBao:
                    ReJionShowBao(evn);
                    break;
                case NetEventId.OnStartTing:
                    ReJionShowBao(evn);
                    break;

                default:
                    table.OnNetRequestEventBack(id, evn, this);
                    break;
            }
        }
        private void OnSendLiangPai(EventData evn)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeLiangDao);
            int[] arr = (int[])evn.data1;
            sfsObject.PutIntArray(RequestKey.KeyCardsArr, arr);

            SendGameRequest(sfsObject);
            table.StopAutoOutCard();

            sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeThrowoutCard);
            sfsObject.PutInt(RequestKey.KeyOpCard, (int)evn.data2);

            SendGameRequest(sfsObject);


        }

        protected void ReJionShowBao(EventData evn)
        {
            table.RejoinShowBao();
        }

        protected void OnZiDongDaPai(EventData evn)
        {
            table.ChangeZiDongDaPai();
        }

        protected virtual void OnDismissRoom(EventData data)
        {
            int type = (int)data.data1;
            ISFSObject iobj = new SFSObject();

            if (App.GameData.GStatus == YxEGameStatus.Normal)
            {
                YxDebug.Log("当前游戏状态 正常 直接退出游戏");

                //非茶馆开房
                Regex reg = new Regex("^[0-9]+$");
                Match ma = reg.Match(table.RoomInfo.TeaID);
                if (table.UserDatas[table.PlayerSeat].id == table.OwnerId && !ma.Success)
                {
                    SendFrameRequest("dissolve", iobj);
                }
                else
                {
                    EventDispatch.Dispatch((int)TableDataEventId.OnSendLeaveRoomState, new EventData(0));
                    App.QuitGame();
                }
            }
            else
            {
                YxDebug.Log("正在游戏状态 投票解散");
                //游戏过程中要解散房间
                iobj.PutUtfString("cmd", "dismiss");
                iobj.PutInt(RequestKey.KeyType, type);
                SendFrameRequest("hup", iobj);
            }
        }

        protected void OnOutMahjong(EventData evn)
        {
            table.StopAutoOutCard();

            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeThrowoutCard);
            sfsObject.PutInt(RequestKey.KeyOpCard, (int)evn.data1);

            SendGameRequest(sfsObject);
        }

        protected void OnTingPai(EventData evn)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeTing);
            sfsObject.PutInt(RequestKey.KeyOpCard, (int)evn.data1);

            SendGameRequest(sfsObject);
        }

        protected void OnSendXJFD(EventData evn)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeDan);
            sfsObject.PutIntArray(RequestKey.KeyCards, (int[])evn.data1);

            SendGameRequest(sfsObject);
        }

        protected void OnChiClick(EventData evn)
        {
            DVoidTArray<int> call = (array) =>
            {
                SFSObject sfsObject = SFSObject.NewInstance();
                sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeCPG);
                sfsObject.PutInt(RequestKeyOther.KeyTType, MjRequestData.MJOpreateTypeChi);
                sfsObject.PutIntArray(RequestKey.KeyCards, array);

                SendGameRequest(sfsObject);
            };
            table.GetChiData(call);
        }

        protected void OnPengClick(EventData evn)
        {
            DVoidTArray<int> call = (array) =>
            {
                SFSObject sfsObject = SFSObject.NewInstance();
                sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeCPG);
                sfsObject.PutInt(RequestKeyOther.KeyTType, MjRequestData.MJOpreateTypePeng);
                sfsObject.PutIntArray(RequestKey.KeyCards, array);
                SendGameRequest(sfsObject);
            };

            table.GetPengData(call);
        }

        protected void OnGangClick(EventData evn)
        {
            DVoidSfsObject call = (obj) =>
            {
                SendGameRequest(obj);
            };

            ISFSObject sfsObj = SFSObject.NewInstance();

            table.GetGangData(sfsObj, call);
        }

        protected void OnHuClick(EventData evn)
        {
            DVoidSfsObject call = (obj) =>
            {
                SendGameRequest(obj);
            };
            SFSObject sfsObject = SFSObject.NewInstance();

            table.GetHuData(sfsObject, call);
        }

        protected void OnTingClick(EventData evn)
        {
            table.GetTingData();
        }

        protected void OnXJFDClick(EventData evn)
        {
            table.ChooseXJFD();
        }

        protected void OnGuoClick(EventData evn)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeCPG);
            sfsObject.PutInt(RequestKeyOther.KeyTType, MjRequestData.MJOpreateTypeNone);
            SendGameRequest(sfsObject);
        }

        protected virtual void OutPutForGuo()
        {
            table.OutPutForGuo();
        }

        protected void OnUserReady(EventData evn)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            string key = GameKey + RequestCmd.Ready;
            SendFrameRequest(key, sfsObject);
        }

        protected virtual void OnGameRestart(EventData evn)
        {
            table.Reset();
            table.SetGameStatus(EnGameStatus.GameFree);
            //发送用户准备
            OnUserReady(null);
        }

        protected void OnGameContinue(EventData evn)
        {
            table.Reset();
            table.SetGameStatus(EnGameStatus.GameFree);

            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyRoomType, App.RoomId);
            //			sfsObject.PutUtfString ("-gtype", UtilData.RoomType);
            string key = GameKey + "continue";
            SendFrameRequest(key, sfsObject);

            //			var data = SFSObject.NewInstance();
            //			data.PutInt(RequestKey.KeyRoomType, gameType);
            //			IRequest request = new ExtensionRequest(GameKey + RequestCmd.QuickGame, data);
            //			YxDebug.Log("<color=#ff0000>{0}</color> Post Request-----------------", "RemoteCtrl", null, GameKey);
            //			YxDebug.Log(data.GetDump(true), "");
            //			YxWindowManager.ShowWaitFor();
            //			SfsManager.Send(request);

        }

        protected void OnUserDetail(EventData evn)
        {
            int seat = (int)evn.data1;
            table.OnUserDetail(seat);
        }

        protected void OnTotalResult(EventData evn)
        {
            table.OnTotalResult();
        }

        protected virtual void OnUserTalk(EventData evn)
        {
            //EnChatType type = (EnChatType)evn.data1;
            //SFSObject sfsObject = SFSObject.NewInstance();
            //sfsObject.PutInt("seat", table.PlayerSeat);
            //if (type == EnChatType.text)
            //{
            //    string text = (string)evn.data2;
            //    sfsObject.PutUtfString("text", text);
            //}
            //else
            //{
            //    int key = (int)evn.data2;
            //    if (type == EnChatType.exp)
            //        sfsObject.PutInt("exp", key + UtilDef.ExpPlush);
            //    else
            //        sfsObject.PutInt("exp", key + UtilDef.SortTalkPlush);
            //}

            //SendFrameRequest(GameKey + RequestCmd.UserTalk, sfsObject);

            EnChatType type = (EnChatType)evn.data1;
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt("seat", table.PlayerSeat);
            if (type == EnChatType.ani)
            {
                string username = (string)evn.data2;
                int key = (int)evn.data3;
                sfsObject.PutInt("ani", key);
                sfsObject.PutUtfString("name", username);
                SendFrameRequest(GameKey + RequestCmd.UserTalk, sfsObject);
                return;
            }

            if (type == EnChatType.text)
            {
                string text = (string)evn.data2;
                sfsObject.PutUtfString("text", text);
            }
            else
            {
                int key = (int)evn.data2;
                if (type == EnChatType.exp)
                    sfsObject.PutInt("exp", key + UtilDef.ExpPlush);
                else
                    sfsObject.PutInt("exp", key + UtilDef.SortTalkPlush);
            }

            SendFrameRequest(GameKey + RequestCmd.UserTalk, sfsObject);
        }

        protected void SendVoiceChat(EventData evn)
        {
            int len = (int)evn.data1;
            string url = (string)evn.data2;
            SFSObject sfsObj = new SFSObject();
            sfsObj.PutUtfString("url", url);
            sfsObj.PutInt(RequestKey.KeySeat, table.PlayerSeat);
            sfsObj.PutInt("len", len);
            SendFrameRequest("sound", sfsObj);
        }


        protected void GetNeedCard(EventData evn)
        {
            SFSObject sfsObj = new SFSObject();
            sfsObj.PutInt(RequestKey.KeyType, MjRequestData.MJReqGetNeedCard);
            sfsObj.PutInt(RequestKey.KeyCard, (int)evn.data1);
            SendGameRequest(sfsObj);
        }

        protected void OnChangeRoom(int eventid, EventData data)
        {
            ChangeRoom();
        }

        protected virtual void OnChangeRoom()
        {
            table.Reset();
            table.SetGameStatus(EnGameStatus.GameFree);
            table.resetUserDetail();
            table.OnOtherUserOut();
        }

        //-----------------------------------------------------------------------------
        //在游戏开始时 要获得当前的 key 
        protected void GetSoundKey()
        {
            Facade.Instance<TwManager>().SendAction
                (
                    "soundApi",
                    new Dictionary<string, object>(),
                    msg =>
                    {
                        var key = msg.ToString();
                        if (key != null)
                        {
                            _soundKey = key;
                            EventDispatch.Dispatch((int)UIEventId.OnGetSoundKey, new EventData(_soundKey));
                        }
                    },
                    false,
                    ErrMsg =>
                    {
                        var errDic = (IDictionary)ErrMsg;
                        string show = errDic["errorMessage"].ToString();
                    }, false
                );
        }

        protected void OnJiaPiao(EventData evn)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeShowPiao);
            sfsObject.PutInt("piao", (int)evn.data1);

            SendGameRequest(sfsObject);
        }

        //点听或游金牌，查询可胡的牌
        protected void OnQueryHuList(int eventId, EventData evn)
        {
            DVoidSfsObject call = (obj) =>
            {
                SendGameRequest(obj);
            };
            SFSObject sfsObject = SFSObject.NewInstance();

            table.RequestQueryHuList(sfsObject, call, evn);
        }

        protected void OnClickStartTing(int eventId, EventData evn)
        {
            bool result = (bool)evn.data1;

            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestStartTing);
            if (result)
                sfsObject.PutInt("ok", 0);

            SendGameRequest(sfsObject);
        }

        protected void OnJiamaByType(int eventId, EventData evn)
        {
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestTypeJiaMa);
            sfsObject.PutInt("jiaMa", (int)evn.data1);

            SendGameRequest(sfsObject);
        }

        //绝杠
        protected void OnJueGang(int eventId, EventData evn)
        {
            //DVoidTArray<int> call = (array) =>
            //{
            //    SFSObject sfsObject = SFSObject.NewInstance();
            //    sfsObject.PutInt(RequestKey.KeyType, MjRequestData.MJRequestJueGang);              
            //    sfsObject.PutIntArray(RequestKey.KeyCards, array);
            //    SendGameRequest(sfsObject);
            //};
            //table.GetJueGangData(call);

            DVoidSfsObject call = (obj) => { SendGameRequest(obj); };
            ISFSObject sfsObj = SFSObject.NewInstance();
            table.GetJueGangData(sfsObj, call);
        }

        //-----------------------------------------------------------------------------
        public void appChange()
        {
            App.QuitGame();
        }

        public void AssamblyLoadFinish()
        {
            EventDispatch.Dispatch((int)GameEventId.InitTable);
        }

        void OnDestroy()
        {
            EventDispatch.Instance.OnDestroy();
            CpgLocalSave.Instance.OnDestroy();
        }
    }
}
