using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.pdk.DDz2Common;
using Assets.Scripts.Game.pdk.DdzEventArgs;
using Assets.Scripts.Game.pdk.InheritCommon;
using Assets.Scripts.Game.pdk.PokerRule;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.pdk.DDzGameListener.AudioListener
{
    public class DDzAudioListener : ServEvtListener
    {
        /// <summary>
        /// 语言种类，p是普通话，""空字符是方言
        /// </summary>
        const string LanguageType = "p";

        //用于播放音效的声音列表
        [SerializeField] 
        protected AudioSource[] AudioSourcesList = new AudioSource[5];

        /// <summary>
        /// 标记声音大小
        /// </summary>
        private float _soundVolume;

        // Use this for initialization
        void Awake () {
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeGrab, OnTypeGrab);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypePass, OnTypePass);
            PdkGameManager.AddOnServResponseEvtDic(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Ddz2RemoteServer.AddOnVoiceChatEvt(OnUserSpeak);
        }

        private void OnUserSpeak(object sender, DdzbaseEventArgs args)
        {
            if (!App.GetGameData<GlobalData>().IsPlayVoiceChat) return;

            var param = args.IsfObjData;
            int len = param.ContainsKey("len") ? param.GetInt("len") : 1;

            StartCoroutine(MuteAudioTemply(len));
        }

        /// <summary>
        /// 暂时停止音频播放一段时间
        /// </summary>
        /// <returns></returns>
        private IEnumerator MuteAudioTemply(float stoptime)
        {
            foreach (AudioSource audiosource in AudioSourcesList)
            {
                audiosource.mute = true;
            }
            Facade.Instance<MusicManager>().SetMusicPause(true);
            //App.GetGameData<GlobalData>().MusicAudioSource.mute = true;

            yield return new WaitForSeconds(stoptime);
            foreach (AudioSource audiosource in AudioSourcesList)
            {
                audiosource.mute = false;
            }
            Facade.Instance<MusicManager>().SetMusicPause(false);
            //App.GetGameData<GlobalData>().MusicAudioSource.mute = false;
        }


        void Start()
        {
            App.GetGameData<GlobalData>().OnhandCdsNumChanged = OnSomeBodyHandCdsChanged;

            _soundVolume = PlayerPrefs.GetFloat(SettingCtrl.SoundValueKey, 1);

            SettingCtrl.OnSoundValueChangeEvt = OnSoundValueChange;
        }

        /// <summary>
        /// 当有调试声音大小时
        /// </summary>
        /// <param name="value"></param>
        void OnSoundValueChange(float value)
        {
            _soundVolume = value;
        }


        public override void RefreshUiInfo()
        {
           // throw new System.NotImplementedException();
        }

        /// <summary>
        /// 当有人叫了分了
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        protected void OnTypeGrab(object obj, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            //分数
            var score = data.GetInt(GlobalConstKey.C_Score);
            var seat = data.GetInt(GlobalConstKey.C_Sit);//座位
            var sex = App.GetGameData<GlobalData>().GetUserInfo(seat).GetShort(NewRequestKey.KeySex);
            if (sex != 0 && sex != 1) sex = 0;
            string soundName = "";
            if (score > 0)
            {
                soundName = LanguageType + sex + "_" + score + "fen";
            }
            else
            {
                soundName = LanguageType + sex + "_" + "bujiao";
            }


            PlayAudioOneShot(soundName);
        }


        /// <summary>
        /// 当是上家出牌时，之前说的话要消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypeOutCard(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            var sex = App.GetGameData<GlobalData>().GetUserInfo(data.GetInt(RequestKey.KeySeat)).GetShort(NewRequestKey.KeySex);
            if (sex != 0 && sex != 1) sex = 0;
            var cards = data.GetIntArray(RequestKey.KeyCards);
            string cardsAudioName =
                data.ContainsKey(NewRequestKey.KeyCardType) ? GetAudioNameBytypeFirst(cards, data.GetInt(NewRequestKey.KeyCardType)) : GetAudioNameBytypeFirst(cards);
            string soundName = LanguageType + sex + "_" + cardsAudioName;
            PlayAudioOneShot(soundName);

            PlayTexiaoSound(soundName);

            PlayAudioOneShot("Special_give");
        }


        private void PlayTexiaoSound(string soundName)
        {
            switch (soundName)
            {
                case "p0_3dai1":
                    {
                        PlayAudioOneShot("k_3morecard");
                        break;
                    }
                case "p0_3dai2":
                    {
                        PlayAudioOneShot("k_3morecard");
                        break;
                    }
                case "p1_3dai1":
                    {
                        PlayAudioOneShot("k_3morecard");
                        break;
                    }
                case "p1_3dai2":
                    {
                        PlayAudioOneShot("k_3morecard");
                        break;
                    }
                case "p0_sanshun":
                    {
                        PlayAudioOneShot("k_shunzi");
                        break;
                    }
                case "p0_shuangshun":
                    {
                        PlayAudioOneShot("k_shunzi");
                        break;
                    }
                case "p0_shunzi":
                    {
                        PlayAudioOneShot("k_shunzi");
                        break;
                    }
                case "p1_shuangshun":
                    {
                        PlayAudioOneShot("k_shunzi");
                        break;
                    }
                case "p1_shunzi":
                    {
                        PlayAudioOneShot("k_shunzi");
                        break;
                    }
                case "p0_huojian":
                    {
                        PlayAudioOneShot("k_huojian");
                        break;
                    }
                case "p1_huojian":
                    {
                        PlayAudioOneShot("k_huojian");
                        break;
                    }
            }
        }


        /// <summary>
        /// 有人不要
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnTypePass(object sender, DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            var seat = data.GetInt(GlobalConstKey.C_Sit);//座位
            var sex = App.GetGameData<GlobalData>().GetUserInfo(seat).GetShort(NewRequestKey.KeySex);
            if (sex != 0 && sex != 1) sex = 0;

            var i = Random.Range(0, 3);
            string buyaoTittle = "buyao";
            switch (i)
            {
                case 1:
                    buyaoTittle = "buyao_1";
                    break;
                case 2:
                    buyaoTittle = "buyao_2";
                    break;
            }

            string soundName = LanguageType + sex + "_" + buyaoTittle;
            PlayAudioOneShot(soundName);
        }

        /// <summary>
        /// 当有人手牌数量剩1张时
        /// </summary>
        /// <param name="userSeat"></param>
        /// <param name="cdsLeftNum"></param>
        private void OnSomeBodyHandCdsChanged(int userSeat,int cdsLeftNum)
        {
           // if (cdsLeftNum != 1 && cdsLeftNum != 2) return;
            if (cdsLeftNum != 1) return;

            var user = App.GetGameData<GlobalData>().GetUserInfo(userSeat);
            short sex = 0;
            if (user.ContainsKey(NewRequestKey.KeySex)) sex = user.GetShort(NewRequestKey.KeySex);
            if (sex != 0 && sex != 1) sex = 0;
            //string zhangshu = cdsLeftNum == 1 ? "1zhang" : "2zhang";
            string soundName = LanguageType + sex + "_" + "1zhang";
            PlayAudioOneShot(soundName);
        }

        /// <summary>
        /// 标记取到的audioSource
        /// </summary>
        private short _avlaudioI=0;
        /// <summary>
        /// 用一个极有可能没有被占用的AudioSource播放声音clip
        /// </summary>
        public void PlayAudioOneShot(string soundName){
            Facade.Instance<MusicManager>().Play(soundName);
/*            var soundClip = App.GetGameData<GlobalData>().GetSoundClip(soundName);
            if (soundClip == null) return;
            if (_avlaudioI >= AudioSourcesList.Length) _avlaudioI = 0;

            var listi = _avlaudioI++;
            AudioSourcesList[listi].volume = _soundVolume;
            AudioSourcesList[listi].PlayOneShot(soundClip);*/
        }

        private const string FeijiDaiChibang = "feiji";//"feijidaichibang";

        /// <summary>
        /// 获得声音文件的标记名字部分
        /// </summary>
        /// <param name="cards">获得的牌值（包括花色的）</param>
        /// <param name="cardType">卡牌类型</param>
        /// <returns></returns>
        public static string GetAudioName(int[] cards,int cardType=-1)
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
                    else
                    {
                        return "huojian";
                    }
                case 3:
                    return "3zhang" + str[1];
                case 4:

                    if (cardType == (int)CardType.C1122)
                        return "shuangshun";


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


                    bool cod0 = cardType == (int)CardType.C32;
                    bool cod1 = (str[0] == str[1] && str[3] == str[4]);
                    if (cod0 || cod1)
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
                        return "4dai2dui";
                    }
                    if (str[0] == str[1] && str[1] == str[2] && str[2] == str[3])
                    {
                        return "4dai2dui";
                    }
                    if (str[0] == str[1] && str[1] == str[2])
                    {
                        return FeijiDaiChibang;
                    }
                    if (str[0] != str[1])
                    {
                        if (str[1] == str[2] && str[2] == str[3])
                        {
                            return FeijiDaiChibang;
                        }
                    }
                    if (str[0] != str[1] && str[1] != str[2])
                    {
                        if (str[2] == str[3] && str[2] == str[4])
                        {
                            return FeijiDaiChibang;
                        }
                    }
                    if (str[0] == str[1] && str[2] == str[3] && str[0] != str[2])
                    {
                        return "shuangshun";
                    }
                    return "shunzi";
                case 10:
                    if (str[0] == str[1] && str[1] == str[2])
                    {
                        return FeijiDaiChibang;
                    }
                    if (str[1] != str[2])
                    {
                        if (str[2] == str[3] && str[2] == str[4])
                        {
                            return FeijiDaiChibang;
                        }
                    }
                    if (str[7] == str[8] && str[7] == str[9])
                    {
                        return FeijiDaiChibang;
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

        /// <summary>
        /// 先根据牌型进行声音名称的筛选
        /// </summary>
        /// <param name="cards"></param>
        /// <param name="cardType"></param>
        /// <returns></returns>
        public static string GetAudioNameBytypeFirst(int[] cards, int cardType = -1)
        {
            CardType cdType;
            if (cardType == -1)
                cdType = PokerRuleUtil.GetCdsType(cards);
            else
                cdType = (CardType)cardType;

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
                    else
                    {
                        return "huojian";
                    }
                case 3:
                    return "3zhang" + str[1];
                case 4:

                    if (cardType == (int)CardType.C1122)
                        return "shuangshun";


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


                    bool cod0 = cardType == (int)CardType.C32;
                    bool cod1 = (str[0] == str[1] && str[3] == str[4]);
                    if (cod0 || cod1)
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
                    //if (cardType == CardType.C1112223434)

                    if (str[2] == str[3] && str[3] == str[4] && str[4] == str[5])
                    {
                        return "4dai2dui";
                    }
                    if (str[4] == str[5] && str[5] == str[6] && str[6] == str[7])
                    {
                        return "4dai2dui";
                    }
                    if (str[0] == str[1] && str[1] == str[2] && str[2] == str[3])
                    {
                        return "4dai2dui";
                    }
                    if (str[0] == str[1] && str[1] == str[2])
                    {
                        return FeijiDaiChibang;
                    }
                    if (str[0] != str[1])
                    {
                        if (str[1] == str[2] && str[2] == str[3])
                        {
                            return FeijiDaiChibang;
                        }
                    }
                    if (str[0] != str[1] && str[1] != str[2])
                    {
                        if (str[2] == str[3] && str[2] == str[4])
                        {
                            return FeijiDaiChibang;
                        }
                    }
                    if (str[0] == str[1] && str[2] == str[3] && str[0] != str[2])
                    {
                        return "shuangshun";
                    }
                    return "shunzi";
                case 10:
                    if (cdType == CardType.C1112223434) return FeijiDaiChibang;

                    if (str[0] == str[1] && str[1] == str[2])
                    {
                        return FeijiDaiChibang;
                    }
                    if (str[1] != str[2])
                    {
                        if (str[2] == str[3] && str[2] == str[4])
                        {
                            return FeijiDaiChibang;
                        }
                    }
                    if (str[7] == str[8] && str[7] == str[9])
                    {
                        return FeijiDaiChibang;
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
