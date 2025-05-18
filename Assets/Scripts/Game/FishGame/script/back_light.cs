using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.script
{
    public class back_light : MonoBehaviour
    {
        float startTime;
        public float delayTime=0.5f;
        float ScaleX = 0;
        float ScaleY = 0;
	
        public float big;
        public float rot;
        public float dis;
        float RotationZ = 0;
	
        float a = 1.0f;
        // Use this for initialization
        void Start ()
        {
            startTime = Time.time+delayTime;
            StartCoroutine(big_Rot(big,rot,dis));
        }
	
        IEnumerator big_Rot(float bigTime,float RotTime,float disTime)
        {
            while(startTime>Time.time)
            {
                yield return 0;
            }
            Transform tr=gameObject.GetComponent<tk2dSprite>().transform;
            while(startTime+bigTime>Time.time)
            {
                tr.localScale= new Vector3(ScaleX, ScaleY, 0);
                ScaleX += 1.5f / (bigTime / Time.deltaTime);
                ScaleY += 1.5f / (bigTime / Time.deltaTime);
			
                gameObject.transform.localRotation = Quaternion.Euler(0, 0, RotationZ);
                RotationZ += 80.0f / (bigTime / Time.deltaTime);
                yield return 0;
            }
            while(startTime+bigTime+RotTime>Time.time)
            {
                gameObject.transform.localRotation = Quaternion.Euler(0, 0, RotationZ);
                RotationZ += 40.0f / (bigTime / Time.deltaTime);
                yield return 0;
            }
            float r = gameObject.GetComponent<tk2dSprite>().color.r;
            float g = gameObject.GetComponent<tk2dSprite>().color.g;
            float b = gameObject.GetComponent<tk2dSprite>().color.b;

            tk2dSprite ts = gameObject.GetComponent<tk2dSprite>();
            while(startTime+big+RotTime+disTime>Time.time)
            {
			
                gameObject.transform.localRotation = Quaternion.Euler(0, 0, RotationZ);
                RotationZ += 40.0f / (bigTime / Time.deltaTime);

                ts.color = new Color(r, g, b, a);
                a -= 1/(disTime / Time.deltaTime);
                yield return 0;
            }
        }
        // Update is called once per frame
        void Update () 
        {
		
        }
    }
}
