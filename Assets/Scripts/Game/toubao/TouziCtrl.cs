using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.toubao
{
    public class TouziCtrl : MonoBehaviour
    {
        public GameObject TouZiAni;

        public UISprite saizi1;
        public UISprite saizi2;
        public UISprite saizi3;

        public GameObject saiziParent;

        private UISprite[] saiziArr;

        public float rollTime = 1f;

        public ShowBetResult BetResult;

        public HistoryPanel HistoryPanel;

        void Start()
        {
            saiziArr = new[] { saizi1, saizi2, saizi3 };
        }

        public void PlaySaiZiAni(int[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                saiziArr[i].spriteName = "p" + points[i];
                saiziArr[i].MakePixelPerfect();
            }
            saiziParent.SetActive(false);
            TouZiAni.SetActive(true);
            StartCoroutine(saiziAniIe(points));
        }

        IEnumerator saiziAniIe(int[] points)
        {
            yield return new WaitForSeconds(rollTime);
            saiziParent.SetActive(true);
            TouZiAni.SetActive(false);
            HistoryPanel.CreateHistoryItem(points);
            BetResult.ShowResult(points);
        }

        public int[] test;
        public bool testbool;
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (testbool)
                {
                    PlaySaiZiAni(test);
                }
                else
                {
                    PlaySaiZiAni(new int[] { Random.Range(1, 7), Random.Range(1, 7), Random.Range(1, 7) });
                }
            }
        }
    }
}
