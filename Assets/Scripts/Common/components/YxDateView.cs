using System;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Common.components
{
    public class YxDateView : YxView
    { 
        [Tooltip("年")]
        public YxBaseLabelAdapter YearLabel;
        [Tooltip("月")]
        public YxBaseLabelAdapter MonthLabel;
        [Tooltip("日")]
        public YxBaseLabelAdapter DayLabel;
        [Tooltip("时")]
        public YxBaseLabelAdapter HourLabel;
        [Tooltip("分")]
        public YxBaseLabelAdapter MinuteLabel;
        [Tooltip("秒")]
        public YxBaseLabelAdapter SecondLabel;
        [Tooltip("显示今天")]
        public bool ShowToday;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            if (!(Data is DateTime))
            {
                if (!ShowToday)
                {
                    return;
                }
                Data = DateTime.Now;
            }
            var dateTime = (DateTime)Data;  
            if (YearLabel != null)
            {
                YearLabel.Text(dateTime.Year.ToString("D2"));
            }
            if (MonthLabel != null)
            {
                MonthLabel.Text(dateTime.Month.ToString("D2"));
            }
            if (DayLabel != null)
            {
                DayLabel.Text(dateTime.Day.ToString("D2"));
            }
            if (HourLabel != null)
            {
                HourLabel.Text(dateTime.Hour.ToString("D2"));
            }
            if (MinuteLabel != null)
            {
                MinuteLabel.Text(dateTime.Minute.ToString("D2"));
            }
            if (SecondLabel != null)
            {
                SecondLabel.Text(dateTime.Second.ToString("D2"));
            }
        }
    }
}
