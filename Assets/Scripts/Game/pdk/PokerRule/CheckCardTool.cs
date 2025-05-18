using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.pdk.PokerCdCtrl;
using Assets.Scripts.Game.pdk.DDz2Common;

namespace Assets.Scripts.Game.pdk.PokerRule
{
    /// <summary>
    /// 把一组牌 单张 对子，三张，4张 分组，每组是排序好的
    /// </summary>
    public struct CdSplitStruct
    {
        /// <summary>
        /// 原始卡牌组数据,没有经过花色过滤
        /// </summary>
        public readonly int[] OrgCds;
        /// <summary>
        /// 按照 单张，对子 3张 4张 顺序 排列的牌组,的所有牌（记住是所有牌的牌值）
        /// </summary>
        public readonly int[] SortedCds;
        /// <summary>
        /// 以从小到大牌值顺序排列的原始不带花色牌组牌组
        /// </summary>
        public readonly int[] OrderSortedCds;
        /// <summary>
        /// 单牌组
        /// </summary>
        public readonly List<int> DanCds;
        /// <summary>
        /// 对牌组,只存牌值
        /// </summary>
        public readonly List<int> DuiCds;
        /// <summary>
        /// 3牌组,只存牌值
        /// </summary>
        public readonly List<int> ThreeCds;
        /// <summary>
        /// 4牌组,只存牌值
        /// </summary>
        public readonly List<int> FourCds;

        public CdSplitStruct(int[] cards)
        {
            OrgCds = (int[])cards.Clone();

            DanCds = new List<int>();
            DuiCds = new List<int>();
            ThreeCds = new List<int>();
            FourCds = new List<int>();

            if (cards == null) throw new Exception("CdSplitStruct 生成错误  cards为null");

            OrderSortedCds = PokerRuleUtil.GetSortedValues(cards);

            var cdsDic = new Dictionary<int, short>(); //牌值 ，个数

            for (int i = 0; i < OrderSortedCds.Length; i++)
            {
                if (!cdsDic.ContainsKey(OrderSortedCds[i]))
                {
                    cdsDic[OrderSortedCds[i]] = 1;
                    continue;
                }

                cdsDic[OrderSortedCds[i]] += 1;
            }

            foreach (var key in cdsDic.Keys)
            {
                switch (cdsDic[key])
                {
                    case 1:
                        DanCds.Add(key);
                        break;
                    case 2:
                        DuiCds.Add(key);
                        break;
                    case 3:
                        ThreeCds.Add(key);
                        break;
                    case 4:
                        FourCds.Add(key);
                        break;
                }
            }

            SortedCds = new int[DanCds.Count + DuiCds.Count * 2 + ThreeCds.Count * 3 + FourCds.Count * 4];
            int sortedI = 0;
            foreach (var cd in DanCds)
            {
                SortedCds[sortedI] = cd;
                sortedI++;
            }
            foreach (var cd in DuiCds)
            {
                SortedCds[sortedI] = cd;
                sortedI++;
                SortedCds[sortedI] = cd;
                sortedI++;
            }
            foreach (var cd in ThreeCds)
            {
                SortedCds[sortedI] = cd;
                sortedI++;
                SortedCds[sortedI] = cd;
                sortedI++;
                SortedCds[sortedI] = cd;
                sortedI++;
            }
            foreach (var cd in FourCds)
            {
                SortedCds[sortedI] = cd;
                sortedI++;
                SortedCds[sortedI] = cd;
                sortedI++;
                SortedCds[sortedI] = cd;
                sortedI++;
                SortedCds[sortedI] = cd;
                sortedI++;
            }
        }
    }


    public class CheckCardTool
    {

        const int NOT_EXIT = -1;//异常
        const int BU_CHU = 0;//不出..
        const int YI_ZHANG = 1;//一张..
        const int DUI_ZI = 2;//对子..
        const int BOMB = 4;//普通炸弹..
        const int SHUN_ZI = 5;//顺子..
        const int SHUANG_SHUN = 6;//双顺..
        const int TIAN_ZHA = 7;//双王..
        const int SUPER_BOMB = 8;//超级炸弹
        const int PLANE_3 = 9;//3带  三条 三顺
        const int PLANE_3_1 = 10;//3带1..
        const int PLANE_3_2 = 11;//3带2..
        const int PLANE_4 = 12;//4带
        const int PLANE_4_1 = 13;//4带1
        const int PLANE_4_2 = 14;//4带2

