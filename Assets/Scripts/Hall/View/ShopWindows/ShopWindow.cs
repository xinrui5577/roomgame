using System;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    /// <summary>
    /// 商店窗口
    /// </summary>
    [Obsolete("Use Assets.Scripts.Hall.View.ShopWindows.YxShopWindow")]
    public class ShopWindow:YxNguiWindow
    {
        [Tooltip("item预制")]
        public ShopItemView PerfabGoodsItem;
        [Tooltip("标签预制体")]
        public ShopTableItemView PerfabTabelsItem;
        [Tooltip("标签的grid")]
        public UIGrid TableGrid;
        [Tooltip("item的grid")]
        public UIGrid GoodsGrid;    
        [Tooltip("滚动条")]
        public UIScrollView ScrollView;
        private UIGrid _curItemParent;
        [Tooltip("界面刷新处理")]
        public List<EventDelegate> OnWindowFresh;

        public ShopTableItemView[] SpecialTabs;
        protected override void OnAwake()
        {
            CurTwManager.SendAction("goodsItem", new Dictionary<string, object>(), OnData, false);
        }

        private void OnData(object msg)
        {
            ShopModel.Instance.Convert(msg);
            FreshView();
        }

        protected override void OnFreshView()
        {
            //创建tabel；
            var shopModel = ShopModel.Instance;
            var typeDises = shopModel.GoodsTyps;
            foreach (var tab in SpecialTabs)
            {
                if (tab == null) continue;
                var key = tab.name;
                var has = typeDises.ContainsKey(key);
                typeDises[key] = null;
                tab.SetActive(has);
            }
            foreach (var typeDise in typeDises)
            {
                var index = typeDise.Key;
                var typeName = typeDise.Value; 
                if (string.IsNullOrEmpty(index)|| string.IsNullOrEmpty(typeName))
                {
                    continue;
                }
                CreateTabel(index, typeName); 
            }
            TableGrid.repositionNow = true;
            TableGrid.Reposition();
            if (_selectedToggle != null)
            {
                OnChangeToggle(_selectedToggle);
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnWindowFresh.WaitExcuteCalls());
            }
        }

        private void CreateTabel(string index, string typeName)
        {
            var tabel = Instantiate(PerfabTabelsItem);
            var tabelTs = tabel.transform;
            tabelTs.parent = TableGrid.transform;
            tabelTs.gameObject.SetActive(true);
            tabelTs.localScale = Vector3.one;
            tabel.name = index;
            tabel.SetName(typeName);
        }

        private UIToggle _selectedToggle; //todo 临时处理
        public void OnChangeToggle(UIToggle toggle)
        {
            if (!toggle.value) return;
            _selectedToggle = toggle;
            if (ScrollView != null)
            {
                ScrollView.ResetPosition();
            }
            CreateItemParent();
            var goodses = ShopModel.Instance.Goodses;
            var index = toggle.name;
            if (!goodses.ContainsKey(index)) return;
            var mUnit = goodses[index];
            foreach (var shopModelUnit in mUnit)
            {
                CreateGoodsItem(shopModelUnit);
            }
            _curItemParent.repositionNow = true;
            _curItemParent.Reposition(); 
        }

        private void CreateGoodsItem(ShopModelUnit data)
        { 
            var goodsItem = Instantiate(PerfabGoodsItem);
            var tabelTs = goodsItem.transform;
            tabelTs.parent = _curItemParent.transform;
            tabelTs.gameObject.SetActive(true);
            tabelTs.localScale = Vector3.one;
            goodsItem.name = data.Id;
            goodsItem.UpdateView(data);
        }

        public void OnOpenPay()
        {
            var cfg = App.Config as SysConfig;
            if (cfg == null) return;
            var info = LoginInfo.Instance;
            Application.OpenURL(cfg.GetRecharge(info.user_id,info.token));
        }
         
        /// <summary>
        /// 购买元宝
        /// </summary>
        /// <param name="obj"></param>
        public void OnBuyGold(GameObject obj)
        {
            OnOpenPay();
        }


        public override void OnDestroy()
        {
            base.OnDestroy();
            ShopModel.Instance.Gc();
        }

        private void CreateItemParent()
        {
            if (_curItemParent != null)
            {
                Destroy(_curItemParent.gameObject);
            }
            var perfabTs = GoodsGrid.transform;
            _curItemParent = Instantiate(GoodsGrid);
            var ts = _curItemParent.transform;
            ts.parent = perfabTs.parent;
            ts.gameObject.SetActive(true);
            ts.localPosition = perfabTs.localPosition;
            ts.localScale = perfabTs.localScale;
            ts.localRotation = perfabTs.localRotation;
        }

        [SerializeField]
        protected string ConstUrl = "";
        /// <summary>
        /// 打开固定的url
        /// </summary>
        public void OpenConstUrl()
        {
            Application.OpenURL(ConstUrl);
        }
    }
}
