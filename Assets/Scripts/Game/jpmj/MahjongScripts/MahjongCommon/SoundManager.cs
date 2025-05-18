using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon
{
    public enum EnCommonSound
    {
        begin,
        clock,
        get,
        lipai,
        liu_ju,
        saizi,
        select,
        send,
        zhuapai,
    }


    public enum EnGameSound
    {
        buhua,
        chi,
        hu,
        peng,
        gang,
        ting,
        zimo,
        mobao,
        youjin,
        shuangyou,
        sanyou,
        sanjindao,

    }

    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        public float MusicVolume
        {
            set
            {
                Facade.Instance<MusicManager>().MusicVolume = value;
            }
        }


        public float EffectVolume
        {
            set
            {
                Facade.Instance<MusicManager>().EffectVolume = value;
            }
        }

        private int[] _sex = new int[UtilDef.GamePlayerCnt];

        private string[] CommenName;

        private string[] GameName;

        private const string SortTalk = "msg_";

        void Awake()
        {
            Instance = this;
        }


        void Start()
        {
            CommenName = new[]
            {
                "begin",
                "clock",
                "get",
                "lipai",
                "liu_ju",
                "saizi",
                "select",
                "send",
                "zhuapai"
            };

            GameName = new[]
            {
                "buhua",
                "chi",
                "hu",
                "peng",
                "gang",
                "ting",
                "zimo",
                "mobao",

                //泉州 start         
                "youjin",
                "shuangyou",
                "sanyou",
                "sanjindao",
            };
        }

        public void PlayMj(int chair, int mjValue)
        {
            string fileName = GetSexString(chair) + mjValue;
            Facade.Instance<MusicManager>().Play(CommenName[(int)EnCommonSound.send]);
            int sex = _sex[chair];
            string source = MusicSource(sex);
            Facade.Instance<MusicManager>().Play(fileName, source);        
        }

        public void PlayGameEffect(int chair, EnGameSound index)
        {          
            string fileName = GetSexString(chair) + GameName[(int) index];
            int sex = _sex[chair];
            string source = MusicSource(sex);
            Facade.Instance<MusicManager>().Play(fileName, source);
        }

        public void PlayCommonEffect(EnCommonSound index)
        {
            Facade.Instance<MusicManager>().Play(CommenName[(int) index]);
        }

        public void PlaySortTalk(int chair, int index)
        {
            int sex = _sex[chair];
            string source = MusicSource(sex);         
            Facade.Instance<MusicManager>().Play(SortTalk + GetSexString(chair) + index, source);
        }

        private string MusicSource(int sex)
        {
            string source = "";
            if (sex == 0)
            {
                source = "woman";
            }
            else if (sex == 1)
            {
                source = "man";              
            }
            return source;
        }

        public void SetSex(int chair, int sex)
        {
            _sex[chair] = sex;
        }

        private string GetSexString(int chair)
        {
            return _sex[chair] == 1 ? "man_": "woman_";
        }

        void OnDestroy()
        {
            Instance = null;
        }
    }
}