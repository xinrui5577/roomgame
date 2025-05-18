using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base
{

    /// <summary>
    /// 单向可循环组件（单向等距版，先定义好每个Item的size；
    /// 根据size位于视图中的位置来确定所在的index）
    /// </summary>
    public class SingleTrackWrapContent : MonoBehaviour
    {
        public delegate void ItemFreshAction(GameObject item,int realIndex);
        public delegate void ContentFreshFinish(int showMinIndex, int showMaxIndex);
        [Tooltip("Item size")]
        public int ItemSize=100;
        [Tooltip("Wrap item prefab")]
        public GameObject PrefabItem;
        [Tooltip("生成UI数量")]
        public int ViewWrapCounts = 3;
        [Tooltip("直接移动强度")]
        public int MoveStrenth= 10000;
        /// <summary>
        /// Item 刷新事件
        /// </summary>
        public ItemFreshAction OnItemFresh;
        /// <summary>
        /// Content 刷新事件
        /// </summary>
        public ContentFreshFinish OnContentFresh;

        /// <summary>
        /// Main panel
        /// </summary>
        protected UIPanel MPanel
        {
            get
            {
                if (_mPanel== null)
                {
                    _mPanel= NGUITools.FindInParents<UIPanel>(gameObject);
                }
                return _mPanel;
            }
        }

        private UIPanel _mPanel;

        /// <summary>
        /// Main Scroll
        /// </summary>
        protected UIScrollView MScroll
        {
            get
            {
                if (_scroll==null)
                {
                    _scroll = MPanel.GetComponent<UIScrollView>();
                }
                return _scroll;
            }
        }

        private UIScrollView _scroll;

        /// <summary>
        /// Child UI
        /// </summary>
        protected List<Transform> MChildren=new List<Transform>();

        public List<Transform> Children
        {
            get
            {
                return MChildren;
            }
        }

        /// <summary>
        /// Main panel init pos
        /// </summary>
        protected Vector3 MPanelInitVec;
        /// <summary>
        /// 最小索引
        /// </summary>
        protected int MinIndex = 100;
        /// <summary>
        /// 最大索引
        /// </summary>
        protected int MaxIndex;

        private int _totalCount;

        protected Transform MTrans;

        protected GameObject MGameObj;
        public void Init(int dataCount)
        {
            if (dataCount<=0)
            {
                return;
            }
            SetRange(dataCount);
            MTrans = transform;
            MGameObj = gameObject;
            MChildren.Clear();
            if (MScroll == null) return;
            MPanel.onClipMove = delegate{ WrapContent(); };
            MPanelInitVec = MPanel.transform.localPosition;
            InitChild();
            if (CheckItemsShow())
            {
                WrapContent();
            }
        }

        /// <summary>
        /// Init Child
        /// </summary>
        private void InitChild()
        {
            if (MChildren.Count==0)
            {
                for (int i = 0; i < ViewWrapCounts; i++)
                {
                    MChildren.Add(MGameObj.transform.GetChildView(i, PrefabItem.GetComponent<YxView>()).transform);
                }
                ResetChildPositions();
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        private void WrapContent()
        {
            float extents;
            float ext2;
            Vector3 center;
            GetShowRange(out extents, out ext2, out center);
            for (int i = 0, imax = MChildren.Count; i < imax; ++i)
            {
                Transform t = MChildren[i];
                float distance = GetDistance(t.localPosition, center);
                Vector3 pos = t.localPosition;
                if (distance < -extents)
                {
                    pos = SetPos(pos, ext2);
                }
                else if (distance > extents)
                {
                    pos = SetPos(pos, -ext2);
                }
                CheckItemVaild(t, pos);
            }

            if (OnContentFresh!=null)
            {
                var showMax = int.MinValue;
                var showMin = int.MaxValue;
                GetSideIndex(ref showMax, ref showMin);
                OnContentFresh(showMin,showMax);
            }
        }
        /// <summary>
        /// 检测索引有效性
        /// </summary>
        /// <returns></returns>
        private bool CheckIndexVaild()
        {
            return MaxIndex - MinIndex>=ViewWrapCounts;
        }

        /// <summary>
        /// 检测Item显示相关
        /// </summary>
        /// <returns></returns>
        private bool CheckItemsShow()
        {
            if (!CheckIndexVaild())//如果不满足显示限制条件，则隐藏相应的item
            {
                for (int i = 0, imax = MChildren.Count; i < imax; ++i)
                {
                    Transform t = MChildren[i];
                    if (!CheckItemVaild(t, t.localPosition))
                    {
                        t.gameObject.TrySetComponentValue(false);
                    }
                }
                return false;
            }
            else
            {
                for (int i = 0, imax = MChildren.Count; i < imax; ++i)
                {
                    MChildren[i].gameObject.TrySetComponentValue(true);
                }
            }
            return true;
        }

        /// <summary>
        /// 刷新UI
        /// </summary>
        /// <param name="item"></param>
        protected virtual void UpdateItem(Transform item)
        {
            if (OnItemFresh != null)
            {
                int realIndex = GetRealIndex(item.localPosition);
                OnItemFresh(item.gameObject,realIndex);
            }
        }
        /// <summary>
        /// 检测Item 有效性
        /// </summary>
        /// <param name="item"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected bool CheckItemVaild(Transform item,Vector3 pos)
        {
            int realIndex = GetRealIndex(pos);
            if (realIndex <=MaxIndex && realIndex >=MinIndex&& _totalCount!=0)
            {
                item.gameObject.TrySetComponentValue(true);
                item.localPosition = pos;
                UpdateItem(item);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 重置子UI 布局
        /// </summary>
        public virtual void ResetChildPositions()
        {
            int flag = MScroll.movement == UIScrollView.Movement.Vertical ? -1 :1;
            for (int i = 0,showIndex=0, imax = MChildren.Count; i < imax; ++i)
            {
                Transform t = MChildren[i];
                if (t.gameObject.activeInHierarchy)
                {
                    t.localPosition = Vector3.zero;
                    t.localPosition=SetPos(t.localPosition, flag*showIndex * ItemSize);
                    CheckItemVaild(t, t.localPosition);
                    showIndex++;
                }
            }
        }

        /// <summary>
        /// 获取对应位置的真实索引（UI索引）
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected int GetRealIndex(Vector3 pos)
        {
            var realIndex = Mathf.RoundToInt((MScroll.movement == UIScrollView.Movement.Horizontal?pos.x:pos.y)/ ItemSize);
            return realIndex;
        }

        /// <summary>
        ///  Item Change
        /// </summary>
        /// <param name="type"></param>
        /// <param name="modifyIndex"></param>
        /// <param name="dataCount"></param>
        public void OnItemModify(ModifyType type,int modifyIndex=0,int dataCount=0)
        {
            switch (type)
            {
                case ModifyType.Insert:
                    SetRange(GetRangeLimit()-1);
                    ItemMoveCheck(ItemSize,modifyIndex,dataCount);
                    break;
                case ModifyType.Remove:
                    SetRange(GetRangeLimit()+1);
                    ItemMoveCheck(-ItemSize, modifyIndex, dataCount);
                    break;
                case ModifyType.Update:
                    break;
            }
            if (CheckItemsShow())
            {
                WrapContent();
            }
        }
        /// <summary>
        /// Item 移动检测
        /// </summary>
        /// <param name="moveDistance"></param>
        /// <param name="modifyIndex"></param>
        /// <param name="dataCount"></param>
        private void ItemMoveCheck(int moveDistance, int modifyIndex, int dataCount)
        {
            var showMax = int.MinValue;
            var showMin = int.MaxValue;
            GetSideIndex(ref showMax, ref showMin);
            var panelMove = SetPos(MPanel.transform.localPosition, moveDistance);
            if (modifyIndex > showMax)
            {
                SpringPanel.Begin(MPanel.gameObject, panelMove, MoveStrenth);
            }
            if (modifyIndex <= showMax && modifyIndex >= showMin && showMax < MaxIndex)
            {
                for (int i = 0, max = MChildren.Count; i < max; ++i)
                {
                    Transform t = MChildren[i];
                    if (t.gameObject.activeInHierarchy)
                    {
                        t.localPosition = SetPos(t.localPosition, moveDistance);
                    }
                }
                SpringPanel.Begin(MPanel.gameObject, panelMove, MoveStrenth);
            }
            if (dataCount <= ViewWrapCounts)
            {
                if (showMax < MaxIndex)
                {
                    SpringPanel.Begin(MPanel.gameObject, MPanelInitVec, MoveStrenth);
                }
                if (dataCount < ViewWrapCounts)
                {
                    SpringPanel.Begin(MPanel.gameObject, MPanelInitVec, MoveStrenth);
                }
            }
        }

        /// <summary>
        /// 回去对应点与中点的距离(根据对应方向计算x与y)
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="centerPos"></param>
        /// <returns></returns>
        private float GetDistance(Vector3 pos,Vector3 centerPos)
        {
            return MScroll.movement == UIScrollView.Movement.Vertical? pos.y - centerPos.y: pos.x - centerPos.x;
        }
        /// <summary>
        /// 获取计算后的位置
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        private Vector3 SetPos(Vector3 pos,float range)
        {
            pos+= (MScroll.movement == UIScrollView.Movement.Vertical)?Vector3.up * range:Vector3.right* range;
            return pos;
        }

        /// <summary>
        /// 获取显示范围相关参数
        /// </summary>
        /// <param name="extents"></param>
        /// <param name="ext2"></param>
        /// <param name="center"></param>
        private void GetShowRange(out float extents, out float ext2, out Vector3 center)
        {
            extents = ItemSize * MChildren.Count * 0.5f;
            Vector3[] corners = MPanel.worldCorners;
            for (int i = 0; i < 4; ++i)
            {
                Vector3 v = corners[i];
                v = MTrans.InverseTransformPoint(v);
                corners[i] = v;
            }
            center = Vector3.Lerp(corners[0], corners[2], 0.5f);
            ext2 = extents * 2f;
        }
        /// <summary>
        /// 获取边界索引
        /// </summary>
        /// <param name="maxShowIndex"></param>
        /// <param name="minShowIndex"></param>
        protected void GetSideIndex(ref int maxShowIndex, ref int minShowIndex)
        {
            for (int i = 0, lenth = MChildren.Count; i < lenth; i++)
            {
                var item = MChildren[i];
                if (item.gameObject.activeInHierarchy)
                {
                    int realIndex = GetRealIndex(MChildren[i].localPosition);
                    maxShowIndex = Math.Max(maxShowIndex, realIndex);
                    minShowIndex = Math.Min(minShowIndex, realIndex);
                }
            }
        }

        public void SetRange(int value)
        {
            _totalCount = value;
            if (MScroll.movement == UIScrollView.Movement.Vertical)
            {
                MaxIndex = 0;
                MinIndex = Math.Min(-value+1,0);
            }
            else
            {
                MinIndex = 0;
                MaxIndex = Math.Max(value - 1,0);
            }
        }

        protected int GetRangeLimit()
        {
            return MScroll.movement == UIScrollView.Movement.Vertical ? MinIndex : MaxIndex;
        }
    }
    /// <summary>
    /// UI 变动类型
    /// </summary>
    public enum ModifyType
    {
        Insert,
        Remove,
        Update
    }
}
