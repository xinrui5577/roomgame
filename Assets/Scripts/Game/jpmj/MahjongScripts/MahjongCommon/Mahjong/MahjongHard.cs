using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.GameTable;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Tool;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    public class MahjongHard : MahjongGroup
    {
        protected MahjongItem LastGetIn;
        protected bool _isTing;

        public bool IsTing
        {
            get { return _isTing; }
        }


        public override MahjongItem GetInMahjong(int value)
        {
            LastGetIn = MahjongManager.Instance.GetMahjong(value);
            AddMahjong(LastGetIn);

            string strValue = "";
            foreach (MahjongItem item1 in MahjongList)
            {
                strValue += "[" + item1.ToString() + "] ";
            }

            YxDebug.Log("查找麻将 " + strValue);

            //位子放到最后
            LastGetIn.transform.localPosition = GetHardLastMjPos();
            //转动方向
            LastGetIn.RotaTo(new Vector3(-90, 0, 0), Vector3.zero, GameConfig.GetInCardRoteTime, GameConfig.GetInCardWaitTime);

            return LastGetIn;
        }

        protected virtual void SortMahjongForHand()
        {
            if (MahjongList.Count < 2)
            {
                return;
            }
            //这里是假的排序 把最后一张牌 放在那里 是随机放的
            var index = Random.Range(0, MahjongList.Count - 2);
            var last = MahjongList[MahjongList.Count - 1];
            MahjongList.Remove(last);
            MahjongList.Insert(index, last);
        }

        public override List<MahjongItem> GetInMahjong(int[] value)
        {
            List<MahjongItem> list = MahjongManager.Instance.GetMahjongList(value);

            AddMahjong(list);

            SortMahjong();

            SetMahjongPos();

            return list;
        }

        public List<MahjongItem> GetInMahjongWithRoat(int[] value)
        {
            List<MahjongItem> list = MahjongManager.Instance.GetMahjongList(value);
            var pos = Vector3.zero;
            if (MahjongList.Count > 0)
            {
                pos = MahjongList[MahjongList.Count - 1].transform.localPosition;
            }
            AddMahjong(list);
            foreach (MahjongItem item in list)
            {
                pos += new Vector3(MahjongManager.MagjongSize.x, 0, 0);
                item.transform.localPosition = pos;

                item.RotaTo(new Vector3(-90, 0, 0), Vector3.zero, GameConfig.GetInCardRoteTime, GameConfig.GetInCardWaitTime, ()
                                                                                                                             =>
                    {
                        YxDebug.Log("补张的转动动画 完成");
                    });
            }

            DelayTimer.StartTimer(GameConfig.GetInCardRoteTime + GameConfig.GetInCardWaitTime, () =>
            {
                SortMahjongForHand();

                SetMahjongPos();

                YxDebug.Log("补张动画接受后排序");
            });

            return list;
        }

        public Vector3 GetPos(List<MahjongItem> list)
        {
            var pos = Vector3.zero;
            if (MahjongList.Count > 0)
            {
                pos=MahjongList[MahjongList.Count - 1].transform.localPosition;
            }
            AddMahjong(list);
            return pos;
        }

        public virtual void SetLastCardPos(int value)
        {
            if (MahjongList.Count > 1)
            {
                MahjongItem lastMj = MahjongList[MahjongList.Count - 1];
                lastMj.transform.localPosition = GetHardLastMjPos();
            }
        }

        public virtual void OnSendMahjong(int[] Value, float time, float wait)
        {
            List<MahjongItem> List = MahjongManager.Instance.GetMahjongList(Value);

            foreach (MahjongItem item in List)
            {
                AddMahjong(item);
                item.RotaTo(new Vector3(-90, 0, 0), Vector3.zero, time, wait);
            }

            SetMahjongPos();
        }

        public virtual void OnSendMahjongForJp(int[] Value, float time, float wait, int laizi)
        {
            List<MahjongItem> List = MahjongManager.Instance.GetMahjongList(Value);

            foreach (MahjongItem item in List)
            {
                AddMahjong(item);
                if (item.Value == laizi) item.IsSign = true;
                item.RotaTo(new Vector3(-90, 0, 0), Vector3.zero, time, wait);
            }
            SortMahjongForHand();
            SetMahjongPos();
        }


        public virtual void OnSendOverSortMahjong(float time, float wait, int laizi)
        {
            foreach (MahjongItem item in MahjongList)
            {
                item.RotaTo(new Vector3(-90, 0, 0), time);
            }

            DelayTimer.StartTimer(wait, () =>
            {
                SortMahjongForHand();
                SetMahjongPos();

                foreach (MahjongItem item in MahjongList)
                {
                    item.RotaTo(new Vector3(0, 0, 0), time);
                }
            });
        }

        public virtual void SortHandMahjong()
        {
            SortMahjongForHand();
            SetMahjongPos();
        }

        protected Vector3 GetHardLastMjPos()
        {
            if (MahjongList.Count < 1) return Vector3.one;

            Vector3 pos = MahjongList[MahjongList.Count - 2].transform.localPosition;
            pos += new Vector3(MahjongManager.MagjongSize.x * 1.2f, 0, 0);
            return pos;
        }

        public virtual MahjongItem ThrowOut(int value)
        {
            MahjongItem Mahjong = GetMahjongItemByValue(value);

            Mahjong.ShowNormal();

            MahjongManager.Instance.ExchangeByValue(value, Mahjong);

            MahjongList.Remove(Mahjong);

            SortMahjongForHand();

            if (GameConfig.GetInEffect)
            {
                PickUpMahjongAction(Mahjong);
            }
            else
            {
                SetMahjongPos();
            }

            MahjongManager.Instance.Recycle(Mahjong);

            LastGetIn = null;

            return Mahjong;
        }

        public virtual MahjongItem ThrowOutByValue(int value)
        {
            MahjongItem Mahjong = null;
            for (int i = MahjongList.Count - 1; i >= 0; i--)
            {
                if (MahjongList[i].Value == value)
                {
                    Mahjong = MahjongList[i];
                }
            }
            if (Mahjong == null)
            {
                Mahjong = GetMahjongItemByValue(value);
            }
            Mahjong.ShowNormal();
            MahjongManager.Instance.ExchangeByValue(value, Mahjong);
            MahjongList.Remove(Mahjong);
            SortMahjongForHand();
            if (GameConfig.GetInEffect)
            {
                PickUpMahjongAction(Mahjong);
            }
            else
            {
                SetMahjongPos();
            }
            MahjongManager.Instance.Recycle(Mahjong);
            LastGetIn = null;
            return Mahjong;
        }

        public virtual void PickUpMahjongAction(MahjongItem item)
        {
            if (LastGetIn == null)
            {
                SetMahjongPos();
                return;
            }
            //扔出的牌 是刚刚抓上来的 不需要处理
            if (item == LastGetIn)
            {
                return;
            }
            //获得 最后麻将 应该在的位子
            var index = new MjIndex();
            foreach (var mjItem in MahjongList)
            {
                if (mjItem == LastGetIn)
                {
                    break;
                }
                index = GetNextIndex(index);
            }
            //如果在最后 直接排序 不要动作
            if (index.x + 1 == MahjongList.Count)
            {
                SetMahjongPos();
                return;
            }

            var pos = GetPos(index);
            LastGetIn.GetInMjAction(pos, (s) =>
            {
                //移动结束后 所有的麻将 要移动到相对应的位子
                if (s.Equals("MoveFinish"))
                {
                    index = new MjIndex();
                    foreach (var mahjongItem in MahjongList)
                    {
                        if (mahjongItem != LastGetIn)
                        {
                            var mjPos = GetPos(index);
                            mahjongItem.MoveToAction(mjPos, GameConfig.PickUpTime);
                        }
                        index = GetNextIndex(index);
                    }
                }
                else if (s.Equals("PutDownFinish"))
                {
                    SetMahjongPos();
                }
            });

        }

        public virtual MahjongItem GetMahjongItemByValue(int value)
        {
            if (IsTing)
                return MahjongList[MahjongList.Count - 1];

            int index = Random.Range(0, MahjongList.Count);

            return MahjongList[index];
        }

        protected override Vector3 GetPos(MjIndex index)
        {
            Vector3 mahjongSize = MahjongManager.MagjongSize;
            float Dis = -MahjongList.Count * mahjongSize.x / 2 - mahjongSize.x * 1.2f / 2;

            return new Vector3(Dis + mahjongSize.x * (index.x + 0.5f), mahjongSize.y * 0.5f, mahjongSize.z * 0.5f);
        }

        public void RemoveMahjong(List<int> value, bool sort = true)
        {
            for (int i = 0; i < value.Count; i++)
            {
                MahjongItem item = GetMahjongItemByValue(value[i]);
                if (item != null)
                {
                    MahjongList.Remove(item);
                    MahjongManager.Instance.Recycle(item);
                }
            }

            if (sort)
                SetMahjongPos();
        }

        public virtual void RemoveMahjongByValue(int value, bool sort = true)
        {
            MahjongItem temp = MahjongList.Find((a) => { return a.Value == value; });

            if (temp != null)
            {
                MahjongList.Remove(temp);
                MahjongManager.Instance.Recycle(temp);
            }
            else
            {
                MahjongItem item = GetMahjongItemByValue(value);
                if (item != null)
                {
                    MahjongList.Remove(item);
                    MahjongManager.Instance.Recycle(item);
                }
            }

            if (sort)
                SetMahjongPos();
        }

        public void RemoveMahjong(int value, bool sort = true)
        {
            MahjongItem item = GetMahjongItemByValue(value);
            if (item != null)
            {
                MahjongList.Remove(item);
                MahjongManager.Instance.Recycle(item);
            }

            if (sort)
                SetMahjongPos();
        }

        public void RemoveAllMj()
        {
            foreach (MahjongItem item in MahjongList)
            {
                MahjongManager.Instance.Recycle(item);
            }

            MahjongList.Clear();
        }

        public virtual void GameResultRota(float time)
        {
            SetMahjongPos();

            foreach (MahjongItem item in MahjongList)
            {
                if (item.gameObject.GetComponent<MouseRoll>() != null)
                {
                    item.gameObject.GetComponent<MouseRoll>().ResetPos();
                }
                item.ShowNormal();
                item.RotaTo(new Vector3(90, 0, 0), time);
            }
        }

        public virtual void OnLieLow()
        {
            SetMahjongPos();
            foreach (MahjongItem item in MahjongList)
            {
                item.ShowNormal();
                item.transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            }
        }

        public virtual void SetLaizi(int laizi, bool sort = true)
        {
            if (laizi != UtilDef.NullMj)
            {
                foreach (MahjongItem item in MahjongList)
                {
                    //判断赖子是春夏秋冬
                    if (laizi >= 96 && laizi < 100)
                    {
                        if ((item.Value >= 96) && (item.Value < 100))
                            item.IsSign = true;
                    }
                    else if ((laizi >= 100) && (laizi < 104))//判断赖子是梅兰竹菊
                    {
                        if (item.Value >= 100 && item.Value < 104)
                            item.IsSign = true;
                    }
                    else if (item.Value == laizi)
                        item.IsSign = true;
                    else
                        item.IsSign = false;
                }
            }

            if (sort)
            {
                SortMahjong();
                SetMahjongPos();
            }
        }

        public virtual void SetTingPai()
        {
            _isTing = true;

            SetMahjongPos();

            foreach (MahjongItem item in MahjongList)
            {
                item.RotaTo(new Vector3(-90, 0, 0), GameConfig.GetInCardRoteTime);
            }
        }

        public virtual void BuckleCards()
        {
            SetMahjongPos();

            foreach (MahjongItem item in MahjongList)
            {
                item.RotaTo(new Vector3(-90, 0, 0), GameConfig.GetInCardRoteTime);
            }
        }

        public override void Reset()
        {
            base.Reset();
            _isTing = false;
        }

        public virtual void SetTingPaiNeedOutCard()
        {
            var lastMj = GetLastMj();
            lastMj.SetMjRota(0, 0, 0);
        }

        //两个赖子
        public virtual void OnSendOverSortMahjong(float time, float wait, int laizi, int laizi1) { }
        public virtual void SetLaizi(int laizi, int laizi1, bool sort = true) { }

        public virtual MahjongItem PopMahjong()
        {
            MahjongItem item = MahjongList[MahjongList.Count - 1];
            MahjongList.RemoveAt(MahjongList.Count - 1);
            MahjongManager.Instance.Recycle(item);
            return item;
        }

        public virtual void SortAndAdjustMahjong()
        {
            SortMahjong();
            OnLieLow();
        }
    }
}