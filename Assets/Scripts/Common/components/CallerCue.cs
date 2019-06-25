using System;
using UnityEngine;
using YxFramwork.Common.PropertyAttributes;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Common.components
{
    /// <summary>
    /// 消息提醒
    /// </summary>
    public class CallerCue : BaseOnOff
    {
        [FlagEnum(typeof(CallerCueId)), Tooltip("id")]
        public CallerCueId Id;
        [Tooltip("类型")]
        public CallerCueType BaseType;
        /// <summary>
        /// 提醒图标
        /// </summary>
        [Tooltip("提醒图标")]
        public GameObject Cue;  

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public override void SetState(bool state)
        {
            if (Cue == null) return;
            Cue.SetActive(state);
        }

        public override int GetId()
        {
            return (int)Id;
        }

        public override string GetBaseType()
        {
            return BaseType.ToString();
        }
    }

    [Flags]
    public enum CallerCueId
    {
        Task = 1,
        Bank = 2,
        Friend = 4,
        Notice = 8,
        Shop = 16,
        CaffAskList = 32,
        CaffBill = 64
    }

    public enum CallerCueType
    {
        hall
    }
}
