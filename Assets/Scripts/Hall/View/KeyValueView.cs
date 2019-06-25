/** 
 *文件名称:     RuleItem.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-06-02 
 *描述:         规则的perfab
 *历史记录: 
*/

using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 
    /// </summary>
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
            if (Data == null) return;
            var kvData = Data as KeyValueData;
            if (kvData == null) return;
            KeyLabel.text = kvData.KeyString;
            var value = kvData.ValueString;
            ValueLabel.text = value;
        }

        public int ItemHeight
        {
            get { return ValueLabel.height; }
        }
    }

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