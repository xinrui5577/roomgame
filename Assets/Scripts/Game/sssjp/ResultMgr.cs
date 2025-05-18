using System.Collections;
using System.Collections.Generic;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.sssjp
{

    public class ResultMgr : MonoBehaviour
    {

        [SerializeField]
#pragma warning disable 649
        private GameObject _betPrefab;
#pragma warning restore 649

        [HideInInspector]
        public List<int> WinSeats = new List<int>(); //服務器座位號

        [HideInInspector]
        public List<int> LoseSeats = new List<int>();

        private readonly List<GameObject> _betList = new List<GameObject>();


        // Use this for initialization
        protected void Start()
        {
            CreateBets(12);
        }


        public void ResultFlyChips(ISFSObject data)
        {
            var gdata = App.GetGameData<SssGameData>();

            ISFSArray resultArray = data.GetSFSArray("score");
            foreach (ISFSObject result in resultArray)
            {

                int seat = result.GetInt("seat");
                //显示玩家输赢的分数
                int score = result.GetInt("score");

                SssPlayer resultPanel = gdata.GetPlayer<SssPlayer>(seat, true);
                resultPanel.ShowResultLabel(score);

                if (result.ContainsKey("gold"))
                {
                    resultPanel.Coin = result.GetLong("gold"); 
                }

                if (score == 0)
                    continue;

                if (score > 0)
                {
                    WinSeats.Add(seat);
                }
                else
                {
                    LoseSeats.Add(seat);
                }
            }

            DoResult();
        }


        public void DoResult()
        {
            if (_betPrefab == null) return;
            
            if (WinSeats.Count == 0 || LoseSeats.Count == 0)
                return;

            var gdata = App.GetGameData<SssGameData>();

            int betCount;
            int count = 0;      //用于记录筹码飞向的赢家的座位号
            //如果只有一个人数,那么扔出的筹码是赢的人数*12
            if (LoseSeats.Count == 1)
            {
                betCount = WinSeats.Count * 12;
                foreach (int winSeat in WinSeats)
                {
                    Transform birParent = gdata.GetPlayer<SssPlayer>(LoseSeats[0], true).BetParent;
                    StartCoroutine(MoveBet(birParent, gdata.GetPlayer<SssPlayer>(winSeat, true).BetParent, betCount));
                }
            }
            else
            {
                betCount = 12;
                foreach (int lostSeat in LoseSeats)
                {
                    Transform birParent = gdata.GetPlayer<SssPlayer>(lostSeat, true).BetParent;
                    StartCoroutine(MoveBet(birParent, gdata.GetPlayer<SssPlayer>(WinSeats[count++ % WinSeats.Count], true).BetParent, betCount));

                }
            }
        }


        private int _betIndex;
        IEnumerator MoveBet(Transform from, Transform to, int count = 12)
        {
            Facade.Instance<MusicManager>().Play("add_gold");
            for (int i = 0; i < count; i++)
            {
                GameObject betObj = GetOneBet(_betIndex++);
                betObj.transform.parent = from;
                betObj.transform.localPosition = Vector3.zero;
                betObj.transform.localScale = Vector3.one;
                Bet bet = betObj.GetComponent<Bet>();
                bet.TargetTran = to;
                bet.transform.parent = transform;
                Vector3 target = GetRandomPos();
                bet.BeginMove(target, 0.3f);
                yield return null;// new WaitForSeconds(0f);
            }
        }

        public void Reset()
        {
            WinSeats.Clear();
            LoseSeats.Clear();
            foreach (GameObject bet in _betList)
            {
                bet.SetActive(false);
            }
            _betIndex = 0;
        }


        /// <summary>
        /// 获取一个随机的坐标
        /// </summary>
        /// <returns></returns>
        Vector3 GetRandomPos()
        {
            int posX = Random.Range(-64, 64);
            int posY = Random.Range(-64, 64);

            return new Vector3(posX, posY, 0);
        }


        /// <summary>
        /// 创建筹码
        /// </summary>
        /// <param name="betCount">创建的个数</param>
        void CreateBets(int betCount)
        {
            if (_betPrefab == null) return;
            for (int i = 0; i < betCount; i++)
            {
                GameObject bet = Instantiate(_betPrefab);
                bet.transform.parent = transform;
                bet.transform.localScale = Vector3.one;
                bet.SetActive(false);
                _betList.Add(bet);
            }
        }


        GameObject GetOneBet(int index)
        {
            //如果获取的个数大于当前筹码的个数,创建12个
            if (index > _betList.Count - 1)
            {
                CreateBets(12);
            }

            GameObject bet = _betList[index];
            bet.SetActive(true);
            return bet;
        }


    }
}