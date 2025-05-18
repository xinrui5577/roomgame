using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.sssjp.ImgPress.Main;
using Assets.Scripts.Game.sssjp.Tool;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.View;
using CnList = Assets.Scripts.Game.sssjp.HelpLz.CnList;
using VnList = Assets.Scripts.Game.sssjp.HelpLz.VnList;
using SssDun = Assets.Scripts.Game.sssjp.HelpLz.SssDun;

#pragma warning disable 649

namespace Assets.Scripts.Game.sssjp
{
    public class ChoiseWay1 : ChoiseMgr
    {
        #region     测试部分方法
        //public override void Test(int[] cardArray)
        //{
        //    Init();
        //    _cardValList = cardArray.ToList();
        //    HelpLz.SortList(_cardValList);

        //    YxDebug.Log(" =============== cards Array ========================== ");
        //    YxDebug.LogArray(_cardValList);
        //    YxDebug.Log(" ====================================================== ");

        //    HelpLz help = new HelpLz();
        //    List<HelpLz.PlayerDuns> pd = help.getPlayerDuns(_cardValList);

        //    //初始化手牌,待删
        //    for (int i = 0; i < CardsList.Count; i++)
        //    {
        //        CardsList[i].SetCardId(_cardValList[i]);
        //        CardsList[i].SetCardFront();
        //    }

        //    RespositionCards();//初始化手牌位置

        //    if (SpecialBtn != null)
        //    {
        //        if (pd.Count > 0 && pd[0].SpecialType > CardType.none)
        //        {
        //            _specialDuns = pd[0];
        //            SpecialBtn.gameObject.SetActive(true);
        //        }
        //        else
        //        {
        //            _specialDuns = null;
        //            SpecialBtn.gameObject.SetActive(false);
        //        }
        //    }

        //    SetChoiseView(_cardValList);
        //}
        #endregion

        public UIButton SpecialBtn;

        public GameObject[] TypeBtns;

        public ChoiseLine1[] Lines;

        public GameObject FinishView;

        public GameObject TypeBtnsView;

        /// <summary>
        /// 是否完成了选牌
        /// </summary>
        private bool _isFinish;


        /// <summary>
        /// 记录牌型字典
        /// </summary>
        private readonly Dictionary<CardType, List<SssDun>> _cardTypeDic = new Dictionary<CardType, List<SssDun>>();


        private List<int> _cardValList;

        /// <summary>
        /// 特殊牌型按钮
        /// </summary>
        private HelpLz.PlayerDuns _specialDuns;


        public override void Init()
        {
            InitPokerList();
            InitAutoSelectBtn();
            InitLines();
        }


        /// <summary>
        /// 检测扑克个数
        /// </summary>
        protected override void InitPokerList()
        {
            if (CardsList.Count == CardCount)
                return;

            if (CardsList.Count > CardCount)
            {
                //删除多余的牌
                int count = CardsList.Count - CardCount;
                for (int i = 0; i < count; i++)
                {
                    var temp = CardsList[i];
                    CardsList.Remove(temp);
                    Destroy(temp);
                }
            }
            else
            {
                int count = CardCount - CardsList.Count;
                for (int i = 0; i < count; i++)
                {
                    PokerCard clone = Instantiate(CardPrefab);
                    clone.transform.parent = PokerParent;
                    clone.name = "poker " + i;
                    clone.transform.localScale = Vector3.one * .6f;
                    clone.GetComponent<PokerCard>().SetCardDepth(130 + i * 3);

                    CardsList.Add(clone);
                    AddPokerEvent(clone);
                }
            }
        }

        public void AddPokerEvent(PokerCard poker)
        {
            AddPokerTriggerOnClick(this, poker, "OnClickCard");
            AddPokerTriggerOnDragOver(this, poker, "OnDragOverCard");
        }


        public void AddPokerTriggerOnDragOver(MonoBehaviour target, PokerCard poker, string mothName)
        {
            var trigger = GetUiEventTrigger(poker);
            var ed = GetEventDelegate(target, poker, mothName);
            EventDelegate.Add(trigger.onDragOver, ed);
        }


