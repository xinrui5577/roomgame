using System.Collections.Generic;
using Assets.Scripts.Game.biji.EventII;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.biji.Sound
{
    public class BjSound : MonoBehaviour
    {
        public enum EnAudio
        {
            None,
            WuLong,
            DuiZi,
            ShunZi,
            TongHua,
            TongHuaShun,
            SanTiao,
            StartGame,
            StartCompare,
        }

        public class SoundData
        {
            public EnAudio Audio;
            public int Sex;
            public int Index;
            public int CardValue;

            public SoundData(EnAudio audio)
            {
                Audio = audio;
            }

            public SoundData(EnAudio audio, int sex)
            {
                Audio = audio;
                Sex = sex;
            }
        }

        private Dictionary<EnAudio, string> AudioDic = new Dictionary<EnAudio, string>();

        void Start()
        {
            AudioDic[EnAudio.None] = "";
            AudioDic[EnAudio.WuLong] = "wulong";
            AudioDic[EnAudio.DuiZi] = "duizi";
            AudioDic[EnAudio.ShunZi] = "shunzi";
            AudioDic[EnAudio.TongHua] = "tonghua";
            AudioDic[EnAudio.TongHuaShun] = "tonghuashun";
            AudioDic[EnAudio.SanTiao] = "santiao";
            AudioDic[EnAudio.StartGame] = "startgame";
            AudioDic[EnAudio.StartCompare] = "startcompare";
        }


        public void PlayPersonSound(SoundData data)
        {
            string soundName = "";
            string sourceName = data.Sex == 1 ? "man" : "woman";
            soundName += (data.Sex == 1) ? "m_" : "w_";
            soundName += AudioDic[data.Audio];

            Facade.Instance<MusicManager>().Play(soundName, sourceName);
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
                case "PlayerEffect":
                    PlayerEffect(sData);
                    break;

            }
        }
    }
}
