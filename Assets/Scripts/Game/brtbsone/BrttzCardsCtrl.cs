using UnityEngine;
using System.Collections;
using Sfs2X.Entities.Data;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using System.Collections.Generic;
using YxFramwork.View;

namespace Assets.Scripts.Game.brtbsone
{
   //棋牌控制
    public class BrttzCardsCtrl : MonoBehaviour
    {
        public GameObject[] Target;
        public ISFSArray CardsValue = new SFSArray();
        public UIGrid Grid;
        public GameObject CardClone;
        public DicePointsCtrl Dices;
        public CardPoint[] CardPoints;
        public HistoryCardsCtrl HistoryCardsCtrl;
        public int FirstCardsNum = 8;


        /// <summary>
        /// 从零开始在牌墙中第几排牌墙发牌,不能大于4
        /// </summary>
        public int CutNum = 0;
        /// <summary>
        /// 每局游戏开始时是否需要洗牌,false为发完40张牌之后才洗牌,换庄自动洗牌.true为每局都洗牌
        /// </summary>
        public bool IsNeedRefresh = false;

        //一共有多少张牌
        public int MaxCardsNum = 40;
        /// <summary>
        /// 麻将缩放tween的两个值,索引0为初始值,索引1为目标值
        /// </summary>
        public float[] CardsBgScaleNums;

        /// <summary>
        /// 已经给出了几张牌
        /// </summary>
        protected int HasGiveNum;
        //保存牌堆上的牌
        protected List<GameObject> Cards = new List<GameObject>();
        /// <summary>
        /// 牌堆索引
        /// </summary>
        protected int CardsArrindex;
        /// <summary>
        /// 庄家每门的输赢
        /// </summary>
        internal int[] Bpg;
        /// <summary>
        /// 发牌状态
        /// </summary>
        protected int GiveCardsStatus;
        /// <summary>
        /// 记录发了多少张牌
        /// </summary>
        protected int Index;
        /// <summary>
        /// 发牌区域的索引
        /// </summary>
        protected int Arrindex = -1;
        /// <summary>
        /// 翻牌时在牌墙里的索引
        /// </summary>
        protected int ShowIndex;
        /// <summary>
        /// 发牌时在牌墙里的索引
        /// </summary>
        protected int CardIndex;
        /// <summary>
        /// 骰子数组
        /// </summary>
        protected int[] DicesNums;
        /// <summary>
        /// 骰子的值
        /// </summary>
        protected int DicePoint;
        /// <summary>
        /// 打骰子从那家开始发牌
        /// </summary>
        protected int StartNum;
        /// <summary>
        /// 记录已经出了哪些牌
        /// </summary>
        protected Dictionary<int, int> HistoryCards = new Dictionary<int, int>();
        /// <summary>
        /// 是否是重连
        /// </summary>
        protected bool IsRejoin = false;


        public void GetHistoryCards(ISFSObject gameInfo)
        {
            var history = gameInfo.GetIntArray(Parameter.RCards);
            HasGiveNum = history.Length;
            if (history.Length == 0) return;
            for (int i = 0; i < history.Length; i++)
            {
                if (HistoryCards.ContainsKey(history[i]))
                    HistoryCards[history[i]]++;
            }
        }
        //----------------发牌:----

        public virtual void BeginGiveCards(ISFSObject responseData)
        {
            var gdata = App.GetGameData<BrttzGameData>();
            if (responseData.ContainsKey(Parameter.Bpg))
            {
                Bpg = responseData.GetIntArray(Parameter.Bpg);
            }
            if (responseData.ContainsKey(Parameter.Total))
            {
                gdata.GetPlayerInfo().CoinA = responseData.GetLong(Parameter.Total);
            }
            gdata.ResultUserTotal = responseData.ContainsKey(Parameter.Win) ? responseData.GetInt(Parameter.Win) : 0;
            gdata.ResultBnakerTotal = responseData.ContainsKey(Parameter.Bwin) ? responseData.GetLong(Parameter.Bwin) : 0;
            CardsValue = responseData.GetSFSArray(Parameter.Cards);
        }

