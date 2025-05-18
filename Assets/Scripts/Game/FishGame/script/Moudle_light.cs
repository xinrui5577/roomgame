using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.FishGame.script
{
    public class Moudle_light : MonoBehaviour
    {
        float ScaleX = 0;
        float ScaleY = 0;

        float RotationZ = 0;

        float a = 1.0f;

        float startTime;

        // Use this for initialization
        void asd(params int[] a)
        {
    
        }
        void Start ()
        {
            startTime = Time.time;
            ScaleX = 0;
            ScaleY = 0;
            RotationZ = 0;
            a = 1.0f;
            StartCoroutine(move(0.5f,0.5f,0.5f));
        }

        //public void reset_move()
        //{
        //    startTime = Time.time;
        //    ScaleX = 0;
        //    ScaleY = 0;
        //    RotationZ = 0;
        //    a = 1.0f;
        //    StartCoroutine(move(0.5f, 0.5f, 0.5f));
        //}
        IEnumerator move(float ScaleTime, float RotationTime, float AlphaTime)
        {
            //放大时间
            while (startTime + ScaleTime > Time.time)
            {
                gameObject.transform.localScale = new Vector3(ScaleX, ScaleY, 0);
                ScaleX += 1.5f / (ScaleTime / Time.deltaTime);
                ScaleY += 1.5f / (ScaleTime / Time.deltaTime);
                //Time.deltaTime
                yield return 0;
            }
            //旋转时间
            while (startTime + ScaleTime + RotationTime > Time.time)
            {
                gameObject.transform.localRotation = Quaternion.Euler(0, 0, RotationZ);
                RotationZ -= 25.0f / (RotationTime / Time.deltaTime);
                yield return 0;
            }
            //淡化时间
            float r = gameObject.GetComponent<tk2dSprite>().color.r;
            float g = gameObject.GetComponent<tk2dSprite>().color.g;
            float b = gameObject.GetComponent<tk2dSprite>().color.b;
            while (startTime + ScaleTime +RotationTime+ AlphaTime > Time.time)
            {
                gameObject.GetComponent<tk2dSprite>().color = new Color(r, g, b, a);
                a -= 1/(AlphaTime / Time.deltaTime);
                yield return 0;
            }
        }
        // Update is called once per frame
        void Update ()
        {
        }
    }
}
