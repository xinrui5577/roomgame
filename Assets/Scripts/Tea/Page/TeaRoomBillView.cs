using System.Collections.Generic;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

/*===================================================
 *文件名称:     TeaRoomBillView.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-10-12
 *描述:        	茶馆房间账单面板
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Tea.Page
{
    public class TeaRoomBillView : YxPageListView 
    {
        #region UI Param
        #endregion

        #region Data Param
        [Tooltip("key茶馆口令")]
        public string KeyId= "id";
        [Tooltip("Key解散房间请求")]
        public string KeyDissolveRoom = "group.dissolveRoom";
        #endregion

        #region Local Data
        /// <summary>
        /// Key房间号
        /// </summary>
        private const string KeyRoomId = "roomId";
        #endregion

        #region Life Cycle

        /// <summary>
        /// 重写请求参数：处理茶馆账单附属请求参数id(茶馆ID)
        /// </summary>
        protected override void SetActionDic()
        {
            if (ParamKeys.Count!=ParamValues.Count)
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
        /// 点击解散
        /// </summary>
        public void OnDissolveClick(TeaRoomBillItem billItem)
        {
            var itemData=billItem.GetData<TeaRoomBillItemData>();
            if (itemData!=null)
            {
                CurItem = billItem;
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic[KeyRoomId] =itemData.RoomId;
                Facade.Instance<TwManager>().SendAction(KeyDissolveRoom, dic, OnDissolveSucess);
            }
        }
        /// <summary>
        /// 解散成功回调
        /// </summary>
        /// <param name="data"></param>
        private void OnDissolveSucess(object data)
        {
            TeaUtil.GetBackString(data);
            RemoveChildItem(CurItem);
        }
        #endregion
    }
}
