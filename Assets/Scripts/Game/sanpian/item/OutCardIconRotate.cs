using UnityEngine;

namespace Assets.Scripts.Game.sanpian.item
{
    public class OutCardIconRotate : MonoBehaviour
    {
        private float waitTime = 0.5f;
        private float rotateTime = 1f;
        private float m_time = 0;
        // Update is called once per frame
        void Update ()
        {
            m_time += Time.deltaTime;
            if (m_time <= rotateTime)
            {
                transform.Rotate(new Vector3(0, 0,- Time.deltaTime / rotateTime*360));
            }

            if (m_time >= waitTime + rotateTime)
            {
                transform.rotation=new Quaternion(0,0,0,0);
                m_time = 0;
            }
        }
    }
}
