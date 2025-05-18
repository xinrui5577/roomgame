using UnityEngine;

namespace Assets.Scripts.Game.brnn
{
    public class SelectAnimation : MonoBehaviour
    {

        public GameObject SelectBig;
        protected void Update()
        {
            SelectBig.transform.Rotate(0, 0, -800 * Time.deltaTime);
        }
    }
}