        /// <summary>
        /// 分析cds返回int型
        /// </summary>
        /// <param name="cds"></param>
        /// <returns></returns>
        public delegate CardType ReturnCardType(int[] cds);
        /// <summary>
        /// 获得一组手牌的牌型
        /// </summary>
        private readonly ReturnCardType _getCdsType;
        /// <summary>
        /// 获得牌型
        /// </summary>
        /// <param name="getcdsTypeMethod"></param>
        public CheckCardTool(ReturnCardType getcdsTypeMethod)
        {
            _getCdsType = getcdsTypeMethod;
        }

        //-----------------对外智能选牌使用的方法

        /// <summary>
        /// 在不需要比较别家出牌，自己先手出牌的情况下
        /// </summary>
        /// <param name="selectCds">玩家从手牌中提出的牌</param>
        /// <param name="handCds">手牌</param>
        /// <returns></returns>
        public int[] GetcdsWithOutCompare(int[] selectCds, int[] handCds)
        {
            //在框选的范围内找合法的牌,不考虑 对子和单牌的情况
            var selcdtype = _getCdsType(selectCds);
            //如果直接是合法牌，则直接返回框选的牌
            if (selcdtype != CardType.None && selcdtype != CardType.Exception)//&& selcdtype != CardType.C1 && selcdtype != CardType.C2
            {
                return selectCds;
            }

            var cdsplitStruct = new CdSplitStruct(selectCds);
            var canoutCds = CheckOutWithOutType1And2(cdsplitStruct);
            if (canoutCds != null && canoutCds.Length > 0) return canoutCds;

            //所有手牌的struct
            var hdSplitSutuct = new CdSplitStruct(handCds);

            var posbShun = CheckPossbShunzi(cdsplitStruct, hdSplitSutuct);
            if (posbShun != null) return posbShun;

            var posbLd = CheckpossbLianDui(cdsplitStruct, hdSplitSutuct);
            if (posbLd != null) return posbLd;

            var posbSanTake = CheckThreeWith(cdsplitStruct, hdSplitSutuct);
            if (posbSanTake != null) return posbSanTake;

            return null;
        }

        /// <summary>
        ///  在有别家出牌的情况下 智能检测出牌
        /// </summary>
        /// <param name="selectCds">已经框选出来的牌值</param>
        /// <param name="canOutCardListDic">提示牌组和对应的类型</param>
        /// <param name="compareCds">要去比较的牌,上家牌</param>
        public int[] ChkCanoutCdListWithLastCdList(int[] selectCds, Dictionary<int[],CardType> canOutCardListDic,int[] compareCds)
        {
            if (canOutCardListDic == null || canOutCardListDic.Count < 1)
            {
                return null;
            }

            //检查选出的牌是不是就是提示牌组中的某组牌
            var selcdsPureSortValues = PokerRuleUtil.GetSortedValues(selectCds);
            var selCdsType = PokerRuleUtil.GetCdsType(selectCds);
            var selcdsLen = selectCds.Length;
            foreach (var canoutCds in canOutCardListDic.Keys)
            {
                if (selcdsLen == canoutCds.Length && selCdsType == canOutCardListDic[canoutCds])
                {
                    if (DDzUtil.IsTwoArrayEqual(selcdsPureSortValues, PokerRuleUtil.GetSortedValues(canoutCds)))
                        return canoutCds;
                }

                //检查选的牌是不是提示牌组的子集
                if (DDzUtil.IsSubsetArray(PokerRuleUtil.GetSortedValues(canoutCds),selcdsPureSortValues))
                {
                    return canoutCds;
                }
            }
            


            //检查提示牌组中有没有

            //DDzUtil.IsTwoArrayEqual()


/*            var listTemp = new List<int>();

            //如果是提示牌的子集 直接返回相应的提示牌组
            var posbFitCdsList = new List<int[]>();
            foreach (var canOutcds in canOutCardList)
            {
                listTemp.Clear();
                listTemp.AddRange(canOutcds);

                bool findNotMatch = false;
                for (int i = 0; i < selectCds.Length; i++)
                {
                    if (listTemp.Contains(selectCds[i]) || listTemp.Contains(HdCdsCtrl.GetValue(selectCds[i])))
                    {
                        posbFitCdsList.Add(canOutcds);
                        listTemp.Remove(selectCds[i]);
                    }
                    else
                    {
                        findNotMatch = true;
                        break;
                    }
                }

                if (!findNotMatch) return canOutcds;
            }

            #region 进一步查找可能的牌
            //如果框选的牌中，含有有符合提示的牌组中的某张牌 ，则看下是不是  三带的情况的牌组
            if (posbFitCdsList.Count > 0)
            {
                var compareType = _getCdsType(PokerRuleUtil.GetSortedValues(compareCds));
                //如果不是三带的类型 就不用检测了
                if (compareType != CardType.C31 && compareType != CardType.C32) return null;

                foreach (var posbFitCds in posbFitCdsList)
                {
                    //排除不是三顺和三条的提示牌
                    if (_getCdsType(PokerRuleUtil.GetSortedValues(posbFitCds)) != CardType.C3) continue;

                    //拼凑可能合法的牌组
                    listTemp.Clear();
                    listTemp.AddRange(selectCds);
                    for (int i = 0; i < posbFitCds.Length; i++)
                    {
                        listTemp.Remove(posbFitCds[i]);
                    }

                    for (int i = 0; i < posbFitCds.Length; i++)
                    {
                        listTemp.Add(posbFitCds[i]);
                    }

                    var finalCds = PokerRuleUtil.GetSortedValues(listTemp.ToArray());

                    if (finalCds.Length == compareCds.Length && _getCdsType(finalCds) == compareType)
                        return finalCds;
                }
            }
            #endregion*/

            return null;
        }


