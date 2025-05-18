using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using YxFramwork.Common;
using YxFramwork.Tool;
using com.yxixia.utile.YxDebug;
using UnityEngine;

namespace Assets.Scripts.Game.Tbs
{
    /// <summary>
    /// 筹码管理 单例类
    /// </summary>
    public class BetManager : MonoBehaviour
    {

        /// <summary>
        /// 是否可以下注
        /// </summary>
        public bool IsBeginBet = false;
        /// <summary>
        /// 筹码父节点
        /// </summary>
        public GameObject BG;
        /// <summary>
        /// 筹码
        /// </summary>
        public GameObject[] Bets;
        /// <summary>
        /// 当前选择的筹码下标
        /// </summary>
        public int CurBetIndex;
        /// <summary>
        /// 筹码值
        /// </summary>
        public int[] BetsValue;
        /// <summary>
        /// 自己下的筹码个数
        /// </summary>
        [HideInInspector]
        public int[] BetsCount;
        /// <summary>
        /// 下注区域最大的下注个数
        /// </summary>
        public int BetRegionMaxBet;
        /// <summary>
        /// 是否在加注
        /// </summary>
        public bool IsAddBet;
        /// <summary>
        /// 加注按键
        /// </summary>
        public UIButton AddBet;
        /// <summary>
        /// 减注按键
        /// </summary>
        public UIButton SubtractBet;

        protected void Start()
        {
            CurBetIndex = 0;
            OnBetClick(null, CurBetIndex);
            BetsCount = new int[BetsValue.Length];
            OnChangeBetClick();
        }

        public void OnBetClick(GameObject gob, int index = -1)
        {
            CurBetIndex = gob == null ? index : int.Parse(gob.name);
            var gmanager = App.GetRServer<TbsRemoteController>();
            /**********点击下注***********/
            if (!gmanager.HasGetGameInfo)
            {
                return;
            }

            int gold = BetsValue[CurBetIndex];

            gold = IsAddBet ? gold : -gold;

            if (App.GetGameData<TbsGameData>().GetPlayer<TbsPlayer>().UserBetRegion.IsBet(gold))
            {
                YxDebug.Log("下注!!" + gold);
                IDictionary bet = new Dictionary<string, object>();
                bet.Add("gold", gold);
                gmanager.SendRequest(GameRequestType.Bet, bet);
            }
        }

        public void CheckBets()
        {
            if (Bets.Length != BetsValue.Length)
            {
                YxDebug.Log("筹码与值的长度不相等!");
            }
            else
            {
                for (int i = 0; i < Bets.Length; i++)
                {
                    Bets[i].transform.FindChild("Title").GetComponent<UILabel>().text = YxUtiles.ReduceNumber(BetsValue[i]);
                }
            }
        }

        /// <summary>
        /// 默认返回bets下标0
        /// </summary>
        /// <param name="index">Bets下标</param>
        /// <param name="gold">通过钱获取</param>
        /// <returns></returns>
        public GameObject GetBet(int index = -1, int gold = -1)
        {
            if (index > 0)
            {
                if (index >= Bets.Length)
                {
                    YxDebug.Log("index > Bets的长度!");
                    return Bets[0];
                }
                return Bets[index];
            }

            if (gold > 0)
            {
                for (int i = 0; i < BetsValue.Length; i++)
                {
                    if (gold >= BetsValue[i])
                    {
                        if (i + 1 < BetsValue.Length)
                        {
                            if (gold < BetsValue[i + 1])
                            {
                                return Bets[i];
                            }
                        }
                        else
                        {
                            return Bets[i];
                        }

                    }
                }
                YxDebug.Log("Gold不在BetsValue中!!");
                return Bets[0];
            }

            return Bets[0];
        }
        /// <summary>
        /// 获取筹码个数
        /// </summary>
        /// <param name="gold">金币数量</param>
        /// <returns></returns>
        public int[] GetBetsValues(int gold)
        {
            var len = new int[BetsValue.Length];
            int betNum = 0;

            for (int i = len.Length - 1; i >= 0; i--)
            {
                if (gold >= BetsValue[i])
                {
                    gold -= BetsValue[i];
                    len[i]++;
                    betNum++;
                    i++;
                }
            }

            var values = new int[betNum];

            int index = 0;

            for (int i = 0; i < len.Length; i++)
            {
                for (int j = 0; j < len[i]; j++)
                {
                    values[index] = BetsValue[i];
                    index++;
                }
            }

            return values;

        }

