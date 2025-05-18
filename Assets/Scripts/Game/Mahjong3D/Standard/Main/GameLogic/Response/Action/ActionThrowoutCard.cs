using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionThrowoutCard : AbsCommandAction
    {
        protected int mPlayerSeat;
        protected int mThrowoutCard;

        public virtual void ThrowoutCardAction(ISFSObject data)
        {
            SetData(data);
            LogicAction();
        }

        protected void SetData(ISFSObject data)
        {
            mPlayerSeat = data.GetInt(RequestKey.KeySeat);
            mThrowoutCard = data.GetInt(RequestKey.KeyOpCard);
            //DataCenter.CurrOpSeat = mPlayerSeat;

            if (DataCenter.CurrOpChair == 0)
            {
                GameCenter.EventHandle.Dispatch((int)EventKeys.QueryHuCard, new QueryHuArgs() { PanelState = false });
            }
            else
            {
                DataCenter.ThrowoutCard = mThrowoutCard;
            }
            GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(null);
        }

        protected void LogicAction()
        {
            var currChair = DataCenter.CurrOpChair;
            var groups = Game.MahjongGroups;
            groups.OnClearFlagMahjong();

            if (DataCenter.CurrOpChair == 0)
            {
                MahjongContorl.ClearSelectCard();
                if (!DataCenter.OwnerThrowoutCardFlag)
                {
                    groups.MahjongHandWall[currChair].ThrowOut(mThrowoutCard);
                }
            }
            else
            {
                groups.MahjongHandWall[currChair].ThrowOut(mThrowoutCard);
            }

            DataCenter.OwnerThrowoutCardFlag = false;
            var item = groups.MahjongThrow[currChair].GetInMahjong(mThrowoutCard);
            Game.TableManager.ShowOutcardFlag(item);
            MahjongUtility.PlayMahjongSound(currChair, mThrowoutCard);

            //金币房时 玩家选择听牌时，如果时间到了系统自动出牌， 恢复为正常出牌状态
            if (DataCenter.Room.RoomType == MahRoomType.YuLe)
            {
                var playerHand = Game.MahjongGroups.PlayerHand;
                if (playerHand.CurrState == HandcardStateTyps.ChooseNiuTing || playerHand.CurrState == HandcardStateTyps.ChooseTingCard)
                {
                    playerHand.SetHandCardState(HandcardStateTyps.Normal);
                }
            }
        }

        /// <summary>
        /// 跟庄
        /// </summary>
        /// <param name="data"></param>
        protected void OnGenzhuang(ISFSObject data)
        {
            if (data.ContainsKey("genZhuang"))
            {
                var gold = data.GetIntArray("genZhuang");
                var chairGold = new int[gold.Length];
                for (int i = 0; i < gold.Length; i++)
                {
                    var chair = MahjongUtility.GetChair(i);
                    chairGold[chair] = gold[i];
                }
            }
        }
    }
}
