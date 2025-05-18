using System.Collections;
using UnityEngine;
using YxFramwork.Enums;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.Windows.TabPages
{
    [RequireComponent(typeof(UIToggle), typeof(UIToggledObjects))]
    public class YxTabItem : YxView
    {
        [Tooltip("标签默认的名字，如果用图片区分不同的标签，请设置null")]
        public UILabel NameLabel;
        [Tooltip("标签按下的名字，用来区分不同的样式")]
        public UILabel DownNameLabel;
        [Tooltip("标签没有点击的图片，如果NameLabel为空，则会使用此属性来更改")]
        public UISprite TableUpSprite;
        [Tooltip("标签点击的的图片，如果NameLabel为空，则会使用此属性来更改")]
        public UISprite TableDownSprite;
        /// <summary>
        /// 标签sprite的前缀
        /// </summary>
        [Tooltip("标签sprite的格式")]
        public string TableUpSpriteFormat = "{0}";
        public string TableDownSpriteFormat = "{0}";
        private UIToggle _toggle;
        private UIToggledObjects _toggledObj;

        protected override void OnAwake()
        {
            base.OnAwake();
            _toggledObj = GetComponent<UIToggledObjects>();
        }

        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (!(Data is TabData)) return;
            var data = (TabData) Data;
            var view = data.View; 
            if (view != null && _toggledObj!=null)
            { 
                var activates = _toggledObj.activate;
                activates.Clear();
                activates.Add(view.gameObject);
            }
            OnSelected(data.StarttingState);
            SetName(data); 
        }
        private void SetName(TabData data)
        {
            if (NameLabel != null)
            {
                var tabelName = data.Name;
                if (!string.IsNullOrEmpty(tabelName))
                {
                    NameLabel.text = tabelName;
                    if (DownNameLabel != null)
                    {
                        DownNameLabel.text = tabelName;
                    }
                }
                return;
            }
            if (TableUpSprite != null) TableUpSprite.spriteName = string.Format(TableUpSpriteFormat, data.UpStateName);
            if (TableDownSprite != null) TableDownSprite.spriteName = string.Format(TableDownSpriteFormat, data.DownStateName);
        }
        /// <summary>
        /// 选择
        /// </summary>
        public void OnSelected(bool isToggle = true)
        {
            GetToggle().value = isToggle;
        }

        public void StarttingState(bool state)
        {
            GetToggle().startsActive = state;
        }

        public UIToggle GetToggle()
        {
            return _toggle ?? (_toggle = GetComponent<UIToggle>());
        }
    }
}
