using System.Collections.Generic;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     TrendManager.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-07-06
 *描述:        	走势管理，收到走势变化信息后，同步消息到各个走势区域，并预测下期走势,
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class TrendManager :BaseMono 
    {
        #region UI Param
        [Tooltip("局数")]
        public YxBaseLabelAdapter Round;
        [Tooltip("天王")]
        public YxBaseLabelAdapter TianNum;
        [Tooltip("庄")]
        public YxBaseLabelAdapter ZNum;
        [Tooltip("庄对")]
        public YxBaseLabelAdapter ZdNum;
        [Tooltip("闲")]
        public YxBaseLabelAdapter XNum;
        [Tooltip("闲对")]
        public YxBaseLabelAdapter XdNum;
        [Tooltip("和")]
        public YxBaseLabelAdapter HNum;
        #endregion

        #region Data Param
        #endregion

        #region Local Data
        /// <summary>
        /// 局数
        /// </summary>
        private int _roundNum;
        /// <summary>
        /// 天牌数量
        /// </summary>
        private int _tNum;
        /// <summary>
        /// 庄数量
        /// </summary>
        private int _zNum;
        /// <summary>
        /// 庄对数量
        /// </summary>
        private int _zDNum;
        /// <summary>
        /// 闲数量
        /// </summary>
        private int _xNum;
        /// <summary>
        /// 闲对数量
        /// </summary>
        private int _xDNum;
        /// <summary>
        /// 和数量
        /// </summary>
        private int _hNum;

        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<LocalRequest, List<TrendData>>(LocalRequest.Records, OnGetDatas);
            Facade.EventCenter.AddEventListeners<LocalRequest, TrendData>(LocalRequest.SingleRecord, OnSingleRecord);
        }

        #endregion

        #region Function
        /// <summary>
        /// 获得批量的回放数据
        /// </summary>
        /// <param name="datas"></param>
        protected void OnGetDatas(List<TrendData> datas)
        {
            _roundNum = 0;
            _zNum = 0;
            _zDNum = 0;
            _hNum = 0;
            _xNum = 0;
            _xDNum = 0;
            _tNum = 0;
            GetAccountData(datas);
            ShowAccount();
            Facade.EventCenter.DispatchEvent(LocalRequest.BeadRoadList, datas.ConvertAll(item=>item as IRecycleData));
            Facade.EventCenter.DispatchEvent(LocalRequest.BigRoadList, datas);
        }
        /// <summary>
        /// 单独的回放消息
        /// </summary>
        /// <param name="data"></param>
        private void OnSingleRecord(TrendData data)
        {
            SingleAccount(data);
            ShowAccount();
            Facade.EventCenter.DispatchEvent(EnumTrendType.BeadRoad, data as IRecycleData);
            Facade.EventCenter.DispatchEvent(EnumTrendType.BigRoad, data as IRecycleData);
        }

        /// <summary>
        /// 获取统计数据
        /// </summary>
        private void GetAccountData(List<TrendData> data)
        {
            foreach (var trendData in data)
            {
                SingleAccount(trendData);
            }
        }

        private void SingleAccount(TrendData trendData)
        {
            switch (trendData.Win)
            {
                case ConstantData.KeyBetBanker:
                    _zNum += 1;
                    break;
                case ConstantData.KeyBetEqual:
                    _hNum += 1;
                    break;
                case ConstantData.KeyBetLeisure:
                    _xNum += 1;
                    break;
            }
            _zDNum += trendData.ZhuangDui ? 1 : 0;
            _xDNum += trendData.XianDui ? 1 : 0;
            _tNum += trendData.XianTian ? 1 : 0;
            _tNum += trendData.ZhuangTian ? 1 : 0;
            _roundNum++;
        }

        /// <summary>
        /// 显示统计数据
        /// </summary>
        private void ShowAccount()
        {
            Round.TrySetComponentValue(_roundNum.ToString());
            TianNum.TrySetComponentValue(_tNum.ToString());
            ZNum.TrySetComponentValue(_zNum.ToString());
            ZdNum.TrySetComponentValue(_zDNum.ToString());
            XNum.TrySetComponentValue(_xNum.ToString());
            XdNum.TrySetComponentValue(_xDNum.ToString());
            HNum.TrySetComponentValue(_hNum.ToString());
        }
        #endregion
    }
}
