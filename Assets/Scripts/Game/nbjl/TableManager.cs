using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Enums;
using YxFramwork.Framework;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using Random = System.Random;

/*===================================================
 *文件名称:     TableManager.cs
 *作者:         AndeLee
 *版本:         1.0
 *Unity版本:    5.4.0f3
 *创建时间:     2018-07-05
 *描述:        	桌面管理
 *历史记录: 
=====================================================*/
namespace Assets.Scripts.Game.nbjl
{
    public class TableManager : RecycleWindow
    {
        #region UI Param
        [Tooltip("桌面显示玩家")]
        public NbjlPlayer[] ShowPlayers;
        [Tooltip("其它玩家")]
        public NbjlPlayer OtherPlayer;
        [Tooltip("当前玩家")]
        public NbjlPlayer SelfPlayer;
        [Tooltip("庄家")]
        public NbjlPlayer BankerPlayer;
        [Tooltip("下注区域")]
        public BetArea[] BetAreas;

        #endregion

        #region Data Param
        public NbjlGameData Data
        {
            get
            {
                return App.GetGameData<NbjlGameData>();
            }
        }
        #endregion

        #region Local Data
        /// <summary>
        /// 下注区域标识
        /// </summary>
        private readonly List<string> _pos = new List<string>(){"z", "x","h","zd","xd"};
        /// <summary>
        /// 下注区域名称
        /// </summary>
        private readonly List<string> _posName = new List<string>() { "庄家", "闲家", "和", "庄对", "闲对" };
        /// <summary>
        /// 随机
        /// </summary>
        private Random _random;
        /// <summary>
        /// 当前玩家是否在排行榜中
        /// </summary>
        private bool _selfInRank;
        #endregion

        #region Life Cycle

        protected override void OnAwake()
        {
            Facade.EventCenter.AddEventListeners<LocalRequest, List<NbjlPlayerInfo>>(LocalRequest.PlayerInfos, OnPlayersInfo);
            Facade.EventCenter.AddEventListeners<LocalRequest, BetData>(LocalRequest.ReqBet, OnPlayerBet);
            Facade.EventCenter.AddEventListeners<LocalRequest, int>(LocalRequest.ReqResult,OnGameResult);
            Pool.OnInitAction += delegate(YxView view)
            {
                var chipItem = view as ChipItem;
                if (chipItem != null)
                {
                    var item = chipItem;
                    item.Id = Pool.NextValidIndex.ToString();
                    item.SetDepth(item);
                }
            };
            _random=new Random();
        }

        #endregion

        #region Function

