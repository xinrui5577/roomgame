using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Game.paijiu.ImgPress.Main;
using YxFramwork.Common;


namespace Assets.Scripts.Game.paijiu.Mgr
{
    public class CompareMgr : MonoBehaviour
    {

        private readonly List<int> _seatList = new List<int>();


        /// <summary>
        /// 添加参与比牌的座位号
        /// </summary>
        /// <param name="seat"></param>
        public void AddSeat(int seat)
        {
            _seatList.Add(seat);
        }


        /// <summary>
        /// 开始比牌
        /// </summary>
        public void StartCompare()
        {
            _seatList.Sort((seat1, seat2) => seat1 - seat2);      //座位号排序
            StartCoroutine(BeginCompare());
        }

        private IEnumerator BeginCompare()
        {
            const int groupCount = 2;   //手牌组的个数
            var groupIndex = 0;         //组索引
            var cardCounter = 0;
            var seatListCount = _seatList.Count;
            var chumenSeat = (YxFramwork.Common.App.GetGameData<PaiJiuGameData>().BankerSeat + seatListCount + 1) % seatListCount;

            var main = App.GetGameManager<PaiJiuGameManager>();
            var gdata = App.GetGameData<PaiJiuGameData>();

            var panels = gdata.PlayerList;

            while (groupIndex < groupCount)
            {
                int seat = _seatList[(cardCounter + chumenSeat) % seatListCount];   //由初门先翻牌
                //找到对应的牌,翻牌
                var user = (PaiJiuPlayer)panels[seat];
                var userBetPoker = user.UserBetPoker;

                int firstCardIndex = groupIndex * 2;    //翻牌牌组的第一张牌索引
                userBetPoker.SetBetPokerInfo(groupIndex);

                userBetPoker.PlayerPokers[firstCardIndex % userBetPoker.PlayerPokers.Count].TurnCard();     //翻开牌组的第一张牌
                yield return new WaitForSeconds(0.5f);
                userBetPoker.PlayerPokers[(firstCardIndex + 1) % userBetPoker.PlayerPokers.Count].TurnCard();   //翻开牌组的第二张牌
                yield return new WaitForSeconds(0.5f);
                userBetPoker.ShowGroupType(groupIndex);


                cardCounter++;
                groupIndex = cardCounter / seatListCount;
            }
        }


        public void StopCompare()
        {
            StopAllCoroutines();
        }


        public void Reset()
        {
            StopCompare();
            _seatList.Clear();
        }
    }


}