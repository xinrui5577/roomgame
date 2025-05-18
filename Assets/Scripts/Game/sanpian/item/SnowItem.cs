using UnityEngine;

namespace Assets.Scripts.Game.sanpian.item
{
    public class SnowItem : MonoBehaviour
    {
        private float xSpeed;
        private float ySpeed;
        private float RotateSpeed;
        private float timer;
        private Transform m_transform;
        private UISprite sprite;
        // Use this for initialization
        void Start ()
        {
            xSpeed = Random.Range(-0.01f, 0.01f);
            ySpeed = Random.Range(-0.01f,-0.03f);
            RotateSpeed = Random.Range(-5,5);
            timer = 0;
            m_transform = this.transform;
            sprite = m_transform.GetComponent<UISprite>();
            sprite.spriteName = Random.Range(0, 3) + "";
        }
	
        // Update is called once per frame
        void FixedUpdate ()
        {
            timer += Time.deltaTime;
            m_transform.Translate(new Vector3(xSpeed, ySpeed, 0),Space.World);
            m_transform.Rotate(new Vector3(0, 0, RotateSpeed));
            if (timer>=3)
            {
                Destroy(gameObject);
            }
        }
    }
}
