using System.Collections.Generic;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

/*===================================================
 *文件名称:     TeaMemberInfoView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-10-18
 *描述:        	茶馆成员信息面板
 *              成员加入申请，已加入成员信息相关信息
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Tea.Page
{
    public class TeaMemberInfoView : YxPageListView
    {
        #region UI Param
        #endregion

        #region Data Param
        [Tooltip("key茶馆口令")]
        public string KeyId = "id";
        [Tooltip("key成员限制权限")]
        public string KeyMemberLimitAction = "group.teaHouseAudit";
        [Tooltip("key成员创建房间权限请求")]
        public string KeyCreateRoomLimitAction = "group.createRoomCtrl";
        [Tooltip("key玩家Id")]
        public string KeyUserId = "user_id";
        [Tooltip("key成员授权状态（1.进入茶馆2.请离茶馆）")]
        public string KeyLimitStatus = "status";
        [Tooltip("key授权类型:0.授权创建 1.取消授权")]
        public string KeyLimitType = "type";

        #endregion

        #region Local Data
        #endregion

        #region Life Cycle

        protected override void SetActionDic()
        {
            if (ParamKeys.Count != ParamValues.Count)
            {
                Debug.LogError(string.Format("{0} paramKeys count is not equals paramValues count,please check resource!!!", name));
                return;
            }
            if (TryAddItemToList(ParamKeys,KeyId))
            {
                ParamValues.Add(TeaUtil.CurTeaId);
            }
            base.SetActionDic();
        }

        #endregion

        #region Function

        /// <summary>
        /// 成员权限操作
        /// </summary>
        /// <param name="memberItem"></param>
        /// <param name="status">1.同意 2.拒绝（请离）</param>
        public void OnAgreeClick(TeaMemberInfoItem memberItem,string status)
        {
            var itemData = memberItem.GetData<TeaMemberInfoItemData>();
            if (itemData != null)
            {
                CurItem = memberItem;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic[KeyId] = TeaUtil.CurTeaId;
                dic[KeyUserId] = itemData.UserId;
                dic[KeyLimitStatus] = status;
                Facade.Instance<TwManager>().SendAction(KeyMemberLimitAction, dic, OnLimitChange);
            }
        }

        /// <summary>
        /// 权限操作成功回调(同意申请，拒绝申请或请离茶馆)
        /// </summary>
        /// <param name="data"></param>
        private void OnLimitChange(object data)
        {
            TeaUtil.GetBackString(data);
            RemoveChildItem(CurItem);
        }

        /// <summary>
        /// 更改成员授权状态
        /// </summary>
        /// <param name="memberItem">成员信息Item</param>
        /// <param name="status">授权状态值</param>
        public void OnCreateRightChange(TeaMemberInfoItem memberItem,string status)
        {
            var itemData = memberItem.GetData<TeaMemberInfoItemData>();
            if (itemData != null)
            {
                int changeStatus = 0;
                int.TryParse(status, out changeStatus);
                CurItem = memberItem;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic[KeyId] = TeaUtil.CurTeaId;
                dic[KeyUserId] = itemData.UserId;
                dic[KeyLimitType] = changeStatus;
                Facade.Instance<TwManager>().SendAction(KeyCreateRoomLimitAction, dic, success =>
                {
                    itemData.ChangeCreateStatus(changeStatus);
                    ChangeChildItem(memberItem, itemData);
                });
            }
        }

        #endregion
    }
}
