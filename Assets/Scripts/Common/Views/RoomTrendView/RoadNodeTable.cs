using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Interface;
using Assets.Scripts.Common.Models;
using Assets.Scripts.Common.Views.RoomTrendView.RoomTrendCfgs;
using UnityEngine;

namespace Assets.Scripts.Common.Views.RoomTrendView
{
    /// <summary>
    /// 节点集合（表）
    /// </summary>
    public class RoadNodeTable
    {
        /// <summary>
        /// 节点集合
        /// </summary>
        public List<RoadNode> Nodes;
        /// <summary>
        /// 每个直落的数量
        /// </summary>
        public List<int> LineCount;
        /// <summary>
        /// 节点字典
        /// </summary>
        public Dictionary<Vector2, RoadNode> NodeDic;
        /// <summary>
        /// 当前列的x轴起始位置
        /// </summary>
        public int StartX;
        /// <summary>
        /// 当前X坐标
        /// </summary>
        public int CurrentX;
        /// <summary>
        /// 当前Y
        /// </summary>
        public int CurrentY;
        /// <summary>
        /// 当前是否为红
        /// </summary>
        public bool CurrentRed;
        /// <summary>
        /// 行高
        /// </summary>
        public int RolMaxHeight;
        /// <summary>
        /// 类型步数
        /// </summary>
        public int Nstep;
        /// <summary>
        /// 第一个节点
        /// </summary>
        public bool First;

        /// <summary>
        /// 大路图
        /// </summary>
        /// <param name="result"></param>
        /// <param name="lineHeight"></param>
        /// <returns></returns>
        public RoadNodeTable(List<ITrendReciveData> result, int lineHeight)
        {
            CurrentX = 1;
            CurrentY = 1;
            StartX = 1;
            RolMaxHeight = lineHeight;
            Nodes = new List<RoadNode>();
            NodeDic = new Dictionary<Vector2, RoadNode>();
            LineCount = new List<int> { 0 };
            First = true;
            foreach (var it in result)
            {
                AddSingleItem(it);
            }
        }

        public RoadNodeTable(RoadNodeTable copy)
        {
            CurrentX = copy.CurrentX;
            CurrentY = copy.CurrentY;
            StartX = copy.StartX;
            RolMaxHeight = copy.RolMaxHeight;
            Nodes = copy.Nodes.ToList();
            NodeDic = new Dictionary<Vector2, RoadNode>(copy.NodeDic);
            LineCount = copy.LineCount.ToList();
            First = copy.First;
        }

        /// <summary>
        /// 大路添加单条数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool AddSingleItem(ITrendReciveData values)
        {
            bool returnState = false;
            foreach (var value in values.GetResultArea())
            {
                int val = -1;

                if (int.TryParse(value, out val))
                {
                    returnState = InsertNode(int.Parse(value) == 0);
                }
                else
                {
                    switch (value)
                    {
                        case "x":
                            returnState = InsertNode(true);
                            break;
                        case "z":
                            returnState = InsertNode(false);
                            break;
                        case "h":
                            int nLast = Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                            if (Nodes.Count != 0)
                            {
                                Nodes[nLast].DrawCount++;
                            }
                            break;
                    }
                }
                return returnState;
            }
//            return false;
//            if (value is RbWarTrendData)//红黑大战
//            {
//                var item = value as RbWarTrendData;
//                bool returnState = InsertNode(item.Area == 0);
//                return returnState;
//            }
            //            else if (value is NbjlTrendData)//新百家乐
            //            {
            //                var item = value as NbjlTrendData;
            //                bool returnState = false;
            //                if ((item.ResultStates & (int)TrendBit.Zhuang) != 0)
            //                {
            //                    returnState = InsertNode(true);
            //                }
            //                else if ((item.ResultStates & (int)TrendBit.Xian) != 0)
            //                {
            //                    returnState = InsertNode(false);
            //                }
            //                else//如果是和 则增加数量
            //                {
            //                    int nLast = Nodes.Count > 0 ? Nodes.Count - 1 : 0;
            //                    if (Nodes.Count != 0)
            //                    {
            //                        Nodes[nLast].DrawCount++;
            //                    }
            //                }
            //                return returnState;
            //            }

            return false;
        }