        //----------------------------------------------------------------end

        /// <summary>
        /// 检测出的牌可能组成的顺子
        /// </summary>
        /// <param name="cdsplitStruct"></param>
        /// <param name="hdSplitSutuct">所有手牌</param>
        private int[] CheckPossbShunzi(CdSplitStruct cdsplitStruct, CdSplitStruct hdSplitSutuct)
        {
            var danCds = cdsplitStruct.DanCds;
            if (cdsplitStruct.OrgCds.Length != danCds.Count || danCds.Count < 2)
                return null;

            //排序好的 框出来的牌
            var sortedCds = PokerRuleUtil.GetSortedValues(danCds.ToArray());

            var listTemp = new List<int>();
            listTemp.AddRange(hdSplitSutuct.DanCds);
            listTemp.AddRange(hdSplitSutuct.DuiCds);
            listTemp.AddRange(hdSplitSutuct.ThreeCds);
            listTemp.AddRange(hdSplitSutuct.FourCds);

            var keys = listTemp.ToArray();

            var allposbShunList = PokerRuleUtil.GetAllPossibleShun(keys, 5);
            if (allposbShunList == null || allposbShunList.Count<1) return null;

            //存手牌中最终找到的那组顺牌
            var findShun = new List<int>();
            foreach (var shunCds in allposbShunList)
            {
                var shuncdsLen = shunCds.Length;
                if (shuncdsLen >= 5)
                {
                    if (shunCds[0] <= sortedCds[0] && shunCds[shuncdsLen - 1] >= sortedCds[sortedCds.Length - 1])
                    {
                        findShun.Clear();
                        findShun.AddRange(shunCds);
                        break;
                    }
                }
            }
            if (findShun.Count < 1) return null;

            return findShun.ToArray();
        }

