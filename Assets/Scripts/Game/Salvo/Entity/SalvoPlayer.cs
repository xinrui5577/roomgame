using System;
using UnityEngine;
using YxFramwork.Common.Adapters;
using YxFramwork.Framework;

namespace Assets.Scripts.Game.Salvo.Entity
{
    public class SalvoPlayer : YxBaseGamePlayer
    { 
        protected override void SetCoin(long coin)
        {
            SetCoinValue(coin);
        }

        public void SetCoinValue(long coin, float processTime=0, int count = 13, Action<float> onFinish = null, Action<float> onTimes = null)
        {
            Info.CoinA = coin;
            var numAdapter = CoinLabel as YxBaseNumberAdapter;
            if (numAdapter == null) return; 
            numAdapter.SetNumber(coin, processTime, 53, onFinish,onTimes);
        }
    }
}
