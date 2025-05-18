/** 
 *文件名称:     BankExtractBindWindow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-05-23 
 *描述:         绑定兑换信息界面
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.BankWindows
{
    public class BankExtractBindWindow : YxNguiWindow
    {
        #region UI Param
        [Tooltip("手机号输入框")]
        public UIInput PhoneInput;
        [Tooltip("Title文本")]
        public UILabel TitleLabel;
        [Tooltip("按钮文本")]
        public UILabel BtnLabel;
        #endregion
        #region Data Param
        [Tooltip("标题文本格式（绑定状态）")]
        public string TitleBindStateFormat = "更换{0}";
        [Tooltip("标题文本格式（未绑定状态）")]
        public string TitleUnBindStateFormat = "绑定{0}";
        [Tooltip("按钮文本提示（绑定状态）")]
        public string BtnBindStateFormat = "更换";
        [Tooltip("按钮文本提示（未绑定状态）")]
        public string BtnUnBindStateFormat = "绑定";

        [Tooltip("请求接口名称")]
        public string ActionName= "depositBand";
        [Tooltip("参数Key")]
        public List<string> ParamKeys=new List<string>();
        [Tooltip("参数Value")]
        public List<UIInput> ParamValues=new List<UIInput>();
        [Tooltip("绑定状态显示处理")]
        public List<EventDelegate> OnBindStateShow; 
        public bool BindState
        {
            get; set; }

        #endregion
        #region Local Data
        /// <summary>
        /// 本地资源
        /// </summary>
        private ExtractItemData _curData;
        #endregion 
        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            _curData = GetData<ExtractItemData>();
            if(_curData == null)
            {
                YxDebug.LogError("_curData==========null");
                return;
            }
            BindState = _curData.BindInfo.BindState;
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(OnBindStateShow.WaitExcuteCalls());
            }
            var platfomName = _curData.PlatformName;
            platfomName = BindState
                ? string.Format(TitleBindStateFormat, platfomName)
                : string.Format(TitleUnBindStateFormat, platfomName);
            var btnNotice = BindState ? BtnBindStateFormat : BtnUnBindStateFormat;
            TitleLabel.TrySetComponentValue(platfomName);
            BtnLabel.TrySetComponentValue(btnNotice);
            if (PhoneInput != null)
            {
                var phone = _curData.BindInfo.BindPhoneNumber;
                PhoneInput.enabled = string.IsNullOrEmpty(phone);
                PhoneInput.value = phone;
            }
        }

        #endregion 
        #region Function

        public void OnClickBindBtn()
        {
            var dic = GetParams();
            if(!dic.Count.Equals(ParamKeys.Count))
            {
                YxMessageBox.Show("绑定信息不完整，请完善");
                return;
            }
            Facade.Instance<TwManager>().SendAction(ActionName, dic, OnSuccessBind);
        }

        private Dictionary<string,object> GetParams()
        {
            var dic=new Dictionary<string,object>();
            var count = ParamKeys.Count;
            if(count==ParamValues.Count)
            {
                for (var i = 0; i < count; i++)
                {
                    var key = ParamKeys[i];
                    var value = ParamValues[i].value;
                    if(string.IsNullOrEmpty(key)||string.IsNullOrEmpty(value))
                    {
                        continue;
                    }
                    dic.Add(key,value);
                }
            }
            else
            {
                YxDebug.LogError("ParamKeys count is not equal with ");
            }
            return dic;
        }

        protected virtual void OnSuccessBind(object obj)
        {
           
            if (_curData!=null)
            {
                if (_curData.BindInfo!=null)
                {
                    var data = obj as Dictionary<string,object>;
                    var info=new ExtractBindInfo(data);
                    if(info!=null)
                    {
                        _curData.BindInfo= info;
                        CallBack(_curData);
                    }
                    else
                    {
                        YxDebug.LogError("Info is null");
                    }
                 
                }
                else
                {
                    YxDebug.LogError("BindInfo is null");
                }

            }
            else
            {
                YxDebug.LogError("_curData is null");
               
            }
            Close();
        }

        #endregion
    }
}
