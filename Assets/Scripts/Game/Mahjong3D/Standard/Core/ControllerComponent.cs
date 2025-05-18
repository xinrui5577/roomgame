using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    /// <summary>
    /// 语言，震动等开关
    /// </summary>
    public enum CtrlSwitchType
    {
        None = 0,
        Open = 1,
        Close = 2,
    }

    public class ControllerComponent : BaseComponent, IGameReadyCycle, IReconnectedCycle
    {
        #region 回放特效控制
        public void ReplayPlaySound(int card)
        {
            ReplayPlaySound(card.ToString());
        }

        public void ReplayPlaySound(string soundName)
        {
            // 回放音效默认用男声
            var sound = "man";
            var source = sound;
            if (LanguageVoice == (int)CtrlSwitchType.Close)
            {
                source = App.GameKey + sound;
            }
            Facade.Instance<MusicManager>().Play(sound + "_" + soundName, source);
        }

        public void PlayOperateEffect(int chair, PoolObjectType effectType)
        {
            ReplayPlaySound(effectType.ToString());
            GameCenter.Hud.UIPanelController.PlayPlayerUIEffect(chair, effectType);
        }
        #endregion

        /// <summary>
        /// 一局结束状态
        /// </summary>
        private bool mSingleHuState;

        public bool SingleHuState
        {
            get { return mSingleHuState; }
            set
            {
                if (GameCenter.DataCenter.Room.RoomType == MahRoomType.FanKa)
                {
                    mSingleHuState = value;
                }
            }
        }

        /// <summary>
        /// 禁止出牌标志
        /// </summary>
        public bool ForbbidToken { get; set; }

        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
        }

        public void OnGameReadyCycle()
        {
            SingleHuState = false;
            ForbbidToken = false;
        }

        public void OnReconnectedCycle()
        {
            ForbbidToken = false;
        }

        public string GetSex(int sex)
        {
            return UserInfo.GetSexToMW(sex) == 0 ? "woman" : "man";
        }

        public void PlaySound(int chair, string soundName)
        {
            var data = GameCenter.DataCenter.Players[chair];
            var sound = GetSex(data.SexI);
            var source = sound;
            if (LanguageVoice == (int)CtrlSwitchType.Close)
            {
                source = App.GameKey + sound;
            }
            Facade.Instance<MusicManager>().Play(sound + "_" + soundName, source);
        }

        /// <summary>
        /// 语言设置控制
        /// </summary>
        public int LanguageVoice
        {
            get { return PlayerPrefs.GetInt(App.GameKey + "LanguageVoice"); }
            set
            {
                PlayerPrefs.SetInt(App.GameKey + "LanguageVoice", value);
                //设置快捷语和播放速率
                if (value == (int)CtrlSwitchType.Open)
                {
                    //切换快捷语
                    Facade.GetInterimManager<YxChatManager>().ChangeChatDbBundleName("ChatDb");
                    //设置音效播放速率
                    //Facade.Instance<MusicManager>().SoundSource.pitch = GameCenter.DataCenter.Config.TimeNormalAudioClipRate;
                }
                else if (value == (int)CtrlSwitchType.Close)
                {
                    Facade.GetInterimManager<YxChatManager>().ChangeChatDbBundleName(App.GameKey + "ChatDb");
                    //Facade.Instance<MusicManager>().SoundSource.pitch = GameCenter.DataCenter.Config.TimeLocalismAudioClipRate;
                }
            }
        }
    }
}