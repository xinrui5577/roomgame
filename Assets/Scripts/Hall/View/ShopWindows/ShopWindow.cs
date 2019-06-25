using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Common;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Hall.View.ShopWindows
{
    /// <summary>
    /// 商店窗口 todo 继承 yxtabpagewindow
    /// </summary>
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
        [Tooltip("自己金币")]
        public UILabel Coin;
        [Tooltip("金币adapter")]
        public NguiLabelAdapter CoinAdapter;
        [Tooltip("自己的元宝")]
        public UILabel Gold;
        [Tooltip("自己的奖券")]
        public UILabel Coupon;
        [Tooltip("自己的房卡")]
        public UILabel RoomCard;
        [Tooltip("滚动条")]
        public UIScrollView ScrollView;
        private UIGrid _curItemParent;

        protected override void OnAwake()
        {
            InitStateTotal = 2;
            YxWindowManager.ShowWaitFor();
            Facade.Instance<TwManger>().SendAction("goodsItem", new Dictionary<string, object>(), OnData, false);
        }

        private void OnData(object msg)
        {
            ShopModel.Instance.Convert(msg);
            FreshView();
        }

        protected override void OnStart()
        {
            OnBindDate();
        } 

        protected override void OnFreshView()
        {
            //创建tabel；
            var shopModel = ShopModel.Instance;
            var goodses = shopModel.Goodses;
            var types = goodses.Keys;
            var typeDises = shopModel.GoodsTyps;
            var count = types.Count;
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
            goodsItem.OnDataUpdate = OnFreshView;
        }

        public void OnOpenPay()
        {
            var cfg = App.Config as SysConfig;
            if (cfg == null) return;
            var info = LoginInfo.Instance;
            Application.OpenURL(cfg.GetRecharge(info.user_id,info.token));
        }

        private void OnFreshView(object msg)
        {
            OnBindDate();
        }

        /// <summary>
        /// 购买元宝
        /// </summary>
        /// <param name="obj"></param>
        public void OnBuyGold(GameObject obj)
        {
            OnOpenPay();
        }

        protected void OnBindDate()
        {
            var userInfoModel = UserInfoModel.Instance;
            var userinfo = userInfoModel.UserInfo;
            if (Coin != null) Coin.text = userinfo.CoinA.ToString(CultureInfo.InvariantCulture);
            YxTools.TrySetComponentValue(CoinAdapter, userinfo.CoinA, "1");
            if (Gold != null) Gold.text = userinfo.CashA.ToString(CultureInfo.InvariantCulture);
            if (Coupon != null) Coupon.text = userinfo.CouponA.ToString(CultureInfo.InvariantCulture);
            var backPack = userInfoModel.BackPack; 
            if (RoomCard != null) RoomCard.text = backPack.GetItem("item2_q").ToString(CultureInfo.InvariantCulture);
        } 

        protected override void OnDestroy()
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
