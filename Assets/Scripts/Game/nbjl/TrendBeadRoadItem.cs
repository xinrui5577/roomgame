using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.nbjl
{ /*===================================================
 *文件名称:     TrendBeadRoadItem.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-06-26
 *描述:         珠盘路走势Item
 *历史记录: 
=====================================================*/
    public class TrendBeadRoadItem : YxView 
    {
        #region UI Param
        [Tooltip("结果类型（庄闲和）")]
        public UISprite ResultType;
        [Tooltip("庄对")]
        public GameObject ZhuangDui;
        [Tooltip("闲对")]
        public GameObject XianDui;
        #endregion

        #region Data Param
        #endregion

        #region Local Data
        #endregion

        #region Life Cycle

        #endregion

        #region Function

        protected override void OnFreshView()
        {
            base.OnFreshView();
            var data = Data as TrendData;
            if (data != null)
            {
                var trendData = data;
                OnFreshBeadData(trendData);
            }
        }

        private void OnFreshBeadData(TrendData data)
        {
            ZhuangDui.TrySetComponentValue(data.ZhuangDui);
            XianDui.TrySetComponentValue(data.XianDui);
            ResultType.TrySetComponentValue(data.Win);
        }
        #endregion
    }

    /// <summary>
    /// 走势类型
    /// </summary>
    public enum EnumTrendType
    {
        /// <summary>
        /// 大路
        /// </summary>
        BigRoad,
        /// <summary>
        /// 大眼仔路
        /// </summary>
        BigEyeRoad,
        /// <summary>
        /// 小路
        /// </summary>
        SmallRoad,
        /// <summary>
        /// 曱甴路
        /// </summary>
        RoachRoad,
        /// <summary>
        /// 珠盘路
        /// </summary>
        BeadRoad,
    }

    /// <summary>
    /// 走势数据
    /// </summary>
    public class TrendData: IRecycleData
    {
        /// <summary>
        /// 庄对子
        /// </summary>
        public bool ZhuangDui { get; set; }
        /// <summary>
        /// 闲对子
        /// </summary>
        public bool XianDui { get; set; }
        /// <summary>
        /// 庄天王
        /// </summary>
        public bool ZhuangTian {get; set; }
        /// <summary>
        /// 闲天王
        /// </summary>
        public bool XianTian { get; set; }
        /// <summary>
        /// 胜负结果
        /// </summary>
        public string Win { get; set; }

        public int ResultStates
        {
            get
            {
                return GetResultStates();
            }
        }

        public TrendData(ISFSObject data)
        {
            Win= data.GetUtfString(ConstantData.KeyWinCount);
            ZhuangDui= data.GetBool(ConstantData.KeyBetBankerDouble);
            XianDui= data.GetBool(ConstantData.KeyBetLeisureDouble);
            ZhuangTian = data.GetBool(ConstantData.KeyBetBankerKing);
            XianTian = data.GetBool(ConstantData.KeyBetLeisureKing);
        }

        /// <summary>
        /// 返回结果状态
        /// </summary>
        /// <returns></returns>
        private int GetResultStates()
        {
            var state = 0;
            switch (Win)
            {
                case ConstantData.KeyBetBanker:
                    state |= (int)TrendBit.Zhuang;
                    break;
                case ConstantData.KeyBetLeisure:
                    state |= (int)TrendBit.Xian;
                    break;
                case ConstantData.KeyBetEqual:
                    state |= (int)TrendBit.He;
                    break;
            }
            state|=ZhuangDui? (int)TrendBit.ZhuangDui:0;
            state |= XianDui ? (int)TrendBit.XianDui : 0;
            state |= ZhuangTian ? (int)TrendBit.ZhuangTian : 0;
            state |= XianTian ? (int)TrendBit.XianTian : 0;
            return state;
        }

        public TrendData SetTrendData(int status)
        {
            ZhuangDui = (status & (int)TrendBit.ZhuangDui) != 0;
            XianDui = (status & (int)TrendBit.XianDui) != 0;
            ZhuangTian = (status & (int)TrendBit.ZhuangTian) != 0;
            XianTian = (status & (int)TrendBit.XianTian) != 0;
            if ((status & (int)TrendBit.Zhuang) != 0)
            {
                Win = ConstantData.KeyBetBanker;
            }
            else
                if ((status & (int)TrendBit.Xian) != 0)
            {
                Win = ConstantData.KeyBetLeisure;
            }
            else
            {
                Win = ConstantData.KeyBetEqual;
            }
            return this;
        }

        public TrendData()
        {

        }
    }
    /// <summary>
    /// 走势位值
    /// </summary>
    public enum TrendBit
    {
        Zhuang=1,
        Xian=2,
        He=4,
        ZhuangDui=8,
        XianDui=16,
        ZhuangTian=32,
        XianTian=64,
    }

}