using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.TabPages;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
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

        public override void OnTableClick(YxTabItem tableView)
        {
            if (!tableView.GetToggle().value) return;
            
            YxWindowUtils.CreateItemParent(ContainerTs,ref _containerTs,ContainerTs.parent);
            var tdata = tableView.GetData<TabData>();
            if (tdata == null) return;
            if (NameInView != null) NameInView.text = tdata.Name;
            var gk = tdata.Data.ToString();
            var rulePath = App.RuleListPath;
            var ruleName = string.Format("{0}_{1}",rulePath,gk);
            var pfb = ResourceManager.LoadAsset(rulePath, ruleName, ruleName);
            if (pfb == null) return;
            var content = YxWindowUtils.CreateGameObject(pfb, _containerTs);
            var widget = content.GetComponent<UIWidget>();
            widget.SetAnchor(_containerTs.gameObject,0,0,0,0);
            UpOrder(); 
        }
    }
}
