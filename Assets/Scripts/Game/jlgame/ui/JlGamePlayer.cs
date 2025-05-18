using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.jlgame.Modle;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.jlgame.ui
{
    public class JlGamePlayer : YxBaseGamePlayer
    {
        public UILabel CountLabel;
        public GameObject RobObj;
        public GameObject WinAni;
        public UILabel FoldNumLabel;
        public Transform CardsArea;
        public Transform OutCardPos;

        public List<JlGameCardItem> CardItemList = new List<JlGameCardItem>();
        protected int StartCardIndex = 0;
        //牌的飞行时间
        public float flyTime = 1f;

        private float _countDownTime;

        private bool _isStop;
        //开始时接牌
        public void GetCardsOnStart(JlGameCardItem jlGameCard, bool isSelf, int cardNum)
        {
            CardItemList.Add(jlGameCard);
            jlGameCard.SetCardDepth(StartCardIndex + 3);
            jlGameCard.transform.SetParent(CardsArea);
            Vector3 posTemp = CardsArea.localPosition;
            Vector3 scaleTemp;
            if (isSelf)
            {
                jlGameCard.MyCardFlag = true;
                scaleTemp = Vector3.one;
                posTemp = new Vector3(StartCardIndex * 80f, 0, posTemp.z);
                StartCardIndex++;
            }
            else
            {
                scaleTemp = Vector3.one * 0.35f;
                posTemp = new Vector3(0, StartCardIndex * 2.16f, posTemp.z);
                StartCardIndex++;
            }
            TweenPosition.Begin(jlGameCard.gameObject, 0.4f, posTemp);
            TweenScale.Begin(jlGameCard.gameObject, 0.4f, scaleTemp);
        }

        public void CreateCards(JlGameCardItem jlGameCardItem, bool isSelf, int len, int[] cards, UIEventListener.VoidDelegate click = null)
        {
            for (int i = 0; i < len; i++)
            {
                JlGameCardItem jlGameCard = (JlGameCardItem)Instantiate(jlGameCardItem, new Vector3(0.002f * i - 0.1f, 0, 0), Quaternion.identity);
                jlGameCard.GetComponent<UIEventListener>().onClick = click;
                jlGameCard.transform.SetParent(CardsArea);
                jlGameCard.transform.localScale = Vector3.one * 0.4f;
                jlGameCard.SetCardDepth(i + 3);
                jlGameCard.SetBackKey();

                Vector3 posTemp = CardsArea.localPosition;
                Vector3 scaleTemp;
                if (isSelf)
                {
                    jlGameCard.MyCardFlag = true;
                    posTemp = new Vector3(StartCardIndex * 80f, 0, posTemp.z);
                    scaleTemp = Vector3.one;
                    StartCardIndex++;
                }
                else
                {
                    scaleTemp = Vector3.one * 0.35f;
                    posTemp = new Vector3(0, StartCardIndex * 2.16f, posTemp.z);
                    StartCardIndex++;
                }

                jlGameCard.transform.localPosition = posTemp;
                jlGameCard.transform.localScale = scaleTemp;
                CardItemList.Add(jlGameCard);
            }

            SetCardValue(cards, len);

            List<JlGameCardItem> foldCards = new List<JlGameCardItem>();
            if (GetInfo<JlGameUserInfo>().FoldCards != null)
            {
                for (int i = 0; i < GetInfo<JlGameUserInfo>().FoldCards.Length; i++)
                {
                    JlGameCardItem foldJlGameCard = (JlGameCardItem)Instantiate(jlGameCardItem);
                    foldJlGameCard.Value = GetInfo<JlGameUserInfo>().FoldCards[i];
                    foldCards.Add(foldJlGameCard);
                }
            }

            CreatFoldCard(foldCards);
            FreshFoldCardShow(GetInfo<JlGameUserInfo>().FoldNum, len, GetInfo<JlGameUserInfo>().FoldScore);
        }

        public virtual void CreatFoldCard(List<JlGameCardItem> foldCard)
        {

        }

        public virtual void SetCardValue(int[] cards = null, int handNum = 0)
        {
            int len = CardItemList.Count;
            if (len <= 0)
            {
                return;
            }

            if (handNum != len && handNum == 0)
            {
                YxDebug.Log("此时手牌与服务器不同步 请检查");
            }
            CardItemList[len - 1].SetLen(handNum);

            if (GetInfo<JlGameUserInfo>().IsTrusteeship)
            {
                RobObj.SetActive(GetInfo<JlGameUserInfo>().IsTrusteeship);
            }
        }



        public virtual void FreshFoldCardShow(int foldNum, int handNum, int foldScore = 0)
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

            SetCardValue(null, handNum);

            if (foldNum == 0 && handNum == 0)
            {
                StartCoroutine(PlayWinAni());
            }
        }

        public virtual void OutCard(JlGameOutCardsArea outCardsArea, int cardVaule)
        {
            var cardItem = CardItemList[CardItemList.Count - 1];
            cardItem.Value = cardVaule;
            cardItem.transform.position = OutCardPos.position;
            outCardsArea.FromHandToOutArea(cardItem);

            CardItemList.Remove(cardItem);
            SetCardValue(null, CardItemList.Count);
        }

        public IEnumerator PlayWinAni()
        {
            Facade.Instance<MusicManager>().Play("DaTong");
            WinAni.SetActive(true);
            WinAni.GetComponent<Animator>().enabled = true;
            yield return new WaitForSeconds(0.5f);
            WinAni.GetComponent<Animator>().Stop();
        }

        public void ShowRob(bool show)
        {
            RobObj.SetActive(show);
        }

        public void SetCountNum(int time,bool stop=false)
        {
            CountLabel.text = time.ToString();
            _isStop = stop;
            _countDownTime = time;
            InvokeRepeating("FreshTime",0,1);
        }

        private void FreshTime()
        {
            _countDownTime--;
            CountLabel.text = _countDownTime.ToString();
            if (_countDownTime <= 0)
            {
                CancelInvoke("FreshTime");
                CountLabel.text = _isStop ? 0.ToString() : "";
            }
        }

        public void HideCountDown()
        {
            CancelInvoke("FreshTime");
            CountLabel.text = "";
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                StartCoroutine(PlayWinAni());
            }
        }
        public virtual void Reset()
        {
            foreach (var t in CardItemList)
            {
                DestroyImmediate(t.gameObject);
            }
            CardItemList.Clear();

            if (FoldNumLabel != null)
            {
                FoldNumLabel.gameObject.SetActive(false);
            }

            StartCardIndex = 0;

            WinAni.SetActive(false);
            RobObj.SetActive(false);
        }
    }
}
