using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.@base;
using Assets.Scripts.Common.YxPlugins.Social.SocialViews.Data;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.YxPlugins.Social.SocialViews.Photo
{
    /// <summary>
    /// 亲友圈相册Item
    /// </summary>
    public class SocialPhotoItem : BaseSocialSelectWrapItem
    {
        [Tooltip("相册图片")]
        public YxBaseTextureAdapter PhotoImage;
        public bool HaveData { private set; get;}
        [Tooltip("存在数据事件")]
        public List<EventDelegate> HaveDataAction=new List<EventDelegate>();

        protected override void DealFreshData()
        {
            base.DealFreshData();
            string photoData;
            ParseDicData.TryGetValueWitheKey(out photoData, SocialTools.KeyData);
            HaveData =!string.IsNullOrEmpty(photoData);
            if (!string.IsNullOrEmpty(photoData))
            {
                PhotoImage.ShowImage(photoData);
            }
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(HaveDataAction.WaitExcuteCalls());
            }
        }
    }
}
