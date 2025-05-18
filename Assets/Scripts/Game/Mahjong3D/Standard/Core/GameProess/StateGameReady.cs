using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class StateGameReady : FsmState
    {
        public override void OnEnter(FsmStateArgs args)
        {
            GameCenter.Instance.SetIgonreReconnectState(false);
            //执行继承IGameEndCycle接口脚本
            GameCenter.Lifecycle.GameEndCycle();
            //执行继承IGameReadyCycle接口脚本
            GameCenter.Lifecycle.GameReadyCycle();

            var rserver = App.RServer;
            var db = GameCenter.DataCenter;            
            if (!rserver.IsInGame && db.Room.RoomType == MahRoomType.YuLe)
            {
                rserver.ChangeRoom();
            }
            else
            {                
                //当前用户没有准被 发送准备消息
                if (db.OneselfData != null && !db.OneselfData.IsReady)
                {
                    if (db.ConfigData.AutoReady)
                    {
                        GameCenter.EventHandle.Dispatch((int)EventKeys.C2SPlayerReady);
                    }
                    else
                    {
                        GameCenter.EventHandle.Dispatch((int)EventKeys.ReadyBtnCtrl, new PanelTriggerArgs() { ReadyState = true });
                    }
                }
            }
            var handler = GameCenter.GameLogic.DispatchResponseHandlers(CustomProl.ReadyLogic);
            if (null != handler)
            {
                handler(null);
            }
        }

        public override void OnLeave(bool isShutdown) { }
    }
}