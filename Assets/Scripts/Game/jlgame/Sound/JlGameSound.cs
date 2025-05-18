using System.Collections.Generic;
using Assets.Scripts.Game.jlgame.EventII;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jlgame.Sound
{
    public class JlGameSound : MonoBehaviour
    {
        public enum EnAudio
        {
            None,
            OutCard,
            FoldCard,
            HasFoldCard,
            KillDragon,
            Dragon,
            Click,
            Sendcard,
        }

        public class SoundData
        {
            public EnAudio Audio;
            public int Sex;
            public string CardName;

            public SoundData(EnAudio audio, int sex)
            {
                Audio = audio;
                Sex = sex;
            }

            public SoundData(EnAudio audio, int sex, int card)
            {
                Audio = audio;
                Sex = sex;
                var cardColor = card >> 4;
                var cardValue = card & 0x0f;
                var value = cardValue >= 10 ? cardValue.ToString() : cardValue.ToString("x");
                CardName = string.Format("{0}_{1}", cardColor, value);

            }
        }

        private Dictionary<EnAudio, string> AudioDic = new Dictionary<EnAudio, string>();

        protected void Start()
        {
            AudioDic[EnAudio.None] = "";
            AudioDic[EnAudio.OutCard] = "OutCard";
            AudioDic[EnAudio.FoldCard] = "FoldCard";
            AudioDic[EnAudio.HasFoldCard] = "HasFoldCard";
            AudioDic[EnAudio.KillDragon] = "KillDragon";
            AudioDic[EnAudio.Dragon] = "Dragon";
            AudioDic[EnAudio.Click] = "Click"; 
            AudioDic[EnAudio.Sendcard] = "Sendcard"; 
        }


        public void PlayCardSound(SoundData data)
        {
            string soundName = "";
            soundName += (data.Sex == 1) ? "m_" : "w_";
            soundName += data.CardName;
            Facade.Instance<MusicManager>().Play(soundName, data.Sex == 1 ? "man" : "woman");
        }
        public void PlayRemindSound(SoundData data)
        {
            string soundName = "";
            soundName += (data.Sex == 1) ? "m_" : "w_";
            soundName += AudioDic[data.Audio];
            Facade.Instance<MusicManager>().Play(soundName, data.Sex == 1?"man":"woman");
        }

        public void PlayEffect(SoundData data)
        {
            string soundName = AudioDic[data.Audio];
            Facade.Instance<MusicManager>().Play(soundName);
        }

        public void OnRecive(EventData data)
        {
            SoundData sData = (SoundData)data.Data;
            switch (data.Name)
            {
                case "CardSound":
                    PlayCardSound(sData);
                    break;
                case "RemindSound":
                    PlayRemindSound(sData);
                    break;
                case "PlayEffect":
                    PlayEffect(sData);
                    break;
            }
        }
    }
}