        /// <summary>
        /// 初始化每道
        /// </summary>
        private void InitLines()
        {
            int len = Lines.Length;

            for (int i = 0; i < len; i++)
            {
                var line = Lines[i];
                var btn = line.SelectBtn;

                //初始化选择按钮点击按钮事件
                if (btn != null)
                {
                    CreateEventDelegate(this, "OnClickSelectBtn", btn, i);
                }

                //初始化取消按钮点击按钮事件
                btn = line.ReselectBtn;
                if (btn != null)
                {
                    CreateEventDelegate(this, "OnClickReselectBtn", btn, i);
                }

                int count = i == 0 ? 3 : 5;
                if (line.CardList.Count == count) continue;     //已经有这么多牌了 无需创建牌
                for (int j = 0; j < count; j++)
                {
                    PokerCard poker = Instantiate(CardPrefab);
                    AddPokerTriggerOnClick(this, poker, "OnClickLinePoker");
                    line.SetOneCard(poker);
                }
                line.ParentReposition();
            }
        }

        /// <summary>
        /// 代理添加事件,当点击每道的牌时
        /// </summary>
        /// <param name="card"></param>
        void OnClickLinePoker(PokerCard card)
        {
            if (!_isFinish) return;
            ExchangeCard(card);
        }


        void CreateEventDelegate(MonoBehaviour target, string methodName, UIButton btn, System.Object obj = null)
        {
            var list = new List<EventDelegate>();

            var ed = new EventDelegate(target, methodName);
            if (obj != null)
            {
                ed.parameters[0] = new EventDelegate.Parameter(obj);
                EventDelegate.Add(btn.onClick, ed);
            }
            list.Add(ed);
            btn.onClick = list;
        }


        /// <summary>
        /// 校验选择信息
        /// </summary>
        /// <param name="index"></param>
        protected void OnClickSelectBtn(int index)
        {

            var cards = _selectInfo.SelectedCards;

            int count = index == 0 ? 3 : 5;
            if (cards.Count != count)
            {
                MessageShow(string.Format("您选择的牌,不是{0}张!!", count));
                return;
            }

            var dunCardList = _selectInfo.Dun.Cards;
            Resort(dunCardList);
            _selectInfo = GetLineInfo(dunCardList, index);


            //隐藏掉已选择的牌
            for (int i = 0; i < count; i++)
            {
                var card = cards[i];
                cards[i].gameObject.SetActive(false);
                int id = card.Id;
                //移除已选择的牌
                if (_cardValList.Contains(id))
                    _cardValList.Remove(card.Id);
            }

            RespositionCards();
            NoSelected(); //清除选中的牌
            _selectInfo.Reset();

            SetChoiseView(_cardValList);

            string errorMsg = string.Empty;
            if (CheckIsDaoShui(ref errorMsg))
            {
                MessageShow(errorMsg);
                return;
            }

            count = _cardValList.Count;
            //自动补全第三道
            if (count < 6 && count > 0)
            {
                int len = Lines.Length;
                int lineIndex = -1;
                for (int i = 0; i < len; i++)
                {
                    var lineDun = Lines[i].LineInfo.Dun;
                    if (lineDun == null || lineDun.Cards.Count < 1)
                    {
                        lineIndex = i;
                        break;
                    }
                }
                if (lineIndex >= 0)
                {
                    _selectInfo.Dun.Cards = new List<int>(_cardValList);
                    AutoSelectToList();
                    OnClickSelectBtn(lineIndex);
                }
            }
        }


        public override void OnDragOverCard(PokerCard card)
        {
            if (_isFinish) return;
            OnClickCard(card);
        }



        void OnClickReselectBtn(int index)
        {
            var line = Lines[index];
            var lineSelectInfo = line.LineInfo;
            if (lineSelectInfo == null) return;
            var cardsList = lineSelectInfo.SelectedCards;
            if (cardsList == null) return;
            int count = cardsList.Count;


            for (int i = 0; i < count; i++)
            {
                var card = cardsList[i];
                card.gameObject.SetActive(true);
                card.IsSelect = false;
                _cardValList.Add(card.Id);
            }

            line.Reselect();
            RespositionCards();
            NoSelected();     //清除选中的牌
            SetChoiseView(_cardValList);
        }


