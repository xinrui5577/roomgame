using UnityEngine;
namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class LyzzwmMahjongPlayerHand : MahjongPlayerHand
    { 
        protected override void ThrowCardClickEvent(Transform mahjong)
        {
            //如果允许当前用户出牌
            if (HasToken)
            {
                var dataCenter = GameCenter.DataCenter;
                var temp = mahjong.GetComponent<MahjongContainer>();
                ChooseMj = temp;

                if (temp.Lock)
                {
                    return;
                }
                //花牌不允许打出
                if (temp.MahjongCard.Value >= (int)MahjongValue.ChunF)
                {
                    return;
                }
                if (null != mPutOutFunc && mPutOutFunc(temp))
                {
                    return;
                }
                bool flag = dataCenter.IsLaizi(temp.Value);
                //赖子牌是否允许打出
                if (flag && !dataCenter.Config.AllowLaiziPut)
                {
                    //如果手牌中全部都是赖子可以打出
                    foreach (MahjongContainer mj in mMahjongList)
                    {
                        if (!dataCenter.IsLaizi(mj.Value))
                        {
                            return;
                        }
                    }
                }
                //出牌动画               
                dataCenter.ThrowoutCard = temp.Value;
                dataCenter.OwnerThrowoutCardFlag = true;
                GameCenter.Scene.MahjongGroups.MahjongHandWall[0].ThrowOut(temp);

                //发送请求
                HasToken = false;
                //通知网络 发送出牌消息        
                GameCenter.EventHandle.Dispatch<C2SThrowoutCardArgs>(
                    (int)EventKeys.C2SThrowoutCard,
                    (param) => { param.Card = temp.Value; });

                GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(null);
                GameCenter.Hud.GetPanel<PanelQueryHuCard>().Close();
            }
        }
    }
}