        public void GiveCardsOnFrist(ISFSObject gameInfo)
        {
            if (!gameInfo.ContainsKey(Parameter.RollResult))
                return;
            var data = gameInfo.GetSFSObject(Parameter.RollResult);
            var cardsValueFrist = data.GetSFSArray(Parameter.Cards);
            int temp = 0;

            for (int i = 0; i < FirstCardsNum; i++)
            {
                var go = Cards[i];
                go.transform.parent = Target[temp].GetComponent<BrttzCardPostion>().TargetPositions[i % 2];
                go.transform.localScale = Vector3.one;
                go.transform.localPosition = Vector3.zero;
                var mj = go.GetComponent<MjCard>();
                mj.Mahjong.spriteName = "card";
                mj.Mahjong.MakePixelPerfect();
                var values = cardsValueFrist.GetSFSObject(temp).GetIntArray(Parameter.Cards);
                mj.Target.spriteName = "A_" + values[i % 2];
                mj.Target.gameObject.SetActive(true);
                if (i % 2 != 0) temp++;
            }
            for (int i = 0; i < 4; i++)
            {
                int type = cardsValueFrist.GetSFSObject(i).GetInt(Parameter.Type);
                int value = cardsValueFrist.GetSFSObject(i).GetInt(Parameter.Value);
                CardPoints[i].ShowPointValue(type, value);
            }
            IsRejoin = true;
        }

        public virtual void GetDicesPoints(int[] dices)
        {
            DicesNums = dices;
            var count = DicesNums.Length;
            for (int i = 0; i < count; i++)
            {
                DicePoint += DicesNums[i];
            }
            GiveCardsStatus = 0;
            Invoke("BeginGiveCards", 1);
        }

        public virtual void GetDicesPoints(ISFSObject responseData)
        {
            if (responseData.ContainsKey(Parameter.Dices))
            {
                DicesNums = responseData.GetIntArray(Parameter.Dices);
            }
            var count = DicesNums.Length;
            for (int i = 0; i < count; i++)
            {
                DicePoint += DicesNums[i];
            }
            Dices.gameObject.SetActive(true);
            //播放摇骰子的音效
            Facade.Instance<MusicManager>().Play("touzi");
            Dices.Anim.enabled = true;
            for (int i = 0; i < Dices.Dices.Length; i++)
            {
                Dices.Dices[i].spriteName = "dice_" + DicesNums[i];
            }
        }
        
        public virtual void BeginGiveCards()
        {
            BeginClone();
            InvokeRepeating("ReadyGiveCards", 0, 0.1f);
        }
        protected void ReadyGiveCards()
        {
            if (GiveCardsStatus == 2)
            {
                OnGiveCardsOver();
                CancelInvoke("ReadyGiveCards");
            }
        }
        public virtual void OnGiveCardsOver()
        {
            ShowIndex = 0;
            StartNum = DicePoint == 0 ? 0 : ((DicePoint - 2) % 4);
            CardsArrindex = 2 * (StartNum + 1) + 2 * CutNum;
            InvokeRepeating("ShowCards", 0.5f, 0.5f);
        }
        protected int ShowNum = 0;
        protected int ShowTarget = 0;
        /// <summary>
        /// 翻牌
        /// </summary>
        public virtual void ShowCards()
        {
            if (CardsValue == null) return;
            if (ShowIndex % 2 == 0 && ShowIndex != 0)
            {
                //显示牌值
                ShowPoints(StartNum);
                StartNum++;
                StartNum = StartNum >= 4 ? StartNum % 4 : StartNum;
            }
            if (ShowIndex >= 8)
            {
                //发够多少张牌之后停止发牌
                CancelInvoke("ShowCards");
                return;
            }
            if (HasGiveNum > 32)
                CardsArrindex = CardsArrindex >= 8 ? CardsArrindex % 8 : CardsArrindex;
            else
                CardsArrindex = CardsArrindex >= (8 + 2 * CutNum) ? CardsArrindex % (8 + 2 * CutNum) + 2 * CutNum : CardsArrindex;
            var go = Cards[CardsArrindex];
            var mj = go.GetComponent<MjCard>();
            var value = CardsValue.GetSFSObject(StartNum).GetIntArray(Parameter.Cards);
            if (HistoryCards.ContainsKey(value[ShowIndex % 2]))
            {
                HistoryCards[value[ShowIndex % 2]]++;
                ShowNum = value[ShowIndex % 2] - 1;
                ShowTarget = HistoryCards[value[ShowIndex % 2]];
                Invoke("ShowHistoryChange", 0.4f);
            }
            mj.CardValue = value[ShowIndex % 2];
            mj.TurnCard();

            ShowIndex++;
            CardsArrindex++;
        }

