using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Game.SportsCarClub
{
    public class BetBtnRotate : MonoBehaviour
    {
        public float rotateSpeed = -200;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            AutoRotate();
        }

        private void AutoRotate()
        {
            transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
        }
    }
}