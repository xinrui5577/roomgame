using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.jlgame.Modle;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGameSelfPlayer : JlGamePlayer
    {
        public UILabel FoldScoreLabel;
        public Transform FoldCardArea;
        public GameObject TrusteeshipObj;
        public List<int> MyCardsValueList;
        public EventDelegate RejoinFresh;

        private int _foldCardIndex;

        public override void SetCardValue(int[] cards = null, int handcard = 0)
        {
            if (cards != null) MyCardsValueList = new List<int>(cards);
            SortHandList(ref MyCardsValueList);
            StartCoroutine(CardValueForStart());
        }

        public void SortHandList(ref List<int> list)
        {
            list.Sort();
            List<CardGroup> groupList = new List<CardGroup>();
            while (list.Count > 0)
            {
                CardGroup group = new CardGroup();
                int temp = list[0];
                group.CardValue = temp;
                var sortGroup = temp >> 4;
                switch (sortGroup)
                {
                    case 1:
                        group.GValue = 4;
                        break;
                    case 2:
                        group.GValue = 2;
                        break;
                    case 3:
                        group.GValue = 3;
                        break;
                    case 4:
                        group.GValue = 1;
                        break;
                }
                list.RemoveAt(0);
                groupList.Add(group);
            }
            groupList.Sort((a, b) =>
            {
                if (a.GValue > b.GValue)
                {
                    return 1;
                }
                if (a.GValue < b.GValue)
                {
                    return -1;
                }

                if (a.GValue == b.GValue)
                {
                    if (a.CardValue > b.CardValue)
                    {
                        return 1;
                    }
                    if (a.CardValue < b.CardValue)
                    {
                        return -1;
                    }

                }
                return 0;
            });
            foreach (CardGroup t in groupList)
            {
                list.Add(t.CardValue);
            }
            YxDebug.LogArray(list);
        }

        IEnumerator CardValueForStart()
        {
            if (MyCardsValueList.Count != CardItemList.Count)
            {
                YxDebug.LogError("手牌与手牌值长度不相等，手牌数为：" + CardItemList.Count + "，值的数量为：" + MyCardsValueList.Count + "手牌值为：");
            }
            for (int i = 0; i < MyCardsValueList.Count; i++)
            {
                CardItemList[i].Value = MyCardsValueList[i];
                CardItemList[i].SetCardDepth(i);
                yield return new WaitForSeconds(0.005f);
            }

             FreshHandCard(null);
            CardsArea.GetComponent<UIGrid>().Reposition();
            if (GetInfo<JlGameUserInfo>().ActiveCards != null)
            {
                FreshHandCard(GetInfo<JlGameUserInfo>().ActiveCards);

                GetInfo<JlGameUserInfo>().ActiveCards = null;
                if (!GetInfo<JlGameUserInfo>().IsCurSpeaker)
                {
                    NoCanClickCard();
                }
            }
            if (GetInfo<JlGameUserInfo>().IsTrusteeship)
            {
                ShowTrusteeshipObj(true);
            }
           
            if (TrusteeshipObj.GetComponent<UIButton>().onClick.Count == 0)
            {
                TrusteeshipObj.GetComponent<UIButton>().onClick.Add(RejoinFresh);
            }
        }

        public override void OutCard(JlGameOutCardsArea outCardsArea, int cardVaule)
        {
            for (int i = 0; i < CardItemList.Count; i++)
            {
                if (CardItemList[i].Value == cardVaule)
                {
                    outCardsArea.FromHandToOutArea(CardItemList[i]);
                    MyCardsValueList.Remove(cardVaule);
                    CardItemList.Remove(CardItemList[i]);
                    CardsArea.GetComponent<UIGrid>().repositionNow = true;
                }
            }
        }

        public override void FreshFoldCardShow(int foldNum, int handNum, int foldScore = 0)
        {
            if (foldNum == 0 || foldNum == -1)
            {
                if (FoldNumLabel != null) FoldNumLabel.gameObject.SetActive(false);
            }
            else
            {
                if (FoldNumLabel != null)
                {
                    FoldNumLabel.gameObject.SetActive(true);
                    FoldNumLabel.text = string.Format("盖牌数：{0}", foldNum);
                }
            }
            if (FoldScoreLabel != null&& foldScore!=0 && foldScore != -1)
            {
                FoldScoreLabel.text = foldScore.ToString();
            }
            if (foldNum == 0 && handNum == 0)
            {
                StartCoroutine(PlayWinAni());
            }
        }

        public override void CreatFoldCard(List<JlGameCardItem> foldCard)
        {
            if(foldCard.Count==0)return;
            for (int i = 0; i < foldCard.Count; i++)
            {
                FromHandToFoldArea(foldCard[i],true);
            }

        }
        public void SortCard()
        {
            for (int i = 0; i < CardItemList.Count; i++)
            {
                if (CardItemList[i].IsCardSelect)
                {
                    CardItemList[i].SetCardUp(false);
                }
            }
        }

        public void FreshHandCard(int[] cards,bool enable=true)
        {
            if (cards == null)
            {
                foreach (var t in CardItemList)
                {
                    t.SetCardDeath();
                }
            }
            else
            {
                if (cards.Length == 0)
                {
                    foreach (var t in CardItemList)
                    {
                        t.SetCardDeath();
                        t.IsFoldCard = true;
                    }
                }
                else
                {
                    foreach (var t in CardItemList)
                    {
                        t.SetCardDeath();
                    }

                    foreach (var t in cards)
                    {
                        foreach (var t1 in CardItemList)
                        {
                            if (t == t1.Value)
                            {
                                t1.SetCardActive(enable);
                            }
                        }
                    }
                }
            }

        }

        public void NoCanClickCard()
        {
            foreach (var t in CardItemList)
            {
                t.NoSpeaker();
            }
        }

        public void FoldCard(int card)
        {
            for (int i = 0; i < CardItemList.Count; i++)
            {
                if (CardItemList[i].Value == card)
                {
                    CardItemList[i].SetCardDeath();
//                    CardItemList[i].IsFoldCard = false;
                    FromHandToFoldArea(CardItemList[i]);
                    MyCardsValueList.Remove(card);
                    CardItemList.Remove(CardItemList[i]);
                }
            }
            CardsArea.GetComponent<UIGrid>().repositionNow = true;
        }
            
        public void FromHandToFoldArea(JlGameCardItem jlGameCardItem,bool rejoin=false)
        {
            var cardDepth = 30;
            var posTemp = FoldCardArea.position;
            jlGameCardItem.transform.parent = FoldCardArea.transform;
            Vector3 scaleTemp = Vector3.one * 0.75f; 
           
            posTemp.x = posTemp.x + (++_foldCardIndex) * -0.1500f;
            jlGameCardItem.SetCardDepth(cardDepth- _foldCardIndex);
            jlGameCardItem.SetCardDeath();

            var moveTime = 0.4f;
            if (rejoin)
            {
                moveTime = 0;
            }
            iTween.MoveTo(jlGameCardItem.gameObject, posTemp, moveTime);
            iTween.ScaleTo(jlGameCardItem.gameObject, scaleTemp, moveTime);
        }

        public override void Reset()
        {
            base.Reset();
            _foldCardIndex = 0;
            ShowTrusteeshipObj(false);
            while (FoldCardArea.transform.childCount>0)
            {
                DestroyImmediate(FoldCardArea.transform.GetChild(0).gameObject);
            }
        }

        public void ShowTrusteeshipObj(bool isShow)
        {
            TrusteeshipObj.SetActive(isShow);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var sss = new int[] { 45, 33, 73, 60, 20, 56, 25, 24, 35, 22, 34, 28, 21 };
                SetCardValue(sss);
            }
        }
    }
    public class CardGroup
    {
        /// <summary>
        /// 该组的大小，仅在排序中使用此值
        /// </summary>
        public int GValue = 0;

        public int CardValue = 0;

    }
}
