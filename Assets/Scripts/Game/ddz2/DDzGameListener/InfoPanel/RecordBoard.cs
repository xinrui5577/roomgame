using System.Collections.Generic;
using Assets.Scripts.Game.ddz2.DdzEventArgs;
using Assets.Scripts.Game.ddz2.DDz2Common;
using Assets.Scripts.Game.ddz2.DDzGameListener.OneRoundResultPanel;
using Assets.Scripts.Game.ddz2.InheritCommon;
using UnityEngine;
using YxFramwork.ConstDefine;
using YxFramwork.Framework.Core;

namespace Assets.Scripts.Game.ddz2.DDzGameListener.InfoPanel
{
    public class RecordBoard : MonoBehaviour
    {
        /// <summary>
        /// 牌的副数
        /// </summary>
        [SerializeField]
        protected int Deck = 1;

        /// <summary>
        /// 数组偏移(显示的最小数字是几)
        /// </summary>
        [SerializeField]
        protected int MinNum = 3;

        ///// <summary>
        ///// 大王个数显示
        ///// </summary>
        //[SerializeField]
        //protected UILabel _bigKingCountLabel;

        ///// <summary>
        ///// 小王个数显示
        ///// </summary>
        //[SerializeField]
        //protected UILabel _littleKingCountLabel;


        /// <summary>
        /// 特殊颜色
        /// </summary>
        [SerializeField]
        protected Color SpecialColor;

        /// <summary>
        /// 普通颜色
        /// </summary>
        [SerializeField]
        protected Color NormalColor;


        /// <summary>
        /// 预制体
        /// </summary>
        [SerializeField]
        protected RecordItem ItemPrefab;


        [SerializeField]
        protected UIGrid ItemParent;

        public GameObject OpenBoardBtn;

        protected List<RecordItem> ItemList = new List<RecordItem>();

        /// <summary>
        /// 记牌器服务器配置字段
        /// </summary>
        protected const string ShowRecordBoardKey = "-showrb";


        protected void Awake()
        {
            OnAwake();
        }

