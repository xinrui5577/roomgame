using System;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    [Obsolete("Use Assets.Scripts.Hall.View.ShopWindows.YxShopTableItemView")]
    public class ShopTableItemView : YxView
    {
        public YxBaseLabelAdapter LabelName;
        public YxBaseLabelAdapter LabelUpName;
        private string _tableName;

        protected override void OnStart()
        {
            InitStateTotal = 2;
        }

        public void SetName(string tName)
        {
            _tableName = tName;
            FreshView();
        }

        protected override void OnFreshView()
        {
            LabelName.Text(_tableName);
            if(LabelUpName!=null)LabelUpName.Text(_tableName);
        }
    }
}
