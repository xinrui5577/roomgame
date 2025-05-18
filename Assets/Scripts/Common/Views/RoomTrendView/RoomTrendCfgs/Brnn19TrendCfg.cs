using System.Collections.Generic;
using Assets.Scripts.Common.Interface;
using UnityEngine;

namespace Assets.Scripts.Common.Views.RoomTrendView.RoomTrendCfgs
{
    public class Brnn19TrendCfg : MonoBehaviour, ITrendCfg
    {
        public ITrendReciveData CreatTrendReciveData(object data)
        {
            return new Brnn19TrendData(data);
        }
    }
    public class Brnn19TrendData : ITrendReciveData
    {
        /// <summary>
        /// 中奖的位置
        /// </summary>
        public List<string> Wins = new List<string>();
        /// <summary>
        /// 当前局数内出现的次数
        /// </summary>
        public List<int> ShowNums=new List<int>();

        public Brnn19TrendData(object data)
        {
            var recordDic = data as Dictionary<string, object>;
            if (recordDic != null)
            {
                var win = recordDic["win"].ToString();
                var winStr = win.Split(',');
                Wins.AddRange(winStr);

             
            }
        }

        public ITrendReciveData SetResultArea()
        {
            throw new System.NotImplementedException();
        }

        public List<string> GetResultArea()
        {
            return Wins;
        }

        public int GetResultType()
        {
            return -1;
        }
    }
}
