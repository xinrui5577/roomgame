using Sfs2X.Entities.Data;


namespace Assets.Scripts.Game.paijiu
{
    public class Group
    {
        public int[] Cards;
        public int Type;

        public Group(int[] arr)
        {
            Cards = arr;
            Type = GetType(Cards);
        }

        public int CompareGroup(Group g)
        {
            if (g.Type > Type)
            {
                return 1;
            }
            else if (g.Type < Type)
            {
                return -1;
            }
            else
            {
                //如果 不是特殊牌型 比较牌的大小
                if (Type < (int)PaiJiuType.Digaojiu && g.Type < (int)PaiJiuType.Digaojiu)
                {
                    return CompareCard(Cards[1], g.Cards[1]);
                }
                return 0;
            }
        }

        public ISFSObject GetSfsObject()
        {
            SFSObject ret = new SFSObject();
            ret.PutInt("type", Type);
            ret.PutIntArray("cards", Cards);
            return ret;
        }

        public int CompareCard(int c1, int c2)
        {
            int bigVal1 = GetCardBigVal(c1);
            int bigVal2 = GetCardBigVal(c2);
            return bigVal2 - bigVal1 > 0 ? 1 : bigVal2 - bigVal1 < 0 ? -1 : 0;
        }

        public int GetCardBigVal(int card)
        {
            return card / 0x100;    //对16取整
        }
        bool isTianWangJiu = false;
        bool isGuiTou = false;

        public int GetType(int[] cards)
        {
            if (cards.Length != 2)
            {
                return (int)PaiJiuType.None;
            }
            //小的在前
            SortIntArray();

            if (isGuiTou)
            {
                //鬼子
                if (cards[1] == (int)PaiJiuType.Futou && GetCardVal(cards[0]) == 9)
                {
                    return (int)PaiJiuType.Guizi;
                }
            }

            //至尊宝
            if (cards[1] == (int)PaiJiuType.Ersi && cards[0] == (int)PaiJiuType.Dingsan)
            {
                return (int)PaiJiuType.Zhizhunbao;
            }
            
            if (isTianWangJiu)
            {
                //天王久
                if (cards[1] == (int)PaiJiuType.Tian && GetCardVal(cards[0]) == 9)
                {
                    return (int)PaiJiuType.Tianwang9;
                }
            }

            if (cards[1] == cards[0])
            {
                switch (cards[1])
                {
                    case (int)PaiJiuType.Tian://天
                        return (int)PaiJiuType.Tianwang9;
                    case (int)PaiJiuType.Di://地
                        return (int)PaiJiuType.ShuangDi;
                    case (int)PaiJiuType.Ren://人
                        return (int)PaiJiuType.ShuangEr;
                    case (int)PaiJiuType.E://鹅
                        return (int)PaiJiuType.ShuangEr;
                    case (int)PaiJiuType.Mei://梅
                    case (int)PaiJiuType.Changsan://长三
                    case (int)PaiJiuType.Bandeng://板凳
                        return (int)PaiJiuType.Shuang_Mei_San_Ban;
                    case (int)PaiJiuType.Gaojiaoqi://高脚
                    case (int)PaiJiuType.Tongcui6://凌林
                    case (int)PaiJiuType.Futou://斧头
                    case (int)PaiJiuType.Hongtoushi://红头
                        return (int)PaiJiuType.Shuang_Gao_Ling_Fu_Hong;
                }
            }
            else
            {
                int val1 = GetCardVal(cards[1]);
                int val0 = GetCardVal(cards[0]);
                int bigVal0 = GetCardBigVal(cards[0]);
                int bigVal1 = GetCardBigVal(cards[1]);

                if (val1 == val0 && bigVal0 == bigVal1)
                {
                    switch (val1)
                    {
                        case 9:
                            return (int)PaiJiuType.Za9;
                        case 8:
                            return (int)PaiJiuType.Za8;
                        case 7:
                            return (int)PaiJiuType.Za7;
                        case 5:
                            return (int)PaiJiuType.Za5;
                    }
                }
                else
                {

                    if (cards[1] == (int)PaiJiuType.Tian && val0 == 8)
                    {
                        return (int)PaiJiuType.Tiangang;
                    }
                    else if (val0 == 8 && cards[1] == (int)PaiJiuType.Di)
                    {
                        return (int)PaiJiuType.Digang;
                    }
                    else if (cards[1] == (int)PaiJiuType.Tian && cards[0] == (int)PaiJiuType.Cza7_25)
                    {
                        return (int)PaiJiuType.Tiangaojiu;
                    }
                    else if (cards[1] == (int)PaiJiuType.Di && cards[0] == (int)PaiJiuType.Gaojiaoqi)
                    {
                        return (int)PaiJiuType.Digaojiu;
                    }

                    return GetGroupDian();
                }
            }

            return (int)PaiJiuType.None;
        }
        /// <summary>
        /// 从小到大排序
        /// </summary>
        public void SortIntArray()
        {
            for (int i = 0; i < Cards.Length - 1; i++)
            {
                for (int j = i + 1; j < Cards.Length; j++)
                {
                    if (Cards[j] < Cards[i])
                    {
                        int temp = Cards[i];
                        Cards[i] = Cards[j];
                        Cards[j] = temp;
                    }
                }
            }
        }
        public int GetGroupDian()
        {
            int val1 = GetCardVal(Cards[1]);
            int val0 = GetCardVal(Cards[0]);

            return (val1 + val0) % 10;
        }

