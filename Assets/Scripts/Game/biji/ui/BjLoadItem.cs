using System.Collections.Generic;
using Assets.Scripts.Game.biji.Modle;
using UnityEngine;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjLoadItem : MonoBehaviour
    {
        public UIGrid Grid;
        public int Depth;
        public GameObject DeleteBtn;

        public CardType CardType;
        public List<BjCardItem> Cards = new List<BjCardItem>();
        public List<int> CardsValue;
        public List<int> CardsPoekerValue;

        public void AddCards(List<BjCardItem> cardItems, CardType cardType)
        {
            if (cardItems.Count == 0) return;
            Cards.AddRange(cardItems);

            if (cardType == CardType.ShunZi || cardType == CardType.TongHuaShun)
            {
                var dicVal = new List<int>();
                for (int i = 0; i < Cards.Count; i++)
                {
                    dicVal.Add(GameHelp.GetValue(Cards[i].Value));
                }

                for (int j = 0; j < Cards.Count; j++)
                {
                    if (dicVal[j] == 0xe)
                    {
                        if (dicVal.Contains(2) || dicVal.Contains(3))
                        {
                            Cards[j].PoKerCard.Id = GameHelp.GetColor(Cards[j].PoKerCard.Id) + 1;
                        }
                    }
                }
            }

            Cards.Sort((x, y) =>
            {
                if (GameHelp.IsLaizi(x.PoKerCard.Id) == GameHelp.IsLaizi(y.PoKerCard.Id))
                {
                    int valX = GameHelp.GetValue(x.PoKerCard.Id);
                    int valY = GameHelp.GetValue(y.PoKerCard.Id);

                    if (valX == valY)
                    {
                        return GameHelp.GetColorVal(y.PoKerCard.Id) - GameHelp.GetColorVal(x.PoKerCard.Id);
                    }
                    else
                    {
                        return valY - valX;
                    }
                }
                else
                {
                    return GameHelp.IsLaizi(y.PoKerCard.Id) - GameHelp.IsLaizi(x.PoKerCard.Id);
                }

            });


            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.parent = Grid.transform;
                Cards[i].MoveFlag = false;
                Cards[i].IsTop = true;
                Cards[i].name = (i + 1).ToString();
                Cards[i].SetCardDepth(i + Depth);
                if (!CardsValue.Contains(Cards[i].Value))
                {
                    CardsValue.Add(Cards[i].Value);
                    CardsPoekerValue.Add(Cards[i].PoKerCard.Id);
                }
            }


            CardType = cardType;
            Grid.repositionNow = true;
            DeleteBtn.SetActive(true);
        }

        public void OnDeleteBtn(UIGrid grid, List<int> cardsValue, List<BjCardItem> settleCardsList, BjCardItem cardItem=null)
        {
            if (cardItem != null)
            {
                for (int i = 0; i < Cards.Count; i++)
                {
                    if (Cards[i].King.spriteName.Equals("0x52") && Cards[i].King.gameObject.activeSelf)
                    {
                        Cards[i].King.gameObject.SetActive(false);
                        Cards[i].Value = 81;
                    }
                    if (Cards[i].King.spriteName.Equals("0x62") && Cards[i].King.gameObject.activeSelf)
                    {
                        Cards[i].King.gameObject.SetActive(false);
                        Cards[i].Value = 97;
                    }
                }
                cardItem.transform.parent = grid.transform;
                cardItem.MoveFlag = false;
                cardItem.IsTop = false;
                if (cardItem.King.spriteName.Equals("0x52") && cardItem.King.gameObject.activeSelf)
                {
                    cardItem.King.gameObject.SetActive(false);
                    cardItem.Value = 81;
                }
                if (cardItem.King.spriteName.Equals("0x62") && cardItem.King.gameObject.activeSelf)
                {
                    cardItem.King.gameObject.SetActive(false);
                    cardItem.Value = 97;
                }
             
                cardsValue.Add(cardItem.Value);
                Cards.Remove(cardItem);
                settleCardsList.Add(cardItem);
                CardsValue.Remove(cardItem.Value);
                CardsPoekerValue.Remove(cardItem.PoKerCard.Id);
                Grid.repositionNow = true;
            }
            else
            {
                for (int i = 0; i < Cards.Count; i++)
                {
                    Cards[i].transform.parent = grid.transform;
                    Cards[i].MoveFlag = false;
                    Cards[i].IsTop = false;
                    if (Cards[i].King.spriteName.Equals("0x52") && Cards[i].King.gameObject.activeSelf)
                    {
                        Cards[i].King.gameObject.SetActive(false);
                        Cards[i].Value = 81;
                    }
                    if (Cards[i].King.spriteName.Equals("0x62") && Cards[i].King.gameObject.activeSelf)
                    {
                        Cards[i].King.gameObject.SetActive(false);
                        Cards[i].Value= 97;
                    }
                    cardsValue.Add(Cards[i].Value);
                }
                settleCardsList.AddRange(Cards);
                CardsPoekerValue.Clear();
                Cards.Clear();
                CardsValue.Clear();
            }
            grid.repositionNow = true;
            DeleteBtn.SetActive(CardsValue.Count != 0);
            CardType = CardType.None;
        }

        public void ChangeParent(BjLoadItem bjLoadItem)
        {
            if (Cards.Count == 0) return;
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.parent = bjLoadItem.Grid.transform;

            }
            bjLoadItem.Cards.AddRange(Cards);
            Cards.Clear();

            DeleteBtn.SetActive(false);
            bjLoadItem.CardsValue.Clear();
            bjLoadItem.CardsPoekerValue.Clear();
            bjLoadItem.CardsPoekerValue.AddRange(CardsPoekerValue);
            bjLoadItem.CardsValue.AddRange(CardsValue);
            CardsValue.Clear();
            CardsPoekerValue.Clear();

            bjLoadItem.CardType = CardType;
            CardType = CardType.None;

            bjLoadItem.DeleteBtn.SetActive(true);
            bjLoadItem.Grid.repositionNow = true;
        }

        public void ExchangeData(BjLoadItem bjLoadItem)
        {
            var otherCards = new List<int>(bjLoadItem.CardsValue);
            var otherCardsPoker = new List<int>(bjLoadItem.CardsPoekerValue);
            var otherType = bjLoadItem.CardType;

            var selfCards = new List<int>(CardsValue);
            var selfCardsPorker = new List<int>(CardsPoekerValue);
            var selfType = CardType;

            bjLoadItem.CardsValue.Clear();
            bjLoadItem.CardsPoekerValue.Clear();
            bjLoadItem.CardsValue.AddRange(selfCards);
            bjLoadItem.CardsPoekerValue.AddRange(selfCardsPorker);

            CardsValue.Clear();
            CardsPoekerValue.Clear();

            CardsValue.AddRange(otherCards);
            CardsPoekerValue.AddRange(otherCardsPoker);

            bjLoadItem.CardType = selfType;
            CardType = otherType;

            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].Value = otherCards[i];
                bjLoadItem.Cards[i].Value = selfCards[i];
            }
        }

        public bool CompareCard(BjLoadItem bjLoadItem)
        {
            if (CardType < bjLoadItem.CardType)
            {
                ExchangeData(bjLoadItem);
                return true;
            }

            if (CardType == bjLoadItem.CardType)
            {
                if (CardType == CardType.DuiZi)
                {
                    if (GameHelp.GetValue(Cards[1].Value) < GameHelp.GetValue(bjLoadItem.Cards[1].Value))
                    {
                        ExchangeData(bjLoadItem);
                        return true;
                    }

                    if (GameHelp.GetValue(Cards[1].Value) == GameHelp.GetValue(bjLoadItem.Cards[1].Value))
                    {
                        if (GameHelp.GetColorVal(Cards[1].Value) < GameHelp.GetColorVal(bjLoadItem.Cards[1].Value))
                        {
                            ExchangeData(bjLoadItem);
                            return true;
                        }
                    }
                }
                else
                {
                    GameHelp gameHelp = new GameHelp();

                    if (gameHelp.Compare(CardType, CardsPoekerValue, bjLoadItem.CardsPoekerValue))
                    {
                        ExchangeData(bjLoadItem);
                        return true;
                    }
                }
            }
            return false;
        }
        public List<int> RealCards()
        {
            var realcards = new List<int>();
            for (int i = 0; i < Cards.Count; i++)
            {
                realcards.Add(Cards[i].PoKerCard.Id);
            }
            return realcards;
        }


        public void Reset()
        {
            DeleteBtn.SetActive(false);
        }
    }
}
