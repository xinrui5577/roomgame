using System.Collections.Generic;
using Assets.Scripts.Common.components;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Windows.GpsViews
{
    public class GpsMoreItemView : YxView
    {
        /// <summary>
        /// Item预制体
        /// </summary>
        public GpsItemView ItemViewPrefab;
        /// <summary>
        /// Grid预制体
        /// </summary>
        public YxNguiGrid GridPrefab;
        /// <summary>
        /// 小组标题
        /// </summary>
        public UILabel GroupTitleLabel;
        /// <summary>
        /// 标题背景
        /// </summary>
        public UISprite TitleBg;
        /// <summary>
        /// 标题背景前缀
        /// </summary>
        public string TitleBgPrefix = "titlebg_";
        /// <summary>
        /// 
        /// </summary>
        public Vector4 ItemPadding = new Vector4(10,20,10,10);
        private UIWidget _widget;
        private YxNguiGrid _grid;

        protected override void OnAwake()
        {
            _widget = GetComponent<UIWidget>();
        }

        protected override void OnFreshView()
        {
            YxWindowUtils.CreateMonoParent(GridPrefab, ref _grid);
            var itemData = GetData<GpsMoreItemData>();
            if (itemData == null) return;
            var gridTs = _grid.transform;
            var localPosition = gridTs.localPosition;
            localPosition.y = -ItemPadding.y;
            gridTs.localPosition = localPosition;
            SetTitle(itemData.Title);
            SetTitleBg(itemData.TitleType);
            var list = itemData.List;
            var count = list.Count;
            for (var i = 0; i < count; i++)
            {
                var info = list[i];
                if(info==null)continue;
                var item = YxWindowUtils.CreateItem(ItemViewPrefab, gridTs);
                item.UpdateView(info);
            }
            _grid.UpdateGrid();
            MackUp();
            CallBack(itemData.Id);
        }

        private void SetTitleBg(string itemDataType)
        {
            if (TitleBg == null) return;
            TitleBg.spriteName = string.Format("{0}{1}", TitleBgPrefix, itemDataType);
        }

        private void MackUp()
        {
            if (_widget==null) return;
            _widget.width = Width;
            _widget.height = Height;
        }

        public void SetTitle(string title)
        {
            if (GroupTitleLabel == null)return;
            GroupTitleLabel.text = string.IsNullOrEmpty(title) ? "" : title;
        }
         
        /// <summary>
        /// 
        /// </summary>
        public override int Width
        {
            get
            {
                if (_grid == null) return (int)(ItemPadding.x + ItemPadding.w);
                var maxPerLine = _grid.maxPerLine;
                if (_grid.arrangement == UIGrid.Arrangement.Horizontal)
                {
                    return (int)(_grid.cellWidth * maxPerLine + ItemPadding.x + ItemPadding.w);
                }
                var c = Mathf.CeilToInt((float)_grid.GetChildList().Count / maxPerLine);
                return (int)(_grid.cellWidth * c + ItemPadding.x + ItemPadding.w);
            }
        }

        public override int Height
        {
            get
            {
                if (_grid == null) return (int)(ItemPadding.y + ItemPadding.z);
                var maxPerLine = _grid.maxPerLine;
                if (_grid.arrangement == UIGrid.Arrangement.Vertical)
                {
                    return (int)(_grid.cellHeight * maxPerLine + ItemPadding.y + ItemPadding.z);
                }
                var c = Mathf.CeilToInt((float)_grid.GetChildList().Count / maxPerLine);
                return (int)(_grid.cellHeight * c + ItemPadding.y + ItemPadding.w);
            }
        }
    }

    public class GpsMoreItemData
    {
        public int Id = 0;
        public string Title;
        /// <summary>
        /// 0：正常   1：警告距离
        /// </summary>
        public string TitleType = "";
        public readonly List<YxBaseUserInfo> List = new List<YxBaseUserInfo>();
        public GpsMoreItemData()
        {
        }
        public GpsMoreItemData(YxBaseUserInfo info)
        {
            List.Add(info);
        }
    }
}