        void RespositionCards()
        {
            PokerParent.GetComponent<UIGrid>().Reposition();
        }

      

        public override void ShowChoiseView(ISFSObject cardData)
        {
            base.ShowChoiseView(cardData);

            InitPokerList();

            gameObject.SetActive(true);

            int[] cardArray = cardData.GetIntArray("cards");

            _cardValList = cardArray.ToList();
            HelpLz.SortList(_cardValList);

//            HelpLz help = new HelpLz();
//            List<HelpLz.PlayerDuns> pd = help.getPlayerDuns(_cardValList);

            //初始化手牌,待删
            for (int i = 0; i < CardsList.Count; i++)
            {
                CardsList[i].SetCardId(_cardValList[i]);
                CardsList[i].SetCardFront();
            }

            RespositionCards();//初始化手牌位置
//
//            if (SpecialBtn != null)
//            {
//                if (pd.Count > 0 && pd[0].SpecialType > CardType.none)
//                {
//                    _specialDuns = pd[0];
//                    SpecialBtn.gameObject.SetActive(true);
//                }
//                else
//                {
//                    _specialDuns = null;
//                    SpecialBtn.gameObject.SetActive(false);
//                }
//            }

            SetChoiseView(_cardValList);
        }



        /// <summary>
        /// 显示选牌界面
        /// </summary>
        /// <param name="cardValList"></param>
        private void SetChoiseView(List<int> cardValList)
        {
            if (cardValList == null) return;
            int count = cardValList.Count;

            if (SpecialBtn != null)
                SpecialBtn.gameObject.SetActive(count >= 13 && _specialDuns != null);

            int btnState = GetBtnState(cardValList);
            ShowTypeBtns(btnState);

            _isFinish = count < 3;

            FinishView.SetActive(_isFinish);
            TypeBtnsView.SetActive(!_isFinish);
        }




        /// <summary>
        /// 检测牌型
        /// </summary>
        /// <param name="cards">牌</param>
        /// <param name="line">第几行,不为0时,排数是五张</param>
        /// <returns></returns>
        public CardType CheckCardType(List<int> cards, int line = 2)
        {
            //除去可能产生的不必要的信息
            cards = cards.GetRange(0, line == 0 ? 3 : 5);

            //检测牌型
            bool isTongHua = line != 0;
            bool isShunZi = line != 0;
            int samecount = 1;
            List<int> sameNumList = new List<int>();
            Resort(cards);
            int color = HelpLz.GetColor(cards[0]);
            int value = HelpLz.GetValue(cards[0]);
            for (int i = 1; i < cards.Count; i++)
            {
                int card = cards[i];
                if (card == 0)
                    continue;
                int cardColor = HelpLz.GetColor(card);
                int cardValue = HelpLz.GetValue(card);
                if (cardColor != color)
                {
                    isTongHua = false;
                }
                if (cardValue - value != 1)
                {
                    isShunZi = false;
                }

                if (cardValue == value)
                {
                    samecount++;
                }
                else
                {
                    if (samecount > 0)
                    {
                        sameNumList.Add(samecount);
                        samecount = 1;
                    }
                }

                value = cardValue;
            }
            sameNumList.Add(samecount);
            if (line == 0)
            {
                if (sameNumList.Count == 2)
                    return CardType.yidui;
                if (sameNumList.Count == 1)
                    return CardType.santiao;

                return CardType.sanpai;
            }


            if (isTongHua && isShunZi)
            {
                return CardType.tonghuashun;
            }
            if (isShunZi)
            {
                return CardType.shunzi;
            }
            if (isTongHua)
            {
                return CardType.tonghua;
            }
            if (sameNumList.Count > 0)
            {
                if (sameNumList.Count == 1)
                {
                    samecount = sameNumList[0];
                    if (samecount == 2)
                    {
                        return CardType.yidui;
                    }
                    else if (samecount == 3)
                    {
                        return CardType.santiao;
                    }
                    else if (samecount == 4)
                    {
                        return CardType.tiezhi;
                    }
                }
                else if (sameNumList.Count == 2)
                {
                    samecount = sameNumList[0];
                    int samecount1 = sameNumList[1];
                    if ((samecount == 2 && samecount1 == 3) || (samecount == 3 && samecount1 == 2))
                    {
                        return CardType.hulu;
                    }
                    else if (samecount == 4 || samecount1 == 4)
                    {
                        return CardType.tiezhi;
                    }
                }
                else if (sameNumList.Count == 3)
                {
                    if (sameNumList.Any(item => item == 3))
                    {
                        return CardType.santiao;
                    }
                    return CardType.liangdui;
                }
                else if (sameNumList.Count == 4)
                    return CardType.yidui;
            }

            return CardType.sanpai;
        }


