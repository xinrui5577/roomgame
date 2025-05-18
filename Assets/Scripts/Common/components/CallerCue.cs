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
        /// <summary>
        /// 任务
        /// </summary>
        Task = 1,
        /// <summary>
        /// 银行
        /// </summary>
        Bank = 2,
        /// <summary>
        /// 好友
        /// </summary>
        Friend = 4,
        /// <summary>
        /// 公告
        /// </summary>
        Notice = 8,
        /// <summary>
        /// 商店
        /// </summary>
        Shop = 16,
        /// <summary>
        /// 茶馆列表
        /// </summary>
        CaffAskList = 32,
        /// <summary>
        /// 茶馆账单
        /// </summary>
        CaffBill = 64,
        /// <summary>
        /// 比赛
        /// </summary>
        Match=128,
        /// <summary>
        /// 邮件
        /// </summary>
        Mail=256,
        
    }

    public enum CallerCueType
    {
        /// <summary>
        /// 大厅
        /// </summary>
        hall,
        /// <summary>
        /// 任务
        /// </summary>
        task
    }
}
