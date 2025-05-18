using System.Collections.Generic;
using Assets.Scripts.Game.sanpian.server;
using com.yxixia.utile.Utiles;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.DataBundles;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.sanpian.item
{
    public class ResultItem : MonoBehaviour
    {
        Color MyColor = new Color(1, 1, 0);
        Color OtherColor = new Color(1, 1, 1);
        public UILabel Name;
        public UILabel RedScore;
        public UILabel GreenScore;
        public UILabel SanPianRedScore;
        public UILabel SanPianGreenScore;
        public UIGrid CardsArea;
        public UITexture head;
        public UISprite TouYou;

        private List<GameObject> cardsGj=new List<GameObject>();
        public void SetInfo(ISFSObject user,int flagSeat,int index,int touyouIndex)
        {
            if (touyouIndex > -1)
            {
                TouYou.gameObject.SetActive(true);
                TouYou.spriteName = touyouIndex + 1 + "you";
            }
            else
            {
                TouYou.gameObject.SetActive(false);
            }
            int seat = user.GetInt("seat");
            if (seat == App.GetGameManager<SanPianGameManager>().RealPlayer.userInfo.Seat)
            {
                Name.color = MyColor;
            }
            else
            {
                Name.color = OtherColor;
            }
            Name.text = user.GetUtfString("name");
            int m_score = user.GetInt("win");
            if (m_score>=0)
            {
                GreenScore.gameObject.SetActive(true);
                GreenScore.text = "+" + YxUtiles.ReduceNumber(m_score);
            }
            else
            {
                RedScore.gameObject.SetActive(true);
                RedScore.text = YxUtiles.ReduceNumber(m_score);
            }
            m_score = user.GetInt("rewardScore");
            if (m_score >= 0)
            {
                SanPianGreenScore.gameObject.SetActive(true);
                SanPianGreenScore.text = "+" + YxUtiles.ReduceNumber(m_score) ;
            }
            else
            {
                SanPianRedScore.gameObject.SetActive(true);
                SanPianRedScore.text = "" + YxUtiles.ReduceNumber(m_score);
            }
//            head.mainTexture = App.GetGameManager<SanPianGameManager>().PlayerArr[index].UIInfo.HeadTexture.mainTexture; ;

            var curUser = App.GetGameManager<SanPianGameManager>().PlayerArr[index];

            PortraitDb.SetPortrait(curUser.userInfo.HeadImage, curUser.UIInfo.HeadTexture, curUser.userInfo.Sex);
            int[] Temp_cards = user.GetIntArray("cards");
            List<int> cards=new List<int>(Temp_cards);
            App.GetGameManager<SanPianGameManager>().RealPlayer.SortHandList(ref cards);
            int len = cards.Count;
            CardItem cardItem = App.GetGameManager<SanPianGameManager>().cardItem;
            for (int i = 0; i < len; i++)
            {
                CardItem card = (CardItem)Instantiate(cardItem, Vector3.zero, Quaternion.identity);
                card.transform.SetParent(CardsArea.transform);
                card.transform.localScale = Vector3.one * 0.4f;
                card.Value = cards[i];
                card.SetCardDepth(i);
                cardsGj.Add(card.gameObject);
            }
            len = len < 18 ? 18 : len > 27 ? 27 : len;
            CardsArea.cellWidth = 600f / ((float)len - 1f);
            CardsArea.Reposition();
        }

        public void Reset()
        {
            int len = cardsGj.Count;
            for (int i = 0; i < len; i++)
            {
                Destroy(cardsGj[i]);
            }
            RedScore.gameObject.SetActive(false);
            GreenScore.gameObject.SetActive(false);
            SanPianRedScore.gameObject.SetActive(false);
            SanPianGreenScore.gameObject.SetActive(false);
            TouYou.gameObject.SetActive(false);
        }
    }
}
