using System;
using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.InheritCommon;
using JetBrains.Annotations;
using UnityEngine;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.PokerCdCtrl
{
    /// <summary>
    /// 手牌控制器单列类
    /// </summary>
    public class HdCdsCtrl : MonoBehaviour
    {
        /// <summary>
        /// 标记没有任何牌值
        /// </summary>
        public const int NoneCdValue = 0;
        /// <summary>
        /// 小王
        /// </summary>
        public const int SmallJoker = 0x51;
        /// <summary>
        /// 大王
        /// </summary>
        public const int BigJoker = 0x61;
        /// <summary>
        /// 赖子
        /// </summary>
        public const int MagicKing = 0x71;

        /// <summary>
        /// 当松开手指后获取的那个牌值
        /// </summary>
        [HideInInspector]
        public int RelseseCdValue;
        /// <summary>
        /// 当按下时获取的那个牌值
        /// </summary>
        [HideInInspector]
        public int PressCdValue;
        /// <summary>
        /// 当滑动手指时经过的那个牌值
        /// </summary>
        [HideInInspector]
        public int DragOverCdValue;


        /// <summary>
        /// 当dragover某个牌
        /// </summary>
        /// <param name="isFinishSelect">是否已经阔选完毕</param>
        public void OnDargOverCd(bool isFinishSelect = false)
        {
            var totalLen = _handCdsGobList.Count;
            for (int i = 0; i < totalLen; i++)
            {
                _handCdsGobList[i].GetComponent<PokerCardItem>().ResetCdColor();
            }
            //如果只单选了一张牌
            if (isFinishSelect && PressCdValue == RelseseCdValue && DragOverCdValue == NoneCdValue)
            {
                ClickOnCd(PressCdValue);
                OnFinishSelectEvent();

                //reset所有参数值
                RelseseCdValue = NoneCdValue;
                PressCdValue = NoneCdValue;
                DragOverCdValue = NoneCdValue;

                return;
            }

            var gobs = GetSelcedSomeCds(PressCdValue, DragOverCdValue);
            if (gobs == null) return;
            var len = gobs.Length;
            for (int i = 0; i < len; i++)
            {
                if (!isFinishSelect)
                {
                    gobs[i].GetComponent<PokerCardItem>().ChangeCdDark();
                }
                else
                {
                    gobs[i].GetComponent<PokerCardItem>().IsCdUp = !gobs[i].GetComponent<PokerCardItem>().IsCdUp;
                }
            }

            //如果选择完毕后，查找被选择的牌值，并触发相应事件
            if (isFinishSelect)
            {
                OnFinishSelectEvent();

                //reset所有参数值
                RelseseCdValue = NoneCdValue;
                PressCdValue = NoneCdValue;
                DragOverCdValue = NoneCdValue;
            }
        }

        /// <summary>
        /// 工具_intList
        /// </summary>
        private readonly List<int> _intList = new List<int>();
        /// <summary>
        /// 如果选择完毕后，查找被选择的牌值，并触发相应事件
        /// </summary>
        private void OnFinishSelectEvent()
        {
            var len = _handCdsGobList.Count;
            _intList.Clear();
            for (int i = 0; i < len; i++)
            {
                var pokerCdItem = _handCdsGobList[i].GetComponent<PokerCardItem>();
                if (pokerCdItem != null && pokerCdItem.IsCdUp)
                {
                    _intList.Add(pokerCdItem.CdValue);
                }
            }

            Facade.EventCenter.DispatchEvent(GlobalConstKey.KeyHdCds, new HdCdCtrlEvtArgs(_intList.ToArray()));
        }

        /// <summary>
        /// 当判断只选择了一个牌时，直接做牌抬起或者放下的变化
        /// </summary>
        /// <param name="cdValue">被选择的牌的牌值</param>
        private void ClickOnCd(int cdValue)
        {
            var cdItemGob = _handCdsGobList.Find(x => x.GetComponent<PokerCardItem>().CdValue == cdValue);
            cdItemGob.GetComponent<PokerCardItem>().IsCdUp = !cdItemGob.GetComponent<PokerCardItem>().IsCdUp;
        }

        /// <summary>
        /// 查找被选中的手牌
        /// </summary>
        /// <param name="cdStart">选牌开始点的牌</param>
        /// <param name="cdEnd">选牌结束点的牌</param>
        private GameObject[] GetSelcedSomeCds(int cdStart, int cdEnd)
        {
            //只扩选了一张牌
            if (cdStart == cdEnd)
            {
                var cdItemGob = _handCdsGobList.Find(x => x.GetComponent<PokerCardItem>().CdValue == cdStart);
                return new[] { cdItemGob };
            }

            //一下是扩选了多张牌的情况
            var pureSt = GetValue(cdStart);
            var pureEd = GetValue(cdEnd);

            //当起始点牌值小于结束点牌大小
            if (pureSt < pureEd)
            {
                return ChoseCds(cdStart, cdEnd);
            }

            //当起始点牌值大于结束点牌大小
            if (pureSt > pureEd)
            {
                return ChoseCds(cdEnd, cdStart);
            }

            //当起始点和牌值大小等于结束点的牌值大小,比如:3个不同花色的A
            return ChoseCds(cdStart, cdEnd) ?? ChoseCds(cdEnd, cdStart);
        }

        /// <summary>
        /// 从牌表某个值域中把牌找出来
        /// </summary>
        /// <param name="cdSmall">小牌起始点,但这个牌值是有花色信息的</param>
        /// <param name="cdBig">大牌终止点，但这个牌值是有花色信息的</param>
        /// <returns></returns>
        private GameObject[] ChoseCds(int cdSmall, int cdBig)
        {
            var handCdsLen = _handCdsGobList.Count;
            var goblistTemp = new List<GameObject>();
            for (int i = 0; i < handCdsLen; i++)
            {
                if (_handCdsGobList[i].GetComponent<PokerCardItem>().CdValue == cdBig)
                {
                    goblistTemp.Add(_handCdsGobList[i]);
                    for (int j = i + 1; j < handCdsLen; j++)
                    {
                        goblistTemp.Add(_handCdsGobList[j]);
                        if (_handCdsGobList[j].GetComponent<PokerCardItem>().CdValue == cdSmall)
                        {
                            return goblistTemp.ToArray();
                        }
                    }
                    break;
                }
            }
            return null;
        }


        /// <summary>
        /// 创造手牌时的原型
        /// </summary>
        [SerializeField]
        protected GameObject CdItemOrg;
        /// <summary>
        /// 手牌gobs
        /// </summary>
        private readonly List<GameObject> _handCdsGobList = new List<GameObject>();
        /// <summary>
        /// 放手牌的grid
        /// </summary>
        [SerializeField, UsedImplicitly]
        private UIGrid _hdGrid;

        public bool HaveHandCard
        {
            get { return _hdGrid.transform.childCount > 0; }
        }


        /*        // Use this for initialization
                void Start ()
                {

                    var cds = new int[] { 0x13, BigJoker, 0x34, 0x35, 0x15, SmallJoker, 0x36, 0x37, 0x18, };
                    AllocateCds(cds);
                }*/

        /// <summary>
        /// 当需要重置手牌时，删除之前grid中牌，同时按照实际手牌数据重新发牌
        /// </summary>
        /// <param name="newhdCds"></param>
        public void ReSetHandCds(int[] newhdCds)
        {
            if (newhdCds == null) return;
            CleanHandCards();
            AllocateCds(newhdCds);
        }

        /// <summary>
        /// 发牌有动画
        /// </summary>
        /// <param name="newhdCds"></param>
        public void ResetHandCdsWithAnim(int[] newhdCds)
        {
            if (newhdCds == null) return;
            CleanHandCards();
            AllocateCdsWithAnim(newhdCds);
        }

        /// <summary>
        /// 清除手上的手牌
        /// </summary>
        void CleanHandCards()
        {
            _hdGrid.transform.DestroyChildren();       //清楚已有手牌
            _handCdsGobList.Clear();
        }



        /// <summary>
        /// 移除某些手牌
        /// </summary>
        /// <param name="losecds"></param>
        public void RemoveHdCds(int[] losecds)
        {
            var len = losecds.Length;
            for (int i = 0; i < len; i++)
            {
                var gob = _handCdsGobList.Find(x => x.GetComponent<PokerCardItem>().CdValue == losecds[i]);
                _handCdsGobList.Remove(gob);
                DestroyImmediate(gob);
            }
            _hdGrid.repositionNow = true;
            _hdGrid.Reposition();
        }

        /// <summary>
        /// 给手牌发牌,无过程
        /// </summary>
        /// <param name="cds"></param>
        public void AllocateCds(int[] cds)
        {
            if (cds == null || cds.Length < 1)
            {
                return;
            }

            cds = SortCds(cds);
            for (int i = 0; i < cds.Length; i++)
            {
                AddHandCdGob(CdItemOrg, 2 * i + 100, cds[i]);
            }
            _hdGrid.repositionNow = true;
            _hdGrid.Reposition();
        }

        public void AllocateCdsWithAnim(int[] cds)
        {
            if (cds.Length < 1) return;
            cds = SortCds(cds);

            int len = cds.Length;
            _cards = new int[len];
            Array.Copy(cds, _cards, len);
            _index = 0;

            CreatOneMoveCard();
        }


        private void CreatOneMoveCard()
        {
            var gob = AddHandCdGob(CdItemOrg, _index, _cards[_index]);
            gob.name = string.Format("ShiftCard{0}", _index);

            var card = gob.GetComponent<ShiftCard>();
            card.OnCardLeftMove = new EventDelegate(OnCardLeftMove);
            card.SetLastObj(_index == 0 ? null : _handCdsGobList[_index - 1]);

            Facade.EventCenter.DispatchEvent(GlobalConstKey.PlaySound, "k_give");
        }


        private void OnCardLeftMove()
        {
            _index++;
            if (_index < _cards.Length)
            {
                CreatOneMoveCard();
            }
            else
            {
                for (int i = 0; i < _index; i++)
                {
                    var card = _handCdsGobList[i].GetComponent<ShiftCard>();
                    card.CancelMove();
                }
            }
        }

        private int[] _cards;

        private int _index;

        private GameObject AddHandCdGob(GameObject cdItemOrg, int layerIndex, int cdValueData)
        {
            var gob = _hdGrid.gameObject.AddChild(cdItemOrg);
            gob.name = "gob" + layerIndex;
            _handCdsGobList.Add(gob);
            gob.SetActive(true);
            var pokerItem = gob.GetComponent<PokerCardItem>();
            pokerItem.SetHdcdctrlInstance(this);
            pokerItem.SetLayer(layerIndex * 2 + 100);
            pokerItem.SetCdValue(cdValueData);
            return gob;
        }

        /// <summary>
        /// 排序一组牌的值，从大到小排序
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public static int[] SortCds(IEnumerable<int> cards)
        {
            if (cards == null) return null;

            var list = new List<int>();
            list.AddRange(cards);
            list.Sort((x, y) => GetValue(y).CompareTo(GetValue(x)));
            return list.ToArray();
        }

        /// <summary>
        /// 解析一个扑克的牌值
        /// </summary>
        /// <param name="cdValueData">带花色的手牌信息</param>
        /// <returns></returns>
        public static int GetValue(int cdValueData)
        {
            if (cdValueData == SmallJoker || cdValueData == BigJoker
                || cdValueData == MagicKing)
            {
                return cdValueData;
            }

            cdValueData = cdValueData & 0xf;
            return cdValueData;
        }

        /// <summary>
        /// 仅仅抬起某个牌值的手牌
        /// </summary>
        /// <param name="pureCdvalue">单纯牌大小的值</param>
        public void JustUpCd(int pureCdvalue)
        {
            var cdItemGob = _handCdsGobList.Find(x =>
                {
                    var poker = x.GetComponent<PokerCardItem>();
                    var thisCdPure = GetValue(x.GetComponent<PokerCardItem>().CdValue);

                    return poker.IsCdUp == false && thisCdPure == pureCdvalue;
                }
               );
            cdItemGob.GetComponent<PokerCardItem>().IsCdUp = true;
        }

        /// <summary>
        /// 重置所有手牌的回原位置
        /// </summary>
        public void RepositionAllHdCds()
        {
            foreach (var cdGob in _handCdsGobList)
            {
                cdGob.GetComponent<PokerCardItem>().IsCdUp = false;
            }
        }

        /// <summary>
        /// 得到手牌中抬起的牌的牌值组（包含花色信息）
        /// </summary>
        /// <returns></returns>
        public List<int> GetUpCdList()
        {
            List<int> seletedCards = new List<int>();
            for (int i = 0; i < _handCdsGobList.Count; i++)
            {
                var card = _handCdsGobList[i];
                var item = card.GetComponent<PokerCardItem>();
                if (item.IsCdUp) seletedCards.Add(item.CdValue);
            }
            return seletedCards;
        }

        /// <summary>
        /// 根据牌值（包括花色）抬起某些牌，
        /// </summary>
        /// <param name="upcds"></param>
        public void UpCdList(int[] upcds)
        {
            RepositionAllHdCds();
            var len = upcds.Length;
            for (int i = 0; i < len; i++)
            {
                var cdItemGob = _handCdsGobList.Find(x => x.GetComponent<PokerCardItem>().CdValue == upcds[i]);
                cdItemGob.GetComponent<PokerCardItem>().IsCdUp = true;
            }
        }

        public bool CheckLuckyCards()
        {
            //_cards 正常情况是用
            //_handCdsGobList 重连时使用，因为重连时 _cards 是 null

            var list = new List<int>();
            if (_cards != null)
            {
                for (int i = 0; i < _cards.Length; i++)
                {
                    if (CheckCard(_cards[i]))
                    {
                        list.Add(_cards[i]);
                    }
                }
            }
            else
            {
                for (int i = 0; i < _handCdsGobList.Count; i++)
                {
                    var card = _handCdsGobList[i].GetComponent<PokerCardItem>().CdValue;
                    if (CheckCard(card))
                    {
                        list.Add(card);
                    }
                }
            }

            //检查2王
            if (CheckCardModel(list, 2, (card) => card == SmallJoker || card == BigJoker))
            {
                return true;
            }
            //检查4个2
            if (CheckCardModel(list, 4, (card) => card == 0x1F || card == 0x2F || card == 0x3F || card == 0x4F))
            {
                return true;
            }
            return false;
        }

        private bool CheckCard(int card)
        {
            return card == 0x1F || card == 0x2F || card == 0x3F || card == 0x4F || card == SmallJoker || card == BigJoker;
        }

        private bool CheckCardModel(List<int> cards, int result, Func<int, bool> expression)
        {
            var value = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                if (expression(cards[i])) value++;
            }
            return value >= result;
        }
    }

    /// <summary>
    /// 手牌控制eventhandler 相关参数
    /// </summary>
    public class HdCdCtrlEvtArgs : EventArgs
    {
        /// <summary>
        /// 被选中的手牌的牌值列表（包括花色）
        /// </summary>
        public readonly int[] SelectedCds;

        /// <summary>
        /// 已经选择了的手牌的构造方法
        /// </summary>
        /// <param name="selCds"></param>
        public HdCdCtrlEvtArgs(int[] selCds)
        {
            SelectedCds = selCds;
        }
    }
}
