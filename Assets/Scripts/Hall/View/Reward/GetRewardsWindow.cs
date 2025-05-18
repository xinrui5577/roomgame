using System.Collections.Generic;
using Assets.Scripts.Common.Windows;
using UnityEngine;

namespace Assets.Scripts.Hall.View.Reward
{
    /// <summary>
    /// 领取奖励界面
    /// </summary>
    public class GetRewardsWindow : YxNguiWindow
    {
        public UIButton SureBtn;

        public UIGrid LayoutGrid;

        public GameObject ItemPrefab;

        public void Init(object data)
        {

        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            while (LayoutGrid.transform.childCount>0)
            {
                DestroyImmediate(LayoutGrid.transform.GetChild(0).gameObject);
            }
            Dictionary<string, object> param = (Dictionary<string, object>)Data;
            Dictionary<string, object> rewards = (Dictionary<string, object>)param["reward"];
            foreach (var reward in rewards)
            {
                GameObject obj=NGUITools.AddChild(LayoutGrid.gameObject, ItemPrefab);
                if (obj)
                {
                    RewardItem item = obj.GetComponent<RewardItem>();
                    item.Init(reward.Key,int.Parse(reward.Value.ToString()));
                }
            }
        }
    }
}
