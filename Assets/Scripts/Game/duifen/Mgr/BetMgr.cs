using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using Sfs2X.Entities.Data;
using System;


namespace Assets.Scripts.Game.duifen.Mgr
{
    /// <summary>
    /// 公共筹码管理类 单例
    /// </summary>
    public class BetMgr : MonoBehaviour {


        /// <summary>
        /// 筹码prefab
        /// </summary>
        public GameObject BetPrefab = null;
        /// <summary>
        /// 筹码堆父节点
        /// </summary>
        public Transform BetFather;
     
       

        public AddBetBtnItem[] AddBetBtns = null;

        public Transform BetParent = null;

        private int _betDepthCount;


        public void PlayerBet(DuifenPlayerPanel playerPanel, int money, int depth = 99)
        {
            if (money <= 0)
            {
                return;
            }

            GameObject[] bets = CreatBetArray(money, 9, playerPanel.transform, depth);

            for (int i = 0; i < bets.Length; i++)
            {
                Bet bet = bets[i].GetComponent<Bet>();
                bet.transform.parent = BetParent;
                BetParent.GetComponent<UIPanel>().depth = 4; //为下注筹码在牌上方飞出,设置层级关系

                bet.BeginMove(
                    bet.transform.localPosition, bet.GetTableRandomPos(),
                    i*App.GetGameData<DuifenGlobalData>().BetSpeace, BetFinishedType.None, 0.3f,
                    () =>
                    {
                        gameObject.SetActive(true);
                        bet.transform.parent = BetParent;
                        BetParent.GetComponent<UIPanel>().depth = 2; //将层级重置回正常,为发牌时牌在桌面筹码上方飞出
                    });
            }
        }


        /// <summary>
        /// 创建一个筹码
        /// </summary>
        /// <param name="tf">创建筹码的父层级</param>
        /// <param name="sprName">获取贴图的索引,1-4</param>
        /// <param name="depth">显示层级</param>
        /// <param name="betVal">加注值</param>
        /// <returns></returns>
        public GameObject CreatBet(Transform tf, string sprName,int depth,int betVal)
        {
            GameObject gob = Instantiate(BetPrefab);
            gob.transform.parent = tf;
            gob.transform.localScale = Vector3.one * 0.6f;
            gob.transform.localPosition = Vector3.zero;
            Bet bet = gob.GetComponent<Bet>();

            bet.SetImage(sprName);
            bet.BetValueLabel.text = App.GetGameData<DuifenGlobalData>().GetShowGoldValue(betVal, true);
        
            bet.BetDepth = 110 + _betDepthCount;
            _betDepthCount += 2;

            return gob;
        }

        public Transform Winner;

        public void MoveAllBetToSomeWhere()
        {
            int childCount = BetParent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var bet = BetParent.GetChild(i).GetComponent<Bet>();
                if(bet == null)
                    continue;
                BetParent.GetComponent<UIPanel>().depth = 4;
                bet.BeginMove(from: bet.transform.localPosition, to: Winner.localPosition, delay: 0.5f, fType: BetFinishedType.Destroy, flyTime: 0.7f,
                    callback:() =>
                    {
                        BetParent.GetComponent<UIPanel>().depth = 2;
                    });

            }
        }

        /// <summary>
        /// 创建一堆筹码
        /// </summary>
        /// <param name="money">最大钱数</param>
        /// <param name="maxBet">最大筹码数</param>
        /// <param name="trans">下注父层级</param>
        /// <param name="depth">渲染层级</param>
        /// <returns></returns>
        public GameObject[] CreatBetArray(int money,int maxBet,Transform trans,int depth = 99)
        {
            List<GameObject> bets = new List<GameObject>();

            int betCount = 0;
            for (int i = AddBetBtns.Length - 1; i >= 0; i--)
            {
                if (betCount > maxBet && money <= 0)
                    break;

                var betItem = AddBetBtns[i];
                if (money >= betItem.BetValue)
                {
                    money -= betItem.BetValue;
                    var bet = CreatBet(trans, betItem.SpriteName, depth, betItem.BetValue);
                    bets.Add(bet);
                    betCount++;
                    i++;
                }
            }
            return bets.ToArray();
        }



        public void PanDuan(ISFSObject data)
        {

            int ante;
            ISFSObject responseData = null;
            if (data.ContainsKey("cargs2"))
            {
                responseData = data.GetSFSObject("cargs2");
            }

            if (responseData == null)
            {
                return;
            }


            if (!Int32.TryParse(responseData.GetUtfString("-ante"), out ante))
            {
                com.yxixia.utile.YxDebug.YxDebug.LogError("=== Ante is wrong!! ===");
            }
            else
            {
                ante = 1;
            }

            App.GetGameData<DuifenGlobalData>().Ante = ante;

            string str = responseData.ContainsKey("-anteconfig")
                ? responseData.GetUtfString("-anteconfig")
                : "5|5#10#20";
            //如果包含此字段,则是由后台设置筹码的面值和个数,数据格式为: 1#2#3#4#...
            if (ante > 0)
            {
                bool couldSet = true;

                //*首先找到每局的盲注值,只用于对分游戏
                if(str.Contains("|"))
                {
                   string[] tStr = str.Split('|');
                    int guoBet;
                    if(int.TryParse( tStr[0], out guoBet))
                    {
                        App.GetGameData<DuifenGlobalData>().GuoBet = guoBet;
                        
                    }
                    str = tStr[1];
                }
                //*/

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

        void SetAddBtns(int[] array)
        {
            int max = array.Length > AddBetBtns.Length ? AddBetBtns.Length : array.Length;
            for (int i = 0; i < max; i++)
            {
                AddBetBtns[i].AddTimes = array[i];
            }
        }

        
        public void AddBet(GameObject amount)
        {
          GameObject bet = Instantiate(BetPrefab);
          UISprite sp = bet.GetComponent<UISprite>();
          if (sp == null)
          {
              sp = BetPrefab.AddComponent<UISprite>();
          }

          sp.spriteName = amount.GetComponent<UISprite>().spriteName;
          sp.MakePixelPerfect();

          bet.transform.localPosition = Vector3.zero;
        }



      
       
    }
}
