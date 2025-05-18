using System.Collections;
using Assets.Scripts.Common.Components;
using Assets.Scripts.Game.Fishing.datas;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Common;
using YxFramwork.Common.Model;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;
using YxFramwork.View;

namespace Assets.Scripts.Game.Fishing.GenerateInfos
{
    public class BossGenerateInfo : GenerateInfo
    {
        public float MaxIntervalTime = 60;

        public override IEnumerator Notice(FishData fishData)
        {
            Facade.Instance<MusicManager>().Play("sound_warning_1");
            var pre = ResourceManager.LoadAsset(string.Format("BossEnterTip_{0}",fishData.FishId));
            if (pre == null) yield break;
            var gMgr = App.GetGameManager<FishingGameManager>();
            var go = GameObjectUtile.Instantiate(pre, gMgr.TipContainer);
            var animator = go.GetComponent<Animator>();
            if (animator==null)yield break;
            var len = animator.GetCurrentAnimatorStateInfo(0).length;
            Destroy(go, len);
        }

        public override IEnumerator Wait()
        {
           var intervalTime = Random.Range(IntervalTime, MaxIntervalTime);
            yield return new WaitForSeconds(intervalTime);
        }
    }
}
