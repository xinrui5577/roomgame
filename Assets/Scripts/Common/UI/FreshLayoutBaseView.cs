
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 回调时会重新计算布局
    /// </summary>
    public abstract class FreshLayoutBaseView : YxView
    {
        private readonly List<YxView> _bufferViewList = new List<YxView>();//RuleGroupView
        private int _viewCount;
        private bool _needFresh;
        private int _viewFreshMaxCount;
        private int _viewFreshCount;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            _needFresh = false;
            _viewFreshCount = 0;
            _viewCount = 0;
            InitViews();
            FreshLayout();
        }

        protected int BufferViewCount
        {
            get { return _bufferViewList.Count; }
        }

        /// <summary>
        /// 获取缓存view
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected YxView GetBufferView(int index)
        {
            return _bufferViewList[index];
        }

        protected void ClearBufferView()
        {
            _bufferViewList.Clear();
        }

        /// <summary>
        /// 初始化Views
        /// </summary>
        protected abstract void InitViews();
        protected void FreshLayout()
        {
            _viewFreshMaxCount = (1 << _viewCount) - 1;
            _needFresh = true;
        }

        /// <summary>
        /// 创建新的组
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="list"></param>
        /// <param name="pts"></param>
        /// <returns></returns>
        protected int CreateNewView<T>(int startIndex, int count, IList<T> list,Transform pts)
        {
            var i = startIndex;
            for (; i < count; i++)
            {
                var data = list[i];
                var view = CreateView(data,pts);
                if (view == null)
                {
                    continue;
                }
                view.IdCode = _viewCount;
                view.gameObject.SetActive(true);
                view.UpdateViewWithCallBack(data, FreshLayout);
                _bufferViewList.Add(view);
                _viewCount++;
            }
            return i;
        }


        /// <summary>
        /// 获取缓存View
        /// </summary>
        /// <returns></returns>
        protected int FreshBufferView<T>(int startIndex, int count, IList<T> list)
        {
            var i = startIndex;
            for (; i < count; i++)
            {
                var groupData = list[i];
                var itemView = _bufferViewList[i];
                itemView.gameObject.SetActive(true);
                itemView.IdCode = _viewCount;
                itemView.UpdateViewWithCallBack(groupData, FreshLayout);
                _viewCount++;
            }
            return i;
        }


        /// <summary>
        /// 隐藏多余的
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        protected int HideOdd(int startIndex, int count)
        {
            if (count <= 0) return startIndex;
            var i = startIndex;
            for (; i < count; i++)
            {
                var itemView = _bufferViewList[i];
                itemView.Hide();
            }
            return i;
        }

        private void FreshLayout(object obj)
        {
            var index = (int)obj;
            _viewFreshCount |= 1 << index;
            if (!_needFresh || _viewFreshCount != _viewFreshMaxCount) return;
            _needFresh = false;
            _viewFreshCount = 0;
            OnFreshLayout();
        }

        /// <summary>
        /// 刷新布局
        /// </summary>
        protected abstract void OnFreshLayout();

        /// <summary>
        /// 创建View
        /// </summary>
        /// <returns></returns>
        protected abstract YxView CreateView<T>(T data,Transform pts, Vector3 pos = default(Vector3));
    }
}
