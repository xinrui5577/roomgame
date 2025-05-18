/*===================================================
 *文件名称:     FengGangHelpler.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2019-01-31
 *描述:        	风杠检测脚本
 *历史记录: 
=====================================================*/

using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;

namespace Assets.Scripts.Game.Mahjong2D.Game.Data
{
    public class FengGangHelper:MonoSingleton<FengGangHelper>
    {
        #region Function
        private List<string> _forbidList = new List<string>();
        private List<string> _fengList=new List<string>();

        public bool LuanFeng;

        public char NumCellFlag = '-';

        public string NumCellFormat = "{0}{1}{2}";

        public void InitFengDic(bool isLuanFeng)
        {
            LuanFeng = isLuanFeng;
            InitForbidTree();
            var zfbList = new List<int>()
            {
               (int)EnumMahjongValue.Zhong,
               (int)EnumMahjongValue.Fa,
               (int)EnumMahjongValue.Bai
            };
            var dnxbList = new List<int>()
            {
                (int)EnumMahjongValue.Dong,
                (int)EnumMahjongValue.Nan,
                (int)EnumMahjongValue.Xi,
                (int)EnumMahjongValue.Bei
            };
            CreateFengList(zfbList, _fengList,LuanFeng);
            CreateFengList(dnxbList, _fengList,LuanFeng);
            if (!LuanFeng)
            {
                var patchFeng = new List<List<int>>()
                {
                 new List<int>()
                {
                  (int)EnumMahjongValue.Zhong,
                  (int)EnumMahjongValue.Zhong,
                  (int)EnumMahjongValue.Fa,
                  (int)EnumMahjongValue.Bai
                },
                new List<int>()
                {
                 (int)EnumMahjongValue.Zhong,
                 (int)EnumMahjongValue.Fa,
                 (int)EnumMahjongValue.Fa,
                 (int)EnumMahjongValue.Bai
                },
                new List<int>()
                {
                 (int)EnumMahjongValue.Zhong,
                 (int)EnumMahjongValue.Fa,
                 (int)EnumMahjongValue.Bai,
                 (int)EnumMahjongValue.Bai
                }
                };
                SetDicWithList(_fengList, patchFeng);
            }
        }


        void InitForbidTree()
        {
            _forbidList=new List<string>();
            var forBidList=new List<List<int>>()
            {
               GetListByValue(EnumMahjongValue.Dong,3),
               GetListByValue(EnumMahjongValue.Nan,3),
               GetListByValue(EnumMahjongValue.Xi,3),
               GetListByValue(EnumMahjongValue.Bei,3),
               GetListByValue(EnumMahjongValue.Zhong,3),
               GetListByValue(EnumMahjongValue.Fa,3),
               GetListByValue(EnumMahjongValue.Bai,3),
               GetListByValue(EnumMahjongValue.Dong,4),
               GetListByValue(EnumMahjongValue.Nan,4),
               GetListByValue(EnumMahjongValue.Xi,4),
               GetListByValue(EnumMahjongValue.Bei,4),
               GetListByValue(EnumMahjongValue.Zhong,4),
               GetListByValue(EnumMahjongValue.Fa,4),
               GetListByValue(EnumMahjongValue.Bai,4),
            };
            SetDicWithList(_forbidList, forBidList);
        }

        /// <summary>
        /// 检查选择的风杠类型是否满足条件
        /// </summary>
        /// <param name="selectList"></param>
        /// <returns></returns>
        public bool CheckFengGangLimit(List<int> selectList)
        {
            string info=GetValueFormat(selectList);
            return DicExistInfo(_fengList, info);
        }

        public List<List<int>> GetDataByInfos(List<string> infos)
        {
            var list=new List<List<int>>();
            for (int i = 0; i < infos.Count; i++)
            {
                list.Add(GetDataByInfo(infos[i]));
            }
            return list;
        }

        public List<int> GetDataByInfo(string info)
        {
            var list=new List<int>();
            var array=info.Split(NumCellFlag);
            var count = array.Length;
            for (int i = 0; i < count; i++)
            {
                int result;
                if (int.TryParse(array[i],out result))
                {
                    list.Add(result);
                }
            }
            return list;
        }

        /// <summary>
        /// 检测当前牌型中所有有效的风杠类型
        /// </summary>
        /// <param name="checkList">检测列表</param>
        /// <param name="widthNum">根据杠牌数量检测</param>
        /// <param name="num">杠牌数量</param>
        public List<string> CheckValueType(List<int> checkList,bool widthNum=false,int num=3)
        {
            var dnxbList = checkList.FindAll(value => value < (int) EnumMahjongValue.Zhong);
            var zfbList = checkList.FindAll(value => value >=(int) EnumMahjongValue.Zhong);
            var fengList=new List<string>();
            if (widthNum)
            {
                CreateFengList(dnxbList, fengList, false,widthNum,num);
                CreateFengList(zfbList, fengList, false, widthNum, num);
            }
            else
            {
                CreateFengList(dnxbList, fengList, false);
                CreateFengList(zfbList, fengList, false);
            }
            return fengList.FindAll(fengItem=> _fengList.Contains(fengItem));
        }



