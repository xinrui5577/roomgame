using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Mahjong2D.CommonFile.Data;
using Assets.Scripts.Game.Mahjong2D.Game.Data;
using Assets.Scripts.Game.Mahjong2D.Game.GameCtrl;
using Assets.Scripts.Game.Mahjong2D.Game.Item;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Common.MahJongCommon
{
    /// <summary>
    /// 麻将层(UI)处理,包括手牌，组牌，打牌三个部分，墙牌是特殊计算,只负责UI处理，不存储数据,
    /// 麻将数据放在对应的plyer中，所见即所得原则
    /// </summary>
    public class DefEnvpanel : MonoBehaviour
    {
        /// <summary>
        /// 手牌
        /// </summary>
        [SerializeField]
        private HandPile _handPile;
        /// <summary>
        /// 组牌
        /// </summary>
        [SerializeField]
        private GroupPile _groupPile;
        /// <summary>
        /// 打牌
        /// </summary>
        [SerializeField]
        private OutPile _outPile;
        /// <summary>
        /// 所有的存在标记的麻将
        /// </summary>
        private List<Transform> _flagItems=new List<Transform>();
        /// <summary>
        /// 打出飞牌时间
        /// </summary>
        [SerializeField]
        public float ThrowCardTime = 1;
        /// <summary>
        /// 移动到牌堆之前的等待的时间
        /// </summary>
        [SerializeField]
        public float MotoOutCardsTime = 0;
        [Tooltip("自动状态打牌速率")]
        public float AutoTimeRate = 1f;
        [Tooltip("打牌自动落下，默认值为false")]
        public bool ThrowCardAutoDown = false;
        /// <summary>
        /// 打出的最后一张牌
        /// </summary>
        public MahjongItem LastOutMahjong
        {
            get
            {
                if (_outPile.OutCard.ThrowCard!=null)
                {
                    return _outPile.OutCard.ThrowCard;
                }
                else
                {
                    if (ThrowCardAutoDown)
                    {
                        var lastItem = _outPile.Layout.GetLastItem();
                        if (lastItem!=null)
                        {
                            return lastItem.GetComponent<MahjongItem>();
                        }
                    }
                }
                return null;
            }
            set
            {
                _outPile.OutCard.ThrowCard = value;
            }
        }

        public MahjongItem LastGetInMahjong
        {
            get
            {
                if(_handPile.NewCard.GetIn!=null)
                {
                    return _handPile.NewCard.GetIn;
                }
                return null;
            }
            set { _handPile.NewCard.GetIn = value; }
        }

        public List<Transform> HandCards
        {
            get
            {
                List<Transform> pileList = _handPile.GetList();
                if (LastGetInMahjong!=null)
                {
                    pileList.Add(LastGetInMahjong.transform);
                }
                return pileList;
            }
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

        #region 通用
        /// <summary>
        /// 手牌或组牌中某些牌
        /// </summary>
        /// <param name="pile"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<Transform> FindCardInPiles(MahjongPile pile, int value)
        {
            List<Transform> trans = pile.GetList();
            trans = trans.FindAll(item => item.GetComponent<MahjongItem>().Value.Equals(value));
            return trans;
        }
        /// <summary>
        /// 查找打出的牌中与组牌中与选中牌面相同的值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<Transform> FindOutCardByValue(int value)
        {
            List<Transform> outCards = new List<Transform>();
            outCards.AddRange(FindCardInPiles(_outPile, value));
            outCards.AddRange(FindCardInGroups(value));
            _flagItems = outCards;
            return outCards;
        }
        /// <summary>
        /// 清除标记
        /// </summary>
        public void ClearMahjongFlag()
        {
            if (_flagItems==null||_flagItems.Count==0)
            {
                return;
            }
            for (int i = 0,lenth= _flagItems.Count; i < lenth; i++)
            {
                if (_flagItems[i]!=null)
                {
                    MahjongItem item = _flagItems[i].GetComponent<MahjongItem>();
                    if (item != null)
                    {
                        item.SetColor(Color.white);
                    }
                }
            }
            _flagItems.Clear();
        }

        public void ClearHandFlag()
        {
            List<Transform> hands = HandCards;
            foreach (var trans in hands)
            {
                MahjongItem item = trans.GetComponent<MahjongItem>();
                if (item!=null)
                {
                    item.SetColor(Color.white);
                }
            }
            if (_handPile.NewCard.GetIn != null)
            {
              _handPile.NewCard.GetIn.GetComponent<MahjongItem>().SetColor(Color.white);
            }
        }

        #endregion

        #region 手牌

        /// <summary>
        /// 添加手牌
        /// </summary>
        /// <param name="value">牌值</param>
        /// <param name="isSingle">是否为单张处理</param>
        /// <param name="changeNum">获得牌后是否要刷新牌数显示</param>
        public virtual Transform AddHandCard(int value, bool isSingle,bool changeNum=true)
        {
            return _handPile.AddCard(value, isSingle,changeNum);
        }

        /// <summary>
        /// 添加手牌
        /// </summary>
        public virtual void AddHandCard(MahjongItem item)
        {
            _handPile.AddItem(item.transform);
        }

        /// <summary>
        /// 亮倒手牌
        /// </summary>
        public void ShowHandCard()
        {
            if (_handPile.NewCard.GetIn!=null)
            {
                _handPile.NewCard.GetIn.SelfData.Action=EnumMahJongAction.Lie;
            }
            _handPile.ChangeAction(EnumMahJongAction.Lie);
            ClearHandFlag();
        }
        /// <summary>
        /// 获得一张手牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public MahjongItem GetHandCardByValue(int value)
        {
            return _handPile.GetCardByValue(value);
        }

        /// <summary>
        /// 随机获得一张手牌，为了保持其它几家看起来显示一致，更具对应的value进行处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public MahjongItem GetHandItem(int value)
        {
            List<Transform> list = _handPile.GetList();
            int lenth = list.Count;
            if (lenth==0)
            {
                Debug.LogError("手牌为0");
                return null;
            }
            return list[value%lenth].GetComponent<MahjongItem>();
        }
        /// <summary>
        /// 获取对应数量的手牌
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<MahjongItem> GetHandItems(int num)
        {
            List<Transform> list = _handPile.GetList();
            List<MahjongItem> mahJongItems=new List<MahjongItem>();
            for (int i = 0; i < num; i++)
            {
                mahJongItems.Add(list[i].GetComponent<MahjongItem>());
            }
            return mahJongItems;
        }

        public void SetItemAsNewGetIn(int value)
        {
            if (_handPile)
            {
                _handPile.SetMahjongAsNewGetIn(GetHandCardByValue(value));
                _handPile.ResetPosition();
            }
        }

        public void SetHandCardsInvalid()
        {
            List<Transform> trans = _handPile.GetList();
            SetItemsColor(Color.gray, trans);
            foreach (Transform tran in trans)
            {
               GameTools.DestroyUserContorl(tran);
            }
            if (LastGetInMahjong!=null)
            {
                LastGetInMahjong.SetColor(Color.gray);
                GameTools.DestroyUserContorl(LastGetInMahjong);
            }
        }

        public void SetItemsColor(Color color,List<Transform> mahjongs)
        {
            foreach (var item in mahjongs)
            {
                var mahjongItem = item.GetComponent<MahjongItem>();
                if (mahjongItem)
                {
                    mahjongItem.SetColor(color);
                }
            }
        }

  

        public bool OnGetNewCard()
        {
            return _handPile.NewCard.GetIn != null;
        }

        public void SetHandCardPush(bool push)
        {
            _handPile.ChangeAction(push? EnumMahJongAction.Push: EnumMahJongAction.StandNo);
        }

        /// <summary>
        /// 一次添加多张手牌
        /// </summary>
        /// <param name="handCards"></param>
        public virtual void AddHandCards(List<int> handCards)
        {
            for (int i = 0, lenth = handCards.Count; i < lenth; i++)
            {
                _handPile.AddCard(handCards[i],false);
            }
            _handPile.Layout.ResetPositionNow = true;
        }

        /// <summary>
        /// 刷新手牌
        /// </summary>
        /// <param name="values"></param>
        public virtual void RefreshHandList(List<int> values)
        {
            List<Transform> items = _handPile.Layout.GetChildList();
            for (int i = 0,lenth=items.Count; i <lenth; i++)
            {
                MahjongItem item=items[i].GetComponent<MahjongItem>();
                item.Value = values[i];
                item.JudgeHunTag(App.GetGameManager<Mahjong2DGameManager>().LaiZiNum);
            }
            _handPile.ResetPosition();
        }
        /// <summary>
        /// 清除手牌状态（颜色标记）
        /// </summary>
        public virtual void ClearHandMahjongState()
        {
            List<Transform> items = _handPile.Layout.GetChildList();
            for (int i = 0, lenth = items.Count; i < lenth; i++)
            {
                MahjongItem item = items[i].GetComponent<MahjongItem>();
                item.SetColor(Color.white);
            }
        }

        #endregion
        #region 组牌

        /// <summary>
        /// 添加一个组牌
        /// </summary>
        /// <param name="data"></param>
        /// <param name="items"></param>
        /// <param name="isOther"></param>
        public virtual MahjongGroupItem AddGroup(MahjongGroupData data,List<MahjongItem> items ,bool isOther)
        {
            MahjongGroupItem group= _groupPile.AddGroup(data,items,isOther);
            _groupPile.ResetPosition();
            return group;
        }
        /// <summary>
        /// 更改一个组牌
        /// </summary>
        /// <param name="value"></param>
        /// <param name="addItem"></param>
        public virtual void ChangeGroup(int value,MahjongItem addItem)
        {
            _groupPile.ChangeGroup(value,addItem);
        }

        public virtual MahjongGroupItem GetGroupByValue(int value)
        {
            return _groupPile.GetGroupItem(value);
        }

        #endregion
        #region 打出的牌
        public virtual void SetOutCardsWidth(int lenth)
        {
            _outPile.Layout.SetLayoutLenth(lenth);
        }

        /// <summary>
        /// 某个玩家打出一张牌,这个打出是到达打出位置
        /// </summary>
        /// <param name="item"></param>
        /// <param name="direct"></param>
        /// <param name="isAuto"></param>
        /// <param name="moveTime"></param>
        public virtual void ThrowOutCard(MahjongItem item,bool direct=false,bool isAuto=false,float moveTime=0, AsyncCallback finishCall = null)
        {
            MahjongItem newGetItem = _handPile.NewCard.GetIn;//打牌的玩家一定新抓了一张牌，放到手牌中
            if (newGetItem != null)
            {
                if (item.Value != newGetItem.Value)
                {
                    _handPile.AddItem(newGetItem.transform);
                }
            }
            else
            {
                YxDebug.LogError("获得的最后一张牌是空的,应该是谁吃碰杠");
            }
            _handPile.NewCard.GetIn = null;
            if (item!=null)
            {
                if (direct)
                {
                    _outPile.ThrowOutCard(item, moveTime,finishCall);
                }
                else
                {
                    _outPile.ThrowOutCard(item, isAuto?ThrowCardTime*AutoTimeRate:ThrowCardTime, finishCall);//找一张牌打出去
                }
            }
            else
            {
                YxDebug.LogError("要打的牌是空的，什么情况？");
            }
        }
        /// <summary>
        /// 直接显示打出的那张牌
        /// </summary>
        public virtual void ThrowOutCardShow()
        {
            _outPile.ThrowOutCard();
        }

        /// <summary>
        /// 打牌到出牌牌堆中
        /// </summary>
        /// <param name="direct">是否直接落下</param>
        /// <param name="isAuto">是否处于自动状态（包括托管与上听状态）</param>
        public virtual void ThrowOutCardToOutCard(bool direct=false,bool isAuto=false)
        {
             _outPile.ThrowOutCardFinished(isAuto?MotoOutCardsTime* AutoTimeRate: MotoOutCardsTime, direct);
        }
        /// <summary>
        ///移动到打出牌
        /// </summary>
        /// <param name="item"></param>
        /// <param name="moveTime"></param>
        public virtual void MoveCardToOutPile(MahjongItem item,float moveTime)
        {
            _outPile.AddCardToPiles(item,moveTime);
        }

        /// <summary>
        /// 添加多张打出牌
        /// </summary>
        /// <param name="outCards"></param>
        public virtual void AddOutCards(List<int> outCards)
        {
            _outPile.AddItems(outCards);
        }
        /// <summary>
        /// 查找打出的某些牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual List<Transform> FindCardInGroups(int value)
        {
            List<Transform> trans = _groupPile.GetList();
            List<Transform> returnTrans=new List<Transform>();
            for (int i = 0,lenth=trans.Count; i < lenth; i++)
            {
                MahjongGroupItem group= trans[i].GetComponent<MahjongGroupItem>();
                List<MahjongItem> items = group.ItemList;
                int max = items.Count;
                if (max.Equals(4)) //碰的跳过
                {
                    continue;
                }
                for (int j = 0; j <max; j++)
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
        /// 直接显示最后一张打出的牌
        /// </summary>
        /// <param name="value"></param>
        public virtual void SetLastOutMahjongShow(int value)
        {
            if (!ThrowCardAutoDown)
            {
                MahjongItem item = FindLastMahjongItem(value);
                _outPile.ThrowOutCard(item, 0);
                item.SelfData.ShowDirection = EnumShowDirection.Self;
            }
        }

        public void SetLastInCardOnReJoin(int value)
        {
            _handPile.SetGetCardItem(value);
        }

        public MahjongItem FindLastMahjongItem(int value)
        {
            MahjongItem item = FindLastOutCardInGroup();
            if (item.Value.Equals(value))
            {
                return item;
            }
            else
            {
                YxDebug.LogError("LastItem value is wrong");
                return null;
            }
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
