using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 麻将坐标
    /// </summary>
    public struct MahjongVecter
    {
        public int x;
        public int y;
        public int z;

        public MahjongVecter(MahjongVecter temp)
        {
            x = temp.x;
            y = temp.y;
            z = temp.z;
        }
    }

    public class MahjongGroup : MonoBehaviour
    {
        public int MahjongCnt;//麻将的个数
        public int RowCnt;//一行的个数
        public int ColCnt;//列     
        public int Chair;

        protected MahjongVecter mNowVecter;
        protected List<MahjongContainer> mMahjongList = new List<MahjongContainer>();

        public virtual void OnInitalization() { }

        public List<MahjongContainer> MahjongList { get { return mMahjongList; } }

        public virtual MahjongContainer this[int index]
        {
            get
            {
                if (index < MahjongList.Count) { return MahjongList[index]; }
                return null;
            }
        }

        protected virtual void AddMahjongToList(MahjongContainer item)
        {
            if (item == null) return;
            item.transform.ExSetParent(transform);
            mMahjongList.Add(item);
        }

        protected virtual void AddMahjongToList(IList<MahjongContainer> list)
        {
            if (null == list || list.Count == 0) return;
            for (int i = 0; i < list.Count; i++)
            {
                AddMahjongToList(list[i]);
            }
        }

        public virtual void SetMahjongPos()
        {
            MahjongContainer item;
            mNowVecter = new MahjongVecter();
           
            for (int i = 0; i < mMahjongList.Count; i++)
            {
                item = mMahjongList[i];
                item.ResetPos();
                Transform mahjongTf = item.transform;
                Vector3 pos = GetPos(mNowVecter);
                mahjongTf.localPosition = pos;
                mNowVecter = GetNextIndex(mNowVecter);
            }
        }

        protected virtual Vector3 GetPos(MahjongVecter index)
        {
            Vector3 mahjongSize = DefaultUtils.MahjongSize;
            return new Vector3(mahjongSize.x * (index.x + 0.5f), -mahjongSize.y * (0.5f + mNowVecter.z), -mahjongSize.z * (index.y + 0.5f));
        }

        protected virtual MahjongVecter GetNextIndex(MahjongVecter index)
        {
            MahjongVecter next = new MahjongVecter(index);
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

        /// <summary>
        /// 添加麻将到列表
        /// </summary>
        protected virtual void AddMahjongToGroup(MahjongContainer item)
        {
            if (item == null) return;
            item.transform.ExSetParent(transform);
            mMahjongList.Add(item);
        }

        /// <summary>
        /// 添加麻将组到列表
        /// </summary>
        protected virtual void AddMahjongToGroup(IList<MahjongContainer> list)
        {
            if (null == list || list.Count == 0) return;
            for (int i = 0; i < list.Count; i++)
            {
                AddMahjongToGroup(list[i]);
            }
        }

        public virtual MahjongContainer GetInMahjong(int value)
        {
            MahjongContainer item = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
            AddMahjongToList(item);
            SetMahjongPos();
            if (GameCenter.DataCenter.IsLaizi(value))
            {
                item.Laizi = true;
            }
            return item;
        }

        public virtual MahjongContainer GetInMahjong(MahjongContainer item)
        {
            if (null == item) return null;
            AddMahjongToList(item);
            SetMahjongPos();
            return item;
        }

        public virtual List<MahjongContainer> GetInMahjong(IList<int> value)
        {
            if (null != value && value.Count == 0) return null;
            List<MahjongContainer> list = GameCenter.Scene.MahjongCtrl.PopMahjong(value);
            AddMahjongToList(list);
            SetMahjongPos();
            return list;
        }

        public virtual void OnReset()
        {
            mNowVecter = new MahjongVecter();
            for (int i = 0; i < MahjongList.Count; i++)
            {
                GameCenter.Scene.MahjongCtrl.PushMahjongToPool(MahjongList[i]);
            }
            MahjongList.Clear();
        }

        public Vector3 GetLastMjPos()
        {
            if (mMahjongList.Count > 0)
            {
                return mMahjongList[mMahjongList.Count - 1].transform.position;
            }
            return Vector3.zero;
        }

        public virtual MahjongContainer GetLastMj()
        {
            if (mMahjongList.Count > 0)
            {
                return mMahjongList[mMahjongList.Count - 1];
            }
            return null;
        }
    }
}