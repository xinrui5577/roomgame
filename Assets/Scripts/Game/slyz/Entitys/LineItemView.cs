using System;
using System.Collections;
using Assets.Scripts.Game.slyz.GameScene;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.slyz.Entitys
{
    /// <summary>
    /// 
    /// </summary>
    public class LineItemView : MonoBehaviour
    {
        /// <summary>
        /// 获得奖励时的标记
        /// </summary>
        public GameObject WinSign;
        /// <summary>
        /// 获得label
        /// </summary>
        public YxBaseLabelAdapter GainLabelAdapter;
        /// <summary>
        /// 获得奖励牌子
        /// </summary>
        public WinBrandView WinBrand;
        /// <summary>
        /// 牌
        /// </summary>
        public UISprite[] CardSprite;

        public Action<int> RollbackFinishedAction;
        public void Init()
        {
            SetSignActive(false);
            SetGainLabelActive(false);
            SetWinBrandActive(false);
            RollbackFinishedAction = null;
        }

        public void SetSignActive(bool active)
        {
            if (WinSign == null) return;
            WinSign.SetActive(active);
        }

        public void SetGainLabelActive(bool active)
        {
            if (GainLabelAdapter == null) return;
            GainLabelAdapter.gameObject.SetActive(active);
        }

        public void SetWinBrandActive(bool active)
        {
            if (WinBrand == null) return;
            if (active)
            {
                WinBrand.Show();
            }
            else
            {
                WinBrand.Hide();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public UISprite GetCard(int index)
        {
            return CardSprite[index];
        }

        private Coroutine _playRollback;
        public void PlayRollbackCards(CardTeam cardTeam)
        {
            if (_playRollback != null)
            {
                StopCoroutine(_playRollback);
            }
            _playRollback = StartCoroutine(RollbackCards(cardTeam));
        }

        private IEnumerator RollbackCards(CardTeam cardTeam)
        {
            var rollWait = new WaitForSeconds(0.1f);
            var count = CardSprite.Length-1;
            var cardValues = cardTeam.cards;
            var i = 0;
            for (; i < count; i++)
            {
                var cardValue = cardValues[i];
                var cardValue16 = "0x" + cardValue.ToString("X2");
                var cardSprite = CardSprite[i];
                var cardAni = cardSprite.gameObject.AddComponent<CardAni>();
                cardAni.CardName = cardValue16;
                cardAni.Speed = 5;
                cardAni.Play();
                yield return rollWait;
            }
            var lastCardValue = cardValues[i];
            var lastCardValue16 = "0x" + lastCardValue.ToString("X2");
            var lastCardSprite = CardSprite[i];
            var lastCardAni = lastCardSprite.gameObject.AddComponent<CardAni>();
            lastCardAni.CardName = lastCardValue16;
            lastCardAni.Speed = 5;
            if (RollbackFinishedAction != null)
            {
                lastCardAni.SetCompleteFun(RollbackFinishedAction);
            }
            lastCardAni.Play();
            yield return rollWait;
            if (0 < cardTeam.rate)
            {
                Facade.Instance<MusicManager>().Play("Winning"); 
                WinSign.SetActive(true);
                WinSign.GetComponent<TweenAlpha>().enabled = true;
                SetWinBrandActive(true);
                SetGainLabelActive(true);
                WinBrand.WinLabelAdapter.Text(cardTeam.rate);
                GainLabelAdapter.Text(cardTeam.gold);//.text = YxUtiles.GetShowNumberForm(team.gold);         // 得分 
            }
        }

        public void PlayCardBack()
        {
            CardAni cardAni = null;
            var count = CardSprite.Length-1;
            var i = 0;
            for (; i < count; i++)
            {
                var cardSprite = CardSprite[i];
                if (cardSprite == null) { continue;}
                cardAni = cardSprite.gameObject.AddComponent<CardAni>();
                cardAni.CardName = "BG";
                cardAni.Speed = 5; 
                WinBrand.WinLabelAdapter.Text(0);//.text = "0";
                GainLabelAdapter.Text(0);// mScoreNum1.text = "0";
                cardAni.Play();
            }
            var lastcardSprite = CardSprite[i];
            if (lastcardSprite == null) { return; }
            cardAni = lastcardSprite.gameObject.AddComponent<CardAni>();
            cardAni.CardName = "BG";
            cardAni.Speed = 5;
            WinBrand.WinLabelAdapter.Text(0);//.text = "0";
            GainLabelAdapter.Text(0);// mScoreNum1.text = "0";
            if (RollbackFinishedAction != null)
            {
                cardAni.SetCompleteFun(RollbackFinishedAction);
            }
            cardAni.Play();
        }
    }
}
