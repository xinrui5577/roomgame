using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class XzmjActionSendCard : ActionSendCard
    {
        public override void SendCardAction(ISFSObject data)
        {         
            //重连数据在发牌数据之前时，会造成手牌多牌bug
            if (SkipSend()) return;

            SetData(data);
            StartSendCard();
            float delayTime = Config.MahStartAnimation ? mAniDelayTime : 1;
            GameCenter.GameLogic.SetDelayTime(delayTime);
        }
    }
}
