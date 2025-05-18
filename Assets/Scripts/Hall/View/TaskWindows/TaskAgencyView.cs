/** 
 *文件名称:     TaskAgencyView.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-11-18 
 *描述:         代理界面
 *              1.显示当前平台的游戏代理信息
 *              2.支持复制代理信息到剪切板中
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.Agency;
using UnityEngine;

namespace Assets.Scripts.Hall.View.TaskWindows
{
    public class TaskAgencyView : YxSameReturnView
    {
        [Tooltip("代理信息Grid")]
        public UIGrid ShowGrid;
        [Tooltip("代理")]
        public AgencyItem Item;
        [Tooltip("可见性操作")]
        public List<EventDelegate> OnVisibleAction;
        [Tooltip("Tab名称设置回调")]
        public List<EventDelegate> OnTabNameSetAction;
        /// <summary>
        /// Key数据
        /// </summary>
        private const string KeyData = "data";
        /// <summary>
        /// key 是否可见
        /// </summary>
        private const string KeyVisible = "Visible";
        /// <summary>
        ///  key Tab名称
        /// </summary>
        private const string KeyTabName = "TabName";
        /// <summary>
        /// 是否可见
        /// </summary>
        private bool _visible;

        public bool Visible
        {
            set { _visible = value; }
            get { return _visible; }
        }

        /// <summary>
        /// 标签名称
        /// </summary>
        public string TabName
        {
            private set; get;
        }

        protected override void DealShowData()
        {
            if(Data is Dictionary<string,object>)
            {
                Dictionary<string, object> dic = (Dictionary<string, object>) Data;

                dic.TryGetValueWitheKey(out _visible, KeyVisible);
                if (gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnVisibleAction.WaitExcuteCalls());
                }
                if (dic.ContainsKey(KeyData))
                {
                    List<object> agencys = (List<object>)dic[KeyData];
                    if (dic.ContainsKey(KeyTabName))
                    {
                        TabName = dic[KeyTabName].ToString();
                        if (gameObject.activeInHierarchy)
                        {
                            StartCoroutine(OnTabNameSetAction.WaitExcuteCalls());
                        }
                    }
                    if (ShowGrid)
                    {
                        ShowGrid.transform.DestroyChildren();
                        if (Item)
                        {
                            for (int i = 0, count = agencys.Count; i < count; i++)
                            {
                                var obj = ShowGrid.gameObject.AddChild(Item.gameObject);
                                if (obj)
                                {
                                    AgencyItem item = obj.GetComponent<AgencyItem>();
                                    if (item)
                                    {
                                        item.RefreshItem(agencys[i]);
                                    }
                                }
                            }
                            ShowGrid.repositionNow = true;
                        }
                    }
                }
            }
        }
    }
}
