using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{  
    public abstract class CpgData
    {
        //类型
        public virtual EnGroupType Type
        {
            get { return (EnGroupType)mType; }
        }

        public int Card = DefaultUtils.DefValue;//牌  吃碰杠

        public virtual int MahjongCount
        {
            get { return 0; }
        }

        /// <summary>
        /// 吃碰刚玩家的位置
        /// </summary>
        public int Chair;

        protected int mType;
        protected List<int> CardDatas = new List<int>();   //牌组
        protected List<int> AllCardDatas = new List<int>();
        protected int _fromSeat;
        protected int _seat;
        protected bool _dan;

        public List<int> GetCardDatas { get { return CardDatas; } }
        public List<int> GetAllCardDatas { get { return AllCardDatas; } }

        //cpg中有赖子牌，标记Icon
        public int Laizi;
        //cpg中有laizi1
        public int Laizi1;

        public int Fanpai;

        public bool Dan
        {
            get { return _dan; }
        }

        public int Seat
        {
            get { return _seat; }
            set { _seat = value; }
        }

        public int GetFromChair()
        {
            int target = -1;
            int fromInt = MahjongUtility.GetChairOnCpg(_fromSeat, Chair);
            switch (fromInt)
            {
                case 1:
                    target = 2;
                    break;
                case 2:
                    target = 1;
                    break;
                case 3:
                    target = 0;
                    break;
            }
            return target;
        }

        public virtual int AcrossIndex
        {
            get { return new System.Random().Next(1, 4); }
        }

        public virtual List<int> GetHardCards()
        {
            return CardDatas;
        }

        public virtual int GetOutPutCard()
        {
            return Card;
        }

        public virtual List<int> AllCards()
        {
            return AllCardDatas;
        }

        public abstract void SetAllCardDatas();

        public virtual void SetCardDatas()
        {
        }

        public virtual void SetXfdAllCards(List<int> xfdCards)
        {
        }

        public virtual void SetCard()
        {
            if (Card == DefaultUtils.DefInt) Card = CardDatas[0];
        }

        public virtual void ParseData(ISFSObject data)
        {
            if (data.ContainsKey(AnalysisKeys.KeyTType))
                mType = data.GetInt(AnalysisKeys.KeyTType);
            else if (data.ContainsKey(RequestKey.KeyType))
                mType = data.GetInt(RequestKey.KeyType);
            if (data.ContainsKey(RequestKey.KeyOpCard))
                Card = data.GetInt(RequestKey.KeyOpCard);
            else if (data.ContainsKey(RequestKey.KeyCard))
                Card = data.GetInt(RequestKey.KeyCard);
            if (data.ContainsKey("from"))
                _fromSeat = data.GetInt("from");
            if (data.ContainsKey("dan"))
            {
                _dan = true;
            }
            if (data.ContainsKey(RequestKey.KeySeat))
            {
                _seat = data.GetInt(RequestKey.KeySeat);
            }
            if (data.ContainsKey(RequestKey.KeyCards))
            {
                int[] values = data.GetIntArray(RequestKey.KeyCards);
                foreach (int value in values)
                {
                    CardDatas.Add(value);
                }
            }
            SetCard();
            SetCardDatas();
            SetAllCardDatas();
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < AllCardDatas.Count; i++)
            {
                str += AllCardDatas[i] + " ";
            }
            return str;
        }
    }

    public class CpgChi : CpgData
    {
        public override int MahjongCount
        {
            get { return 3; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            AllCardDatas.Add(Card);
            AllCardDatas.Sort((a, b) =>
            {
                if (a > b) return 1;
                if (a < b) return -1;
                return 0;
            });
        }
    }

    public class CpgPeng : CpgData
    {
        public override int MahjongCount
        {
            get { return 3; }
        }
        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            AllCardDatas.Add(Card);
        }

        public override void SetCardDatas()
        {
            while (MahjongCount - 1 != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }
    }

    public class CpgSelfGang : CpgData
    {
        public override int MahjongCount
        {
            get { return 4; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }

        public override void SetCardDatas()
        {
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }

        public override int GetOutPutCard()
        {
            //自己杠是自己手牌中的 不是别人打出来的
            return DefaultUtils.DefValue;
        }

    }

    public class CpgSelfGangBao : CpgSelfGang
    {
        public override int MahjongCount
        {
            get { return 4; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }

        public override void SetCardDatas()
        {
            while (MahjongCount != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }

        public override int GetOutPutCard()
        {
            //自己杠是自己手牌中的 不是别人打出来的
            return DefaultUtils.DefValue;
        }

    }

    public class CpgZhuaGang : CpgData
    {
        public override int MahjongCount
        {
            get { return 4; }
        }

        public bool Ok;                                 //抓杠独有的  抢杠胡的时候处理

        public override void SetAllCardDatas()
        {
            AllCardDatas.Add(Card);
            AllCardDatas.Add(Card);
            AllCardDatas.Add(Card);
            AllCardDatas.Add(Card);
        }

        public override void SetCardDatas()
        {
            CardDatas.Clear();
            CardDatas.Add(Card);
        }

        public override int GetOutPutCard()
        {
            //抓杠是自己手牌中的 不是别人打出来的
            return DefaultUtils.DefValue;
        }

        public override void ParseData(ISFSObject data)
        {
            base.ParseData(data);
            if (data.ContainsKey("ok"))
            {
                Ok = data.GetBool("ok");
            }
        }
    }

    public class CpgXfdGang : CpgZhuaGang
    {
        public override int MahjongCount
        {
            get { return CardDatas.Count; }
        }

        public override void SetAllCardDatas()
        {
            for (int i = 0; i < CardDatas.Count; i++)
            {
                AllCardDatas.Add(CardDatas[i]);
            }
        }

        public override void SetXfdAllCards(List<int> xfdCards)
        {
            foreach (int xfdCard in xfdCards)
            {
                AllCardDatas.Add(xfdCard);
            }
        }

        public override void SetCardDatas()
        {
        }

        public override void SetCard()
        {
        }
    }

    public class CpgAnJueGang : CpgSelfGang
    {
        public override int MahjongCount
        {
            get { return 3; }
        }
    }

    public class CpgOtherGang : CpgData
    {
        public override int MahjongCount
        {
            get { return 4; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
            AllCardDatas.Add(Card);
        }

        public override void SetCardDatas()
        {
            while (MahjongCount - 1 != CardDatas.Count)
            {
                CardDatas.Add(Card);
            }
        }
    }

    public class CpgXFGang : CpgData
    {
        public override int MahjongCount
        {
            get { return CardDatas.Count; }
        }

        public override EnGroupType Type
        {
            get { return EnGroupType.XFGang; }
        }

        public override void SetAllCardDatas()
        {
            AllCardDatas = new List<int>(CardDatas);
        }

        public override void SetCard()
        {
            //旋风杠无Card
        }

        public override int GetOutPutCard()
        {
            //旋风杠 不是杠别人的
            return DefaultUtils.DefValue;
        }
    }
}