        /// <summary>
        /// 根据牌面值对列表进行排序
        /// </summary>
        /// <param name="list">要排序的牌列表</param>
        /// <param name="up">是否是升序排列</param>
        void Resort(List<int> list, bool up = true)
        {
            HelpLz.SortList(list, up);
        }


        ///<summary>
        ///初始化按键监听
        ///</summary>
        public void InitAutoSelectBtn()
        {
            foreach (CardType btnid in Enum.GetValues(typeof(CardType)))
            {
                foreach (GameObject btn in TypeBtns)
                {
                    if (btn.name.Equals(btnid.ToString()))
                    {
                        Tools.NguiAddOnClick(btn, OnClickListener, (int)btnid);
                    }
                }
            }

            CreateEventDelegate(this, "OnClickSpecialBtn", SpecialBtn);
        }

        /// <summary>
        /// 点击特殊牌型按钮
        /// </summary>
        protected void OnClickSpecialBtn()
        {
            if (_specialDuns == null || _specialDuns.SpecialType <= CardType.none) return;  //如果没有特殊牌型,不予处理

            _selectInfo.CardType = _specialDuns.SpecialType;
            int len = Lines.Length;
            var duns = _specialDuns.Duns;
            for (int i = 0; i < len; i++)
            {
                Lines[i].SetLineCardsVal(duns[i].Cards);
            }

            //隐藏所有牌和选牌按钮
            SetAllPokerCardActive(false);
            TypeBtnsView.SetActive(false);
            FinishView.SetActive(true);
            SetLinesSelectBtn(true);
        }

        /// <summary>
        /// 设置三道的选择按钮
        /// </summary>
        /// <param name="active"></param>
        private void SetLinesSelectBtn(bool active)
        {
            int len = Lines.Length;
            for (int i = 0; i < len; i++)
            {
                Lines[i].SetSelectBtnActive(active);
            }
        }

        /// <summary>
        /// 直接设置所有牌的显示
        /// </summary>
        /// <param name="active"></param>
        void SetAllPokerCardActive(bool active)
        {
            int count = CardsList.Count;
            for (int i = 0; i < count; i++)
            {
                CardsList[i].gameObject.SetActive(active);
            }
        }

        private SelectedInfo _selectInfo = new SelectedInfo();


        protected void OnClickListener(GameObject gob)
        {
            CardType type = (CardType)UIEventListener.Get(gob).parameter;
            switch (type)
            {
                case CardType.yidui:
                case CardType.liangdui:
                case CardType.santiao:
                case CardType.tonghua:
                case CardType.shunzi:
                case CardType.hulu:
                case CardType.tiezhi:
                case CardType.tonghuashun:
                    GetAutoSelect(type);
                    AutoSelectToList();
                    break;
            }
        }



        /// <summary>
        /// 选牌
        /// </summary>
        /// <param name="card"></param>
        /// <param name="isSelect"></param>

        void SelectCard(PokerCard card, bool isSelect)
        {
            card.IsSelect = isSelect;
            if (isSelect)
            {
                MoveYTo(card.transform, 10);
                var selectCards = _selectInfo.SelectedCards;
                if (!selectCards.Contains(card))
                    selectCards.Add(card);

                var dunCards = _selectInfo.Dun.Cards;
                if (!dunCards.Contains(card.Id))
                    dunCards.Add(card.Id);
            }
            else
            {
                MoveYTo(card.transform, 0);
                _selectInfo.SelectedCards.Remove(card);
            }
        }

        /// <summary>
        /// 设置牌的Y轴变化
        /// </summary>
        /// <param name="cardTran">牌</param>
        /// <param name="yPos">Y轴的位置</param>
        void MoveYTo(Transform cardTran, float yPos)
        {
            var v = cardTran.localPosition;
            v = new Vector3(v.x, yPos, v.z);
            cardTran.localPosition = v;
        }


