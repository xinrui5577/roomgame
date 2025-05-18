using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.sss.ImgPress.Main;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;
#pragma warning disable 649

namespace Assets.Scripts.Game.sss
{

    public class ChoiseWay0 : ChoiseMgr
    {

        #region     测试部分方法
        //public override void Test(int[] cardArray)
        //{
        //    Init();
        //    Help1 help = new Help1();
        //    List<int> pokerList = cardArray.ToList();
        //    Resort(pokerList);
        //    List<Help1.PlayerDuns> pd = help.getPlayerDuns(pokerList);

        //    while (pd.Count > 0 && pd[0].Duns[0] == null)
        //    {
        //        pd.Remove(pd[0]);
        //    }

        //    int itemCount = pd.Count > 3 ? 3 : pd.Count;
        //    //初始化数据
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        ChoiseItem item = _choiseItems[i];
        //        item.gameObject.SetActive(true);
        //        if ((int)pd[i].Special - (int)CardType.none >= 0)   //特殊牌型,不用显示每行牌型
        //        {
        //            item.SpecialObj.SetActive(true);
        //            item.NormalObj.SetActive(false);
        //            item.Special = (int)pd[i].Special;
        //            item.SpecialSprite.spriteName = pd[i].Special.ToString();
        //            item.SpecialSprite.MakePixelPerfect();
        //            item.SetChoiseItem(pd[i].Duns);
        //        }
        //        else    //普通牌型,要显示每组牌的牌型
        //        {
        //            item.SpecialObj.SetActive(false);
        //            item.NormalObj.SetActive(true);

        //            item.SetChoiseItem(pd[i].Duns);
        //        }
        //    }
        //    OnClickPlane(_choiseItems[0]);
        //}
        #endregion

        private PokerCard _selectPok1;

        private PokerCard _selectPok2;

        private List<int> _lastList = new List<int>();

        [SerializeField]
        private ChoiseItem[] _choiseItems;


        private ChoiseItem _lastChoiseItem;


        private readonly List<int> _curCardValList = new List<int>();

       

        public override void ShowChoiseView(ISFSObject cardData)
        {
            base.ShowChoiseView(cardData);
            InitPokerList();        //初始化左侧的牌
            RepositionCards();      //将左侧的牌排列成三行

            gameObject.SetActive(true);

            int[] array = cardData.GetIntArray("cards");
            
            Help1 help = new Help1();
            List<int> pokerList = array.ToList();
            Resort(pokerList);
            List<Help1.PlayerDuns> pd = help.getPlayerDuns(pokerList);

            while (pd.Count > 0 && pd[0].Duns[0] == null)
            {
                pd.Remove(pd[0]);
            }

            int itemCount = pd.Count > 3 ? 3 : pd.Count;
            //初始化数据
            for (int i = 0; i < itemCount; i++)
            {
                ChoiseItem item = _choiseItems[i];
                item.gameObject.SetActive(true);
                if ((int)pd[i].Special - (int)CardType.none >= 0)   //特殊牌型,不用显示每行牌型
                {
                    item.SpecialObj.SetActive(true);
                    item.NormalObj.SetActive(false);
                    item.Special = (int)pd[i].Special;
                    item.SpecialSprite.spriteName = pd[i].Special.ToString();
                    item.SpecialSprite.MakePixelPerfect();
                    item.SetChoiseItem(pd[i].Duns);
                }
                else    //普通牌型,要显示每组牌的牌型
                {
                    item.SpecialObj.SetActive(false);
                    item.NormalObj.SetActive(true);

                    item.SetChoiseItem(pd[i].Duns);
                }
            }
            OnClickPlane(_choiseItems[0]);
        }


        public void OnClickPlane(ChoiseItem choise)
        {

            if (choise == null)
            {
                foreach (ChoiseItem item in _choiseItems)
                {
                    item.SelectMark.SetActive(false);
                }
                return;
            }

            _lastChoiseItem = choise;

            foreach (ChoiseItem item in _choiseItems)
            {
                item.SelectMark.SetActive(item.Equals(choise));
            }

            ChoisePlane(choise.CardsList);
            _lastList = new List<int>(choise.CardsList);
        }



