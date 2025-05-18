using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.biji.Sound;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjPlayer : YxBaseGamePlayer
    {
        public GameObject RoomOwnerIcon;
        public GameObject GiveUpSprite;
        public UISprite PlayerGoldItem;
        public UIGrid PlayerGoldGrid;
        public GameObject SettleTip;
        public List<BjCardItem> SettleCardsList;
        public GameObject SettleCardsObj;
        public GameObject ShowCardsObj;
        public List<BjCardItem> ShowCardsList;
        public UIGrid XiPaiGrid;
        public UISprite XiPaiType;
        public UIGrid DaoTotalScoreGrid;
        public UISprite DaoTotalScoreItem;
        public List<UISprite> DaoTypes;
        public UISprite DaoScoreItem;
        public List<UIGrid> DaoScoreGrids;

        public int DaoTypePosFirst;
        public GameObject CoinItem;

        public List<int[]> DaosCards;
        private List<int[]> _daosRealCards;

        private int _index;
        private bool _islook;
        private int _compareIndex = -1;
        private bool _startCompare;
        private int _totalScore;

        public void ShowRoomOwnerIcon()
        {
            RoomOwnerIcon.SetActive(true);
        }

        public void ShowGiveUp()
        {
            GiveUpSprite.SetActive(true);
        }

        public void ShowPlayerGold(long coin)
        {
            while (PlayerGoldGrid.transform.childCount > 0)
            {
                DestroyImmediate(PlayerGoldGrid.transform.GetChild(0).gameObject);
            }
            var goldStr = coin.ToString();
            if (coin >= 0)
            {
                var obj = YxWindowUtils.CreateItem(PlayerGoldItem, PlayerGoldGrid.transform);
                obj.spriteName = "win";
                obj.MakePixelPerfect();

                for (int i = 0; i < goldStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(PlayerGoldItem, PlayerGoldGrid.transform);
                    item.spriteName = "win" + goldStr[i];
                    item.MakePixelPerfect();
                }
            }
            else
            {
                var obj = YxWindowUtils.CreateItem(PlayerGoldItem, PlayerGoldGrid.transform);
                obj.spriteName = "lose";
                obj.MakePixelPerfect();
                for (int i = 1; i < goldStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(PlayerGoldItem, PlayerGoldGrid.transform);
                    item.spriteName = "lose" + goldStr[i];
                    item.MakePixelPerfect();
                }
            }

            PlayerGoldGrid.repositionNow = true;
        }

        public void CallStartSettleCards()
        {
            foreach (var item in SettleCardsList)
            {
                item.transform.localPosition = new Vector3(item.transform.localPosition.x, 0, 0);
            }
            SettleTip.SetActive(true);
            SettleCardsObj.SetActive(true);
            InvokeRepeating("StartSettleCards", 0, 0.1f);
        }

        public void StartSettleCards()
        {
            var index = _index++;
            if (index == 8) _index = 0;
            SettleCardsList[index].Bounce();
        }


        public void HideSettleView()
        {
            SettleTip.SetActive(false);
            SettleCardsObj.SetActive(false);
            CancelInvoke("StartSettleCards");
        }

        public void WaitCompareCard()
        {
            if (SettleTip.activeSelf)
            {
                HideSettleView();
            }

            if (ShowCardsObj.activeSelf)
            {
                return;
            }

            ShowCardsObj.SetActive(true);
            ShowCardsObj.transform.localScale = Vector3.zero;
            TweenScale.Begin(ShowCardsObj, 0.3f, Vector3.one * 0.55f);
        }

        public void OnLookCards()
        {
            if (_startCompare)
            {
                return;
            }

            if (_islook)
            {
                SetShowCardsBack();
            }
            else
            {
                SetShowCardsValue();
            }

            _islook = !_islook;
        }

        private void SetShowCardsBack()
        {
            for (int i = 0; i < ShowCardsList.Count; i++)
            {
                ShowCardsList[i].ShowCardBack();
            }
        }

        private void SetShowCardsValue()
        {
            if (DaosCards == null) return; 

            var cards = new List<int>();
            var realCards=new List<int>();
            for (int i = 0; i < DaosCards.Count; i++)
            {
                cards.AddRange(DaosCards[i]);
                realCards.AddRange(_daosRealCards[i]);
            }

            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i] == realCards[i])
                {
                    ShowCardsList[i].Value = cards[i];
                }
                else
                {
                    ShowCardsList[i].Value = realCards[i];
                    ShowCardsList[i].KingName(cards[i]);
                }
               
            }
        }

        public void SetPutCards(List<int[]> cards, List<int[]> realCards)
        {
            DaosCards = new List<int[]>(cards);
            _daosRealCards=new List<int[]>(realCards);
        }

        public void SetCompareData(int[] daoDatas,int[] daoRealDatas, int daoType,float time, Action<BjSound.EnAudio> musicAction)
        {
            if (!_startCompare)
            {
                _startCompare = true;
                SetShowCardsBack();
            }
         
            StartCoroutine(ShowCompareView(daoDatas, daoRealDatas, daoType, time, musicAction));
        }

        IEnumerator ShowCompareView(int[] daoDatas, int[] daoRealDatas, int daoType, float time, Action<BjSound.EnAudio> musicAction)
        {
            _compareIndex++;
            if (daoDatas == null|| daoRealDatas==null)
            {
                yield break ;
            }

            var daoCards =new List<int>(daoDatas); 
            for (int i = _compareIndex * 3; i < daoDatas.Length + _compareIndex * 3; i++)
            {
                if (daoCards.Contains(daoRealDatas[i - _compareIndex * 3]))
                {
                    daoCards.Remove(daoRealDatas[i - _compareIndex * 3]);
                    ShowCardsList[i].Value = daoRealDatas[i - _compareIndex * 3];
                }
                else
                {
                    ShowCardsList[i].Value = daoRealDatas[i - _compareIndex * 3];

                    var king = 0 ;

                    for (int j = 0; j < daoCards.Count; j++)
                    {
                        if (daoCards[j] == 81 || daoCards[j]==97)
                        {
                            king = daoCards[j];
                            daoCards.Remove(daoCards[j]);
                            break;
                        }
                    }
                    ShowCardsList[i].KingName(king);
                }
              
                ShowCardsList[i].SetCardDepth(i + 10);
                TweenScale.Begin(ShowCardsList[i].gameObject, time, Vector3.one * 1.3f);
            }
            yield return new WaitForSeconds(time);

            for (int i = _compareIndex * 3; i < daoDatas.Length + _compareIndex * 3; i++)
            {
                ShowCardsList[i].SetCardDepth(i + 1);
                ShowCardsList[i].GetComponent<TweenScale>().PlayReverse();
            }

            DaoTypes[_compareIndex].gameObject.SetActive(true);
            DaoTypes[_compareIndex].gameObject.transform.localPosition = new Vector3(DaoTypePosFirst, DaoTypes[_compareIndex].gameObject.transform.localPosition.y, 0);
            var pos = new Vector3(0, DaoTypes[_compareIndex].gameObject.transform.localPosition.y, 0);

            DaoTypes[_compareIndex].spriteName = DaoType(daoType);
            DaoTypes[_compareIndex].MakePixelPerfect();

            TweenPosition.Begin(DaoTypes[_compareIndex].gameObject, time, pos);

            musicAction(SoundAudio(daoType));
        }

        private BjSound.EnAudio SoundAudio(int type)
        {
            BjSound.EnAudio enAudio = BjSound.EnAudio.None;
            switch (type)
            {
                case 1:
                    enAudio = BjSound.EnAudio.WuLong;
                    break;
                case 2:
                    enAudio = BjSound.EnAudio.DuiZi;
                    break;
                case 3:
                    enAudio = BjSound.EnAudio.ShunZi;
                    break;
                case 4:
                    enAudio = BjSound.EnAudio.TongHua;
                    break;
                case 5:
                    enAudio = BjSound.EnAudio.TongHuaShun;
                    break;
                case 6:
                    enAudio = BjSound.EnAudio.SanTiao;
                    break;
            }

            return enAudio;
        }
        private string DaoType(int type)
        {
            var cardType = "";
            switch (type)
            {
                case 1:
                    cardType = "wulong";
                    break;
                case 2:
                    cardType = "duizi";
                    break;
                case 3:
                    cardType = "shunzi";
                    break;
                case 4:
                    cardType = "tonghua";
                    break;
                case 5:
                    cardType = "tonghuashun";
                    break;
                case 6:
                    cardType = "santiao";
                    break;
            }

            return cardType;
        }

        public void SetDaoScore(int score)
        {
            _totalScore += score;
            var scoreStr = score.ToString();
            if (score >= 0)
            {
                var obj = YxWindowUtils.CreateItem(DaoScoreItem, DaoScoreGrids[_compareIndex].transform);
                obj.spriteName = "win";
                obj.MakePixelPerfect();

                for (int i = 0; i < scoreStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(DaoScoreItem, DaoScoreGrids[_compareIndex].transform);
                    item.spriteName = "win" + scoreStr[i];
                    item.MakePixelPerfect();
                }
            }
            else
            {
                var obj = YxWindowUtils.CreateItem(DaoScoreItem, DaoScoreGrids[_compareIndex].transform);
                obj.spriteName = "lose";
                obj.MakePixelPerfect();
                for (int i = 1; i < scoreStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(DaoScoreItem, DaoScoreGrids[_compareIndex].transform);
                    item.spriteName = "lose" + scoreStr[i];
                    item.MakePixelPerfect();
                }
            }
            DaoScoreGrids[_compareIndex].repositionNow = true;
            ShowUserTotalScore();
        }

        private void ShowUserTotalScore()
        {
            var totalStr = _totalScore.ToString();
            while (DaoTotalScoreGrid.transform.childCount > 0)
            {
                DestroyImmediate(DaoTotalScoreGrid.transform.GetChild(0).gameObject);
            }

            if (_totalScore >= 0)
            {
                var obj = YxWindowUtils.CreateItem(DaoTotalScoreItem, DaoTotalScoreGrid.transform);
                obj.spriteName = "win";
                obj.MakePixelPerfect();

                for (int i = 0; i < totalStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(DaoTotalScoreItem, DaoTotalScoreGrid.transform);
                    item.spriteName = "win" + totalStr[i];
                    item.MakePixelPerfect();
                }
            }
            else
            {
                var obj = YxWindowUtils.CreateItem(DaoTotalScoreItem, DaoTotalScoreGrid.transform);
                obj.spriteName = "lose";
                obj.MakePixelPerfect();
                for (int i = 1; i < totalStr.Length; i++)
                {
                    var item = YxWindowUtils.CreateItem(DaoTotalScoreItem, DaoTotalScoreGrid.transform);
                    item.spriteName = "lose" + totalStr[i];
                    item.MakePixelPerfect();
                }
            }
            DaoTotalScoreGrid.repositionNow = true;
            DaoTotalScoreGrid.transform.localScale = Vector3.zero;
            TweenScale.Begin(DaoTotalScoreGrid.gameObject, 0.3f, Vector3.one);
        }


        public void SetXiPaiData(int xiPaiScore, int[] xipaiType)
        {
            if (xiPaiScore == 0) return;

            _totalScore += xiPaiScore;
            ShowUserTotalScore();

            for (int i = 0; i < xipaiType.Length; i++)
            {
                var item = YxWindowUtils.CreateItem(XiPaiType, XiPaiGrid.transform);
                item.spriteName = GetXiPaiType(xipaiType[i]);
                item.MakePixelPerfect();
            }

            XiPaiGrid.repositionNow = true;
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

        public void ResultLoseCoinFly(Transform parent)
        {
            if (CoinItem == null && parent == null) return;
            for (int i = 0; i < 13; i++)
            {
                var x = Random.Range(-50, 50);
                var y = Random.Range(-50, 50);
                var pos = new Vector3(x, y, 0);
                var item = YxWindowUtils.CreateGameObject(CoinItem, transform);
                item.transform.parent = parent;
                var tween = TweenPosition.Begin(item, 0.2f, pos, false);
                tween.delay = i * 0.03f;
            }
        }

        public void ResulWinCoinFly(Transform parent, int coinCount)
        {
            if (CoinItem == null && parent == null) return;
            var index = 0;
            while (parent.transform.childCount > 0)
            {
                if (index < coinCount)
                {
                    var item = parent.transform.GetChild(0);
                    item.transform.parent = transform;
                    var tween = TweenPosition.Begin(item.gameObject, 0.2f, Vector3.zero, false);
                    tween.AddOnFinished(() =>
                    {
                        DestroyImmediate(item.gameObject);
                    });
                }
                else if (index >= coinCount)
                {
                    return;
                }

                index++;
            }
        }

        public void Reset()
        {
            _index = 0;
            _compareIndex = -1;
            _totalScore = 0;
            _islook = false;
            DaosCards = null;
            _startCompare = false;
            GiveUpSprite.SetActive(false);
            ShowCardsObj.SetActive(false);
            SetShowCardsBack();

            while (XiPaiGrid.transform.childCount > 0)
            {
                DestroyImmediate(XiPaiGrid.transform.GetChild(0).gameObject);
            }



            while (DaoTotalScoreGrid.transform.childCount > 0)
            {
                DestroyImmediate(DaoTotalScoreGrid.transform.GetChild(0).gameObject);
            }

            for (int i = 0; i < DaoTypes.Count; i++)
            {
                DaoTypes[i].gameObject.SetActive(false);
                while (DaoScoreGrids[i].transform.childCount > 0)
                {
                    DestroyImmediate(DaoScoreGrids[i].transform.GetChild(0).gameObject);
                }
            }
        }
    }
}
