using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using YxFramwork.Common;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.hg
{
    public class HgTrendView : YxView
    {
        public EventObject EventObj;

        public UIGrid SpotGrid;
        public UISprite SpotItem;

        public UIGrid CardTypeGrid;
        public UISprite CardTypeItem;
        public string CardTypeNormalPrefix;
        public string CardTypeSpecialPrefix;

        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "SetRecord":
                    SetRecord((bool)data.Data);
                    break;
            }
        }

        public void SetRecord(bool isSmooth = false)
        {
            var gdata = App.GetGameData<HgGameData>();
            var recordSpot = gdata.RecordSpot;
            var recordCardType = gdata.RecordCardType;

            SetSpotView(recordSpot, isSmooth);
            SetCardTypeView(recordCardType, isSmooth);
        }

        private void SetSpotView(List<List<string>> recordSpot, bool isSmooth)
        {
            var spotCout = recordSpot.Count;

            if (spotCout == 0 || SpotGrid == null) return;

            var spotEnough = false;

            if (SpotGrid.transform.childCount == 20)
            {
                spotEnough = true;
            }
            else
            {
                while (SpotGrid.transform.childCount > 0)
                {
                    DestroyImmediate(SpotGrid.transform.GetChild(0).gameObject);
                }
            }

            var index = 0;

            if (spotCout > 20)
            {
                spotCout -= 20;
                for (int i = spotCout; i < recordSpot.Count; i++)
                {
                    for (int j = 0; j < recordSpot[i].Count; j++)
                    {
                        UISprite item;
                        if (spotEnough)
                        {
                            item = SpotGrid.transform.GetChild(index).GetComponent<UISprite>();
                            index++;
                        }
                        else
                        {
                            item = YxWindowUtils.CreateItem(SpotItem, SpotGrid.transform);

                        }
                        item.spriteName = string.Format("spot{0}", recordSpot[i][j]);
                        item.name = i.ToString();
                    }
                   
                }
            }
            else
            {

                for (int i = 0; i < spotCout; i++)
                {
                    for (int j = 0; j < recordSpot[i].Count; j++)
                    {
                        var item = YxWindowUtils.CreateItem(SpotItem, SpotGrid.transform);
                        item.spriteName = string.Format("spot{0}", recordSpot[i][j]);
                        item.name = i.ToString();
                    }
                }
            }


            if (isSmooth)
            {
                SpotGrid.animateSmoothly = true;
            }

            SpotGrid.repositionNow = true;
        }

        private void SetCardTypeView(List<int> recordCardType, bool isSmooth)
        {
            var cardTypeCout = recordCardType.Count;
            if (cardTypeCout == 0 || CardTypeGrid == null) return;
            var cardtypeEnoufh = false;

            if (CardTypeGrid.transform.childCount == 7)
            {
                cardtypeEnoufh = true;
            }
            else
            {
                while (CardTypeGrid.transform.childCount > 0)
                {
                    DestroyImmediate(CardTypeGrid.transform.GetChild(0).gameObject);
                }
            }

            var index = 0;

            if (cardTypeCout > 7)
            {

                cardTypeCout -= 7;
                for (int i = cardTypeCout; i < recordCardType.Count; i++)
                {
                    if (cardtypeEnoufh)
                    {
                        var item = CardTypeGrid.transform.GetChild(index).GetComponent<UISprite>();
                        if (item.GetComponentInChildren<UILabel>())
                        {
                            item.spriteName = recordCardType[i] == 0 ? CardTypeNormalPrefix : CardTypeSpecialPrefix;
                            item.GetComponentInChildren<UILabel>().text = WinCardName(recordCardType[i]);
                        }
                        else
                        {
                            item.spriteName = WinCardType(recordCardType[i]);
                        }
                        item.name = i.ToString();
                        index++;
                    }
                    else
                    {
                        var item = YxWindowUtils.CreateItem(CardTypeItem, CardTypeGrid.transform);
                        if (item.GetComponentInChildren<UILabel>())
                        {
                            item.spriteName = recordCardType[i] == 0 ? CardTypeNormalPrefix : CardTypeSpecialPrefix;
                            item.GetComponentInChildren<UILabel>().text = WinCardName(recordCardType[i]);
                        }
                        else
                        {
                            item.spriteName = WinCardType(recordCardType[i]);
                        }
                        item.name = i.ToString();
                    }
                }
            }
            else
            {
                for (int i = 0; i < cardTypeCout; i++)
                {
                    var item = YxWindowUtils.CreateItem(CardTypeItem, CardTypeGrid.transform);
                    if (item.GetComponentInChildren<UILabel>())
                    {
                        item.spriteName = recordCardType[i] == 0 ? CardTypeNormalPrefix : CardTypeSpecialPrefix;
                        item.GetComponentInChildren<UILabel>().text = WinCardName(recordCardType[i]);
                    }
                    else
                    {
                        item.spriteName = WinCardType(recordCardType[i]);
                    }

                    item.name = i.ToString();
                }
            }

            if (isSmooth)
            {
                CardTypeGrid.animateSmoothly = true;
            }

            CardTypeGrid.repositionNow = true;
        }

        private string WinCardType(int type)
        {
            var cardType = "";
            switch (type)
            {
                case (int)CardType.DanPai:
                    cardType = "h_";
                    cardType += "danzhang";
                    break;
                case (int)CardType.DuiZi:
                    cardType = "l_";
                    cardType += "duizi";
                    break;
                case (int)CardType.ShunZi:
                    cardType = "l_";
                    cardType += "shunzi";
                    break;
                case (int)CardType.JinHua:
                    cardType = "l_";
                    cardType += "jinhua";
                    break;
                case (int)CardType.ShunJin:
                    cardType = "l_";
                    cardType += "shunjin";
                    break;
                case (int)CardType.BaoZi:
                    cardType = "l_";
                    cardType += "baozi";
                    break;
            }

            return cardType;
        }

        private string WinCardName(int type)
        {
            var cardType = "";
            switch (type)
            {
                case (int)CardType.DanPai:
                    cardType = "单张";
                    break;
                case (int)CardType.DuiZi:
                    cardType = "对子";
                    break;
                case (int)CardType.ShunZi:
                    cardType = "顺子";
                    break;
                case (int)CardType.JinHua:
                    cardType = "金花";
                    break;
                case (int)CardType.ShunJin:
                    cardType = "顺金";
                    break;
                case (int)CardType.BaoZi:
                    cardType = "豹子";
                    break;
            }

            return cardType;

        }

    }
}
