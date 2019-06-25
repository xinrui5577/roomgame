/** 
 *文件名称:     BankExtractView.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-01-30 
 *描述:         银行提现功能
 *历史记录: 
*/

using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Hall.View.PageListWindow;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;
using YxFramwork.View;

namespace Assets.Scripts.Hall.View.BankWindows
{
    public class BankExtractView : YxSameReturnView
    {
        #region UI Param
        [Tooltip("显示父级")]
        public GameObject ShowArea;
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
        [Tooltip("成功转换处理")]
        public List<EventDelegate> OnExtractSuccess;

        #endregion

        #region Data Param     
        [Tooltip("提现文本格式")]
        public string ExtractFormat = "可提现{0}元";
        [Tooltip("提现接口")]
        public string ExtractAciton = "depositCoin";
        [Tooltip("兑换文本格式")]
        public string ExtractRateFormat = "比例:{0}";
        [Tooltip("确认提示信息格式")]
        public string SureNoticeFormat = "提取方式:{0}\n消耗金币:{1}\n兑换金额:{2}元\n确认提取现金？";
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
        /// 当前兑换方式
        /// </summary>
        private string _curCostType;
        /// <summary>
        /// 兑换金额
        /// </summary>
        private long _extractValue;
        /// <summary>
        /// Key消耗货币数量
        /// </summary>
        private string _keyExtractValue= "depositValue";
        /// <summary>
        /// Key消耗货币类型
        /// </summary>
        private string _keyExtractCostType = "depositType";
        /// <summary>
        /// key平台类型
        /// </summary>
        private string _keyExtractPlatformType = "depositAccountType";
        /// <summary>
        /// key兑换帐号
        /// </summary>
        private string _keyExtractAccount = "depositAccount";
        /// <summary>
        /// key兑换密码
        /// </summary>
        private string _keyPassword = "password";
        #endregion

        #region Life Cycle
        #endregion

        protected override void OnAwake()
        {
            if (ShowArea)
            {
                ShowArea.SetActive(false);
            }
        }

        protected Type GetType()
        {
            return typeof(ExtractItemData);
        }

        protected override void DealShowData()
        {
            if(Data is Dictionary<string,object>)
            {
                ExtractData data=new ExtractData(Data, GetType());
                if(ShowArea)
                {
                    var visible = data.Visible;
                    ShowArea.SetActive(visible);
                    if (visible)
                    {
                        if (Option)
                        {
                            _dataDic = data.DataDic;
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

                    }
                }

            }
        }

        #region Function

        public void OnClickExtractView()
        {
            string platform = "";
            float value = 0;
            string password = "";
            if (PlatformId)
            {
                platform = PlatformId.value;
            }
            if (CostNumber)
            {
                value = float.Parse(CostNumber.value);  
            }
            if (Password)
            {
                password = Password.value;
            }
            if (string.IsNullOrEmpty(platform))
            {
                YxMessageBox.Show("账号不能为空");
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                YxMessageBox.Show("密码不能为空");
                return;
            }
            long realValue = YxUtiles.RecoverShowNumber(value );
            if (realValue < 1)
            {
                YxMessageBox.Show("输入金额过小，请重新输入!!!");
                return;
            }
            YxDebug.Log("Real value is:" + realValue);
            YxDebug.Log("show value is :"+value);
            var returnValue = YxUtiles.RecoverShowNumber(value);
            YxDebug.Log("Real exchange value is :" + returnValue);
            YxMessageBox.Show(
           string.Format(SureNoticeFormat,_curCostType, value, _extractValue),
           null,
           (window, btnname) =>
           {
               switch (btnname)
               {
                   case YxMessageBox.BtnLeft:
                       Facade.Instance<TwManger>().SendAction(
                                ExtractAciton,
                                new Dictionary<string, object>()
                                {
                                { _keyExtractPlatformType,_curCostData.CostType},
                                { _keyExtractCostType,_curCostData.CostType},
                                { _keyExtractAccount,platform},
                                { _keyExtractValue,returnValue},
                                { _keyPassword,password},
                            },
                            delegate (object msg)
                            {
                                StartCoroutine(YxTools.WaitExcuteCalls(OnExtractSuccess));
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
            if (PlatformId)
            {
                PlatformId.value = "";
            }
            if (CostNumber)
            {
                CostNumber.value = "";
            }
            if (Password)
            {
                Password.value = "";
            }
        }

        public void OnCostTypeChange(string value)
        {
            if (_dataDic.ContainsKey(value))
            {
                _curCostType = value;
                _curCostData = _dataDic[value] as ExtractItemData;
            }
            else
            {
                YxDebug.LogError("There is not exist such key in dic,key is:"+value);
            }
        }

        public void UpdataExtractNum()
        {
            if (ExtractRate)
            {
                ExtractRate.text = string.Format(ExtractRateFormat, YxUtiles.ReduceNumber(_curCostData.Rate));
            }
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
                _extractValue = (YxUtiles.RecoverShowNumber(double.Parse(num)) / _curCostData.Rate);          
                ShowLabel.text = string.Format(ExtractFormat, _extractValue);
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
        private const string KeyVissible = "visible";
        /// <summary>
        /// 是否可见
        /// </summary>
        private bool _visible;

        public bool Visible
        {
            get
            {
                return _visible;
            }
        }

        public ExtractData(object data, Type type) : base(data, type)
        {

        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            base.ParseData(dic);
            YxTools.TryGetValueWitheKey(dic,out _visible,KeyVissible);
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
        private const string KeyCostType = "CostType";
        /// <summary>
        /// Key倍率
        /// </summary>
        private const string KeyRate= "Rate";
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

        public ExtractItemData(object data) : base(data)
        {

        }

        public ExtractItemData(object data, Type type) : base(data, type)
        {

        }

        protected override void ParseData(Dictionary<string, object> dic)
        {
            YxTools.TryGetValueWitheKey(dic, out _platformType, KeyType);
            YxTools.TryGetValueWitheKey(dic, out _costType, KeyCostType);
            YxTools.TryGetValueWitheKey(dic, out _rate, KeyRate);
        }
    }
}
