using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Windows.TabPages;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    [Obsolete("Use Assets.Scripts.Hall.View.RuleWindows.YxRuleTabWindow")]
    public class RuleWindow : YxTabPageWindow {

        public string PrefixUpStateName;
        public string PrefixDownStateName;
        public Transform ContainerTs;
        [Tooltip("指定的gamek")]
        public string AppointGameKey;
        [Tooltip("在内容中显示规则名字")]
        public UILabel NameInView;
        private Transform _containerTs;

        protected override void OnAwake()
        {
            CreateTabels();
        }

        private void CreateTabels()
        {
            var dict = GameListModel.Instance.GameUnitModels;
            var gamelist = new List<GameUnitModel>();
            foreach (var keyValue in dict)
            {
                var model = keyValue.Value;
                if (model.RoomKind < 1) continue;
                gamelist.Add(model);
            }
            var count = gamelist.Count;
            TabDatas = new TabData[count];
            for (var i = 0; i < count; i++)
            {
                var gd = gamelist[i];
                var gk = gd.GameKey;
                var tdata = new TabData
                {
                    Name = gd.GameName,
                    UpStateName = string.Format("{0}{1}", PrefixUpStateName, gk),
                    DownStateName = string.Format("{0}{1}", PrefixDownStateName, gk),
                    Data = gk
                }; 
                TabDatas[i] = tdata;
            }
            if (TabDatas.Length < 1) return;
            var td = TabDatas[0];
            td.StarttingState = true;
            UpdateView(-1);
        }

        public void SetFirstTab(string gamekey)
        {
            var count = TabDatas.Length;
            for (var i = 0; i < count; i++)
            {
                var tdata = TabDatas[i];
                tdata.StarttingState = tdata.Data.Equals(gamekey);
            }
        }

        protected override void TabSelectAction(YxTabItem tableView)
        {
            base.TabSelectAction(tableView);
            TabItemClick(tableView);
        }
        /// <summary>
        /// 重置方法//todo:ngui scorllview 重置有问题，待查找出问题后弃用
        /// </summary>
        /// <param name="moveObj"></param>
        public void SpringToStart(GameObject moveObj)
        {
            SpringPanel.Begin(moveObj, Vector3.zero, 10000);
        }

        public override void OnTableClick(YxTabItem tableView)
        {
            if (!tableView.GetToggle().value) return;
            TabItemClick(tableView);
        }

        private void TabItemClick(YxTabItem tableView)
        {
            YxWindowUtils.CreateItemParent(ContainerTs, ref _containerTs, ContainerTs.parent);
            var tdata = tableView.GetData<TabData>();
            if (tdata == null) return;
            if (NameInView != null) NameInView.text = tdata.Name;
            var gk = tdata.Data.ToString();
            var prefix = App.Skin.GameInfo;
            var ruleName = string.Format("rulelist_{0}", gk);
            var namePrefix = string.Format("{0}_{1}", prefix, gk);
            var bundleName = string.Format("{0}/{1}", namePrefix, ruleName);
            var pfb = ResourceManager.LoadAsset(prefix, bundleName, ruleName);
            if (pfb == null) return;
            var content = YxWindowUtils.CreateGameObject(pfb, _containerTs);
            var widget = content.GetComponent<UIWidget>();
            if (widget)
            {
                widget.SetAnchor(_containerTs.gameObject, 0, 0, 0, 0);
            }
            UpOrder();
        }
    }
}