        /// <summary>
        /// 底注
        /// </summary>
        public int Ante;

        /// <summary>
        /// 设置底注
        /// </summary>
        public void SetAnte(int ante, string[] chips)
        {
            YxDebug.Log(" 设置 Ante == " + ante);

            Ante = ante;
            var gdata = App.GetGameData<TbsGameData>();
            if (gdata.IsCreatRoom)
            {
                var antes = new[] { 1, 2, 5, 10, 20 };
                var value = Math.Floor((float)gdata.BankerLimit / 12);
                var firstValue = int.Parse(value.ToString(CultureInfo.InvariantCulture).Substring(0, 1));
                var digit = value.ToString(CultureInfo.InvariantCulture).Length;
                var minValue = firstValue * Math.Pow(10, digit - 1);
                for (int i = 0; i < BetsValue.Length; i++)
                {
                    BetsValue[i] = (int)minValue * antes[i];
                }
            }
            else
            {
                for (int i = 0; i < BetsValue.Length; i++)
                {
                    BetsValue[i] = int.Parse(chips[i]);
                }
            }

            CheckBets();
        }

        public void OpenPanel()
        {
            BG.SetActive(true);
        }

        public void ClosePanel()
        {
            BG.SetActive(false);
        }

        /// <summary>
        /// 改变加减注类型按键监听
        /// </summary>
        public void OnChangeBetClick()
        {
            AddBet.isEnabled = IsAddBet;
            SubtractBet.isEnabled = !IsAddBet;

            IsAddBet = !IsAddBet;
            if (IsAddBet)
            {
                foreach (var bet in Bets)
                {
                    bet.GetComponent<UIButton>().isEnabled = true;
                    bet.transform.FindChild("Selected").GetComponent<UISprite>().color = Color.white;
                }
            }
            else
            {
                for (int i = 0; i < Bets.Length; i++)
                {
                    Bets[i].GetComponent<UIButton>().isEnabled = BetsCount[i] > 0;
                    Bets[i].transform.FindChild("Selected").GetComponent<UISprite>().color = BetsCount[i] > 0 ? Color.white :
                        Color.gray;
                }
            }

        }
        /// <summary>
        /// 刷新减注筹码显示
        /// </summary>
        public void ReSubtractBet()
        {
            if (!IsAddBet)
            {
                for (int i = 0; i < Bets.Length; i++)
                {
                    Bets[i].GetComponent<UIButton>().isEnabled = BetsCount[i] > 0;
                    Bets[i].transform.FindChild("Selected").GetComponent<UISprite>().color = BetsCount[i] > 0 ? Color.white :
                        Color.gray;
                }
            }
        }
        /// <summary>
        /// 自己的筹码变化
        /// </summary>
        public void ChangeBetCount(int gold)
        {
            int add = gold > 0 ? 1 : -1;
            gold = Mathf.Abs(gold);
            for (int i = 0; i < BetsValue.Length; i++)
            {
                BetsCount[i] += gold == BetsValue[i] ? add : 0;
                BetsCount[i] = BetsCount[i] < 0 ? 0 : BetsCount[i];
            }
        }
        /// <summary>
        /// 重置筹码管理类
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < BetsCount.Length; i++)
            {
                BetsCount[i] = 0;
            }
        }

    }
}
