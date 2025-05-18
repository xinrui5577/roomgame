/** 
 *文件名称:     OutPile.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-03-20 
 *描述:         打牌牌堆，继承于麻将堆，只是多了一张打出牌的额外处理
 *历史记录: 
*/

using Assets.Scripts.Game.lyzz2d.Game;
using Assets.Scripts.Game.lyzz2d.Game.Item;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.lyzz2d.Utils.UI
{
    public class OutPile : MahjongPile
    {
        private ThrowOutCard _throw;
        private MahjongItem outCard;

        /// <summary>
        ///     打出牌的x轴倍数
        /// </summary>
        public float OutCardScaleX = 1.5f;

        /// <summary>
        ///     打出牌的y轴倍数
        /// </summary>
        public float OutCardScaleY = 1.5f;

        private TweenPosition pos;
        private TweenScale scale;

        public ThrowOutCard OutCard
        {
            set { _throw = value; }
            get
            {
                if (_throw == null)
                {
                    _throw = GetComponentInChildren<ThrowOutCard>();
                }
                return _throw;
            }
        }

        /// <summary>
        ///     直接打出一张牌
        /// </summary>
        public void ThrowOutCard()
        {
            var lastItem = Layout.GetLastItem().GetComponent<MahjongItem>();
            ThrowOutCard(lastItem, 0);
        }

        public void ThrowOutCard(MahjongItem item, float time)
        {
            _throw.ThrowCard = item;
            item.SelfData.Action = EnumMahJongAction.StandWith;
            item.SelfData.Direction = EnumMahJongDirection.Vertical;
            item.SelfData.ShowDirection = EnumShowDirection.Self;
            GameTools.AddChild(_throw.transform, _throw.ThrowCard.transform, 1, 1, false);
            item.gameObject.SetActive(false);
            item.gameObject.SetActive(true);
            pos = TweenPosition.Begin(item.gameObject, time, _throw.transform.position);
            scale = TweenScale.Begin(item.gameObject, time, new Vector3(OutCardScaleX, OutCardScaleY, 0));
            App.GetGameManager<Lyzz2DGameManager>().OnSomeOneThrowCard += OutCardFinish;
            App.GetGameManager<Lyzz2DGameManager>().OnSomeOneNeedCard += OnThrowOutShow;
            Invoke("OnThrowOutShow", time);
        }

        /// <summary>
        ///     飞到地方啊.....
        /// </summary>
        private void OnThrowOutShow()
        {
            CancelInvoke("OnThrowOutShow");
            if (_throw.ThrowCard != null)
            {
                _throw.ThrowCard.SelfData.Action = EnumMahJongAction.Lie;
                if (pos != null)
                {
                    Destroy(pos);
                }
                if (scale != null)
                {
                    Destroy(scale);
                }
                _throw.ThrowCard.transform.localPosition = Vector3.zero;
                App.GetGameManager<Lyzz2DGameManager>().OnSomeOneNeedCard -= OnThrowOutShow;
            }
        }

        public void ThrowOutCardFinished(float Time)
        {
            if (_throw.ThrowCard != null)
            {
                TweenFinish(Time);
            }
        }

        private void TweenFinish(float waitTime)
        {
            Invoke("OutCardFinish", waitTime);
        }

        private void OutCardFinish()
        {
            CancelInvoke("OutCardFinish");
            if (_throw.ThrowCard != null)
            {
                if (pos != null)
                {
                    Destroy(pos);
                }
                if (scale != null)
                {
                    Destroy(scale);
                }
                var item = _throw.ThrowCard;
                Layout.AddItem(item.transform, true);
                ParseItemToThis(item);
                item.gameObject.SetActive(false);
                item.gameObject.SetActive(true);
                _throw.ThrowCard = null;
                App.GetGameManager<Lyzz2DGameManager>().OnSomeOneThrowCard -= OutCardFinish;
            }
        }

        public MahjongItem GetLastCard()
        {
            if (_throw.ThrowCard != null)
            {
                return _throw.ThrowCard;
            }
            return Layout.GetLastItem().GetComponent<MahjongItem>();
        }

        public override void ResetPile()
        {
            base.ResetPile();
            if (OutCard.ThrowCard != null)
            {
                OutCard.ThrowCard = null;
            }
            var lenth = OutCard.transform.childCount;
            while (lenth > 0)
            {
                Destroy(OutCard.transform.GetChild(0).gameObject);
                lenth--;
            }
        }
    }
}