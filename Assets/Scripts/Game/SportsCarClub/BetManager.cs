using UnityEngine;
using com.yxixia.utile.YxDebug;
using YxFramwork.Common;
using YxFramwork.Tool;
using System.Collections.Generic;
using System;

namespace Assets.Scripts.Game.SportsCarClub
{
    /// <summary>
    /// 筹码管理
    /// </summary>
    public class BetManager : MonoBehaviour
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static BetManager _instance;

        public static BetManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new BetManager();
            }

            return _instance;
        }

        void Awake()
        {
            _instance = this;
        }

        /// <summary>
        /// 是否可以下注
        /// </summary>
        public bool IsBeginBet = false;
        /// <summary>
        /// 筹码
        /// </summary>
        public GameObject[] Bets;
        /// <summary>
        /// 当前选择的筹码下标
        /// </summary>
        public int CurBetIndex;
        /// <summary>
        /// 玩家下注起始位置
        /// </summary>
        public Transform AllPlayerPos;

        public TweenInfo TweenInfo;

        // Use this for initialization
        void Start ()
        {
            CurBetIndex = 0;
            OnBetClick(null,CurBetIndex);
            //CheckBets();
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public void OnBetClick(GameObject gob,int index = -1)
        {
            CurBetIndex = gob == null ? index : int.Parse(gob.name);
            for (int i = 0; i < Bets.Length; i++)
            {
                var go = Bets[i].transform.FindChild("Selected").gameObject;
                go.SetActive(i == CurBetIndex);
            }
        }

        public void CheckBets()
        {
            var betValue = App.GetGameData<CschGameData>().AnteRate;
            var betLen = Bets.Length;
            var min = Mathf.Min(betLen, betValue.Count);
            var i = 0;
            for (; i < min; i++)
            {
                var bet = Bets[i];
                bet.SetActive(true);
                bet.transform.FindChild("Title").GetComponent<UILabel>().text = YxUtiles.ReduceNumber(betValue[i]);
            }
            for (; i < betLen; i++)
            {
                Bets[i].SetActive(false);
            }
        }
        /// <summary>
        /// 通过gold获取显示钱的值
        /// </summary>
        /// <param name="gold"></param>
        /// <returns></returns>
        public string GoldString(int gold)
        {
            if (gold < 1000)
            {
                return gold.ToString();
            }
            else if (gold < 10000)
            {
                return ((int)(gold/1000)).ToString() + "千";
            }
            else
            {
                return ((int)(gold / 10000)).ToString() + "万";
            }
        }
        /// <summary>
        /// 默认返回bets下标0
        /// </summary>
        /// <param name="index">Bets下标</param>
        /// <param name="gold">通过钱获取</param>
        /// <returns></returns>
        public GameObject GetBet(int index = -1,int gold = -1)
        {
            if (index > 0)
            {
                if (index < Bets.Length) { return Bets[index];}
                YxDebug.Log("index > Bets的长度!");
                return Bets[0];
            }
            if (gold > 0)
            {
                var betValues = App.GetGameData<CschGameData>().AnteRate;
                var len = betValues.Count;
                for (var i = len-1; i > 0; i--)
                {
                    var bet = betValues[i];
                    if (gold >= bet)
                    {
                        return Bets[i];
                    }
                }
                YxDebug.Log("Gold不在BetsValue中!!");
                return Bets[0];
            }
            return Bets[0];
        }
    }
    [Serializable]
    public class TweenInfo
    {
        public AnimationCurve TweenCurve;
        public float Delay;
        public float Duration;
        public List<EventDelegate> OnFinish;
    }
}
