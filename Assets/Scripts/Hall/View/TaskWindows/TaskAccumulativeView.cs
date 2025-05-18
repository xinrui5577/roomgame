using System;
using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.TaskWindows
{
    /// <summary>
    /// 满局任务（累计任务）
    /// </summary>
    public class TaskAccumulativeView : TaskBasseView
    {
        /// <summary>
        /// item 预设
        /// </summary>
        public GameObject PrefabItem;
        /// <summary>
        /// 显示的Grid
        /// </summary>
        public UIGrid ShowGrid;
        /// <summary>
        /// 显示区域
        /// </summary>
        public GameObject ShowPanel;
        /// <summary>
        /// 没有记录的提示
        /// </summary>
        public GameObject NullNotice;
        /// <summary>
        /// 任务列表
        /// </summary>
        private string _keytask = "task";
        /// <summary>
        /// 完成显示标识0:完成后不显示1.完成后显示
        /// </summary>
        private string _keyFinihsedShow = "finishShow";
        /// <summary>
        /// 完成显示标识
        /// </summary>
        private bool _finishShow;
        /// <summary>
        /// 完成显示标识
        /// </summary>
        private const int _finishShowTag = 1;

        /// <summary>
        /// 领取奖励接口
        /// </summary>
        private const string KeyTaskReward = "taskReward";
        /// <summary>
        /// 玩家信息接口
        /// </summary>
        private const string KeyUserData = "userData";
        /// <summary>
        ///背包接口
        /// </summary>
        private const string KeyGetProps = "getProps";
        private System.Collections.Generic.List<string> _finishList;

        protected override void OnEnable()
        {
            base.OnEnable();
            Facade.Instance<TwManager>().SendAction
                (
                "taskList",
                new Dictionary<string, object>(),
                    data =>
                    {
                        if (ShowGrid)
                        {
                            SpringPanel.Begin(ShowPanel, Vector3.zero, 10000);
                            while (ShowGrid.transform.childCount > 0)
                            {
                                DestroyImmediate(ShowGrid.transform.GetChild(0).gameObject);
                            }
                            _finishList = new List<string>();
                            Dictionary<string, object> param = (Dictionary<string, object>)data;
                            _finishShow = (int.Parse(param[_keyFinihsedShow].ToString())).Equals(_finishShowTag);
                            var taskObj = param[_keytask];
                            if (taskObj == null|| taskObj is Array||taskObj is IList)
                            {
                                if (NullNotice)
                                {
                                    NullNotice.SetActive(true);
                                }
                                return;
                            }
                            Dictionary<string, object> tasks = (Dictionary<string, object>)param[_keytask];
                            if (NullNotice)
                            {
                                NullNotice.SetActive(!(tasks.Count > 0));
                            }
                            foreach (var taskItem in tasks)
                            {
                                GameObject item = NGUITools.AddChild(ShowGrid.gameObject, PrefabItem);
                                if (item)
                                {
                                    AccumulativeItem aItem = item.GetComponent<AccumulativeItem>();
                                    if (aItem.Init(taskItem))
                                    {
                                        _finishList.Add(aItem.name);
                                    }
                                }
                            }
                            ShowGrid.repositionNow = true;
                        }
                    });
        }
        public void OnClickGetAll()
        {
            if (_finishList.Count == 0)
            {
                YxMessageBox.Show("目前没有任务奖励可以领取");
                return;
            }
            OnClickGetRewardBtn(_finishList);
        }

        public static void OnClickGetRewardBtn(System.Collections.Generic.List<string> tasks)
        {
            Dictionary<string, object> param = new Dictionary<string, object>();
            var actions = new[] {KeyTaskReward,KeyUserData,KeyGetProps };
            param["task"] = tasks;
            Facade.Instance<TwManager>().SendActions
            (
           actions,
           param,
           OnGetReward
           );
        }

        private static void OnGetReward(object data)
        {
            Dictionary<string, object> getData = (Dictionary<string, object>)data;
            YxWindow getWindow = YxWindowManager.OpenWindow("GetRewardsWindow", true);
            getWindow.UpdateView(getData[KeyTaskReward]);
            UserInfoModel.Instance.Convert(getData[KeyUserData]);
            UserInfoModel.Instance.ConvertBackPack(getData[KeyGetProps]);
            Facade.EventCenter.DispatchEvent<string,object>("HallWindow_hallMenuChange");
        }
    }
}
