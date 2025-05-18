using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.SpinningWindows
{
    /// <summary>
    /// 转盘item视图
    /// </summary>
    public class SpinningItemView : YxView
    {
        /// <summary>
        /// 奖品名称
        /// </summary>
        [Tooltip("奖品名称")]
        public UILabel RewardName;
        /// <summary>
        /// 奖品个数
        /// </summary>
        [Tooltip("奖品个数")]
        public UILabel RewardCount;
        /// <summary>
        /// 奖品图片
        /// </summary>
        [Tooltip("奖品图片")]
        public UITexture RewardIcon;
         
        protected override void OnFreshView()
        {
            var sData = Data as SpinningItemData;
            if (sData == null) return;
            RewardName.text = sData.Name;
            RewardCount.text = sData.Count.ToString();
            AsyncImage.Instance.GetAsyncImage(sData.ImgUrl,( texture,hashCode) =>
            {
                RewardIcon.mainTexture = texture;
            });
        }
    }

    public class SpinningItemData
    {
        public int Id = -1;
        public int Count = 0;
        public string Name = "";
        public string ImgUrl = "";
        public int Index = -1;
        public string Msg = "";
        /* {"id": "1",
         "name": "\u73b0\u91d1\u5927\u5956",
         "img": null,
         "grade": "2"}*/
        public SpinningItemData(IDictionary<string, object> dict, int index = -1)
        {
            Index = index;
            if (dict == null) return;
            if (dict.ContainsKey("id"))
            {
                if (!int.TryParse(dict["id"].ToString(), out Id))
                {
                    Id = -1;
                }
            }
            if (dict.ContainsKey("num"))
            {
                if (!int.TryParse(dict["num"].ToString(), out Count))
                {
                    Count = 0;
                }
            } 
            if (dict.ContainsKey("name"))
            {
                var temp = dict["name"];
                if(temp != null)Name = temp.ToString();
            }
            if (dict.ContainsKey("img"))
            {
                var temp = dict["img"];
                if (temp != null) ImgUrl = temp.ToString();
            }
            if (dict.ContainsKey("good_info"))
            {
                var temp = dict["good_info"];
                if (temp != null) Msg = temp.ToString();
            }
        }
    }
}
