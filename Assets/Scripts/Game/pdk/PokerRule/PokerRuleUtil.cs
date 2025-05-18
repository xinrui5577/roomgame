using System.Linq;
using System;
using System.Collections.Generic;
using Assets.Scripts.Game.pdk.PokerCdCtrl;

namespace Assets.Scripts.Game.pdk.PokerRule
{

    public static class PokerRuleUtil
    {
       public const int SmallJoker = 0x51;
       public const int BigJoker = 0x61;
       public const int MagicKing = 0x71;

        /// <summary>
        /// 判断牌组类型
        /// </summary>
        /// <param name="orgcds">原始牌组，可能没排序</param>
        /// <returns></returns>
        public static CardType GetCdsType(int[]orgcds)
        {
            var sortedCds = SortCds(orgcds);
            if (sortedCds == null) return CardType.None;
            var len = sortedCds.Length;
            var cardList = new List<Card>();
            for (int i = 0; i < len; i++)
            {
                cardList.Add(Card.DeskToAi(sortedCds[i]));
            }
            var result = GetCardTypeResult(cardList,orgcds.Length);

            if (result == null) return CardType.None;

            return result.getType();
        }


        /// <summary>
        /// 从服务器端copy的卡牌类型判断方法
        /// </summary>
        /// <param name="list"></param>
        /// <param name="orgCdsLen">原始牌组长度</param>
        /// <returns></returns>
        static CardTypeResult GetCardTypeResult(List<Card> list, int orgCdsLen)
        {
            int len = list.Count;
            if (len == 0)
            {
                //			GameUtil.logError("landlord getCardType error,len is 0",0);
                return null;
            }
            int first = list[0].getWeight();
            int last = list[len - 1].getWeight();
            if (len <= 4)
            {
                if (first == last)
                {

                    switch (len)
                    {
                        case 1:
                            return new CardTypeResult(CardType.C1, first);
                        case 2:
                            return new CardTypeResult(CardType.C2, first);
                        case 4:
                            return new CardTypeResult(CardType.C4, first);
                    }

                }
                //判断2连对
                else if (len == 4)
                {
                    CardCount ci = getCardCount(list);
                    List<int> arr1 = ci.a[1];
                    // 连对
                    if (arr1.Count * 2 == len && isSequenceArr(arr1))
                    {
                        return new CardTypeResult(CardType.C1122, last);
                    }
                }

            }
            else
            {
                CardCount ci = getCardCount(list);
                List<int> arr0 = ci.a[0];
                List<int> arr1 = ci.a[1];
                List<int> arr2 = ci.a[2];
                List<int> arr3 = ci.a[3];

                // 链子
                if (arr0.Count == len && isSequenceArr(arr0))
                {
                    return new CardTypeResult(CardType.C123, last);
                }

                // 连对
                if (arr1.Count * 2 == len && isSequenceArr(arr1))
                {
                    return new CardTypeResult(CardType.C1122, last);
                }

                // 3带一对
                if (len == 5 && (arr1.Count * 2 + arr0.Count) == arr2.Count * 2)
                {
                    return new CardTypeResult(CardType.C32, arr2[0]);
                }

                // 分析飞机			
                int count = arr2.Count;
                if (count > 0)
                {
                    /*                    //如果是三顺 存在类似 333，444，555，777 这样的飞机带单排的情况和 333，555，666，777，888，带1张单牌的 两种特殊情况
                                        if (!isSequenceArr(arr2) && (count==4 ||count==5))
                                        {
                                            var listTemp = new List<int>();
                                            //去头
                                            for (int i = 1; i < count; i++)
                                            {
                                                listTemp.Add(arr2[i]);
                                            }

                                            //去尾
                                            listTemp.Clear();
                                            for (int i = 0; i < count - 1; i++)
                                            {
                                                listTemp.Add(arr2[i]);
                                            }

                                        }
                                        else */
                    if (isSequenceArr(arr2))
                    {
                        if (arr0.Count + arr1.Count * 2 + arr3.Count * 4 == count * 2)
                        {
                            return new CardTypeResult(CardType.C1112223434, arr2[count - 1], ci);
                        }
                    }
                }

                //如果上面没分析出飞机，则进一步吧炸弹拆成3张的，分析飞机
                if (arr3.Count > 0)
                {
                    var threeCdsList = new List<int>();
                    threeCdsList.AddRange(arr2);
                    threeCdsList.AddRange(arr3);

                    var threeGpList = AnalyAllPartOfShun(threeCdsList.ToArray(), 2);
                    if (threeGpList.Select(cds => cds.Length).Any(cdsLen => cdsLen * 2 == orgCdsLen - cdsLen * 3))
                    {
                        var maxcd = arr2[count - 1];
                        if (arr3[count - 1] > maxcd) maxcd = arr3[count - 1];
                        return new CardTypeResult(CardType.C1112223434, maxcd, ci);
                    }
                }

            }
            return null;
        }



        /** 获得单张的个数，对子的个数，三张的个数，4张的个数 */

