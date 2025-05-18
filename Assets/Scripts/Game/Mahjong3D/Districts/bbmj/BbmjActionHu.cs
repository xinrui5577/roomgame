using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class BbmjActionHu : ActionHu
    {
        public override void ZimoAction(ISFSObject data)
        {
            OnHu(data);
            if (null == mZimoTask)
            {
                mZimoTask = ContinueTaskManager.NewTask()
                .AppendFuncTask(() => BbmjZimoTask())
                .AppendFuncTask(() => HandcardCtrlTask())
                .AppendFuncTask(() => ZhaNiaoAnimation())
                .AppendActionTask(ActionCallback, Config.TimeHuAniInterval);
            }
            mZimoTask.Start();
        }

        protected IEnumerator<float> BbmjZimoTask()
        {
            if (GameUtils.CheckStopTask())
            {
                yield return ContinueTaskAgent.Shutdown;
            }
            yield return Config.TimeHuAniInterval;
            var huCard = mArgs.HuCard;
            var huChair = MahjongUtility.GetChair(mArgs.HuSeats[0]);
            //如果本家自摸，移除本家自摸的牌
            if (huChair == 0)
            {
                Game.MahjongGroups.PlayerHand.PopMahjong();
            }
            //游金了之后，抓到白板胡，叫白燕白
            if (huCard == 87 && DataCenter.Players[huChair].IsAuto)
            {
                MahjongUtility.PlayPlayerSound(huChair, "baiyanbai");
                GameCenter.Scene.PlayPlayerEffect(huChair, PoolObjectType.hu);
            }
            else
            {
                MahjongUtility.PlayOperateEffect(huChair, PoolObjectType.hu);
            }
            SetHuCard(huChair, huCard).Laizi = GameCenter.DataCenter.IsLaizi(huCard);
        }
    }
}
