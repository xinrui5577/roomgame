using System;
using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhRuleInfoView : MonoBehaviour
    {
        public GameObject View;

        public UIGrid Grid;

        public GameObject Item;

        public List<string> KeyList;

        public List<string> NameList;

        public void OnRecieve(EventData data)
        {
            switch (data.Name)
            {
                case "Show":
                    View.SetActive(true);
                    break;
                case "RuleInfo":
                    SetRuleInfo(data.Data);
                    break;
            }
        }


        public void Hide()
        {
            View.SetActive(false);
        }

        protected void SetRuleInfo(object data)
        {
            Dictionary<string, string> ruleInfo = (Dictionary<string, string>)data;

            foreach (KeyValuePair<string, string> keyValuePair in ruleInfo)
            {
                int index = KeyList.IndexOf(keyValuePair.Key);
                if (index>=0)
                {
                    GameObject obj = Instantiate(Item);
                    obj.SetActive(true);
                    Grid.AddChild(obj.transform);
                    obj.transform.localScale = Vector3.one;
                   
                    JhRuleInfoItem jhItem = obj.GetComponent<JhRuleInfoItem>();
                    string name = NameList[index];
                    string vlaue = keyValuePair.Value;
                    if (name.Contains("_"))
                    {
                        string[] temp = name.Split('_');
                        name = temp[0];
                        if (temp[1].Contains("#"))
                        {
                            string[] vlaues = temp[1].Split('#');
                            for (int j = 0; j < vlaues.Length; j += 2)
                            {
                                if (vlaues[j] == vlaue)
                                {
                                    vlaue = vlaues[j + 1];
                                    break;
                                }
                            }
                        }
                        else if (temp[1].Contains("+"))
                        {
                            string plus = temp[1].Substring(temp[1].IndexOf("+", StringComparison.Ordinal)+1);
                            vlaue = vlaue+plus;
                        }
                    }

                    jhItem.SetInfo(name, vlaue);
                }
            }
        }
    }
}
