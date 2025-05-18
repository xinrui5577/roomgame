using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionReconnect : AbsCommandAction
    {
        public virtual void ReconnectAction(ISFSObject data)
        {
            //更新牌
            UpdateMahjongGroup();
            //设置手牌状态
            SetReconectCardState();
            //更新牌桌
            UpdateMahjongTable();
            MahjongQueryHuTip();
            //开启允许托管权限
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
        }

        protected virtual void UpdateMahjongGroup()
        {
            var currChair = DataCenter.CurrOpChair;
            //设置发牌点
            Game.MahjongGroups.SetSendCardSPos(DataCenter.BankerSeat, DataCenter.OneselfData.Seat);
            //设置抓牌点
            Game.MahjongGroups.SetCatchCardStartPos(DataCenter.SaiziPoint);
            var removeCardNum = DataCenter.Room.MahjongCount - DataCenter.LeaveMahjongCnt;
            //宝牌：再牌墙中随机位置移除牌
            var baoList = DataCenter.Game.BaoIndexList;
            if (baoList != null)
            {
                var num = baoList.Length;
                removeCardNum -= num;
                DataCenter.LeaveMahjongCnt -= num;
            }
            //移除牌墙中的牌              
            Game.MahjongGroups.PopMahFromCurrWall(removeCardNum);
            //如果有翻牌，显示翻牌            
            if (DataCenter.Game.FanCard > 0)
            {
                DataCenter.LeaveMahjongCnt -= 1;
                Game.MahjongGroups.PopMahFromCurrWall(1);
                Game.TableManager.SetShowMahjong(DataCenter.Game.FanCard);
            }
            //如果有翻宝，显示翻牌
            if (baoList != null && baoList.Length > 0)
            {
                var tempBao = DataCenter.Game.BaoCard;
                if (tempBao == 0)
                {
                    tempBao = 17;
                }
                //宝牌是否显示
                var showBao = !base.DataCenter.ConfigData.AnBao && base.DataCenter.OneselfData.IsAuto;
                Game.TableManager.SetShowBao(tempBao, showBao);
                //移除翻宝扣掉的牌
                for (int i = 0; i < baoList.Length; i++)
                {
                    Game.MahjongGroups.OnFanbaoRmoveMahjong(baoList[i]);
                }
            }
            //设置麻将牌
            MahjongUserInfo player;
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                player = DataCenter.Players[i];
                //设置出牌
                Game.MahjongGroups.MahjongThrow[i].GetInMahjong(player.OutCards);
                //设置吃碰杠牌
                Game.MahjongGroups.MahjongCpgs[i].SetCpgArray(player.CpgDatas.ToArray());
                //设置手牌 
                Game.MahjongGroups.MahjongHandWall[i].GetInMahjong(player.HardCards);
                //听牌
                if (DataCenter.Players[i].IsAuto)
                {
                    Game.MahjongGroups.MahjongHandWall[i].SetHandCardState(HandcardStateTyps.Ting);
                }
                //设置补张
                if (!player.BuzhangCards.ExIsNullOjbect() && player.BuzhangCards.Length > 0)
                {
                    var num = player.BuzhangCards.Length;
                    DataCenter.LeaveMahjongCnt -= num;
                    Game.MahjongGroups.PopMahFromCurrWall(num);
                    Game.MahjongGroups.MahjongOther[i].GetInMahjong(player.BuzhangCards);
                }
            }
            //设置自己手中的赖子牌
            Game.MahjongGroups.PlayerHand.SetLaizi(DataCenter.Game.LaiziCard);
        }

        protected virtual void SetReconectCardState()
        {
            //设置打牌状态
            if (DataCenter.ReconectCardState == 0)
            {
                var currChair = DataCenter.CurrOpChair;
                if (DataCenter.SelfCurrOp)
                {
                    Game.MahjongGroups.PlayerToken = true;
                }
                //设置最后麻将的位子
                Game.MahjongGroups.MahjongHandWall[currChair].SetLastCardPos(DataCenter.GetInMahjong);
                //如果有听并且是自己出牌，自动打牌
                if (DataCenter.CurrOpChair == 0 && DataCenter.Players[0].IsAuto)
                {
                    var lastCard = Game.MahjongGroups.MahjongHandWall[currChair].SetTingPaiNeedOutCard();
                    if (DataCenter.OperateMenu == 0)
                    {
                        GameCenter.EventHandle.Dispatch<C2SThrowoutCardArgs>((int)EventKeys.C2SThrowoutCard, (param) =>
                        {
                            param.Card = lastCard.Value;
                        });
                    }
                    else
                    {
                        Game.MahjongGroups.PlayerHand.SetMahjongNormalState(lastCard);
                    }
                }
            }
            else if (DataCenter.ReconectCardState == 1)//牌已出 等待其他玩家响应
            {
                Game.MahjongGroups.PlayerToken = false;
            }
        }

        /// <summary>
        /// 显示麻将查询提示
        /// </summary>
        protected virtual void MahjongQueryHuTip()
        {
            if (!DataCenter.ConfigData.MahjongQuery && MahjongUtility.TingTipCtrl != 0) return;
            GameCenter.Shortcuts.MahjongQuery.OnReconnectRecordMahjong();
            var list = DataCenter.OneselfData.TingList;
            if (!DataCenter.OneselfData.IsAuto && null != list)
            {
                GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(list);
            }
        }

        protected virtual void UpdateMahjongTable()
        {
            Game.TableManager.HideSaizi();
            //设置Dnxb
            Game.TableManager.SwitchDirection(DataCenter.CurrOpSeat);
            //设置倒计时      
            Game.TableManager.StartTimer(Config.TimeOutcard);
            //设置上次出牌用户的箭头标记
            if (DataCenter.OldOpChair != DefaultUtils.DefValue)
            {
                var outPutMj = Game.MahjongGroups.MahjongThrow[DataCenter.OldOpChair].GetLastMahjong();
                if (outPutMj != null)
                {
                    Game.TableManager.ShowOutcardFlag(outPutMj);
                }
            }
        }
    }
}
