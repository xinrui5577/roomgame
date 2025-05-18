using UnityEngine;
using YxFramwork.Manager;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public partial class MahjongUtility
    {
        /// <summary>
        /// 设置玩家音效播放速度
        /// </summary>
        public static float SetPlayerAudioClipRate
        {
            set { Facade.Instance<MusicManager>().SoundSource.pitch = value; }
        }

        public static string Sex(int sex)
        {
            return YxFramwork.Common.Model.UserInfo.GetSexToMW(sex) == 0 ? "woman" : "man";
        }

        //播放麻将音效
        public static void PlayMahjongSound(int chair, int value)
        {
            MahjongUserInfo data = GameCenter.DataCenter.Players[chair];
            if (data == null) return;

            string sound = Sex(data.SexI);
            string source = sound;
            if (LanguageVoice == (int)CtrlSwitchType.Close)
            {
                source = GameKey + sound;
            }
            if (GameCenter.DataCenter.Config.RondomPlayMahjongSound)
            {
                int n = Random.Range(0, 2);
                if (n == 1)
                {
                    PlaySound(sound + "_" + value + "_1", source);
                    return;
                }
            }
            PlaySound(sound + "_" + value, source);
        }

        public static void PlayPlayerSound(int chair, PoolObjectType effectType)
        {
            PlayPlayerSound(chair, effectType.ToString());
        }

        public static void PlayPlayerSound(int chair, string soundName)
        {
            MahjongUserInfo data = GameCenter.DataCenter.Players[chair];
            if (data == null) return;

            string sound = Sex(data.SexI);
            string source = sound;
            if (LanguageVoice == (int)CtrlSwitchType.Close)
            {
                source = GameKey + sound;
            }
            PlaySound(sound + "_" + soundName, source);
        }

        /// <summary>
        /// CPG等特效和声音
        /// </summary>
        /// <param name="chair">座位号</param>
        /// <param name="effectType">特效对象</param>
        /// <param name="music">特效声音</param>
        public static void PlayOperateEffect(int chair, PoolObjectType effectType)
        {
            MahjongUserInfo data = GameCenter.DataCenter.Players[chair];
            if (data == null) return;

            string sound = Sex(data.SexI);
            string source = sound;
            if (LanguageVoice == (int)CtrlSwitchType.Close)
            {
                source = GameKey + sound;
            }
            //播放特效
            GameCenter.Hud.UIPanelController.PlayPlayerUIEffect(chair, effectType);
            //播放特效声音
            PlaySound(sound + "_" + effectType.ToString(), source);
        }

        /// <summary>
        /// 播放环境特效和音效
        /// </summary>
        public static void PlayEnvironmentEffect(int chair, PoolObjectType effectType)
        {
            //播放特效
            GameCenter.Scene.PlayPlayerEffect(chair, effectType);
            //播放特效声音
            PlayEnvironmentSound(effectType.ToString());
        }

        /// <summary>
        /// 播发特效
        /// </summary>   
        public static EffectObject PlayMahjongEffect(PoolObjectType effectType)
        {
            return GameCenter.Pools.Pop<EffectObject>(effectType);
        }

        /// <summary>
        /// 播发特效和音效
        /// </summary>      
        public static EffectObject PlayMahjongEffectAndAudio(PoolObjectType effectType)
        {
            PlayEnvironmentSound(effectType.ToString());
            return PlayMahjongEffect(effectType);
        }

        public static void PlayOperateSound(int chair, string soundName)
        {
            MahjongUserInfo data = GameCenter.DataCenter.Players[chair];
            if (data == null) return;

            string sound = Sex(data.SexI);
            string source = sound;
            if (LanguageVoice == (int)CtrlSwitchType.Close)
                source = GameKey + sound;
            PlaySound(sound + "_" + soundName, source);
        }
    }
}