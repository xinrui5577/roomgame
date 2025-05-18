using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.jpmj.MahjongScripts.GameUI
{
    public class GameInfoUp : MonoBehaviour
    {

        public GameObject LoopParnet;
        public float LoopTime;
        public float MoveTime;
        protected List<Transform> LoopObj = new List<Transform>(); 
        // Use this for initialization
        void Start ()
        {
            StartLoop();
        }
	    
        protected virtual void StartLoop()
        {
            if (LoopParnet.transform.childCount < 2)
                return;

            foreach (Transform tf in LoopParnet.transform)
            {
                LoopObj.Add(tf);
            }

            StartCoroutine(Loop());
        }

        protected virtual IEnumerator Loop()
        {
            
            while (true)
            {
                yield return new WaitForSeconds(LoopTime);

                yield return LoopAnimation();
            }
        }

        protected virtual IEnumerator LoopAnimation()
        {
            var first = LoopObj[0];
            var firstRt = first.GetComponent<RectTransform>();
            float dis = firstRt.rect.size.y;

            float perDis = dis/MoveTime;

            float val = 0;
            float bTime = Time.time;
            float perTime = bTime;
            float disMove = 0;
            while (val < MoveTime)
            {
                var time = Time.time - perTime;
                perTime = Time.time;
                val = Time.time - bTime;
                float disPerMove = perDis * time;
                if ((disMove + disPerMove) > dis)
                {
                    disPerMove = dis - disMove;
                }
                disMove += disPerMove;
                foreach (Transform chair in LoopParnet.transform)
                {
                    chair.localPosition += new Vector3(0, disPerMove,0);
                }

                yield return 10;
            }
            var last = LoopObj[LoopObj.Count-1];
            first.transform.localPosition = last.localPosition + new Vector3(0, -dis, 0);
            LoopObj.Remove(first);
            LoopObj.Add(first);
        }
    }
}
