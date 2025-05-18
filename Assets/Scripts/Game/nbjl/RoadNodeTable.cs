//表格初始值

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game.nbjl
{ /*===================================================
 *文件名称:     RoadNodeTable.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-07-07
 *描述:        	节点走势数据
 *历史记录: 
=====================================================*/

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
        public RoadNodeTable(List<TrendData> result,int lineHeight)
        {
            CurrentX = 1;
            CurrentY = 1;
            StartX = 1;
            RolMaxHeight = lineHeight;
            Nodes = new List<RoadNode>();
            NodeDic = new Dictionary<Vector2, RoadNode>();
            LineCount = new List<int> {0};
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
        /// <param name="it"></param>
        /// <returns></returns>
        public bool AddSingleItem(TrendData it)
        {
            bool returnState = false;
            if ((it.ResultStates &(int)TrendBit.Zhuang) != 0)
            {
                returnState=InsertNode(true);
            }
            else if ((it.ResultStates & (int)TrendBit.Xian) != 0)
            {
                returnState=InsertNode(false);
            }
            else//如果是和 则增加数量
            {
                int nLast = Nodes.Count > 0 ?Nodes.Count - 1 : 0;
                if (Nodes.Count != 0)
                {
                    Nodes[nLast].DrawCount++;
                }
            }
            return returnState;
        }

        public bool AddSingleItem(RoadNode node)
        {
            bool returnState = false;
            if (CheckHasNode(node.X, node.Y))
            {
                int nLast = Nodes.Count > 0 ? Nodes.Count - 1 : 0;
                if (Nodes.Count != 0)
                {
                    Nodes[nLast].DrawCount++;
                }
            }
            else
            {
                InsertNode(node.IsRed, node.DrawCount);
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
        public RoadNodeTable(RoadNodeTable bigRoad, EnumTrendType type,int maxHeight)
        {
            CurrentX = 1;
            CurrentY = 1;
            StartX = 1;
            Nodes = new List<RoadNode>();
            NodeDic=new Dictionary<Vector2, RoadNode>();
            LineCount=new List<int>();
            RolMaxHeight = maxHeight;
            LineCount.Add(0);
            First = true;
            Nstep = (int)type+1;
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
        public  bool AddSingleItem(RoadNode it, RoadNodeTable bigRoad)
        {
            bool returnState = false;
            //满足此两项条件 方可产生节点
            if (it.X > Nstep || (it.X == Nstep && it.Y > 1))
            {
                //看整齐
                if (it.Y == 1)
                {
                    returnState=InsertNode(bigRoad.GetRolCount(it.X - 2) == bigRoad.GetRolCount(it.X + 1 - Nstep));
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
            LineCount[StartX-1]++;
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
            if (nRol - 1<0|| nRol>LineCount.Count)
            {
                return 0;
            }
            return LineCount[nRol-1];
        }
    }

    /// <summary>
    /// 节点数据
    /// </summary>
    public class RoadNode:IRecycleData
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
    };
}