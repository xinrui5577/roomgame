using System.Collections.Generic;
using YxFramwork.ConstDefine;
using Sfs2X.Entities.Data;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class CpgModel
    {
        public List<int> Cards = new List<int>();
        public int OpChair { get { return Seat.ExSeatS2C(); } }
        public int FromChair { get { return FromChair.ExSeatS2C(); } }

        /// <summary>
        /// 操作牌
        /// </summary>
        public int OpCard;
        /// <summary>
        /// 吃碰杠类型 cpgprotocol
        /// </summary>
        public int Type;
        /// <summary>
        /// 被 cpg 的玩家
        /// </summary>
        public int FromSeat;
        /// <summary>
        /// cpg 的玩家
        /// </summary>
        public int Seat;
        /// <summary>
        /// 抓杠独有的  抢杠胡的时候处理
        /// </summary>
        public bool Ok;
        /// <summary>
        /// 麻将牌扣着
        /// </summary>
        public bool Hide;
        /// <summary>
        /// sign 指向
        /// </summary>
        public RelativeSeat RelativeSeat = RelativeSeat.None;

        public CpgModel() { }

        public CpgModel(ISFSObject data)
        {
            ParseData(data);
        }

        public virtual void ParseData(ISFSObject data)
        {
            if (data.ContainsKey(AnalysisKeys.KeyTType))
            {
                Type = data.GetInt(AnalysisKeys.KeyTType);
            }
            else if (data.ContainsKey(RequestKey.KeyType))
            {
                Type = data.GetInt(RequestKey.KeyType);
            }
            Hide = Type == CpgProtocol.AnGang;


            if (data.ContainsKey(RequestKey.KeyOpCard))
            {
                OpCard = data.GetInt(RequestKey.KeyOpCard);
                Cards.Add(OpCard);
            }
            else if (data.ContainsKey(RequestKey.KeyCard))
            {
                OpCard = data.GetInt(RequestKey.KeyCard);
                Cards.Add(OpCard);
            }

            if (data.ContainsKey(RequestKey.KeyCard))
            {
                FromSeat = data.GetInt("from");
            }
            else
            {
                FromSeat = -1;
            }
            Seat = data.TryGetInt(RequestKey.KeySeat);
            Ok = data.TryGetBool("ok");

            var cards = data.TryGetIntArray(RequestKey.KeyCards);
            if (cards != null) Cards.AddRange(cards);

            Cards.Sort((a, b) =>
            {
                if (a > b) return 1;
                if (a < b) return -1;
                return 0;
            });

            SetRelativeSeat(Seat, FromSeat);
        }

        /// <summary>
        /// 计算相对位置
        /// </summary>
        private void SetRelativeSeat(int seat, int formSeat)
        {
            if (formSeat == -1) return;

            int chair = seat.ExSeatS2C();
            int formChair = formSeat.ExSeatS2C();

            RelativeSeat = GameUtils.GetRelativeSeat(chair, formChair);
        }

        public override string ToString()
        {
            var str = "";
            for (int i = 0; i < Cards.Count; i++)
            {
                str += Cards[i] + " ";
            }
            return str;
        }
    }
}
