/** 
 *文件名称:     MatchWindow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-01-18 
 *描述:         比赛界面，默认以比赛开始时间倒序排序。
 *              支持按比赛状态排序
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Hall.View.PageListWindow;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.MatchWindow
{
    public class MatchWindow : YxPageListWindow
    {
        #region UI Param
        [Tooltip("刷新事件")]
        public List<EventDelegate> FreshEvent=new List<EventDelegate>();
        #endregion
        #region Data Param
        [Tooltip("按比赛时间排序（默认排序方式）")]
        public string SortByTimesName = "按比赛开始时间排序";
        [Tooltip("按照比赛状态排序数据更新提示内容")]
        public string MatchRefreshNotice = "比赛数据已更新";
        #endregion 
        #region Local Data
        /// <summary>
        /// 当前的排序方式
        /// </summary>
        private string _curShortName;
        /// <summary>
        /// 用于排序刷新的
        /// </summary>
        private List<YxData> _cacheItems=new List<YxData>();
  
        #endregion
        #region Life Cycle
        protected override void OnAwake()
        {
            _curShortName = SortByTimesName;
            base.OnAwake();
        }

        protected override void OnActionCallBackDic()
        {
            base.OnActionCallBackDic();
            if (!_curShortName.Equals(SortByTimesName))
            {
                SortByState();
                RefreshView(_cacheItems);
            }
        }

        public override void OnDestroy()
        {
            Reset();
            base.OnDestroy();
        }

        protected override Type GetItemType()
        {
            return typeof(MatchItemData);
        }

        #endregion
        #region Function
        public void OnSelectChange(string value)
        {
            if (!_curShortName.Equals(value))
            {
                SortByType(value);
            }
        }

        private void SortByType(string type)
        {
            _curShortName = type;
            if (_curShortName.Equals(SortByTimesName))
            {
                SortByTimes();
            }
            else
            {
                SortByState();
            }
            RefreshView(_cacheItems);
            if (FreshEvent.Count > 0)
            {
                foreach (var Event in FreshEvent)
                {
                    if (Event != null)
                    {
                        Event.Execute();
                    }
                }
            }
        }
        private void SortByTimes()
        {
            _cacheItems = Items.ToList();
        }

        private void SortByState()
        {
            _cacheItems = Items.ToList();
            _cacheItems.Sort((left, right) =>
            {
                int count = Enum.GetValues(typeof (EnumMatchState)).Length;//偷个懒，有规律，猜猜看。           
                int leftState = ((int) ((MatchItemData)left).State + 1)%count;
                int rightState = ((int)((MatchItemData)right).State + 1) % count;
                if (leftState != rightState)
                {
                    if (leftState>rightState)
                    {
                        return -1;
                    }
                    else
                    {
                        return 1;
                    }                  
                }
                else
                {
                    return 0;
                }
            });
        }

        private void OnMatchStateChange(object objData)
        {
            if (objData is object[])
            {
                var arr = objData as object[];
                MatchItem item=arr[0] as MatchItem;
                MatchItemData data=arr[1] as MatchItemData;
                if (Items != null && Items.Count > 0)
                {
                    if (!_curShortName.Equals(SortByTimesName))
                    {
                        YxMessageBox.Show(
                             MatchRefreshNotice,
                                         null,
                          (window, btnname) =>
                          {
                              switch (btnname)
                              {
                                  case YxMessageBox.BtnMiddle:
                                      SortByType(_curShortName);
                                      break;
                              }
                          });
                    }
                    else
                    {
                        if (data != null)
                        {
                            data.NeedCloseWhenLoad = false;
                            if (item != null) item.UpdateView(data);
                        }
                    }
                }
                else
                {
                    YxDebug.LogError("参数不正确");
                }
            }
 
        }
        #endregion
    }
}
