using System.Collections.Generic;
using Assets.Scripts.Game.jpmj.MahjongScripts.MahjongCommon;
using Assets.Scripts.Game.jpmj.MahjongScripts.Public;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{ //2D麻将的朝向
    public enum EnD2MjType
    {
        Up,
        Me,
    }

    public class D2MahjongMng : MonoBehaviour
    {

        public static D2MahjongMng Instance;

        public Sprite[] MahjongUpValues;
        public Sprite[] MahjongMeValues;
        public Sprite BgUp;
        public Sprite BgMe;

        public GameObject SingleMe;
        public GameObject SingleUp;

        public Sprite Bg;

        protected Dictionary<int, Sprite> _mahjongUpKv = new Dictionary<int, Sprite>();
        protected Dictionary<int, Sprite> _mahjongMeKv = new Dictionary<int, Sprite>();

        public bool IsContainMahjongMeKey(int value)
        {
            return _mahjongMeKv.ContainsKey(value);
        }

        protected Vector2 _mjSizeUp;
        protected Vector2 _mjSizeMe;

        /// <summary>
        /// 麻将牌背面
        /// </summary>
        public GameObject TileBackUp;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                Init();
            }
        }

        public void Init()
        {
            for (int i = 0; i < MahjongUpValues.Length; i++)
            {
                string name = MahjongUpValues[i].name;
                name = name.Trim("tile_meUp_".ToCharArray());
                _mahjongUpKv.Add(int.Parse(name), MahjongUpValues[i]);
            }

            for (int i = 0; i < MahjongMeValues.Length; i++)
            {
                string name = MahjongMeValues[i].name;
                name = name.Trim("tile_me_".ToCharArray());
                _mahjongMeKv.Add(int.Parse(name), MahjongMeValues[i]);
            }

            _mjSizeUp = new Vector2(Instance.BgUp.rect.width, Instance.BgUp.rect.height);
            _mjSizeMe = new Vector2(Instance.BgMe.rect.width, Instance.BgMe.rect.height);
        }

        public UiCardGroup GetGroup(int[] value, EnD2MjType type, bool isHaveBg = false, int laizi = UtilDef.NullMj, EnGroupType cpgType = EnGroupType.None)
        {
            GameObject[] list = new GameObject[value.Length];
            Vector2 mjSize = new Vector2();
            for (int i = 0; i < value.Length; i++)
            {
                list[i] = GetMj(value[i], type, laizi, cpgType,i);

                switch (type)
                {
                    case EnD2MjType.Me:
                        mjSize = MjSizeMe;
                        break;
                    case EnD2MjType.Up:
                        mjSize = MjSizeUp;
                        break;
                }
            }
            //tileBack_up_0
            UiCardGroup cardGroup = UiCardGroup.create(list, mjSize, isHaveBg ? Bg : null, cpgType);
            return cardGroup;
        }


        #region 精品麻将新加的方法
        /// <summary>
        /// 精品麻将用的方法,结算时候显示用
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <param name="isHaveBg"></param>
        /// <param name="laizi">财神</param>
        /// <param name="gangtou">杠头</param>
        /// <param name="cpgType"></param>
        /// <returns></returns>
        public UiCardGroup GetGroupForJp(int[] value, EnD2MjType type, bool isHaveBg = false, int laizi = UtilDef.NullMj,int gangtou = UtilDef.NullMj, EnGroupType cpgType = EnGroupType.None)
        {
            GameObject[] list = new GameObject[value.Length];
            Vector2 mjSize = new Vector2();
            for (int i = 0; i < value.Length; i++)
            {
                list[i] = GetMjForJp(value[i], type, laizi, cpgType, i, gangtou);

                switch (type)
                {
                    case EnD2MjType.Me:
                        mjSize = MjSizeMe;
                        break;
                    case EnD2MjType.Up:
                        mjSize = MjSizeUp;
                        break;
                }
            }
            //tileBack_up_0
            UiCardGroup cardGroup = UiCardGroup.create(list, mjSize, isHaveBg ? Bg : null, cpgType);
            return cardGroup;
        }

        private GameObject GetMjForJp(int value, EnD2MjType type, int laizi = UtilDef.NullMj, EnGroupType cpgType = EnGroupType.None, int cdindex = 0, int gangtou = UtilDef.NullMj)
        {
            GameObject ret = null;

            //暗杠时显示背面  只在精品麻将中有这种限制,第四张要反过来
            if (GameConfig.GameKey == "jpmj" && cpgType == EnGroupType.AnGang && TileBackUp != null && cdindex < 3)
            {
                ret = Instantiate(TileBackUp);
                return ret;
            }


            switch (type)
            {
                case EnD2MjType.Me:
                    ret = Instantiate(SingleMe);
                    Image valueImageMe = ret.transform.FindChild("CdValue").GetComponent<Image>();

                    //修正花牌显示
                    if (value >= 96)
                        valueImageMe.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

                    valueImageMe.sprite = Instantiate(_mahjongMeKv[value]);
                    valueImageMe.SetNativeSize();
                    break;
                case EnD2MjType.Up:
                    ret = Instantiate(SingleUp);
                    Image valueImage = ret.transform.FindChild("CdValue").GetComponent<Image>();

                    //修正花牌显示
                    if (value >= 96)
                        valueImage.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

                    valueImage.sprite = Instantiate(_mahjongUpKv[value]);
                    valueImage.SetNativeSize();
                    break;
            }

            //如果花牌为赖子牌
            if (laizi != UtilDef.NullMj)
            {
                bool result = false;

                if (value == laizi) result = true;

                if (laizi >= 96 && value >= 96)
                    result = true;

                ret.transform.Find("Laizi").gameObject.SetActive(result);
            }

            //添加杠头
            if (gangtou != UtilDef.NullMj && value == gangtou)
            {
                var o = ret.transform.Find("GangTou").gameObject;
                if (o != null)
                    o.SetActive(true);
            }

            //针对精品麻将的杠头显示
            if (GameConfig.GameKey == "jpmj")
            {
                var cod1 = gangtou >= 96 && gangtou <= 103;
                var cod2 = value >= 96 && value <= 103;

                if (cod1 && cod2)
                {
                    var o = ret.transform.Find("GangTou").gameObject;
                    if (o != null)
                        o.SetActive(true);
                }
            }
            return ret;
        }


        #endregion

        public virtual UiCardGroup GetXjfdGroup(int[] value, Dictionary<int, int> signs, EnD2MjType type, bool isHaveBg = false,
            int laizi = UtilDef.NullMj)
        {
            return null;
        }

        public UiCardGroup GetGroup(int value, EnD2MjType type, bool isHaveBg = false, int laizi = UtilDef.NullMj)
        {
            GameObject[] list = new GameObject[1];
            Vector2 mjSize = new Vector2();
            list[0] = GetMj(value, type, laizi);
            switch (type)
            {
                case EnD2MjType.Me:
                    mjSize = MjSizeMe;
                    break;
                case EnD2MjType.Up:
                    mjSize = MjSizeUp;
                    break;
            }

            UiCardGroup cardGroup = UiCardGroup.create(list, mjSize, isHaveBg ? Bg : null);
            return cardGroup;
        }


        public virtual GameObject GetMj(int value, EnD2MjType type, int laizi = UtilDef.NullMj, EnGroupType cpgType = EnGroupType.None,int cdindex=0)
        {
            GameObject ret = null;

            //暗杠时显示背面  只在精品麻将中有这种限制,第四张要反过来
            if (GameConfig.GameKey == "jpmj" && cpgType == EnGroupType.AnGang && TileBackUp != null && cdindex<3)
            {
                ret = Instantiate(TileBackUp);
                return ret;
            }

            switch (type)
            {
                case EnD2MjType.Me:
                    ret = Instantiate(SingleMe);
                    Image valueImageMe = ret.transform.FindChild("CdValue").GetComponent<Image>();

                    //修正花牌显示
                    if (value >= 96)
                        valueImageMe.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

                    valueImageMe.sprite = Instantiate(_mahjongMeKv[value]);
                    valueImageMe.SetNativeSize();
                    break;
                case EnD2MjType.Up:
                    ret = Instantiate(SingleUp);
                    Image valueImage = ret.transform.FindChild("CdValue").GetComponent<Image>();

                    //修正花牌显示
                    if (value >= 96)
                        valueImage.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

                    valueImage.sprite = Instantiate(_mahjongUpKv[value]);
                    valueImage.SetNativeSize();
                    break;
            }

            //如果花牌为赖子牌
            if (laizi != UtilDef.NullMj)
            {
                bool result = false;

                if (value == laizi) result = true;

                if (laizi >= 96 && value >= 96)
                    result = true;

                ret.transform.Find("Laizi").gameObject.SetActive(result);
            }

            return ret;
        }

        public static Vector2 MjSizeUp
        {
            get
            {
                if (Instance != null)
                    return Instance._mjSizeUp;
                else
                    return Vector2.zero;
            }
        }

        public static Vector2 MjSizeMe
        {
            get
            {
                if (Instance != null)
                    return Instance._mjSizeMe;
                else
                    return Vector2.zero;
            }
        }

        void OnDestroy()
        {
            Instance = null;
        }

        public virtual UiCardGroup GetGroup(int[] value, EnD2MjType type, bool isHaveBg, int laizi, int laizi1)
        { return null; }
        public virtual UiCardGroup GetGroup(int value, EnD2MjType type, bool isHaveBg, int laizi, int laizi1)
        { return null; }
        public virtual GameObject GetMj(int value, EnD2MjType type, int laizi, int laizi1) { return null; }
    }
}