        /// <summary>
        /// 将牌变为选择的方案牌型
        /// </summary>
        /// <param name="intList"></param>
        void ChoisePlane(List<int> intList)
        {
            //第一次选择的时候不用换牌,直接赋值
            if (_lastList == null || _lastList.Count == 0)
            {
                for (int i = 0; i < intList.Count; i++)
                {
                    PokerCard ca = CardsList[i].GetComponent<PokerCard>();
                    ca.SetCardId(intList[i]);
                    ca.SetCardFront();
                }
                return;
            }

            List<int> tempList = _lastList.GetRange(0, 13);
            if (intList.Count != tempList.Count)
            {
                YxDebug.LogError("码牌数据错误");
                return;
            }

            bool change = false;
            for (int i = 0; i < tempList.Count; i++)
            {
                int index = i;
                var tempObj = CardsList[i];
                if (intList[i] != tempList[i])
                {
                    index = tempList.IndexOf(intList[i]);

                    CardsList[i] = CardsList[index];
                    CardsList[index] = tempObj;

                    int tempInt = tempList[i];
                    tempList[i] = tempList[index];
                    tempList[index] = tempInt;
                    --i;
                    change = true;
                }
                PokerCard card = tempObj.GetComponent<PokerCard>();
                card.MoveTo(CardsList[index].transform.localPosition, RepositionCard(index));
            }
            if (change)
            {
                Facade.Instance<MusicManager>().Play("select");
            }

            _lastList = tempList;
            ResetSelectedCards();
        }

        void RepositionCards()
        {
            for (int i = 0; i < CardsList.Count; i++)
            {
                Vector3 v3 = RepositionCard(i);
                CardsList[i].transform.localPosition = v3;
            }
        }

        Vector3 RepositionCard(int x)
        {
            if (x < 3)
            {
                return GetPokerPosition(x, 0);
            }
            else
            {
                x -= 3;
                return GetPokerPosition(x % 5, x / 5 + 1);
            }
        }

    

        /// <summary>
        /// 扑克的位置
        /// </summary>
        /// <param name="indexX">扑克的列数</param>
        /// <param name="indexY">扑克的行数</param>
        /// <returns></returns>
        Vector3 GetPokerPosition(int indexX, int indexY)
        {
            int spaceX = -85;
            int spaceY = -120;
            int minX = indexY == 0 ? 1 : 2;

            return new Vector3((indexX - minX) * spaceX, (indexY - 1) * spaceY, 0);
        }



        public override void OnClickCard(PokerCard card)
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

            int index1 = CardsList.IndexOf(_selectPok1);
            int index2 = CardsList.IndexOf(_selectPok2);
            List<int> tempList = new List<int>(_lastList);
            int tempInt = tempList[index1];
            tempList[index1] = tempList[index2];
            tempList[index2] = tempInt;

            ExchangePokersPosition(_selectPok1, _selectPok2);
            var tempObj = CardsList[index1];
            CardsList[index1] = CardsList[index2];
            CardsList[index2] = tempObj;
            _lastList = tempList;

            foreach (ChoiseItem item in _choiseItems)
            {
                item.SelectMark.SetActive(false);
            }
            _lastChoiseItem = null;
            ResetSelectedCards();
        }

        void ResetSelectedCards()
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

        public void OnClickNothing()
        {
            ResetSelectedCards();
        }

        /// <summary>
        /// 更换两张牌的位置
        /// </summary>
        public void ExchangePokersPosition(PokerCard poker1, PokerCard poker2)
        {

            if (poker1 == null || poker2 == null)
                return;
            Facade.Instance<MusicManager>().Play("select");
            Vector3 tempPos = poker1.transform.localPosition;
            poker1.MoveTo(poker1.transform.localPosition, poker2.transform.localPosition);
            poker2.MoveTo(poker2.transform.localPosition, tempPos);
        }

