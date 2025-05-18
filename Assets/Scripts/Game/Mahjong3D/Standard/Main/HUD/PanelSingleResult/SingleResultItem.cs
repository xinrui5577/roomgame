using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class SingleResultItem : MonoBehaviour
    {
        public Text HuInfo;
        public Text UserName;
        public UiCardGroupCtrl CardGroup;
        public TextItem[] ItemArray;

        public Button Detail;

        [HideInInspector]
        public int Chair = -1;

        public string Name { set { UserName.text = value; } }

        public string HuInfoTxt { set { HuInfo.text = value; } }

        public TextItem GetItem(TextType type)
        {
            TextItem item = null;
            for (int i = 0; i < ItemArray.Length; i++)
            {
                item = ItemArray[i];
                if (type == item.Type)
                {
                    return item;
                }
            }
            return null;
        }

        public void SetItem(TextType type, float value)
        {
            var item = GetItem(type);
            if (null == item) return;

            var flag = value != 0;
            item.gameObject.SetActive(flag);

            if (flag)
            {
                item.Txt = value.ToString();
            }
        }

        public TextItem SetItem(TextType type, string value)
        {
            var item = GetItem(type);
            if (null != item)
            {
                item.Txt = value;
            }
            return item;
        }

        public int HuTypeTxt
        {
            set
            {
                var item = GetItem(TextType.HuType);
                if (null != item)
                {
                    item.gameObject.SetActive(true);
                    if (value == 1) { item.Txt = "胡"; }
                    else if (value == 2) { item.Txt = "自摸"; }
                    else if (value == -1) { item.Txt = "点炮"; }
                    else
                    {
                        item.Txt = "";
                        item.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void SetCpgCard(List<CpgData> cpgList)
        {
            if (null == cpgList || cpgList.Count == 0) return;
            for (int i = 0; i < cpgList.Count; i++)
            {
                UiCardGroup uiGroup = GameCenter.Assets.GetUIMahjongGroup(cpgList[i].AllCards(), UIMahjongType.Small, cpgList[i].Type);
                CardGroup.AddUiCdGroup(uiGroup);
            }
        }

        public void SetCpgCard(List<CpgModel> cpgList)
        {
            if (null == cpgList || cpgList.Count == 0) return;
            for (int i = 0; i < cpgList.Count; i++)
            {
                EnGroupType cpgType = EnGroupType.None;
                if (cpgList[i].Type == CpgProtocol.AnGang)
                {
                    cpgType = EnGroupType.AnGang;
                }

                UiCardGroup uiGroup = GameCenter.Assets.GetUIMahjongGroup(cpgList[i].Cards, UIMahjongType.Small, cpgType);
                CardGroup.AddUiCdGroup(uiGroup);
            }
        }

        public UiCardGroup SetCards(IList<int> array)
        {
            if (null == array || array.Count == 0) return null;
            UiCardGroup uiGroupCard = GameCenter.Assets.GetUIMahjongGroupSmall(array);
            CardGroup.AddUiCdGroup(uiGroupCard);
            return uiGroupCard;
        }

        public UiCardGroup SetCard(int value)
        {
            UiCardGroup uiGroupCard = GameCenter.Assets.GetUIMahjongGroupSmall(new int[] { value });
            CardGroup.AddUiCdGroup(uiGroupCard);
            return uiGroupCard;
        }

        public void SortCardGroup()
        {
            CardGroup.Sort(0);
        }

        public bool Banker
        {
            set
            {
                var sign = GetComponent<BankerSign>();
                if (sign != null)
                {
                    sign.Set(value);
                }
            }
        }

        public void SetDetail(bool isOn)
        {
            if (Detail != null) Detail.gameObject.SetActive(isOn);
        }

        public void OnReset()
        {
            Chair = -1;
            CardGroup.Clear();
            gameObject.SetActive(false);
            Banker = false;
        }
    }
}