using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.script
{
    public class little_light1 : MonoBehaviour {
	
        float startTime;
        float Alpha;
        float RotationZ = 0;
        public float delayTime=0.6f;
        // Use this for initialization
        void Start ()
        {
            startTime = Time.time+delayTime;
            StartCoroutine(show_and_disappear(0.4f,0.1f,0.4f));
        }
        IEnumerator show_and_disappear(float showTime,float RottationTime,float disTime)
        {
            float r = gameObject.GetComponent<tk2dSprite>().color.r;
            float g = gameObject.GetComponent<tk2dSprite>().color.g;
            float b = gameObject.GetComponent<tk2dSprite>().color.b;
            while(startTime>Time.time)
            {
                yield return 0;
            }
            tk2dSprite tk = gameObject.GetComponent<tk2dSprite>();
            while(startTime+showTime>Time.time)
            {
                tk.color=new Color(r,g,b,Alpha);
                Alpha+= 1/(showTime/Time.deltaTime);
                yield return 0;
            }
            while(startTime+showTime+RottationTime>Time.time)
            {
                gameObject.transform.localRotation = Quaternion.Euler(0,0,RotationZ);
                RotationZ -=25.0f/(RottationTime/Time.deltaTime);
                yield return 0;
            }
            while(startTime+showTime+RottationTime+disTime>Time.time)
            {
                tk.color = new Color(r, g, b, Alpha);
                Alpha-=1/(disTime/Time.deltaTime);
                yield return 0;
            }
        }
        // Update is called once per frame
        void Update ()
        {
		
        }
    }
}
