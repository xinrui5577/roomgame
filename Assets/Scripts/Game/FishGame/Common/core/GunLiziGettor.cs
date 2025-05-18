using UnityEngine;
using System.Collections;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.FishGame.Common.core
{
    public class GunLiziGettor : MonoBehaviour {
        public Transform Preafab_TsLiziKa; 

        public float TimeGunLizi = 20F;
        public AnimationCurve Curve_PopUpY;

        private bool UsingGunLizi = false;
        public void GetLiziKaFrom(Vector3 worldPos)
        {
            if (UsingGunLizi)
                return;
            UsingGunLizi = true;
            StartCoroutine(_Coro_GettingLiziKa(worldPos));
        }

        IEnumerator _Coro_GettingLiziKa(Vector3 worldPos)
        {
            //初始化对象
            Transform tsLiziKa = Instantiate(Preafab_TsLiziKa) as Transform;
            tsLiziKa.parent = transform;
            worldPos.z = Defines.GlobleDepth_LiziKa;
            tsLiziKa.position = worldPos;
            tsLiziKa.localRotation = Quaternion.identity;
            //tsLiziKa.localPosition = new Vector3(tsLiziKa.localPosition.x, tsLiziKa.localPosition.y, 0F);
            var main = GameMain.Singleton;
            //播放音效
            Facade.Instance<MusicManager>().Play("Ioncannoncard");
            //弹起动画
            float useTime = 0.5F;
            float elapse = 0F;
            Vector3 oriLocalPos = tsLiziKa.localPosition;
            while (elapse < useTime)
            {
                tsLiziKa.localPosition = new Vector3(oriLocalPos.x, oriLocalPos.y + Curve_PopUpY.Evaluate(elapse / useTime), oriLocalPos.z);
            
                elapse += Time.deltaTime;
                yield return 0;
            }
            tsLiziKa.localPosition = oriLocalPos;
            yield return new WaitForSeconds(0.5F);
            //飞向炮台

            Vector3 flyDirect = -tsLiziKa.localPosition.normalized;
            float flyDistance = tsLiziKa.localPosition.magnitude;
            float flySpeed = 652.8F;
            useTime = flyDistance / flySpeed;
            elapse = 0F;
        
            while ( elapse < useTime)
            {
                tsLiziKa.localPosition += flyDirect * (flySpeed * Time.deltaTime);
                tsLiziKa.localScale = new Vector3(1F - 0.2F * elapse / useTime, 1F - 0.2F * elapse / useTime, 1F);
                elapse += Time.deltaTime;
                yield return 0;
            }

            //消失

            //换枪
            var p = GetComponent<Player>();
            if (p != null)
            {
                var score = GameMain.Singleton.BSSetting.Dat_PlayersGunScore[p.Idx].Val;
                var style = main.PlayersBatterys.GunNeedScoreToLevelType(score);
                p.ChangeGun(style, GunPowerType.Lizi);
            }
              
            Destroy(tsLiziKa.gameObject);
            //持续时间
            yield return new WaitForSeconds(TimeGunLizi);
            if (p != null)
            {
                var score = GameMain.Singleton.BSSetting.Dat_PlayersGunScore[p.Idx].Val;
                var style = main.PlayersBatterys.GunNeedScoreToLevelType(score);
                p.ChangeGun(style, GunPowerType.Normal);
            }
            //Destroy(tsLiziKa.gameObject);
            UsingGunLizi = false;
        
        }
         
 
    }
}
