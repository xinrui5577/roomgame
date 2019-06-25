using System;
using System.Collections.Generic;
using Assets.Scripts.Common.UI;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.AboutRoomWindows
{
    /// <summary>
    /// 创建房间
    /// </summary>
    public class CreateRoomItemView : YxView
    {
        #region 私有属性
        #endregion
        #region 公有属性　 
        [Tooltip("组视图预制体")]
        public NguiView GroupViewPerfab;
        [Tooltip("标题")]
        public UILabel TitleLabel;
        #endregion

        protected override void OnFreshView()
        {
            if (Data == null) return;
            var ruleInfo = Data as RuleInfo;
            if (ruleInfo == null) return; 
            SetRules(ruleInfo);
        }

        /// <summary>
        /// 设置规则
        /// </summary>
        /// <param name="ruleData"></param>
        private void SetRules(RuleInfo ruleData)
        {
            if (ruleData == null) return; 
            var ruleList = ruleData.GroupDatas;
            var pts = transform;
            _curGroupList = new List<NguiView>();
            _curMaxGroupCount = _curGroupList.Count;
            _groupFreshCount= 0;
            foreach (var groupData in ruleList)
            {
                var view = CreateGroup(groupData, pts, UpdateGroups);
                _curGroupList.Add(view); 
            }
        }

        private List<NguiView> _curGroupList;
        private int _curMaxGroupCount;
        private int _groupFreshCount;
        private void UpdateGroups(object obj)
        {
            _groupFreshCount++;
            if (_curGroupList == null) return;
            if (_groupFreshCount < _curMaxGroupCount) return;
            var curlpos = Vector3.zero;
            var count = _curGroupList.Count;
            for (var i = 0; i < count; i++)
            {
                var view = _curGroupList[i];
                view.gameObject.SetActive(true);
                var gdata = view.GetData<GroupData>();
                if (gdata != null) curlpos.y -= gdata.OffY;
                view.transform.localPosition = curlpos;
                var bound = view.Bounds;
                var size = bound.size;
                curlpos.y -= size.y; 
            }
        }

        /// <summary>
        /// 创建组
        /// </summary>
        /// <param name="groupData">标签</param>
        /// <param name="pts"></param>
        /// <param name="callBack"></param>
        /// <param name="pos"></param>
        private NguiView CreateGroup(GroupData groupData, Transform pts,Action<object> callBack,Vector3 pos=default(Vector3))
        {
            var groupView = YxWindowUtils.CreateItem(GroupViewPerfab, pts, pos);
            groupView.UpdateViewWithCallBack(groupData,callBack);
            return groupView;
        }
    }
}
