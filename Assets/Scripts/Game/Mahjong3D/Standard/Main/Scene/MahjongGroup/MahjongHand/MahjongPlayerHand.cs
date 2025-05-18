using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongPlayerHand : MahjongHand
    {
        public static int PlayerHardLayer = -1;
        public MahjongContainer ChooseMj { get; set; }

        protected bool mHasToken;

        protected Func<MahjongContainer, bool> mPutOutFunc;

        public bool HasToken
        {
            get { return mHasToken; }
            set { mHasToken = value; }
        }

        public void SetMahjongNormalState(MahjongContainer item)
        {
            item.Lock = false;
            item.SetMahjongScript();
            item.SetThowOutCall(ThrowCardClickEvent);
            item.ResetPos();
        }

        public void SetRowCnt()
        {
            RowCnt = GameCenter.DataCenter.Config.HandCardCount + 1;
        }

        protected override void Start()
        {
            base.Start();
            ColCnt = 1;
            RowCnt = 14;
            PlayerHardLayer = gameObject.layer;
            AddActionToDic(HandcardStateTyps.SingleHu, SwitchFreezeState);
            AddActionToDic(HandcardStateTyps.Ting, SwitchFreezeState);
            AddActionToDic(HandcardStateTyps.Normal, SwitchNormalState);
            AddActionToDic(HandcardStateTyps.ChooseTingCard, SwitchChooseTingState);
            AddActionToDic(HandcardStateTyps.TingAndShowCard, SwitchTingAndShowCardsState);
        }

        protected virtual void SwitchNormalState(params object[] args)
        {
            MahjongContainer item = null;
            var list = mMahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.Lock = false;
                item.SetMahjongScript();
                item.SetAllowOffsetStatus(true);
                item.SetThowOutCall(ThrowCardClickEvent);
                item.ResetPos();
            }
            MahjongContorl.ClearSelectCard();
        }

        /// <summary>
        /// 冻结手牌，手牌不能进行何操作
        /// </summary>
        public void SwitchFreezeState(params object[] args)
        {
            MahjongContainer item;
            var list = mMahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.Lock = true;
                //item.ResetPos();
                item.RemoveMahjongScript();
            }
            MahjongContorl.ClearSelectCard();
        }

        protected virtual void SwitchChooseTingState(params object[] args)
        {
            MahjongContainer item;
            List<int> tingList = args[0] as List<int>;
            if (tingList == null || tingList.Count == 0) return;
            var list = mMahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.ResetPos();
                if (!tingList.Contains(item.Value))
                {
                    item.Lock = true;
                    item.RemoveMahjongScript();
                }
                else
                {
                    item.SetMahjongScript();
                    item.SetThowOutCall(TingpaiClickEvent);
                }
            }
            MahjongContorl.ClearSelectCard();
        }

        /// <summary>
        /// 出牌事件
        /// </summary>     
        protected virtual void ThrowCardClickEvent(Transform mahjong)
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
                    return;
                }
                //重置麻将位置
                ResetMahjongPos();
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

        /// <summary>
        /// 听牌点击事件
        /// </summary>      
        private void TingpaiClickEvent(Transform transf)
        {
            if (HasToken)
            {
                MahjongContainer item;
                var Mj = transf.GetComponent<MahjongContainer>();
                if (!Mj.Laizi && !Mj.Lock)
                {
                    HasToken = false;
                    GameCenter.EventHandle.Dispatch<C2STingArgs>((int)EventKeys.C2STing, (args) =>
                    {
                        args.Card = Mj.Value;
                        args.Prol = NetworkProls.Ting;
                    });

                    Mj.ResetPos();
                    var list = mMahjongList;
                    for (int i = 0; i < list.Count; i++)
                    {
                        item = list[i];
                        item.SetMahjongScript();
                        item.SetThowOutCall(ThrowCardClickEvent);
                    }
                }
            }
        }

        protected override void SwitchTingAndShowCardsState(params object[] args)
        {
            int[] tingList = args[0] as int[];
            if (tingList == null || tingList.Length == 0) return;

            System.Array.Sort(tingList);
            var queue = new Queue<int>(tingList);
            int niuValue = queue.Dequeue();
            MahjongContainer item;
            var list = mMahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.Lock = true;
                if (item.Value == niuValue)
                {
                    item.Lock = false;
                    if (queue.Count > 0)
                    {
                        niuValue = queue.Dequeue();
                    }
                    else
                    {
                        niuValue = 0;
                    }
                }
                item.RemoveMahjongScript();
            }
            SortMahjong();
            SetMahjongPos();
        }

        /// <summary>
        /// 手牌恢复未抬起状态
        /// </summary>
        public void HandCardsResetPos()
        {
            MahjongContorl.ClearSelectCard();
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].ResetPos();
            }
        }

        public void ResetMahjongPos()
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].ResetPos();
            }
        }

        public override void SortHandMahjong()
        {
            base.SortHandMahjong();
            MahjongContorl.ClearSelectCard();
        }

        protected override void AddMahjongToList(MahjongContainer item)
        {
            base.AddMahjongToList(item);
            item.ChangeToHardLayer(true);
            item.SetMahjongScript();
            item.SetThowOutCall(ThrowCardClickEvent);
        }

        public override MahjongContainer SetTingPaiNeedOutCard()
        {
            var lastMj = GetLastMj();
            lastMj.Lock = false;
            lastMj.SetMahjongScript();
            lastMj.SetThowOutCall(ThrowCardClickEvent);
            return lastMj;
        }

        public override MahjongContainer GetMahjongItemByValue(int value)
        {
            for (int i = mMahjongList.Count - 1; i >= 0; i--)
            {
                if (mMahjongList[i].Value == value)
                {
                    return mMahjongList[i];
                }
            }
            return null;
        }

        public override void GameResultRota(float time)
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].ChangeToHardLayer(false);
                mMahjongList[i].RemoveMahjongScript();
            }
            base.GameResultRota(time);
        }

        public override void SetLastCardPos(int value)
        {
            if (value == DefaultUtils.DefInt || mMahjongList.Find(item => item.Value == value) == null)//当前不是抓牌 是吃碰杠后的
            {
                base.SetLastCardPos(value);
                return;
            }
            if (mMahjongList.Count > 1)
            {
                MahjongContainer findItem = mMahjongList.Find((item) =>
                {
                    return item.Value == value;
                });

                if (findItem != null)
                {
                    mMahjongList.Remove(findItem);
                }
                SetMahjongPos();

                mMahjongList.Add(findItem);
                findItem.transform.localPosition = GetHardLastMjPos();
            }
        }

        public void SetLastCardPos(MahjongContainer item)
        {
            item.transform.localPosition = GetHardLastMjPos();
        }

        public MahjongContainer ReverseGetMahjong(int value)
        {
            for (int i = mMahjongList.Count - 1; i >= 0; i--)
            {
                if (value == mMahjongList[i].Value)
                {
                    return mMahjongList[i];
                }
            }
            return null;
        }

        public override MahjongContainer ThrowOut(int value)
        {
            var mahjong = ReverseGetMahjong(value);
            if (mahjong != null)
            {
                mahjong.ChangeToHardLayer(false);
            }
            return ThrowOut(mahjong);
        }

        /// <summary>
        /// 查询胡牌，显示听牌标记
        /// </summary>
        public void OnQueryMahjong(IList<int> cards)
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].IsTingCard = false;
                if (null != cards)
                {
                    for (int j = 0; j < cards.Count; j++)
                    {
                        if (cards[j] == mMahjongList[i].Value)
                        {
                            mMahjongList[i].IsTingCard = true;
                        }
                    }
                }
            }
        }

        protected override void SortMahjongForHand()
        {
            SortMahjong();
        }

        public void RemoveMahjong(MahjongContainer card)
        {
            mMahjongList.Remove(card);
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(card);
        }

        //当抢杠胡
        public virtual void OnQiangganghu(int value)
        {
            var findItem = mMahjongList.Find((item) =>
            {
                return item.Value == value;
            });
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(findItem);
            mMahjongList.Remove(findItem);
            SortHandMahjong();
        }

        protected virtual bool AniSortCheck(int lastCard, int getCard)
        {
            var flag = false;
            if (GameCenter.DataCenter.IsLaizi(getCard))
            {
                flag = true;
                //多个赖子牌情况
                //if (db.IsLaizi(last.Value) && last.Value < getCard.Value) flag = false;
            }
            else
            {
                flag = lastCard > getCard;
            }
            return flag;
        }

        public override void SortTweener()
        {
            //ResetMahjongPos();
            if (mMahjongList.Count < 2)
            {
                SetMahjongPos();
                return;
            }
            // 新抓的牌   
            var getCard = MahjongList[mMahjongList.Count - 1];
            // 麻将排序
            SortMahjong();
            // 排序后，最后一张牌
            var last = MahjongList[mMahjongList.Count - 1];
            // 动画类型 
            var flag = AniSortCheck(last.Value, getCard.Value);
            // 获取麻将位置点
            var layoutPos = GetLayoutPosition();
            // 平移动画时间
            var translationAniTimer = 0.03f * (MahjongList.Count - 1);
            // 偏移索引
            var offsetIndex = (RowCnt - MahjongList.Count) / 6;

            //上升 平移 下落
            if (flag)
            {
                Action translateAction = () =>
                {
                    for (int i = 0; i < MahjongList.Count; i++)
                    {
                        var item = MahjongList[i];
                        var xOffset = layoutPos[i + offsetIndex].x;
                        if (getCard == item)
                        {
                            item.Tweener.Transfrom(xOffset, translationAniTimer, () => { item.Tweener.Down(); });
                        }
                        else
                        {
                            item.Tweener.Transfrom(xOffset, translationAniTimer);
                        }
                    }
                };
                var offsetY = DefaultUtils.MahjongSize.y + 0.2f;
                getCard.Tweener.Up(offsetY, 0.2f, translateAction);
            }
            else
            {
                for (int i = 0; i < MahjongList.Count; i++)
                {
                    var item = MahjongList[i];
                    var xOffset = layoutPos[i + offsetIndex].x;
                    item.Tweener.Transfrom(xOffset, translationAniTimer);
                }
            }
        }
    }
}