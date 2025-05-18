using UnityEngine;
using System.Collections;
using YxFramwork.Manager;
using Sfs2X.Entities.Data;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;
using YxFramwork.View;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.LXGameScripts
{
    public class LXGameManager : YxGameManager
    {
        [HideInInspector]
        public bool CanQuit;
        protected bool IsTrusteeship = false;//托管
        protected override void OnAwake()
        {
            base.OnAwake();
            ResisteEvent();
        }
        protected virtual void ResisteEvent()
        {
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.OnTrusteeshipOpen, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.OnCancelTrusteeship, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.WhenIconStop, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.OnCloseBtnClick, Resiste);
        }

        protected virtual void Resiste(int id, EventData data)
        {
            EventID.GameEventId type = (EventID.GameEventId)id;
            switch (type)
            {
                case EventID.GameEventId.OnTrusteeshipOpen:
                    CanQuit = false;
                    IsTrusteeship = true;
                    EventDispatch.Dispatch((int)EventID.GameEventId.ChangeButtonIcon, new EventData(0, true));
                    StartCoroutine("StartGameByTrust");
                    break;
                case EventID.GameEventId.OnCancelTrusteeship:
                    CanQuit = true;
                    IsTrusteeship = false;
                    StopCoroutine("ReStartGameBySelf");
                    EventDispatch.Dispatch((int)EventID.GameEventId.ChangeButtonIcon, new EventData(0, false));
                    break;
                case EventID.GameEventId.WhenIconStop:
                    OnStopBySelf();
                    break;
                case EventID.GameEventId.OnCloseBtnClick:
                    OnQuitGameClick();
                    break;
                default:
                    ResisteOther(id, data);
                    break;
            }
        }

        protected virtual void ResisteOther(int id, EventData data)
        {
            Debug.Log("没有找到注册事件");
        }

        #region 一些没有代码的重写
        public override void OnGetGameInfo(ISFSObject gameInfo)
        {
            CanQuit = true;
        }

        public override void OnGetRejoinInfo(ISFSObject gameInfo)
        {

        }

        public override void GameResponseStatus(int type, ISFSObject response)
        {
            YxDebug.Log("------> RemoteServer: OnServerResponse() CMDID = rqst and type = " + type + "" + "! \n");
            switch ((RequestCmd)type)
            {
                case RequestCmd.CmdStartGame:
                    YxDebug.Log("开始游戏");
                    CanQuit = false;
                    Facade.Instance<MusicManager>().Stop();
                    App.GetGameData<OverallData>().SetResponseData(response);
                    StopAllCoroutines();
                    EventDispatch.Dispatch((int)EventID.GameEventId.PlayInsertCoinsAnim);
                    EventDispatch.Dispatch((int)EventID.GameEventId.StartRollJetton);
                    EventDispatch.Dispatch((int)EventID.GameEventId.GetIconOrder);
                    //播放滚动声音
                    break;
                case RequestCmd.CmdCaiChiChange:
                    YxDebug.Log("彩池变化");
                    long caichi = response.GetLong("caichi");
                    EventDispatch.Dispatch((int)EventID.GameEventId.AlterPotGold, new EventData(caichi));
                    break;
                case RequestCmd.CmdGetMessageWhenWin:
                    YxDebug.Log("中彩通知");
                    var data = response.GetUtfString("data");
                    StartCoroutine(ShowNotice(data));
                    break;
            }
        }

        private IEnumerator ShowNotice(string data)
        {
            //97的data的形式 18-01-08,16:50:56,2000,235,游客_225891  之后有可能会换
            yield return new WaitForSeconds(5);
            var infos = data.Split(',');
            if (infos.Length < 5) yield break;
            var msg = string.Format("恭喜玩家:{0} 获得{1}分,并获得{2}彩金", infos[4], infos[2], infos[3]);
            var noticeData = new YxNoticeMessageData { Message = msg, ShowType = 1000 };
            YxNoticeMessage.ShowNoticeMsg(noticeData);
        }

        public override void GameStatus(int status, ISFSObject info)
        {

        }
        #endregion
        /// <summary>
        /// 当图片停止时调用
        /// </summary>
        protected virtual void OnStopBySelf()
        {
            //发送更新彩池
            EventDispatch.Dispatch((int)EventID.GameEventId.ChangeButtonIcon, new EventData(3));
            Facade.Instance<MusicManager>().Stop();
            SFSObject sfsObject = SFSObject.NewInstance();
            sfsObject.PutInt(RequestKey.KeyType, (int)RequestCmd.CmdCaiChiChange);
            App.GetRServer<LXGameServe>().SendGameRequest(sfsObject);
            EventDispatch.Dispatch((int)EventID.GameEventId.ShowRedLineIfWin);//显示红线
            EventDispatch.Dispatch((int)EventID.GameEventId.AlterRoundScore);//显示本局得分
            EventDispatch.Dispatch((int)EventID.GameEventId.PlayEffect);//播放特效
            SFSObject data = SFSObject.NewInstance();
            data.PutInt(RequestKey.KeyType, (int)RequestCmd.CmdGetMessageWhenWin);
            App.GetRServer<LXGameServe>().SendGameRequest(data);//更新彩池
            OverallData gameData = App.GetGameData<OverallData>();
            gameData.GetPlayer().Coin = gameData.UserGold;
            gameData.GetPlayer().UpdateView();
            var isWin = App.GetGameData<OverallData>().Response.IsWin;
            //App.GetGameData<OverallData>().SetPlayerData();
            if (IsTrusteeship)
            {
                if (isWin)
                    StartCoroutine(ReStartGameBySelf(5f));//中奖等4s
                else
                    StartCoroutine(ReStartGameBySelf(2f));//未中奖等2s
                EventDispatch.Dispatch((int)EventID.GameEventId.ChangeButtonIcon, new EventData(2));
            }
            if (isWin)
            {
                StartCoroutine(HideAllEffectAfter(5f)); //自动隐藏特效
            }
            else
            {
                StartCoroutine(HideAllEffectAfter(2f)); //自动隐藏特效
            }
            CanQuit = true;
        }

        protected virtual IEnumerator ReStartGameBySelf(float wait)
        {
            yield return new WaitForSeconds(wait);
            if (IsTrusteeship)
            {
                EventDispatch.Dispatch((int)EventID.GameEventId.OnStartClick);
                EventDispatch.Dispatch((int)EventID.GameEventId.ChangeButtonIcon, new EventData(3));
            }
        }

        protected virtual IEnumerator StartGameByTrust()
        {
            yield return new WaitForSeconds(0.5f);
            EventDispatch.Dispatch((int)EventID.GameEventId.OnStartClick);
            EventDispatch.Dispatch((int)EventID.GameEventId.ChangeButtonIcon, new EventData(3));
        }

        protected virtual IEnumerator HideAllEffectAfter(float wait)
        {
            yield return new WaitForSeconds(wait - 1.5f);
            if (!IsTrusteeship) EventDispatch.Dispatch((int)EventID.GameEventId.ChangeButtonIcon, new EventData(0));
            yield return new WaitForSeconds(wait);
            EventDispatch.Dispatch((int)EventID.GameEventId.HideAllEffect);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
            EventDispatch.Instance.OnDestroy();
        }
    }

    public enum RequestCmd
    {
        /// <summary>
        /// 彩池金额变化
        /// </summary>
        CmdCaiChiChange = 0x00,
        /// <summary>
        /// 开始游戏
        /// </summary>
        CmdStartGame = 0x01,
        /// <summary>
        /// 中彩通知
        /// </summary>
        CmdGetMessageWhenWin = 0x02,
    }
}