using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.script
{
    public class coin_script : MonoBehaviour {
        float startTime;
        float delayTime=0.5f;
	
        float ScaleX = 0;
        float ScaleY = 0;
	
        // Use this for initialization
        void Start ()
        {
            startTime = Time.time+delayTime;
            StartCoroutine(big_small(0.5f,0.1f));
        }
        IEnumerator big_small(float bigTime,float smallTime)
        {
            Transform tr = gameObject.GetComponent<tk2dSprite>().transform;
            while(startTime>Time.time)
            {
                yield return 0;
            }
            while(bigTime+startTime>Time.time)
            {
                tr.localScale = new Vector3(ScaleX, ScaleY, 0);
                ScaleX += 1.2f / (bigTime / Time.deltaTime);
                ScaleY += 1.2f / (bigTime / Time.deltaTime);
                yield return 0;
            }
            tr.localScale = new Vector3(1.2f, 1.2f, 0);
            while(bigTime+smallTime+startTime>Time.time)
            {
                tr.localScale = new Vector3(ScaleX, ScaleY, 0);
                ScaleX -= 0.2f / (bigTime / Time.deltaTime);
                ScaleY -= 0.2f / (bigTime / Time.deltaTime);
                yield return 0;
            }
            tr.localScale = new Vector3(1, 1, 0);
        }
        // Update is called once per frame
        void Update () {
	
        }
    }
}
