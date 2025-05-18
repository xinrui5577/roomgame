using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;
using YxFramwork.Tool;

/*===================================================
 *文件名称:     SlideShowItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-10-24
 *描述:        	轮播图单图
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Hall.View.SlideShowView
{
    public class SlideShowItem : YxView 
    {
        #region UI Param
        [Tooltip("显示图片")]
        public YxBaseTextureAdapter ShowTexture;

        #endregion

        #region Data Param

        /// <summary>
        /// 链接Url
        /// </summary>
        public string LinkUrl { private set; get; }

        #endregion

        #region Local Data

        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if(Data!=null)
            {
                if(Data is SlideItemData)
                {
                    var slideData = Data as SlideItemData;
                    AsyncImage.Instance.GetAsyncImage(slideData.TextureUrl, (texture, hashCode) =>
                    {
                        ShowTexture.TrySetComponentValue(texture);
                    });
                    LinkUrl = slideData.LinkUrl;
                }
            }
        }

        #endregion

        #region Function

        #endregion
    }

    public class SlideItemData : YxData
    {
        /// <summary>
        /// Key 图片地址
        /// </summary>
        private const string KeyTexUrl = "TexUrl";

        /// <summary>
        /// Key 连接地址
        /// </summary>
        private const string KeyLinkUrl = "LinkUrl";

        public string TextureUrl { private set; get;}

        public string LinkUrl { private set; get;}

        public SlideItemData(object data) : base(data)
        {

        }

        public SlideItemData(object data, Type type) : base(data, type)
        {

        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            var textureUrl = "";
            var linkUrl = "";
            dic.TryGetValueWitheKey(out textureUrl, KeyTexUrl);
            dic.TryGetValueWitheKey(out linkUrl, KeyLinkUrl);
            TextureUrl = textureUrl;
            LinkUrl = linkUrl;
        }
    }
}
