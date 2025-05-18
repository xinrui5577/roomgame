using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.Windows.PromoteWindows
{
    public class PromoteExtractRecordWindow : YxWindow
    {
        public string ActionName = "getSpreadAwardHistory";
        public PromoteExtractRecordItem PrefabItem;
        public YxBaseGridAdapter PrefabGrid;
        public GameObject HasDataContainer;
        public GameObject NoDataContainer;

        private YxBaseGridAdapter _grid;


        protected override void OnAwake()
        {
            base.OnAwake();
            CurTwManager.SendAction(ActionName,null,UpdateView);
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dict = GetData<Dictionary<string, object>>();
            if (dict == null)
            {
                OnChangeState(false);
                return;
            }
            if (!dict.ContainsKey("data"))
            {
                OnChangeState(false);
                return;
            }
            var list = dict["data"] as List<object>;
            if (list == null || list.Count <= 0)
            {
                OnChangeState(false);
                return;
            }
            OnChangeState(true);
            var count = list.Count;
            YxWindowUtils.CreateMonoParent(PrefabGrid,ref _grid,PrefabGrid.transform.parent);
            var itemParent = _grid.transform;
            for (var i = 0; i < count; i++)
            {
                var itemData = list[i];
                var info = new PromoteExtractRecordInfo();
                info.Parse(itemData);
                var item = CreateItem(itemParent);
                item.UpdateView(info);
            }
            _grid.Reposition();
        }

        protected void OnChangeState(bool hasData)
        {
            if (HasDataContainer)
            {
                HasDataContainer.SetActive(hasData);
            }
            if (NoDataContainer)
            {
                NoDataContainer.SetActive(!hasData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemParent"></param>
        private PromoteExtractRecordItem CreateItem(Transform itemParent)
        {
            return YxWindowUtils.CreateItem(PrefabItem, itemParent);
        }

        public override YxEUIType UIType
        {
            get { return GetComponent<YxBaseAdapter>().UIType; }
        }
    }
}
