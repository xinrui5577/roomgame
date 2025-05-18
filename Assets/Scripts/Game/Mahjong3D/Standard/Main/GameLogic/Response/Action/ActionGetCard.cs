using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionGetCard : AbsCommandAction
    {
        public void GetCardAction(ISFSObject data)
        {
            SetData(data);
            if (DataCenter.Config.Buzhang)
            {
                //补张
                var command = GameCenter.GameLogic.GetGameResponseLogic<CommandBuzhang>();
                var buzhangAction = command.LogicAction;
                var count = buzhangAction.BuzhangQueue.Count;
                if (count > 0)
                {
                    var delayTimer = DataCenter.Config.TimeBuzhangMessageDelay;
                    GameCenter.GameLogic.SetDelayTime(count * delayTimer);
                    ContinueTaskManager.NewTask().AppendFuncTask(GetCardBuZhangTask).Start();
                }
                else OnGetCard();
            }
            else
            {
                OnGetCard();
            }
            //CheckHandCards();
        }

        public void SetData(ISFSObject data)
        {
            DataCenter.LeaveMahjongCnt--;
            DataCenter.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);
            DataCenter.GetInMahjong = data.TryGetInt(RequestKey.KeyOpCard);
            DataCenter.Game.HuangZhuang = data.TryGetBool(AnalysisKeys.KeyHuangZhuang);
            DataCenter.Players[DataCenter.CurrOpChair].IsTuiDan = data.ContainsKey("tuidan");

            if (data.TryGetBool(AnalysisKeys.KeyHuangZhuang))
            {
                GameCenter.EventHandle.Dispatch((int)EventKeys.HuangzhuangTip);
            }
            if (DataCenter.CurrOpChair == 0)
            {
                var opMenu = data.TryGetInt(AnalysisKeys.KeyOp);
                if (GameUtils.BinaryCheck(OperateKey.OpreateGang, opMenu))
                {
                    //根据server数据 指定杠牌
                    var array = data.TryGetIntArray("gangcard");
                    if (array != null)
                    {
                        DataCenter.GangCard.AddRange(array);
                    }
                }
            }
        }

        public void CheckHandCards()
        {
            if (DataCenter.CurrOpChair != 0) return;
            var group = Game.MahjongGroups;
            var cardCount = DataCenter.Config.HandCardCount;
            var handCards = group.PlayerHand.MahjongList.Count;
            var list = group.MahjongCpgs[0].CpgList;
            for (int i = 0; i < list.Count; i++)
            {
                int num = list[i].Data.GetAllCardDatas.Count;
                if (num == 4)
                {
                    cardCount++;
                }
                handCards += num;
            }
            if (handCards > cardCount)
            {
                //重连请求
                GameCenter.Network.SendReJoinGame();
            }
        }

        public IEnumerator<float> GetCardBuZhangTask()
        {
            var command = GameCenter.GameLogic.GetGameResponseLogic<CommandBuzhang>();
            var buzhangAction = command.LogicAction;

            while (buzhangAction.BuzhangQueue.Count > 0)
            {
                var card = buzhangAction.BuzhangQueue.Dequeue();
                //在墙中移除
                DataCenter.LeaveMahjongCnt--;
                Game.MahjongGroups.PopMahFromCurrWall(1);
                //添加到手牌中
                var chair = buzhangAction.BuZhangChair;
                Game.MahjongGroups.MahjongHandWall[chair].GetInMahjong(chair == 0 ? card : 0);
                yield return DataCenter.Config.TimeBuzhangAniDelay;
                //移除手牌中的
                Game.MahjongGroups.MahjongHandWall[chair].RemoveMahjong(card);
                //添加到胡牌中
                Game.MahjongGroups.MahjongOther[chair].GetInMahjong(card);
                MahjongUtility.PlayPlayerSound(DataCenter.CurrOpChair, "buhua");
                yield return DataCenter.Config.TimeBuzhangAniDelay;
            }
            OnGetCard();
            GameCenter.Shortcuts.SwitchCombination.Close((int)GameSwitchType.HasBuzhang);
            if (GameCenter.Shortcuts.SwitchCombination.IsOpen((int)GameSwitchType.AiAgency))
            {
                yield return Config.TimeTingPutCardWait;
                GameCenter.EventHandle.Dispatch<C2SThrowoutCardArgs>((int)EventKeys.C2SThrowoutCard, (param) =>
                {
                    param.Card = DataCenter.GetInMahjong;
                });
            }
            if (DataCenter.CurrOpChair == 0)
            {
                //重新展示听提示
                var tingList = DataCenter.Players[0].TingList;
                GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(tingList);
            }
        }

        private void OnGetCard()
        {
            //第一次抓牌 隐藏塞子
            if (DataCenter.Game.FirstGetCard)
            {
                DataCenter.Game.FirstGetCard = false;
                Game.TableManager.StartTimer(Config.TimeOutcard);
                Game.TableManager.SwitchDirection(DataCenter.CurrOpSeat);
            }
            Game.MahjongGroups.PopMahFromCurrWall();
            Game.MahjongGroups.MahjongHandWall[DataCenter.CurrOpChair].GetInMahjong(DataCenter.GetInMahjong);
            MahjongUtility.PlayEnvironmentSound("zhuapai");
        }
    }
}