        private void ShowHistoryChange()
        {
            HistoryCardsCtrl.RefrshDataOnPlay(ShowNum, ShowTarget);
        }

        public virtual void ShowPoints(int index)
        {
            //显示牌值
            int type = CardsValue.GetSFSObject(index).GetInt(Parameter.Type);
            int value = CardsValue.GetSFSObject(index).GetInt(Parameter.Value);
            CardPoints[index].ShowPointValue(type, value);
        }
        public void GetIsXiPai(ISFSObject data)
        {
            if ((data.ContainsKey(Parameter.XiPai) && data.GetBool(Parameter.XiPai)) || IsNeedRefresh)
            {
                HasGiveNum = 0;
                InitHistoryCards();
                YxMessageTip.Show("开始洗牌");
            }
            ShowAllCards();
            RefreshHistoryCards();
        }

        public void GetGameInfoOnCheck(ISFSObject data)
        {
            if (data.ContainsKey(Parameter.Dices))
            {
                DicesNums = data.GetIntArray(Parameter.Dices);
            }
            var count = DicesNums.Length;
            for (int i = 0; i < count; i++)
            {
                DicePoint += DicesNums[i];
            }
        }

        /// <summary>
        /// 显示牌堆,最多显示20张牌
        /// </summary>
        public void ShowAllCards()
        {
            ClearCardsWall();
            HasGiveNum = HasGiveNum > MaxCardsNum ? 0 : HasGiveNum;
            int count = MaxCardsNum - HasGiveNum >= 20 ? 20 : MaxCardsNum - HasGiveNum;
            count = count == MaxCardsNum % FirstCardsNum ? (MaxCardsNum % FirstCardsNum) + FirstCardsNum : count;
            if (Grid == null) return;
            for (int i = 0; i < count; i++)
            {
                var go = Instantiate(CardClone);
                Cards.Add(go);
                if (i % 2 == 0)
                {
                    var temp = go.GetComponent<UISprite>();
                    if (temp != null) temp.depth++;
                }
                go.transform.parent = Grid.transform;
                go.name = "Card" + i;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
            }
            Grid.repositionNow = true;
            Grid.Reposition();
        }

        public void ReceiveResult(ISFSObject responseData)
        {
            if (responseData.ContainsKey(Parameter.Total))
            {
                App.GameData.GetPlayer().Coin = responseData.GetLong(Parameter.Total);
            }
            if (responseData.ContainsKey(Parameter.BankerGold))
            {
                App.GetGameData<BrttzGameData>().BankerPlayer.Coin = responseData.GetLong(Parameter.BankerGold);
            }
            if (responseData.ContainsKey(Parameter.Bpg))
            {
                Bpg = new[] { 0, 0, 0, 0, 0, 0 };
                Bpg = responseData.GetIntArray(Parameter.Bpg);
            }
            App.GetGameData<BrttzGameData>().ResultUserTotal = responseData.ContainsKey(Parameter.Win) ? responseData.GetInt(Parameter.Win) : 0;
            App.GetGameData<BrttzGameData>().ResultBnakerTotal = responseData.ContainsKey(Parameter.Bwin) ? responseData.GetLong(Parameter.Bwin) : 0;
        }

        /// <summary>
        /// 清理牌墙
        /// </summary>
        protected void ClearCardsWall()
        {
            if (Cards.Count <= 0) return;
            while (Cards.Count != 0)
            {
                Destroy(Cards[0]);
                Cards.RemoveAt(0);
            }
        }

        protected virtual void InstantiateChip()
        {
            if (Index % 2 == 0 && Index != 0)
            {
                Arrindex++;
                if (Arrindex >= 4) Arrindex = Arrindex % 4;
            }
            if (Index >= 8)
            {
                //发够多少张牌之后停止发牌
                CancelInvoke("GiveCards");
                GiveCardsStatus = 2;
                return;
            }
            HasGiveNum++;
            if (HasGiveNum > 32)
                CardIndex = CardIndex >= 8 ? CardIndex % 8 : CardIndex;
            else
                CardIndex = CardIndex >= (8 + 2 * CutNum) ? CardIndex % (8 + 2 * CutNum) + 2 * CutNum : CardIndex;
            var go = Cards[CardIndex];
            var target = Target[Arrindex].GetComponent<BrttzCardPostion>().TargetPositions[Index % 2];
            go.transform.parent = target.transform;
            go.transform.localScale = Vector3.one;

            var sp = go.GetComponent<SpringPosition>();
            sp.target = Vector3.zero;
            sp.enabled = true;

            var ts = go.GetComponent<TweenScale>();
            //ts.from = Vector3.one * 0.8f;
            //ts.to = Vector3.one * 1.5f;
            if (CardsBgScaleNums != null && CardsBgScaleNums.Length == 2)
            {
                ts.from = Vector3.one * CardsBgScaleNums[0];
                ts.to = Vector3.one * CardsBgScaleNums[1];
            }
            ts.duration = 0.5f;
            ts.PlayForward();

            Facade.Instance<MusicManager>().Play("card");
            Index++;
            CardIndex++;
            //可以添加一些移动
        }





