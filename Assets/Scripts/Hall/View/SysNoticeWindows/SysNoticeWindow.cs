using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Hall.View.SysNoticeWindows
{
    /// <summary>
    /// 旧版公告
    /// </summary>
    public class SysNoticeWindow : YxNguiWindow
    {
        public UIGrid ActionGrid;
        public UIGrid NoticeGrid;
        public ActionItemView ActionItemPerfab;
        public NoticeItemView NoticeItemPerfab;

        private const string ActionItemKey = "activity";
        private const string NoticeItemKey = "news";
        // Use this for initialization
        protected override void OnAwake ()
        {
            var parm = new Dictionary<string, object>();
            Facade.Instance<TwManger>().SendActions(new[] { ActionItemKey, NoticeItemKey }, parm, OnCallBack, false);
        }

        private void OnCallBack(object msg)
        {
            var data = msg as Dictionary<string, object>;
            if (data == null) return;
            if (data.ContainsKey(ActionItemKey))
            {
                InitActionView(data[ActionItemKey] as List<object>);
            }
            if (data.ContainsKey(NoticeItemKey))
            {
                InitNoticeView(data[NoticeItemKey] as List<object>);
            }
        }

        private void InitNoticeView(IEnumerable<object> data)
        {
            if (data == null) return;
            var pts = NoticeGrid.transform;
            foreach (var itemDate in data)
            {
                var itemDict = itemDate as Dictionary<string, object>;
                if(itemDict==null)continue;
                var item = Instantiate(NoticeItemPerfab);
                var ts = item.transform;
                ts.parent = pts;
                ts.localScale = Vector3.one;
                ts.localPosition = Vector3.zero;
                var title = itemDict["subject_m"].ToString();
                var date = itemDict["create_dt"].ToString();
                var url = itemDict["detail_url_x"].ToString();
                item.SetData(title, date, url);
                item.gameObject.SetActive(true);
            }
            NoticeGrid.enabled = true;
            NoticeGrid.repositionNow = true;
        }

        private void InitActionView(IEnumerable<object> data)
        {
            if (data == null) return;
            var pts = ActionGrid.transform;
            foreach (var itemDate in data)
            {
                var itemDict = itemDate as Dictionary<string, object>;
                if (itemDict == null) continue;
                
                var item = Instantiate(ActionItemPerfab);
                var ts = item.transform;
                ts.parent = pts;
                ts.localScale = Vector3.one;
                ts.localPosition = Vector3.zero;

                var adata = new ActionData(itemDict); 
                item.UpdateView(adata);
                item.gameObject.SetActive(true);
            } 
            ActionGrid.repositionNow = true;
        } 
    }
}
