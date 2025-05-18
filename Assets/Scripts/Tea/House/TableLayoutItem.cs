/** 
 *文件名称:     TableLayoutItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-10-30 
 *描述:         茶馆桌面位置布局样式
 *              结合TeaTableLayout使用
 *历史记录: 
*/

using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tea.House
{
    public class TableLayoutItem : MonoBehaviour
    {
        [Tooltip("当前样式对应的座位数量")]
        public int SeatNum;
        public List<Vector3> Pos=new List<Vector3>();

        public List<Vector3> GetLayoutPos()
        {
            return Pos;
        }
    }
}
