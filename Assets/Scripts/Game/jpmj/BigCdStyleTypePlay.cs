using System.Collections;
using System.Globalization;
using System.Linq;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon.DataDefine;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using System.Collections.Generic;

namespace Assets.Scripts.Game.jpmj
{
    public class BigCdStyleTypePlay : MonoBehaviour
    {
        private const int CaiShenGui2 = 1 << 1;				//双财神归位
        private const int CaiShenGui3 = 1 << 2;				//三财神归位
        private const int CaiShenGui4 = 1 << 3;				//四财神归为
        private const int CaiShenDiaoCaiShen = 1 << 4;		//财神掉财神
        private const int CaiShenZuoKezi = 1 << 5;			//财神做刻
        private const int CaiShenZuoNiu = 1 << 6;			//财神做牛
        private const int SiMeiRenHu = 1 << 7;				//四美人
        private const int SiFengQi = 1 << 8;                //四风齐
        private const int YingDiao = 1 << 9;				//硬掉
        private const int CaiShenGang = 1 << 10;			//财神暗杠
        private const int PphKey = 1 << 11;					//碰碰胡
        private const int HunYiSe = 1 << 12;				//混一色
        private const int QingYiSe = 1 << 13;				//清一色
        private const int TianHu = 1 << 14;					//天胡
        private const int DiHu = 1 << 15;					//地胡
        private const int QuanZiType = 1 << 16;				//全字
        private const int PingHu = 1 << 17;					//平胡
        private const int GgHu = 1 << 18;                   //杠杠胡
        private const int CaiShenDanDiao = 1 << 19;			//财神单调
        private const int YingDiaoCaiShen = 1 << 20;		//硬掉财神
        private const int BaGangTou = 1 << 21;				//八仙过海
        private const int SiBaiBan = 1 << 22;				//四白板
        private const int ChongCao6 = 1 << 23;				//6虫草
        private const int ChongCao7 = 1 << 24;				//7虫草
        private const int ChongCao8 = 1 << 25;				//8虫草
        private const int SanMeiRenHu = 1 <<26;				//三美人
        private const int Sgangtou = 1 << 27;				//三杠头
        private const int Qixiannv = 1 << 28;				//七仙女

        private readonly int[] _ctypeValues = new int[]
        {
            CaiShenGui2,
            CaiShenGui3,
            CaiShenGui4,
            CaiShenDiaoCaiShen,
            CaiShenDanDiao,
            YingDiaoCaiShen,
            YingDiao,
            CaiShenZuoKezi,
            CaiShenZuoNiu,
            SiMeiRenHu,
            SiFengQi,
            CaiShenGang,
            PphKey,
            HunYiSe,
            QingYiSe,
            TianHu,
            DiHu,
            QuanZiType,
            PingHu,
            GgHu,
            BaGangTou,
            SiBaiBan,
            ChongCao6,
            ChongCao7,
            ChongCao8,
            SanMeiRenHu,
            Sgangtou,
            Qixiannv
        };

        /// <summary>
        /// 三财神胡倒
        /// </summary>
        private const int Caishenhu3 = 1 << 1;
        /// <summary>
        /// 4财神胡倒
        /// </summary>
        private const int Caishenhu4 = 1 << 2;

        public GameObject ImgParent;
        public Image[] Wings;

       // private Image _wordImg;

        private int _value;

        private readonly Dictionary<int, GameObject> _cdtypeGobdic = new Dictionary<int, GameObject>();


