using System;
using Assets.Scripts.Game.Fishing.datas;
using UnityEngine;

namespace Assets.Scripts.Game.Fishing.entitys
{
    /// <summary>
    /// 子弹
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        public BulletData Data;
        /// <summary>
        /// 速度
        /// </summary>
        public float Speed = 800F;
        /// <summary>
        /// 预制体
        /// </summary>
        public Web WebPrefab;

        public Action<Bullet> CheckOutSide; 

        private Transform _transform;

        void Start()
        {
            _transform = transform;
        }

        void Update()
        {
            var target = Data.TargetFish;

            if (target != null && target.Availability)
            {
                var targetTs = target.TheSwimmer.CurTransform;
                var relative = _transform.InverseTransformPoint(targetTs.position);
                var angle = Mathf.Atan2(relative.y, relative.x) * Mathf.Rad2Deg;
                _transform.Rotate(0, 0, angle - 90);
            }
            var pos = _transform.localPosition;
            pos.z = 0;
            pos += Speed * Time.deltaTime * _transform.up;
            _transform.localPosition = pos;
            //判断是否到边界
            if (CheckOutSide!=null) CheckOutSide(this);
            
        }

        void OnTriggerEnter(Collider other)
        {
            var fish = other.GetComponentInParent<Fish>();
            
            if (fish == null || !fish.TheSwimmer.enabled) return;
            var target = Data.TargetFish;
            if (target != null && target != fish && target.TheSwimmer.enabled)
            {
                return;
            }
            fish.BeHit();
            //生成渔网
            CreateWeb(_transform.position);
            if (!Data.IsPenetrate)
            {
                Destroy(gameObject);
            }
            var hitData = new HitData
            {
                BeatenFishData = fish.Data,
                TheBulletData = Data
            };
            var player = Data.Player;
            if (player != null)
            {
                player.KillFish(hitData);
            }
        }

        private void CreateWeb(Vector3 pos)
        {
            if (WebPrefab == null) return;
            var webGo = Instantiate(WebPrefab);
            var webTs = webGo.transform;
            webTs.parent = _transform.parent;
            webTs.position = pos;
            webTs.localRotation = Quaternion.identity;
            webTs.localScale = Vector3.one;
        }

    }
}
