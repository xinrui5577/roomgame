/** 
 *文件名称:     AgencyItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-11-18 
 *描述:         代理信息
 *              单独一位代理的相关信息
 *历史记录: 
*/

using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Hall.View.Agency
{
    public class AgencyItem : MonoBehaviour
    {
        [Tooltip("联系方式预设")]
        public AgencySingleItem Item;
        [Tooltip("布局Gird")]
        public UIGrid Grid;
        public void RefreshItem(object data)
        {
            if(data is Dictionary<string,object>)
            {
                Dictionary<string, object> dic =(Dictionary<string, object>)data;
                if (Grid)
                {
                    while (Grid.transform.childCount > 0)
                    {
                        DestroyImmediate(Grid.transform.GetChild(0).gameObject);
                    }
                    int index = 0;
                    foreach (var iData in dic)
                    {
                        var obj= NGUITools.AddChild(Grid.gameObject, Item.gameObject);
                        if (obj)
                        {
                            AgencySingleItem item = obj.GetComponent<AgencySingleItem>();
                            if (item)
                            {
                                item.Refresh(iData.Key,iData.Value.ToString());
                            }
                        }
                        index++;
                    }
                    Grid.repositionNow = true;
                }
        
            }

        }
    }
}
