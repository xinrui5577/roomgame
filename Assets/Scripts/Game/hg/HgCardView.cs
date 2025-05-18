using System.Collections;
using System.Collections.Generic;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.hg
{
    public class HgCardView : MonoBehaviour
    {
        public EventObject EventObj;

        public Transform CardPts;

        public HgCardItem HgCardItem;

        public List<HgCardItem> AllCardItems;

        public UISprite BlcakCardType;

        public UISprite RedCardType;

        public Vector3 MovePosVec = Vector3.zero;

        private CardValue _cardValue;

        private HgGameManager _gmanager
        {
            get { return App.GetGameManager<HgGameManager>(); }
        }

        public List<Transform> CardParent;
        public List<UISprite> CardTypeNn;

        private List<HgCardItem> cardItems = new List<HgCardItem>();


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                var ss = new int[] { 1, 2, 3, 4, 5, 6 };
                foreach (var s in ss)
                {

                    if (s == 3)
                    {
                        continue;
                    }
                    Debug.LogError("s" + s);
                }
            }
        }

        IEnumerator CreatCard(CardValue cardValue)
        {
            CardPts.transform.localPosition = MovePosVec;
            for (int i = 0; i < 25; i++)
            {
                var item = YxWindowUtils.CreateItem(HgCardItem, CardPts);
                item.SetDepth(i);
                item.transform.localPosition = new Vector3(item.transform.localPosition.x, item.transform.localPosition.y + 3f * i, 0);
                cardItems.Add(item);
            }
            var index = -1;
            var depth = 0;
            float delay = 0;
            for (int i = cardItems.Count - 1; i >= 0; i--)
            {
                if (i % 5 == 4)
                {
                    depth = 0;
                    index++;
                }
                delay += 0.05f;
                var depth1 = depth;
                cardItems[i].SetDepth(depth++);
                cardItems[i].transform.parent = CardParent[index];
                var obj = TweenPosition.Begin(cardItems[i].gameObject, 0.15f, new Vector3(depth1 * 64, 0, 0)); //
                obj.delay = delay;
            }
            yield return new WaitForSeconds(1.5f);
            for (int i = 0; i < CardParent.Count; i++)
            {
                for (int j = 0; j < CardParent[i].childCount; j++)
                {
                    var obj = TweenPosition.Begin(CardParent[i].transform.GetChild(j).gameObject, 0.15f, new Vector3((CardParent[i].childCount - 1) * 64, 0, 0));
                    obj.delay = 0.15f;
                }
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < CardParent.Count; i++)
            {
                for (int j = 0; j < CardParent[i].childCount; j++)
                {
                    var obj = TweenPosition.Begin(CardParent[i].transform.GetChild(j).gameObject, 0.15f, new Vector3(j * 64, 0, 0));
                    obj.GetComponent<HgCardItem>().SetDepth(j);
                    obj.delay = 0.15f;

                    if (j != CardParent[i].childCount - 2)
                    {
                        obj.GetComponent<HgCardItem>().SetCardValue(cardValue.Cards[i][j]);
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < CardParent.Count; i++)
            {
                for (int j = 0; j < CardParent[i].childCount; j++)
                {
                    Vector3 vec;
                    var item = CardParent[i].transform.GetChild(j);
                    if (j < 4)
                    {
                        vec = new Vector3(item.transform.localPosition.x + 32, 0, 0);
                    }
                    else
                    {
                        vec = new Vector3(item.transform.localPosition.x - 32, 0, 0);
                    }
                    TweenPosition.Begin(item.gameObject, 0.05f, vec);
                }
                yield return new WaitForSeconds(0.5f);
                for (int j = 0; j < CardParent[i].childCount; j++)
                {
                    CardParent[i].transform.GetChild(j).GetComponent<TweenPosition>().PlayReverse();
                    var obj = CardParent[i].transform.GetChild(j);
                    obj.GetComponent<HgCardItem>().SetDepth(j);
                    obj.GetComponent<HgCardItem>().SetCardValue(cardValue.Cards[i][j]);
                }

                yield return new WaitForSeconds(0.5f);
                CardTypeNn[i].spriteName = GetCardTypeNnName(cardValue.NnDatas[i]);
                CardTypeNn[i].gameObject.SetActive(true);
                CardTypeNn[i].GetComponent<TweenScale>().PlayForward();
            }
            _gmanager.LaterSend = false;
        }


        public void OnRecive(EventData data)
        {
            switch (data.Name)
            {
                case "Creat":
                    CreatCards();
                    break;
                case "Show":
                    ShowCardValue((CardValue)data.Data);
                    break;
                case "Clear":
                    Clear();
                    break;
            }
        }

        private string GetCardTypeNnName(NnData nnData)
        {
            var str = "";
            if (nnData.Type > 10)
            {
                str = string.Format("niu{0}", nnData.Type);
            }
            else
            {
                str = string.Format("niu{0}", nnData.Niu);
            }
            return str;
        }

        public void CreatCards()
        {
            if (AllCardItems.Count == 0) return;
            CardPts.transform.localPosition = MovePosVec;
            _gmanager.LaterSend = true;
            for (int i = 0; i < AllCardItems.Count; i++)
            {
                AllCardItems[i].GetComponent<TweenPosition>().onFinished.Clear();
                AllCardItems[i].GetComponent<TweenPosition>().PlayForward();
                AllCardItems[i].GetComponent<TweenPosition>().AddOnFinished(() =>
                {
                    Facade.Instance<MusicManager>().Play("sendcard");
                });
                if (i == AllCardItems.Count - 1)
                {
                    _gmanager.LaterSend = false;
                }
            }
        }


        public void ShowCardValue(CardValue cardValue)
        {
            _gmanager.LaterSend = true;
            _cardValue = cardValue;
            if (AllCardItems.Count == 0)
            {
                if (cardValue.NnDatas.Count != 0)
                {
                    StartCoroutine(CreatCard(cardValue));
                }

                if (cardValue.CardsDatas.Count != 0)
                {

                }
            }
            else
            {
                RatateCard(cardValue.BlackCards, 0, cardValue.BlackCards.Count, () =>
                {
                    BlcakCardType.gameObject.SetActive(true);
                    BlcakCardType.transform.GetChild(0).gameObject.SetActive(_cardValue.BlackCardType != 0);
                    var cardName = CardTypeValue(_cardValue.BlackCardType);
                    BlcakCardType.spriteName = cardName;
                    Facade.Instance<MusicManager>().Play(cardName);
                });

                var total = cardValue.BlackCards.Count * 2;

                RatateCard(cardValue.RedCards, cardValue.BlackCards.Count, total, () =>
                {
                    RedCardType.gameObject.SetActive(true);
                    RedCardType.transform.GetChild(0).gameObject.SetActive(_cardValue.RedCardType != 0);
                    var cardName = CardTypeValue(_cardValue.RedCardType);
                    RedCardType.spriteName = cardName;
                    Facade.Instance<MusicManager>().Play(cardName);
                });

            }

        }

        private void RatateCard(List<int> cardValues, int index, int length, EventDelegate.Callback callback = null)
        {
            for (int i = index; i < length; i++)
            {
                if (i == length - 1)
                {
                    AllCardItems[i].RotateCardBg(cardValues[i % cardValues.Count], callback, true);

                    _gmanager.LaterSend = false;
                }
                else
                {
                    AllCardItems[i].RotateCardBg(cardValues[i % 3]);
                }
            }

        }


        private string CardTypeValue(int type)
        {
            var cardType = "";
            switch (type)
            {
                case (int)CardType.DanPai:
                    cardType = "danzhang";
                    break;
                case (int)CardType.DuiZi:
                    cardType = "duizi";
                    break;
                case (int)CardType.ShunZi:
                    cardType = "shunzi";
                    break;
                case (int)CardType.JinHua:
                    cardType = "jinhua";
                    break;
                case (int)CardType.ShunJin:
                    cardType = "shunjin";
                    break;
                case (int)CardType.BaoZi:
                    cardType = "baozi";
                    break;
            }

            return cardType;
        }

        public void Clear()
        {
            _gmanager.LaterSend = true;
            BlcakCardType.gameObject.SetActive(false);
            RedCardType.gameObject.SetActive(false);

            CardPts.transform.localPosition = new Vector3(0, 500, 0);

            for (int i = 0; i < AllCardItems.Count; i++)
            {
                AllCardItems[i].GetComponent<TweenPosition>().onFinished.Clear();
                AllCardItems[i].GetComponent<TweenPosition>().PlayReverse();
                AllCardItems[i].Clear();
            }

            for (int i = 0; i < cardItems.Count; i++)
            {
                DestroyImmediate(cardItems[i].gameObject);

            }

            if (CardTypeNn.Count != 0)
            {
                for (int i = 0; i < CardTypeNn.Count; i++)
                {
                    CardTypeNn[i].gameObject.SetActive(false);
                    CardTypeNn[i].GetComponent<TweenScale>().ResetToBeginning();
                }
            }
            cardItems.Clear();
            _gmanager.LaterSend = false;
        }
    }

    public enum CardType
    {
        DanPai,
        DuiZi,
        ShunZi,
        JinHua,
        ShunJin,
        BaoZi
    }
}
