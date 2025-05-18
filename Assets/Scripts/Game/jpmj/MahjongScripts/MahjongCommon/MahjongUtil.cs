using System.Collections.Generic;
using System.Linq;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{
    internal class MahjongUtil
    {
        public static BetterList<int> PreCheckTingPai(int[] list) {
            int val = 0;
            int handItemNum = list.Length;
            BetterList<int> outlist;
            if (handItemNum > 12) {
                outlist = GetSingleNum(list);
                if (outlist.size > 2) {
                    outlist.Clear();
                }
            }
            else {
                outlist = new BetterList<int>();
            }
            for (int i = 0; i < handItemNum; i++) {
                if (val == list[i]) {
                    continue;
                }
                val = list[i];
                int[] tmpArr = new int[handItemNum - 1];
                int j;
                for (j = 0; j < i; j++) {
                    tmpArr[j] = list[j];
                }
                for (j = i + 1; j < handItemNum; j++) {
                    tmpArr[j - 1] = list[j];
                }
                //YxDebugList(tmpArr);
                if (_checkTingPai(tmpArr, true)) {
                    outlist.Add(val);
                }
            }
            return outlist;
        }

        public static bool CheckTingPai(int[] list) {
            BetterList<int> singls = GetSingleNum(list);
            if (singls.size == 1) {
                return true;
            }
            return _checkTingPai(list, true);
        }

        /// <summary>
        /// 检测这组牌是否可以胡
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool CheckHuPai(int[] list) {
            if (IsQiDui(list)) {
                return true;
            }
            return _checkHupai(list, true);
        }

        /// <summary>
        /// 检测是否能胡某个牌
        /// </summary>
        /// <param name="list">手牌</param>
        /// <param name="value">要检测的牌</param>
        /// <returns></returns>
        public static bool CheckHuPai(int[] list, int value) {
            return CheckHuPai(InsertCard(list,value));
        }

        public static int[] InsertCard(int[] list, int value) {
            int len = list.Length;
            int[] tmpList = new int[len + 1];
            int i = 0;
            for (; i < len; i++) {
                if (list[i] < value) {
                    tmpList[i] = list[i];
                } else {
                    tmpList[i] = value;                 
					break;
                }
            }
			if(i<len){
				for (; i < len; i++) {
	                tmpList[i + 1] = list[i];
	            }	
			}else{
				tmpList[len] = value;
			}
            
            return tmpList;
        }

        private static bool _checkTingPai(int[] list, bool checkJiang) {
            int size = list.Length;
            if (size == 1) {
                //YxDebugList(list);
                return true;
            }
            if (size == 2) {
                // YxDebugList(list);
                return list[1] - list[0] < 3;
            }
            int i;
            //将
            if (checkJiang) {
                for (i = 0; i < size - 1; i++) {
                    if (list[i] == list[i + 1]) {
                        if (_checkTingPai(RemoveListItem(list, i, 2), false)) {
                            return true;
                        }
                        i++;
                    }
                }
            }
            //AAA
            for (i = 0; i < size - 2; i++) {
                if (list[i] == list[i + 2]) {
                    if (_checkTingPai(RemoveListItem(list, i, 3), false)) {
                        return true;
                    }
                }
            }
            //ABC
            for (i = 0; i < size - 2; i++) {
                if (list[i] == list[i + 1]) {
                    continue;
                }
                for (int j = i + 2; j < size; j++) {
                    if (list[j] - list[i] > 2) {
                        break;
                    }
                    if (list[j] - list[i] == 2 && list[j] != list[j - 1]) {
                        int[] cpList = RemoveListItem(list, j - 1, 2);
                        cpList = RemoveListItem(cpList, i, 1);
                        if (_checkTingPai(cpList, false)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }



        private static bool _checkHupai(int[] list, bool checkJiang) {
            int size = list.Length;
            if (size == 0) {
                //胡牌了！
                return true;
            }
            int i;
            //将
            if (checkJiang) {
                for (i = 0; i < size - 1; i++) {
                    if (list[i] == list[i + 1]) {
                        if (_checkHupai(RemoveListItem(list, i, 2), false)) {
                            return true;
                        }
                        i++;
                    }
                }
            }
            //AAA
            for (i = 0; i < size - 2; i++) {
                if (list[i] == list[i + 2]) {
                    if (_checkHupai(RemoveListItem(list, i, 3), false)) {
                        return true;
                    }
                }
            }
            //ABC
            for (i = 0; i < size - 2; i++) {
                if (list[i] == list[i + 1]) {
                    continue;
                }
                for (int j = i + 2; j < size; j++) {
                    if (list[j] - list[i] > 2) {
                        break;
                    }
                    if (list[j] - list[i] == 2 && list[j] != list[j - 1]) {
                        int[] cpList = RemoveListItem(list, j - 1, 2);
                        cpList = RemoveListItem(cpList, i, 1);
                        if (_checkHupai(cpList, false)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        private static int[] RemoveListItem(int[] list, int index, int num) {
            int[] mList = new int[list.Length - num];
            for (int i = 0; i < index; i++) {
                mList[i] = list[i];
            }
            for (int i = index + num; i < list.Length; i++) {
                mList[i - num] = list[i];
            }
            return mList;
        }

        public static void YxDebugList(IEnumerable<int> outList) {
            string msg = "pai list:";
            YxDebugList(outList,msg);
        }

        public static void YxDebugList(IEnumerable<int> outList,string msg) {
            foreach (int a in outList) {
                msg += (a >> 4) + "" + (a & 15) + ",";
            }
            YxDebug.Log(msg);
        }

        /// <summary>
        /// 获取列表中的所有单牌
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static BetterList<int> GetSingleNum(int[] list) {
            int len = list.Length;
            BetterList<int> rs = new BetterList<int>();
            for (int i = 0; i < len - 1; i++) {
                if (list[i] == list[i + 1]) {
                    i++;
                }
                else {
                    rs.Add(list[i]);
                }
            }
            //最后一个没有被计算到，所以额外计算
            if (list[len - 1] != list[len - 2]) {
                rs.Add(list[len-1]);
            }
            return rs;
        }

        private static bool IsQiDui(int[] list) {
            if (list.Length != 14) {
                return false;
            }
            return GetSingleNum(list).size == 0;
        }

        public static int[] GetTingList(int[] cards) {
            IList<int> tings = new List<int>();
            IList<int> posibleList = new List<int>();
            //可能胡牌的列表
            int lastCard = -1;
            const int feng = 4 << 4;
            foreach (int card in cards) {
                if (card == lastCard) {
                    continue;
                }
                if (card != lastCard + 1) {
                    if (card != lastCard + 2 && (card & 0xf) > 1) {
                        posibleList.Add(card - 1);
                    }
                    posibleList.Add(card);
                }
                if (card < feng && (card & 0xf) < 9) {
                    posibleList.Add(card + 1);
                }
                lastCard = card;
            }
            foreach (int i in posibleList) {
                if (CheckHuPai(cards, i)) {
                    tings.Add(i);
                }
            }
            return tings.ToArray();
        }

        public static void SortIntArr(int[] cards) {
            int len = cards.Length;
            for (int i = 0; i < len - 1; i++) {
                for (int j = i + 1; j < len; j++) {
                    if (cards[i] > cards[j]) {
                        int tmp = cards[i];
                        cards[i] = cards[j];
                        cards[j] = tmp;
                    }
                }
            }
        }
    }
}
