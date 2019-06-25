/** 
 *文件名称:     TeaTableItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-10-27 
 *描述:         茶馆牌桌布局处理，为了不改变茶馆逻辑因此生成这个脚本，只是处理牌桌上的位置显示
 *历史记录:     2017年10月30日 11:24:38
 *              目前使用外挂式手动处理，可以抽象为算法，待优化
*/

using System.Collections.Generic;
using System.Linq;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using UnityEngine.Networking;
using YxFramwork.Framework;

namespace Assets.Scripts.Tea.House
{
    public class TeaTableLayout :MonoBehaviour
    {
        [Tooltip("多人桌面布局处理")]
        public List<TableLayoutItem> Layouts=new List<TableLayoutItem>();
        /// <summary>
        /// 座位人数
        /// </summary>
        private int _seatNum;

        public int SeatNum
        {
            set
            {
                SetLayoutByNum(value);
            }
            get
            {
                return _seatNum;
            }
        }

        //void Awake()
        //{
        //    SetLayoutByNum(_seatNum);
        //}

        public void SetLayoutByNum(int num)
        {
            _seatNum = num;
            List<Vector3> vecs=GetPosByNum(num);
            Transform trans = transform;
            for (int i = 0,count= trans.childCount; i < count; i++)
            {
                Transform itemTrans= trans.GetChild(i);
                if (i < num)
                {
                    itemTrans.localPosition = vecs[i];
                }
            }
        }

        private List<Vector3> GetPosByNum(int num)
        {
            TableLayoutItem layout = Layouts.Find(item => item.SeatNum == num);
            if(layout==null)
            {
                return new List<Vector3>();
            }
            return layout.GetLayoutPos();
        }
    }
}
