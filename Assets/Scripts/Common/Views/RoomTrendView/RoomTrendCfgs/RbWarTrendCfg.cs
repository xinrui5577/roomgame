using System.Collections.Generic;
using Assets.Scripts.Common.Interface;
using UnityEngine;

namespace Assets.Scripts.Common.Views.RoomTrendView.RoomTrendCfgs
{
    public class RbWarTrendCfg : MonoBehaviour, ITrendCfg
    {
        public ITrendReciveData CreatTrendReciveData(object data)
        {
            return new RbWarTrendData(data);
        }
    }
    public class RbWarTrendData : ITrendReciveData
    {
        /// <summary>
        /// 中奖的位置
        /// </summary>
        public List<string> Area=new List<string>();
        /// <summary>
        /// 中奖的牌型
        /// </summary>
        public int Ctype;

        public RbWarTrendData(object data)
        {
            var recordDic = data as Dictionary<string, object>;
            if (recordDic != null)
            {
                Area.Add(recordDic["area"].ToString()) ;
                Ctype = int.Parse(recordDic["ctype"].ToString());
            }
        }

        public ITrendReciveData SetResultArea()
        {
            Area = new List<string>{"1"};
            return this;
        }

        public List<string> GetResultArea()
        {
            return Area;
        }

        public int GetResultType()
        {
            return Ctype;
        }
    }
}
