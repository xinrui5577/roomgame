using UnityEngine;
using System.Collections.Generic;
using YxFramwork.Common;

namespace Assets.Scripts.Game.LXGameScripts.lx39
{
    /// <summary>
    /// 显示红线
    /// </summary>
    public class ShowLasers : MonoBehaviour
    {
        public GameObject[] Lasers;
        

        public virtual void ShowLaserOnWin()
        {
            List<int> openList = App.GetGameData<OverallData>().Response.LineList;
            if (openList.Count <= 0 || openList.Count > App.GetGameData<OverallData>().Line) return;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i] == 1)
                    Lasers[i].SetActive(true);
                else
                    Lasers[i].SetActive(false);
            }
        }
        public void HideAllLaser()
        {
            foreach (var laser in Lasers)
            {
                laser.SetActive(false);
            }
        }
    }
}