        public static int GetCardVal(int card)
        {
            return card % 0x10;
        }
    }
    //public class PaiJiuHelp
    //{
    

    //    public static int getCardVal(int card)
    //    {
    //        return card % 0x10;
    //    }
    //    bool isTianWangJiu = false;
    //    bool isGuiTou = false;
    //    //需要 排序好的数组 大的在后面
    //    public int GetType(int[] cards)
    //    {
    //        if (cards.Length != 2)
    //        {
    //            return (int)PaiJiuType.none;
    //        }
    //        //小的在前
    //        SortIntArray(ref cards);

    //        if (isGuiTou)
    //        {
    //            //鬼子
    //            if (cards[1] == (int)PaiJiuType.futou && getCardVal(cards[0]) == 9)
    //            {
    //                return (int)PaiJiuType.guizi;
    //            }
    //        }

    //        //至尊宝
    //        if (cards[1] == (int)PaiJiuType.ersi && cards[0] == (int)PaiJiuType.dingsan)
    //        {
    //            return (int)PaiJiuType.zhizhunbao;
    //        }
    //        isTianWangJiu = true;
    //        if (isTianWangJiu)
    //        {
    //            //天王久
    //            if (cards[1] == (int)PaiJiuType.tian && getCardVal(cards[0]) == 9)
    //            {
    //                return (int)PaiJiuType.tianwang_9;
    //            }
    //        }

    //        if (cards[1] == cards[0])
    //        {
    //            switch (cards[1])
    //            {
    //                case (int)PaiJiuType.tian://天
    //                    return (int)PaiJiuType.tianwang_9;
    //                case (int)PaiJiuType.di://地
    //                    return (int)PaiJiuType.shuang_di;
    //                case (int)PaiJiuType.ren://人
    //                    return (int)PaiJiuType.shuang_er;
    //                case (int)PaiJiuType.e://鹅
    //                    return (int)PaiJiuType.shuang_er;
    //                case (int)PaiJiuType.mei://梅
    //                case (int)PaiJiuType.changsan://长三
    //                case (int)PaiJiuType.bandeng://板凳
    //                    return (int)PaiJiuType.shuang_mei_san_ban;
    //                case (int)PaiJiuType.gaojiaoqi://高脚
    //                case (int)PaiJiuType.tongcui6://凌林
    //                case (int)PaiJiuType.futou://斧头
    //                case (int)PaiJiuType.hongtoushi://红头
    //                    return (int)PaiJiuType.shuang_gao_ling_fu_hong;
    //            }
    //        }
    //        else
    //        {
    //            int val1 = getCardVal(cards[1]);
    //            int val0 = getCardVal(cards[0]);

    //            if (val1 == val0)
    //            {
    //                switch (val1)
    //                {
    //                    case 9:
    //                        return (int)PaiJiuType.za9;
    //                    case 8:
    //                        return (int)PaiJiuType.za8;
    //                    case 7:
    //                        return (int)PaiJiuType.za7;
    //                    case 5:
    //                        return (int)PaiJiuType.za5;
    //                }
    //            }
    //            else
    //            {

