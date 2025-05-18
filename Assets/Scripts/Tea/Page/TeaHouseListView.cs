using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Manager;

/*===================================================
 *文件名称:     TeaHouseListView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-11-23
 *描述:        	茶馆房间列表
 *              所有茶馆列表与馆主茶馆
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Tea.Page
{
    public class TeaHouseListView : YxPageListView 
    {
        #region UI Param
        [Tooltip("查找信息View")]
        public YxView FindInfoView;
        #endregion

        #region Data Param
        [Tooltip("加入茶馆成功回调")]
        public List<EventDelegate> OnEnterTeaHouseSuccess = new List<EventDelegate>();
        [Tooltip("查找茶馆信息回调")]
        public List<EventDelegate> OnFindTeaHouseSuccess = new List<EventDelegate>();
        [Tooltip("查找不到对应茶馆提示")]
        public string FindEmptyNotice = "找不到此茶馆！";
        #endregion

        #region Local Data
        /// <summary>
        /// 茶馆状态
        /// </summary>
        protected const string KeyTeaHouseState = "mstatus";
        #endregion

        #region Life Cycle
        #endregion

        #region Function

        /// <summary>
        /// 点击进入茶馆按钮
        /// </summary>
        /// <param name="item"></param>
        public void ClickJoinBtn(TeaHouseListItem item)
        {
            var teaHouseData = item.GetData<TeaHouseListeItemData>();
            if (teaHouseData!=null)
            {
                TeaUtil.FindTeaHouse(teaHouseData.TeaId,null, success =>
                {
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(OnEnterTeaHouseSuccess.WaitExcuteCalls());
                    }
                });
            }
        }
        
        /// <summary>
        /// 点击申请加入茶馆按钮
        /// </summary>
        /// <param name="item"></param>
        public void ClickApplyBtn(TeaHouseListItem item)
        {
            TeaUtil.ApplyJoinTeaHouse(item.TeaHouseIdLabel.Value, null, success =>
            {
                var data = item.GetData<TeaHouseListeItemData>();
                if (data!=null)
                {
                    data.OnApplySuccess();
                    ChangeChildItem(item,data);
                }
            });
        }

        /// <summary>
        /// 点击查找茶馆按钮
        /// </summary>
        /// <param name="teaId"></param>
        public void ClickFindTeaBtn(string teaId)
        {
            TeaUtil.FindTeaHouse(teaId, null, success =>
            {
                var dic = success as Dictionary<string, object>;
                var teaData=new TeaData(dic);
                var itemData=new TeaHouseListeItemData(success,typeof(TeaHouseListeItemData));
                if (!teaData.Mstatus.Equals(TeaState.Invalid))
                {
                    if (FindInfoView)
                    {
                        FindInfoView.UpdateView(itemData);
                    }
                    if (gameObject.activeInHierarchy)
                    {
                        StartCoroutine(OnFindTeaHouseSuccess.WaitExcuteCalls());
                    }
                }
                else
                {
                    YxWindowManager.ShowMessageWindow(FindEmptyNotice);
                }
            },false);
        }


        #endregion
    }
}
