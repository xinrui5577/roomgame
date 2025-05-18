using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts.Game.biji.ui;

namespace Assets.Scripts.Game.biji.Modle
{
    public class GameHelp1
    {
        protected ValCntList ValueDic;
        protected ColCntList ColorDic;

        public Dictionary<CardType, List<List<int>>> dicCards = new Dictionary<CardType, List<List<int>>>();
        protected bool LaiziHh;

        public void SetHongHei(bool honghei)
        {
            LaiziHh = honghei;
        }
        public GameHelp1()
        {
            //0 为 赖子
            ValueDic = new ValCntList();
            ColorDic = new ColCntList();
        }
        protected void InitDic(List<int> cards)
        {
            ValueDic.Clear();
            ColorDic.Clear();
            foreach (var t in cards)
            {
                if (Tools.IsLaizi(t))
                {

                }
                else
                {
                    ValueDic.AddCard(t);
                    ColorDic.AddCard(t);
                }
            }
        }
        protected List<int> Add1ToCards(List<int> cards)
        {
            List<int> c1List = new List<int>();

            foreach (var c in cards)
            {
                if (Tools.GetValue(c) == 0xe)
                {
                    c1List.Add(Tools.GetColor(c) + 1);
                }
            }

            List<int> newCards = new List<int>();

            newCards.AddRange(cards);

            newCards.AddRange(c1List);

            return newCards;
        }

        protected int[] GetLaiziCnt(List<int> cards, List<int> nolaizi)
        {
            int[] laizi = new int[] { 0, 0 };

            foreach (var c in cards)
            {
                if (Tools.IsLaizi(c))
                {
                    if (c == 0x51)
                    {
                        laizi[1]++;
                    }
                    else if (c == 0x61)
                    {
                        laizi[0]++;
                    }
                }
                else
                {
                    if (nolaizi != null)
                    {
                        nolaizi.Add(c);
                    }
                }
            }

            return laizi;
        }

        public class MaxToMin : IComparer<int>
        {
            public int Compare(int o1, int o2)
            {
                return o2 - o1;
            }
        }
        public List<List<int>> LianShun(List<int> cards)
        {
            cards.Sort(new MaxToMin());//大到小
            InitDic(cards);
            int[] laizi = GetLaiziCnt(cards, null);
            int laiziCnt = laizi[0] + laizi[1];
            List<List<int>> lanxu = GetLianXuCard(cards, laiziCnt, 9, true);
            return lanxu;
        }
        public CardType CheckType(List<int> cards)
        {
            if (cards.Count != 3)
            {
                return CardType.None;
            }
            cards.Sort(new MaxToMin());//大到小
            InitDic(cards);
            bool sameColor = ColorDic.Count == 1;
            int[] laizi = GetLaiziCnt(cards, null);
            int laiziCnt = laizi[0] + laizi[1];
            if (LaiziHh && sameColor)
            {
                if (laiziCnt == 2)
                {
                    sameColor = false;
                }
                else if (laiziCnt == 1)
                {
                    ColCntItem cn = ColorDic[0];
                    if (Tools.CheckHongHei(cn.Color))
                    {
                        sameColor = laizi[0] > 0;
                    }
                    else
                    {
                        sameColor = laizi[1] > 0;
                    }
                }
            }
            List<int> keys = ValueDic.GetValues();
            List<List<int>> lanxu = GetLianXuCard(cards, laiziCnt, 3, true);
            bool isLian = lanxu.Count!=0;

            //三条
            if (keys.Count == 1)
            {
                return CardType.SanTiao;
            }
            else if (sameColor && isLian)
            {
                return CardType.TongHuaShun;
            }
            else if (sameColor)
            {
                return CardType.TongHua;
            }
            else if (isLian)
            {
                return CardType.ShunZi;
            }
            else if (keys.Count == 2)
            {
                return CardType.DuiZi;
            }
            else
            {
                return CardType.SanPai;
            }
        }

