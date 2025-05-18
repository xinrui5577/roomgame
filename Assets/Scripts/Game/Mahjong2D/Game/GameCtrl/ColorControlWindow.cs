/** 
 *文件名称:     ColorControlWindow.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-05-10 
 *描述:         颜色控制面板
 *              功能描述：
 *                    1.桌面背景颜色自定义
 *                    2.麻将背景颜色样式选择
 *                    3.麻将牌面样式选择
 *历史记录: 
*/

using System;
using Assets.Scripts.Common.Windows;
using Assets.Scripts.Game.Mahjong2D.Common.UI;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Component.GameTable;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class ColorControlWindow : YxNguiWindow
    {
        [Tooltip("桌面颜色控制进度条")]
        public UISlider ColorSlider;
        [Tooltip("麻将背景颜色样式")]
        public UIToggle[] BgItems;
        [Tooltip("麻将牌面样式")]
        public UIToggle[] ValueItems;
        [Tooltip("桌面显示缩略图")]
        public UITexture ShowTexture;
        [Tooltip("颜色控制进度条")]
        public ColorSelect ColorChanger;
        /// <summary>
        /// 选择颜色
        /// </summary>
        private Color _selectColor;

        protected override void OnAwake()
        {
            base.OnAwake();
            if (ValueItems != null)
            {
                ValueItems[0].value = false;
                ValueItems[(int)MahjongItem.EnumMahjongValueType].value = true;
            }
            if (BgItems != null)
            {
                BgItems[0].value = false;
                BgItems[(int)MahjongItem.EnumMahJongColorType].value = true;
            }
            if (ColorSlider!=null)
            {
                ColorSlider.value = GameTable.Instance.ColorPercent;
            }
            ColorChanger.OnColorChange += OnTotalColroChange;
        }
        public void OnSelectValue(GameObject selectObj,bool state)
        {
            if (state)
            {
                EnumMahJongValueType type = (EnumMahJongValueType)Enum.Parse(typeof(EnumMahJongValueType), selectObj.name);
                PlayerPrefs.SetInt(ConstantData.KeyMahjongValueType,(int)type);
                MahjongItem.OnChangeValueType(type);
            }
        }

        public void OnSelectBg(GameObject selectObj, bool state)
        {
            if (state)
            {
                EnumMahJongColorType type = (EnumMahJongColorType) Enum.Parse(typeof (EnumMahJongColorType), selectObj.name);
                PlayerPrefs.SetInt(ConstantData.KeyMahjongBgType,(int)type);
                MahjongItem.OnChangeBgType(type);
            }
        
        }

        public void OnTotalColroChange(Color totalColor)
        {
            _selectColor = totalColor;
            GameTable.Instance.SetColor(totalColor);
        }

        public override void OnDestroy()
        {
            ColorChanger.OnColorChange += OnTotalColroChange;
            float value = ColorSlider.value;
            PlayerPrefs.SetFloat(ConstantData.KeyTableColorPercent,value);
            PlayerPrefs.SetString(ConstantData.KeyTableColor,GameTools.ParseColorToStr(_selectColor));
            GameTable.Instance.ColorPercent = value;
            base.OnDestroy();
        }
    }
}
