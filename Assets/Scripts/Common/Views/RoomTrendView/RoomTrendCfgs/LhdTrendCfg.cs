using System.Collections.Generic;
using Assets.Scripts.Common.Interface;
using UnityEngine;

namespace Assets.Scripts.Common.Views.RoomTrendView.RoomTrendCfgs
{
    public class LhdTrendCfg : MonoBehaviour, ITrendCfg
    {
        public ITrendReciveData CreatTrendReciveData(object data)
        {
            return new LhdTrendData(data);
        }
    }

    public class LhdTrendData : ITrendReciveData
    {
        /// <summary>
        /// ÖÐ½±µÄÎ»ÖÃ
        /// </summary>
        public List<string> Wins=new List<string>();

        public LhdTrendData(object data)
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
            Wins = new List<string> { "l" };
            return this;
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
