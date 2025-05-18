using YxFramwork.Common.Utils;
using System.Collections.Generic;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ReplayUserData
    {
        public int Seat;
        public string Name;
        public float Gold;
        public float Gang;
        public bool IsTing;

        //自摸 2| 胡别人的牌 1| 打酱油的 0| HuTypePao -1
        public int HuType;

        public int Chair
        {
            get { return Seat.ExSeatS2C(); }
        }
    }

    public class MahjongReplayGameData : YxReplayGameData
    {
        /// <summary>
        /// 每一帧的数据
        /// </summary> 
        public Dictionary<int, ReplayFrameData> FrameMessage { get; set; }

        public Dictionary<int, ReplayFrameData> PlayerCards { get; set; }

        public List<ReplayUserData> ReplayUserDatas = new List<ReplayUserData>();

        protected override YxBaseGameUserInfo OnInitUser(Dictionary<string, object> userData)
        {
            int seat = int.Parse(userData["seat"].ToString());
            int gold = int.Parse(userData["coin_a"].ToString());
            int gang = int.Parse(userData["gang"].ToString());
            int type = int.Parse(userData["type"].ToString());
            var name = userData["nick_m"].ToString();

            ReplayUserData info = new ReplayUserData();
            info.Seat = seat;
            info.Gold = gold;
            info.Gang = gang;
            info.Name = name;
            info.HuType = type;

            ReplayUserDatas.Add(info);
            return base.OnInitUser(userData);
        }

        public ReplayUserData GetUserData(int seat)
        {
            for (int i = 0; i < ReplayUserDatas.Count; i++)
            {
                if (seat == ReplayUserDatas[i].Seat)
                {
                    return ReplayUserDatas[i];
                }
            }
            return null;
        }

        public void AnalysisFrameData()
        {
            FrameMessage = new Dictionary<int, ReplayFrameData>();
            PlayerCards = new Dictionary<int, ReplayFrameData>();

            //解析回放数据
            for (int i = 0; i < ReplayData.Count; i++)
            {
                int index = FrameMessage.Count;
                FrameMessage.Add(index, new ReplayFrameData(ReplayData[i].ToString()));
            }

            //添加自定义消息 GameOver
            var frameIndex = FrameMessage.Count;
            var frameData = new ReplayFrameData();
            frameData.SetFramePorocol(ReplayProls.GameOver);
            FrameMessage.Add(frameIndex, frameData);

            foreach (var item in FrameMessage)
            {
                if (item.Value.Porocol == ReplayProls.Allowcate)
                {
                    PlayerCards.Add(item.Value.OpChair, item.Value);
                }
            }
        }

        public void OnResetFrameDatas()
        {
            foreach (var item in FrameMessage)
            {
                item.Value.OnReset();
            }
        }
    }
}