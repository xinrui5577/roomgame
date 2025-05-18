using System.Collections.Generic;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.entitys;
using com.yxixia.utile.Utiles;
using UnityEngine;

namespace Assets.Scripts.Game.Fishing.commons
{
    /// <summary>
    /// 金币堆容器
    /// </summary>
    public class CoinPileContaint : MonoBehaviour
    {
        protected readonly Queue<Coinpile> MoveCoinpiles = new Queue<Coinpile>();
        protected readonly Queue<Coinpile> AddCoinpiles = new Queue<Coinpile>();
        public Coinpile PrefCoinpile;
        /// <summary>
        /// 
        /// </summary>
        public int WaitCount = 4;
        public int Distance = 70;

        public float Speed = 1;
        /// <summary>
        /// 等待间隔
        /// </summary>
        public float WaitInterval = 0.5f;

        private CoinPileState _state = CoinPileState.Check;

        /// <summary>
        /// 添加金币堆
        /// </summary>
        /// <param name="coinData"></param>
        public void AddCoinPile(CoinData coinData)
        { 
            var coinpile = CreateCoinPile();
            coinpile.SetCoinData(coinData);
            AddCoinpiles.Enqueue(coinpile);
            if (WaitCount - MoveCoinpiles.Count < AddCoinpiles.Count)
            {
                ChangeSpeedUp();
            }
            else
            {
                ChangeNormalSpeed();
            }
        }

        public Coinpile CreateCoinPile()
        {
            var coinPile = GameObjectUtile.Instantiate(PrefCoinpile,transform);
            coinPile.SetActive(false);
            var ts = coinPile.transform;
            ts.SetParent(transform);
            ts.localPosition = Vector3.zero;
            return coinPile;
        }

        private float _lastTime;
        private float _deltaTime;
        void Update()
        {
           
            _deltaTime = Time.deltaTime;
            switch (_state)
            { 
                case CoinPileState.Check:
                    OnCheck();
                    return;
                case CoinPileState.Move:
                    OnMove();
                    return;
                case CoinPileState.Wait:
                    OnWait();
                    return;
            }
        }

        private void OnCheck()
        {
            var moveCount = MoveCoinpiles.Count;
            if (moveCount > 0)
            {
                var tmp = MoveCoinpiles.Dequeue();
                Destroy(tmp.gameObject);
            }
            var addCount = AddCoinpiles.Count;
            if (addCount > 0)
            {
                //添加
                moveCount = MoveCoinpiles.Count;
                var freeCount = Mathf.Min(addCount,WaitCount - moveCount);
                for (var j = 0; j < freeCount; j++)
                {
                    var coinpile = AddCoinpiles.Dequeue();
                    MoveCoinpiles.Enqueue(coinpile);
                }
            } 
            moveCount = MoveCoinpiles.Count;
            if (moveCount < 1)
            {
                return;
            }
           
            var i = 0;
            var totalDis = (WaitCount-1) * Distance;
            foreach (var moveCoinpile in MoveCoinpiles)
            {
                var ts = moveCoinpile.transform;
                moveCoinpile.SetActive(true);
                var startPos = totalDis - i * Distance;
                var endPos = startPos + Distance;
                var pos = ts.localPosition;
                pos.x = startPos;
                ts.localPosition = pos;
                moveCoinpile.StartPos = pos;
                var endpos = pos;
                endpos.x = endPos;
                moveCoinpile.EndPos = endpos;
                moveCoinpile.NeedHide = i == 0;
                i++;
            } 
            addCount = AddCoinpiles.Count;
            if (addCount > 0)
            {
                //切换速度
                ChangeSpeedUp();
                _state = CoinPileState.Move;
            }
            else
            {
                ChangeNormalSpeed();
                _state = CoinPileState.Wait;
            }
            _lastTime = 0;
        }

        private float _curSpeed;
        private void OnMove()
        {
            _lastTime += _deltaTime * _curSpeed; 
            foreach (var moveCoinpile in MoveCoinpiles)
            {
                var ts = moveCoinpile.transform;
                ts.localPosition = Vector3.Lerp(moveCoinpile.StartPos, moveCoinpile.EndPos, _lastTime);
            }
            if (_lastTime >= 1)
            {
                _state = CoinPileState.Check;
                _lastTime = 0;
            }
        }

        private void OnWait()
        {
            _lastTime += _deltaTime;
            if (_lastTime < WaitInterval) return; 
            _lastTime = _lastTime - WaitInterval;
            _state = CoinPileState.Move;
        }

        private void ChangeNormalSpeed()
        {
            _curSpeed = Speed;
        }

        private void ChangeSpeedUp()
        {
            _curSpeed = Speed * 10;
        }

        enum CoinPileState
        {
            Check,
            Move,
            Wait,
        }
    }
}
