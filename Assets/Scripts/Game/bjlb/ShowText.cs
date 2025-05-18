using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.bjlb
{
    public class ShowText : MonoBehaviour
    {

        public GameObject TextGameObject;
        private List<GameObject> _labeList = new List<GameObject>();
        public UIGrid Grid;

        public void AddText(string str)
        {
            GameObject temp = Instantiate(Grid.gameObject, TextGameObject);
            temp.GetComponent<UILabel>().text = str;
            Grid.Reposition();
            _labeList.Add(temp);
            if (_labeList.Count == 1)
            {
                InvokeRepeating("CycleLabel",0,0.01f);
            }
        }

        private int _p = -50;
        private void CycleLabel()
        {
            int lastP = _labeList.Count*50;
            if (_p >= lastP)
            {
                _p = -50;
                foreach (GameObject temp in _labeList)
                {
                    temp.SetActive(false);
                    DestroyObject(temp);
                }
                _labeList.Clear();
                CancelInvoke("CycleLabel");
                Grid.gameObject.transform.localPosition = new Vector3(-120,_p,0);
                return;
            }
            if (_p %50 == 0 )
            {
                CancelInvoke("CycleLabel");
                if (_p == -50)
                {
                    InvokeRepeating("CycleLabel", 0, 0.01f);
                }
                else
                {
                    InvokeRepeating("CycleLabel", 3, 0.01f);
                }
            
            }
            _p ++;
            Grid.gameObject.transform.localPosition = new Vector3(-120,_p,0);
        }

        private GameObject Instantiate(GameObject go, GameObject cloned)
        {
            GameObject temp = (GameObject)Instantiate(cloned);
            temp.transform.position = cloned.transform.position;
            temp.transform.parent = go.transform;
            temp.transform.localScale = go.transform.localScale;
            temp.SetActive(true);
            return temp;
        }
    }
}
