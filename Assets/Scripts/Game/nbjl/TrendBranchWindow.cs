using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Utils;
using UnityEngine;
using YxFramwork.Framework.Core;

/*===================================================
 *文件名称:     TrendBranchWindow.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-07-06
 *描述:        	衍生路
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class TrendBranchWindow : TrendWindow
    {
        [Tooltip("庄家下期走势")]
        public TrendNormalItem BankerNextTrend;
        [Tooltip("闲家下期走势")]
        public TrendNormalItem LeisureNextTrend;
        /// <summary>
        /// 行数
        /// </summary>
        public int RowNum=6;
        /// <summary>
        /// 大路数据
        /// </summary>
        private RoadNodeTable _datas;
        /// <summary>
        /// 衍生数据
        /// </summary>
        private RoadNodeTable _branchDatas;

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<LocalRequest, List<TrendData>>(LocalRequest.BigRoadList, OnGetMainDatas);
            if (LeisureNextTrend)
            {
                LeisureNextTrend.ResultType.spriteName = "9999";
            }
            if (BankerNextTrend)
            {
                BankerNextTrend.ResultType.spriteName = "9999";
            }
        }

        /// <summary>
        /// 同步大路数据
        /// </summary>
        protected void OnGetMainDatas(List<TrendData> datas)
        {
            _datas = new RoadNodeTable(datas, 6);
            switch (Type)
            {
                case EnumTrendType.BigRoad:
                    _branchDatas = _datas;
                    break;
                default:
                    _branchDatas=new RoadNodeTable(_datas,Type,6);
                    break;
            }
            CacheData = _branchDatas.Nodes.ConvertAll(item=>item as IRecycleData);
            AddChildrenToShow();
            GetPridict();
        }
        protected override void OnSingleRecord(IRecycleData data)
        {
            var itemData = data as TrendData;
            if (_datas==null)
            {
                _datas = new RoadNodeTable(new List<TrendData>(), 6);
            }
            if (_branchDatas==null)
            {
                switch (Type)
                {
                    case EnumTrendType.BigRoad:
                        _branchDatas = _datas;
                        break;
                    default:
                        _branchDatas = new RoadNodeTable(_datas, Type, 6);
                        break;
                }
            }
            if (itemData != null)
            {
                if (!_datas.AddSingleItem(itemData))
                {
                    if(CacheData.Count>0)
                    {
                        var cachelast = CacheData.Last() as RoadNode;
                        var getLast = _datas.Nodes.Last();
                        if (cachelast != null && (cachelast.X == getLast.X && cachelast.Y == getLast.Y))
                        {
                            var lastView = CacheViews.Last();
                            CacheData[CacheData.Count - 1] = getLast;
                            lastView.UpdateView(getLast);
                        }
                    }
                }
                else
                {
                    switch (Type)
                    {
                        case EnumTrendType.BigRoad:
                            _branchDatas = _datas;
                            break;
                        default:
                            _branchDatas.AddSingleItem(_datas.Nodes.Last(), _datas);
                            break;
                    }
                    if (_branchDatas.Nodes.Count>0)
                    {
                        var lastNode = _branchDatas.Nodes.Last();
                        AddChildToShow(lastNode);
                    }
                }
            }
            GetPridict();
        }

        /// <summary>
        /// 下期预测
        /// </summary>
        private void GetPridict()
        {
            TrendData banker=new TrendData().SetTrendData((int)TrendBit.Zhuang);
            TrendData leisure = new TrendData().SetTrendData((int)TrendBit.Xian);
            if (BankerNextTrend)
            {
                SetTest(banker, BankerNextTrend);
            }
            if (LeisureNextTrend)
            {
                SetTest(leisure, LeisureNextTrend);
            }
        }

        private void SetTest(TrendData data,TrendNormalItem item)
        {
            RoadNodeTable testBankerRoad = new RoadNodeTable(_datas);
            testBankerRoad.AddSingleItem(data);
            RoadNodeTable viewTable=new RoadNodeTable(testBankerRoad,Type,6);
            if (testBankerRoad.Nodes.Count>0)
            {
                switch (Type)
                {
                    case EnumTrendType.BigRoad:
                        break;
                    default:
                        viewTable.AddSingleItem(testBankerRoad.Nodes.Last(), testBankerRoad);
                        break;
                }
            }
            if (viewTable.Nodes.Count > 0)
            {
                var lastNode = viewTable.Nodes.Last();
                var isRed = lastNode.IsRed ? ConstantData.KeyBetBanker : ConstantData.KeyBetLeisure;
                item.ResultType.TrySetComponentValue(string.Format(item.SpriteFormat, isRed));
            }
            else
            {
                item.ResultType.spriteName = "9999";
            }
        }
    }
}
