using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong2D.Game.Data
{
    /// <summary>
    /// 吃，碰，杠的记录
    /// </summary>
    public class MahjongGroupData
    {
        public GroupType type;
        public int[] values;
        private int index;

        /// <summary>
        /// 最上方一张牌是否允许横放
        /// </summary>
        public bool TopLandScape;

        public MahjongGroupData(GroupType type)
        {
            this.type = type;
            switch (type)
            {
                case GroupType.Chi:
                case GroupType.Peng:
                case GroupType.JueGang:
                    values = new int[3];
                    break;
                default:
                    values = new int[4];
                    break;
            }

            index = 0;
        }


        public void AddValue(int val)
        {
            values[index++] = val;
        }

        /// <summary>
        /// 根据乱风数据处理数组排序
        /// </summary>
        public void SortDataByLuanFengData()
        {
            var count = values.Length;
            if (count == 4)
            {
                //当相同的牌数是2张时，相同的两张放在中间位置，即第二张与第四张位置
                //当相同的牌数是3张时，相同的三张牌放在底部，不同的一张放在上面，且横向摆放
                int maxLenth = 1;
                int maxKey = 0;
                var valueDic = new Dictionary<int, int>();
                for (int i = 0; i < count; i++)
                {
                    var checkValue = values[i];
                    if (valueDic.ContainsKey(checkValue))
                    {
                        valueDic[checkValue] += 1;
                        if (valueDic[checkValue] > maxLenth)
                        {
                            maxLenth = valueDic[checkValue];
                            maxKey = checkValue;
                        }
                    }
                    else
                    {
                        valueDic.Add(checkValue, 1);
                    }
                }
                var newArray = new int[4];
                switch (maxLenth)
                {
                    case 1:
                        TopLandScape = true;
                        newArray = values.ToArray();
                        break;
                    case 2:
                        newArray[1] = newArray[3] = maxKey;
                        bool isFirst = true;
                        foreach (var dicItem in valueDic)
                        {
                            var key = dicItem.Key;
                            if (key != maxKey)
                            {
                                if (isFirst)
                                {
                                    if (dicItem.Value==2)
                                    {
                                        newArray[2] = key;
                                    }
                                    newArray[0] = key;
                                    isFirst = false;
                                }
                                else
                                {
                                    newArray[2] = key;
                                }
                            }
                        }
                        TopLandScape = false;
                        break;
                    case 3:
                        newArray[0] = newArray[1] = newArray[2] = maxKey;
                        foreach (var dicItem in valueDic)
                        {
                            var key = dicItem.Key;
                            if (key != maxKey)
                            {
                                newArray[3] = key;
                                break;
                            }
                        }
                        TopLandScape = true;
                        break;
                    case 4:
                        Debug.LogError("Feng Group num is Four,value is:"+maxKey);
                        newArray = values.ToArray();
                        break;
                }
                values = newArray.ToArray();
            }
        }
    }
}