        /// <summary>
        /// 自动选牌
        /// </summary>
        private void AutoSelectToList()
        {
            if (_selectInfo == null)
            {
                _selectInfo = new SelectedInfo();
                return;
            }
            if (_selectInfo.Dun == null)
                return;

            var cardsVal = new List<int>(_selectInfo.Dun.Cards);

            NoSelected();
            var selectCardList = GetSelectCardList(cardsVal);       //获取选中的牌

            for (int i = 0; i < selectCardList.Count; i++)
            {
                var card = selectCardList[i];
                SelectCard(card, true);         //选择选中的牌
            }
        }

        /// <summary>
        /// 通过牌值获取选择的牌
        /// </summary>
        /// <param name="selectValList"></param>
        /// <returns></returns>
        List<PokerCard> GetSelectCardList(List<int> selectValList)
        {
            var selectCardList = new List<PokerCard>();
            int count = selectValList.Count;
            for (int i = 0; i < count; i++)
            {
                var card = CardsList.Find(pok => selectValList[i] == pok.Id);
                if (card == null) continue;
                selectCardList.Add(card);
            }
            return selectCardList;
        }


        /// <summary>
        /// 清除选中的牌
        /// </summary>
        private void NoSelected()
        {
            for (int i = 0; i < CardsList.Count; i++)
            {
                var card = CardsList[i];
                card.IsSelect = false;
                MoveYTo(card.transform, 0);

                _selectInfo.Dun.Cards.Remove(card.Id);
                _selectInfo.SelectedCards.Remove(card);
            }
        }


        /// <summary>
        /// 自动选牌
        /// </summary>
        /// <param name="key"></param>
        private void GetAutoSelect(CardType key)
        {
            if (!_cardTypeDic.ContainsKey(key))
            {
                return;
            }

            CardType type = _selectInfo.CardType;
            int index = type == key ? _selectInfo.Index : -1;
            var list = _cardTypeDic[key];
            int count = list.Count;
            index = (index + 1) % count;

            _selectInfo.Index = index;
            _selectInfo.CardType = key;
            var dun = list[index];
            _selectInfo.Dun = new SssDun
            {
                Cards = new List<int>(dun.Cards),
                CardType = dun.CardType
            };
        }


        /// <summary>
        /// 根据状态位显示牌型按钮状态
        /// </summary>
        /// <param name="state">状态位</param>
        private void ShowTypeBtns(int state)
        {
            if (TypeBtns == null || TypeBtns.Length <= 0)
            {
                return;
            }

            int len = TypeBtns.Length;
            for (int i = 0; i < len; i++)
            {
                TypeBtns[i].SetActive((state & 0x1) > 0);
                state = state >> 1;
            }
        }


