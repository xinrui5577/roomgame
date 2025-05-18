using UnityEngine;

namespace Assets.Scripts.Game.brnn
{
    public class CountDownDestory : MonoBehaviour
    {
        public float CountTime;
        public void OnEnable()
        {
            Invoke("CountDown", CountTime);
        }

        public void CountDown()
        {
            DestroyObject(gameObject);
        }
    }
}
