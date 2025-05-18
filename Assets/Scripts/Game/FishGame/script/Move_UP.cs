using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.script
{
    public class Move_UP : MonoBehaviour {

        float startTime = 0;
        float delayTime = 0;

        float UPY = 0;

        public float fupTime = 0;
        public float fdownTime = 0;
        public float fupS = 0;
        public float fdownS = 0;

        private int dir;

        private static Move_UP move;

        public static Move_UP Singlton
        {
            get
            {
                if (move == null)
                    move = GameObject.FindObjectOfType(typeof(Move_UP)) as Move_UP;
                return move;
            }
        }
        // Use this for initialization
        void Start ()
        {

        }
        public void Play()
        {
            startTime = Time.time + delayTime;
            dir = 1;
            StartCoroutine(UP(fupTime, fdownTime, fupS, fdownS));
        }

        public void rePlay()
        {
            startTime = Time.time + delayTime;
            StartCoroutine(DOWN(fdownTime, fupTime, fdownS, fupS));

        }
        IEnumerator DOWN(float upTime, float downTime, float upS, float downS)
        {
            float x = gameObject.transform.localPosition.x;
            float y = gameObject.transform.localPosition.y;
            float z = gameObject.transform.localPosition.z;
            while (upTime + startTime > Time.time)
            {
                gameObject.transform.localPosition = new Vector3(x, y - UPY, z);
                UPY -= upS / (upTime / Time.deltaTime);
                yield return 0;
            }
            gameObject.transform.localPosition = new Vector3(x, y - upS, z);


            while (upTime + downTime + startTime > Time.time)
            {
                gameObject.transform.localPosition = new Vector3(x, y - UPY, z);
                UPY += downS / (downTime / Time.deltaTime);
                yield return 0;
            }

            gameObject.transform.localPosition = new Vector3(x, y + upS - downS, z);
            UPY = 0;
        }
        // IEnumerable Down(float downTime,
        IEnumerator UP(float upTime,float downTime,float upS,float downS)
        {
            float x=gameObject.transform.localPosition.x;
            float y = gameObject.transform.localPosition.y;
            float z=gameObject.transform.localPosition.z;
            while (upTime + startTime > Time.time)
            {
                gameObject.transform.localPosition = new Vector3(x, y+UPY, z);
                UPY += upS / (upTime / Time.deltaTime);
                yield return 0;
            }
            gameObject.transform.localPosition = new Vector3(x, y+upS, z);
            while (upTime + downTime + startTime > Time.time)
            {
                gameObject.transform.localPosition = new Vector3(x, y + UPY, z);
                UPY -= downS / (downTime / Time.deltaTime);
                yield return 0;
            }

            gameObject.transform.localPosition = new Vector3(x, y + upS-downS, z);
            UPY = 0;
        }
        // Update is called once per frame
        void Update ()
        {
	    
        }
    }
}