        public virtual void BeginGiveMingCards(ISFSObject responseData)
        {

        }

        public virtual void SendMingCardFirst(ISFSObject gameInfo)
        {

        }



        //public int[] GetResultList()
        //{
        //    if (CardsValue.Count == 0) return null;
        //    int[] results = { -1, -1, -1 };
        //    var bankerType = CardsValue.GetSFSObject(3).GetInt(Parameter.Type);
        //    var bankerValue = CardsValue.GetSFSObject(3).GetInt(Parameter.Value);
        //    for (int i = 0; i < 3; i++)
        //    {
        //        var type = CardsValue.GetSFSObject(i).GetInt(Parameter.Type);
        //        var value = CardsValue.GetSFSObject(i).GetInt(Parameter.Value);
        //        if (bankerType < type) results[i] = 1;
        //        else if (bankerType == type && bankerValue < value) results[i] = 1;
        //    }
        //    return results;
        //}

        //protected int[] GetResultList(ISFSArray fristCardsValues)
        //{
        //    if (fristCardsValues.Count == 0) return null;
        //    int[] results = { -1, -1, -1 };
        //    var bankerType = fristCardsValues.GetSFSObject(3).GetInt(Parameter.Type);
        //    var bankerValue = fristCardsValues.GetSFSObject(3).GetInt(Parameter.Value);
        //    for (int i = 0; i < 3; i++)
        //    {
        //        var type = fristCardsValues.GetSFSObject(i).GetInt(Parameter.Type);
        //        var value = fristCardsValues.GetSFSObject(i).GetInt(Parameter.Value);
        //        if (bankerType < type) results[i] = 1;
        //        else if (bankerType == type && bankerValue < value) results[i] = 1;
        //    }
        //    return results;
        //}

        public void Reset()
        {
            ReSetGiveCardsStatus();
            DicePoint = 0;
            DicesNums = new int[2];
            CancelInvoke("ShowCards");
            foreach (var point in CardPoints)
            {
                point.Init();
            }
            CancelInvoke("GiveCards");
            CancelInvoke("ReadyGiveCards");
            Dices.Reset();
            CancelInvoke();
            IsRejoin = false;
        }

        public void ReSetGiveCardsStatus()
        {
            GiveCardsStatus = 0;
            CancelInvoke("GiveCards");
        }

        protected void GiveCards()
        {
            InstantiateChip();
        }

        public void BeginClone()
        {
            if (GiveCardsStatus == 0)
            {
                GiveCardsStatus = 1;
                Index = 0;
                Arrindex = DicePoint == 0 ? 0 : ((DicePoint - 2) % 4);
                CardIndex = 2 * (Arrindex + 1) + 2 * CutNum;
                InvokeRepeating("GiveCards", 0.5f, 0.3f);
            }
        }

        protected void RefreshHistoryCards()
        {
            int[] history = new int[HistoryCards.Count];
            for (int i = 0; i < history.Length; i++)
            {
                history[i] = HistoryCardsCtrl.MaxMahjongNum - HistoryCards[i + 1];
            }
            HistoryCardsCtrl.RefreshData(history);
        }

        public void InitHistoryCards()
        {
            HistoryCardsCtrl.InitMahjong();
            HistoryCards.Clear();
            HistoryCards.Add(1, 0);
            HistoryCards.Add(2, 0);
            HistoryCards.Add(3, 0);
            HistoryCards.Add(4, 0);
            HistoryCards.Add(5, 0);
            HistoryCards.Add(6, 0);
            HistoryCards.Add(7, 0);
            HistoryCards.Add(8, 0);
            HistoryCards.Add(9, 0);
            HistoryCards.Add(10, 0);
        }
    }
}