using System;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.entitys;
using Assets.Scripts.Game.Fishing.enums;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Game.Fishing.Factorys
{
    /// <summary>
    /// 金币生产者
    /// </summary>
    public class CoinFactory : MonoBehaviour
    {
        public Coin PrefabCoin;

        public FlyCoinLabel PrefabText;

        void Awake()
        {
            Facade.EventCenter.AddEventListeners<EFishingEventType, CoinData>(EFishingEventType.FlyCoinFinished, OnFlyCoinFinished);
        }

//
//        public FishingPlayer Player;
//        [ContextMenu("Test")]
//        public void Test()
//        {
//            Player.CoinPileContaint.AddCoinPile(100);
//            Player.CoinPileContaint.AddCoinPile(25);
//            Player.CoinPileContaint.AddCoinPile(100,10);
//            Player.CoinPileContaint.AddCoinPile(33);
//            Player.CoinPileContaint.AddCoinPile(44);
//            Player.CoinPileContaint.AddCoinPile(12);
//        }

        private void OnFlyCoinFinished(CoinData coinData)
        {
            var player = coinData.Player;
            if (player == null) return;
            //显示金币堆
            player.CoinPileContaint.AddCoinPile(coinData);
            //刷新
            player.UpdateView();
        }

        /// <summary>
        /// 创建飞币
        /// </summary>
        public void CreateFlyCoin(CoinData coinData,FishingPlayer player,Vector3 startPos,int count=3)
        {
            //创建文字
            var label = CreateCoinLabel(player, startPos);
            label.UpdateView(coinData.AddCoin);
            //创建金币
            var endPos = player.CoinContaint.transform.position;
            for (var i = 0; i < count; i++)
            {
                var coin = CreateCoin(player,startPos);
                coin.SetData(coinData);
                coin.Set(endPos,i ==0);
                coin.SetActive(true);
            }
        }

        /// <summary>
        /// 创建金币
        /// </summary>
        /// <param name="player"></param>
        /// <param name="startPos"></param>
        /// <returns></returns>
        public Coin CreateCoin(FishingPlayer player, Vector3 startPos)
        {
            var coin = Instantiate(PrefabCoin);
            var ts = coin.transform;
            ts.SetParent(player.CoinContaint);
            Random.InitState((int)DateTime.Now.Ticks);
            var r = Random.Range(-100, 100);
            startPos.x += r;
            startPos.y += r;
            ts.position = startPos;
            ts.localScale = Vector3.one;
            ts.rotation = Quaternion.identity;
            return coin;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public FlyCoinLabel CreateCoinLabel(FishingPlayer player, Vector3 startPos)
        {
            var coinLabel = Instantiate(PrefabText);
            var ts = coinLabel.transform;
            ts.SetParent(player.CoinContaint);
            ts.position = startPos;
            ts.localScale = Vector3.one;
            ts.rotation = Quaternion.identity;
            coinLabel.SetActive(true);
            return coinLabel;
        }
    }
}
