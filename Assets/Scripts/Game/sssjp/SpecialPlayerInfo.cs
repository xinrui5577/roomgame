using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sssjp
{
    public class SpecialPlayerInfo : MonoBehaviour
    {

        public UILabel NameLabel;

        public UITexture HeadImage;

        public Transform PokerParent;

        public PokerCard PokerPrefab;

        public Vector3 PokerScale = new Vector3(0.5f, 0.5f, 0.5f);

        private List<int> _cardsList = new List<int>();

        public void SetSpecialPlayerInfo(UserMatchInfo matchInfo)
        {
            int seat = matchInfo.Seat;
            var playerPanel = App.GameData.GetPlayer<SssPlayer>(seat,true);
            HeadImage.mainTexture = playerPanel.HeadPortrait.GetTexture();
            NameLabel.text = playerPanel.Nick;
            _cardsList = new List<int>(matchInfo.Cards);
            CreatePokers(_cardsList);
        }

        void CreatePokers(List<int> cardList)
        {
            int createCount =  cardList.Count - PokerParent.childCount;
            if (createCount <= 0)
                return;
            for (int i = 0; i < createCount; i++)
            {
                CreateOnePoker();
            }
        }

        PokerCard GetOne(int index)
        {
            var child =  PokerParent.GetChild(index);
            return child == null ? CreateOnePoker() : child.GetComponent<PokerCard>();
        }

        PokerCard CreateOnePoker()
        {
            var poker = Instantiate(PokerPrefab);
            var ptran = poker.transform;
            ptran.parent = PokerParent;
            ptran.localScale = PokerScale;
            poker.gameObject.SetActive(true);
            poker.StopAllTween();
            return poker;
        }

        public void ShowSpecialPlayerInfo()
        {
            if (_cardsList == null) return;
            int cardCount = _cardsList.Count;
            if (cardCount == 0) return;
            gameObject.SetActive(true);
            for (int i = 0; i < cardCount; i++)
            {
                var poker = GetOne(i);
                poker.gameObject.SetActive(true);
                poker.SetCardId(_cardsList[i]);
                poker.SetCardFront();
                poker.SetCardDepth(100+i*2);
            }
            var grid = PokerParent.GetComponent<UIGrid>();
            grid.repositionNow = true;
            grid.Reposition();
        }

        public void HideSpecialPlayerInfo()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示特殊牌型的玩家信息
        /// </summary>
        /// <param name="matchInfo"></param>
        internal void ShowSpecialPlayerInfo(UserMatchInfo matchInfo)
        {
            SetSpecialPlayerInfo(matchInfo);
            ShowSpecialPlayerInfo();
        }
    }
}
