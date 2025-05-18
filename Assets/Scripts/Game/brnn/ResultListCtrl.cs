using System.Collections.Generic;
using Assets.Scripts.Game.brnn.Windows;
using UnityEngine;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.brnn
{
    public class ResultListCtrl : MonoBehaviour
    { 
        /// <summary>
        /// 走势菜单显示的结果个数
        /// </summary>
        public int ResultCount = 10;
        public readonly List<bool[]> HistoryData = new List<bool[]>();
        
        public void AddResult(bool[] r)
        {
            if (r.Length > 4)
            {
                r = new [] {r[1], r[2], r[3], r[4]};
            }

            HistoryData.Add(r);
            if (HistoryData.Count > ResultCount)
            {
                HistoryData.RemoveAt(0);
                var win = YxWindowManager.GetWindowInstance<HistoryWindow>();
                if (win != null)
                {
                    win.UpdateView();
                }
            }
        }
    }
}
