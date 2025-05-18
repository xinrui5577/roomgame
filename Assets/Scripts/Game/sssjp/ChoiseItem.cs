using System.Collections.Generic;
using Assets.Scripts.Game.sssjp.Tool;
using UnityEngine;

namespace Assets.Scripts.Game.sssjp
{
    public class ChoiseItem : MonoBehaviour
    {

        public List<int> CardsList = new List<int>();

        public GameObject SpecialObj;

        public GameObject NormalObj;

        public GameObject SelectMark;

        public UISprite SpecialSprite;

        public UILabel[] TypeLabels;

        public HelpLz.SssDun[] Duns;

        public int Special;


        public int GetItemIndex(int val)
        {
            if (CardsList.Contains(val))
            { return CardsList.IndexOf(val); }
            else
            { return -1; }
        }

        public void Reset()
        {
            SpecialObj.SetActive(false);
            NormalObj.SetActive(false);
            CardsList.Clear();
            Special = 9;
        }

        /// <summary>
        /// 设置Item显示内容
        /// </summary>
        /// <param name="duns"></param>
        public void SetChoiseItem(HelpLz.SssDun[] duns)
        {
            Duns = duns;
            for (int i = 0; i < TypeLabels.Length; i++)
            {

                SetTypeLabel(TypeLabels[i], duns[i].CardType, i);
            }

            for (int i = 0; i < duns.Length; i++)
            {
                HelpLz.SssDun dun = duns[i];
                if (i == 0)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        CardsList.Add(dun.Cards[j]);
                    }
                }
                else
                {
                    for (int j = 0; j < 5; j++)
                    {
                        CardsList.Add(dun.Cards[j]);
                    }
                }
            }
        }

        public void SetTypeLabel(UILabel label, CardType type, int line)
        {
            bool isSpecial = false;
            switch (type)
            {
                case CardType.sanpai:
                    label.text = "乌龙";
                    break;

                case CardType.yidui:
                    label.text = "一对";
                    break;

                case CardType.liangdui:
                    label.text = "两对";
                    break;

                case CardType.santiao:
                    if (line == 0)
                    {
                        label.text = "冲三";
                        isSpecial = true;
                    }
                    else
                    {
                        label.text = "三条";
                    }
                    break;

                case CardType.shunzi:
                    label.text = "顺子";
                    break;

                case CardType.tonghua:
                    label.text = "同花";
                    break;

                case CardType.hulu:
                    if (line == 1)
                    {
                        label.text = "中墩葫芦";
                        isSpecial = true;
                    }
                    else
                    {
                        label.text = "葫芦";
                    }
                    break;

                case CardType.tiezhi:
                    isSpecial = true;
                    label.text = "铁支";
                    break;

                case CardType.tonghuashun:
                    isSpecial = true;
                    label.text = "同花顺";
                    break;
            }

            if (isSpecial)
            {
                label.fontSize = 34;
                label.applyGradient = true;
                label.effectColor = Tools.ChangeToColor(0x430000);
            }
            else
            {
                label.fontSize = 30;
                label.applyGradient = false;
                label.effectColor = Tools.ChangeToColor(0x795121);
            }
        }
    }
}
