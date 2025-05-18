using Assets.Scripts.Game.Salvo.Utiles;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.Salvo.View
{
    public class PokerTypeView : YxView
    {
        /// <summary>
        /// 牌型
        /// </summary>
        public GameObject[] PokerTypes;
        /// <summary>
        /// 赔率类型
        /// </summary>
        public YxBaseNumberAdapter[] OddsTypes;

        public int[] Odds;
        public Twinkle Twinkler; 

        private void Awake()
        {
            SetOdds(Odds);
        }

        public void SetOdds(int[] odds)
        { 
            var len = Mathf.Min(odds.Length,OddsTypes.Length);
            for (var i = 0; i < len;i++ )
            {
                OddsTypes[i].SetNumber(odds[i]);
            }
        }

        public void SetBet(int bet)
        {
            if (bet <= 0) return;
            var len = OddsTypes.Length;
            for (var i = 0; i < len; i++)
            {
                OddsTypes[i].SetNumber(Odds[i] * bet); 
            }
        }

        public void Twinkle(int index)
        {
            if (index < 0 || index >= PokerTypes.Length) return;
            Twinkler.Play(PokerTypes[index]);
        }

        public void StopTwinkle()
        {  
            Twinkler.Stop();
        }

        public void SetOdd(int index,long number)
        {
            var odd = OddsTypes[index];
            odd.SetNumber(number, 1.5f, 53);
        }
    }
}
