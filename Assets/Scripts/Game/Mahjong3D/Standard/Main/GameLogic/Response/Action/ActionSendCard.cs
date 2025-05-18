using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionSendCard : AbsCommandAction
    {
        protected ContinueTaskContainer mSendTask;
        protected float mSingleMotionTime = 0.3f;
        protected float mSingleWaitTime = 0.2f;
        protected float mAniDelayTime = 3.8f;
        protected int mFanpai;

        protected bool SkipSend()
        {
            return Game.MahjongGroups.PlayerHand.MahjongList.Count > 0;
        }

        public virtual void SendCardAction(ISFSObject data)
        {
            //重连数据在发牌数据之前时，会造成手牌多牌bug
            if (SkipSend()) return;

            SetData(data);
            StartSendCard();
            float delayTime = Config.MahStartAnimation ? mAniDelayTime : 1;
            GameCenter.GameLogic.SetDelayTime(delayTime);
            //手牌记录
            DataCenter.Players.AddRecordMahjongs();
        }

        protected void SetData(ISFSObject data)
        {
            mFanpai = data.TryGetInt(AnalysisKeys.CardFan);
            DataCenter.Game.FanCard = mFanpai;
            DataCenter.Game.LaiziCard = data.TryGetInt(AnalysisKeys.CardLaizi);
            DataCenter.CurrOpSeat = data.TryGetInt(RequestKey.KeySeat);           
            DataCenter.Players[0].HardCards.AddRange(data.GetIntArray(RequestKey.KeyCards));
        }

        /// <summary>
        /// 开始发牌
        /// </summary>
        protected void StartSendCard()
        {
            if (mSendTask == null)
            {
                mSendTask = ContinueTaskManager.NewTask()
                .AppendTaskFromFunc(Config.MahStartAnimation, (b) =>
                {
                    if (b) { return TableAnimation; }
                    return null;
                })
                .AppendTaskFromFunc(Config.MahStartAnimation, (b) =>
                {
                    return b ? (Func<IEnumerator<float>>)SendCardAnimation : SendCardNoAnimation;
                })
                .SetTaskExecuteType(TaskExecuteType.AsynLine)
                .SetContainerExeType(ContainerExeType.Single)
                .CallBack(TidyMahjongCards);
            }
            mSendTask.Start();
            //重置娱乐房切换房间标志位
            GameCenter.Instance.YuLeBoutState = false;
        }

        /// <summary>
        /// 模拟麻将机动画
        /// </summary>
        /// <returns></returns>
        protected IEnumerator<float> TableAnimation()
        {
            var group = Game.MahjongGroups;
            var table = Game.TableManager.GetParts<MahjongTable>(TablePartsType.Table);
            // 麻将机板下移动画
            table.TableDownAnimation(mSingleMotionTime);
            // 麻将滑动显示动画
            yield return mSingleMotionTime + mSingleWaitTime;
            for (int i = 0; i < group.MahjongWall.Length; i++)
            {
                group.MahjongWall[i].WallSideswayTweener(mSingleMotionTime);
            }
            yield return mSingleMotionTime + mSingleWaitTime;
            // 麻将机板上升 
            table.TableUpAnimation(mSingleMotionTime);
            // 麻将上升动画
            for (int i = 0; i < group.MahjongWall.Length; i++)
            {
                group.MahjongWall[i].WallMoveUpTweener(mSingleMotionTime);
            }
            yield return mSingleMotionTime + mSingleWaitTime;
        }

        /// <summary>
        /// 不需要发牌动画时 直接设置手牌
        /// </summary>
        /// <returns></returns>
        protected IEnumerator<float> SendCardNoAnimation()
        {
            yield return 0;
            var sendCardCount = DataCenter.Config.HandCardCount * DataCenter.MaxPlayerCount;
            Game.MahjongGroups.PopMahFromCurrWall(sendCardCount);
            DataCenter.LeaveMahjongCnt -= sendCardCount;
            MahjongUtility.PlayEnvironmentSound("fapai_one");
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                if (DataCenter.Players[i].HardCards.Count == 0)
                {
                    Game.MahjongGroups.MahjongHandWall[i].GetInMahjong(new int[Config.HandCardCount]);
                }
                else
                {
                    Game.MahjongGroups.MahjongHandWall[i].GetInMahjong(DataCenter.Players[i].HardCards);
                }
            }
        }

        protected virtual IEnumerator<float> SendCardAnimation()
        {
            //发牌动画时重连游戏，会造成手牌多牌bug
            if (SkipSend()) yield return ContinueTaskAgent.Shutdown;

            var getInCnt = 4;
            var meOffset = 0;
            var startChair = DataCenter.CurrOpChair;
            var sendCardCount = DataCenter.Config.HandCardCount * DataCenter.MaxPlayerCount;
            MahjongUtility.PlayEnvironmentSound("fapai");
            DataCenter.LeaveMahjongCnt -= sendCardCount;
            GameCenter.Scene.MahjongGroups.PlayerToken = false;

            while (sendCardCount > 0)
            {
                var hand = Game.MahjongGroups.MahjongHandWall[startChair];
                int[] card;
                if (startChair == 0)
                {
                    //根据上一局发牌数，来确定这一轮发牌数
                    var rate = sendCardCount - DataCenter.MaxPlayerCount * getInCnt;
                    if (rate < 1)
                    {
                        //不足时，重新确定发牌数
                        getInCnt = sendCardCount / DataCenter.MaxPlayerCount;
                    }
                    card = new int[getInCnt];
                    for (int i = 0; i < card.Length; i++)
                    {
                        card[i] = DataCenter.OneselfData.HardCards[meOffset + i];
                    }
                    meOffset += card.Length;
                }
                else
                {
                    card = new int[getInCnt];
                }     

                var handWall = Game.MahjongGroups.MahjongHandWall[startChair];
                // 如果换张时重连，判断当前手牌是是否多牌
                var flag = (handWall.MahjongList.Count + card.Length) <= DataCenter.Config.HandCardCount;
                if (flag)
                {
                    Game.MahjongGroups.PopMahFromCurrWall(getInCnt);
                    handWall.OnSendMahjong(card, Config.TimeSendCardUp, Config.TimeSendCardUpWait);
                    yield return Config.TimeSendCardInterval;
                }
                startChair = (startChair + 1) % DataCenter.MaxPlayerCount;
                sendCardCount -= getInCnt;
            }
            //如果手牌多，请求重连
            var handFlag = Game.MahjongGroups.MahjongHandWall[0].MahjongList.Count > DataCenter.Config.HandCardCount + 1;
            if (handFlag)
            {
                com.yxixia.utile.YxDebug.YxDebug.LogError("手牌多了，请求重连！");                
                GameCenter.Network.SendReJoinGame();
            }
        }

        protected void TidyMahjongCards()
        {
            if (mFanpai > 0)
            {
                Game.TableManager.SetShowMahjong(mFanpai);
            }
            //扣下牌-排序-设置赖子-抬起
            for (int i = 0; i < DataCenter.MaxPlayerCount; i++)
            {
                Game.MahjongGroups.MahjongHandWall[i].OnSendOverSortMahjong(Config.TimeSendCardUp, Config.TimeSendCardUp);
            }
            //允许开启托管
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.PowerAiAgency);
        }
    }
}