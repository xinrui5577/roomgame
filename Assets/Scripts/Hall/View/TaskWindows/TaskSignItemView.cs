using System;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Hall.View.TaskWindows
{
    /// <summary>
    /// 签到视图的item
    /// 数据  SignItemData
    /// </summary>
    public class TaskSignItemView : YxView
    {
        // 连续签到-------------------------
        /// <summary>
        /// 天数文本
        /// </summary>
        [Tooltip("天数 文本形式")]
        public UILabel DateLabel;
        /// <summary>
        /// 天数sprite 图片形式的时候用
        /// </summary>
        [Tooltip("天数sprite 图片形式")]
        public UISprite DateSprite;
        
        // 背景-------------------------
        /// <summary>
        /// 背景
        /// </summary>
        [Tooltip("背景sprite")]
        public UISprite BackGround;
       
        // 签到状态-------------------------
        /// <summary>
        /// 签到状态 文本格式
        /// </summary>
        [Tooltip("签到状态 文本形式")]
        public UILabel SignStateLabel;
        /// <summary>
        /// 签到状态 图片形式
        /// </summary>
        [Tooltip("签到状态 图片形式")]
        public UISprite SignStateSprite;
        // 金币奖励-------------------------
        /// <summary>
        /// 奖励
        /// </summary>
        [Tooltip("奖励")]
        public UILabel RewardLabel;
        [Tooltip("奖励金币adapter")]
        public NguiLabelAdapter RewardLabelAdapter;
        /// <summary>
        /// 奖励类型
        /// </summary>
        [Tooltip("奖励类型")]
        public UISprite RewardType;
        /// <summary>
        /// 奖品类型前缀
        /// </summary>
        [Tooltip("奖品类型的前缀")]
        public string RewardTypePrefix;
        /// <summary>
        /// 天数的格式
        /// </summary>
        [Tooltip("天数的格式")]
        public string CurDayFormat = "第{0}天";
        /// <summary>
        /// 签到按钮
        /// </summary>
        [Tooltip("签到按钮")]
        public UIButton SignBtn;
        /// <summary>
        /// 没有签到的时候是否需要隐藏
        /// </summary>
        [Tooltip("没有签到的时候是否需要隐藏")]
        public bool NoSignHideBtn = false;
        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as SignItemData;
            if (data == null) return;
            if (SignBtn != null)
            {
                SignBtn.isEnabled = data.CanSign;
                if(NoSignHideBtn) SignBtn.gameObject.SetActive(data.CanSign);
            }
            name = data.Day.ToString();
            if (DateSprite != null) DateSprite.spriteName = string.Format(CurDayFormat,data.Day);
            else if (DateLabel != null) DateLabel.text = string.Format(CurDayFormat, data.Day);
            if (BackGround != null) BackGround.spriteName = data.BgName;
            if (SignStateSprite != null)
            {
                SignStateSprite.gameObject.SetActive(!string.IsNullOrEmpty(data.SignState));
                SignStateSprite.spriteName = data.SignState;
                SignStateSprite.MakePixelPerfect();
            }
            else if (SignStateLabel != null) SignStateLabel.text = data.SignState;
            YxTools.TrySetComponentValue(RewardLabel, data.Reward.ToString());
            YxTools.TrySetComponentValue(RewardLabelAdapter, data.Reward,"1");
            if (RewardType != null) RewardType.spriteName = string.Format("{0}{1}",RewardTypePrefix,data.RewardType);
        }
    }

    [Serializable] 
    public class SignItemData
    {
        /// <summary>
        /// 签到日子
        /// </summary>
        public int Day; 
        /// <summary>
        /// 奖励
        /// </summary>
        public int Reward;
        /// <summary>
        /// 奖励
        /// </summary>
        public int RewardType = 1;
        /// <summary>
        /// 背景
        /// </summary>
        [NonSerialized]
        public string BgName;
        /// <summary>
        /// 签到状态
        /// </summary>
        [NonSerialized]
        public string SignState;
        /// <summary>
        /// 可签到
        /// </summary>
        [NonSerialized]
        public bool CanSign;
    }
}