        /// <summary>
        /// 获取按钮状态位,同时初始化字典
        /// </summary>
        /// <param name="pokerList"></param>
        /// <returns></returns>
        private int GetBtnState(List<int> pokerList)
        {
            if (pokerList.Count < 3)
                return 0;

            int btnState = 0;


            CnList cnList = new CnList(pokerList);
            VnList vnList = new VnList(pokerList);

            CnList cn5 = cnList.GetMoreThan(5, true, true);     //同花色5个以上
            VnList vn2 = vnList.GetMoreThan(2, true);           //同数值的2个以上

            _cardTypeDic.Clear();

            //同花顺,128
            if (cn5.Count > 0)
            {
                foreach (var cnItem in cn5)
                {
                    if (HaveAndAdd(cnItem.Cards, CardType.tonghuashun, CardType.tonghuashun))
                    {
                        btnState |= GetStateVal(CardType.tonghuashun);
                    }
                }
            }

            //铁支
            VnList vn4 = vnList.GetEqual(4, true);
            if (vn4.Count > 0)
            {
                foreach (var item in vn4)
                {
                    SssDun dun = new SssDun
                    {
                        CardType = CardType.tiezhi,
                        Cards = item.Cards
                    };
                    AddToDic(_cardTypeDic, CardType.tiezhi, dun);
                }
                btnState |= GetStateVal(CardType.tiezhi);
            }

            VnList vn3 = vnList.GetMoreThan(3, true);

            //葫芦
            if (vn3.Count > 0 && vn2.Count > 0)
            {
                foreach (var item3 in vn3)
                {
                    foreach (var item2 in vn2)
                    {
                        if (item3.Val == item2.Val) continue;

                        AddListToDic(CardType.hulu, _cardTypeDic, item3.Cards.GetRange(0, 3), item2.Cards.GetRange(0, 2));

                        btnState |= GetStateVal(CardType.hulu);
                    }
                }
            }

            //同花
            if (cn5.Count > 0)
            {
                btnState = cn5.Where(item => HaveAndAdd(item.Cards, CardType.tonghua))
                    .Aggregate(btnState, (current, item) => current | GetStateVal(CardType.tonghua));
            }

            //顺子
            {
                List<SssDun> dunList = new List<SssDun>();
                HelpLz help = new HelpLz();
                if (help.CheckShunZi(pokerList, dunList, 5, false))
                {
                    foreach (var dun in dunList)
                    {
                        AddToDic(_cardTypeDic, CardType.shunzi, dun);
                    }
                    btnState |= GetStateVal(CardType.shunzi);
                }
            }

            //三条
            if (vn3.Count > 0)
            {
                int count = vn3.Count;
                for (int i = 0; i < count; i++)
                {
                    AddListToDic(CardType.santiao, _cardTypeDic, vn3[i].Cards.GetRange(0, 3));
                }
                btnState |= GetStateVal(CardType.santiao);
            }

            //两对
            if (vn2.Count > 1)
            {
                int count = vn2.Count;
                for (int i = 0; i < count; i++)
                {
                    int vn2Vi = vn2[i].Val;
                    for (int j = i; j < count; j++)
                    {
                        int vn2Vj = vn2[j].Val;
                        if (vn2Vi == vn2Vj) continue;
                        AddListToDic(CardType.liangdui, _cardTypeDic, vn2[i].Cards.GetRange(0, 2),
                            vn2[j].Cards.GetRange(0, 2));
                        btnState |= GetStateVal(CardType.liangdui);
                    }
                }
            }

            //对子
            if (vn2.Count > 0)
            {
                int count = vn2.Count;
                for (int i = 0; i < count; i++)
                {
                    AddListToDic(CardType.yidui, _cardTypeDic, vn2[i].Cards.GetRange(0, 2));
                }
                btnState |= GetStateVal(CardType.yidui);
            }

            return btnState;
        }

        /// <summary>
        /// 查看是否有指定的牌类型,并加入到字典
        /// </summary>
        /// <param name="cards">牌列表,可多余5个</param>
        /// <param name="checkType">验证的牌类型，与addType相同同</param>
        /// <returns></returns>
        private bool HaveAndAdd(List<int> cards, CardType checkType)
        {
            return HaveAndAdd(cards, checkType, checkType);
        }

        /// <summary>
        /// 查看是否有指定的牌类型
        /// </summary>
        /// <param name="cards">牌列表,可多余5个</param>
        /// <param name="checkType">验证的牌类型</param>
        /// <param name="addType">添加字典中的牌型</param>
        /// <returns></returns>
        bool HaveAndAdd(List<int> cards, CardType checkType, CardType addType)
        {
            bool haveType = false;
            int count = cards.Count;
            for (int i = 0; i < count - 4; i++)
            {
                var tempList = cards.GetRange(i, 5);
                if (CheckCardType(tempList) == checkType)
                {
                    AddListToDic(addType, _cardTypeDic, tempList);
                    haveType = true;
                }
            }
            return haveType;
        }

        /// <summary>
        /// 将牌列表添加到数据添加入字典中
        /// </summary>
        /// <param name="keyType">key</param>
        /// <param name="dic">字典</param>
        /// <param name="cards1">第一组牌</param>
        /// <param name="cards2">第二组牌</param>
        void AddListToDic(CardType keyType, Dictionary<CardType, List<SssDun>> dic, List<int> cards1, List<int> cards2 = null)
        {
            var tempList = cards1;

            if (cards2 != null)
            {
                tempList.AddRange(cards2);
            }

            SssDun dun = new SssDun
            {
                CardType = keyType,
                Cards = tempList
            };
            AddToDic(dic, keyType, dun);
        }