        public int[] BigCdsPrfKeys;
        void Start()
        {
            foreach (var cdtype in _ctypeValues)
            {
                var cdprefab = ResourceManager.LoadAsset(cdtype.ToString(CultureInfo.InvariantCulture));
                if(cdprefab==null)continue;
                var gob = ImgParent.AddChild(cdprefab);
                _cdtypeGobdic[cdtype] = gob;
                gob.SetActive(false);
            }

            if (BigCdsPrfKeys != null)
            {
                var len = BigCdsPrfKeys.Length;

                for (int i = 0; i < len; i++)
                {
                    var cdprefab = ResourceManager.LoadAsset(BigCdsPrfKeys[i].ToString(CultureInfo.InvariantCulture));
                    if (cdprefab == null) continue;
                    var gob = ImgParent.AddChild(cdprefab);
                    _cdtypeGobdic[BigCdsPrfKeys[i]] = gob;
                    gob.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 某个/某几个开关是否开启
        /// </summary>
        /// <param name="type">开关的类型，一般为1,2,4,8...2^n,如果需要第3个和第5个开关是否同时开启则传入：[0x10100]=20</param>
        /// <param name="orgtypeValues">基础值</param>

        /// <returns></returns>
        public bool IsAllowSubCondition(int orgtypeValues, int type)
        {
            return (orgtypeValues & type) == type;
        }


        /// <summary>
        /// 单独开启一个开关
        /// </summary>
        /// <param name="condition">开关的类型，一般为1,2,4,8...2^n</param>
        public void EnableCondition(int condition)
        {
            _value = _value | condition;
        }



        private readonly List<int> _huTypeList = new List<int>();
        private short _sex;
        /// <summary>
        /// 收到胡牌信息后设置大牌特效
        /// </summary>
        /// <param name="tbdata"></param>
        /// <returns>返回要播放的秒数</returns>
        public int OnHuResult(TableData tbdata)
        {
            var huseatlist = tbdata.Result.HuSeat;
            _huTypeList.Clear();
            for (int i = 0; i < UtilData.CurrGamePalyerCnt; i++)
            {
                if (!huseatlist.Contains(i))
                {
                    continue;
                }

                var orghutype = tbdata.Result.CType[i];
                _huTypeList.AddRange(_ctypeValues.Where(type => IsAllowSubCondition(orghutype, type)));

                //加入3，5，6，7，8，9，10的连庄
                var lianzhuangNum = tbdata.Result.LianZhuangNumInfo[i];
                if (lianzhuangNum >= 3 && lianzhuangNum <= 10)//&& lianzhuangNum != 4)
                {
                    _huTypeList.Add(lianzhuangNum*100);
                }

                var orgHutype2 = tbdata.Result.CType2[i];
                if (IsAllowSubCondition(orgHutype2, Caishenhu3))
                {
                    //三财神胡倒的预设
                    _huTypeList.Add(333);
                }
                if (IsAllowSubCondition(orgHutype2, Caishenhu4))
                {
                    //四财神胡倒的预设
                    _huTypeList.Add(444);
                }

                _sex = tbdata.UserDatas[i].Sex;

                break;
            }
            PlayWordStyle();

            return _huTypeList.Count*3;
        }

        private Coroutine _playeCoroutine;
        private void PlayWordStyle()
        {
           
            if (_playeCoroutine!=null)
            {
                StopCoroutine(_playeCoroutine);
            }
            _playeCoroutine= StartCoroutine(PlayScale());
        }




        /// <summary>
        /// 重置资源
        /// </summary>
        public void Reset()
        {
            StopAllCoroutines();
            foreach (var image in Wings)
            {
                image.gameObject.SetActive(false);
            }

            foreach (var gob in _cdtypeGobdic.Values)
            {
                gob.SetActive(false);
            }
        }

        private IEnumerator PlayScale()
        {
            foreach (var htype in _huTypeList)
            {
                if (!_cdtypeGobdic.ContainsKey(htype)) continue;
                Facade.Instance<MusicManager>().PlayFromSource(htype.ToString(CultureInfo.InvariantCulture), _sex == 0 ? 0 : 1);
   

                var gob = _cdtypeGobdic[htype];
                gob.SetActive(true);
                var wordImg = gob.GetComponent<Image>();

                if (wordImg == null) continue;
                ImgParent.SetActive(true);

                wordImg.transform.localScale = Vector3.one;
                wordImg.SetNativeSize();

                var image = wordImg;
                int i = 0;
                while (i < 10)
                {
                    yield return new WaitForSeconds(0.01f);
                    image.transform.localScale += new Vector3(0.1f, 0.1f, 0);
                    i++;
                }
                i = 0;
                while (i < 3)
                {
                    yield return new WaitForSeconds(0.01f);
                    image.transform.localScale -= new Vector3(0.1f, 0.1f, 0);
                    i++;
                }

                i = 0;
                while (i < 3)
                {
                    yield return new WaitForSeconds(0.01f);
                    image.transform.localScale += new Vector3(0.1f, 0.1f, 0);
                    i++;
                }

                i = 0;
                while (i < 10)
                {
                    yield return new WaitForSeconds(0.01f);
                    image.transform.localScale -= new Vector3(0.1f, 0.1f, 0);
                    i++;
                }

                yield return new WaitForSeconds(2f);

                wordImg.transform.localScale = Vector3.one;
                wordImg.SetNativeSize();
                ImgParent.SetActive(false);
                gob.SetActive(false);
            }
        }
    }
}
