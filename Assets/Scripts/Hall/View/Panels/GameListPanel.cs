using System.Collections;
using Assets.Scripts.Hall.Controller;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Common.Utils;
using YxFramwork.Controller;
using YxFramwork.Manager;
using YxFramwork.View;
using YxListView = YxFramwork.View.YxListViews.YxListView;

namespace Assets.Scripts.Hall.View.Panels
{
    /// <summary>
    /// 游戏列表
    /// </summary>
    public class GameListPanel : YxBasePanel
    {
        /// <summary>
        /// 游戏列表面板
        /// </summary>
        public YxListView DefaultListView;

        protected YxListView CurListView;

        protected override void OnAwake()
        {
            base.OnAwake();
            DefaultListView.SetActive(false); 
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            CurListView = CreateListView();
            OnShowAllListItem();
            FreshCurListView();
        }

        private int _lastGameGroup = -1;
        /// <summary>
        /// 创建列表视图
        /// </summary>
        private YxListView CreateListView()
        {
            var group = HallController.Instance.CurGameGroup;
            if (CurListView != null)
            {
                if (group == _lastGameGroup)
                {
                    return CurListView;
                }
                if (CurListView == DefaultListView)
                {
                    DefaultListView.SetActive(false);
                }
                else
                {
                    Destroy(CurListView.gameObject);
                }
            }
            if (group < 1) { return DefaultListView;}
            var bundleName = string.Format("GameListView_{0}", group);
            var pre = ResourceManager.LoadAsset(bundleName);
            if (pre == null) { return DefaultListView;}
            var go = Instantiate(pre);
            var listView = go.GetComponent<YxListView>();
            return listView == null ? DefaultListView : listView;
        }

        private int _curGroup = -1;
        private void OnShowAllListItem()
        {
            if (CurListView == null) return;
            var gm = GameListModel.Instance;
            var group = gm.CurGroup;
            if (group == _curGroup) { return;}
            _curGroup = group;
            var gGroup = gm.GetGroup(group);
            IList list = null;
            if (gGroup != null)
            {
                list = gGroup.GameListModels;
            }
            CurListView.StartDataIndex = Util.GetInt("GameList_StartDataIndex");//GameListController.Instance.CurStartDataIndex;
            var dataFormat = GameListController.Instance.DataFormat;
            if (dataFormat != null && dataFormat.Length > 0) CurListView.DataFormat = dataFormat;
            CurListView.UpdateView(list); 
            Util.SetInt("GameList_StartDataIndex", CurListView.StartDataIndex);
        }

        private void FreshCurListView()
        {
            if (CurListView == null) return;
            CurListView.SetActive(true);
        }

    }
}
