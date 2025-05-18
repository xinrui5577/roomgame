/*===================================================
 *文件名称:     IColor.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-17
 *描述:        	颜色接口，包括数据接口与UI接口
 *历史记录: 
=====================================================*/


using System.Collections.Generic;

namespace Assets.Scripts.Game.pludo
{
    /// <summary>
    /// 颜色数据
    /// </summary>
    public interface IColorData
    {
        /// <summary>
        /// 设置颜色参数
        /// </summary>
        /// <param name="color"></param>
        void SetColor(int color);
    }

    /// <summary>
    /// 颜色Item
    /// </summary>
    public interface IColorItem
    {
        /// <summary>
        /// 获取图片内容
        /// </summary>
        /// <param name="format">格式</param>
        /// <param name="color">颜色</param>
        /// <param name="uiType">ui参数</param>
        /// <returns></returns>
        string GetSpriteName(ItemColor color, ColorItemUiType uiType, string format=ConstantData.DefColorFormat);
        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="colorSprite">处理颜色sprite</param>
        /// <param name="spriteName">value</param>
        void SetColorItem(UISprite colorSprite,string spriteName);
        /// <summary>
        /// 图片集合
        /// </summary>
        /// <param name="colorSprites">需要受颜色控制的图片</param>
        /// <param name="uiTypes">显示类型</param>
        void SetColorItems(List<UISprite> colorSprites, List<ColorItemUiType> uiTypes);
    }

    /// <summary>
    /// 颜色
    /// </summary>
    public enum ItemColor
    {
        Blue,
        Red,
        Yellow,
        Green,
    }
    /// <summary>
    /// 地图UI类型
    /// </summary>
    public enum ColorItemUiType
    {
        RoadRectangle,                        //矩形
        RoadSquare,                           //正方形
        RoadTriangle,                         //三角形
        RoadBegin,                            //起始点
        RoadFinal,                            //终点
        RoadStar,                             //完成状态（星星）
        HeadFrame,                            //头像框
        PlaneFire,                            //飞机火焰
        PlaneNormal,                          //飞机常规状态
        PlaneSelect,                          //飞机选中状态
        UserInfoFrame,                        //玩家信息框
        ResultTitle,                          //结果Titele
        RoadPlaneStay,                        //飞机停留状态
    }
}
