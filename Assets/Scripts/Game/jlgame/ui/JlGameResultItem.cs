using Assets.Scripts.Common.Adapters;
using Assets.Scripts.Game.jlgame.Modle;
using Assets.Scripts.Game.jlgame.network;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameResultItem : MonoBehaviour
    {
        public UILabel UserName;
        public NguiTextureAdapter UserHead;
        public UILabel WinScore;
        public UILabel LoseScore;
        public UILabel FoldCardNum;
        public UILabel FoldScore;
        public GameObject RoomOwner;
        public GameObject BigChase;
        public GameObject SmallChase;
        public UIGrid CardsGrid;
        public JlGameCardItem JlGameCardItem;

        private JlGameGameTable _gdata
        {
            get { return App.GetGameData<JlGameGameTable>(); }
        }
        public void SetData(ResultData resultData)
        {
            if (resultData.IsYouSelf)
            {
                UserName.color=Color.yellow;
            }

            UserName.text = resultData.Name;
            PortraitDb.SetPortrait(resultData.Head, UserHead, resultData.Sex);

            RoomOwner.SetActive(resultData.IsRoomOwner);
            _gdata.GetPlayer(resultData.Seat, true).Coin += resultData.Win;
            if (resultData.Win > 0)
            {
                WinScore.text = string.Format("+{0}", resultData.Win);
            }
            else
            {
                LoseScore.text = resultData.Win.ToString();
            }

            if (resultData.IsNoFold)
            {
                BigChase.SetActive(true);
                SmallChase.SetActive(true);
            }
        

            if (resultData.FoldCards.Length > 0)
            {
                FoldCardNum.gameObject.SetActive(true);
                FoldScore.gameObject.SetActive(true);
                FoldCardNum.text = string.Format("盖牌数：{0}", resultData.FoldCards.Length);
                FoldScore.text = string.Format("盖牌分：{0}", resultData.FoldScore);
                for (int i = 0; i < resultData.FoldCards.Length; i++)
                {
                    var cardItem = YxWindowUtils.CreateItem(JlGameCardItem, CardsGrid.transform);
                    cardItem.Value = resultData.FoldCards[i];
                    cardItem.SetCardDepth(i+4);
                }
            }

            CardsGrid.repositionNow = true;
        }
    }
}
