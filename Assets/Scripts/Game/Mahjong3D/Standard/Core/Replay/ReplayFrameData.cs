using System.Collections.Generic;
using System.Text;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 回放的帧数据
    /// </summary>
    public class ReplayFrameData
    {
        /// <summary>
        /// 上一帧数据
        /// </summary>
        public ReplayFrameData LastFrameData;
        /// <summary>
        /// 当前操作
        /// </summary>
        public int OpChair { get; private set; }
        /// <summary>
        /// 协议号
        /// </summary>
        public int Porocol { get; private set; }
        /// <summary>
        /// 手牌
        /// </summary>
        public List<int> Cards { get; private set; }
        /// <summary>
        /// 完整消息
        /// </summary>
        private string mMessage;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(mMessage)) return "";

            var builder = new StringBuilder();
            builder.Append("Seat : " + OpChair);
            builder.Append("  Porocol : " + Porocol);
            builder.Append("  Cards : ");
            for (int i = 0; i < Cards.Count; i++)
            {
                builder.Append(" " + Cards[i]);
            }
            return builder.ToString();
        }

        public ReplayFrameData() { }

        public void SetFramePorocol(int porocol)
        {
            Porocol = porocol;
        }

        public ReplayFrameData(string message)
        {
            Analysis(message);
        }

        public void SetLastFrameData(ReplayFrameData lastFrame)
        {
            if (lastFrame != null)
            {
                //避免循环引用
                lastFrame.LastFrameData = null;
                LastFrameData = lastFrame;
            }
        }

        public void Analysis(string message)
        {
            mMessage = message;
            var strArray = message.Split(':');

            //解析座位号和协议号
            var str1 = strArray[0];
            var strData = str1.Split(',');
            OpChair = int.Parse(strData[0]);
            Porocol = int.Parse(strData[1]);

            //解析牌数据
            AnalysisCards(strArray[1]);
        }

        private void AnalysisCards(string cards)
        {
            Cards = new List<int>();
            if (cards.Equals("0"))
            {
                Cards.Add(0);
            }
            else
            {
                var datas = LitJson.JsonMapper.ToObject(cards);
                if (datas.IsArray)
                {
                    for (int i = 0; i < datas.Count; i++) Cards.Add((int)datas[i]);
                }
                else
                {
                    Cards.Add(int.Parse(cards));
                }
            }
        }

        public void OnReset()
        {
            LastFrameData = null;
        }
    }
}
