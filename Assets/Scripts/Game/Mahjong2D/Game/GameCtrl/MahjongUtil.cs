using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    internal class MahjongUtil
    {
        /// <summary>
        /// 检测这组牌是否可以胡
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool CheckHuPai(int[] list)
        {
            return _checkHupai(list, true);
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
                //debugList(list);
                return true;
            }
            if (size == 2) {
                // debugList(list);
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

        private static bool _checkTingPai258(int[] list, bool checkJiang)
        {
            int size = list.Length;
            if (size == 1)
            {
                //debugList(list);
                return true;
            }
            if (size == 2)
            {
                // debugList(list);
                return list[1] - list[0] < 3;
            }
            int i;
            //将
            bool isFoundJiang258 = false;
            if (checkJiang)
            {
                for (i = 0; i < size - 1; i++)
                {
                    if (list[i] == list[i + 1] && Is258(list[i]))
                    {
                        isFoundJiang258 = true;
                        if (_checkTingPai258(RemoveListItem(list, i, 2), false))
                        {
                            return true;
                        }
                        i++;
                    }
                }
            }
            if (!isFoundJiang258)
                return false;
            //AAA
            for (i = 0; i < size - 2; i++)
            {
                if (list[i] == list[i + 2])
                {
                    if (_checkTingPai258(RemoveListItem(list, i, 3), false))
                    {
                        return true;
                    }
                }
            }
            //ABC
            for (i = 0; i < size - 2; i++)
            {
                if (list[i] == list[i + 1])
                {
                    continue;
                }
                for (int j = i + 2; j < size; j++)
                {
                    if (list[j] - list[i] > 2)
                    {
                        break;
                    }
                    if (list[j] - list[i] == 2 && list[j] != list[j - 1])
                    {
                        int[] cpList = RemoveListItem(list, j - 1, 2);
                        cpList = RemoveListItem(cpList, i, 1);
                        if (_checkTingPai258(cpList, false))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool _checkHupai(int[] list, bool checkJiang) {
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

        // 检查是否胡牌 必须是258做将
        public static bool _checkHupai258(int[] list, bool checkJiang)
        {
            int size = list.Length;
            if (size == 0)
            {
                //胡牌了！
                return true;
            }
            int i;
            //将
            if (checkJiang)
            {
                for (i = 0; i < size - 1; i++)
                {
                    if ( list[i] == list[i + 1] && Is258(list[i]) )
                    {
                        if (_checkHupai258(RemoveListItem(list, i, 2), false))
                        {
                            return true;
                        }
                        i++;
                    }
                }
            }
            //AAA
            for (i = 0; i < size - 2; i++)
            {
                if (list[i] == list[i + 2])
                {
                    if (_checkHupai258(RemoveListItem(list, i, 3), false))
                    {
                        return true;
                    }
                }
            }
            //ABC
            for (i = 0; i < size - 2; i++)
            {
                if (list[i] == list[i + 1])
                {
                    continue;
                }
                for (int j = i + 2; j < size; j++)
                {
                    if (list[j] - list[i] > 2)
                    {
                        break;
                    }
                    if (list[j] - list[i] == 2 && list[j] != list[j - 1])
                    {
                        int[] cpList = RemoveListItem(list, j - 1, 2);
                        cpList = RemoveListItem(cpList, i, 1);
                        if (_checkHupai258(cpList, false))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static int[] RemoveListItem(int[] list, int index, int num)
        {
            int[] mList = new int[list.Length - num];
            for (int i = 0; i < index; i++) {
                mList[i] = list[i];
            }
            for (int i = index + num; i < list.Length; i++) {
                mList[i - num] = list[i];
            }
            return mList;
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

        public static bool IsQiDui(int[] list) {
            if (list.Length != 14) {
                return false;
            }
            return GetSingleNum(list).size == 0;
        }

        public static int[] GetTingList(int[] cards, bool must258 = false) {
            IList<int> tings = new List<int>();
            IList<int> posibleList = new List<int>();
            // 特别的将一色听牌的可能牌并不连续
            bool isAll258 = true;
            foreach (int card in cards)
            {
                if (!Is258(card))
                {
                    isAll258 = false;
                }
            }
            if (isAll258)
            {
                tings.Add(18); tings.Add(21); tings.Add(24);
                tings.Add(50); tings.Add(53); tings.Add(56);
                tings.Add(34); tings.Add(37); tings.Add(40);
            }
            
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

                if (CheckHuPai(InsertCard(cards, i)))
                {
                    tings.Add(i);
                }
            }
            return tings.ToArray();
        }
        // 获取一个int型数组元素组成的字符串 以","分割
        public static string GetIntListString(int[] list)
        {
            string retStr = "";
            foreach (int item in list)
            {
                retStr = retStr + (EnumMahjongValue)item + ",";
            }
            return retStr;
        }

        // 判断一张牌是否是258
        public static bool Is258(int value)
        {
            if (value/16 < 1 || 3 < value/16)
                return false;

            if (value%16 != 2 && value%16 != 5 && value%16 != 8)
                return false;

            return true;
        }
    }
}
