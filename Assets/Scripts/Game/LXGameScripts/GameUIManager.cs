using UnityEngine;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.LXGameScripts
{
    public class GameUiManager : MonoBehaviour
    {
        public BottomPourTable PourTable;
        public GameMoveManager GameMove;
        public GameSettingWindow Setting;

        protected virtual void Awake()
        {
            ResisteEvent();
        }

        protected virtual void ResisteEvent()
        {
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.AlterPotGold, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.AlterRoundScore, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.InitBottomPourData, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.OnStartClick, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.OnStopClick, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.StartRollJetton, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.GetIconOrder, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.HideAllEffect, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.ClearRoundScore, Resiste);
            EventDispatch.Instance.RegisteEvent((int)EventID.GameEventId.SettingBtnClick, Resiste);
        }

        protected virtual void Resiste(int id, EventData data)
        {
            EventID.GameEventId type = (EventID.GameEventId)id;
            switch (type)
            {
                case EventID.GameEventId.AlterPotGold:
                    PourTable.ChangeAllScore(data);
                    break;
                case EventID.GameEventId.AlterRoundScore:
                    PourTable.ChangeRoundScore();
                    break;
                case EventID.GameEventId.InitBottomPourData:
                    PourTable.InitData(data);
                    break;
                case EventID.GameEventId.OnStartClick:
                    OnStartClick();
                    break;
                case EventID.GameEventId.OnStopClick:
                    OnStopClick();
                    break;
                case EventID.GameEventId.StartRollJetton:
                    GameMove.IsStartRollJetton = true;
                    break;
                case EventID.GameEventId.GetIconOrder:
                    GameMove.GetJettonOrder();
                    break;
                case EventID.GameEventId.HideAllEffect:
                    HideAllEffect();
                    break;
                case EventID.GameEventId.ClearRoundScore:
                    PourTable.ClearRoundScore();
                    break;
                case EventID.GameEventId.SettingBtnClick:
                    Setting.Show();
                    break;
                default:
                    OtherResiste(id, data);
                    break;
            }
        }

        protected virtual void OtherResiste(int id, EventData data)
        {
            Debug.Log("没有找到key为:" + id + "的委托");
        }
        protected virtual void OnStartClick()
        {
            if (GameMove.IsStartRollJetton)
                return;
            EventDispatch.Dispatch((int)EventID.GameEventId.HideAllEffect);
            EventDispatch.Dispatch((int)EventID.GameEventId.ClearRoundScore);
            SFSObject data = SFSObject.NewInstance();
            data.PutInt(RequestKey.KeyType, (int)RequestCmd.CmdStartGame);
            data.PutInt("ante", PourTable.AnteTimes);
            App.GetRServer<LXGameServe>().SendGameRequest(data);
        }
        protected virtual void HideAllEffect()
        {
            
        }
        protected virtual void OnStopClick()
        {
            if (!GameMove.IsStartRollJetton) return;
            GameMove.StopRoll();
        }
    }
}