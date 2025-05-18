using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.BaiTuan
{

    public class ResultListCtrl : MonoBehaviour
    {
        public GameObject Result;
        private List<GameObject> _resultObjects = new List<GameObject>();
        public UIGrid Grid;
        public UISprite BgSprite;
        public int ResultListCount = 11;
        public int ResultNum;


        public void AddResultOnFrist(int[] r)
        {
            if (r == null) return;
            int t = 0;
            var temp = Instantiate(Result);
            temp.transform.parent = Grid.transform;
            temp.transform.localScale = Result.transform.localScale;
            temp.GetComponent<SetResultList>().AddResultOnFrist(r);
            temp.SetActive(true);
            _resultObjects.Add(temp);
            CalculateList();
        }

        public void AddResult(int[] r)
        {
            int[] s = { 0, 0, 0 };
            ResultNum = 0;
            
            if (r[0] > r[3])
            {
                s[0] = 1;
            }
            if (r[1] > r[3])
            {
                s[1] = 1;
            }
            if (r[2] > r[3])
            {
                s[2] = 1;
            }
            for (int i = 0; i < s.Length; i++)
            {
                ResultNum += s[i];
            }
            var temp = Instantiate(Result);
            temp.transform.parent = Result.transform.parent;
            temp.transform.localScale = Result.transform.localScale;
            temp.GetComponent<SetResultList>().AddResult(s);
            temp.SetActive(true);
            _resultObjects.Add(temp);
            CalculateList();

        }

        private void CalculateList()
        {
            if (_resultObjects.Count >= ResultListCount)
            {
                var n = _resultObjects.Count - ResultListCount;
                for (var i = 0; i < n; i++)
                {
                    _resultObjects[i].SetActive(false);
                    DestroyObject(_resultObjects[i]);
                    _resultObjects.Remove(_resultObjects[i]);
                }

            }
            Grid.Reposition();
        }
    }
}