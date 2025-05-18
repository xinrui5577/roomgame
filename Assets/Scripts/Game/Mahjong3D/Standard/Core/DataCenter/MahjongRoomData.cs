using System.Collections.Generic;
using Sfs2X.Entities.Data;
using System.Linq;
using System;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum MahGameLoopType
    {
        Round,      //用局算
        Circle,     //用圈算
    }

    public enum MahRoomType
    {
        FanKa,
        YuLe,
    }

    public class MahjongRoomData : IRuntimeData
    {
        public int RoomID;
        public int MaxRound;
        public int NextBaner;//下一局庄家
        public string TeaID; //茶馆ID
        public int ConsumeNum; //消耗品数量 
        public List<int> SysCards;
        public string ConsumeType = string.Empty; //开房用的消耗品类型 coin_a 金币 cash_a 元宝 item 房卡  

        private int mCurrRound;

        public MahRoomType RoomType { get; private set; }
        public MahGameLoopType LoopType { get; private set; }

        public int CurrRound
        {
            get { return mCurrRound > MaxRound ? MaxRound : mCurrRound; }
            set { mCurrRound = value; }
        }

        /// <summary>
        /// 实际局数，按圈计算时，可能打很多局
        /// </summary>
        public int RealityRound { get; set; }

        public int MahjongCount
        {
            get
            {
                int value = 0;
                if (SysCards != null)
                {
                    for (int i = 0; i < SysCards.Count; i++)
                    {
                        value += GameUtils.GetMahjongCardAount(SysCards[i]);
                    }
                    return value;
                }
                else return 0;
            }
        }

        public void ResetData() { }

        public void SetData(ISFSObject data)
        {
            RoomID = data.TryGetInt("rid");
            TeaID = data.TryGetString("teaId");
            MaxRound = data.TryGetInt("maxRound");
            //消耗品类型和数量    
            ConsumeNum = data.TryGetInt("consumeNum");
            ConsumeType = data.TryGetString("consumeType");
            //设置圈或局 
            RealityRound = data.TryGetInt("round");
            if (data.ContainsKey("quan"))
            {
                LoopType = MahGameLoopType.Circle;
                CurrRound = data.GetInt("quan") + 1;
            }
            else if (data.ContainsKey("round"))
            {
                LoopType = MahGameLoopType.Round;
                //lisi--修改--start--
                //原来：
                //CurrRound = data.ContainsKey("seq") ? RealityRound : RealityRound + 1;
                CurrRound = RealityRound;
                //lisi--end--
                RealityRound = CurrRound;
            }
            //房卡还是金币房
            if (data.ContainsKey("gtype"))
            {
                var type = data.GetInt("gtype");
                if (type >= 0)
                {
                    RoomType = MahRoomType.YuLe;
                }
                else
                {
                    RoomType = MahRoomType.FanKa;
                }
            }
            //获取麻将牌值
            if (data.ContainsKey("pcards"))
            {
                string cardsStr = data.GetUtfString("pcards");
                char[] cards = cardsStr.ToArray();
                SysCards = new List<int>();
                for (int i = 0; i < cards.Length / 2; i++)
                {
                    string card = new string(new[] { cards[i * 2], cards[i * 2 + 1] });
                    int cardValue = Convert.ToInt32(card, 16);
                    SysCards.Add(cardValue);
                }
                SysCards.Sort((a, b) =>
                {
                    if (a > b) return 1;
                    if (a < b) return -1;
                    return 0;
                });
            }
            NextBaner = data.ContainsKey("nextBank") ? data.GetInt("nextBank") : -1;
        }
    }
}