using System.Collections.Generic;
using Assets.Scripts.Common.Components;
using Assets.Scripts.Hall.Models;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Controller;
using YxFramwork.Framework;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.ListViews
{
    /// <summary>
    /// 
    /// </summary>
    public class DeskListItem : YxNguiListItem
    {
        [Tooltip("桌子标题")]
        public UILabel DeskNameLabel;
        [Tooltip("最小限制")]
        public UILabel MinCoinLabel;
        [Tooltip("最大限制")]
        public UILabel MaxCoinLabel;
        [Tooltip("RoomListItemView的容器")]
        public Transform ViewContainer;
        [Tooltip("桌面名称格式")]
        public string NameFormat = "{0} {1:D2}";

        protected override bool CheckData(object newObj, object oldObj)
        {
            if (newObj == null) return true;
            if (oldObj == null) return true;
            return !newObj.Equals(oldObj);
        }

        private DeskItemData _deskItemData;
        protected override void FreshData()
        {
            var dict = GetData<Dictionary<string, object>>();
            if (dict == null) return;
            _deskItemData = new DeskItemData();
            _deskItemData.Parse(dict);
            FreshInfo(_deskItemData);
            CreateStyleView(_deskItemData);//itemType 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deskItemData"></param>
        protected virtual void FreshInfo(DeskItemData deskItemData)
        {
            if (DeskNameLabel!=null)
            {
                DeskNameLabel.text = string.Format(NameFormat, deskItemData.RoomName, deskItemData.Id);
            }
            if (MinCoinLabel != null)
            {
                MinCoinLabel.text = deskItemData.MinCoin.ToString();
            }
            if (MaxCoinLabel != null)
            {
                var max = deskItemData.MaxCoin;
                MaxCoinLabel.text = max > 0 ? max.ToString():"∞";
            }
        }

        private GameObject _view;
        /// <summary>
        //  创建视图
        /// </summary>
        /// <param name="deskItemData"></param>
        private void CreateStyleView(DeskItemData deskItemData)
        {
            var gamekey = App.LoadingGameKey;
            //有子背景
            if (_view != null) Destroy(_view);
            var prefix = App.Skin.GameInfo;
            var deskItemName = string.Format("deskitem_{0}",  gamekey);//deskitem_gamekey
            var namePrefix = string.Format("{0}_{1}", prefix, gamekey);//gameinfo_gamekey
            var bundleName = string.Format("{0}/{1}", namePrefix, deskItemName);//gameinfo_gamekey/deskitem_gamekey
            _view = ResourceManager.LoadAsset(prefix, bundleName, deskItemName);
            if (_view == null) return;
            _view = Instantiate(_view);
            _view.name = string.Format("{0}({1})", deskItemName, deskItemData.Id);
            var ts = _view.transform;
            ts.parent = ViewContainer;
            ts.localPosition = Vector3.zero;
            ts.localScale = Vector3.one;
            ts.localRotation = Quaternion.identity;
            var styleView = _view.GetComponent<YxView>();
            if (styleView != null)
            {
                styleView.MainYxView = this;
                styleView.Init(deskItemData);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public void OnDeskClick()
        {
            if (_deskItemData == null) return;
            var roomId = _deskItemData.Id;
            RoomListController.Instance.JoinFindRoom(roomId,App.LoadingGameKey);
        }
    }
}
