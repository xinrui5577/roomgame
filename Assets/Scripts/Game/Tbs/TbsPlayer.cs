using YxFramwork.Framework;
using UnityEngine;

namespace Assets.Scripts.Game.Tbs
{
    public class TbsPlayer : YxBasePlayer
    {
        /// <summary>
        /// 玩家下注区域
        /// </summary>
        public BetRegion UserBetRegion;
        /// <summary>
        /// 玩家手牌管理
        /// </summary>
        public BetPoker UserBetPoker;
        /// <summary>
        /// 聊天父节点
        /// </summary>
        public GameObject ChatFather;
        /// <summary>
        /// 聊天文字
        /// </summary>
        public UILabel ChatLabel;
        /// <summary>
        /// 聊天表情
        /// </summary>
        public UISprite ChatBrow;
        /// <summary>
        /// 语音聊天播放对象
        /// </summary>
        public AudioSource VoiceChat;
        /// <summary>
        /// 语音聊天播放动画
        /// </summary>
        public UISprite VoiceChatAnim;
        /// <summary>
        /// 庄家图标
        /// </summary>
        public GameObject BankerIcon;
        /// <summary>
        /// 是否可以退出
        /// </summary>
        public bool IsExit;

        public void Reset()
        {
            IsExit = true;
            UserBetPoker.Reset();
            UserBetRegion.ClearBet();
        }
    }
}
