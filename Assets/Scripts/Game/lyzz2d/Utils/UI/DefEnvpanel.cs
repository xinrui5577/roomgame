using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Game;
using Assets.Scripts.Game.lyzz2d.Game.Data;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Utils.UI
{
    /// <summary>
    ///     麻将层(UI)处理,包括手牌，组牌，打牌三个部分，墙牌是特殊计算,只负责UI处理，不存储数据,
    ///     麻将数据放在对应的plyer中，所见即所得原则
    /// </summary>
    public class DefEnvpanel : MonoBehaviour
    {
        /// <summary>
        ///     所有的存在标记的麻将
        /// </summary>
        private List<Transform> _flagItems = new List<Transform>();

        /// <summary>
        ///     组牌
        /// </summary>
        [SerializeField] private GroupPile _groupPile;

        /// <summary>
        ///     手牌
        /// </summary>
        [SerializeField] private HandPile _handPile;

        /// <summary>
        ///     打牌
        /// </summary>
        [SerializeField] private OutPile _outPile;

        [SerializeField]
        /// <summary>
        /// 移动到牌堆的时间
        /// </summary>
        public float MotoOutCardsTime = 1;

        [SerializeField]
        /// <summary>
        /// 打出飞牌时间
        /// </summary>
        public float ThrowCardTime = 1;

        /// <summary>
        ///     打出的最后一张牌
        /// </summary>
        public MahjongItem LastOutMahjong
        {
            get
            {
                if (_outPile.OutCard.ThrowCard != null)
                {
                    return _outPile.OutCard.ThrowCard;
                }
                return null;
            }
            set { _outPile.OutCard.ThrowCard = value; }
        }

        public MahjongItem LastGetInMahjong
        {
            get
            {
                if (_handPile.NewCard.GetIn != null)
                {
                    return _handPile.NewCard.GetIn;
                }
                return null;
            }
            set { _handPile.NewCard.GetIn = value; }
        }

        #region 整体处理

        public virtual void Reset(bool isOther)
        {
            if (_handPile)
            {
                _handPile.ResetPile();
            }
            if (_groupPile)
            {
                _groupPile.ResetPile();
            }
            if (_outPile)
            {
                _outPile.ResetPile();
            }
            if (isOther)
            {
                if (_handPile.NewCard.GetIn != null)
                {
                    _handPile.NewCard.GetIn.SelfData.Action = EnumMahJongAction.StandNo;
                }
                _handPile.ChangeAction(EnumMahJongAction.StandNo);
            }
            else
            {
                if (_handPile.NewCard.GetIn != null)
                {
                    _handPile.NewCard.GetIn.SelfData.Action = EnumMahJongAction.StandWith;
                }
                _handPile.ChangeAction(EnumMahJongAction.StandWith);
            }
        }

        #endregion

        public void ClearHandFlag()
        {
            var hands = _handPile.GetList();
            foreach (var trans in hands)
            {
                var item = trans.GetComponent<MahjongItem>();
                if (item != null)
                {
                    item.SetColor(Color.white);
                }
            }
            if (_handPile.NewCard.GetIn != null)
            {
                _handPile.NewCard.GetIn.GetComponent<MahjongItem>().SetColor(Color.white);
            }
        }

        #region 通用

        /// <summary>
        ///     手牌或组牌中某些牌
        /// </summary>
        /// <param name="pile"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<Transform> FindCardInPiles(MahjongPile pile, int value)
        {
            var trans = pile.GetList();
            trans = trans.FindAll(item => item.GetComponent<MahjongItem>().Value.Equals(value));
            return trans;
        }

        /// <summary>
        ///     查找打出的牌中与组牌中与选中牌面相同的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<Transform> FindOutCardByValue(int value)
        {
            var _outCards = new List<Transform>();
            _outCards.AddRange(FindCardInPiles(_outPile, value));
            _outCards.AddRange(FindCardInGroups(value));
            _flagItems = _outCards;
            return _outCards;
        }

        /// <summary>
        ///     清除标记
        /// </summary>
        public void ClearMahjongFlag()
        {
            if (_flagItems == null || _flagItems.Count == 0)
            {
                return;
            }
            for (int i = 0, lenth = _flagItems.Count; i < lenth; i++)
            {
                if (_flagItems[i] != null)
                {
                    var item = _flagItems[i].GetComponent<MahjongItem>();
                    if (item != null)
                    {
                        item.SetColor(Color.white);
                    }
                }
            }
            _flagItems.Clear();
        }

        public void SetHandCardsColor(Color color, bool removeControl)
        {
            var trans = _handPile.GetList();
            foreach (var tran in trans)
            {
                var item = tran.GetComponent<MahjongItem>();
                if (item != null)
                {
                    item.SetColor(color);
                    if (removeControl)
                    {
                        GameTools.DestroyUserContorl(item);
                    }
                }
            }
        }

        #endregion

        #region 手牌

        /// <summary>
        ///     添加手牌
        /// </summary>
        /// <param name="pile"></param>
        /// <param name="value"></param>
        public virtual Transform AddHandCard(int value, bool isSingle, bool changeNum = true)
        {
            return _handPile.AddCard(value, isSingle, changeNum);
        }

        /// <summary>
        ///     添加手牌
        /// </summary>
        /// <param name="pile"></param>
        /// <param name="value"></param>
        public virtual void AddHandCard(MahjongItem item)
        {
            _handPile.AddItem(item.transform);
        }

        /// <summary>
        ///     亮倒手牌
        /// </summary>
        public void ShowHandCard()
        {
            if (_handPile.NewCard.GetIn != null)
            {
                _handPile.NewCard.GetIn.SelfData.Action = EnumMahJongAction.Lie;
            }
            _handPile.ChangeAction(EnumMahJongAction.Lie);
            ClearHandFlag();
        }

        /// <summary>
        ///     获得一张手牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public MahjongItem GetHandCardByValue(int value)
        {
            return _handPile.GetCardByValue(value);
        }

        /// <summary>
        ///     随机获得一张手牌，为了保持其它几家看起来显示一致，更具对应的value进行处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public MahjongItem GetHandItem(int value)
        {
            var list = _handPile.GetList();
            var lenth = list.Count;
            return list[value%lenth].GetComponent<MahjongItem>();
        }

        /// <summary>
        ///     获取对应数量的手牌
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<MahjongItem> GetHandItems(int num)
        {
            var list = _handPile.GetList();
            var mahJongItems = new List<MahjongItem>();
            for (var i = 0; i < num; i++)
            {
                mahJongItems.Add(list[i].GetComponent<MahjongItem>());
            }
            return mahJongItems;
        }

        public bool OnGetNewCard()
        {
            return _handPile.NewCard.GetIn != null;
        }

        /// <summary>
        ///     一次添加多张手牌
        /// </summary>
        /// <param name="_handCards"></param>
        /// <param name="isAddControl"></param>
        public virtual void AddHandCards(List<int> _handCards)
        {
            for (int i = 0, lenth = _handCards.Count; i < lenth; i++)
            {
                _handPile.AddCard(_handCards[i], false);
            }
            _handPile.Layout.ResetPositionNow = true;
        }

        /// <summary>
        ///     刷新手牌
        /// </summary>
        /// <param name="values"></param>
        public virtual void RefreshHandList(List<int> values)
        {
            var items = _handPile.Layout.GetChildList();
            if (items.Count != values.Count)
            {
                YxDebug.LogError("UI数量与数据数量数量不一致，去看看");
                YxDebug.LogError(string.Format("玩家实际手牌数量（数据）{0},目前UI数量{1}", values.Count, items.Count));
                YxDebug.Log("-----------------------------------值----------------------------");
                foreach (var item in values)
                {
                    YxDebug.Log((EnumMahjongValue) item);
                }
                YxDebug.Log("-----------------------------------UI----------------------------");
                foreach (var item in items)
                {
                    YxDebug.Log((EnumMahjongValue) item.GetComponent<MahjongItem>().Value);
                }
            }
            for (int i = 0, lenth = items.Count; i < lenth; i++)
            {
                var item = items[i].GetComponent<MahjongItem>();
                item.Value = values[i];
                item.JudgeHunTag(App.GetGameManager<Lyzz2DGameManager>().LaiZiNum);
            }
            _handPile.Layout.ResetPositionNow = true;
        }

        /// <summary>
        ///     清除手牌状态（颜色标记）
        /// </summary>
        public virtual void ClearHandMahjongState()
        {
            var items = _handPile.Layout.GetChildList();
            for (int i = 0, lenth = items.Count; i < lenth; i++)
            {
                var item = items[i].GetComponent<MahjongItem>();
                item.SetColor(Color.white);
            }
        }

        #endregion

        #region 组牌

        /// <summary>
        ///     添加一个组牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isOther"></param>
        public virtual MahjongGroupItem AddGroup(MahjongGroupData data, List<MahjongItem> items, bool isOther)
        {
            var group = _groupPile.AddGroup(data, items, isOther);
            _groupPile.ResetPosition();
            return group;
        }

        /// <summary>
        ///     更改一个组牌
        /// </summary>
        /// <param name="value"></param>
        /// <param name="addItem"></param>
        public virtual void ChangeGroup(int value, MahjongItem addItem)
        {
            _groupPile.ChangeGroup(value, addItem);
        }

        public virtual MahjongGroupItem GetGroupByValue(int value)
        {
            return _groupPile.GetGroupItem(value);
        }

        #endregion

        #region 打出的牌

        /// <summary>
        ///     某个玩家打出一张牌,这个打出是到达打出位置
        /// </summary>
        /// <param name="item"></param>
        public virtual void ThrowOutCard(MahjongItem item)
        {
            YxDebug.Log("打出一张牌" + (EnumMahjongValue) item.Value);
            _outPile.ThrowOutCard(item, ThrowCardTime); //找一张牌打出去
            var NewGetItem = _handPile.NewCard.GetIn; //打牌的玩家一定新抓了一张牌，放到手牌中
            if (NewGetItem != null)
            {
                if (item.Value != NewGetItem.Value)
                {
                    _handPile.AddItem(NewGetItem.transform);
                }
            }
            _handPile.NewCard.GetIn = null;
        }

        /// <summary>
        ///     直接显示打出的那张牌
        /// </summary>
        public virtual void ThrowOutCardShow()
        {
            _outPile.ThrowOutCard();
        }

        public virtual void ThrowOutCardToOutCard()
        {
            _outPile.ThrowOutCardFinished(MotoOutCardsTime);
        }

        /// <summary>
        ///     添加多张打出牌
        /// </summary>
        /// <param name="_outCards"></param>
        public virtual void AddOutCards(List<int> _outCards)
        {
            _outPile.AddItems(_outCards);
        }

        /// <summary>
        ///     查找打出的某些牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual List<Transform> FindCardInGroups(int value)
        {
            var trans = _groupPile.GetList();
            var returnTrans = new List<Transform>();
            for (int i = 0, lenth = trans.Count; i < lenth; i++)
            {
                var group = trans[i].GetComponent<MahjongGroupItem>();
                var items = group.ItemList;
                var max = items.Count;
                if (max.Equals(4)) //碰的跳过
                {
                    continue;
                    ;
                }
                for (var j = 0; j < max; j++)
                {
                    if (items[j].Value.Equals(value))
                    {
                        returnTrans.Add(items[j].transform);
                    }
                }
            }
            return returnTrans;
        }

        /// <summary>
        ///     直接显示最后一张打出的牌
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetLastOutMahjongShow(int value)
        {
            var item = FindLastMahjongItem(value);
            _outPile.ThrowOutCard(item, 0);
            item.SelfData.ShowDirection = EnumShowDirection.Self;
        }

        public void SetLastInCardOnReJoin(int value)
        {
            var item = _handPile.GetLast().GetComponent<MahjongItem>();
            _handPile.SetGetCardItem(value);
        }

        public MahjongItem FindLastMahjongItem(int value)
        {
            var item = FindLastOutCardInGroup();
            if (item.Value.Equals(value))
            {
                return item;
            }
            YxDebug.LogError("LastItem value is wrong");
            return null;
        }

        public MahjongItem FindLastOutCardInGroup()
        {
            return _outPile.GetLast().GetComponent<MahjongItem>();
        }

        #endregion

        #region 断线重连

        #endregion
    }
}