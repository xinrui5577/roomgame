using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ActionBuzhang : AbsCommandAction
    {
        /// <summary>
        /// 游戏开始时补张缓存
        /// </summary>
        protected Queue<BuZhangData> mBuZhangQueue = new Queue<BuZhangData>();
        /// <summary>
        /// 游戏开始时补张缓存
        /// </summary>
        protected Queue<int> mGetCardBuzhangQueue = new Queue<int>();
        /// <summary>
        /// 游戏发牌时补张
        /// </summary>
        protected ContinueTaskContainer mBuzhangTask;
        /// <summary>
        /// 补张的座位号
        /// </summary>
        protected int mBuZhangChair;

        public Queue<int> BuzhangQueue
        {
            get { return mGetCardBuzhangQueue; }
        }

        public int BuZhangChair
        {
            get { return mBuZhangChair; }
        }

        public void BuZhangAction(ISFSObject data)
        {
            var buData = new BuZhangData()
            {
                chair = MahjongUtility.GetChair(data),
                Cards = data.GetIntArray(RequestKey.KeyCards),
                BuZhangCards = data.GetIntArray("buZhangCard"),
            };
            mBuZhangQueue.Enqueue(buData);
        }

        public void BuZhangFinishAction(ISFSObject data)
        {           
            ContinueTaskManager.NewTask().AppendFuncTask(BuZhangAnimation).Start();
            var delayTimer = DataCenter.Config.TimeBuzhangAniDelay * mBuZhangQueue.Count;
            GameCenter.GameLogic.SetDelayTime(delayTimer);
        }

        public void GetInBuZhangAction(ISFSObject data)
        {
            mBuZhangChair = MahjongUtility.GetChair(data);
            mGetCardBuzhangQueue.Enqueue(data.GetInt(RequestKey.KeyOpCard));
            GameCenter.Shortcuts.SwitchCombination.Open((int)GameSwitchType.HasBuzhang);
        }

        public IEnumerator<float> BuZhangAnimation()
        {
            BuZhangData buData;
            while (mBuZhangQueue.Count > 0)
            {
                buData = mBuZhangQueue.Dequeue();
                //移除手牌中的
                Game.MahjongGroups.MahjongHandWall[buData.chair].RemoveMahjong(buData.BuZhangCards);
                //添加到胡牌中
                Game.MahjongGroups.MahjongOther[buData.chair].GetInMahjong(buData.BuZhangCards);
                yield return DataCenter.Config.TimeBuzhangAniDelay;
                //在墙中移除
                DataCenter.LeaveMahjongCnt -= buData.BuZhangCards.Length;
                Game.MahjongGroups.PopMahFromCurrWall(buData.BuZhangCards.Length);
                //添加到手牌中
                Game.MahjongGroups.MahjongHandWall[buData.chair].GetInMahjongWithRoat(buData.Cards);
                MahjongUtility.PlayPlayerSound(buData.chair, "buhua");
                //补张的牌 添加到手牌中
                if (buData.chair == 0)
                {
                    //移除花牌
                    DataCenter.Players.RemoveHandCardData(0, buData.BuZhangCards);
                    //添加新牌           
                    DataCenter.Players.AddHandCardData(0, buData.Cards);
                    //重新展示听提示
                    var tingList = DataCenter.Players[0].TingList;
                    GameCenter.Shortcuts.MahjongQuery.ShowQueryTip(tingList);
                }
                yield return DataCenter.Config.TimeBuzhangAniDelay;
            }
        }

        public override void OnReset()
        {
            mBuZhangQueue.Clear();
            mGetCardBuzhangQueue.Clear();
        }
    }

    public struct BuZhangData
    {
        public int chair;
        public int[] Cards;
        public int[] BuZhangCards;
    }
}