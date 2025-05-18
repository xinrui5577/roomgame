using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jh.Sound
{
    public class JhSound : MonoBehaviour
    {
        public enum EnAudio
        {
            Start,
            Ready,
            Follow,
            Add,
            Compare,
            GiveUp,
            Look,

            Talk,

            Chip,
            Win,
            Lost,
            Card,

            CompareAnimate,
            ResultGetChip
        }

        public class SoundData
        {
            public EnAudio Audio;
            public int Sex;
            public int Index;

            public SoundData(EnAudio audio)
            {
                Audio = audio;
            }

            public SoundData(EnAudio audio, int sex)
            {   
                Audio = audio;
                Sex = sex;
            }

            public SoundData(EnAudio audio, int sex, int index)
            {
                Audio = audio;
                Sex = sex;
                Index = index;
            }

            public SoundData(int sex, int index)
            {
                Sex = sex;
                Index = index;
            }
        }

        private Dictionary<EnAudio, string> AudioDic = new Dictionary<EnAudio, string>();

        void Start()
        {
            AudioDic[EnAudio.Start] = "start";
            AudioDic[EnAudio.Ready] = "ready";
            AudioDic[EnAudio.Follow] = "follow";
            AudioDic[EnAudio.Add] = "add";
            AudioDic[EnAudio.Compare] = "cmp";
            AudioDic[EnAudio.GiveUp] = "giveup";
            AudioDic[EnAudio.Look] = "watch";
            AudioDic[EnAudio.Talk] = "msg";
            AudioDic[EnAudio.Chip] = "g_addchip";
            AudioDic[EnAudio.Win] = "g_win";
            AudioDic[EnAudio.Lost] = "";
            AudioDic[EnAudio.Card] = "g_card";
            AudioDic[EnAudio.CompareAnimate] = "g_cmp_anim";
            AudioDic[EnAudio.ResultGetChip] = "g_get_chip";
        }


        public void PlayPersonSound(SoundData data)
        {
            string soundName = "";
            soundName += (data.Sex == 1) ? "m_" : "w_";
            soundName += AudioDic[data.Audio];
            if (data.Index != 0)
            {
                if (data.Audio == EnAudio.Follow)
                {
                    if (data.Index <= 3)
                    {
                        soundName += "" + data.Index;
                    }
                    else
                    {
                        soundName += "" + 3;
                    }
                }
                if (data.Audio == EnAudio.Start)
                {
                    if (data.Index <= 2)
                    {
                        soundName += "" + data.Index;
                    }
                    else
                    {
                        soundName += "" + 2;
                    }
                }

                if (data.Sex == 1)
                {
                    if (data.Audio == EnAudio.GiveUp)
                    {
                        if (data.Index <= 2)
                        {
                            soundName += "" + data.Index;
                        }
                        else
                        {
                            soundName += "" + 2;
                        }
                       
                    }
                }
            }

            Facade.Instance<MusicManager>().Play(soundName);
        }

        public void PlayPersonTalk(SoundData data)
        {
            string soundName = "";
            soundName += (data.Sex == 1) ? "m_" : "w_";
            soundName += AudioDic[EnAudio.Talk];
            soundName += "" + data.Index;
            Facade.Instance<MusicManager>().Play(soundName);
        }

        public void PlayerEffect(SoundData data)
        {
            if (AudioDic[data.Audio] == "") return;
            Facade.Instance<MusicManager>().Play(AudioDic[data.Audio]);
        }

        public void OnRecive(EventData data)
        {
            SoundData sData = (SoundData)data.Data;
            switch (data.Name)
            {
                case "PersonSound":
                    PlayPersonSound(sData);
                    break;
                case "PersonTalk":
                    PlayPersonTalk(sData);
                    break;
                case "PlayerEffect":
                    PlayerEffect(sData);
                    break;

            }
        }
    }
}
