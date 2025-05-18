using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public enum UIMahjongType
    {
        Small,
        Big,
    }

    public class AssetsComponent : BaseComponent
    {
        /// <summary>
        /// 引用注入资源
        /// </summary>
        public TypeBinderManager TypeBinder { get; private set; }
        /// <summary>
        /// 麻将模板
        /// </summary>
        public MahjongContainer MahjongTemplate { get; private set; }
        /// <summary>
        /// 麻将材质管理资源
        /// </summary>
        public MahjongMiscAssets MaterialAssets { get; private set; }
        /// <summary>
        /// 麻将贴图管理
        /// </summary>
        public MahjongCardAssets MahjongCardAssets { get; private set; }

        // 麻将牌材质
        public Material MahjongNormal { get { return MaterialAssets.GetAsset<Material>("MjMateNormal"); } }
        public Material MahjongGolden { get { return MaterialAssets.GetAsset<Material>("MjMateGolden"); } }
        public Material MahjongBlue { get { return MaterialAssets.GetAsset<Material>("MjMateBlue"); } }
        public Material MahjongGray { get { return MaterialAssets.GetAsset<Material>("MjMateGray"); } }

        public override void OnLoad()
        {
            // 麻将配置信息
            var gameConfig = GameUtils.GetInstanceAssets<GameObject>("GameConfig");
            if (gameConfig != null)
            {
                gameConfig.name = "GameConfig";
                TypeBinder = gameConfig.GetComponent<TypeBinderManager>();
            }
            // 麻將模板
            var templeteObj = GameUtils.GetInstanceAssets<GameObject>("MahjongContainer");
            if (templeteObj != null)
            {
                MahjongTemplate = templeteObj.GetComponent<MahjongContainer>();
                MahjongTemplate.name = "MahjongContainer";
            }
            // 材质资源
            MaterialAssets = GameUtils.GetInstanceAssets<MahjongMiscAssets>("MaterialAssets");
            // 麻将牌信息
            MahjongCardAssets = GameUtils.GetInstanceAssets<MahjongCardAssets>("MahjongCardAssets");
            // 收集麻将牌
            var sprites = MahjongCardAssets.MahjongSprites;
            for (int i = 0; i < sprites.Length; i++)
            {
                int value = int.Parse(sprites[i].name);
                mMahjongSprites.Add(value, sprites[i]);
            }
            sprites = MahjongCardAssets.MahjongSmallSprites;
            for (int i = 0; i < sprites.Length; i++)
            {
                string name = sprites[i].name;
                name = name.Trim("tile_s_".ToCharArray());
                mMahjongSmallSprites.Add(int.Parse(name), sprites[i]);
            }
            RectTransform bigRect = MahjongCardAssets.UIMahjong[(int)UIMahjongType.Big].transform as RectTransform;
            RectTransform smallRect = MahjongCardAssets.UIMahjong[(int)UIMahjongType.Small].transform as RectTransform;
            mSizeBig = new Vector2(bigRect.rect.width, bigRect.rect.height);
            mSizeSmall = new Vector2(smallRect.rect.width, smallRect.rect.height);

            // 设置麻将牌材质
            SwitchMahjongSkin();
        }

        /// <summary>
        /// 换麻将皮肤
        /// </summary>
        public void SwitchMahjongSkin()
        {
            var assetsName = "BlankCardSkin_" + MahjongUtility.MahjongCardColor;
            var texture = GameUtils.GetAssets<Texture>(assetsName);
            if (texture != null)
            {
                if (MahjongGray != null) MahjongGray.mainTexture = texture;
                if (MahjongBlue != null) MahjongBlue.mainTexture = texture;
                if (MahjongGolden != null) MahjongGolden.mainTexture = texture;
                if (MahjongNormal != null) MahjongNormal.mainTexture = texture;
            }
        }

        /// <summary>
        /// 获取麻将牌贴图
        /// </summary>
        public Sprite GetMahjongSprite(int card)
        {
            if (mMahjongSprites.ContainsKey(card)) return mMahjongSprites[card];
            return null;
        }

        /// <summary>
        /// 获取麻将牌贴图
        /// </summary>
        public Sprite GetMahjongSmallSprite(int card)
        {
            if (mMahjongSmallSprites.ContainsKey(card)) return mMahjongSmallSprites[card];
            return null;
        }

        #region 2D麻将牌
        private Dictionary<int, Sprite> mMahjongSprites = new Dictionary<int, Sprite>();
        private Dictionary<int, Sprite> mMahjongSmallSprites = new Dictionary<int, Sprite>();
        private Vector2 mSizeSmall;
        private Vector2 mSizeBig;

        public UiCardGroup GetUIMahjongGroupBig(IList<int> cards, bool isHaveBg = false)
        {
            return GetUIMahjongGroup(cards, UIMahjongType.Big, isHaveBg: isHaveBg);
        }

        public UiCardGroup GetUIMahjongGroupSmall(IList<int> cards, bool isHaveBg = false)
        {
            return GetUIMahjongGroup(cards, UIMahjongType.Small, isHaveBg: isHaveBg);
        }

        public UiCardGroup GetUIMahjongGroup(IList<int> cards, UIMahjongType uiType, EnGroupType cpgType = EnGroupType.None, bool isHaveBg = false)
        {
            Vector2 mjSize = Vector2.zero;
            GameObject[] list = new GameObject[cards.Count];
            UiCardGroup cardGroup;
            switch (uiType)
            {
                case UIMahjongType.Big:
                    mjSize = mSizeBig;
                    break;
                case UIMahjongType.Small:
                    mjSize = mSizeSmall;
                    break;
            }
            if (cpgType == EnGroupType.XFDan || cpgType == EnGroupType.YaoDan || cpgType == EnGroupType.JiuDan || cpgType == EnGroupType.ZFBDan)
            {
                List<int> cardsList = new List<int>();
                Dictionary<int, int> dic = new Dictionary<int, int>();
                for (int i = 0; i < cards.Count; i++)
                {
                    if (!dic.ContainsKey(cards[i]))
                    {
                        dic[cards[i]] = 1;
                        cardsList.Add(cards[i]);
                    }
                    else
                    {
                        dic[cards[i]]++;
                    }
                }
                GameObject[] xjfdList = new GameObject[cardsList.Count];
                for (int i = 0; i < cardsList.Count; i++)
                {
                    if (dic.ContainsKey(cardsList[i]))
                    {
                        xjfdList[i] = GetUIMahjong(cardsList[i], uiType);
                        if (dic[cardsList[i]] > 1)
                        {
                            GameObject temp = GameUtils.GetAssets<GameObject>("NumSign" + dic[cardsList[i]]);
                            temp.SetActive(false);
                            var obj = Instantiate(temp);
                            if (obj != null)
                            {
                                obj.transform.parent = xjfdList[i].transform;
                                obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
                                obj.gameObject.layer = xjfdList[i].gameObject.layer;
                                switch (uiType)
                                {
                                    case UIMahjongType.Big:
                                        obj.transform.localPosition = new Vector3(-20, -37, 0);
                                        obj.transform.localScale = Vector3.one * 0.7f;
                                        break;
                                    case UIMahjongType.Small:
                                        obj.transform.localPosition = new Vector3(-7, -3f, 0);
                                        obj.transform.localScale = Vector3.one * 0.4f;
                                        break;
                                }
                                obj.SetActive(true);
                            }
                        }
                    }
                }
                cardGroup = UiCardGroup.Create(xjfdList, mjSize, isHaveBg ? MahjongCardAssets.TileBackground : null, cpgType);
            }
            else
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    //暗杠时显示背面, 第四张要反过来 
                    if (cpgType == EnGroupType.AnGang && i < 3)
                    {
                        list[i] = GetUIMahjongBg(uiType);
                    }
                    else
                    {
                        list[i] = GetUIMahjong(cards[i], uiType);
                    }
                }
                cardGroup = UiCardGroup.Create(list, mjSize, isHaveBg ? MahjongCardAssets.TileBackground : null, cpgType);
            }
            return cardGroup;
        }

        public GameObject GetUIMahjongBg(UIMahjongType type)
        {
            return Instantiate(MahjongCardAssets.UIMahjongBg[(int)type]).gameObject;
        }

        public GameObject GetUIMahjong(int card, UIMahjongType uiType)
        {
            var obj = Instantiate(MahjongCardAssets.UIMahjong[(int)uiType]);
            switch (uiType)
            {
                case UIMahjongType.Big:
                    obj.CardValue.sprite = GetMahjongSprite(card);
                    obj.CardValue.SetNativeSize();
                    break;
                case UIMahjongType.Small:
                    obj.CardValue.sprite = GetMahjongSmallSprite(card);
                    obj.CardValue.SetNativeSize();
                    break;
            }
            var flag = GameCenter.DataCenter.IsLaizi(card);
            if (flag) obj.SetLaizi();
            return obj.gameObject;
        }
        #endregion

        #region 麻将标记贴图
        private Dictionary<string, Sprite> mSprites = new Dictionary<string, Sprite>();

        public Sprite GetSprite(string name)
        {
            Sprite sprite;
            if (!mSprites.TryGetValue(name, out sprite))
            {
                var obj = GameUtils.GetAssets<GameObject>(name);
                if (obj != null)
                {
                    var comp = obj.GetComponent<SpriteRenderer>();
                    if (comp != null)
                    {
                        sprite = comp.sprite;
                        mSprites.Add(name, sprite);
                    }
                }
            }
            return sprite;
        }
        #endregion 

    }
}