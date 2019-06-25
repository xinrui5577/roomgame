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
        /// <summary>
        /// 数据
        /// </summary>
        private string _keyData = "data";

        protected override void DealShowData()
        {
            if(Data is Dictionary<string,object>)
            {
                Dictionary<string, object> dic = (Dictionary<string, object>) Data;
                if(dic.ContainsKey(_keyData))
                {
                    List<object> agencys = (List<object>)dic[_keyData];
                    if (ShowGrid)
                    {
                        ClearTrans(ShowGrid.transform);
                        if (Item)
                        {
                            for (int i = 0, count = agencys.Count; i < count; i++)
                            {
                                var obj = NGUITools.AddChild(ShowGrid.gameObject,Item.gameObject);
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
