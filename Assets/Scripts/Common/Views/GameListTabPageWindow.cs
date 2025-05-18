using System.Collections.Generic;
using Assets.Scripts.Common.Windows.TabPages;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using ResourceManager = YxFramwork.Manager.ResourceManager;

namespace Assets.Scripts.Common.Views
{
    /// <summary>
    /// 游戏规则视口
    /// </summary>
    public class GameListTabPageWindow : YxTabPageWindow
    {
        [Tooltip("标签未选中状态")]
        public string PrefixUpStateName;
        [Tooltip("标签选中状态")]
        public string PrefixDownStateName;
        public GameFilterType Filter;
        protected override void OnAwake()
        {
            base.OnAwake();
            CreateTabels();
        }

        private void CreateTabels()
        {
            if (PerfabTableItem == null) return;
            var dict = GameListModel.Instance.GameUnitModels;
            var gamelist = new List<GameUnitModel>();
            var ruleName = App.Skin.Rule;
            foreach (var keyValue in dict)
            {
                var model = keyValue.Value;
                switch (Filter)
                {
                    case GameFilterType.None:
                        gamelist.Add(model);
                        break;
                    case GameFilterType.Rule:
                        if (ResourceManager.HasRes(ruleName, string.Format("{0}_{1}.ab", ruleName, model.GameKey)))
                        {
                            gamelist.Add(model);
                        }
                        break;
                    default:
                        if ((int) model.GameState == (int) Filter)
                        {
                            gamelist.Add(model);
                        }
                        break;
                }
            }
            var count = gamelist.Count;
            TabDatas = new TabData[count];
            for (var i = 0; i < count; i++)
            {
                var glModle = gamelist[i];
                var gk = glModle.GameKey;
                var tdata = new TabData
                {
                    Name = glModle.GameName,
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

        protected override void OnChageTab(YxView view, TabData tabData)
        {
            if (view == null) { return; }
            view.ShowWithData(tabData.Data);
        }

        public enum GameFilterType
        {
            None,
            Developing,
            CreateMode,
            Match,
            Desk,
            Rule
        }
    }
}