        /// <summary>
        /// 查看是否有倒水
        /// </summary>
        /// <param name="list"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private bool NoDaoShui(List<int> list,ref string errorMsg)
        {

            List<int> tempList2 = list.GetRange(8, 5);
            List<int> tempList1 = list.GetRange(3, 5);
            int result = MatchListType(tempList2, 2, tempList1, 1);

            if (result < 0)   //第二、三行倒水
            {
                errorMsg = "第2,3行出现倒水";
                return false;
            }
            
            List<int> tempList0 = list.GetRange(0, 3);
            result = MatchListType(tempList1, 1, tempList0, 0);

            if (result < 0)      //第一、二行倒水
            {
                errorMsg = "第1,2行出现倒水";
                return false;
            }

            return true;        //没有倒水,可以更换
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
            Help1.VnList vnList1 = new Help1.VnList(list1);
            Help1.VnList vnList2 = new Help1.VnList(list2);
            CardType type1 = CheckCardType(list1, line1);
            CardType type2 = CheckCardType(list2, line2);

            YxDebug.Log(string.Format(" === 第{0}行牌型为{1} , 第{2}行牌型为{3} === ", line1, type1, line2, type2));

            int result = (int)type1 - (int)type2;
            if (result == 0)
            {
                for (int i = 0; i < vnList2.Count; i++)
                {
                    if (vnList1[i].Val != vnList2[i].Val)
                    {
                        return vnList1[i].Val.CompareTo(vnList2[i].Val);
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// 根据牌面值对列表进行排序
        /// </summary>
        /// <param name="list">要排序的牌列表</param>
        /// <param name="mode">1为升序,-1为降序,默认升序</param>
        void Resort(List<int> list, int mode = 1)
        {
            list.Sort((x, y) => mode * Help1.GetValue(x).CompareTo(Help1.GetValue(y)));
        }

        /// <summary>
        /// 检测牌型
        /// </summary>
        /// <param name="cards">牌</param>
        /// <param name="line">第几行</param>
        /// <returns></returns>
        public CardType CheckCardType(List<int> cards, int line)
        {
            //除去可能产生的不必要的信息
            cards = cards.GetRange(0, line == 0 ? 3 : 5);

            //检测牌型
            bool isTongHua = line != 0;
            bool isShunZi = line != 0;
            int samecount = 1;          
            List<int> sameNumList = new List<int>();
            Resort(cards);
            int color = Help1.GetColor(cards[0]);
            int value = Help1.GetValue(cards[0]);
            for (int i = 1; i < cards.Count; i++)
            {
                int card = cards[i];
                if (card == 0)
                    continue;
                int cardColor = Help1.GetColor(card);
                int cardValue = Help1.GetValue(card);
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


        public void OnClickOkBtn()
        {

            ISFSObject obj = new SFSObject();
            ISFSArray arr = new SFSArray();
            string errorMsg = string.Empty;
            if (_lastChoiseItem != null && _lastChoiseItem.Special > 0)
            {
                obj.PutInt("special", _lastChoiseItem.Special);
            }
            else
            {
                if (!NoDaoShui(_lastList,ref errorMsg))
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

            int beginIndex = 0;
            for (int i = 0; i < 3; i++)
            {
                int count = i == 0 ? 3 : 5;
                List<int> tempList = _lastList.GetRange(beginIndex, count);
                ISFSObject arrItem = new SFSObject();
                arrItem.PutInt("type", (int)CheckCardType(tempList, i));
                Help1.SortList(tempList);
                arrItem.PutIntArray("cards", tempList.ToArray());
                arr.AddSFSObject(arrItem);
                beginIndex += count;
            }

            obj.PutSFSArray("duninfo", arr);

            obj.PutInt("type", GameRequestType.FinishChoise);

            App.GetRServer<SssGameServer>().SendGameRequest(obj);
        }


        public override void Reset()
        {
            _selectPok1 = null;
            _selectPok2 = null;
            _lastList = null;

            foreach (var card in CardsList)
            {
                var pokerCard = card.GetComponent<PokerCard>();
                pokerCard.SetCardId(0);
                pokerCard.SetCardFront();

            }
            _curCardValList.Clear();

            foreach (ChoiseItem item in _choiseItems)
            {
                if (item != null)
                    item.Reset();
            }
        }
    }
}