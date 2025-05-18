using System.Collections.Generic;
using Assets.Scripts.Common.Models;
using Assets.Scripts.Common.Models.CreateRoomRules;
using Assets.Scripts.Common.UI;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    /// <summary>
    /// 创建房间
    /// </summary>
    public class CreateRoomItemView : FreshLayoutBaseView
    {
        #region 公有属性　 
        [Tooltip("组视图预制体")]
        public RuleGroupView GroupViewPerfab;//RuleGroupView
        [Tooltip("标题")]
        public UILabel TitleLabel;
        #endregion


        protected override void OnAwake()
        {
            InitStateTotal = 2;
            CheckIsStart = true;
            base.OnAwake();
        }

        protected override void InitViews()
        {
            var ruleData = Data as CreateRoomRuleInfo;
            if (ruleData == null)
            {
                CallBack(IdCode);
                return;
            }
            var ruleList = ruleData.GroupDatas;
            if (ruleList != null)
            {
                FreshSingleGroup(ruleList);
            }
            else
            {
                var rows = ruleData.RowData;
                if (rows != null)
                {
                    FreshMultiGroup(rows);
                }
            }
        }
         
        /// <summary>
        /// 刷新单行多个组的样式
        /// </summary>
        private void FreshMultiGroup(IList<GroupRowData> rows)
        {
            var rowsCount = rows.Count;
            var oldCount = BufferViewCount;
            var oddOldCount = oldCount;
            var pts = transform;
            for (var j = 0; j < rowsCount; j++)
            {
                var row = rows[j];
                var groupDatas = row.GroupDatas;
                var dataCount = groupDatas.Count;
                var minCount = dataCount > oddOldCount ? oddOldCount : oddOldCount - dataCount;
                oddOldCount -= minCount;
                //1、使用缓存池里的对象
                var i = FreshBufferView(0,minCount,groupDatas);
                //3、不够的创建新的对象
                CreateNewView(i, dataCount, groupDatas, pts);
            }
            //2、多余的隐藏
            HideOdd(oldCount - oddOldCount - 1, oddOldCount);
        }

        /// <summary>
        /// 刷新单行单个组的样式
        /// </summary>
        private void FreshSingleGroup(IList<GroupData> ruleList)
        {
            //1、使用缓存池里的对象
            var oldCount = BufferViewCount;
            var dataCount = ruleList.Count;
            var minCount = Mathf.Min(dataCount, oldCount);
            var i = FreshBufferView(0,minCount,ruleList);
            //2、多余的隐藏
            i = HideOdd(i, oldCount);
            //3、不够的创建新的对象
            CreateNewView(i,dataCount, ruleList,transform);
        }

        protected override void OnFreshLayout()
        {
            var curlposY = 0f;
            var count = BufferViewCount;
            for (var i = 0; i < count; i++)
            {
                var view = GetBufferView(i) as RuleGroupView;
                if(view == null)continue;
                //                view.gameObject.SetActive(true);
                if (!view.IsShow()) continue;
                var gdata = view.GetData<GroupData>();
                if (gdata != null)
                {
                    curlposY += gdata.OffY;
                }
                var tsPos = view.transform.localPosition;
                tsPos.y = curlposY;
                view.transform.localPosition = tsPos;
                var size = view.Bounds.size;
                curlposY -= size.y;
            }
        }

        protected override YxView CreateView<T>(T data, Transform pts, Vector3 pos = default(Vector3))
        {
            var groupView = YxWindowUtils.CreateItem(GroupViewPerfab, pts, pos);
            return groupView;
        }
    }
}
