using Assets.Scripts.Common;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Common.components;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Controller;

namespace Assets.Scripts.Hall.View.BackPackWindows
{
    /// <summary>
    /// 背包
    /// </summary>
    public class BackPackWindow : YxNguiWindow
    {
        public UIGrid ItemGridPerfab;
        public UILabel CoinLabel;
        public UILabel CashLabel;
        public BackPackItemView ItemPerfab;

        private UIGrid _itemGrid;

        protected override void OnAwake()
        {
            base.OnAwake();
            UserController.Instance.GetBackPack(UpdateView);
        }

        protected override void OnFreshView()
        {
            UpUserInfo();
            base.OnFreshView();
            var back = UserInfoModel.Instance.BackPack;
            var keys = back.GetKeys();
            YxWindowUtils.CreateItemGrid(ItemGridPerfab, ref _itemGrid); 
            var ts = _itemGrid.transform;
            foreach (var key in keys)
            {
                var count = back.GetItem(key);
                if(count<1)continue;
                var item = YxWindowUtils.CreateItem(ItemPerfab, ts);
                item.UpdateView();
            }
            if (_itemGrid is YxNguiGrid)
            {
                var iGrid = _itemGrid as YxNguiGrid;
                iGrid.Makeup();
            }
            _itemGrid.repositionNow = true;
            _itemGrid.Reposition();
        }

        private void UpUserInfo()
        {
            var user = UserInfoModel.Instance.UserInfo;
            if (CoinLabel != null)
            {
                CoinLabel.text = user.CoinA.ToString();
            }
            if (CashLabel != null)
            {
                CashLabel.text = user.CashA.ToString();
            }

        }

        public void OnOpenPay()
        {
            var cfg = App.Config as SysConfig;
            if (cfg == null) return;
            var info = LoginInfo.Instance;
            Application.OpenURL(cfg.GetRecharge(info.user_id, info.token));
        }
    }
}
