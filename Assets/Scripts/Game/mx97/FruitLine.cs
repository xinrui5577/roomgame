using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Game.mx97
{
    //需要注意的线: 0 7 6; 1 8 5; 2 3 4; 2 8 6; 2 1 0; 3 8 7; 4 5 6; 4 8 0

    [System.Serializable]
    public class FruitGroup
    {
        public GameObject[] fruitElement;
        [System.NonSerialized]
        public List<string> finalName = new List<string>();
    }

    public class FruitLine : MonoBehaviour
    {
        public FruitGroup fruitGroups = new FruitGroup();
        public List<FruitGroup> listFruit;

        public List<int> lineValue = new List<int>();

        private static FruitLine instance;
        public static FruitLine getInstance()
        {
            return instance;
        }

        // Use this for initialization
        void Start()
        {
            instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void finalFruit()
        {
            FruitLine.getInstance().lineValue.Clear();

            foreach (var item_listFruit in listFruit)
            {
                item_listFruit.finalName.Clear();
                foreach (var item_Element in item_listFruit.fruitElement)
                {
                    foreach (var sprite in item_Element.GetComponentsInChildren<UISprite>())
                    {
                        if (sprite.transform.localPosition.y == 0)
                        {
                            item_listFruit.finalName.Add(sprite.spriteName);
                        }
                    }
                }
            }

            for (int i = 0; i < listFruit.Count; i++)
            {
                if (listFruit[i].finalName[0] == listFruit[i].finalName[1] && listFruit[i].finalName[0] == listFruit[i].finalName[2])
                {
                    lineValue.Add(1);
                }
                else if (listFruit[i].finalName[0] == "YingTao")
                {
                    lineValue.Add(1);
                }
                else if(listFruit[i].finalName[0].Contains("Bar") && listFruit[i].finalName[1].Contains("Bar") && listFruit[i].finalName[2].Contains("Bar"))
                {
                    lineValue.Add(1);
                }
                else
                {
                    lineValue.Add(0);
                }

            }
        }
    }

}

