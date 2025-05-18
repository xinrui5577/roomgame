using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.sanpian.item
{
    public class MoveUserHead : MonoBehaviour
    {
        public GameObject[]UserHeads;

        public void MoveHead(List<int> turnIndex,Action callBack)
        {
            for (int i = 0; i < turnIndex.Count; i+=2)
            {
                StartCoroutine(MoveHeadIe(UserHeads[turnIndex[i]-1], UserHeads[turnIndex[i+1]-1],callBack));
            }        
        }


        IEnumerator MoveHeadIe(GameObject from, GameObject to, Action callBack)
        {
            from.SetActive(false);
            List<GameObject> list=new List<GameObject>();
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.005f); 
                GameObject target=Instantiate(from);
                list.Add(target);
                target.gameObject.SetActive(true);
                target.GetComponent<UISprite>();
                target.transform.parent = transform;
                target.transform.localScale=Vector3.one;
                target.transform.position = from.transform.position;
                AnimatedAlpha a=target.AddComponent<AnimatedAlpha>();
                a.alpha = 1.0f - i*0.1f;
                iTween.MoveTo(target,to.transform.position,2f);
            }
            yield return new WaitForSeconds(2f);
            foreach (var o in list)
            {
                Destroy(o);
            }
            from.SetActive(true);
            callBack();
        }
    }
}
