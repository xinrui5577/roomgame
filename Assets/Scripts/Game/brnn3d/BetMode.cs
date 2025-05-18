using UnityEngine;
using DG.Tweening;
using YxFramwork.Common;

namespace Assets.Scripts.Game.brnn3d
{
    public class BetMode : MonoBehaviour
    {
        public Transform[] CoinDemos = new Transform[15];
        public Transform[] EastCoins = new Transform[15];
        public Transform[] SouthCoins = new Transform[15];
        public Transform[] WestCoins = new Transform[15];
        public Transform[] NorthCoins = new Transform[15];

        public Transform[] CoinWeiZhis = new Transform[4];
        public Transform CoinFirstWeiZhi;
        public Transform[] OtherCoinFirstWeiZhi = new Transform[4];
         
        /// <summary>
        /// 实例化筹码
        /// </summary>
        /// <param name="coinType"></param>
        /// <param name="iArea"></param>
        /// <param name="byStation"></param>
        public void InstanceCoinDemo(int coinType, int iArea, int byStation)
        {

            Transform areaTf = null;
            switch (iArea)
            {
                case 0:
                    areaTf = EastCoins[coinType];
                    break;
                case 1:
                    areaTf = SouthCoins[coinType];
                    break;
                case 2:
                    areaTf = WestCoins[coinType];
                    break;
                case 3:
                    areaTf = NorthCoins[coinType];
                    break;
            }

            var obj = Instantiate(CoinDemos[coinType]);
            obj.gameObject.SetActive(true);
            obj.parent = CoinFirstWeiZhi.parent;
            obj.localEulerAngles = new Vector3(0, 0, 0);
            obj.localPosition = byStation == App.GameData.SelfSeat ? CoinFirstWeiZhi.localPosition : OtherCoinFirstWeiZhi[Random.Range(0, 4)].localPosition;
            var tw = obj.DOLocalMove(CoinWeiZhis[iArea].localPosition, 0.5f);
            tw.OnComplete(delegate
                {
                    if (areaTf != null)
                    {
                        obj.parent = areaTf.parent;
                        var tww = obj.DOLocalMove(areaTf.localPosition, 0.3f);
                        tww.OnComplete(delegate
                        {
                            obj.localPosition = areaTf.localPosition + new Vector3(Random.Range(-3, 4) / (1.0f * 10), 0f, Random.Range(-2, 2) / (1.0f * 10));
                        });
                    }
                });
        }

        //删除金币的列表
        public void DeletCoinList()
        {
            DeleteCoinItemListFromParent(EastCoins[0].parent);
            DeleteCoinItemListFromParent(SouthCoins[0].parent);
            DeleteCoinItemListFromParent(WestCoins[0].parent);
            DeleteCoinItemListFromParent(NorthCoins[0].parent);
        }

        void DeleteCoinItemListFromParent(Transform parent)
        {
            foreach (Transform tf in parent)
            {
                if (tf.name.Contains("Coin0") || tf.name.Contains("Coin1") || tf.name.Contains("Coin2") || tf.name.Contains("Coin3") || tf.name.Contains("Coin4") || tf.name.Contains("Coin5") || tf.name.Contains("Plane")||
                    tf.name.Contains("Coin6") || tf.name.Contains("Coin7") || tf.name.Contains("Coin8") || tf.name.Contains("Coin9") || tf.name.Contains("Coin10") || tf.name.Contains("Coin11")||tf.name.Contains("Coin12") || tf.name.Contains("Coin13") || tf.name.Contains("Coin14"))
                    continue;
                Destroy(tf.gameObject);
            }
        }

    }
}