        private void CreateFengList(List<int> checkList,List<string> fengList, bool widthEqual,bool widthNum=false,int numCount=0)
        {
            var listCount = checkList.Count;
            if (listCount>=3)
            {
                if (widthNum)
                {
                    CreateFengGangTree(checkList, numCount, fengList, widthEqual);
                }
                else
                {
                    CreateFengGangTree(checkList, 3, fengList, widthEqual);
                    CreateFengGangTree(checkList, 4, fengList, widthEqual);
                }
            }
        }

        private bool DicExistInfo(List<string> fengList,string checkInfo)
        {
            return fengList.Contains(checkInfo);
        }


        /// <summary>
        ///  创建风杠列表
        /// </summary>
        /// <param name="list">校验牌</param>
        /// <param name="totalDepth">最大深度（牌型长度）</param>
        /// <param name="fengList">结果</param>
        /// <param name="widthEqual">是否允许重复使用同一元素</param>
        private void CreateFengGangTree(List<int> list,int totalDepth, List<string> fengList,bool widthEqual)
        {
            if (list == null || list.Count == 0) return;
            var count = list.Count;
            for (int i = 0; i < count; i++)
            {
                var value = list[i];
                TreeNode rootNode = new TreeNode()
                {
                    Depth = 0,
                    Index = i,
                    Value = value,
                    SaveValueDic = new Dictionary<int, int>(),
                    Total = value,
                    ChildNodes = new List<TreeNode>()
                };
                rootNode.SaveValueDic.Add(value,1);
                CreateChildNode(list, rootNode, totalDepth, widthEqual);
                FiltrateFengGang(rootNode,totalDepth, rootNode.Value.ToString(),fengList);
            }
        }

        private void FiltrateFengGang(TreeNode node,int depth,string info, List<string> fengList)
        {
            if(node==null)
            {
                return;
            }
            var count = node.ChildNodes.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    var childNode = node.ChildNodes[i];
                    var newInfo = string.Format(NumCellFormat, info, NumCellFlag, childNode.Value);
                    FiltrateFengGang(childNode, depth, newInfo, fengList);
                }
            }
            else
            {
                if (_forbidList.Contains(info))
                {
                }
                else
                {
                    if (node.Depth == depth - 1)
                    {
                        TryAddDicValue(fengList, info);
                    }
                }
            }
        }

        private void TryAddDicValue(List<string> fengList,string info)
        {
            if (!fengList.Contains(info))
            {
                fengList.Add(info);
            }
        }

        private void CreateChildNode(List<int> treeData,TreeNode parentNode,int totalDepth,bool widthEqual)
        {
            if (parentNode == null) return;
            var count = treeData.Count;
            if (parentNode.Depth<totalDepth-1)
            {
                for (int i = 0; i < count; i++)
                {
                    if (CheckIndex(i,parentNode.Index, widthEqual))
                    {
                        var value = treeData[i];
                        TreeNode childNode= new TreeNode()
                        {
                            Index = i,
                            Value = value,
                            Depth = parentNode.Depth + 1,
                            SaveValueDic = new Dictionary<int, int>(),
                            Total = parentNode.Total+value,
                            ChildNodes = new List<TreeNode>()
                        };
                        parentNode.ChildNodes.Add(childNode);
                        if (parentNode.SaveValueDic.ContainsKey(value))
                        {
                            parentNode.SaveValueDic[value] += 1;
                        }
                        else
                        {
                            parentNode.SaveValueDic.Add(value, 1);
                        }
                        CreateChildNode(treeData,childNode,totalDepth, widthEqual);
                    }
                }
            }
        }

        private bool CheckIndex(int curIndex,int parentNodeIndex,bool widthEqual)
        {
            return widthEqual ? curIndex >= parentNodeIndex : curIndex > parentNodeIndex;
        }

        private void SetDicWithList(List<string> fengList,List<List<int>> lists)
        {
            var count = lists.Count;
            for (int i = 0; i < count; i++)
            {
                var list = lists[i];
                var info = GetValueFormat(list);
                fengList.Add(info);
            }
        }

        private List<int> GetListByValue(EnumMahjongValue mahjongValue,int count)
        {
            var list=new List<int>();
            int value =(int)mahjongValue;
            for (int i = 0; i < count; i++)
            {
                list.Add(value);
            }
            return list;
        }

        private string GetValueFormat(List<int> values)
        {
            if (values == null|| values.Count==0)
            {
                return string.Empty;
            }
            var count = values.Count;
            string typeInfo = values[0].ToString();
            for (int i = 1; i < count; i++)
            {
                typeInfo = string.Format(NumCellFormat, typeInfo, NumCellFlag, values[i]);
            }
            return typeInfo;
        }

        #endregion
    }
    /// <summary>
    /// 节点
    /// </summary>
    class TreeNode
    {
        /// <summary>
        /// 深度
        /// </summary>
        public int Depth;
        /// <summary>
        /// 索引
        /// </summary>
        public int Index;
        /// <summary>
        /// 节点值
        /// </summary>
        public int Value;

        /// <summary>
        ///节点总和
        /// </summary>
        public int Total;
        public Dictionary<int, int> SaveValueDic=new Dictionary<int, int>(); 
        /// <summary>
        /// 子节点
        /// </summary>
        public List<TreeNode> ChildNodes;
    }
}
