using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.PokerCdCtrl;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.ConstDefine;

namespace Assets.Scripts.Game.pdk.PokerRule
{
    public class CardManager
    {
        /// <summary>
        /// 提示牌组
        /// </summary>
        private readonly List<int[]>_tishiGroupList = new List<int[]>();
        /// <summary>
        /// 提示牌组对应的牌组和类型
        /// </summary>
        private Dictionary<int[],CardType> _tishiGroupListDic = new Dictionary<int[], CardType>(); 

        /// <summary>
        /// 获得提示牌组
        /// </summary>
        public List<int[]> GetTishiGroupList {
            get { return _tishiGroupList; }
        }
        /// <summary>
        /// 获得提示牌组对应的字典
        /// </summary>
        public Dictionary<int[], CardType> GetTishiGroupDic
        {
            get { return _tishiGroupListDic; }
        }

        /// <summary>
        /// 检查是否可管，lastOutCds不为空，则表示要进行牌型比较，否则只检查upcdList是否是合法牌型就可以了 
        /// </summary>
        /// <param name="upcdList">抬起的手牌</param>
        /// <param name="upcdshasLz">抬起的手牌中是否有赖子</param>
        /// <param name="lastOutCds">如果不为空，表示有要比较的牌</param>
        /// <returns></returns>
        public Dictionary<int[], CardType> CheckCanGuanCds(int[] upcdList, bool upcdshasLz,int[] lastOutCds)
        {
            if (lastOutCds == null || lastOutCds.Length < 1)
            {
                return !upcdshasLz ? PackingCdsToDic(upcdList) : GetAllPosbLzTransTyps(upcdList);
            }

            if (!upcdshasLz)
            {
                //管不起上一手出的牌
                if (!PokerRuleUtil.JustCompareCds(upcdList, lastOutCds)) return null;
                return PackingCdsToDic(upcdList);
            }

            var otCdsallPosbLzDic = GetAllPosbLzTransTyps(upcdList);

            //存储可以管起lastOutCds的牌组dic
            var canOutcdsTypeDic = new Dictionary<int[], CardType>();
            foreach (var cds in otCdsallPosbLzDic.Keys.Where(cds => PokerRuleUtil.JustCompareCds(cds, lastOutCds)))
            {
                canOutcdsTypeDic[cds] = otCdsallPosbLzDic[cds];
            }
            return canOutcdsTypeDic;
        }

        /// <summary>
        /// 把一组不包含纯赖子值的牌组打包 ,牌组值对应的牌类型
        /// </summary>
        /// <returns></returns>
        private Dictionary<int[], CardType> PackingCdsToDic(int[] cds)
        {
            var type = PokerRuleUtil.GetCdsType(cds);
            if (type != CardType.Exception && type != CardType.None)
            {
                var cdTypeDic = new Dictionary<int[], CardType>();
                cdTypeDic[cds] = type;
                return cdTypeDic;
            }
            return null;
        }

        /// <summary>
        /// 获得所有赖子可能组成的的牌组类型
        /// </summary>
        /// <param name="cdsArray">某个可能包含赖子的牌组</param>
        /// <returns></returns>
        private Dictionary<int[], CardType> GetAllPosbLzTransTyps(IEnumerable<int> cdsArray)
        {
            var checkCdtype = new CheckAllposbCdsType(cdsArray);
            return checkCdtype.GetCdsTypeDic();
/*
            //穷举方法找各种可能的类型，效率很低，会生成各种可以忽略掉的类型
            var posbCdType =new Dictionary<int[],CardType>();
            var cdsValueDb = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15};
            var lzTrsnValues = new int[lzNum];
            TraverseAllPosbLz(ref posbCdType, cdsValueDb, pureValueUplist.ToArray(), lzNum, ref lzTrsnValues);
            return posbCdType;*/
        }

        /// <summary>
        /// 遍历所有可能的赖子转换的类型
        /// </summary>
        /// <param name="posbCdType">所有可能由赖子组成的卡牌类型</param>
        /// <param name="cdsValueDb">卡牌可以组成的所有手牌</param>
        /// <param name="pureValueUplist">不带赖子的剩下的牌</param>
        /// <param name="lznum">赖子数量</param>
        /// <param name="lztransValues">赖子转换的值</param>
        private void TraverseAllPosbLz(ref Dictionary<int[],CardType> posbCdType, int[] cdsValueDb, int[] pureValueUplist, int lznum, ref int[] lztransValues)
        {
            if (lznum == 0)
            {
                var allUpCds = new List<int>();
                allUpCds.AddRange(pureValueUplist);
                allUpCds.AddRange(lztransValues);
                var cdsArray = HdCdsCtrl.SortCds(allUpCds);
                var type = PokerRuleUtil.GetCdsType(cdsArray);
                if (type != CardType.Exception && type != CardType.None) posbCdType[cdsArray] = type;
                return;
            }

            var len = cdsValueDb.Length;
            for (int i = 0; i < len; i++)
            {
                lztransValues[lznum - 1] = cdsValueDb[i];
                int nextlznum = lznum - 1;
                TraverseAllPosbLz(ref posbCdType, cdsValueDb, pureValueUplist, nextlznum, ref lztransValues);
            }
        }


        /// <summary>
        ///  根据上一手其他玩家出的牌来，检索出所有可以关的牌，用于提示
        /// </summary>
        /// <param name="handcdsTemp"></param>
        /// <param name="lastoutData"></param>
        public void FindCds(int[] handcdsTemp, ISFSObject lastoutData)
        {
            //清除旧的提示列表
            _tishiGroupList.Clear();
            _tishiGroupi = 0;

            var lastcdType = CardType.None;
            if (lastoutData.ContainsKey(RequestKey.KeyCardType))
                lastcdType = (CardType)lastoutData.GetInt(RequestKey.KeyCardType);
            var tishiTemp = CheckTishiGroup(handcdsTemp, lastoutData.GetIntArray(RequestKey.KeyCards), lastcdType);
            if(tishiTemp==null || tishiTemp.Count<1) return;
            foreach (var cds in tishiTemp)
            {
                _tishiGroupList.Add(cds);
            }

/*            foreach (var cds in dic.Keys)
            {
                _tishiGroupList.Add(cds);
            }*/
          
        }

        /// <summary>
        /// 提示牌组list的下标
        /// </summary>
        private int _tishiGroupi=0;
        public int[] GetOneTishiGroup()
        {
            if (_tishiGroupList.Count < 1) return null;

            if (_tishiGroupi < _tishiGroupList.Count)
                return _tishiGroupList[_tishiGroupi++];

            _tishiGroupi = 1;
            return _tishiGroupList[0];
        }