        public static CardCount getCardCount(List<Card> list)
        {
            CardCount card_index = new CardCount();
            int[] count = analyzeCards(list);
            for (int i = 0; i < 15; ++i)
            {
                int v = count[i];
                if (v == 0)
                    continue;
                if (v > 4)
                    v = 4;
                card_index.a[v - 1].Add(i + 3); //card's weight
            }
            return card_index;
        }

        /// <summary>
        /// 获取每种权值的牌的数量
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static int[] analyzeCards(List<Card> cards)
        {
            int[] count = new int[15]; // 13,14是小王和大王，如果存在的话
            for (int i = 0; i < 15; i++)
                count[i] = 0;

            foreach (Card card in cards)
            {
                int index = card.getWeight() - 3;
                if (index >= 0)
                {
                    count[index]++;
                    if (count[index] > 4)
                    {
                        count[index] = 4;
                    }
                }
            }


            return count;
        }

        /** 判断是否排序好 */
        private static bool isSequenceArr(List<int> list)
        {
            bool result = false;

            int count = list.Count;
            if (count != 0)
            {
                int first = list[0];
                int last = list[count - 1];
                if (Math.Abs(first - last) == count - 1 && (last < 15) && first < 15)
                {
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 检查是否有赖子
        /// </summary>
        /// <param name="cdsArray"></param>
        /// <returns></returns>
        public static bool CheckHaslz(int[] cdsArray)
        {
            var len = cdsArray.Length;
            for (int i = 0; i < len; i++)
            {
                if (cdsArray[i] == HdCdsCtrl.MagicKing) return true;
            }
            return false;
        }


        /// <summary>
        /// 获得花色
        /// </summary>
        /// <param name="cdValue">可能带花色的牌值</param>
        /// <returns></returns>
        public static int GetColor(int cdValue)
        {
            return cdValue >> 4;
        }

        /// <summary>
        /// 解析一个扑克的牌值
        /// </summary>
        /// <param name="cdValueData">带花色的手牌信息</param>
        /// <returns></returns>
        public static int GetValue(int cdValueData)
        {
            if (cdValueData == SmallJoker || cdValueData == BigJoker
                || cdValueData == MagicKing)
            {
                return cdValueData;
            }

            cdValueData = cdValueData & 0xf;
            return cdValueData;
        }

        /// <summary>
        /// 获取一组牌的值，从大到小排序,但不去花色
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static int[] SortCds(IEnumerable<int> cards)
        {
            if (cards == null) return null;

            var list = new List<int>();
            list.AddRange(cards);
            list.Sort((x, y) => GetValue(y).CompareTo(GetValue(x)));
            return list.ToArray();
        }


        /// <summary>
        /// 获取一组牌的值，从小到大排序,要考虑大小王等牌（过滤掉花色）
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static int[] GetSortedValues(int[] cards)
        {
            int len = cards.Length;
            var values = new int[cards.Length];
            for (int i = 0; i < len; i++)
            {
                values[i] = cards[i] < SmallJoker ? cards[i] & 0xf : cards[i];
            }

            for (int i = 0; i < len - 1; i++)
            {
                for (int j = i + 1; j < len; j++)
                {
                    if (values[i] > values[j])
                    {
                        int temp = values[i];
                        values[i] = values[j];
                        values[j] = temp;
                    }
                }
            }
            return values;
        }


        /// <summary>
        /// 直接比较两组牌
        /// </summary>
        /// <param name="outCds">本次出的牌</param>
        /// <param name="lastOutCds">要被管的牌</param>
        /// <returns>true为出的牌能管上，反之false</returns>
        public static bool JustCompareCds(int[] outCds, int[] lastOutCds)
        {
            if (outCds == null) throw new Exception("JustCompareCds： outcds为空");
            if (lastOutCds == null) throw new Exception("JustCompareCds： lastOutCds为空");

            var otCdstype = GetCdsType(outCds);
            if (otCdstype == CardType.Exception || otCdstype == CardType.None) return false;
            if (otCdstype == CardType.C42) return true;

            var lastOtCdsType = GetCdsType(lastOutCds);
            if (otCdstype == lastOtCdsType)
            {
                if (outCds.Length != lastOutCds.Length) return false;

                switch (otCdstype)
                {
                    case CardType.C1:
                        return JustCompareMinValue(outCds, lastOutCds);
                    case CardType.C2:
                        return JustCompareMinValue(outCds, lastOutCds);
                    case CardType.C3:
                        return JustCompareMinValue(outCds, lastOutCds);
                    case CardType.C4:
                        return JustCompareMinValue(outCds, lastOutCds);
                    case CardType.C5:
                        return JustCompareMinValue(outCds, lastOutCds);
                    case CardType.C123:
                        return JustCompareMinValue(outCds, lastOutCds);
                    case CardType.C1122:
                        return JustCompareMinValue(outCds, lastOutCds);
                    case CardType.C111222:
                        return JustCompareMinValue(outCds, lastOutCds);
                }

                var outcdsSplit = new CdSplitStruct(outCds);
                var lastOutcdsSplit = new CdSplitStruct(lastOutCds);
                switch (otCdstype)
                {
                    case CardType.C31:
                        return outcdsSplit.ThreeCds[0] > lastOutcdsSplit.ThreeCds[0];
                    case CardType.C32:
                        return outcdsSplit.ThreeCds[0] > lastOutcdsSplit.ThreeCds[0];
                    case CardType.C11122234:
                        return outcdsSplit.ThreeCds[0] > lastOutcdsSplit.ThreeCds[0];
                    case CardType.C1112223434:
                        return outcdsSplit.ThreeCds[0] > lastOutcdsSplit.ThreeCds[0];
                    case CardType.C411:
                        return outcdsSplit.FourCds[0] > lastOutcdsSplit.FourCds[0];
                }

            }
            else
            {
                if (lastOtCdsType == CardType.C42) return false;
                switch (otCdstype)
                {
                    case CardType.C4:
                        {
                            return lastOtCdsType != CardType.C5;
                        }
                    case CardType.C5:
                        {
                            return true;
                        }
                    default:
                        return false;
                }
            }

            //有情况没有考虑到
            throw new Exception("两组牌比较时，此方法某种情况没有考虑到");
        }

        /// <summary>
        /// 在牌组类型和牌数相同时，仅仅比较，牌组中的最小值就能决出大小
        /// </summary>
        /// <returns></returns>
        private static bool JustCompareMinValue(int[] outCds, int[] lastOutCds)
        {
            var sortedOtcds = GetSortedValues(outCds);
            var sortedLastOtcds = GetSortedValues(lastOutCds);
            return sortedOtcds[0] > sortedLastOtcds[0];
        }


        /// <summary>
        /// 获得一组牌值(不含重复值)中所有可能组成的大顺子（碎片不小于合法长度）前提是可能检索出一个合法长度的顺子
        /// </summary>
        /// <param name="cdvalueList"> 里面必须都是单张值不能有重复值</param>
        /// <param name="limitNum">顺的最小合法长度</param>
        /// <returns></returns>
        public static List<int[]> GetAllPossibleShun(int[] cdvalueList, int limitNum)
        {

            if (cdvalueList == null || cdvalueList.Length < limitNum) return new List<int[]>();

            //过滤A以上的牌
            var cardsTempList = new List<int>();
            var sortedorgCds = GetSortedValues(cdvalueList);
            for (int i = 0; i < sortedorgCds.Length; i++)
            {
                if (sortedorgCds[i] > 14) break;

                cardsTempList.Add(sortedorgCds[i]);
            }

            //可以组成顺子的所有牌
            var cards = cardsTempList.ToArray();

            var cardsLen = cards.Length;
            if (cardsLen < limitNum) return new List<int[]>();//总长度小于最小合法长度
            var posbShunList = new List<int[]>();

            var canLianCds = new List<int>();
            for (int i = 0; i < cardsLen - 1; i++)
            {
                if (cards[i] == cards[i + 1] - 1)
                {
                    canLianCds.Add(cards[i]);

                    if (i == cardsLen - 2)
                    {
                        canLianCds.Add(cards[i + 1]);
                        posbShunList.Add(canLianCds.ToArray());
                        break;
                    }
                }
                else
                {
                    canLianCds.Add(cards[i]);
                    posbShunList.Add(canLianCds.ToArray());
                    canLianCds.Clear();
                }
            }
            return posbShunList;
        }


        /// <summary>
        /// 检索所有可能的顺子包括大顺子里分解出的小顺子（不一定指单龙）
        /// </summary>
        /// <param name="cdsArray">需要检索的牌组</param>
        /// <param name="limitNum">顺牌的最小合法长度</param>
        /// <returns></returns>
        public static List<int[]> AnalyAllPartOfShun(int[]cdsArray,int limitNum)
        {
            if (cdsArray == null || cdsArray.Length < limitNum) return new List<int[]>();
            var allPosbDaShun = GetAllPossibleShun(cdsArray, limitNum);
            if (allPosbDaShun == null || allPosbDaShun.Count<1) return new List<int[]>();
            var allshun = new List<int[]>();
            //从每个大顺中 分解出所有子顺
            foreach (var dashun in allPosbDaShun)
            {
                FindAllPartOfShun(dashun, allshun, limitNum); 
            }
            return allshun;
        }



        /// <summary>
        /// 获得一组大顺（cds）中可能分解出的所有小顺子
        /// </summary>
        /// <param name="cds">一组顺牌</param>
        /// <param name="allShun">存储顺牌的容器</param>
        /// <param name="minlenLimit">连牌的最小长度</param>
        private static void FindAllPartOfShun(int[] cds, List<int[]> allShun, int minlenLimit)
        {

            var cdsLen = cds.Length;
            if (cdsLen < minlenLimit) return;

            int lasti = minlenLimit;

            while (lasti <= cdsLen)
            {
                var shunCds = new int[lasti];
                for (int i = 0; i < lasti; i++)
                {
                    shunCds[i] = cds[i];
                }
                allShun.Add(shunCds);
                lasti++;
            }
            var newCds = new int[cdsLen - 1];
            Array.Copy(cds, 1, newCds, 0, cdsLen - 1);
            FindAllPartOfShun(newCds, allShun, minlenLimit);
        }
    }

}
