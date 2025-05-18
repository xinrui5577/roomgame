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
using VnItem= Assets.Scripts.Game.sssjp.HelpLz.VnItem;

#pragma warning disable 649

namespace Assets.Scripts.Game.sssjp.skin1
{
    public class ChoiseWay1Sk1 : ChoiseMgr
    {
        #region     测试部分方法
        //public override void Test(int[] cardArray)
        //{
        //    Init();
        //    _cardValList = cardArray.ToList();
        //    HelpLz.SortList(_cardValList, false);

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


        //    if (pd.Count > 0 && pd[0].SpecialType > CardType.none)
        //    {
        //        _specialDuns = pd[0];
        //        if (SpecialBtn != null)
        //        {
        //            SpecialBtn.gameObject.SetActive(true);
        //        }
        //        else
        //        {
        //            OnClickSpecialBtn();
        //            ShowSepcialMark(pd[0].SpecialType);
        //        }
        //        return;
        //    }

        //    _specialDuns = null;
        //    if (SpecialBtn != null)
        //    {
        //        SpecialBtn.gameObject.SetActive(false);
        //    }
        //    SetChoiseView(new List<int>(_cardValList));
        //}
        #endregion

        public UIButton SpecialBtn;

        public GameObject[] TypeBtns;

        public ChoiseLine1[] Lines;

        public GameObject FinishView;

        public GameObject TypeBtnsView;

        public GameObject CanelAllBtn;

        /// <summary>
        /// 是否完成了选牌
        /// </summary>
        private bool _isFinish;

        /// <summary>
        /// 记录牌型字典
        /// </summary>
        private readonly Dictionary<CardType, List<SssDun>> _cardTypeDic = new Dictionary<CardType, List<SssDun>>();


        private List<int> _cardValList = new List<int>();

        /// <summary>
        /// 特殊牌型按钮
        /// </summary>
        private HelpLz.PlayerDuns _specialDuns;

