using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Common.Utils;
using UnityEngine;

namespace Assets.Scripts.Hall.View
{
    /// <summary>
    /// 奖励物品与对应的Icon
    /// </summary>
    public class RewardItem : MonoBehaviour
    {
        /// <summary>
        /// 显示的图标
        /// </summary>
        public UISprite ShowIcon;
        /// <summary>
        /// 显示的数量
        /// </summary>
        public UILabel ShowNum;
        [Tooltip("显示数量adapter")]
        public NguiLabelAdapter ShowNumAdapter;
        /// <summary>
        /// 奖励结构
        /// </summary>
        public RewardItemType LayoutType=RewardItemType.Right;


        public void Init(string iconName, int showNum)
        {
            if (ShowIcon)
            {
                ShowIcon.spriteName = iconName;
                ShowIcon.MakePixelPerfect();
            }
            ShowNum.TrySetComponentValue(string.Format("x{0}", showNum));
            ShowNumAdapter.TrySetComponentValue(showNum, iconName, "x{0}");
        }
    }
    /// <summary>
    /// 奖励结构样式(图标与数量结构)
    /// </summary>
    public enum RewardItemType
    {
        Right,                  //默认结构，左右结构
        BottomType,             //上下结构
    }
}
