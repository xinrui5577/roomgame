using UnityEngine;
using System.Collections.Generic;


namespace Assets.Scripts.Game.sss
{
    public class ChoiseLine1 : MonoBehaviour
    {
        /// <summary>
        /// 第几行
        /// </summary>
        public int LineIndex;

        /// <summary>
        /// 重选按钮(将牌返回到选牌区)
        /// </summary>
        public UIButton ReselectBtn;     //重新选择按钮

        /// <summary>
        /// 选择行按钮
        /// </summary>
        public UIButton SelectBtn;

        /// <summary>
        /// 牌型
        /// </summary>
        public UISprite CardTypeSprite;

        public UIGrid PokerParent;

        /// <summary>
        /// 基础层级设置
        /// </summary>
        public int BaseDepth = 100;

        /// <summary>
        /// 牌组
        /// </summary>
        [HideInInspector]
        public List<PokerCard> CardList;



        private SelectedInfo _lineInfo = new SelectedInfo();



        public SelectedInfo LineInfo { get { return _lineInfo; } }


        /// <summary>
        /// 选择赋值行
        /// </summary>
        /// <param name="selectInfo"></param>
        public void SelectLine(SelectedInfo selectInfo)
        {
            SaveSelectInfo(selectInfo);
            SetLineCardsVal(_lineInfo.Dun.Cards);

            ParentReposition();
            SetCardTypeSprite(_lineInfo.CardType.ToString());
            SetLineState(true);
        }


        /// <summary>
        /// 设置牌
        /// </summary>
        /// <param name="cardValList"></param>
        public void SetLineCardsVal(List<int> cardValList)
        {
            int count = cardValList.Count;
            if (count != CardList.Count) return;
            for (int i = 0; i < count; i++)
            {
                var card = CardList[i];
                card.SetCardId(cardValList[i]);
                card.SetCardFront();
                card.gameObject.SetActive(true);
            }
            _lineInfo.Dun.Cards = cardValList;
        }

        public void SetSelectBtnActive(bool active)
        {
            SelectBtn.gameObject.SetActive(active);
        }

        /// <summary>
        /// 将信息保存到本地
        /// </summary>
        /// <param name="selectInfo"></param>
        void SaveSelectInfo(SelectedInfo selectInfo)
        {
            if(_lineInfo == null)
            {
                _lineInfo = new SelectedInfo();
            }
            _lineInfo.CardType = selectInfo.CardType;
            var dun = new Help1.SssDun();
            var selectDun = selectInfo.Dun;
            dun.Cards = new List<int>(selectDun.Cards);
            dun.CardType = selectDun.CardType;
            _lineInfo.Dun = dun;
            _lineInfo.Index = selectInfo.Index;
            _lineInfo.SelectedCards = new List<PokerCard>(selectInfo.SelectedCards);
        }

        /// <summary>
        /// 设置行状态
        /// </summary>
        /// <param name="finishSelect">选完牌</param>
        void SetLineState(bool finishSelect)
        {
            ReselectBtn.gameObject.SetActive(finishSelect);
            CardTypeSprite.gameObject.SetActive(finishSelect);

            //与状态相反
            SetSelectBtnActive(!finishSelect);
        }


        /// <summary>
        /// 重新选择赋值该行
        /// </summary>
        public void Reselect()
        {
            int childCount = PokerParent.transform.childCount;
            for (int i = 0; i < childCount; i++)
            {
                PokerParent.GetChild(i).gameObject.SetActive(false);
            }
            SetLineState(false);

            var cards = _lineInfo.SelectedCards;
            foreach (var card in cards)
            {
                card.gameObject.SetActive(true);
            }
            _lineInfo.Reset();
        }


        /// <summary>
        /// 设置牌型
        /// </summary>
        /// <param name="spriteName"></param>
        void SetCardTypeSprite(string spriteName)
        {
            CardTypeSprite.spriteName = spriteName;
            CardTypeSprite.MakePixelPerfect();
            CardTypeSprite.gameObject.SetActive(true);
        }



        /// <summary>
        /// 设置一张牌,包括大小,显示层级等信息
        /// </summary>
        /// <param name="card"></param>
        public void SetOneCard(PokerCard card)
        {
            card.name = "line";
            card.transform.parent = PokerParent.transform;
            card.transform.localScale = Vector3.one * 0.6f;
            card.SetCardDepth(BaseDepth + CardList.Count * 4);
            CardList.Add(card);

            //不可点击
            card.gameObject.SetActive(false);
        }

        public void ParentReposition()
        {
            PokerParent.repositionNow = true;
            PokerParent.Reposition();
        }

    }
}