        //public List<int> _TestLsit;

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.G))
        //    {
        //        _TestLsit = new List<int>(_cardValList) ;
        //    }

        //    if (Input.GetKeyDown(KeyCode.T))
        //    {
        //        Test(_TestLsit.ToArray());
        //    }
        //}


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
                    clone.transform.localScale = Vector3.one;
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
                if (line.CardList.Count == count) continue;     //牌够了 无需创建牌
                for (int j = 0; j < count; j++)
                {
                    PokerCard poker = Instantiate(CardPrefab);
                    AddPokerTriggerOnClick(this, poker, "OnClickLinePoker");
                    line.SetOneCard(poker);
                }
                line.ParentReposition();
            }
        }

        protected void OnClickReselectBtn(int index)
        {
            ReselectLine(index);
            NoSelected();
            SetLinesSelectBtn(true);
        }

        /// <summary>
        /// 代理添加事件,当点击每道的牌时
        /// </summary>
        /// <param name="card"></param>
        protected void OnClickLinePoker(PokerCard card)
        {
            if (!_isFinish) return;
            ExchangeCard(card);
        }


        void CreateEventDelegate(MonoBehaviour target, string methodName, UIButton btn, System.Object obj = null)
        {
            if (btn == null) return;
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
            //Resort(dunCardList);

            string errorMsg = string.Empty;
            if (CheckIsDaoShui(_selectInfo, index, ref errorMsg))
            {
                MessageShow(errorMsg);
                return;
            }

            _selectInfo = GetLineInfo(dunCardList, index);

            ReselectLine(index);
            Lines[index].SelectLine(_selectInfo);

            //隐藏掉已选择的牌
            for (int i = 0; i < count; i++)
            {
                var card = cards[i];
                cards[i].gameObject.SetActive(false);
                int id = card.Id;
                //移除已选择的牌
                if (_cardValList.Contains(id))
                {
                    _cardValList.Remove(card.Id);
                }
            }

            RespositionCards();
            _selectInfo.Reset();

            SetChoiseView(new List<int>(_cardValList));


            NoSelected(); //清除选中的牌
            //自动补全第三道
            count = _cardValList.Count;
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
            else if (count <= 0)
            {
                SetLinesSelectBtn(false);
            }
        }


        public override void OnDragOverCard(PokerCard card)
        {
            if (_isFinish) return;
            OnClickCard(card);
        }



        void ReselectLine(int index)
        {
            var line = Lines[index];
            var lineSelectInfo = line.LineInfo;
            if (lineSelectInfo == null) return;
            var cardsList = lineSelectInfo.Dun.Cards;
            if (cardsList == null) return;
            int count = cardsList.Count;


            for (int i = 0; i < count; i++)
            {
                var cardVal = cardsList[i];
                _cardValList.Add(cardVal);
                var poker = CardsList.Find(pok => pok.Id == cardVal && !pok.gameObject.activeSelf);
                if (poker == null)
                {
                    continue;
                }
                poker.IsSelect = false;
                poker.gameObject.SetActive(true);
            }

            line.Reselect();
            RespositionCards();
            SetChoiseView(new List<int>(_cardValList));
        }


        void RespositionCards()
        {
            PokerParent.GetComponent<UIGrid>().Reposition();
        }


        public override void ShowChoiseView(ISFSObject cardData)
        {
            if (cardData.ContainsKey("seats"))
            {
                var seats = cardData.GetIntArray("seats");
                if (!seats.Contains(App.GameData.SelfSeat))
                {
                    return;
                }
            }

            base.ShowChoiseView(cardData);
            InitPokerList();

            gameObject.SetActive(true);

            int[] cardArray = cardData.GetIntArray("cards");

            _cardValList = cardArray.ToList();
            //假牌数据
            //_cardValList = new List<int>() { 45, 29, 28, 27, 43, 59, 26, 41, 40, 39, 55, 38, 53 };
            //_cardValList = new List<int>() { 23, 39, 55, 22, 38, 54, 21, 37, 20, 36, 47, 35, 72 };
            YxDebug.LogArray(_cardValList);                     //打印数组

            HelpLz.SortList(_cardValList, false);
            HelpLz help = new HelpLz();

            //List<HelpLz.PlayerDuns> pd = help.getPlayerDuns(_cardValList);

            //初始化手牌,待删
            for (int i = 0; i < CardsList.Count; i++)
            {
                CardsList[i].SetCardId(_cardValList[i]);
                CardsList[i].SetCardFront();
            }

            RespositionCards();//初始化手牌位置

            //if (pd.Count > 0 && pd[0].SpecialType > CardType.none)
            //{
            //    _specialDuns = pd[0];
            //    if (SpecialBtn != null)
            //    {
            //        SpecialBtn.gameObject.SetActive(true);
            //    }
            //    else
            //    {
            //        OnClickSpecialBtn();
            //        ShowSepcialMark(pd[0].SpecialType);
            //    }
            //    return;
            //}

            _specialDuns = null;
            if (SpecialBtn != null)
            {
                SpecialBtn.gameObject.SetActive(false);
            }
            SetChoiseView(new List<int>(_cardValList));
        }

        public List<Texture> SpecialTextureList;

        public GameObject SpecialMark;

        /// <summary>
        /// 特殊牌型的文字特效
        /// </summary>
        public ParticleSystem SpecialLabelParticle;


        private void ShowSepcialMark(CardType special)
        {
            SpecialMark.SetActive(true);
            Renderer component = SpecialLabelParticle.GetComponent<Renderer>();
            CardType cardType = special;
            string s = cardType.ToString();
            component.material.mainTexture = SpecialTextureList.Find(tex => tex.name == s);
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

            _cardTypeDic.Clear();

            var magicCardsList = SeparateMagicCard(cardValList);
            int btnState = 0;
            if (magicCardsList.Count > 0)
            {
                int magicCount = magicCardsList.Count;
                for (int i = 0; i <= magicCount; i++)
                {
                    btnState |= GetBtnState(cardValList, magicCardsList.GetRange(0, i));       //从这里开始
                }
            }
            else
            {
                btnState = GetBtnState(cardValList, new List<int>());
            }

            //从这里开始加癞子
            ShowTypeBtns(btnState);

            _isFinish = count < 3;

            TypeBtnsView.SetActive(!_isFinish);
            FinishView.SetActive(_isFinish);
            SetCancelAllBtn(_isFinish);
        }

        List<int> SeparateMagicCard(List<int> cardsList)
        {
            List<int> magicList = new List<int>();
            for (int i = 0; i < cardsList.Count;)
            {
                int val = cardsList[i];
                if (val >= 0x51)
                {
                    magicList.Add(val);
                    cardsList.Remove(val);
                }
                else
                {
                    i++;
                }
            }

            return magicList;
        }


        bool IsWuTong(VnList vnList, int magicCount)
        {
            return vnList.GetMoreThan(5 - magicCount, true).Count > 0;
        }

        private bool IsTiezhi(VnList vnList, int magicCount)
        {
            return vnList.GetMoreThan(4 - magicCount, true).Count > 0;
        }



        bool IsShunZi(VnList vnList, int magicCount)
        {
            var cnt1 = vnList.GetMoreThan(1, true);
            if (cnt1[0].Val == 14) cnt1.Add(new HelpLz.VnItem(1));
            int count = cnt1.Count;
            int checkCount = count + magicCount - 4;
            if (checkCount < 0)
                return false;


            for (int i = 0; i < checkCount + 1; i++)
            {
                int lastVal = cnt1[i].Val;
                int tempMCount = magicCount;
                int shunziCounter = 1;
                for (int j = i + 1; j < count; j++)
                {
                    int thisVal = cnt1[j].Val;
                    thisVal = thisVal == 14 && lastVal < 10 ? 1 : thisVal;
                    if (lastVal - thisVal == 1)
                    {
                        lastVal = thisVal;
                        shunziCounter++;
                        if (shunziCounter >= 5 - magicCount)
                        {
                            if (cnt1[0].Val == 14)
                            {
                                cnt1.Remove(cnt1[count - 1]);
                            }
                            return true;
                        }
                    }
                    else if (tempMCount > 0)
                    {
                        j--;
                        lastVal--;
                        --tempMCount;
                    }
                    else
                    {
                        break;
                    }

                }
            }
            if (cnt1[0].Val == 14)
            {
                cnt1.Remove(cnt1[count - 1]);
            }
            return false;
        }

        bool IsTonghua(CnList cn)
        {
            return cn.Count < 2;
        }

        bool IsLiangdui(VnList vn, int magicCount)
        {
            int cnt2 = vn.GetMoreThan(2, true).Count;
            int cnt1 = vn.GetMoreThan(1, true).Count;
            return vn.GetMoreThan(2, true).Count + Mathf.Min(cnt1 - cnt2, magicCount) >= 2;
        }

        bool IsYidui(VnList vn, int magicCount)
        {
            return vn.GetMoreThan(2 - magicCount, true).Count > 0;
        }

        bool IsSantiao(VnList vn, int magicCount)
        {
            return vn.GetMoreThan(3 - magicCount, true).Count > 0;
        }

        bool IsHulu(VnList vn, int magicCount)
        {
            int cnt3 = vn.GetMoreThan(3, true).Count;
            int cnt2 = vn.GetMoreThan(2, true).Count;

            int score = cnt3 * 10 + cnt2;

            return magicCount > 0 ? score >= 2 : score >= 12;
        }


        public CardType CheckCardType(List<int> cards, int line = 2)
        {
            var tempList = cards.GetRange(0, line == 0 ? 3 : 5);
            var magicList = SeparateMagicCard(tempList);
            VnList vnlist = new VnList(tempList);
            int magicCount = magicList.Count;
            if (line != 0)
            {
                if (IsWuTong(vnlist, magicCount))
                {
                    return CardType.wutong;
                }
                bool tonghua = IsTonghua(new CnList(tempList));
                bool shunzi = IsShunZi(vnlist, magicCount);

                if (tonghua && shunzi)
                {
                    return CardType.tonghuashun;
                }

                if (IsTiezhi(vnlist, magicCount))
                {
                    return CardType.tiezhi;
                }
                if (IsHulu(vnlist, magicCount))
                {
                    return CardType.hulu;
                }
                if (tonghua)
                {
                    return CardType.tonghua;
                }
                if (shunzi)
                {
                    return CardType.shunzi;
                }
            }
            if (IsSantiao(vnlist, magicCount))
            {
                return CardType.santiao;
            }
            if (IsLiangdui(vnlist, magicCount))
            {
                return CardType.liangdui;
            }
            if (IsYidui(vnlist, magicCount))
            {
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
            SetLinesSelectBtn(false);
            SetCancelAllBtn(false);
        }

        void SetCancelAllBtn(bool active)
        {
            CanelAllBtn.GetComponent<BoxCollider>().enabled = active;
            var btn = CanelAllBtn.GetComponent<UIButton>();
            btn.state = active ? UIButtonColor.State.Normal : UIButtonColor.State.Disabled;
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
                case CardType.wutong:
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
                MoveYTo(card.transform, 26);
                var selectCards = _selectInfo.SelectedCards;
                if (!selectCards.Contains(card))
                {
                    selectCards.Add(card);
                    var dunCards = _selectInfo.Dun.Cards;
                    dunCards.Add(card.Id);
                }
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
            YxDebug.LogArray(_selectInfo.Dun.Cards);
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
            var tempCardList = new List<PokerCard>(CardsList);
            int count = selectValList.Count;
            for (int i = 0; i < count; i++)
            {
                int cardId = selectValList[i];
                var card = tempCardList.Find(pok => (cardId == pok.Id && pok.gameObject.activeSelf));
                tempCardList.Remove(card);
                
                if (card == null)
                {
                    Debug.LogError("Find Error card!!");
                    continue;
                }

                if (card.IsSelect)
                {
                    continue;
                }

                selectCardList.Add(card);
            }
            return selectCardList;
        }


        /// <summary>
        /// 清除选中的牌
        /// </summary>
        public void NoSelected()
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
        /// <param name="magicCardList"></param>
        /// <returns></returns>
        private int GetBtnState(List<int> pokerList, List<int> magicCardList)
        {
            if (pokerList.Count + magicCardList.Count < 3)
                return 0;

            //普通牌为0,必然三癞子,必然三条
            if (pokerList.Count == 0)
            {
                AddListToDic(magicCardList.Count == 5 ? CardType.wutong : CardType.santiao, _cardTypeDic, magicCardList);
                return GetStateVal(CardType.santiao);
            }

            int btnState = 0;
            CnList cnList = new CnList(pokerList);
            VnList vnList = new VnList(pokerList);
            int magicCount = magicCardList.Count;

            CnList cn5 = cnList.GetMoreThan(5 - magicCount, true, true);     //同花色5个以上


            //五同
            {
                int maxIndex = GetCount(5, magicCount);
                VnList vn5 = vnList.GetMoreThan(maxIndex, true);
                if (vn5.Count > 0)
                {
                    foreach (var item in vn5)
                    {
                        var tempList = item.Cards.GetRange(0, maxIndex);
                        AddListToDic(CardType.wutong, _cardTypeDic, tempList, magicCardList);
                        btnState |= GetStateVal(CardType.wutong);
                    }
                }
            }

            //同花顺
            if (cn5.Count > 0)
            {
                foreach (var cn5Item in cn5)
                {
                    var checkList = new List<int>(cn5Item.Cards);

                    //有A,加入到2之后
                    HelpLz.SortList(checkList, false);
                    if (HelpLz.GetValue(checkList[0]) == 14)
                    {
                        checkList.Add(checkList[0]);
                    }

                    int loopTime = checkList.Count + magicCount - 4; //循环次数
                    int maxIndex = 5 - magicCount;
                    var tempList = new List<int>();

                    for (int i = 0; i < loopTime; i++)
                    {
                        tempList.Clear();
                        int lastVal = checkList[i];
                        tempList.Add(lastVal);
                        int tempMcount = magicCount; //对癞子牌进行处理
                        for (int j = i + 1; j < checkList.Count && tempList.Count < maxIndex && tempMcount >= 0; j++)
                        {
                            int thisVal = checkList[j];

                            int different = getDiff(lastVal, thisVal);
                            //HelpLz.GetValue(lastVal) - HelpLz.GetValue(thisVal);

                            if (different == 0)
                            {
                            }
                            else if (different == 1)
                            {
                                tempList.Add(thisVal);
                                lastVal = thisVal;
                            }
                            else if (tempMcount > 0 && different > 0)
                            {
                                tempMcount -= different - 1;
                                if (tempMcount >= 0)
                                {
                                    tempList.Add(thisVal);
                                    lastVal = thisVal;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (tempList.Count >= maxIndex)
                        {
                            AddListToDic(CardType.tonghuashun, _cardTypeDic, new List<int>(tempList), magicCardList);
                            btnState |= GetStateVal(CardType.tonghuashun);
                        }
                    }
                }
            }

            if (magicCount > 3) return btnState;        //4张癞子必然五同,无需后面的统计

            //铁支
            {
                int maxIndex = GetCount(4, magicCount);
                VnList vn4 = vnList.GetEqual(maxIndex, true);
                if (vn4.Count > 0)
                {
                    foreach (var item in vn4)
                    {
                        var tempCards = new List<int>();
                        tempCards.AddRange(item.Cards.GetRange(0, maxIndex));
                        tempCards.AddRange(magicCardList);
                        SssDun dun = new SssDun
                        {
                            CardType = CardType.tiezhi,
                            Cards = tempCards
                        };
                        if (CouldAddOtherCards(dun.Cards, 5 - tempCards.Count, vnList))
                        {
                            AddToDic(_cardTypeDic, CardType.tiezhi, dun);
                        }
                    }
                    btnState |= GetStateVal(CardType.tiezhi);
                }
            }

            if (magicCount > 2) return btnState;        //3张癞子  至少铁质,无需后面计算

           


            //同花
            if (cn5.Count > 0)
            {
                foreach (var cnItem in cn5)
                {
                    var tempCardsList = cnItem.Cards;
                    int cardCount = tempCardsList.Count;
                    int count = GetCount(5, magicCount);
                    int loop = cardCount - 4 + magicCount;
                    for (int i = 0; i < loop; i++)
                    {
                        var tempCards = tempCardsList.GetRange(i, count);
                        if (IsShunZi(new VnList(tempCards), magicCount)) continue;
                        AddListToDic(CardType.tonghua, _cardTypeDic, tempCards,
                            magicCardList);
                        btnState |= GetStateVal(CardType.tonghua);
                    }
                }
            }

            //TODO 顺子 , 有问题 , 同花顺的时候会掩盖掉同花顺的牌 , 如 红桃 方块 10~6,10张牌，只会检测出同花顺
            {
                var checkList = new List<int>(pokerList);

                //有A,加入到2之后
                HelpLz.SortList(checkList, false);
                if (HelpLz.GetValue(checkList[0]) == 14)
                {
                    checkList.Add(checkList[0]);
                }

                int loopTime = checkList.Count + magicCount - 4;      //循环次数
                int maxIndex = 5 - magicCount;
                var tempList = new List<int>();

                for (int i = 0; i < loopTime; i++)
                {
                    tempList.Clear();
                    int lastVal = checkList[i];
                    tempList.Add(lastVal);
                    int tempMcount = magicCount;      //对癞子牌进行处理
                    for (int j = i + 1; j < checkList.Count && tempList.Count < maxIndex && tempMcount >= 0; j++)
                    {
                        int thisVal = checkList[j];

                        int different = getDiff(lastVal, thisVal); //HelpLz.GetValue(lastVal) - HelpLz.GetValue(thisVal);

                        if (different == 0)
                        {
                        }
                        else if (different == 1)
                        {
                            tempList.Add(thisVal);
                            lastVal = thisVal;
                        }
                        else if (different > 0 && tempMcount > 0)
                        {
                            tempMcount -= different - 1;
                            if (tempMcount >= 0)
                            {
                                tempList.Add(thisVal);
                                lastVal = thisVal;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (tempList.Count >= maxIndex && !IsTonghua(new CnList(tempList)))
                    {
                        AddListToDic(CardType.shunzi, _cardTypeDic, new List<int>(tempList), magicCardList);
                        btnState |= GetStateVal(CardType.shunzi);
                    }
                }
            }


            //三条
            {
                int maxIndex = GetCount(3, magicCount);
                var vn3 = vnList.GetMoreThan(maxIndex, true);
                if (vn3.Count > 0)
                {
                    int count = vn3.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var cardList = vn3[i].Cards.GetRange(0, maxIndex);
                        cardList.AddRange(magicCardList);
                        if (CouldAddOtherCards(cardList, 5 - cardList.Count, vnList))
                        {
                            AddListToDic(CardType.santiao, _cardTypeDic, cardList);
                            btnState |= GetStateVal(CardType.santiao);
                        }
                    }
                }
            }

            if (magicCount > 1) return btnState;    //必然三条

            //葫芦
            {
                //只有两个以内的来自时,会有葫芦
                //int count3 = GetCount(3, magicCount);
                //var tempvn3 = vnList.GetMoreThan(count3, true);
                //var tempvn2 = vnList.GetMoreThan(2, false); //癞子必然用在三条上

                //tempvn2.Sort((o1, o2) => o1.Val - o2.Val);
                //if (tempvn3.Count > 0 && tempvn2.Count > 0)
                //{
                //    foreach (var item3 in tempvn3)
                //    {
                //        foreach (var item2 in tempvn2)
                //        {
                //            if (item2.Val == item3.Val || (count3 <= item2.Cards.Count && item2.Val > item3.Val))
                //            {
                //                continue;
                //            }
                //            VnList tempVnList = new VnList { item3, item2 };
                //            if (!IsHulu(tempVnList, magicCount))
                //            {
                //                continue;
                //            }
                //            List<int> tempCardList = new List<int>(item3.Cards.GetRange(0, count3));
                //            tempCardList.AddRange(item2.Cards.GetRange(0, 2));
                //            AddListToDic(CardType.hulu, _cardTypeDic, tempCardList,
                //                magicCardList.GetRange(0, magicCount));

                //            btnState |= GetStateVal(CardType.hulu);
                //        }
                //    }
                //}

                int count3 = GetCount(3, magicCount);
                //优先使用3跳,再使用4条,5条
                for (int i = count3; i < 5; i++)
                {
                    var tempvn3 = vnList.GetEqual(i, true);
                    if (tempvn3 == null || tempvn3.Count == 0)
                        continue;
                    //优先使用2条,再使用3条,4条,5条
                    for (int j = 2; j < 5; j++)
                    {
                        var tempvn2 = vnList.GetEqual(j, false);
                        if (tempvn2 == null || tempvn2.Count == 0)
                            break;

                        tempvn2.Sort((o1, o2) => o1.Val - o2.Val);
                        if (tempvn3.Count > 0 && tempvn2.Count > 0)
                        {
                            foreach (var item3 in tempvn3)
                            {
                                foreach (var item2 in tempvn2)
                                {
                                    if (item2.Val == item3.Val|| (magicCount > 0 && count3 <= item2.Cards.Count && item2.Val > item3.Val))
                                    {
                                        continue;
                                    }
                                    VnList tempVnList = new VnList { item3, item2 };
                                    if (!IsHulu(tempVnList, magicCount))
                                    {
                                        continue;
                                    }
                                    List<int> tempCardList = new List<int>(item3.Cards.GetRange(0, count3));
                                    tempCardList.AddRange(item2.Cards.GetRange(0, 2));
                                    AddListToDic(CardType.hulu, _cardTypeDic, tempCardList,
                                        magicCardList.GetRange(0, magicCount));

                                    btnState |= GetStateVal(CardType.hulu);
                                }
                            }
                        }
                    }
                }
            }


            //对子
            {
                int maxIndex = GetCount(2, magicCount);
                VnList vn2 = vnList.GetMoreThan(maxIndex, true);
                if (vn2.Count > 0)
                {
                    int count = vn2.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var cardList = vn2[i].Cards.GetRange(0, maxIndex);
                        cardList.AddRange(magicCardList);
                        if (CouldAddOtherCards(cardList, 5 - cardList.Count, vnList))
                        {
                            AddListToDic(CardType.yidui, _cardTypeDic, cardList);
                            btnState |= GetStateVal(CardType.yidui);
                        }
                    }
                }
            }

            if (magicCount > 0) return btnState;    //一个对子 + 赖子   必然三条

            //两对
            {
                VnList vn2 = vnList.GetMoreThan(2, true); //同数值的2个以上
                if (vn2.Count > 1)
                {
                    //2张优先3张排序
                    for (int i = 0; i < vn2.Count - 1; i++)
                    {
                        for (int j = 0; j < vn2.Count - 1 - i; j++)
                        {
                            if (vn2[j].Cards.Count > vn2[j + 1].Cards.Count)
                            {
                               var temp = vn2[j + 1];
                                vn2[j + 1] = vn2[j];
                                vn2[j] = temp;
                            }
                        }
                    }

                    VnList vn2have2 = new VnList();
                    VnList vn2have3 = new VnList();
                    for (int i = 0; i < vn2.Count; i++)
                    {
                        if (vn2[i].Cards.Count>2)
                        {
                            vn2have3.Add(vn2[i]);
                        }
                        else
                        {
                            vn2have2.Add(vn2[i]);
                        }
                    }

                    if (vn2have2!=null)
                    {
                        LiangDuiAdd2T2(vn2have2, vn2have2, ref btnState, vnList);
                    }
                    if (vn2have3 != null)
                    {
                        LiangDuiAdd2T3(vn2have3, vn2have2, ref btnState, vnList);
                        LiangDuiAdd2T2(vn2have3, vn2have3, ref btnState, vnList);
                    }
                }
            }
            return btnState;
        }

        private void LiangDuiAdd2T2(VnList vn2,VnList vn2Tonumber,ref int btnState, VnList vnList)
        {
            for (int i = 0; i < vn2.Count; i++)
            {
                VnItem vn2Vi = vn2[i];
                for (int j = i; j < vn2Tonumber.Count; j++)
                {
                    VnItem vn2Vj = vn2Tonumber[j];
                    if (vn2Vi.Val == vn2Vj.Val) continue;
                    var cardList = vn2Vi.Cards.GetRange(0, 2);
                    cardList.AddRange(vn2Vj.Cards.GetRange(0, 2));
                    if (CouldAddOtherCards(cardList, 5 - cardList.Count, vnList))
                    {
                        AddListToDic(CardType.liangdui, _cardTypeDic, cardList);
                        btnState |= GetStateVal(CardType.liangdui);
                    }
                }
            }
        }
        private void LiangDuiAdd2T3(VnList vn2, VnList vn2Tonumber, ref int btnState, VnList vnList)
        {
            for (int i = 0; i < vn2.Count; i++)
            {
                VnItem vn2Vi = vn2[i];
                for (int j = 0; j < vn2Tonumber.Count; j++)
                {
                    VnItem vn2Vj = vn2Tonumber[j];
                    if (vn2Vi.Val == vn2Vj.Val) continue;
                    var cardList = vn2Vi.Cards.GetRange(0, 2);
                    cardList.AddRange(vn2Vj.Cards.GetRange(0, 2));
                    if (CouldAddOtherCards(cardList, 5 - cardList.Count, vnList))
                    {
                        AddListToDic(CardType.liangdui, _cardTypeDic, cardList);
                        btnState |= GetStateVal(CardType.liangdui);
                    }
                }
            }
        }
        bool CheckShunzi(List<int> cardList, List<int> magicCardList)
        {
            bool haveShunzi = false;
            var checkList = new List<int>(cardList);
            int magicCount = magicCardList.Count;

            //有A,加入到2之后
            HelpLz.SortList(checkList, false);
            if (HelpLz.GetValue(checkList[0]) == 14)
            {
                checkList.Add(checkList[0]);
            }

            int loopTime = checkList.Count + magicCount - 4;      //循环次数
            int maxIndex = 5 - magicCount;
            var tempList = new List<int>();

            for (int i = 0; i < loopTime; i++)
            {
                tempList.Clear();
                int lastVal = checkList[i];
                tempList.Add(lastVal);
                int tempMcount = magicCount;      //对癞子牌进行处理
                for (int j = i + 1; j < checkList.Count && tempList.Count < maxIndex && tempMcount >= 0; j++)
                {
                    int thisVal = checkList[j];

                    int different = getDiff(lastVal, thisVal); //HelpLz.GetValue(lastVal) - HelpLz.GetValue(thisVal);

                    if (different == 0)
                    {
                    }
                    else if (different == 1)
                    {
                        tempList.Add(thisVal);
                        lastVal = thisVal;
                    }
                    else if (different > 0 && tempMcount > 0)
                    {
                        tempMcount -= different - 1;
                        if (tempMcount >= 0)
                        {
                            tempList.Add(thisVal);
                            lastVal = thisVal;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (tempList.Count >= maxIndex && !IsTonghua(new CnList(tempList)))
                {
                    AddListToDic(CardType.shunzi, _cardTypeDic, new List<int>(tempList), magicCardList);
                    haveShunzi = true;
                }
            }

            return haveShunzi;
        }


        /// <summary>
        /// 补全三条,两对,一对,铁枝等牌型
        /// </summary>
        /// <param name="outList"></param>
        /// <param name="maxCount"></param>
        /// <param name="vnList"></param>
        /// <param name="curCount"></param>
        /// <param name="otherList">添加牌数组,临时记录使用</param>
        /// <returns></returns>
        private bool CouldAddOtherCards(List<int> outList, int maxCount, VnList vnList, int curCount = 1,
            List<int> otherList = null)
        {
            var curvn = vnList.GetEqual(curCount, false);
            if (curvn == null || curvn.Count == 0)
            {
                if (curCount > 4)
                {
                    return false;
                }
                return CouldAddOtherCards(outList, maxCount, vnList, curCount + 1, otherList);
            }
            curvn.Sort((o1, o2) => o1.Val - o2.Val);
            if (otherList == null)
            {
                otherList = new List<int>();
            }
            int curvnCount = curvn.Count;
            for (int i = 0; i < curvnCount; i++)
            {
                var c = curvn[i].Cards[0];
                if (c % 16 == outList[0] % 16)
                    continue;
                otherList.Add(c);
                --maxCount;
                if (maxCount <= 0)
                {
                    Resort(otherList, false);
                    outList.AddRange(otherList);
                    return true;
                }
            }

            return CouldAddOtherCards(outList, maxCount, vnList, curCount + 1, otherList);
        }


        int getDiff(int lastVal, int thisVal)
        {
            int lval = HelpLz.GetValue(lastVal);
            int tval = HelpLz.GetValue(thisVal);
            return tval == 14 && lval < 10 ? lval - 1 : lval - tval;
        }

        int GetCount(int max, int magicCount)
        {
            max -= magicCount;
            return max > 0 ? max : 1;
        }

        protected bool CheckCardType(List<int> cardList, int magicCount, CardType checkType)
        {
            switch (checkType)
            {
                case CardType.yidui:
                    return IsYidui(new VnList(cardList), magicCount);
                case CardType.liangdui:
                    return IsLiangdui(new VnList(cardList), magicCount);
                case CardType.santiao:
                    return IsSantiao(new VnList(cardList), magicCount);
                case CardType.shunzi:
                    return IsShunZi(new VnList(cardList), magicCount) && !IsTonghua(new CnList(cardList));
                case CardType.tonghua:
                    return IsTonghua(new CnList(cardList)) && !IsShunZi(new VnList(cardList), magicCount);
                case CardType.hulu:
                    return IsHulu(new VnList(cardList), magicCount);
                case CardType.tiezhi:
                    return IsTiezhi(new VnList(cardList), magicCount);
                case CardType.tonghuashun:
                    return IsTonghua(new CnList(cardList)) && IsShunZi(new VnList(cardList), magicCount);
                case CardType.wutong:
                    return IsWuTong(new VnList(cardList), magicCount);

                default:
                    return false;
            }
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
            return 1 << (int)cardType - 1;
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
                var linePokers = Lines[i].CardList;
                int count = linePokers.Count;
                for (int j = 0; j < count; j++)
                {
                    if (linePokers[j].Equals(_selectPok1))
                    {
                        valList.Remove(cardId1);
                        valList.Add(cardId2);
                        var info = GetLineInfo(valList, i);
                        Lines[i].SelectLine(info);
                        continue;
                    }

                    if (linePokers[j].Equals(_selectPok2))
                    {
                        valList.Remove(cardId2);
                        valList.Add(cardId1);
                        var info = GetLineInfo(valList, i);

                        Lines[i].SelectLine(info);
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
            lineInfo.SelectedCards = GetSelectCardList(new List<int>(cardList));
            return lineInfo;
        }


        /// <summary>
        /// 获取扑克在数组中的索引
        /// </summary>
        /// <param name="cardList"></param>
        /// <param name="card"></param>
        /// <param name="from"></param>
        /// <returns></returns>
        protected int GetPokerIndex(List<PokerCard> cardList, PokerCard card, int from)
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

            for (int i = 2; i >= 0; i--)
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
            for (int i = 0; i < Lines.Length; i++)
            {
                ReselectLine(i);
            }

            SetAllPokerCardActive(true);
            SetLinesSelectBtn(true);
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

                if (MatchListType(lineInfo1.Dun.Cards, index1, lineInfo2.Dun.Cards, index2))
                {
                    errorMsg = string.Format("第{0},{1}行出现倒水!", Mathf.Min(index1, index2) + 1, Mathf.Max(index1, index2) + 1);
                    return true;
                }
            }
            return false;
        }

        bool CheckIsDaoShui(SelectedInfo selectLineInfo, int index, ref string errorMsg)
        {
            int len = Lines.Length;    //比较的次数
            for (int i = 0; i < len - 1; i++)
            {
                int index2 = (index + i + 1) % len;
                var lineInfo2 = Lines[index2].LineInfo;

                if (MatchListType(selectLineInfo.Dun.Cards, index, lineInfo2.Dun.Cards, index2))
                {
                    errorMsg = string.Format("第{0},{1}行出现倒水!", Mathf.Min(index, index2) + 1, Mathf.Max(index, index2) + 1);
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
        bool MatchListType(List<int> list1, int line1, List<int> list2, int line2)
        {
            if (list1 == null || list1.Count < 1 || list2 == null || list2.Count < 1)
                return false;

            CardType type1 = CheckCardType(list1, line1);
            CardType type2 = CheckCardType(list2, line2);
            YxDebug.Log(string.Format(" === 第{0}行牌型为{1} , 第{2}行牌型为{3} === ", line1, type1, line2, type2));
            int result = type1 - type2;
            int dif = line1 - line2;

            //同牌型比较
            if (result == 0)
            {
                var clone1 = new List<int>(list1);
                var clone2 = new List<int>(list2);
                SeparateMagicCard(clone1);
                SeparateMagicCard(clone2);

                var vnList1 = new VnList(clone1);
                var vnList2 = new VnList(clone2);

                switch (type1)
                {
                    //必然没有癞子
                    case CardType.sanpai:
                    case CardType.liangdui:
                        vnList1 = new VnList(list1);
                        vnList2 = new VnList(list2);
                        for (int i = 0; i < vnList2.Count; i++)
                        {
                            int v1 = vnList1[i].Val;
                            int v2 = vnList2[i].Val;
                            if (v1 != v2)
                            {
                                return v1.CompareTo(v2) * dif < 0;
                            }
                        }
                        break;

                    //只会有一个癞子
                    case CardType.yidui:
                        //癞子数相同
                        if (clone1.Count == clone2.Count)
                        {
                            return vnList1.Compare(vnList2) * dif < 0;
                        }

                        //癞子数不同,比最大一张牌
                        int minCount = Mathf.Min(vnList1.Count, vnList2.Count);
                        for (int i = 0; i < minCount; i++)
                        {
                            int v1 = vnList1[i].Val;
                            int v2 = vnList2[i].Val;
                            if (v1 != v2)
                            {
                                return v1.CompareTo(v2) * dif < 0;
                            }
                        }
                        break;

                    case CardType.santiao:
                        //癞子数相同
                        if (clone1.Count == clone2.Count)
                        {
                            return vnList1.Compare(vnList2) * dif < 0;
                        }

                        //找出2张以上的牌
                        VnList mtt1 = vnList1.GetMoreThan(2, true);
                        VnList mtt2 = vnList2.GetMoreThan(2, true);
                        minCount = Mathf.Min(mtt1.Count, mtt2.Count);
                        //只需比较牌值   TODO 做两个铁支被拆成4对的检测
                        for (int i = 0; i < minCount; i++)
                        {
                            int v1 = mtt1[i].Val;
                            int v2 = mtt2[i].Val;
                            if (v1 != v2)
                            {
                                return v1.CompareTo(v2) * dif < 0;
                            }
                        }
                        break;

                    case CardType.shunzi:
                    case CardType.tonghuashun:
                        int ct1 = vnList1.Count;
                        int ct2 = vnList2.Count;
                        bool biger = ct1 > ct2;

                        minCount = biger ? ct2 : ct1;
                        int cha = ct1 + ct2 - minCount * 2;
                        for (int i = minCount - 1; i >= 0; i--)
                        {
                            int index = biger ? i + cha : i;
                            int v1 = vnList1[index].Val;
                            v1 = v1 == 14 && vnList1[i + 1].Val < 10 ? 1 : v1;
                            index = biger ? i : i + cha;
                            int v2 = vnList2[index].Val;
                            v2 = v2 == 14 && vnList2[i + 1].Val < 10 ? 1 : v2;
                            if (v1 != v2)
                                return v1.CompareTo(v2) * dif < 0;
                        }
                        break;

                    case CardType.tonghua:
                        //来自是 0x51, 0x61 直接加入A组
                        if (list1.Count != list2.Count) return false;

                        var comList1 = new List<int>(list1);
                        var comList2 = new List<int>(list2);
                        HelpLz.SortList(comList1, false);
                        HelpLz.SortList(comList2, false);
                        for (int i = 0; i < comList1.Count; i++)
                        {
                            int cv1 = comList1[i];
                            int cv2 = comList2[i];
                            int comval1 = HelpLz.GetValue(cv1);
                            int comval2 = HelpLz.GetValue(cv2);
                            if (comval1 == comval2)
                            {
                                if (comval1 == 14)
                                {
                                    if ((cv1 >= 0x51 || cv2 >= 0x51) && cv1 != cv2)
                                    {
                                        return (cv1 - cv2) * dif < 0;
                                    }
                                }
                                continue;
                            }
                            return (comval1 - comval2) * dif < 0;
                        }
                        return false;

                    case CardType.hulu:
                        //有来自也是1个癞子两对的形式
                        if (clone1.Count == clone2.Count)
                        {
                            return vnList1.Compare(vnList2) * dif < 0;
                        }
                        minCount = Mathf.Min(vnList1.Count, vnList2.Count);
                        for (int i = 0; i < minCount; i++)
                        {
                            int v1 = vnList1[i].Val;
                            int v2 = vnList2[i].Val;
                            if (v1 != v2)
                            {
                                return v1.CompareTo(v2) * dif < 0;
                            }
                        }
                        break;

                    case CardType.tiezhi:
                        minCount = Mathf.Min(vnList1.Count, vnList2.Count);
                        for (int i = 0; i < minCount; i++)
                        {
                            int v1 = vnList1[i].Val;
                            int v2 = vnList2[i].Val;
                            if (v1 != v2)
                            {
                                return v1.CompareTo(v2) * dif < 0;
                            }
                        }
                        break;

                    case CardType.wutong:
                        int val1 = vnList1.Count > 0 ? vnList1[0].Val : 14;
                        int val2 = vnList2.Count > 0 ? vnList2[0].Val : 14;
                        return val1.CompareTo(val2) * dif < 0;
                }

            }
            return result * dif < 0;
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
            SpecialMark.SetActive(false);
        }
    }
}