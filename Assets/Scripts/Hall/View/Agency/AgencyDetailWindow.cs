/** 
 *文件名称:     AgencyDetail.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2018-05-01 
 *描述:    
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Common.components;
using Assets.Scripts.Common.Utils;
using Assets.Scripts.Common.Windows;
using UnityEngine;

namespace Assets.Scripts.Hall.View.Agency
{
    public class AgencyDetailWindow : YxNguiWindow
    {
        #region UI Param
        [Tooltip("代理名称")]
        public UILabel AgencyName;
        [Tooltip("代理联系方式")]
        public UILabel AgencyContent;

        #endregion

        #region Data Param

        #endregion

        #region Local Data

        /// <summary>
        /// Key 键值对的关键字
        /// </summary>
        private const string KeyValueKey = "key";

        private const string KeyValue = "value";
        #endregion

        #region Life Cycle

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var dic = GetData < Dictionary<string, object>>();
            if (dic == null) { return; }
            
            string key;
            string value;
            dic.TryGetValueWitheKey(out key, "key");
            dic.TryGetValueWitheKey(out value, "value");
            Data = new YxKeyValueData//将 dict 数据转换为 keyvalueData，并赋值给原始 Data
            {
                Key = key,
                Value = value
            };
            AgencyName.TrySetComponentValue(key);
            AgencyContent.TrySetComponentValue(value);
        }

        #endregion

        #region Function 

        #endregion

    }
}