    //                if (cards[1] == (int)PaiJiuType.tian && val0 == 8)
    //                {
    //                    return (int)PaiJiuType.tiangang;
    //                }
    //                else if (val1 == 8 && cards[0] == (int)PaiJiuType.di)
    //                {
    //                    return (int)PaiJiuType.digang;
    //                }
    //                else if (cards[1] == (int)PaiJiuType.tian && cards[0] == (int)PaiJiuType.cza7_25)
    //                {
    //                    return (int)PaiJiuType.tiangaojiu;
    //                }
    //                else if (cards[1] == (int)PaiJiuType.di && cards[0] == (int)PaiJiuType.gaojiaoqi)
    //                {
    //                    return (int)PaiJiuType.digaojiu;
    //                }

    //                return getGroupDian(cards);
    //            }
    //        }

    //        return (int)PaiJiuType.none;
    //    }

    //    /// <summary>
    //    /// 从小到大排序
    //    /// </summary>
    //    /// <param name="cards"></param>
    //    public void SortIntArray(ref int[] cards)
    //    {
    //        for (int i = 0; i < cards.Length - 1; i++)
    //        {
    //            for (int j = i + 1; j < cards.Length; j++)
    //            {
    //                if (cards[j] < cards[i])
    //                {
    //                    int temp = cards[i];
    //                    cards[i] = cards[j];
    //                    cards[j] = temp;
    //                }
    //            }
    //        }
    //    }

    //    public int getGroupDian(int[] cards)
    //    {
    //        int val1 = getCardVal(cards[1]);
    //        int val0 = getCardVal(cards[0]);

    //        return (val1 + val0) % 10;
    //    }

    //    public int getCardBigVal(int card)
    //    {
    //        return card / 0x100;
    //    }

    //    public int compareCard(int c1, int c2)
    //    {
    //        int val1 = getCardBigVal(c1);
    //        int val2 = getCardBigVal(c2);
    //        return val2 - val1 > 0 ? 1 : val2 - val1 < 0 ? -1 : 0;
    //    }
    //}
    public enum PaiJiuType
    {

        None = 0,
        Digaojiu = 100,
        Tiangaojiu = 101,
        Digang = 102,
        Tiangang = 103,
        Za5 = 104,
        Za7 = 106,
        Za8 = 107,
        Za9 = 108,
        // ReSharper disable once InconsistentNaming
        Shuang_Gao_Ling_Fu_Hong = 109,
        // ReSharper disable once InconsistentNaming
        Shuang_Mei_San_Ban = 110,
        ShuangEr = 111,
        ShuangRen = 112,
        ShuangDi = 113,
        Tianwang9 = 114,
        Zhizhunbao = 115,
        Guizi = 116,


        Tian = 0xC0C,
        Di = 0xB02,
        Ren = 0xA08,
        E = 0x904,
        Mei = 0x80A,
        Changsan = 0x806,
        Bandeng = 0x804,
        Futou = 0x70B,
        Hongtoushi = 0x70A,
        Gaojiaoqi = 0x707,
        Tongcui6 = 0x706,
        // ReSharper disable once InconsistentNaming
        Cza9_45 = 0x619,
        // ReSharper disable once InconsistentNaming
        Cza9_36 = 0x609,
        // ReSharper disable once InconsistentNaming
        Cza8_26 = 0x518,
        // ReSharper disable once InconsistentNaming
        Cza8_35 = 0x508,
        // ReSharper disable once InconsistentNaming
        Cza7_25 = 0x417,
        // ReSharper disable once InconsistentNaming
        Cza7_34 = 0x407,
        Ersi = 0x306,
        // ReSharper disable once InconsistentNaming
        Cza5_14 = 0x215,
        // ReSharper disable once InconsistentNaming
        Cza5_32 = 0x205,
        Dingsan = 0x103,
    }
    public enum OneCardType
    {
        Tongchuiliu = 0x706,
        Gaojiaoqi = 0x707,
        Hongtoushi = 0x70a,
        Futou = 0x70b,
        Bandeng = 0x804,
        Changsan = 0x806,
        Meipai = 0x80A,
        Epai = 0x904,
        Renpai = 0xa08,
        Dipai = 0xb02,
        Tianpai = 0xc0c,
    }
}
