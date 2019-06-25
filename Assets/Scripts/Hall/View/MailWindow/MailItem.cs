/** 
 *文件名称:     MailItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-01-26 
 *描述:         邮件窗口
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.MailWindow
{
    public class MailItem : YxView
    {
        #region UI Param
        [Tooltip("标题文本")]
        public UILabel TitleLabel;
        [Tooltip("时间文本")]
        public UILabel TimeLabel;
        [Tooltip("读取状态")]
        public GameObject[] ReadState;

        #endregion

        #region Data Param
        public string MailId
        {
            get
            {
                if (_curData!=null)
                {
                    return _curData.MailId;
                }
                else
                {
                    return "";
                }
            }
        }

        public int Status
        {
            get
            {
                if (_curData != null)
                {
                    return _curData.Status;
                }
                return -1;
            }
            set
            {
                if (_curData != null)
                {
                     _curData.Status=value;
                }
            }
        }

        #endregion

        #region Local Data
        /// <summary>
        /// 当前详情数据
        /// </summary>
        private MailItemData _curData;

        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            CheckItemData();
        } 

        #endregion

        #region Function

        protected bool CheckItemData()
        {
            if (Data is MailItemData)
            {
                _curData = Data as MailItemData;
                RefreshView();
                return true;
            }
            return false;
        }

        public void RefreshView()
        {
            YxTools.TrySetComponentValue(TimeLabel, _curData.Time);
            YxTools.TrySetComponentValue(TitleLabel, _curData.Title);
            var state = _curData.Status;
            if (ReadState != null)
            {
                var lenth = ReadState.Length;
                if (lenth > state)
                {
                    for (int i = 0; i < lenth; i++)
                    {
                        ReadState[i].SetActive(false);
                    }
                    ReadState[state].SetActive(true);
                }
            }
        }
        #endregion

    }

    /// <summary>
    /// 邮件概要数据
    /// </summary>
    public class MailItemData : YxData
    {
        /// <summary>
        /// Key读取状态
        /// </summary>
        private const string KeyStatus = "status_i";
        /// <summary>
        /// Key邮件ID
        /// </summary>
        private const string KeyId = "mid";
        /// <summary>
        /// Key标题
        /// </summary>
        private const string KeyTitle = "subject_m";
        /// <summary>
        /// Key创建时间
        /// </summary>
        private const string KeyTime = "create_dt";
        /// <summary>
        /// Title
        /// </summary>
        private string _title;
        /// <summary>
        /// 时间
        /// </summary>
        private string _time;
        /// <summary>
        /// 邮件Id
        /// </summary>
        private string _mailId;
        /// <summary>
        ///读取状态 0,未读 1，已读
        /// </summary>
        private int _status;

        public string Title
        {
            get { return _title; }
        }

        public string Time
        {
            get {return _time; }
        }

        public string MailId
        {
            get { return _mailId; }
        }

        public bool IsRead
        {
            get { return _status.Equals(1); }
        }

        public int Status
        {
            get { return _status; }
            set { _status = value; }
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            YxTools.TryGetValueWitheKey(dic,out _title,KeyTitle);
            YxTools.TryGetValueWitheKey(dic, out _time, KeyTime);
            YxTools.TryGetValueWitheKey(dic, out _mailId,KeyId);
            YxTools.TryGetValueWitheKey(dic,out _status, KeyStatus);
        }

        public MailItemData(object data) : base(data)
        {

        }

    }
}
