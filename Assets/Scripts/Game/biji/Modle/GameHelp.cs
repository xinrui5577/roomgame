using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.biji.ui;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.biji.Modle
{
    class GameHelp
    {
        public bool HongHei;

        public static int GetValue(int card)
        {
            return card & 0xf;
        }

        public static int GetColor(int card)
        {
            return card & 0xf0;
        }

        public static int GetColorVal(int card)
        {
            int color = card & 0xf0;
            if (color == 0x10)
            {
                return 1;
            }
            if (color == 0x20)
            {
                return 3;
            }
            if (color == 0x30)
            {
                return 2;
            }
            if (color == 0x40)
            {
                return 4;
            }
            return 5;
        }
        public class ComparerValMaxMin : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                int cha = GetValue(y) - GetValue(x);
                if (cha == 0)
                {
                    return GetColorVal(y) - GetColorVal(x);
                }
                return cha;
            }
        }

        public class ComparerColorValMaxMin : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return GetColorVal(y) - GetColorVal(x);
            }
        }

        public class Dic : Dictionary<int, List<int>>
        {
            protected int Type;
            public Dic(int type)
            {
                Type = type;
            }

            public void AddCardList(List<int> cards)
            {
                foreach (int card in cards)
                {
                    AddCard(card);
                }
            }
            public void AddCard(int card)
            {
                int val = Type == 0 ? card & 0xf : card & 0xf0;
                if (ContainsKey(val))
                {
                    this[val].Add(card);
                }
                else
                {
                    this[val] = new List<int>();
                    this[val].Add(card);
                }
            }
            public int GetCnt(int val)
            {
                if (ContainsKey(val))
                {
                    return this[val].Count;
                }
                return 0x0;
            }
            public List<int> GetMoreThan(int cnt)
            {
                List<int> ret = new List<int>();
                foreach (KeyValuePair<int, List<int>> keyValuePair in this)
                {
                    if (GetCnt(keyValuePair.Key) > cnt)
                    {
                        ret.Add(keyValuePair.Key);
                    }
                }
                return ret;
            }

            public List<int> GetOne(int cnt)
            {
                List<int> ret = new List<int>();
                foreach (KeyValuePair<int, List<int>> keyValuePair in this)
                {
                    if (GetCnt(keyValuePair.Key) == cnt)
                    {
                        ret.Add(keyValuePair.Key);
                    }
                }
                return ret;
            }

            public List<int> GetEqually(int cnt, IComparer<int> comparer = null)
            {
                List<int> ret = new List<int>();
                foreach (KeyValuePair<int, List<int>> keyValuePair in this)
                {
                    if (GetCnt(keyValuePair.Key) == cnt)
                    {
                        ret.Add(keyValuePair.Key);
                    }
                }
                return ret;
            }
        }

        public static int IsLaizi(int cards)
        {
            if (cards == 0x51)
            {
                return 1;
            }
            if (cards == 0x61)
            {
                return 2;
            }
            return 0;
        }

        public static int[] RemoveLaizi(List<int> cards, List<int> nolaizi)
        {
            if (nolaizi != null)
            {
                nolaizi.Clear();
            }
            int[] laizi = new int[2];
            foreach (int card in cards)
            {
                int type = IsLaizi(card);
                if (type != 0)
                {
                    laizi[type - 1]++;
                }
                else
                {
                    if (nolaizi != null)
                    {
                        nolaizi.Add(card);
                    }
                }
            }
            return laizi;
        }

        public bool GetHongHei(int color)
        {
            if (color == 0x30 || color == 0x40 || color == 0x50)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetLaiziCnt(int[] laizi, int color = -1, List<int> laiziCard = null)
        {
            if (HongHei)
            {
                if (color != -1 && GetHongHei(color))
                {
                    if (laiziCard != null)
                    {
                        laiziCard.Add(0x51);
                    }
                    return laizi[0];
                }
                if (color != -1 && !GetHongHei(color))
                {
                    if (laiziCard != null)
                    {
                        laiziCard.Add(0x61);
                    }
                    return laizi[1];
                }
            }

            if (laizi[0] != 0)
            {
                if (laiziCard != null)
                {
                    laiziCard.Add(0x51);
                }
            }
            if (laizi[1] != 0)
            {
                if (laiziCard != null)
                {
                    laiziCard.Add(0x61);
                }
            }

            return laizi[0] + laizi[1];
        }

        public CardType CheckType(List<int> cards)
        {
            if (cards.Count != 3)
            {
                return CardType.None;
            }

            if (GetSanTiao(cards).Count > 0)
            {
                return CardType.SanTiao;
            }

            if (GetTongHuaShun(cards).Count > 0)
            {
                return CardType.TongHuaShun;
            }

            if (GetTongHua(cards).Count > 0)
            {
                return CardType.TongHua;
            }

            if (GetShunZi(cards).Count > 0)
            {
                return CardType.ShunZi;
            }

            if (GetDuiZi(cards).Count > 0)
            {
                return CardType.DuiZi;
            }

            if (GetSanPai(cards).Count > 0)
            {
                return CardType.SanPai;
            }
            return CardType.None;
        }

        protected List<List<int>> GetLianXuCard(List<int> nolaizi, int laiziCnt, List<int> laiziCard)
        {
            List<List<int>> lianxuArray = new List<List<int>>();

            List<int> add1Cards = new List<int>(nolaizi);
            for (int i = 0; i < nolaizi.Count; i++)
            {
                if (GetValue(nolaizi[i]) == 0xe)
                {
                    add1Cards.Add(GetColor(nolaizi[i]) + 1);
                }
            }
            Dic dic = new Dic(0);
            dic.AddCardList(add1Cards);
            for (int i = 0xe; i >= 0x1; i--)
            {
                if (dic.ContainsKey(i))
                {
                    if (dic.ContainsKey(i - 1) && dic.ContainsKey(i - 2))
                    {
                        List<int> card = dic[i];
                        List<int> card1 = dic[i - 1];
                        List<int> card2 = dic[i - 2];
                        foreach (int i1 in card)
                        {
                            foreach (int i2 in card1)
                            {
                                foreach (int i3 in card2)
                                {
                                    List<int> shunzi = new List<int>();
                                    if (GetValue(i1) == 0x1)
                                    {
                                        shunzi.Add(GetColor(i1) + 14);
                                    }
                                    else
                                    {
                                        shunzi.Add(i1);
                                    }

                                    if (GetValue(i2) == 0x1)
                                    {
                                        shunzi.Add(GetColor(i2) + 14);
                                    }
                                    else
                                    {
                                        shunzi.Add(i2);
                                    }
                                    if (GetValue(i3) == 0x1)
                                    {
                                        shunzi.Add(GetColor(i3) + 14);
                                    }
                                    else
                                    {
                                        shunzi.Add(i3);
                                    }

                                    lianxuArray.Add(shunzi);
                                }
                            }
                        }
                    }
                    if (laiziCnt > 0 && dic.ContainsKey(i - 1))
                    {
                        List<int> card = dic[i];
                        List<int> card1 = dic[i - 1];
                        foreach (int i1 in card)
                        {
                            foreach (int i2 in card1)
                            {
                                List<int> shunzi = new List<int>();
                                if (GetValue(i1) == 0x1)
                                {
                                    shunzi.Add(GetColor(i1) + 14);
                                }
                                else
                                {
                                    shunzi.Add(i1);
                                }

                                if (GetValue(i2) == 0x1)
                                {
                                    shunzi.Add(GetColor(i2) + 14);
                                }
                                else
                                {
                                    shunzi.Add(i2);
                                }

                                shunzi.Add(laiziCard[0]);
                                lianxuArray.Add(shunzi);
                            }
                        }
                    }
                    if (laiziCnt > 0 && dic.ContainsKey(i - 2))
                    {
                        List<int> card = dic[i];
                        List<int> card2 = dic[i - 2];
                        foreach (int i1 in card)
                        {
                            foreach (int i3 in card2)
                            {
                                List<int> shunzi = new List<int>();
                                if (GetValue(i1) == 0x1)
                                {
                                    shunzi.Add(GetColor(i1) + 14);
                                }
                                else
                                {
                                    shunzi.Add(i1);
                                }
                                shunzi.Add(laiziCard[0]);
                                if (GetValue(i3) == 0x1)
                                {
                                    shunzi.Add(GetColor(i3) + 14);
                                }
                                else
                                {
                                    shunzi.Add(i3);
                                }
                                lianxuArray.Add(shunzi);
                            }
                        }
                    }
                    if (laiziCnt > 1)
                    {
                        List<int> card = dic[i];
                        foreach (int i1 in card)
                        {
                            List<int> shunzi = new List<int>();
                            shunzi.Add(laiziCard[0]);
                            shunzi.Add(laiziCard[1]);
                            if (GetValue(i1) == 0x1)
                            {
                                shunzi.Add(GetColor(i1) + 14);
                            }
                            else
                            {
                                shunzi.Add(i1);
                            }
                            lianxuArray.Add(shunzi);
                        }

                    }
                }
            }


            return lianxuArray;
        }

        public int GetLaiZiCount(List<int> cards)
        {
            List<int> nolaiziCards = new List<int>();
            int[] laizi = RemoveLaizi(cards, nolaiziCards);
            int laiziCnt = GetLaiziCnt(laizi);
            return laiziCnt;
        }

        public List<List<int>> GetSanTiao(List<int> cards)
        {
            List<List<int>> returnList = new List<List<int>>();
            List<int> nolaiziCards = new List<int>();
            int[] laizi = RemoveLaizi(cards, nolaiziCards);
            int laiziCnt = GetLaiziCnt(laizi);
            Dic dicVal = new Dic(0);
            dicVal.AddCardList(nolaiziCards);
            int needCardCnt = 3 - laiziCnt;
            List<int> val3 = dicVal.GetMoreThan(needCardCnt - 1);
            val3.Sort(new ComparerValMaxMin());
            foreach (int key in val3)
            {
                List<int> val3Cards = new List<int>(dicVal[key]);
                val3Cards.Sort(new ComparerColorValMaxMin());
                int needlaizi = 3 - val3Cards.Count;
                if (needlaizi == -1)
                {
                    List<int[]> groups = GetGroup(val3Cards.Count, 3, false);
                    foreach (int[] group in groups)
                    {
                        List<int> santiao = new List<int>();
                        foreach (int index in group)
                        {
                            santiao.Add(val3Cards[index]);
                        }
                        returnList.Add(santiao);
                    }

                }
                else if (needlaizi == 0)
                {
                    returnList.Add(val3Cards);
                }
                else if (needlaizi == 1)
                {
                    if (laiziCnt == 1)
                    {
                        int laiziCard = laizi[0] > 0 ? 0x51 : 0x61;
                        val3Cards.Add(laiziCard);
                        returnList.Add(val3Cards);
                    }
                    else if (laiziCnt == 2)
                    {
                        List<int> santiao = new List<int>(val3Cards);
                        santiao.Add(0x51);
                        returnList.Add(santiao);
                        santiao = new List<int>(val3Cards);
                        santiao.Add(0x61);
                        returnList.Add(santiao);
                    }
                }
                else if (needlaizi == 2)
                {
                    List<int> santiao = new List<int>(val3Cards);
                    santiao.Add(0x51);
                    santiao.Add(0x61);
                    returnList.Add(santiao);
                }
            }

            return returnList;
        }

        public List<List<int>> GetTongHuaShun(List<int> cards)
        {
            List<List<int>> returnList = new List<List<int>>();
            List<int> nolaiziCards = new List<int>();
            int[] laizi = RemoveLaizi(cards, nolaiziCards);
            Dic dicVal = new Dic(1);
            dicVal.AddCardList(nolaiziCards);
            foreach (KeyValuePair<int, List<int>> pair in dicVal)
            {
                int color = pair.Key;
                List<int> laiziCard = new List<int>();
                int laiziCnt = GetLaiziCnt(laizi, color, laiziCard);
                List<int> sameColorCards = pair.Value;
                sameColorCards.Sort(new ComparerValMaxMin());
                List<List<int>> lianxu = GetLianXuCard(sameColorCards, laiziCnt, laiziCard);
                if (lianxu.Count > 0)
                {
                    returnList.AddRange(lianxu);
                }
            }

            return returnList;
        }

        public List<List<int>> GetTongHua(List<int> cards)
        {
            List<List<int>> returnList = new List<List<int>>();
            List<int> nolaiziCards = new List<int>();
            int[] laizi = RemoveLaizi(cards, nolaiziCards);
            Dic dicVal = new Dic(1);
            dicVal.AddCardList(nolaiziCards);
            foreach (KeyValuePair<int, List<int>> pair in dicVal)
            {
                int color = pair.Key;
                List<int> laiziCard = new List<int>();
                int laiziCnt = GetLaiziCnt(laizi, color, laiziCard);
                int needCardCnt = 3 - laiziCnt;
                List<int> sameColorCards = pair.Value;
                sameColorCards.Sort(new ComparerValMaxMin());
                if (sameColorCards.Count >= 3)
                {
                    List<int[]> groups = GetGroup(sameColorCards.Count, 3, false);
                    foreach (int[] group in groups)
                    {
                        List<int> tonghua = new List<int>();
                        foreach (int index in group)
                        {
                            tonghua.Add(sameColorCards[index]);
                        }
                        returnList.Add(tonghua);
                    }

                }
                if (sameColorCards.Count >= needCardCnt)
                {
                    if (laiziCnt == 1)
                    {
                        List<int[]> groups = GetGroup(sameColorCards.Count, 2, false);
                        foreach (int[] group in groups)
                        {
                            List<int> tonghua = new List<int>();
                            foreach (int index in group)
                            {
                                tonghua.Add(sameColorCards[index]);
                            }
                            tonghua.Add(laiziCard[0]);
                            returnList.Add(tonghua);
                        }
                    }
                    else if (laiziCnt == 2)
                    {
                        if (sameColorCards.Count >= 2)
                        {
                            List<int[]> groups = GetGroup(sameColorCards.Count, 2, false);
                            foreach (int[] group in groups)
                            {
                                List<int> tonghua = new List<int>();
                                foreach (int index in group)
                                {
                                    tonghua.Add(sameColorCards[index]);
                                }
                                tonghua.Add(laiziCard[0]);
                                returnList.Add(tonghua);
                            }
                        }

                        foreach (int c in sameColorCards)
                        {
                            List<int> tonghua = new List<int>();
                            tonghua.Add(c);
                            tonghua.Add(laiziCard[0]);
                            tonghua.Add(laiziCard[1]);
                            returnList.Add(tonghua);
                        }
                    }
                }

            }
            return returnList;
        }


        public List<List<int>> GetShunZi(List<int> cards)
        {
            List<List<int>> returnList = new List<List<int>>();
            List<int> nolaiziCards = new List<int>();
            int[] laizi = RemoveLaizi(cards, nolaiziCards);
            List<int> laiziCard = new List<int>();
            int laiziCnt = GetLaiziCnt(laizi, -1, laiziCard);
            List<List<int>> lianxu = GetLianXuCard(nolaiziCards, laiziCnt, laiziCard);
            if (lianxu.Count > 0)
            {
                for (int i = 0; i < lianxu.Count;)
                {
                    List<int> check = new List<int>();
                    int[] checkLaizi = RemoveLaizi(lianxu[i], check);
                    Dic col = new Dic(1);
                    col.AddCardList(check);
                    if (col.Count == 1)
                    {
                        if (HongHei)
                        {
                            bool hh = GetHongHei(col.First().Key);
                            if (hh && checkLaizi[0] != 0 && checkLaizi[1] == 0)
                            {
                                lianxu.RemoveAt(i);
                                continue;
                            }
                        }
                        //                        else
                        //                        {
                        //                            lianxu.RemoveAt(i);
                        //                            continue;
                        //                        }
                    }
                    i++;
                }
                returnList.AddRange(lianxu);
            }
            return returnList;
        }

        public List<List<int>> GetDuiZi(List<int> cards)
        {
            List<List<int>> returnList = new List<List<int>>();
            List<int> nolaiziCards = new List<int>();
            int[] laizi = RemoveLaizi(cards, nolaiziCards);
            List<int> laiziCards = new List<int>();
            int laiziCnt = GetLaiziCnt(laizi, -1, laiziCards);
            Dic dicVal = new Dic(0);
            dicVal.AddCardList(nolaiziCards);
            List<int> val2 = dicVal.GetMoreThan(1);
            List<int> val1 = dicVal.GetOne(1);

            val2.Sort(new ComparerValMaxMin());
            val1.Sort(new ComparerValMaxMin());

            foreach (int key in val2)
            {
                List<int> val3Cards = new List<int>(dicVal[key]);
                val3Cards.Sort(new ComparerColorValMaxMin());
                List<int[]> groups = GetGroup(val3Cards.Count, 2, false);
                foreach (int[] group in groups)
                {
                    List<int> duizi = new List<int>();
                    foreach (int index in group)
                    {
                        duizi.Add(val3Cards[index]);
                    }

                    foreach (var key1 in val1)
                    {
                        List<int> val4Cards = new List<int>(dicVal[key1]);
                        var newDuiZi = new List<int>(duizi);
                        newDuiZi.AddRange(val4Cards);
                        returnList.Add(newDuiZi);
                    }
                }
            }
            if (laiziCnt > 0)
            {
                foreach (int laiziC in laiziCards)
                {
                    //                    foreach (int card in cards)
                    //                    {
                    //                        List<int> duizi = new List<int>();
                    //                        duizi.Add(card);
                    //                        duizi.Add(laiziC);
                    //                        returnList.Add(duizi);
                    //                    }
                    foreach (var key1 in val1)
                    {
                        List<int> val4Cards = new List<int>(dicVal[key1]);
                        List<int> duizi = new List<int>();
                        duizi.AddRange(val4Cards);
                        duizi.Add(laiziC);
                        foreach (var key2 in val1)
                        {
                            List<int> dui = new List<int>(duizi);
                            List<int> val5Cards = new List<int>(dicVal[key2]);
                            if (val5Cards[0] != val4Cards[0])
                            {
                                dui.Add(val5Cards[0]);
                                returnList.Add(dui);
                            }
                        }
                    }
                }
            }

            return returnList;
        }


        public List<List<int>> GetSanPai(List<int> cards)
        {
            List<List<int>> returnList = new List<List<int>>();
            List<int> nolaiziCards = new List<int>();
            int[] laizi = RemoveLaizi(cards, nolaiziCards);
            Dic dic = new Dic(0);
            dic.AddCardList(nolaiziCards);
            List<int> values = dic.GetMoreThan(0);
            List<int[]> groups = GetGroup(values.Count, 3, false);
            foreach (int[] g in groups)
            {
                //判断 这3个牌是不是连续的 是不是 相同颜色的
                int val = values[g[0]];
                int val1 = values[g[1]];
                int val2 = values[g[2]];

                if (val - val1 == 1 && val1 - val2 == 1)
                {
                    continue;
                }
                List<int> card = dic[val];
                List<int> card1 = dic[val1];
                List<int> card2 = dic[val2];
                foreach (int i1 in card)
                {
                    foreach (int i2 in card1)
                    {
                        foreach (int i3 in card2)
                        {
                            if (GetColor(i1) == GetColor(i2) && GetColor(i2) == GetColor(i3))
                            {
                                continue;
                            }

                            List<int> sanpai = new List<int>();
                            sanpai.Add(i1);
                            sanpai.Add(i2);
                            sanpai.Add(i3);
                            returnList.Add(sanpai);
                        }
                    }
                }
            }

            return returnList;
        }

        public bool Compare(CardType type, List<int> cards, List<int> cards1)
        {
            List<int> trueCards = ReplaceLaizi(cards, type);
            List<int> trueCards1 = ReplaceLaizi(cards1, type);
            for (int i = 0; i < trueCards.Count(); i++)
            {
                int val1 = GetValue(trueCards[i]);
                int val2 = GetValue(trueCards1[i]);
                //                if (val1 - val2 != 0)
                //                {
                //                    return val1 - val2;
                //                }

                if (val1 - val2 != 0)
                {
                    if (val1 - val2 < 0)
                    {
                        return true;
                    }
                    if (val1 - val2 > 0)
                    {
                        return false;
                    }
                }
            }
            for (int i = 0; i < trueCards.Count; i++)
            {
                int val1 = GetColorVal(trueCards[i]);
                int val2 = GetColorVal(trueCards1[i]);
                //                if (val1 - val2 != 0)
                //                {
                //                    return val1 - val2;
                //                }
                if (val1 - val2 != 0)
                {
                    if (val1 - val2 < 0)
                    {
                        return true;
                    }
                    if (val1 - val2 > 0)
                    {
                        return false;
                    }
                }

            }
            return false;
        }

        public List<int> ReplaceLaizi(List<int> cards, CardType type, bool min = false, bool quanHong = false)
        {
            if (!quanHong && !min)
            {
                cards.Sort(new ComparerValMaxMin());
            }
            List<int> nolaizi = new List<int>();
            int[] laizi = RemoveLaizi(cards, nolaizi);
            int laiziCnt = laizi[0] + laizi[1];
            if (laiziCnt == 0)
            {
                int big = nolaizi[0];
                int bigVal = GetValue(big);
                int middle = nolaizi[1];
                int middleVal = GetValue(middle);
                int small = nolaizi[2];
                int smallVal = GetValue(small);
                if (bigVal == 14&& smallVal== 2&&middleVal==3)
                {
                    var newCard=new List<int>();
                    newCard.Add(nolaizi[1]);
                    newCard.Add(nolaizi[2]);
                    newCard.Add(nolaizi[0]);
                    return newCard;
                }
                else
                {
                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (GetValue(cards[i]) == 1)
                        {
                            cards[i] = GetColor(cards[i]) + 14;
                        }
                    }
                }

                return cards;
            }
          
            if (type == CardType.SanTiao)
            {
                int val = GetValue(nolaizi[0]);
                if (HongHei)
                {
                    if (laizi[0] > 0)
                    {
                        nolaizi.Add(0x40 + val);
                    }
                    if (laizi[1] > 0)
                    {
                        nolaizi.Add(0x20 + val);
                    }
                }
                else
                {
                    if (quanHong)
                    {
                        for (int i = 0; i < laiziCnt; i++)
                        {
                            nolaizi.Add(0x20 + val);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < laiziCnt; i++)
                        {
                            nolaizi.Add(0x40 + val);
                        }
                    }
                }
            }
            else if (type == CardType.TongHuaShun)
            {
                int big = nolaizi[0];
                int bigVal = GetValue(big);
                int color = GetColor(big);
                if (laiziCnt == 1)
                {
                    int small = nolaizi[1];
                    int smallVal = GetValue(small);
                    if (bigVal == 14 && smallVal == 2)
                    {
                        bigVal = 2;
                        smallVal = 1;
                    }
                    if (bigVal - smallVal == 1)
                    {
                        if (bigVal != 14)
                        {
                            if (min)
                            {
                                nolaizi.Add(color + smallVal - 1);
                            }
                            else
                            {
                                nolaizi.Add(color + bigVal + 1);
                            }
                        }
                        else
                        {
                            nolaizi.Add(color + smallVal - 1);
                        }
                    }
                    else
                    {
                        nolaizi.Add(color + bigVal - 1);
                    }
                }
                else if (laiziCnt == 2)
                {
                    if (bigVal == 14)
                    {
                        nolaizi.Add(color + bigVal - 1);
                        nolaizi.Add(color + bigVal - 2);
                    }
                    else if (bigVal == 13)
                    {
                        nolaizi.Add(color + bigVal + 1);
                        nolaizi.Add(color + bigVal - 1);
                    }
                    else
                    {
                        if (min)
                        {
                            nolaizi.Add(color + bigVal - 1);
                            nolaizi.Add(color + bigVal - 2);
                        }
                        else
                        {
                            nolaizi.Add(color + bigVal + 1);
                            nolaizi.Add(color + bigVal + 2);
                        }
                    }
                }

            }
            else if (type == CardType.TongHua)
            {
                int color = GetColor(cards[0]);
                if (laiziCnt == 1)
                {
                    nolaizi.Add(color + 14);
                }
                else if (laiziCnt == 2)
                {
                    nolaizi.Add(color + 14);
                    nolaizi.Add(color + 14);
                }
            }
            else if (type == CardType.ShunZi)
            {
                int big = nolaizi[0];
                int bigVal = GetValue(big);
                int color = 0;
                if (HongHei)
                {
                    if (laizi[0] > 0)
                    {
                        color = 0x40;
                    }
                    if (laizi[1] > 0)
                    {
                        color = 0x20;
                    }
                }
                else
                {
                    color = quanHong ? 0x20 : 0x40;
                }
                if (laiziCnt == 1)
                {
                    int small = nolaizi[1];
                    int smallVal = GetValue(small);
                    if (bigVal == 14 && smallVal == 2)
                    {
                        bigVal = 2;
                        smallVal = 1;
                    }
                    if (bigVal - smallVal == 1)
                    {
                        if (bigVal != 14)
                        {
                            if (min)
                            {
                                nolaizi.Add(color + smallVal - 1);
                            }
                            else
                            {
                                nolaizi.Add(color + bigVal + 1);
                            }
                        }
                        else
                        {
                            nolaizi.Add(color + smallVal - 1);
                        }
                    }
                    else
                    {
                        nolaizi.Add(color + bigVal - 1);
                    }
                }
                else if (laiziCnt == 2)
                {
                    if (bigVal == 14)
                    {
                        nolaizi.Add(color + bigVal - 1);
                        nolaizi.Add(color + bigVal - 2);
                    }
                    else if (bigVal == 13)
                    {
                        nolaizi.Add(color + bigVal + 1);
                        nolaizi.Add(color + bigVal - 1);
                    }
                    else
                    {
                        if (min)
                        {
                            nolaizi.Add(color + bigVal - 1);
                            nolaizi.Add(color + bigVal - 2);
                        }
                        else
                        {
                            nolaizi.Add(color + bigVal + 1);
                            nolaizi.Add(color + bigVal + 2);
                        }
                    }
                }
            }
            else if (type == CardType.DuiZi)
            {
                int big = nolaizi[0];
                int bigVal = GetValue(big);
                if (HongHei)
                {
                    if (laizi[0] > 0)
                    {
                        nolaizi.Add(0x40 + bigVal);
                    }
                    if(laizi[1] > 0)
                    {
                        nolaizi.Add(0x20 + bigVal);
                    }
                }
                else
                {
                    if (quanHong)
                    {
                        nolaizi.Add(0x20 + bigVal);
                    }
                    else
                    {
                        nolaizi.Add(0x40 + bigVal);
                    }
                }
            }
            else if (type == CardType.SanPai)
            {
                if (laiziCnt == 1)
                {
                    nolaizi.Add(0x40 + 14);
                }
            }
            nolaizi.Sort(new ComparerValMaxMin());
            for (int i = 0; i < nolaizi.Count; i++)
            {
                if (GetValue(nolaizi[i]) == 1)
                {
                    nolaizi[i] = GetColor(nolaizi[i]) + 14;
                }
            }
            return nolaizi;
        }

        public List<int[]> GetGroup(int cnt, int num, bool lianxu)
        {
            if (cnt < num)
            {
                return null;
            }

            List<int[]> ret = new List<int[]>();
            int[] indexArr = new int[cnt];
            for (int i = 0; i < cnt; i++)
            {
                indexArr[i] = i;
            }

            if (cnt == num)
            {
                ret.Add(indexArr);
                return ret;
            }
            if (lianxu)
            {
                for (int i = 0; i <= cnt - num + 1; i++)
                {
                    int[] item = new int[num];
                    int index = 0;
                    item[index++] = i;
                    for (int j = i + 1; j < i + num; j++)
                    {
                        item[index++] = j;
                    }
                    if (index == num)
                    {
                        ret.Add(item);
                    }
                }
            }
            else
            {
                for (int i = 0; i <= cnt - num + 1; i++)
                {
                    for (int k = i + 1; k <= cnt - num + 1; k++)
                    {
                        int[] item = new int[num];
                        int index = 0;
                        item[index++] = i;
                        for (int j = k; j < k + num - 1; j++)
                        {
                            item[index++] = j;
                        }
                        if (index == num)
                        {
                            ret.Add(item);
                        }
                    }
                }
            }

            return ret;
        }
    }
}
