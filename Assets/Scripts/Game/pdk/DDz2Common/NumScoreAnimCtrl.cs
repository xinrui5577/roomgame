using System.Collections;
using System.Globalization;
using UnityEngine;
using YxFramwork.GDGeek.Tweener;

namespace Assets.Scripts.Game.pdk.DDz2Common
{
    public class NumScoreAnimCtrl : MonoBehaviour
    {

        public GameObject[] ScoreNum;
        [SerializeField]
        protected float AplheaDurition1 = 1f;
        [SerializeField]
        protected float AplheaDurition2 = 1f;
        [SerializeField]
        protected float FadeLaterTime = 1f;

        private void SetNum(int num)
        {
            var simple = "+";
            if (num < 0) simple = "-";
            ScoreNum[0].GetComponent<UISprite>().spriteName = simple;
            ScoreNum[0].SetActive(true);
            var scoleLen = ScoreNum.Length;
            for (int i = 1; i < scoleLen; i++)
            {
                ScoreNum[i].SetActive(false);
            }
            var absNum = Mathf.Abs(num);



            var numCharArray = absNum.ToString(CultureInfo.InvariantCulture).ToCharArray();
            var numCharArrayLen = numCharArray.Length;
            if (numCharArrayLen > scoleLen - 1)
            {
                Debug.LogError("现在数字数组长度为" + (ScoreNum.Length - 1) + "不能显示更多位数");
                return;
            }

            int j = 1;
            for (int i = 0; i < numCharArrayLen; i++)
            {
                ScoreNum[j].GetComponent<UISprite>().spriteName = simple + numCharArray[i];
                ScoreNum[j].SetActive(true);
                j++;
            }
        }

        private void Onfinish()
        {
            foreach (var gob in ScoreNum)
            {
                if (!gob.activeSelf) continue;
                var twpos = gob.GetComponent<TweenLocalPosition>();
                twpos.StopAllCoroutines();
                twpos.Reset();
                twpos.Play(true);
                var twalpha = gob.GetComponent<TweenAlpha>();
                twalpha.StopAllCoroutines();
                twalpha.ResetToBeginning();
                twalpha.duration = AplheaDurition2;
                twalpha.from = 1;
                twalpha.to = 0;
                twalpha.PlayForward();
                
            }


        }

        public void ShowScoreNum(int num)
        {
            if (ScoreNum[0].GetComponent<TweenLocalPosition>().isActiveAndEnabled)
            {
                StartCoroutine(StartLater(num));
                return;
            }
            SetNum(num);
            foreach (var gob in ScoreNum)
            {
                if(!gob.activeSelf)continue;
                var twpos = gob.GetComponent<TweenLocalPosition>();
                twpos.StopAllCoroutines();
                twpos.Reset();
                var twalpha = gob.GetComponent<TweenAlpha>();
                twalpha.StopAllCoroutines();
                twalpha.ResetToBeginning();
                twalpha.duration = AplheaDurition1;
                twalpha.from = 0;
                twalpha.to = 1;
                twalpha.PlayForward();
            }

            StopCoroutine("FadeLater");
            StartCoroutine(FadeLater());
        }

        private IEnumerator StartLater(int num)
        {

            while (ScoreNum[0].GetComponent<TweenLocalPosition>().isActiveAndEnabled)
            {
                yield return new WaitForEndOfFrame();
            }
            ShowScoreNum(num);
        }

        private IEnumerator FadeLater()
        {
            yield return new WaitForSeconds(FadeLaterTime);
            Onfinish();
        }

    }
}