        /// <summary>
        /// 排除赖子牌因素，检索各种提示的可能
        /// </summary>
        /// <param name="hdCds">不包含赖子的手牌牌值组</param>
        /// <param name="lastoutCds"></param>
        /// <param name="getlastOutCdsType"></param>
        private List<int[]> CheckTishiGroup(int[] hdCds, int[] lastoutCds, CardType getlastOutCdsType = CardType.None)
        {
            var lastOyutType = getlastOutCdsType;
            if (getlastOutCdsType == CardType.None || getlastOutCdsType == CardType.Exception)
                lastOyutType = PokerRuleUtil.GetCdsType(lastoutCds);
            //如果上一手出的牌是天炸，则不用检索提示了

            var cdsTypeDic = new Dictionary<int[], CardType>();
            var cdSplit = new CdSplitStruct(hdCds);

            //var cdsTypeDic = GetAllLegalCdsTypeDic(purehdCdsValue);
            switch (lastOyutType)
            {
                case CardType.C1:
                    CheckTishiGpWhenLstOutC1(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C2:
                    CheckTishiGpWhenLstOutC2(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C3:
                    CheckTishiGpWhenLstOutC3(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C31:
                    CheckTishiGpWhenLstOutC31(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C32:
                    CheckTishiGpWhenLstOutC32(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C111222:
                    CheckTishiGpWhenLstOutC111222(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C1112223434:
                    CheckTishiGpWhenLstOutC1112223434(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C11122234:
                    CheckTishiGpWhenLstOutC11122234(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C1122:
                    CheckTishiGpWhenLstOutC1122(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C123:
                    CheckTishiGpWhenLstOutC123(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C411:
                    CheckTishiGpWhenLstOutC411(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
       /*         case CardType.C422:
                    CheckTishiGpWhenLstOutC422(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;*/
                case CardType.C4:
                    CheckTishiGpWhenLstOutC4(ref cdsTypeDic, cdSplit, lastoutCds);
                    break;
                case CardType.C5://超级炸弹还没考虑
                    break;
                case CardType.C42:
                    return new List<int[]>();
            }


            //把提示牌组，按权值从小到大排序添加----------start--
            var tishiList = new List<int[]>();
            var zhadan = new List<int[]>();
 
            //是不是c1或c2类型
            bool isC1OrC2Type = lastOyutType == CardType.C1 || lastOyutType == CardType.C2;
            //如果是单牌或者对子的类型也要单独拿出来排序
            var danCdsTemp = new List<int[]>();
            var duiCdsTemp = new List<int[]>();
            var sanCdsTemp = new List<int[]>();
            var fourCdsTemp = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] == CardType.C4)
                {
                     zhadan.Add(cds);
                     continue;
                }

                if (isC1OrC2Type)
                {
                    if (cdSplit.DanCds.Contains(cds[0]))
                    {
                        danCdsTemp.Add(cds);
                        continue;
                    }

                    if (cdSplit.DuiCds.Contains(cds[0]))
                    {
                        duiCdsTemp.Add(cds);
                        continue;
                    }

                    if (cdSplit.ThreeCds.Contains(cds[0]))
                    {
                        sanCdsTemp.Add(cds);
                        continue;
                    }

                    if (cdSplit.FourCds.Contains(cds[0]))
                    {
                        fourCdsTemp.Add(cds);
                        continue;
                    }
                }

                tishiList.Add(cds);
            }

            danCdsTemp.Sort(DDzUtil.SortCdsintes);
            duiCdsTemp.Sort(DDzUtil.SortCdsintes);
            sanCdsTemp.Sort(DDzUtil.SortCdsintes);
            fourCdsTemp.Sort(DDzUtil.SortCdsintes);
            tishiList.Sort(DDzUtil.SortCdsintes);
            zhadan.Sort(DDzUtil.SortCdsintes);
            tishiList.AddRange(danCdsTemp);
            tishiList.AddRange(duiCdsTemp);
            tishiList.AddRange(sanCdsTemp);
            tishiList.AddRange(fourCdsTemp);
            tishiList.AddRange(zhadan);
            //-------------------------------------------------------------------------end
            _tishiGroupListDic = cdsTypeDic;

            if (lastOyutType != CardType.C4)
            {
                var fourCds = cdSplit.FourCds;
                var cdsNeedRemove = new List<int[]>();
                foreach (var intse in tishiList)
                {
                    var len = intse.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (fourCds.Contains(intse[i]) && PokerRuleUtil.GetCdsType(intse)!=CardType.C4)
                        {
                            cdsNeedRemove.Add(intse);
                        }
                    }
                }

                foreach (var intse in cdsNeedRemove)
                {
                    tishiList.Remove(intse);
                }
            }

            return tishiList;
        }



        /// <summary>
        /// 获得一个牌中3带的最小的那张牌
        /// </summary>
        /// <param name="cds"></param>
        /// <returns>如果return -1 表示没找到最小的三张那个牌</returns>
        private int GetMinValue3PartOfCds(int[] cds)
        {
            var sortedLastOutCd = PokerRuleUtil.GetSortedValues(cds);
            var dictoNum = new Dictionary<int, int>();
            foreach (var cd in sortedLastOutCd)
            {
                if (!dictoNum.ContainsKey(cd))
                {
                    dictoNum[cd] = 1;
                    continue;
                }
                dictoNum[cd]++;
            }

            foreach (var cd in dictoNum.Keys.Where(cd => dictoNum[cd] == 3))
            {
                return cd;
            }

            return -1;
        }

        /// <summary>
        /// 获得一个牌中4带的最大的那张牌
        /// </summary>
        /// <param name="cds"></param>
        /// <returns>如果return -1 表示没找到最大的4张那个牌</returns>
        private int GetMaxValue4PartOfCds(int[] cds)
        {
            var sortedLastOutCd = PokerRuleUtil.GetSortedValues(cds);
            var dictoNum = new Dictionary<int, int>();
            foreach (var cd in sortedLastOutCd)
            {
                if (!dictoNum.ContainsKey(cd))
                {
                    dictoNum[cd] = 1;
                    continue;
                }
                dictoNum[cd]++;
            }
            var last4Cd = -1;
            foreach (var cd in dictoNum.Keys)
            {
                if (dictoNum[cd] == 4) last4Cd = cd;
            }
            return last4Cd;
        }

        /// <summary>
        /// 直接赋值大牌组类型，炸弹，天炸等，能管大部分关别的类型
        /// </summary>
        /// <param name="cdsTypeDic"></param>
        /// <param name="cdSplit"></param>
        private void SetBigCdsTypeDic(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            CheckC4(ref cdsTypeDic, cdSplit);
            CheckC42(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC1(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit,int[] lastoutCds)
        {
            CheckAllC1(ref cdsTypeDic, cdSplit);
            var lastoutcdValues = PokerRuleUtil.GetSortedValues(lastoutCds);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] == CardType.C1 && cds[0] <= lastoutcdValues[0]) intsNeedRemove.Add(cds);
            }
            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }

            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC2(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit, int[] lastoutCds)
        {
            CheckAllC2(ref cdsTypeDic, cdSplit);
            var lastoutcdValues = PokerRuleUtil.GetSortedValues(lastoutCds);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] == CardType.C2 && cds[0] <= lastoutcdValues[0]) intsNeedRemove.Add(cds);
            }
            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }

            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC3(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit, int[] lastoutCds)
        {
            CheckAllC3(ref cdsTypeDic, cdSplit);
            var lastoutcdValues = PokerRuleUtil.GetSortedValues(lastoutCds);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] == CardType.C3 && cds[0] <= lastoutcdValues[0]) intsNeedRemove.Add(cds);
            }
            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }
            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC31(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit, int[] lastoutCds)
        {
            var minlastotCdvalue = GetMinValue3PartOfCds(lastoutCds);
            if (minlastotCdvalue == -1) return;
           
            CheckAllC31(ref cdsTypeDic, cdSplit);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                //要保证CheckAllC31这个方法执行后cds[0] 代表的是三牌的部分
                if (cdsTypeDic[cds] == CardType.C31 && cds[0] <= minlastotCdvalue) intsNeedRemove.Add(cds);
            }
            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }

            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC32(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit, int[] lastoutCds)
        {

            var minlastotCdvalue = GetMinValue3PartOfCds(lastoutCds);
            if (minlastotCdvalue == -1) return;
            CheckAllC32(ref cdsTypeDic, cdSplit);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                //要保证CheckAllC31这个方法执行后cds[0] 代表的是三牌的部分
                if (cdsTypeDic[cds] == CardType.C32 && cds[0] <= minlastotCdvalue) intsNeedRemove.Add(cds);
            }
            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }
            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC111222(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit,
                                               int[] lastoutCds)
        {
            var minlastotCdvalue = GetMinValue3PartOfCds(lastoutCds);
            if (minlastotCdvalue == -1) return;

            CheckAllC111222(ref cdsTypeDic, cdSplit);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if(cdsTypeDic[cds] != CardType.C111222) continue;

                if (cds[0] <= minlastotCdvalue || cds.Length != lastoutCds.Length) intsNeedRemove.Add(cds);
            }

            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }
            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC1112223434(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit,
                                       int[] lastoutCds)
        {
            var minlastotCdvalue = GetMinValue3PartOfCds(lastoutCds);
            if (minlastotCdvalue == -1) return;

            CheckC1112223434(ref cdsTypeDic, cdSplit);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] != CardType.C1112223434 ) continue;

                //长度不同的C1112223434 要移除
                if (cds.Length != lastoutCds.Length)
                {
                    intsNeedRemove.Add(cds);
                    continue;
                }

                //小于lastoutCds的C1112223434要移除
                var minvaluecurCds = GetMinValue3PartOfCds(cds);
                if (minvaluecurCds <= minlastotCdvalue) intsNeedRemove.Add(cds);
            }

            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }

            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC11122234(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit,
                               int[] lastoutCds)
        {
            var minlastotCdvalue = GetMinValue3PartOfCds(lastoutCds);
            if (minlastotCdvalue == -1) return;

            CheckC11122234(ref cdsTypeDic, cdSplit);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] != CardType.C11122234) continue;

                if (cds.Length != lastoutCds.Length)
                {
                    intsNeedRemove.Add(cds);
                    continue;
                }

                var minvaluecurCds = GetMinValue3PartOfCds(cds);
                if (minvaluecurCds <= minlastotCdvalue) intsNeedRemove.Add(cds);
            }

            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }

            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC1122(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit,
                                                     int[] lastoutCds)
        {
            CheckAllC1122(ref cdsTypeDic, cdSplit);
            var lastoutcdValues = PokerRuleUtil.GetSortedValues(lastoutCds);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] != CardType.C1122) continue;

                if (cds.Length != lastoutCds.Length)
                {
                    intsNeedRemove.Add(cds);
                    continue;
                }

                if (cds[0] <= lastoutcdValues[0]) intsNeedRemove.Add(cds);
            }
            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }
            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC123(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit,
                                             int[] lastoutCds)
        {
            CheckAllC123(ref cdsTypeDic, cdSplit);
            var lastoutcdValues = PokerRuleUtil.GetSortedValues(lastoutCds);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] != CardType.C123) continue;

                if (cds.Length != lastoutCds.Length)
                {
                    intsNeedRemove.Add(cds);
                    continue;
                }

                if (cds[0] <= lastoutcdValues[0]) intsNeedRemove.Add(cds);
            }
            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }
            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

        private void CheckTishiGpWhenLstOutC411(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit,
                                                int[] lastoutCds)
        {

            Check411(ref cdsTypeDic, cdSplit);
            Check422(ref cdsTypeDic, cdSplit);

            var maxlast4Cd = GetMaxValue4PartOfCds(lastoutCds);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] != CardType.C411) continue;

                if (cds.Length != lastoutCds.Length)
                {
                    intsNeedRemove.Add(cds);
                    continue;
                }

                var maxcur4Cd = GetMaxValue4PartOfCds(cds);
                if (maxcur4Cd <= maxlast4Cd) intsNeedRemove.Add(cds);
            }

            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }

            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }

/*        private void CheckTishiGpWhenLstOutC422(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit,
                                        int[] lastoutCds)
        {

            Check422(ref cdsTypeDic, cdSplit);

            var maxlast4Cd = GetMaxValue4PartOfCds(lastoutCds);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] != CardType.C422) continue;

                if (cds.Length != lastoutCds.Length)
                {
                    intsNeedRemove.Add(cds);
                    continue;
                }

                var maxcur4Cd = GetMaxValue4PartOfCds(cds);
                if (maxcur4Cd <= maxlast4Cd) intsNeedRemove.Add(cds);
            }


            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }

            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
        }*/

        private void CheckTishiGpWhenLstOutC4(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit,
                                                int[] lastoutCds)
        {
            SetBigCdsTypeDic(ref cdsTypeDic, cdSplit);
            var intsNeedRemove = new List<int[]>();
            foreach (var cds in cdsTypeDic.Keys)
            {
                if (cdsTypeDic[cds] == CardType.C4 && cds[0] <= PokerRuleUtil.GetValue(lastoutCds[0])) intsNeedRemove.Add(cds);
            }


            foreach (var intse in intsNeedRemove)
            {
                cdsTypeDic.Remove(intse);
            }
        }

/*
        /// <summary>
        /// 分析某一组牌中所有合法的牌型(不考虑赖子情况)
        /// </summary>
        /// <param name="cds">不带纯赖子值的牌组</param>
        /// <returns>返回纯牌值和类型组合的字典</returns>
        private Dictionary<int[], CardType> GetAllLegalCdsTypeDic(int[] cds)
        {
            var cdsTypeDic = new Dictionary<int[], CardType>();
            var cdSplit = new CdSplitStruct(cds);
            CheckAllC1(ref cdsTypeDic, cdSplit);
            CheckAllC2(ref cdsTypeDic, cdSplit);
            CheckAllC3(ref cdsTypeDic, cdSplit);
            CheckAllC31(ref cdsTypeDic, cdSplit);
            CheckAllC32(ref cdsTypeDic, cdSplit);
            CheckAllC123(ref cdsTypeDic, cdSplit);
            CheckAllC1122(ref cdsTypeDic, cdSplit);
            CheckAllC111222(ref cdsTypeDic, cdSplit);
            CheckC11122234(ref cdsTypeDic, cdSplit);
            CheckC1112223434(ref cdsTypeDic, cdSplit);
            CheckC4(ref cdsTypeDic, cdSplit);
            Check411(ref cdsTypeDic, cdSplit);
            Check422(ref cdsTypeDic, cdSplit);
            CheckC42(ref cdsTypeDic, cdSplit);
            return cdsTypeDic;
        }
*/

        //找所有可能的单牌
        private void CheckAllC1(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            var list = new List<int>();
            list.AddRange(cdSplit.DanCds);
            list.AddRange(cdSplit.DuiCds);
            list.AddRange(cdSplit.ThreeCds);
            list.AddRange(cdSplit.FourCds);
            foreach (var cd in list)
            {
                cdsTypeDic[new int[]{cd}] = CardType.C1;
            }
        }

        //找所有可能的对牌,不破坏炸弹
        private void CheckAllC2(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            foreach (var duicd in cdSplit.DuiCds)
            {
                cdsTypeDic[new int[] { duicd, duicd }] = CardType.C2;
            }
            foreach (var sancd in cdSplit.ThreeCds)
            {
                cdsTypeDic[new int[] { sancd, sancd }] = CardType.C2;
            }
        }

        /// <summary>
        /// 检索c3但不破坏炸弹
        /// </summary>
        /// <param name="cdsTypeDic"></param>
        /// <param name="cdSplit"></param>
        private void CheckAllC3(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            foreach (var threeCd in cdSplit.ThreeCds)
            {
                cdsTypeDic[new int[] { threeCd, threeCd, threeCd }] = CardType.C3;
            }
        }

        /// <summary>
        /// 检索c31的情况 但不破坏炸弹
        /// </summary>
        /// <param name="cdsTypeDic"></param>
        /// <param name="cdSplit"></param>
        private void CheckAllC31(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            foreach (var threeCd in cdSplit.ThreeCds)
            {
                //找c31
                //如果有单牌，找单牌最小
                if (cdSplit.DanCds.Count > 0) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.DanCds[0] }] = CardType.C31;
                //对牌里面找单
                else if (cdSplit.DuiCds.Count > 0) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.DuiCds[0] }] = CardType.C31;
                //在3牌里面找
                else if (cdSplit.ThreeCds.Count > 1)
                {
                    if (threeCd != cdSplit.ThreeCds[0]) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.ThreeCds[0] }] = CardType.C31;
                    else cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.ThreeCds[1] }] = CardType.C31;
                }
            }
        }

        /// <summary>
        /// 检索c32的情况 但不破坏炸弹
        /// </summary>
        /// <param name="cdsTypeDic"></param>
        /// <param name="cdSplit"></param>
        private void CheckAllC32(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            var sortedCds = cdSplit.SortedCds;
            var threeCds = cdSplit.ThreeCds;
            if(sortedCds.Length<5) return;
            foreach (var threeCd in threeCds)
            {
                var cds = new int[5];
                cds[0] = threeCd;
                cds[1] = threeCd;
                cds[2] = threeCd;

                var i = 3;
                var cdTemp = threeCd;
                foreach (var cd in sortedCds.Where(cd => cd != cdTemp))
                {
                    cds[i++] = cd;
                    if (i > 4) break;
                }
                cdsTypeDic[cds] = CardType.C32;

/*                //找c32
                //在对牌里面找
                if (cdSplit.DuiCds.Count > 0) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.DuiCds[0], cdSplit.DuiCds[0] }] = CardType.C32;
                //在3牌里面找
                else if (cdSplit.ThreeCds.Count > 1)
                {
                    if (threeCd != cdSplit.ThreeCds[0]) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.ThreeCds[0], cdSplit.ThreeCds[0] }] = CardType.C32;
                    else cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.ThreeCds[1], cdSplit.ThreeCds[1] }] = CardType.C32;
                }*/
            }
        }

        /// <summary>
        /// 检索一组牌中所有的单龙放入cdsTypeDic
        /// </summary>
        /// <param name="cdsTypeDic">牌组对应类型的字典</param>
        /// <param name="cdSplit">牌组经过划分整理的容器</param>
        private void CheckAllC123(ref Dictionary<int[], CardType> cdsTypeDic,CdSplitStruct cdSplit)
        {
            var listTemp = new List<int>();
            listTemp.AddRange(cdSplit.DanCds);
            listTemp.AddRange(cdSplit.DuiCds);
            listTemp.AddRange(cdSplit.ThreeCds);
            listTemp.AddRange(cdSplit.FourCds);

            var allshunList = PokerRuleUtil.AnalyAllPartOfShun(listTemp.ToArray(), 5);
            foreach (var cds in allshunList)
            {
                cdsTypeDic[cds] = CardType.C123;
            }
        }

        /// <summary>
        /// 检索一组牌中所有的连对放入cdsTypeDic
        /// </summary>
        /// <param name="cdsTypeDic">牌组对应类型的字典</param>
        /// <param name="cdSplit">牌组经过划分整理的容器</param>
        private void CheckAllC1122(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            var listTemp = new List<int>();
            listTemp.AddRange(cdSplit.DuiCds);
            listTemp.AddRange(cdSplit.ThreeCds);
            listTemp.AddRange(cdSplit.FourCds);

            var allshunList = PokerRuleUtil.AnalyAllPartOfShun(listTemp.ToArray(),2);
            foreach (var cds in allshunList)
            {
                //把cds扩充为连对儿
                var duiCdsList = new List<int>();
                var cdsLen = cds.Length;
                for (int i = 0; i < cdsLen; i++)
                {
                    duiCdsList.AddRange(new int[] { cds[i],cds[i]});
                }
                cdsTypeDic[duiCdsList.ToArray()] = CardType.C1122;
            }
        }
        /// <summary>
        /// 检索一组牌中所有的C111222
        /// </summary>
        /// <param name="cdsTypeDic">牌组对应类型的字典</param>
        /// <param name="cdSplit">牌组经过划分整理的容器</param>
        private void CheckAllC111222(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            var listTemp = new List<int>();
            listTemp.AddRange(cdSplit.ThreeCds);
            listTemp.AddRange(cdSplit.FourCds);

            var allshunList = PokerRuleUtil.AnalyAllPartOfShun(listTemp.ToArray(), 2);
            foreach (var cds in allshunList)
            {
                //把cds扩充为连三儿
                var duiCdsList = new List<int>();
                var cdsLen = cds.Length;
                for (int i = 0; i < cdsLen; i++)
                {
                    duiCdsList.AddRange(new int[] { cds[i], cds[i], cds[i] });
                }
                cdsTypeDic[duiCdsList.ToArray()] = CardType.C111222;
            }
        }

        private void CheckC11122234(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            var listTemp = new List<int>();
            listTemp.AddRange(cdSplit.ThreeCds);
            listTemp.AddRange(cdSplit.FourCds);

            var allshunList = PokerRuleUtil.AnalyAllPartOfShun(listTemp.ToArray(), 2);
            foreach (var cds in allshunList)
            {
                var pureSanValues = cds;

                //把不包含在3牌部分的3牌，分为3个单牌放入
                var threeCdsNotInThree = cdSplit.ThreeCds.Where(cd => !pureSanValues.Any(v => v == cd)).ToList();
                //找到已经被拆为3牌的fourcds
                var fourCdsInthree = cdSplit.FourCds.Where(cd => pureSanValues.Any(v => v == cd));
                //找到已经被拆为3牌的fourcds
                var fourCdsnotInThree = cdSplit.FourCds.Where(cd => !pureSanValues.Any(v => v == cd)).ToList();

                //先找c11122234------------------------------------start---
                //把所有的不包含在pureSanValues中的牌值找出来
                var danCdsList = new List<int>();
                danCdsList.AddRange(cdSplit.DanCds);
                //添加4牌里面
                danCdsList.AddRange(fourCdsInthree);
                //把对牌分为2个单牌放入
                foreach (var cd in cdSplit.DuiCds)
                {
                    danCdsList.AddRange(new int[] { cd, cd });
                }
                //把不包含三连中的三牌放入
                foreach (var cd in threeCdsNotInThree)
                {
                    danCdsList.AddRange(new int[] { cd, cd, cd });
                }
                //把不包含三连中的4牌放入
                foreach (var cd in fourCdsnotInThree)
                {
                    danCdsList.AddRange(new int[] { cd, cd, cd, cd });
                }
                var sanLen = pureSanValues.Length;
                //如果C11122234找不到，C1112223344也不可能找到
                if (danCdsList.Count < sanLen) return;

                var c11122234Cds = new List<int>();
                var danPartCds = new int[sanLen];
                for (var i = 0; i < sanLen; i++)
                {
                    //添加三连部分
                    c11122234Cds.AddRange(new int[] { pureSanValues[i], pureSanValues[i], pureSanValues[i] });
                    //添加单牌部分
                    danPartCds[i] = danCdsList[i];
                }
                //和并为C11122234的牌组
                c11122234Cds.AddRange(danPartCds);
                cdsTypeDic[c11122234Cds.ToArray()] = CardType.C11122234;
                //-----------------------------------------------------------------------------------------------end;
            }
        }

        /// <summary>
        /// 检查飞机带2张
        /// </summary>
        /// <param name="cdsTypeDic"></param>
        /// <param name="cdSplit"></param>
        private void CheckC1112223434(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            var listTemp = new List<int>();
            listTemp.AddRange(cdSplit.ThreeCds);
            listTemp.AddRange(cdSplit.FourCds);

            var allshunList = PokerRuleUtil.AnalyAllPartOfShun(listTemp.ToArray(), 2);


            var dancds = cdSplit.DanCds;
            //存在于三顺中的4牌
            var fourInDanCds = new List<int>();
            //把对子分解成单牌
            var duiToDanCds = new List<int>();
            foreach (var duiCd in cdSplit.DuiCds)
            {
                duiToDanCds.AddRange(new int[] { duiCd, duiCd });
            }
            //不存在与三顺中的3牌
            var threeToDancds = new List<int>();
            //不存在三顺中的4牌
            var fourToDanCds = new List<int>();

            var otherPartCdsList = new List<int>();

            foreach (var pureSanValues in allshunList)
            {
             
                threeToDancds.Clear();
                var values = pureSanValues;
                foreach (var threeCd in cdSplit.ThreeCds.Where(threeCd => !values.Contains(threeCd)))
                {
                    threeToDancds.AddRange(new int[] { threeCd, threeCd, threeCd});
                }

                fourInDanCds.Clear();
                fourToDanCds.Clear();
                foreach (var fcd in cdSplit.FourCds)
                {
                    if (!values.Contains(fcd))
                    {
                        fourToDanCds.AddRange(new int[] { fcd, fcd, fcd, fcd });
                    }
                    else
                    {
                        fourInDanCds.Add(fcd);
                    }
                }

                otherPartCdsList.Clear();
                otherPartCdsList.AddRange(dancds);
                otherPartCdsList.AddRange(fourInDanCds);
                otherPartCdsList.AddRange(duiToDanCds);
                otherPartCdsList.AddRange(threeToDancds);
                otherPartCdsList.AddRange(fourToDanCds);

                var sanLen = pureSanValues.Length;
                if (otherPartCdsList.Count*2 >= sanLen)
                {
                    var c1112223434Cds = new List<int>();
                    var otherptCds = new List<int>();
                    int j = 0;
                    for (var i = 0; i < sanLen; i++)
                    {
                        //添加三连部分
                        c1112223434Cds.AddRange(new int[] { pureSanValues[i], pureSanValues[i], pureSanValues[i] });
                        //添加带的牌部分
                        otherptCds.Add(otherPartCdsList[j++]);
                        otherptCds.Add(otherPartCdsList[j++]);
                    }
                    c1112223434Cds.AddRange(otherptCds);
                    cdsTypeDic[c1112223434Cds.ToArray()] = CardType.C1112223434;
/*                    var c1112223344Cds = new List<int>();
                    var duipartcds = new List<int>();
                    for (var i = 0; i < sanLen; i++)
                    {
                        //添加三连部分
                        c1112223344Cds.AddRange(new int[] { pureSanValues[i], pureSanValues[i], pureSanValues[i] });
                        duipartcds.AddRange(new int[] { duiCdsList[i], duiCdsList[i] });
                    }
                    //和并为C1112223344的牌组
                    c1112223344Cds.AddRange(duipartcds);
                    cdsTypeDic[c1112223344Cds.ToArray()] = CardType.C1112223434;*/
                }
            }
        }

        private void CheckC4(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            //cdsTypeDic填入炸弹c4
            foreach (var cd in cdSplit.FourCds)
            {
                cdsTypeDic[new int[] { cd, cd, cd, cd }] = CardType.C4;
            }
        }

        private void Check411(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {

            //找411
            //所有不包含炸弹的单值
            var danListTemp = new List<int>();
            danListTemp.AddRange(cdSplit.DanCds);
            foreach (var cd in cdSplit.DuiCds)
            {
                danListTemp.AddRange(new int[] { cd, cd });
            }
            foreach (var cd in cdSplit.ThreeCds)
            {
                danListTemp.AddRange(new int[] { cd, cd, cd });
            }

            //检查是否可以有411
            if (danListTemp.Count + (cdSplit.FourCds.Count - 1) * 4 < 2) return;


            foreach (var cd in cdSplit.FourCds)
            {
                var allCanbeDancds = new List<int>();
                allCanbeDancds.AddRange(danListTemp);
                var curFourCd = cd;
                foreach (var fcd in cdSplit.FourCds.Where(fcd => fcd != curFourCd))
                {
                    allCanbeDancds.AddRange(new int[] { fcd, fcd, fcd, fcd });
                }

                cdsTypeDic[new int[] { cd, cd, cd, cd, allCanbeDancds[0], allCanbeDancds[1] }] = CardType.C411;
            }
        }

        private void Check422(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            //找422
            var duiValueList = new List<int>();
            duiValueList.AddRange(cdSplit.DuiCds);
            duiValueList.AddRange(cdSplit.ThreeCds);
            if (duiValueList.Count + (cdSplit.FourCds.Count - 1) * 2 < 2) return;
            foreach (var cd in cdSplit.FourCds)
            {
                var allCanbeDuicds = new List<int>();
                allCanbeDuicds.AddRange(duiValueList);
                var curFourCd = cd;
                foreach (var fcd in cdSplit.FourCds.Where(fcd => fcd != curFourCd))
                {
                    allCanbeDuicds.AddRange(new int[] { fcd, fcd });
                }

                cdsTypeDic[new int[] { cd, cd, cd, cd, allCanbeDuicds[0], allCanbeDuicds[0], allCanbeDuicds[1], allCanbeDuicds[1] }] = CardType.C411;
            }
        }

        /// <summary>
        /// 找天炸
        /// </summary>
        /// <param name="cdsTypeDic"></param>
        /// <param name="cdSplit"></param>
        private void CheckC42(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            if (cdSplit.DanCds.Contains(PokerRuleUtil.BigJoker) && cdSplit.DanCds.Contains(PokerRuleUtil.SmallJoker))
            {
                cdsTypeDic[new int[] { PokerRuleUtil.SmallJoker, PokerRuleUtil.BigJoker }] = CardType.C42;
            }
        }





/*

        /// <summary>
        /// 找c3,c31,c32
        /// </summary>
        /// <param name="cdsTypeDic"></param>
        /// <param name="cdSplit"></param>
        private void CheckAllC3C31C32(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            //检索对牌c3,c31,c32
            foreach (var threeCd in cdSplit.ThreeCds)
            {
                //找c3
                cdsTypeDic[new int[] { threeCd, threeCd, threeCd }] = CardType.C3;

                //找c31
                //如果有单牌，找单牌最小
                if (cdSplit.DanCds.Count > 0) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.DanCds[0] }] = CardType.C31;
                //对牌里面找单
                else if (cdSplit.DuiCds.Count > 0) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.DuiCds[0] }] = CardType.C31;
                //在3牌里面找
                else if (cdSplit.ThreeCds.Count > 1)
                {
                    if (threeCd != cdSplit.ThreeCds[0]) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.ThreeCds[0] }] = CardType.C31;
                    else cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.ThreeCds[1] }] = CardType.C31;
                }

                //找c32
                //在对牌里面找
                if (cdSplit.DuiCds.Count > 0) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.DuiCds[0], cdSplit.DuiCds[0] }] = CardType.C32;
                //在3牌里面找
                else if (cdSplit.ThreeCds.Count > 1)
                {
                    if (threeCd != cdSplit.ThreeCds[0]) cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.ThreeCds[0], cdSplit.ThreeCds[0] }] = CardType.C32;
                    else cdsTypeDic[new int[] { threeCd, threeCd, threeCd, cdSplit.ThreeCds[1], cdSplit.ThreeCds[1] }] = CardType.C32;
                }

            }
        }
        
        
        /// <summary>
        /// 检测某一组连3牌可以组成的C11122234 ，C1112223434
        /// </summary>
        /// <param name="pureSanValues">连3的纯牌值，不包括数量</param>
        /// <param name="cdsTypeDic">牌组对应类型的字典</param>
        /// <param name="cdSplit">牌组经过划分整理的容器</param>
        private void CheckC111222343344(int[] pureSanValues, ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            //把不包含在3牌部分的3牌，分为3个单牌放入
            var threeCdsNotInThree = cdSplit.ThreeCds.Where(cd => !pureSanValues.Any(v => v == cd)).ToList();
            //找到已经被拆为3牌的fourcds
            var fourCdsInthree = cdSplit.FourCds.Where(cd => pureSanValues.Any(v => v == cd));
            //找到已经被拆为3牌的fourcds
            var fourCdsnotInThree = cdSplit.FourCds.Where(cd => !pureSanValues.Any(v => v == cd)).ToList();

            //先找c11122234------------------------------------start---
            //把所有的不包含在pureSanValues中的牌值找出来
            var danCdsList = new List<int>();
            danCdsList.AddRange(cdSplit.DanCds);
            //添加4牌里面
            danCdsList.AddRange(fourCdsInthree);
            //把对牌分为2个单牌放入
            foreach (var cd in cdSplit.DuiCds)
            {
                danCdsList.AddRange(new int[] { cd, cd });
            }
            //把不包含三连中的三牌放入
            foreach (var cd in threeCdsNotInThree)
            {
                danCdsList.AddRange(new int[] { cd, cd, cd });
            }
            //把不包含三连中的4牌放入
            foreach (var cd in fourCdsnotInThree)
            {
                danCdsList.AddRange(new int[] { cd, cd, cd, cd });
            }
            var sanLen = pureSanValues.Length;
            //如果C11122234找不到，C1112223344也不可能找到
            if (danCdsList.Count < sanLen) return;

            var c11122234Cds = new List<int>();
            var danPartCds = new int[sanLen];
            for (var i = 0; i < sanLen; i++)
            {
                //添加三连部分
                c11122234Cds.AddRange(new int[] { pureSanValues[i], pureSanValues[i], pureSanValues[i] });
                //添加单牌部分
                danPartCds[i] = danCdsList[i];
            }
            //和并为C11122234的牌组
            c11122234Cds.AddRange(danPartCds);
            cdsTypeDic[c11122234Cds.ToArray()] = CardType.C11122234;
            //-----------------------------------------------------------------------------------------------end;

            //再找c1112223344
            var duiCdsList = new List<int>();
            duiCdsList.AddRange(cdSplit.DuiCds);
            duiCdsList.AddRange(threeCdsNotInThree);
            foreach (var fourCd in fourCdsnotInThree)
            {
                duiCdsList.AddRange(new int[] { fourCd, fourCd });
            }
            if (duiCdsList.Count >= sanLen)
            {
                var c1112223344Cds = new List<int>();
                var duipartcds = new List<int>();
                for (var i = 0; i < sanLen; i++)
                {
                    //添加三连部分
                    c1112223344Cds.AddRange(new int[] { pureSanValues[i], pureSanValues[i], pureSanValues[i] });
                    duipartcds.AddRange(new int[] { duiCdsList[i], duiCdsList[i] });
                }
                //和并为C1112223344的牌组
                c1112223344Cds.AddRange(duipartcds);
                cdsTypeDic[c1112223344Cds.ToArray()] = CardType.C1112223434;
            }

        }

        /// <summary>
        /// 检查c4,c411,c422
        /// </summary>
        /// <param name="cdsTypeDic"></param>
        /// <param name="cdSplit"></param>
        private void CheckC4AndC411AndC422(ref Dictionary<int[], CardType> cdsTypeDic, CdSplitStruct cdSplit)
        {
            //cdsTypeDic填入炸弹c4
            foreach (var cd in cdSplit.FourCds)
            {
                cdsTypeDic[new int[] { cd, cd, cd, cd }] = CardType.C4;
            }


            //找411
            //所有不包含炸弹的单值
            var danListTemp = new List<int>();
            danListTemp.AddRange(cdSplit.DanCds);
            foreach (var cd in cdSplit.DuiCds)
            {
                danListTemp.AddRange(new int[] { cd, cd });
            }
            foreach (var cd in cdSplit.ThreeCds)
            {
                danListTemp.AddRange(new int[] { cd, cd, cd });
            }

            //如果没有c411 ，则更没有可能有422
            if (danListTemp.Count + (cdSplit.FourCds.Count - 1) * 4 < 2) return;
            foreach (var cd in cdSplit.FourCds)
            {
                var allCanbeDancds = new List<int>();
                allCanbeDancds.AddRange(danListTemp);
                var curFourCd = cd;
                foreach (var fcd in cdSplit.FourCds.Where(fcd => fcd != curFourCd))
                {
                    allCanbeDancds.AddRange(new int[] { fcd, fcd, fcd, fcd });
                }

                cdsTypeDic[new int[] { cd, cd, cd, cd, allCanbeDancds[0], allCanbeDancds[1] }] = CardType.C411;
            }


            //找422
            var duiValueList = new List<int>();
            duiValueList.AddRange(cdSplit.DuiCds);
            duiValueList.AddRange(cdSplit.ThreeCds);
            if (duiValueList.Count + (cdSplit.FourCds.Count - 1) * 2 < 2) return;
            //找411
            foreach (var cd in cdSplit.FourCds)
            {
                var allCanbeDuicds = new List<int>();
                allCanbeDuicds.AddRange(duiValueList);
                var curFourCd = cd;
                foreach (var fcd in cdSplit.FourCds.Where(fcd => fcd != curFourCd))
                {
                    allCanbeDuicds.AddRange(new int[] { fcd, fcd });
                }

                cdsTypeDic[new int[] { cd, cd, cd, cd, allCanbeDuicds[0], allCanbeDuicds[0], allCanbeDuicds[1], allCanbeDuicds[1] }] = CardType.C422;
            }
        }

*/

/*        public static void test()
        {
            CheckAllposbCdsType d = new CheckAllposbCdsType(new int[] { 0x16,0x17, 0x27, 0x37, 0x47, PokerRuleUtil.MagicKing });
            var dic = d.GetCdsTypeDic();

            foreach (var cds in dic.Keys)
            {
                var type = dic[cds];

                string str = type.ToString() + ": ";

                foreach (var cd in cds)
                {
                    str += " " + cd;
                }

                Debug.LogError(str);
            }
        }*/
    }

    /// <summary>
    /// 在有赖子牌的情况下检索所有肯能的牌型
    /// </summary>
    internal class CheckAllposbCdsType
    {
        /// <summary>
        /// 牌组中排除掉赖子以后的剩余牌
        /// </summary>
        protected readonly List<int> CdsWithOutLzList = new List<int>();

        /// <summary>
        /// 牌组中的赖子数量
        /// </summary>
        protected readonly int LaiZiNum;


        public CheckAllposbCdsType(IEnumerable<int> cdsArray)
        {
            //把赖子牌过滤出来
            foreach (var cd in cdsArray)
            {
                if (cd == HdCdsCtrl.MagicKing)
                {
                    LaiZiNum++;
                    continue;
                }
                CdsWithOutLzList.Add(cd);
            }
        }

        public Dictionary<int[], CardType> GetCdsTypeDic()
        {
            if (CdsWithOutLzList.Count > 0 && LaiZiNum <= 0)
            {
                var cds = CdsWithOutLzList.ToArray();

                var type = PokerRuleUtil.GetCdsType(cds);
                var cdstypeDic = new Dictionary<int[], CardType>();
                cdstypeDic[cds] = type;
                if (type == CardType.Exception || type == CardType.None) return null;
                return cdstypeDic;
            }

            if (CdsWithOutLzList.Count <= 0) return null;


            var totalNum = CdsWithOutLzList.Count + LaiZiNum;

            switch (totalNum)
            {
                case 2:
                    return new GetCdsTypeToDicCdsLen2(CdsWithOutLzList, LaiZiNum).GetCdsTypeDic();
                case 3:
                    return null;//new GetCdsTypeToDicCdsLen3(CdsWithOutLzList, LaiZiNum).GetCdsTypeDic();
                case 4:
                    return new GetCdsTypeToDicCdsLen4(CdsWithOutLzList, LaiZiNum).GetCdsTypeDic();
            }

            //以下考虑的情况是确定赖子只会有1个的时候做的处理，现在玩法中赖子只能有1个，所以可以用，以后赖子需求多个时，在扩展GetCdsTypeToDicCdsLen5类，深入开发-----------------start


            var cdsValueDb = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            //包括赖子转换值的所有牌的数组
            var purecdslen = CdsWithOutLzList.Count;
            var compcds = new int[purecdslen+1];
            CdsWithOutLzList.ToArray().CopyTo(compcds,0);

            var lzcdstypeDic = new Dictionary<int[], CardType>();
            //用这个list排除相同卡牌类型的int[]
            var keyList = new List<CardType>();
            keyList.AddRange(new CardType[]{CardType.C1, CardType.C1112223434, 
               CardType.C1122, CardType.C123, CardType.C2,
                CardType.C32, CardType.C4, });
            //只有一个赖子的情况下，赖子从大到小转换，就能把每种类型最合适的类型找到
            for (var i = cdsValueDb.Length - 1; i >= 0; i--)
            {
                compcds[purecdslen] = cdsValueDb[i];
                var type = PokerRuleUtil.GetCdsType(compcds);
                if (keyList.Contains(type))
                {
                    var cds = new int[compcds.Length];
                    compcds.CopyTo(cds, 0);
                    lzcdstypeDic[PokerRuleUtil.SortCds(cds)] = type;
                    keyList.Remove(type);
                }
            }

            return lzcdstypeDic;
            //---------------------------------------end
        }
    }



    internal abstract class GetCardsTypetoDic
    {
        /// <summary>
        /// 牌组中排除掉赖子以后的剩余牌
        /// </summary>
        protected readonly List<int> CdsWithOutLzList = new List<int>();

        
        /// <summary>
        /// 牌组中的赖子数量
        /// </summary>
        protected readonly int LaiZiNum;

        protected GetCardsTypetoDic(List<int> cdsWithOutLzList, int laiziNum)
        {
            CdsWithOutLzList = cdsWithOutLzList;

            LaiZiNum = laiziNum;
        }
        public abstract Dictionary<int[], CardType> GetCdsTypeDic();


    }
    
    /// <summary>
    /// 当含赖子的牌组长度为2时
    /// </summary>
    internal class GetCdsTypeToDicCdsLen2 : GetCardsTypetoDic
    {
        public GetCdsTypeToDicCdsLen2(List<int> cdsWithOutLzList, int laiziNum) : base(cdsWithOutLzList, laiziNum)
        {
        }

        public override Dictionary<int[], CardType> GetCdsTypeDic()
        {
            if (CdsWithOutLzList.Count == 1 && LaiZiNum == 1)
            {
                var cdstypeDic = new Dictionary<int[], CardType>();
                var cds = new int[2] { CdsWithOutLzList[0], PokerRuleUtil.GetValue(CdsWithOutLzList[0]) };

                cdstypeDic[cds] = CardType.C2;

                return cdstypeDic;
            }

            return null;
        }
    }


    /// <summary>
    /// 当含赖子的牌组长度为3时
    /// </summary>
    internal class GetCdsTypeToDicCdsLen3 : GetCardsTypetoDic
    {
        public GetCdsTypeToDicCdsLen3(List<int> cdsWithOutLzList, int laiziNum)
            : base(cdsWithOutLzList, laiziNum)
        {

        }

        public override Dictionary<int[], CardType> GetCdsTypeDic()
        {
            if (CdsWithOutLzList.Count == 2 && LaiZiNum == 1)
            {
                if (CdsWithOutLzList[0] != CdsWithOutLzList[1]) return null;

                var cdstypeDic = new Dictionary<int[], CardType>();
                var cds = new int[3] { CdsWithOutLzList[0], CdsWithOutLzList[1] ,PokerRuleUtil.GetValue(CdsWithOutLzList[0])};
                cdstypeDic[cds]= CardType.C3;
                return cdstypeDic;
            }

            if (CdsWithOutLzList.Count == 1 && LaiZiNum == 2)
            {
                var value = PokerRuleUtil.GetValue(CdsWithOutLzList[0]);
                var cds = new int[3] { CdsWithOutLzList[0], value, value };
                var cdstypeDic = new Dictionary<int[], CardType>();
                cdstypeDic[cds] = CardType.C3;
                return cdstypeDic;
            }

            return null;
        }
    }


    /// <summary>
    /// 当含赖子的牌组长度为4时
    /// </summary>
    internal class GetCdsTypeToDicCdsLen4 : GetCardsTypetoDic
    {
        public GetCdsTypeToDicCdsLen4(List<int> cdsWithOutLzList, int laiziNum)
            : base(cdsWithOutLzList, laiziNum)
        {

        }

        public override Dictionary<int[], CardType> GetCdsTypeDic()
        {

            if (CdsWithOutLzList.Count == 3 && LaiZiNum == 1)
            {
                var value0 = PokerRuleUtil.GetValue(CdsWithOutLzList[0]);
                var value1 = PokerRuleUtil.GetValue(CdsWithOutLzList[1]);
                var value2 = PokerRuleUtil.GetValue(CdsWithOutLzList[2]);
                var cdstypeDic = new Dictionary<int[], CardType>();
                if (value0 == value1 && value1 == value2)
                {
                    var cds = new int[4] { CdsWithOutLzList[0], CdsWithOutLzList[1], CdsWithOutLzList[2], value1 };
                    cdstypeDic[cds] = CardType.C4;

/*                    var cds2 = new int[4] { CdsWithOutLzList[0], CdsWithOutLzList[1], CdsWithOutLzList[2], PokerRuleUtil.MagicKing };
                    cdstypeDic[cds2] = CardType.C31;*/
                }
/*                else if (value0 != value1 && value1 == value2)
                {
                    var cds2 = new int[4] { CdsWithOutLzList[0], CdsWithOutLzList[1], CdsWithOutLzList[2], value2};
                    cdstypeDic[cds2] = CardType.C31;
                }
                else if (value0 == value1 && value1 != value2)
                {
                    var cds2 = new int[4] { CdsWithOutLzList[0], CdsWithOutLzList[1], value0, CdsWithOutLzList[2] };
                    cdstypeDic[cds2] = CardType.C31;
                }*/

                return cdstypeDic;
            }


            if (CdsWithOutLzList.Count == 2 && LaiZiNum == 2)
            {
                var value0 = PokerRuleUtil.GetValue(CdsWithOutLzList[0]);
                var value1 = PokerRuleUtil.GetValue(CdsWithOutLzList[1]);
                var cdstypeDic = new Dictionary<int[], CardType>();

                if (value0 == value1)
                {
                    var cds = new int[4] { CdsWithOutLzList[0], CdsWithOutLzList[1], value0, value0 };
                    cdstypeDic[cds] = CardType.C4;

/*                    var cds2 = new int[4] { CdsWithOutLzList[0], CdsWithOutLzList[1], value0, PokerRuleUtil.MagicKing };
                    cdstypeDic[cds2] = CardType.C31;*/

                    return cdstypeDic;
                }
/*                else
                {
                    var cds = value0 < value1
                                  ? new int[4] { CdsWithOutLzList[0], CdsWithOutLzList[1], value1, value1 }
                                  : new int[4] { CdsWithOutLzList[1], value0, value0, CdsWithOutLzList[0] };
                    cdstypeDic[cds] = CardType.C31;
                    return cdstypeDic;
                }*/
            }

            if (CdsWithOutLzList.Count == 1 && LaiZiNum == 3)
            {
                var value = PokerRuleUtil.GetValue(CdsWithOutLzList[0]);
                var cdstypeDic = new Dictionary<int[], CardType>();

                var cds = new int[4] {CdsWithOutLzList[0],value,value,value};
                cdstypeDic[cds] = CardType.C4;

/*                var cds2 = new int[4] { CdsWithOutLzList[0], 15, 15, 15};
                cdstypeDic[cds2] = CardType.C31;*/

                return cdstypeDic;
            }

            return null;
        }
    }











    //--------------------------------以下的类等到赖子可能有多于1个时在考虑开发吧

    /// <summary>
    /// 当含赖子的牌组长度为5时
    /// </summary>
    internal class GetCdsTypeToDicCdsLen5 : GetCardsTypetoDic
    {
        //单龙的最小长度
        public const int MinC123Len = 5;
        //连对的牌值最小组成长度（比如现在是3对最少）
        public const int MinC1122Len = 3;
        //连三最小的组成长度（比如现在是2连三最少）
        public const int MinC111222Len = 2;

        //分牌
        protected readonly CdSplitStruct CdsWithoutlzlistcdsplit;


        public GetCdsTypeToDicCdsLen5(List<int> cdsWithOutLzList, int laiziNum)
            : base(cdsWithOutLzList, laiziNum)
        {
            CdsWithoutlzlistcdsplit = new CdSplitStruct(cdsWithOutLzList.ToArray());
        }

        /// <summary>
        /// 查找可能组成的最大连续牌值组（这里的龙指的是 链子的情况，可能包括 2连，3连 4连.... 取决于lianNumLimt）的牌值下限
        /// </summary>
        /// <param name="sortedCdvalues"></param>
        /// <param name="lianLen"></param>
        /// <returns></returns>
        protected int[] FindBestLian(int[] sortedCdvalues, int lianLen)
        {
            //能的组成龙的牌值上限
            const int longCdHighLimitValue = 14;
            //能的组成龙的牌值下限
            const int longCdLowLimitValue = 3;

            var minvalue = sortedCdvalues[0];
            var maxvlaue = sortedCdvalues[sortedCdvalues.Length - 1];

            //排除不可能组成龙的情况
            if (sortedCdvalues.Any(cd => cd > longCdHighLimitValue)) return null;

            //排除最小值和最大值的区间太大 无法组成龙的情况
            if (maxvlaue - minvalue + 1 > lianLen) return null;

            //找到最大龙的龙下限
            while (true)
            {
                if (minvalue + lianLen - 1 > longCdHighLimitValue) minvalue--;
                else break;
            }
            if (minvalue < longCdLowLimitValue) return null;

            var lian = new int[lianLen];
            var j = 0;
            for (var i = minvalue; i < minvalue + lianLen; i++)
            {
                lian[j++] = i;
            }
            return lian;
        }



        /// <summary>
        /// 检查牌组能不能成为单龙
        /// </summary>
        /// <param name="cdstypeDic"></param>
        /// <param name="lianLen">单龙的长度</param>
        protected virtual void CheckCanbeC123(ref Dictionary<int[], CardType> cdstypeDic,int lianLen)
        {
            if (CdsWithoutlzlistcdsplit.DanCds.Count != CdsWithOutLzList.Count) return;

            if (CdsWithOutLzList.Count + LaiZiNum!=lianLen) return;

            if (lianLen < MinC123Len) return;

            var dansArray = CdsWithoutlzlistcdsplit.DanCds.ToArray();

            var bestLian = FindBestLian(dansArray, lianLen);
            var bestLianList = new List<int>();
            bestLianList.AddRange(bestLian);

            foreach (var cd in dansArray)
            {
                bestLianList.Remove(cd);
            }

            bestLianList.AddRange(CdsWithOutLzList);

            cdstypeDic[PokerRuleUtil.SortCds(bestLianList)]=CardType.C123;
        }

        /// <summary>
        /// 检查牌组能不能有连对
        /// </summary>
        /// <param name="cdstypeDic"></param>
        /// <param name="lianLen"></param>
        protected virtual void CheckCanbeC1122(ref Dictionary<int[], CardType> cdstypeDic,int lianLen)
        {
            if(CdsWithoutlzlistcdsplit.ThreeCds.Count+CdsWithoutlzlistcdsplit.FourCds.Count>0) return;

            if (CdsWithOutLzList.Count + LaiZiNum != lianLen) return;

            if(lianLen<MinC1122Len) return;

            var simpleValueCds = new List<int>();
            simpleValueCds.AddRange(CdsWithoutlzlistcdsplit.DanCds);
            simpleValueCds.AddRange(CdsWithoutlzlistcdsplit.DuiCds);
            if(simpleValueCds.Count > lianLen) return;

            var bestLian = FindBestLian(simpleValueCds.ToArray(), lianLen);

            //转换成连对
            var transC1122List = new List<int>();
            foreach (var cdv in bestLian)
                transC1122List.AddRange(new int[] { cdv, cdv });

            //移除不是赖子的牌的牌值组成
            foreach (var cd in CdsWithoutlzlistcdsplit.DanCds)
            {
                transC1122List.Remove(cd);
            }
            foreach (var cd in CdsWithoutlzlistcdsplit.DuiCds)
            {
                transC1122List.Remove(cd);
                transC1122List.Remove(cd);
            }

            //把不是赖子的牌（带花色）加回来
            transC1122List.AddRange(CdsWithOutLzList);
            cdstypeDic[PokerRuleUtil.SortCds(transC1122List)] = CardType.C1122;
        }


        /// <summary>
        /// 检查牌组能不能有连三
        /// </summary>
        /// <param name="cdstypeDic"></param>
        /// <param name="lianLen"></param>
        protected virtual void CheckCanbeC111222(ref Dictionary<int[], CardType> cdstypeDic, int lianLen)
        {
            if (CdsWithoutlzlistcdsplit.FourCds.Count > 0) return;

            if (CdsWithOutLzList.Count + LaiZiNum != lianLen) return;

            if (lianLen < MinC111222Len) return;

            var simpleValueCds = new List<int>();
            simpleValueCds.AddRange(CdsWithoutlzlistcdsplit.DanCds);
            simpleValueCds.AddRange(CdsWithoutlzlistcdsplit.DuiCds);
            simpleValueCds.AddRange(CdsWithoutlzlistcdsplit.ThreeCds);
            if (simpleValueCds.Count > lianLen) return;

            var bestLian = FindBestLian(simpleValueCds.ToArray(), lianLen);

            //转换成连对
            var transC111222List = new List<int>();
            foreach (var cdv in bestLian)
                transC111222List.AddRange(new int[] { cdv, cdv, cdv });

            //移除不是赖子的牌的牌值组成
            foreach (var cd in CdsWithoutlzlistcdsplit.DanCds)
            {
                transC111222List.Remove(cd);
            }
            foreach (var cd in CdsWithoutlzlistcdsplit.DuiCds)
            {
                transC111222List.Remove(cd);
                transC111222List.Remove(cd);
            }
            foreach (var cd in CdsWithoutlzlistcdsplit.DuiCds)
            {
                transC111222List.Remove(cd);
                transC111222List.Remove(cd);
                transC111222List.Remove(cd);
            }

            //把不是赖子的牌（带花色）加回来
            transC111222List.AddRange(CdsWithOutLzList);
            cdstypeDic[PokerRuleUtil.SortCds(transC111222List)] = CardType.C111222;
        }


        public override Dictionary<int[], CardType> GetCdsTypeDic()
        {
            if (CdsWithOutLzList.Count + LaiZiNum != 5) return null;

            if (CdsWithOutLzList.Count == 1 && LaiZiNum == 4)
            {
                var value = PokerRuleUtil.GetValue(CdsWithOutLzList[0]);
                var cdstypeDic = new Dictionary<int[], CardType>();
                var cds1 = new int[5]
                    {CdsWithOutLzList[0], value, value, PokerRuleUtil.MagicKing, PokerRuleUtil.MagicKing};
                cdstypeDic[cds1] = CardType.C32;

                //找可能组成的最大的龙
                CheckCanbeC123(ref cdstypeDic,5);

                return cdstypeDic;
            }
            else
            {
                var cdstypeDic = new Dictionary<int[], CardType>();
                //找可能组成的最大的龙
                CheckCanbeC123(ref cdstypeDic,5);
                return cdstypeDic;
            }
        }
    }


