using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Common.UI
{
    /// <summary>
    /// 自定义布局处理，默认无旋转，grid感觉不好用自己写一个处理
    /// </summary>
    public class DefLayout : MonoBehaviour
    {

        /// <summary>
        /// 每行的最大数量
        /// </summary>
        [SerializeField]
        public int maxPerLine = 0;
        /// <summary>
        /// 锚点位置，即每行的起始位置
        /// </summary>
        public UIWidget.Pivot pivot= UIWidget.Pivot.TopLeft;
        /// <summary>
        /// 布局方向
        /// </summary>
        [SerializeField]
        public Directon directon;
        [Tooltip("排序方式")]
        public SortType SortType= SortType.Grid;
        /// <summary>
        /// 隐藏时不处理
        /// </summary>
        [SerializeField]
        public bool HideWhenDisable = false;
        /// <summary>
        /// 按组排序
        /// </summary>
        [SerializeField]
        public bool SortByGroup;
        /// <summary>
        /// 单组的数量
        /// </summary>
        public int GroupNumber;
        /// <summary>
        /// 组间间隔宽度
        /// </summary>
        [SerializeField]
        public int GroupCellWidth;
        /// <summary>
        /// 组间间隔高度
        /// </summary>
        [SerializeField]
        public int GroupCellHeight;
        /// <summary>
        /// 宽
        /// </summary>
        [SerializeField]
        private int _cellWidth;
        /// <summary>
        /// 高
        /// </summary>
        [SerializeField]
        private int _cellHeight;
        /// <summary>
        /// 立即刷新标识
        /// </summary>
        private bool mReposition;

        /// <summary>
        /// 每组间距宽的和
        /// </summary>
        private int _totalGroupCellWidth;

        /// <summary>
        /// 每组间距高的和
        /// </summary>
        private int _totalGroupCellHeight;

        /// <summary>
        /// 存储布局的所有子对象
        /// </summary>
        private List<Transform> _itemList;

        /// <summary>
        /// 添加监听
        /// </summary>
        public Action<Transform> OnAddItem;

        /// <summary>
        /// 删除监听
        /// </summary>
        public Action<Transform> OnDeleteItem;

        /// <summary>
        /// 总长度
        /// </summary>
        public int TotalLenth; 

        void Awake()
        {
            _itemList=new List<Transform>();
            GetChildList();
        }

        public bool ResetPositionNow
        {
            get
            {
                return mReposition;
            }
            set
            {
                mReposition = value;
                if (mReposition)
                {
                    Reposition();
                }
            }
        }

        public int Width
        {
            get { return _cellWidth; }
            set
            {
                _cellWidth = value;
                Reposition();
            }
        }


        public int Height
        {
            get { return _cellHeight; }
            set
            {              
                _cellHeight = value;
                Reposition();
            }
        }

        public void SetLayoutLenth(int lenth)
        {
            maxPerLine = lenth;
            Reposition();
        }
        [ContextMenu("Execute")]
        public void Reposition()
        {
            maxPerLine = maxPerLine == 0 ? maxPerLine = int.MaxValue : maxPerLine;
            if(_itemList==null)
            {
                _itemList=new List<Transform>();
            }
            _itemList = GetChildList();
            ResetPosition(_itemList);
        }
        /// <summary>
        /// 获得子对象
        /// </summary>
        /// <returns></returns>
        public List<Transform> GetChildList()
        {
            List<Transform> list=new List<Transform>();
            Transform mTransForm=transform;
            for (int i = 0; i < mTransForm.childCount; i++)
            {
                Transform t = mTransForm.GetChild(i);
                if (!HideWhenDisable||(t&&NGUITools.GetActive(t.gameObject)))
                {
                    list.Add(t);
                }
            }
            _itemList = list;
            return list;
        }
        
        public int GetIndex(Transform trans)
        {
            return GetChildList().IndexOf(trans);
        }

        public Transform GetLastItem()
        {
            List<Transform> transList = GetChildList();
            if(transList.Count==0)
            {
                return null;
            }
            return transList.Last();
        }

        public Transform GetItem(int index)
        {
            List<Transform> list = GetChildList();
            return (index<list.Count)?list[index]:null;
        }

        public void AddItem(Transform trans, bool auto = false,float offsetScaleX=1,float offsetScaleY=1)
        {
            GameTools.AddChild(transform, trans,offsetScaleX,offsetScaleY);
            if (OnAddItem != null)
            {
                OnAddItem(trans);
            }
            ResetPositionNow = auto;

        }

        public void RemoveItem(Transform trans,bool auto=false)
        {
            if(OnDeleteItem!=null)
            {
                OnDeleteItem(trans);
            }
            ResetPositionNow = auto;
        }

        public void ClearAll()
        {
            foreach (var item in GetChildList())
            {
                DestroyImmediate(item.gameObject);
            }

        }
        protected virtual void ResetPosition(List<Transform> list)
        {
            int max = list.Count;
            int xIndex=0;
            int yIndex=0;
            for (int i = 0; i < max; i++)
            {
                Transform t = list[i];
                t.localPosition = GetPosByPos(i);
                t.name = string.Format("{0}_{1}", xIndex, yIndex);
            }
        }

        private void GetLineIndex(ref int indexX,ref int indexY,int index)
        {
            if(directon==Directon.Horizontal)
            {
                indexX = index%maxPerLine;
                indexY = index/maxPerLine;
            }
            else
            {
                indexX = index / maxPerLine;
                indexY = index % maxPerLine;
            }
        }
        
        private float GetSnapX(int index)
        {
            float x=0;
            bool isNotFirstOnLine = true;
            if (index==0)
            {
                _totalGroupCellWidth = 0;
                isNotFirstOnLine = false;
            }
            if (SortByGroup)
            {
                bool isNotFirstInGroup = index % GroupNumber != 0;
                if (isNotFirstInGroup && isNotFirstOnLine)
                {
                    _totalGroupCellWidth += GroupCellWidth;
                }
            }

            if (SortType==SortType.Grid)
            {
                x = index * _cellWidth + _totalGroupCellWidth;
            }
            else
            {
                x = 0;
            }
            
            switch (pivot)
            {
                case UIWidget.Pivot.TopRight:
                case UIWidget.Pivot.Right:
                case UIWidget.Pivot.BottomRight:
                    x*=-1;
                    break;
            }

            return x;
        }
        private float GetSnapY(int index)
        {
            float y = 0;
            if (index == 0)
            {
                _totalGroupCellHeight = 0;
            }
            if (SortByGroup)
            {
                bool isInGroup = index % GroupNumber == 0;
                if (isInGroup)
                {
                    _totalGroupCellHeight += GroupCellHeight;
                }
            }
            y = index * _cellHeight + _totalGroupCellHeight;
            switch (pivot)
            {
                case UIWidget.Pivot.Bottom:
                case UIWidget.Pivot.BottomLeft:
                case UIWidget.Pivot.BottomRight:
                    break;
                default:
                    y *= -1;
                    break;
            }
            return y;
        }
        /// <summary>
        /// 这个是根据当前已有的对象个数来处理的
        /// </summary>
        /// <returns></returns>
        public Vector3 GetNextItemPos()
        {
            return GetPosByPos(GetChildList().Count);
        }

        private Vector3 GetPosByPos(int index)
        {
            int xIndex = 0;
            int yIndex = 0;
            GetLineIndex(ref xIndex, ref yIndex, index);
            return new Vector3(GetSnapX(xIndex), GetSnapY(yIndex), 0);            
        }

        public Vector2 GetLayoutBounds()
        {
            var childCount = GetChildList().Count;
            return  new Vector2(Math.Min(childCount,maxPerLine)*Width, (childCount/maxPerLine+ (childCount%maxPerLine>0?1:0)) *Height);
            
        }

    }
    public enum Directon
    {
        Horizontal,
        Vertical,
    }
    public enum SortType
    {
        Grid,
        Table
    }
}