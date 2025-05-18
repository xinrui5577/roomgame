using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongHand : MahjongGroup
    {
        /// <summary>
        /// 显示牌数量
        /// </summary>
        protected int mTingAndShowCardsNum;

        protected HandcardStateTyps mCurrState = HandcardStateTyps.None;

        protected Dictionary<HandcardStateTyps, Action<object[]>> HandcardStateActions = new Dictionary<HandcardStateTyps, Action<object[]>>();

        protected List<Vector3> mLayoutPosition = new List<Vector3>();

        public HandcardStateTyps CurrState
        {
            get { return mCurrState; }
            protected set { mCurrState = value; }
        }

        protected virtual void Start()
        {
            AddActionToDic(HandcardStateTyps.Daigu, OnBuckleCard);
            AddActionToDic(HandcardStateTyps.SingleHu, OnBuckleCard);
            AddActionToDic(HandcardStateTyps.Ting, OnBuckleCard);
            AddActionToDic(HandcardStateTyps.TingAndShowCard, SwitchTingAndShowCardsState);
            AddActionToDic(HandcardStateTyps.Normal, (param) => { });
        }

        protected void AddActionToDic(HandcardStateTyps key, Action<object[]> action)
        {
            HandcardStateActions[key] = action;
        }

        public override void OnReset()
        {
            base.OnReset();
            mTingAndShowCardsNum = 0;
            SetHandCardState(0);
        }

        protected virtual void SortMahjong()
        {
            mMahjongList.Sort((a, b) =>
            {
                if (CurrState == HandcardStateTyps.TingAndShowCard)
                {
                    if (a.Lock && !b.Lock) return 1;
                    if (!a.Lock && b.Lock) return -1;
                }
                if (a.Laizi && !b.Laizi) return -1;
                if (!a.Laizi && b.Laizi) return 1;
                if (a.Value < b.Value) return -1;
                if (a.Value > b.Value) return 1;
                if (a.Value == b.Value)
                {
                    //index 小数排再前面
                    if (a.MahjongIndex > b.MahjongIndex) return 1;
                    if (a.MahjongIndex < b.MahjongIndex) return -1;
                }
                return 0;
            });
        }

        /// <summary>
        /// 设置手牌状态
        /// </summary>   
        public virtual bool SetHandCardState(HandcardStateTyps state, params object[] args)
        {
            if (CurrState == state) return false;
            mCurrState = state;
            if (HandcardStateActions.ContainsKey(state))
            {
                HandcardStateActions[state](args);
            }
            return true;
        }

        /// <summary>
        /// 扣牌
        /// </summary>
        public void OnBuckleCard(params object[] args)
        {
            SetMahjongPos();
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].Tweener.Rotate(new Vector3(-90, 0, 0), GameCenter.DataCenter.Config.TimeGetInCardRote);
            }
        }

        /// <summary>
        /// 听之后显示几张手牌
        /// </summary>
        /// <param name="args"></param>
        protected virtual void SwitchTingAndShowCardsState(params object[] args)
        {
            MahjongContainer item;
            var tingList = args[0] as int[];
            var list = mMahjongList;
            if (tingList == null || tingList.Length == 0) return;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                if (i < tingList.Length)
                {
                    item.Value = tingList[i];
                    item.Lock = false;
                }
                else
                {
                    item.Lock = true;

                }
                item.ShowNormal();
                item.ResetPos();
            }
            ShowCards(tingList.Length);
            mTingAndShowCardsNum = tingList.Length;
        }

        public virtual void ShowCards(int showNum)
        {
            var list = mMahjongList;
            SortHandMahjong();
            for (int i = 0; i < list.Count; i++)
            {
                int zhengFu = -1;
                if (i < showNum)
                {
                    zhengFu = 1;
                }
                list[i].Tweener.Rotate(new Vector3(90 * zhengFu, 0, 0), GameCenter.DataCenter.Config.TimeGetInCardRote);
            }
        }

        protected override void AddMahjongToList(MahjongContainer item)
        {
            if (item == null) return;
            mMahjongList.Add(item);
            item.transform.ExSetParent(transform);
        }

        public virtual MahjongContainer GetMahjongItemByValue(int value)
        {
            if (mMahjongList.Count == 0) return null;
            return mMahjongList[mMahjongList.Count - 1];
        }

        protected virtual void SortMahjongForHand()
        {
            var list = mMahjongList;
            if (list.Count <= 2)
            {
                return;
            }
            var index = 0;
            if (CurrState == HandcardStateTyps.TingAndShowCard && list.Count > mTingAndShowCardsNum)
            {
                index = UnityEngine.Random.Range(mTingAndShowCardsNum, list.Count - 2);
            }
            else
            {
                index = UnityEngine.Random.Range(0, list.Count - 2);
            }
            var last = list[list.Count - 1];
            list.Remove(last);
            list.Insert(index, last);
        }

        public override MahjongContainer GetInMahjong(int value)
        {
            var LastGetIn = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
            AddMahjongToList(LastGetIn);
            if (GameCenter.DataCenter.IsLaizi(value))
            {
                LastGetIn.Laizi = true;
            }
            //位子放到最后
            LastGetIn.transform.localPosition = GetHardLastMjPos();
            //转动方向
            LastGetIn.Tweener.RotateFrom(new Vector3(-90, 0, 0), Vector3.zero, GameCenter.DataCenter.Config.TimeGetInCardRote);
            return LastGetIn;
        }

        public MahjongContainer GetInMahjongNoAni(int value)
        {
            var LastGetIn = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
            AddMahjongToList(LastGetIn);
            if (GameCenter.DataCenter.IsLaizi(value))
            {
                LastGetIn.Laizi = true;
            }
            //位子放到最后
            LastGetIn.transform.localPosition = GetHardLastMjPos();
            return LastGetIn;
        }

        public override List<MahjongContainer> GetInMahjong(IList<int> value)
        {
            var list = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
            AddMahjongToList(list);
            SortMahjong();
            SetMahjongPos();
            return list;
        }

        public virtual void SetLastCardPos(int value)
        {
            if (mMahjongList.Count > 1)
            {
                var lastMj = mMahjongList[mMahjongList.Count - 1];
                lastMj.transform.localPosition = GetHardLastMjPos();
            }
        }

        public virtual void OnSendMahjong(IList<int> Value, float time, float wait)
        {
            MahjongContainer item;
            var list = GameCenter.Scene.MahjongCtrl.PopMahjong(Value);
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                AddMahjongToList(item);
                item.Tweener.RotateFrom(new Vector3(-90, 0, 0), Vector3.zero, time);
            }
            SetMahjongPos();
        }

        public virtual void OnSendOverSortMahjong(float time, float wait)
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                mMahjongList[i].Tweener.Rotate(new Vector3(-90, 0, 0), time);
            }
            ContinueTaskManager.NewTask().AppendActionTask(() =>
            {
                SetLaizi(GameCenter.DataCenter.Game.LaiziCard);
                for (int i = 0; i < mMahjongList.Count; i++)
                {
                    mMahjongList[i].Tweener.Rotate(new Vector3(0, 0, 0), time);
                }
            }, wait).Start();
        }

        public virtual void SortHandMahjong()
        {
            SortMahjong();
            SetMahjongPos();
        }

        public Vector3 GetHardLastMjPos()
        {
            var count = mMahjongList.Count;
            if (count < 1) return Vector3.one;
            var pos = mMahjongList[(count - 2) % count].transform.localPosition;
            return new Vector3(DefaultUtils.MahjongSize.x * 1.2f + pos.x, 0.2f, pos.z);
        }

        public virtual MahjongContainer ThrowOut(int value)
        {
            if (MahjongList.Count == 0) return null;

            MahjongContainer mahjong = null;
            if (GameCenter.DataCenter.CurrOpUserInfo().IsAuto)
            {
                mahjong = MahjongList[MahjongList.Count - 1];
            }
            else
            {
                var index = UnityEngine.Random.Range(0, MahjongList.Count);
                mahjong = MahjongList[index];
            }
            return ThrowOut(mahjong);
        }

        public virtual MahjongContainer ThrowOut(MahjongContainer mahjong)
        {
            if (mahjong == null) return null;
            mahjong.ShowNormal();
            mMahjongList.Remove(mahjong);
            //SortMahjongForHand();
            //SetMahjongPos();
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(mahjong);
            SortTweener();
            return mahjong;
        }

        protected override Vector3 GetPos(MahjongVecter index)
        {
            if ((Chair == 0 || Chair == 2) && !GameCenter.DataCenter.Config.SortByCenter)
            {
                var mahjongSize = DefaultUtils.MahjongSize;
                float Dis = -mMahjongList.Count * mahjongSize.x / 2 - mahjongSize.x * 1.2f / 2;
                var pos = new Vector3(Dis + mahjongSize.x * (index.x + 0.5f), mahjongSize.y * 0.5f, mahjongSize.z * 0.5f);
                pos.x = (pos.x - (RowCnt - mMahjongList.Count) / 2 * mahjongSize.x / 2);
                return pos;
            }
            else
            {
                var mahjongSize = DefaultUtils.MahjongSize;
                float Dis = -mMahjongList.Count * mahjongSize.x / 2 - mahjongSize.x * 1.2f / 2;
                return new Vector3(Dis + mahjongSize.x * (index.x + 0.5f), mahjongSize.y * 0.5f, mahjongSize.z * 0.5f);
            }
        }

        public void RemoveMahjong(IList<int> value)
        {
            for (int i = 0; i < value.Count; i++)
            {
                var item = GetMahjongItemByValue(value[i]);
                if (item != null)
                {
                    mMahjongList.Remove(item);
                    GameCenter.Scene.MahjongCtrl.PushMahjongToPool(item);
                }
            }
            SetMahjongPos();
        }

        public void RemoveMahjong(int value)
        {
            var item = GetMahjongItemByValue(value);
            if (item != null)
            {
                mMahjongList.Remove(item);
                GameCenter.Scene.MahjongCtrl.PushMahjongToPool(item);
            }
            SetMahjongPos();
        }

        public void RemoveAllMj()
        {
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                GameCenter.Scene.MahjongCtrl.PushMahjongToPool(mMahjongList[i]);
            }
            mMahjongList.Clear();
        }

        public virtual void GameResultRota(float time)
        {
            SetMahjongPos();
            MahjongContainer item;
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                item = mMahjongList[i];
                item.ResetPos();
                item.ShowNormal();
                item.Tweener.Rotate(new Vector3(90, 0, 0), time);
            }
        }

        public virtual void SetLaizi(int laizi)
        {
            if (laizi != DefaultUtils.DefValue)
            {
                MahjongContainer item;
                for (int i = 0; i < mMahjongList.Count; i++)
                {
                    item = mMahjongList[i];
                    item.Laizi = GameCenter.DataCenter.IsLaizi(item.Value);
                }
            }
            SortMahjong();
            SetMahjongPos();
        }

        public virtual MahjongContainer SetTingPaiNeedOutCard()
        {
            var lastMj = GetLastMj();
            lastMj.SetMjRota(0, 0, 0);
            return lastMj;
        }

        public virtual MahjongContainer PopMahjong()
        {
            var item = mMahjongList[mMahjongList.Count - 1];
            mMahjongList.RemoveAt(mMahjongList.Count - 1);
            GameCenter.Scene.MahjongCtrl.PushMahjongToPool(item);
            return item;
        }

        public List<MahjongContainer> GetInMahjongWithRoat(int[] value)
        {
            var list = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
            if (MahjongList.Count != 0)
            {
                var pos = MahjongList[MahjongList.Count - 1].transform.localPosition;
                AddMahjongToList(list);
                for (int i = 0; i < list.Count; i++)
                {
                    var item = list[i];
                    pos += new Vector3(DefaultUtils.MahjongSize.x, 0, 0);
                    item.transform.localPosition = pos;
                    item.Tweener.RotateFrom(new Vector3(-90, 0, 0), Vector3.zero, 0.15f);
                }
                ContinueTaskManager.NewTask().AppendActionTask(() =>
                {
                    SortMahjongForHand();
                    SetMahjongPos();
                }, 0.3f).Start();
            }
            else
            {
                AddMahjongToList(list);
                SortMahjongForHand();
                SetMahjongPos();
            }
            return list;
        }

        public override void SetMahjongPos()
        {
            var count = MahjongList.Count;
            var layoutPos = GetLayoutPosition();
            int offset = (RowCnt - count) / 6;
            for (int i = 0; i < count; i++)
            {
                var index = 0;
                if (layoutPos.Count - 1 >= i)
                {
                    index = i + offset;
                }
                else
                {
                    index = layoutPos.Count - 1;
                }
                MahjongList[i].transform.localPosition = layoutPos[index];
            }
        }

        protected List<Vector3> GetLayoutPosition()
        {
            if (mLayoutPosition.Count == 0)
            {
                var size = DefaultUtils.MahjongSize;
                var offsetPos = -RowCnt * size.x * 0.5f - size.x * 1.2f * 0.5f;
                for (int i = 0; i < RowCnt; i++)
                {
                    Vector3 v3 = new Vector3();
                    offsetPos = offsetPos + size.x;
                    v3[0] = offsetPos;
                    v3[1] = size[1] * 0.5f;
                    v3[2] = size[2] * 0.5f;
                    mLayoutPosition.Add(v3);
                }
            }
            return mLayoutPosition;
        }

        public virtual void SortTweener()
        {
            if (mMahjongList.Count < 2
                || CurrState == HandcardStateTyps.Ting
                || CurrState == HandcardStateTyps.NiuTing)
            {
                SetMahjongPos();
                return;
            }

            // 排序动画            
            var lastCard = MahjongList[mMahjongList.Count - 1];//抓来的牌
            var targetIndex = UnityEngine.Random.Range(0, mMahjongList.Count - 2);//插入索引           
            mMahjongList.Remove(lastCard);
            mMahjongList.Insert(targetIndex, lastCard);// 插入新位置    

            var layoutPos = GetLayoutPosition();
            var translationAniTimer = 0.02f * (MahjongList.Count - 1);
            var offsetIndex = (RowCnt - MahjongList.Count) / 6;

            Action translateAction = () =>
            {
                for (int i = 0; i < MahjongList.Count; i++)
                {
                    var item = MahjongList[i];
                    var xOffset = layoutPos[i + offsetIndex].x;
                    if (lastCard == item)
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
            lastCard.Tweener.Up(offsetY, 0.2f, translateAction);
        }
    }
}