        protected List<List<int>> GetLianXuCard(List<int> cards, int laiziCnt, int len, bool checkOne)
        {
            List<List<int>> lianxuArray = new List<List<int>>();
            if (cards.Count + laiziCnt < len)
            {
                return lianxuArray;
            }
            List<int> newCards = Add1ToCards(cards);
            ValCntList checkList = new ValCntList(newCards);
            List<int> values = checkList.GetValues();
            if (values.Count >= len)
            {
                for (int i = 0; i < values.Count;)
                {
                    if (values.Count - i + laiziCnt < len)
                    {
                        break;
                    }
                    int checkVal = values[i];
                    int containsNum = 1;
                    for (int j = 1; j < len; j++)
                    {
                        if (values.Contains(checkVal - j))
                        {
                            containsNum++;
                        }
                    }
                    if (containsNum >= len)
                    {
                        List<int> lx = new List<int>();
                        for (int j = checkVal; j > checkVal - len; j--)
                        {
                            lx.Add(checkList.FindItemByValue(j).RemoveOneCard());
                        }
                        values = checkList.GetValues();
                        lianxuArray.Add(lx);
                        if (checkOne)
                        {
                            return lianxuArray;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            //加入一个赖子的时候
            if (laiziCnt > 0 && values.Count + 1 >= len)
            {
                for (int i = 0; i < values.Count;)
                {
                    if (values.Count - i + laiziCnt < len)
                    {
                        break;
                    }
                    int checkVal = values[i];
                    int containsNum = 1;
                    for (int j = 1; j < len; j++)
                    {
                        if (values.Contains(checkVal - j))
                        {
                            containsNum++;
                        }
                    }
                    if (containsNum + 1 >= len)
                    {
                        List<int> lx = new List<int>();
                        for (int j = checkVal; j > checkVal - len; j--)
                        {
                            ValCntItem find = checkList.FindItemByValue(j);
                            if (find != null)
                            {
                                lx.Add(find.RemoveOneCard());
                            }
                            else
                            {
                                lx.Add(0);
                                laiziCnt--;
                            }
                        }
                        values = checkList.GetValues();
                        lianxuArray.Add(lx);
                        if (checkOne)
                        {
                            return lianxuArray;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            if (laiziCnt > 1 && values.Count + 2 >= len)
            {
                //加入两个赖子
                for (int i = 0; i < values.Count;)
                {
                    if (values.Count - i + laiziCnt < len)
                    {
                        break;
                    }
                    int checkVal = values[i];
                    int containsNum = 1;
                    for (int j = 1; j < len; j++)
                    {
                        if (values.Contains(checkVal - j))
                        {
                            containsNum++;
                        }
                    }
                    if (containsNum + 2 >= len)
                    {
                        List<int> lx = new List<int>();
                        for (int j = checkVal; j > checkVal - len; j--)
                        {
                            ValCntItem find = checkList.FindItemByValue(j);
                            if (find != null)
                            {
                                lx.Add(find.RemoveOneCard());
                            }
                            else
                            {
                                lx.Add(0);
                                laiziCnt--;
                            }
                        }

                        values = checkList.GetValues();
                        lianxuArray.Add(lx);
                        if (checkOne)
                        {
                            return lianxuArray;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return lianxuArray;
        }

        public class max_min : IComparer<int>
        {
            public int Compare(int o1, int o2)
            {
                return o2 - o1;
            }
        }

        public List<XipaiType> GetXiPaiType(List<int> cards)
        {
            List<XipaiType> typeArray = new List<XipaiType>();
            cards.Sort(new max_min());
            InitDic(cards);
            int[] laizi = GetLaiziCnt(cards, null);
            int laiziCnt = laizi[0] + laizi[1];
            ValCntList vn3 = ValueDic.GetMoreThan(3);
            ValCntList vn2 = ValueDic.GetMoreThan(2);
            int cnt3 = vn3.Count;
            if (laiziCnt > 0)
            {
                int laizic = laiziCnt;
                int cnt32 = Math.Min(vn2.Count - vn3.Count, laizic);
                laizic -= cnt32;
                cnt3 += cnt32;
                if (laizic > 1)
                {
                    int cnt31 = Math.Min(ValueDic.Count - vn2.Count, laizic / 2);
                    laizic -= cnt31 * 2;
                    cnt3 += cnt31;
                }
            }
            if (cnt3 <=1)
            {
                List<List<int>> getLx9 = GetLianXuCard(cards, laiziCnt, 9, true);
                bool isQuanShun = getLx9.Count!=0;
                bool isQuanColo = ColorDic.Count == 1;
                if (isQuanColo && LaiziHh)
                {
                    if (laiziCnt == 1)
                    {
                        if (Tools.CheckHongHei(ColorDic[0].Color))
                        {
                            isQuanColo = laizi[0] > 0;
                        }
                        else
                        {
                            isQuanColo = laizi[1] > 0;
                        }
                    }
                    else
                    {
                        isQuanColo = false;
                    }
                }

              
                if (isQuanShun)
                {
                    typeArray.Add(XipaiType.lianshun);
                    if (isQuanColo)
                    {
                        typeArray.Add(XipaiType.qinglianshun);
                        typeArray.Add(XipaiType.shunqing2);
                        typeArray.Add(XipaiType.shunqing3);
                        typeArray.Add(XipaiType.tonghuan);
                    }
                }

                if (!(isQuanShun && isQuanColo))
                {
                    //清顺
                    List<List<int>> shunzi3 = new List<List<int>>();
                    //先找到 不需要赖子的
                    foreach (var item in ColorDic)
                    {
                        List<int> checkLianxu = item.Cards;
                        List<List<int>> getLx = GetLianXuCard(checkLianxu, 0, 3, false);
                        shunzi3.AddRange(getLx);
                    }

                  
                    if (laiziCnt > 0)
                    {
                        List<int> checkCars = new List<int>(cards);

                        foreach (var sz in shunzi3)
                        {
                            foreach (var t in sz)
                            {
                                checkCars.Remove(t);
                            }
                        }


                        ColCntList check = new ColCntList(checkCars);
                        int lzcnt = laiziCnt;
                        int[] lzArr =(int[])laizi.Clone();

                        foreach (var cn in check)
                        {
                            if (LaiziHh)
                            {
                                if (Tools.CheckHongHei(cn.Color))
                                {
                                    if (lzArr[0] == 0)
                                    {
                                        continue;
                                    }
                                    List<int> checkLianxu = cn.Cards;
                                    List<List<int>> getLx = GetLianXuCard(checkLianxu, 1, 3, false);
                                    if (getLx.Count > 0)
                                    {
                                        shunzi3.AddRange(getLx);
                                        lzArr[0]--;
                                    }
                                }
                                else
                                {
                                    if (lzArr[1] == 0)
                                    {
                                        continue;
                                    }
                                    List<int> checkLianxu = cn.Cards;
                                    List<List<int>> getLx = GetLianXuCard(checkLianxu, 1, 3, false);
                                    if (getLx.Count > 0)
                                    {
                                        shunzi3.AddRange(getLx);
                                        lzArr[1]--;
                                    }
                                }
                            }
                            else
                            {
                                if (lzcnt == 0)
                                {
                                    continue;
                                }
                                List<int> checkLianxu = cn.Cards;
                                List<List<int>> getLx = GetLianXuCard(checkLianxu, 1, 3, false);
                                if (getLx.Count > 0)
                                {
                                    shunzi3.AddRange(getLx);
                                    lzcnt--;
                                }
                            }
                        }

                        if (LaiziHh)
                        {
                            lzcnt = lzArr[0] + lzArr[1];
                        }
                        if (lzcnt > 1)
                        {
                            shunzi3.Add(new List<int>());
                        }
                    }

                    if (shunzi3.Count > 1)
                    {
                        typeArray.Add(XipaiType.shunqing2);
                    }

                    if (shunzi3.Count > 2)
                    {
                        typeArray.Add(XipaiType.shunqing3);
                    }

                    //三同花
                    int lzCnt = laiziCnt;
                    int[] arr =(int[]) laizi.Clone();
                    int tonghuaCnt = 0;

                    foreach (var cn in ColorDic)
                    {
                        int cnt = cn.Cnt();
                        tonghuaCnt += cnt / 3;
                        if (lzCnt > 0)
                        {
                            if (LaiziHh)
                            {
                                if (Tools.CheckHongHei(cn.Color))
                                {
                                    if (cnt % 3 == 2)
                                    {
                                        tonghuaCnt += 1;
                                        arr[0]--;
                                    }
                                }
                                else
                                {
                                    if (cnt % 3 == 2)
                                    {
                                        tonghuaCnt += 1;
                                        arr[1]--;
                                    }
                                }
                            }
                            else
                            {
                                if (cnt % 3 == 2)
                                {
                                    tonghuaCnt += 1;
                                    lzCnt--;
                                }
                            }

                        }
                    }
                   
                    if (LaiziHh)
                    {
                        lzCnt = arr[0] + arr[1];
                    }
                    if (lzCnt > 1)
                    {
                        tonghuaCnt++;
                    }
                    if (tonghuaCnt > 2)
                    {
                        typeArray.Add(XipaiType.tonghuan);
                    }
                }
            }

            //全红 全黑
            int hongCnt = 0;
            int heiCnt = 0;

            foreach (var c in cards)
            {
                if (CheckHongHei(c))
                {
                    hongCnt++;
                }
                else
                {
                    heiCnt++;
                }
            }
            if (hongCnt >= 9)
            {
                typeArray.Add(XipaiType.quanhong);
            }
            if (heiCnt >= 9)
            {
                typeArray.Add(XipaiType.quanhei);
            }
            return typeArray;
        }
        private bool CheckHongHei(int card)
        {
            if (LaiziHh)
            {
                if (card == 0x61)
                {
                    return true;
                }
                if (card == 0x51)
                {
                    return false;
                }
            }
            else
            {
                if (card == 0x61|| card == 0x51)
                {
                    return true;
                }
            }
               
            int color =Tools.GetColor(card);
            if (color < 0x30)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public enum XipaiType
    {
        shunqing2,
        shunqing3,
        tonghuan,
        qinglianshun,
        lianshun,
        quanhong,
        quanhei


    }
}
