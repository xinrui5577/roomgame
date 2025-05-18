using System.Collections.Generic;
using com.yxixia.utile.YxDebug;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.fillpit
{
    public class BetStack : MonoBehaviour
    {
        /// <summary>
        /// 筹码值显示
        /// </summary>
        public UILabel Bet;
        /// <summary>
        /// 筹码值
        /// </summary>
        public int BetValue;
        /// <summary>
        /// 是否可以添加筹码 allin 以后此堆不可在添加
        /// </summary>
        [HideInInspector]
        public bool IsAdd;
        /// <summary>
        /// 属于此堆的筹码
        /// </summary>
        public List<Bet> Bets;
        /// <summary>
        /// 牌堆谁可以赢
        /// </summary>
        //[HideInInspector]
        public bool[] Belong;
      
        /// <summary>
        /// 设置归属
        /// </summary>
        /// <param name="tof"></param>
        public void SetBelong(bool tof)
        {
            if (Belong == null || Belong.Length == 0)
            {
                Belong = new bool[App.GetGameData<FillpitGameData>().PlayerList.Length];
            }

            for (int index = 0; index < Belong.Length; index++)
            {
                Belong[index] = tof;
            }
        }
        /// <summary>
        /// 根据一个bool数组设置归属
        /// </summary>
        /// <param name="belongs"></param>
        public void SetBelong(bool[] belongs)
        {
            if (belongs.Length < Belong.Length)
            {
                YxDebug.LogError("数组长度不能低于本地的长度!");
                return;
            }

            for (int i = 0; i < Belong.Length; i++)
            {
                Belong[i] = belongs[i];
            }
        }

        /// <summary>
        /// 复位
        /// </summary>
        public void Reset(bool isClearBet = true)
        {
            BetValue = 0;
            IsAdd = true;
            SetBelong(true);
            gameObject.SetActive(false);

            if (!isClearBet)
                return;

            if (Bets == null || Bets.Count == 0)
            {
                Bets = new List<Bet>();
            }
            else
            {
                Bets.Clear();
            }
        }
        /// <summary>
        /// 发送所有筹码到某处
        /// </summary>
        public void SendBetToSomewhere(List<Transform> somewhere)
        {
            //生成筹码 最多16个
            GameObject[] betObjs = App.GetGameManager<FillpitGameManager>().BetMgr.CreatBetArray(BetValue, 16,transform);

            foreach (GameObject betObj in betObjs)
            {
                betObj.SetActive(false);
                Bets.Add(betObj.GetComponent<Bet>());
            }

            for (int i = 0; i < Bets.Count; i++)
            {
                Bet bet = Bets[i];
                bet.transform.parent = somewhere[i%somewhere.Count];
                bet.BeginMove(bet.transform.localPosition, Vector3.zero, i * App.GetGameData<FillpitGameData>().BetSpeace, BetFinishedType.Destroy,
                    () =>
                    {
                        
                    });
            }

            Reset();
            gameObject.SetActive(false);
        }
    }
}
