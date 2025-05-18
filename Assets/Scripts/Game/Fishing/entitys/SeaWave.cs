using System.Collections;
using com.yxixia.utile.Utiles;
using UnityEngine;
using YxFramwork.Framework.Core;
using YxFramwork.Manager;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 海浪
    /// </summary>
    public class SeaWave : MonoBehaviour
    {
        /// <summary>
        /// 大小
        /// </summary>
        public float Size = 512;

        public string SoundName;

        private Transform _transform;

        void Awake()
        {
            _transform = transform;
        }

        void Start()
        {
            Facade.Instance<MusicManager>().Play(SoundName);
        }

        public Vector3 Speed;

        public void BeginStart(float distance, float totalTime)
        {
            distance += Size;
            Speed = Vector3.left * (distance / totalTime);
            this.SetActive(true);
        }

        void Update()
        {
            _transform.localPosition += Time.deltaTime * Speed;
        }

        public IEnumerator Move(float distance,float totalTime)
        {
            distance += Size;
            var speed = Vector3.left * (distance / totalTime);
            var pos = transform.localPosition;
            var targetPos = pos;
            targetPos.x -= distance;
            while (pos.x > targetPos.x)
            {
                pos = pos + Time.deltaTime * speed;
                transform.localPosition = pos;
                yield return null;
            }
        }
    }
}
