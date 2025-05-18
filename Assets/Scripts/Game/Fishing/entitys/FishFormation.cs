using System.Collections;
using Assets.Scripts.Game.Fishing.datas;
using Assets.Scripts.Game.Fishing.enums;
using Assets.Scripts.Game.Fishing.Factorys;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Fishing.entitys
{
    public class FishFormation : MonoBehaviour
    {
        /// <summary>
        /// 持续时间
        /// </summary>
        public float Duration;
        /// <summary>
        /// 鱼阵数据
        /// </summary>
        public FishFormationInfo[] FormationInfos;

        /// <summary>
        /// 撒鱼
        /// </summary>
        public float SprinkleFish()
        {
            var gmgr = App.GetGameManager<FishingGameManager>();
            _fishFactory = gmgr.TheFishFactory;
            var fishPond = gmgr.TheFishPond;
            var rect = fishPond.BoundRect;
            foreach (var finfo in FormationInfos)
            {
                var pos = Vector3.zero;
                var dir = finfo.Direction;
                switch (dir)
                {
                    case EDirection.Top:
                        pos.y = rect.yMax;
                        break;
                    case EDirection.Right:
                        pos.x = rect.xMax;
                        break;
                    case EDirection.Bottom:
                        pos.y = rect.yMin;
                        break;
                    case EDirection.Left:
                        pos.x = rect.xMin;
                        break;
                }
                finfo.transform.localPosition = pos;
                StartCoroutine(SprinkleFishing(finfo));
            }
            return Duration;
        }

        private FishFactory _fishFactory;
        private IEnumerator SprinkleFishing(FishFormationInfo finfo)
        {
            var wait = new WaitForSeconds(finfo.Spacing);
            var i = 0;
            while (finfo.Times-- >0)
            {
                finfo.SprinkleOnce(i, _fishFactory);
                i++;
                yield return wait;
            }
            OnFinished();
        }

        private int _curFinishedTotal;
        private void OnFinished()
        {
            _curFinishedTotal++;
            if (_curFinishedTotal < FormationInfos.Length)
            {
                return;
            }
            Destroy(gameObject);
        }

        void OnDestroy()
        {
            StopAllCoroutines();
        }
    }
}