/*    /// <summary>
    /// 当含赖子的牌组长度为6时
    /// </summary>
    internal class GetCdsTypeToDicCdsLen6 : GetCdsTypeToDicCdsLen5
    {
        public GetCdsTypeToDicCdsLen6(List<int> cdsWithOutLzList, int laiziNum) : base(cdsWithOutLzList, laiziNum)
        {



        }

        public override Dictionary<int[], CardType> GetCdsTypeDic()
        {
            if (CdsWithOutLzList.Count + LaiZiNum != 6) return null;

            
            //检查4带2个单牌
            if(CdsWithOutLzList.Count==1)

        }
    }*/


    /*
                 switch (cdsType)
            {
                case CardType.C1:

                    break;
                case CardType.C2:
                    break;
                case CardType.C3:
                    break;
                case CardType.C31:
                    break;
                case CardType.C32:
                    break;
                case CardType.C111222:
                    break;
                case CardType.C1112223434:
                    break;
                case CardType.C11122234:
                    break;
                case CardType.C1122:
                    break;
                case CardType.C123:
                    break;
                case CardType.C411:
                    break;
                case CardType.C422:
                    break;
            }


            switch (cdsType)
            {
                case CardType.C4:
                    break;
                case CardType.C5:
                    break;
                case CardType.C42:
                    break;
            }
     
     */
}
