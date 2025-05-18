using System.Collections.Generic;
using Assets.Scripts.Game.sssjp.ImgPress.Main;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.sssjp
{
    public class ChoiseMgr : MonoBehaviour,IChoisePanel
    {
        public virtual void Test(int[] cardArray)
        {
            
        }

        public virtual void Init()
        {
            
        }

        public virtual void ShowChoiseView(Sfs2X.Entities.Data.ISFSObject cardData)
        {
            //移动时钟
            App.GetGameManager<SssjpGameManager>().MoveClock(ClockParent);
        }
   

        public virtual void OnClickCard(PokerCard card)
        {
            
        }

        public virtual void OnDragOverCard(PokerCard card)
        {
            
        }

        public virtual void Reset()
        {
            //gameObject.SetActive(false);
        }

        public virtual void HideChoiseView()
        {
            gameObject.SetActive(false);
            App.GetGameManager<SssjpGameManager>().MoveClock();
        }

        /// <summary>
        /// 自定义数字,一共有多少张扑克
        /// </summary>
        [SerializeField]
        protected int CardCount = 13;

        [HideInInspector]
        public List<PokerCard> CardsList = new List<PokerCard>();


        [SerializeField]
        protected PokerCard CardPrefab;

        /// <summary>
        /// 扑克的父层级
        /// </summary>
        [SerializeField]
        protected Transform PokerParent;

        /// <summary>
        /// 倒计时中标位置
        /// </summary>
        public Transform ClockParent;

        /// <summary>
        /// 检测扑克个数
        /// </summary>
        protected virtual void InitPokerList()
        {
            if (CardsList.Count == CardCount)
                return;


            if (CardsList.Count > CardCount)
            {
                //删除多余的牌
                int count = CardsList.Count - CardCount;
                for (int i = 0; i < count; i++)
                {
                    var temp = CardsList[i];
                    CardsList.Remove(temp);
                    Destroy(temp);
                }
            }
            else
            {
                int count = CardCount - CardsList.Count;
                for (int i = 0; i < count; i++)
                {
                    PokerCard clone = Instantiate(CardPrefab);
                    clone.transform.parent = PokerParent;
                    clone.name = "poker " + i;
                    clone.transform.localScale = Vector3.one * .6f;
                    clone.GetComponent<PokerCard>().SetCardDepth(130 + i * 2);
                    CardsList.Add(clone);
                }
            }
        }

        public void AddPokerTriggerOnClick(MonoBehaviour target, PokerCard poker,string mothName)
        {
            var trigger = GetUiEventTrigger(poker);
            var ed = GetEventDelegate(target, poker, mothName);
            EventDelegate.Add(trigger.onClick, ed);
        }

        protected UIEventTrigger GetUiEventTrigger(PokerCard card)
        {
            return card.GetComponentInChildren<UIEventTrigger>();
        }

        protected EventDelegate GetEventDelegate(MonoBehaviour target, PokerCard poker, string mothName)
        {
            var ed = new EventDelegate(target, mothName);
            ed.parameters[0] = new EventDelegate.Parameter(poker, "PokerCard");
            return ed;
        }
    }
}