        /// <summary>
        /// 检测出牌可能组成的连对
        /// </summary>
        /// <param name="cdsplitStruct"></param>
        /// <param name="hdSplitSutuct"></param>
        /// <returns></returns>
        private int[] CheckpossbLianDui(CdSplitStruct cdsplitStruct, CdSplitStruct hdSplitSutuct)
        {
            var danCds = cdsplitStruct.DanCds;
            var duiCds = cdsplitStruct.DuiCds;
            if (cdsplitStruct.OrgCds.Length != danCds.Count + duiCds.Count * 2 || (danCds.Count + duiCds.Count) < 2) return null;

            var chooseCdsValueList = new List<int>();
            chooseCdsValueList.AddRange(danCds);
            chooseCdsValueList.AddRange(duiCds);
            chooseCdsValueList.Sort();

            //存手牌中对牌的值列表
            var allduiHandCdValue = hdSplitSutuct.DuiCds;

            //框选出的牌
            var chooseCds = chooseCdsValueList.ToArray();
            //所有可能的连对 组
            var alposbLdList = PokerRuleUtil.GetAllPossibleShun(allduiHandCdValue.ToArray(), 3);

            if (alposbLdList == null || alposbLdList.Count<1) return null;//没有找到符合条件的连对

            var findLd = new List<int>();
            foreach (var ldCds in alposbLdList)
            {
                var ldLen = ldCds.Length;
                if (ldLen >= 3)
                {
                    if (ldCds[0] <= chooseCds[0] &&
                        ldCds[ldCds.Length - 1] >= chooseCds[chooseCds.Length - 1])
                    {
                        findLd.Clear();
                        findLd.AddRange(ldCds);
                        break;
                    }
                }
            }

            if (findLd.Count < 1) return null; //没有找到符合条件的连对

            var findldLen = findLd.Count;
            var returnLdCds = new int[findldLen * 2];
            int j = 0;
            for (int i = 0; i < findldLen; i++)
            {
                returnLdCds[j++] = findLd[i];
                returnLdCds[j++] = findLd[i];
            }

            return returnLdCds;
        }

