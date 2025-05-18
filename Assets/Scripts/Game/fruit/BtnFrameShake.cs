using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Assets.Scripts.Game.fruit
{
    public class BtnFrameShake : MonoBehaviour
    {
        private float currentTime;
        private float blinkUpdate;
        public float shakeSpace = .2f;

        // Use this for initialization
        void Start()
        {
            Invoke("destoryScripts", 1);
        }

        // Update is called once per frame
        void Update()
        {
            currentTime = Time.time;
            btnFrameShake();
        }

        void btnFrameShake()
        {
            if (currentTime - blinkUpdate > shakeSpace)
            {
                blinkUpdate = Time.time;
                if (gameObject.GetComponent<Image>() ?? gameObject.AddComponent<Image>())
                    gameObject.GetComponent<Image>().enabled = !gameObject.GetComponent<Image>().enabled;
            }
        }

        void destoryScripts()
        {
            GetComponent<Image>().enabled = false;
            Destroy(this);
        }
    }

}