        protected void OnAwake()
        {
            CreateRecordItems();

            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeFirstOut, OnTypeFirstOut);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeOutCard, OnTypeOutCard);
            Facade.EventCenter.AddEventListeners<int, DdzbaseEventArgs>(GlobalConstKey.TypeAllocate, OnAllocateCds);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyOnRejoin, OnRejoin);
            Facade.EventCenter.AddEventListeners<string, DdzbaseEventArgs>(GlobalConstKey.KeyGetGameInfo, OnGetGameInfo);
        }


        private void OnGetGameInfo(DdzbaseEventArgs args)
        {
            InitRecordBoardActive(args);
        }

        private void InitRecordBoardActive(DdzbaseEventArgs args)
        {
            var gameInfo = args.IsfObjData;
            if (!gameInfo.ContainsKey(NewRequestKey.KeyCargs2)) return;
            var cargs2 = gameInfo.GetSFSObject(NewRequestKey.KeyCargs2);
            if (cargs2.ContainsKey(ShowRecordBoardKey))
            {
                bool showBoard = int.Parse(cargs2.GetUtfString(ShowRecordBoardKey)) > 0;
                gameObject.SetActive(showBoard);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }


        private void OnRejoin(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            if (data.ContainsKey(RequestKey.KeyState) && data.GetInt(RequestKey.KeyState) > GlobalConstKey.StatusIdle)
            {
                OpenBoardBtn.SetActive(true);
                InitRecordItemCount();

                if (data.ContainsKey(NewRequestKey.KeyAllOutCards))
                {
                    var cards = data.GetIntArray(NewRequestKey.KeyAllOutCards);
                    RemoveCards(cards);
                }

                if(data.ContainsKey(RequestKey.KeyUser))
                {
                    var selfData = data.GetSFSObject(RequestKey.KeyUser);
                    if (selfData.ContainsKey(RequestKey.KeyCards))
                    {
                        var cards = selfData.GetIntArray(RequestKey.KeyCards);
                        RemoveCards(cards);

                    }
                }
            }
        }

        
        public void CreateRecordItems()
        {
            //初始化王牌
            {
                var item = CreateOneItem("bigking", true);
                item.CardVal = 97;
                item = CreateOneItem("littleking", true);
                item.CardVal = 81;
            }

            //初始化普通牌
            for (int i = 15; i >= MinNum; i--)
            {
                string text = GetCardMarkText(i);
                var item = CreateOneItem(text, false);
                item.CardVal = i;
            }
            ItemParent.Reposition();
            ItemParent.repositionNow = true;
        }

        string GetCardMarkText(int val)
        {
            switch (val)
            {
                case 1:
                case 14:
                    return "A";
                case 2:
                case 15:
                    return "2";
                case 11:
                    return "J";
                case 12:
                    return "Q";
                case 13:
                    return "K";
                default:
                    return val.ToString();
            }
        }

        void InitRecordItemCount()
        {
            int normalCount = Deck * 4;    //普通牌一副4张
            foreach (var item in ItemList)
            {
                if (item.CardVal == 97 || item.CardVal == 81) //普通牌一副1张
                {
                    item.CardCount = Deck;
                    item.SetCardCountLabel(Deck, SpecialColor);
                    continue;
                }
                item.CardCount = normalCount;
                item.SetCardCountLabel(normalCount, SpecialColor);
            }
        }


        private RecordItem CreateOneItem(string text, bool isSprite)
        {
            var go = GetOne(ItemParent.transform);
            go.gameObject.SetActive(true);
            go.name = text;
            if (isSprite)
            {
                go.SetSpriteMark(text);
            }
            else
            {
                go.SetCardMarkLabel(text, NormalColor);
            }
            
            ItemList.Add(go);
            return go;
        }


        RecordItem GetOne(Transform parent)
        {
            var go = Instantiate(ItemPrefab);

            var tran = go.transform;
            tran.parent = parent;
            tran.localScale = Vector3.one;

            return go;
        }




        /// <summary>
        /// 发牌阶段,移除自己手中的牌
        /// </summary>
        /// <param name="args"></param>
        protected void OnAllocateCds(DdzbaseEventArgs args)
        {
            OpenBoardBtn.SetActive(true);
            InitRecordItemCount();      //初始化记牌器个数
            var data = args.IsfObjData;
            var cards = data.GetIntArray(GlobalConstKey.C_Cards);
            RemoveCards(cards);         //删除自己手牌中出现过的牌
        }

        /// <summary>
        /// 在记录板中移除出过的牌
        /// </summary>
        void RemoveCardFromRecoredboard(int val)
        {
            if (val == 97)  //大王
            {
                var item = GetItemInList(val);
                item.RemoveOne(NormalColor);
                item = GetItemInList(81);
                item.CardCountLabel.color = NormalColor;    //大王被打出,小王也设置为普通色
                return;
            }

            if (val == 81)      //小王
            {
                var item = GetItemInList(val);
                item.RemoveOne(NormalColor);
                item = GetItemInList(97);
                item.CardCountLabel.color = NormalColor;    //小王被打出,大王也设置为普通色
                return;
            }

            //普通牌
            int cardVal = GetCardVal(val);
            var recordItem = GetItemInList(cardVal);
            recordItem.RemoveOne(NormalColor);
        }

      
        private RecordItem GetItemInList(int cardVal)
        {
            return ItemList.Find(item => item.CardVal == cardVal);
        }


        /// <summary>
        /// 当有人出牌,移除除其他玩家出过的牌
        /// </summary>
        /// <param name="args"></param>
        protected void OnTypeOutCard(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;
            int seat = data.GetInt(RequestKey.KeySeat);
            if(seat == YxFramwork.Common.App.GetGameData<DdzGameData>().SelfSeat)   //自己出牌无需移除(已经在发牌的过程中移除过了)
            {
                return;
            }
            var cards = data.GetIntArray(GlobalConstKey.C_Cards);
            RemoveCards(cards);
        }

        /// <summary>
        /// 接收底牌信息,如果是自己的底牌,从记牌器中移除
        /// </summary>
        /// <param name="args"></param>
        protected void OnTypeFirstOut(DdzbaseEventArgs args)
        {
            var data = args.IsfObjData;

            int seat = data.GetInt(RequestKey.KeySeat);
            if(seat != YxFramwork.Common.App.GetGameData<DdzGameData>().SelfSeat)   //不是自己的底牌,无需移除
            {
                return;
            }
            var cards = data.GetIntArray(GlobalConstKey.C_Cards);
            RemoveCards(cards);
        }


        /// <summary>
        /// 获取牌信息,移除出过的牌
        /// </summary>
        /// <param name="cards">牌数组</param>
        void RemoveCards(int[] cards)
        {
            if (cards == null)
            {
                return;
            }

            foreach (var val in cards)
            {
                RemoveCardFromRecoredboard(val);
            }
        }


        /// <summary>
        /// 获取牌面数值
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        int GetCardVal(int val)
        {
            int cardVal = val % 16;
            if (cardVal == 1)
            {
                cardVal = 14;
            }
            else if (cardVal == 2)
            {
                cardVal = 15;
            }
            return cardVal;
        }

        

        public UITweener Tween;

        public void OnClickOpenBtn()
        {
            bool active = ItemParent.gameObject.activeSelf;
            if (active)
            {
                Tween.PlayReverse();
            }
            else
            {
                Tween.enabled = true;
                Tween.ResetToBeginning();
                Tween.PlayForward();
            }
            ItemParent.gameObject.SetActive(!active);
        }
    }
}