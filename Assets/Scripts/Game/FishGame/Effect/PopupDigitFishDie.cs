using Assets.Scripts.Game.FishGame.Common.Utils;
using Assets.Scripts.Game.FishGame.Common.core;
using Assets.Scripts.Game.FishGame.Common.external.NemoPoolGOs;
using Assets.Scripts.Game.FishGame.Fishs;
using UnityEngine;
using System.Collections;
using YxFramwork.Common;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.FishGame.Effect
{
    public class PopupDigitFishDie : MonoBehaviour {
        public tk2dTextMesh Prefab_Digit;
        public float Duration;
        public AnimationCurve Curve_OffsetY;
        public AnimationCurve Curve_Alpha;
        [System.Serializable]
        public class ScaleData
        {
            public Fish Fish_;
            public float Scale = 1F;
        }
      
        void Start()
        {
            var gdata = App.GetGameData<FishGameData>();
            gdata.EvtFishKilled += Handle_FishKilled; 
        }

        void Handle_FishKilled(Player killer, int bulletScore, int fishOddBonus, int bulletOddsMulti, Fish fish, int reward)
        {
            if (fish.HittableType == HittableType.AreaBomb) return;
            Popup(fishOddBonus * bulletScore, fish.transform.position, killer.transform, fish.Odds);
        }

        public void Popup(int num, Vector3 worldPos, Transform tsParent, int odds)
        {
            if (num == 0)
                return;
            StartCoroutine(_Coro_Popup(num, worldPos, tsParent, odds));
        }

        IEnumerator _Coro_Popup(int num, Vector3 worldPos, Transform tsParent, int odds)
        {
            //tk2dTextMesh digit = Instantiate(Prefab_Digit) as tk2dTextMesh;
            var digit = Pool_GameObj.GetObj(Prefab_Digit.gameObject).GetComponent<tk2dTextMesh>();
            digit.gameObject.SetActive(true);
            digit.text = YxUtiles.GetShowNumberToString(num);
            digit.Commit();

            var ts = digit.transform;
            ts.parent = tsParent;
            ts.localRotation = Quaternion.identity;

            var scale = Mathf.Min(0.5f + odds / 5f * 0.3f, 1.5f); 
            ts.localScale = new Vector3(scale, scale, 1F);

            var oriPos = worldPos; 
            oriPos.z = Defines.GlobleDepth_DieFishPopDigit;
            ts.position = oriPos;
             
            var elapse = 0F;
            var c = Prefab_Digit.color;
         
            while (elapse < Duration)
            {
                yield return 0;
                var prct = elapse / Duration;
                ts.position = oriPos + new Vector3(0F, Curve_OffsetY.Evaluate(prct), 0F);
                digit.Commit();
                elapse += Time.deltaTime;
                
            }

            yield return new WaitForSeconds(2);
            elapse = 0;
            while (elapse < Duration)
            {
                var prct = elapse / Duration;
                c.a = Curve_Alpha.Evaluate(prct);
                digit.color = c;
                digit.Commit();
                elapse += Time.deltaTime;
                yield return 0;
            } 
            //Destroy(digit.gameObject);
            digit.gameObject.SetActive(false);
            Pool_GameObj.RecycleGO(Prefab_Digit.gameObject, digit.gameObject); 
        } 
    }
}
