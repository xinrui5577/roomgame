using UnityEngine;
using YxFramwork.Common.Utils;

namespace Assets.Scripts.Game.duifen
{
    /// <summary>
    /// 全局变量
    /// </summary>
    public class DuifenGlobalData : YxGameData
    {

        private int _guoBet = 1;

        /// <summary>
        /// 开局的盲注值
        /// </summary>
        public int GuoBet
        {
            set { _guoBet = value > 0 ? value : 1; }
            get { return _guoBet; }
        }
        
        /// <summary>
        /// 是否接收到GameInfo了
        /// </summary>
        public bool IsGameInfo;
        /// <summary>
        /// 说话等待时间
        /// </summary>
        public int SpeakCd;
        /// <summary>
        /// 游戏是否进行中
        /// </summary>
        public bool IsGameing;
        /// <summary>
        /// 筹码之间的动画间隔
        /// </summary>
        public float BetSpeace = 0.05f;

        /// <summary>
        /// 是否是房间模式
        /// </summary>
        public bool IsRoomGame = false;

        /// <summary>
        /// 房主的id信息
        /// </summary>
        [HideInInspector]
        public int OwnerId = 0;

        /// <summary>
        /// 是否已经开始了游戏
        /// </summary>
        public bool IsPlayed
        {
            get { return CurRound > 0; }
        }

        [HideInInspector]
        public int CurRound = 0;

        private int _showGoldRate = 1;

        /// <summary>
        /// 显示筹码的面值
        /// </summary>
        public int ShowGoldRate
        {
            set { _showGoldRate = value > 0 ? value : 1; }
            get { return _showGoldRate; }
        }

        /// <summary>
        /// 将筹码转换为显示字
        /// </summary>
        /// <param name="gold"></param>
        /// <param name="isPic"></param>
        /// <returns></returns>
        public string GetShowGoldValue(int gold , bool isPic = false)
        {
            int val = gold > 0 ? gold : -gold;

            return (gold < 0 ? "-" : "") + Tool.Tools.GetShowGold((float)val / ShowGoldRate, isPic);
        }

        public bool IsRoomOwner
        {
            get { return GetPlayerInfo().Id == OwnerId; }
        }
    }
}
