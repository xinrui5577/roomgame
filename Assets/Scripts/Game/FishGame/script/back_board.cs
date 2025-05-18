using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.script
{
    public class back_board : MonoBehaviour {
        float startTime;
        // Use this for initialization
        void Start ()
        {
            startTime = Time.time;
            StartCoroutine(move(0.5f,0.1f));
        }
        IEnumerator move(float Time1,float Time2)
        {
            float x=gameObject.GetComponent<tk2dSlicedSprite>().transform.position.x;
            float y=gameObject.GetComponent<tk2dSlicedSprite>().transform.position.y;
            float z=gameObject.GetComponent<tk2dSlicedSprite>().transform.position.z;
            while(startTime+Time1>Time.time)
            {
                gameObject.GetComponent<tk2dSlicedSprite>().transform.position = new Vector3(x,y,z);
                y-=215/(Time1/Time.deltaTime);
                yield return 0;
            }
		
            while(startTime+Time1+Time2>Time.time)
            {
                gameObject.GetComponent<tk2dSlicedSprite>().transform.position=new Vector3(x,y,z);
                y+=20/(Time2/Time.deltaTime);
                yield return 0;
            }
        }
        // Update is called once per frame
        void Update ()
        {
	
        }
    }
}
