using UnityEngine;

namespace Assets.Scripts.Game.lswc.Starry
{
    /// <inheritdoc />
    /// <summary>
    /// 星空旋转控制
    /// </summary>
    public class LSStarryControl : MonoBehaviour
    {
        public float Speed=2.0f;
        public float Rate = 0.02f;
        private Vector3 rotateEuler;
        private void Start()
        {
            rotateEuler = new Vector3(0,Speed*Rate,0);
            InvokeRepeating("Rotate",0,Rate);
        }

        private void Rotate()
        {
            transform.localEulerAngles += rotateEuler;
        }
    }
}
