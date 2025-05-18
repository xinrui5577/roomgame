using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.lyzz2d.Utils.UI
{
    /// <summary>
    ///     自定义布局处理，默认无旋转，grid感觉不好用自己写一个处理
    /// </summary>
    public class DefLayout : MonoBehaviour
    {
        public enum Directon
        {
            Horizontal,
            Vertical
        }

        public enum Sorting
        {
            None,
            Alphabetic,
            Horizontal,
            Vertical,
            Custom
        }

        /// <summary>
        ///     高
        /// </summary>
        [SerializeField] private int _cellHeight;

        /// <summary>
        ///     宽
        /// </summary>
        [SerializeField] private int _cellWidth;

        /// <summary>
        ///     存储布局的所有子对象
        /// </summary>
        private List<Transform> _itemList;

        /// <summary>
        ///     每组间距高的和
        /// </summary>
        private int _totalGroupCellHeight;

        /// <summary>
        ///     每组间距宽的和
        /// </summary>
        private int _totalGroupCellWidth;

        /// <summary>
        ///     布局方向
        /// </summary>
        [SerializeField] public Directon directon;

        /// <summary>
        ///     组间间隔高度
        /// </summary>
        [SerializeField] public int GroupCellHeight;

        /// <summary>
        ///     组间间隔宽度
        /// </summary>
        [SerializeField] public int GroupCellWidth;

        /// <summary>
        ///     单组的数量
        /// </summary>
        public int GroupNumber;

        /// <summary>
        ///     隐藏时不处理
        /// </summary>
        [SerializeField] public bool HideWhenDisable = false;

        /// <summary>
        ///     每行的最大数量
        /// </summary>
        [SerializeField] public int maxPerLine;

        /// <summary>
        ///     立即刷新标识
        /// </summary>
        private bool mReposition;

        /// <summary>
        ///     添加监听
        /// </summary>
        public Action<Transform> OnAddItem;

        /// <summary>
        ///     删除监听
        /// </summary>
        public Action<Transform> OnDeleteItem;

        /// <summary>
        ///     锚点位置，即每行的起始位置
        /// </summary>
        public UIWidget.Pivot pivot = UIWidget.Pivot.TopLeft;

        /// <summary>
        ///     按组排序
        /// </summary>
        [SerializeField] public bool SortByGroup;

        /// <summary>
        ///     对齐方式（与NGUI的grid的主要区别在于这个部分）
        /// </summary>
        public Sorting sorting = Sorting.None;

        /// <summary>
        ///     总长度
        /// </summary>
        public int TotalLenth;

        public bool ResetPositionNow
        {
            get { return mReposition; }
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

        private void Awake()
        {
            _itemList = new List<Transform>();
            GetChildList();
        }

        [ContextMenu("Execute")]
        public void Reposition()
        {
            maxPerLine = maxPerLine == 0 ? maxPerLine = int.MaxValue : maxPerLine;
            if (_itemList == null)
            {
                _itemList = new List<Transform>();
            }
            _itemList = GetChildList();
            ResetPosition(_itemList);
        }

        /// <summary>
        ///     获得子对象
        /// </summary>
        /// <returns></returns>
        public List<Transform> GetChildList()
        {
            var list = new List<Transform>();
            var mTransForm = transform;
            for (var i = 0; i < mTransForm.childCount; i++)
            {
                var t = mTransForm.GetChild(i);
                if (!HideWhenDisable || (t && NGUITools.GetActive(t.gameObject)))
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
            return GetChildList().Last();
        }

        public Transform GetItem(int index)
        {
            var list = GetChildList();
            return index < list.Count ? list[index] : null;
        }

        public void AddItem(Transform trans, bool auto = false, float offsetScaleX = 1, float offsetScaleY = 1)
        {
            GameTools.AddChild(transform, trans, offsetScaleX, offsetScaleY);
            if (OnAddItem != null)
            {
                OnAddItem(trans);
            }
            ResetPositionNow = auto;
        }

        public void RemoveItem(Transform trans, bool auto = false)
        {
            if (OnDeleteItem != null)
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
            var x = 0;
            var y = 0;
            var maxX = 0;
            var maxY = 0;
            Transform t;
            Vector3 pos;
            var max = list.Count;
            var xIndex = 0;
            var yIndex = 0;
            for (var i = 0; i < max; i++)
            {
                t = list[i];
                pos = t.localPosition;
                GetLineIndex(ref xIndex, ref yIndex, i);
                pos = new Vector3(GetSnapX(xIndex), GetSnapY(yIndex), 0);
                t.localPosition = pos;
                t.name = string.Format("{0}_{1}", xIndex, yIndex);
            }
        }

        private void GetLineIndex(ref int indexX, ref int indexY, int index)
        {
            if (directon == Directon.Horizontal)
            {
                indexX = index%maxPerLine;
                indexY = index/maxPerLine;
            }
            else
            {
                indexX = index/maxPerLine;
                indexY = index%maxPerLine;
            }
        }

        private float GetSnapX(int index)
        {
            float x = 0;
            var isNotFirstOnLine = true;
            if (index == 0)
            {
                _totalGroupCellWidth = 0;
                isNotFirstOnLine = false;
            }
            if (SortByGroup)
            {
                var isNotFirstInGroup = index%GroupNumber != 0;
                if (isNotFirstInGroup && isNotFirstOnLine)
                {
                    _totalGroupCellWidth += GroupCellWidth;
                }
            }
            x = index*_cellWidth + _totalGroupCellWidth;
            switch (pivot)
            {
                case UIWidget.Pivot.TopRight:
                case UIWidget.Pivot.Right:
                case UIWidget.Pivot.BottomRight:
                    x *= -1;
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
                var isInGroup = index%GroupNumber == 0;
                if (isInGroup)
                {
                    _totalGroupCellHeight += GroupCellHeight;
                }
            }
            y = index*_cellHeight + _totalGroupCellHeight;
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
    }
}