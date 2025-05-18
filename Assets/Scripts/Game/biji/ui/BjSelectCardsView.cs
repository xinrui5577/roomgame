using System;
using System.Collections.Generic;
using Assets.Scripts.Game.biji.EventII;
using Assets.Scripts.Game.biji.Modle;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.View;

namespace Assets.Scripts.Game.biji.ui
{
    public class BjSelectCardsView : MonoBehaviour
    {
        public EventObject EventObj;
        public GameObject View;
        public GameObject AcrossBg;
        public GameObject VerticalBg;
        public GameObject GiveUpBtn;
        public UISprite SotColortBtn;
        public UISprite SortValueBtn;
        public UILabel CountDown;

        public List<UISprite> ButtonList;
        public List<BjLoadItem> AcrossLoads;
        public List<BjLoadItem> VerticalLoads;

        public List<int> MyCardsValueList;

        public UIGrid SettleCardsGrid;
        public List<BjCardItem> SettleCardsList;

        private List<XipaiType> _xiPaiTypes;
        private List<CardGroup> _groupList;
        //        private int _index;
        private float _time;
        private bool _xiPai;
        private List<List<int>> _lianShun;
        private GameHelp _gameHelp;
        private GameHelp1 _gameHelp1;

        //        private bool _clickAuto;
        protected void Start()
        {
            _gameHelp = new GameHelp();
            _gameHelp1 = new GameHelp1();
        }

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Rule":
                    OnRule(data.Data);
                    break;
                case "Start":
                    OnStart(data.Data);
                    break;
                case "Close":
                    View.SetActive(false);
                    CancelInvoke("TimeChange");
                    //                    _clickAuto = false;
                    Reset();
                    break;
                case "Error":
                    Reset();
                    break;
            }
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
//                                int[] aa = new[] { 73, 24, 38, 27, 81, 42, 97, 69, 55 };//联顺的情况的牌值
//                                int[] aa = new[] { 81, 68, 50, 27, 19, 20, 46, 42, 41 };
//                int[] aa = new[] { 18, 67, 20, 21, 38, 71, 73, 75, 81 };
                int[] aa = new[] { 97, 81, 69,70,24,39,60,57,52};
                //                int[] aa = new[] { 30, 71, 67, 81, 72, 40, 97, 75, 74 };
                _xiPaiTypes = _gameHelp1.GetXiPaiType(new List<int>(aa));
                _lianShun = _gameHelp1.LianShun(new List<int>(aa));
//                _gameHelp.HongHei = true;
//                _gameHelp1.SetHongHei(true);
                SetCardsValue(aa);
                SortCardsValue();
                GetAllCardType();
//                _xiPai = true;
            }
        }

        public void OnRule(object data)
        {
            var ruleData = (ISFSObject)data;
            var giveUp = ruleData.GetBool("giveUp");
            GiveUpBtn.SetActive(giveUp);
            var hongHei = ruleData.GetBool("hongHei");
            _gameHelp.HongHei = hongHei;
            _gameHelp1.SetHongHei(hongHei);
            _xiPai = ruleData.GetBool("xiPai");
        }

        public void OnStart(object data)
        {
            Reset();
            var cardsData = (ISFSObject)data;
            View.SetActive(true);
            var cards = cardsData.GetIntArray("cards");
            SetCardsValue(cards);
            _xiPaiTypes = _gameHelp1.GetXiPaiType(new List<int>(cards));
            _lianShun = _gameHelp1.LianShun(new List<int>(cards));
            SortCardsValue();
            GetAllCardType();
            //            InvokeRepeating("StartSettleCards", 0, 0.1f);

            var cd = cardsData.GetInt("cd");
            SetTime(cd);

            //            StartCoroutine(WaitClickAuto());
        }

        //        IEnumerator WaitClickAuto()
        //        {
        //            yield return new WaitForSeconds(1f);
        //            _clickAuto = true;
        //        }

        public void OnChangeViewBtn()
        {
            if (AcrossBg.activeSelf)
            {
                for (int i = 0; i < AcrossLoads.Count; i++)
                {
                    AcrossLoads[i].ChangeParent(VerticalLoads[i]);
                }
            }
            else
            {
                for (int i = 0; i < VerticalLoads.Count; i++)
                {
                    VerticalLoads[i].ChangeParent(AcrossLoads[i]);
                }
            }
            AcrossBg.SetActive(!AcrossBg.activeSelf);
            VerticalBg.SetActive(!VerticalBg.activeSelf);

        }

        public void OnAchieveBtn()
        {
            ISFSObject obj = new SFSObject();
            ISFSArray arr = new SFSArray();
            if (AcrossBg.activeSelf)
            {
                for (int i = 0; i < AcrossLoads.Count; i++)
                {
                    if (AcrossLoads[i].CardsValue.Count < 3)
                    {
                        EventObj.SendEvent("TipEvent", "Remind", "头墩还未设置牌型");
                        return;
                    }
                    ISFSObject arrItem = new SFSObject();
                    arrItem.PutInt("type", CheckCardType(AcrossLoads[i].CardType));
//                    Debug.LogError("发送此道的值");
//                    YxDebug.LogArray(AcrossLoads[i].CardsValue);
                    arrItem.PutIntArray("cards", AcrossLoads[i].CardsValue.ToArray());
//                    Debug.LogError("发送此道的真值");
//                    YxDebug.LogArray(AcrossLoads[i].RealCards());
                    arrItem.PutIntArray("realcards", AcrossLoads[i].RealCards().ToArray());
                    arr.AddSFSObject(arrItem);
                }
                obj.PutSFSArray("daolist", arr);
            }
            else
            {
                for (int i = 0; i < VerticalLoads.Count; i++)
                {
                    if (VerticalLoads[i].CardsValue.Count == 0)
                    {
                        EventObj.SendEvent("TipEvent", "Remind", "头墩还未设置牌型");
                        return;
                    }
                    ISFSObject arrItem = new SFSObject();
                    arrItem.PutInt("type", CheckCardType(VerticalLoads[i].CardType));
                    arrItem.PutIntArray("cards", VerticalLoads[i].CardsValue.ToArray());
                    arrItem.PutIntArray("realcards", VerticalLoads[i].RealCards().ToArray());
                    arr.AddSFSObject(arrItem);
                }
                obj.PutSFSArray("daolist", arr);
            }
            obj.PutInt("type", 5);
            EventObj.SendEvent("ServerEvent", "SendCards", obj);
//            Reset();
        }

        private int CheckCardType(CardType cardType)
        {
            int type = 0;
            switch (cardType)
            {
                case CardType.SanPai:
                    type = 1;
                    break;
                case CardType.DuiZi:
                    type = 2;
                    break;
                case CardType.ShunZi:
                    type = 3;
                    break;
                case CardType.TongHua:
                    type = 4;
                    break;
                case CardType.TongHuaShun:
                    type = 5;
                    break;
                case CardType.SanTiao:
                    type = 6;
                    break;
            }
            return type;
        }

        public void SetCardsValue(int[] card)
        {
            var cards = new List<int>(card);
            SetCardGroup(cards);

        }

        //        public void StartSettleCards()
        //        {
        //            var index = _index++;
        //
        //            if (index >= SettleCardsList.Count - 1)
        //            {
        //                CancelInvoke("StartSettleCards");
        //            }
        //            SettleCardsList[index].Bounce();
        //        }
        private void SetCardsValue()
        {
            for (int i = 0; i < MyCardsValueList.Count; i++)
            {
                if (SettleCardsList[i].MoveFlag)
                {
                    SettleCardsList[i].MoveUp();
                }
                SettleCardsList[i].Value = MyCardsValueList[i];
            }
        }

        public void SetCardGroup(List<int> cards)
        {
            _groupList = new List<CardGroup>();
            while (cards.Count > 0)
            {
                CardGroup group = new CardGroup();
                int temp = cards[0];
                group.CardId = temp;
                group.CardValue = temp & 0x0f;
                var sortGroup = temp >> 4;
                switch (sortGroup)
                {
                    case 1:
                        group.GValue = 1;
                        break;
                    case 2:
                        group.GValue = 3;
                        break;
                    case 3:
                        group.GValue = 2;
                        break;
                    case 4:
                        group.GValue = 4;
                        break;
                    default:
                        group.GValue = 5;
                        break;
                }

                cards.RemoveAt(0);
                _groupList.Add(group);
            }
        }

        /// <summary>
        /// 按花色排
        /// </summary>
        public void SortCardsColor()
        {
            SotColortBtn.gameObject.SetActive(false);
            SortValueBtn.gameObject.SetActive(true);
            if (MyCardsValueList != null)
            {
                MyCardsValueList.Clear();
            }
            MyCardsValueList = new List<int>();
            _groupList.Sort((a, b) =>
            {
                if (a.GValue == b.GValue)
                {
                    return b.CardId - a.CardId;
                }
                else
                {
                    return b.GValue - a.GValue;
                }
            });
            foreach (CardGroup t in _groupList)
            {
                MyCardsValueList.Add(t.CardId);
            }

            SetCardsValue();
            //            Debug.LogError("按花色排");
            //            YxDebug.LogArray(MyCardsValueList);
        }
        /// <summary>
        /// 按值排
        /// </summary>
        public void SortCardsValue()
        {
            SotColortBtn.gameObject.SetActive(true);
            SortValueBtn.gameObject.SetActive(false);
            if (MyCardsValueList != null)
            {
                MyCardsValueList.Clear();
            }
            MyCardsValueList = new List<int>();
            _groupList.Sort((a, b) =>
            {
                if (GameHelp.IsLaizi(a.CardId) == GameHelp.IsLaizi(b.CardId))
                {
                    if (a.CardValue == b.CardValue)
                    {
                        return b.GValue - a.GValue;
                    }
                    else
                    {
                        return b.CardValue - a.CardValue;
                    }
                }
                else
                {
                    return GameHelp.IsLaizi(b.CardId) - GameHelp.IsLaizi(a.CardId);
                }

            });

            foreach (CardGroup t in _groupList)
            {
                MyCardsValueList.Add(t.CardId);
            }
            SetCardsValue();
            //            Debug.LogError("按值排");
            //            YxDebug.LogArray(MyCardsValueList);
        }

        public void SetTime(int time)
        {
            _time = time;
            CountDown.gameObject.SetActive(true);
            InvokeRepeating("TimeChange", 0, 1);
        }

        private void TimeChange()
        {
            _time--;
            CountDown.text = _time.ToString();
            if (_time <= 0)
            {
                CountDown.gameObject.SetActive(false);
                CancelInvoke("TimeChange");
            }
        }

        public void OnCardMoveDown(BjCardItem cardItem)
        {
            if (cardItem.IsTop)
            {
                cardItem.GetComponentInParent<BjLoadItem>().OnDeleteBtn(SettleCardsGrid, MyCardsValueList, SettleCardsList, cardItem);

                SetCardsValue(MyCardsValueList.ToArray());

                if (SotColortBtn.gameObject.activeSelf)
                {
                    SortCardsValue();
                }
                else
                {
                    SortCardsColor();
                }

                GetAllCardType();
            }
        }
        /// <summary>
        /// 点击选择每道的牌
        /// </summary>
        /// <param name="bjLoadItem"></param>
        public void OnLoadClick(BjLoadItem bjLoadItem)
        {
            List<BjCardItem> cards = new List<BjCardItem>();
            List<int> cardsValue = new List<int>();
            if (bjLoadItem.Cards.Count == 3) return;
            for (int i = 0; i < SettleCardsList.Count; i++)
            {
                if (SettleCardsList[i].MoveFlag)
                {
                    if (cards.Count == 3 || bjLoadItem.Cards.Count + cards.Count == 3)
                    {
                        SettleCardsList[i].MoveFlag = false;
                        continue;
                    }
                    cards.Add(SettleCardsList[i]);
                    cardsValue.Add(SettleCardsList[i].Value);
                }
            }

            bjLoadItem.AddCards(cards, GetLoadCardType(cardsValue));
            if (cards.Count != 3)
            {
                bjLoadItem.CardType = GetLoadCardType(bjLoadItem.CardsValue);
            }

            RemoveCardItem(cards);
            SettleCardsGrid.repositionNow = true;

            if (SettleCardsList.Count == 3)
            {
                for (int j = 0; j < SettleCardsList.Count; j++)
                {
                    SettleCardsList[j].MoveFlag = true;
                }

                if (AcrossBg.activeSelf)
                {
                    for (int i = 0; i < AcrossLoads.Count; i++)
                    {
                        if (AcrossLoads[i].CardsValue.Count == 0)
                        {
                            OnLoadClick(AcrossLoads[i]);
                        }
                        else if (AcrossLoads[i].CardsValue.Count == 2)
                        {
                            for (int j = 0; j < SettleCardsList.Count; j++)
                            {
                                SettleCardsList[j].MoveFlag = false;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < VerticalLoads.Count; i++)
                    {
                        if (VerticalLoads[i].CardsValue.Count == 0)
                        {
                            OnLoadClick(VerticalLoads[i]);
                        }
                        else if (VerticalLoads[i].CardsValue.Count == 2)
                        {
                            for (int j = 0; j < SettleCardsList.Count; j++)
                            {
                                SettleCardsList[j].MoveFlag = false;
                            }
                        }
                    }
                }
            }
            else if (SettleCardsList.Count == 0)
            {
                if (AcrossBg.activeSelf)
                {
                    if (_xiPai)
                    {
                        GetXiPai(AcrossLoads);
                    }

                    if (AcrossLoads[2].CompareCard(AcrossLoads[1]))
                    {
                        EventObj.SendEvent("TipEvent", "Remind", "出现相公,系统自动变道");
                    }
                    if (AcrossLoads[1].CompareCard(AcrossLoads[0]))
                    {
                        EventObj.SendEvent("TipEvent", "Remind", "出现相公,系统自动变道");
                    }
                    if (AcrossLoads[2].CompareCard(AcrossLoads[1]))
                    {
                        EventObj.SendEvent("TipEvent", "Remind", "出现相公,系统自动变道");
                    }

//                    if (_xiPai)
//                    {
                        ChangeLaiZi(AcrossLoads);
//                    }
                }
                else
                {
                    if (_xiPai)
                    {
                        GetXiPai(VerticalLoads);
                    }
                    if (VerticalLoads[2].CompareCard(VerticalLoads[1]))
                    {
                        EventObj.SendEvent("TipEvent", "Remind", "出现相公,系统自动变道");
                    }
                    if (VerticalLoads[1].CompareCard(VerticalLoads[0]))
                    {
                        EventObj.SendEvent("TipEvent", "Remind", "出现相公,系统自动变道");
                    }
                    if (VerticalLoads[2].CompareCard(VerticalLoads[1]))
                    {
                        EventObj.SendEvent("TipEvent", "Remind", "出现相公,系统自动变道");
                    }

//                    if (_xiPai)
//                    {
                        ChangeLaiZi(VerticalLoads);
//                    }
                }
            }
            GetAllCardType();
        }

        private void GetXiPai(List<BjLoadItem> loadItems)
        {
            var loadTypeDic = new Dictionary<CardType, int>();
            var laiZiIndex = new List<int>();
            for (int i = loadItems.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < loadItems[i].CardsPoekerValue.Count; j++)
                {
                    if (loadItems[i].CardsPoekerValue[j] == 81 || loadItems[i].CardsPoekerValue[j] == 97)
                    {
                        laiZiIndex.Add(i);
                    }
                }

                if (loadTypeDic.ContainsKey(loadItems[i].CardType))
                {
                    loadTypeDic[loadItems[i].CardType]++;
                }
                else
                {
                    loadTypeDic.Add(loadItems[i].CardType, 1);
                }
            }
            if (_xiPaiTypes.Count == 0) return;

            if (_xiPaiTypes.Contains(XipaiType.qinglianshun))
            {
                if (loadTypeDic.ContainsKey(CardType.TongHuaShun))
                {
                    for (int j = 0; j < laiZiIndex.Count; j++)
                    {
                        if (_gameHelp.GetTongHuaShun(loadItems[laiZiIndex[j]].CardsValue).Count > 0)
                        {
                            loadItems[laiZiIndex[j]].CardType = CardType.TongHuaShun;
                        }
                    }
                }

            }

            if (_xiPaiTypes.Contains(XipaiType.lianshun))
            {
                if (_xiPaiTypes.Contains(XipaiType.shunqing3))
                {
                    if (loadTypeDic.ContainsKey(CardType.TongHuaShun))
                    {
                        for (int j = 0; j < laiZiIndex.Count; j++)
                        {
                            if (_gameHelp.GetTongHuaShun(loadItems[laiZiIndex[j]].CardsValue).Count > 0)
                            {
                                loadItems[laiZiIndex[j]].CardType = CardType.TongHuaShun;
                            }
                        }
                    }

                }
                else if (_xiPaiTypes.Contains(XipaiType.shunqing2))
                {
                    if (loadTypeDic.ContainsKey(CardType.TongHuaShun)|| loadTypeDic.ContainsKey(CardType.ShunZi))
                    {
                        for (int j = 0; j < laiZiIndex.Count; j++)
                        {
                            if (_gameHelp.GetTongHuaShun(loadItems[laiZiIndex[j]].CardsValue).Count > 0)
                            {
                                loadItems[laiZiIndex[j]].CardType = CardType.TongHuaShun;
                            }
                            else
                            {
                                if (_gameHelp.GetShunZi(loadItems[laiZiIndex[j]].CardsValue).Count > 0)
                                {
                                    loadItems[laiZiIndex[j]].CardType = CardType.ShunZi;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (loadTypeDic.ContainsKey(CardType.ShunZi))
                    {
                        for (int j = 0; j < laiZiIndex.Count; j++)
                        {
                            if (_gameHelp.GetShunZi(loadItems[laiZiIndex[j]].CardsValue).Count > 0)
                            {
                                loadItems[laiZiIndex[j]].CardType = CardType.ShunZi;
                            }
                        }
                    }
                }

            }

            if (_xiPaiTypes.Contains(XipaiType.shunqing2))
            {
                if (loadTypeDic.ContainsKey(CardType.TongHuaShun))
                {
                    for (int j = 0; j < laiZiIndex.Count; j++)
                    {
                        if (_gameHelp.GetTongHuaShun(loadItems[laiZiIndex[j]].CardsValue).Count > 0)
                        {
                            loadItems[laiZiIndex[j]].CardType = CardType.TongHuaShun;
                        }
                    }
                }
            }
            if (_xiPaiTypes.Contains(XipaiType.shunqing3) || _xiPaiTypes.Contains(XipaiType.tonghuan))
            {
                if (loadTypeDic.ContainsKey(CardType.TongHuaShun)|| loadTypeDic.ContainsKey(CardType.TongHua))
                {
                    for (int j = 0; j < laiZiIndex.Count; j++)
                    {
                        if (_gameHelp.GetTongHuaShun(loadItems[laiZiIndex[j]].CardsValue).Count > 0)
                        {
                            loadItems[laiZiIndex[j]].CardType = CardType.TongHuaShun;
                        }
                        else
                        {
                            if (_gameHelp.GetTongHua(loadItems[laiZiIndex[j]].CardsValue).Count > 0)
                            {
                                loadItems[laiZiIndex[j]].CardType = CardType.TongHua;
                            }
                        }
                    }
                }
            }
        }

        private void ChangeLaiZi(List<BjLoadItem> loadItems)
        {
            for (int i = 0; i < loadItems.Count; i++)
            {
                var min = false;
                if (_xiPaiTypes.Contains(XipaiType.lianshun) || _xiPaiTypes.Contains(XipaiType.qinglianshun))
                {
                    var specialNum = 0;
                    for (int j = 0; j < loadItems.Count; j++)
                    {
                        if (loadItems[i].CardType == CardType.ShunZi || loadItems[i].CardType == CardType.TongHuaShun)
                        {
                            specialNum++;
                        }
                    }

                    if (specialNum == 3)
                    {
                        for (int j = 8 - (i * 3); j > 8 - (i * 3) - 3; j--)
                        {
                            if (_lianShun[0][8 - (i * 3)] == 0)
                            {
                                min = true;
                            }
                        }
                    }
                }

                bool quanHong = _xiPaiTypes.Contains(XipaiType.quanhong);
                var noLaiZi = _gameHelp.ReplaceLaizi(loadItems[i].CardsPoekerValue, loadItems[i].CardType, min, quanHong);
                var cards=new List<int>(loadItems[i].CardsValue);
                for (int j = 0; j < noLaiZi.Count; j++)
                {
                    if (cards.Contains(noLaiZi[j]))
                    {
                        cards.Remove(noLaiZi[j]);
                        loadItems[i].Cards[j].Value = noLaiZi[j];
                    }
                    else
                    {
                        loadItems[i].Cards[j].Value = noLaiZi[j];

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
                        loadItems[i].Cards[j].KingName(king);
                    }
                }
            }
        }

        private void RemoveCardItem(List<BjCardItem> bjCardItem)
        {
            for (int i = 0; i < bjCardItem.Count; i++)
            {
                SettleCardsList.Remove(bjCardItem[i]);
                MyCardsValueList.Remove(bjCardItem[i].Value);
            }
            SetCardsValue(MyCardsValueList.ToArray());
        }

        public void OnDeleteBtn(BjLoadItem bjLoadItem)
        {
            bjLoadItem.OnDeleteBtn(SettleCardsGrid, MyCardsValueList, SettleCardsList);
            SetCardsValue(MyCardsValueList.ToArray());

            if (SotColortBtn.gameObject.activeSelf)
            {
                SortCardsValue();
            }
            else
            {
                SortCardsColor();
            }

            GetAllCardType();
        }


        public void ButtonState(int state)
        {
            for (int i = 0; i < ButtonList.Count; i++)
            {
                var isShow = (state & (1 << (i + 2))) != 0;
                ButtonList[i].GetComponent<BoxCollider>().enabled = isShow;
                ButtonList[i].color = isShow ? Color.white : Color.gray;
            }
        }

        public void Reset()
        {

            ResetIndex();
            if (AcrossBg.activeSelf)
            {
                for (int i = 0; i < AcrossLoads.Count; i++)
                {
                    AcrossLoads[i].OnDeleteBtn(SettleCardsGrid, MyCardsValueList, SettleCardsList);
                }

            }
            if (VerticalBg.activeSelf)
            {
                for (int i = 0; i < VerticalLoads.Count; i++)
                {
                    VerticalLoads[i].OnDeleteBtn(SettleCardsGrid, MyCardsValueList, SettleCardsList);
                }

            }

            SettleCardsGrid.repositionNow = true;
            SettleCardsGrid.Reposition();
        }

        public void OnGiveUpBtn()
        {
            YxMessageBox.Show(
                "投降后,只输每局最大的积分,不需要支付别人的喜牌奖励，是否确定投降?",
                null,
                (window, btnname) =>
                {
                    switch (btnname)
                    {
                        case YxMessageBox.BtnLeft:
                            EventObj.SendEvent("ServerEvent", "GiveUp", null);
                            Reset();
                            View.SetActive(false);
                            break;
                    }
                },
                true,
                YxMessageBox.LeftBtnStyle | YxMessageBox.RightBtnStyle
            );

        }

        #region 逻辑处理

        public void GetAllCardType()
        {
            ButtonState(CheckHasType());
        }

        public CardType GetLoadCardType(List<int> loadCard)
        {
            var type = _gameHelp.CheckType(loadCard);
            //            Debug.LogError("牌类型" + type);
            return type;
        }

        private int _duiZiIndex;
        private int _shunZiIndex;
        private int _tongHuaIndex;
        private int _tongHuaShunIndex;
        private int _sanTiaoIndex;

        private void ResetIndex()
        {
            //            _index = 0;
            _duiZiIndex = 0;
            _shunZiIndex = 0;
            _tongHuaIndex = 0;
            _tongHuaShunIndex = 0;
            _sanTiaoIndex = 0;
        }

        private void ResetTableCards()
        {
            for (int i = 0; i < SettleCardsList.Count; i++)
            {
                SettleCardsList[i].MoveFlag = true;
                SettleCardsList[i].MoveUp();
            }
        }

        public void OnAutoMatch()
        {
            if (SettleCardsList.Count == 0) return;//|| !_clickAuto
            Reset();
            var autoIndex = 2;
            var i = 5;
            for (; i >= 0; i--)
            {
                var type = CheckHasType();

                var typeCards = GetTypeCards(i);

                if ((type & (1 << (i + 1))) != 0)
                {
                    List<BjCardItem> cards = new List<BjCardItem>();
                    List<int> cardsValue = new List<int>();
                    if (typeCards.Count >= 1)
                    {
                        i = 6;

                        for (int k = 0; k < typeCards[0].Count; k++)
                        {
                            for (int j = 0; j < SettleCardsList.Count; j++)
                            {
                                if (SettleCardsList[j].Value == typeCards[0][k])
                                {
                                    cards.Add(SettleCardsList[j]);
                                    cardsValue.Add(SettleCardsList[j].Value);
                                }
                            }
                        }
                    }
                    if (AcrossBg.activeSelf)
                    {
                        AcrossLoads[autoIndex].AddCards(cards, GetLoadCardType(cardsValue));
                    }
                    else
                    {
                        VerticalLoads[autoIndex].AddCards(cards, GetLoadCardType(cardsValue));
                    }

                    RemoveCardItem(cards);
                    SettleCardsGrid.repositionNow = true;
                    autoIndex--;
                    if (autoIndex < 0)
                    {
                        if (AcrossBg.activeSelf)
                        {
                            AcrossLoads[2].CompareCard(AcrossLoads[1]);

                            AcrossLoads[1].CompareCard(AcrossLoads[0]);

                            AcrossLoads[2].CompareCard(AcrossLoads[1]);

                            ChangeLaiZi(AcrossLoads);
                        }
                        else
                        {
                            VerticalLoads[2].CompareCard(VerticalLoads[1]);

                            VerticalLoads[1].CompareCard(VerticalLoads[0]);

                            VerticalLoads[2].CompareCard(VerticalLoads[1]);

                            ChangeLaiZi(VerticalLoads);
                        }
                     
                        ButtonState(0);
                        return;
                    }
                }
            }
        }

        public int CheckHasType()
        {
            CardType type = CardType.SanPai;
            var sanTiaoCards = _gameHelp.GetSanTiao(MyCardsValueList);
            if (sanTiaoCards.Count > 0)
            {
                type |= CardType.SanTiao;
            }
            var tongHuaShunCards = _gameHelp.GetTongHuaShun(MyCardsValueList);
            if (tongHuaShunCards.Count > 0)
            {
                type |= CardType.TongHuaShun;
            }
            var tongHuaCards = _gameHelp.GetTongHua(MyCardsValueList);
            if (tongHuaCards.Count > 0)
            {
                type |= CardType.TongHua;
            }
            var shunZiCards = _gameHelp.GetShunZi(MyCardsValueList);
            if (shunZiCards.Count > 0)
            {
                type |= CardType.ShunZi;
            }
            var duiZiCards = _gameHelp.GetDuiZi(MyCardsValueList);
            if (duiZiCards.Count > 0)
            {
                type |= CardType.DuiZi;
            }
            return (int)type;

        }
        private List<List<int>> GetTypeCards(int type)
        {
            List<List<int>> typeCards = new List<List<int>>();
            switch (type)
            {
                case 5:
                    typeCards = _gameHelp.GetSanTiao(MyCardsValueList);
                    break;
                case 4:
                    typeCards = _gameHelp.GetTongHuaShun(MyCardsValueList);
                    break;
                case 3:
                    typeCards = _gameHelp.GetTongHua(MyCardsValueList);
                    break;
                case 2:
                    typeCards = _gameHelp.GetShunZi(MyCardsValueList);
                    break;
                case 1:
                    typeCards = _gameHelp.GetDuiZi(MyCardsValueList);
                    break;
                case 0:
                    typeCards = _gameHelp.GetSanPai(MyCardsValueList);
                    break;
            }
            return typeCards;
        }


        public void OnDuiZiBtn()
        {
            ResetTableCards();

            var duiZiList = _gameHelp.GetDuiZi(MyCardsValueList);

            if (_duiZiIndex >= duiZiList.Count)
            {
                _duiZiIndex = 0;
            }

            for (int j = 0; j < SettleCardsList.Count; j++)
            {
                for (int i = 0; i < duiZiList[_duiZiIndex].Count; i++)
                {
                    if (SettleCardsList[j].Value == duiZiList[_duiZiIndex][i])
                    {
                        SettleCardsList[j].MoveUp();
                    }
                }
            }
            _duiZiIndex++;
        }
        public void OnShunZiBtn()
        {
            ResetTableCards();
            var shunZiList = _gameHelp.GetShunZi(MyCardsValueList);
            if (_shunZiIndex >= shunZiList.Count)
            {
                _shunZiIndex = 0;
            }

            for (int j = 0; j < SettleCardsList.Count; j++)
            {
                for (int i = 0; i < shunZiList[_shunZiIndex].Count; i++)
                {
                    if (SettleCardsList[j].Value == shunZiList[_shunZiIndex][i])
                    {
                        SettleCardsList[j].MoveUp();
                    }
                }
            }
            _shunZiIndex++;
        }

        public void OnTongHuaBtn()
        {
            ResetTableCards();
            var tongHuaList = _gameHelp.GetTongHua(MyCardsValueList);
            if (_tongHuaIndex >= tongHuaList.Count)
            {
                _tongHuaIndex = 0;
            }
            for (int j = 0; j < SettleCardsList.Count; j++)
            {
                for (int i = 0; i < tongHuaList[_tongHuaIndex].Count; i++)
                {
                    if (SettleCardsList[j].Value == tongHuaList[_tongHuaIndex][i])
                    {
                        SettleCardsList[j].MoveUp();
                    }
                }
            }
            _tongHuaIndex++;
        }
        public void OnTongHuaShunBtn()
        {
            ResetTableCards();
            var tongHuaShunList = _gameHelp.GetTongHuaShun(MyCardsValueList);
            if (_tongHuaShunIndex >= tongHuaShunList.Count)
            {
                _tongHuaShunIndex = 0;
            }

            for (int j = 0; j < SettleCardsList.Count; j++)
            {
                for (int i = 0; i < tongHuaShunList[_tongHuaShunIndex].Count; i++)
                {
                    if (SettleCardsList[j].Value == tongHuaShunList[_tongHuaShunIndex][i])
                    {
                        SettleCardsList[j].MoveUp();
                    }
                }
            }
            _tongHuaShunIndex++;
        }
        public void OnSanTiaonBtn()
        {
            ResetTableCards();

            var sanTiaoList = _gameHelp.GetSanTiao(MyCardsValueList);

            if (_sanTiaoIndex >= sanTiaoList.Count)
            {
                _sanTiaoIndex = 0;
            }
            for (int j = 0; j < SettleCardsList.Count; j++)
            {
                for (int i = 0; i < sanTiaoList[_sanTiaoIndex].Count; i++)
                {
                    if (SettleCardsList[j].Value == sanTiaoList[_sanTiaoIndex][i])
                    {
                        SettleCardsList[j].MoveUp();
                    }
                }
            }
            _sanTiaoIndex++;

        }


        #endregion 
    }
    public class CardGroup
    {
        /// <summary>
        /// 该组的大小，仅在排序中使用此值
        /// </summary>
        public int GValue;

        public int CardId;

        public int CardValue;

    }


    [Flags]
    public enum CardType
    {
        None = 0,
        SanPai = 1 << 1,
        DuiZi = 1 << 2,
        ShunZi = 1 << 3,
        TongHua = 1 << 4,
        TongHuaShun = 1 << 5,
        SanTiao = 1 << 6,
    }

}
