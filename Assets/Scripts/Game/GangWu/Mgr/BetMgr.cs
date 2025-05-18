using System.Collections.Generic;
using Assets.Scripts.Game.GangWu.Main;
using Assets.Scripts.Game.GangWu.Tool;
using com.yxixia.utile.YxDebug;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.GangWu.Mgr
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
        /// 小筹码堆
        /// </summary>
        public BetStack[] SmallBetStack;

        /// <summary>
        /// 下筹码按钮
        /// </summary>
        [SerializeField]
        private GameObject[] _addBetBtns = null;

   
        /// <summary>
        /// 创建一个筹码
        /// </summary>
        /// <param name="tf"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public GameObject CreatBet(Transform tf, int index)
        {
            var gob = Instantiate(BetPrefab);
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
        /// <param name="trans">位置</param>
        /// <returns></returns>
        public GameObject[] CreatBetArray(int money, int maxBet, Transform trans)
        {
            var bets = new List<GameObject>();
            var i = 0;
            var length = _addBetBtns.Length;

            while (money > 0 && i < maxBet)
            {
                i++;
                int index;
                if (money >= _addBetBtns[length - 1].GetComponent<AddBetBtn>().AddValue)
                {
                    index = length - 1;
                    money -= _addBetBtns[length - 1].GetComponent<AddBetBtn>().AddValue;
                }
                else if (money >= _addBetBtns[length - 2].GetComponent<AddBetBtn>().AddValue)
                {
                    index = length - 2;
                    money -= _addBetBtns[length - 2].GetComponent<AddBetBtn>().AddValue;
                }
                else if (money >= _addBetBtns[length - 3].GetComponent<AddBetBtn>().AddValue)
                {
                    index = length - 3;
                    money -= _addBetBtns[length - 3].GetComponent<AddBetBtn>().AddValue;
                }
                else if (money >= _addBetBtns[length - 4].GetComponent<AddBetBtn>().AddValue)
                {
                    index = length - 4;
                    money -= _addBetBtns[length - 4].GetComponent<AddBetBtn>().AddValue;
                }
                else if (money >= _addBetBtns[length - 5].GetComponent<AddBetBtn>().AddValue)
                {
                    index = length - 5;
                    money -= _addBetBtns[length - 5].GetComponent<AddBetBtn>().AddValue;
                }
                else
                {
                    index = 0;
                    money = 0;
                }

                var bet = CreatBet(trans, index);
                bets.Add(bet);
            }
         
            return bets.ToArray();
        }

      


        /// <summary>
        /// 收集桌面上的筹码
        /// </summary>
        public void CollectBet()
        {
            var gdata = App.GetGameData<GangwuGameData>();

            var playerList = gdata.PlayerList;

            //筹码处理
            foreach (var yxBaseGamePlayer in playerList)
            {
                var player = (PlayerPanel) yxBaseGamePlayer;
                if (player.Info != null && player.BetMoney > 0)
                {

                    for (var i = 0; i < player.Bets.Count; i++)
                    {
                        var bet = player.Bets[i];
                        //BigBetStack.Bets.Add(bet);
                        bet.transform.parent = BetFather;
                        bet.BeginMove(bet.transform.localPosition, BigBetStack.transform.localPosition, i * gdata.BetSpeace, BetFinishedType.Destroy,
                            () =>
                            {
                                BigBetStack.gameObject.SetActive(true);
                            });
                    }
                    player.Bets.Clear();
                }
            }

            AssignedStack();
        }


        /// <summary>
        /// 收集筹码数值,无过程
        /// </summary>
        public void CollectBetValue()
        {
            BigBetStack.gameObject.SetActive(true);
        }

        /// <summary>
        /// 分筹码堆
        /// </summary>
        /// <returns></returns>
        public void AssignedStack()
        {
            var gdata = App.GameData;
            var playerList = gdata.PlayerList;
            var len = playerList.Length;
            while (true)
            {
                var minMoney = 0;
                var playerIndex = -1;
                var allMoney = 0;
                //获取最小allin的值和位置
                for (var i = 0; i < len; i++)
                {
                    PlayerPanel player = (PlayerPanel)playerList[i];

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

                    for (var i = 0; i < len; i++)
                    {
                        var player = (PlayerPanel)playerList[i];
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

                    BigBetStack.Bet.text = YxUtiles.ReduceNumber(BigBetStack.BetValue);//App.GetGameData<GlobalData>().GetShowGold(BigBetStack.BetValue));
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
        public void AddToBigStack(int min)
        {
            var playerList = App.GameData.PlayerList;
            int len = playerList.Length;
            //int subNum = 0;
            Debug.Log("AddToBigStack");
            for (var i = 0; i < len; i++)
            {
                var player = (PlayerPanel)playerList[i];
                if (player.Info == null) continue;

                if (player.BetMoney > 0)
                {
                    var num = player.BetMoney > min ? min : player.BetMoney;
                    BigBetStack.BetValue += num;
                    player.BetMoney -= num;
                    player.BetLabel.gameObject.SetActive(false);
                    //subNum += player.BetMoney;
                    if (BigBetStack.Belong[player.Info.Seat])
                    {
                        BigBetStack.Belong[player.Info.Seat] = player.CurGameType != PlayerGameType.Fold;
                    }
                }
                else
                {
                    BigBetStack.Belong[i] = false;
                }
            }

            BigBetStack.Bet.text = YxUtiles.ReduceNumber(BigBetStack.BetValue);//App.GetGameData<GlobalData>().GetShowGold(BigBetStack.BetValue);
            BigBetStack.IsAdd = false;
        }
        /// <summary>
        /// 数据转移到小筹码堆上
        /// </summary>
        public void MoveDataToSmall()
        {
            YxDebug.Log("MoveDataToSmall");
            var gdata = App.GameData;
            var playerList = gdata.PlayerList;
            var len = playerList.Length;
            foreach (var stack in SmallBetStack)
            {
                if (stack.IsAdd)
                {
                    stack.IsAdd = BigBetStack.IsAdd;
                    stack.BetValue = BigBetStack.BetValue;
                    stack.SetBelong(BigBetStack.Belong);
                    stack.Bet.gameObject.SetActive(true);
                    stack.Bet.text = YxUtiles.ReduceNumber(stack.BetValue);//App.GetGameData<GlobalData>().GetShowGold(stack.BetValue);

                    BigBetStack.IsAdd = true;
                    BigBetStack.BetValue = 0;
                    BigBetStack.Bet.text = YxUtiles.ReduceNumber(stack.BetValue);//App.GetGameData<GlobalData>().GetShowGold(stack.BetValue);
                    //BigBetStack.Reset(false);
                    break;
                }
            }
            for (var i = 0; i < len; i++)
            {
                PlayerPanel panel = (PlayerPanel)playerList[i];
                if (panel.BetMoney <= 0 && BigBetStack.Belong[i])
                {
                    BigBetStack.Belong[i] = panel.CurGameType != PlayerGameType.AllIn;
                }
            }
        }

        #region 分筹码到赢家
        
        /// <summary>
        /// 分发筹码到指定的赢家
        /// </summary>
        public void SendBetToWin(List<ISFSObject> wins)
        {
            Facade.Instance<MusicManager>().Play("receiveBet");
            var wheres = new List<Transform>();
            var gdata = App.GameData;
            for (var i = 0; i < wins.Count; i++)
            {
                if (BigBetStack.Belong[wins[i].GetInt("seat")])
                {
                    wheres.Add(gdata.GetPlayer(wins[i].GetInt("seat"),true).HeadPortrait.transform);
                }
            }

            if (wheres.Count != 0)
            {
                BigBetStack.SendBetToSomewhere(wheres);
            }

            wheres.Clear();

            foreach (var stack in SmallBetStack)
            {
                for (var i = 0; i < wins.Count; i++)
                {
                    if (stack.Belong[wins[i].GetInt("seat")])
                    {
                        wheres.Add(gdata.GetPlayer(wins[i].GetInt("seat"), true).HeadPortrait.transform);
                    }
                }

                if (wheres.Count != 0)
                {
                    stack.SendBetToSomewhere(wheres);
                }
            }
        }

        #endregion

        /// <summary>
        /// 刷新加注按钮
        /// </summary>
        public void RefreshAddBetBtns()
        {
            var gdata = App.GetGameData<GangwuGameData>();
            var leastRoomGold = gdata.LeastRoomGold; //当前桌面游戏玩家筹码的最小值
            var minRoomGold = gdata.CardCount > 3 ? 0 : gdata.MinRoomGold;      //第三轮(即玩家4张牌)时,解除限制

            var betSum = App.GetGameManager<GangWuGameManager>().SpeakMgr.AddBetSum;

            for (var i = 0; i < _addBetBtns.Length; i++)
            {
                var betVal = _addBetBtns[i].GetComponent<AddBetBtn>().AddValue;
                var couldClick = leastRoomGold  - betSum - minRoomGold >= betVal;

                Tools.SetBtnCouldClick(_addBetBtns[i],couldClick);
                if (!couldClick)
                    _addBetBtns[i].GetComponent<AddBetBtn>().StopPress();
            }
        }


        /// <summary>
        /// 设置筹码下注倍数
        /// </summary>
        public void SetChipsTime()
        {
            var array = App.GameData.AnteRate;
            
            if (array == null || array.Count <= 0) return;

            int btnCount = _addBetBtns.Length;
            for (int i = 0; i < btnCount; i++)
            {
                _addBetBtns[i].gameObject.SetActive(false);
            }

            int count = Mathf.Min(btnCount, array.Count);       //获取按钮个数和配置信息个数相对少个哪一个
            for (int i = 0; i < count; i++)
            {
                var btn = _addBetBtns[i].GetComponent<AddBetBtn>();
                btn.AddValue = array[i];
                btn.gameObject.SetActive(true);
            }
        }

        public void Reset(bool clearBets = true)
        {
            BigBetStack.Reset();
            for (var i = 0; i < SmallBetStack.Length; i++)
            {
                SmallBetStack[i].Reset();
            }
        }

    }
    public class ChipData
    {
        /// <summary>
        /// 背景名称
        /// </summary>
        public int BgId;
        /// <summary>
        /// 值
        /// </summary>
        public int Value;

        public int Depth;
    }
}
