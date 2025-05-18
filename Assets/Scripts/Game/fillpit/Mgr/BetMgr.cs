using System;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;


namespace Assets.Scripts.Game.fillpit.Mgr
{
    /// <summary>
    /// 公共筹码管理类 单例
    /// </summary>
    public class BetMgr : MonoBehaviour {

        /// <summary>
        /// 筹码prefab
        /// </summary>
        public GameObject BetPrefab = null;


        public AddBetBtnItem[] AddBetBtns = null;

        public Transform BetParent = null;

        private int _betDepthCount;

        public int BetSpriteMaxCount = 4;

        public string BetSpriteNameFormat = "bet_{0}";


        /// <summary>
        /// 创建一个筹码
        /// </summary>
        /// <param name="tf">创建筹码的父层级</param>
        /// <param name="sprName">获取贴图的索引,1-4</param>
        /// <param name="depth">显示层级</param>
        /// <param name="betVal">加注值</param>
        /// <returns></returns>
        public GameObject CreatBet(Transform tf, string sprName, int depth, int betVal)
        {
            GameObject gob = Instantiate(BetPrefab);
            gob.transform.parent = tf;
            gob.transform.localScale = Vector3.one * 0.6f;
            gob.transform.localPosition = Vector3.zero;
            Bet bet = gob.GetComponent<Bet>();

            bet.SetImage(sprName);
            bet.BetValueLabel.text = YxUtiles.ReduceNumber(betVal);//App.GetGameData<GlobalData>().GetShowGold(betVal, true);

            bet.BetDepth = 200 + _betDepthCount;
            _betDepthCount += 2;

            return gob;
        }

        public void SetAddBtns(IList<int> array)
        {
            int btnLen = AddBetBtns.Length;
            int arrayLen = array.Count;
            for (int i = 0; i < btnLen; i++)
            {
                var btn = AddBetBtns[i];
                if (i >= arrayLen)
                {
                    btn.gameObject.SetActive(false);
                    continue;
                }
                btn.AddBetValue = array[i];
                btn.gameObject.SetActive(true);
            }
        }

        public void SetAddBtns()
        {
            var anteRate = App.GameData.AnteRate;
            SetAddBtns(anteRate);
        }

        public void PanDuan(ISFSObject data)
        {
            ISFSObject responseData = null;
            if (data.ContainsKey("cargs2"))
            {
                responseData = data.GetSFSObject("cargs2");
            }

            if (responseData == null)
            {
                return;
            }
        
            //如果包含此字段,则是由后台设置筹码的面值和个数,数据格式为: 1#2#3#4#...
            if (responseData.ContainsKey("-anteRate"))
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
                        com.yxixia.utile.YxDebug.YxDebug.LogError(
                            "=== There is some string count not be changed to int!! === ");
                        couldSet = false;
                        break;
                    }
                }

                if (couldSet)
                {
                    SetAddBtns(array);
                }
                else
                {
                    SetDefaultAddBtn();
                }
            }
            else
            {
                SetDefaultAddBtn();
            }
        }

        /// <summary>
        /// 添加默认按钮倍数
        /// </summary>
        void SetDefaultAddBtn()
        {
            var anteRate = App.GameData.AnteRate;

            //如果包含此字段,则是由后台设置筹码的面值和个数,数据格式为: 1#2#3#4#...
            if (anteRate == null || anteRate.Count <= 0) return;
            int rateCount = anteRate.Count;
            int btnCount = AddBetBtns.Length;
            int minCount = Mathf.Min(rateCount, btnCount);
            var arrayList = new List<int>();
            for (int i = 0; i < minCount; i++)
            {
                arrayList.Add(anteRate[i]);
            }

            SetAddBtns(arrayList);
        }

        //void SetAddBtns(IList<int> array)
        //{
        //    int max = array.Count > AddBetBtns.Length ? AddBetBtns.Length : array.Count;
        //    for (int i = 0; i < max; i++)
        //    {
        //        var betItem = AddBetBtns[i];
        //        betItem.AddTimes = array[i];
        //        betItem.gameObject.SetActive(true);
        //    }
        //}


        /// <summary>
        /// 创建一堆筹码
        /// </summary>
        /// <param name="money">最大钱数</param>
        /// <param name="maxBet">最大筹码数</param>
        /// <param name="trans">下注父层级</param>
        /// <param name="depth">渲染层级</param>
        /// <returns></returns>
        public GameObject[] CreatBetArray(int money, int maxBet, Transform trans, int depth = 99)
        {
            List<GameObject> bets = new List<GameObject>();
            var anteRate = App.GameData.AnteRate;
            int anteCount = anteRate.Count;
            int betCount = 0;
            for (int i = anteCount - 1; i >= 0;)
            {
                if (betCount > maxBet && money <= 0)
                    break;

                int betVal = anteRate[i];
                if (money >= betVal)
                {
                    money -= betVal;
                    string spriteName = string.Format(BetSpriteNameFormat, i % BetSpriteMaxCount);
                    var bet = CreatBet(trans, spriteName, depth, betVal);
                    bets.Add(bet);
                    betCount++;
                }
                else
                {
                    i--;
                }
                //var betItem = AddBetBtns[i];
                //if (!betItem.gameObject.activeSelf) continue;
                //if (money >= betItem.BetValue)
                //{
                //    money -= betItem.BetValue;
                //    var bet = CreatBet(trans, betItem.SpriteName, depth, betItem.BetValue);
                //    bets.Add(bet);
                //    betCount++;
                //    i++;
                //}
            }
            return bets.ToArray();
        }

        public void AddBet(GameObject amount)
        {
            GameObject bet = Instantiate(BetPrefab);
            UISprite sp = bet.GetComponent<UISprite>() ?? BetPrefab.AddComponent<UISprite>();

            sp.spriteName = amount.GetComponent<UISprite>().spriteName;
            sp.MakePixelPerfect();

            bet.transform.localPosition = Vector3.zero;
        }


        public void MoveAllChipToSomewhere(Transform targetTran)
        {
            var panel = BetParent.GetComponent<UIPanel>();
            panel.depth = 4;
            var chipCount = BetParent.childCount;
            var tarPos = BetParent.InverseTransformPoint(targetTran.position);
            
            for (int i = 0; i < chipCount; i++)
            {
                var chip = BetParent.GetChild(i);
                var bet = chip.GetComponent<Bet>();
                bet.Tp.duration = 0.3f;
                bet.BeginMove(chip.localPosition, tarPos, 0f, BetFinishedType.Destroy, () => { panel.depth = 2; });
            }
        }
    }
}
