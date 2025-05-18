using System.Globalization;
using Assets.Scripts.Common.Models.CreateRoomRules;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Common.UI
{
    /// <summary>
    /// 
    /// </summary>
    public class NguiCRSlider : NguiCRComponent
    {
        [Tooltip("Ngui的slider")]
        public UISlider Slider;
        [Tooltip("描述label")]
        public UILabel FileLabel;
        [Tooltip("最小label")]
        public UILabel MinLabel;
        [Tooltip("最大label")]
        public UILabel MaxLabel;
        [Tooltip("当前值label")]
        public UILabel ValueLabel;
        [Tooltip("每个分段点的样式")]
        public Transform PointTs;

        protected override void OnStart()
        {
            base.OnStart();
            Slider.onChange.Add(new EventDelegate(ChangeValue));
        }

        protected string Max;
        protected string Min;
        protected override void OnFreshCRCView(ItemData itemData)
        {
            var range = itemData.Range;//如果格式不正确，直接让他报错
            var rangeLen = range.Length;
            Min = range[0];
            Max = range[rangeLen - 1]; 
            if (FileLabel != null)
            {
                FileLabel.text = itemData.Name;
            } 
            SetLabelValue(MinLabel, Min, itemData);
            SetLabelValue(MaxLabel, Max, itemData);
            InitCurValue(itemData);
            UpdateWidget(itemData.Width, itemData.Height);
        }

        private void InitCurValue(ItemData itemData)
        {
            float slider;
            int step;
            var value = itemData.Value;
            float outValue;
            float.TryParse(value, out outValue);
            var range = itemData.Range;
            var rangeLen = range.Length;
            if (rangeLen > 2)
            {
                slider = outValue / (rangeLen - 1);
                var cur = range[(int)outValue];
                SetLabelValue(ValueLabel, cur, itemData);
                step = rangeLen;
            }
            else
            {
                float max, min;
                float.TryParse(Min, out min);
                float.TryParse(Max, out max);
                slider = (outValue - min) / (max - min);
                SetLabelValue(ValueLabel, outValue.ToString(CultureInfo.InvariantCulture), itemData);
                step = 0;
            }
            Slider.value = slider;
            Slider.numberOfSteps = step;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="itemData"></param>
        private void SetLabelValue(UILabel label ,string value, ItemData itemData)
        {
            if (label != null)
            {
                if (itemData.NeedConvert)
                {
                    long val;
                    long.TryParse(value, out val);
                    var v = YxUtiles.GetShowNumber(val, itemData.FloatValidity);
                    label.text = itemData.SliderType == YxECrSliderType.Float ? v.ToString("N") : ((int)v).ToString("N0");
                }
                else
                {
                    label.text = value.ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        public void ChangeValue()
        {
            var data = GetData<ItemData>();
            if (data == null) return;
            var sliderValue = Slider.value;
            var range = data.Range;
            var stepValues = range.Length;
            var curValue = stepValues > 2 ? ChangeStepsValue(data, sliderValue, range):ChangeNoStepsValue(data, sliderValue, range);
            SaveValue(data, curValue);
        }

        public string ChangeNoStepsValue(ItemData data,float value, string[] steps)
        {
            float max, min;
            float.TryParse(Min, out min);
            float.TryParse(Max, out max);
            var curValue = min + (max - min) * value;
            var showValue = "";

            if (data.NeedConvert) //压缩格式
            {
                var longValue = (long) curValue;
                switch (data.SliderType)
                {
                    case YxECrSliderType.Integer:
                        showValue = ((long)YxUtiles.GetShowNumber(longValue, data.FloatValidity)).ToString(CultureInfo.InvariantCulture);
                        var rate = App.ShowGoldRate;
                        curValue = ((long)((float)longValue / rate)) * rate;
                        break;
                    case YxECrSliderType.Float:
                        showValue = YxUtiles.GetShowNumber(longValue, data.FloatValidity).ToString(CultureInfo.InvariantCulture);
                        curValue = longValue;
                        break;
                }
            }
            else
            {
                switch (data.SliderType)
                {
                    case YxECrSliderType.Integer:
                        var longValue = (long)curValue;
                        showValue = longValue.ToString();
                        curValue = longValue;
                        break;
                    case YxECrSliderType.Float:
                        var validity = Mathf.Pow(10, data.FloatValidity);
                        curValue = (long) (curValue * validity) / validity;
                        showValue = curValue.ToString(CultureInfo.InvariantCulture);
                        break;
                }
            }
            SetCurrValueLable(showValue);
            var curValueStr = curValue.ToString(CultureInfo.InvariantCulture);
            data.Value = curValueStr;
            return curValueStr;
        }
        public string ChangeStepsValue(ItemData data,float value,string[] steps)
        { 
            var step = steps.Length;
            var fff = (step - 1) * value;
            var index = Mathf.CeilToInt(fff);
            var curValue = steps[index];
            data.Value = index.ToString();
            SetCurrValueLable(curValue);
            return index.ToString();
        }

        /// <summary>
        /// 设置当前的值
        /// </summary>
        /// <param name="curValue"></param>
        private void SetCurrValueLable(string curValue)
        {
            if (ValueLabel==null)
            {
                return;
            }
            ValueLabel.text = curValue;
        }

        protected void SaveValue(ItemData data,string value)
        {
            var info = data.Parent;
            CreateRoomRuleInfo.SaveItemValue(info.CurTabId, data.Id, info.GameKey, value);
        }
    }
}
