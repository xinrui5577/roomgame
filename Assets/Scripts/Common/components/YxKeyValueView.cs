using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Common.Utils;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.components
{
    /// <inheritdoc />
    /// <summary>
    /// key和value的视图
    /// </summary>
    public class YxKeyValueView : YxView
    {
        [Tooltip("key的Label")]
        public YxBaseLabelAdapter KeyLable;
        [Tooltip("value的Label")]
        public YxBaseLabelAdapter ValueLable;
        [Tooltip("value的图标")]
        public YxBaseTextureAdapter Icon;
        [Tooltip("value的图标")]
        public YxBaseWidgetAdapter Widget;
        [Tooltip("分割表示")]
        public char SpliteFlag = ':';

        protected override void OnAwake()
        {
            base.OnAwake();
            if (Widget == null)
            {
                Widget = ValueLable;
            }
        }

        protected override void OnFreshView()
        {
            var data = GetData<YxKeyValueData>();
            if (data == null) { return;}
            SetKeyLabel(data.Key);
            SetValueLabel(data.Value);
            SetIcon(data.IconUrl);
        }

        /// <summary>
        /// 设置key
        /// </summary>
        protected virtual void SetKeyLabel(string key)
        {
            if (KeyLable == null) return;
            string.Format("{0}{1}", key, SpliteFlag);
            KeyLable.Text(key);
        }

        /// <summary>
        /// 设置value
        /// </summary>
        protected virtual void SetValueLabel(string value)
        {
            if (ValueLable == null) return;
            ValueLable.Text(value);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        protected virtual void SetIcon(string iconUrl)
        {
            YxAdapterUtile.SetTexture(Icon, iconUrl);
        }

        public int GetHeight()
        {
            if (Widget == null)
            {
                if (ValueLable == null) return 0;

                return ValueLable.Height;
            }
            return Widget.Height;
        }
    }

    public class YxKeyValueData
    {
        public string Key;
        public string Value;
        public string IconUrl;
    }
}
