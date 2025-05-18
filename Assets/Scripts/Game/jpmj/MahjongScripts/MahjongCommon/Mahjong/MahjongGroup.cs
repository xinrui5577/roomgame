using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.Mahjong
{
    /// <summary>
    /// 麻将集合
    /// 左下角对齐
    /// 
    /// </summary>
    
    public class MjIndex
    {
        public int x;
        public int y;
        public int z;

        public MjIndex()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public MjIndex(MjIndex temp)
        {
            x = temp.x;
            y = temp.y;
            z = temp.z;
        }
    }

    public class MahjongGroup : MonoBehaviour
    {
        public int MahjongCnt;          //麻将的个数
        public int RowCnt;              //一行的个数
        public int ColCnt = 1;              //列
        public int Chair;

        protected List<MahjongItem> MahjongList = new List<MahjongItem>();

        public List<MahjongItem> GetMahjongList { get { return MahjongList; } }

        /// <summary>
        /// 麻将列表中的所有麻将指定一个新的localRotation
        /// </summary>
        /// <param name="v"></param>
        public virtual void ChangeMahJonitemListRotation(Vector3 v)
        {
            foreach (var miItem in MahjongList)
            {
                miItem.transform.localRotation = Quaternion.Euler(v);
            }
        }

        //显示的是 这个面向值
        protected virtual void SortMahjong()
        {
            MahjongList.Sort((a, b) =>
            {
                //先判断是否是赖子
                if (a.IsSign && !b.IsSign)
                    return -1;
                if (!a.IsSign && b.IsSign)
                    return 1;

                if (a.Value < b.Value)
                    return -1;

                if (a.Value > b.Value)
                    return 1;

                if (a.Value == b.Value)
                {
                    if (a.MahjongIndex > b.MahjongIndex)
                        return -1;

                    if (a.MahjongIndex < b.MahjongIndex)
                        return 1;
                }

                return 0;
            });
        }

        protected MjIndex NowIndex;

        protected virtual void SetMahjongPos()
        {
            NowIndex = new MjIndex();

            for (int i = 0; i < MahjongList.Count; i++)
            {
                Transform mahjongTf = MahjongList[i].transform;

                Vector3 pos = GetPos(NowIndex);
                mahjongTf.localPosition = pos;

                NowIndex = GetNextIndex(NowIndex);
            }
        }

        protected virtual Vector3 GetPos(MjIndex index)
        {
            Vector3 mahjongSize = MahjongManager.MagjongSize;

            return new Vector3(mahjongSize.x * (index.x + 0.5f), -mahjongSize.y * (0.5f + NowIndex.z), -mahjongSize.z * (index.y + 0.5f));
        }

        protected virtual MjIndex GetNextIndex(MjIndex index)
        {
            MjIndex next = new MjIndex(index);
            if (RowCnt != 0 && next.x >= RowCnt - 1)
            {
                next.x = 0;
                if (next.y++ >= ColCnt - 1)
                {
                    next.z++;
                    next.y = 0;
                }
            }
            else
            {
                next.x++;
            }

            return next;
        }

        protected virtual void AddMahjong(MahjongItem item)
        {
            MahjongList.Add(item);
            item.transform.SetParent(transform);
            item.Reset();
        }

        protected virtual void AddMahjong(List<MahjongItem> item)
        {
            foreach (MahjongItem mahjongItem in item)
            {
                AddMahjong(mahjongItem);
            }
        }

        public virtual Vector3 GetNextMjPos()
        {
            MjIndex next = GetNextIndex(NowIndex);

            return GetPos(next);
        }

        public virtual MahjongItem GetInMahjong(int value)
        {
            MahjongItem item = MahjongManager.Instance.GetMahjong(value);

            AddMahjong(item);

            SetMahjongPos();

            return item;
        }

        public virtual MahjongItem GetInMahjong(MahjongItem item)
        {
            AddMahjong(item);

            SetMahjongPos();

            return item;
        }

        public virtual MahjongItem GetInMahjong(int value, bool isSign)
        {
            MahjongItem item = GetInMahjong(value);
            if (isSign) item.IsSign = true;
            return item;
        }

        public virtual List<MahjongItem> GetInMahjong(int[] value)
        {
            List<MahjongItem> list = MahjongManager.Instance.GetMahjongList(value);

            AddMahjong(list);

            SetMahjongPos();

            return list;
        }

        public virtual void Reset()
        {
            NowIndex = new MjIndex();

            MahjongList.Clear();
        }

        public  Vector3 GetLastMjPos()
        {
            if (MahjongList.Count > 0)
            {
                return MahjongList[MahjongList.Count - 1].transform.position;
            }

            return Vector3.zero;
        }

        public int GetMjCnt()
        {
            return MahjongList.Count;
        }

        public virtual MahjongItem GetLastMj()
        {
            if (MahjongList.Count > 0)
                return MahjongList[MahjongList.Count - 1];

            return null;
        }
    }
}
