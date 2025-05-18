/** 
 *文件名称:     MailDetailWindow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-01-26 
 *描述:         邮件详情View
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.MailWindow
{
    public class MailDetailView : YxView
    {
        #region UI Param
        [Tooltip("标题")]
        public UILabel Title;
        [Tooltip("内容")]
        public UILabel Content;
        [Tooltip("发送人")]
        public UILabel Author;
        [Tooltip("时间")]
        public UILabel Time;
        #endregion
        #region Data Param
        #endregion
        #region Local Data
        /// <summary>
        /// 临时邮件详情
        /// </summary>
        private MailDetailInfo _cacheInfo;
        #endregion
        #region Life Cycle
        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (Data==null)
            {
                return;
            }
            if(Data is MailDetailInfo)
            {
                _cacheInfo=Data as MailDetailInfo;
               
                Show();
            }
        }

        protected override void OnShow()
        {
            if(_cacheInfo!=null)
            {
                Author.TrySetComponentValue(_cacheInfo.Sender);
                Content.TrySetComponentValue(_cacheInfo.Content);
                Time.TrySetComponentValue(_cacheInfo.Time);
            }
  
        }
        #endregion
        #region Function
        #endregion 	
    }

    /// <summary>
    /// 邮件详细信息
    /// </summary>
    public class MailDetailInfo : YxData
    {
        /// <summary>
        /// Key状态
        /// </summary>
        private const string KeyStatus = "status_i";
        /// <summary>
        /// Key邮件ID
        /// </summary>
        private const string KeyMid = "mid";
        /// <summary>
        /// Key内容
        /// </summary>
        private const string KeyContent = "content_x";
        /// <summary>
        /// Key标题
        /// </summary>
        private const string KeyTitle = "subject_m";
        /// <summary>
        /// Key发送人
        /// </summary>
        private const string KeySender = "author_m";
        /// <summary>
        /// Key创建时间
        /// </summary>
        private const string KeyCreateTime = "create_dt";
        /// <summary>
        /// 状态
        /// </summary>
        private int _status;
        /// <summary>
        /// ID
        /// </summary>
        private string _mid;
        /// <summary>
        /// 内容
        /// </summary>
        private string _content;
        /// <summary>
        /// 标题
        /// </summary>
        private string _title;
        /// <summary>
        /// 发送人
        /// </summary>
        private string _sender;
        /// <summary>
        /// 邮件时间
        /// </summary>
        private string _time;

        public int Status
        {
            get
            {
                return _status;
            }
            set { _status = value; }
        }

        public string Mid
        {
            get
            {
                return _mid;                
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
        }

        public string Time
        {
            get
            {
                return _time;
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
        }

        public string Sender {
            get
            {
                return _sender;              
            }
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _status, KeyStatus);
            dic.TryGetValueWitheKey(out _content, KeyContent);
            dic.TryGetValueWitheKey(out _mid, KeyMid);
            dic.TryGetValueWitheKey(out _title, KeyTitle);
            dic.TryGetValueWitheKey(out _sender, KeySender);
            dic.TryGetValueWitheKey(out _time, KeyCreateTime);
        }

        public MailDetailInfo(object data):base(data)
        {

        }
    }
}
