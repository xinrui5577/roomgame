using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Tea
{
    public class TeaStatisticsSingleItem : YxView
    {
        [Tooltip("游戏名字")]
        public UILabel GameName;
        [Tooltip("游戏统计")]
        public UILabel Total;
        [Tooltip("统计显示")]
        public string TotalFormat;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var gameData = (KeyValuePair<string, object>)Data;
            GameName.TrySetComponentValue(gameData.Key);
            Total.TrySetComponentValue(string.Format("{0}{1}", gameData.Value, TotalFormat));
        }
    }
}