        /// <summary>
        /// 添加到字典中
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="dun"></param>
        private void AddToDic(Dictionary<CardType, List<SssDun>> dic, CardType key, SssDun dun)
        {
            if (dic == null)
            {
                dic = new Dictionary<CardType, List<SssDun>>();
            }
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, new List<SssDun>());
            }
            dic[key].Add(dun);
        }





        /// <summary>
        /// 将牌型转化成状态位
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        private int GetStateVal(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.yidui:
                    return 1;
                case CardType.liangdui:
                    return 2;
                case CardType.santiao:
                    return 4;
                case CardType.shunzi:
                    return 8;
                case CardType.tonghua:
                    return 16;
                case CardType.hulu:
                    return 32;
                case CardType.tiezhi:
                    return 64;
                case CardType.tonghuashun:
                    return 128;

                default:
                    return 0;
            }
        }


        /// <summary>
        /// 点击扑克按钮
        /// </summary>
        /// <param name="card"></param>
        public override void OnClickCard(PokerCard card)
        {

            bool selected = !card.IsSelect;
            //反选牌
            SelectCard(card, selected);

            if (!selected)
            {
                _selectInfo.Dun.Cards.Remove(card.Id);
            }
        }

        private PokerCard _selectPok1;

        private PokerCard _selectPok2;


        private void ExchangeCard(PokerCard card)
        {
            if (_selectPok1 == null)
            {
                _selectPok1 = card;
                _selectPok1.SetCardSelected(true);
                return;
            }

            if (_selectPok2 == null)
            {
                if (_selectPok1.Equals(card))
                {
                    ResetSelectedCards();
                    return;
                }
                _selectPok2 = card;
            }

            int cardId1 = _selectPok1.Id;
            int cardId2 = _selectPok2.Id;

            int len = Lines.Length;
            for (int i = 0; i < len; i++)
            {
                var lineInfo = Lines[i].LineInfo;
                var valList = lineInfo.Dun.Cards;
                int count = valList.Count;
                for (int j = 0; j < count; j++)
                {
                    if (valList[j] == cardId1)
                    {
                        valList[j] = cardId2;
                        GetLineInfo(valList, i);
                        continue;
                    }

                    if (valList[j] == cardId2)
                    {
                        valList[j] = cardId1;
                        GetLineInfo(valList, i);
                    }
                }
            }

            ResetSelectedCards();
        }


        /// <summary>
        /// 通过牌和行数,获取牌的信息
        /// </summary>
        /// <param name="cardList">牌组,第0行3张,其它行5张</param>
        /// <param name="lineIndex">第几行索引，最带为3</param>
        /// <returns></returns>
        SelectedInfo GetLineInfo(List<int> cardList, int lineIndex)
        {
            var lineInfo = new SelectedInfo();
            var cardType = CheckCardType(cardList, lineIndex);
            lineInfo.Dun.Cards = cardList;
            lineInfo.Dun.CardType = cardType;
            lineInfo.CardType = cardType;
            lineInfo.SelectedCards = GetSelectCardList(cardList);
            Lines[lineIndex].SelectLine(lineInfo);
            return lineInfo;
        }


        /// <summary>
        /// 获取扑克在数组中的索引
        /// </summary>
        /// <param name="cardList"></param>
        /// <param name="card"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        int GetPokerIndex(List<PokerCard> cardList, PokerCard card, int from)
        {
            int count = cardList.Count;

            for (int i = from; i < count; i++)
            {
                if (cardList[i].Equals(card))
                {
                    return i;
                }
            }
            return 0;
        }


        public void ResetSelectedCards()
        {
            if (_selectPok1 != null)
            {
                _selectPok1.SetCardSelected(false);
                _selectPok1 = null;
            }
            if (_selectPok2 != null)
            {
                _selectPok2.SetCardSelected(false);
                _selectPok2 = null;
            }
        }

        public void OnClickOkBtn()
        {
            ISFSObject obj = new SFSObject();
            ISFSArray arr = new SFSArray();
            if (_selectInfo != null && _selectInfo.CardType > CardType.none)
            {
                obj.PutInt("special", (int)_selectInfo.CardType);
            }
            else
            {
                string errorMsg = string.Empty;
                if (CheckIsDaoShui(ref errorMsg))
                {
                    YxMessageBox.Show(new YxMessageBoxData()
                    {
                        Msg = errorMsg,
                        Delayed = 3,
                    });
                    YxDebug.Log("不能倒水");
                    return;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                var lineInfo = Lines[i].LineInfo;
                ISFSObject arrItem = new SFSObject();
                arrItem.PutInt("type", (int)lineInfo.CardType);
                var tempList = lineInfo.Dun.Cards;
                HelpLz.SortList(tempList, false);
                arrItem.PutIntArray("cards", tempList.ToArray());
                arr.AddSFSObject(arrItem);
            }
            obj.PutSFSArray("duninfo", arr);

            //发送到服务器
            obj.PutInt("type", GameRequestType.FinishChoise);
            App.GetRServer<SssjpGameServer>().SendGameRequest(obj);
        }

        /// <summary>
        /// 点击取消按钮,取消所有选择
        /// </summary>
        public void OnClickCancelAllBtn()
        {
            _isFinish = false;
            SetLinesSelectBtn(_isFinish);
            SetAllPokerCardActive(true);
            for (int i = 0; i < Lines.Length; i++)
            {
                OnClickReselectBtn(i);
            }
        }

        /// <summary>
        /// 查看所有道是否有倒水
        /// </summary>
        /// <returns></returns>
        private bool CheckIsDaoShui(ref string errorMsg)
        {
            int len = Lines.Length;    //比较的次数
            for (int i = 0; i < len; i++)
            {
                int index1 = i;
                int index2 = (i + 1) % len;
                var lineInfo1 = Lines[index1].LineInfo;
                var lineInfo2 = Lines[index2].LineInfo;

                if (MatchListType(lineInfo1.Dun.Cards, index1, lineInfo2.Dun.Cards, index2) < 0)
                {
                    errorMsg = string.Format("第{0},{1}行出现倒水!", Mathf.Min(index1, index2) + 1, Mathf.Max(index1, index2) + 1);
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// 小于0,倒水
        /// </summary>
        /// <param name="list1"></param>
        /// <param name="line1"></param>
        /// <param name="list2"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        int MatchListType(List<int> list1, int line1, List<int> list2, int line2)
        {
            if (list1 == null || list1.Count < 1 || list2 == null || list2.Count < 1)
                return 0;
            var vnList1 = new VnList(list1);
            var vnList2 = new VnList(list2);
            CardType type1 = CheckCardType(list1, line1);
            CardType type2 = CheckCardType(list2, line2);

            YxDebug.Log(string.Format(" === 第{0}行牌型为{1} , 第{2}行牌型为{3} === ", line1, type1, line2, type2));

            int result = type1 - type2;
            int dif = line1 - line2;
            if (result == 0)
            {
                for (int i = 0; i < vnList2.Count; i++)
                {
                    if (vnList1[i].Val != vnList2[i].Val)
                    {
                        return vnList1[i].Val.CompareTo(vnList2[i].Val) * dif;
                    }
                }
            }
            return result * dif;
        }



        void MessageShow(string info)
        {
            YxMessageBox.Show(new YxMessageBoxData
            {
                Msg = info
            });
        }

        public override void Reset()
        {
            base.Reset();
            _isFinish = false;
            _selectInfo.Reset();
            _specialDuns = null;
            if (SpecialBtn != null) SpecialBtn.gameObject.SetActive(false);
            OnClickCancelAllBtn();
        }
    }



    public class SelectedInfo
    {
        public CardType CardType;
        public int Index;
        public SssDun Dun;
        public List<PokerCard> SelectedCards;

        public SelectedInfo()
        {
            CardType = CardType.none;
            Index = -1;
            Dun = new SssDun { Cards = new List<int>() };
            SelectedCards = new List<PokerCard>();
        }

        public void Reset()
        {
            CardType = CardType.none;
            Index = -1;
            if (Dun == null) Dun = new SssDun();
            Dun.Cards.Clear();
            Dun.CardType = CardType.none;
            SelectedCards.Clear();
        }
    }
}