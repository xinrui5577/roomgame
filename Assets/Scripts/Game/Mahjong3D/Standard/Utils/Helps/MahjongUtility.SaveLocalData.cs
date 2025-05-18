using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class MahjongUtility
    {
        /// <summary>
        /// 游戏声音控制
        /// </summary>
        public static float MusicSound
        {
            get { return GetMusicVolume(); }
            set { SetMusicVolume(value); }
        }

        /// <summary>
        /// 游戏特效声音控制
        /// </summary>
        public static float MusicEffect
        {
            get { return GetEffectVolume(); }
            set { SetEffectVolume(value); }
        }

        /// <summary>
        /// 震动开关控制
        /// </summary>
        public static int ShakeCtrl
        {
            get { return PlayerPrefs.GetInt(GameKey + "ShakeCtrl"); }
            set { PlayerPrefs.SetInt(GameKey + "ShakeCtrl", value); }
        }

        /// <summary>
        /// 麻将牌颜色控制
        /// </summary>
        public static int MahjongCardColor
        {
            get { return PlayerPrefs.GetInt(GameKey + "MahjongCardColor"); }
            set { PlayerPrefs.SetInt(GameKey + "MahjongCardColor", value); }
        }

        /// <summary>
        /// 麻将桌颜色控制
        /// </summary>
        public static int MahjongTableColor
        {
            get { return PlayerPrefs.GetInt(GameKey + "MahjongTableColor"); }
            set { PlayerPrefs.SetInt(GameKey + "MahjongTableColor", value); }
        }


        /// <summary>
        /// 听牌提示开关
        /// </summary>
        public static int TingTipCtrl
        {
            get { return PlayerPrefs.GetInt(GameKey + "TingTipCtrl"); }
            set { PlayerPrefs.SetInt(GameKey + "TingTipCtrl", value); }
        }

        /// <summary>
        /// 方言设置控制
        /// </summary>
        public static int LanguageVoice
        {
            get { return PlayerPrefs.GetInt(GameKey + "LanguageVoice"); }
            set
            {
                PlayerPrefs.SetInt(GameKey + "LanguageVoice", value);
                //设置快捷语和播放速率
                if (value == (int)CtrlSwitchType.Open)
                {
                    //切换快捷语
                    var chat = Facade.GetInterimManager<YxChatManager>();
                    if (chat != null )
                    {
                        chat.ChangeChatDbBundleName("ChatDb");
                    }
                    //设置音效播放速率
                    SetPlayerAudioClipRate = GameCenter.DataCenter.Config.TimeNormalAudioClipRate;
                }
                else if (value == (int)CtrlSwitchType.Close)
                {
                    Facade.GetInterimManager<YxChatManager>().ChangeChatDbBundleName(GameKey + "ChatDb");
                    SetPlayerAudioClipRate = GameCenter.DataCenter.Config.TimeLocalismAudioClipRate;
                }
            }
        }

        /// <summary>
        /// 保存房间内其他玩家id，用于重新创建房间之后邀请好友
        /// </summary>
        public static string RecordCountineGameData
        {
            get { return PlayerPrefs.GetString("CountineGameData"); }
            set { PlayerPrefs.SetString("CountineGameData", value); }
        }
    }
}