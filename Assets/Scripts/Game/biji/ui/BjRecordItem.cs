using System.Collections.Generic;
using Assets.Scripts.Game.biji.network;
using com.yxixia.utile.Utiles;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjRecordItem : MonoBehaviour
    {
        public UILabel UserName;
        public UIGrid TotalSocreGrid;
        public UISprite ScoreItem;

        public List<BjCardItem> LoadFirstCards;
        public List<BjCardItem> LoadMiddleCards;
        public List<BjCardItem> LoadEndCards;

        public void SetView(string userName, int userGold, CompareData compareData)
        {
            if (compareData.DaosCards.Count == 0)
            {
                for (int i = 0; i < LoadFirstCards.Count; i++)
                {
                    LoadFirstCards[i].ShowCardBack();
                    LoadMiddleCards[i].ShowCardBack();
                    LoadEndCards[i].ShowCardBack();
                }
                return;
            }
            UserName.text = userName;
            SetScore(ScoreItem, userGold, TotalSocreGrid, true);
            for (int i = 0; i < compareData.DaosCards.Count; i++)
            {
                var cards = new List<int>(compareData.DaosCards[i]); ;
                var realCards = compareData.DaoRealCards[i];
                switch (i)
                {
                    case 0:
                        YxDebug.LogArray(compareData.DaosCards[i]);
                        YxDebug.LogArray(compareData.DaoRealCards[i]);

                        for (int j = 0; j < compareData.DaosCards[i].Length; j++)
                        {
                            if (cards.Contains(realCards[j]))
                            {
                                cards.Remove(realCards[j]);
                                LoadFirstCards[j].Value = realCards[j];
                                LoadFirstCards[j].KingName(0);
                            }
                            else
                            {
                                LoadFirstCards[j].Value = realCards[j];
                                var king = 0;

                                for (int k = 0; k < cards.Count; k++)
                                {
                                    if (cards[k] == 81 || cards[k] == 97)
                                    {
                                        king = cards[k];
                                        cards.Remove(cards[k]);
                                        break;
                                    }
                                }
                                LoadFirstCards[j].KingName(king);
                            }
                        }

                        break;
                    case 1:
                        for (int j = 0; j < compareData.DaosCards[i].Length; j++)
                        {
                            if (cards.Contains(realCards[j]))
                            {
                                cards.Remove(realCards[j]);
                                LoadMiddleCards[j].Value = realCards[j];
                                LoadMiddleCards[j].KingName(0);
                            }
                            else
                            {
                                LoadMiddleCards[j].Value = realCards[j];
                                var king = 0;

                                for (int k = 0; k < cards.Count; k++)
                                {
                                    if (cards[k] == 81 || cards[k] == 97)
                                    {
                                        king = cards[k];
                                        cards.Remove(cards[k]);
                                        break;
                                    }
                                }
                                LoadMiddleCards[j].KingName(king);
                            }
                        }
                        break;
                    case 2:
                        for (int j = 0; j < compareData.DaosCards[i].Length; j++)
                        {
                            if (cards.Contains(realCards[j]))
                            {
                                cards.Remove(realCards[j]);
                                LoadEndCards[j].Value = realCards[j];
                                LoadEndCards[j].KingName(0);
                            }
                            else
                            {
                                LoadEndCards[j].Value = realCards[j];
                                var king = 0;

                                for (int k = 0; k < cards.Count; k++)
                                {
                                    if (cards[k] == 81 || cards[k] == 97)
                                    {
                                        king = cards[k];
                                        cards.Remove(cards[k]);
                                        break;
                                    }
                                }
                                LoadEndCards[j].KingName(king);
                            }
                        }

                        break;
                }
            }
        }

        public void SetScore(UISprite numItem, int score, UIGrid grid, bool needAdd = false)
        {
            while (grid.transform.childCount > 0)
            {
                DestroyImmediate(grid.transform.GetChild(0).gameObject);
            }
            var scoreStr = score.ToString();
            if (score >= 0)
            {
                if (needAdd)
                {
                    var obj = YxWindowUtils.CreateItem(numItem, grid.transform);
                    obj.spriteName = "win";
                    obj.MakePixelPerfect();
                }

                for (int i = 0; i < scoreStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(numItem, grid.transform);
                    item.spriteName = "win" + scoreStr[i];
                    item.MakePixelPerfect();
                }
            }
            else
            {
                if (needAdd)
                {
                    var obj = YxWindowUtils.CreateItem(numItem, grid.transform);
                    obj.spriteName = "lose";
                    obj.MakePixelPerfect();
                }

                for (int i = 1; i < scoreStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(numItem, grid.transform);
                    item.spriteName = "lose" + scoreStr[i];
                    item.MakePixelPerfect();
                }
            }

            grid.repositionNow = true;
        }
    }
}
