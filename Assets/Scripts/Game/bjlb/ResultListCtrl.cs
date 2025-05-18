using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.bjlb
{
    public class ResultListCtrl : MonoBehaviour
    {

        public GameObject Result;
        private List<GameObject> _resultObjects = new List<GameObject>();
        public UIGrid Grid;
        public SpringPosition Sp;
        public GameObject Collider;
        public UISprite BgSprite;

       public void Start()
        {
            InvokeRepeating("Cycle",1,0.3f);
        }
        public void AddResult(bool[] r)
        {
            var temp = Instantiate(Result);
            temp.transform.parent = Result.transform.parent;
            temp.transform.localScale = Result.transform.localScale;
            temp.GetComponent<SetResultList>().AddResult(r);
            temp.SetActive(true);
            _resultObjects.Add(temp);
            if (_resultObjects.Count >= 10)
            {
                _resultObjects[0].SetActive(false);
                DestroyObject(_resultObjects[0]);
                _resultObjects.Remove(_resultObjects[0]);
            }
            Grid.Reposition();
        }

        public void OpenHistory()
        {
            Sp.enabled = false;
            Sp.target = new Vector3(0,140,0);
            Sp.enabled = true;
            Collider.SetActive(true);
        }
        public void CloseHistory()
        {
            Sp.enabled = false;
            Sp.target = new Vector3(0, 600, 0);
            Sp.enabled = true;
            Collider.SetActive(false);
        }

        private bool _cycle;
        protected void Cycle()
        {
            BgSprite.color = _cycle  ? new Color(1, (105f / 255f), 0, (200f / 255f)) : new Color(0, 1, (66f / 255f), (200f / 255f));
            _cycle = !_cycle;
        }
    }
}
