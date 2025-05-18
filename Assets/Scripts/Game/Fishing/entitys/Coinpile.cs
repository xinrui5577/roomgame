using System;
using Assets.Scripts.Game.Fishing.datas;
using UnityEngine;
using UnityEngine.UI;
using YxFramwork.Common;
using YxFramwork.Common.Adapters;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 金币堆
    /// </summary>
    public class Coinpile : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public YxBaseLabelAdapter CoinLabel;

        public Slider Slider;
        public Vector3 StartPos;
        public Vector3 EndPos;

        private CanvasGroup _canvasGroup;
        private int _target = 10;
        void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            Slider.minValue = 0;
            Slider.value = 0;
        }

        public void SetCoinData(CoinData coinData)
        {
            var coin = coinData.AddCoin;
            var rate = coin / coinData.Rate;
            _target = rate / 10+1;
            if (CoinLabel!=null) CoinLabel.Text(coin);
        }

        public bool NeedHide { get; set; }

        private float _lastTime;
        private float _curAlpha = 1.2f;
        void Update()
        {
            var deltaTime = Time.deltaTime;
            if (Slider != null)
            {
                if (_lastTime <=1)
                {
                    _lastTime += deltaTime * 5;
                    Slider.value = Mathf.Lerp(0, _target, _lastTime);
                }
            }
            if (_canvasGroup != null)
            {
                if (NeedHide)
                {
                    _curAlpha -= deltaTime * 0.5f;
                    _canvasGroup.alpha = _curAlpha;
                }
            }
        }
    }
}
