/** 
 *文件名称:     RuleItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-06-02 
 *描述:         规则的perfab
 *历史记录: 
*/

using System;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use Assets.Scripts.Common.components.YxKeyValueView")]
    public class KeyValueView : YxView
    {
        /// <summary>
        /// key 文本
        /// </summary>
        public UILabel KeyLabel;
        public UILabel ValueLabel;
        /// <summary>
        /// 组的分割线
        /// </summary>
        public UISprite Line;
        /// <summary>
        /// 分割表示
        /// </summary>
        public char SpliteFlag = ':';

        protected override void OnFreshView()
        {
            var kvData = Data as KeyValueData;
            if (kvData == null) return;
            KeyLabel.text = string.Format("{0}{1}", kvData.KeyString, SpliteFlag);
            var value = kvData.ValueString;
            ValueLabel.text = value;
        }

        public int ItemHeight
        {
            get { return ValueLabel.height; }
        }

        public void SaveValue()
        {
            var kvData = Data as KeyValueData;
            if (kvData == null) return;
            Facade.Instance<YxGameTools>().PasteBoard = kvData.ValueString;
        }
    }
    [Obsolete("Use Assets.Scripts.Common.components.YxKeyValueData")]
    public class KeyValueData
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string KeyString;
        /// <summary>
        /// 内容
        /// </summary>
        public string ValueString;
    }
}