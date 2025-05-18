using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel;
using Assets.Scripts.Game.ddz2.InheritCommon;
using Assets.Scripts.Game.ddz2.PokerRule;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.AudioListener
{
    public class DDzAudioListener : ServEvtListener
    {
        /// <summary>
        /// 语言种类，p是普通话
        /// </summary>
        public string LanguageType = "p";

        //用于播放音效的声音列表
        //[SerializeField] 
        //protected AudioSource[] AudioSourcesList = new AudioSource[5];

        private bool _isCounting;
        private float _waitTime;

        void PlayAndPauseBgSound(string soundName)
        {
            var audioClip = Facade.Instance<MusicManager>().GetAudioClip(soundName);
            if (audioClip == null) return;

            _waitTime = audioClip.length + 0.5f;
            PlayAudioOneShot(soundName);
            if (!_isCounting)
            {
                _isCounting = true;
                StartCoroutine(PauseSoundAndPlay());
            }
        }

        IEnumerator PauseSoundAndPlay()
        {
            Facade.Instance<MusicManager>().SetMusicPause(true);
            while (_isCounting && _waitTime >= 0)
            {
                yield return null;
                _waitTime -= Time.deltaTime;
            }
            _isCounting = false;
            _waitTime = 0;
            Facade.Instance<MusicManager>().SetMusicPause(false);
        }


        // Use this for initialization
        protected void Awake()
        {
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, OnTypeAllocate);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeGrab, OnTypeGrab);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypePass, OnTypePass);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeDoubleOver, OnDoubleOver);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyRemind, TimeRemind);
            Facade.EventCenter.AddEventListeners<string, string>(GlobalConstKey.PlaySound, PlayChunTian);
            Facade.EventCenter.AddEventListeners<string, string>(GlobalConstKey.PlaySoundAndPauseBgSound, PlayAndPauseBgSound);
           
        }
  
     


        private void OnTypeAllocate(DdzbaseEventArgs obj)
        {
            PlayAudioOneShot("k_start");
        }

        private void PlayChunTian(string soundName)
        {
            PlayAudioOneShot(soundName);
        }

        private void TimeRemind(DdzbaseEventArgs obj)
        {
            PlayAudioOneShot("k_remind");
        }

        /// <summary>
        /// 当收到加倍已经结束的信息
        /// </summary>
        protected virtual void OnDoubleOver(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var rates = data.GetIntArray("jiabei");
            var len = rates.Length;


            for (int i = 0; i < len; i++)
            {
                var userInfo = App.GetGameData<DdzGameData>().GetOnePlayerInfo(i);
                if (userInfo == null) continue;
                //if(i==landSeat)continue;//忽略地主喊叫

                var sex = userInfo.SexI;
                if (sex != 0 && sex != 1) sex = 0;
                string source = sex == 1 ? "man" : "woman";
                string soundName;

                if (rates[i] > 1)
                {
                    soundName = LanguageType + sex + "_jiabei";
                }
                else
                {
                    soundName = LanguageType + sex + "_bujiabei";
                }

                PlayAudioOneShot(soundName, source);
            }
        }


        protected void Start()
        {
            App.GetGameData<DdzGameData>().OnhandCdsNumChanged = OnHandCdsChanged;
        }
                        
        public override void RefreshUiInfo()
        {
            
        }

        /// <summary>
        /// 当有人叫了分了
        /// </summary>
        /// <param name="args"></param>
        protected void OnTypeGrab(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var gdata = App.GetGameData<DdzGameData>();
            //分数
            var score = data.GetInt(GlobalConstKey.C_Score);
            var seat = data.GetInt(GlobalConstKey.C_Sit);//座位
            var sex = gdata.GetOnePlayerInfo(seat, true).SexI;
            if (sex != 0 && sex != 1) sex = 0;
            string source = sex == 1 ? "man" : "woman";
            //string soundName;
            //if (score > 0)
            //{
            //    soundName = string.Format("{0}{1}_{2}fen", LanguageType, sex, score); //LanguageType + sex + "_" + score + "fen";    //叫很喊出分数
            //}
            //else
            //{
            //    soundName = string.Format("{0}{1}_{2}fen", LanguageType, sex, score);  //LanguageType + sex + "_" + "bujiao";   //0分布叫
            //}
            string soundName = string.Format("{0}{1}_{2}{3}", LanguageType, sex, score,
                gdata.RobModel ? "qiangdizhu" : "fen");
            PlayAudioOneShot(soundName, source);
        }


        /// <summary>
        /// 当是上家出牌时，之前说的话要消失
        /// </summary>
        /// <param name="args"></param>
        private void OnTypeOutCard(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int seat = data.GetInt(GlobalConstKey.C_Sit);
            var sex = App.GetGameData<DdzGameData>().GetOnePlayerInfo(seat, true).SexI;
            if (sex != 0 && sex != 1) sex = 0;
            var cards = data.GetIntArray(RequestKey.KeyCards);
            string source = sex == 1 ? "man" : "woman";
            //string soundName = string.Format("{0}{1}_{2}", LanguageType, sex, GetAudioName(cards)); // LanguageType + sex + "_" + GetAudioName(cards);

            string soundName;
            if (data.ContainsKey("ctype"))
            {
                int cardType = data.GetInt("ctype");
                soundName = string.Format("{0}{1}_{2}", LanguageType, sex,
                    GetAudioName(cards, (CardType) cardType));
            }
            else
            {
                soundName = string.Format("{0}{1}_{2}", LanguageType, sex, GetAudioName(cards));
            }

            PlayAudioOneShot(soundName, source);
            int cardCount = cards.Length;

            string outCardSoundName = cardCount > 3 ? "k_3morecard" : "k_outcard";
            PlayAudioOneShot(outCardSoundName);
        }

        



        /// <summary>
        /// 有人不要
        /// </summary>
        /// <param name="args"></param>
        private void OnTypePass(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var seat = data.GetInt(GlobalConstKey.C_Sit);//座位
            var sex = App.GetGameData<DdzGameData>().GetOnePlayerInfo(seat, true).SexI;
            if (sex != 0 && sex != 1) sex = 0;
            string source = sex == 1 ? "man" : "woman";
            string soundName = string.Format("{0}{1}_buyao", LanguageType, sex);
            PlayAudioOneShot(soundName, source);
        }

        /// <summary>
        /// 当有人手牌数量剩1张时
        /// </summary>
        /// <param name="userSeat"></param>
        private void OnHandCdsChanged(int userSeat)
        {
            var ddzPlayer = App.GetGameData<DdzGameData>().GetPlayer<DdzPlayer>(userSeat, true);
            if (ddzPlayer == null) return;
            int cardCount = ddzPlayer.CardCount;
            if (cardCount > 2) return;

            int sex = ddzPlayer.Info.SexI;

            string source = sex == 1 ? "man" : "woman";
            string soundName = string.Format("{0}{1}_{2}zhang", LanguageType, sex, cardCount); 
            PlayAudioOneShot(soundName, source);

            if (cardCount > 0)
                PlayAudioOneShot("Special_alert");
        }


        /// <summary>
        /// 用一个极有可能没有被占用的AudioSource播放声音clip
        /// </summary>
        public void PlayAudioOneShot(string audioName, string source = "AudioSources", int delayed = 0)
        {
            Facade.Instance<MusicManager>().Play(audioName, source, delayed);
        }


        /// <summary>
        /// 获得声音文件的标记名字部分
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="cardType">获得的牌值（包括花色的）</param>
        /// <returns></returns>
        public static string GetAudioName(int[] cards,CardType cardType)
        {
            var str = new string[cards.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                str[i] = cards[i].ToString("x").Substring(1, 1);

            }
            System.Array.Sort(str);

            switch (cardType)
            {
                case CardType.C1:
                    if (cards[0] == 81)
                    {
                        return "xiaowang";
                    }
                    if (cards[0] == 97)
                    {
                        return "dawang";
                    }
                    return str[0];

                case CardType.C123:
                    if (str[0] == str[1] && str[1] != str[2]) return "shuangshun";   //查看是否是双顺
                    return "shunzi";
                case CardType.C1122:
                    return "shuangshun";
                case CardType.C111222:
                    return "feiji";
                case CardType.C1112223344:
                case CardType.C11122234:
                    return "feijidaichibang";

                case CardType.C2:
                    if (cards[0] == 97 || cards[1] == 97 || cards[0] == 81 || cards[1] == 81)
                    {
                        return "huojian";
                    }
                    if (str[0] == str[1] || HaveMagic(cards))
                    {
                        if (HaveMagic(cards))
                        {
                            return "1dui" + str[1];
                        }
                        return "1dui" + str[0];
                    }
                    return "1dui" + str[1];

                case CardType.C3:
                    return "3zhang" + str[1];
                case CardType.C31:
                    return "3dai1";
                case CardType.C32:
                    return "3dai2";


                case CardType.C4:
                    if (HaveMagic(cards))     //如果有癞子
                    {
                        if (str[1] == str[2] && str[2] == str[3])
                        {
                            return "zhadan";
                        }
                    }
                    return str[0] != str[3] ? "3dai1" : "zhadan";

                case CardType.C411:
                    return "4dai2";

                case CardType.C42:
                    return "huojian";

                case CardType.C5:
                    //超级炸弹
                    break;

            }
            return string.Empty;
        }

        /// <summary>
        /// 获得声音文件的标记名字部分
        /// </summary>
        /// <param name="cards">获得的牌值（包括花色的）</param>
        /// <returns></returns>
        public static string GetAudioName(int[] cards)
        {
            var str = new string[cards.Length];
            for (int i = 0; i < cards.Length; i++)
            {
                str[i] = cards[i].ToString("x").Substring(1, 1);

            }
            System.Array.Sort(str);
            switch (cards.Length)
            {
                case 1:
                    if (cards[0] == 81)
                    {

                        return "xiaowang";
                    }
                    if (cards[0] == 97)
                    {
                        return "dawang";
                    }
                    return str[0];
                case 2:
                    if (cards[0] == 97 || cards[1] == 97 || cards[0] == 81 || cards[1] == 81)
                    {
                        return "huojian";
                    }
                    if (str[0] == str[1] || HaveMagic(cards))
                    {
                        if (HaveMagic(cards))
                        {
                            return "1dui" + str[1];
                        }
                        return "1dui" + str[0];

                    }

                    return "huojian";

                case 3:
                    return "3zhang" + str[1];
                case 4:
                    if (HaveMagic(cards))
                    {
                        if (str[1] == str[2] && str[2] == str[3])
                        {
                            return "zhadan";
                        }
                    }
                    if (str[0] != str[3])
                    {
                        return "3dai1";
                    }
                    return "zhadan";
                case 5:
                    if (HaveMagic(cards))
                    {
                        if (str[1] == str[2] && str[2] == str[3] && str[3] == str[4])
                        {

                            return "zhadan";
                        }
                    }

                    if (str[0] != str[1] && str[2] != str[3])
                    {

                        return "shunzi";
                    }
                    if (str[0] == str[1] && str[3] == str[4])
                    {
                        return "3dai2";
                    }
                    return "4dai1";
                case 6:

                    if (str[0] != str[1] && str[1] != str[2] && str[2] != str[3])
                    {
                        return "shunzi";
                    }

                    if (str[0] == str[1] && str[2] == str[3] && str[4] == str[5] && str[1] != str[2] && str[3] != str[4])
                    {
                        return "shuangshun";
                    }
                    if (str[0] == str[1] && str[1] == str[2] && str[3] == str[4] && str[4] == str[5])
                    {
                        return "feiji";
                    }
                    return "4dai2";
                case 8:
                    if (str[2] == str[3] && str[3] == str[4] && str[4] == str[5])
                    {
                        return "4dai2dui";
                    }
                    if (str[4] == str[5] && str[5] == str[6] && str[6] == str[7])
                    {
                        if (str[0] == str[1])
                            return "4dai2dui";
                        return "feijidaichibang";
                    }
                    if (str[0] == str[1] && str[1] == str[2] && str[2] == str[3])
                    {
                        return "4dai2dui";
                    }
                    if (str[0] == str[1] && str[1] == str[2])
                    {
                        return "feijidaichibang";
                    }
                    if (str[0] != str[1])
                    {
                        if (str[1] == str[2] && str[2] == str[3])
                        {
                            return "feijidaichibang";
                        }
                    }
                    if (str[0] != str[1] && str[1] != str[2])
                    {
                        if (str[2] == str[3] && str[2] == str[4])
                        {
                            return "feijidaichibang";
                        }
                    }
                    if (str[0] == str[1] && str[2] == str[3] && str[0] != str[2])
                    {
                        if (str[3] == str[4])
                        {
                            return "feijidaichibang";
                        }
                        return "shuangshun";
                    }
                    return "shunzi";
                case 10:
                    if (str[0] == str[1] && str[1] == str[2])
                    {
                        return "feijidaichibang";
                    }
                    if (str[1] != str[2])
                    {
                        if (str[2] == str[3] && str[2] == str[4])
                        {
                            return "feijidaichibang";
                        }
                    }
                    if (str[7] == str[8] && str[7] == str[9])
                    {
                        return "feijidaichibang";
                    }
                    if (str[0] == str[1])
                    {
                        return "shuangshun";
                    }
                    return "shunzi";
                default:
                    if (str.Length % 2 == 0)
                    {
                        if (str[0] == str[1])
                        {
                            return "shuangshun";
                        }
                        else
                        {
                            return "shunzi";
                        }
                    }
                    else
                    {
                        if (str[0] == str[1] && str[0] == str[2])
                        {
                            return "sanshun";
                        }

                        else
                        {
                            return "shunzi";
                        }
                    }
            }
        }

        private static bool HaveMagic(IEnumerable<int> cards)
        {
            return cards.Any(s => s == 113);
        }
    }

}