        /// <summary>
        /// /玩家信息变化
        /// </summary>
        /// <param name="players"></param>
        private void OnPlayersInfo(List<NbjlPlayerInfo> players)
        {
            var playerList = players.ToList();
            var playerSeat = App.GameData.SelfSeat;
            var bankerSeat = App.GetGameData<NbjlGameData>().BankerSeat;
            var selfIndex= playerList.FindIndex(player =>player!=null&&player.Seat == playerSeat);
            _selfInRank = selfIndex > -1&& selfIndex<6;
            if (selfIndex>-1)
            {
                var selfInfo = playerList[selfIndex];
                SelfPlayer.UpdateView(selfInfo);
            }
            var bankerIndex = playerList.FindIndex(player => player != null && player.Seat == bankerSeat);
            if (bankerIndex > -1)
            {
                playerList.RemoveAt(bankerIndex);
            }
            var playerIndex = 0;
            var playerCount = playerList.Count;
            foreach (NbjlPlayer player in ShowPlayers)
            {
                NbjlPlayerInfo info = null;
                while (info==null)
                {
                    if (playerIndex < playerCount)
                    {
                        info = playerList[playerIndex];
                         playerIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
                player.UpdateView(info);
            }
        }
        
        /// <summary>
        /// 玩家下注
        /// </summary>
        /// <param name="data"></param>
        private void OnPlayerBet(BetData data)
        {
            var gold = data.Gold;
            var selfGold = 0;
            var chipDatas = GetChipDatas(gold);
            var position = data.Position;
            var seat = data.Seat;
            var areaCount = BetAreas.Length;
            NbjlPlayer sendPlayer=null;
            BetArea getArea=null;
            bool selfBet = seat == App.GameData.SelfSeat;
            for (int i = 0; i < areaCount; i++)
            {
                BetArea area = BetAreas[i];
                if (area.AreaType == position)
                {
                    getArea = BetAreas[i];
                }
            }
            foreach (var player in ShowPlayers)
            {
                if (!player.gameObject.activeInHierarchy)
                {
                    continue;
                }
                if (player.Info != null)
                {
                    if (player.Info.Seat.Equals(seat))
                    {
                        sendPlayer = player;
                    }
                }
            }
            if (selfBet&& sendPlayer==null)
            {
                App.GameData.GStatus = YxEGameStatus.PlayAndConfine;
                App.GameData.GetPlayerInfo().CoinA -= gold;
                sendPlayer = SelfPlayer;
            }
            if (sendPlayer == null)
            {
                sendPlayer = OtherPlayer;
            }
            PlayChips(sendPlayer, getArea, chipDatas,gold,selfBet);

        }

        public void PlayChips(NbjlPlayer sender,BetArea area,List<ChipData> chipDatas,int gold,bool selfBet)
        {
            List<YxView> items=new List<YxView>();
            var index = _pos.FindIndex(item => item == area.AreaType);
            var posName = _posName[index];
            foreach (ChipData chipData in chipDatas)
            {
                var view= AddChildToShow(chipData);
                
                TweenFromTo(view, sender.ChipPosVec, area.ChipPosVec,GetRandomTime());
                sender.ShowBet(posName, chipData.Value);
                items.Add(view);
            }
            area.Items.AddRange(items);
            if (selfBet)
            {
                area.ShowBetNum(gold, gold);
                if (_selfInRank)
                {
                    if (SelfPlayer)
                    {
                        SelfPlayer.FreshCoinA();
                    }
                }
            }
            else
            {
                area.ShowBetNum(gold, 0);
            }
        }

        private void OnGameResult(int result)
        {
            CancelInvoke("StartMove");
            Invoke("StartMove",1);
        }

        /// <summary>
        /// 开始移动到庄家位置
        /// </summary>
        private void StartMove()
        {
            var areaCount = BetAreas.Length;
            var winData = App.GetGameData<NbjlGameData>().BankerWin;
            bool bankerGet = false;
            foreach (var data in winData)
            {
                if (data>0)
                {
                    bankerGet = true;
                    break;
                }
            }
            if (bankerGet)
            {
                Facade.Instance<MusicManager>().Play(ConstantData.KeySoundFlushBet);
            }
            for (int i = 0; i < areaCount; i++)
            {
                bool areaWin = winData[i] > 0;
                if (areaWin)
                {
                    foreach (var item in BetAreas[i].Items)
                    {
                        TweenFromTo(item,item.transform.localPosition, BankerPlayer.ChipPosVec, GetRandomTime());
                    }
                    BetAreas[i].Items.Clear();
                }
            }
            Invoke("BankerSend", 2);
            CancelInvoke("StartMove");
        }

        /// <summary>
        /// 庄家发送筹码到下注区域
        /// </summary>
        private void BankerSend()
        {
            CancelInvoke("BankerSend");
            var areaCount = BetAreas.Length;
            var winData = App.GetGameData<NbjlGameData>().BankerWin;
            for (int i = 0; i < areaCount; i++)
            {
                var winNum = winData[i];
                bool areaLose = winNum < 0;
                var area = BetAreas[i];
                if (areaLose)
                {
                    List<ChipData> chips = GetChipDatas(-winNum);
                    if(chips.Count>0)
                    {
                        Facade.Instance<MusicManager>().Play(ConstantData.KeySoundFlushBet);
                    }
                    var list=new List<YxView>();
                    foreach (ChipData chipdDat in chips)
                    {
                        var view = AddChildToShow(chipdDat);
                        TweenFromTo(view,BankerPlayer.ChipPosVec, area.ChipPosVec, GetRandomTime());
                        list.Add(view);
                    }
                    BetAreas[i].Items.AddRange(list);
                }
            }
            Invoke("BetAreaSend", 1.5f);
            CancelInvoke("BankerSend");
        }

        /// <summary>
        /// 下注区域筹码移动
        /// </summary>
        private void BetAreaSend()
        {
            var areaCount = BetAreas.Length;
      
            for (int i = 0; i < areaCount; i++)
            {
                var area = BetAreas[i];
                if (area.Items.Count>0)
                {
                    Facade.Instance<MusicManager>().Play(ConstantData.KeySoundFlushBet);
                    foreach (var item in area.Items)
                    {
                        TweenFromTo(item, item.transform.localPosition, OtherPlayer.ChipPosVec, GetRandomTime());
                    }
                    area.Items.Clear();
                }
            }
            Invoke("RestoreAll",1.5f);
            CancelInvoke("BetAreaSend");
        }

        /// <summary>
        /// 回收
        /// </summary>
        private void RestoreAll()
        {
            Pool.StoreAll();
            foreach (var area in BetAreas)
            {
                area.Reset();
            }
        }

        /// <summary>
        /// 移动筹码到指定位置
        /// </summary>
        /// <param name="view"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="time"></param>
        private void TweenFromTo(YxView view,Vector3 from,Vector3 to,float time)
        {
            var tween = view.GetComponent<TweenPosition>() ?? view.gameObject.AddComponent<TweenPosition>();
            tween.@from = from;
            tween.duration = time;
            tween.to = to;
            tween.ResetToBeginning();
            tween.PlayForward();
        }

        private List<ChipData> GetChipDatas(int gold)
        {
            var rates = Data.AnteRate;
            var count = rates.Count;
            List<ChipData> chipDatas = new List<ChipData>();
            for (int i = count - 1; i >=0 ; i--)
            {
                var itemValue = rates[i];
                if (gold >= itemValue)
                {
                    var chipCount = gold/itemValue;
                    for (int j = 0; j < chipCount; j++)
                    {
                        ChipData item = new ChipData()
                        {
                            Type = i,
                            Value = itemValue
                        };
                        chipDatas.Add(item);
                    }
                    gold -= chipCount*itemValue;
                }
                if (gold == 0)
                {
                    break;
                }
            }
            return chipDatas;
        }

        private float GetRandomTime()
        {
            var num=_random.Next(5, 10);
            float time = (float) num/10;
            return time;
        }

        #endregion
        }
}
