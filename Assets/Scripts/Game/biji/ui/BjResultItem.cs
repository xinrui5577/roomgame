using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.biji.Modle;
using Assets.Scripts.Game.biji.network;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common.DataBundles;
using YxFramwork.Common.Model;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjResultItem : MonoBehaviour
    {
        public NguiTextureAdapter UserHead;
        public UILabel UserName;
        public UILabel UserId;
        public List<BjCardItem> LoadFirst = new List<BjCardItem>();
        public UIGrid LoadFirstScoreGrid;
        public List<BjCardItem> LoadMiddle = new List<BjCardItem>();
        public UIGrid LoadMiddleScoreGrid;
        public List<BjCardItem> LoadEnd = new List<BjCardItem>();
        public UIGrid LoadEndScoreGrid;
        public UIGrid XiGoldGrid;
        public UIGrid TotalScoreGrid;
        public UISprite ScoreItem;
        public UIGrid XiPaiTypeGrid;
        public UISprite XiPaiTypeItem;

        public void SetResultItemView(YxBaseUserInfo userInfo, CompareData reusltData, BjGameTable gameTable)
        {
            PortraitDb.SetPortrait(userInfo.AvatarX, UserHead, userInfo.SexI);
            UserName.text = userInfo.NickM;
            UserId.text = userInfo.UserId;

            for (int i = 0; i < reusltData.DaosCards.Count; i++)
            {
                var daoCards = new List<int>(reusltData.DaosCards[i]);
                switch (i)
                {
                    case 0:
                        for (int j = 0; j < reusltData.DaosCards[i].Length; j++)
                        {
                            if (daoCards.Contains(reusltData.DaoRealCards[i][j]))
                            {
                                daoCards.Remove(reusltData.DaoRealCards[i][j]);
                                LoadFirst[j].Value = reusltData.DaoRealCards[i][j];
                            }
                            else
                            {
                                LoadFirst[j].Value = reusltData.DaoRealCards[i][j];
                                var king = 0;
                                for (int k = 0; k < daoCards.Count; k++)
                                {
                                    if (daoCards[k] == 81 || daoCards[k] == 97)
                                    {
                                        king = daoCards[k];
                                        daoCards.Remove(daoCards[k]);
                                        break;
                                    }
                                }
                                LoadFirst[j].KingName(king);
                            }
                        }
                        break;
                    case 1:
                        for (int j = 0; j < reusltData.DaosCards[i].Length; j++)
                        {
                            if (daoCards.Contains(reusltData.DaoRealCards[i][j]))
                            {
                                daoCards.Remove(reusltData.DaoRealCards[i][j]);
                                LoadMiddle[j].Value = reusltData.DaoRealCards[i][j];
                            }
                            else
                            {
                                LoadMiddle[j].Value = reusltData.DaoRealCards[i][j];
                                var king = 0;
                                for (int k = 0; k < daoCards.Count; k++)
                                {
                                    if (daoCards[k] == 81 || daoCards[k] == 97)
                                    {
                                        king = daoCards[k];
                                        daoCards.Remove(daoCards[k]);
                                        break;
                                    }
                                }
                                LoadMiddle[j].KingName(king);
                            }
                        }
                        break;
                    case 2:
                        for (int j = 0; j < reusltData.DaosCards[i].Length; j++)
                        {

                            if (daoCards.Contains(reusltData.DaoRealCards[i][j]))
                            {
                                daoCards.Remove(reusltData.DaoRealCards[i][j]);
                                LoadEnd[j].Value = reusltData.DaoRealCards[i][j];
                            }
                            else
                            {
                                LoadEnd[j].Value = reusltData.DaoRealCards[i][j];
                                var king = 0;
                                for (int k = 0; k < daoCards.Count; k++)
                                {
                                    if (daoCards[k] == 81 || daoCards[k] == 97)
                                    {
                                        king = daoCards[k];
                                        daoCards.Remove(daoCards[k]);
                                        break;
                                    }
                                }
                                LoadEnd[j].KingName(king);
                            }
                        }
                        break;
                }
            }

            int totalGold = 0;

            for (int i = 0; i < reusltData.DaosScore.Count; i++)
            {
                totalGold += reusltData.DaosScore[i];
                switch (i)
                {
                    case 0:
                        SetDaoScore(reusltData.DaosScore[i], LoadFirstScoreGrid);
                        break;
                    case 1:
                        SetDaoScore(reusltData.DaosScore[i], LoadMiddleScoreGrid);
                        break;
                    case 2:
                        SetDaoScore(reusltData.DaosScore[i], LoadEndScoreGrid);
                        break;
                }
            }

            SetDaoScore(reusltData.XiPaiScore, XiGoldGrid);
            totalGold += reusltData.XiPaiScore;
            userInfo.CoinA += totalGold;
            gameTable.GetPlayer<BjPlayer>(userInfo.Seat, true).ShowPlayerGold(userInfo.CoinA);

            SetDaoScore(totalGold, TotalScoreGrid);

            for (int i = 0; i < reusltData.XiPaiInfos.Length; i++)
            {
                var item = YxWindowUtils.CreateItem(XiPaiTypeItem, XiPaiTypeGrid.transform);
                item.spriteName = GetXiPaiType(reusltData.XiPaiInfos[i]);
                item.MakePixelPerfect();
            }

            XiPaiTypeGrid.repositionNow = true;
        }

        public void SetDaoScore(int score, UIGrid grid)
        {
            var scoreStr = score.ToString();
            if (score >= 0)
            {
                var obj = YxWindowUtils.CreateItem(ScoreItem, grid.transform);
                obj.spriteName = "win";
                obj.MakePixelPerfect();

                for (int i = 0; i < scoreStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(ScoreItem, grid.transform);
                    item.spriteName = "win" + scoreStr[i];
                    item.MakePixelPerfect();
                }
            }
            else
            {
                var obj = YxWindowUtils.CreateItem(ScoreItem, grid.transform);
                obj.spriteName = "lose";
                obj.MakePixelPerfect();
                for (int i = 1; i < scoreStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(ScoreItem, grid.transform);
                    item.spriteName = "lose" + scoreStr[i];
                    item.MakePixelPerfect();
                }
            }

            grid.repositionNow = true;
        }

        private string GetXiPaiType(int xiPaiType)
        {
            var type = "";
            switch (xiPaiType)
            {
                case 0:
                    type = "sanqing";
                    break;
                case 1:
                    type = "quanhei";
                    break;
                case 2:
                    type = "quanhong";
                    break;
                case 3:
                    type = "shuangshunqing";
                    break;
                case 4:
                    type = "sanshunqing";
                    break;
                case 5:
                    type = "shuangsantiao";
                    break;
                case 6:
                    type = "quansantiao";
                    break;
                case 7:
                    type = "sigetou";
                    break;
                case 8:
                    type = "lianshun";
                    break;
                case 9:
                    type = "qinglianshun";
                    break;
                case 10:
                    type = "tonguan";
                    break;
            }

            return type;
        }

    }
}
