using UnityEngine;

namespace Assets.Scripts.Game.bjlb
{
    public class CountDownDestory : MonoBehaviour
    {
        public float CountTime;
        protected void OnEnable()
        {
            Invoke("CountDown", CountTime);
        }

        private void CountDown()
        {
            DestroyObject(gameObject);
        }
    }
}
