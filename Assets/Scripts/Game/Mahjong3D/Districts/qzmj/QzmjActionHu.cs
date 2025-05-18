using Sfs2X.Entities.Data;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class QzmjActionHu : ActionHu
    {
        private QzmjOperateType mHuType;

        protected override void ParseData(ISFSObject data)
        {
            base.ParseData(data);
            if (data.ContainsKey("youjinhu"))
            {
                mHuType = (QzmjOperateType)data.GetInt("youjinhu");
            }
            else if (data.ContainsKey("sanjinhu"))
            {
                mHuType = QzmjOperateType.sanjindao;
            }
            else
            {
                mHuType = QzmjOperateType.No;
            }
        }

        protected override IEnumerator<float> ZimoTask()
        {
            if (GameUtils.CheckStopTask())
            {
                yield return ContinueTaskAgent.Shutdown;
            }
            yield return Config.TimeHuAniInterval;
            var huCard = mArgs.HuCard;
            var huChair = MahjongUtility.GetChair(mArgs.HuSeats[0]);
            string huType = "";
            if (DataCenter.Config.PlaySpecialHuSound)
            {
                huType = IsSpecialHu(mArgs.Result[mArgs.HuSeats[0]].CType);
            }
            //设置胡牌，分张除外
            if (!DataCenter.Game.FenzhangFlag)
            {
                SetHuCard(huChair, huCard);
            }

            // 音效 特效显示   
            if (mHuType == QzmjOperateType.No)
            {
                MahjongUtility.PlayOperateSound(huChair, huType);
                GameCenter.Hud.UIPanelController.PlayPlayerUIEffect(huChair, PoolObjectType.zimo);
            }
            else
            {               
                var effectType = PoolObjectType.zimo;
                switch (mHuType)
                {
                    case QzmjOperateType.youjin:
                        effectType = PoolObjectType.youjin;
                        break;
                    case QzmjOperateType.shuangyou:
                        effectType = PoolObjectType.shuangyou;
                        break;
                    case QzmjOperateType.sanyou:
                        effectType = PoolObjectType.sanyou;
                        break;
                    case QzmjOperateType.sanjindao:
                        effectType = PoolObjectType.sanjindao;
                        break;
                }
                MahjongUtility.PlayOperateSound(huChair, mHuType.ToString());
                GameCenter.Hud.UIPanelController.PlayPlayerUIEffect(huChair, effectType);
            }
        }
    }
}
