using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using Sfs2X.Entities.Data;
using System;
using YxFramwork.Tool;


namespace Assets.Scripts.Game.paijiu.Mgr
{
    /// <summary>
    /// 公共筹码管理类
    /// </summary>
    public class BetMgr : MonoBehaviour
    {


        /// <summary>
        /// 筹码prefab
        /// </summary>
        public GameObject BetPrefab = null;

        private readonly List<GameObject> _betList = new List<GameObject>();

        public AddBetBtnItem[] AddBetBtns = null;

        private int _betDepthCount;


        /// <summary>
        /// 创建一个筹码
        /// </summary>
        /// <param name="tf">创建筹码的父层级</param>
        /// <param name="sprName">获取贴图的索引,1-4</param>
        /// <param name="depth">显示层级</param>
        /// <param name="betVal">筹码数值</param>
        /// <returns></returns>
        public GameObject CreatBet(Transform tf, string sprName, int depth, int betVal)
        {
            GameObject gob = Instantiate(BetPrefab);
            gob.transform.parent = tf;
            gob.transform.localScale = Vector3.one * 0.6f;
            gob.transform.localPosition = Vector3.zero;
            Bet bet = gob.GetComponent<Bet>();

            bet.SetImage(sprName);
            bet.SetDepth(depth);
            bet.BetValueLabel.text =YxUtiles.ReduceNumber(betVal);

            bet.BetDepth = 110 + _betDepthCount;
            _betDepthCount += 2;

            return gob;
        }

        /// <summary>
        /// 创建一堆筹码
        /// </summary>
        /// <param name="money">最大钱数</param>
        /// <param name="maxBet">最大筹码数</param>
        /// <param name="trans">位置</param>
        /// <param name="depth">层级</param>
        /// <returns></returns>
        public GameObject[] CreatBetArray(int money, int maxBet, Transform trans, int depth = 300)
        {
            List<GameObject> bets = new List<GameObject>();

            var betCount = 0;
            for (var i = AddBetBtns.Length - 1; i >= 0; i--)
            {
                if (betCount > maxBet && money <= 0)
                    break;

                var betItem = AddBetBtns[i];

                if (money < betItem.BetValue)
                    continue;

                money -= betItem.BetValue;
                var bet = CreatBet(trans, betItem.SpriteName, depth, betItem.BetValue);
                bets.Add(bet);
                _betList.Add(bet);
                betCount++;
                i++;
            }
            return bets.ToArray();
        }



        public void InitChips(ISFSObject data)
        {

            int ante;
            var responseData = data;
            if (data.ContainsKey("cargs2"))
            {
                responseData = data.GetSFSObject("cargs2");
            }

            if (responseData == null)
            {
                return;
            }


            if (!Int32.TryParse(responseData.GetUtfString("-ante"), out ante) || ante <= 0)
            {
                com.yxixia.utile.YxDebug.YxDebug.LogError("=== Ante is wrong!! ===");
                ante = 1;
            }

            App.GetGameData<PaiJiuGameData>().Ante = ante;

            //如果包含此字段,则是由后台设置筹码的面值和个数,数据格式为: 1#2#3#4#...
            if (responseData.ContainsKey("-anteRate") && ante > 0)
            {
                bool couldSet = true;
                string str = responseData.GetUtfString("-anteRate");

                //处理筹码的面值
                string[] tempStr = str.Split('#');

                int[] array = new int[tempStr.Length > AddBetBtns.Length ? AddBetBtns.Length : tempStr.Length];

                for (int i = 0; i < array.Length; i++)
                {
                    if (!Int32.TryParse(tempStr[i], out array[i]))
                    {
                        com.yxixia.utile.YxDebug.YxDebug.LogError("=== There is some string count not be changed to int!! === ");
                        couldSet = false;
                        break;
                    }
                }

                if (couldSet)
                {
                    SetAddBtns(array);
                }
            }
        }

        private void SetAddBtns(int[] array)
        {
            var max = array.Length > AddBetBtns.Length ? AddBetBtns.Length : array.Length;
            for (var i = 0; i < max; i++)
            {
                AddBetBtns[i].AddTimes = array[i];
            }
        }


        public void AddBet(GameObject amount)
        {
            var bet = Instantiate(BetPrefab);
            var sp = bet.GetComponent<UISprite>() ?? BetPrefab.AddComponent<UISprite>();

            sp.spriteName = amount.GetComponent<UISprite>().spriteName;
            sp.MakePixelPerfect();

            bet.transform.localPosition = Vector3.zero;
        }

        public void Reset()
        {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _betList.Count; i++)
            {
                Destroy(_betList[i]);
            }
            _betList.Clear();
        }


    }
}
