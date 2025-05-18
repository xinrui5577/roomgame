using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class HuDetailItem : MonoBehaviour
    {
        public Text Description;
        public Text Score;
        public Text Players;

        public void SetData(MahjongResult.ScoreDetail detail)
        {
            Description.text = detail.Description;
            Score.text = detail.AmountScore.ToString();
            Players.text = SeatsDetail(detail.MatterSeats);
        }

        private string SeatsDetail(int[] seats)
        {
            if (seats.Length > 1)
            {
                return PlayAmount();
            }
            else
            {
                return PlaySeat(seats[0]);
            }
        }

        private string PlayAmount()
        {
            var date = GameCenter.DataCenter.MaxPlayerCount;
            switch (date)
            {
                case 2: return "对家";
                case 3: return "二家";
                case 4: return "三家";
            }
            return "";
        }

        private string PlaySeat(int seat)
        {
            int chair = MahjongUtility.GetChair(seat);
            var count = GameCenter.DataCenter.MaxPlayerCount;

            if (count == 2)
            {
                return "对家";
            }
            else if (count == 3)
            {
                if (chair == 1) return "上家";
                else return "下家";
            }
            else if (count == 4)
            {
                switch (chair)
                {
                    case 1: return "上家";
                    case 2: return "对家";
                    case 3: return "下家";
                }
            }
            return "";
        }
    }
}
