using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.brtbsone
{
    public class ResultListCtrl : MonoBehaviour
    {
        public GameObject Result;
        private List<GameObject> _resultObjects = new List<GameObject>();
        public UIGrid Grid;
        public int ResultListCount = 11;

        public void AddResult(int[] r)
        {
            if (r == null) return;
            var temp = Instantiate(Result);
            temp.transform.parent = Grid.transform;
            temp.transform.localScale = Result.transform.localScale;
            temp.GetComponent<SetResultList>().AddResult(r);
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

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