        public bool AddSingleItem(RoadNode node)
        {
            bool returnState = false;
            if (!CheckHasNode(node.X, node.Y))
            {
                InsertNode(node.IsRed);
                returnState = true;
            }

            return returnState;
        }


        /// <summary>
        /// 三小路
        /// </summary>
        /// <param name="bigRoad"></param>
        /// <param name="type"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public RoadNodeTable(RoadNodeTable bigRoad, EnumTrendType type, int maxHeight)
        {
            CurrentX = 1;
            CurrentY = 1;
            StartX = 1;
            Nodes = new List<RoadNode>();
            NodeDic = new Dictionary<Vector2, RoadNode>();
            LineCount = new List<int>();
            RolMaxHeight = maxHeight;
            LineCount.Add(0);
            First = true;
            Nstep = (int)type + 1;
            var bigRoadList = bigRoad.Nodes;
            //遍历大路单
            foreach (var it in bigRoadList)
            {
                AddSingleItem(it, bigRoad);
            }
        }

        /// <summary>
        /// 添加单独数据（三小路）
        /// </summary>
        /// <param name="it"></param>
        /// <param name="bigRoad"></param>
        /// <returns></returns>
        public bool AddSingleItem(RoadNode it, RoadNodeTable bigRoad)
        {
            bool returnState = false;
            //满足此两项条件 方可产生节点
            if (it.X > Nstep || (it.X == Nstep && it.Y > 1))
            {
                //看整齐
                if (it.Y == 1)
                {
                    returnState = InsertNode(bigRoad.GetRolCount(it.X - 2) == bigRoad.GetRolCount(it.X + 1 - Nstep));
                }
                else
                {
                    //看有无
                    int nTempX = it.X + 1 - Nstep;
                    returnState = bigRoad.CheckHasNode(nTempX, it.Y) ? InsertNode(true) : InsertNode(!bigRoad.CheckHasNode(nTempX, it.Y - 1));
                }
            }
            return returnState;
        }

        /// <summary>
        /// 插入节点数据
        /// </summary>
        /// <param name="bRed"></param>
        /// <param name="nDrawCount"></param>
        /// <returns></returns>
        protected virtual bool InsertNode(bool bRed, int nDrawCount = 0)
        {
            if (First)
            {
                CurrentRed = bRed;
                First = false;
            }
            else
            {
                //是否需要另起一列
                if (bRed != CurrentRed)
                {
                    CurrentRed = bRed;
                    CurrentY = 1;
                    StartX++;
                    CurrentX = StartX;
                    LineCount.Add(0);
                    while (CheckHasNode(CurrentX, CurrentY))
                    {
                        StartX++;
                        CurrentX = StartX;
                        LineCount.Add(0);
                    }
                }
                else
                {
                    if (CheckHasNode(CurrentX, CurrentY + 1) || CurrentY + 1 > RolMaxHeight)
                    {
                        CurrentX++;
                    }
                    else
                    {
                        CurrentY++;
                    }
                }
            }
            RoadNode newNode = new RoadNode(CurrentX, CurrentY, CurrentRed, nDrawCount);
            Nodes.Add(newNode);
            NodeDic.Add(new Vector2(CurrentX, CurrentY), newNode);
            LineCount[StartX - 1]++;
            return true;
        }
        /// <summary>
        /// 检测节点是否存在
        /// </summary>
        /// <param name="nX"></param>
        /// <param name="nY"></param>
        /// <returns></returns>
        protected bool CheckHasNode(int nX, int nY)
        {
            return NodeDic.ContainsKey(new Vector2(nX, nY));
        }

        /// <summary>
        /// 获得指定列长度
        /// </summary>
        /// <param name="nRol"></param>
        /// <returns></returns>
        private int GetRolCount(int nRol)
        {
            if (nRol - 1 < 0)
            {
                return 0;
            }
            return LineCount[nRol - 1];
        }
    }

    /// <summary>
    /// 节点数据
    /// </summary>
    public class RoadNode
    {
        public int X;               //X坐标
        public int Y;               //Y坐标
        public bool IsRed;          //是否为红
        public int DrawCount;       //在该节点上的和 数量

        public RoadNode(int x, int y, bool bRed, int nDrawCount = 0)
        {
            X = x;
            Y = y;
            IsRed = bRed;
            DrawCount = nDrawCount;
        }
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
}