        /// <summary>
        /// 检测潜在的三带的牌
        /// </summary>
        /// <param name="cdsplitStruct"> 已经被弹起的牌值的数据结构</param>
        /// <param name="hdSplitSutuct"> 手牌</param>
        /// <returns></returns>
        private int[] CheckThreeWith(CdSplitStruct cdsplitStruct, CdSplitStruct hdSplitSutuct)
        {
            var danList = cdsplitStruct.DanCds;
            var duiList = cdsplitStruct.DuiCds;

            var sanList = hdSplitSutuct.ThreeCds;

            foreach (var v in sanList)
            {
                int value = v;

                if (value < PokerRuleUtil.SmallJoker) value &= 0xf;

                if (danList.Contains(value) || duiList.Contains(value))
                {
                    var orderCds = cdsplitStruct.OrderSortedCds;
                    var otherCds = new List<int>();
                    for (int i = 0; i < orderCds.Length; i++)
                    {
                        if (orderCds[i] != value) otherCds.Add(orderCds[i]);
                    }

                    //组合一下牌变成wholecards
                    otherCds.AddRange(new int[] { value, value, value });
                    if (_getCdsType != null)
                    {
                        int[] cds = otherCds.ToArray();
                        cds = PokerRuleUtil.GetSortedValues(cds);
                        var type = _getCdsType(cds);
                        //如果有合法牌返回这组牌
                        if (type != CardType.None && type != CardType.Exception)
                        {
                            return otherCds.ToArray();
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 智能选牌 在某个范围内 选出 除了单 和对子 之外的牌型
        /// </summary>
        private int[] CheckOutWithOutType1And2(CdSplitStruct cdsplitStruct)
        {
            //找飞机
            var feiji = FindFeiJi(cdsplitStruct);
            if (feiji != null) return feiji;

            #region  连对 顺子找长度比较长的牌

            var cdsList = new List<int[]>();

            //找连对，
            var lianDui = FindType2Abc(cdsplitStruct);
            cdsList.Add(lianDui);
            //找顺子，
            var shunzi = FindType123(cdsplitStruct);
            cdsList.Add(shunzi);
            //找3带，
            var typ3TakeCds = FindType3TakeCds(cdsplitStruct);
            cdsList.Add(typ3TakeCds);

            int[] chooseCds = null;
            foreach (var cds in cdsList)
            {
                if (chooseCds == null)
                {
                    chooseCds = cds;
                }
                else
                {

                    if (cds != null && cds.Length > chooseCds.Length)
                    {
                        chooseCds = cds;
                    }
                }
            }

            if (chooseCds != null && chooseCds.Length > 0) return chooseCds;

            #endregion
/*
            //找4带2
            var type42Cds = FindType42Cds(cdsplitStruct);
            if (type42Cds != null) return type42Cds;*/

            //找炸弹
            var bomb = FindBomb(cdsplitStruct);
            if (bomb != null) return bomb;

            return null;
        }

/*
        /// <summary>
        /// 把一组牌 单张 对子，三张，4张 分组，每组是排序好的
        /// </summary>
        public struct CdSplitStruct
        {
            /// <summary>
            /// 原始卡牌组数据,没有经过花色过滤
            /// </summary>
            public readonly int[] OrgCds;
            /// <summary>
            /// 按照 单张，对子 3张 4张 顺序 排列的牌组
            /// </summary>
            public readonly int[] SortedCds;
            //以从小到大顺序排列的牌组
            public readonly int[] OrderSortedCds;
            /// <summary>
            /// 单牌组
            /// </summary>
            public readonly List<int> DanCds;
            /// <summary>
            /// 对牌组,只存牌值
            /// </summary>
            public readonly List<int> DuiCds;
            /// <summary>
            /// 3牌组,只存牌值
            /// </summary>
            public readonly List<int> ThreeCds;
            /// <summary>
            /// 4牌组,只存牌值
            /// </summary>
            public readonly List<int> FourCds;

            public CdSplitStruct(int[] cards)
            {
                OrgCds = (int[])cards.Clone();

                DanCds = new List<int>();
                DuiCds = new List<int>();
                ThreeCds = new List<int>();
                FourCds = new List<int>();

                if (cards == null) throw new Exception("CdSplitStruct 生成错误  cards为null");

                OrderSortedCds = GetSortedValues(cards);

                var cdsDic = new Dictionary<int, short>(); //牌值 ，个数

                for (int i = 0; i < OrderSortedCds.Length; i++)
                {
                    if (!cdsDic.ContainsKey(OrderSortedCds[i]))
                    {
                        cdsDic[OrderSortedCds[i]] = 1;
                        continue;
                    }

                    cdsDic[OrderSortedCds[i]] += 1;
                }

                foreach (var key in cdsDic.Keys)
                {
                    switch (cdsDic[key])
                    {
                        case 1:
                            DanCds.Add(key);
                            break;
                        case 2:
                            DuiCds.Add(key);
                            break;
                        case 3:
                            ThreeCds.Add(key);
                            break;
                        case 4:
                            FourCds.Add(key);
                            break;
                    }
                }

                SortedCds = new int[DanCds.Count + DuiCds.Count * 2 + ThreeCds.Count * 3 + FourCds.Count * 4];
                int sortedI = 0;
                foreach (var cd in DanCds)
                {
                    SortedCds[sortedI] = cd;
                    sortedI++;
                }
                foreach (var cd in DuiCds)
                {
                    SortedCds[sortedI] = cd;
                    sortedI++;
                    SortedCds[sortedI] = cd;
                    sortedI++;
                }
                foreach (var cd in ThreeCds)
                {
                    SortedCds[sortedI] = cd;
                    sortedI++;
                    SortedCds[sortedI] = cd;
                    sortedI++;
                    SortedCds[sortedI] = cd;
                    sortedI++;
                }
                foreach (var cd in FourCds)
                {
                    SortedCds[sortedI] = cd;
                    sortedI++;
                    SortedCds[sortedI] = cd;
                    sortedI++;
                    SortedCds[sortedI] = cd;
                    sortedI++;
                    SortedCds[sortedI] = cd;
                    sortedI++;
                }
            }
        }
*/

        /// <summary>
        /// 在一组牌中找到4带2牌型的子集
        /// </summary>
        /// <param name="cdSplitStruct">卡牌组</param>
        public int[] FindType42Cds(CdSplitStruct cdSplitStruct)
        {
            var cdsLen = cdSplitStruct.OrgCds.Length;
            if (cdsLen < 6)
            {
                return null;
            }

            var fourCds = cdSplitStruct.FourCds;
            if (fourCds.Count < 1)
            {
                return null;
            }

            //type42牌型list
            var type42CdsList = new List<int>();

            for (int i = 0; i < 4; i++)
                type42CdsList.Add(fourCds[0]);

            //先找4带2单的可能
            var dancds = cdSplitStruct.DanCds;
            if (dancds.Count > 1)
            {
                for (int i = 0; i < 2; i++)
                    type42CdsList.Add(dancds[i]);

                return type42CdsList.ToArray();
            }

            //再找4带2对的可能
            var duiCds = cdSplitStruct.DuiCds;
            if (duiCds.Count > 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    type42CdsList.Add(duiCds[i]);
                    type42CdsList.Add(duiCds[i]);
                }
                return type42CdsList.ToArray();
            }

            //再找4带1对的可能
            if (duiCds.Count == 1)
            {
                type42CdsList.Add(duiCds[0]);
                type42CdsList.Add(duiCds[0]);
                return type42CdsList.ToArray();
            }

            //4带2的2那部分
            var compcdsPartof2 = new List<int>();
            compcdsPartof2.AddRange(cdSplitStruct.DanCds);
            foreach (var i in cdSplitStruct.DuiCds)
            {
                compcdsPartof2.Add(i);
                compcdsPartof2.Add(i);
            }
            foreach (var i in cdSplitStruct.ThreeCds)
            {
                compcdsPartof2.Add(i);
                compcdsPartof2.Add(i);
                compcdsPartof2.Add(i);
            }

            if (compcdsPartof2.Count < 2) return null;
            for (int i = 0; i < 2; i++)
                type42CdsList.Add(compcdsPartof2[i]);

            return type42CdsList.ToArray();
        }

        /// <summary>
        ///  在一组牌中找到炸弹牌型的子集
        /// </summary>
        /// <param name="cdSplitStruct"></param>
        /// <returns></returns>
        public int[] FindBomb(CdSplitStruct cdSplitStruct)
        {
            var cdsLen = cdSplitStruct.OrgCds.Length;
            if (cdsLen < 4)
            {
                return null;
            }

            var fourCds = cdSplitStruct.FourCds;
            if (fourCds.Count < 1) return null;

            return new[] { fourCds[0], fourCds[0], fourCds[0], fourCds[0] };
        }

        /// <summary>
        /// 找飞机,三带不算在飞机范围内，最少8张牌算一个飞机
        /// </summary>
        /// <param name="cdSplitStruct"></param>
        /// <returns></returns>
        public int[] FindFeiJi(CdSplitStruct cdSplitStruct)
        {
            var cdsLen = cdSplitStruct.OrgCds.Length;
            if (cdsLen < 10)
            {
                return null;
            }

            //找3顺
            var threeShun = CheckShun(cdSplitStruct.ThreeCds.ToArray(), 2);
            threeShun = RemoveBigCdsInArray(threeShun);
            if (threeShun == null || threeShun.Length<2) return null;

            //存储顺在一起的3张牌组,未*3
            var canLian3Cds = new List<int>();
            canLian3Cds.AddRange(threeShun);
            var threeCdsLen = canLian3Cds.Count;

            //获取飞机带的部分
            var sortedCds = cdSplitStruct.SortedCds;

            //var threeCdsLenX2 = threeCdsLen * 2;
            //可组成如果带的部分的牌数不足，则返回null
            if ((sortedCds.Length - threeCdsLen * 3) < threeCdsLen*2)
            {
                return null;
            }


            //最终选出的牌   //---------------------------------------------------------------------------------
            var finalCds = new List<int>();
            foreach (var cd in canLian3Cds)
            {
                finalCds.Add(cd);
                finalCds.Add(cd);
                finalCds.Add(cd);
            }

            //三带的部分
            var otherPartOfcds = new List<int>();
            //添加单牌部分
            otherPartOfcds.AddRange(cdSplitStruct.DanCds);

            //添加对牌部分
            foreach (var duiCd in cdSplitStruct.DuiCds)
            {
                otherPartOfcds.AddRange(new int[] { duiCd, duiCd });
            }


            foreach (var threeCd in cdSplitStruct.ThreeCds)
            {
                if (!canLian3Cds.Contains(threeCd))
                {
                    otherPartOfcds.AddRange(new int[] { threeCd, threeCd, threeCd});
                }
            }

            var threeTakeLen = threeCdsLen*2;

            for (int i = 0; i < threeTakeLen; i++)
            {
                finalCds.Add(otherPartOfcds[i]);
            }
            return finalCds.ToArray();

            /*            //先找可能三带单的飞机
                            var dancds = cdSplitStruct.DanCds;
                            if (dancds.Count >= threeCdsLen)
                            {
                                for (int i = 0; i < threeCdsLen; i++)
                                {
                                    finalCds.Add(dancds[i]);
                                }

                                return finalCds.ToArray();
                            }

                            //再找可能是三代对牌
                            var duiCds = cdSplitStruct.DuiCds;
                            if (duiCds.Count >= threeCdsLen)
                            {
                                for (int i = 0; i < threeCdsLen; i++)
                                {
                                    finalCds.Add(duiCds[i]);
                                    finalCds.Add(duiCds[i]);
                                }

                                return finalCds.ToArray();
                            }

                            //记录带的部分的个数
                            int addI = 0;
                            for (int i = 0; i < sortedCds.Length; i++)
                            {
                                if (!canLian3Cds.Contains(sortedCds[i]))
                                {
                                    finalCds.Add(sortedCds[i]);
                                    addI++;
                                    if (addI == threeCdsLen)
                                    {
                                        break;
                                    }
                                }
                            }
                            return finalCds.ToArray();*/
        }

        /// <summary>
        /// 找3带
        /// </summary>
        /// <param name="cdSplitStruct"></param>
        /// <returns></returns>
        public int[] FindType3TakeCds(CdSplitStruct cdSplitStruct)
        {
            var sortedCd = cdSplitStruct.SortedCds;
            var sortedCdLen = sortedCd.Length;
            var threeCds = cdSplitStruct.ThreeCds;
            if (threeCds.Count < 1 || sortedCdLen < 5) return null;

            var typ3TakeCds = new List<int>();
            typ3TakeCds.AddRange(new int[3] { threeCds[0], threeCds[0], threeCds[0] });


            //先找三代2个单牌
            var danCds = cdSplitStruct.DanCds;
            if (danCds.Count > 1)
            {
                typ3TakeCds.AddRange(new int[] { danCds[0], danCds[1] });

                return typ3TakeCds.ToArray();
            }


            //再找三代对牌
            var duicds = cdSplitStruct.DuiCds;
            if (danCds.Count > 0)
            {
                typ3TakeCds.Add(duicds[0]);
                typ3TakeCds.Add(duicds[0]);
                return typ3TakeCds.ToArray();
            }

            //以上找不到，就从牌值最小的牌开始找
            for (int i = 0; i < sortedCdLen; i++)
            {
                if (threeCds[0] != sortedCd[i])
                {
                    typ3TakeCds.Add(sortedCd[i]);
                    if (typ3TakeCds.Count == 5) return typ3TakeCds.ToArray();
                }
            }

            return null;
        }

        /// <summary>
        ///  找连对
        /// </summary>
        /// <param name="cdSplitStruct"></param>
        /// <returns></returns>
        public int[] FindType2Abc(CdSplitStruct cdSplitStruct)
        {

            var len = cdSplitStruct.DuiCds.Count + cdSplitStruct.ThreeCds.Count;
            if (len < 3) return null;

            var allpossbCds = new List<int>();
            allpossbCds.AddRange(cdSplitStruct.DuiCds);
            allpossbCds.AddRange(cdSplitStruct.ThreeCds);

            var canLianduiCds = CheckShun(allpossbCds.ToArray(), 3);
            canLianduiCds = RemoveBigCdsInArray(canLianduiCds);
            if (canLianduiCds == null || canLianduiCds.Length < 3) return null;
 
            var typ2AbcList = new List<int>();
            foreach (var cd in canLianduiCds)
            {
                typ2AbcList.Add(cd);
                typ2AbcList.Add(cd);
            }
            return typ2AbcList.ToArray();
       
        }

        /// <summary>
        /// 找顺子
        /// </summary>
        /// <param name="cdSplitStruct"></param>
        /// <returns></returns>
        public int[] FindType123(CdSplitStruct cdSplitStruct)
        {
            var len = cdSplitStruct.DanCds.Count + cdSplitStruct.DuiCds.Count + cdSplitStruct.ThreeCds.Count +
                      cdSplitStruct.FourCds.Count;

            if (len < 5) return null;

            var allPossbCds = new List<int>();

            allPossbCds.AddRange(cdSplitStruct.DanCds);
            allPossbCds.AddRange(cdSplitStruct.DuiCds);
            allPossbCds.AddRange(cdSplitStruct.ThreeCds);
            allPossbCds.AddRange(cdSplitStruct.FourCds);

            var danshun = CheckShun(allPossbCds.ToArray(), 5);
            danshun = RemoveBigCdsInArray(danshun);
            if (danshun == null || danshun.Length < 5) return null;

            return danshun;
        }

        /// <summary>
        /// 找对
        /// </summary>
        /// <param name="cdSplitStruct"></param>
        /// <returns></returns>
        public int[] FindType2(CdSplitStruct cdSplitStruct)
        {
            if (cdSplitStruct.DuiCds.Count > 0) return new[] { cdSplitStruct.DuiCds[0], cdSplitStruct.DuiCds[0] };
            if (cdSplitStruct.ThreeCds.Count > 0) return new[] { cdSplitStruct.ThreeCds[0], cdSplitStruct.ThreeCds[0] };
            if (cdSplitStruct.FourCds.Count > 0) return new[] { cdSplitStruct.FourCds[0], cdSplitStruct.FourCds[0] };

            return null;
        }

        /// <summary>
        /// 找单
        /// </summary>
        /// <param name="cdSplitStruct"></param>
        /// <returns></returns>
        public int[] FindType1(CdSplitStruct cdSplitStruct)
        {

            var sotedCds = cdSplitStruct.SortedCds;

            if (sotedCds.Length > 0)
                return new[] { sotedCds[0] };

            return null;
        }

        /// <summary>
        /// 找到顺牌，不管是连对 还是顺子 还是连三, 最小是连2组,！但！！！检索以后可能还存在 带有2的链子，这种情况，在使用方法后检查
        /// </summary>
        /// <param name="orgcards"></param>
        /// <param name="lenlimit">限制顺牌的最小长度</param>
        /// <returns></returns>
        public int[] CheckShun(int[] orgcards, int lenlimit)
        {
            var cards = PokerRuleUtil.GetSortedValues(orgcards);

            var cardsLen = cards.Length;
            if (cardsLen < lenlimit || lenlimit < 2) return null;

            var canLianCds = new List<int>();
            for (int i = 0; i < cardsLen - 1; i++)
            {
                if (cards[i] == cards[i + 1] - 1)
                {
                    canLianCds.Add(cards[i]);

                    if (i == cardsLen - 2)
                    {
                        canLianCds.Add(cards[i + 1]);
                    }
                }
                else
                {
                    canLianCds.Add(cards[i]);
                    if (canLianCds.Count >= lenlimit)
                    {
                        break;
                    }
                    canLianCds.Clear();
                }
            }

            if (canLianCds.Count >= lenlimit)
                return canLianCds.ToArray();

            return null;
        }


        /// <summary>
        /// 移除数组中的不能组成链子的大牌
        /// </summary>
        /// <param name="cds"></param>
        /// <returns></returns>
        private int[] RemoveBigCdsInArray(IEnumerable<int> cds)
        {
            if (cds == null) return null;

            return cds.Where(cd => PokerRuleUtil.GetValue(cd) < 15).ToArray();
        }

/*
        /// <summary>
        /// 获得一组牌值(不含重复值)中所有可能组成的顺子的碎片（碎片小于合法长度）前提是可能检索出一个合法长度的顺子
        /// </summary>
        /// <param name="cdvalueList"> 里面必须都是单张值不能有重复值</param>
        /// <param name="limitNum">顺的最小合法长度</param>
        /// <returns></returns>
        public List<int[]> GetAllPossibleShun(int[] cdvalueList, int limitNum)
        {

            if (cdvalueList == null || cdvalueList.Length < limitNum) return null;

            //过滤A以上的牌
            var cardsTempList = new List<int>();
            var sortedorgCds = PokerRuleUtil.GetSortedValues(cdvalueList);
            for (int i = 0; i < sortedorgCds.Length; i++)
            {
                if (sortedorgCds[i] > 14) break;

                cardsTempList.Add(sortedorgCds[i]);
            }

            //可以组成顺子的所有牌
            var cards = cardsTempList.ToArray();

            var cardsLen = cards.Length;
            if (cardsLen < limitNum) return null;//总长度小于最小合法长度
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

*/



    }
}
