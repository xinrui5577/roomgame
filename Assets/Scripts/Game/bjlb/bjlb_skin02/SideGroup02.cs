using System;
using UnityEngine;

namespace Assets.Scripts.Game.bjlb.bjlb_skin02
{
    public class SideGroup02 : MonoBehaviour
    {

        /// <summary>
        /// 牌组(牌和盖数字牌)的预制
        /// </summary>
        public GameObject CardGroupPrefab;

        /// <summary>
        /// 只显示背面盖牌
        /// </summary>
        public UISprite CoverCard;

        /// <summary>
        /// 每个牌的动画平移距离
        /// </summary>
        public float Shift;

        public UITweener[] Tweens;

        private CardGroup02[] _cardGroupArray;


        /// <summary>
        /// 设置Group,显示的牌,牌移动的位置等
        /// </summary>
        /// <param name="cards"></param>
        public void SetSideGroup(int[] cards)
        {
            int cardCount = cards.Length;
            InitCardGroup(cardCount);      //检测和初始化牌的个数

            for (int i = 0; i < cardCount; i++)
            {
                Vector3 targetPos = new Vector3(32*i, 0, 0);    //设置牌的动画移动终点
                _cardGroupArray[i].SetGroup(cards[i], i*2, targetPos);
            }
        }


        void InitCardGroup(int count)
        {
            //如果没有创建过数组,数组创建
            if (_cardGroupArray == null)
            {
                _cardGroupArray = new CardGroup02[count];
                for (int i = 0; i < count; i++)
                {
                    var child = gameObject.AddChild(CardGroupPrefab);
                    var cardGroup = child.GetComponent<CardGroup02>();
                    _cardGroupArray[i] = cardGroup;
                }
                SetCoverCard(count);        //重置盖牌的动画位置
                return;
            }

            //如果数组个数不合适,填补少的个数
            int len = _cardGroupArray.Length;
            if (len >= count) return;
            var tempArray = new CardGroup02[count];
            Array.Copy(_cardGroupArray, tempArray, len);
            _cardGroupArray = tempArray;
            for (int i = len; i < count; i++)
            {
                var child = gameObject.AddChild(CardGroupPrefab);
                var cardGroup = child.GetComponent<CardGroup02>();
                _cardGroupArray[i] = cardGroup;
            }
            SetCoverCard(count);        //重置盖牌的动画位置
        }

        /// <summary>
        /// 设置盖牌的动画位置
        /// </summary>
        /// <param name="count"></param>
        void SetCoverCard(int count)
        {
            CoverCard.depth = count * 2;      //设置盖牌的层级
            var tween = CoverCard.GetComponent<TweenPosition>();

            if (tween == null) return;
            tween.to = new Vector3(64 + (count - 1)*Shift, 0, 0);
        }

        public void BeginMatchCard()
        {
            int len = _cardGroupArray.Length;
            for (int i = 0; i < len; i++)
            {
                _cardGroupArray[i].Play();
            }

            for (int i = 0; i < Tweens.Length; i++)
            {
                var tween = Tweens[i];
                tween.ResetToBeginning();
                tween.PlayForward();
            }
        }
    }
}
