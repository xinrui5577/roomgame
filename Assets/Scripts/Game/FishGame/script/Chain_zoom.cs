using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.script
{
    public class Chain_zoom : MonoBehaviour 
    {
        float startTime = 0;
        float delayTime = 0;

        float scalY = 0;

        public float fbigTime = 0;
        public float fsmallTime = 0;
        public float fbigS = 0;
        public float fsmallS = 0;

        public Move_UP ref_moveup;
        // Use this for initialization
        void Start ()
        {
        
        }
        public void Play()
        {
            startTime = Time.time + delayTime;
            StartCoroutine(zoom(fbigTime, fsmallTime, fbigS, fsmallS));
        }
        public void rePlay()
        {
            startTime = Time.time + delayTime;
            StartCoroutine(shrink(fsmallTime, fbigTime, fsmallS, fbigS));
        }
        IEnumerator shrink(float bigTime, float smallTime, float bigS, float smallS)
        {
            float x = gameObject.transform.localScale.x;
            float y = gameObject.transform.localScale.y;
            float z = gameObject.transform.localScale.z;
            while (bigTime + startTime > Time.time)
            {
                gameObject.transform.localScale = new Vector3(x, y - scalY, z);
                scalY -= bigS / (bigTime / Time.deltaTime);
                yield return 0;
            }
            gameObject.transform.localScale = new Vector3(x, y - bigS, z);
            while (bigTime + smallTime + startTime > Time.time)
            {
                gameObject.transform.localScale = new Vector3(x, y - scalY, z);
                scalY += smallS / (smallTime / Time.deltaTime);
                yield return 0;
            }
            gameObject.transform.localScale = new Vector3(x, y + bigS - smallS, z);
            scalY = 0;
        }
        IEnumerator zoom(float bigTime, float smallTime, float bigS, float smallS)
        {
            float x = gameObject.transform.localScale.x;
            float y = gameObject.transform.localScale.y;
            float z = gameObject.transform.localScale.z;
            while (bigTime + startTime > Time.time)
            {
                gameObject.transform.localScale = new Vector3(x, y+scalY, z);
                scalY += bigS / (bigTime / Time.deltaTime);
                yield return 0;
            }
            gameObject.transform.localScale = new Vector3(x, y + bigS, z);
            while (bigTime + smallTime + startTime > Time.time)
            {
                gameObject.transform.localScale = new Vector3(x, y+scalY, z);
                scalY -= smallS / (smallTime / Time.deltaTime);
                yield return 0;
            }
            gameObject.transform.localScale = new Vector3(x, y + bigS - smallS, z);
            scalY = 0;
        }
        // Update is called once per frame
        void Update ()
        {
	
        }
    }
}
