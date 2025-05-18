using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common.Utils;

/*===================================================
 *文件名称:     PludoMapItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-12-17
 *描述:        	地图item
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.pludo.View
{
    public class PludoMapItem : PludoColorItem
    {
        #region UI Param
        #endregion

        #region Data Param

        /// <summary>
        /// 数据ID
        /// </summary>
        public int DataId;
        /// <summary>
        /// 类型
        /// </summary>
        public EnumMapItemType EnumMapType;

        /// <summary>
        /// 飞机位置
        /// </summary>
        public Vector3 MapPos
        {
            get { return transform.localPosition; }
        }

        #endregion

        #region Local Data
        /// <summary>
        /// 地图数据
        /// </summary>
        private PludoMapItemData _curData;
        #endregion

        #region Life Cycle


        protected override void OnFreshViewWithData()
        {
            base.OnFreshViewWithData();
            _curData = Data as PludoMapItemData;
            if (_curData != null)
            {
                DataId = _curData.MapDataId;
                Id = _curData.MapDataId.ToString();
                name = Id;
                EnumMapType =(EnumMapItemType) _curData.MapItemType;
            }
        }

        #endregion

        #region Function
        /// <summary>
        /// 获取飞机位置
        /// </summary>
        /// <param name="planeIndex"></param>
        /// <returns></returns>
        public virtual Vector3 GetPlanePos(int planeIndex=0)
        {
            return MapPos;
        }

        #endregion
    }

    public class PludoMapItemData : ColorItemData
    {
        /// <summary>
        /// 地图数据ID
        /// </summary>
        private int _dataId;
        /// <summary>
        /// 颜色类型
        /// </summary>
        private int _itemType;


        public int MapItemType
        {
            get
            {
                return _itemType;
            }
        }

        public int MapDataId
        {
            get
            {
                return _dataId;
            }
        }

        public bool IsBegin
        {
            get
            {
                return CheckMapState(EnumMapItemType.Begin);
            }
        }


        protected override void ParseData(ISFSObject data)
        {
            base.ParseData(data);
            SfsHelper.Parse(data, ConstantData.KeyColor,ref CurColor);
            SfsHelper.Parse(data, ConstantData.KeyMapId, ref _dataId);
            SfsHelper.Parse(data, ConstantData.KeyMapType, ref _itemType);
        }

        public PludoMapItemData(ISFSObject data):base(data)
        {

        }

        public PludoMapItemData(int color, int dataId,int itemType) : base(color)
        {
            _dataId = dataId;
            _itemType = itemType;
        }

        /// <summary>
        /// 校验地图类型
        /// </summary>
        /// <param name="checkType">检测类型</param>
        /// <returns></returns>
        public bool CheckMapState(EnumMapItemType checkType)
        {
            return (_itemType & (int) checkType) == (int) checkType;
        }
    }

    /// <summary>
    /// 地图类型
    /// </summary>
    public enum EnumMapItemType
    {
        Normal=1,                             //常规地图
        Begin=2,                              //地图起始位置
        FlyStart=4,                           //飞行起始位置
        FlyTo =8,                             //飞行停止点
        End =16,                              //常规地图最终位置
        Run =32,                              //安全区
        ReadyArea=64,                         //准备区（包括起飞点与基地部分组成）
        BeginBlue =32768,                     //蓝色开始
        BeginRed = 65536,                     //红色开始
        BeginYellow =131072,                  //黄色开始
        BeginGreen = 262144,                  //绿色开始

    }
}
