using System.Collections.Generic;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionChangeCard : AbsCommandAction
    {
        protected int mHuanType;// 换牌类型 0顺时针 1逆时针 2对家
        protected int[] mChangeCards;// 扣下去的牌数组
        protected int[] mNewCards;// 换过来的牌数组  

        protected void SetData(ISFSObject data)
        {
            mHuanType = data.GetInt("huanType");
            mNewCards = data.GetIntArray("cards");
            mChangeCards = data.GetIntArray("changecards");
        }

        /// <summary>
        /// 换张选牌开始
        /// </summary>
        public void ChangeCardsStartAction(ISFSObject data)
        {
            //关闭开启托管权限
            GameCenter.Shortcuts.SwitchCombination.Close((int)GameSwitchType.PowerAiAgency);          
            //显示UI
            GameCenter.EventHandle.Dispatch((int)EventKeys.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                CtrlState = true,
                StateFlagType = (int)PlayerStateFlagType.SelectCard
            });
            //切换手牌状态为换牌
            Game.MahjongGroups.PlayerHand.SetHandCardState(HandcardStateTyps.ExchangeCards, 3);
        }

        /// <summary>
        /// 换张选牌结束
        /// </summary>
        public void ChangeCardsEndAction(ISFSObject data)
        {
            SetData(data);
            GameCenter.GameLogic.SetDelayTime(3.5f);
            GameCenter.EventHandle.Dispatch((int)EventKeys.SetPlayerFlagState, new PlayerStateFlagArgs()
            {
                CtrlState = false
            });
            GameCenter.EventHandle.Dispatch((int)EventKeys.ChangeCardTip, new HuanAndDqArgs()
            {
                HuanType = mHuanType
            });
            //停止定时任务
            Game.TableManager.StopTimer();
            //移除旧手牌数据
            for (int i = 0; i < mChangeCards.Length; i++)
            {
                DataCenter.OneselfData.HardCards.Remove(mChangeCards[i]);
            }
            //添加新手牌数据
            for (int i = 0; i < mNewCards.Length; i++)
            {
                DataCenter.OneselfData.HardCards.Add(mNewCards[i]);
            }
            //所有玩家牌扣到桌子上
            var mahHand = Game.MahjongGroups.PlayerHand;
            var xzMahHand = mahHand.GetComponent<XzmjMahjongPlayerHand>();
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                if (i == 0)
                {
                    //超时扣牌
                    if (Game.MahjongGroups.SwitchGorup[0].MahjongList.Count == 0)
                    {
                        xzMahHand.SwitchCardsTimeOut(mChangeCards);
                        Game.MahjongGroups.SwitchGorup[0].AddMahToSwitch(mChangeCards);
                    }
                }
                else
                {
                    //其他家扣牌
                    var array = new int[mChangeCards.Length];
                    var mahList = Game.MahjongGroups.MahjongHandWall[i].MahjongList;
                    for (int j = 0; j < array.Length; j++)
                    {
                        var item = mahList[j];
                        item.gameObject.SetActive(false);
                        array[j] = item.Value;
                    }
                    Game.MahjongGroups.SwitchGorup[i].AddMahToSwitch(array);
                }
            }
            //3人血战，换牌动画设置
            if (DataCenter.MaxPlayerCount == 3)
            {
                var switchGorup = Game.MahjongGroups.SwitchGorup;
                //将麻将牌放到组中             
                for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
                {
                    for (int j = 0; j < switchGorup[i].MahjongCnt; j++)
                    {
                        if (mHuanType == 0)
                        {
                            if (i < 2) switchGorup[i].MahjongList[j].transform.SetParent(switchGorup.Group1);
                            else switchGorup[i].MahjongList[j].transform.SetParent(switchGorup.Group2);
                        }
                        else
                        {
                            if (i == 1) switchGorup[i].MahjongList[j].transform.SetParent(switchGorup.Group2);
                            else switchGorup[i].MahjongList[j].transform.SetParent(switchGorup.Group1);
                        }
                    }
                }
            }
            Game.MahjongGroups.SwitchGorup.StartRotation(mHuanType);
            ContinueTaskManager.NewTask().AppendFuncTask(HuanCardTask).Start();
            //手牌记录
            DataCenter.Players.AddRecordMahjongs();
        }

        protected IEnumerator<float> HuanCardTask()
        {
            yield return 1.5f;
            Game.MahjongGroups.SwitchGorup.OnResetSwitchNodes();
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                Game.MahjongGroups.SwitchGorup[i].OnReset();
                var hand = Game.MahjongGroups.MahjongHandWall[i];               
                if (i == 0)
                {
                    // 如果换张时重连，判断当前手牌是是否多牌
                    var flag = (hand.MahjongList.Count + mNewCards.Length) <= hand.RowCnt;
                    if (flag)
                    {
                        for (int j = 0; j < mNewCards.Length; j++)
                        {
                            hand.GetInMahjong(mNewCards[j]);
                        }
                    }                   
                }
                else
                {
                    var mahList = Game.MahjongGroups.MahjongHandWall[i].MahjongList;
                    for (int j = 0; j < mahList.Count; j++)
                    {
                        mahList[j].gameObject.SetActive(true);
                    }
                }
            }
            //开启托管权限
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
            Game.MahjongGroups.MahjongHandWall[0].SortHandMahjong();
            Game.MahjongGroups.PlayerHand.SetHandCardState(HandcardStateTyps.Normal);
            GameCenter.Controller.ForbbidToken = DataCenter.ConfigData.HasDingQue;
        }
    }
}
