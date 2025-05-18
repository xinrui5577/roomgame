using System.Collections.Generic;
using Assets.Scripts.Common.Models;
using com.yxixia.utile.Utiles;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    /// <summary>
    /// 商品视图，商店，背包都可以重复利用
    /// </summary>
    public class YxGoodsView : YxView
    {
        /// <summary>
        /// 
        /// </summary>
        public string ActionName = "store.getGoods";
        public string ActionType;
        /// <summary>
        /// 商品item预制体
        /// </summary>
        public YxView ItemViewPerfab;
        /// <summary>
        /// 
        /// </summary>
        public YxBaseGridAdapter GridPerfab;
        /// <summary>
        /// 
        /// </summary>
        private YxBaseGridAdapter _goodsgrid;
        protected override void OnAwake()
        {
            base.OnAwake();
            CheckIsStart = true;
            InitStateTotal = 1;
            SendAction(ActionType);
        }

        protected override bool CheckData(object newObj, object oldObj)
        { 
            if (newObj == null)
            {
                return oldObj != null;
            }
            return !newObj.Equals(oldObj);
        }

        protected override void OnFreshView()
        {
            var goodsDict = GetData<Dictionary<string,object>>();
            YxWindowUtils.CreateMonoParent(GridPerfab,ref _goodsgrid,null, "goodsContainer");
            if (goodsDict == null)
            { 
                return;
            }
            if (!goodsDict.ContainsKey("goods")) { return;}
            var type = goodsDict.ContainsKey("type") ? goodsDict["type"].ToString() : "";
            var goods = goodsDict["goods"] as Dictionary<string,object>;
            if (goods == null) { return ; }
            var gridTs = _goodsgrid.transform;
            foreach (var keyValue in goods)
            {
                var item = YxWindowUtils.CreateItem(ItemViewPerfab, gridTs);
                item.Id = type; 
                item.UpdateView(keyValue.Value);
            }
            _goodsgrid.Reposition();
        }

        public void OnChageTab(string type)
        {
            SendAction(type);
        }

        protected virtual void SendAction(string type)
        {
            if (string.IsNullOrEmpty(type)) { return;}
            var parm = new Dictionary<string, object>();
            parm["type"] = type;
            CurTwManager.SendAction(ActionName, parm, UpdateView);
        }
    }
}
