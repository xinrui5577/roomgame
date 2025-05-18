using System;
using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;

/*===================================================
 *文件名称:     PludoColorItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-17
 *描述:        	颜色item类
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoColorItem : PludoFreshView, IColorItem
    {
        #region UI Param
        [Tooltip("颜色图与UiTypes对齐")]
        public List<UISprite> ColorSprites=new List<UISprite>();
        #endregion

        #region Data Param

        [Tooltip("UI类型与ColorSprites对齐")]
        public List<ColorItemUiType> UiTypes = new List<ColorItemUiType>();
        /// <summary>
        /// 颜色
        /// </summary>
        public int Color { get { return CurColorData.ItemColor; } }
        #endregion

        #region Local Data

        protected ColorItemData CurColorData;
        #endregion

        #region Life Cycle

        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            CurColorData = Data as ColorItemData;
            if (CurColorData!=null)
            {
                OnColorItemFresh();
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        protected virtual void OnColorItemFresh()
        {
            SetColorItems(ColorSprites, UiTypes);
        }

        public string GetSpriteName(ItemColor color, ColorItemUiType uiType, string format = ConstantData.DefColorFormat)
        {
            return string.Format(format,color,uiType);
        }

        public virtual void SetColorItem(UISprite colorSprite, string spriteName)
        {
            colorSprite.TrySetComponentValue(spriteName);
        }

        public void SetColorItems(List<UISprite> colorSprites, List<ColorItemUiType> uiTypes)
        {
            var count = Math.Min(colorSprites.Count, uiTypes.Count);
            int curColor=(int)ItemColor.Blue;
            if (CurColorData!=null)
            {
                curColor = CurColorData.ItemColor;
            }
            for (int i = 0; i < count; i++)
            {
                var sprite = colorSprites[i];
                SetColorItem(sprite, GetSpriteName((ItemColor)curColor, uiTypes[i]));
                sprite.MakePixelPerfect();
            }

        }

        #endregion

        #region Function

        #endregion
    }

    /// <summary>
    /// 颜色数据类
    /// </summary>
    public class ColorItemData: IColorData
    {

        protected int CurColor;

        public int ItemColor
        {
            get
            {
                return CurColor;
            }
        }


        public virtual void SetColor(int color)
        {
            CurColor =color;
        }

        public ColorItemData(ISFSObject data)
        {
            ParseData(data);
        }

        public ColorItemData(int color)
        {
            SetColor(color);
        }

        public ColorItemData()
        {

        }


        protected virtual void ParseData(ISFSObject data)
        {

        }
    }
}
