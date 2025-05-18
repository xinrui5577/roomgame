using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using UnityEngine;
using System.Collections;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class FlyingCoinManager : MonoBehaviour
    {

        public FlyingCoin Prefab_Gold;
        public FlyingCoin Prefab_GoldBig;
        public float AppearOffsetX = 0.1F;
        public float AppearOffsetY = 0.01F;
        public float FlySpeed = 5F;
        public float AppearInterval = 0.05F;
        public AnimationCurve Curve_Scale;
        public bool PlaySound = true;
        public tk2dTextMesh CoinNumPerfab;
        public float HMoveSpeed = 55;
        public float WaitTime = 0.35f;
        public int MoveCount = 5;
        private Transform _coinsContainer;
        /// <summary>
        /// 飞金币
        /// </summary>
        /// <param name="fishDieWorldPos">死亡世界位置</param>
        /// <param name="num">金币个数(Max:11)</param>
        /// <param name="delay">延时</param>
        public void FlyFrom(Vector3 fishDieWorldPos, int num, float delay)
        {
            StartCoroutine(_Coro_FlytProcess(fishDieWorldPos, num, delay));
        }

        private Transform GetCoinsContainer()
        {
            if (_coinsContainer == null)
            {
                var go = new GameObject("CoinsContainer");
                _coinsContainer = go.transform;
                _coinsContainer.parent = transform;
                _coinsContainer.localPosition = Vector3.zero;
                _coinsContainer.localRotation = Quaternion.identity;
            }
            return _coinsContainer;
        }

        private bool _nextStop;
        private int coinSpace;

        private IEnumerator _Coro_FlytProcess(Vector3 fishDieWorldPos, int num, float delay)
        {
            int numFly;
            var typeCoin = FlyingCoinType.Sliver;
            if (num < 10)
            {
                numFly = num;
            }
            else
            {
                numFly = num/5 + (num%5 != 0 ? 1 : 0);
                if (numFly > 11) numFly = 11; //最大飞11颗 
                typeCoin = FlyingCoinType.Glod;
            }
            yield return new WaitForSeconds(delay);
            //播放声音
            if (PlaySound && GameMain.Singleton != null)
            {
                var musicMgr = Facade.Instance<MusicManager>();
                if (typeCoin == FlyingCoinType.Sliver)
                {
                    musicMgr.Play("getsilver");
                }
                if (typeCoin == FlyingCoinType.Glod)
                {
                    musicMgr.Play("getgold");
                }
            }

            var fishDielocalPos = transform.InverseTransformPoint(fishDieWorldPos);
            fishDielocalPos.z = 0F;
            //var useGoldPrefab = t==FlyingCoinType.Glod ? Prefab_GoldBig : Prefab_Gold;

            FlyingCoin useGoldPrefab;
            float offy;
            if (typeCoin == FlyingCoinType.Glod)
            {
                useGoldPrefab = Prefab_GoldBig;
                offy = 12;
            }
            else
            {
                useGoldPrefab = Prefab_Gold;
                offy = 6.5f;
            }

            //生成币
            var golds = new FlyingCoin[numFly];
            var startLocalPos = fishDielocalPos;
                // -new Vector3(AppearOffsetX * (num - 1) * 0.5F, AppearOffsetY * (num - 1) * 0.5F, 0F);
            //startLocalPos += startLocalPos.normalized * 38.4F;//*1.3F,为延长距离,使得金币更像是鱼中飞出
            var maxTime = 0F;
            var con = new GameObject("coins");
            var ts = con.transform;
            if (coinSpace++ > 50) coinSpace = 0;
            ts.parent = GetCoinsContainer();
            ts.localPosition = new Vector3(0, 0, coinSpace*0.0001f);
            ts.localRotation = Quaternion.identity;
            var target = Vector3.zero;
            for (var i = 0; i < numFly; ++i)
            {
                var gold = Pool_GameObj.GetObj(useGoldPrefab.gameObject).GetComponent<FlyingCoin>();
                golds[i] = gold;
                gold.FlySpeed = FlySpeed;
                var goldTs = gold.transform;
                goldTs.parent = ts;
                goldTs.localRotation = Quaternion.identity;
                var offPos = new Vector3(AppearOffsetX*i, AppearOffsetY*i, 0F);
                offPos = fishDielocalPos.x < transform.localRotation.x ? -offPos : offPos;
                goldTs.localPosition = startLocalPos + offPos;
                var flyNeedTime = gold.FlytoPosZero(Curve_Scale, target);
                target.y += offy;
                if (flyNeedTime > maxTime)
                    maxTime = flyNeedTime;
                yield return new WaitForSeconds(AppearInterval);
            }

            yield return new WaitForSeconds(maxTime - AppearInterval*numFly);
            var coinNum = Pool_GameObj.GetObj(CoinNumPerfab.gameObject).GetComponent<tk2dTextMesh>();

            var coinNumTs = coinNum.transform;
            coinNum.text = num.ToString();
            coinNumTs.parent = ts;
            coinNumTs.localRotation = Quaternion.identity;
            coinNumTs.localPosition = new Vector3(0, target.y, 10);

            while (_nextStop)
            {
                yield return new WaitForSeconds(WaitTime);
            }
            _nextStop = true;
            yield return new WaitForSeconds(WaitTime);
            _nextStop = false;
            var times = 1;
            var pos = ts.localPosition;
            while (times < MoveCount)
            {
                ts.localPosition = pos;
                pos.x -= HMoveSpeed;
                times++;
                yield return new WaitForSeconds(WaitTime);
            }

            //消失
            for (var i = 0; i != numFly; ++i)
            {
                Pool_GameObj.RecycleGO(null, golds[i].gameObject);
                //Destroy(golds[i].gameObject);
            }
            Destroy(con);
        }
          
        private void OnDisable()
        {
            if (_coinsContainer!=null)Destroy(_coinsContainer.gameObject);
        }

//        void OnGUI()
//        {
//            if (GUILayout.Button("Fly"))
//            {
//                FlyFrom(new Vector3(-200F, 200F, 0),100, 1);
//            }
//        }
    }
}
