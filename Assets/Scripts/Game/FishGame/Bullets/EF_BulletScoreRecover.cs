using Assets.Scripts.Game.FishGame.Effect;
using Assets.Scripts.Game.FishGame.Language;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.Bullets
{
    public class EF_BulletScoreRecover : MonoBehaviour {
        public GameObject Prefab_GOText;
        public Vector3 LocalPosStart = new Vector3(0.57F, -0.2432756F, -0.01F);
        public Vector3 LocalPosTarget =  new Vector3(0.57F,0.02472025F,-0.01F);
        // Use this for initialization
        public LanguageItem Li_Hint;
        void Awake() {
            //BulletScoreRecover bsr = GetComponent<BulletScoreRecover>();
            BulletScoreRecover.EvtBulletScoreStartRecover += Handle_BulletScoreStartRecover;
        }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(60F);
            BulletScoreRecover.EvtBulletScoreStartRecover -= Handle_BulletScoreStartRecover;
            Destroy(this);
        }
        void Handle_BulletScoreStartRecover(int playerIdx, int score)
        {
            StartCoroutine(_Coro_EffectProcess(playerIdx,score));
        }

        IEnumerator _Coro_EffectProcess(int pidx,int score)
        {
            if(Prefab_GOText == null)
                yield break;
        

            GameObject goText = Instantiate(Prefab_GOText) as GameObject;
            Transform tsText = goText.transform;
        
            Ef_tk2dTextWithShadow textWithShadow = goText.GetComponent<Ef_tk2dTextWithShadow>();
         

            textWithShadow.text = string.Format(Li_Hint.CurrentText,score);
            textWithShadow.Commit();

            tsText.parent = GameMain.Singleton.PlayersBatterys[pidx].transform;
            tsText.localPosition = LocalPosStart;
            tsText.localRotation = Quaternion.identity;
        
            //ÒÆ¶¯
            float timeUse = 0.2F;
            float elapse = 0F;
            while (elapse < timeUse)
            {
                yield return 1;
                tsText.localPosition = Vector3.Slerp(LocalPosStart, LocalPosTarget, elapse / timeUse);
                elapse += Time.deltaTime;
            }
            tsText.localPosition = LocalPosTarget;
            yield return new WaitForSeconds(6F);//¹Ì¶¨ÏÔÊ¾(5s)

            //ÉÁË¸(4s)
            timeUse = 2F;
            elapse = 0F;
            textWithShadow.Alpha = 1F;
            textWithShadow.Commit();

            while (elapse < timeUse)
            {
                yield return 0;
                textWithShadow.Alpha = Mathf.Abs(Mathf.Cos(4F * Mathf.PI * elapse / timeUse));
                textWithShadow.Commit();
                elapse += Time.deltaTime;
            }

            yield return 0;
            //ÉÁË¸ÏûÊ§(1.5s)
            timeUse = 1.5F;
            elapse = 0F;
            textWithShadow.Alpha = 1F;
            textWithShadow.Commit();
            while (elapse < timeUse)
            {
                yield return 0;
                textWithShadow.Alpha = Mathf.Abs(Mathf.Cos(0.5F * Mathf.PI * elapse / timeUse));
                textWithShadow.Commit();

                elapse += Time.deltaTime;
            }

            Destroy(goText);
        }
    }
}
