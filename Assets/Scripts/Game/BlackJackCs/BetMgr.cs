using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.BlackJackCs
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
        //public Transform BetFather;


        /// <summary>
        /// 是否在筹码上显示面值
        /// </summary>
        [Tooltip("是否在筹码上显示面值")]
        public bool ShowBetValue = true;
       

        public AddBetBtnItem[] AddBetBtns = null;

        private int _betDepthCount;

        /// <summary>
        /// 创建一个筹码
        /// </summary>
        /// <param name="tf">创建筹码的父层级</param>
        /// <param name="nameId">获取贴图的索引,1-4</param>
        /// <returns></returns>
        public GameObject CreateBet(Transform tf, int nameId)
        {
            GameObject gob = Instantiate(BetPrefab);
            gob.transform.parent = tf;
            gob.transform.localScale = Vector3.one;
            gob.transform.localPosition = Vector3.zero;
            Bet bet = gob.GetComponent<Bet>();

            bet.SetImage(nameId);
            //应客户要求,在筹码上添加数值
            if (App.GetGameData<BlackJackGameData>().IsRoomGame)
            {
                bet.BetValueLabel.gameObject.SetActive(true);

                if (nameId == 4)
                {
                    bet.BetValueLabel.text = "5";
                }
                else
                {
                    bet.BetValueLabel.text = nameId.ToString();
                }
            }
            else
            {
                bet.BetValueLabel.gameObject.SetActive(false);
            }
            bet.BetDepth = 110 + _betDepthCount;
            _betDepthCount += 2;
            return gob;
        }


        /// <summary>
        /// 创建一个筹码
        /// /// </summary>
        /// <param name="tf">筹码的父层级</param>
        /// <param name="money">下注的筹码面值(可能不现实)</param>
        /// <param name="spriteName">筹码的贴图名称</param>
        /// <returns></returns>
        public GameObject CreatBet(Transform tf, int money, string spriteName)
        {
            GameObject gob = Instantiate(BetPrefab);
            gob.transform.parent = tf;
            gob.transform.localScale = Vector3.one;
            gob.transform.localPosition = Vector3.zero;
            UISprite sp = gob.GetComponent<UISprite>();
            sp.spriteName = spriteName;
            sp.depth = _betDepthCount;

            if (!ShowBetValue)
            {
                gob.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                UILabel label = gob.GetComponentInChildren<UILabel>();
                label.text = YxUtiles.ReduceNumber(money); //Tool.Tools.GetChipFaceValue(money , true);
                label.depth = _betDepthCount + 1;
            }
            _betDepthCount += 2;
            return gob;
        }


        /// <summary>
        /// 创建一堆筹码
        /// </summary>
        /// <param name="money">最大钱数</param>
        /// <param name="maxBet">最大筹码数</param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public GameObject[] CreatBetArray(int money,int maxBet,Transform trans)
        {

            List<GameObject> bets = new List<GameObject>();
            int i = 0;
            int betM = money;
            int length = AddBetBtns.Length;


            while (betM > 0 && i < maxBet)
            {
                i++;

                for (int j = length - 1; j >= 0; j--)
                {
                    var go = AddBetBtns[j];
                    //如果钱的面值大于或等于当前筹码的面值,创建一个当前筹码
                    if (betM >= go.BetValue)
                    {
                        UISprite spr = go.GetComponent<UISprite>() ?? go.transform.GetComponentInChildren<UISprite>();
                        name = spr.spriteName;
                        betM -= go.BetValue;        //总钱数减去创建的筹码值
                        if (!string.IsNullOrEmpty(name))
                        {
                            GameObject bet = CreatBet(trans, go.BetValue, name);
                            bets.Add(bet);
                        }
                        break;
                    }
                }
            }

            return bets.ToArray();
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

        public void Reset()
        {
            _betDepthCount = 0;
        }
    
    }
}
