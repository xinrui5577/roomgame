using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base
{
    /// <summary>
    /// 可循环item
    /// </summary>
    public class BaseSocialWrapItem : YxView
    {
        /// <summary>
        /// Item 唯一Id（Real id）
        /// </summary>
        [HideInInspector]
        public string OnlyId;
        /// <summary>
        /// 转换后的字典数据
        /// </summary>
        protected Dictionary<string, object> ParseDicData;
  
        protected override void OnFreshView()
        {
            if (Data!=null&&Data is IDictionary)
            {
                ParseDicData = Data as Dictionary<string, object>;
                if (ParseDicData!=null)
                {
                    ParseDicData.TryGetValueWitheKey(out OnlyId,SocialTools.KeyId, string.Empty);
                    if (OnlyId != string.Empty)
                    {
                        DealFreshData();
                    }
                    else
                    {
                        YxDebug.LogEvent(SocialTools.KeyNoticeOnlyIdEmpty);
                    }
                }
            }
        }
        /// <summary>
        /// 刷新数据
        /// </summary>
        protected virtual void DealFreshData()
        {

        }
    }
}
