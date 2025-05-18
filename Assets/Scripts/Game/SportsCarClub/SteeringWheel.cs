using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class SteeringWheel : MonoBehaviour
    {
        public float maxRotateAngle = 50;
        public int direction = 1;

        #region 单例
        private static SteeringWheel instance;
        public static SteeringWheel GetInstance()
        {
            return instance;
        }
        #endregion

        void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void DoRotate(bool reset = false)
        {
            if (gameObject.GetComponent<TweenRotation>())
                Destroy(gameObject.GetComponent<TweenRotation>());

            var component = transform.gameObject.AddComponent<TweenRotation>();

            if (!reset)
            {
                direction = Random.Range(-1, 2);
                if (direction == 0) direction = 1;

                component.duration = 1.5f;
                component.to = new Vector3(0, 0, 0);
                component.to = new Vector3(0, 0, maxRotateAngle * direction);
            }
            else
            {
                component.duration = 1f;
                component.from = new Vector3(0, 0, maxRotateAngle * direction);
                component.to = new Vector3(0, 0, 0);
            }
        }
    }
}