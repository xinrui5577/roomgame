using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.Texas.Mgr
{
    /// <summary>
    /// 公共筹码管理类 单例
    /// </summary>
    public class BetMgr : MonoBehaviour
    { 
        /// <summary>
        /// 筹码prefab
        /// </summary>
        public GameObject BetPrefab;
        /// <summary>
        /// 筹码堆父节点
        /// </summary>
        public Transform BetFather;
        /// <summary>
        /// 大筹码堆
        /// </summary>
        public BetStack BigBetStack;
        /// <summary>
        /// 小筹码堆grid
        /// </summary>
        public UIGrid SmallBet;
        /// <summary>
        /// 小筹码堆
        /// </summary>
        public BetStack[] SmallBetStack;

        /// <summary>
        /// 创建一个筹码
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public GameObject CreatBet(Transform tf, int index)
        {
            GameObject gob = Instantiate(BetPrefab);
            gob.transform.parent = tf;
            gob.transform.localScale = Vector3.one;
            gob.transform.localPosition = Vector3.zero;
            gob.GetComponent<Bet>().SetImage(index);

            return gob;
        }

        /// <summary>
        /// 创建一堆筹码
        /// </summary>
        /// <param name="money">最大钱数</param>
        /// <param name="maxBet">最大筹码数</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public GameObject[] CreatBetArray(long money, long maxBet, Transform trans)
        {
            var bets = new List<GameObject>();

            var i = 0;
            var betM = money;
            int ante = App.GetGameData<TexasGameData>().Ante;

            while (betM > 0 && i < maxBet)
            {
                i++;
                int index;
                if (money >= ante * 10)
                {
                    index = 3;
                    betM -= ante * 10;
                }
                else if (money >= ante * 5)
                {
                    index = 2;
                    betM -= ante * 5;
                }
                else if (money >= ante * 2)
                {
                    index = 1;
                    betM -= ante * 2;
                }
                else
                {
                    index = 0;
                    betM = 0;
                }

                var bet = CreatBet(trans, index);
                bets.Add(bet);
            }

            return bets.ToArray();
        }

        /// <summary>
        /// 收集桌面上的筹码
        /// </summary>
        public virtual void CollectBet()
        {
            var gdata = App.GetGameData<TexasGameData>();
            var playerList = gdata.PlayerList;
            var playerCount = playerList.Length;
            
            //筹码处理
            for (var i = 0; i < playerCount; i++)
            {
                var player = gdata.GetPlayer<PlayerPanel>(i);
                if (player.Info == null || player.BetMoney <= 0) { continue;}
                var bets = player.Bets;
                var betCount = player.Bets.Count;
                for (var j = 0; j < betCount; j++)
                {
                    var bet = bets[j];
                    bet.transform.parent = BetFather;
                    bet.BeginMove(bet.transform.localPosition, BigBetStack.transform.localPosition, j * gdata.BetSpeace, BetFinishedType.Destroy,
                        () =>
                        {
                            BigBetStack.gameObject.SetActive(true);
                        });
                }
                bets.Clear();
            }
            AssignedStack();
            SmallBet.Reposition();
        }
        /// <summary>
        /// 收集筹码数值,无过程
        /// </summary>
        public virtual void CollectBetValue()
        {
            AssignedStack();
            BigBetStack.gameObject.SetActive(true);
            SmallBet.Reposition();
        }

        /// <summary>
        /// 分筹码堆
        /// </summary>
        /// <returns></returns>
        public void AssignedStack()
        {
            var gdata = App.GameData;
            var playerList = gdata.PlayerList;
            var playerCount = playerList.Length;
            while (true)
            {
                var minMoney = 0L;
                var playerIndex = -1;
                var allMoney = 0L;
                //获取最小allin的值和位置
                for (var i = 0; i < playerCount; i++)
                {
                    var player = gdata.GetPlayer<PlayerPanel>(i);

                    if (player.CurGameType == PlayerGameType.AllIn && player.BetMoney > 0 && (minMoney == 0 || minMoney > player.BetMoney))
                    {
                        minMoney = player.BetMoney;
                        playerIndex = i;
                    }
                    allMoney += player.BetMoney;
                }
                //如果没有allin则直接加到大筹码堆上
                if (playerIndex < 0)
                {
                    if (allMoney == 0)
                        return;

                    if (!BigBetStack.IsAdd)
                    {
                        MoveDataToSmall();
                    }

                    for (int i = 0; i < playerCount; i++)
                    {
                        var player = gdata.GetPlayer<PlayerPanel>(i);
                        if (player.Info == null)
                        {
                            BigBetStack.Belong[i] = false;
                            continue;
                        }

                        BigBetStack.BetValue += player.BetMoney;
                        player.BetMoney = 0;
                        player.BetLabel.gameObject.SetActive(false);
                        if (BigBetStack.Belong[player.Info.Seat])
                        {
                            BigBetStack.Belong[player.Info.Seat] = player.CurGameType != PlayerGameType.Fold;
                        }
                    }

                    BigBetStack.Bet.text = YxUtiles.ReduceNumber(BigBetStack.BetValue);
                }
                else
                {
                    //如果有allin判断大筹码堆是否可以添加
                    if (!BigBetStack.IsAdd)
                    {
                        MoveDataToSmall();
                    }

                    AddToBigStack(minMoney);

                    continue;
                }
                break;
            }
        }

        /// <summary>
        /// 往大筹码堆上添加筹码
        /// </summary>
        /// <returns></returns>
        public void AddToBigStack(long min)
        {
            var gdata = App.GameData;
            var playerList = gdata.PlayerList;
            var playerCount = playerList.Length;
            for (var i = 0; i < playerCount; i++)
            {
                var player = gdata.GetPlayer<PlayerPanel>(i);
                if (player.BetMoney > 0 && player.Info != null)
                {
                    var num = player.BetMoney > min ? min : player.BetMoney;
                    BigBetStack.BetValue += num;
                    player.BetMoney -= num;
                    player.BetLabel.gameObject.SetActive(false);
                    if (BigBetStack.Belong[player.Info.Seat])
                    {
                        BigBetStack.Belong[player.Info.Seat] = player.CurGameType != PlayerGameType.Fold;
                    }
                }
                else
                {
                    if (player.Info == null)
                    {
                        BigBetStack.Belong[i] = false;
                    }
                }
            }

            BigBetStack.Bet.text = YxUtiles.ReduceNumber(BigBetStack.BetValue);
            BigBetStack.IsAdd = false;
        }
        /// <summary>
        /// 数据转移到小筹码堆上
        /// </summary>
        public void MoveDataToSmall()
        {
            YxDebug.Log("MoveDataToSmall");
            foreach (var stack in SmallBetStack)
            {
                if (stack.IsAdd)
                {
                    stack.IsAdd = BigBetStack.IsAdd;
                    stack.BetValue = BigBetStack.BetValue;
                    stack.SetBelong(BigBetStack.Belong);
                    stack.Bet.gameObject.SetActive(true);
                    stack.Bet.text = YxUtiles.ReduceNumber(stack.BetValue);

                    BigBetStack.IsAdd = true;
                    BigBetStack.BetValue = 0;
                    BigBetStack.Bet.text = YxUtiles.ReduceNumber(BigBetStack.BetValue);
                    //BigBetStack.Reset(false);
                    break;
                }
            }
            var gdata = App.GameData;
            var playerList = gdata.PlayerList;
            var playerCount = playerList.Length;
            for (var i = 0; i < playerCount; i++)
            {
                var player = gdata.GetPlayer<PlayerPanel>(i);
                if (player.BetMoney <= 0 && BigBetStack.Belong[i])
                {
                    BigBetStack.Belong[i] = player.CurGameType != PlayerGameType.AllIn;
                }
            }
        }

        #region 分筹码到赢家
        /// <summary>
        /// 分发筹码到指定的赢家
        /// </summary>
        public virtual void SendBetToWin(List<ISFSObject> wins)
        {
            Facade.Instance<MusicManager>().Play("receiveBet");
            var wheres = new List<Transform>();
            var gdata = App.GameData;
            var winCount = wins.Count;
            for (var i = 0; i < winCount; i++)
            {
                var someOne = wins[i];
                if (someOne.GetInt("gold") <= 0) continue;

                var seat = someOne.GetInt("seat");
                if (BigBetStack.Belong[seat])
                {
                    var p = gdata.GetPlayer<PlayerPanel>(seat,true);
                    if(p == null) { continue;}
                    var ts = p.HeadPortrait.transform;
                    wheres.Add(ts);
                }
            }

            if (wheres.Count != 0)
            {
                BigBetStack.SendBetToSomewhere(wheres);
            }

            wheres.Clear();

            foreach (var stack in SmallBetStack)
            {
                for (var i = 0; i < winCount; i++)
                {
                    var seat = wins[i].GetInt("seat");
                    if (stack.Belong[seat])
                    {
                        var player = gdata.GetPlayer<PlayerPanel>(seat,true);
                        wheres.Add(player.HeadPortrait.transform);
                    }
                }

                if (wheres.Count != 0)
                {
                    stack.SendBetToSomewhere(wheres);
                }
            }

        }

        #endregion

        public virtual void Reset()
        {
            BigBetStack.Reset();
            for (var i = 0; i < SmallBetStack.Length; i++)
            {
                SmallBetStack[i].Reset();
            }
        }

    }
}
