using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.Windows.MatchWindows
{

    /// <summary>
    /// Item
    /// </summary>
    public class YxMatchItem : YxView
    {
        [Tooltip("比赛名字")]
        public YxBaseLabelAdapter MatchName;
        [Tooltip("参赛人数")]
        public YxBaseLabelAdapter NumOfParticipants;
        [Tooltip("比赛游戏")]
        public YxBaseLabelAdapter GameName;
        [Tooltip("比赛图标")]
        public YxBaseTextureAdapter Icon;

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = GetData<MatchItemData>();
            if (data == null) { return;}
            FreshMatchName(data.Name);
            FreshNumOfParticipants(data.Pcount);
            FreshGameName(data.GameName);
            FreshIcon(data.IconUrl);
        }

        private void FreshIcon(string dataIconUrl)
        {
            AsyncImage.Instance.GetAsyncImage(dataIconUrl, (t2, code) =>
            {
                if (Icon == null) return;
                Icon.SetTextureWithCheck(t2, code);
            });
        }

        /// <summary>
        /// 刷新比比赛名称
        /// </summary>
        private void FreshMatchName(string matchName)
        {
            if (!MatchName) return;
            MatchName.Text(matchName);
        }

        /// <summary>
        /// 刷新参赛者人数
        /// </summary>
        private void FreshNumOfParticipants(int numOfParticipants)
        {
            if (!NumOfParticipants) return;
            NumOfParticipants.Text(numOfParticipants.ToString());
        }

        /// <summary>
        /// 刷新游戏名
        /// </summary>
        private void FreshGameName(string gameName)
        {
            if (!MatchName) return;
            GameName.Text(gameName);
        }

        /// <summary>
        /// 报名
        /// </summary>
        public void OnSignUp()
        {
            //预约
            //报名
            //免费进入
        }


        /// <summary>
        /// 比赛item数据
        /// </summary>
        public class MatchItemData
        {
            public string Name;
            public int Pcount;
            public string GameName;
            public string IconUrl;

            public void Parse(Dictionary<string,object> dict)
            {
                if (dict == null) return;
                dict.Parse("name",ref Name);
                dict.Parse("pcount",ref Pcount);
                dict.Parse("gameName",ref GameName);
                dict.Parse("icon", ref IconUrl);
            }
        }
    }
}
