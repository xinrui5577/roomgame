/** 
 *文件名称:     BankExtractView.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-01-30 
 *描述:         银行提现功能
 *历史记录:     功能需求为：
 *                        1.后台配置开关可以控制提现功能是否开放,开关统一控制提现与提现记录
 *                        2.提现功能支持多种提现方式与提现比例。
 *              2018-06-07 17:45:53
 *              兑换方式：选择方式增加tab式选择
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows.TabPages;
using Assets.Scripts.Hall.View.PageListWindow;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common.Model;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.BankWindows
{
    public class BankExtractView : YxSameReturnView
    {
        #region UI Param
        [Tooltip("选项")]
        public UIPopupList Option;
        [Tooltip("平台帐号")]
        public UIInput PlatformId;
        [Tooltip("消耗货币")]
        public UIInput CostNumber;
        [Tooltip("密码")]
        public UIInput Password;
        [Tooltip("兑换比例")]
        public UILabel ExtractRate;
        [Tooltip("绑定状态提示")]
        public UILabel BindNotice;
        [Tooltip("手机号码")]
        public UIInput PhoneInput;
        [Tooltip("绑定帐号")]
        public UILabel BindAccount;
        [Tooltip("手机验证码")]
        public UIInput TelephoneVerify;
        [Tooltip("提现方式文本")]
        public UILabel ExtractTypeNotice;
        [Tooltip("提现方式相关文本")]
        public UILabel ExtractTypeAbout;
        [Tooltip("成功转换处理")]
        public List<EventDelegate> OnExtractSuccess;
        [Tooltip("显示开关处理")]
        public List<EventDelegate> OnVisibleAction;
        [Tooltip("绑定状态切换操作")]
        public List<EventDelegate> OnBindStateChangeAction;
        [Tooltip("提现提示信息")]
        public UILabel ExtracNotice;

        #endregion

        #region Data Param     
        [Tooltip("提现文本格式")]
        public string ExtractFormat = "可提现{0}元";
        [Tooltip("提现接口")]
        public string ExtractAciton = "depositCoin";
        [Tooltip("兑换文本格式")]
        public string RateFormat = "{0}:1";
        [Tooltip("确认提示信息格式")]
        public string SureNoticeFormat = "提取方式:{0}\n消耗金币:{1}\n兑换金额:{2}元\n确认提取现金？";
        [Tooltip("按钮绑定状态提示(已绑定)")]
        public string BtnBindStateNotice = "更换{0}";
        [Tooltip("按钮绑定状态提示(未绑定)")]
        public string BtnUnBindStateNotice = "绑定{0}";
        [Tooltip("帐号未绑定提示")]
        public string AccountUnBindFormat = "未绑定{0}帐号";
        [Tooltip("绑定信息请求")]
        public string BindInfoAction= "depositBandInfo";
        [Tooltip("绑定类型格式")]
        public string BindTypeFormat="bind_{0}";
        [Tooltip("兑换提示文本格式")]
        public string TypeNoticeFormat = "收款{0}";
        [Tooltip("Tab标签按下状态图片命名格式")]
        public string TabDownFormat = "Down_{0}";
        [Tooltip("Tab标签抬起状态图片命名格式")]
        public string TabUpFormat = "Up_{0}";
        [Tooltip("输入框组合，用来收集输入框的所有信息")]
        public InputGroup TheInputGroup;
        [Tooltip("指定账号为空时的提示")]
        public string PlatformTip = "账号不能为空";

        public bool ViewVisible
        {
            get; set;
        }

        public bool RecordVisible
        {
            get; set;
        }

        public bool BindCouldChange
        {
            get; set;
        }

        public bool ChangeBtnShow
        {
            get
            {
                if (_curCostData!=null)
                {
                    return !BindCouldChange && _curCostData.BindInfo.BindState;
                }
                else
                {
                    YxDebug.LogError("_curCostData is null");
                    return false;
                }
            }
        }

        #endregion

        #region Local Data

        /// <summary>
        /// 为方便处理，将主体数据转换为keypair形式
        /// </summary>
        private Dictionary<string, YxData> _dataDic = new Dictionary<string, YxData>();
        /// <summary>
        /// 当前兑换数据
        /// </summary>
        private ExtractItemData _curCostData;
        /// <summary>
        /// 绑定信息
        /// </summary>
        private Dictionary<string, ExtractBindInfo> BindInfo = new Dictionary<string, ExtractBindInfo>();
        /// <summary>
        /// 当前兑换方式
        /// </summary>
        private string _curCostType;
        /// <summary>
        /// 兑换金额
        /// </summary>
        private double _extractValue;
        /// <summary>
        /// 绑定的手机号
        /// </summary>
        private string _bindPhone;
        /// <summary>
        /// Key消耗货币数量
        /// </summary>
        private const string KeyExtractValue= "depositValue";
        /// <summary>
        /// Key消耗货币类型
        /// </summary>
        private const string KeyExtractCostType = "depositType";
        /// <summary>
        /// key平台类型
        /// </summary>
        private const string KeyExtractPlatformType = "depositAccountType";
        /// <summary>
        /// key兑换帐号
        /// </summary>
        private const string KeyExtractAccount = "depositAccount";
        /// <summary>
        /// key兑换密码
        /// </summary>
        private const string KeyPassword = "password";
        /// <summary>
        /// key验证码
        /// </summary>
        private const string KeyTelephoneVerify = "telVerify";
        #endregion

        #region Life Cycle
        #endregion

        protected Type GetType()
        {
            return typeof(ExtractItemData);
        }

        protected override void DealShowData()
        {
            if (Data is Dictionary<string, object>)
            {
                ExtractData data = new ExtractData(Data, GetType());
                ViewVisible = data.ExtractVisible;
                RecordVisible = data.RecordVisible;
                BindCouldChange = data.CouldChangeBind;
                if (gameObject.activeInHierarchy)
                {
                StartCoroutine(OnVisibleAction.WaitExcuteCalls());
                }
                ExtracNotice.TrySetComponentValue(data.ExtractNotice);
                if (ViewVisible)
                {
                    _dataDic = data.DataDic;
                    if (Option)
                    {
                        Option.Clear();
                        foreach (var item in data.DataDic)
                        {
                            Option.AddItem(item.Key);
                        }
                        if (Option.items.Count > 0)
                        {
                            var showName = Option.items[0];
                            Option.Set(showName);
                            _curCostType = showName;
                            _curCostData = _dataDic[showName] as ExtractItemData;
                        }
                        else
                        {
                            YxDebug.LogError("Extract types count is zero,please check again!");
                        }
                    }
                    else
                    {
                        var tabWindow = MainYxView as YxTabPageWindow;
                        if (tabWindow)
                        {  
                            List<TabData> datas=new List<TabData>();
                            foreach (var item in data.DataDic)
                            {
                                var itemData = item.Value as ExtractItemData;
                                var tabName = itemData != null ? itemData.PlatformType: item.Key;
                                TabData tabData=new TabData()
                                {
                                    Name = item.Key,
                                    UpStateName=string.Format(TabUpFormat, tabName),
                                    DownStateName = string.Format(TabDownFormat, tabName),
                                    Data =item.Key
                                };
                                datas.Add(tabData);
                            }
                            tabWindow.TabDatas = datas.ToArray();
                            tabWindow.UpdateView();
                        }
                    }

                }
            }
        }

        #region Function

        public void OnClickExtractView()
        {
            string platform = "";
            double value = 0;
            string password = "";
            if (PlatformId)
            {
                platform = PlatformId.value;
            }
            else
            {
                platform = _curCostData.BindInfo.BindAccount;
            }
            if (CostNumber)
            {
                double.TryParse(CostNumber.value,out value);  
            }
            else
            {
                return;
            }
            if (Password)
            {
                password = Password.value;
            }
            if (string.IsNullOrEmpty(platform))
            {
                YxMessageBox.Show(PlatformTip);
                return;
            }
            if (Password && string.IsNullOrEmpty(password))
            {
                YxMessageBox.Show("密码不能为空");
                return;
            }
 
            long realValue = YxUtiles.RecoverShowNumber(value);

            if (realValue < 1)
            {
                YxMessageBox.Show("请输入正确的兑换金额!!!");
                return;
            }
            var flag = TelephoneVerify != null;
            var telVerify = flag ? TelephoneVerify.value : null;
            if ("".Equals(telVerify))
            {
                YxMessageBox.Show("请输入验证码!!!");
                return;
            }
            YxDebug.Log("Real value is:" + realValue);
            YxDebug.Log("show value is :"+value);
            var returnValue = YxUtiles.RecoverShowNumber(value);
            YxDebug.Log("Real exchange value is :" + returnValue);
            YxMessageBox.Show(
           string.Format(SureNoticeFormat,_curCostType, value, GlobalUtile.ReduceNumber(_extractValue)),
           null,
           (window, btnname) =>
           {
               switch (btnname)
               {
                   case YxMessageBox.BtnLeft:
                       var dict = new Dictionary<string, object>()
                       {
                           {KeyExtractPlatformType, _curCostData.PlatformType},
                           {KeyExtractCostType, _curCostData.CostType},
                           {KeyExtractAccount, platform},
                           {KeyExtractValue, returnValue},
                           {KeyPassword, password},
                           {KeyTelephoneVerify, telVerify}
                       };
                       if (TheInputGroup != null)
                       {
                           TheInputGroup.Imbark(dict);
                       }
                       Facade.Instance<TwManager>().SendAction(ExtractAciton, dict,
                            delegate
                            {
                                if (gameObject.activeInHierarchy)
                                {
                                    StartCoroutine(OnExtractSuccess.WaitExcuteCalls());
                                }
                                Clear();
                            }
                            );
                            break;
                       case YxMessageBox.BtnRight:
                       break;
                         }
                        },
                        true,
                        YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
                        );

        }

        private void Clear()
        {
            PlatformId.TrySetComponentValue("");
            CostNumber.TrySetComponentValue("");
            Password.TrySetComponentValue("");
        }

        public void OnCostTypeChange(string value)
        {
            if (_dataDic.ContainsKey(value))
            {
                _curCostType = value;
                _curCostData = _dataDic[value] as ExtractItemData;
                FreshBindInfo();
            }
            else
            {
                YxDebug.LogError("There is not exist such key in dic,key is:"+value);
            }
        }

        public void UpdataExtractNum()
        {
            ExtractRate.TrySetComponentValue(string.Format(RateFormat, YxUtiles.ReduceNumber(_curCostData.Rate)));
            if (ShowLabel)
            {
                string num = "";
                if (CostNumber)
                {
                    num = CostNumber.value;
                }              
                if (string.IsNullOrEmpty(num))
                {
                    ShowLabel.text = "";
                    return;
                }
                double getValue;
                if (double.TryParse(num,out getValue))
                {
                    var bankCoin = YxUtiles.GetShowNumber(UserInfoModel.Instance.UserInfo.BankCoin);
                    if (getValue > bankCoin)
                    {
                        getValue = bankCoin;
                        CostNumber.value = getValue.ToString();
                    }
                    _extractValue = (double)YxUtiles.RecoverShowNumber(getValue) / _curCostData.Rate;
                    ShowLabel.text = string.Format(ExtractFormat, GlobalUtile.ReduceNumber(_extractValue));
                }
            }
        }

        public void UpdataExtractNumByCoin()
        {
            ExtractRate.TrySetComponentValue(string.Format(RateFormat, YxUtiles.ReduceNumber(_curCostData.Rate)));
            if (ShowLabel)
            {
                string num = "";
                if (CostNumber)
                {
                    num = CostNumber.value;
                }
                if (string.IsNullOrEmpty(num))
                {
                    ShowLabel.text = "";
                    return;
                }
                double getValue;
                if (double.TryParse(num, out getValue))
                {
                    var coin = YxUtiles.GetShowNumber(UserInfoModel.Instance.UserInfo.CoinA);
                    if (getValue > coin)
                    {
                        getValue = coin;
                        CostNumber.value = getValue.ToString();
                    }
                    _extractValue = (double)YxUtiles.RecoverShowNumber(getValue) / _curCostData.Rate;
                    ShowLabel.text = string.Format(ExtractFormat, GlobalUtile.ReduceNumber(_extractValue));
                }
            }
        }

        public void OnClickBindBtn()
        {
            MainYxView.OpenWindowWithData(_curCostData.BinwWindow, _curCostData, OnBindStateChange);
        }

        public void OnBindStateChange(object obj)
        {
            if (obj!=null)
            {
               var getData = obj as ExtractItemData;
                if (getData!=null)
                {
                    _curCostData = getData;
                    _dataDic[_curCostType] = _curCostData;
                    var key=string.Format(BindTypeFormat, _curCostData.PlatformType);
                    BindInfo[key] = _curCostData.BindInfo;
                    FreshBindInfo();
                }
                else
                {
                    YxDebug.LogError("OnBindStateChange getData is null");
                }
               
            }
            else
            {
                YxDebug.LogError("OnBindStateChange obj is null");
            }
        }

        private void FreshBindInfo()
        {
            if (_curCostData != null)
            {
                var platformType = _curCostData.PlatformType;
                var bindType = string.Format(BindTypeFormat, platformType);
                if (!BindInfo.ContainsKey(bindType))
                {
                    BindInfo.Add(bindType, new ExtractBindInfo(new Dictionary<string, object>()
                    {
                    {ExtractBindInfo.KeyType,platformType}, {ExtractBindInfo.KeyAccount,"" }
                    }));
                }
                var bindData = BindInfo[bindType];
                _curCostData.BindInfo = bindData;
                if(gameObject.activeInHierarchy)
                {
                    StartCoroutine(OnBindStateChangeAction.WaitExcuteCalls());
                }
                var notice = bindData.BindState
                    ? string.Format(BtnBindStateNotice, _curCostData.PlatformName)
                    : string.Format(BtnUnBindStateNotice, _curCostData.PlatformName);
                var account = bindData.BindState ? bindData.BindAccount : string.Format(AccountUnBindFormat, _curCostData.PlatformName);
                BindNotice.TrySetComponentValue(notice);
                BindAccount.TrySetComponentValue(account);
                if (PhoneInput != null)
                {
                    if (string.IsNullOrEmpty(bindData.BindPhoneNumber))
                    {
                        bindData.BindPhoneNumber = _bindPhone;
                    }
                    else
                    {
                        _bindPhone = bindData.BindPhoneNumber;
                    }
                    PhoneInput.value = _bindPhone;
                }
                ExtractTypeNotice.TrySetComponentValue(string.Format(TypeNoticeFormat, _curCostData.PlatformName));
                ExtractTypeAbout.TrySetComponentValue(_curCostData.AboutNotice);
            }
            else
            {
                YxDebug.LogError("_curCostData is null");
            }
        }

        public void GetBindInfo()
        {
            Facade.Instance<TwManager>().SendAction(BindInfoAction, null,
                delegate (object data)
                {
                    var bindInfos = new YxData(data, typeof(ExtractBindInfo));
                    var dict = bindInfos.DataDic;
                    if (dict == null) return;
                    if (dict.ContainsKey("phone"))
                    {
                        DictionaryHelper.Parse(dict, "phoen", ref _bindPhone);
                    }
                    BindInfo = dict.ToDictionary(t => t.Key, t => t.Value as ExtractBindInfo);
                    FreshBindInfo();
                }
                );
        }

        public void OnTabItemSelect(YxTabItem tab)
        {
            if(tab.GetToggle().value)
            {
                var data = tab.GetData() as TabData;
                if (data!=null&&data.Data!=null)
                {
                    OnCostTypeChange(data.Data.ToString());
                }
            }
        }

        #endregion
    }
    /// <summary>
    /// 提现数据
    /// </summary>
    public class ExtractData : YxData
    {
        /// <summary>
        /// Key提现显示开关
        /// </summary>
        private const string KeyExtractVissible = "ExtractVisible";
        /// <summary>
        /// Key提现记录显示开关
        /// </summary>
        private const string KeyRecordVisible = "RecordVisible";
        /// <summary>
        /// Key 改绑开关 
        /// </summary>
        private const string KeyCouldChange = "CouldChangeBind";
        /// <summary>
        /// Key 提现提示
        /// </summary>
        private const string KeyExtractNotice = "ExtracNotice";
        /// <summary>
        /// 提现功能是否可见
        /// </summary>
        private bool _extractVisible;
        /// <summary>
        /// 提现功能是否可见
        /// </summary>
        private bool _recordVisible;
        /// <summary>
        /// 是否可以更改绑定状态
        /// </summary>
        private bool _couldChangeBindState;
        /// <summary>
        /// 提现提示
        /// </summary>
        private string _extractNotice;

        public bool ExtractVisible
        {
            get
            {
                return _extractVisible;
            }
        }

        public bool RecordVisible
        {
            get
            {
                return _recordVisible;
            }
        }

        public bool CouldChangeBind
        {
            get
            {
                return _couldChangeBindState;
            }
        }

        public string ExtractNotice
        {
            get
            {
                return _extractNotice;
            }
        }

        public ExtractData(object data, Type type) : base(data, type)
        {

        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            base.ParseData(dic);
            dic.TryGetValueWitheKey(out _extractVisible, KeyExtractVissible);
            dic.TryGetValueWitheKey(out _recordVisible, KeyRecordVisible);
            dic.TryGetValueWitheKey(out _couldChangeBindState, KeyCouldChange);
            dic.TryGetValueWitheKey(out _extractNotice, KeyExtractNotice);
        }
    }
    /// <summary>
    /// 提现信息数据
    /// </summary>
    public class ExtractItemData : YxData
    {
        /// <summary>
        /// Key平台类型
        /// </summary>
        private const string KeyType="type";
        /// <summary>
        /// Key消耗类型
        /// </summary>
        private const string KeyCostType = "costType";
        /// <summary>
        /// Key倍率
        /// </summary>
        private const string KeyRate= "rate";
        /// <summary>
        /// Key name
        /// </summary>
        private const string KeyName= "name";
        /// <summary>
        /// Key 绑定窗口名称
        /// </summary>
        private const string KeyBindWidow = "bindwindow";
        /// <summary>
        /// Key相关提示
        /// </summary>
        private const string KeyAboutNotice = "noticeAbout";
        /// <summary>
        /// 倍率
        /// </summary>
        private int _rate;
        /// <summary>
        /// 消耗类型
        /// </summary>
        private int _costType;
        /// <summary>
        /// 平台类型 (0.微信1.支付宝2.银行卡)
        /// </summary>
        private string _platformType;
        /// <summary>
        /// 平台名称
        /// </summary>
        private string _platformName;
        /// <summary>
        /// 绑定窗口名称
        /// </summary>
        private string _bindWindowName;
        /// <summary>
        /// 兑换提示
        /// </summary>
        private string _aboutNotice;

        public int Rate
        {
            get { return _rate; }
        }

        public int CostType
        {
            get {return _costType; }
        }

        public string PlatformType
        {
            get {return _platformType; }
        }

        public string PlatformName
        {
            get { return _platformName; }
        }

        public string BinwWindow
        {
            get { return _bindWindowName; }
        }

        public string AboutNotice
        {
            get{return _aboutNotice;}
        }

        /// <summary>
        /// 绑定信息
        /// </summary>
        public ExtractBindInfo BindInfo
        {
            set; get;
        }

        public ExtractItemData(object data) : base(data)
        {

        }

        public ExtractItemData(object data, Type type) : base(data, type)
        {

        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            dic.TryGetValueWitheKey(out _platformType, KeyType);
            dic.TryGetValueWitheKey(out _costType, KeyCostType);
            dic.TryGetValueWitheKey(out _rate, KeyRate);
            dic.TryGetValueWitheKey(out _platformName, KeyName);
            dic.TryGetValueWitheKey(out _bindWindowName, KeyBindWidow);
            dic.TryGetValueWitheKey(out _aboutNotice, KeyAboutNotice);
        }
    }
    /// <summary>
    /// 单独的绑定信息数据
    /// </summary>
    public class ExtractBindInfo:YxData
    {
        /// <summary>
        /// Key平台类型
        /// </summary>
        public const string KeyType = "type";
        /// <summary>
        /// Key平台类型
        /// </summary>
        public const string KeyAccount = "depositAccount";
        /// <summary>
        /// Key平台类型
        /// </summary>
        public const string KeyPhone = "phone";
        /// <summary>
        /// 平台类型 (0.微信1.支付宝2.银行卡)
        /// </summary>
        private string _platformType;
        /// <summary>
        /// 绑定帐号
        /// </summary>
        private string _bindAccount;
        /// <summary>
        /// 手机号
        /// </summary>
        private string _phoneNumber;
        public string PlatformType
        {
            get { return _platformType; }
        }
        /// <summary>
        /// 绑定状态
        /// </summary>
        public bool BindState
        {
            get
            {
                return !string.IsNullOrEmpty(_bindAccount);
            }

        }

        public string BindAccount
        {
            get { return _bindAccount; }
            set { _bindAccount = value;}
        }

        public string BindPhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value;}
        }

        public ExtractBindInfo(object data) : base(data)
        {
        }

        public ExtractBindInfo(object data, Type type) : base(data, type)
        {
        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            YxTools.TryGetValueWitheKey(dic, out _platformType, KeyType);
            YxTools.TryGetValueWitheKey(dic, out _bindAccount, KeyAccount);
            YxTools.TryGetValueWitheKey(dic, out _phoneNumber, KeyPhone);
        }
    }

}
