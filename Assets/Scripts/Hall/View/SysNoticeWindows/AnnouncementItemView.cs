using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View.SysNoticeWindows
{
    /// <summary>
    /// 公告item
    /// </summary>
    public class AnnouncementItemView : YxView
    {
        /// <summary>
        /// 内容图片
        /// </summary>
        [Tooltip("图片内容")]
        public UITexture Poster;
        [Tooltip("文本内容")]
        public GameObject TextContent;
        [Tooltip("标题")]
        public UILabel TitleLabel;
        [Tooltip("内容")]
        public UILabel ContentLabel;
        [Tooltip("日期")]
        public UILabel DataLabel;
        [Tooltip("作者")]
        public UILabel AuthorLabel;

        protected override void OnFreshView()
        {
            var acData = GetData<AnnouncementData>();
            if (acData == null) return;
            if (Poster!=null)
            {
                if (!string.IsNullOrEmpty(acData.PosterUrl))
                { 
                    Poster.gameObject.SetActive(true);
                    if(TextContent!=null)TextContent.SetActive(false);
                    YxWindowManager.ShowWaitFor();
                    AsyncImage.Instance.GetAsyncImage(acData.PosterUrl, texture2d =>
                        {
                            YxWindowManager.HideWaitFor();
                            Poster.mainTexture = texture2d;
                        });
                    return;
                }
                Poster.gameObject.SetActive(false);
            }
            if (TextContent == null) return;
            TextContent.SetActive(true);
            if(TitleLabel!=null)TitleLabel.text = acData.Title;
            if (ContentLabel != null) ContentLabel.text = acData.Content;
            if (DataLabel != null) DataLabel.text = acData.Date;
            if (AuthorLabel != null) AuthorLabel.text = acData.Author;
        }

        public void OnActionClick()
        {
            var acData = GetData<AnnouncementData>();
            if (acData == null) return;
            if (string.IsNullOrEmpty(acData.ClickUrl)) return;
            Application.OpenURL(acData.ClickUrl);
        }
    }

    public class AnnouncementData
    {
        /// <summary>
        /// 图片url
        /// </summary>
        public string PosterUrl = "";
        /// <summary>
        /// 点击url
        /// </summary>
        public string ClickUrl = "";
        /// <summary>
        /// 标题
        /// </summary>
        public string Title = ""; 
        /// <summary>
        /// 内容
        /// </summary>
        public string Content = "";
        /// <summary>
        /// 日期
        /// </summary>
        public string Date;
        /// <summary>
        /// 作者
        /// </summary>
        public string Author;

        public AnnouncementData(IDictionary<string, object> obj)
        {
            if (obj == null) return;
            if (obj.ContainsKey("pic_url_x"))
            {
                var temp = obj["pic_url_x"];
                if (temp != null) PosterUrl = temp.ToString();
            }
            if (obj.ContainsKey("detail_url_x"))
            {
                var temp = obj["detail_url_x"];
                if (temp != null) ClickUrl = temp.ToString();
            }
            if (obj.ContainsKey("title_m"))
            {
                var temp = obj["title_m"];
                if (temp != null) Title = temp.ToString();
            }
            if (obj.ContainsKey("desc_x"))
            {
                var temp = obj["desc_x"];
                if (temp != null) Content = temp.ToString();
            }
            if (obj.ContainsKey("create_dt"))
            {
                var temp = obj["create_dt"];
                if (temp != null) Date = temp.ToString();
            }
            if (obj.ContainsKey("release_auther"))
            {
                var temp = obj["release_auther"];
                if (temp != null) Author = temp.ToString();
            }
        }
    }
}
