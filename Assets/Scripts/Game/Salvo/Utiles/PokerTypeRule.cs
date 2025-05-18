using System;
using System.Collections.Generic;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.Salvo.Utiles
{
    public class PokerTypeRule
    {
        public static int MinJack = 8;
        /// <summary>
        /// 获取好牌
        /// </summary>
        /// <param name="pokers"></param>
        /// <param name="helddate"></param>
        /// <returns></returns>
        public static PokerType GetGoodPokerIndex(int[] pokers,out HeldDate helddate)
        {
            var dict = new Dictionary<int, HeldDate>();
            var suitState = 0;
            var len = pokers.Length;
            var valueArray = new int[len];
            var hasBKing = false;
            var kingIndex = -1;
            //采样
            for (var i = 0; i < len;i++ )
            { 
                var poker = pokers[i];
                var suit = (poker>>4)&0xF;//花色
                var value = poker & 0xF;//数值
                valueArray[i] = value;
                if (suit > 4) //大王
                {
                    hasBKing = true;
                    kingIndex = i;
                    continue;
                }
                if (dict.ContainsKey(value)) dict[value].Add(i);
                else dict.Add(value, new HeldDate(0,i)); 
                suitState |= (1 << (suit-1));
            }
            YxDebug.LogArray(valueArray);
            //求出牌型
            var count = dict.Count;//不同值得个数
            YxDebug.Log("个数: " + count + ",     花色: " + Convert.ToString(suitState, 2).PadLeft(5,'0'));
            switch (count)
            {
                case 1://五条
                    //直接返回5张
                    helddate = new HeldDate(0x1f);
                    return PokerType.KindOf5;
                case 2://四条,葫芦      返回4张 或  5张
                    {
                        var values = dict.Values;
                        var maxc = hasBKing ? 3 : 4;
                        foreach (var date in values)
                        {
                            var vc = date.Count();
                            YxDebug.Log(vc + " : " + maxc);
                            if (vc < maxc) continue;
                            helddate = date;
                            if (hasBKing) helddate.Add(kingIndex);
                            return PokerType.KindOf4;//4条
                        }
                        helddate = new HeldDate(0x1f); //葫芦
                        return PokerType.FullHouse;
                    }
                case 3://三条,两对      返回3张或4张
                    {
                        var values = dict.Values;
                        var temp = new HeldDate();
                        var maxc = hasBKing ? 2 : 3;
                        var minc = hasBKing ? 1 : 2;
                        foreach (var date in values)
                        {
                            var vc = date.Count();
                            if (vc >= maxc)//三条
                            {
                                helddate = date;
                                if (hasBKing) helddate.Add(kingIndex);
                                return PokerType.KindOf3;
                            }
                            if (vc >= minc) temp = temp | date;
                        }
                        helddate = temp; //两对
                        if (hasBKing) helddate.Add(kingIndex);
                        return PokerType.PairOf2;
                    }
                case 4://一对         返回2张
                    {
                        var maxc = 2;
                        if (hasBKing)
                        {
                            var type = CheckFive(valueArray, out helddate, true, suitState, count);
                            if (type != PokerType.None) return type;
                            maxc = 1;
                        }
                        HeldDate max = null;
                        var temp = 0;
                        foreach (var date in dict)
                        {
                            var key = date.Key;
                            var value = date.Value;
                            var vc = value.Count();
                            if (key < MinJack) continue; //是否大于最小对
                            if (vc < maxc) continue; //是否是一对
                            if (temp > key) continue;
                            temp = key;
                            max = value;
                        }
                        helddate = max;
                        if (max == null) return PokerType.None;
                        if (hasBKing) helddate.Add(kingIndex);
                        return PokerType.JacksUp;
                    }
                case 5://杂牌,同花大顺,同花顺,同花,顺子
                     return CheckFive(valueArray, out helddate, hasBKing, suitState, count); 
                default : 
                    helddate = null;
                    return PokerType.None;
            }  
        }

        public static PokerType CheckFive(int[] valueArray, out HeldDate helddate, bool hasBKing,int suitState,int difCount)
        {
            Array.Sort(valueArray);
            YxDebug.LogArray(valueArray);
            var valueCount = valueArray.Length;
            var maxIndex = valueCount - 1;
            var max = valueArray[maxIndex];
            var minIndex = hasBKing ? 1 : 0;
            var min = valueArray[minIndex];
            var isShun = (max - min) < 5;//顺子
            var isSame = GetOneCount(suitState) == 1;//是否同花 
            
            YxDebug.Log("最大值: " + max + "  顺子:" + isShun + "  同花: " + isSame);
            if (isSame) //同花
            {
                helddate = new HeldDate(0x1f);
                if (!isShun) return PokerType.Flush; //同花顺
                return min > 9 ? PokerType.RoyalFlush : PokerType.StrFlush; //同花大顺  |  同花顺
            }
            if (isShun)//顺子
            {
                helddate = new HeldDate(0x1f);
                return PokerType.Straight;
            }
            helddate = null;
            return PokerType.None;
        }



        public static int GetOneCount(int suitState)
        {
            suitState = (suitState & 0x55555555) + ((suitState >> 1) & 0x55555555);
            suitState = (suitState & 0x33333333) + ((suitState >> 2) & 0x33333333);
            suitState = (int) ((suitState & 0x0f0f0f0f0f) + ((suitState >> 4) & 0x0f0f0f0f));
            return suitState;
        }
    }

    public enum PokerType
    {
        KindOf5=9,//5条
        RoyalFlush=8,//大同花顺
        StrFlush=7,//同花顺
        KindOf4=6,//4条
        FullHouse=5,//葫芦
        Flush = 4,//同花
        Straight = 3, //顺子
        KindOf3=2,//3条
        PairOf2=1,//2对  
        JacksUp=0,//1对
        None = -1
    }


    public class HeldDate
    {
        public int Value { get; private set; }

        public HeldDate(int value = 0)
        {
            Value = value;
        }

        public HeldDate(int value, int index)
        {
            Value = value;
            Add(index);
        }

        public void Add(int index)
        {
            Value |= 1 << index;
            
        }

        public static HeldDate operator |(HeldDate date1, HeldDate date2)
        {
            var f = date1.Value | date2.Value;
            return new HeldDate(f);
        }

        public bool Has(int index)
        {
            return (Value & (1 << index)) > 0;
        }

        public int Count()
        {
            return PokerTypeRule.GetOneCount(Value); 
        } 
    }
}
