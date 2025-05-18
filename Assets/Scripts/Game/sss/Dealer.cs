using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.sss
{
    public class Dealer : MonoBehaviour
    {

        public Transform BrithTran;
        public GameObject PokerPrefab;


        public float SpaceTime = 0.03f;

        readonly List<GameObject> _pokerList = new List<GameObject>();

        /// <summary>
        /// 是否正在发牌
        /// </summary>
        bool _dealing;

        /// <summary>
        /// 发牌多个玩家的牌,有过程
        /// </summary>
        /// <param name="seats"></param>
        public void FaPai(int[] seats)
        {
            if (!_dealing)
            {
                _dealing = true;
                StartCoroutine(DoFaPai(seats));
            }
        }

        /// <summary>
        /// 为一个玩家创建13张牌,无过程
        /// </summary>
        public void FaPai(SssPlayer user)
        {
            int userCardsCount = user.UserCardList.Count;

            //如果玩家手牌够13张,则不需要发牌
            if (userCardsCount >= 13)
            {
                return;
            }

            int diff = 13 - userCardsCount;

            _dealing = false;
            StopAllCoroutines();
            for (int i = 0; i < diff; i++)    //每人补全发牌
            {
                GameObject poker = GetPoker((userCardsCount + i + 1) * 2);
                user.UserCardList.Add(poker);
            }
        }

        public float Scale = 0.6f;

        /// <summary>
        /// 记录扑克的个数
        /// </summary>
        private int _pokerCounter;

        IEnumerator DoFaPai(int[] seats)
        {
            var gdata = App.GetGameData<SssGameData>();
            int seatsLength = seats.Length;
            int pokerCount = seatsLength * 13;
            while (_pokerCounter < pokerCount && _dealing)
            {
                foreach (int seat in seats)
                {
                    GameObject poker = GetPoker(_pokerCounter + 1);
                    SssPlayer user = gdata.GetPlayer<SssPlayer>(seat, true);
                    Facade.Instance<MusicManager>().Play("sendcard");

                    user.UserCardList.Add(poker);
                    poker.transform.parent = user.PokerParent;
                    poker.transform.localScale = Vector3.one * Scale;
                    MoveCard(poker, poker.transform.localPosition, user.GetTargetPos());
                    RotateCard(poker, new Vector3(0, 0, -70), Vector3.zero);
                    AlphaCard(poker, 0, 1);
                }
                yield return new WaitForSeconds(SpaceTime);
            }

            //刷新一次扑克的父层级,保证扑克的正确渲染
            for (int i = 0; i < seats.Length; i++)
            {
                Transform parent = gdata.GetPlayer<SssPlayer>(i).PokerParent;
                parent.gameObject.SetActive(false);
                parent.gameObject.SetActive(true);
            }

            //扑克飞到手中再展开
            yield return new WaitForSeconds(0.3f);
            foreach (int seat in seats)
            {
                SssPlayer user = gdata.GetPlayer<SssPlayer>(seat, true);
                user.Waitting();
                if (user.FinishChoise)
                    user.MoveHandCardWithAnim();
            }

            _dealing = false;
            StopAllCoroutines();
        }



        GameObject GetPoker(int depth)
        {

            GameObject poker;
            if (_pokerCounter >= _pokerList.Count)
            {
                poker = Instantiate(PokerPrefab);
                poker.name = "poker_" + _pokerCounter;
                _pokerList.Add(poker);
            }
            else
            {
                poker = _pokerList[_pokerCounter];
            }
            InitPoker(poker, depth);
            ++_pokerCounter;
            return poker;
        }

        /// <summary>
        /// 将扑克初始化到发牌位置
        /// </summary>
        /// <param name="poker"></param>
        /// <param name="depth"></param>
        void InitPoker(GameObject poker, int depth)
        {

            poker.SetActive(true);
            Transform tran = poker.transform;
            tran.parent = BrithTran;
            tran.localPosition = Vector3.zero;
            tran.localScale = Vector3.one * 0.7f;
            tran.localEulerAngles = new Vector3(0, 0, -70);
            poker.GetComponent<PokerCard>().SetCardDepth(depth);
        }


        protected void RepositionCards(List<GameObject> pokerList)
        {
            if (pokerList.Count == 0)
                return;
            int mid = (pokerList.Count + 1) / 2;
            for (int i = 0; i < pokerList.Count; i++)
            {
                Vector3 tarPos = Vector3.right * 16 * (i - mid);

                MoveCard(pokerList[i], pokerList[i].transform.localPosition, tarPos);
            }
        }

        /// <summary>
        /// 移动牌
        /// </summary>
        /// <param name="poker">移动扑克</param>
        /// <param name="fromPos"></param>
        /// <param name="targetPos">目标位置</param>
        /// <param name="durtation"></param>
        void MoveCard(GameObject poker, Vector3 fromPos, Vector3 targetPos, float durtation = 0.3f)
        {
            var tp = poker.GetComponent<TweenPosition>();
            tp.from = fromPos;
            tp.to = targetPos;
            tp.duration = durtation;

            tp.ResetToBeginning();
            tp.PlayForward();
        }


        void RotateCard(GameObject poker, Vector3 from, Vector3 to, float durtation = 0.3f)
        {
            TweenRotation tr = poker.GetComponent<TweenRotation>();
            tr.from = from;
            tr.to = to;
            tr.duration = durtation;

            tr.ResetToBeginning();
            tr.PlayForward();
        }


        void AlphaCard(GameObject poker, float from, float to, float durtation = 0.3f)
        {
            TweenAlpha ta = poker.GetComponent<TweenAlpha>();
            ta.from = from;
            ta.to = to;
            ta.duration = durtation;

            ta.ResetToBeginning();
            ta.PlayForward();
        }


        public List<ShootItem> ShootItemList = new List<ShootItem>();



        public void Reset()
        {
            _dealing = false;
            foreach (GameObject poker in _pokerList)
            {
                poker.SetActive(false);
                PokerCard pokerCard = poker.GetComponent<PokerCard>();
                pokerCard.SetCardId(0);
                pokerCard.SetCardFront();
            }
            _pokerCounter = 0;
            ShootItemList.Clear();
            StopAllCoroutines();
        }
    }
}
