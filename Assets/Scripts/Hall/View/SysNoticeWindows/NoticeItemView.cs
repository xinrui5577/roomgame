using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.SysNoticeWindows
{
    public class NoticeItemView : YxView
    {
        private string _titleContent;
        private string _dateContent;
        private string _url;
        /// <summary>
        /// 标题
        /// </summary>
        public UILabel TitleLabel;
        /// <summary>
        /// 日期
        /// </summary>
        public UILabel DateLabel;

        public void SetData(string title,string date,string url)
        {
            _url = url;
            _titleContent = title;
            _dateContent = date;
            FreshView();
        }

        public override void SetOrder(int order)
        {
        }

        protected override void OnFreshView()
        {
            TitleLabel.text = _titleContent;
            DateLabel.text = _dateContent;
        }

        public void OnClickItem()
        {
            Application.OpenURL(_url);
        }
    }
}
