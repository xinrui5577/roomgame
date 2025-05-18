using System;
using UnityEngine;

namespace Assets.Scripts.Game.FishGame.Utils
{
    public class RollingOver : MonoBehaviour
    {
        /// <summary>
        /// 开始
        /// </summary>
        public Vector3 Frome;
        /// <summary>
        /// 结束
        /// </summary>
        public Vector3 To;

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public void AddOnFinish(Vector3 rotation,Action onFinish)
        { 
        }
    }
}
