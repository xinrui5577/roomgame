using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.bjlb
{
    public class Talk : MonoBehaviour
    {

        public UITable Table;
        public GameObject LabelGameObject;
        public List<GameObject> LabeList = new List<GameObject>();
        public UIInput Input;
        public UIScrollBar ScrollBar;
      
        public void AddText(string str)
        {
            Input.value = Input.value + str;
            Invoke("GetFocus", 0.2f);
        }

        public virtual void AddTalkText(string str, string target)
        {
            if (LabeList.Count >= 50)
            {
                LabeList[0].SetActive(true);
                Destroy(LabeList[0]);
                LabeList.RemoveAt(0);
            }
            GameObject temp = Instantiate(Table.gameObject, LabelGameObject.gameObject);
            temp.GetComponent<UILabel>().text = str;
            temp.name = target;
            LabeList.Add(temp);
            Table.Reposition();
            if(ScrollBar!=null) ScrollBar.value = 1;

        }

        private GameObject Instantiate(GameObject go, GameObject cloned)
        {
            GameObject temp = (GameObject) Instantiate(cloned);
            temp.transform.position = cloned.transform.position;
            temp.transform.parent = go.transform;
            temp.transform.localScale = go.transform.localScale;
            temp.SetActive(true);
            return temp;
        }

        public void SendText()
        {
            if (Input.value != "")
            {
                //SmartManager.UserTalk(Input.value, _targetUser);
            }
            Input.value = "";
            Invoke("GetFocus", 0.2f);
        }

        protected void GetFocus()
        {
            Input.isSelected = true;
        }
    }
}
