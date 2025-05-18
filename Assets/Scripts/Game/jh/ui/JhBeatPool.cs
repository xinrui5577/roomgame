using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.jh.ui
{
    public class JhBeatPool : MonoBehaviour
    {
        public GameObject ChipPrefab;

        public UIWidget FanWei;

        protected int deep = 1;

        protected List<GameObject> ChipList = new List<GameObject>(); 
        public void Beat(GameObject startPos, int index, int value)
        {
            GameObject chip = Instantiate(ChipPrefab);
            chip.transform.parent = transform;
            chip.transform.localPosition = startPos.transform.localPosition;
            chip.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
             foreach(UIWidget wg in chip.GetComponentsInChildren<UIWidget>()){
                wg.depth = deep++;
            }

            TweenPosition twPos = chip.GetComponent<TweenPosition>();
            twPos.@from = chip.transform.localPosition;
            twPos.to = GetToPos();
            twPos.PlayForward();
            
            JhChip c = chip.GetComponent<JhChip>();
            c.SetChip(index,value);

            ChipList.Add(chip);
        }

        public void Beat(int index, int value)
        {
            GameObject chip = Instantiate(ChipPrefab);
            chip.transform.parent = transform;
            chip.transform.localPosition = GetToPos();
            chip.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            foreach (UIWidget wg in chip.GetComponentsInChildren<UIWidget>())
            {
                wg.depth = deep++;
            }

            JhChip c = chip.GetComponent<JhChip>();
            c.SetChip(index, value);

            ChipList.Add(chip);
        }

        public void Reset()
        {
            foreach (var c in ChipList)
            {
                Destroy(c);
            }

            ChipList = new List<GameObject>();
            deep = 1;
        }

        public void ResultMoveChips(GameObject toObj,EventDelegate callBack)
        {

            GameObject last = ChipList[ChipList.Count - 1];
            foreach (var chip in ChipList)
            {
                TweenPosition twPos = chip.GetComponent<TweenPosition>();
                twPos.from = chip.transform.localPosition;
                twPos.to = toObj.transform.localPosition;
                twPos.ResetToBeginning();
                twPos.PlayForward();
                var chip1 = chip;
                twPos.onFinished.Add(new EventDelegate(() =>
                {
                    chip1.SetActive(false);
                    if (chip1 == last)
                    {
                        callBack.Execute();
                    }
                }));
            }
        }

        protected Vector2 GetToPos()
        {
            Vector2 size = FanWei.localSize;
            Vector2 pos = FanWei.GetComponent<Transform>().localPosition;
            float x = Random.Range(-size.x / 2, size.x / 2);
            float y = Random.Range(-size.y / 2, size.y / 2);
            x += pos.x;
            y += pos.y;
            return  new Vector2(x,y);
        }
    }
}
